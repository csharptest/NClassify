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
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Constraints;
using NClassify.Generator.CodeGenerators.Types;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class SimpleFieldGenerator : BaseFieldGenerator
    {
        private SimpleType _primitive;
        private SimpleTypeGenerator _generator;

        public SimpleFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _primitive = fld.DeclaringType.ParentConfig.ResolveName<SimpleType>(
                fld.DeclaringType, ((SimpleTypeRef) fld).TypeName
                );

            _generator = new SimpleTypeGenerator(_primitive);
        }

        public override FieldType FieldType
        {
            get
            {
                return _primitive.Type;
            }
        }

        protected override IList<BaseConstraintGenerator> CreateConstraints()
        {
            List<BaseConstraintGenerator> constraints = new List<BaseConstraintGenerator>(
                    base.CreateConstraints()
                );

            constraints.Add(new IsValidConstraintGenerator(this));
            return constraints.AsReadOnly();
        }

        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + _primitive.QualifiedName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            return String.Format("new {0}({1})", GetPublicType(code), code.MakeConstant(FieldType, value));
        }

        public override string ToXmlString(CsCodeWriter code, string name)
        {
            return _generator.ValueField.ToXmlString(code, name + ".Value");
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("new {0}({1})",
                GetPublicType(code),
                _generator.ValueField.FromXmlString(code, name)
                );
        }
    }
}
