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
    class MatchConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly MatchConstraint _rule;

        public MatchConstraintGenerator(BaseFieldGenerator field, MatchConstraint rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            code.WriteLine("private static readonly " +
                CsCodeWriter.Global + "System.Text.RegularExpressions.Regex __valid_{0} = " + "new " +
                CsCodeWriter.Global + "System.Text.RegularExpressions.Regex({1}, {2});",
                _field.CamelName, code.MakeConstant(FieldType.String, _rule.Pattern), 
                    (_rule.Multiline 
                    ? CsCodeWriter.Global + "System.Text.RegularExpressions.RegexOptions.Multiline"
                    : CsCodeWriter.Global + "System.Text.RegularExpressions.RegexOptions.Singleline")
                    +
                    (_rule.IgnoreCase 
                    ? " | " + CsCodeWriter.Global + "System.Text.RegularExpressions.RegexOptions.IgnoreCase"
                    : "")
                );
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            using (code.WriteBlock("if (!__valid_{0}.IsMatch(value.ToString()))", _field.CamelName))
            {
                code.WriteLine(
                    "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                    "{0}NClassify.Library.ResourceMessages.MustMatchFormat, TypeFields.{2}, {1}));",
                    CsCodeWriter.Global, code.MakeConstant(FieldType.String, _rule.Pattern), _field.PropertyName);
                code.WriteLine("return false;");
            }
        }
    }
}
