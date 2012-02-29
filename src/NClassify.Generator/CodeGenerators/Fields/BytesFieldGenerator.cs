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

        public override string ToXmlString(CsCodeWriter code, string name)
        {
            if (XmlOptions.Format != null)
                return base.ToXmlString(code, name);
            return String.Format("{0}.ToBase64()", name);
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("{0}.FromBase64({1})", GetStorageType(code), name);
        }
    }
}
