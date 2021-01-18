using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Core;
using Base.Defs;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.View.ViewControllers.PhoenixBase;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace AssortedAdjustments
{
    internal static class DataHelpers
    {
        public static void Print()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<TacCharacterDef>().Where(d => d.IsHuman))
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                Logger.Info($"[DataHelpers_Print] BonusStats: {def.Data.BonusStats}");
                Logger.Info($"[DataHelpers_Print] Armor: {String.Join(",", def.Data.BodypartItems.Select(i => i.name))}");
                Logger.Info($"[DataHelpers_Print] Equipment: {String.Join(",", def.Data.EquipmentItems.Select(i => i.name))}");
                Logger.Info($"[DataHelpers_Print] Inventory: {String.Join(",", def.Data.InventoryItems.Select(i => i.name))}");
                Logger.Info($"[DataHelpers_Print] ---");
            }
            */



            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<HealAbilityDef>().ToList())
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                Logger.Info($"[DataHelpers_Print] Description: {def.ViewElementDef?.Description?.LocalizeEnglish()}");

                Logger.Info($"[DataHelpers_Print] AdditionalEffectDef: {def.AdditionalEffectDef?.name}");

                Logger.Info($"[DataHelpers_Print] ---");
            }

            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<MultiEffectDef>().ToList())
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                //Logger.Info($"[DataHelpers_Print] Description: {def.ViewElementDef?.Description?.LocalizeEnglish()}");

                Logger.Info($"[DataHelpers_Print] EffectDefs: {String.Join(",", def.EffectDefs.Select(e => e.name))}");

                Logger.Info($"[DataHelpers_Print] ---");
            }
            */

            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<WeaponDef>())
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                //Logger.Info($"[DataHelpers_Print] Description: {def.ViewElementDef.Description.LocalizeEnglish()}");

                Logger.Info($"[DataHelpers_Print] DropOnActorDeath: {def.DropOnActorDeath}");
                Logger.Info($"[DataHelpers_Print] DestroyOnActorDeathPerc: {def.DestroyOnActorDeathPerc}");
                Logger.Info($"[DataHelpers_Print] DestroyWhenUsed: {def.DestroyWhenUsed}");
                Logger.Info($"[DataHelpers_Print] IsMountedToBody: {def.IsMountedToBody}");
                Logger.Info($"[DataHelpers_Print] BehaviorOnDisable: {def.BehaviorOnDisable}");

                Logger.Info($"[DataHelpers_Print] ---");
            }
            */

            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<TacMissionTypeDef>())
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                Logger.Info($"[DataHelpers_Print] Description: {def.Description.LocalizeEnglish()}");

                Logger.Info($"[DataHelpers_Print] DontRecoverItems: {def.DontRecoverItems}");
                Logger.Info($"[DataHelpers_Print] MaxPlayerUnits: {def.MaxPlayerUnits}");
                Logger.Info($"[DataHelpers_Print] AllowResourceCrateDeployment: {def.AllowResourceCrateDeployment}");
                Logger.Info($"[DataHelpers_Print] DifficultyThreatLevel: {def.DifficultyThreatLevel}");

                Logger.Info($"[DataHelpers_Print] ---");
            }
            */

            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<TimeRemainingFormatterDef>())
            {
                Logger.Info($"[DataHelpers_Print] Def: {def.name}");
                Logger.Info($"[DataHelpers_Print] Type: {def.GetType().Name}");
                Logger.Info($"[DataHelpers_Print] InfiniteText: {def.InfiniteText.Localize(null)}");

                Logger.Info($"[DataHelpers_Print] DaysText: {def.DaysText.Localize(null)}");
                Logger.Info($"[DataHelpers_Print] HoursText: {def.HoursText.Localize(null)}");

                Logger.Info($"[DataHelpers_Print] ---");
            }
            */

            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<HealFacilityComponentDef>())
            {
                Logger.Info($"[DataHelpers_Print] def: {def.name}, Type: {def.GetType().Name}, HealMutog: {def.HealMutog}, HealSoldier: {def.HealSoldier}, BaseHeal: {def.BaseHeal}, BaseStaminaHeal: {def.BaseStaminaHeal}");
            }
            */

            
            // Get vanilla descriptions
            /*
            foreach (var def in defRepository.DefRepositoryDef.AllDefs.OfType<ViewElementDef>())
            {
                Logger.Info($"[DataHelpers_Print] def: {def.name}, GUID: {def.Guid}");
                Logger.Info($"[DataHelpers_Print] DisplayName1: {def.DisplayName1.LocalizeEnglish()}");
                Logger.Info($"[DataHelpers_Print] DisplayName2: {def.DisplayName2.LocalizeEnglish()}");
                Logger.Info($"[DataHelpers_Print] Description: {def.Description.LocalizeEnglish()}");

                Logger.Info($"[DataHelpers_Print] SmallIcon: {def.SmallIcon}");
                Logger.Info($"[DataHelpers_Print] LargeIcon: {def.LargeIcon}");
                Logger.Info($"[DataHelpers_Print] InventoryIcon: {def.InventoryIcon}");
                Logger.Info($"[DataHelpers_Print] RosterIcon: {def.RosterIcon}");
            }
            */
        }



        // Get localization keys to use them elsewhere
        [HarmonyPatch(typeof(LocalizedTextBind), "Localize")]
        public static class LocalizedTextBind_Localize_Patch
        {
            public static bool Prepare()
            {
                return false;
            }

            public static void Postfix(LocalizedTextBind __instance, string __result)
            {
                try
                {
                    Logger.Info($"[DataHelpers][LocalizedTextBind_Localize_POSTFIX] LocalizationKey: {__instance.LocalizationKey}");
                    Logger.Info($"[DataHelpers][LocalizedTextBind_Localize_POSTFIX] Localization: {__result}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
