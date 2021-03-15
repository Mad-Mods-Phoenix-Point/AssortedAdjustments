using Base.Core;
using Base.Defs;
using Base.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Statuses;
using System.Collections.Generic;
using System.Linq;

namespace AssortedAdjustments.Patches
{
    internal static class AbilityAdjustments
    {
        public static void Apply()
        {
            DefRepository defRepository = GameUtl.GameComponent<DefRepository>();

            List<string> tacticalAbilitiesToChange = new List<string> { "RetrieveTurret", "BigBooms" };
            foreach (TacticalAbilityDef taDef in defRepository.DefRepositoryDef.AllDefs.OfType<TacticalAbilityDef>().Where(d => tacticalAbilitiesToChange.Any(s => d.name.Contains(s))))
            {
                if (taDef.name.Contains("RetrieveTurret"))
                {
                    taDef.ActionPointCost = 0.25f;
                }
                else if (taDef.name.Contains("BigBooms"))
                {
                    taDef.WillPointCost = 4f;
                }
                Logger.Info($"[AbilityAdjustments_Apply] taDef: {taDef.name}, GUID: {taDef.Guid}, ActionPointCost: {taDef.ActionPointCost}, WillPointCost: {taDef.WillPointCost}, Description: {taDef.ViewElementDef?.Description.Localize()}");
            }

            List<string> passiveAbilitiesToChange = new List<string> { "Cautious", "Reckless", "Strongman" };
            foreach (PassiveModifierAbilityDef pmaDef in defRepository.DefRepositoryDef.AllDefs.OfType<PassiveModifierAbilityDef>().Where(d => passiveAbilitiesToChange.Any(s => d.name.Contains(s))))
            {
                if (pmaDef.name.Contains("Cautious"))
                {
                    pmaDef.StatModifications[0].Value = 0.95f;
                    pmaDef.ViewElementDef.Description = new LocalizedTextBind("20% bonus accuracy and -5% damage dealt", true);
                }
                else if (pmaDef.name.Contains("Reckless"))
                {
                    pmaDef.StatModifications[1].Value = -0.05f;
                    pmaDef.ViewElementDef.Description = new LocalizedTextBind("10% bonus damage dealt and -5% accuracy", true);
                }
                else if (pmaDef.name.Contains("Strongman"))
                {
                    pmaDef.StatModifications[0].Value = -10f;
                    pmaDef.ViewElementDef.Description = new LocalizedTextBind("Gain Heavy weapons proficiency with +20% accuracy, +2 Strength and -10 perception", true);
                }

                Logger.Info($"[AbilityAdjustments_Apply] pmaDef: {pmaDef.name}, GUID: {pmaDef.Guid}, Description: {pmaDef.ViewElementDef?.Description.Localize()}");
                for (int i = 0; i < pmaDef.StatModifications.Length; i++)
                {
                    Logger.Info($"[AbilityAdjustments_Apply] StatModifications[{i}] => TargetStat: {pmaDef.StatModifications[i].TargetStat}, Modification: {pmaDef.StatModifications[i].Modification}, Value: {pmaDef.StatModifications[i].Value}");
                }
            }



            // Frenzy
            foreach (FrenzyStatusDef def in defRepository.DefRepositoryDef.AllDefs.OfType<FrenzyStatusDef>())
            {
                if (def.name.Contains("Frenzy")) // This is needed as there are are than one
                {
                    def.SpeedCoefficient = 0.33f;
                    def.Visuals.Description = new LocalizedTextBind("Speed increase of 33% and panic immunity", true);

                    Logger.Info($"[AbilityAdjustments_Apply] ({def.name}), SpeedCoefficient: {def.SpeedCoefficient}, Description: {def.Visuals.Description.Localize()}");
                }
            }
            foreach (InstilFrenzyAbilityDef def in defRepository.DefRepositoryDef.AllDefs.OfType<InstilFrenzyAbilityDef>())
            {
                def.ViewElementDef.Description = new LocalizedTextBind("Instills Frenzy in allies within 20 tiles for two turns, increasing their speed by 33% and granting them panic immunity", true);

                Logger.Info($"[AbilityAdjustments_Apply] ({def.name}), Description: {def.ViewElementDef.Description.Localize()}");
            }
            foreach (HealAbilityDef def in defRepository.DefRepositoryDef.AllDefs.OfType<HealAbilityDef>())
            {
                if (def.name.Contains("Stimpack"))
                {
                    def.ViewElementDef.Description = new LocalizedTextBind("Instills Frenzy for two turns, increasing speed by 33% and granting panic immunity", true);

                    Logger.Info($"[AbilityAdjustments_Apply] ({def.name}), Description: {def.ViewElementDef.Description.Localize()}");
                }
            }
        }
    }
}
