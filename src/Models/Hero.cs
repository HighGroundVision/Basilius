using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{

    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public List<string> NameAliases { get; set; } = new List<string>();      

        public string ImageProfile { get; set; }
        public string ImageIcon { get; set; }
        public string ImageBanner { get; set; }
        public string ImagePortrait { get; set; }

        public bool Enabled { get; set; }
        public bool NewPlayerEnable { get; set; }
        public bool CaptainsModeEnabled { get; set; }
        public bool AbilityDraftEnabled { get; set; }
        public bool AbilityReplaceRequired { get; set; }

        public int Complexity { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public int Legs { get; set; }
        public double ArmorPhysical { get; set; }
        public int MagicalResistance { get; set; }

        public string AttackCapabilities { get; set; }
        public int AttackDamageMin { get; set; }
        public int AttackDamageMax { get; set; }
        public double AttackRate { get; set; }
        public double AttackAnimationPoint { get; set; }
        public int AttackAcquisitionRange { get; set; }
        public int AttackRange { get; set; }
        public int ProjectileSpeed { get; set; }

        public string AttributePrimary { get; set; }
        public int AttributeBaseStrength { get; set; }
        public double AttributeStrengthGain { get; set; }
        public int AttributeBaseIntelligence { get; set; }
        public double AttributeIntelligenceGain { get; set; }
        public int AttributeBaseAgility { get; set; }
        public double AttributeAgilityGain { get; set; }

        public int StatusHealth { get; set; }
        public double StatusHealthRegen { get; set; }
        public int StatusMana { get; set; }
        public double StatusManaRegen { get; set; }

        public int BountyXP { get; set; }
        public int BountyGoldMin { get; set; }
        public int BountyGoldMax { get; set; }

        public int MovementSpeed { get; set; }
        public double MovementTurnRate { get; set; }
        public bool HasAggressiveStance { get; set; }
       
        public string Team { get; set; }

        public int VisionDaytimeRange { get; set; }
        public int VisionNighttimeRange { get; set; }

        public List<int> Abilities { get; set; } = new List<int>();
        public List<int> AbilityDraftPool { get; set; } = new List<int>();
        public List<Talent> Talents { get; set; } =  new List<Talent>();
    }
}
