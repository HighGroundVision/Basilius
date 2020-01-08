using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HGV.Basilius
{
    public class MetaClient
    {
        public static Lazy<MetaClient> Instance = new Lazy<MetaClient>(() => new MetaClient());

        List<Hero> heroes { get; set; }
        List<Item> items { get; set; }
        List<Ability> abilities { get; set; }
        List<Talent> talents { get; set; }

        Dictionary<int, int> clusters { get; set; }
        Dictionary<int, string> regions { get; set; }
        Dictionary<int, string> modes { get; set; }

        private MetaClient()
        {
            this.heroes = new List<Hero>();
            this.items = new List<Item>();
            this.abilities = new List<Ability>();
            this.talents = new List<Talent>();
            this.clusters = new Dictionary<int, int>();
            this.regions = new Dictionary<int, string>();
            this.modes = new Dictionary<int, string>();

            this.Load();

        }
        private void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Heroes.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.heroes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Hero>>(json);
            }

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Items.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item>>(json);
            }


            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Clusters.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.clusters = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
            }

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Regions.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.regions = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            }

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Regions.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.regions = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            }

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Modes.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.modes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            }

            this.abilities = this.heroes.SelectMany(_ => _.Abilities).ToList();
            this.talents = this.heroes.SelectMany(_ => _.Talents).ToList();
        }

        public string GetModeName(int mode)
        {
            string name = string.Empty;
            this.modes.TryGetValue(mode, out name);
            return name;
        }

        public Dictionary<int, string> GetModes()
        {
            return this.modes;
        }

        public string GetRegionName(int region)
        {
            string name = string.Empty;
            this.regions.TryGetValue(region, out name);
            return name;
        }
        public int GetRegionId(int cluster)
        {
            var region = 0;
            this.clusters.TryGetValue(cluster, out region);
            return region;
        }

        public Dictionary<int, string> GetRegions()
        {
            return this.regions;
        }

        public List<Hero> GetHeroes()
        {
            return this.heroes;
        }

        public List<Hero> GetADHeroes()
        {
            return this.heroes.Where(_ => _.AbilityDraftEnabled == true).ToList();
        }

        public List<Hero> GetCMHeroes()
        {
            return this.heroes.Where(_ => _.CaptainsModeEnabled == true).ToList();
        }

        public List<Ability> GetSkills()
        {
            return this.abilities;
        }

        public List<Ability> GetAbilities()
        {
            return this.abilities.Where(_ => _.IsSkill == true).ToList();
        }

        public List<Ability> GetUltimates()
        {
            return this.abilities.Where(_ => _.IsUltimate == true).ToList();
        }

        public List<Talent> GetTalents()
        {
            return this.talents;
        }

        public List<Item> GetItems()
        {
            return this.items;
        }
    }
}
