using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace BinhShelfCalculator.RevitReader
{
    public class ShelfSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return ShelfParameterValidator.HasRequiredSizeParameters(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
