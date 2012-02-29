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
                        "{0}NClassify.Library.Resources.MustBeLongerThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, _rule.MinLength, _field.PascalName);
                    code.WriteLine("return false;");
                }
            }
            if (_rule.MaxLength < uint.MaxValue)
            {
                using (code.WriteBlock("if (value{0} > {1})", accessor, _rule.MaxLength))
                {
                    code.WriteLine(
                        "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                        "{0}NClassify.Library.Resources.MustBeShorterThan, TypeFields.{2}, {1}));",
                        CsCodeWriter.Global, _rule.MaxLength, _field.PascalName);
                    code.WriteLine("return false;");
                }
            }
        }
    }
}
