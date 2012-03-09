using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NClassify.Library;

namespace NClassify.Test.Tests
{
    [TestClass]
    public class ResourceTest
    {
        [TestMethod]
        public void TestResources()
        {
            Assert.AreEqual("The field {0} is invalid.", ResourceMessages.InvalidField);
        }
    }
}
