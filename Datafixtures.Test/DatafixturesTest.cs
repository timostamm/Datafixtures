using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Datafixtures
{


    [TestClass]
    public class DatafixturesTest
    {



        [TestMethod]
        public void TestDependency()
        {
            var branch = loader.Add<BranchFixture>();
            loader.Load();
            Assert.AreEqual("Branch Leaf", branch.Result);
        }


        [TestMethod]
        public void TestSingleInstances()
        {
            var branch = loader.Add<BranchFixture>();
            var branch2 = loader.Add<BranchFixture>();
            loader.Load();
            Assert.AreSame(branch, branch2);
        }


        [TestMethod]
        public void TestCyclicDependency()
        {
            try
            {
                loader.Add<FixtureDependingOnItself>();
                Assert.Fail("There should have been an exception");
            }
            catch (Exception ex)
            {
                StringAssert.Contains(ex.Message, "Detected cyclic dependency");
            }
        }


        [TestMethod]
        public void TestDependingOnService()
        {
            loader.RegisterService(new DummyService());
            FixtureDependingOnService e = loader.Add<FixtureDependingOnService>();
            loader.Load();
            Assert.AreEqual("DummyService", e.Result);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestNotLoaded()
        {
            var r = new LeafFixture().Result;
        }



        private FixtureLoader loader;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            loader = new FixtureLoader();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            loader = null;
        }



        class LeafFixture : Fixture<string>
        {
            protected override string DoLoad()
            {
                return "Leaf";
            }
        }

        class BranchFixture : Fixture<string>
        {
            private readonly LeafFixture leaf;
            public BranchFixture(LeafFixture leaf)
            {
                this.leaf = leaf;
            }
            protected override string DoLoad()
            {
                return "Branch " + leaf.Result;
            }
        }

        class FixtureDependingOnItself : Fixture<string>
        {
            public FixtureDependingOnItself(FixtureDependingOnItself self)
            { }
            protected override string DoLoad()
            {
                return "this should never work";
            }
        }

        class FixtureDependingOnService : Fixture<string>
        {
            private readonly DummyService service;
            public FixtureDependingOnService(DummyService service)
            {
                this.service = service;
            }
            protected override string DoLoad()
            {
                return this.service.GetType().Name;
            }
        }

        class DummyService
        { }




    }
}
