using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.View;

namespace AssortedAdjustments.Patches
{
    internal static class AutoEvacuation
    {
        [HarmonyPatch(typeof(TacticalView), "ShowExitMissionPrompt")]
        public static class GeoFaction_ShowExitMissionPrompt_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableAutoEvacuation;
            }

            // Override!
            public static bool Prefix(TacticalView __instance, TacticalActor ____selectedActor)
            {
                try
                {
                    Logger.Debug($"[GeoFaction_ShowExitMissionPrompt_PREFIX] Overriding exit mission prompt.");

                    ExitMissionAbility exitMissionAbility = ____selectedActor.GetAbility<ExitMissionAbility>();
                    //GameUtl.GetMessageBox().ShowSimplePrompt(this.UICommonDef.ShouldExitMissionPrompt.Localize(null), MessageBoxIcon.Question, MessageBoxButtons.YesNo, new MessageBox.MessageBoxCallback(this.OnExecuteAbilityConfirmationResult), null, ability);

                    // TacticalView.OnExecuteAbilityConfirmationResult(MessageBoxCallbackResult res)
                    //if (res.DialogResult != MessageBoxResult.Yes)
                    //{
                    //    return;
                    //}

                    TacticalAbility tacticalAbility = exitMissionAbility as TacticalAbility;
                    TacticalAbilityTarget tacticalAbilityTarget = tacticalAbility?.GetTargets().FirstOrDefault<TacticalAbilityTarget>();
                    if (tacticalAbilityTarget != null)
                    {
                        tacticalAbility.Activate(tacticalAbilityTarget);
                        __instance.ResetViewState();
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
}
