using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.PhoenixBases.FacilityComponents;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.View.DataObjects;
using PhoenixPoint.Geoscape.View.ViewControllers;
using PhoenixPoint.Geoscape.View.ViewControllers.BaseRecruits;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.View.ViewStates;
using UnityEngine;

namespace AssortedAdjustments.Patches.UIEnhancements
{
    internal static class ExtendedBaseInfo
    {
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



        [HarmonyPatch(typeof(UIModuleGeoAssetDeployment), "SetBaseButtonElement")]
        public static class UIModuleGeoAssetDeployment_SetBaseButtonElement_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Postfix(UIModuleGeoAssetDeployment __instance, GeoDeployAssetBaseElementController element, GeoSite site)
            {
                try
                {
                    GeoPhoenixBase phoenixBase = site.GetComponent<GeoPhoenixBase>();
                    if(phoenixBase == null)
                    {
                        return;
                    }

                    Transform anchor = element.PhoenixBaseDetailsRoot.transform?.parent?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //Logger.Debug($"{anchor.name}");
                    //Logger.Debug($"{anchor.parent.name}");
                    //Logger.Debug($"{anchor.parent.parent.name}");

                    string tipText = "<!--FONTSIZE:30-->";
                    tipText += $"<size=42><color=#ECBA62>{phoenixBase.Site.Name}</color></size>";
                    tipText += "\n";
                    tipText += $"Healing: {phoenixBase.Stats.HealSoldiersHP} ({phoenixBase.Stats.HealMutogHP}) HP/h";
                    tipText += "\n";
                    tipText += $"Recreation: {phoenixBase.Stats.HealSoldiersStamina} ST/h";
                    tipText += "\n";
                    tipText += $"Training: {phoenixBase.Stats.TrainSoldiersXP} XP/h";

                    List<GeoCharacter> soldiers = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    if (soldiers.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>SOLDIERS</color></size>";
                        foreach (GeoCharacter soldier in soldiers)
                        {
                            tipText += "\n";
                            tipText += $"{soldier.DisplayName.Split((char)32).First()} ({soldier.GetClassViewElementDefs().FirstOrDefault().DisplayName1.Localize()}, Level {soldier.Progression.LevelProgression.Level})";
                        }
                    }

                    // Mutogs
                    List<GeoCharacter> mutogs = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            tipText += "\n";
                            tipText += $"{mutog.DisplayName}";
                        }
                    }

                    // Vehicles
                    List<GeoCharacter> vehicles = phoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>VEHICLES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            tipText += "\n";
                            tipText += $"{vehicle.DisplayName}";
                        }
                    }

                    // Aircraft
                    List<GeoVehicle> aircrafts = phoenixBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=36><color=#ECBA62>AIRCRAFT</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            tipText += "\n";
                            tipText += $"{aircraft.Name} ({aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace})";
                        }
                    }

                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        Logger.Debug($"[UIModuleGeoAssetDeployment_SetBaseButtonElement_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        Logger.Debug($"[UIModuleGeoAssetDeployment_SetBaseButtonElement_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; // Doesn't seem to work
                        anchorGo.GetComponent<UITooltipText>().Position = UITooltip.Position.RightMiddle;
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(RecruitsBaseDeployInfoController), "SetBaseInfo")]
        public static class RecruitsBaseDeployInfoController_SetBaseInfo_Patch
        {
            public static bool Prepare()
            {
                return AssortedAdjustments.Settings.EnableUIEnhancements && AssortedAdjustments.Settings.ShowExtendedBaseInfo;
            }

            public static void Prefix(RecruitsBaseDeployInfoController __instance, ref RecruitsBaseDeployData baseInfo)
            {
                try
                {
                    // Fix wrong soldier count (vanilla does not exclude vehicles)
                    baseInfo.SoldiersAtBase = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).Count();

                    // Include Mutogs?
                    //baseInfo.SoldiersAtBase = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman || c.TemplateDef.IsMutog).Count();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            public static void Postfix(RecruitsBaseDeployInfoController __instance, RecruitsBaseDeployData baseInfo)
            {
                try
                {
                    Transform anchor = __instance.BaseNameText.transform?.parent;
                    if (anchor == null)
                    {
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //Logger.Debug($"{anchor.name}");
                    //Logger.Debug($"{anchor.parent.name}");
                    //Logger.Debug($"{anchor.parent.parent.name}");

                    string tipText = "<!--FONTSIZE:36-->";
                    tipText += $"<size=52><color=#ECBA62>{baseInfo.PhoenixBase.Site.Name}</color></size>";
                    tipText += "\n";
                    tipText += $"Healing: {baseInfo.PhoenixBase.Stats.HealSoldiersHP} ({baseInfo.PhoenixBase.Stats.HealMutogHP}) HP/h";
                    tipText += "\n";
                    tipText += $"Recreation: {baseInfo.PhoenixBase.Stats.HealSoldiersStamina} ST/h";
                    tipText += "\n";
                    tipText += $"Training: {baseInfo.PhoenixBase.Stats.TrainSoldiersXP} XP/h";

                    List<GeoCharacter> soldiers = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    if (soldiers.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>SOLDIERS</color></size>";
                        foreach (GeoCharacter soldier in soldiers)
                        {
                            tipText += "\n";
                            tipText += $"{soldier.DisplayName.Split((char)32).First()} ({soldier.GetClassViewElementDefs().FirstOrDefault().DisplayName1.Localize()}, Level {soldier.Progression.LevelProgression.Level})";
                        }
                    }

                    // Mutogs
                    List<GeoCharacter> mutogs = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            tipText += "\n";
                            tipText += $"{mutog.DisplayName}";
                        }
                    }

                    // Vehicles
                    List<GeoCharacter> vehicles = baseInfo.PhoenixBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>VEHICLES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            tipText += "\n";
                            tipText += $"{vehicle.DisplayName}";
                        }
                    }

                    // Aircraft
                    List<GeoVehicle> aircrafts = baseInfo.PhoenixBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        tipText += "\n";
                        tipText += "\n";
                        tipText += $"<size=42><color=#ECBA62>AIRCRAFT</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            tipText += "\n";
                            tipText += $"{aircraft.Name} ({aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace})";
                        }
                    }

                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        Logger.Debug($"[RecruitsBaseDeployInfoController_SetBaseInfo_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                        return;
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        Logger.Debug($"[RecruitsBaseDeployInfoController_SetBaseInfo_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; // Doesn't seem to work
                        anchorGo.GetComponent<UITooltipText>().Position = UITooltip.Position.RightMiddle;
                        anchorGo.GetComponent<UITooltipText>().TipText = tipText;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
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
                        PhoenixBaseExtendedInfoData.HealOutput = $"Healing: {geoPhoenixBase.Stats.HealSoldiersHP} ({geoPhoenixBase.Stats.HealMutogHP}) HP/h";
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
                    __instance.AvailableSlotsText.fontSize = 32;
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
                        throw new InvalidOperationException("Anchor not found. Cannot attach tooltip.");
                    }

                    //Logger.Debug($"{anchor.name}");
                    //Logger.Debug($"{anchor.parent.name}");
                    //Logger.Debug($"{anchor.parent.parent.name}");

                    String info = "<!--FONTSIZE:24-->";

                    info += "\n";
                    info += "<size=42><color=#ECBA62>CURRENT BASE</color></size>";
                    info += "\n";
                    info += $"<size=36><color=#FFFFFF>Utility</color></size>";
                    info += "\n";
                    info += "<size=30>";

                    if (__instance.PxBase.Layout.QueryFacilitiesWithComponent<SatelliteUplinkFacilityComponent>(true).Any())
                    {
                        info += $"Scanning Range: {__instance.PxBase.SiteScanner.MaxRange.InMeters / 1000} km";
                        info += "\n";
                    }

                    info += $"Storage: {__instance.PxBase.Stats.MaxItemCapacity} units";
                    info += "\n";
                    info += $"Healing: {__instance.PxBase.Stats.HealSoldiersHP} ({__instance.PxBase.Stats.HealMutogHP}) HP/h";
                    info += "\n";
                    info += $"Recreation: {__instance.PxBase.Stats.HealSoldiersStamina} ST/h";
                    info += "\n";
                    info += $"Training: {__instance.PxBase.Stats.TrainSoldiersXP} XP/h";
                    info += "\n";
                    info += $"Repairs: {__instance.PxBase.Stats.RepairVehiclesHP} HP/h";
                    info += "\n";

                    if (__instance.PxBase.Stats.ResourceOutput.Values.Count > 0) {
                        info += "\n";
                        info += $"<size=36><color=#FFFFFF>Resources</color></size>";
                        info += "\n";
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (ResourceUnit resourceUnit in __instance.PxBase.Stats.ResourceOutput.Values)
                        {
                            stringBuilder.Append(resourceUnit.Type).Append(": ").Append(resourceUnit.Value * 24 + "/d").Append("\n");
                        }
                        info += $"{stringBuilder.ToString()}";
                    }

                    info += "</size>";


                    List<GeoCharacter> allSoldiers = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsHuman).ToList();
                    List<GeoCharacter> bruisedSoldiers = allSoldiers.Where(c => c.Health.IntValue < c.Health.IntMax || c.Fatigue.Stamina.IntValue < c.Fatigue.Stamina.IntMax).OrderBy(c => c.Health.IntValue / c.Health.IntMax).ThenBy(c => c.Fatigue.Stamina.IntValue / c.Fatigue.Stamina.IntMax).ToList();

                    // For testing
                    //bruisedSoldiers = allSoldiers;

                    if (allSoldiers.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>TREATMENT</color></size>";
                        info += "\n";
                        if (bruisedSoldiers.Count > 0)
                        {
                            foreach (GeoCharacter soldier in bruisedSoldiers)
                            {
                                info += $"<size=36><color=#FFFFFF>{soldier.DisplayName}</color></size>";
                                info += "\n";
                                if (soldier.Health.IntValue < soldier.Health.IntMax)
                                {
                                    info += $"HP: <color=#CC3333>{soldier.Health.IntValue}/{soldier.Health.IntMax}</color>";
                                }
                                else
                                {
                                    info += $"HP: {soldier.Health.IntValue}/{soldier.Health.IntMax}";
                                }
                                info += ", ";
                                if (soldier.Fatigue.Stamina.IntValue < soldier.Fatigue.Stamina.IntMax)
                                {
                                    info += $"ST: <color=#CC3333>{soldier.Fatigue.Stamina.IntValue}/{soldier.Fatigue.Stamina.IntMax}</color>";
                                }
                                else
                                {
                                    info += $"ST: {soldier.Fatigue.Stamina.IntValue}/{soldier.Fatigue.Stamina.IntMax}";
                                }
                                info += ", ";
                                info += $"XP: {soldier.Progression.LevelProgression.CurrentLevelExperience}/{soldier.Progression.LevelProgression.ExperienceNeededForNextLevel}";
                                info += "\n";
                            }
                        }
                        else
                        {
                            info += $"<size=36><color=#FFFFFF>All soldiers are healed and rested.</color></size>";
                            info += "\n";
                        }
                    }

                    List<GeoCharacter> mutogs = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsMutog).ToList();
                    if (mutogs.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>MUTOGS</color></size>";
                        foreach (GeoCharacter mutog in mutogs)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{mutog.DisplayName}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            if (mutog.Health.IntValue < mutog.Health.IntMax)
                            {
                                info += $"Health: <color=#CC3333>{mutog.Health.IntValue}/{mutog.Health.IntMax}</color>";
                            }
                            else
                            {
                                info += $"Health: {mutog.Health.IntValue}/{mutog.Health.IntMax}";
                            }
                            info += "</size>";
                            info += "\n";
                        }
                    }

                    List<GeoCharacter> vehicles = __instance.PxBase.SoldiersInBase.Where(c => c.TemplateDef.IsVehicle).ToList();
                    if (vehicles.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>VEHICLES</color></size>";
                        foreach (GeoCharacter vehicle in vehicles)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{vehicle.DisplayName}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            if (vehicle.Health.IntValue < vehicle.Health.IntMax)
                            {
                                info += $"Health: <color=#CC3333>{vehicle.Health.IntValue}/{vehicle.Health.IntMax}</color>";
                            }
                            else
                            {
                                info += $"Health: {vehicle.Health.IntValue}/{vehicle.Health.IntMax}";
                            }
                            info += "</size>";
                            info += "\n";   
                        }
                    }

                    List<GeoVehicle> aircrafts = __instance.PxBase.VehiclesAtBase.ToList();
                    if (aircrafts.Count > 0)
                    {
                        info += "\n";
                        info += "<size=42><color=#ECBA62>AIRCRAFT</color></size>";
                        foreach (GeoVehicle aircraft in aircrafts)
                        {
                            info += "\n";
                            info += $"<size=36><color=#FFFFFF>{aircraft.Name}</color></size>";
                            info += "\n";
                            info += "<size=30>";
                            info += $"Health: {(aircraft.VehicleDef.BaseStats.MaxMaintenancePoints - aircraft.VehicleDef.BaseStats.MaintenancePoints)}/{aircraft.VehicleDef.BaseStats.MaxMaintenancePoints}";
                            info += ", ";
                            info += $"Space: {aircraft.UsedCharacterSpace}/{aircraft.MaxCharacterSpace}";
                            info += "</size>";
                            info += "\n";
                        }
                    }


                    // Attach tooltip
                    GameObject anchorGo = anchor.gameObject;
                    if (anchorGo.GetComponent<UITooltipText>() != null)
                    {
                        Logger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip already exists. Refreshing.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; //Default: 140
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
                        anchorGo.GetComponent<UITooltipText>().UpdateText(info);
                    }
                    else
                    {
                        anchorGo.AddComponent<UITooltipText>();
                        //anchorGo.AddComponent<CanvasRenderer>();

                        Logger.Debug($"[UIModuleBaseLayout_SetLeftSideInfo_POSTFIX] Tooltip not found. Creating.");
                        anchorGo.GetComponent<UITooltipText>().MaxWidth = 280; //Default: 140
                        anchorGo.GetComponent<UITooltipText>().TipText = info;
                        anchorGo.GetComponent<UITooltipText>().UpdateText(info);
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
