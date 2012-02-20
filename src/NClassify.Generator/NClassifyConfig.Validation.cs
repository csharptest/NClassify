using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace NClassify.Generator
{
    partial class NClassifyConfig
    {
        public readonly string FilePath;

        public NClassifyConfig() { }
        private NClassifyConfig(string inputFile, string defaultNamespace)
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
            SetTypeHeirarchy(Items.OfType<BaseType>(), new NamespaceType(Settings.Namespace));
        }

        public static NClassifyConfig Read(string inputFile, string defaultNamespace)
        {
            NClassifyConfig config = new NClassifyConfig(inputFile, defaultNamespace);
            config.LoadImports();
            config.Validate();
            return config;
        }

        private void SetTypeHeirarchy(IEnumerable<BaseType> types, BaseType parent)
        {
            foreach(BaseType t in types)
            {
                t.ParentType = parent;
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
            throw new NotImplementedException();
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
        internal virtual string QualifiedName { get { return CsWriter.CombineNames(Namespace, PascalName); } }
        internal virtual string PascalName { get { return CsWriter.ToPascalCase(Name); } }
    }

    class NamespaceType : BaseType
    {
        private readonly string _namespace;
        public NamespaceType(string name)
        {
            string[] names = (name ?? String.Empty).Trim('.').Split('.');
            Name = names[names.Length - 1];
            names[names.Length - 1] = String.Empty;
            _namespace = CsWriter.CombineNames(names);
        }

        internal override string Namespace { get { return _namespace; } }
        internal override string QualifiedName { get { return CsWriter.CombineNames(Namespace, Name); } }
        internal override string PascalName { get { return Name; } }
    }
}
