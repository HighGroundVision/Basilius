﻿using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace HGV.Basilius.Tools.Collection
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();

            var jsonLangDota = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/dota_english.json");
            JObject rootLangDota = JObject.Parse(jsonLangDota);
            JObject dataLangDota = (JObject)rootLangDota["lang"]["Tokens"];

            var jsonLangAbilties = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/abilities_english.json");
            JObject rootLangAbilties = JObject.Parse(jsonLangAbilties);
            JObject dataLangAbilties = (JObject)rootLangAbilties["lang"]["Tokens"];

            var jsonActiveHeroes = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/activelist.json");
            JObject rootActiveHeroes = JObject.Parse(jsonActiveHeroes);
            JObject activeHeroes = (JObject)rootActiveHeroes["whitelist"];

            var jsonAbilities = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_abilities.json");
            JObject rootAbilities = JObject.Parse(jsonAbilities);
            JObject abiltiesData = (JObject)rootAbilities["DOTAAbilities"];

            var jsonHeroes = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_heroes.json");
            JObject rootHeroes = JObject.Parse(jsonHeroes);
            JObject heroesData = (JObject)rootHeroes["DOTAHeroes"];

            var jsonItems = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/items.json");
            JObject rootItems = JObject.Parse(jsonItems);
            JObject itemsData = (JObject)rootItems["DOTAAbilities"];

            var jsonRegions = await client.GetStringAsync("https://api.stratz.com/api/v1/Region");
            JArray rootRegions = JArray.Parse(jsonRegions);

            var jsonClusters = await client.GetStringAsync("https://api.stratz.com/api/v1/Cluster");
            JArray rootClusters = JArray.Parse(jsonClusters);

            var jsonModes = await client.GetStringAsync("https://api.stratz.com/api/v1/GameMode");
            JObject rootModes = JObject.Parse(jsonModes);

            var activeItems = new List<string>();
            foreach (JProperty property in itemsData.Properties())
            {
                if (property.Name == "Version")
                    continue;

                activeItems.Add(property.Name);
            }

            var items = new List<Item>();
            foreach (var key in activeItems)
            {
                var item = ExtractItem(dataLangAbilties, itemsData, key);
                items.Add(item);
            }

            var modes = rootModes.Properties()
                .Select(_ => new { id = rootModes[_.Name]["id"], name = rootModes[_.Name]["name"] })
                .ToDictionary(_ => _.id, _ => _.name);
            var outputModes = Newtonsoft.Json.JsonConvert.SerializeObject(modes, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("Modes.json", outputModes);

            var regions = rootRegions
                .Select(_ => new { id = (int)_["id"], name = (string)dataLangDota[((string)_["displayName"]).Replace("#", "")], latitude = (double)_["latitude"], longitude = (double)_["longitude"] })
                .ToList();

            regions.RemoveAll(_ => _.id == 0);
            var outputRegions = Newtonsoft.Json.JsonConvert.SerializeObject(regions, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("Regions.json", outputRegions);

            var clusters = rootClusters.ToDictionary(_ => (int)_["id"], _ => (int)_["regionId"]);
            var outputClusters = Newtonsoft.Json.JsonConvert.SerializeObject(clusters, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("Clusters.json", outputClusters);

            var outputItems = Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("Items.json", outputItems);

            var heroes = new List<Hero>();
            foreach (JProperty property in activeHeroes.Properties())
            {
                var hero = ExtractHeroData(dataLangDota, dataLangAbilties, abiltiesData, heroesData, property.Name);
                heroes.Add(hero);
            }

            var outputHeroes = Newtonsoft.Json.JsonConvert.SerializeObject(heroes, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("Heroes.json", outputHeroes);


        }

        private static Item ExtractItem(JObject languageData, JObject itemsData, string key)
        {
            var itemData = itemsData[key];

            var item = new Item();
            item.Id = (int)itemData["ID"];
            item.Key = key;

            var name = "DOTA_Tooltip_ability_" + key;
            var desc = "DOTA_Tooltip_ability_" + key + "_Description";
            item.Name = (string)languageData[name];
            item.Description = (string)languageData[desc];

            // Other Stuff...

            if (itemData["AbilitySpecial"] != null && itemData["AbilitySpecial"].Type == JTokenType.Array)
            {
                var items = (JArray)itemData["AbilitySpecial"];
                foreach (JObject speical in items)
                {
                    var property = speical.Properties().FirstOrDefault();
                    var values = new List<double>();

                    if (property.Value.Type == JTokenType.Float || property.Value.Type == JTokenType.Integer)
                    {
                        values.Add((double)property.Value);
                    }
                    else if (property.Value.Type == JTokenType.Array)
                    {
                        foreach (var valueItem in property.Value)
                        {
                            values.Add((double)valueItem);
                        }
                    }

                    if(item.AbilitySpecial.ContainsKey(property.Name) == false)
                    {
                        item.AbilitySpecial.Add(property.Name, values);
                    }
                }
            }

            if (item.Description != null)
            {
                foreach (var pair in item.AbilitySpecial)
                {
                    var identifier = "%" + pair.Key + "%";
                    var value = string.Join("/", pair.Value.ToArray());
                    item.Description = item.Description.Replace(identifier, value);
                }

                item.Description = item.Description.Replace("%%", "%");
            }

            return item;
        }

        private static Hero ExtractHeroData(JObject languageDota, JObject languageAbilties, JObject abiltiesData, JObject heroesData, string key)
        {
            var hero = new Hero();
            hero.Id = getValue<int>(heroesData, "npc_dota_hero_base", key, "HeroID");
            hero.Key = key;
            hero.Name = (string)languageDota[key] ?? (string)languageAbilties[key] ?? "Unknown";

            hero.Enabled = isTrue(heroesData, "npc_dota_hero_base", key, "Enabled");
            hero.NewPlayerEnable = isTrue(heroesData, "npc_dota_hero_base", key, "new_player_enable");
            hero.CaptainsModeEnabled = isTrue(heroesData, "npc_dota_hero_base", key, "CMEnabled");
            hero.AbilityDraftEnabled = !isTrue(heroesData, "npc_dota_hero_base", key, "AbilityDraftDisabled");
            hero.BotImplemented = isTrue(heroesData, "npc_dota_hero_base", key, "BotImplemented");

            hero.Complexity = getValue<int>(heroesData, "npc_dota_hero_base", key, "Complexity");
            var roles = getList<string>(heroesData, "npc_dota_hero_base", key, "Role", ',');
            var levels = getList<int>(heroesData, "npc_dota_hero_base", key, "Rolelevels", ',');
            if (roles.Count == levels.Count)
            {
                for (int i = 0; i < roles.Count; i++)
                {
                    var role = new RoleComplexity()
                    {
                        Role = roles[i],
                        Level = levels[i],
                    };
                    hero.Roles.Add(role);
                }
            }

            hero.ArmorPhysical = getValue<double>(heroesData, "npc_dota_hero_base", key, "ArmorPhysical");
            hero.MagicalResistance = getValue<int>(heroesData, "npc_dota_hero_base", key, "MagicalResistance");
            hero.AttackCapabilities = getValue<string>(heroesData, "npc_dota_hero_base", key, "AttackCapabilities");
            hero.AttackDamageMin = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackDamageMin");
            hero.AttackDamageMax = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackDamageMax");
            hero.AttackRate = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttackRate");
            hero.AttackAnimationPoint = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttackAnimationPoint");
            hero.AttackAcquisitionRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackAcquisitionRange");
            hero.AttackRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackRange");
            hero.ProjectileSpeed = getValue<int>(heroesData, "npc_dota_hero_base", key, "ProjectileSpeed");

            hero.AttributePrimary = getValue<string>(heroesData, "npc_dota_hero_base", key, "AttributePrimary");
            hero.AttributeBaseStrength = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttributeBaseStrength");
            hero.AttributeStrengthGain = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttributeStrengthGain");
            hero.AttributeBaseIntelligence = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttributeBaseIntelligence");
            hero.AttributeIntelligenceGain = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttributeIntelligenceGain");
            hero.AttributeBaseAgility = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttributeBaseAgility");
            hero.AttributeAgilityGain = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttributeAgilityGain");

            hero.StatusHealth = getValue<int>(heroesData, "npc_dota_hero_base", key, "StatusHealth");
            hero.StatusHealthRegen = getValue<double>(heroesData, "npc_dota_hero_base", key, "StatusHealthRegen");
            hero.StatusMana = getValue<int>(heroesData, "npc_dota_hero_base", key, "StatusMana");
            hero.StatusManaRegen = getValue<double>(heroesData, "npc_dota_hero_base", key, "StatusManaRegen");

            hero.BountyXP = getValue<int>(heroesData, "npc_dota_hero_base", key, "BountyXP");
            hero.BountyGoldMin = getValue<int>(heroesData, "npc_dota_hero_base", key, "BountyGoldMin");
            hero.BountyGoldMax = getValue<int>(heroesData, "npc_dota_hero_base", key, "BountyGoldMax");

            hero.MovementCapabilities = getValue<string>(heroesData, "npc_dota_hero_base", key, "MovementCapabilities");
            hero.MovementSpeed = getValue<int>(heroesData, "npc_dota_hero_base", key, "MovementSpeed");
            hero.MovementTurnRate = getValue<double>(heroesData, "npc_dota_hero_base", key, "MovementTurnRate");
            hero.HasAggressiveStance = isTrue(heroesData, "npc_dota_hero_base", key, "HasAggressiveStance");
            hero.Legs = getValue<int>(heroesData, "npc_dota_hero_base", key, "Legs");

            var team = getValue<string>(heroesData, "npc_dota_hero_base", key, "TeamName");
            hero.Team = team == "DOTA_TEAM_GOODGUYS" ? 1 : 2;
            hero.TeamName = team == "DOTA_TEAM_GOODGUYS" ? "Radiant" : "Dire";

            hero.VisionDaytimeRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "VisionDaytimeRange");
            hero.VisionNighttimeRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "VisionNighttimeRange");

            // Get Abilties
            var abilityTalentStart = getValue<int>(heroesData, "npc_dota_hero_base", key, "AbilityTalentStart");
            for (int i = 1; i < abilityTalentStart; i++)
            {
                var field = string.Format("Ability{0}", i);
                var abilityKey = getValue<string>(heroesData, "npc_dota_hero_base", key, field);

                if (string.IsNullOrWhiteSpace(abilityKey) == false)
                {
                    var ability = ExtractAbilityData(languageAbilties, abiltiesData, abilityKey);
                    ability.HeroId = hero.Id;
                    ability.Index = i;
                    hero.Abilities.Add(ability);
                }
            }

            foreach (var ability in hero.Abilities)
            {
                IsAbilityDrafEnabled(ability, hero, heroesData);
            }

            // Get Talents
            var x = 0;
            var talentsLevels = new List<int>() { 10, 10, 15, 15, 20, 20, 25, 25 };
            for (int i = abilityTalentStart; i <= 24; i++)
            {
                var field = string.Format("Ability{0}", i);
                var abilityKey = getValue<string>(heroesData, "npc_dota_hero_base", key, field);
                if (string.IsNullOrWhiteSpace(abilityKey) == false)
                {
                    var talent = ExtractTalent(languageAbilties, abiltiesData, abilityKey);
                    talent.HeroId = hero.Id;
                    talent.Level = talentsLevels[x++];
                    hero.Talents.Add(talent);
                }
            }

            return hero;
        }

        private static void IsAbilityDrafEnabled(Ability ability, Hero hero, JObject heroesData)
        {
            // Gruads
            if (hero.AbilityDraftEnabled == false)
            {
                ability.AbilityDraftEnabled = false;
                return;
            }

            // Gruads
            if (ability.Key == "generic_hidden")
            {
                ability.AbilityDraftEnabled = false;
                return;
            }

            var heroData = heroesData[hero.Key];

            if (heroData["AbilityDraftIgnoreCount"] != null)
            {
                if ((int)heroData["AbilityDraftIgnoreCount"] == ability.Index)
                {
                    ability.AbilityDraftEnabled = false;
                    return;
                }
            }

            if (ability.IsGrantedByScepter == true)
            {
                if(hero.Abilities.Any(_ => _.AbilityDraftUltScepterAbility == ability.Key))
                {
                    ability.AbilityDraftEnabled = true;
                    return;
                }
            }

            // Checks
            if (heroData["AbilityDraftAbilities"] != null)
            {
                foreach (var item in heroData["AbilityDraftAbilities"])
                {
                    if ((string)item == ability.Key)
                    {
                        ability.AbilityDraftEnabled = true;
                    }
                }
            }
            else
            {
                ability.AbilityDraftEnabled = true;
            }
        }

        private static Talent ExtractTalent(JObject languageAbilties, JObject abiltiesData, string abilityKey)
        {
            var talent = new Talent();
            talent.Id = getValue<int>(abiltiesData, "ability_base", abilityKey, "ID");
            talent.Key = abilityKey;

            var name = "DOTA_Tooltip_ability_" + abilityKey;
            talent.Description = (string)languageAbilties[name];

            if(talent.Description.Contains("{s:value}"))
            {
                var value = getSpecialValue(abiltiesData, abilityKey);
                talent.Description = talent.Description.Replace("{s:value}", value);
            }
            
            return talent;
        }

        private static Ability ExtractAbilityData(JObject languageData, JObject abiltiesData, string key)
        {
            var abilityData = abiltiesData[key];

            var ability = new Ability();
            ability.Id = getValue<int>(abiltiesData, "ability_base", key, "ID");
            ability.Key = key;

            var name = "DOTA_Tooltip_ability_" + key;
            var desc = "DOTA_Tooltip_ability_" + key + "_Description";
            var desc_ags = "DOTA_Tooltip_ability_" + key + "_aghanim_description";

            ability.Name = (string)languageData[name];
            ability.Description = (string)languageData[desc];
            ability.UpgradeDescription = (string)languageData[desc_ags];

            ability.AbilityType = getValue<string>(abiltiesData, "ability_base", key, "AbilityType");
            ability.AbilityBehaviors = getList<string>(abiltiesData, "ability_base", key, "AbilityBehavior", '|');
            ability.IsSkill = ability.AbilityBehaviors.Contains("DOTA_ABILITY_BEHAVIOR_HIDDEN") == false && ability.AbilityType.Contains("DOTA_ABILITY_TYPE_ULTIMATE") == false;
            ability.IsUltimate = ability.AbilityBehaviors.Contains("DOTA_ABILITY_BEHAVIOR_HIDDEN") == false && ability.AbilityType.Contains("DOTA_ABILITY_TYPE_ULTIMATE") == true;
            ability.AbilityUnitTargetTeam = getValue<string>(abiltiesData, "ability_base", key, "AbilityUnitTargetTeam");
            ability.AbilityUnitTargetType = getList<string>(abiltiesData, "ability_base", key, "AbilityUnitTargetType", '|');
            ability.AbilityUnitTargetFlags = getList<string>(abiltiesData, "ability_base", key, "AbilityUnitTargetFlags");
            ability.SpellImmunityType = getValue<string>(abiltiesData, "ability_base", key, "SpellImmunityType");
            ability.SpellDispellableType = getValue<string>(abiltiesData, "ability_base", key, "SpellDispellableType");
            ability.AbilityUnitDamageType = getValue<string>(abiltiesData, "ability_base", key, "AbilityUnitDamageType");

            ability.HasScepterUpgrade = isTrue(abiltiesData, "ability_base", key, "HasScepterUpgrade");
            ability.IsGrantedByScepter = isTrue(abiltiesData, "ability_base", key, "IsGrantedByScepter");
            ability.AbilityDraftPreAbility = getValue<string>(abiltiesData, "ability_base", key, "AbilityDraftPreAbility");
            ability.AbilityDraftUltScepterAbility = getValue<string>(abiltiesData, "ability_base", key, "AbilityDraftUltScepterAbility");

            ability.OnCastbar = isTrue(abiltiesData, "ability_base", key, "OnCastbar");
            ability.OnLearnbar = isTrue(abiltiesData, "ability_base", key, "OnLearnbar");

            ability.AbilityCastRange = getList<int>(abiltiesData, "ability_base", key, "AbilityCastRange");
            ability.AbilityCastRangeBuffer = getValue<int>(abiltiesData, "ability_base", key, "AbilityCastRangeBuffer");

            ability.AbilityCastPoint = getList<double>(abiltiesData, "ability_base", key, "AbilityCastPoint");
            ability.AbilityChannelTime = getList<double>(abiltiesData, "ability_base", key, "AbilityChannelTime");
            ability.AbilityCooldown = getList<double>(abiltiesData, "ability_base", key, "AbilityCooldown");
            ability.AbilityDuration = getList<double>(abiltiesData, "ability_base", key, "AbilityDuration");
            ability.AbilityDamage = getList<double>(abiltiesData, "ability_base", key, "AbilityDamage");
            ability.AbilityManaCost = getList<double>(abiltiesData, "ability_base", key, "AbilityManaCost");

            ability.AbilityModifierSupportValue = getValue<double>(abiltiesData, "ability_base", key, "AbilityModifierSupportValue");
            ability.AbilityModifierSupportBonus = getValue<double>(abiltiesData, "ability_base", key, "AbilityModifierSupportBonus");

            ability.FightRecapLevel = getValue<int>(abiltiesData, "ability_base", key, "FightRecapLevel");

            ability.AbilityDraftEnabled = false;
            ability.Linked = getValue<string>(abiltiesData, "ability_base", key, "LinkedAbility") ?? getValue<string>(abiltiesData, "ability_base", key, "AbilityDraftPreAbility");

            if (abilityData["AbilitySpecial"] != null && abilityData["AbilitySpecial"].Type == JTokenType.Array)
            {
                var items = (JArray)abilityData["AbilitySpecial"];
                foreach (JObject item in items)
                {
                    var property = item.Properties().SingleOrDefault();
                    var values = new List<double>();

                    if (property.Value.Type == JTokenType.Float || property.Value.Type == JTokenType.Integer) {
                        values.Add((double)property.Value);
                    } else if (property.Value.Type == JTokenType.Array) {
                        foreach (var valueItem in property.Value)
                        {
                            values.Add((double)valueItem);
                        }
                    }

                    if (ability.AbilitySpecial.ContainsKey(property.Name) == false)
                    {
                        ability.AbilitySpecial.Add(property.Name, values);
                    }
                }
            }

            if(ability.Description != null)
            {
                foreach (var pair in ability.AbilitySpecial)
                {
                    var identifier = "%" + pair.Key + "%";
                    var value = string.Join("/", pair.Value.ToArray());
                    ability.Description = ability.Description.Replace(identifier, value);
                }

                ability.Description = ability.Description.Replace("\\n\\n", " ").Replace("%%", "%");
            }

            return ability;
        }

        static T getValue<T>(JObject data, string master, string key, string field) 
        {
            var baseData = data[master];
            var itemData = data[key];

            if(itemData[field] != null)
            {
                return itemData[field].Value<T>();
            }
            else if (baseData[field] != null)
            {
                return baseData[field].Value<T>();
            }
            else
            {
                return default(T);
            }
        }

        static string getSpecialValue(JObject data, string key)
        {
            var itemData = data[key];

            if (itemData["AbilitySpecial"] != null)
            {
                return itemData["AbilitySpecial"].ToArray().FirstOrDefault()["value"].Value<string>();
            }
            else
            {
                return string.Empty;
            }
        }

        static List<T> getList<T>(JObject data, string master, string key, string field, char delimator = ' ')
        {
            var baseData = data[master];
            var itemData = data[key];

            if (itemData[field] != null)
            {
                var value = (string)itemData[field];
                var items = value.Split(delimator, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToList();
                var list = items.Select(_ => (T)Convert.ChangeType(_, typeof(T))).ToList();
                return list;
            }
            else if (baseData[field] != null)
            {
                var value = (string)baseData[field];
                var items = value.Split(delimator, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToList();
                var list = items.Select(_ => (T)Convert.ChangeType(_, typeof(T))).ToList();

                var defaultValue = default(T);
                if (list.All(_ => EqualityComparer<T>.Default.Equals(_, defaultValue)) == true)
                {
                    return new List<T>();
                }
                else
                {
                    return list;
                }                
            }
            else
            {
                return new List<T>();
            }
        }

        static bool isTrue(JObject data, string master, string key, string field)
        {
            var baseData = data[master];
            var itemData = data[key];

            if (itemData[field] != null)
            {
                return (int)itemData[field] == 1;
            }
            else if (baseData[field] != null)
            {
                return (int)baseData[field] == 1;
            }
            else
            {
                return false;
            }
        }
    }
}
