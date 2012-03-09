using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class RangeConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly RangeConstraint _rule;

        public RangeConstraintGenerator(BaseFieldGenerator field, RangeConstraint rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            if (_rule.MinValue != null)
            {
                using (code.WriteBlock("if (value.CompareTo({0}) < 0)", code.MakeConstant(_field.FieldType, _rule.MinValue)))
                {
                    code.WriteLine(
                        "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                        "{0}NClassify.Library.ResourceMessages.MustBeGreaterThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, code.MakeConstant(_field.FieldType, _rule.MinValue), _field.PropertyName);
                    code.WriteLine("return false;");
                }
            }
            if (_rule.MaxValue != null)
            {
                using (code.WriteBlock("if (value.CompareTo({0}) < 0)", code.MakeConstant(_field.FieldType, _rule.MaxValue)))
                {
                    code.WriteLine(
                        "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                        "{0}NClassify.Library.ResourceMessages.MustBeLessThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, code.MakeConstant(_field.FieldType, _rule.MaxValue), _field.PropertyName);
                    code.WriteLine("return false;");
                }
            }
        }
    }
}
