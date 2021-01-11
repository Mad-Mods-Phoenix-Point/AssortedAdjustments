using System;
using Harmony;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Common.Core;
using Base.UI;
using PhoenixPoint.Geoscape.Entities.Sites;
using UnityEngine;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class ExtendedHavenInfo
    {
        private static string WordToTitleCase(string s)
        {
            return String.Concat(s.First().ToString().ToUpper(), s.Remove(0, 1).ToLower());
        }



        // Show recruit class on haven popup
        [HarmonyPatch(typeof(UIModuleSelectionInfoBox), "SetHaven")]
        public static class UIModuleSelectionInfoBox_SetHaven_Patch
        {
            internal static string recruitAvailableText = "";

            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.ShowExtendedHavenInfo;
            }

            public static void Postfix(UIModuleSelectionInfoBox __instance, GeoSite ____site, bool showRecruits)
            {
                try
                {
                    Logger.Debug($"[UIModuleSelectionInfoBox_SetHaven_POSTFIX] Haven: {____site.Name}");

                    if (!showRecruits)
                    {
                        return;
                    }

                    GeoUnitDescriptor recruit = ____site.GetComponent<GeoHaven>()?.AvailableRecruit;
                    if (recruit == null)
                    {
                        return;
                    }

                    string className = recruit.Progression.MainSpecDef.ViewElementDef.DisplayName1.Localize();
                    string level = recruit.Level.ToString();

                    if (String.IsNullOrEmpty(recruitAvailableText))
                    {
                        recruitAvailableText = WordToTitleCase(__instance.RecruitAvailableText.text.Split((char)32).First() + ":");
                    }
                    __instance.RecruitAvailableText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    __instance.RecruitAvailableText.text = $"{recruitAvailableText} <color=#f4a22c>{className}</color> (Level {level})";
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        // Show trading info on haven popup
        [HarmonyPatch(typeof(UIModuleSelectionInfoBox), "SetHaven")]
        public static class UIModuleSelectionInfoBox_SetHaven_Patch2
        {
            internal static string sitePopulationText = "";

            private static string GetResourceName(ResourceType type)
            {
                switch (type)
                {
                    case ResourceType.Materials: return new LocalizedTextBind("Geoscape/KEY_GEOSCAPE_MATERIALS").Localize();
                    case ResourceType.Supplies: return new LocalizedTextBind("Geoscape/KEY_GEOSCAPE_FOOD").Localize();
                    case ResourceType.Tech: return new LocalizedTextBind("Geoscape/KEY_GEOSCAPE_TECH").Localize();
                }
                return type.ToString();
            }

            private static string GetResourceEntry(int quantity, ResourceType type, int substring = 1)
            {
                string name = GetResourceName(type);
                if (name.Length > substring)
                {
                    name = name.Substring(0, substring);
                }
                if (name.Length > 0)
                {
                    name = $" {WordToTitleCase(name)}";
                }
                switch (type)
                {
                    case ResourceType.Materials:
                        return $"<color=#ed6e2b>{quantity}{name}</color>";

                    case ResourceType.Supplies:
                        return $"<color=#3def1b>{quantity}{name}</color>";

                    case ResourceType.Tech:
                        return $"<color=#1893e1>{quantity}{name}</color>";
                }
                return $"<color=#FFFFFF>{quantity}{name}</color>";
            }

            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.ShowExtendedHavenInfo;
            }

            public static void Postfix(UIModuleSelectionInfoBox __instance, GeoSite ____site)
            {
                try
                {
                    Logger.Debug($"[UIModuleSelectionInfoBox_SetHaven_POSTFIX] Haven: {____site.Name}");

                    List<HavenTradingEntry> resourcesAvailable = ____site.GetComponent<GeoHaven>()?.GetResourceTrading();
                    Text textAnchor = __instance.SitePopulationText;

                    if (resourcesAvailable?.Count > 0 && textAnchor != null)
                    {
                        string format = "<size=26>Exchange {0} for {1} ({2})</size>\n";

                        if (String.IsNullOrEmpty(sitePopulationText))
                        {
                            sitePopulationText = textAnchor.text; 
                        }
                        textAnchor.horizontalOverflow = HorizontalWrapMode.Overflow;
                        textAnchor.lineSpacing = 0.8f;

                        textAnchor.text = $"{sitePopulationText}\n\n";
                        textAnchor.text += string.Concat(resourcesAvailable.Select(e => string.Format(format, GetResourceEntry(e.HavenReceiveQuantity, e.HavenWants, 99), GetResourceEntry(e.HavenOfferQuantity, e.HavenOffers, 99), e.ResourceStock)));
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
