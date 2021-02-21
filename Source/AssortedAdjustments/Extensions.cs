using System;

namespace AssortedAdjustments
{
    internal static class StringExtensions
    {
        public static bool Contains(this string haystack, string needle, StringComparison comp)
        {
            return haystack?.IndexOf(needle, comp) >= 0;
        }
    }
}
