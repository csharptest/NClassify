using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class UInt8FieldGenerator : BaseUnsignedFieldGenerator
    {
        public UInt8FieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("{0}System.Xml.XmlConvert.ToByte({1})", CsCodeWriter.Global, name);
        }
    }
}
