using Autodesk.Revit.DB;

namespace BinhShelfCalculator.RevitReader
{
    public static class ShelfParameterValidator
    {
        public const string ParameterLength = "DÀI";
        public const string ParameterWidth = "RỘNG";
        public const string ParameterHeight = "CAO";

        public static bool HasRequiredSizeParameters(Element element)
        {
            if (element == null) return false;

            return element.LookupParameter(ParameterLength) != null
                && element.LookupParameter(ParameterWidth) != null
                && element.LookupParameter(ParameterHeight) != null;
        }
    }
}
