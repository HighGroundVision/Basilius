using HGV.Basilius.Client;
using HGV.Basilius.Contants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HGV.Basilius.Tests
{
    [TestClass]
    public class MetaClientTests
    {
        [TestMethod]
        public void ModesCount()
        {
            IMetaClient client = new MetaClient();
            var modes = client.GetModes();

            Assert.AreEqual(25, modes.Count);
        }

        [TestMethod]
        public void GetMode()
        {
            IMetaClient client = new MetaClient();
            var mode = client.GetMode(1);

            Assert.AreEqual("All Pick", mode);
        }

        [TestMethod]
        public void RegionsCount()
        {
            IMetaClient client = new MetaClient();
            var regions = client.GetRegions();

            Assert.AreEqual(23, regions.Count());
        }

        [TestMethod]
        public void GetRegion()
        {
            IMetaClient client = new MetaClient();
            var region = client.GetRegion(1);

            Assert.AreEqual(1, region.Id);
            Assert.AreEqual("USWest", region.Key);
            Assert.AreEqual("US West", region.Name);
            Assert.AreEqual(47.6052, region.Latitude);
            Assert.AreEqual(-122.16999816894531, region.Longitude);
            Assert.IsTrue(Enumerable.SequenceEqual(new List<int>() { 111,112,113,114,117,118 }, region.Clusters));
        }

        [TestMethod]
        public void GetRegionFromCluster()
        {
            IMetaClient client = new MetaClient();
            var region = client.GetRegionFromCluster(111);

            Assert.AreEqual(1, region.Id);
            Assert.AreEqual("USWest", region.Key);
            Assert.AreEqual("US West", region.Name);
            Assert.AreEqual(47.6052, region.Latitude);
            Assert.AreEqual(-122.16999816894531, region.Longitude);
            Assert.IsTrue(Enumerable.SequenceEqual(new List<int>() { 111,112,113,114,117,118 }, region.Clusters));
        }

        [TestMethod]
        public void HeroesCount()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes();

            Assert.AreEqual(124, heroes.Count());
        }

        [TestMethod]
        public void HeroesEnabled()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(enabled: true);

            Assert.AreEqual(123, heroes.Count());
        }

        [TestMethod]
        public void CaptainsModeEnabled()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(CaptainsModeEnabled: true);

            Assert.AreEqual(123, heroes.Count());
        }

        [TestMethod]
        public void AbilityDraftEnabled()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(AbilityDraftEnabled: true);

            Assert.AreEqual(123, heroes.Count());
        }

        [TestMethod]
        public void NewPlayerEnable()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(NewPlayerEnable: true);

            Assert.AreEqual(105, heroes.Count());
        }

        [TestMethod]
        public void PrimaryAttribute_AGI()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(PrimaryAttribute: PrimaryAttributes.AGI);

            Assert.AreEqual(39, heroes.Count());
        }

        [TestMethod]
        public void PrimaryAttribute_STR()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(PrimaryAttribute: PrimaryAttributes.STR);

            Assert.AreEqual(42, heroes.Count());
        }

        [TestMethod]
        public void PrimaryAttribute_INT()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes(PrimaryAttribute: PrimaryAttributes.INT);

            Assert.AreEqual(43, heroes.Count());
        }

        // Tinker WTF?
        [TestMethod]
        public void AbilityDraftPool()
        {
            IMetaClient client = new MetaClient();
            var heroes = client.GetHeroes();
            var collection = heroes.Where(_ => _.AbilityDraftPool.Count() > 4).ToList();

            Assert.AreEqual(1, collection.Count);
        }

        [TestMethod]
        public void GetHero()
        {
            IMetaClient client = new MetaClient();
            var hero = client.GetHero(1);

            Assert.AreEqual(1, hero.Id);
            Assert.AreEqual("Anti-Mage", hero.Name);

            // TODO: Assert Hero Properties
        }

        [TestMethod]
        public void AbilitiesCount()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities();

            Assert.AreEqual(2424, abilities.Count());
        }

        [TestMethod]
        public void SkillCount()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(IsSkill: true);

            Assert.AreEqual(804, abilities.Count());
        }

        [TestMethod]
        public void UltimateCount()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(IsUltimate: true);

            Assert.AreEqual(155, abilities.Count());
        }

        [TestMethod]
        public void IsGrantedByScepter()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(IsGrantedByScepter: true);

            Assert.AreEqual(49, abilities.Count());
        }

        
        [TestMethod]
        public void IsGrantedByShard()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(IsGrantedByShard: true);

            Assert.AreEqual(43, abilities.Count());
        }

        [TestMethod]
        public void HasScepterUpgrade()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(HasScepterUpgrade: true);

            Assert.AreEqual(124, abilities.Count());
        }

        [TestMethod]
        public void AbilitiesByHeroId()
        {
            IMetaClient client = new MetaClient();
            var abilities = client.GetAbilities(HeroId: 1);

            Assert.AreEqual(5, abilities.Count());
        }

        [TestMethod]
        public void GetAbility()
        {
            IMetaClient client = new MetaClient();
            var ability = client.GetAbility(5003);

            Assert.AreEqual(5003, ability.Id);
            Assert.AreEqual("Mana Break", ability.Name);

            // TODO: Assert Ability Properties
        }

        [TestMethod]
        public void ItemsCount()
        {
            IMetaClient client = new MetaClient();
            var items = client.GetItems();

            Assert.AreEqual(442, items.Count());
        }

        [TestMethod]
        public void NeutralCount()
        {
            IMetaClient client = new MetaClient();
            var items = client.GetItems(IsNeutralDrop: true);

            Assert.AreEqual(125, items.Count());
        }

        [TestMethod]
        public void GetItem()
        {
            IMetaClient client = new MetaClient();
            var item = client.GetItem(1);

            Assert.AreEqual(1, item.Id);
            Assert.AreEqual("Blink Dagger", item.Name);

            // TODO: Assert Item Properties
        }
    }
}

