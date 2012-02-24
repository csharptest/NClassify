using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class IsValidConstraintGenerator : BaseConstraintGenerator
    {
        public override void WriteChecks(CsCodeWriter code)
        {
            code.WriteLine("if (!value.IsValid()) return false;");
        }
    }
}
