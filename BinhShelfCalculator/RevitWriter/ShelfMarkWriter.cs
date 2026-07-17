using Autodesk.Revit.DB;
using System;

namespace BinhShelfCalculator.RevitWriter
{
    public static class ShelfMarkWriter
    {
        public static string BuildMarkText(string itemName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                throw new ArgumentException("Tên vật dụng không được trống.", nameof(itemName));
            }

            if (quantity < 0)
            {
                throw new ArgumentException("Số lượng vật dụng không được âm.", nameof(quantity));
            }

            string unit = GetUnitName(itemName);
            return "Kệ đựng " + unit + ": " + quantity + " " + unit;
        }

        public static void WriteItemQuantity(Document document, Element element, string itemName, int quantity)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            string markText = BuildMarkText(itemName, quantity);
            Parameter markParameter = element.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

            if (markParameter == null)
            {
                throw new InvalidOperationException("Đối tượng được chọn không có tham số Mark.");
            }

            if (markParameter.IsReadOnly)
            {
                throw new InvalidOperationException("Tham số Mark của đối tượng đang ở chế độ chỉ đọc.");
            }

            using (Transaction transaction = new Transaction(document, "Binh Shelf - Write Item Mark"))
            {
                transaction.Start();

                if (!markParameter.Set(markText))
                {
                    transaction.RollBack();
                    throw new InvalidOperationException("Revit không cho phép ghi giá trị vào Mark.");
                }

                transaction.Commit();
            }
        }

        private static string GetUnitName(string itemName)
        {
            string normalized = itemName.Trim().ToLowerInvariant();

            if (ContainsAny(normalized, "cốc", "coc")) return "cốc";
            if (ContainsAny(normalized, "bát", "bat")) return "bát";
            if (ContainsAny(normalized, "đĩa", "dia")) return "đĩa";
            if (ContainsAny(normalized, "khay")) return "khay";
            if (ContainsAny(normalized, "vỉ", "vi")) return "vỉ";
            if (ContainsAny(normalized, "bình", "binh")) return "bình";
            if (ContainsAny(normalized, "bếp", "bep")) return "bếp";
            if (ContainsAny(normalized, "âu", "au")) return "âu";

            return normalized;
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            foreach (string value in values)
            {
                if (text.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
