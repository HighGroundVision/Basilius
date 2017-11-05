using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Basilius.Model
{
    public class Ability
    {
        public int ability_id { get; set; }
        public int hero_id { get; set; }
        public string key { get; set; }
        public string name { get; set; }

        public bool basic { get; set; }
        public bool ultimate { get; set; }
        public bool scepter { get; set; }
        public bool spell_immunity { get; set; }
        public bool spell_dispellable { get; set; }

        public List<string> behaviors { get; set; }
        public string target { get; set; }
        public List<string> target_types { get; set; }
        public string damage_type { get; set; }
        public List<double> cast_range { get; set; }
        public List<double> cast_point { get; set; }
        public List<double> channel_time { get; set; }
        public List<double> cooldown { get; set; }
        public List<double> manacost { get; set; }
        public List<double> duration { get; set; }
        public List<double> damage { get; set; }       
        public double support_value { get; set; }
        public int support_bonus { get; set; }

        // public List<string> keywords { get; set; }

        public Ability()
        {
            this.behaviors = new List<string>();
            this.cast_range = new List<double>();
            this.cast_point = new List<double>();
            this.channel_time = new List<double>();
            this.cooldown = new List<double>();
            this.duration = new List<double>();
            this.duration = new List<double>();
            this.damage = new List<double>();
            this.manacost = new List<double>();
            // this.keywords = new List<string>();
        }

    }
}
