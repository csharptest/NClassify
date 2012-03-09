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
    class LengthConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly LengthConstraint _rule;

        public LengthConstraintGenerator(BaseFieldGenerator field, LengthConstraint rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            string accessor = ".Length";
            switch (_field.FieldType)
            {
                case FieldType.Bytes:
                case FieldType.String:
                    break;
                    //case FieldType.Uri:
                    //    accessor = ".AbsoluteUri.Length";
                    //    break;
                default:
                    throw new ArgumentOutOfRangeException("Length constraint does not apply to fields of type " +
                                                          _field.FieldType);
            }

            if (_rule.MinLength > 0)
            {
                using (code.WriteBlock("if (value{0} < {1})", accessor, _rule.MinLength))
                {
                    code.WriteLine(
                        "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                        "{0}NClassify.Library.ResourceMessages.MustBeLongerThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, _rule.MinLength, _field.PropertyName);
                    code.WriteLine("return false;");
                }
            }
            if (_rule.MaxLength < uint.MaxValue)
            {
                using (code.WriteBlock("if (value{0} > {1})", accessor, _rule.MaxLength))
                {
                    code.WriteLine(
                        "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                        "{0}NClassify.Library.ResourceMessages.MustBeShorterThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, _rule.MaxLength, _field.PropertyName);
                    code.WriteLine("return false;");
                }
            }
        }
    }
}
