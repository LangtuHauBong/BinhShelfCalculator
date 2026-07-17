using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.Models;
using BinhShelfCalculator.RevitReader;
using BinhShelfCalculator.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinhShelfCalculator.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdCalculateShelf : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null || uidoc.Document == null)
            {
                message = "Không tìm thấy document Revit đang mở.";
                return Result.Failed;
            }

            Document doc = uidoc.Document;

            try
            {
                Element selectedElement = GetSelectedOrPickShelf(uidoc);
                if (selectedElement == null)
                {
                    return Result.Cancelled;
                }

                ShelfInstanceInfo shelfInfo = ShelfInstanceReader.Read(doc, selectedElement);
                ShelfLibraryService service = new ShelfLibraryService();

                ShelfCalculateWindow window = new ShelfCalculateWindow(doc, selectedElement, shelfInfo, service);
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Calculate Shelf", ex.Message);
                return Result.Failed;
            }
        }

        private Element GetSelectedOrPickShelf(UIDocument uidoc)
        {
            ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            if (selectedIds != null && selectedIds.Count == 1)
            {
                Element element = doc.GetElement(selectedIds.First());
                if (element != null && ShelfParameterValidator.HasRequiredSizeParameters(element))
                {
                    return element;
                }
            }

            Reference reference = uidoc.Selection.PickObject(
                ObjectType.Element,
                new ShelfSelectionFilter(),
                "Chọn một giá/kệ có tham số DÀI, RỘNG, CAO");

            return doc.GetElement(reference.ElementId);
        }
    }
}
