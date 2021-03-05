using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using System;
using System.Reflection;

namespace AssortedAdjustments.LimitedWar
{
    // Raise haven alertness after attacks happened
    [HarmonyPatch(typeof(GeoSite), "DestroySite")]
    public static class GeoSite_DestroySite_Patch_RaiseAlertness
    {
        public static bool Prepare()
        {
            return Config.Enable && (Config.AttacksRaiseHavenAlertness || Config.AttacksRaiseFactionAlertness);
        }

        public static void Postfix(GeoSite __instance)
        {
            try
            {
                GeoHaven haven = Store.DefenseMission?.Haven;
                GeoFaction owner = haven?.Site?.Owner;
                if (haven == null || Resolver.IsAlienOrPhoenix(owner))
                {
                    return;
                }

                if (Config.AttacksRaiseFactionAlertness && owner != null)
                {
                    Logger.Info($"{haven.Site.Name} has lost. Raising alertness of {owner.GetPPName()}");
                    
                    // @ToDo: Limit the alertness raise to havens in range?
                    //foreach(GeoHaven.HavenRangeEntry hre in haven.HavensByRange)
                    //{
                    //    GeoHaven h = hre.Haven;
                    //    typeof(GeoHaven).GetMethod("IncreaseAlertness", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(h, null);
                    //}

                    foreach (GeoHaven h in owner.Havens)
                    {
                        typeof(GeoHaven).GetMethod("IncreaseAlertness", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(h, null);
                    }
                }
                else if (haven.Site?.State == GeoSiteState.Functioning)
                {
                    typeof(GeoHaven).GetMethod("IncreaseAlertness", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(haven, null);
                    Logger.Info($"{haven.Site.Name} has lost. It is now on {haven.AlertLevel}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}