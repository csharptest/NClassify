using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class BytesFieldGenerator : BaseFieldGenerator
    {
        public BytesFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + "NClassify.Library.ByteArray";
        }
    }
}
