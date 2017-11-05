using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using HGV.Basilius.Model;


namespace HGV.Basilius.Clients
{
    public class ItemMetaClient
    {
        public async Task<List<Item>> GetItems()
        {
            var items = new List<Item>();


            using (var client = new HttpClient())
            {
                var npc_items_json = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/items.json");
                var npc_items_root = JObject.Parse(npc_items_json);
                var items_collection = npc_items_root["DOTAAbilities"].ToList();

                var npc_skills_json = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_abilities.json");
                var npc_skills_root = JObject.Parse(npc_skills_json);
                var skill_defaults = npc_skills_root["DOTAAbilities"]["ability_base"];

                var item_data_json = await client.GetStringAsync("https://www.dota2.com/jsfeed/heropediadata?feeds=itemdata&l=english");
                var item_data_root = JObject.Parse(item_data_json);
                var item_data = item_data_root["itemdata"];

                foreach (JProperty i in items_collection)
                {
                    var key = i.Name;
                    if (key == "Version")
                        continue;
                    if (key == "item_river_painter")
                        continue;
                    if (key == "item_river_painter2")
                        continue;
                    if (key == "item_river_painter3")
                        continue;
                    if (key == "item_river_painter4")
                        continue;
                    if (key == "item_river_painter5")
                        continue;
                    if (key == "item_river_painter6")
                        continue;
                    if (key == "item_river_painter7")
                        continue;

                    var tag = key.Replace("item_", "");
                    var x = item_data[tag];
                    if (x == null)
                        continue;

                    // Create hero
                    var item = new Item();
                    item.key = key;
                    item.tag = tag;
                    item.name = (string)x["dname"];

                    // Set defaults
                    MapItem(item, skill_defaults);

                    // Map Hero details
                    var data = (JToken)i.First;
                    MapItem(item, data);

                    items.Add(item);
                }
            }

            return items;
        }

        private void MapItem(Item item, JToken item_data)
        {
            if (item_data["ID"] != null)
            {
                item.item_id = (int)item_data["ID"];
            }

            if (item_data["AbilityCastRange"] != null)
            {
                var value = (string)item_data["AbilityCastRange"];
                item.cast_range = value.Split(' ').Select(_ => int.Parse(_)).ToList();
            }

            if (item_data["AbilityCooldown"] != null)
            {
                var value = (string)item_data["AbilityCooldown"];
                item.cooldown = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (item_data["ItemKillable"] != null)
            {
                item.destructible = (int)item_data["ItemKillable"] == 1;
            }

            if (item_data["ItemDisassemblable"] != null)
            {
                item.disassemblable = (int)item_data["ItemDisassemblable"] == 1;
            }

            if (item_data["ItemDroppable"] != null)
            {
                item.droppable = (int)item_data["ItemDroppable"] == 1;
            }

            if (item_data["ItemInitialCharges"] != null)
            {
                item.initial_charges = (int)item_data["ItemInitialCharges"];
            }

            if (item_data["AbilityManaCost"] != null)
            {
                var value = (string)item_data["AbilityManaCost"];
                item.manacost = value.Split(' ').Select(_ => int.Parse(_)).ToList();
            }

            if (item_data["ItemPermanent"] != null)
            {
                item.permanent = (int)item_data["ItemPermanent"] == 1;
            }

            if (item_data["ItemCost"] != null)
            {
                item.price = (int)item_data["ItemCost"];
            }

            if (item_data["ItemPurchasable"] != null)
            {
                item.purchasable = (int)item_data["ItemPurchasable"] == 1;
            }

            if (item_data["ItemQuality"] != null)
            {
                item.quality = (string)item_data["ItemQuality"];
            }

            if (item_data["ItemRecipe"] != null)
            {
                item.recipe = (int)item_data["ItemRecipe"] == 1;
            }

            if (item_data["ItemRequiresCharges"] != null)
            {
                item.requires_charges = (int)item_data["ItemRequiresCharges"] == 1;
            }

            if (item_data["SecretShop"] != null)
            {
                item.require_secret_shop = (int)item_data["SecretShop"] == 1;
            }

            if (item_data["SideShop"] != null)
            {
                item.require_side_shop = (int)item_data["SideShop"] == 1;
            }

            if (item_data["ItemSellable"] != null)
            {
                item.sellable = (int)item_data["ItemSellable"] == 1;
            }

            if (item_data["ItemStackable"] != null)
            {
                item.stackable = (int)item_data["ItemStackable"] == 1;
            }

            if (item_data["AbilityBehavior"] != null)
            {
                var ability_behavior = (string)item_data["AbilityBehavior"];

                switch (ability_behavior)
                {
                    case "DOTA_ABILITY_BEHAVIOR_NONE":
                        break;
                    default:
                        var behavior = ability_behavior.Replace("DOTA_ABILITY_BEHAVIOR_", "").Replace(" ", "").Replace("_", " ").ToLower();
                        item.behaviors = behavior.Split('|').ToList();
                        break;
                }
            }
        }
    }
}
