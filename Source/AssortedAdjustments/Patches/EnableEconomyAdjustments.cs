using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;

namespace AssortedAdjustments.Patches
{
    internal static class EconomyAdjustments
    {
        public static void Apply()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(typeof(EconomyAdjustments).Namespace);
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();
            List<ItemDef> allItemDefs = defRepository.DefRepositoryDef.AllDefs.OfType<ItemDef>().ToList();

            foreach (ItemDef iDef in allItemDefs)
            {
                //Logger.Info($"[EconomyAdjustments_Apply] ItemDef: {iDef.name}");

                float resourceMultiplier = AssortedAdjustments.Settings.ResourceMultiplier;
                float costMultiplier = AssortedAdjustments.Settings.CostMultiplier;
                //Logger.Info($"[EconomyAdjustments_Apply] resourceMultiplier: {resourceMultiplier}");
                //Logger.Info($"[EconomyAdjustments_Apply] costMultiplier: {costMultiplier}");

                float tech = (float)Math.Round(iDef.ManufactureTech * resourceMultiplier);
                float materials = (float)Math.Round(iDef.ManufactureMaterials * resourceMultiplier);
                float mutagen = (float)Math.Round(iDef.ManufactureMutagen * resourceMultiplier);
                float crystals = (float)Math.Round(iDef.ManufactureLivingCrystals * resourceMultiplier);
                float oricalcium = (float)Math.Round(iDef.ManufactureOricalcum * resourceMultiplier);
                float mutane = (float)Math.Round(iDef.ManufactureProteanMutane * resourceMultiplier);

                float cost = (float)Math.Round(iDef.ManufacturePointsCost * costMultiplier);

                //Logger.Info($"[EconomyAdjustments_Apply] ItemDef: {iDef.name}, orgTech: {iDef.ManufactureTech}, modTech: {tech}");
                //Logger.Info($"[EconomyAdjustments_Apply] ItemDef: {iDef.name}, orgMaterials: {iDef.ManufactureMaterials}, modMaterials: {materials}");
                //Logger.Info($"[EconomyAdjustments_Apply] ItemDef: {iDef.name}, orgPointsCost: {iDef.ManufacturePointsCost}, modPointsCost: {cost}");

                iDef.ManufactureTech = tech;
                iDef.ManufactureMaterials = materials;
                iDef.ManufactureMutagen = mutagen;
                iDef.ManufactureLivingCrystals = crystals;
                iDef.ManufactureOricalcum = oricalcium;
                iDef.ManufactureProteanMutane = mutane;

                iDef.ManufacturePointsCost = cost;
            }


            // Patches
            //harmony.Patch(typeof(ItemDef).GetProperty("ScrapPrice").GetGetMethod(), null, new HarmonyMethod(typeof(EconomyAdjustments).GetMethod("Postfix_ItemDef_ScrapPrice")));
            HarmonyHelpers.PatchGetter(harmony, typeof(ItemDef), "ScrapPrice", typeof(EconomyAdjustments), null, "Postfix_ItemDef_ScrapPrice");
        }



        public static void Postfix_ItemDef_ScrapPrice(ItemDef __instance, ref ResourcePack __result, ResourcePack ____scrapPrice)
        {
            try
            {
                float scrapMultiplier = AssortedAdjustments.Settings.ScrapMultiplier;
                //Logger.Info($"[ItemDef_ScrapPrice_POSTFIX] scrapMultiplier: {scrapMultiplier}");

                float tech = (float)Math.Round(__instance.ManufactureTech * scrapMultiplier);
                float materials = (float)Math.Round(__instance.ManufactureMaterials * scrapMultiplier);
                float mutagen = (float)Math.Round(__instance.ManufactureMutagen * scrapMultiplier);
                float crystals = (float)Math.Round(__instance.ManufactureLivingCrystals * scrapMultiplier);
                float oricalcium = (float)Math.Round(__instance.ManufactureOricalcum * scrapMultiplier);
                float mutane = (float)Math.Round(__instance.ManufactureProteanMutane * scrapMultiplier);

                ResourcePack result = (____scrapPrice = new ResourcePack(new ResourceUnit[]
                {
                            new ResourceUnit(ResourceType.Tech, tech),
                            new ResourceUnit(ResourceType.Materials, materials),
                            new ResourceUnit(ResourceType.Mutagen, mutagen),
                            new ResourceUnit(ResourceType.LivingCrystals, crystals),
                            new ResourceUnit(ResourceType.Orichalcum, oricalcium),
                            new ResourceUnit(ResourceType.ProteanMutane, mutane)
                }));

                __result = result;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
