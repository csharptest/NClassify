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

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class ListConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly PredefinedValue _rule;

        public ListConstraintGenerator(BaseFieldGenerator field, PredefinedValue rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            code.Write("private static readonly {0}[] __in_{1} = new {0}[] {{", _field.GetStorageType(code), _field.CamelName);
            int ix = 0;
            string[] list = (string[]) _rule.Values.Clone();
            Array.Sort(list);

            foreach (string possible in _rule.Values)
            {
                if (ix++ > 0) code.Write(", ");
                code.Write(code.MakeConstant(_field.FieldType, possible));
            }
            code.WriteLine("};");
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            if (_field.FieldType != FieldType.String)
                throw new ApplicationException("The possible values constraints only applies to fields of type String.");

            using (code.WriteBlock("if (global::System.Array.BinarySearch(__in_{0}, value) < 0)", _field.CamelName))
            {
                code.WriteLine(
                    "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                    "{0}NClassify.Library.ResourceMessages.MustBeOneOf, TypeFields.{2}, string.Join(\", \", __in_{1})));",
                    CsCodeWriter.Global, _field.CamelName, _field.PropertyName);
                code.WriteLine("return false;");
            }
        }
    }
}
