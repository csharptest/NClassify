#region Copyright (c) 2012 Roger O Knapp
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
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;
using System.Text.RegularExpressions;
using System.IO;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class CodeConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly CodedConstraint _rule;

        public CodeConstraintGenerator(BaseFieldGenerator field, CodedConstraint rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            if (code.Language != _rule.Language)
                return;

            Match m;
            if (!String.IsNullOrEmpty(_rule.MethodBody) &&
                (m = new Regex(@"^(?<Name>[a-zA-Z][\w_]*)\(value\)$").Match(_rule.Code)).Success)
            {
                using (code.WriteBlock("private static bool {0}({1} value)", m.Groups["Name"].Value, _field.GetStorageType(code)))
                using (StringReader rdr = new StringReader(_rule.MethodBody))
                {
                    string line;
                    while(null != (line = rdr.ReadLine()))
                    {
                        line = line.Trim();
                        if (line.Length == 0)
                            continue;
                        if (line.StartsWith("}"))
                            code.Indent--;
                        code.WriteLine(line);
                        if (line.EndsWith("{"))
                            code.Indent--;
                    }
                }
            }
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            if (code.Language != _rule.Language)
                return;
            
            using (code.WriteBlock("if (!({0}))", _rule.Code))
            {
                code.WriteLine("if (onError != null) " +
                    "onError(new {0}NClassify.Library.ValidationError(TypeFields.{1}, {0}NClassify.Library.ResourceMessages.InvalidField, TypeFields.{1}));",
                    CsCodeWriter.Global, _field.PropertyName);
                code.WriteLine("return false;");
            }
        }
    }
}
