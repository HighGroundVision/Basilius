using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Basilius.Model
{
    public class Hero
    {
        public int hero_id { get; set; }
        public string key { get; set; }
        public string tag { get; set; }
        public string name { get; set; }

        public bool enabled { get; set; }
        public bool captains_mode_enabled { get; set; }
        public bool tournament_enabled { get; set; }
        public bool ability_draft_enabled { get; set; }

        public string roles { get; set; }
        public int complexity { get; set; }

        public double armor { get; set; }
        public double magical_resistance { get; set; }

        public int attack_capabilities { get; set; }         // DOTA_UNIT_CAP_MELEE_ATTACK or DOTA_UNIT_CAP_RANGED_ATTACK
        public int attack_damage_min { get; set; }
        public int attack_damage_max { get; set; }
        public double attack_rate { get; set; }
        public double attack_animation_point { get; set; }
        public int attack_acquisition_range { get; set; }
        public int attack_range { get; set; }
        public int attack_projectile_speed { get; set; }

        public int attribute_primary { get; set; }          // DOTA_ATTRIBUTE_STRENGT or DOTA_ATTRIBUTE_AGILITY or DOTA_ATTRIBUTE_INTELLECT
        public int attribute_base_strength { get; set; }
        public double attribute_strength_gain { get; set; }
        public int attribute_base_intelligence { get; set; }
        public double attribute_intelligence_gain { get; set; }
        public int attribute_base_agility { get; set; }
        public double attribute_agility_gain { get; set; }

        public int bounty_xp { get; set; }
        public int bounty_gold_min { get; set; }
        public int bounty_gold_max { get; set; }

        public int movement_speed { get; set; }
        public double movement_turn_rate { get; set; }

        public int status_health { get; set; }
        public double status_health_regen { get; set; }
        public int status_mana { get; set; }
        public double status_mana_regen { get; set; }

        public int vision_daytime_range { get; set; }
        public int vision_nighttime_range { get; set; }

        public List<Ability> abilities { get; set; }
        public List<Talenet> talenets { get; set; }

        public Hero()
        {
            this.abilities = new List<Ability>();
            this.talenets = new List<Talenet>();
        }
    }
}
