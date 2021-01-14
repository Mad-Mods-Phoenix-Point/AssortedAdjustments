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

namespace AssortedAdjustments.Patches
{
    internal static class CustomRecruitGeneration
    {
        public static void Apply()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(typeof(EconomyAdjustments).Namespace);
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<GameDifficultyLevelDef> gameDifficultyLevelDefDefs = defRepository.DefRepositoryDef.AllDefs.OfType<GameDifficultyLevelDef>().ToList();
            foreach (GameDifficultyLevelDef gdlDef in gameDifficultyLevelDefDefs)
            {
                gdlDef.RecruitsGenerationParams.HasArmor = AssortedAdjustments.Settings.RecruitGenerationHasArmor;
                gdlDef.RecruitsGenerationParams.HasWeapons = AssortedAdjustments.Settings.RecruitGenerationHasWeapons;
                gdlDef.RecruitsGenerationParams.HasConsumableItems = AssortedAdjustments.Settings.RecruitGenerationHasConsumableItems;
                gdlDef.RecruitsGenerationParams.HasInventoryItems = AssortedAdjustments.Settings.RecruitGenerationHasInventoryItems;
                gdlDef.RecruitsGenerationParams.CanHaveAugmentations = AssortedAdjustments.Settings.RecruitGenerationCanHaveAugmentations;

                /*
                Logger.Info($"[CustomRecruitGeneration_Apply] gdlDef: {gdlDef.Name.Localize()}");
                Logger.Info($"[CustomRecruitGeneration_Apply] StartingSquadGenerationParams:");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasArmor: {gdlDef.StartingSquadGenerationParams.HasArmor}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasWeapons: {gdlDef.StartingSquadGenerationParams.HasWeapons}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasConsumableItems: {gdlDef.StartingSquadGenerationParams.HasConsumableItems}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasInventoryItems: {gdlDef.StartingSquadGenerationParams.HasInventoryItems}");
                Logger.Info($"[CustomRecruitGeneration_Apply] CanHaveAugmentations: {gdlDef.StartingSquadGenerationParams.CanHaveAugmentations}");
                Logger.Info($"[CustomRecruitGeneration_Apply] EnduranceBonus: {gdlDef.StartingSquadGenerationParams.EnduranceBonus}");
                Logger.Info($"[CustomRecruitGeneration_Apply] WillBonus: {gdlDef.StartingSquadGenerationParams.WillBonus}");
                Logger.Info($"[CustomRecruitGeneration_Apply] SpeedBonus: {gdlDef.StartingSquadGenerationParams.SpeedBonus}");
                Logger.Info("---");
                Logger.Info($"[CustomRecruitGeneration_Apply] gdlDef: {gdlDef.Name.Localize()}");
                Logger.Info($"[CustomRecruitGeneration_Apply] RecruitsGenerationParams:");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasArmor: {gdlDef.RecruitsGenerationParams.HasArmor}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasWeapons: {gdlDef.RecruitsGenerationParams.HasWeapons}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasConsumableItems: {gdlDef.RecruitsGenerationParams.HasConsumableItems}");
                Logger.Info($"[CustomRecruitGeneration_Apply] HasInventoryItems: {gdlDef.RecruitsGenerationParams.HasInventoryItems}");
                Logger.Info($"[CustomRecruitGeneration_Apply] CanHaveAugmentations: {gdlDef.RecruitsGenerationParams.CanHaveAugmentations}");
                Logger.Info($"[CustomRecruitGeneration_Apply] EnduranceBonus: {gdlDef.RecruitsGenerationParams.EnduranceBonus}");
                Logger.Info($"[CustomRecruitGeneration_Apply] WillBonus: {gdlDef.RecruitsGenerationParams.WillBonus}");
                Logger.Info($"[CustomRecruitGeneration_Apply] SpeedBonus: {gdlDef.RecruitsGenerationParams.SpeedBonus}");
                Logger.Info("---");
                */
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
                        { "Technician", "PX_SniperStarting_CharacterTemplateDef" },
                        { "Priest", "PX_HeavyStarting_CharacterTemplateDef" }
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
                    TacticalItemDef helmet = geoUnitDescriptor.ArmorItems.Where(e => e.name.Contains("Helmet") || e.name.Contains("Priest_Head")).FirstOrDefault();
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
            public static bool Prefix(GeoPhoenixFaction __instance, Dictionary<GeoUnitDescriptor, ResourcePack> ____nakedRecruits, GeoLevelController ____level, TimeUnit ____lastNakedRecruitRefresh)
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
                        ResourcePack value = __instance.GenerateNakedRecruitsCost();
                        ____nakedRecruits.Add(geoUnitDescriptor, value);
                    }
                    ____lastNakedRecruitRefresh = ____level.Timing.Now;
                    __instance.SpawnedRecruitNotification = true;
                    Action<IEnumerable<GeoUnitDescriptor>> recruitsRegenerated = __instance.RecruitsRegenerated;
                    if (recruitsRegenerated == null)
                    {
                        return false;
                    }
                    recruitsRegenerated(__instance.NakedRecruits.Keys);


                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }



        /*
        // Applied for *all* recruits (Now called on the difficulty level def in Apply() to not strip the initial squad of their equipment...)
        [HarmonyPatch(typeof(FactionCharacterGenerator), "ApplyGenerationParameters")]
        public static class FactionCharacterGenerator_ApplyGenerationParameters_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration;
            }

            public static void Prefix(FactionCharacterGenerator __instance, GeoUnitDescriptor unit, ref CharacterGenerationParams generationParams)
            {
                try
                {
                    if (unit == null || generationParams == null)
                    {
                        return;
                    }

                    Logger.Debug($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] Overriding character generation parameters according to settings.");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] Class: {unit.Progression?.MainSpecDef?.ClassTag?.className}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] Faction: {unit.Faction?.Name.Localize()}");

                    CharacterGenerationParams overrideGenerationParams = new CharacterGenerationParams
                    {
                        HasArmor = AssortedAdjustments.Settings.RecruitGenerationHasArmor,
                        HasWeapons = AssortedAdjustments.Settings.RecruitGenerationHasWeapons,
                        HasConsumableItems = AssortedAdjustments.Settings.RecruitGenerationHasConsumableItems,
                        HasInventoryItems = AssortedAdjustments.Settings.RecruitGenerationHasInventoryItems,
                        CanHaveAugmentations = AssortedAdjustments.Settings.RecruitGenerationCanHaveAugmentations,
                        EnduranceBonus = generationParams.EnduranceBonus,
                        WillBonus = generationParams.WillBonus,
                        SpeedBonus = generationParams.SpeedBonus
                    };

                    // Bonus stats depending on class?
                    //switch (unit.Progression.MainSpecDef.ClassTag.className)
                    //{
                    //    case "Heavy":
                    //        overrideGenerationParams.EnduranceBonus = 2;
                    //        break;
                    //
                    //    default: break;
                    //}

                    generationParams = overrideGenerationParams;

                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] HasArmor: {generationParams.HasArmor}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] HasWeapons: {generationParams.HasWeapons}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] HasConsumableItems: {generationParams.HasConsumableItems}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] HasInventoryItems: {generationParams.HasInventoryItems}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] CanHaveAugmentations: {generationParams.CanHaveAugmentations}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] EnduranceBonus: {generationParams.EnduranceBonus}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] WillBonus: {generationParams.WillBonus}");
                    Logger.Info($"[FactionCharacterGenerator_ApplyGenerationParameters_PREFIX] SpeedBonus: {generationParams.SpeedBonus}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */



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
    }
}
