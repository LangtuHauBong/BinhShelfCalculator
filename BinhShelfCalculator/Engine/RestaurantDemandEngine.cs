using BinhShelfCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinhShelfCalculator.Engine
{
    public class RestaurantDemandResult
    {
        public int TableCount { get; set; }
        public int GuestsPerTable { get; set; }
        public int TotalGuests { get; set; }
        public ItemBoxType Item { get; set; }
        public int RequiredQuantity { get; set; }
    }

    public static class RestaurantDemandEngine
    {
        public static RestaurantDemandResult Calculate(int tableCount, int guestsPerTable, ItemBoxType item)
        {
            if (tableCount <= 0)
            {
                throw new ArgumentException("Số bàn phải lớn hơn 0.");
            }

            if (guestsPerTable <= 0)
            {
                throw new ArgumentException("Số khách / bàn phải lớn hơn 0.");
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.QuantityPerGuest <= 0)
            {
                throw new ArgumentException("Số lượng trên 1 khách của vật dụng phải lớn hơn 0.");
            }

            int totalGuests = checked(tableCount * guestsPerTable);
            int requiredQuantity = (int)Math.Ceiling(totalGuests * item.QuantityPerGuest);

            return new RestaurantDemandResult
            {
                TableCount = tableCount,
                GuestsPerTable = guestsPerTable,
                TotalGuests = totalGuests,
                Item = item,
                RequiredQuantity = requiredQuantity
            };
        }

        public static int CalculateCupQuantity(int tableCount, int guestsPerTable, IEnumerable<ItemBoxType> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ItemBoxType cup = items.FirstOrDefault(x =>
            {
                string name = (x.Name ?? string.Empty).Trim().ToLowerInvariant();
                return name.Contains("cốc") || name.Contains("coc");
            });

            if (cup == null)
            {
                throw new ArgumentException("Không tìm thấy vật dụng 'Cốc' trong Shelf Library.");
            }

            return Calculate(tableCount, guestsPerTable, cup).RequiredQuantity;
        }
    }
}
