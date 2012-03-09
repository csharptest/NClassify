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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NClassify.Library;
using System.Diagnostics;

namespace NClassify.Test.Tests
{
    [TestClass]
    public class TestConverter
    {
        [TestMethod]
        public void TestToFromTimeSpan()
        {
            Random r = new Random();

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

            for (int i = 0; i < 10000; i++)
            {
                long ticks = (((long)r.Next(2048) << 30) + r.Next());
                ticks -= ticks % TimeSpan.TicksPerMillisecond; // round to nearest ms
                TimeSpan value = new TimeSpan(ticks);
                foreach (var fmt in formats)
                {
                    string text = TypeConverter.Instance.ToString(value, fmt, null);
                    TimeSpan result = TypeConverter.Instance.ParseTimeSpan(text, fmt, null);

                    if (i == 0) Trace.TraceInformation("{0} => {1} == {2}", value, fmt, text);
                    Assert.AreEqual(value.Ticks, result.Ticks);

                    TimeSpan neg = value.Negate();
                    text = TypeConverter.Instance.ToString(neg, fmt, null);
                    result = TypeConverter.Instance.ParseTimeSpan(text, fmt, null);

                    if (i == 0) Trace.TraceInformation("{0} => {1} == {2}", value, fmt, text);
                    Assert.AreEqual(neg.Ticks, result.Ticks);
                }
            }
        }
    }
}
