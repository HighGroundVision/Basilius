﻿using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using HGV.Basilius.Contants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HGV.Basilius.Tools.Collection;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Enter Stratz Token");
    var stratzToken = Console.ReadLine();

    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stratzToken);

    var clusters = await ExtractCluster(client);
    var regions = await ExtractRegions(client, clusters);
    var modes = await ExtractGameModes(client);

    client.DefaultRequestHeaders.Clear();

    if (Enum.GetValues(typeof(Contants.ServerRegion)).Length != regions.Count)
      throw new InvalidOperationException("ServerRegion Enumeration needs to be updated");

    if (Enum.GetValues(typeof(Contants.GameMode)).Length != modes.Count)
      throw new InvalidOperationException("GameMode Enumeration needs to be updated");


    var rootLanaguage = await ExtractRootLanaguage(client);
    var abilityLanaguage = await ExtractAbilityLanaguage(client);
    var keys = await GetHeroes(client);
    var abilities = await ExtractAbilities(client, keys, abilityLanaguage);
    var heroes = await ExtractHeroes(client, rootLanaguage, abilities);
    var items = await ExtractItems(client, abilityLanaguage);

    var formatting = Formatting.Indented;
    File.WriteAllText("Regions.json", JsonConvert.SerializeObject(regions, formatting));
    File.WriteAllText("Clusters.json", JsonConvert.SerializeObject(clusters, formatting));
    File.WriteAllText("Modes.json", JsonConvert.SerializeObject(modes, formatting));
    File.WriteAllText("Abilities.json", JsonConvert.SerializeObject(abilities, formatting));
    File.WriteAllText("Heroes.json", JsonConvert.SerializeObject(heroes, formatting));
    File.WriteAllText("Items.json", JsonConvert.SerializeObject(items, formatting));
  }

  #region Lanaguage
  

  private static async Task<Dictionary<string, string>> ExtractAbilityLanaguage(HttpClient client)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };
    var options = new VdfSerializerSettings()
    {
      UsesEscapeSequences = true,
    };

    var vfd = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/abilities_english.txt");
    var root = VdfConvert.Deserialize(vfd, options);
    var json = root.Value.ToJson(settings);

    JObject tokens = (JObject)json["Tokens"];
    var collection = tokens.Properties().ToDictionary(_ => _.Name.ToUpper(), _ => (string)_.Value);
    return collection;

    //JObject root = await GetJson(client, "https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/abilities_english.txt");
    //JObject tokens = (JObject)root["Tokens"];
    //var collection = tokens.Properties().ToDictionary(_ => _.Name.ToUpper(), _ => (string)_.Value);
    //return collection;
  }


  private static async Task<Dictionary<string, string>> ExtractRootLanaguage(HttpClient client)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace,
    };
    var options = new VdfSerializerSettings()
    {
      UsesEscapeSequences = true,
      MaximumTokenSize = int.MaxValue/2,
    };

    var vfd = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/dota_english.txt");
    var root = VdfConvert.Deserialize(vfd, options);
    var json = root.Value.ToJson(settings);

    JObject tokens = (JObject)json["Tokens"];
    var collection = tokens.Properties().ToDictionary(_ => _.Name.ToUpper(), _ => (string)_.Value);
    return collection;

    //JObject root = await GetJson(client, "https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/resource/localization/dota_english.txt");
    //JObject tokens = (JObject)root["Tokens"];
    //var collection = tokens.Properties().ToDictionary(_ => _.Name.ToUpper(), _ => (string)_.Value);
    //return collection;
  }

  #endregion

  #region Game Modes
  private static async Task<Dictionary<int, string>> ExtractGameModes(HttpClient client)
  {
    var json = await client.GetStringAsync("https://api.stratz.com/api/v1/GameMode");
    JObject root = JObject.Parse(json);

    var modes = root.Properties()
        .Select(_ => _.Value)
        .Cast<JObject>()
        .Select(_ => new
        {
          Id = (int)_["id"],
          Name = (string)_["name"],
        })
        .ToDictionary(_ => _.Id, _ => _.Name);

    return modes;
  }

  #endregion

  #region Regions

  private static async Task<List<Region>> ExtractRegions(HttpClient client, Dictionary<int, int> clusters)
  {
    var json = await client.GetStringAsync("https://api.stratz.com/api/v1/Region");
    JArray root = JArray.Parse(json);

    var regions = root.Select(_ => new
    {
      Id = (int)_["id"],
      Key = (string)_["name"] ?? "Unknown",
      Name = (string)_["clientName"] ?? "Unknown",
      Group = (string)_["leaderboardDivision"] ?? "other",
      Latitude = (double)_["latitude"],
      Longitude = (double)_["longitude"],
    }).Select(_ => new Region()
    {
      Id = _.Id,
      Key = _.Key,
      Name = _.Name,
      Group = _.Group,
      Latitude = _.Latitude,
      Longitude = _.Longitude,
      Clusters = clusters.Where(x => x.Value == _.Id).Select(_ => _.Key).ToList()
    }).ToList();

    return regions;
  }

  #endregion

  #region Cluster
  private static async Task<Dictionary<int, int>> ExtractCluster(HttpClient client)
  {
    var json = await client.GetStringAsync("https://api.stratz.com/api/v1/Cluster");
    JArray root = JArray.Parse(json);

    var clusters = root.Select(_ => new
    {
      clusterId = (int)_["id"],
      regionId = (int)_["regionId"],
    }).ToList();

    var collection = clusters.ToDictionary(_ => _.clusterId, _ => _.regionId);
    return collection;
  }

  #endregion

  #region Heroes

  private static async Task<List<string>> GetHeroes(HttpClient client)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };
    var options = new VdfSerializerSettings()
    {
      UsesEscapeSequences = true,
    };

    var vfd = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/activelist.txt");
    var root = VdfConvert.Deserialize(vfd, options);
    var heroesData = root.Value.ToJson(settings) as JObject;

    var heroes = new List<string>();
    foreach (JProperty property in heroesData.Properties())
    {
      heroes.Add(property.Name);
    }

    return heroes;
  }

  private static async Task<Dictionary<int, double>> GetHeroWinrate(HttpClient client)
  {
    var data = new Dictionary<int, double>();

    var jsonHeroes = await client.GetStringAsync("https://windrun.io/api/heroes");
    JObject rootHeroes = JObject.Parse(jsonHeroes);
    var key = rootHeroes["data"]["patches"]["overall"][0].ToString();
    var stats = rootHeroes["data"]["heroStats"] as JObject;

    foreach (var item in stats.Properties())
    {
      var id = int.Parse(item.Name);
      var obj = item.Value as JObject;
      var patch = obj[key] as JObject;
      var value = patch["winrate"].Value<double>();
      data.Add(id, value);
    }

    return data;
  }

  private static async Task<List<Hero>> ExtractHeroes(HttpClient client, Dictionary<string, string> language, List<Ability> abilities)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };
    var options = new VdfSerializerSettings()
    {
      UsesEscapeSequences = true,
    };

    var vfd = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_heroes.txt");
    var root = VdfConvert.Deserialize(vfd, options);
    var heroesData = root.Value.ToJson(settings) as JObject;
    //JObject heroesData = (JObject)json["DOTAHeroes"];

    //var jsonHeroes = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_heroes.json");
    //JObject rootHeroes = JObject.Parse(jsonHeroes);
    //JObject heroesData = (JObject)rootHeroes["DOTAHeroes"];

    var activeHeroes = new List<string>();
    foreach (JProperty property in heroesData.Properties())
    {
      if (property.Name == "Version")
        continue;

      if (property.Name == "npc_dota_hero_base")
        continue;

      activeHeroes.Add(property.Name);
    }

    var winrates = await GetHeroWinrate(client);
    var heroes = new List<Hero>();
    foreach (var key in activeHeroes)
    {
      var hero = ExtractHeroData(language, abilities, heroesData, key);
      winrates.TryGetValue(hero.Id, out double winrate);
      hero.Winrate = winrate;
      heroes.Add(hero);
    }

    return heroes;
  }

  private static Hero ExtractHeroData(Dictionary<string, string> language, List<Ability> abilities, JObject heroesData, string key)
  {
    var hero = new Hero();
    hero.Id = getValue<int>(heroesData, "npc_dota_hero_base", key, "HeroID");
    hero.Key = key;

    var workshop_guide_name = getValue<string>(heroesData, "npc_dota_hero_base", key, "workshop_guide_name");
    if (language.TryGetValue(key.ToUpper(), out string hero_name))
      hero.Name = hero_name;
    else if (workshop_guide_name != null)
      hero.Name = workshop_guide_name;
    else
      hero.Name = "Unknown";

    hero.NameAliases = getList<string>(heroesData, "npc_dota_hero_base", key, "NameAliases", ';');

    var img = hero.Id;
    hero.ImageProfile = $"https://hyperstone.highgroundvision.com/images/heroes/profile/{img}.png";
    hero.ImageBanner = $"https://hyperstone.highgroundvision.com/images/heroes/banner/{img}.png";
    hero.ImageIcon = $"https://hyperstone.highgroundvision.com/images/heroes/icon/{img}.png";
    hero.ImagePortrait = $"https://hyperstone.highgroundvision.com/images/heroes/portrait/{img}.png";
    hero.Animation = $"https://hyperstone.highgroundvision.com/images/heroes/animation/{img}.webm";

    hero.Enabled = isTrue(heroesData, "npc_dota_hero_base", key, "Enabled");
    hero.NewPlayerEnable = hero.Enabled && isTrue(heroesData, "npc_dota_hero_base", key, "new_player_enable");
    hero.CaptainsModeEnabled = hero.Enabled && isTrue(heroesData, "npc_dota_hero_base", key, "CMEnabled");
    hero.AbilityDraftEnabled = hero.Enabled && !isTrue(heroesData, "npc_dota_hero_base", key, "AbilityDraftDisabled");

    hero.Complexity = getValue<int>(heroesData, "npc_dota_hero_base", key, "Complexity");
    var roles = getList<string>(heroesData, "npc_dota_hero_base", key, "Role", ',');
    hero.Roles.AddRange(roles);

    hero.ArmorPhysical = getValue<double>(heroesData, "npc_dota_hero_base", key, "ArmorPhysical");
    hero.MagicalResistance = getValue<int>(heroesData, "npc_dota_hero_base", key, "MagicalResistance");
    var attack = getValue<string>(heroesData, "npc_dota_hero_base", key, "AttackCapabilities");
    hero.AttackCapabilities = attack == "DOTA_UNIT_CAP_RANGED_ATTACK" ? "RANGED" : "MELEE";
    hero.AttackDamageMin = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackDamageMin");
    hero.AttackDamageMax = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackDamageMax");
    hero.AttackRate = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttackRate");
    hero.AttackAnimationPoint = getValue<double>(heroesData, "npc_dota_hero_base", key, "AttackAnimationPoint");
    hero.AttackAcquisitionRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackAcquisitionRange");
    hero.AttackRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "AttackRange");
    hero.ProjectileSpeed = getValue<int>(heroesData, "npc_dota_hero_base", key, "ProjectileSpeed");

    var primary = getValue<string>(heroesData, "npc_dota_hero_base", key, "AttributePrimary");
    hero.AttributePrimary = primary.Replace("DOTA_ATTRIBUTE_", "");
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

    hero.MovementSpeed = getValue<int>(heroesData, "npc_dota_hero_base", key, "MovementSpeed");
    hero.MovementTurnRate = getValue<double>(heroesData, "npc_dota_hero_base", key, "MovementTurnRate");
    hero.HasAggressiveStance = isTrue(heroesData, "npc_dota_hero_base", key, "HasAggressiveStance");
    hero.Legs = getValue<int>(heroesData, "npc_dota_hero_base", key, "Legs");

    var team = getValue<string>(heroesData, "npc_dota_hero_base", key, "TeamName");
    hero.Team = team == "DOTA_TEAM_GOODGUYS" ? Team.Radiant : Team.Dire;

    hero.VisionDaytimeRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "VisionDaytimeRange");
    hero.VisionNighttimeRange = getValue<int>(heroesData, "npc_dota_hero_base", key, "VisionNighttimeRange");

    var abilityTalentStart = getValue<int>(heroesData, "npc_dota_hero_base", key, "AbilityTalentStart");


    for (int i = 1; i < abilityTalentStart; i++)
    {
      var ability_key = getValue<string>(heroesData, "npc_dota_hero_base", key, "Ability" + i);
      if (string.IsNullOrWhiteSpace(ability_key))
        continue;

      var ability = abilities.Find(_ => _.Key == ability_key);
      if (ability == null)
        continue;

      ability.HeroId = hero.Id;

      hero.Abilities.Add(ability.Id);
    }

    var levels = new Queue<int>(new List<int>() { 10, 10, 15, 15, 20, 20, 25, 25 });
    for (int i = abilityTalentStart; i <= 24; i++)
    {
      var ability_key = getValue<string>(heroesData, "npc_dota_hero_base", key, "Ability" + i);
      if (string.IsNullOrWhiteSpace(ability_key))
        continue;

      var ability = abilities.Find(_ => _.Key == ability_key);
      if (ability == null)
        continue;

      levels.TryDequeue(out int lvl);

      var talent = new Talent()
      {
        Id = ability.Id,
        Key = ability_key,
        Name = ability.Name,
        Level = lvl,
      };

      hero.Talents.Add(talent);
    }

    var abilityDraftIgnoreCount = hero.Id == 20 ? 0 : getValue<int>(heroesData, "npc_dota_hero_base", key, "AbilityDraftIgnoreCount");
    var abilityDraftAbilities = heroesData[key]["AbilityDraftAbilities"];
    if (abilityDraftAbilities != null)
    {
      foreach (JProperty item in abilityDraftAbilities)
      {
        var ability_key = (string)item.Value;

        var ability = abilities.Find(_ => _.Key == ability_key);
        if (ability == null)
          continue;

        ability.HeroId = hero.Id;

        hero.AbilityDraftPool.Add(ability.Id);
      }
    }
    else
    {
      for (int i = 0; i < hero.Abilities.Count; i++)
      {
        var count = (i + 1);

        if (abilityDraftIgnoreCount != 0 && count == abilityDraftIgnoreCount)
          continue;

        var id = hero.Abilities[i];

        var ability = abilities.Find(_ => _.Id == id);
        if (ability == null)
          continue;

        if (ability.Key == "generic_hidden")
          continue;

        if (ability.AbilityBehaviors.Contains(AbilityBehaviors.HIDDEN))
          continue;

        if (ability.AbilityBehaviors.Contains(AbilityBehaviors.NOT_LEARNABLE))
          continue;

        if (ability.IsGrantedByScepter == true)
          continue;

        if (ability.IsGrantedByShard == true)
          continue;

        hero.AbilityDraftPool.Add(id);
      }
    }

    var collection = abilities.Join(hero.AbilityDraftPool, _ => _.Id, _ => _, (lhs, rhs) => lhs).ToList();
    hero.AbilityReplaceRequired = !collection.Any(_ => _.IsUltimate) || collection.Count < 4;

    return hero;
  }

  #endregion

  #region Items

  private static async Task<List<Item>> ExtractItems(HttpClient client, Dictionary<string, string> language)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };
    var options = new VdfSerializerSettings()
    {
      UsesEscapeSequences = true,
    };


    var vfd_abilities = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_abilities.txt");
    var abilitiesData = VdfConvert.Deserialize(vfd_abilities, options).Value.ToJson(settings) as JObject;
    //JObject abilitiesData = (JObject)rootAbilities["DOTAAbilities"];

    //var jsonAbilities = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_abilities.json");
    //JObject rootAbilities = JObject.Parse(jsonAbilities);
    //JObject abilitiesData = (JObject)rootAbilities["DOTAAbilities"];

    var vfd_items = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/items.txt");
    var itemsData = VdfConvert.Deserialize(vfd_items, options).Value.ToJson(settings) as JObject;
    // JObject itemsData = (JObject)rootItems["DOTAAbilities"];
    itemsData.Add("ability_base", abilitiesData["ability_base"]);

    //var jsonItems = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/items.json");
    //JObject rootItems = JObject.Parse(jsonItems);
    //JObject itemsData = (JObject)rootItems["DOTAAbilities"];
    //itemsData.Add("ability_base", abilitiesData["ability_base"]);

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
      var item = ExtractItem(language, itemsData, key);
      items.Add(item);
    }

    return items;
  }

  private static Item ExtractItem(Dictionary<string, string> language, JObject itemsData, string key)
  {
    var itemData = itemsData[key];

    var item = new Item();
    item.Id = getValue<int>(itemsData, "ability_base", key, "ID");
    item.Key = key;

    var name = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper();
    if (language.TryGetValue(name, out string item_name))
      item.Name = item_name;

    var desc = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper() + "_DESCRIPTION";
    if (language.TryGetValue(desc, out string item_desc))
      item.Description = item_desc;

    var img = item.Id;
    item.Image = $"https://hyperstone.highgroundvision.com/images/items/{img}.png";

    item.ItemAliases = getList<string>(itemsData, "ability_base", key, "ItemAliases", ';');
    item.ItemCost = getValue<int>(itemsData, "ability_base", key, "ItemCost");
    item.ItemInitialCharges = getValue<int>(itemsData, "ability_base", key, "ItemInitialCharges");
    item.ItemCombinable = isTrue(itemsData, "ability_base", key, "ItemCombinable");
    item.ItemPermanent = isTrue(itemsData, "ability_base", key, "ItemPermanent");
    item.ItemStackable = isTrue(itemsData, "ability_base", key, "ItemStackable");
    item.ItemRecipe = isTrue(itemsData, "ability_base", key, "ItemRecipe");
    item.ItemDroppable = isTrue(itemsData, "ability_base", key, "ItemDroppable");
    item.ItemPurchasable = isTrue(itemsData, "ability_base", key, "ItemPurchasable");
    item.ItemSellable = isTrue(itemsData, "ability_base", key, "ItemSellable");
    item.ItemRequiresCharges = isTrue(itemsData, "ability_base", key, "ItemRequiresCharges");
    item.ItemKillable = isTrue(itemsData, "ability_base", key, "ItemKillable");
    item.ItemDisassemblable = isTrue(itemsData, "ability_base", key, "ItemDisassemblable");
    item.ItemDeclaresPurchase = isTrue(itemsData, "ability_base", key, "ItemDeclaresPurchase");
    item.ItemIsNeutralDrop = isTrue(itemsData, "ability_base", key, "ItemIsNeutralDrop");
    item.ItemShareability = getValue<string>(itemsData, "ability_base", key, "ItemShareability");
    item.ItemSupport = isTrue(itemsData, "ability_base", key, "ItemSupport");
    item.ItemContributesToNetWorthWhenDropped = isTrue(itemsData, "ability_base", key, "ItemContributesToNetWorthWhenDropped");
    item.ItemStockMax = getValue<int>(itemsData, "ability_base", key, "ItemStockMax");
    item.ItemStockTime = getValue<double>(itemsData, "ability_base", key, "ItemStockTime");
    item.ItemQuality = getValue<string>(itemsData, "ability_base", key, "ItemQuality");
    item.ItemShopTags = getList<string>(itemsData, "ability_base", key, "ItemShopTags", ';');
    item.AllowedInBackpack = isTrue(itemsData, "ability_base", key, "AllowedInBackpack");
    item.IsObsolete = isTrue(itemsData, "ability_base", key, "IsObsolete");
    item.AbilityUnitTargetType = getList<string>(itemsData, "ability_base", key, "AbilityUnitTargetType", '|');
    item.AbilityUnitTargetTeam = getValue<string>(itemsData, "ability_base", key, "AbilityUnitTargetTeam");
    item.AbilityBehavior = getList<string>(itemsData, "ability_base", key, "AbilityBehavior", '|');
    item.AbilityCastRange = getList<double>(itemsData, "ability_base", key, "AbilityCastRange");
    item.AbilityCastPoint = getList<double>(itemsData, "ability_base", key, "AbilityCastPoint");
    item.AbilityCooldown = getList<double>(itemsData, "ability_base", key, "AbilityCooldown");
    item.AbilityManaCost = getList<double>(itemsData, "ability_base", key, "AbilityManaCost");
    item.AbilityChannelTime = getList<double>(itemsData, "ability_base", key, "AbilityChannelTime");

    if (itemData["AbilitySpecial"] != null && itemData["AbilitySpecial"].Type == JTokenType.Array)
    {
      var items = (JArray)itemData["AbilitySpecial"];
      foreach (JObject special in items)
      {
        var property = special.Properties().FirstOrDefault();
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

        if (item.AbilitySpecial.ContainsKey(property.Name) == false)
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

      item.Description = item.Description.Replace("%abilitychanneltime%", string.Join('/', item.AbilityChannelTime));
      item.Description = item.Description.Replace("%%", "%");
    }

    return item;
  }

  #endregion

  #region Abilities

  public record AbilityStat(int AbilityId, double Winrate, double PickRate, double AvgPickPosition, double PickPosStdDev);

  private static async Task<List<AbilityStat>> GetAbilityWinrate(HttpClient client)
  {
    var data = new List<AbilityStat>();

    var jsonAbilities = await client.GetStringAsync("https://windrun.io/api/abilities");
    JObject rootAbilities = JObject.Parse(jsonAbilities);
    var stats = rootAbilities["data"]["abilityStats"] as JArray;

    foreach (var item in stats)
    {
      var id = item["abilityId"].Value<int>();
      var winrate = item["winrate"].Value<double>();
      var pickrate = item["pickRate"].Value<double>();
      var avgPickPosition = (item["avgPickPosition"].Type == JTokenType.Null) ? 0 : item["avgPickPosition"].Value<double>();
      var pickPosStdDev = (item["pickPosStdDev"].Type == JTokenType.Null) ? 0 : item["pickPosStdDev"].Value<double>();
      data.Add(new AbilityStat(id, winrate, pickrate, avgPickPosition, pickPosStdDev));
    }

    return data;
  }


  private static async Task<List<Ability>> ExtractAbilities(HttpClient client, List<string> heroes, Dictionary<string, string> language)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };

    var identities = await GetAbilityIdentities(client);
    var abilities = new List<Ability>();
    var stats = await GetAbilityWinrate(client);

    foreach (var hero in heroes)
    {
      var vfd = await client.GetStringAsync($"https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/heroes/{hero}.txt");
      var root = VdfConvert.Deserialize(vfd).ToJson(settings);
      var data = root.Value as JObject;

      foreach (JProperty property in data.Properties())
      {
        if (property.Name == "Version")
          continue;

        var key = property.Name;
        var obj = property.Value as JObject;
        var ability = ExtractAbility(language, identities, obj, key);

        var stat = stats.Find(_ => _.AbilityId == ability.Id);
        if(stat is not null)
        {
          ability.Winrate = stat.Winrate;
          ability.AvgPickPosition = stat.AvgPickPosition;
          ability.PickPosStdDev = stat.PickPosStdDev;
          ability.PickRate = stat.PickRate;
        }

        abilities.Add(ability);

      }

      await Task.Delay(100);
    }

    return abilities;
  }

  private static async Task<Dictionary<string, int>> GetAbilityIdentities(HttpClient client)
  {
    var settings = new VdfJsonConversionSettings()
    {
      ObjectDuplicateKeyHandling = DuplicateKeyHandling.Replace,
      ValueDuplicateKeyHandling = DuplicateKeyHandling.Replace
    };

    var vfd = await client.GetStringAsync("https://raw.githubusercontent.com/dotabuff/d2vpkr/master/dota/scripts/npc/npc_ability_ids.txt");
    var abilities_idenities = VdfConvert.Deserialize(vfd);
    var root = abilities_idenities.ToJson(settings);

    var data = (JObject)root.Value;
    var abilities = (JObject)data["UnitAbilities"];
    var locked = (JObject)abilities["Locked"];

    var identities = new Dictionary<string, int>();
    foreach (JProperty property in locked.Properties())
    {
      var value = property.Value.ToString();
      identities.Add(property.Name, int.Parse(value));
    }

    return identities;
  }

  private static Ability ExtractAbility(Dictionary<string, string> language, Dictionary<string, int> identities, JObject abiltiesData, string key)
  {
    // var abilityData = abiltiesData[key];

    var ability = new Ability();
    ability.Id = identities[key];
    ability.Key = key;

    var name = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper();
    var desc = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper() + "_DESCRIPTION";
    var desc_ags = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper() + "_AGHANIM_DESCRIPTION";
    var ad_note = "DOTA_TOOLTIP_ABILITY_" + key.ToUpper() + "_ABILITYDRAFT_NOTE";

    if (language.TryGetValue(name, out string ability_name))
      ability.Name = ability_name;

    if (language.TryGetValue(desc, out string ability_desc))
      ability.Description = ability_desc;

    if (language.TryGetValue(desc_ags, out string ability_ags_desc))
      ability.UpgradeDescription = ability_ags_desc;

    if (language.TryGetValue(ad_note, out string ability_ad_note))
      ability.AbilityDraftNote = ability_ad_note;


    ability.AbilityType = getValue<string>(abiltiesData, "AbilityType", "DOTA_ABILITY_TYPE_BASIC");
    ability.IsAttribute = ability.AbilityType.Contains("DOTA_ABILITY_TYPE_ATTRIBUTES");
    ability.IsSkill = ability.AbilityType.Contains("DOTA_ABILITY_TYPE_BASIC");
    ability.IsUltimate = ability.AbilityType.Contains("DOTA_ABILITY_TYPE_ULTIMATE");

    var img = ability.Id;
    if (ability.IsSkill || ability.IsUltimate)
      ability.Image = $"https://hyperstone.highgroundvision.com/images/abilities/{img}.png";
    else
      ability.Image = $"https://hyperstone.highgroundvision.com/images/abilities/5002.png";

    ability.AbilityBehaviors = getList<string>(abiltiesData, "AbilityBehavior", '|');
    ability.IsPassive = ability.AbilityBehaviors.Contains(AbilityBehaviors.PASSIVE);
    ability.IsChannelled = ability.AbilityBehaviors.Contains(AbilityBehaviors.CHANNELLED);
    ability.NoTarget = ability.AbilityBehaviors.Contains(AbilityBehaviors.NO_TARGET);
    ability.PointTarget = ability.AbilityBehaviors.Contains(AbilityBehaviors.POINT);
    ability.VectorTargeting = ability.AbilityBehaviors.Contains(AbilityBehaviors.VECTOR_TARGETING);
    ability.IsAOE = ability.AbilityBehaviors.Contains(AbilityBehaviors.AOE);
    ability.IsToggle = ability.AbilityBehaviors.Contains(AbilityBehaviors.TOGGLE);
    ability.IsAutocast = ability.AbilityBehaviors.Contains(AbilityBehaviors.TOGGLE);
    ability.IsDirectional = ability.AbilityBehaviors.Contains(AbilityBehaviors.AUTOCAST);
    ability.IsAura = ability.AbilityBehaviors.Contains(AbilityBehaviors.AURA);
    ability.IsAttack = ability.AbilityBehaviors.Contains(AbilityBehaviors.ATTACK);
    ability.DoesntCancelChannel = ability.AbilityBehaviors.Contains(AbilityBehaviors.DONT_CANCEL_CHANNEL);
    ability.DisabledByRoot = ability.AbilityBehaviors.Contains(AbilityBehaviors.ROOT_DISABLES);

    ability.AbilityUnitTargetTeam = getValue<string>(abiltiesData, "AbilityUnitTargetTeam", string.Empty);
    // getValue<string>(abiltiesData, "ability_base", key, "AbilityUnitTargetTeam");
    ability.AbilityUnitTargetType = getList<string>(abiltiesData, "AbilityUnitTargetType", '|');
    ability.AbilityUnitTargetFlags = getList<string>(abiltiesData, "AbilityUnitTargetFlags");
    ability.SpellImmunityType = getValue<string>(abiltiesData, "SpellImmunityType");
    ability.SpellDispellableType = getValue<string>(abiltiesData, "SpellDispellableType");
    ability.AbilityUnitDamageType = getValue<string>(abiltiesData, "AbilityUnitDamageType");

    ability.IsGrantedByShard = isTrue(abiltiesData, "IsGrantedByShard");
    ability.HasShardUpgrade = ability.IsGrantedByShard == true ? false : isTrue(abiltiesData, "HasShardUpgrade");

    ability.IsGrantedByScepter = isTrue(abiltiesData, "IsGrantedByScepter");
    ability.HasScepterUpgrade = ability.IsGrantedByScepter == true ? false : isTrue(abiltiesData, "HasScepterUpgrade");

    ability.AbilityDraftPreAbility = getValue<string>(abiltiesData, "AbilityDraftPreAbility");
    ability.AbilityDraftUltScepterAbility = getValue<string>(abiltiesData, "AbilityDraftUltScepterAbility");

    ability.OnCastbar = isTrue(abiltiesData, "OnCastbar", true);
    ability.OnLearnbar = isTrue(abiltiesData, "OnLearnbar", true);

    ability.AbilityCastRange = getList<int>(abiltiesData, "AbilityCastRange");
    ability.AbilityCastRangeBuffer = getValue<int>(abiltiesData, "AbilityCastRangeBuffer");

    ability.AbilityCastPoint = getList<double>(abiltiesData, "AbilityCastPoint");
    ability.AbilityChannelTime = getList<double>(abiltiesData, "AbilityChannelTime");
    ability.AbilityCooldown = getList<double>(abiltiesData, "AbilityCooldown");
    ability.AbilityDuration = getList<double>(abiltiesData, "AbilityDuration");
    ability.AbilityDamage = getList<double>(abiltiesData, "AbilityDamage");
    ability.AbilityManaCost = getList<double>(abiltiesData, "AbilityManaCost");

    ability.AbilityModifierSupportValue = getValue<double>(abiltiesData, "AbilityModifierSupportValue");
    ability.AbilityModifierSupportBonus = getValue<double>(abiltiesData, "AbilityModifierSupportBonus");

    ability.Linked = getValue<string>(abiltiesData, "LinkedAbility") ?? getValue<string>(abiltiesData, "AbilityDraftPreAbility");

    if (abiltiesData["AbilitySpecial"] is not null && abiltiesData["AbilitySpecial"].Type == JTokenType.Array)
    {
      var items = (JArray)abiltiesData["AbilitySpecial"];
      foreach (JObject item in items)
      {
        var property = item.Properties().SingleOrDefault();
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

        if (ability.AbilitySpecial.ContainsKey(property.Name) == false)
        {
          ability.AbilitySpecial.Add(property.Name, values);
        }
      }
    }

    if (ability.Description != null)
    {
      foreach (var pair in ability.AbilitySpecial)
      {
        var identifier = "%" + pair.Key + "%";
        var value = string.Join("/", pair.Value.ToArray());
        ability.Description = ability.Description.Replace(identifier, value);
      }

      ability.Description = ability.Description.Replace("\\n\\n", " ").Replace("%%", "%");
    }

    if (ability.Name != null)
    {
      foreach (var pair in ability.AbilitySpecial)
      {
        var value = string.Join("/", pair.Value.ToArray());
        ability.Name = ability.Name.Replace("{s:" + pair.Key + "}", value);
      }
    }

    return ability;

  }

  #endregion

  #region Helpers

  static T getValue<T>(JObject data, string field, T value = default)
  {
    try
    {
      if (data[field] is null)
        return value;
      else
        return data[field].Value<T>();
    }
    catch (Exception)
    {
      return default(T);
    }
  }

  static List<T> getList<T>(JObject data, string field, char delimator = ' ')
  {
    if (data[field] is not null)
    {
      var value = (string)data[field];
      var items = value.Split(delimator, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToList();
      var list = items.Select(_ => (T)Convert.ChangeType(_, typeof(T))).ToList();
      return list;
    }
    else
    {
      return new List<T>();
    }
  }


  static T getValue<T>(JObject data, string master, string key, string field)
  {
    try
    {
      var baseData = data[master];
      var itemData = data[key];

      if (itemData[field] != null)
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
    catch (Exception)
    {
      return default(T);
    }
  }

  static bool isTrue(JObject data, string field, bool value = false)
  {
    if (data[field] is not null)
    {
      return (int)data[field] == 1;
    }
    else
    {
      return value;
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

  #endregion
}
