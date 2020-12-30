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
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.View.ViewControllers.PhoenixBase;

namespace AssortedAdjustments.Patches
{
    internal static class FacilityAdjustments
    {
        internal static float currentHealFacilityHealOutput;
        internal static float currentHealFacilityStaminaHealOutput;
        internal static float currentVehicleSlotFacilityAircraftHealOuput;
        internal static float currentVehicleSlotFacilityVehicleHealOuput;
        internal static float currentFoodProductionFacilitySuppliesOutput;



        public static void Apply()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(typeof(EconomyAdjustments).Namespace);
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<HealFacilityComponentDef> healFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<HealFacilityComponentDef>().ToList();
            foreach (HealFacilityComponentDef hfcDef in healFacilityComponentDefs)
            {
                if (hfcDef.name.Contains("MedicalBay"))
                {
                    hfcDef.BaseHeal = AssortedAdjustments.Settings.MedicalBayBaseHeal;
                    currentHealFacilityHealOutput = hfcDef.BaseHeal;
                }
                else if (hfcDef.name.Contains("LivingQuarters"))
                {
                    hfcDef.BaseStaminaHeal = AssortedAdjustments.Settings.LivingQuartersBaseStaminaHeal;
                    currentHealFacilityStaminaHealOutput = hfcDef.BaseStaminaHeal;
                }
                Logger.Info($"[FacilityAdjustments_Apply] hfcDef: {hfcDef.name}, GUID: {hfcDef.Guid}, BaseHeal: {hfcDef.BaseHeal}, BaseStaminaHeal: {hfcDef.BaseStaminaHeal}");
            }

            List<VehicleSlotFacilityComponentDef> vehicleSlotFacilityComponentDefs = defRepository.DefRepositoryDef.AllDefs.OfType<VehicleSlotFacilityComponentDef>().Where(vsfDef => vsfDef.name.Contains("VehicleBay")).ToList();
            foreach (VehicleSlotFacilityComponentDef vsfDef in vehicleSlotFacilityComponentDefs)
            {
                vsfDef.AircraftHealAmount = AssortedAdjustments.Settings.VehicleBayAircraftHealAmount;
                vsfDef.VehicleHealAmount = AssortedAdjustments.Settings.VehicleBayVehicleHealAmount;

                currentVehicleSlotFacilityAircraftHealOuput = vsfDef.AircraftHealAmount;
                currentVehicleSlotFacilityVehicleHealOuput = vsfDef.VehicleHealAmount;

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

                    // When added here they are also affected by general research buffs. This is NOT intended.
                    /*
                    if(AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit materials = new ResourceUnit(ResourceType.Materials, value);
                        resources.AddUnique(materials);
                    }
                    */
                }
                else if (rgfDef.name.Contains("ResearchLab"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Research, AssortedAdjustments.Settings.ResearchLabGenerateResearchAmount);
                    resources.Set(supplies);

                    // When added here they are also affected by general research buffs (Synedrion research). This is NOT intended.
                    /*
                    if (AssortedAdjustments.Settings.ResearchLabGenerateTechAmount > 0f)
                    {
                        float value = AssortedAdjustments.Settings.ResearchLabGenerateTechAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                        ResourceUnit tech = new ResourceUnit(ResourceType.Tech, value);
                        resources.AddUnique(tech);
                    }
                    */
                }
                else if (rgfDef.name.Contains("FoodProduction"))
                {
                    ResourcePack resources = rgfDef.BaseResourcesOutput;
                    ResourceUnit supplies = new ResourceUnit(ResourceType.Supplies, AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount);
                    resources.Set(supplies);

                    currentFoodProductionFacilitySuppliesOutput = AssortedAdjustments.Settings.FoodProductionGenerateSuppliesAmount;
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



            HarmonyHelpers.Patch(harmony, typeof(ResourceGeneratorFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_ResourceGeneratorFacilityComponent_UpdateOutput");
            HarmonyHelpers.Patch(harmony, typeof(HealFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_HealFacilityComponent_UpdateOutput");
            HarmonyHelpers.Patch(harmony, typeof(VehicleSlotFacilityComponent), "UpdateOutput", typeof(FacilityAdjustments), null, "Postfix_VehicleSlotFacilityComponent_UpdateOutput");

            // UI
            HarmonyHelpers.Patch(harmony, typeof(UIFacilityTooltip), "Show", typeof(FacilityAdjustments), null, "Postfix_UIFacilityTooltip_Show");
            HarmonyHelpers.Patch(harmony, typeof(UIFacilityInfoPopup), "Show", typeof(FacilityAdjustments), null, "Postfix_UIFacilityInfoPopup_Show");
        }



        // Patches
        public static void Postfix_ResourceGeneratorFacilityComponent_UpdateOutput(ResourceGeneratorFacilityComponent __instance)
        {
            try
            {
                if(__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                {
                    string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                    string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                    string facilityId = __instance.Facility.FacilityId.ToString();
                    Logger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, ResourceOutput: {__instance.ResourceOutput}");

                    if (__instance.Def.name.Contains("FoodProduction"))
                    {
                        currentFoodProductionFacilitySuppliesOutput = __instance.ResourceOutput.Values.Where(u => u.Type == ResourceType.Supplies).First().Value;

                        /*
                        foreach (ResourceUnit resourceUnit in __instance.ResourceOutput.Values)
                        {
                            if (resourceUnit.Type == ResourceType.Supplies)
                            {
                                currentFoodProductionFacilitySuppliesOutput = resourceUnit.Value;
                            }
                        }
                        */
                    }
                }

                // All factions
                if (__instance.Def.name.Contains("FabricationPlant"))
                {
                    float value = AssortedAdjustments.Settings.FabricationPlantGenerateMaterialsAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                    ResourceUnit materials = new ResourceUnit(ResourceType.Materials, value);
                    __instance.ResourceOutput.AddUnique(materials);
                }
                else if (__instance.Def.name.Contains("ResearchLab"))
                {
                    float value = AssortedAdjustments.Settings.ResearchLabGenerateTechAmount / AssortedAdjustments.Settings.GenerateResourcesBaseDivisor;
                    ResourceUnit tech = new ResourceUnit(ResourceType.Tech, value);
                    __instance.ResourceOutput.AddUnique(tech);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix_HealFacilityComponent_UpdateOutput(HealFacilityComponent __instance)
        {
            try
            {
                if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                {
                    string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                    string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                    string facilityId = __instance.Facility.FacilityId.ToString();
                    Logger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, HealOutput: {__instance.HealOutput}, StaminaHealOutput: {__instance.StaminaHealOutput}");

                    if (__instance.Def.name.Contains("MedicalBay"))
                    {
                        currentHealFacilityHealOutput = __instance.HealOutput;
                    }
                    else if (__instance.Def.name.Contains("LivingQuarters"))
                    {
                        currentHealFacilityStaminaHealOutput = __instance.StaminaHealOutput;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        
        public static void Postfix_VehicleSlotFacilityComponent_UpdateOutput(VehicleSlotFacilityComponent __instance)
        {
            try
            {
                if (__instance.Facility.PxBase.Site.Owner is GeoPhoenixFaction)
                {
                    string owningFaction = __instance.Facility.PxBase.Site.Owner.Name.Localize();
                    string facilityName = __instance.Facility.ViewElementDef.DisplayName1.Localize();
                    string facilityId = __instance.Facility.FacilityId.ToString();
                    Logger.Info($"[ResourceGeneratorFacilityComponent_UpdateOutput_POSTFIX] owningFaction: {owningFaction}, facilityName: {facilityName}, facilityId: {facilityId}, AircraftHealOuput: {__instance.AircraftHealOuput}, VehicletHealOuput: {__instance.VehicletHealOuput}");

                    currentVehicleSlotFacilityAircraftHealOuput = __instance.AircraftHealOuput;
                    currentVehicleSlotFacilityVehicleHealOuput = __instance.VehicletHealOuput;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }



        public static void Postfix_UIFacilityTooltip_Show(UIFacilityTooltip __instance, PhoenixFacilityDef facility, GeoPhoenixBase currentBase)
        {
            try
            {
                if(currentBase == null)
                {
                    return;
                }

                if (facility.name.Contains("MedicalBay"))
                {
                    //float baseHealOutput = facility.GetComponent<HealFacilityComponentDef>().BaseHeal;
                    //float currentBonusValue = currentHealFacilityHealOutput > baseHealOutput ? (currentHealFacilityHealOutput - baseHealOutput) : 0;
                    //string currentBonus = currentBonusValue > 0 ? $"({baseHealOutput} + {currentBonusValue})" : "";

                    __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {currentHealFacilityHealOutput} Hit Points per hour for each medical facility in the base.";
                }
                else if (facility.name.Contains("LivingQuarters"))
                {
                    __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {currentHealFacilityStaminaHealOutput} Stamina points per hour for each living quarters in the base.";
                }
                else if (facility.name.Contains("VehicleBay"))
                {
                    __instance.Description.text = $"Vehicles and aircraft at the base recover {currentVehicleSlotFacilityVehicleHealOuput} Hit Points per hour. Allows maintenance of 2 ground vehicles and 2 aircraft.";
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
                    int foodProductionUnits = (int)Math.Round(currentFoodProductionFacilitySuppliesOutput * 24);
                    __instance.Description.text = $"A food production facility that generates enough food for {foodProductionUnits} soldiers each day.";
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix_UIFacilityInfoPopup_Show(UIFacilityInfoPopup __instance, GeoPhoenixFacility facility)
        {
            try
            {
                if (facility.Def.name.Contains("MedicalBay"))
                {
                    __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {currentHealFacilityHealOutput} Hit Points per hour for each medical facility in the base.";
                }
                else if (facility.Def.name.Contains("LivingQuarters"))
                {
                    __instance.Description.text = $"All soldiers at the base (even if assigned to an aircraft) will recover {currentHealFacilityStaminaHealOutput} Stamina points per hour for each living quarters in the base.";
                }
                else if (facility.Def.name.Contains("VehicleBay"))
                {
                    __instance.Description.text = $"Vehicles and aircraft at the base recover {currentVehicleSlotFacilityVehicleHealOuput} Hit Points per hour. Allows maintenance of 2 ground vehicles and 2 aircraft.";
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
                    int foodProductionUnits = (int)Math.Round(currentFoodProductionFacilitySuppliesOutput * 24);
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
