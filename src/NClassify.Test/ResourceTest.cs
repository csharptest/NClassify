using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NClassify.Library;

namespace NClassify.Test
{
    [TestClass]
    public class ResourceTest
    {
        [TestMethod]
        public void TestResources()
        {
            Assert.AreEqual("The field {0} is invalid.", Resources.InvalidField);
        }

        [TestMethod]
        public void TestToFromTimeSpan()
        {
            Random r = new Random();

            long ticks = (((long)r.Next(2048) << 30) + r.Next());
            ticks -= ticks % TimeSpan.TicksPerMillisecond; // round to nearest ms
            TimeSpan test = new TimeSpan(-ticks);
            string text = test.ToString();

            DateTime start = DateTime.Now;
            for (int i = 0; i < 1000000; i++)
                GC.KeepAlive(TimeSpan.Parse(text));
            Console.WriteLine("TimeSpan.Parse = {0}", DateTime.Now - start);
            start = DateTime.Now;

            text = TypeConverter.Instance.ToString(test, "{1}", null);
            for (int i = 0; i < 1000000; i++)
                GC.KeepAlive(TypeConverter.Instance.ParseTimeSpan(text, "{1}", null));
            Console.WriteLine("TypeConverter.ParseTimeSpan = {0}", DateTime.Now - start);

            string[] formats =
                new string[]
                    {
                        "{0}",
                        "{1}",
                        "0x{1:x16}",
                        "stuff b4{1}1morestuffafter",
                        "{2}{3} {4:d2}:{5:d2}:{6:d2}.{7:d3}ms",
                        "{2}{3}d {4}h {5}m {6}s {7}ms",
                        "{8} days",
                        "{9} hours",
                        "{10:f10} minutes",
                        "Total seconds: {11:n3}",
                        "{12}ms",
                    };

            for( int i=0; i < 10000; i++ )
            {
                ticks = (((long)r.Next(2048) << 30) + r.Next());
                ticks -= ticks % TimeSpan.TicksPerMillisecond; // round to nearest ms
                test = new TimeSpan(ticks);
                foreach (var fmt in formats)
                {
                    text = TypeConverter.Instance.ToString(test, fmt, null);
                    TimeSpan result = TypeConverter.Instance.ParseTimeSpan(text, fmt, null);

                    if (i == 0) Console.WriteLine("{0} => {1} == {2}", test, fmt, text);
                    Assert.AreEqual(test.Ticks, result.Ticks);

                    TimeSpan neg = test.Negate();
                    text = TypeConverter.Instance.ToString(neg, fmt, null);
                    result = TypeConverter.Instance.ParseTimeSpan(text, fmt, null);

                    if (i == 0) Console.WriteLine("{0} => {1} == {2}", test, fmt, text);
                    Assert.AreEqual(neg.Ticks, result.Ticks);
                }
            }
        }
    }
}
