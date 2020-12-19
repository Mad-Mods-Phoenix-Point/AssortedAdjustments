using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.View.ViewControllers.PhoenixBase;

namespace AssortedAdjustments.Patches
{
    internal static class FacilityAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<HealFacilityComponentDef> healFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<HealFacilityComponentDef>().Where(hfcDef => hfcDef.name.Contains("MedicalBay")).ToList();
            foreach (HealFacilityComponentDef hfcDef in healFacilityComponentDefs)
            {
                hfcDef.BaseHeal = AssortedAdjustments.Settings.MedicalBayBaseHeal;
                Logger.Info($"[FacilityAdjustments_Apply] hfcDef: {hfcDef.name}, GUID: {hfcDef.Guid}, BaseHeal: {hfcDef.BaseHeal}");
            }

            List<VehicleSlotFacilityComponentDef> vehicleSlotFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<VehicleSlotFacilityComponentDef>().Where(vsfDef => vsfDef.name.Contains("VehicleBay")).ToList();
            foreach (VehicleSlotFacilityComponentDef vsfDef in vehicleSlotFacilityComponentDefs)
            {
                vsfDef.AircraftHealAmount = 4; // Default: 2 (not used?)
                vsfDef.VehicleHealAmount = AssortedAdjustments.Settings.VehicleBayVehicleHealAmount;
                Logger.Info($"[FacilityAdjustments_Apply] vsfDef: {vsfDef.name}, GUID: {vsfDef.Guid}, AircraftHealAmount: {vsfDef.AircraftHealAmount}, VehicleHealAmount: {vsfDef.VehicleHealAmount}");
            }

            List<ResourceGeneratorFacilityComponentDef> resourceGeneratorFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<ResourceGeneratorFacilityComponentDef>().ToList();
            foreach (ResourceGeneratorFacilityComponentDef rgfDef in resourceGeneratorFacilityComponentDefs)
            {
                if (rgfDef.name.Contains("FabricationPlant"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Production, AssortedAdjustments.Settings.FabricationPlantGenerateProductionAmount);
                    resources.Set(supplies);

                    if(AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit materials = new ResourceUnit(ResourceType.Materials, value);
                        resources.AddUnique(materials);
                    }
                }
                else if (rgfDef.name.Contains("ResearchLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Research, AssortedAdjustments.Settings.ResearchLabGenerateResearchAmount);
                    resources.Set(supplies);

                    if (AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.ResearchLabGenerateTechAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit tech = new ResourceUnit(ResourceType.Tech, value);
                        resources.AddUnique(tech);
                    }
                }
                else if (rgfDef.name.Contains("FoodProduction"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Supplies, AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount);
                    resources.Set(supplies);
                }
                else if (rgfDef.name.Contains("BionicsLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Research, AssortedAdjustments.Settings.BionicsLabGenerateResearchAmount);
                    resources.Set(supplies);
                }
                else if (rgfDef.name.Contains("MutationLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Mutagen, AssortedAdjustments.Settings.MutationLabGenerateMutagenAmount);
                    resources.Set(supplies);
                }

                Logger.Info($"[FacilityAdjustments_Apply] rgfDef: {rgfDef.name}, GUID: {rgfDef.Guid}, BaseResourcesOutput: {rgfDef.BaseResourcesOutput.ToString()}");
            }
        }



        // User interface
        [HarmonyPatch(typeof(UIFacilityTooltip), "Show")]
        public static class UIFacilityTooltip_Show_Patch
        {
            public static void Postfix(UIFacilityTooltip __instance, PhoenixFacilityDef facility)
            {
                try
                {
                    if (facility.name.Contains("MedicalBay"))
                    {
                        __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {AssortedAdjustments.Settings.MedicalBayBaseHeal} Hit Points per hour for each medical facility in the base.";
                    }
                    else if (facility.name.Contains("VehicleBay"))
                    {
                        __instance.Description.text = $"Vehicles and aircraft at the base recover {AssortedAdjustments.Settings.VehicleBayVehicleHealAmount} Hit Points per hour. Allows maintenance of 2 ground vehicles and 2 aircraft.";
                    }
                    else if (facility.name.Contains("FabricationPlant"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Every plant generates {AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount} material per hour.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.name.Contains("ResearchLab"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Every lab generates {AssortedAdjustments.Settings.ResearchLabGenerateTechAmount} tech per hour.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.name.Contains("FoodProduction"))
                    {
                        int foodProductionUnits = (int)Math.Round(AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount * 24);
                        __instance.Description.text = $"A food production facility that generates enough food for {foodProductionUnits} soldiers each day.";
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIFacilityInfoPopup), "Show")]
        public static class UIFacilityInfoPopup_Show_Patch
        {
            public static void Postfix(UIFacilityInfoPopup __instance, GeoPhoenixFacility facility)
            {
                try
                {
                    if (facility.Def.name.Contains("MedicalBay"))
                    {
                        __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {AssortedAdjustments.Settings.MedicalBayBaseHeal} Hit Points per hour for each medical facility in the base.";
                    }
                    else if (facility.Def.name.Contains("VehicleBay"))
                    {
                        __instance.Description.text = $"Vehicles and aircraft at the base recover {AssortedAdjustments.Settings.VehicleBayVehicleHealAmount} Hit Points per hour. Allows maintenance of 2 ground vehicles and 2 aircraft.";
                    }
                    else if (facility.Def.name.Contains("FabricationPlant"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Every plant generates {AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount} material per hour.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.Def.name.Contains("ResearchLab"))
                    {
                        string org = __instance.Description.text;
                        string add = $"Every lab generates {AssortedAdjustments.Settings.ResearchLabGenerateTechAmount} tech per hour.";
                        __instance.Description.text = $"{org}\n{add}";
                    }
                    else if (facility.Def.name.Contains("FoodProduction"))
                    {
                        int foodProductionUnits = (int)Math.Round(AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount * 24);
                        __instance.Description.text = $"A food production facility that generates enough food for {foodProductionUnits} soldiers each day.";
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
