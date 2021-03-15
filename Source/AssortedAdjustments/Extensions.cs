using PhoenixPoint.Common.Entities.Items;
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

    internal static class ItemManufacturingExtensions
    {
        public static void Cancel(this ItemManufacturing itemManufacturing, ItemManufacturing.ManufactureQueueItem item)
        {
            itemManufacturing.Queue.Remove(item);
        }
    }
}
