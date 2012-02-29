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
                    "{0}NClassify.Library.Resources.MustMatchFormat, TypeFields.{2}, {1}));",
                    CsCodeWriter.Global, code.MakeConstant(FieldType.String, _rule.Pattern), _field.PascalName);
                code.WriteLine("return false;");
            }
        }
    }
}
