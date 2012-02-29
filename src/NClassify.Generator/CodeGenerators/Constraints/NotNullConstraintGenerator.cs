using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class NotNullConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;

        public NotNullConstraintGenerator(BaseFieldGenerator field)
        {
            _field = field;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            using (code.WriteBlock("if (object.ReferenceEquals(null, value))"))
            {
                code.WriteLine(
                    "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{1}, " +
                    "{0}NClassify.Library.Resources.MustNotBeNull, TypeFields.{1}));",
                    CsCodeWriter.Global, _field.PascalName);
                code.WriteLine("return false;");
            }
        }
    }
}
