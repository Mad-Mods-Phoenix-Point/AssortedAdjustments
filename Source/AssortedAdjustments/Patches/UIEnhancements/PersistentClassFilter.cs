using System;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Common.Entities.GameTagsTypes;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class PersistentClassFilter
    {
        internal static List<ClassTagDef> ClassFilterState;
        internal static bool ClassFilterInitialized = false;



        // Remember current filter state when switching from manufacture to scrap and vice versa
        [HarmonyPatch(typeof(UIModuleManufacturing), "DoFilter")]
        public static class UIModuleManufacturing_DoFilter_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.PersistentClassFilter;
            }

            public static void Prefix(UIModuleManufacturing __instance, List<ClassTagDef> ____classFilter)
            {
                try
                {
                    ClassFilterState = ____classFilter.ToList();
                    Logger.Info($"[UIModuleManufacturing_DoFilter_PREFIX] ClassFilterState: {String.Join(", ", ClassFilterState.Select(t => t.className))}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIModuleManufacturing), "SetClassFilters")]
        public static class UIModuleManufacturing_SetClassFilters_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.PersistentClassFilter;
            }

            public static void Prefix(UIModuleManufacturing __instance, ref List<ClassTagDef> ____classFilter, List<ClassTagDef> ____availableClassTags)
            {
                try
                {
                    if (!ClassFilterInitialized)
                    {
                        if (AssortedAdjustments.Settings.PersistentClassFilterInitDisabled)
                        {
                            ClassFilterState = new List<ClassTagDef>();
                        }
                        else
                        {
                            ClassFilterState = ____availableClassTags.ToList();
                        }
                        ClassFilterInitialized = true;
                    }

                    ____classFilter = ClassFilterState.ToList();
                    Logger.Info($"[UIModuleManufacturing_SetClassFilters_PREFIX] ____classFilter: {String.Join(", ", ____classFilter.Select(t => t.className))}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            public static void Postfix(UIModuleManufacturing __instance)
            {
                try
                {
                    //MethodInfo ___DoFilter = typeof(UIModuleManufacturing).GetMethod("DoFilter", BindingFlags.NonPublic | BindingFlags.Instance);
                    //___DoFilter.Invoke(__instance, new object[] { null, null });
                    //__instance.ClassFiltersNavHolder.RefreshInteractableList();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
