using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Levels;

namespace AssortedAdjustments.Patches
{
    internal static class DisableNothingFound
    {
        private static GeoFaction visitingFaction = null;
        private static readonly string NothingFoundID = "EXPSITE_02";



        [HarmonyPatch(typeof(GeoscapeEventSystem), "PhoenixFaction_OnSiteFirstTimeVisited")]
        public static class GeoscapeEventSystem_PhoenixFaction_OnSiteFirstTimeVisited_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.DisableNothingFound;
            }

            // Storing otherwise inaccessible controller for patches of called methods in "PhoenixFaction_OnSiteFirstTimeVisited"
            public static void Prefix(GeoscapeEventSystem __instance, GeoFaction controller)
            {
                visitingFaction = controller;
            }

            // Releasing
            public static void Postfix(GeoscapeEventSystem __instance, GeoFaction controller)
            {
                visitingFaction = null;
            }
        }



        [HarmonyPatch(typeof(GeoscapeEventSystem), "GetValidEventsForSite")]
        public static class GeoscapeEventSystem_GetValidEventsForSite_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.DisableNothingFound;
            }

            public static void Postfix(GeoscapeEventSystem __instance, List<GeoscapeEventDef> outEvents)
            {
                try
                {
                    Logger.Debug($"[GeoscapeEventSystem_GetValidEventsForSite_POSTFIX] outEvents: {outEvents.Join()}");

                    if (outEvents == null || outEvents.Count <= 1)
                    {
                        return;
                    }
                    outEvents.RemoveWhere(e => e.EventID == NothingFoundID);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(GeoscapeEventSystem), "SetEventForSite")]
        public static class GeoscapeEventSystem_SetEventForSite_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.DisableNothingFound;
            }

            public static void Prefix(GeoscapeEventSystem __instance, GeoSite site, ref string eventID)
            {
                try
                {
                    Logger.Debug($"[GeoscapeEventSystem_SetEventForSite_PREFIX] eventID: {eventID}");

                    if (eventID != NothingFoundID)
                    {
                        return;
                    }

                    List<string> events = __instance.EmptyExplorationEventIds;
                    if (events.Count <= 1)
                    {
                        if (visitingFaction == null)
                        {
                            return;
                        }

                        List<GeoscapeEventDef> newEventList = new List<GeoscapeEventDef>();
                        __instance.GetValidEventsForSite(site, visitingFaction, newEventList, true);
                        Logger.Debug($"[GeoscapeEventSystem_SetEventForSite_PREFIX] newEventList: {newEventList.Join()}");

                        events = newEventList.Select(e => e.EventID).Where(e => e != NothingFoundID).ToList();
                    }
                    else
                    {
                        events.RemoveAll(e => e == NothingFoundID);
                    }

                    // No other events to play
                    if (events.Count <= 1)
                    {
                        return; 
                    }

                    eventID = events.GetRandomElement();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}