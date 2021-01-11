using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsSharedData;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch(typeof(DieAbility), "ShouldDestroyItem")]
    public static class DieAbility_ShouldDestroyItem_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.EnablePlentifulItemDrops;
        }

        public static void Prefix(DieAbility __instance, TacticalItem item)
        {
            try
            {
                Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] Item: {item.DisplayName}, DestroyOnActorDeathPerc: {item.TacticalItemDef.DestroyOnActorDeathPerc}");

                if (item.TacticalItemDef is WeaponDef wDef)
                {
                    Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] {item.DisplayName} is a weapon.");

                    if (!AssortedAdjustments.Settings.AllowWeaponDrops)
                    {
                        Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] destructionChance: {item.TacticalItemDef.DestroyOnActorDeathPerc}");
                        return;
                    }
                    else
                    {
                        if (AssortedAdjustments.Settings.HealthBasedWeaponDestruction)
                        {
                            float currentHealth = item.GetHealth().IntValue;
                            float maxHealth = item.GetHealth().IntMax;
                            int healthPercent = (int)((currentHealth / maxHealth) * 100);
                            int destructionChance = 100 - healthPercent;
                            Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] currentHealth: {currentHealth}");
                            Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] maxHealth: {maxHealth}");
                            Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] healthPercent: {healthPercent}");
                            Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] destructionChance: {destructionChance}");

                            item.TacticalItemDef.DestroyOnActorDeathPerc = destructionChance;
                        }
                        else
                        {
                            Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] destructionChance: {AssortedAdjustments.Settings.FlatWeaponDestructionChance}");

                            item.TacticalItemDef.DestroyOnActorDeathPerc = AssortedAdjustments.Settings.FlatWeaponDestructionChance;
                        }
                    }
                }
                else
                {
                    Logger.Info($"[DieAbility_ShouldDestroyItem_PREFIX] destructionChance: {AssortedAdjustments.Settings.ItemDestructionChance}");

                    item.TacticalItemDef.DestroyOnActorDeathPerc = AssortedAdjustments.Settings.ItemDestructionChance;
                }

                // If items should NEVER get destroyed by chance, modifiy result to false and skip original method
                /*
                if (AssortedAdjustments.Settings.DestroyOnActorDeathPercent <= 0)
                {
                    __result = false;
                    return false;
                }
                */
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Postfix(DieAbility __instance, bool __result, TacticalItem item)
        {
            string result = __result ? "destroyed" : "dropped";
            Logger.Info($"[DieAbility_ShouldDestroyItem_POSTFIX] {item.DisplayName} will be {result}.");
        }
    }



    // Drop armor too
    [HarmonyPatch(typeof(DieAbility), "DropItems")]
    public static class DieAbility_DropItems_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.EnablePlentifulItemDrops && AssortedAdjustments.Settings.AllowArmorDrops;
        }

        public static void Postfix(DieAbility __instance)
        {
            try
            {
                TacticalActor actor = __instance.TacticalActor;
                IEnumerable<TacticalItem> items = actor?.BodyState?.GetArmourItems();
                if (items?.Any() != true)
                {
                    return;
                }

                Logger.Debug($"[DieAbility_DropItems_POSTFIX] Try to drop armor of {actor.ViewElementDef?.Name}");

                SharedData sharedData = SharedData.GetSharedDataFromGame();
                SharedGameTagsDataDef sharedGameTags = sharedData.SharedGameTags;
                GameTagDef armor = sharedGameTags.ArmorTag, manufacturable = sharedGameTags.ManufacturableTag, mounted = sharedGameTags.MountedTag;

                int count = 0;
                foreach (TacticalItem item in items)
                {
                    TacticalItemDef def = item.ItemDef as TacticalItemDef;
                    GameTagsList tags = def?.Tags;
                    if (tags == null || tags.Count == 0 || !tags.Contains(manufacturable) || def.IsPermanentAugment)
                    {
                        continue;
                    }
                    if (tags.Contains(armor) || tags.Contains(mounted))
                    {
                        Logger.Info($"[DieAbility_DropItems_POSTFIX] Dropping {def.ViewElementDef.Name}");
                        item.Drop(sharedData.FallDownItemContainerDef, actor);
                        count++;
                    }
                }

                if (count > 0)
                {
                    Logger.Info($"[DieAbility_DropItems_POSTFIX] Dropped {count} pieces from {actor.ViewElementDef?.Name} of {actor.TacticalFaction?.Faction?.FactionDef?.GetName()}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    // Prevent dupes from squad member deaths
    [HarmonyPatch(typeof(GeoMission), "GetDeadSquadMembersArmour")]
    public static class GeoMission_GetDeadSquadMembersArmour_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.EnablePlentifulItemDrops && AssortedAdjustments.Settings.AllowArmorDrops;
        }

        // Override!
        public static bool Prefix(GeoMission __instance, ref IEnumerable<GeoItem> __result)
        {
            try
            {
                Logger.Debug($"[GeoMission_GetDeadSquadMembersArmour_PREFIX] Discard armor of dead squaddies as it should already have been added when they died...");
                __result = Enumerable.Empty<GeoItem>();

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
