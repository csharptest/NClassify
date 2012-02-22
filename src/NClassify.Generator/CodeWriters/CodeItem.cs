using System;

namespace NClassify.Generator.CodeWriters
{
    public class CodeItem
    {
        public CodeItem(string name)
        {
            Name = name;
            Access = FieldAccess.Public;
            ClsCompliant = true;
            XmlAttribute = XmlAttributeType.Element;
        }
        public string Name;
        public FieldAccess Access;
        public string XmlName;
        public string Description;

        public bool Obsolete;
        public bool ClsCompliant;

        public XmlAttributeType XmlAttribute;
        public enum XmlAttributeType
        {
            Element,
            Attribute,
            Text,
        };
    }
}