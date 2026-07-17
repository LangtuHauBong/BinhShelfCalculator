using Autodesk.Revit.DB;
using System;

namespace BinhShelfCalculator.RevitWriter
{
    public static class ShelfMarkWriter
    {
        public static void WriteCupQuantity(Document document, Element element, int cupQuantity)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (cupQuantity < 0)
            {
                throw new ArgumentException("Số lượng cốc không được âm.");
            }

            Parameter markParameter = element.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

            if (markParameter == null)
            {
                throw new InvalidOperationException("Đối tượng được chọn không có tham số Mark.");
            }

            if (markParameter.IsReadOnly)
            {
                throw new InvalidOperationException("Tham số Mark của đối tượng đang ở chế độ chỉ đọc.");
            }

            using (Transaction transaction = new Transaction(document, "Binh Shelf - Write Cup Mark"))
            {
                transaction.Start();

                if (!markParameter.Set("CỐC " + cupQuantity))
                {
                    transaction.RollBack();
                    throw new InvalidOperationException("Revit không cho phép ghi giá trị vào Mark.");
                }

                transaction.Commit();
            }
        }
    }
}
