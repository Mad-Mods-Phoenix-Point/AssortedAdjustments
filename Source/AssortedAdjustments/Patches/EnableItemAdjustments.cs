using Base.Core;
using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities.Weapons;
using System.Collections.Generic;
using System.Linq;

namespace AssortedAdjustments.Patches
{
    internal static class ItemAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<string> weaponsToChange = new List<string> { "Subjector", "HeavyRocketLauncher", "ShotgunRifle", "LaserArrayPack" };
            foreach (WeaponDef wDef in defRepository.DefRepositoryDef.AllDefs.OfType<WeaponDef>().Where(d => weaponsToChange.Any(s => d.name.Contains(s))))
            {
                if (wDef.name.Contains("Subjector"))
                {
                    wDef.SpreadDegrees = 0.55f; // Default: 0.8
                    wDef.DamagePayload.DamageKeywords[0].Value = 60; // Damage, default: 50
                    wDef.DamagePayload.DamageKeywords[1].Value = 30; // Pierce, default: 20
                    wDef.DamagePayload.DamageKeywords[2].Value = 15; // Virus, default: 10

                    Logger.Info($"[ItemAdjustments_Apply] wDef: {wDef.name}, SpreadDegrees: {wDef.SpreadDegrees}, Damage: {wDef.DamagePayload.DamageKeywords[0].Value}, Pirece: {wDef.DamagePayload.DamageKeywords[1].Value}, Virus: {wDef.DamagePayload.DamageKeywords[2].Value}");
                }
                else if (wDef.name.Contains("HeavyRocketLauncher"))
                {
                    wDef.SpreadDegrees = 0.25f; // Default: 0

                    Logger.Info($"[ItemAdjustments_Apply] wDef: {wDef.name}, SpreadDegrees: {wDef.SpreadDegrees}");
                }
                else if (wDef.name.Contains("ShotgunRifle"))
                {
                    wDef.SpreadDegrees = 3f; // Default: 4

                    Logger.Info($"[ItemAdjustments_Apply] wDef: {wDef.name}, SpreadDegrees: {wDef.SpreadDegrees}");
                }
                else if (wDef.name.Contains("LaserArrayPack"))
                {
                    wDef.SpreadDegrees = 0.1f; // Default: 0
                    wDef.Tags.Remove(Utilities.GetGameTagDef("ExplosiveWeapon_TagDef"));
                    wDef.DamagePayload.DamageKeywords[0].Value = 55f;

                    Logger.Info($"[ItemAdjustments_Apply] wDef: {wDef.name}, SpreadDegrees: {wDef.SpreadDegrees}, Damage: {wDef.DamagePayload.DamageKeywords[0].Value}, Tags: {wDef.Tags.Join()}");
                }
            }
        }
    }
}
