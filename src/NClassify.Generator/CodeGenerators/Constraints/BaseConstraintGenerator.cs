using System;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    abstract class BaseConstraintGenerator : IMemberGenerator<CsCodeWriter>
    {
        #region IMemberGenerator<CsWriter> Members

        public virtual void DeclareTypes(CsCodeWriter code) { }
        public virtual void DeclareStaticData(CsCodeWriter code) { }
        public virtual void WriteMember(CsCodeWriter code) { }

        public abstract void WriteChecks(CsCodeWriter code);

        #endregion
    }
}
