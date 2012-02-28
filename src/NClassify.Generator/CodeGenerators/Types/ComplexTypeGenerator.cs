using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class ComplexTypeGenerator : BaseTypeGenerator<ComplexType>
    {
        public ComplexTypeGenerator(ComplexType type)
            : base(type)
        { }

        public override void DeclareType(CsCodeWriter code)
        {
            string[] derives = new string[]
                                   {
                                       CsCodeWriter.Global + "System.ICloneable",
                                       CsCodeWriter.Global + "System.Xml.Serialization.IXmlSerializable",
                                       String.Format("{0}NClassify.Library.IMessage", CsCodeWriter.Global),
                                   };
            using (code.DeclareClass(
                new CodeItem(PascalName)
                    {
                        Access = Access,
                        XmlName = Name,
                    }, 
                    derives))
            {
                WriteChildren(code, Type.ChildTypes);
                Fields.ForAll(x => x.DeclareTypes(code));

                using (code.CodeRegion("Static Data"))
                    Fields.ForAll(x => x.DeclareStaticData(code));
                using (code.CodeRegion("Instance Fields"))
                    Fields.ForAll(x => x.DeclareInstanceData(code));
                using (code.CodeRegion("Instance Members"))
                {
                    Fields.ForAll(x => x.WriteMember(code));
                    WriteMembers(code, Fields);
                }
            }
        }

        protected override void WriteMembers(CsCodeWriter code, ICollection<BaseFieldGenerator> fields)
        {
            base.WriteMembers(code, fields);

            using (code.WriteBlock("public void Initialize()"))
            {
                foreach (var fld in fields)
                {
                    if (fld.HasBackingName != null)
                        code.WriteLine("{0} = true;", fld.HasBackingName);
                }
            }

            using (code.WriteBlock("public void Clear()"))
            {
                foreach (var fld in fields)
                {
                    if (fld.HasBackingName != null)
                        code.WriteLine("{0} = false;", fld.HasBackingName);
                    code.WriteLine("{0} = {1};", fld.FieldBackingName, fld.MakeConstant(code, null));
                }
            }

            code.WriteLine("object {0}System.ICloneable.Clone() {{ return Clone(); }}", CsCodeWriter.Global);
            using (code.WriteBlock("public {0} Clone()", PascalName))
            {
                code.WriteLine("{0} value = ({0})this.MemberwiseClone();", PascalName);
                foreach (var fld in fields)
                    fld.WriteClone(code);
                code.WriteLine("return value;");
            }

            using (code.WriteBlock("public void MergeFrom({0}NClassify.Library.IMessage other)", CsCodeWriter.Global))
            {
                code.WriteLine("if (other is {0}) MergeFrom(({0})other);", PascalName);
            }
            
            using (code.WriteBlock("public void MergeFrom({0} other)", PascalName))
            {
                foreach (var fld in fields)
                    fld.WriteCopy(code, "other");
            }

            using (code.WriteBlock("{0}System.Xml.Schema.XmlSchema {0}System.Xml.Serialization.IXmlSerializable.GetSchema()", CsCodeWriter.Global))
            {
                code.WriteLine("return null;");
            }

            WriteXmlReadMembers(code, fields);
            WriteXmlWriteMembers(code, fields);
        }

        protected void WriteXmlReadMembers(CsCodeWriter code, ICollection<BaseFieldGenerator> rawfields)
        {
            List<BaseFieldGenerator> fields = new List<BaseFieldGenerator>(rawfields);
            fields.Sort((a, b) => StringComparer.Ordinal.Compare(a.XmlName, b.XmlName));

            string xmlns = CsCodeWriter.Global + "System.Xml";
            using (code.WriteBlock("public void ReadXml({0}.XmlReader reader)", xmlns))
                code.WriteLine("ReadXml(\"{0}\", reader);", XmlName);

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
                code.WriteLine(
                    "global::System.Text.StringBuilder sbuilder = new global::System.Text.StringBuilder();");

                code.WriteLine("string[] fields = new string[] {{ \"{0}\" }};",
                               String.Join("\", \"", fields.Select(f => f.XmlName).ToArray()));
                bool hasMessage = fields.Exists(f => f.IsMessage);
                if (hasMessage)
                    code.WriteLine("bool[] isMessage = new bool[] {{ {0} }};",
                                   String.Join(", ", fields.Select(f => f.IsMessage ? "true" : "false").ToArray()));

                using (code.WriteBlock("while (!reader.EOF && reader.Depth >= depth)"))
                {
                    code.WriteLine("if (reader.NodeType == global::System.Xml.XmlNodeType.EndElement) break;");
                    code.WriteLine("bool isElement = reader.NodeType == global::System.Xml.XmlNodeType.Element;");
                    code.WriteLine("bool isAttribute = reader.NodeType == global::System.Xml.XmlNodeType.Attribute;");
                    using (code.WriteBlock("if (!isElement && !isAttribute)"))
                    {
                        code.WriteLine("reader.Read();");
                        code.WriteLine("continue;");
                    }

                    code.WriteLine("int field = global::System.Array.BinarySearch(fields, reader.LocalName, {0}System.StringComparer.Ordinal);", CsCodeWriter.Global);

                    if (hasMessage)
                    {
                        using (code.WriteBlock("if (isElement && field >= 0 && isMessage[field])"))
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
                    using (hasMessage ? code.WriteBlock("else") : null)
                    {
                        code.WriteLine("sbuilder.Length = 0;");
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

                        //global::System.Console.WriteLine("{0} = {1}", name, sbuilder);
                        using (code.WriteBlock("switch(field)"))
                        {
                            for (int i = 0; i < fields.Count; i++)
                            {
                                if (fields[i].IsMessage)
                                    continue;

                                using (code.WriteBlock("case {0}:", i))
                                {
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
        }

        protected void WriteXmlWriteMembers(CsCodeWriter code, ICollection<BaseFieldGenerator> fields)
        {
            string xmlns = CsCodeWriter.Global + "System.Xml";
            using (code.WriteBlock("public void WriteXml({0}.XmlWriter writer)", xmlns))
                code.WriteLine("WriteXml(\"{0}\", writer);", XmlName);
            
            using (code.WriteBlock("public void WriteXml(string localName, {0}.XmlWriter writer)", xmlns))
            {
                code.WriteLine("writer.WriteStartElement(localName);");
                code.WriteLine("MergeTo(writer);");
                code.WriteLine("writer.WriteFullEndElement();");
            }

            using (code.WriteBlock("public void MergeTo({0}.XmlWriter writer)", xmlns))
            {
                Action<BaseFieldGenerator> write =
                    (fld) =>
                    {
                        using (fld.HasBackingName != null ? code.WriteBlock("if ({0})", fld.HasBackingName) : null)
                            fld.WriteXmlOutput(code, fld.FieldBackingName);
                    };

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Attribute))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Element))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Text))
                    write(fld);
            }
        }
    }
}
