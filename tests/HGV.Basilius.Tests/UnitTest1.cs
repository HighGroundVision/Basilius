using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HGV.Basilius.Clients;
using System.Threading.Tasks;

namespace HGV.Basilius.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var client = new HeroMetaClient();
            var heroes = await client.GetHeroes();

            Assert.IsTrue(heroes.Count > 0);
        }
    }
}
