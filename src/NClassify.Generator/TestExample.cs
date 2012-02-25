﻿using System;
using System.Xml.Serialization;
using NClassify.Example;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NClassify.Library;

namespace Example
{
    class TestExample
    {
        static void Main()
        {
            SampleMsg child = new SampleMsg();
            child.Initialize();

            Console.WriteLine(child.ToXml("message"));
            TestRoundTripXml(child);

            SingleFields msg = new SingleFields();
            msg.Initialize();
            msg.SampleMsg = child;

            Console.WriteLine(msg.ToXml("single"));
            TestRoundTripXml(msg);

            ArrayFields ary = new ArrayFields();
            ary.ReadXml("sample", msg.ToXml("sample"));
            ary.ReadXml("sample", new SingleFields { Int16 = 2, Int32 = 2, Int64 = 2, Int8 = 2 }.ToXml("sample"));
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList[ary.SampleMsgList.Count - 1].Initialize();

            Console.WriteLine(ary.ToXml("array"));
            TestRoundTripXml(ary);

            Console.ReadLine();
        }

        static void TestRoundTripXml<T>(T msg) where T : IMessage
        {
            string xml = msg.ToXml("root");
            IMessage copy = (IMessage)msg.Clone();
            if (xml != copy.ToXml("root"))
                throw new ApplicationException();
        
            copy.Clear();
            copy.ReadXml("root", xml);

            if (xml != copy.ToXml("root"))
                throw new ApplicationException();
        }
    }
}
