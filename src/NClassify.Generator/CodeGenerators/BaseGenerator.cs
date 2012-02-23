using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Types;

namespace NClassify.Generator.CodeGenerators
{
    public abstract class BaseGenerator<TWriter> where TWriter : CodeWriter
    {
        private readonly NClassifyConfig _config;

        protected BaseGenerator(NClassifyConfig config)
        {
            _config = config;
        }

        public NClassifyConfig Config { get { return _config; } }

        public virtual void GenerateCode(TextWriter writer)
        {
            using (TWriter code = OpenWriter(writer))
            {
                using (code.WriteNamespace(Config.RootNamespace.QualifiedName.Split('.', ':')))
                {
                    GenerateChildren(code, Config.RootNamespace);
                }
                CloseWriter(code);
            }
        }

        protected virtual void GenerateChildren(TWriter code, BaseType parent)
        {
            List<BaseTypeGenerator> types = new List<BaseTypeGenerator>();
            Config.GetEnumerations(parent).ForAll(x => types.Add(new EnumTypeGenerator(x)));
            Config.GetSimpleTypes(parent).ForAll(x => types.Add(new SimpleTypeGenerator(x)));
            Config.GetComplexTypes(parent).ForAll(x => types.Add(new ComplexTypeGenerator(x)));
            Config.GetServices(parent).ForAll(x => types.Add(new ServiceTypeGenerator(x)));

            types.OfType<IMemberGenerator<TWriter>>()
                .ForAll(x => x.DeclareStaticData(code));

            types.OfType<IMemberGenerator<TWriter>>()
                .ForAll(x => x.DeclareTypes(code));
        }

        protected abstract TWriter OpenWriter(TextWriter writer);
        protected virtual void CloseWriter(TWriter cw) { cw.Dispose(); }
    }
}
