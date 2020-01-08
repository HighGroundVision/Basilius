using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{
    public class Ability
    {
        public static int GENERIC = 6251;

        public int Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string UpgradeDescription { get; set; }

        public string Image { get { return string.Format("https://hgv-hyperstone.azurewebsites.net/abilities/{0}.png", this.Key); } }

        public int HeroId { get; set; }

        public string AbilityType { get; set; } // DOTA_ABILITY_TYPE_BASIC | DOTA_ABILITY_TYPE_ATTRIBUTES | DOTA_ABILITY_TYPE_ULTIMATE
        public List<string> AbilityBehaviors { get; set; } // DOTA_ABILITY_BEHAVIOR_NONE | DOTA_ABILITY_BEHAVIOR_NOT_LEARNABLE | DOTA_ABILITY_BEHAVIOR_NO_TARGET | DOTA_ABILITY_BEHAVIOR_IMMEDIATE | DOTA_ABILITY_BEHAVIOR_IGNORE_CHANNEL

        public string AbilityUnitTargetTeam { get; set; }
        public List<string> AbilityUnitTargetType { get; set; }
        public List<string> AbilityUnitTargetFlags { get; set; }

        public string SpellImmunityType { get; set; }
        public string SpellDispellableType { get; set; }
        public string AbilityUnitDamageType { get; set; }

        public bool IsSkill { get; set; }
        public bool IsUltimate { get; set; }
        public bool HasScepterUpgrade { get; set; }
        public bool IsGrantedByScepter { get; set; }

        public bool OnCastbar { get; set; }
        public bool OnLearnbar { get; set; }

        public List<int> AbilityCastRange { get; set; }
        public int AbilityCastRangeBuffer { get; set; }

        public List<double> AbilityCastPoint { get; set; }
        public List<double> AbilityChannelTime { get; set; }
        public List<double> AbilityCooldown { get; set; }
        public List<double> AbilityDuration { get; set; }
        public List<double> AbilityDamage { get; set; }
        public List<double> AbilityManaCost { get; set; }
        
        public double AbilityModifierSupportValue { get; set; }
        public double AbilityModifierSupportBonus { get; set; }

        public int FightRecapLevel { get; set; }

        public bool AbilityDraftEnabled { get; set; }

        public string Linked { get; set; }

        public Dictionary<string, List<double>> AbilitySpecial { get; set; }
        public List<string> Keywords { get; set; }

        public Ability()
        {
            this.AbilityBehaviors = new List<string>();
            this.AbilityUnitTargetType = new List<string>();
            this.AbilityUnitTargetFlags = new List<string>();
            this.AbilityCastRange = new List<int>();
            this.AbilityCastPoint = new List<double>();
            this.AbilityChannelTime = new List<double>();
            this.AbilityCooldown = new List<double>();
            this.AbilityDuration = new List<double>();
            this.AbilityDamage = new List<double>();
            this.AbilityManaCost = new List<double>();
            this.AbilitySpecial = new Dictionary<string, List<double>>();
            this.Keywords = new List<string>();
        }
    }
}
