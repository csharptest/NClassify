
#pragma warning disable 1591

namespace NClassify.Library
{
    public interface IValidate
    {
        bool IsValid();
    }

    public interface IMessage : global::System.ICloneable, IValidate, global::System.Xml.Serialization.IXmlSerializable
    {
        void Clear();
        void Initialize();

        void MergeFrom(IMessage other);

        void ReadXml(string localName, global::System.Xml.XmlReader reader);
        void MergeFrom(global::System.Xml.XmlReader reader);
        
        void WriteXml(string localName, global::System.Xml.XmlWriter writer);
        void MergeTo(global::System.Xml.XmlWriter reader);
    }

    public interface IDispatchStub : global::System.IDisposable
    {
        void CallMethod<TRequest, TResponse>(string methodName, TRequest request, TResponse response)
            where TRequest : class, global::NClassify.Library.IMessage
            where TResponse : class, global::NClassify.Library.IMessage;
    }

    public interface IServerStub
    {
        IMessage CallMethod(string methodName, global::System.Action<IMessage> readInput);
    }
}
