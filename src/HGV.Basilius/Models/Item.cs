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

        public string Image { get { return string.Format("https://hgv-hyperstone.azurewebsites.net/items/{0}.png", this.Key); } }

        public List<string> AbilityUnitTargetType { get; set; }
        public string AbilityUnitTargetTeam { get; set; }
        public List<string> AbilityBehavior { get; set; }
        public double AbilityCastRange { get; set; }
        public double AbilityCastPoint { get; set; }
        public double AbilityCooldown { get; set; }
        public double AbilityManaCost { get; set; }
        public Dictionary<string, List<double>> AbilitySpecial { get; set; }

        public List<string> ItemDeclarations { get; set; }
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
        public bool SideShop { get; set; }
        public bool ShouldBeSuggested { get; set; }
        public bool ShouldBeInitiallySuggested { get; set; }
        public bool IsTempestDoubleClonable { get; set; }
        public bool AllowedInBackpack { get; set; }
        public bool PlayerSpecificCooldown { get; set; }
        public bool IsObsolete { get; set; }

        public int ItemInitialCharges { get; set; }
        public int ItemHideCharges { get; set; }
        public int ItemStockMax { get; set; }
        public double ItemStockTime { get; set; }

        public string ItemShareability { get; set; }
        public List<string> ItemShopTags { get; set; }
        public List<string> ItemAliases { get; set; }
        public string ItemQuality { get; set; }

        public Item()
        {
            this.AbilityUnitTargetType = new List<string>();
            this.AbilityBehavior = new List<string>();
            this.AbilitySpecial = new Dictionary<string, List<double>>();
            this.ItemDeclarations = new List<string>();
            this.ItemShopTags = new List<string>();
            this.ItemAliases = new List<string>();
        }
    }
}
