using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class ReaderWriterGenerator
    {
        private readonly string _xmlName;
        private readonly BaseType _type;
        private bool _subclass;

        public ReaderWriterGenerator(BaseType type, string xmlName, bool subclass)
        {
            _type = type;
            _xmlName = xmlName;
            _subclass = subclass;
        }

        public void WriteXmlReadMembers(CsCodeWriter code, IEnumerable<BaseFieldGenerator> rawfields)
        {
            List<BaseFieldGenerator> fields = new List<BaseFieldGenerator>(rawfields);
            fields.Sort((a, b) => StringComparer.Ordinal.Compare(a.XmlOptions.XmlName, b.XmlOptions.XmlName));
            bool hasMessage = fields.Exists(f => f.IsMessage);

            code.WriteLine("private static readonly string[] _fieldNamesToIx = new string[] {{ \"{0}\" }};",
                           String.Join("\", \"", fields.Select(f => f.XmlOptions.XmlName).ToArray()));
            if (hasMessage)
                code.WriteLine("private static readonly bool[] _isMessageFldIx = new bool[] {{ {0} }};",
                               String.Join(", ", fields.Select(f => f.IsMessage ? "true" : "false").ToArray()));

            string xmlns = CsCodeWriter.Global + "System.Xml";
            if (!_subclass)
            {
                using (code.WriteBlock("public void ReadXml({0}.XmlReader reader)", xmlns))
                    code.WriteLine("ReadXml(\"{0}\", reader);", _xmlName);

                using (code.WriteBlock("public void ReadXml(string localName, {0}.XmlReader reader)", xmlns))
                {
                    code.WriteLine("reader.MoveToContent();");
                    code.WriteLine("if (!reader.IsStartElement(localName))");
                    code.WriteLineIndent("throw new global::System.FormatException();");

                    code.WriteLine("if (reader.MoveToFirstAttribute())");
                    code.WriteLineIndent("MergeFrom(reader);");

                    code.WriteLine("bool empty = reader.IsEmptyElement;");
                    code.WriteLine("reader.ReadStartElement(localName);");
                    using (code.WriteBlock("if (!empty)"))
                    {
                        code.WriteLine("MergeFrom(reader);");
                        code.WriteLine("reader.ReadEndElement();");
                    }
                }
                using (code.WriteBlock("public void MergeFrom({0}.XmlReader reader)", xmlns))
                {
                    code.WriteLine("int depth = reader.Depth;");
                    using (code.WriteBlock("while (!reader.EOF && reader.Depth >= depth)"))
                    {
                        code.WriteLine("if (reader.NodeType == global::System.Xml.XmlNodeType.EndElement) break;");
                        using (code.WriteBlock("if (reader.NodeType != global::System.Xml.XmlNodeType.Element && reader.NodeType != global::System.Xml.XmlNodeType.Attribute)"))
                        {
                            code.WriteLine("reader.Read();");
                            code.WriteLine("continue;");
                        }
                        code.WriteLine("else MergeField(reader);");
                    }
                }
            }
            using (code.WriteBlock("protected {1} void MergeField({0}.XmlReader reader)", xmlns, _subclass ? "override" : "virtual"))
            {
                code.WriteLine("int field = global::System.Array.BinarySearch(_fieldNamesToIx, reader.LocalName, {0}System.StringComparer.Ordinal);", CsCodeWriter.Global);
                code.WriteLine("bool isElement = reader.NodeType == global::System.Xml.XmlNodeType.Element;");
                code.WriteLine("bool isAttribute = isElement ? false : reader.NodeType == global::System.Xml.XmlNodeType.Attribute;");
                code.WriteLine("if (!isElement && !isAttribute) throw new global::System.FormatException();");

                using (code.WriteBlock("if (field < 0)"))
                {
                    if (_subclass)
                        code.WriteLine("base.MergeField(reader);");
                    else
                    {
                        code.WriteLine("if (isElement)");
                        code.WriteLineIndent("reader.ReadInnerXml();");
                        code.WriteLine("else if (!reader.MoveToNextAttribute())");
                        code.WriteLineIndent("reader.MoveToElement();");
                    }
                }
                if (hasMessage)
                {
                    using (code.WriteBlock("else if (_isMessageFldIx[field])"))
                    {
                        code.WriteLine("if (!isElement)");
                        code.WriteLineIndent("throw new global::System.FormatException();");

                        using (code.WriteBlock("switch(field)"))
                        {
                            for (int i = 0; i < fields.Count; i++)
                            {
                                if (!fields[i].IsMessage)
                                    continue;

                                using (code.WriteBlock("case {0}:", i))
                                {
                                    fields[i].ReadXmlMessage(code);
                                    code.WriteLine("break;");
                                }
                            }
                        }
                    }
                }
                using (code.WriteBlock("else"))
                {
                    code.WriteLine("global::System.Text.StringBuilder sbuilder = new global::System.Text.StringBuilder();");
                    using (code.WriteBlock("if (isAttribute)"))
                    {
                        code.WriteLine("sbuilder.Append(reader.Value);");
                        code.WriteLine("if (!reader.MoveToNextAttribute())");
                        code.WriteLineIndent("reader.MoveToElement();");
                    }
                    code.WriteLine("else if (reader.IsEmptyElement)");
                    code.WriteLineIndent("reader.ReadStartElement();");
                    using (code.WriteBlock("else"))
                    {
                        code.WriteLine("int stop = reader.Depth;");
                        using (code.WriteBlock("while (reader.Read() && reader.Depth > stop)"))
                        {
                            code.WriteLine("while (reader.IsStartElement()) reader.Skip();");
                            code.WriteLine("if (((1 << (int)reader.NodeType) & 0x6018) != 0)");
                            code.WriteLineIndent("sbuilder.Append(reader.Value);");
                        }
                        code.WriteLine("reader.ReadEndElement();");
                    }
                    using (code.WriteBlock("switch(field)"))
                    {
                        for (int i = 0; i < fields.Count; i++)
                        {
                            if (fields[i].IsMessage)
                                continue;

                            using (code.WriteBlock("case {0}:", i))
                            {
                                using (fields[i].XmlOptions.IgnoreEmpty ? code.WriteBlock("if (sbuilder.Length > 0)") : null)
                                    fields[i].ReadXmlValue(code, "sbuilder.ToString()");
                                code.WriteLine("break;");
                            }
                        }
                        using (code.WriteBlock("default:"))
                        {
                            code.WriteLine("break;");
                        }
                    }
                }
            }
        }

        public void WriteXmlWriteMembers(CsCodeWriter code, IEnumerable<BaseFieldGenerator> rawfields)
        {
            List<BaseFieldGenerator> fields = new List<BaseFieldGenerator>(rawfields);
            string xmlns = CsCodeWriter.Global + "System.Xml";
            if (!_subclass)
            {
                using (code.WriteBlock("public void WriteXml({0}.XmlWriter writer)", xmlns))
                    code.WriteLine("WriteXml(\"{0}\", writer);", _xmlName);

                using (code.WriteBlock("public void WriteXml(string localName, {0}.XmlWriter writer)", xmlns))
                {
                    code.WriteLine("writer.WriteStartElement(localName);");
                    code.WriteLine("MergeTo(writer);");
                    code.WriteLine("writer.WriteFullEndElement();");
                }
            }
            using (code.WriteBlock("public {1} void MergeTo({0}.XmlWriter writer)", xmlns, _subclass ? "override" : "virtual"))
            {
                if (_subclass)
                    code.WriteLine("base.MergeTo(writer);");

                Action<BaseFieldGenerator> write =
                    (fld) =>
                    {
                        using (fld.HasBackingName != null ? code.WriteBlock("if ({0})", fld.HasBackingName) : null)
                            fld.WriteXmlOutput(code, fld.FieldBackingName);
                    };

                foreach (var fld in fields.Where(f => f.XmlOptions.AttributeType == XmlAttributeType.Attribute))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlOptions.AttributeType == XmlAttributeType.Element))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlOptions.AttributeType == XmlAttributeType.Text))
                    write(fld);
            }
        }

    }
}
