using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators
{
    class CsCodeGenerator : BaseGenerator<CsCodeWriter>
    {
        public CsCodeGenerator(NClassifyConfig config) : base(config)
        {
        }

        protected override CsCodeWriter OpenWriter(System.IO.TextWriter writer)
        {
            CsCodeWriter code = new CsCodeWriter(writer);
            code.WriteFilePreamble();
            return code;
        }
    }
}
