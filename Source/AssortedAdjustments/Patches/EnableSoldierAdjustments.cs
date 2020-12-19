using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Entities.Characters;

namespace AssortedAdjustments.Patches
{
    internal static class SoldierAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<BaseStatSheetDef> baseStatSheetDefs = defRepository.DefRepositoryDef.AllDefs.OfType<BaseStatSheetDef>().ToList();
            foreach (BaseStatSheetDef bssDef in baseStatSheetDefs)
            {
                bssDef.PersonalAbilitiesCount = AssortedAdjustments.Settings.PersonalAbilitiesCount;
                bssDef.MaxStrength = AssortedAdjustments.Settings.MaxStrength;
                bssDef.MaxWill = AssortedAdjustments.Settings.MaxWill;
                bssDef.MaxSpeed = AssortedAdjustments.Settings.MaxSpeed;

                Logger.Info($"[SoldierAdjustments_Apply] bssDef: {bssDef.name}, GUID: {bssDef.Guid}, PersonalAbilitiesCount: {bssDef.PersonalAbilitiesCount}, Attributes: {bssDef.MaxStrength}, {bssDef.MaxWill}, {bssDef.MaxSpeed}");
            }
        }
    }
}
