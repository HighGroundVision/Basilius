using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Basilius
{
    public class Item
    {
        public int Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }

        public List<string> AbilityUnitTargetType { get; set; } = new List<string>();
        public string AbilityUnitTargetTeam { get; set; }
        public List<string> AbilityBehavior { get; set; } = new List<string>();
        public List<double> AbilityCastRange { get; set; }
        public List<double> AbilityCastPoint { get; set; }
        public List<double> AbilityCooldown { get; set; }
        public List<double> AbilityManaCost { get; set; }
        public List<double> AbilityChannelTime { get; set; }

        public Dictionary<string, List<double>> AbilitySpecial { get; set; } = new Dictionary<string, List<double>>();

        public int ItemCost { get; set; }
        public bool ItemCombinable { get; set; }
        public bool ItemPermanent { get; set; }
        public bool ItemStackable { get; set; }
        public bool ItemRecipe { get; set; }
        public bool ItemDroppable { get; set; }
        public bool ItemPurchasable { get; set; }
        public bool ItemSellable { get; set; }
        public bool ItemRequiresCharges { get; set; }
        public bool ItemKillable { get; set; }
        public bool ItemDisassemblable { get; set; }
        public bool ItemDeclaresPurchase { get; set; }
        public bool ItemSupport { get; set; }
        public bool ItemContributesToNetWorthWhenDropped { get; set; }
        public bool AllowedInBackpack { get; set; }
        public bool IsObsolete { get; set; }

        public int ItemInitialCharges { get; set; }
        public int ItemStockMax { get; set; }
        public double ItemStockTime { get; set; }
        public bool ItemIsNeutralDrop { get; set; }

        public string ItemShareability { get; set; }
        public List<string> ItemShopTags { get; set; } =  new List<string>();
        public List<string> ItemAliases { get; set; } = new List<string>();
        public string ItemQuality { get; set; }
    }
}
