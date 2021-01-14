using System;
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.UI.MessageBox;
using Harmony;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.View;

namespace AssortedAdjustments.Patches
{
    internal static class SmartEvacuation
    {
        [HarmonyPatch(typeof(TacticalView), "ShowExitMissionPrompt")]
        public static class GeoFaction_ShowExitMissionPrompt_Patch
        {
            internal static IEnumerable<TacticalActor> allActiveSquadmembers;



            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSmartEvacuation;
            }

            // Override!
            public static bool Prefix(TacticalView __instance, TacticalActor ____selectedActor)
            {
                // Callback Helper
                void OnEvacuateSquadConfirmationResult(MessageBoxCallbackResult res)
                {
                    if (res.DialogResult != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    // Evacuate current actor
                    TacticalAbility tacticalAbility = res.UserData as TacticalAbility;
                    TacticalAbilityTarget tacticalAbilityTarget = tacticalAbility?.GetTargets().FirstOrDefault<TacticalAbilityTarget>();
                    if (tacticalAbilityTarget != null)
                    {
                        tacticalAbility.Activate(tacticalAbilityTarget);
                    }

                    // Evacuate squadmembers
                    foreach (TacticalActor tActor in allActiveSquadmembers)
                    {
                        TacticalAbility tAbility = tActor.GetAbility<ExitMissionAbility>() as TacticalAbility;
                        if (tAbility == null)
                        {
                            tAbility = tActor.GetAbility<EvacuateMountedActorsAbility>() as TacticalAbility;
                        }
                        TacticalAbilityTarget taTarget = tAbility?.GetTargets().FirstOrDefault<TacticalAbilityTarget>();
                        //Logger.Info($"[GeoFaction_ShowExitMissionPrompt_PREFIX] ActorGridPosition: {taTarget.ActorGridPosition}");

                        if (taTarget != null)
                        {
                            tAbility.Activate(taTarget);
                        }
                    }
                    __instance.ResetViewState();
                }



                try
                {
                    Logger.Debug($"[GeoFaction_ShowExitMissionPrompt_PREFIX] Overriding exit mission prompt to only trigger if the whole squad is in some exit zone.");

                    ExitMissionAbility exitMissionAbility = ____selectedActor.GetAbility<ExitMissionAbility>();
                    allActiveSquadmembers = __instance.TacticalLevel.CurrentFaction.TacticalActors.Where(a => a != ____selectedActor && a.IsActive);
                    Logger.Info($"[GeoFaction_ShowExitMissionPrompt_PREFIX] allActiveSquadmembers: {allActiveSquadmembers.Select(a => a.DisplayName).ToArray().Join(null, ", ")}");

                    bool isSquadInExitZone = true;
                    foreach(TacticalActor tActor in allActiveSquadmembers)
                    {
                        TacticalAbility tAbility = tActor.GetAbility<ExitMissionAbility>() as TacticalAbility;
                        if (tAbility == null)
                        { 
                            tAbility = tActor.GetAbility<EvacuateMountedActorsAbility>() as TacticalAbility;
                        }
                        Logger.Info($"[GeoFaction_ShowExitMissionPrompt_PREFIX] actor: {tActor.DisplayName}, canEvacuate: {tAbility?.HasValidTargets}");

                        if (!tAbility.HasValidTargets)
                        {
                            isSquadInExitZone = false;
                            // Don't break to test for a while
                            //break;
                        }
                    }
                    Logger.Info($"[GeoFaction_ShowExitMissionPrompt_PREFIX] isSquadInExitZone: {isSquadInExitZone}");

                    if (isSquadInExitZone)
                    {
                        GameUtl.GetMessageBox().ShowSimplePrompt("Evacuate Squad?", MessageBoxIcon.Question, MessageBoxButtons.YesNo, new MessageBox.MessageBoxCallback(OnEvacuateSquadConfirmationResult), null, exitMissionAbility);
                    }



                    return false;



                    /*
                    // This would evacuate the unit directly, no questions asked. BEWARE: Would also evacuate soldier on minimal movement at level start (while still in deployment/evacuation zone)!
                    TacticalAbility tacticalAbility = exitMissionAbility as TacticalAbility;
                    TacticalAbilityTarget tacticalAbilityTarget = tacticalAbility?.GetTargets().FirstOrDefault<TacticalAbilityTarget>();
                    if (tacticalAbilityTarget != null)
                    {
                        tacticalAbility.Activate(tacticalAbilityTarget);
                        __instance.ResetViewState();
                        return false;
                    }
                    return true;
                    */
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
