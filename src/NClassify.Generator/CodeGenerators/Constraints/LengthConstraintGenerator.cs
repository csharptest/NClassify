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
            switch(_field.FieldType)
            {
                case FieldType.Bytes:
                case FieldType.String:
                    break;
                case FieldType.Uri:
                    accessor = ".AbsoluteUri.Length";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Length constraint does not apply to fields of type " + _field.FieldType);
            }

            if (_rule.MinLength > 0)
                code.WriteLine("if (value{0} < {1}) return false;", accessor, _rule.MinLength);
            if (_rule.MaxLength < uint.MaxValue)
                code.WriteLine("if (value{0} > {1}) return false;", accessor, _rule.MaxLength);
        }
    }
}
