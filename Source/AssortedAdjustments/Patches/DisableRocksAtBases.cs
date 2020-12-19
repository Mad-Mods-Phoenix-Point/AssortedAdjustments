using System;
using System.Collections.Generic;
using Harmony;
using Mtree;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(GeoPhoenixBaseLayout), "GenerateRockPlacableTiles")]
    public static class GeoPhoenixBaseLayout_GenerateRockPlacableTiles_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.DisableRocksAtBases;
        }

        public static void Postfix(GeoPhoenixBaseLayout __instance, List<Vector2Int> outResult)
        {
            try
            {
                Logger.Debug($"[GeoPhoenixBaseLayout_GenerateRockPlacableTiles_POSTFIX] Clearing rock tiles.");
                outResult.Clear();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }



    [HarmonyPatch(typeof(GeoPhoenixBaseLayout), "GenerateBuildableTiles")]
    public static class GeoPhoenixBaseLayout_GenerateBuildableTiles_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.DisableRocksAtBases;
        }

        public static void Prefix(GeoPhoenixBaseLayout __instance, List<Vector2Int> ____rockTiles)
        {
            try
            {
                Logger.Debug($"[GeoPhoenixBaseLayout_GenerateBuildableTiles_PREFIX] Clearing rock tiles.");
                ____rockTiles.Clear();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }



    [HarmonyPatch(typeof(GeoPhoenixBaseLayout), "GenerateFreeTiles")]
    public static class GeoPhoenixBaseLayout_GenerateFreeTiles_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.DisableRocksAtBases;
        }

        public static void Prefix(GeoPhoenixBaseLayout __instance, List<Vector2Int> ____rockTiles)
        {
            try
            {
                Logger.Debug($"[GeoPhoenixBaseLayout_GenerateFreeTiles_PREFIX] Clearing rock tiles.");
                ____rockTiles.Clear();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
