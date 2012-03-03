#pragma warning disable 1591
namespace NClassify.Library
{
    public sealed class EmptyMessage : IBuilder
    {
        private static readonly EmptyMessage Instance = new EmptyMessage();
        public static EmptyMessage DefaultInstance { get { return Instance; } }

        bool IBuilder.IsReadOnly() { return false; }
        void IBuilder.MakeReadOnly() { }
        void IBuilder.Clear() { }
        void IBuilder.AcceptDefaults() { }
        void IBuilder.MergeFrom(IMessage reader) { }
        void IBuilder.ReadXml(string localName, global::System.Xml.XmlReader reader) { }
        void IBuilder.MergeFrom(global::System.Xml.XmlReader reader) { }
        void IMessage.WriteXml(string localName, global::System.Xml.XmlWriter writer) { }
        void IMessage.MergeTo(global::System.Xml.XmlWriter reader) { }
        object global::System.ICloneable.Clone() { return this; }
        bool IValidate.IsValid() { return true; }
        void IValidate.AssertValid() { }
        int IValidate.GetBrokenRules(global::System.Action<ValidationError> onError) { return 0; }
        global::System.Xml.Schema.XmlSchema global::System.Xml.Serialization.IXmlSerializable.GetSchema() { return null; }
        void global::System.Xml.Serialization.IXmlSerializable.ReadXml(global::System.Xml.XmlReader reader) { }
        void global::System.Xml.Serialization.IXmlSerializable.WriteXml(global::System.Xml.XmlWriter writer) { }
    }
}