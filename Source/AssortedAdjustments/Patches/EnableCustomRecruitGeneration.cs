using System;
using Harmony;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.Entities;
using System.Collections.Generic;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Levels;
using Base.Core;
using UnityEngine;

namespace AssortedAdjustments.Patches
{
    internal static class CustomRecruitGeneration
    {
        [HarmonyPatch(typeof(GeoPhoenixFaction), "RegenerateNakedRecruits")]
        public static class GeoPhoenixFaction_RegenerateNakedRecruits_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableCustomRecruitGeneration;
            }

            public static bool Prefix(GeoPhoenixFaction __instance, Dictionary<GeoUnitDescriptor, ResourcePack> ____nakedRecruits, GeoLevelController ____level, TimeUnit ____lastNakedRecruitRefresh)
            {
                try
                {
                    Logger.Debug($"[GeoPhoenixFaction_RegenerateNakedRecruits_PREFIX] Generate a custom amount of new recruits. Potentially edit their parameters.");

                    ____nakedRecruits.Clear();

                    // MAD:
                    //int num = __instance.FactionDef.MaxNakedRecruitsAvailability.RandomValue();
                    int num = Mathf.Clamp(AssortedAdjustments.Settings.RecruitGenerationCount, 1, 4); // UI cannot handle more than four
                    //:DAM

                    CharacterGenerationContext context = ____level.CharacterGenerator.GenerateCharacterGeneratorContext(__instance);
                    for (int i = 0; i < num; i++)
                    {
                        GeoUnitDescriptor geoUnitDescriptor = ____level.CharacterGenerator.GenerateRandomUnit(context);
                        ____level.CharacterGenerator.ApplyRecruitDifficultyParameters(geoUnitDescriptor);
                        ResourcePack value = __instance.GenerateNakedRecruitsCost();
                        ____nakedRecruits.Add(geoUnitDescriptor, value);
                    }
                    ____lastNakedRecruitRefresh = ____level.Timing.Now;
                    __instance.SpawnedRecruitNotification = true;
                    Action<IEnumerable<GeoUnitDescriptor>> recruitsRegenerated = __instance.RecruitsRegenerated;
                    if (recruitsRegenerated == null)
                    {
                        return false;
                    }
                    recruitsRegenerated(__instance.NakedRecruits.Keys);


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
}
