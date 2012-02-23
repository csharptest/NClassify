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
            new XmlSerializer(typeof (CallbacksResponse));

            foreach (Type t in typeof(TestExample).Assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof(System.Xml.Serialization.XmlTypeAttribute), false).Length == 0)
                    continue;
                Console.WriteLine("Testing {0}", t);
                
                XmlSerializer ser = new XmlSerializer(t);
                object copy, obj = t.IsEnum ? Enum.GetValues(t).GetValue(0) : Activator.CreateInstance(t);
                StringWriter writer = new StringWriter();
                using (XmlWriter w = XmlWriter.Create(writer, new XmlWriterSettings { CloseOutput = false, Indent = true }))
                    ser.Serialize(w, obj);

                string xml = writer.ToString();
                Console.WriteLine(xml);

                using (XmlReader r = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { CloseInput = false }))
                    copy = ser.Deserialize(r);

                if (copy.GetType() != obj.GetType())
                    throw new ApplicationException();
            }
        }
    }
}
