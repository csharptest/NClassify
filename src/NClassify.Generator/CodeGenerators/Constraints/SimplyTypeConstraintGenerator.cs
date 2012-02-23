using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class SimplyTypeConstraintGenerator : BaseConstraintGenerator
    {
        private readonly SimpleType _type;

        public SimplyTypeConstraintGenerator(SimpleType type)
        {
            _type = type;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            code.WriteLine("if (!{0}.IsValidValue(value)) return false;", _type.QualifiedName);
        }
    }
}
