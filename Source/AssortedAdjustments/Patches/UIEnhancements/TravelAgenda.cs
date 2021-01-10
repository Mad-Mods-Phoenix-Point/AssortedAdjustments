using System;
using System.Reflection;
using Harmony;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Entities;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Geoscape.View.ViewControllers;
using PhoenixPoint.Geoscape.View.ViewStates;
using Base.Core;
using PhoenixPoint.Geoscape.Entities.Abilities;
using Base.UI;
using PhoenixPoint.Common.UI;
using UnityEngine.UI;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities.PhoenixBases;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.View.DataObjects;
using PhoenixPoint.Geoscape.View;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class TravelAgenda
    {
        internal static bool fetchedSiteNames = false;
        internal static string unexploredSiteName = "UNEXPLORED SITE";
        internal static string explorationSiteName = "EXPLORATION SITE";
        internal static string scavengingSiteName = "SCAVENGING SITE";
        internal static string ancientSiteName = "ANCIENT SITE";

        internal static string actionExploring = "INVESTIGATES";
        internal static string actionTraveling = "TRAVELS TO";
        internal static string actionRepairing = "REPAIRING:";

        internal static Image aircraftIcon = null;

        internal static Color vehicleTrackerColor = new Color32(251, 191, 31, 255);
        internal static Color manufactureTrackerColor = new Color32(235, 110, 42, 255);
        internal static Color researchTrackerColor = new Color32(42, 245, 252, 255);
        internal static Color facilityTrackerColor = new Color32(185, 185, 185, 255);
        internal static Color defaultTrackerColor = new Color32(155, 155, 155, 255);
        internal static UIFactionDataTrackerElement trackerElementDefault = null;

        // Helpers
        private static bool GetTravelTime(GeoVehicle vehicle, out float travelTime, GeoSite target = null)
        {
            travelTime = 0f;

            if (target == null && vehicle.FinalDestination == null)
            {
                return false;
            }

            var currentPosition = vehicle.CurrentSite?.WorldPosition ?? vehicle.WorldPosition;
            var targetPosition = target == null ? vehicle.FinalDestination.WorldPosition : target.WorldPosition;
            var travelPath = vehicle.Navigation.FindPath(currentPosition, targetPosition, out bool hasTravelPath);

            if (!hasTravelPath || travelPath.Count < 2)
            {
                return false;
            }

            float distance = 0;

            for (int i = 0, len = travelPath.Count - 1; i < len;)
            {
                distance += GeoMap.Distance(travelPath[i].Pos.WorldPosition, travelPath[++i].Pos.WorldPosition).Value;
            }

            travelTime = distance / vehicle.Stats.Speed.Value;
            //Logger.Info($"[TravelAgenda_GetTravelTime] travelTime: {travelTime}");

            return true;
        }

        private static float GetExplorationTime(GeoVehicle vehicle, float hours)
        {
            try
            {
                if (vehicle == null)
                {
                    return hours;
                }

                object updateable = typeof(GeoVehicle).GetField("_explorationUpdateable", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(vehicle);

                if (updateable == null)
                {
                    return hours;
                }

                NextUpdate endTime = (NextUpdate)updateable.GetType().GetProperty("NextUpdate").GetValue(updateable);
                return (float)-(vehicle.Timing.Now - endTime.NextTime).TimeSpan.TotalHours;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return hours;
            }
        }

        private static string GetSiteName(GeoSite site, GeoFaction faction)
        {
            string siteName = null;

            if (String.IsNullOrEmpty(siteName))
            {
                if (site.GetInspected(faction))
                {
                    if (site.Type == GeoSiteType.PhoenixBase)
                    {
                        siteName = site.Name;
                    }
                    else if (site.Type == GeoSiteType.Haven)
                    {
                        siteName = site.Name;
                    }
                    else if (site.Type == GeoSiteType.AlienBase)
                    {
                        GeoAlienBase alienBase = site.GetComponent<GeoAlienBase>();
                        GeoAlienBaseTypeDef alienBaseTypeDef = alienBase.AlienBaseTypeDef;

                        siteName = alienBaseTypeDef.Name.Localize();
                    }
                    else if (site.Type == GeoSiteType.Scavenging)
                    {
                        siteName = scavengingSiteName;
                    }
                    else if (site.Type == GeoSiteType.Exploration)
                    {
                        siteName = explorationSiteName;
                    }
                    else if (site.IsArcheologySite)
                    {
                        siteName = ancientSiteName;
                    }
                }
                else
                {
                    siteName = unexploredSiteName;
                }
            }

            // Last resort
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = "POI";
            }

            return $"{siteName}";
        }

        private static string AppendTime(float hours)
        {
            string prefix = "   ~ ";
            string time = HoursToText(hours);
            string postfix = "";

            return $"{prefix}{time}{postfix}";
        }

        private static string HoursToText(float hours)
        {
            TimeUnit timeUnit = TimeUnit.FromHours(hours);
            TimeRemainingFormatterDef timeFormatter = new TimeRemainingFormatterDef();
            timeFormatter.DaysText = new LocalizedTextBind("{0}d", true);
            timeFormatter.HoursText = new LocalizedTextBind("{0}h", true);
            string timeString = UIUtil.FormatTimeRemaining(timeUnit, timeFormatter);

            return timeString;
        }



        // Patches
        [HarmonyPatch(typeof(UIStateVehicleSelected), "EnterState")]
        public static class UIStateVehicleSelected_EnterState_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIStateVehicleSelected __instance)
            {
                try
                {
                    if(!fetchedSiteNames)
                    {
                        UIModuleSelectionInfoBox ____selectionInfoBoxModule = (UIModuleSelectionInfoBox)AccessTools.Property(typeof(UIStateVehicleSelected), "_selectionInfoBoxModule").GetValue(__instance, null);

                        unexploredSiteName = ____selectionInfoBoxModule.UnexploredSiteTextKey.Localize();
                        explorationSiteName = ____selectionInfoBoxModule.ExplorationSiteTextKey.Localize();
                        scavengingSiteName = ____selectionInfoBoxModule.ScavengingSiteTextKey.Localize();
                        ancientSiteName = ____selectionInfoBoxModule.AncientSiteTextKey.Localize();

                        //Logger.Info($"[UIStateVehicleSelected_EnterState_POSTFIX] unexploredSiteName: {unexploredSiteName}");
                        //Logger.Info($"[UIStateVehicleSelected_EnterState_POSTFIX] explorationSiteName: {explorationSiteName}");
                        //Logger.Info($"[UIStateVehicleSelected_EnterState_POSTFIX] scavengingSiteName: {scavengingSiteName}");
                        //Logger.Info($"[UIStateVehicleSelected_EnterState_POSTFIX] ancientSiteName: {ancientSiteName}");

                        fetchedSiteNames = true;
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
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIModuleInfoBar __instance)
            {
                try
                {
                    if (aircraftIcon == null)
                    {
                        aircraftIcon = __instance.AirVehiclesLabel.transform.parent.gameObject.GetComponentInChildren<Image>(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIFactionDataTrackerElement), "Init")]
        public static class UIFactionDataTrackerElement_Init_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIFactionDataTrackerElement __instance, object objToTrack, string text)
            {
                try
                {
                    //Logger.Debug($"[UIFactionDataTrackerElement_Init_POSTFIX] objToTrack: {objToTrack}, text: {text}");

                    __instance.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    __instance.TrackedTime.alignment = TextAnchor.MiddleRight;
                    
                    __instance.TrackedTime.fontSize = 36;
                    __instance.TrackedName.fontSize = 36;

                    // As the elements are re-used and NOT recreated they have to be reset to their default visuals first.
                    // Otherwise, if no condition below is true they retain the colors of their old item, resulting in wrongly colored elements!
                    if (trackerElementDefault != null)
                    {
                        __instance.TrackedName.color = trackerElementDefault.TrackedName.color;
                        __instance.TrackedTime.color = trackerElementDefault.TrackedTime.color;
                        __instance.Icon.color = trackerElementDefault.Icon.color;
                    }
                    else
                    {
                        __instance.TrackedName.color = defaultTrackerColor;
                        __instance.TrackedTime.color = defaultTrackerColor;
                        __instance.Icon.color = defaultTrackerColor;
                    }

                    if (__instance.TrackedObject is GeoVehicle vehicle)
                    {
                        //Logger.Info($"[UIFactionDataTrackerElement_Init_POSTFIX] TrackedObject is VEHICLE ({vehicle.Name}). Setting colors...");

                        __instance.TrackedName.color = vehicleTrackerColor;
                        __instance.TrackedTime.color = vehicleTrackerColor;
                        __instance.Icon.color = vehicleTrackerColor;
                        //__instance.Icon.sprite = def.InventoryIcon; // SmallIcon, LargeIcon, InventoryIcon, RosterIcon

                        // Borrowed from UIModuleInfoBar, fetched at UIModuleInfoBar.Init()
                        __instance.Icon.sprite = aircraftIcon.sprite;
                    }
                    else if(__instance.TrackedObject is ResearchElement re)
                    {
                        //Logger.Info($"[UIFactionDataTrackerElement_Init_POSTFIX] TrackedObject is ResearchElement ({re.GetLocalizedName()}). Setting colors...");

                        __instance.TrackedName.color = researchTrackerColor;
                        __instance.TrackedTime.color = researchTrackerColor;
                        __instance.Icon.color = researchTrackerColor;
                    }
                    else if (__instance.TrackedObject is ItemManufacturing.ManufactureQueueItem mqi)
                    {
                        //Logger.Info($"[UIFactionDataTrackerElement_Init_POSTFIX] TrackedObject is ItemManufacturing.ManufactureQueueItem ({mqi.ManufacturableItem.Name}). Setting colors...");

                        __instance.TrackedName.color = manufactureTrackerColor;
                        __instance.TrackedTime.color = manufactureTrackerColor;
                        __instance.Icon.color = manufactureTrackerColor;
                    }
                    else if (__instance.TrackedObject is GeoPhoenixFacility gpf)
                    {
                        //Logger.Info($"[UIFactionDataTrackerElement_Init_POSTFIX] TrackedObject is GeoPhoenixFacility ({gpf.PxBase.Site.Name}). Setting colors...");

                        if (gpf.IsRepairing)
                        {
                            __instance.TrackedName.text = text; // Overriding viewdef
                        }
                        
                        //__instance.TrackedName.color = facilityTrackerColor;
                        //__instance.TrackedTime.color = facilityTrackerColor;
                        //__instance.Icon.color = facilityTrackerColor;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIFactionDataTrackerElement), "SetTime")]
        public static class UIFactionDataTrackerElement_SetTime_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIFactionDataTrackerElement __instance)
            {
                try
                {
                    string org = __instance.TrackedTime.text;
                    string pre = "~ ";

                    __instance.TrackedTime.text = $"{pre}{org}";
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateVehicleSelected), "AddTravelSite")]
        public static class UIStateVehicleSelected_AddTravelSite_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIStateVehicleSelected __instance, GeoSite site)
            {
                try
                {
                    GeoVehicle ___SelectedVehicle = (GeoVehicle)AccessTools.Property(typeof(UIStateVehicleSelected), "SelectedVehicle").GetValue(__instance, null);
                    MoveVehicleAbility ability = ___SelectedVehicle.GetAbility<MoveVehicleAbility>();

                    if (ability == null)
                    {
                        return;
                    }
                    GeoAbilityTarget target = new GeoAbilityTarget(site);
                    if (!ability.CanActivate(target))
                    {
                        return;
                    }

                    GeoVehicle vehicle = ___SelectedVehicle;
                    string siteName = GetSiteName(vehicle.FinalDestination, vehicle.Owner);
                    string vehicleInfo = $"{vehicle.VehicleID} {actionTraveling} {siteName}";

                    UIModuleFactionAgendaTracker ____factionTracker = (UIModuleFactionAgendaTracker)AccessTools.Property(typeof(UIStateVehicleSelected), "_factionTracker").GetValue(__instance, null);
                    List<UIFactionDataTrackerElement> ____currentTrackedElements = (List<UIFactionDataTrackerElement>)AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_currentTrackedElements").GetValue(____factionTracker);

                    foreach (UIFactionDataTrackerElement trackedElement in ____currentTrackedElements)
                    {
                        // Update
                        if (trackedElement.TrackedObject == vehicle)
                        {
                            Logger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] {vehicle.Name} already tracked. Updating.");

                            trackedElement.TrackedName.text = vehicleInfo;

                            AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_needsRefresh").SetValue(____factionTracker, true);
                            MethodInfo ___UpdateData = typeof(UIModuleFactionAgendaTracker).GetMethod("UpdateData", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                            ___UpdateData.Invoke(____factionTracker, null);

                            return;
                        }
                    }

                    // Add
                    Logger.Debug($"[UIStateVehicleSelected_AddTravelSite_POSTFIX] {vehicle.Name} currently not tracked. Adding to tracker.");

                    MethodInfo ___GetFreeElement = typeof(UIModuleFactionAgendaTracker).GetMethod("GetFreeElement", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo ___OnAddedElement = typeof(UIModuleFactionAgendaTracker).GetMethod("OnAddedElement", BindingFlags.NonPublic | BindingFlags.Instance);

                    UIFactionDataTrackerElement freeElement = (UIFactionDataTrackerElement)___GetFreeElement.Invoke(____factionTracker, null);
                    freeElement.Init(vehicle, vehicleInfo, vehicle.VehicleDef.ViewElement, false);
                    //freeElement.Init(vehicle, vehicleInfo, null, false);

                    ___OnAddedElement.Invoke(____factionTracker, new object[] { freeElement });
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIStateVehicleSelected), "OnVehicleArrived")]
        public static class UIStateVehicleSelected_OnVehicleArrived_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIStateVehicleSelected __instance, GeoVehicle vehicle, bool justPassing)
            {
                try
                {
                    if (justPassing)
                    {
                        return;
                    }

                    // Special update is only needed if the currently selected vehicle arrives at some POI (switching vehicles refreshes the tracker)
                    GeoVehicle ___SelectedVehicle = (GeoVehicle)AccessTools.Property(typeof(UIStateVehicleSelected), "SelectedVehicle").GetValue(__instance, null);
                    if (vehicle != ___SelectedVehicle)
                    {
                        return;
                    }

                    UIModuleFactionAgendaTracker ____factionTracker = (UIModuleFactionAgendaTracker)AccessTools.Property(typeof(UIStateVehicleSelected), "_factionTracker").GetValue(__instance, null);
                    List<UIFactionDataTrackerElement> ____currentTrackedElements = (List<UIFactionDataTrackerElement>)AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_currentTrackedElements").GetValue(____factionTracker);

                    foreach (UIFactionDataTrackerElement trackedElement in ____currentTrackedElements)
                    {
                        // Remove
                        if (trackedElement.TrackedObject == vehicle)
                        {
                            Logger.Debug($"[UIStateVehicleSelected_OnVehicleArrived_POSTFIX] {vehicle.Name} is tracked. Removing.");

                            // Dispose
                            MethodInfo ___Dispose = typeof(UIModuleFactionAgendaTracker).GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance);
                            ___Dispose.Invoke(____factionTracker, new object[] { trackedElement });

                            // And immediately request an update (which sometimes doesn't trigger automatically because of paused state)
                            MethodInfo ___UpdateData = typeof(UIModuleFactionAgendaTracker).GetMethod("UpdateData", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                            ___UpdateData.Invoke(____factionTracker, null);

                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateVehicleSelected), "OnContextualItemSelected")]
        public static class UIStateVehicleSelected_OnContextualItemSelected_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIStateVehicleSelected __instance, GeoAbility ability)
            {
                try
                {
                    Logger.Info($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] ability: {ability.AbilityDef.name}");

                    GeoVehicle vehicle = null;
                    string siteName = "ERR";
                    string vehicleInfo = "ERR";

                    if (ability is MoveVehicleAbility)
                    {
                        vehicle = (GeoVehicle)ability.GeoActor;
                        siteName = GetSiteName(vehicle.FinalDestination, vehicle.Owner);
                        vehicleInfo = $"{vehicle.VehicleID} {actionTraveling} {siteName}";
                    }
                    else if (ability is ExploreSiteAbility)
                    {
                        vehicle = (GeoVehicle)ability.GeoActor;
                        siteName = GetSiteName(vehicle.CurrentSite, vehicle.Owner);
                        vehicleInfo = $"{vehicle.VehicleID} {actionExploring} {siteName}";
                    }
                    
                    if (vehicle == null)
                    {
                        return;
                    }

                    UIModuleFactionAgendaTracker ____factionTracker = (UIModuleFactionAgendaTracker)AccessTools.Property(typeof(UIStateVehicleSelected), "_factionTracker").GetValue(__instance, null);
                    List<UIFactionDataTrackerElement> ____currentTrackedElements = (List<UIFactionDataTrackerElement>)AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_currentTrackedElements").GetValue(____factionTracker);

                    foreach (UIFactionDataTrackerElement trackedElement in ____currentTrackedElements)
                    {
                        // Update
                        if (trackedElement.TrackedObject == vehicle)
                        {
                            Logger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] {vehicle.Name} already tracked. Updating.");

                            trackedElement.TrackedName.text = vehicleInfo;

                            AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_needsRefresh").SetValue(____factionTracker, true);
                            MethodInfo ___UpdateData = typeof(UIModuleFactionAgendaTracker).GetMethod("UpdateData", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                            ___UpdateData.Invoke(____factionTracker, null);

                            return;
                        }
                    }

                    // Add
                    Logger.Debug($"[UIStateVehicleSelected_OnContextualItemSelected_POSTFIX] {vehicle.Name} currently not tracked. Adding to tracker.");

                    MethodInfo ___GetFreeElement = typeof(UIModuleFactionAgendaTracker).GetMethod("GetFreeElement", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo ___OnAddedElement = typeof(UIModuleFactionAgendaTracker).GetMethod("OnAddedElement", BindingFlags.NonPublic | BindingFlags.Instance);

                    UIFactionDataTrackerElement freeElement = (UIFactionDataTrackerElement)___GetFreeElement.Invoke(____factionTracker, null);
                    freeElement.Init(vehicle, vehicleInfo, vehicle.VehicleDef.ViewElement, false);
                    //freeElement.Init(vehicle, vehicleInfo, null, false);

                    ___OnAddedElement.Invoke(____factionTracker, new object[] { freeElement });
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIStateVehicleSelected), "OnVehicleSiteExplored")]
        public static class UIStateVehicleSelected_OnVehicleSiteExplored_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIStateVehicleSelected __instance, GeoVehicle vehicle)
            {
                try
                {
                    UIModuleFactionAgendaTracker ____factionTracker = (UIModuleFactionAgendaTracker)AccessTools.Property(typeof(UIStateVehicleSelected), "_factionTracker").GetValue(__instance, null);
                    List<UIFactionDataTrackerElement> ____currentTrackedElements = (List<UIFactionDataTrackerElement>)AccessTools.Field(typeof(UIModuleFactionAgendaTracker), "_currentTrackedElements").GetValue(____factionTracker);

                    foreach (UIFactionDataTrackerElement trackedElement in ____currentTrackedElements)
                    {
                        // Remove
                        if (trackedElement.TrackedObject == vehicle)
                        {
                            Logger.Debug($"[UIStateVehicleSelected_OnVehicleArrived_POSTFIX] {vehicle.Name} is tracked. Removing.");

                            // Dispose
                            MethodInfo ___Dispose = typeof(UIModuleFactionAgendaTracker).GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance);
                            ___Dispose.Invoke(____factionTracker, new object[] { trackedElement });

                            // And immediately request an update (which sometimes doesn't trigger automatically because of paused state)
                            MethodInfo ___UpdateData = typeof(UIModuleFactionAgendaTracker).GetMethod("UpdateData", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                            ___UpdateData.Invoke(____factionTracker, null);

                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleFactionAgendaTracker), "UpdateData", new Type[] { typeof(UIFactionDataTrackerElement) })]
        public static class UIModuleFactionAgendaTracker_UpdateData_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static bool Prefix(UIModuleFactionAgendaTracker __instance, ref bool __result, UIFactionDataTrackerElement element, GeoFaction ____faction, GeoscapeViewContext ____context)
            {
                try
                {
                    if (element.TrackedObject is GeoVehicle vehicle)
                    {
                        if (vehicle.Travelling && GetTravelTime(vehicle, out float travelTime))
                        {

                            TimeUnit arrivalTime = TimeUnit.FromHours(travelTime);
                            //Logger.Info($"[UIModuleFactionAgendaTracker_UpdateData_PREFIX] element.TrackedObject: {element.TrackedObject}, arrivalTime: {arrivalTime}");

                            element.UpdateData(arrivalTime, true, null);
                            __result = arrivalTime <= TimeUnit.Zero;
                        }
                        else if (vehicle.IsExploringSite)
                        {
                            float explorationTimeHours = GetExplorationTime(vehicle, (float)vehicle.CurrentSite.ExplorationTime.TimeSpan.TotalHours);
                            TimeUnit explorationTimeEnd = TimeUnit.FromHours(explorationTimeHours);
                            //Logger.Info($"[UIModuleFactionAgendaTracker_UpdateData_PREFIX] element.TrackedObject: {element.TrackedObject}, explorationTimeEnd: {explorationTimeEnd}");

                            element.UpdateData(explorationTimeEnd, true, null);
                            __result = explorationTimeEnd <= TimeUnit.Zero;
                        }
                        return false;
                    }
                    else if (element.TrackedObject is GeoPhoenixFacility facility && facility.IsRepairing)
                    {
                        TimeUnit repairsCarriedOut = facility.GetTimeLeftToUpdate();
                        //Logger.Info($"[UIModuleFactionAgendaTracker_UpdateData_PREFIX] element.TrackedObject: {element.TrackedObject}, repairsCarriedOut: {repairsCarriedOut}");

                        element.UpdateData(repairsCarriedOut, true, null);
                        __result = repairsCarriedOut <= TimeUnit.Zero;

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



        [HarmonyPatch(typeof(UIModuleFactionAgendaTracker), "InitialSetup")]
        public static class UIModuleFactionAgendaTracker_InitialSetup_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            // Store UIFactionDataTrackerElement prefab
            public static void Prefix(UIModuleFactionAgendaTracker __instance)
            {
                trackerElementDefault = __instance.TrackerRowPrefab;
            }

            public static void Postfix(UIModuleFactionAgendaTracker __instance, GeoFaction ____faction)
            {
                if (____faction is GeoPhoenixFaction geoPhoenixFaction)
                {
                    MethodInfo ___GetFreeElement = typeof(UIModuleFactionAgendaTracker).GetMethod("GetFreeElement", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo ___OnAddedElement = typeof(UIModuleFactionAgendaTracker).GetMethod("OnAddedElement", BindingFlags.NonPublic | BindingFlags.Instance);

                    // Vehicles
                    foreach (GeoVehicle vehicle in geoPhoenixFaction.Vehicles.Where(v => v.Travelling || v.IsExploringSite))
                    {
                        Logger.Debug($"[UIModuleFactionAgendaTracker_InitialSetup_POSTFIX] Adding/Reapplying vehicle related tracker elements.");

                        UIFactionDataTrackerElement freeElement = (UIFactionDataTrackerElement)___GetFreeElement.Invoke(__instance, null);

                        string siteName = "ERR";
                        string vehicleInfo = "ERR";
                        if (vehicle.Travelling)
                        {
                            siteName = GetSiteName(vehicle.FinalDestination, vehicle.Owner);
                            vehicleInfo = $"{vehicle.VehicleID} {actionTraveling} {siteName}";
                        }
                        else if (vehicle.IsExploringSite)
                        {
                            siteName = GetSiteName(vehicle.CurrentSite, vehicle.Owner);
                            vehicleInfo = $"{vehicle.VehicleID} {actionExploring} {siteName}";
                        }

                        freeElement.Init(vehicle, vehicleInfo, vehicle.VehicleDef.ViewElement, false);
                        //freeElement.Init(vehicle, vehicleInfo, null, false);

                        ___OnAddedElement.Invoke(__instance, new object[] { freeElement });
                    }

                    // Facilities under repair
                    IEnumerable<GeoPhoenixFacility> facilitiesUnderRepair = geoPhoenixFaction.Bases.SelectMany((GeoPhoenixBase t) => from f in t.Layout.Facilities where f.IsRepairing && f.GetTimeLeftToUpdate() != TimeUnit.Zero select f);
                    foreach (GeoPhoenixFacility facility in facilitiesUnderRepair)
                    {
                        UIFactionDataTrackerElement freeElement = (UIFactionDataTrackerElement)___GetFreeElement.Invoke(__instance, null);

                        string facilityName = I2.Loc.LocalizationManager.GetTranslation(facility.Def.ViewElementDef.DisplayName1.LocalizationKey);
                        string facilityInfo = $"{actionRepairing} {facilityName}";
                        freeElement.Init(facility, facilityInfo, facility.Def.ViewElementDef, false);

                        ___OnAddedElement.Invoke(__instance, new object[] { freeElement });
                    }
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleSiteContextualMenu), "SetMenuItems")]
        public static class UIModuleSiteContextualMenu_SetMenuItems_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowTravelAgenda;
            }

            public static void Postfix(UIModuleSiteContextualMenu __instance, GeoSite site, List<SiteContextualMenuItem> ____menuItems)
            {
                try
                {
                    foreach (SiteContextualMenuItem item in ____menuItems)
                    {
                        GeoVehicle vehicle = item.Ability?.GeoActor as GeoVehicle;

                        if (item.Ability is MoveVehicleAbility move && move.GeoActor is GeoVehicle v && v.CurrentSite != site)
                        {
                            if (GetTravelTime(v, out float eta, site))
                            {
                                item.ItemText.text += AppendTime(eta);
                            }
                        }
                        else if (item.Ability is ExploreSiteAbility explore)
                        {
                            float hours = GetExplorationTime(vehicle, (float)site.ExplorationTime.TimeSpan.TotalHours);
                            item.ItemText.text += AppendTime(hours);
                        }
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
