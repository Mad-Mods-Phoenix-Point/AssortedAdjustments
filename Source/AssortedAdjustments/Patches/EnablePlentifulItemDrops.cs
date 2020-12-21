using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using PhoenixPoint.Common.Entities.Items;
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

                    if (!AssortedAdjustments.Settings.OverrideWeaponDrops)
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



    // Reference for later additions
    /*
    [HarmonyPatch(typeof(DieAbility), "DropItems")]
    public static class DieAbility_DropItems_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.EnablePlentifulItemDrops && AssortedAdjustments.Settings.AllowWeaponDrops;
        }

        public static bool Prefix(DieAbility __instance)
        {
            try
            {
                Logger.Debug($"[DieAbility_DropItems_PREFIX] TacticalActorBase: {__instance.TacticalActorBase.DisplayName}");
                Logger.Debug($"[DieAbility_DropItems_PREFIX] DieAbilityDef.DestroyItems: {__instance.DieAbilityDef.DestroyItems}");

                MethodInfo ___GetDroppableItems = typeof(DieAbility).GetMethod("GetDroppableItems", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo ___ShouldDestroyItem = typeof(DieAbility).GetMethod("ShouldDestroyItem", BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (TacticalItem item in (IEnumerable<TacticalItem>)___GetDroppableItems.Invoke(__instance, null))
                {
                    Logger.Debug($"[DieAbility_DropItems_PREFIX] TacticalItem: {item.DisplayName}");
                }

                if (__instance.DieAbilityDef.DestroyItems)
                {
                    foreach (TacticalItem item in (IEnumerable<TacticalItem>)___GetDroppableItems.Invoke(__instance, null))
                    {
                        Logger.Debug($"[DieAbility_DropItems_PREFIX] Destroy: {item.DisplayName}");
                        item.InventoryComponent.RemoveItem(item);
                        item.Destroy();
                    }
                    return false;
                }



                InventoryComponent inventoryComponent = __instance.TacticalActorBase.Mount == null ? null : __instance.TacticalActorBase.Mount.TacticalActorBase?.Inventory;

                if (inventoryComponent != null)
                {
                    foreach (TacticalItem item in (IEnumerable<TacticalItem>)___GetDroppableItems.Invoke(__instance, null))
                    {
                        if (!item.IsBodyPart && !(item.InventoryComponent == null))
                        {
                            item.InventoryComponent.RemoveItem(item);
                            if ((bool)___ShouldDestroyItem.Invoke(__instance, new object[] { item }))
                            {
                                item.Destroy();
                            }
                            else
                            {
                                inventoryComponent.AddItem(item, null);
                            }
                        }
                    }
                }

                foreach (TacticalItem item in (IEnumerable<TacticalItem>)___GetDroppableItems.Invoke(__instance, null))
                {
                    Logger.Info($"[DieAbility_DropItems_PREFIX] Item: {item.DisplayName}, DestroyOnActorDeathPerc: {item.TacticalItemDef.DestroyOnActorDeathPerc}");

                    if ((bool)___ShouldDestroyItem.Invoke(__instance, new object[] { item }))
                    {
                        item.InventoryComponent.RemoveItem(item);
                        item.Destroy();
                    }
                    else
                    {
                        Logger.Info($"[DieAbility_DropItems_PREFIX] Drop: {item.DisplayName}");
                        item.Drop(__instance.TacticalActorBase.TacticalLevel.SharedData.FallDownItemContainerDef, __instance.TacticalActor);
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return true;
            }
        }
    }
    */
}
