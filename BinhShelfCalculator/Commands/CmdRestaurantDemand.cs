using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.UI;
using System;

namespace BinhShelfCalculator.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRestaurantDemand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                ShelfLibraryService service = new ShelfLibraryService();
                RestaurantDemandWindow window = new RestaurantDemandWindow(service);
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Restaurant Demand", ex.Message);
                return Result.Failed;
            }
        }
    }
}
