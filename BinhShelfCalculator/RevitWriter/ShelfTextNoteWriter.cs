using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace BinhShelfCalculator.RevitWriter
{
    public static class ShelfTextNoteWriter
    {
        public static void CreateTextNoteNearElement(Document doc, View view, Element element, string text)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Nội dung TextNote đang trống.");

            ElementId textTypeId = new FilteredElementCollector(doc)
                .OfClass(typeof(TextNoteType))
                .Cast<TextNoteType>()
                .Select(x => x.Id)
                .FirstOrDefault();

            if (textTypeId == null || textTypeId == ElementId.InvalidElementId)
            {
                throw new Exception("Không tìm thấy TextNoteType trong project Revit.");
            }

            XYZ point = GetElementLocationPoint(element);

            using (Transaction tx = new Transaction(doc, "Binh Shelf - Create Text Note"))
            {
                tx.Start();
                TextNote.Create(doc, view.Id, point, text, textTypeId);
                tx.Commit();
            }
        }

        private static XYZ GetElementLocationPoint(Element element)
        {
            LocationPoint locationPoint = element.Location as LocationPoint;
            if (locationPoint != null)
            {
                return locationPoint.Point;
            }

            LocationCurve locationCurve = element.Location as LocationCurve;
            if (locationCurve != null && locationCurve.Curve != null)
            {
                return locationCurve.Curve.Evaluate(0.5, true);
            }

            return XYZ.Zero;
        }
    }
}
