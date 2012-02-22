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
                code.WriteLine("if (value.CompareTo({0}) < 0) return false;", code.MakeConstant(_field.FieldType, _rule.MinValue));
            if (_rule.MaxValue != null)
                code.WriteLine("if (value.CompareTo({0}) > 0) return false;", code.MakeConstant(_field.FieldType, _rule.MaxValue));
        }
    }
}
