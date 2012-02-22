using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    abstract class BaseTypeGenerator<T> : IMemberGenerator<CsCodeWriter>
        where T : BaseType
    {
        protected readonly T Type;
        protected readonly IList<BaseFieldGenerator> Fields;

        protected BaseTypeGenerator(T type)
        {
            Type = type;
            Fields = new ReadOnlyCollection<BaseFieldGenerator>(
                Type.Fields.SafeEnum()
                .Select(f => FieldFactory.Create(f))
                .ToArray());
        }

        public virtual FieldAccess Access { get { return Type.ParentConfig.DefaultAccess; } }

        public string Name { get { return Type.Name; } }
        public string CamelName { get { return CodeWriter.ToCamelCase(Type.Name); } }
        public string PascalName { get { return CodeWriter.ToPascalCase(Type.Name); } }

        #region IMemberGenerator<CsWriter> Members

        public virtual void DeclareStaticData(CsCodeWriter code)
        {}

        public virtual void DeclareInstanceData(CsCodeWriter code)
        {}

        public abstract void WriteMember(CsCodeWriter code);

        #endregion
    }
}
