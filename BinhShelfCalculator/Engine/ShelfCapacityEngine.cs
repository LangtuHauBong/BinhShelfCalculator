using BinhShelfCalculator.Models;
using System;

namespace BinhShelfCalculator.Engine
{
    public static class ShelfCapacityEngine
    {
        public static ShelfCalculationResult Calculate(
            ShelfInstanceInfo shelf,
            ShelfProfile profile,
            ItemBoxType item,
            bool useRecommendedClearHeight,
            double manualClearHeightMm)
        {
            if (shelf == null) throw new ArgumentNullException(nameof(shelf));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (item == null) throw new ArgumentNullException(nameof(item));

            ValidatePositive(shelf.LengthMm, "DÀI của kệ");
            ValidatePositive(shelf.WidthMm, "RỘNG của kệ");
            ValidatePositive(shelf.HeightMm, "CAO của kệ");
            ValidatePositive(profile.BoardThicknessMm, "Chiều dày tấm ngăn");
            ValidatePositive(item.LengthMm, "Dài vật dụng");
            ValidatePositive(item.WidthMm, "Rộng vật dụng");
            ValidatePositive(item.HeightMm, "Cao vật dụng");

            if (item.QuantityPerBox <= 0)
            {
                throw new Exception("Số lượng quy đổi / 1 box phải lớn hơn 0.");
            }

            if (profile.SafetyFactor <= 0 || profile.SafetyFactor > 1.0)
            {
                throw new Exception("Hệ số sử dụng phải lớn hơn 0 và nhỏ hơn hoặc bằng 1.0.");
            }

            double usedClearHeight = useRecommendedClearHeight
                ? item.HeightMm + Math.Max(0, profile.HandlingClearanceMm)
                : manualClearHeightMm;

            ValidatePositive(usedClearHeight, "Khoảng cách thông thủy giữa 2 tấm ngăn");

            double usableLength = shelf.LengthMm - 2.0 * Math.Max(0, profile.SideClearanceMm);
            double usableWidth = shelf.WidthMm - 2.0 * Math.Max(0, profile.FrontBackClearanceMm);

            if (usableLength <= 0 || usableWidth <= 0)
            {
                throw new Exception("Kích thước hữu dụng của mặt kệ không hợp lệ. Kiểm tra khoảng hở mép trái/phải và trước/sau.");
            }

            int tierCount = (int)Math.Floor((shelf.HeightMm + profile.BoardThicknessMm) / (usedClearHeight + profile.BoardThicknessMm));
            if (tierCount < 0) tierCount = 0;

            PlacementOption normal = CalculatePlacement(usableLength, usableWidth, item.LengthMm, item.WidthMm, false);
            PlacementOption rotated = CalculatePlacement(usableLength, usableWidth, item.WidthMm, item.LengthMm, true);
            PlacementOption best = rotated.BoxesPerTier > normal.BoxesPerTier ? rotated : normal;

            int boxesBeforeSafety = tierCount * best.BoxesPerTier;
            int boxesAfterSafety = (int)Math.Floor(boxesBeforeSafety * profile.SafetyFactor);
            int quantityBeforeSafety = boxesBeforeSafety * item.QuantityPerBox;
            int quantityAfterSafety = boxesAfterSafety * item.QuantityPerBox;

            string warning = "";
            if (item.HeightMm > usedClearHeight)
            {
                warning = "Chiều cao vật dụng lớn hơn khoảng cách thông thủy giữa 2 tấm ngăn. Kết quả có thể không sử dụng được.";
                tierCount = 0;
                boxesBeforeSafety = 0;
                boxesAfterSafety = 0;
                quantityBeforeSafety = 0;
                quantityAfterSafety = 0;
            }
            else if (tierCount == 0)
            {
                warning = "Không tạo được tầng kệ hữu dụng từ chiều cao kệ và cấu hình đã nhập.";
            }
            else if (best.BoxesPerTier == 0)
            {
                warning = "Vật dụng không đặt vừa trên mặt phẳng mỗi tầng kệ.";
            }

            return new ShelfCalculationResult
            {
                Shelf = shelf,
                ShelfProfile = profile,
                Item = item,
                UsedClearHeightMm = usedClearHeight,
                UsedRecommendedClearHeight = useRecommendedClearHeight,
                UsableLengthMm = usableLength,
                UsableWidthMm = usableWidth,
                ShelfTierCount = tierCount,
                CountAlongLength = best.CountAlongLength,
                CountAlongWidth = best.CountAlongWidth,
                BoxesPerTier = best.BoxesPerTier,
                TotalBoxesBeforeSafety = boxesBeforeSafety,
                TotalBoxesAfterSafety = boxesAfterSafety,
                TotalQuantityBeforeSafety = quantityBeforeSafety,
                TotalQuantityAfterSafety = quantityAfterSafety,
                ItemRotated = best.Rotated,
                Warning = warning
            };
        }

        private static PlacementOption CalculatePlacement(double shelfLength, double shelfWidth, double itemLength, double itemWidth, bool rotated)
        {
            int countLength = itemLength > 0 ? (int)Math.Floor(shelfLength / itemLength) : 0;
            int countWidth = itemWidth > 0 ? (int)Math.Floor(shelfWidth / itemWidth) : 0;
            if (countLength < 0) countLength = 0;
            if (countWidth < 0) countWidth = 0;

            return new PlacementOption
            {
                CountAlongLength = countLength,
                CountAlongWidth = countWidth,
                BoxesPerTier = countLength * countWidth,
                Rotated = rotated
            };
        }

        private static void ValidatePositive(double value, string name)
        {
            if (value <= 0)
            {
                throw new Exception(name + " phải lớn hơn 0.");
            }
        }

        private class PlacementOption
        {
            public int CountAlongLength { get; set; }
            public int CountAlongWidth { get; set; }
            public int BoxesPerTier { get; set; }
            public bool Rotated { get; set; }
        }
    }
}
