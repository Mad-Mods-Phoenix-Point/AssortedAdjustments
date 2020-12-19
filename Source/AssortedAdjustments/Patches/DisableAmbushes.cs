using System;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Events;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(GeoscapeEventSystem), "PhoenixFaction_OnSiteFirstTimeVisited")]
    public static class GeoscapeEventSystem_PhoenixFaction_OnSiteFirstTimeVisited_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.DisableAmbushes;
        }

        public static void Prefix(GeoscapeEventSystem __instance, GeoSite site, ref int ____ambushProtection)
        {
            try
            {
                Logger.Debug($"[GeoscapeEventSystem_PhoenixFaction_OnSiteFirstTimeVisited_PREFIX] Preventing ambush chance.");

                if (site == null)
                {
                    return;
                }

                if (AssortedAdjustments.Settings.RetainAmbushesInsideMist && site.IsInMist)
                {
                    return;
                }

                // This gets subtracted by one and then checked to be zero or below in original method...
                // Resetting it to two effectively disables ambushes.
                if (____ambushProtection < 2)
                {
                    ____ambushProtection = 2;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
