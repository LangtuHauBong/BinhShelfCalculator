using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.RevitReader;
using BinhShelfCalculator.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinhShelfCalculator.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRestaurantDemand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null || uidoc.Document == null)
            {
                message = "Không tìm thấy document Revit đang mở.";
                return Result.Failed;
            }

            try
            {
                Element shelfElement = GetSelectedOrPickShelf(uidoc);
                if (shelfElement == null)
                {
                    return Result.Cancelled;
                }

                ShelfLibraryService service = new ShelfLibraryService();
                RestaurantDemandWindow window = new RestaurantDemandWindow(uidoc.Document, shelfElement, service);
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
                TaskDialog.Show("Restaurant Demand", ex.Message);
                return Result.Failed;
            }
        }

        private Element GetSelectedOrPickShelf(UIDocument uidoc)
        {
            ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
            Document document = uidoc.Document;

            if (selectedIds != null && selectedIds.Count == 1)
            {
                Element selected = document.GetElement(selectedIds.First());
                if (selected != null && ShelfParameterValidator.HasRequiredSizeParameters(selected))
                {
                    return selected;
                }
            }

            Reference reference = uidoc.Selection.PickObject(
                ObjectType.Element,
                new ShelfSelectionFilter(),
                "Chọn một giá/kệ có tham số DÀI, RỘNG, CAO");

            return document.GetElement(reference.ElementId);
        }
    }
}
