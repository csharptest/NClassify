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
            Assert.AreEqual("The field {0} is not valid.", Resources.BrokenRulesMessage);
        }
    }
}
