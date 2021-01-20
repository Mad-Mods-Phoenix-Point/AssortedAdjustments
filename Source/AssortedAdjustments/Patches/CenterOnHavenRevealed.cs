using System;
using System.Reflection;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.View;
using PhoenixPoint.Geoscape.View.ViewStates;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(UIStateVehicleSelected), "OnVehicleSiteExplored")]
    public static class UIStateVehicleSelected_OnVehicleSiteExplored_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.CenterOnHavenRevealed;
        }

        public static void Prefix(UIStateVehicleSelected __instance, GeoVehicle vehicle)
        {
            try
            {
                if(vehicle.CurrentSite.Type != GeoSiteType.Haven)
                {
                    return;
                }

                // @ToDo: Somethings broken with this (or even vanilla). When selecting the vehicle while currently having an other verhicle selected
                // the game will still draw a travel path from the other verhicle when clicking on the site (or any other site)
                // So, selection without using input/ui is bugging out apparently
                
                GeoscapeViewContext ___Context = (GeoscapeViewContext)AccessTools.Property(typeof(GeoscapeViewState), "Context").GetValue(__instance, null);
                GeoVehicle ___SelectedVehicle = (GeoVehicle)AccessTools.Property(typeof(UIStateVehicleSelected), "SelectedVehicle").GetValue(__instance, null);

                Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] vehicle: {vehicle.name}");
                Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] ___SelectedVehicle: {___SelectedVehicle.name}");
                Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] ___Context.View.SelectedActor: {___Context.View.SelectedActor.name}");
                Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] {vehicle?.Name} revealed haven: {vehicle.CurrentSite?.Name}.");

                if (vehicle != ___Context.View.SelectedActor)
                {
                    Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] Selecting vehicle.");
                    typeof(UIStateVehicleSelected).GetMethod("SelectVehicle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { vehicle, true });
                }
                else
                {
                    Logger.Info($"[GeoFaction_OnVehicleSiteExplored_PREFIX] Chase vehicle.");
                    ___Context.View.ChaseTarget(vehicle, false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
