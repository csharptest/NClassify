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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeGenerators.Types;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class EnumFieldGenerator : BaseFieldGenerator
    {
        private EnumType _enum;
        private EnumTypeGenerator _generator;

        public EnumFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _enum = fld.DeclaringType.ParentConfig.ResolveName<EnumType>(
                     fld.DeclaringType, ((EnumTypeRef)fld).TypeName
                     );
            _generator = new EnumTypeGenerator(_enum);
        }

        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + _enum.QualifiedName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Format("default({0})", GetPublicType(code));

            uint tmp;
            if(uint.TryParse(value, out tmp))
            {
                string name = _enum.Values.Where(e => e.Value == tmp).Select(e => e.Name).FirstOrDefault();
                if (!String.IsNullOrEmpty(name))
                    value = name;
            }

            value = CodeWriter.ToPascalCase(value);
            EnumType.Item item = _enum.Values.FirstOrDefault(e => CodeWriter.ToPascalCase(e.Name) == value);

            if (item == null)
                throw new ApplicationException(
                    String.Format("Unknown default value '{0}' for enumeration {1}", value,
                                  _enum.QualifiedName));

            return String.Format("{0}.{1}", GetPublicType(code), CodeWriter.ToPascalCase(item.Name));
        }

        public override string ToXmlString(CsCodeWriter code, string name)
        {
            if (XmlOptions.Format != null)
                return base.ToXmlString(code, name);
            return String.Format("{0}.ToString()", name);
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("({1}){0}System.Enum.Parse(typeof({1}), {2}, false)", CsCodeWriter.Global, GetStorageType(code), name);
        }
    }
}
