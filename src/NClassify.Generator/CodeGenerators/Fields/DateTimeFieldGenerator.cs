using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class DateTimeFieldGenerator : BaseFieldGenerator
    {
        public DateTimeFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }
        public override string ToXmlString(CsCodeWriter code, string name)
        {
            if (XmlOptions.Format != null)
                return base.ToXmlString(code, name);
            return String.Format("{0}System.Xml.XmlConvert.ToString({1}, {0}System.Xml.XmlDateTimeSerializationMode.RoundtripKind)", CsCodeWriter.Global, name);
        }
        public override string FromXmlString(CsCodeWriter code, string name)
        {
            if (XmlOptions.Format != null)
                return base.FromXmlString(code, name);
            return String.Format("{0}System.Xml.XmlConvert.ToDateTime({1}, {0}System.Xml.XmlDateTimeSerializationMode.RoundtripKind)", CsCodeWriter.Global, name);
        }
    }
}
