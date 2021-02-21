using System;
using Harmony;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.Entities;
using System.Collections.Generic;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Levels;
using Base.Core;
using UnityEngine;
using PhoenixPoint.Tactical.Entities;
using System.Linq;
using Base.Defs;
using PhoenixPoint.Tactical.Entities.Equipments;
using Base;
using PhoenixPoint.Tactical.Entities.Abilities;
using System.Diagnostics;

namespace AssortedAdjustments.Patches
{
    internal static class CustomRecruitGeneration
    {
        // Which methods do trigger customization at all. Check "PhoenixPoint.Geoscape.Core.FactionCharacterGenerator.GenerateUnit()"
        //
        // FactionCharacterGenerator.GenerateHavenRecruit
        // FactionCharacterGenerator.GenerateRandomUnit
        // RecruitEvacuatedOutcomeDef.ApplyOutcome
        // GeoEventChoiceOutcome.GenerateFactionReward
        // GeoLevelController.CreateCharacterFromTemplate
        // GeoPhoenixFaction.CreateInitialSquad
        // GeoscapeTutorial.InitSquad
        private static readonly string[] contextsForCustomPools = new string[] { "RecruitEvacuatedOutcomeDef.ApplyOutcome", "GeoEventChoiceOutcome.GenerateFactionReward", "GeoPhoenixFaction.CreateInitialSquad", "GeoscapeTutorial.InitSquad" };

        // Custom ability pools by class. All skills not in this list will get removed from the pool before skills are picked by the generator
        private static readonly Dictionary<string, List<string>> abilityPoolByTemplate = new Dictionary<string, List<string>>
        {
            { "Assault", new List<string> { "Cautious", "CloseQuartersSpecialist", "Brainiac", "Helpful", "Pitcher", "Resourceful", "SelfDefenseSpecialist", "Focused", "Thief", "GoodShot" } },
            { "Heavy", new List<string> { "BioChemist", "Crafty", "Cautious", "Brainiac", "Helpful", "Pitcher", "Reckless", "Resourceful", "Strongman", "GoodShot" } },
            { "Sniper", new List<string> { "Cautious", "Brainiac", "Helpful", "Pitcher", "Resourceful", "SelfDefenseSpecialist", "Focused", "Thief", "GoodShot" } },

            { "Berserker", new List<string> { "CloseQuartersSpecialist", "Brainiac", "Helpful", "Pitcher", "Reckless", "Resourceful", "SelfDefenseSpecialist", "Thief" } },
            { "Priest", new List<string> { "BioChemist", "Cautious", "Brainiac", "Helpful", "Pitcher", "Resourceful", "SelfDefenseSpecialist", "Focused", "Thief", "GoodShot" } },
            { "Technician", new List<string> { "Cautious", "Brainiac", "Helpful", "Pitcher", "Resourceful", "SelfDefenseSpecialist", "Thief", "GoodShot" } },
            { "Infiltrator", new List<string> { "Cautious", "Brainiac", "Helpful", "Pitcher", "Resourceful", "SelfDefenseSpecialist", "Focused", "Thief", "GoodShot" } },

            // Individualize Sophia, Jacob, Omar, Irina and Takeshi with an adequate personal skill pool
            { "Sophia", new List<string> { "Pitcher", "GoodShot", "Helpful", "Resourceful", "Brainiac", "Thief", "Focused" } },
            { "Jacob", new List<string> { "Pitcher", "Resourceful", "CloseQuartersSpecialist", "Brainiac", "Reckless", "Helpful", "GoodShot" } },
            { "Omar", new List<string> { "Resourceful", "Pitcher", "BioChemist", "Strongman", "Brainiac", "Crafty", "Helpful" } },
            { "Irina", new List<string> { "Brainiac", "Focused", "SelfDefenseSpecialist", "Pitcher", "Resourceful", "Thief", "Helpful" } },
            { "Takeshi", new List<string> { "Helpful", "Resourceful", "Pitcher", "Thief", "Cautious", "GoodShot", "Brainiac" } }
        };

        private static readonly Dictionary<string, List<int>> bonusStatsByTemplate = new Dictionary<string, List<int>>
        {
            { "Assault", new List<int> { 3, 3, 2 } },
            { "Heavy", new List<int> { 4, 3, 1 } },
            { "Sniper", new List<int> { 2, 4, 2 } },

            { "Berserker", new List<int> { 2, 2, 4 } },
            { "Priest", new List<int> { 2, 4, 2 } },
            { "Technician", new List<int> { 4, 2, 2 } },
            { "Infiltrator", new List<int> { 2, 3, 3 } }
        };

        // Add special abilities (from other classes, augmentations or even pandoran)
        private static readonly Dictionary<string, List<string>> customAbilitiesByTemplate;
        /*
        private static readonly Dictionary<string, List<string>> customAbilitiesByTemplate = new Dictionary<string, List<string>>
        {
            { "Assault", new List<string> { "ShadowStep" } },
            { "Heavy", new List<string> { "ExtremeFocus" } },
            { "Sniper", new List<string> { "ArmourBreak" } },

            { "Berserker", new List<string> { "Brawler" } },
            { "Priest", new List<string> { "Inspire" } },
            { "Technician", new List<string> { "ExpertHealer" } },
            { "Infiltrator", new List<string> { "MindSense" } }
        };
        */



        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<GameDifficultyLevelDef> gameDifficultyLevelDefDefs = defRepository.DefRepositoryDef.AllDefs.OfType<GameDifficultyLevelDef>().ToList();
            foreach (GameDifficultyLevelDef gdlDef in gameDifficultyLevelDefDefs)
            {
                gdlDef.RecruitsGenerationParams.HasArmor = AssortedAdjustments.Settings.RecruitGenerationHasArmor;
                gdlDef.RecruitsGenerationParams.HasWeapons = AssortedAdjustments.Settings.RecruitGenerationHasWeapons;
                gdlDef.RecruitsGenerationParams.HasConsumableItems = AssortedAdjustments.Settings.RecruitGenerationHasConsumableItems;
                gdlDef.RecruitsGenerationParams.HasInventoryItems = AssortedAdjustments.Settings.RecruitGenerationHasInventoryItems;
                gdlDef.RecruitsGenerationParams.CanHaveAugmentations = AssortedAdjustments.Settings.RecruitGenerationCanHaveAugmentations;
            }

            foreach (GeoFactionDef gfDef in defRepository.DefRepositoryDef.AllDefs.OfType<GeoFactionDef>())
            {
                if (gfDef.name.Contains("Anu") || gfDef.name.Contains("NewJericho") || gfDef.name.Contains("Synedrion"))
                {
                    gfDef.RecruitIntervalCheckDays = AssortedAdjustments.Settings.RecruitIntervalCheckDays;
                    Logger.Info($"[CustomRecruitGeneration_Apply] gfDef: {gfDef.name}, RecruitIntervalCheckDays: {gfDef.RecruitIntervalCheckDays}");
                }
            }
        }



        // Patches
        [HarmonyPatch(typeof(FactionCharacterGenerator), "GenerateUnit")]
        public static class FactionCharacterGenerator_GenerateUnit_Patch
        {
            private static bool customizationEnabled = false;
            private static List<TacticalAbilityDef> originalPersonalAbilityPool;
            private static int originalBonusStatStrength;
            private static int originalBonusStatWill;
            private static int originalBonusStatSpeed;
            private static int originalAbilityCount;

            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration && AssortedAdjustments.Settings.EnhanceSpecialRecruits;
            }

            public static void Prefix(FactionCharacterGenerator __instance, TacCharacterDef template, List<TacticalAbilityDef> ____personalAbilityPool)
            {
                try
                {
                    if (template == null)
                    {
                        return;
                    }

                    originalPersonalAbilityPool = new List<TacticalAbilityDef>(____personalAbilityPool);
                    originalBonusStatStrength = template.Data.Strength;
                    originalBonusStatWill = template.Data.Will;
                    originalBonusStatSpeed = template.Data.Speed;
                    originalAbilityCount = __instance.BaseStatsSheet.PersonalAbilitiesCount;
                    customizationEnabled = false;

                    string methodName = "";
                    string typeName = "";
                    string qualifiedMethodName = "";
                    StackTrace st = new StackTrace();
                    for (int i = 0; i < st.FrameCount; i++)
                    {
                        StackFrame sf = st.GetFrame(i);
                        methodName = sf.GetMethod().Name;
                        typeName = sf.GetMethod().DeclaringType.Name;
                        qualifiedMethodName = $"{typeName}.{methodName}";
                        //Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] st[{i}] -> {qualifiedMethodName}");

                        // Context for ability customization found
                        if (contextsForCustomPools.Contains(qualifiedMethodName))
                        {
                            customizationEnabled = true;
                            break;
                        }

                        // Already too deep in the stack
                        if (methodName.Contains("MoveNext"))
                        {
                            break;
                        }
                    }

                    if (customizationEnabled)
                    {
                        Logger.Debug("---");
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Found customization context: {qualifiedMethodName}");
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Original ability pool: {originalPersonalAbilityPool.Select(a => a.name).Join()}");
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Checking ability pools for template: {template.name}");

                        if (Utilities.GetKeyByTemplate(template, out string key) && abilityPoolByTemplate != null && abilityPoolByTemplate.ContainsKey(key))
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Found a custom pool for class: {key}. Removing everything else from the pool.");
                            ____personalAbilityPool.RemoveWhere(a => !Utilities.ContainsAny(a.name, abilityPoolByTemplate[key]));
                        }

                        if (customAbilitiesByTemplate != null && customAbilitiesByTemplate.ContainsKey(key))
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Found additional custom abilities for class: {key}. Adding to the pool.");
                            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();
                            foreach (string ability in customAbilitiesByTemplate[key])
                            {
                                TacticalAbilityDef customAbility = defRepository.DefRepositoryDef.AllDefs.OfType<TacticalAbilityDef>().Where(d => d.name.Contains(ability)).FirstOrDefault();
                                if (customAbility != null)
                                {
                                    ____personalAbilityPool.Add(customAbility);
                                }
                            }
                        }
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Modified ability pool: {____personalAbilityPool.Select(a => a.name).Join()}");

                        if (bonusStatsByTemplate != null && bonusStatsByTemplate.ContainsKey(key))
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Original Bonus stats: {template.Data.Strength}, {template.Data.Will}, {template.Data.Speed}");
                            template.Data.Strength += bonusStatsByTemplate[key][0];
                            template.Data.Will += bonusStatsByTemplate[key][1];
                            template.Data.Speed += bonusStatsByTemplate[key][2];
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_PREFIX] Modified bonus stats: {template.Data.Strength}, {template.Data.Will}, {template.Data.Speed}");
                        }

                        if (__instance.BaseStatsSheet.PersonalAbilitiesCount < 7)
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] Original ability count: {__instance.BaseStatsSheet.PersonalAbilitiesCount}");
                            __instance.BaseStatsSheet.PersonalAbilitiesCount += 1;
                            Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] Modified ability count: {__instance.BaseStatsSheet.PersonalAbilitiesCount}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);

                }
            }

            public static void Postfix(FactionCharacterGenerator __instance, ref GeoUnitDescriptor __result, GeoFaction faction, TacCharacterDef template, ref List<TacticalAbilityDef> ____personalAbilityPool)
            {
                try
                {
                    if (customizationEnabled)
                    {
                        // Restore original ability pool
                        ____personalAbilityPool = new List<TacticalAbilityDef>(originalPersonalAbilityPool);
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] Restored ability pool: {____personalAbilityPool.Select(a => a.name).Join()}");

                        // Restore original bonus stats
                        template.Data.Strength = originalBonusStatStrength;
                        template.Data.Will = originalBonusStatWill;
                        template.Data.Speed = originalBonusStatSpeed;
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] Restored bonus stats: {template.Data.Strength}, {template.Data.Will}, {template.Data.Speed}");

                        __instance.BaseStatsSheet.PersonalAbilitiesCount = originalAbilityCount;
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] Restored ability count: {__instance.BaseStatsSheet.PersonalAbilitiesCount}");
                        Logger.Debug("---");
                    }

                    /*
                    if (template == null || !AssortedAdjustments.Settings.CustomizeTutorialSquad)
                    {
                        return;
                    }

                    if (Utilities.GetKeyByTemplate(template, out string key) && abilityPoolByTemplate.ContainsKey(key))
                    {
                        Logger.Debug($"[FactionCharacterGenerator_GenerateUnit_POSTFIX] {key} will get dedicated abilities: {abilityPoolByTemplate[key].Join()}");

                        GeoUnitDescriptor geoUnitDescriptor = new GeoUnitDescriptor(faction, new GeoUnitDescriptor.UnitTypeDescriptor(template));
                        GeoUnitDescriptor.ProgressionDescriptor progressionDescriptor = null;
                        foreach (ClassTagDef classTag in template.ClassTags)
                        {
                            SpecializationDef specializationByClassTag = __instance.GetSpecializationByClassTag(classTag);
                            if (specializationByClassTag != null)
                            {
                                if (progressionDescriptor == null)
                                {
                                    Dictionary<int, TacticalAbilityDef> personalAbilitiesByLevel = new Dictionary<int, TacticalAbilityDef>();

                                    int index = 0;
                                    foreach (string ability in abilityPoolByTemplate[key])
                                    {
                                        if (!String.IsNullOrEmpty(ability))
                                        {
                                            personalAbilitiesByLevel.Add(index, ____personalAbilityPool.Where(a => a.name.Contains(ability)).FirstOrDefault());
                                        }
                                        index++;

                                        // Safeguard
                                        if (index > 6)
                                        {
                                            break;
                                        }
                                    }

                                    progressionDescriptor = new GeoUnitDescriptor.ProgressionDescriptor(specializationByClassTag, personalAbilitiesByLevel);
                                }
                                else
                                {
                                    progressionDescriptor.SecondarySpecDef = specializationByClassTag;
                                }
                            }
                        }
                        if (progressionDescriptor != null && template.Data.LevelProgression.IsValid)
                        {
                            progressionDescriptor.Level = template.Data.LevelProgression.Level;
                            progressionDescriptor.ExtraAbilities.AddRange(template.Data.Abilites);
                        }

                        if (progressionDescriptor != null)
                        {
                            __result.Progression = progressionDescriptor;
                        }
                    }
                    */
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Called only at GeoPhoenixFaction.RegenerateNakedRecruits()
        [HarmonyPatch(typeof(FactionCharacterGenerator), "GenerateRandomUnit")]
        public static class FactionCharacterGenerator_GenerateRandomUnit_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration;
            }

            // Override!
            public static bool Prefix(FactionCharacterGenerator __instance, ref GeoUnitDescriptor __result, CharacterGenerationContext context)
            {
                try
                {
                    if (!(context.Faction is GeoPhoenixFaction))
                    {
                        return true;
                    }

                    List<TacCharacterDef> templates = context.Templates;
                    if (templates.Count == 0)
                    {
                        __result = null;
                        return false;
                    }

                    foreach(var t in templates)
                    {
                        // Lower the weight for Berserker from 100 to 60 (equals Priest, Infiltrator and Technician)
                        if (t.ClassTag.className == "Berserker")
                        {
                            t.RecruitSpawnWeight = 60;
                        }
                        //Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] templates: {t.ClassTag.className}");
                        //Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] templates: {t.RecruitSpawnWeight}");
                    }

                    TacCharacterDef template = templates.WeightedRandomElement((TacCharacterDef def) => def.RecruitSpawnWeight);



                    // MAD:
                    // PX_AssaultStarting_CharacterTemplateDef
                    // PX_HeavyStarting_CharacterTemplateDef
                    // PX_SniperStarting_CharacterTemplateDef
                    // SY_Infiltrator1_CharacterTemplateDef
                    // AN_Berserker1_CharacterTemplateDef
                    // NJ_Technician1_CharacterTemplateDef
                    // AN_JudgementPriest1_CharacterTemplateDef || AN_ScreamingPriest1_CharacterTemplateDef || AN_SynodPriest1_CharacterTemplateDef
                    Dictionary<string, string> classTemplates = new Dictionary<string, string>
                    {
                        { "Assault", "PX_AssaultStarting_CharacterTemplateDef" },
                        { "Heavy", "PX_HeavyStarting_CharacterTemplateDef" },
                        { "Sniper", "PX_SniperStarting_CharacterTemplateDef" },
                        { "Infiltrator", "SY_Infiltrator1_CharacterTemplateDef" },
                        { "Berserker", "AN_Berserker1_CharacterTemplateDef" },
                        { "Technician", "NJ_Technician1_CharacterTemplateDef" },
                        { "Priest", "AN_JudgementPriest1_CharacterTemplateDef" }
                    };
                    Dictionary<string, string> armorSources = new Dictionary<string, string>
                    {
                        { "Assault", "" },
                        { "Heavy", "" },
                        { "Sniper", "" },
                        { "Infiltrator", "PX_SniperStarting_CharacterTemplateDef" },
                        { "Berserker", "PX_AssaultStarting_CharacterTemplateDef" },
                        { "Technician", "PX_HeavyStarting_CharacterTemplateDef" },
                        { "Priest", "PX_SniperStarting_CharacterTemplateDef" }
                    };
                    Dictionary<string, string> equipmentSources = new Dictionary<string, string>
                    {
                        { "Assault", "" },
                        { "Heavy", "" },
                        { "Sniper", "" },
                        { "Infiltrator", "SY_Infiltrator2_CharacterTemplateDef" },
                        { "Berserker", "AN_Berserker2_1_CharacterTemplateDef" },
                        { "Technician", "NJ_Technician2_CharacterTemplateDef" },
                        { "Priest", "AN_JudgementPriest2_CharacterTemplateDef" }
                    };

                    Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] faction: {context.Faction.Name.Localize()}");
                    Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] template: {template.name}");
                    Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] className: {template.ClassTag.className}");

                    DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

                    //List<TacCharacterDef> allHumanTemplates = defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(def => def.IsHuman).ToList();
                    //TacCharacterDef replacementTemplate = allHumanTemplates.Where(d => d.name.Contains("PX_Assault_Berserker_L7_TacCharacterDef")).FirstOrDefault();

                    string className = template.ClassTag.className;
                    string replacementIdentifier = classTemplates[className];
                    Logger.Info($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] replacementIdentifier: {replacementIdentifier}");

                    TacCharacterDef replacementTemplate = defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(def => def.name.Contains(replacementIdentifier)).FirstOrDefault();

                    if (replacementTemplate == null)
                    {
                        throw new NullReferenceException("Replacement template is null");
                    }

                    // Replace default armor with suiting phoenix alternative for non-default classes
                    string armorSourceIdentifier = armorSources[className];
                    Logger.Info($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] armorSourceIdentifier: {armorSourceIdentifier}");

                    if (!String.IsNullOrEmpty(armorSourceIdentifier))
                    {
                        TacCharacterDef armorSourceTemplate = defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(def => def.name.Contains(armorSourceIdentifier)).FirstOrDefault();
                        if (armorSourceTemplate != null)
                        {
                            replacementTemplate.Data.BodypartItems = armorSourceTemplate.Data.BodypartItems.ToArray();
                        }
                        else
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] Armor replacement FAILED.");
                        }
                    }

                    // Replace default equipment with the one of another def for individualization
                    string equipmentSourceIdentifier = equipmentSources[className];
                    Logger.Info($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] equipmentSourceIdentifier: {equipmentSourceIdentifier}");

                    if (!String.IsNullOrEmpty(equipmentSourceIdentifier))
                    {
                        TacCharacterDef equipmentSourceTemplate = defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(def => def.name.Contains(equipmentSourceIdentifier)).FirstOrDefault();
                        if (equipmentSourceTemplate != null)
                        {
                            replacementTemplate.Data.EquipmentItems = equipmentSourceTemplate.Data.EquipmentItems.ToArray();
                            replacementTemplate.Data.InventoryItems = equipmentSourceTemplate.Data.InventoryItems.ToArray();
                        }
                        else
                        {
                            Logger.Debug($"[FactionCharacterGenerator_GenerateRandomUnit_PREFIX] Equipment replacement FAILED.");
                        }
                    }

                    // Finally replace template
                    template = replacementTemplate;
                    //:DAM



                    GeoUnitDescriptor geoUnitDescriptor = __instance.GenerateUnit(context.Faction, template);
                    __instance.RandomizeIdentity(geoUnitDescriptor, GeoCharacterSex.None);



                    // MAD:
                    // Move helmet to inventory just for the looks of it...
                    TacticalItemDef helmet = geoUnitDescriptor.ArmorItems.Where(e => e.name.Contains("Helmet")/*|| e.name.Contains("Priest_Head")*/).FirstOrDefault();
                    if (helmet != null)
                    {
                        geoUnitDescriptor.ArmorItems.Remove(helmet);
                        geoUnitDescriptor.Inventory.Add(helmet);
                    }
                    //:DAM



                    __result = geoUnitDescriptor;
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }



        // Override to always spawn a fixed number of recruits
        [HarmonyPatch(typeof(GeoPhoenixFaction), "RegenerateNakedRecruits")]
        public static class GeoPhoenixFaction_RegenerateNakedRecruits_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration;
            }

            // Override!
            // @ToDo: Learn to use transpiler, it's just one value to change here
            public static bool Prefix(GeoPhoenixFaction __instance, ref Dictionary<GeoUnitDescriptor, ResourcePack> ____nakedRecruits, GeoLevelController ____level, ref TimeUnit ____lastNakedRecruitRefresh)
            {
                try
                {
                    Logger.Debug($"[GeoPhoenixFaction_RegenerateNakedRecruits_PREFIX] Generate a custom amount of new recruits. Potentially edit their parameters.");

                    ____nakedRecruits.Clear();

                    // MAD:
                    //int num = __instance.FactionDef.MaxNakedRecruitsAvailability.RandomValue();
                    int num = Mathf.Clamp(AssortedAdjustments.Settings.RecruitGenerationCount, 1, 4); // UI cannot handle more than four
                    //:DAM

                    CharacterGenerationContext context = ____level.CharacterGenerator.GenerateCharacterGeneratorContext(__instance);
                    for (int i = 0; i < num; i++)
                    {
                        GeoUnitDescriptor geoUnitDescriptor = ____level.CharacterGenerator.GenerateRandomUnit(context);
                        ____level.CharacterGenerator.ApplyRecruitDifficultyParameters(geoUnitDescriptor);
                        ResourcePack resourcePack = __instance.GenerateNakedRecruitsCost();
                        ____nakedRecruits.Add(geoUnitDescriptor, resourcePack);
                    }
                    ____lastNakedRecruitRefresh = ____level.Timing.Now;
                    __instance.SpawnedRecruitNotification = true;
                    Action<IEnumerable<GeoUnitDescriptor>> recruitsRegenerated = __instance.RecruitsRegenerated;
                    if (recruitsRegenerated == null)
                    {
                        return false;
                    }
                    recruitsRegenerated.Invoke(__instance.NakedRecruits.Keys);


                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }



        // Always update haven recruits
        [HarmonyPatch(typeof(GeoHaven), "CheckShouldSpawnRecruit")]
        public static class GeoHaven_CheckShouldSpawnRecruit_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration && AssortedAdjustments.Settings.IgnoreRngFactorsForHavenRecruitGeneration;
            }

            public static bool Prefix(GeoHaven __instance, ref bool __result)
            {
                try
                {
                    Logger.Debug($"[GeoHaven_CheckShouldSpawnRecruit_PREFIX] Ignoring unit limits and rng factors for haven recruit generation");

                    if (!__instance.IsRecruitmentEnabled || !__instance.ZonesStats.CanGenerateRecruit)
                    {
                        __result = false;
                        return false;
                    }
                    if (__instance.QuerySpawnNewRecruit)
                    {
                        __result = true;
                        return false;
                    }
                    GeoFaction phoenixFaction = __instance.Site.GeoLevel.PhoenixFaction;
                    PartyDiplomacy.Relation relation = __instance.Leader.Diplomacy.GetRelation(phoenixFaction);
                    if (relation.Diplomacy <= 0)
                    {
                        __result = false;
                        return false;
                    }

                    __result = true;
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }



        // Info
        [HarmonyPatch(typeof(GeoFaction), "GenerateRecruits", new Type[] { typeof(Timing) })]
        public static class GeoFaction_GenerateRecruits_Patch
        {
            public static void Prefix(GeoFaction __instance, Timing timing)
            {
                try
                {
                    
                    if (__instance.Def.name.Contains("Anu") || __instance.Def.name.Contains("NewJericho") || __instance.Def.name.Contains("Synedrion"))
                    {
                        Logger.Debug($"[GeoFaction_GenerateRecruits_PREFIX] Faction: {__instance.Name.Localize()}");
                        Logger.Debug($"[GeoFaction_GenerateRecruits_PREFIX] RecruitIntervalCheckDays: {__instance.Def.RecruitIntervalCheckDays}");
                        Logger.Debug($"[GeoFaction_GenerateRecruits_PREFIX] GenerateRecruitNextTime: {__instance.GenerateRecruitNextTime}, Now: {timing.Now}");

                        // Update now?
                        //__instance.GenerateRecruitNextTime = timing.Now;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        /*
        // Utility patch to spawn haven recruits on game load
        [HarmonyPatch(typeof(GeoFaction), "OnAfterFactionsLevelStart")]
        public static class GeoFaction_OnAfterFactionsLevelStart_Patch
        {
            public static void Postfix(GeoFaction __instance, GeoLevelController ____level)
            {
                try
                {
                    if(__instance.Name.Localize() != "Disciples of Anu")
                    {
                        return;
                    }

                    Logger.Debug($"[GeoFaction_OnAfterFactionsLevelStart_POSTFIX] Respawning recruits for faction: {__instance.Name.Localize()}");

                    foreach (GeoHaven geoHaven in __instance.Havens)
                    {
                        Logger.Debug($"[GeoFaction_OnAfterFactionsLevelStart_POSTFIX] geoHaven.Site: {geoHaven.Site.Name}");
                        Logger.Debug($"[GeoFaction_OnAfterFactionsLevelStart_POSTFIX] geoHaven.IsRecruitmentEnabled: {geoHaven.IsRecruitmentEnabled}");
                        Logger.Debug($"[GeoFaction_OnAfterFactionsLevelStart_POSTFIX] geoHaven.AvailableRecruit: {geoHaven.AvailableRecruit?.GetName()}");

                        geoHaven.KillRecruit();
                        CharacterGenerationContext context = ____level.CharacterGenerator.GenerateCharacterGeneratorContext(__instance);
                        geoHaven.SpawnNewRecruit(context, null);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */

        /*
        // Not created equally (not possible atm as ____progression.BaseStatSheet is global and readonly)
        [HarmonyPatch(typeof(GeoUnitDescriptor), "FinishInitCharacter")]
        public static class GeoUnitDescriptor_FinishInitCharacter_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration;
            }

            public static void Postfix(GeoUnitDescriptor __instance, ref GeoCharacter character)
            {
                try
                {
                    Logger.Debug($"[GeoUnitDescriptor_FinishInitCharacter_POSTFIX] Not created equally.");

                    if (__instance.Progression != null)
                    {
                        int defaultMaxStrength = __instance.LevelController.CharacterGenerator.BaseStatsSheet.MaxStrength;
                        int defaultMaxWill = __instance.LevelController.CharacterGenerator.BaseStatsSheet.MaxWill;
                        int defaultMaxSpeed = __instance.LevelController.CharacterGenerator.BaseStatsSheet.MaxSpeed;
                        Logger.Info($"[GeoUnitDescriptor_FinishInitCharacter_POSTFIX] ({character.DisplayName}) Default max base stats: {defaultMaxStrength}, {defaultMaxWill}, {defaultMaxSpeed}");

                        int[] allowedStrengthDeviation = new int[] { -3, 4 };
                        int[] allowedWillDeviation = new int[] { -2, 3 };
                        int[] allowedSpeedDeviation = new int[] { -2, 1 };

                        System.Random r = new System.Random();
                        int randomMaxStrength = defaultMaxStrength + r.Next(allowedStrengthDeviation[0], allowedStrengthDeviation[1]);
                        int randomMaxWill = defaultMaxWill + r.Next(allowedWillDeviation[0], allowedWillDeviation[1]);
                        int randomMaxSpeed = defaultMaxSpeed + r.Next(allowedSpeedDeviation[0], allowedSpeedDeviation[1]);

                        CharacterProgression ____progression = (CharacterProgression)AccessTools.Field(typeof(GeoCharacter), "_progression").GetValue(character);

                        ____progression.BaseStatSheet = new BaseStatSheetDef();

                        ____progression.BaseStatSheet.MaxStrength = randomMaxStrength;
                        ____progression.BaseStatSheet.MaxWill = randomMaxWill;
                        ____progression.BaseStatSheet.MaxSpeed = randomMaxSpeed;

                        Logger.Info($"[GeoUnitDescriptor_FinishInitCharacter_POSTFIX] ({character.DisplayName}) Generated max base stats: {randomMaxStrength}, {randomMaxWill}, {randomMaxSpeed}");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */
    }
}
