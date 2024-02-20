using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{
  public class Ability
  {
    public int Id { get; set; }
    public string Key { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string UpgradeDescription { get; set; }

    public string Image { get; set; }

    public int HeroId { get; set; }

    public string AbilityType { get; set; }
    public bool IsAttribute { get; set; }
    public bool IsSkill { get; set; }
    public bool IsUltimate { get; set; }

    public List<string> AbilityBehaviors { get; set; }

    public bool IsToggle { get; set; }
    public bool IsPassive { get; set; }
    public bool IsChannelled { get; set; }
    public bool NoTarget { get; set; }
    public bool PointTarget { get; set; }
    public bool VectorTargeting { get; set; }
    public bool IsAttack { get; set; }
    public bool IsAOE { get; set; }
    public bool IsAutocast { get; set; }
    public bool IsDirectional { get; set; }
    public bool IsAura { get; set; }
    public bool DoesntCancelChannel { get; set; }
    public bool DisabledByRoot { get; set; }

    public string AbilityUnitTargetTeam { get; set; }
    public List<string> AbilityUnitTargetType { get; set; }
    public List<string> AbilityUnitTargetFlags { get; set; }

    public string SpellImmunityType { get; set; }
    public string SpellDispellableType { get; set; }
    public string AbilityUnitDamageType { get; set; }

    public bool HasScepterUpgrade { get; set; }
    public bool IsGrantedByScepter { get; set; }
    public bool HasShardUpgrade { get; set; }
    public bool IsGrantedByShard { get; set; }

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

    public string AbilityDraftPreAbility { get; set; }
    public string AbilityDraftUltScepterAbility { get; set; }
    public string AbilityDraftNote { get; set; }

    public string Linked { get; set; }

    public Dictionary<string, List<double>> AbilitySpecial { get; set; }
    public List<string> Keywords { get; set; }

    public double AvgPickPosition { get; set; }
    public double PickPosStdDev { get; set; }
    public double PickRate { get; set; }
    public double Winrate { get; set; }

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
      this.AbilityType = "DOTA_ABILITY_TYPE_BASIC";
      this.OnCastbar = true;
      this.OnLearnbar = true;
      this.AbilityCastRangeBuffer = 250;
      this.AbilityModifierSupportValue = 1.0;
    }
  }
}
