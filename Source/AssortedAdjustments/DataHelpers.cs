using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Levels.Missions;
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



            /*
            List<PhoenixFacilityDef> phoenixFacilityDefs = defRepository.DefRepositoryDef.AllDefs.OfType<PhoenixFacilityDef>().ToList();
            foreach (PhoenixFacilityDef pfDef in phoenixFacilityDefs)
            {
                Logger.Info($"[FacilityAdjustments_Apply] pfDef: {pfDef.name}, Type: {pfDef.GetType().Name}, Description: {pfDef.ViewElementDef.Description.LocalizeEnglish()}");
            }
            */

            /*
            List<GeoFacilityComponentDef> geoFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<GeoFacilityComponentDef>().ToList();
            foreach (GeoFacilityComponentDef gfDef in geoFacilityComponentDefs)
            {
                Logger.Info($"[FacilityAdjustments_Apply] gfDef: {gfDef.name}, Type: {gfDef.GetType().Name}, ResourcePath: {gfDef.ResourcePath}");
            }
            */

            /*
            foreach (ResourceGeneratorFacilityComponentDef def in defRepository.DefRepositoryDef.AllDefs.OfType<ResourceGeneratorFacilityComponentDef>().ToList())
            {
                Logger.Info($"[FacilityAdjustments_Apply] def: {def.name}, GUID: {def.Guid}, BaseResourcesOutput: {def.BaseResourcesOutput.ToString()}");
            }
            */



            // Get vanilla descriptions
            /*
            List<ViewElementDef> viewElementDefs = defRepository.DefRepositoryDef.AllDefs.OfType<ViewElementDef>().Where(veDef => veDef.name.Contains("VehicleBay")).ToList();
            foreach (ViewElementDef veDef in viewElementDefs)
            {
                Logger.Info($"[FacilityAdjustments_Apply] veDef: {veDef.name}, GUID: {veDef.Guid}, Description: {veDef.Description.LocalizeEnglish()}");
            }
            */
        }
    }
}
