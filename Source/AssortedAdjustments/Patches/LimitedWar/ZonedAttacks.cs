using Base.UI;
using Harmony;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Missions;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AssortedAdjustments.LimitedWar
{
    // Convert haven destruction to attacked zone destruction
    [HarmonyPatch(typeof(GeoSite), "DestroySite")]
    public static class GeoSite_DestroySite_Patch_ConvertDestruction
    {
        public static bool Prepare()
        {
            return Config.Enable && (Config.LimitFactionAttacksToZones || Config.LimitPandoranAttacksToZones);
        }

        public static bool Prefix(GeoSite __instance)
        {
            try
            {
                if (Store.DefenseMission == null)
                {
                    return true;
                }
                IGeoFactionMissionParticipant attacker = Store.DefenseMission.GetEnemyFaction();
                Logger.Info($"{__instance.Name} lost to {attacker.GetPPName()}.");
                if (Resolver.CanDestroyHavens(attacker))
                {
                    return true;
                }
                GeoHavenZone zone = Store.DefenseMission.AttackedZone;
                zone.AddDamage(zone.Health.IntValue);
                zone.AddProduction(0);
                GeoHaven haven = zone.Haven;
                Logger.Info($"Fall of {__instance.Name} converted to {zone.Def.ViewElementDef.DisplayName1.Localize()} destruction.");

                if (haven != null)
                {
                    if ((zone.Def.ProvidesRecruitment || zone.Def.ProvidesEliteRecruitment) && haven.AvailableRecruit != null)
                    {
                        haven.RemoveRecruit();
                    }
                    haven.ZonesStats.UpdateZonesStats();
                }

                __instance.RefreshVisuals();

                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }

    // Expand haven name with zone name in log
    [HarmonyPatch(typeof(GeoscapeLog), "Map_SiteMissionStarted")]
    public static class GeoscapeLog_Map_SiteMissionStarted_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && (Config.LimitFactionAttacksToZones || Config.LimitPandoranAttacksToZones);
        }

        public static void Postfix(GeoSite site, GeoMission mission, List<GeoscapeLogEntry> ____entries)
        {
            try
            {
                if (!(mission is GeoHavenDefenseMission defense) || Store.DefenseMission == null)
                {
                    return;
                }
                IGeoFactionMissionParticipant attacker = Store.DefenseMission.GetEnemyFaction();
                if (Resolver.CanDestroyHavens(attacker))
                {
                    return;
                }
                LocalizedTextBind zoneName = defense.AttackedZone?.Def?.ViewElementDef?.DisplayName1;
                if (zoneName == null || ____entries == null || ____entries.Count < 1)
                {
                    return;
                }
                GeoscapeLogEntry entry = ____entries[____entries.Count - 1];
                Logger.Info($"Converting {attacker.GetPPName()} invasion message to zone invasion.");
                entry.Parameters[0] = new LocalizedTextBind($"{site.Name} ({Utilities.ToTitleCase(zoneName.Localize())})", true);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    // Expand haven name with zone name in log and disable "haven destroyed" sound
    [HarmonyPatch(typeof(GeoscapeLog), "Map_SiteMissionEnded")]
    public static class GeoscapeLog_Map_SiteMissionEnded_Patch
    {
        public static bool Prepare()
        {
            return Config.Enable && (Config.LimitFactionAttacksToZones || Config.LimitPandoranAttacksToZones);
        }

        // Override
        public static bool Prefix(GeoscapeLog __instance, GeoSite site, GeoMission mission, GeoLevelController ____level, GeoscapeLogMessagesDef ____messagesDef, GeoFaction ____faction)
        {
            try
            {
                if (!site.GetInspected(____faction))
                {
                    return false;
                }

                if (!(mission is GeoHavenDefenseMission geoHavenDefenseMission) || Store.DefenseMission == null)
                {
                    return true;
                }

                IGeoFactionMissionParticipant factionMissionParticipant = ____level.GetFactionMissionParticipant(((GeoHavenDefenseMission)mission).AttackerFaction);
                if (Resolver.CanDestroyHavens(factionMissionParticipant))
                {
                    return true;
                }

                LocalizedTextBind zoneName = geoHavenDefenseMission.AttackedZone?.Def?.ViewElementDef?.DisplayName1;
                if (zoneName == null)
                {
                    return true;
                }

                bool flag = ((GeoHavenDefenseMission)mission).Status == GeoscapeMissionStatus.AttackersWon;
                GeoscapeLogEntry entry = new GeoscapeLogEntry
                {
                    Text = (flag ? ____messagesDef.HavenDestroyedMessage : ____messagesDef.HavenRepelledAttackMessage),
                    Parameters = new LocalizedTextBind[]
                    {
                            new LocalizedTextBind($"{site.Name} ({Utilities.ToTitleCase(zoneName.Localize())})", true),
                            factionMissionParticipant.ParticipantName
                    }
                };
                // Add customized entry
                typeof(GeoscapeLog).GetMethod("AddEntry", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { entry, site });



                // No "haven destroyed" sound for zone damage
                //if (flag && ____messagesDef.SoundHavenDestroyed.IsValid())
                //{
                //    ____level.View.QueueLogSound(entry, ____messagesDef.SoundHavenDestroyed);
                //}



                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }
}