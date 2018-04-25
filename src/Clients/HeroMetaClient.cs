using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using HGV.Basilius.Model;


namespace HGV.Basilius.Clients
{
    public class HeroMetaClient
    {
        public async Task<List<Hero>> GetHeroes()
        {
            var heroes = new List<Hero>();
            var skills = new List<Ability>();

            using (var client = new HttpClient())
            {
                var npc_heroes_json = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_heroes.json");
                var npc_heroes_root = JObject.Parse(npc_heroes_json);
                var hero_collection = npc_heroes_root["DOTAHeroes"].ToList();
                var hero_defaults = npc_heroes_root["DOTAHeroes"]["npc_dota_hero_base"];

                var npc_skills_json = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_abilities.json");
                var npc_skills_root = JObject.Parse(npc_skills_json);
                var skill_collection = npc_skills_root["DOTAAbilities"].ToList();
                var skill_defaults = npc_skills_root["DOTAAbilities"]["ability_base"];

                var lang_data_json = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/dota_english.json");
                var lang_data_root = JObject.Parse(lang_data_json);
                var lang_data = lang_data_root["lang"]["Tokens"];

                foreach (JProperty item in skill_collection)
                {
                    var key = item.Name;
                    if (key == "Version")
                        continue;
                    if (key == "ability_base")
                        continue;
                    if (key == "default_attack")
                        continue;
                    if (key == "attribute_bonus")
                        continue;
                    if (key == "ability_deward")
                        continue;

                    var ability = new Ability();

                    var langKey = "DOTA_Tooltip_ability_" + key;
                    var name = lang_data[langKey];
                    if (name == null)
                        continue;

                    ability.name = name.Value<string>();
                    ability.key = key;

                    // Set defaults
                    MapAbility(ability, skill_defaults);

                    // Map Ability details
                    var data = (JToken)item.First;
                    MapAbility(ability, data);

                    skills.Add(ability);
                }

                foreach (JProperty item in hero_collection)
                {
                    var key = item.Name;
                    if (key == "Version")
                        continue;
                    if (key == "npc_dota_hero_base")
                        continue;
                    if (key == "npc_dota_hero_target_dummy")
                        continue;

                    var name = lang_data[key];
                    if (name == null)
                        continue;

                    // Create hero
                    var hero = new Hero();
                    hero.key = key;
                    hero.tag = key.Replace("npc_dota_hero_", "");
                    hero.name = name.Value<string>();

                    // Set defaults
                    MapHero(hero, hero_defaults);

                    // Map Hero details
                    var data = (JToken)item.First;
                    MapHero(hero, data);

                    MapHeroAbilties(hero, data, skills);
                    MapHeroTalenets(hero, data, skills);

                    heroes.Add(hero);
                }
            }

            return heroes;
        }

        private void MapHero(Hero hero, JToken hero_data)
        {
            if (hero_data["HeroID"] != null)
            {
                hero.hero_id = (int)hero_data["HeroID"];
            }

            if (hero_data["Enabled"] != null)
            {
                hero.enabled = (int)hero_data["Enabled"] == 1;
            }
            if (hero_data["CMEnabled"] != null)
            {
                hero.captains_mode_enabled = (int)hero_data["CMEnabled"] == 1;
            }

            hero.tournament_enabled = hero_data["CMTournamentIgnore"] == null;
            hero.ability_draft_enabled = hero_data["AbilityDraftDisabled"] == null;

            if (hero_data["Role"] != null)
            {
                hero.roles = (string)hero_data["Role"];
            }
            if (hero_data["Complexity"] != null)
            {
                hero.complexity = (int)hero_data["Complexity"];
            }

            if (hero_data["ArmorPhysical"] != null)
            {
                hero.armor = (double)hero_data["ArmorPhysical"];
            }
            if (hero_data["MagicalResistance"] != null)
            {
                hero.magical_resistance = (double)hero_data["MagicalResistance"];
            }
            if (hero_data["AttackCapabilities"] != null)
            {
                var attack_capabilities = (string)hero_data["AttackCapabilities"];
                hero.attack_capabilities = attack_capabilities == "DOTA_UNIT_CAP_MELEE_ATTACK" ? 1 : attack_capabilities == "DOTA_UNIT_CAP_RANGED_ATTACK" ? 2 : 0;
            }
            if (hero_data["AttackDamageMin"] != null)
            {
                hero.attack_damage_max = (int)hero_data["AttackDamageMin"];
            }
            if (hero_data["AttackDamageMax"] != null)
            {
                hero.attack_damage_min = (int)hero_data["AttackDamageMax"];
            }
            if (hero_data["AttackRate"] != null)
            {
                hero.attack_rate = (double)hero_data["AttackRate"];
            }
            if (hero_data["AttackAnimationPoint"] != null)
            {
                hero.attack_animation_point = (double)hero_data["AttackAnimationPoint"];
            }
            if (hero_data["AttackAcquisitionRange"] != null)
            {
                hero.attack_acquisition_range = (int)hero_data["AttackAcquisitionRange"];
            }
            if (hero_data["AttackRange"] != null)
            {
                hero.attack_range = (int)hero_data["AttackRange"];
            }
            if (hero_data["ProjectileSpeed"] != null)
            {
                hero.attack_projectile_speed = (int)hero_data["ProjectileSpeed"];
            }
            if (hero_data["AttributePrimary"] != null)
            {
                var attribute_primary = (string)hero_data["AttributePrimary"];
                hero.attribute_primary = attribute_primary == "DOTA_ATTRIBUTE_STRENGTH" ? 1 : attribute_primary == "DOTA_ATTRIBUTE_AGILITY" ? 2 : attribute_primary == "DOTA_ATTRIBUTE_INTELLECT" ? 3 : 0;
            }
            if (hero_data["AttributeBaseStrength"] != null)
            {
                hero.attribute_base_strength = (int)hero_data["AttributeBaseStrength"];
            }
            if (hero_data["AttributeStrengthGain"] != null)
            {
                hero.attribute_strength_gain = (double)hero_data["AttributeStrengthGain"];
            }
            if (hero_data["AttributeBaseIntelligence"] != null)
            {
                hero.attribute_base_intelligence = (int)hero_data["AttributeBaseIntelligence"];
            }
            if (hero_data["AttributeIntelligenceGain"] != null)
            {
                hero.attribute_intelligence_gain = (double)hero_data["AttributeIntelligenceGain"];
            }
            if (hero_data["AttributeBaseAgility"] != null)
            {
                hero.attribute_base_agility = (int)hero_data["AttributeBaseAgility"];
            }
            if (hero_data["AttributeAgilityGain"] != null)
            {
                hero.attribute_agility_gain = (double)hero_data["AttributeAgilityGain"];
            }
            if (hero_data["BountyXP"] != null)
            {
                hero.bounty_xp = (int)hero_data["BountyXP"];
            }
            if (hero_data["BountyGoldMin"] != null)
            {
                hero.bounty_gold_min = (int)hero_data["BountyGoldMin"];
            }
            if (hero_data["BountyGoldMax"] != null)
            {
                hero.bounty_gold_max = (int)hero_data["BountyGoldMax"];
            }
            if (hero_data["MovementSpeed"] != null)
            {
                hero.movement_speed = (int)hero_data["MovementSpeed"];
            }
            if (hero_data["MovementTurnRate"] != null)
            {
                hero.movement_turn_rate = (double)hero_data["MovementTurnRate"];
            }
            if (hero_data["StatusHealth"] != null)
            {
                hero.status_health = (int)hero_data["StatusHealth"];
            }
            if (hero_data["StatusHealthRegen"] != null)
            {
                hero.status_health_regen = (double)hero_data["StatusHealthRegen"];
            }
            if (hero_data["StatusMana"] != null)
            {
                hero.status_mana = (int)hero_data["StatusMana"];
            }
            if (hero_data["StatusManaRegen"] != null)
            {
                hero.status_mana_regen = (double)hero_data["StatusManaRegen"];
            }
            if (hero_data["VisionDaytimeRange"] != null)
            {
                hero.vision_daytime_range = (int)hero_data["VisionDaytimeRange"];
            }
            if (hero_data["VisionNighttimeRange"] != null)
            {
                hero.vision_nighttime_range = (int)hero_data["VisionNighttimeRange"];
            }
        }

        private void MapHeroAbilties(Hero hero, JToken hero_data, List<Ability> skills)
        {
            var hero_id = (int)hero_data["HeroID"];

            if (hero_data["Ability1"] != null)
            {
                var key = (string)hero_data["Ability1"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability2"] != null)
            {
                var key = (string)hero_data["Ability2"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability3"] != null)
            {
                var key = (string)hero_data["Ability3"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability4"] != null)
            {
                var key = (string)hero_data["Ability4"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability5"] != null)
            {
                var key = (string)hero_data["Ability5"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability6"] != null)
            {
                var key = (string)hero_data["Ability6"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability7"] != null)
            {
                var key = (string)hero_data["Ability7"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability8"] != null)
            {
                var key = (string)hero_data["Ability8"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability9"] != null)
            {
                var key = (string)hero_data["Ability9"];
                var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                if (ability != null)
                {
                    ability.hero_id = hero_id;
                    hero.abilities.Add(ability);
                }
            }
            if (hero_data["Ability10"] != null)
            {
                var key = (string)hero_data["Ability10"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability11"] != null)
            {
                var key = (string)hero_data["Ability11"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability12"] != null)
            {
                var key = (string)hero_data["Ability12"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability13"] != null)
            {
                var key = (string)hero_data["Ability13"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability14"] != null)
            {
                var key = (string)hero_data["Ability14"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability15"] != null)
            {
                var key = (string)hero_data["Ability15"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
            if (hero_data["Ability16"] != null)
            {
                var key = (string)hero_data["Ability16"];
                if (key.Contains("special_bonus") == false)
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        ability.hero_id = hero_id;
                        hero.abilities.Add(ability);
                    }
                }
            }
        }

        private void MapHeroTalenets(Hero hero, JToken hero_data, List<Ability> skills)
        {
            var list = new List<int>() { 10, 10, 15, 15, 20, 20, 25, 25 };
            var levels = new Queue<int>(list);

            if (hero_data["Ability10"] != null)
            {
                var key = (string)hero_data["Ability10"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if(ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability11"] != null)
            {
                var key = (string)hero_data["Ability11"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability12"] != null)
            {
                var key = (string)hero_data["Ability12"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability13"] != null)
            {
                var key = (string)hero_data["Ability13"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability14"] != null)
            {
                var key = (string)hero_data["Ability14"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability15"] != null)
            {
                var key = (string)hero_data["Ability15"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability16"] != null)
            {
                var key = (string)hero_data["Ability16"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability17"] != null)
            {
                var key = (string)hero_data["Ability17"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability18"] != null)
            {
                var key = (string)hero_data["Ability18"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability19"] != null)
            {
                var key = (string)hero_data["Ability19"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability20"] != null)
            {
                var key = (string)hero_data["Ability20"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability21"] != null)
            {
                var key = (string)hero_data["Ability21"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability22"] != null)
            {
                var key = (string)hero_data["Ability22"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability23"] != null)
            {
                var key = (string)hero_data["Ability23"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
            if (hero_data["Ability24"] != null)
            {
                var key = (string)hero_data["Ability24"];
                if (key.Contains("special_bonus"))
                {
                    var ability = skills.Where(_ => _.key == key).FirstOrDefault();
                    if (ability != null)
                    {
                        var talenet = new Talenet() { ability_id = ability.ability_id, hero_id = hero.hero_id, key = ability.key, name = ability.name, level = levels.Dequeue() };
                        hero.talenets.Add(talenet);
                    }
                }
            }
        }

        private void MapAbility(Ability ability, JToken ability_data)
        {
            if (ability_data["ID"] != null)
            {
                ability.ability_id = (int)ability_data["ID"];
            }

            if (ability_data["AbilityType"] != null)
            {
                var type = (string)ability_data["AbilityType"];
                ability.basic = type == "DOTA_ABILITY_TYPE_BASIC";
                ability.ultimate = type == "DOTA_ABILITY_TYPE_ULTIMATE";
            }

            if (ability_data["HasScepterUpgrade"] != null)
            {
                ability.scepter = true;
            }

            if (ability_data["AbilityBehavior"] != null)
            {
                var ability_behavior = (string)ability_data["AbilityBehavior"];

                switch (ability_behavior)
                {
                    case "DOTA_ABILITY_BEHAVIOR_NONE":
                        break;
                    default:
                        var behavior = ability_behavior.Replace("DOTA_ABILITY_BEHAVIOR_", "").Replace(" ", "").Replace("_", " ").ToLower();
                        ability.behaviors = behavior.Split('|').ToList();
                        break;
                }
            }

            if (ability_data["AbilityUnitTargetTeam"] != null)
            {
                var target = (string)ability_data["AbilityUnitTargetTeam"];
                ability.target = target.Replace("DOTA_UNIT_TARGET_", "").Replace("_", " ").ToLower();
            }

            if (ability_data["AbilityUnitTargetType"] != null)
            {
                var target = (string)ability_data["AbilityUnitTargetType"];
                target = target.Replace("DOTA_UNIT_TARGET_", "").Replace(" ", "").Replace("_", " ").ToLower();
                ability.target_types = target.Split('|').ToList();
            }

            if (ability_data["AbilityUnitDamageType"] != null)
            {
                var damage_type = (string)ability_data["AbilityUnitDamageType"];
                ability.damage_type = damage_type.Replace("DAMAGE_TYPE", "").Replace("_", " ").ToLower();
            }

            if (ability_data["SpellImmunityType"] != null)
            {
                var target = (string)ability_data["SpellImmunityType"];
                ability.spell_immunity = target == "SPELL_IMMUNITY_ENEMIES_YES" ? true : false;
            }

            if (ability_data["SpellDispellableType"] != null)
            {
                var target = (string)ability_data["SpellDispellableType"];
                ability.spell_dispellable = target == "SPELL_DISPELLABLE_YES" ? true : false;
            }

            if (ability_data["AbilityCastRange"] != null)
            {
                var value = (string)ability_data["AbilityCastRange"];
                ability.cast_range = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityCastPoint"] != null)
            {
                var value = (string)ability_data["AbilityCastPoint"];
                ability.cast_point = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityChannelTime"] != null)
            {
                var value = (string)ability_data["AbilityChannelTime"];
                ability.channel_time = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityCooldown"] != null)
            {
                var value = (string)ability_data["AbilityCooldown"];
                ability.cooldown = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityManaCost"] != null)
            {
                var value = (string)ability_data["AbilityManaCost"];
                ability.manacost = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityDuration"] != null)
            {
                var value = (string)ability_data["AbilityDuration"];
                ability.duration = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityDamage"] != null)
            {
                var value = (string)ability_data["AbilityDamage"];
                ability.damage = value.Split(' ').Select(_ => double.Parse(_)).ToList();
            }

            if (ability_data["AbilityModifierSupportValue"] != null)
            {
                ability.support_value = (double)ability_data["AbilityModifierSupportValue"];
            }

            if (ability_data["AbilityModifierSupportBonus"] != null)
            {
                ability.support_bonus = (int)ability_data["AbilityModifierSupportBonus"];
            }

        }
    }
}
