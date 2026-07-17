using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.UI;
using System;

namespace BinhShelfCalculator.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdOpenShelfLibrary : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                ShelfLibraryService service = new ShelfLibraryService();
                ShelfLibraryWindow window = new ShelfLibraryWindow(service);
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Shelf Library", ex.ToString());
                return Result.Failed;
            }
        }
    }
}
