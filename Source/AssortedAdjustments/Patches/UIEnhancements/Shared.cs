using System;
using Harmony;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class Shared
    {
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
                    if (__instance.TipText.Contains("<!--") && __instance.TipText.Contains("-->"))
                    {
                        Logger.Info($"[UITooltipText_OnMouseEnter_POSTFIX] CONTROL TAGS found in TipText: {__instance.TipText}");

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
    }
}
