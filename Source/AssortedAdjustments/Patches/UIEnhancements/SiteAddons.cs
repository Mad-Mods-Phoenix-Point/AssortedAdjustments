using System;
using Harmony;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Common.Core;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class SiteAddons
    {
        [HarmonyPatch(typeof(GeoSiteVisualsController), "RefreshSiteVisuals")]
        public static class GeoSiteVisualsController_RefreshSiteVisuals_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.HideSiteAddons;
            }

            public static void Postfix(GeoSiteVisualsController __instance, GeoSite site)
            {
                try
                {
                    if (site.Type != GeoSiteType.Haven)
                    {
                        return;
                    }

                    // See lines 167ff in original method
                    if (__instance.SiteSpecialAddonContainer.transform.childCount > 0)
                    {
                        Logger.Debug($"[GeoSiteVisualsController_RefreshSiteVisuals_POSTFIX] Hiding special addons of site: {site.Name}");
                        __instance.SiteSpecialAddonContainer.SetActive(false);
                    }
                    if (__instance.SiteUniqueAddonContainer.transform.childCount > 0)
                    {
                        Logger.Debug($"[GeoSiteVisualsController_RefreshSiteVisuals_POSTFIX] Hiding unique addons of site: {site.Name}");
                        __instance.SiteUniqueAddonContainer.SetActive(false);
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
