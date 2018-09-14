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
        List<Hero> heroes { get; set; }
        List<Ability> abilities { get; set; }
        List<Item> items { get; set; }

        public MetaClient()
        {
            this.heroes = new List<Hero>();
            this.abilities = new List<Ability>();
            this.items = new List<Item>();

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

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Abilities.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.abilities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ability>>(json);
            }

            using (Stream stream = assembly.GetManifestResourceStream("HGV.Basilius.Data.Items.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                this.items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item>>(json);
            }
        }

        public List<Hero> GetHeroes()
        {
            return this.heroes;
        }

        public List<Hero> GetADHeroes()
        {
            var list = this.GetHeroes();
            return list.Where(_ => _.AbilityDraftEnabled == true).ToList();
        }

        public List<Hero> GetCMHeroes()
        {
            var list = this.GetHeroes();
            return list.Where(_ => _.CaptainsModeEnabled == true).ToList();
        }

        public List<Ability> GetAbilities()
        {
            return this.abilities;
        }

        public List<Ability> GetSkills()
        {
            var list = this.GetAbilities();
            return list.Where(_ => _.IsSkill == true).ToList();
        }

        public List<Ability> GetUltimates()
        {
            var list = this.GetAbilities();
            return list.Where(_ => _.IsUltimate == true).ToList();
        }

        public List<Item> GetItems()
        {
            return this.items;
        }
    }
}
