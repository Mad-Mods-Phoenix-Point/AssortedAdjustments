using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.View.ViewControllers.PhoenixBase;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace AssortedAdjustments
{
    internal static class DataHelpers
    {
        public static void Print()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            // WeaponDefs
            /*
            var defs = defRepository.DefRepositoryDef.AllDefs.OfType<WeaponDef>().ToList();

            foreach (var def in defs)
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

            // TacMissionTypeDef
            /*
            var defs = defRepository.DefRepositoryDef.AllDefs.OfType<TacMissionTypeDef>().ToList();

            foreach (var def in defs)
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

            // TimeRemainingFormatterDef
            /*
            var defs = defRepository.DefRepositoryDef.AllDefs.OfType<TimeRemainingFormatterDef>().ToList();

            foreach (var def in defs)
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
            List<PhoenixFacilityDef> phoenixFacilityDefs = defRepository.DefRepositoryDef.AllDefs.OfType<PhoenixFacilityDef>().ToList();
            foreach (PhoenixFacilityDef pfDef in phoenixFacilityDefs)
            {
                Logger.Info($"[DataHelpers_Print] pfDef: {pfDef.name}, Type: {pfDef.GetType().Name}, Description: {pfDef.ViewElementDef.Description.LocalizeEnglish()}");
            }
            */

            /*
            List<GeoFacilityComponentDef> geoFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<GeoFacilityComponentDef>().ToList();
            foreach (GeoFacilityComponentDef gfDef in geoFacilityComponentDefs)
            {
                Logger.Info($"[DataHelpers_Print] gfDef: {gfDef.name}, Type: {gfDef.GetType().Name}, ResourcePath: {gfDef.ResourcePath}");
            }
            */

            /*
            foreach (ResourceGeneratorFacilityComponentDef def in defRepository.DefRepositoryDef.AllDefs.OfType<ResourceGeneratorFacilityComponentDef>().ToList())
            {
                Logger.Info($"[DataHelpers_Print] def: {def.name}, GUID: {def.Guid}, BaseResourcesOutput: {def.BaseResourcesOutput.ToString()}");
            }
            */



            // Get vanilla descriptions
            /*
            var defs = defRepository.DefRepositoryDef.AllDefs.OfType<ViewElementDef>().ToList();
            foreach (ViewElementDef def in defs)
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
    }
}
