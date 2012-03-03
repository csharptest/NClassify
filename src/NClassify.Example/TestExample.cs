using System;
using NClassify.Library;

namespace NClassify.Example
{
    class TestExample
    {
        static void Main()
        {
            CircleA a = new CircleA();
            a.ToString();

            SampleMsg child = new SampleMsg();
            child.AcceptDefaults();
            child.DateModifiedList.Add(DateTime.Now.AddDays(-2));
            child.DateModifiedList.Add(DateTime.Now.AddDays(-1));
            child.DateModifiedList.Add(DateTime.Now.AddDays(0));

            Console.WriteLine(child.ToXml("message"));
            TestRoundTripXml(child);

            SingleFields msg = new SingleFields();
            msg.AcceptDefaults();
            msg.SampleMsg = child;

            Console.WriteLine(msg.ToXml("single"));
            TestRoundTripXml(msg);

            ArrayFields ary = new ArrayFields();
            ary.ReadXml("sample", msg.ToXml("sample"));
            ary.ReadXml("sample", new SingleFields { Int16 = 2, Int32 = 2, Int64 = 2, Int8 = 2 }.ToXml("sample"));
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList.Add(new SampleMsg());
            ary.SampleMsgList[ary.SampleMsgList.Count - 1].AcceptDefaults();

            Console.WriteLine(ary.ToXml("array"));
            TestRoundTripXml(ary);

            Console.ReadLine();
        }

        static void TestRoundTripXml<T>(T msg) where T : IBuilder
        {
            string xml = msg.ToXml("root");
            IBuilder copy = (IBuilder)msg.Clone();
            if (xml != copy.ToXml("root"))
                throw new ApplicationException();
        
            copy.Clear();
            copy.ReadXml("root", xml);

            if (xml != copy.ToXml("root"))
                throw new ApplicationException();
        }
    }
}
