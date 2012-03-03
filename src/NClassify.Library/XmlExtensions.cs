#pragma warning disable 1591
namespace NClassify.Library
{
    public static class XmlExtensions
    {
        public static string ToXml(this IMessage msg, string name)
        {
            global::System.IO.StringWriter xml = new global::System.IO.StringWriter();
            using (global::System.Xml.XmlWriter w = global::System.Xml.XmlWriter.Create(
                xml, new global::System.Xml.XmlWriterSettings() { Indent = true, CloseOutput = false }))
                msg.WriteXml(name, w);
            return xml.ToString();
        }

        public static void ReadXml(this IBuilder msg, string name, string xml)
        {
            using (global::System.Xml.XmlReader r = global::System.Xml.XmlReader.Create(
                new global::System.IO.StringReader(xml), 
                new global::System.Xml.XmlReaderSettings() { CloseInput = false }))
                msg.ReadXml(name, r);
        }
    }
}
