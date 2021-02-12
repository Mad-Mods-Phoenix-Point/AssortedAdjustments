using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [HarmonyPatch(typeof(TacticalView), "OnAbilityExecuted")]
        public static class TacticalView_OnAbilityExecuted_Patch
        {
            internal static IEnumerable<TacticalActor> allActiveSquadmembers;



            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableSmartEvacuation;
            }

            // Override!
            public static bool Prefix(TacticalView __instance, TacticalAbility ability, TacticalActor ____selectedActor)
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
                    if (!__instance.ViewerFaction.IsPlayingTurn || (ability.TacticalActorBase && ability.TacticalActorBase.TacticalFaction != __instance.ViewerFaction) || ability is IdleAbility)
                    {
                        return false;
                    }

                    bool isExitMissionAbilityEnabled = ability?.TacticalActorBase?.GetAbility<ExitMissionAbility>()?.IsEnabled(null) == true;
                    bool isEvacuateMountedActorsAbilityEnabled = ability?.TacticalActorBase?.GetAbility<EvacuateMountedActorsAbility>()?.IsEnabled(null) == true;
                    bool shouldOverridePrompt = isExitMissionAbilityEnabled || isEvacuateMountedActorsAbilityEnabled;
                    //Logger.Debug($"[TacticalView_OnAbilityExecuted_PREFIX] isExitMissionAbilityEnabled: {isExitMissionAbilityEnabled}");
                    //Logger.Debug($"[TacticalView_OnAbilityExecuted_PREFIX] isEvacuateMountedActorsAbilityEnabled: {isEvacuateMountedActorsAbilityEnabled}");
                    //Logger.Debug($"[TacticalView_OnAbilityExecuted_PREFIX] shouldOverridePrompt: {shouldOverridePrompt}");

                    if (ability is IMoveAbility && ability.TacticalActor == ____selectedActor && shouldOverridePrompt)
                    {
                        Logger.Debug($"[TacticalView_OnAbilityExecuted_PREFIX] Overriding exit mission prompt to only trigger if the whole squad is in some exit zone.");



                        // Always called by original method, needed?
                        typeof(TacticalView).GetMethod("UpdateApPool", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { false });
                        //__instance.UpdateApPool(false);



                        TacticalAbility evacuateAbility = ____selectedActor.GetAbility<ExitMissionAbility>();
                        if (evacuateAbility == null)
                        {
                            evacuateAbility = ____selectedActor.GetAbility<EvacuateMountedActorsAbility>();
                        }

                        allActiveSquadmembers = __instance.TacticalLevel.CurrentFaction.TacticalActors.Where(a => a != ____selectedActor && a.IsActive);
                        Logger.Info($"[TacticalView_OnAbilityExecuted_PREFIX] allActiveSquadmembers: {allActiveSquadmembers.Select(a => a.DisplayName).ToArray().Join(null, ", ")}");

                        bool isSquadInExitZone = true;
                        foreach (TacticalActor tActor in allActiveSquadmembers)
                        {
                            TacticalAbility tAbility = tActor.GetAbility<ExitMissionAbility>() as TacticalAbility;
                            if (tAbility == null)
                            {
                                tAbility = tActor.GetAbility<EvacuateMountedActorsAbility>() as TacticalAbility;
                            }
                            if (tAbility == null)
                            {
                                // Has no relevant ability, most likely a turret
                                Logger.Info($"[TacticalView_OnAbilityExecuted_PREFIX] actor: {tActor.DisplayName} has no exit/evacuate ability (IsMetallic: {tActor.IsMetallic}, GameTags: {tActor.TacticalActorBaseDef.GameTags})");
                                continue;
                            }
                            Logger.Info($"[TacticalView_OnAbilityExecuted_PREFIX] actor: {tActor.DisplayName}, canEvacuate: {tAbility?.HasValidTargets}");

                            if (!tAbility.HasValidTargets)
                            {
                                isSquadInExitZone = false;
                                // Don't break to test for a while
                                //break;
                            }
                        }
                        Logger.Info($"[TacticalView_OnAbilityExecuted_PREFIX] isSquadInExitZone: {isSquadInExitZone}");

                        if (isSquadInExitZone)
                        {
                            GameUtl.GetMessageBox().ShowSimplePrompt("Evacuate Squad?", MessageBoxIcon.Question, MessageBoxButtons.YesNo, new MessageBox.MessageBoxCallback(OnEvacuateSquadConfirmationResult), null, evacuateAbility);
                        }



                        return false;
                    }
                    else
                    {
                        return true;
                    }
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
