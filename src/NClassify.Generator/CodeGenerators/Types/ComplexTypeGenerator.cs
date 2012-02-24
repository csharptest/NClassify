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

            string xmlns = CsCodeWriter.Global + "System.Xml";
            using (code.WriteBlock("{0}.Schema.XmlSchema {0}.Serialization.IXmlSerializable.GetSchema()", xmlns))
            {
                code.WriteLine("return null;");
            }

            using (code.WriteBlock("public void ReadXml({0}.XmlReader reader)", xmlns))
            {
            }

            using (code.WriteBlock("public void WriteXml({0}.XmlWriter writer)", xmlns))
                code.WriteLine("WriteXml(\"{0}\", writer);", XmlName);
            
            using (code.WriteBlock("public void WriteXml(string localName, {0}.XmlWriter writer)", xmlns))
            {
                code.WriteLine("writer.WriteStartElement(localName);");
                Action<BaseFieldGenerator> write =
                    (fld) =>
                    {
                        using(fld.HasBackingName != null ? code.WriteBlock("if ({0})", fld.HasBackingName) : null)
                            fld.WriteXmlOutput(code, fld.FieldBackingName);
                    };

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Attribute))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Element))
                    write(fld);

                foreach (var fld in fields.Where(f => f.XmlAttribute == XmlAttributeType.Text))
                    write(fld);

                code.WriteLine("writer.WriteEndElement();");
            }
        }
    }
}
