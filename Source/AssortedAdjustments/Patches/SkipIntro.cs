using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base.Core;
using Base.Levels;
using Base.UI.VideoPlayback;
using PhoenixPoint.Common.Game;
using PhoenixPoint.Home.View.ViewStates;
using PhoenixPoint.Tactical.View.ViewStates;

namespace AssortedAdjustments.Patches
{
    internal static class SkipIntro
    {
        public static bool Prefix_PhoenixGame_RunGameLevel(PhoenixGame __instance, LevelSceneBinding levelSceneBinding, ref IEnumerator<NextUpdate> __result)
        {
            try
            {
                Logger.Info($"[PhoenixGame_RunGameLevel_PREFIX] levelSceneBinding: {levelSceneBinding.ToString()}");

                if (levelSceneBinding == __instance.Def.IntroLevelSceneDef.Binding)
                {
                    Logger.Debug($"[PhoenixGame_RunGameLevel_PREFIX] Skipping intro logos.");
                    __result = Enumerable.Empty<NextUpdate>().GetEnumerator();

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



        public static void Postfix_UIStateHomeScreenCutscene_EnterState(UIStateHomeScreenCutscene __instance, VideoPlaybackSourceDef ____sourcePlaybackDef)
        {
            try
            {
                if (____sourcePlaybackDef == null)
                {
                    return;
                }

                Logger.Info($"[UIStateHomeScreenCutscene_EnterState_POSTFIX] ____sourcePlaybackDef.ResourcePath: {____sourcePlaybackDef.ResourcePath}");

                if (____sourcePlaybackDef.ResourcePath.Contains("Game_Intro_Cutscene"))
                {
                    Logger.Debug($"[UIStateHomeScreenCutscene_EnterState_POSTFIX] Skipping intro movie.");
                    typeof(UIStateHomeScreenCutscene).GetMethod("OnCancel", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }



        public static void Postfix_UIStateTacticalCutscene_EnterState(UIStateTacticalCutscene __instance, VideoPlaybackSourceDef ____sourcePlaybackDef)
        {
            try
            {
                if (____sourcePlaybackDef == null)
                {
                    return;
                }

                Logger.Info($"[UIStateTacticalCutscene_EnterState_POSTFIX] ____sourcePlaybackDef.ResourcePath: {____sourcePlaybackDef.ResourcePath}");

                if (____sourcePlaybackDef.ResourcePath.Contains("LandingSequences"))
                {
                    Logger.Debug($"[UIStateTacticalCutscene_EnterState_POSTFIX] Skipping landing sequence.");
                    typeof(UIStateTacticalCutscene).GetMethod("OnCancel", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(__instance, null);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
