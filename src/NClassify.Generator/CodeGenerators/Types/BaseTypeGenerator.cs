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
        public readonly BaseType Type;
        protected readonly IList<BaseFieldGenerator> Fields;

        protected BaseTypeGenerator(BaseType type)
        {
            Type = type;
            Fields = new ReadOnlyCollection<BaseFieldGenerator>(
                Type.Fields.SafeEnum()
                .Select(f => FieldFactory.Create(this, f))
                .ToArray());

            Access = Type.Access;
            if (Access == FieldAccess.Default) Access = Type.ParentConfig.DefaultAccess;
            if (Access == FieldAccess.Default) Access = FieldAccess.Public;
            if (Access == FieldAccess.Protected && Type.ParentType is NamespaceType)
                Access = FieldAccess.Private;
        }

        public FieldAccess Access { get; set; }

        public string Name { get { return Type.Name; } }
        public virtual string XmlName { get { return Type.Name; } }
        public string CamelName { get { return CodeWriter.ToCamelCase(Type.Name); } }
        public string PascalName { get { return CodeWriter.ToPascalCase(Type.Name); } }

        public virtual bool IsSubclass { get { return false; } }
        public virtual string VirtualApi { get { return "public"; } }
        public bool CallBase(CsCodeWriter code, string method, params object[] args)
        {
            if (IsSubclass)
                code.WriteLine(method, args);
            return IsSubclass;
        }

        protected virtual void WriteChildren<TWriter>(TWriter code, BaseType[] children) where TWriter : CodeWriter
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

        protected virtual void WriteMembers(CsCodeWriter code, ICollection<BaseFieldGenerator> fields)
        {
            if (!IsSubclass)
            {
                code.WriteLine("public bool IsValid() { return 0 == GetBrokenRules(null); }");
                code.WriteLine("public void AssertValid() { GetBrokenRules(RaiseValidationError); }");
                code.WriteLine("void RaiseValidationError(global::NClassify.Library.ValidationError e) { e.RaiseException(); }");
            }
            using (code.WriteBlock(VirtualApi + " int GetBrokenRules(global::System.Action<{0}NClassify.Library.ValidationError> onError)", CsCodeWriter.Global))
            {
                code.WriteLine("int errorCount = 0;");
                foreach (var fld in fields)
                    fld.WriteValidation(code);
                CallBase(code, "errorCount += base.GetBrokenRules(onError);");
                code.WriteLine("return errorCount;");
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
