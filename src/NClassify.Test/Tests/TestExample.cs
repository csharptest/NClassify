#region Copyright (c) 2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NClassify.Example;
using NClassify.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NClassify.Test.Tests
{
    [TestClass]
    public class TestExample
    {
        [TestMethod]
        public void TestCircularMessages()
        {
            CircleA a = new CircleA();
            a.ToString();
        }

        [TestMethod]
        public void TestXmlRoundTrip()
        {
            SampleMsg child = new SampleMsg();
            child.AcceptDefaults();
            child.DateModifiedList.Add(DateTime.Now.AddDays(-2));
            child.DateModifiedList.Add(DateTime.Now.AddDays(-1));
            child.DateModifiedList.Add(DateTime.Now.AddDays(0));

            Trace.TraceInformation(ToXml(child));
            TestRoundTripXml(child);

            SingleFields msg = new SingleFields();
            msg.AcceptDefaults();
            msg.SampleMsg = child;

            Trace.TraceInformation(ToXml(msg));
            TestRoundTripXml(msg);

            ArrayFields ary = new ArrayFields();
            ReadXml(ary, ToXml(msg));
            ReadXml(ary, ToXml(new SingleFields { Int16 = 2, Int32 = 2, Int64 = 2, Int8 = 2 }));
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList[ary.SampleMsgList.Count - 1].AcceptDefaults();

            Trace.TraceInformation(ToXml(ary));
            TestRoundTripXml(ary);
        }

        static void TestRoundTripXml<T>(T msg) where T : IBuilder
        {
            string xml = ToXml(msg);
            IBuilder copy = (IBuilder)msg.Clone();
            if (xml != ToXml(copy))
                throw new ApplicationException();

            copy.Clear();
            ReadXml(copy, xml);

            if (xml != ToXml(copy))
                throw new ApplicationException();
        }


        static string ToXml(IMessage msg)
        {
            global::System.IO.StringWriter xml = new global::System.IO.StringWriter();
            using (global::System.Xml.XmlWriter w = global::System.Xml.XmlWriter.Create(
                xml, new global::System.Xml.XmlWriterSettings() { Indent = true, CloseOutput = false }))
                msg.WriteXml("record", w);
            return xml.ToString();
        }

        static void ReadXml(IBuilder msg, string xml)
        {
            using (global::System.Xml.XmlReader r = global::System.Xml.XmlReader.Create(
                new global::System.IO.StringReader(xml),
                new global::System.Xml.XmlReaderSettings() { CloseInput = false }))
                msg.ReadXml("record", r);
        }
    }
}
