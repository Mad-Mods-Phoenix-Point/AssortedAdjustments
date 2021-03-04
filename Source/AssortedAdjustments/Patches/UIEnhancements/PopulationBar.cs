using System;
using Harmony;
using PhoenixPoint.Common.Game;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewModules;
using UnityEngine;
using UnityEngine.UI;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class PopulationBar
    {
        internal static bool moduleInfoBarAdjustmentsExecuted = false;

        internal static Color green = new Color32(93, 153, 106, 255);
        internal static Color yellow = new Color32(251, 191, 31, 255);
        internal static Color red = new Color32(192, 32, 32, 255);
        internal static Color gray = new Color32(192, 192, 192, 255);



        [HarmonyPatch(typeof(UIModuleInfoBar), "UpdatePopulation")]
        public static class UIModuleInfoBar_UpdatePopulation_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.HidePopulationBar;
            }

            public static void Postfix(UIModuleInfoBar __instance, GeoscapeViewContext ____context)
            {
                try
                {
                    float populationRemaining = (float)____context.View.WorldPopulation / (float)____context.View.StartingWorldPopulation;
                    float populationThreshold = (float)____context.View.GameOverWorldPopulation / (float)____context.View.StartingWorldPopulation;
                    int populationRemainingPercent = (int)Mathf.Ceil(populationRemaining * 100f);
                    int populationThresholdPercent = (int)Mathf.Ceil(populationThreshold * 100f);
                    
                    if(populationRemainingPercent <= (populationThresholdPercent + 10))
                    {
                        __instance.PopulationPercentageText.color = red;
                    }
                    else if (populationRemainingPercent < 50)
                    {
                        __instance.PopulationPercentageText.color = yellow;
                    }
                    else
                    {
                        //__instance.PopulationPercentageText.color = green;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleInfoBar), "Init")]
        public static class UIModuleInfoBar_Init_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && (AssortedAdjustments.Settings.CompressPopulationBar || AssortedAdjustments.Settings.HidePopulationBar);
            }

            public static void Prefix(UIModuleInfoBar __instance, GeoscapeViewContext context)
            {
                try
                {
                    Logger.Debug($"[UIModuleInfoBar_Init_PREFIX] moduleInfoBarAdjustmentsExecuted: {moduleInfoBarAdjustmentsExecuted}");

                    if (moduleInfoBarAdjustmentsExecuted)
                    {
                        return;
                    }


                    LayoutElement layoutPopulationBar = __instance.PopulationBarRoot.GetComponent<LayoutElement>();

                    if (AssortedAdjustments.Settings.CompressPopulationBar)
                    {
                        layoutPopulationBar.preferredWidth = 750f;
                        layoutPopulationBar.minWidth = 750f;

                        __instance.PopulationDeadTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 575f);
                        __instance.PopulationMinTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 575f);
                        __instance.PopulationAllTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 575f);
                    }


                    if (AssortedAdjustments.Settings.HidePopulationBar)
                    {
                        layoutPopulationBar.preferredWidth = 300f;
                        layoutPopulationBar.minWidth = 300f;

                        __instance.PopulationDeadTransform.gameObject.SetActive(false);
                        __instance.PopulationMinTransform.gameObject.SetActive(false);
                        __instance.PopulationAllTransform.gameObject.SetActive(false);
                    }

                    // Declutter
                    Transform tInfoBar = __instance.PopulationBarRoot.transform.parent?.transform;
                    foreach (Transform t in tInfoBar.GetComponentsInChildren<Transform>())
                    {
                        //Logger.Debug($"[UIModuleInfoBar_Init_PREFIX] Transform.name: {t.name}");

                        // Hide useless icons at production and research
                        if (t.name == "UI_Clock")
                        {
                            t.gameObject.SetActive(false);
                        }

                        // Reposition skull icon
                        if (t.name == "skull")
                        {
                            float populationThreshold = (float)context.View.GameOverWorldPopulation / (float)context.View.StartingWorldPopulation;
                            int populationThresholdPercent = (int)Mathf.Ceil(populationThreshold * 100f);
                            Logger.Info($"[UIModuleInfoBar_Init_PREFIX] populationThresholdPercent: {populationThresholdPercent}");

                            // Put it inside the bar
                            if (populationThresholdPercent >= 8)
                            {
                                t.GetComponentInChildren<Image>().color = Color.black;
                                t.localScale = new Vector3(0.7f, 0.7f, 1f);
                                t.Translate(new Vector3(-22f, 28f, 0f));

                                //float translateX = -1 * (6 + (populationThresholdPercent * 8 / 5));
                                //Logger.Info($"[UIModuleInfoBar_Init_PREFIX] translateX: {translateX}");
                                //t.Translate(new Vector3(translateX, 28f, 0f));
                            }
                            // No space so simply hide it
                            else
                            {
                                t.gameObject.SetActive(false);
                            }
                        }

                        // Shrink indicator arrow?
                        //if (t.name == "bar")
                        //{
                        //    t.localScale = new Vector3(0.7f, 0.7f, 1f);
                        //    t.Translate(new Vector3(-1f, -1f, 0f));
                        //}

                        // Colorize the red part of the bar to get rid of the black crosses
                        if (t.name == "tiled_gameover")
                        {
                            // Turn off original...
                            t.gameObject.SetActive(false);
                        }
                        // ...and add a new image to fill the space instead
                        if (t.name == "gameover")
                        {
                            //if(t.gameObject.GetComponents<Image>().Count() > 0)
                            //{
                            //    continue;
                            //}

                            Image i = t.gameObject.AddComponent<Image>();
                            i.transform.localScale = new Vector3(1f, 0.99f, 1f);
                            i.color = red;
                        }
                    }

                    // Set a flag so that this whole stuff is only done ONCE
                    // Otherwise the visual transformations are repeated everytime leading to weird results
                    // This is reset on every level change (see below)
                    moduleInfoBarAdjustmentsExecuted = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(PhoenixGame), "RunGameLevel")]
        public static class PhoenixGame_RunGameLevel_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && (AssortedAdjustments.Settings.CompressPopulationBar || AssortedAdjustments.Settings.HidePopulationBar);
            }

            public static void Prefix()
            {
                moduleInfoBarAdjustmentsExecuted = false;
            }
        }
    }
}
