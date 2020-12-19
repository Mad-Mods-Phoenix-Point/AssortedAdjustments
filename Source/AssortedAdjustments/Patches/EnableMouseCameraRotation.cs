namespace AssortedAdjustments.Patches
{
    // Entrypoint for camera rotation on right click drag?
    /*
    [HarmonyPatch(typeof(TacticalViewState), "OnInputEventInternal")]
    public static class TacticalViewState_OnInputEventInternal_Patch
    {
        public static bool Prefix(TacticalViewState __instance, InputEvent ev)
        {
            try
            {
                if (ev.Name == "Right Mouse Click Move" && ev.InputType == InputType.KeyboardMouse)
                {
                    Logger.Debug($"[TacticalViewState_OnInputEventInternal_PREFIX] Disable Right Click Move");
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
    */
}
