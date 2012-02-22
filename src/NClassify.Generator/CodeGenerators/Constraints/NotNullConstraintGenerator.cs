using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class NotNullConstraintGenerator : BaseConstraintGenerator
    {
        public override void WriteChecks(CsCodeWriter code)
        {
            code.WriteLine("if (null == value) return false;");
        }
    }
}
