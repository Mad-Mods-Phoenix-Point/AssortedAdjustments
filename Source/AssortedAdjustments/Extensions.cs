using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Levels;
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
			ManufacturableItem manufacturableItem = item.ManufacturableItem;
			itemManufacturing.Queue.Remove(item);

			if (!itemManufacturing.UseFactoryManufacturing)
			{
                GeoFaction ___faction = (GeoFaction)AccessTools.Field(typeof(ItemManufacturing), "_faction").GetValue(itemManufacturing);
                ___faction.Wallet.Give(manufacturableItem.ManufacturePrice, OperationReason.Scrap);
			}
		}
    }
}
