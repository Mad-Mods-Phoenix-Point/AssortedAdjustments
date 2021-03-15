using Harmony;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewControllers.Manufacturing;
using PhoenixPoint.Geoscape.View.ViewModules;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AssortedAdjustments.Patches.Bugfixes
{
    [HarmonyPatch(typeof(UIModuleManufacturing), "CancelItem")]
    public static class UIModuleManufacturing_CancelItem_Patch
    {
        // Override
        public static bool Prefix(UIModuleManufacturing __instance, GeoManufactureQueueItem item, GeoPhoenixFaction ____faction)
        {
            try
            {
                // Original method (Really bad... GetSiblingIndex() isn't for runtime and doesn't check inactive objects.)
                //if (this.Mode == UIModuleManufacturing.UIMode.Manufacture)
                //{
                //    int index = item.transform.GetSiblingIndex() + 1;
                //    this._faction.Manufacture.Cancel(index);
                //    if (item.QueueElement != this._faction.Manufacture.Current)
                //    {
                //        this.SetupQueue();
                //    }
                //}


                Logger.Info($"[UIModuleManufacturing_CancelItem_PREFIX] Cancel item: {item.QueueElement.ManufacturableItem.Name.Localize()}");

                if (__instance.Mode == UIModuleManufacturing.UIMode.Manufacture)
                {
                    // See extension method at Extensions.ItemManufacturingExtensions
                    ____faction.Manufacture.Cancel(item.QueueElement);

                    if (item.QueueElement != ____faction.Manufacture.Current)
                    {
                        typeof(UIModuleManufacturing).GetMethod("SetupQueue", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null);
                    }
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }
}
