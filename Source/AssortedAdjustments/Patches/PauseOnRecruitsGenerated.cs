using System;
using Harmony;
using PhoenixPoint.Geoscape.Levels;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(GeoscapeLog), "PhoenixFaction_OnRecruitsRegenerated")]
    public static class GeoscapeLog_PhoenixFaction_OnRecruitsRegenerated_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.PauseOnRecruitsGenerated;
        }

        public static void Prefix(GeoscapeLog __instance, GeoLevelController ____level)
        {
            try
            {
                Logger.Info($"[GeoscapeLog_PhoenixFaction_OnRecruitsRegenerated_PREFIX] Pausing.");

                ____level.View.RequestGamePause();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
