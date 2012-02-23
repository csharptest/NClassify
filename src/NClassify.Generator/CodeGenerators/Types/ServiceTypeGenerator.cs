using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class ServiceTypeGenerator : BaseTypeGenerator<ServiceInfo>
    {
        public ServiceTypeGenerator(ServiceInfo type)
            : base(type)
        { }

        public override void DeclareType(CsCodeWriter code)
        { }
    }
}
