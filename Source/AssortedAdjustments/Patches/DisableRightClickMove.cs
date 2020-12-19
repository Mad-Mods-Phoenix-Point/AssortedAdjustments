using System;
using System.Reflection;
using Harmony;

using PhoenixPoint.Tactical.View.ViewModules;

namespace AssortedAdjustments.Patches
{
    [HarmonyPatch]
    public static class UIStateCharacterSelected_OnRightClickMove_Patch
    {
        public static bool Prepare()
        {
            return AssortedAdjustments.Settings.DisableRightClickMove;
        }

        public static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("UIStateCharacterSelected");
            return AccessTools.Method(type, "OnRightClickMove");
        }

        public static bool Prefix(object __instance)
        {
            try
            {
                Logger.Debug($"[UIStateCharacterSelected_OnRightClickMove_PREFIX] Preventing right click movement.");

                Type UIStateCharacterSelected = AccessTools.TypeByName("UIStateCharacterSelected");
                UIModuleTacticalContextualMenu _contextualMenuModule = (UIModuleTacticalContextualMenu)AccessTools.Property(UIStateCharacterSelected, "_contextualMenuModule").GetValue(__instance, null);

                if (_contextualMenuModule.IsContextualMenuVisible)
                {
                    Traverse CloseContextualMenu = Traverse.Create(__instance).Method("CloseContextualMenu", true);
                    CloseContextualMenu.GetValue();
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
}
