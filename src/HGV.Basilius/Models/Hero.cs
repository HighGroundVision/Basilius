using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{
    public class RoleComplexity
    {
        public string Role { get; set; }
        public int Level { get; set; }
    }

    public class BotSettings
    {
        public List<string> HeroTypes { get; set; }
        public int SoloDesire { get; set; }
        public int RequiresBabysit { get; set; }
        public int ProvidesBabysit { get; set; }
        public int SurvivalRating { get; set; }
        public int RequiresFarm { get; set; }
        public int ProvidesSetup { get; set; }
        public int RequiresSetup { get; set; }
    }

    public class Hero
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Key { get; set; }

        public string ImageProfile { get { return string.Format("https://hgv-hyperstone.azurewebsites.net/heroes/banner/{0}.png", this.Key); } }
        public string ImageIcon { get { return string.Format("https://hgv-hyperstone.azurewebsites.net/heroes/icons/{0}.png", this.Key); } }
        public string ImageBanner { get { return string.Format("https://hgv-hyperstone.azurewebsites.net/heroes/profile/{0}.png", this.Key); } }

        public bool Enabled { get; set; }
        public bool NewPlayerEnable { get; set; }
        public bool CaptainsModeEnabled { get; set; }
        public bool AbilityDraftEnabled { get; set; }

        public int Complexity { get; set; }
        public List<RoleComplexity> Roles { get; set; }

        public bool BotImplemented { get; set; }

        public List<Ability> Abilities { get; set; }
        public List<Talent> Talents { get; set; }
        //public List<string> AbilityDraftAbilities { get; set; } // Move to Ability Details

        public int Legs { get; set; }
        public double ArmorPhysical { get; set; }
        public int MagicalResistance { get; set; }

        public string AttackCapabilities { get; set; }     // DOTA_UNIT_CAP_RANGED_ATTACK | DOTA_UNIT_CAP_MELEE_ATTACK
        public int AttackDamageMin { get; set; }
        public int AttackDamageMax { get; set; }
        public double AttackRate { get; set; }
        public double AttackAnimationPoint { get; set; }
        public int AttackAcquisitionRange { get; set; }
        public int AttackRange { get; set; }
        public int ProjectileSpeed { get; set; }

        public string AttributePrimary { get; set; }       // DOTA_ATTRIBUTE_STRENGTH | DOTA_ATTRIBUTE_AGILITY | DOTA_ATTRIBUTE_INTELLECT
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

        public string MovementCapabilities { get; set; }   // DOTA_UNIT_CAP_MOVE_GROUND
        public int MovementSpeed { get; set; }
        public double MovementTurnRate { get; set; }
        public bool HasAggressiveStance { get; set; }

        public int Team { get; set; }
        public string TeamName { get; set; }            // DOTA_TEAM_GOODGUYS | DOTA_TEAM_BADGUYS

        public int VisionDaytimeRange { get; set; }
        public int VisionNighttimeRange { get; set; }

        public BotSettings Bot { get; set; }
        

        public Hero()
        {
            this.Roles = new List<RoleComplexity>();
            this.Abilities = new List<Ability>();
            this.Talents = new List<Talent>();
            this.Bot = new BotSettings();
        }
    }
}
