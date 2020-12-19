using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using HGV.Basilius.Contants;

namespace HGV.Basilius.Client
{
    public interface IMetaClient
    {
        IReadOnlyDictionary<int, string> GetModes();
        string GetMode(int id);
        IEnumerable<Region> GetRegions();
        Region GetRegion(ServerRegion region);
        Region GetRegion(int id);
        Region GetRegionFromCluster(int id);
        IEnumerable<Hero> GetHeroes();
        IEnumerable<Hero> GetHeroes(bool? enabled = null, bool? CaptainsModeEnabled = null, bool? AbilityDraftEnabled = null, bool? NewPlayerEnable = null, string PrimaryAttribute = null);
        Hero GetHero(int id);
        IEnumerable<Ability> GetAbilities();
        IEnumerable<Ability> GetAbilities(int? HeroId = null,bool? IsAttribute = null, bool? IsSkill = null, bool? IsUltimate = null,bool? IsPassive = null, bool? IsChannelled = null, bool? NoTarget = null,bool? PointTarget = null,bool? VectorTargeting = null,bool? IsAOE = null,bool? IsToggle = null,bool? IsAutocast = null,bool? IsDirectional = null,bool? IsAura = null,bool? IsAttack = null,bool? DoesntCancelChannel = null,bool? DisabledByRoot = null,bool? HasScepterUpgrade = null,bool? IsGrantedByScepter = null,bool? IsGrantedByShard = null,bool? IsLinked = null);
        Ability GetAbility(int id);
        IEnumerable<Item> GetItems();
        IEnumerable<Item> GetItems(bool? IsSellable = null,bool? IsPurchasable = null,bool? IsDroppable = null,bool? IsRecipe = null,bool? IsStackable = null,bool? IsPermanent = null,bool? IsNeutralDrop = null,bool? IsObsolete = null);
        Item GetItem(int id);
    }

    public class MetaClient : IMetaClient
    {
        private Dictionary<int, string> modes = new Dictionary<int, string>();
        private Dictionary<int, int> clusters = new Dictionary<int, int>();
        private List<Region> regions = new List<Region>();
        private List<Hero> heroes = new List<Hero>();
        private List<Item> items = new List<Item>();
        private List<Ability> abilities = new List<Ability>();

        public MetaClient()
        {
            LoadData();
        }

        private void LoadData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var serailizer = JsonSerializer.CreateDefault();

            this.abilities = LoadData<List<Ability>>(assembly, serailizer, "HGV.Basilius.Data.Abilities.json");
            this.heroes = LoadData<List<Hero>>(assembly, serailizer, "HGV.Basilius.Data.Heroes.json");
            this.items = LoadData<List<Item>>(assembly, serailizer, "HGV.Basilius.Data.Items.json");
            this.modes = LoadData<Dictionary<int, string>>(assembly, serailizer, "HGV.Basilius.Data.Modes.json");
            this.clusters = LoadData<Dictionary<int, int>>(assembly, serailizer, "HGV.Basilius.Data.Clusters.json");
            this.regions = LoadData<List<Region>>(assembly, serailizer, "HGV.Basilius.Data.Regions.json");
        }

        private static T LoadData<T>(Assembly assembly, JsonSerializer serailizer, string file)
        {
            using (var stream = assembly.GetManifestResourceStream(file))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return serailizer.Deserialize<T>(jsonReader);
            }
        }

        public IReadOnlyDictionary<int, string> GetModes()
        {
            return this.modes;
        }

        public string GetMode(int id)
        {
            if(this.modes.TryGetValue(id, out string mode))
                return mode;
            else
                return null;
        }

        public IEnumerable<Region> GetRegions()
        {
            return this.regions;
        }

        public Region GetRegion(ServerRegion id)
        {
            return this.GetRegion((int)id);
        }

        public Region GetRegion(int id)
        {
            return this.regions.Find(_ => _.Id == id);
        }

        public Region GetRegionFromCluster(int id)
        {
            if(this.clusters.TryGetValue(id, out int region))
                return this.regions.Find(_ => _.Id == region);
            else
                return null;
        }

        public IEnumerable<Hero> GetHeroes()
        {
            return this.heroes;
        }

        public IEnumerable<Hero> GetHeroes(
            bool? Enabled = null, 
            bool? CaptainsModeEnabled = null, 
            bool? AbilityDraftEnabled = null, 
            bool? NewPlayerEnable = null, 
            string PrimaryAttribute = null)
        {
            var query = this.heroes.AsQueryable();
            if(Enabled.HasValue)
                query = query.Where(_ => _.Enabled == Enabled.Value);
            if(CaptainsModeEnabled.HasValue)
                query = query.Where(_ => _.CaptainsModeEnabled == CaptainsModeEnabled.Value);
            if(AbilityDraftEnabled.HasValue)
                query = query.Where(_ => _.AbilityDraftEnabled == AbilityDraftEnabled.Value);
            if(NewPlayerEnable.HasValue)
                query = query.Where(_ => _.NewPlayerEnable == NewPlayerEnable.Value);
            if(PrimaryAttribute != null)
                query = query.Where(_ => _.AttributePrimary == PrimaryAttribute);

            return query.ToList();
        }

        public Hero GetHero(int id)
        {
            return this.heroes.Find(_ => _.Id == id);
        }

        public IEnumerable<Ability> GetAbilities()
        {
            return this.abilities;
        }

        public IEnumerable<Ability> GetAbilities(
            int? HeroId = null,
            bool? IsAttribute = null, 
            bool? IsSkill = null, 
            bool? IsUltimate = null,
            bool? IsPassive = null, 
            bool? IsChannelled = null, 
            bool? NoTarget = null,
            bool? PointTarget = null,
            bool? VectorTargeting = null,
            bool? IsAOE = null,
            bool? IsToggle = null,
            bool? IsAutocast = null,
            bool? IsDirectional = null,
            bool? IsAura = null,
            bool? IsAttack = null,
            bool? DoesntCancelChannel = null,
            bool? DisabledByRoot = null,
            bool? HasScepterUpgrade = null,
            bool? IsGrantedByScepter = null,
            bool? IsGrantedByShard = null,
            bool? IsLinked = null)
        {
            var query = this.abilities.AsQueryable();
            if(HeroId.HasValue)
                query = query.Where(_ => _.HeroId == HeroId.Value);
            if(IsAttribute.HasValue)
                query = query.Where(_ => _.IsAttribute == IsAttribute.Value);
            if(IsSkill.HasValue)
                query = query.Where(_ => _.IsSkill == IsSkill.Value);
            if(IsUltimate.HasValue)
                query = query.Where(_ => _.IsUltimate == IsUltimate.Value);
            if(IsPassive.HasValue)
                query = query.Where(_ => _.IsPassive == IsPassive.Value);
            if(IsChannelled.HasValue)
                query = query.Where(_ => _.IsChannelled == IsChannelled.Value);
            if(NoTarget.HasValue)
                query = query.Where(_ => _.NoTarget == NoTarget.Value);
            if(PointTarget.HasValue)
                query = query.Where(_ => _.PointTarget == PointTarget.Value);
            if(VectorTargeting.HasValue)
                query = query.Where(_ => _.VectorTargeting == VectorTargeting.Value);
            if(IsAOE.HasValue)
                query = query.Where(_ => _.IsAOE == IsAOE.Value);
            if(IsToggle.HasValue)
                query = query.Where(_ => _.IsToggle == IsToggle.Value);
            if(IsAutocast.HasValue)
                query = query.Where(_ => _.IsAutocast == IsAutocast.Value);
            if(IsDirectional.HasValue)
                query = query.Where(_ => _.IsDirectional == IsDirectional.Value);
            if(IsAura.HasValue)
                query = query.Where(_ => _.IsAura == IsAura.Value);
            if(IsUltimate.HasValue)
                query = query.Where(_ => _.IsUltimate == IsUltimate.Value);
            if(IsAttack.HasValue)
                query = query.Where(_ => _.IsAttack == IsAttack.Value);
            if(DoesntCancelChannel.HasValue)
                query = query.Where(_ => _.DoesntCancelChannel == DoesntCancelChannel.Value);
            if(DisabledByRoot.HasValue)
                query = query.Where(_ => _.DisabledByRoot == DisabledByRoot.Value);
            if(IsUltimate.HasValue)
                query = query.Where(_ => _.IsUltimate == IsUltimate.Value);
            if(HasScepterUpgrade.HasValue)
                query = query.Where(_ => _.HasScepterUpgrade == HasScepterUpgrade.Value);
            if(IsGrantedByScepter.HasValue)
                query = query.Where(_ => _.IsGrantedByScepter == IsGrantedByScepter.Value);
            if(IsGrantedByShard.HasValue)
                query = query.Where(_ => _.IsGrantedByShard == IsGrantedByShard.Value);
            if(IsLinked.HasValue)
                query = query.Where(_ => IsLinked.Value == true ? _.Linked != null : _.Linked == null);

            return query.ToList();
        }

        public Ability GetAbility(int id)
        {
            return this.abilities.Find(_ => _.Id == id);
        }

        public IEnumerable<Item> GetItems()
        {
            return this.items;
        }

        public IEnumerable<Item> GetItems(
            bool? IsSellable = null,
            bool? IsPurchasable = null,
            bool? IsDroppable = null,
            bool? IsRecipe = null,
            bool? IsStackable = null,
            bool? IsPermanent = null,
            bool? IsNeutralDrop = null,
            bool? IsObsolete = null)
        {
            var query = this.items.AsQueryable();
            if(IsSellable.HasValue)
                query = query.Where(_ => _.ItemSellable == IsSellable.Value);
            if(IsPurchasable.HasValue)
                query = query.Where(_ => _.ItemPurchasable == IsPurchasable.Value);
            if(IsDroppable.HasValue)
                query = query.Where(_ => _.ItemDroppable == IsDroppable.Value);
            if(IsRecipe.HasValue)
                query = query.Where(_ => _.ItemRecipe == IsRecipe.Value);
            if(IsStackable.HasValue)
                query = query.Where(_ => _.ItemStackable == IsStackable.Value);
            if(IsPermanent.HasValue)
                query = query.Where(_ => _.ItemPermanent == IsPermanent.Value);
            if(IsNeutralDrop.HasValue)
                query = query.Where(_ => _.ItemIsNeutralDrop == IsNeutralDrop.Value);
            if(IsObsolete.HasValue)
                query = query.Where(_ => _.IsObsolete == IsObsolete.Value);

            return query.ToList();
        }

        public Item GetItem(int id)
        {
            return this.items.Find(_ => _.Id == id);
        }
    }
}
