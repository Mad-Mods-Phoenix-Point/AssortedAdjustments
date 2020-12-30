using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.View.DataObjects;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.View.ViewStates;
using UnityEngine;
using UnityEngine.UI;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class ExtendedBaseInfo
    {
        internal static int tooltipListLimit = 6;
        internal static class PhoenixBaseExtendedInfoData
        {
            public static string HealOutput;
            public static string StaminaOutput;
            public static string ExperienceOutput;
            public static string SkillpointOutput;

            public static new string ToString()
            {
                return $"[PhoenixBaseExtendedInfoData] {HealOutput}, {StaminaOutput}, {ExperienceOutput}, {SkillpointOutput}";
            }
        }



        [HarmonyPatch(typeof(UIStateVehicleSelected), "OnSiteMouseHover")]
        public static class UIStateVehicleSelected_OnSiteMouseHover_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Prefix(UIStateVehicleSelected __instance, GeoSite site)
            {
                try
                {
                    if (site == null)
                    {
                        return;
                    }

                    if (site.Type == GeoSiteType.PhoenixBase)
                    {
                        Logger.Debug($"[UIStateVehicleSelected_OnSiteMouseHover_PREFIX] site: {site.Name}");

                        GeoPhoenixBase geoPhoenixBase = site.GetComponent<GeoPhoenixBase>();
                        geoPhoenixBase.UpdateStats();
                        PhoenixBaseExtendedInfoData.HealOutput = $"Healing: {geoPhoenixBase.Stats.HealSoldiersHP} HP/h";
                        PhoenixBaseExtendedInfoData.StaminaOutput = $"Recreation: {geoPhoenixBase.Stats.HealSoldiersStamina} ST/h";
                        PhoenixBaseExtendedInfoData.ExperienceOutput = $"Training: {geoPhoenixBase.Stats.TrainSoldiersXP} XP/h";
                        PhoenixBaseExtendedInfoData.SkillpointOutput = $"Skill: {geoPhoenixBase.Stats.GainSP} SP/h";

                        Logger.Info($"[UIStateVehicleSelected_OnSiteMouseHover_PREFIX] {PhoenixBaseExtendedInfoData.ToString()}");
                    }    
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(UIModuleShortPhoenixBaseTooltip), "SetTooltipData")]
        public static class UIModuleShortPhoenixBaseTooltip_SetTooltipData_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleShortPhoenixBaseTooltip __instance, PhoenixBaseShortInfoData baseData)
            {
                try
                {
                    string tab = "   ";
                    __instance.AvailableSlotsText.fontSize = 36;
                    __instance.AvailableSlotsText.lineSpacing = 1f;
                    //__instance.AvailableSlotsText.color = Color.white;
                    __instance.AvailableSlotsText.alignment = TextAnchor.UpperLeft;
                    __instance.AvailableSlotsText.resizeTextForBestFit = false;
                    __instance.AvailableSlotsText.alignByGeometry = false;
                    //__instance.AvailableSlotsText.horizontalOverflow = HorizontalWrapMode.Wrap;

                    __instance.AvailableSlotsText.text = $"{tab}{__instance.AvailableSlotsText.text}";



                    if (baseData.IsActivated)
                    {
                        Logger.Debug($"[UIModuleShortPhoenixBaseTooltip_SetTooltipData_POSTFIX] baseData.BaseName: {baseData.BaseName.Localize()}");

                        string org = __instance.AvailableSlotsText.text;
                        string postfix = "";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.HealOutput}\n";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.StaminaOutput}\n";
                        postfix += $"{tab}{PhoenixBaseExtendedInfoData.ExperienceOutput}";

                        __instance.AvailableSlotsText.text = $"{org}\n{postfix}";
                    }
                    else if (!baseData.IsBaseInfested)
                    {
                        __instance.InfestationThreatHeaderText.text = "Infestation";
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(UIModuleBaseLayout), "SetLeftSideInfo")]
        public static class UIModuleBaseLayout_SetLeftSideInfo_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleBaseLayout __instance)
            {
                try
                {
                    Transform anchor = __instance.PersonnelAtBaseText.transform?.parent?.parent?.parent?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Column not found. Cannot attach tooltip.");
                    }

                    //Logger.Debug($"{anchor.name}");
                    //Logger.Debug($"{anchor.parent.name}");
                    //Logger.Debug($"{anchor.parent.parent.name}");

                    String info = "<!--FONTSIZE:36-->";

                    info += "<size=42><color=#ECBA62>BASE</color></size>";
                    info += "\n";
                    info += $"Healing: {__instance.PxBase.Stats.HealSoldiersHP} HP/h";
                    info += "\n";
                    info += $"Recreation: {__instance.PxBase.Stats.HealSoldiersStamina} ST/h";
                    info += "\n";
                    info += $"Training: {__instance.PxBase.Stats.TrainSoldiersXP} XP/h";
                    info += "\n";
                    info += $"Repairs: {__instance.PxBase.Stats.RepairVehiclesHP} HP/h";

                    int remainder = 0;
                    List<GeoCharacter> soldiers = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    if (soldiers.Count > tooltipListLimit)
                    {
                        remainder = soldiers.Count - tooltipListLimit;
                        soldiers = soldiers.OrderBy(c => c.Health.IntValue / c.Health.IntMax).ThenBy(c => c.Fatigue.Stamina.IntValue / c.Fatigue.Stamina.IntMax).Take(tooltipListLimit).ToList();
                    }
                    List<GeoCharacter> vehicles = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    List<GeoVehicle> aircrafts = __instance.PxBase.VehiclesAtBase.ToList();

                    if (soldiers.Count > 0)
                    {
                        info += "\n\n";
                        info += "<size=42><color=#ECBA62>SOLDIERS</color></size>";
                        //info += "\n";
                        //info += $"<size=24>Healing: {__instance.PxBase.Stats.HealSoldiersHP} HP/h, Recreation: {__instance.PxBase.Stats.HealSoldiersStamina} ST/h, Training: {__instance.PxBase.Stats.TrainSoldiersXP} XP/h</size>";
                        info += "\n";
                        foreach (GeoCharacter soldier in soldiers)
                        {
                            info += $"<color=#ffffff>{soldier.DisplayName}</color>";
                            info += "\n";
                            info += $"HP: {soldier.Health.IntValue}/{soldier.Health.IntMax}";
                            info += ", ";
                            info += $"ST: {soldier.Fatigue.Stamina.IntValue}/{soldier.Fatigue.Stamina.IntMax}";
                            info += ", ";
                            info += $"XP: {soldier.Progression.LevelProgression.CurrentLevelExperience}/{soldier.Progression.LevelProgression.ExperienceNeededForNextLevel}";
                            info += "\n";
                            info += "\n";
                        }
                        if (remainder > 0)
                        {
                            info += $"<color=#ffffff>+ {remainder} more</color>";
                        }
                    }

                    if (vehicles.Count > 0)
                    {
                        info += "\n\n";
                        info += "<size=42><color=#ECBA62>VEHICLES</color></size>";
                        //info += "\n";
                        //info += $"<size=24>Repairs: {__instance.PxBase.Stats.RepairVehiclesHP} HP/h</size>";
                        info += "\n";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            info += $"<color=#ffffff>{vehicle.DisplayName}</color>";
                            info += "\n";
                            info += $"Health: {vehicle.Health.IntValue}/{vehicle.Health.IntMax}";
                            info += "\n";
                        }
                    }

                    if (aircrafts.Count > 0)
                    {
                        info += "\n\n";
                        info += "<size=42><color=#ECBA62>AIRCRAFTS</color></size>";
                        //info += "\n";
                        //info += $"<size=24>Repairs: {__instance.PxBase.Stats.RepairVehiclesHP} HP/h</size>";
                        info += "\n";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            info += $"<color=#ffffff>{aircraft.Name}</color>";
                            info += "\n";
                            info += $"Health: {(aircraft.VehicleDef.BaseStats.MaxMaintenancePoints - aircraft.VehicleDef.BaseStats.MaintenancePoints)}/{aircraft.VehicleDef.BaseStats.MaxMaintenancePoints}";
                            info += ", ";
                            info += $"Space: {aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace}";
                            info += "\n";
                        }
                    }

                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        Logger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        Logger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
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
