
#pragma warning disable 1591

namespace NClassify.Library
{
    public interface IValidate
    {
        bool IsValid();
    }

    public interface IMessage : global::System.ICloneable, global::NClassify.Library.IValidate, global::System.Xml.Serialization.IXmlSerializable
    {
        void Clear();
        void Initialize();

        void ReadXml(string localName, global::System.Xml.XmlReader reader);
        void MergeFrom(global::System.Xml.XmlReader reader);
        
        void WriteXml(string localName, global::System.Xml.XmlWriter writer);
    }
}
