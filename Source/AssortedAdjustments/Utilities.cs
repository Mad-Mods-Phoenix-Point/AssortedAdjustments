using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities;

namespace AssortedAdjustments
{
    internal static class Utilities
    {
        public static bool GetKeyByTemplate(TacCharacterDef template, out string key)
        {
            key = null;
            List<string> characters = new List<string> { "Sophia", "Jacob", "Omar", "Irina", "Takeshi" };
            List<string> classes = new List<string> { "Assault", "Heavy", "Sniper", "Berserker", "Priest", "Technician", "Infiltrator" };

            foreach (string s in characters.Concat(classes))
            {
                if (template.name.Contains(s))
                {
                    key = s;
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(string str, List<string> list)
        {
            foreach (string item in list)
            {
                if (str.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
