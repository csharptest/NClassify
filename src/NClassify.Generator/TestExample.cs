using System;
using System.Xml.Serialization;
using NClassify.Generator;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Example
{
    class TestExample
    {
        static void Main()
        {
            CallbacksResponse cb = new CallbacksResponse();
            NClassify.Library.XmlReading.ReadXml("CallbacksResponse",
                new XmlTextReader(
                    new StringReader(
                        @"
<CallbacksResponse>
<callbacks>
<callback verified='0'>
  <callback_id>5</callback_id>
  <uri/>
  <event></event>
  <unk>asdf
<bla>
BAD
<br/>
</bla>
<!-- comment -->
1234</unk>
</callback>
<callback/>
</callbacks>
</CallbacksResponse>
")));
            
            using(XmlWriter w = XmlWriter.Create(Console.Out, new XmlWriterSettings() {Indent = true, CloseOutput = false}))
                cb.WriteXml(w);

            Console.ReadLine();
        }


    }
}

namespace NClassify.Generator
{
    partial class Callback
    {
        public void ReadXml(string localName, global::System.Xml.XmlReader reader)
        {
            while (reader.NodeType != global::System.Xml.XmlNodeType.Element && reader.Read())
            { }

            if (reader.NodeType != global::System.Xml.XmlNodeType.Element || reader.LocalName != localName)
                throw new global::System.FormatException();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    this[reader.LocalName] = reader.Value;
                } while (reader.MoveToNextAttribute());
                reader.MoveToElement();
            }
            global::System.Xml.XmlReader me = reader.ReadSubtree();
            reader.ReadStartElement(localName);
            while (true)
            {
                while (reader.NodeType != global::System.Xml.XmlNodeType.Element && reader.Read())
                { }
                this[reader.LocalName] = reader.ReadElementString();
            }
        }

        private object this[string name]
        {
            get { return null; }
            set { global::System.Console.WriteLine("{0} = {1}", name, value); }
        }
        
    }

}
