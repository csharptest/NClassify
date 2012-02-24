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
            Callback cb = new Callback();
            cb.CallbackId = new CallbackId(5);
            cb.Initialize();
            cb.Uri = new WebUri("http://asdf");
            cb.HasEvent = false;
            
            using(XmlWriter w = XmlWriter.Create(Console.Out, new XmlWriterSettings() {Indent = true, CloseOutput = false}))
                cb.WriteXml(w);

            Console.ReadLine();
        }
    }
}
