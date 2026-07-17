using Autodesk.Revit.UI;
using System;
using System.Reflection;

namespace BinhShelfCalculator
{
    public class App : IExternalApplication
    {
        private const string TabName = "Binh Shelf";
        private const string PanelName = "Shelf Calculator";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                try
                {
                    application.CreateRibbonTab(TabName);
                }
                catch
                {
                    // Tab already exists.
                }

                RibbonPanel panel = null;
                foreach (RibbonPanel existingPanel in application.GetRibbonPanels(TabName))
                {
                    if (existingPanel.Name == PanelName)
                    {
                        panel = existingPanel;
                        break;
                    }
                }

                if (panel == null)
                {
                    panel = application.CreateRibbonPanel(TabName, PanelName);
                }

                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                PushButtonData libraryButtonData = new PushButtonData(
                    "BSC_OpenLibrary",
                    "Shelf\nLibrary",
                    assemblyPath,
                    "BinhShelfCalculator.Commands.CmdOpenShelfLibrary");
                libraryButtonData.ToolTip = "Quản lý thư viện cấu hình kệ và vật dụng dạng box.";

                PushButtonData calculateButtonData = new PushButtonData(
                    "BSC_CalculateShelf",
                    "Calculate\nShelf",
                    assemblyPath,
                    "BinhShelfCalculator.Commands.CmdCalculateShelf");
                calculateButtonData.ToolTip = "Chọn giá/kệ trong Revit, đọc DÀI/RỘNG/CAO và tính sức chứa vật dụng.";

                panel.AddItem(libraryButtonData);
                panel.AddSeparator();
                panel.AddItem(calculateButtonData);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Binh Shelf Calculator", "Không thể khởi động add-in:\n" + ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
