using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator
{
    partial class NClassifyConfig
    {
        public readonly string FilePath;
        private readonly Dictionary<string, BaseType> _typeMap;
        private readonly List<BaseType> _defineTypes;
        private readonly NamespaceType _root;

        public NClassifyConfig()
        {
            _typeMap = new Dictionary<string, BaseType>(StringComparer.Ordinal);
            _defineTypes = new List<BaseType>();
        }

        private NClassifyConfig(string inputFile, string defaultNamespace)
            : this()
        {
            if (!Path.IsPathRooted(inputFile))
                inputFile = Path.Combine(Environment.CurrentDirectory, inputFile);

            try
            {
                NClassifyConfig config = new XmlValidatingReader<NClassifyConfig>(typeof(NClassifyConfig).FullName)
                        .ReadXml(inputFile);

                FilePath = Path.GetFullPath(inputFile);
                _settings = config.Settings;
                Items = config.Items ?? new RootItem[0];
            }
            catch (XmlException xe)
            { throw new ApplicationException(xe.Message, xe); }

            foreach (RootItem item in Items)
                item.ParentConfig = this;

            Settings.Namespace = Settings.Namespace ?? defaultNamespace;
            _root = new NamespaceType(Settings.Namespace);
            SetTypeHeirarchy(Items.OfType<BaseType>(), _root);
        }

        public static NClassifyConfig Read(string inputFile, string defaultNamespace)
        {
            NClassifyConfig config = new NClassifyConfig(inputFile, defaultNamespace);
            config.LoadImports();
            config.Validate();
            return config;
        }

        public IEnumerable<BaseType> AllTypes { get { return _defineTypes; } }

        public NamespaceType RootNamespace { get { return _root; } }
        public IEnumerable<BaseType> RootTypes { get { return _defineTypes.Where(t => t.ParentType == _root); } }

        public IEnumerable<EnumType> GetEnumerations(BaseType parent) { return _defineTypes.OfType<EnumType>().Where(t => t.ParentType == parent); }
        public IEnumerable<SimpleType> GetSimpleTypes(BaseType parent) { return _defineTypes.OfType<SimpleType>().Where(t => t.ParentType == parent); }
        public IEnumerable<ComplexType> GetComplexTypes(BaseType parent) { return _defineTypes.OfType<ComplexType>().Where(t => t.ParentType == parent); }
        public IEnumerable<ServiceInfo> GetServices(BaseType parent) { return _defineTypes.OfType<ServiceInfo>().Where(t => t.ParentType == parent); }

        public FieldAccess DefaultAccess { get { return FieldAccess.Public; } }

        private void SetTypeHeirarchy(IEnumerable<BaseType> types, BaseType parent)
        {
            foreach(BaseType t in types)
            {
                t.ParentType = parent;
                if (t.Fields != null)
                {
                    foreach (FieldInfo fld in t.Fields)
                        fld.DeclaringType = t;
                }

                if (t.ChildTypes != null)
                    SetTypeHeirarchy(t.ChildTypes, t);
            }
        }

        private void LoadImports()
        {
            List<RootItem> items = new List<RootItem>(Items);
            Dictionary<string, string> imported = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int ix = 0; ix < items.Count; ix++)
            {
                ImportFile import = items[ix] as ImportFile;
                if (import == null || imported.ContainsKey(import.FullName))
                    continue;

                NClassifyConfig included = Read(import.FullName, Settings.Namespace);
                items.AddRange(included.Items.Select(i => i.Importing()));
                imported[included.FilePath] = null;
            }

            Items = items.ToArray();
        }

        private void Validate()
        {
            AddTypes(Items.OfType<BaseType>());
            _defineTypes.AddRange(_typeMap.Values.Where(t => !t.IsImported).Distinct());

            foreach (EnumType type in _defineTypes.OfType<EnumType>())
            {
                if (type.Values == null || type.Values.Length == 0)
                    throw new ApplicationException("Enumeration " + type.QualifiedName +
                                                   " must define at least one value");
            }

            foreach (ComplexType type in _defineTypes.OfType<ComplexType>())
            {
                foreach (FieldInfo field in type.Fields)
                {
                    field.PropertyName = field.PropertyName ?? CodeWriter.ToPascalCase(field.Name);
                    if (CodeWriter.ToPascalCase(field.PropertyName ?? field.Name) == type.PascalName)
                        throw new ApplicationException("The field " + field.Name +
                                                       " can not be the same name as the enclosing type " +
                                                       type.QualifiedName);

                    if (field is EnumTypeRef)
                        Validate(type, (EnumTypeRef) field);
                    else if (field is SimpleTypeRef)
                        Validate(type, (SimpleTypeRef)field);
                    else if (field is ComplexTypeRef)
                        Validate(type, (ComplexTypeRef)field);
                    else if (field is Primitive)
                        Validate(type, (Primitive)field);
                }
            }
            foreach (ServiceInfo svc in _defineTypes.OfType<ServiceInfo>())
                foreach (ServiceMethod method in svc.Methods)
                    Validate(svc, method);
        }

        private BaseType ResolveName(BaseType type, string name)
        {
            BaseType value;
            BaseType scope = type;
            while(scope != null)
            {
                if (_typeMap.TryGetValue(CodeWriter.CombineNames(".", scope.QualifiedName, name), out value))
                    return value;

                scope = scope.ParentType;
            }

            if (_typeMap.TryGetValue(name, out value))
                return value;

            throw new ApplicationException("Unable to resolve type '" + name + "' for type " + type.QualifiedName);
        }

        public T ResolveName<T>(BaseType type, string name) where T : BaseType
        {
            BaseType found = ResolveName(type, name);
            if (found is T)
                return (T)found;

            throw new ApplicationException(
                String.Format("The field {0}.{1} references incorrect type {2}.",
                              type.QualifiedName, name, found.GetType()));
        }

        private void Validate(ComplexType type, Primitive tref)
        { }

        private void Validate(ComplexType type, EnumTypeRef tref)
        {
            ResolveName<EnumType>(type, tref.TypeName);
        }

        private void Validate(ComplexType type, SimpleTypeRef tref)
        {
            ResolveName<SimpleType>(type, tref.TypeName);
        }

        private void Validate(ComplexType type, ComplexTypeRef tref)
        {
            ResolveName<ComplexType>(type, tref.TypeName);
        }

        private void Validate(ServiceInfo type, ServiceMethod method)
        {
            if (!String.IsNullOrEmpty(method.Request))
                ResolveName<ComplexType>(type, method.Request);
            if (!String.IsNullOrEmpty(method.Response))
                ResolveName<ComplexType>(type, method.Response);
        }

        private void AddTypes(IEnumerable<BaseType> types)
        {
            foreach(BaseType type in types)
            {
                if (_typeMap.ContainsKey(type.QualifiedName))
                    throw new ApplicationException("The type '" + type.QualifiedName + "' has already been defined.");

                _typeMap.Add(type.QualifiedName, type);

                string lname = CodeWriter.CombineNames(".", type.Namespace, type.Name);
                if (lname != type.QualifiedName)
                {
                    if (_typeMap.ContainsKey(lname))
                        throw new ApplicationException("The type '" + lname + "' has already been defined.");
                    _typeMap.Add(lname, type);
                }

                if (type.ChildTypes != null)
                    AddTypes(type.ChildTypes);
            }
        }
    }

    partial class RootItem
    {
        internal NClassifyConfig ParentConfig;
        internal bool IsImported;

        internal RootItem Importing() { IsImported = true; return this; }
    }

    partial class ImportFile
    {
        internal string FullName
        {
            get { return Path.GetFullPath(Path.Combine(ParentConfig.FilePath, File)); }
        }
    }

    [System.Diagnostics.DebuggerDisplay("{GetType().Name,nq}({QualifiedName,nq})")]
    partial class BaseType
    {
        internal BaseType ParentType;

        internal virtual string Namespace { get { return ParentType.QualifiedName; } }
        internal virtual string QualifiedName { get { return CodeWriter.CombineNames(".", Namespace, PascalName); } }
        internal virtual string PascalName { get { return CodeWriter.ToPascalCase(Name); } }
    }

    public abstract partial class FieldInfo
    {
        internal BaseType DeclaringType;
    }

    public sealed class NamespaceType : BaseType
    {
        private readonly string _namespace;
        public NamespaceType(string name)
        {
            ParentType = null;
            string[] names = (name ?? String.Empty).Trim('.').Split('.');
            Name = names[names.Length - 1];
            names[names.Length - 1] = String.Empty;
            _namespace = CodeWriter.CombineNames(".", names);
        }

        internal override string Namespace { get { return _namespace; } }
        internal override string QualifiedName { get { return CodeWriter.CombineNames(".", Namespace, Name); } }
        internal override string PascalName { get { return Name; } }
    }
}
