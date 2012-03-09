#region Copyright (c) 2010-2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CSharpTest.Net.Collections;

namespace NClassify.Generator.CodeWriters
{
    public abstract class CodeWriter : IndentedTextWriter
    {
        private static readonly Regex NonAlphaNumeric = new Regex(@"[^a-zA-Z0-9]+");
        private static readonly Regex LowerCaseAfterNonAlpha = new Regex(@"[^a-zA-Z][a-z]");
        private static readonly Regex MultipleUnderscores = new Regex(@"__+");
        private static readonly Regex Alpha = new Regex(@"[a-zA-Z]");

        private readonly DisposingList _open;
        private readonly string _beginBlock, _endBlock;

        protected CodeWriter(TextWriter writer, string beginBlock, string endBlock)
            : base(writer)
        {
            _beginBlock = beginBlock;
            _endBlock = endBlock;
            _open = new DisposingList();
        }

        protected override void Dispose(bool disposing)
        {
            _open.Dispose();
            //base.Dispose(disposing);
        }

        public override string ToString()
        {
            return InnerWriter.ToString();
        }

        #region Static Helpers

        public static string RemoveNonAlphaNumeric(string input)
        {
            string name = NonAlphaNumeric.Replace(input, "_");
            name = MultipleUnderscores.Replace(name, "_");
            name = LowerCaseAfterNonAlpha.Replace(name, x => x.Value.ToUpper().TrimStart('_'), int.MaxValue, 1);

            if (name.Length == 0 || Char.IsNumber(name[0]))
                throw new ArgumentException(String.Format("The field name '{0}' is invalid.", input));

            return name;
        }

        public static string ToPascalCase(string input)
        {
            string name = RemoveNonAlphaNumeric(input);

            Match firstAlpha = Alpha.Match(name);
            if (firstAlpha.Success && Char.IsLower(name[firstAlpha.Index]))
                name = name.Substring(0, firstAlpha.Index) + Char.ToUpper(name[firstAlpha.Index]) + name.Substring(1);

            return name;
        }

        public static string ToCamelCase(string input)
        {
            string name = RemoveNonAlphaNumeric(input);

            Match firstAlpha = Alpha.Match(name);
            if (firstAlpha.Success && Char.IsUpper(name[firstAlpha.Index]))
                name = name.Substring(0, firstAlpha.Index) + Char.ToLower(name[firstAlpha.Index]) + name.Substring(1);

            return name;
        }

        public static string CombineNames(string dividor, params string[] names)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in names)
            {
                string tmp = (name ?? String.Empty).Trim();
                if (tmp.Length == 0)
                    continue;
                if (sb.Length > 0)
                    sb.Append(dividor);
                sb.Append(tmp);
            }
            return sb.ToString();
        }

        public static string MakeCppString(string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            foreach (char ch in data ?? String.Empty)
            {
                if (ch >= 32 && ch < 128)
                {
                    if (ch == '\\' || ch == '\'' || ch == '"')
                        sb.Append('\\');
                    sb.Append(ch);
                    continue;
                }
                if (ch == '\r')
                {
                    sb.Append("\\r");
                    continue;
                }
                if (ch == '\n')
                {
                    sb.Append("\\n");
                    continue;
                }
                if (ch == '\t')
                {
                    sb.Append("\\t");
                    continue;
                }

                sb.Append('\\');
                sb.Append((char) ('0' + ((ch >> 6) & 3)));
                sb.Append((char) ('0' + ((ch >> 3) & 7)));
                sb.Append((char) ('0' + (ch & 7)));
            }
            sb.Append('"');
            return sb.ToString();
        }

        #endregion

        public abstract GeneratorLanguage Language { get; }

        public abstract string GetTypeName(FieldType type);

        public abstract string MakeString(string data);

        public abstract IDisposable WriteNamespace(string[] ns);

        public void WriteLineIndent(string format, params object[] args)
        {
            Indent++;
            if (args.Length > 0) 
                WriteLine(format, args);
            else
                WriteLine(format);
            Indent--;
        }

        public void WriteLineIf(bool cond, string format, params object[] args)
        {
            if (cond)
            {
                if (args.Length > 0) 
                    WriteLine(format, args);
                else
                    WriteLine(format);
            }
        }

        public virtual IDisposable CodeRegion(string format, params object[] args)
        { return new ClosingBlock(this, null); }

        public IDisposable WriteBlock(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
                format = String.Format(format, args);

            if (!String.IsNullOrEmpty(format))
            {
                string line;
                using (StringReader r = new StringReader(format))
                    while (null != (line = r.ReadLine()))
                        WriteLine(line);
            }

            return WriteBlock();
        }

        public IDisposable WriteBlock()
        {
            WriteStartBlock();
            return new ClosingBlock(this, x => WriteEndBlock());
        }

        public void WriteStartBlock()
        {
            base.WriteLine(_beginBlock);
            Indent++;
        }

        public void WriteEndBlock()
        {
            Indent--;
            base.WriteLine(_endBlock);
        }

        protected class ClosingBlock : IDisposable
        {
            private CodeWriter _wtr;
            private readonly Action<CodeWriter> _onClose;

            public ClosingBlock(CodeWriter wtr, Action<CodeWriter> onClose)
            {
                _wtr = wtr;
                _onClose = onClose;
                _wtr._open.Add(this);
            }

            void IDisposable.Dispose()
            {
                if (_wtr != null && _wtr._open.Remove(this))
                {
                    if(_onClose != null)
                        _onClose(_wtr);
                    _wtr = null;
                }
            }
        }

        public abstract string MakeConstant(FieldType type, string value);

        public abstract IDisposable DeclareEnum(CodeItem info);
        public abstract void WriteEnumValue(string name, uint value);

        public abstract IDisposable DeclareClass(CodeItem info, string[] implements);
        public abstract IDisposable DeclareStruct(CodeItem info, string[] implements);

        public abstract void DeclareField(CodeItem info, string type, string defaultValue);
        public virtual void DeclareField(CodeItem info, FieldType type, string defaultValue)
        {
            DeclareField(info, GetTypeName(type), defaultValue == null ? null : MakeConstant(type, defaultValue));
        }

        public abstract IDisposable DeclareProperty(CodeItem info, string type);
        public virtual IDisposable DeclareProperty(CodeItem info, FieldType type)
        {
            return DeclareProperty(info, GetTypeName(type));
        }

        public abstract IDisposable DeclareInterface(CodeItem info, string[] implements);
    }
}
