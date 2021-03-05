using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using I2.Loc;
using PhoenixPoint.Tactical.Entities;

namespace AssortedAdjustments
{
    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
    {
        private IEqualityComparer<TValue> valueComparer;

        public DictionaryComparer(IEqualityComparer<TValue> valueComparer = null)
        {
            this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
        {
            if (x.Count != y.Count)
                return false;
            if (x.Keys.Except(y.Keys).Any())
                return false;
            if (y.Keys.Except(x.Keys).Any())
                return false;
            foreach (var pair in x)
                if (!valueComparer.Equals(pair.Value, y[pair.Key]))
                    return false;
            return true;
        }

        public int GetHashCode(Dictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }



    internal static class Utilities
    {
        public static string ToTitleCase(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            TextInfo textInfo = new CultureInfo(LocalizationManager.CurrentLanguageCode).TextInfo;
            return textInfo.ToTitleCase(textInfo.ToLower(s));
        }

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
