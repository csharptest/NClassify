using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class IsValidConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;

        public IsValidConstraintGenerator(BaseFieldGenerator field)
        {
            _field = field;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            using (code.WriteBlock("if (!value.IsValid())"))
            {
                code.WriteLine("if (onError == null)");
                code.WriteLineIndent("return false;");
                code.WriteLine(
                    "{0}System.Collections.Generic.List<{0}NClassify.Library.ValidationError> errors = " +
                    "new {0}System.Collections.Generic.List<{0}NClassify.Library.ValidationError>();",
                    CsCodeWriter.Global
                    );
                code.WriteLine("value.GetBrokenRules(errors.Add);");
                code.WriteLine("onError(new {0}NClassify.Library.ValidationError(TypeFields.{1}, errors));",
                    CsCodeWriter.Global, _field.PascalName);
                code.WriteLine("return false;");
            }
        }
    }
}
