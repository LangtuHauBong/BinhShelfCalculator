using System.Text;

namespace BinhShelfCalculator.Models
{
    public class ShelfCalculationResult
    {
        public ShelfInstanceInfo Shelf { get; set; }
        public ShelfProfile ShelfProfile { get; set; }
        public ItemBoxType Item { get; set; }

        public double UsedClearHeightMm { get; set; }
        public bool UsedRecommendedClearHeight { get; set; }
        public double UsableLengthMm { get; set; }
        public double UsableWidthMm { get; set; }
        public int ShelfTierCount { get; set; }
        public int CountAlongLength { get; set; }
        public int CountAlongWidth { get; set; }
        public int BoxesPerTier { get; set; }
        public int TotalBoxesBeforeSafety { get; set; }
        public int TotalBoxesAfterSafety { get; set; }
        public int TotalQuantityBeforeSafety { get; set; }
        public int TotalQuantityAfterSafety { get; set; }
        public bool ItemRotated { get; set; }
        public string Warning { get; set; }

        public string ToReportText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("KẾT QUẢ TÍNH SỨC CHỨA GIÁ/KỆ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Đối tượng: " + (Shelf.TypeName ?? ""));
            sb.AppendLine("Family: " + (Shelf.FamilyName ?? ""));
            sb.AppendLine("Kích thước kệ: " + Shelf.SizeText);
            sb.AppendLine();
            sb.AppendLine("Cấu hình kệ: " + ShelfProfile.Name);
            sb.AppendLine("Chiều dày tấm ngăn: " + ShelfProfile.BoardThicknessMm.ToString("0") + " mm");
            sb.AppendLine("Khoảng cách thông thủy giữa 2 tấm: " + UsedClearHeightMm.ToString("0") + " mm");
            sb.AppendLine("Hệ số sử dụng: " + ShelfProfile.SafetyFactor.ToString("0.##"));
            sb.AppendLine();
            sb.AppendLine("Vật dụng: " + Item.Name);
            sb.AppendLine("Kích thước vật dụng: " + Item.LengthMm.ToString("0") + " x " + Item.WidthMm.ToString("0") + " x " + Item.HeightMm.ToString("0") + " mm");
            sb.AppendLine("Số lượng quy đổi / 1 box: " + Item.QuantityPerBox);
            sb.AppendLine();
            sb.AppendLine("Dài hữu dụng mỗi tầng kệ: " + UsableLengthMm.ToString("0") + " mm");
            sb.AppendLine("Rộng hữu dụng mỗi tầng kệ: " + UsableWidthMm.ToString("0") + " mm");
            sb.AppendLine("Số tầng kệ hữu dụng: " + ShelfTierCount);
            sb.AppendLine("Cách xếp tối ưu: " + CountAlongLength + " theo chiều dài x " + CountAlongWidth + " theo chiều rộng");
            sb.AppendLine("Xoay vật dụng 90°: " + (ItemRotated ? "Có" : "Không"));
            sb.AppendLine("Số box / 1 tầng kệ: " + BoxesPerTier);
            sb.AppendLine("Tổng box trước hệ số: " + TotalBoxesBeforeSafety);
            sb.AppendLine("Tổng box sau hệ số: " + TotalBoxesAfterSafety);
            sb.AppendLine("Tổng số lượng quy đổi trước hệ số: " + TotalQuantityBeforeSafety);
            sb.AppendLine("TỔNG SỐ LƯỢNG SAU HỆ SỐ: " + TotalQuantityAfterSafety);

            if (!string.IsNullOrWhiteSpace(Warning))
            {
                sb.AppendLine();
                sb.AppendLine("Cảnh báo: " + Warning);
            }

            return sb.ToString();
        }
    }
}
