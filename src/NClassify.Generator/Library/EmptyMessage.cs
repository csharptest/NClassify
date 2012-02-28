#pragma warning disable 1591

namespace NClassify.Library
{
    public sealed class EmptyMessage : IMessage
    {
        public static readonly EmptyMessage Default = new EmptyMessage();

        void IMessage.Clear()
        {
        }

        void IMessage.Initialize()
        {
        }

        void IMessage.MergeFrom(IMessage reader)
        {
        }

        void IMessage.ReadXml(string localName, global::System.Xml.XmlReader reader)
        {
        }

        void IMessage.MergeFrom(global::System.Xml.XmlReader reader)
        {
        }

        void IMessage.WriteXml(string localName, global::System.Xml.XmlWriter writer)
        {
        }

        void IMessage.MergeTo(global::System.Xml.XmlWriter reader)
        {
        }

        object global::System.ICloneable.Clone()
        {
            return this;
        }

        bool IValidate.IsValid()
        {
            return true;
        }

        global::System.Xml.Schema.XmlSchema global::System.Xml.Serialization.IXmlSerializable.GetSchema()
        { return null; }

        void global::System.Xml.Serialization.IXmlSerializable.ReadXml(global::System.Xml.XmlReader reader)
        {
        }

        void global::System.Xml.Serialization.IXmlSerializable.WriteXml(global::System.Xml.XmlWriter writer)
        {
        }
    }
}