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
        List<Item> items { get; set; }
        List<Ability> abilities { get; set; }
        List<Talent> talents { get; set; }

        public MetaClient()
        {
            this.heroes = new List<Hero>();
            this.items = new List<Item>();
            this.abilities = new List<Ability>();
            this.talents = new List<Talent>();

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

            this.abilities = this.heroes.SelectMany(_ => _.Abilities).ToList();
            this.talents = this.heroes.SelectMany(_ => _.Talents).ToList();
        }

        public string GetModeName(int mode)
        {
            switch (mode)
            {
                case 0:
                    return "All Pick";
                case 1:
                    return "All Pick";
                case 2:
                    return "Captains Mode";
                case 3:
                    return "Random Draft";
                case 4:
                    return "Single Draft";
                case 5:
                    return "All Random";
                case 6:
                    return "Unknown";
                case 7:
                    return "Unknown";
                case 8:
                    return "Reverse Captains Mode";
                case 9:
                    return "Unknown";
                case 10:
                    return "Tutorial";
                case 11:
                    return "Mid Only";
                case 12:
                    return "Least Played";
                case 13:
                    return "Limited Heroes";
                case 14:
                    return "Unknown";
                case 15:
                    return "Custom Game Mode";
                case 16:
                    return "Captains Draft";
                case 17:
                    return "Unknown";
                case 18:
                    return "Ability Draft";
                case 19:
                    return "Event Game";
                case 20:
                    return "All Random Death Match";
                case 21:
                    return "Mid 1v1";
                case 22:
                    return "All Pick";
                case 23:
                    return "Turbo";
                case 24:
                    return "Mutation";
                default:
                    return "Unknown";
            }
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

        private List<Ability> GetSkills()
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
