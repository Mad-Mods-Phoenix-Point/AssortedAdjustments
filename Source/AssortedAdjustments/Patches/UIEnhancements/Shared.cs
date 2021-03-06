﻿using System;
using Harmony;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using PhoenixPoint.Geoscape.View.ViewControllers.BaseRecruits;
using PhoenixPoint.Geoscape.View.DataObjects;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View;
using System.Reflection;
using Base.UI;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class Shared
    {
        // Don't display [+ 0] in resources area of the info bar for the LOTA resources as it doesn't make any sense.
        [HarmonyPatch(typeof(UIAnimatedResourceController), "DisplayValue")]
        public static class UIAnimatedResourceController_DisplayValue_Patch
        {
            public static void Postfix(UIAnimatedResourceController __instance, Text ____text)
            {
                try
                {
                    Transform resourceTransform = __instance.transform.parent?.transform;

                    if (resourceTransform == null)
                    {
                        return;
                    }

                    if (resourceTransform.name.Contains("LivingCrystalsRes") || resourceTransform.name.Contains("OrichalcumRes") || resourceTransform.name.Contains("ProteanMutaneRes"))
                    {
                        Logger.Debug($"[UIAnimatedResourceController_DisplayValue_POSTFIX] Adjust display for LOTA resource: {resourceTransform.name}");
                        
                        // Set text without value in brackets
                        ____text.text = __instance.DisplayedValue.ToString();

                        // Shrink required space
                        LayoutElement layout = resourceTransform.GetComponent<LayoutElement>();
                        layout.preferredWidth = 100f;
                        layout.minWidth = 85f;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Uncovered, but not yet excavated ancient sites will be displayed in light gray
        [HarmonyPatch(typeof(GeoSiteVisualsController), "RefreshSiteVisuals")]
        public static class GeoSiteVisualsController_RefreshSiteVisuals_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements;
            }

            public static void Postfix(GeoSiteVisualsController __instance, GeoSite site)
            {
                try
                {
                    if (!site.IsArcheologySite)
                    {
                        return;
                    }

                    GeoSiteVisualsDefs instance = GeoSiteVisualsDefs.Instance;
                    if (site.Owner.IsEnvironmentFaction && site.GetVisible(__instance.Viewer) && !site.IsExcavated())
                    {
                        Material customMaterial = instance.GetAncientSite(false);
                        customMaterial.color = new Color32(224, 224, 224, 255);

                        //__instance.ReplaceSiteVisuals(customMaterial);
                        typeof(GeoSiteVisualsController).GetMethod("ReplaceSiteVisuals", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { customMaterial });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Add class tooltip to RecruitsListElements and beautify the perk titles
        [HarmonyPatch(typeof(RecruitsListElementController), "SetRecruitElement")]
        public static class RecruitsListElementController_SetRecruitElement_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements;
            }

            public static void Postfix(RecruitsListElementController __instance, RecruitsListEntryData entryData, List<RowIconTextController> ____abilityIcons)
            {
                try
                {
                    Transform anchor = __instance.RecruitName.transform;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //Logger.Debug($"{anchor.name}");
                    //Logger.Debug($"{anchor.parent.name}");
                    //Logger.Debug($"{anchor.parent.parent.name}");

                    string classTitle = entryData.Recruit.GetClassViewElementDefs().FirstOrDefault().DisplayName1.Localize();
                    string classDesc = entryData.Recruit.GetClassViewElementDefs().FirstOrDefault().DisplayName2.Localize();
                    string tipText = $"{classTitle} - {classDesc}";

                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        Logger.Debug($"[RecruitsListElementController_SetRecruitElement_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        Logger.Debug($"[RecruitsListElementController_SetRecruitElement_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().TextColor = Color.white;
                        anchorGo.GetComponent<UITooltipText>().Position = UITooltip.Position.Bottom;
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                    }

                    // Fix UI on the fly :-)
                    foreach (RowIconTextController c in ____abilityIcons)
                    {
                        c.DisplayText.fontSize = 26;
                        c.DisplayText.lineSpacing = 0.8f;
                        c.DisplayText.resizeTextForBestFit = false;
                        c.DisplayText.alignment = TextAnchor.MiddleLeft;
                        c.DisplayText.horizontalOverflow = HorizontalWrapMode.Wrap;
                        c.DisplayText.verticalOverflow = VerticalWrapMode.Overflow;

                        /*
                        string s = c.DisplayText.text;
                        Logger.Debug($"[RecruitsListElementController_SetRecruitElement_POSTFIX] Original string: {s}");
                        string[] a = s.Split((char)32);
                        string r = "";
                        for (int i = 0; i < a.Length; i++)
                        {
                            r += a[i];

                            if (i < a.Length-1) {
                                r += " \n";
                            }
                        }
                        Logger.Debug($"[RecruitsListElementController_SetRecruitElement_POSTFIX] Modified string: {r}");

                        c.DisplayText.text = r;
                        */
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Enable modifications of simple tooltips via string commands
        [HarmonyPatch(typeof(UITooltipText), "OnMouseEnter")]
        public static class UITooltipText_OnMouseEnter_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements;
            }

            public static void Postfix(UITooltipText __instance, GameObject ____widget)
            {
                try
                {
                    if (!__instance.Enabled)
                    {
                        return;
                    }

                    // CONTROL TAGS
                    // @ToDo: Allow multiple commands (ALPHA, BGIMAGE, MINWIDTH)?
                    if (__instance.TipText.Contains("<!--") && __instance.TipText.Contains("-->"))
                    {
                        //Logger.Info($"[UITooltipText_OnMouseEnter_POSTFIX] CONTROL TAGS found in TipText: {__instance.TipText}");

                        string s = __instance.TipText;
                        int ctrlStart = s.IndexOf("<!--");
                        int ctrlEnd = s.IndexOf("-->") + 3;
                        int ctrlLength = ctrlEnd - ctrlStart;

                        int cmdStart = s.IndexOf("<!--") + 4;
                        int cmdEnd = s.IndexOf("-->");
                        int cmdLength = cmdEnd - cmdStart;

                        string cmd = s.Substring(cmdStart, cmdLength);
                        Logger.Info($"cmd: {cmd}");

                        if (cmd.Contains("FONTSIZE") && int.TryParse(Regex.Match(cmd, @"\d+").Value, out int fontSize))
                        {
                            Logger.Info($"fontSize: {fontSize}");
                            UITooltip uiTooltip = ____widget?.GetComponent<UITooltip>();
                            Text text = uiTooltip.Text;
                            text.fontSize = fontSize;
                            text.lineSpacing = 1f;
                        }

                        // Cleanup
                        __instance.UpdateText(s.Remove(ctrlStart, ctrlLength));
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Strip string commands if called by update
        [HarmonyPatch(typeof(UITooltipText), "UpdateText")]
        public static class UITooltipText_UpdateText_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements;
            }

            public static void Prefix(UITooltipText __instance, ref string tipText)
            {
                try
                {
                    if (!__instance.Enabled)
                    {
                        return;
                    }

                    // Remove CONTROL TAGS
                    if (tipText.Contains("<!--") && tipText.Contains("-->"))
                    {
                        //Logger.Info($"[UITooltipText_UpdateText_PREFIX] CONTROL TAGS found in tipText: {tipText}");

                        int ctrlStart = tipText.IndexOf("<!--");
                        int ctrlEnd = tipText.IndexOf("-->") + 3;
                        int ctrlLength = ctrlEnd - ctrlStart;

                        // Cleanup
                        tipText = tipText.Remove(ctrlStart, ctrlLength);
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
