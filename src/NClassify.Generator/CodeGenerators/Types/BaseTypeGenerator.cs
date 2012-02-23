using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    internal abstract class BaseTypeGenerator<T> : BaseTypeGenerator
        where T : BaseType
    {
        protected new T Type { get { return (T)base.Type; } }
        protected BaseTypeGenerator(T type) : base(type)
        { }
    }

    abstract class BaseTypeGenerator : IMemberGenerator<CsCodeWriter>
    {
        protected readonly BaseType Type;
        protected readonly IList<BaseFieldGenerator> Fields;

        protected BaseTypeGenerator(BaseType type)
        {
            Type = type;
            Fields = new ReadOnlyCollection<BaseFieldGenerator>(
                Type.Fields.SafeEnum()
                .Select(f => FieldFactory.Create(f))
                .ToArray());

            Access = Type.ParentConfig.DefaultAccess;
        }

        public FieldAccess Access { get; set; }

        public string Name { get { return Type.Name; } }
        public string CamelName { get { return CodeWriter.ToCamelCase(Type.Name); } }
        public string PascalName { get { return CodeWriter.ToPascalCase(Type.Name); } }

        protected void WriteChildren<TWriter>(TWriter code, BaseType[] children) where TWriter : CodeWriter
        {
            if (children != null && children.Length > 0)
            {
                List<BaseTypeGenerator> types = new List<BaseTypeGenerator>();
                children.OfType<EnumType>().ForAll(x => types.Add(new EnumTypeGenerator(x)));
                children.OfType<SimpleType>().ForAll(x => types.Add(new SimpleTypeGenerator(x)));
                children.OfType<ComplexType>().ForAll(x => types.Add(new ComplexTypeGenerator(x)));

                //All nested types will be public
                types.ForAll(x => { x.Access = FieldAccess.Public; });

                types.OfType<IMemberGenerator<TWriter>>()
                    .ForAll(x => x.DeclareTypes(code));
            }
        }

        #region IMemberGenerator<CsWriter> Members

        public abstract void DeclareType(CsCodeWriter code);

        void IMemberGenerator<CsCodeWriter>.DeclareTypes(CsCodeWriter code)
        {
            using (code.CodeRegion("{0}", Name))
                DeclareType(code);
        }

        void IMemberGenerator<CsCodeWriter>.DeclareStaticData(CsCodeWriter code)
        { }

        void IMemberGenerator<CsCodeWriter>.WriteMember(CsCodeWriter code)
        { }

        #endregion
    }
}
