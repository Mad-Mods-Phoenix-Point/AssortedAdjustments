using System;
using System.Collections.Generic;
using Harmony;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Levels;
using PhoenixPoint.Tactical.View.ViewControllers;
using PhoenixPoint.Tactical.View.ViewModules;

namespace AssortedAdjustments.Patches
{
    internal static class ExperienceToSkillpointConversion
    {
        private static bool captureExperience = false;
        private static Dictionary<LevelProgression, int> missionExperience;
        private static Dictionary<GeoTacUnitId, int> convertedSkillpoints;




        [HarmonyPatch(typeof(TacticalFaction), "GiveExperienceForObjectives")]
        public static class TacticalFaction_GiveExperienceForObjectives_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableExperienceToSkillpointConversion;
            }

            public static void Prefix(TacticalFaction __instance)
            {
                try
                {
                    if (__instance?.ParticipantKind != TacMissionParticipant.Player)
                    {
                        return;
                    }

                    Logger.Debug($"[TacticalFaction_GiveExperienceForObjectives_PREFIX] Handling faction: {__instance.TacticalFactionDef.GetName()}.");

                    missionExperience = new Dictionary<LevelProgression, int>(16);
                    convertedSkillpoints = new Dictionary<GeoTacUnitId, int>(16);
                    captureExperience = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            public static void Postfix(TacticalFaction __instance)
            {
                try
                {
                    if (missionExperience == null)
                    {
                        return;
                    }
                    captureExperience = false;

                    float xpConversionRate = __instance.TacticalLevel?.Difficulty?.ExpConvertedToSkillpoints ?? AssortedAdjustments.Settings.XPtoSPConversionRate;
                    float xpConversionMultiplier = AssortedAdjustments.Settings.XPtoSPConversionMultiplier;

                    foreach (TacticalActor actor in __instance.TacticalActors)
                    {
                        LevelProgression levelProgression = actor?.LevelProgression;
                        if (levelProgression == null || !actor.IsAlive || !missionExperience.TryGetValue(levelProgression, out int xpMission)) continue;
                        int xpEarned = levelProgression.Experience - levelProgression.ExperienceReference;
                        int xpToConvert = xpMission - xpEarned;
                        int skillpoints = Math.Max(0, (int)Math.Floor(xpToConvert * xpConversionRate * xpConversionMultiplier));
                        if (skillpoints > 0)
                        {
                            convertedSkillpoints[actor.GeoUnitId] = skillpoints;
                            Logger.Info($"[TacticalFaction_GiveExperienceForObjectives_POSTFIX] xpConversionRate: {xpConversionRate}");
                            Logger.Info($"[TacticalFaction_GiveExperienceForObjectives_POSTFIX] xpConversionMultiplier: {xpConversionMultiplier}");
                            Logger.Info(String.Format("[TacticalFaction_GiveExperienceForObjectives_POSTFIX] (Tac) {0} earned a total of {1} XP({2},{3}) and had the overflow of {3} converted into {4} SP", actor.GetDisplayName(), xpMission, xpEarned, xpToConvert, skillpoints));
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(LevelProgression), "AddExperience")]
        public static class LevelProgression_AddExperience_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableExperienceToSkillpointConversion;
            }

            public static void Postfix(LevelProgression __instance, int experience)
            {
                try
                {
                    if (!captureExperience || missionExperience == null)
                    {
                        return;
                    }
                    Logger.Debug($"[LevelProgression_AddExperience_POSTFIX] Capture experience({experience}).");

                    missionExperience.TryGetValue(__instance, out int xpMission);
                    missionExperience[__instance] = xpMission + experience;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleBattleSummary), "SetProgressSlider")]
        public static class UIModuleBattleSummary_SetProgressSlider_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableExperienceToSkillpointConversion;
            }

            public static void Postfix(UIModuleBattleSummary __instance, SoldierResultElement soldierResultElement)
            {
                try
                {
                    TacticalActor actor = soldierResultElement.Actor;
                    LevelProgression levelProgression = actor?.LevelProgression;
                    if (levelProgression == null || !actor.IsAlive || levelProgression.Experience != levelProgression.ExperienceReference)
                    {
                        return;
                    }

                    if (!convertedSkillpoints.TryGetValue(actor.GeoUnitId, out int skillpoints))
                    {
                        return;
                    }

                    soldierResultElement.EarnedExperience.text = String.Format("SP +{0}", skillpoints);
                    soldierResultElement.EarnedExperience.color = soldierResultElement.DefaultStatus.TextColor;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(GeoCharacter), "ApllyTacticalResult")] // sic!
        public static class GeoCharacter_ApllyTacticalResult_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableExperienceToSkillpointConversion;
            }

            public static void Postfix(GeoCharacter __instance, TacActorUnitResult result)
            {
                try
                {
                    if (convertedSkillpoints == null || !convertedSkillpoints.TryGetValue(result.GeoUnitId, out int skillpoints))
                    {
                        return;
                    }

                    if (skillpoints > 0)
                    {
                        Logger.Info(String.Format("[GeoCharacter_ApllyTacticalResult_POSTFIX] (Geo) {0} has {1} SP and will earn additional {2} SP from XP conversion", __instance.GetName(), result.CharacterProgression.SkillPoints, skillpoints));

                        __instance.Progression.SkillPoints += skillpoints;
                        convertedSkillpoints.Remove(result.GeoUnitId);
                        if (convertedSkillpoints.Count <= 0)
                        {
                            convertedSkillpoints = null;
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
