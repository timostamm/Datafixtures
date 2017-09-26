using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datafixtures.Example.Fixtures;

namespace Datafixtures.Example
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestUnix()
        {
            var loader = new FixtureLoader();
            var msg = loader.Add<UnixStartMessage>();
            loader.Load();
            Assert.AreEqual("hello world, it is 1970", msg.Result);
        }
    }


}
