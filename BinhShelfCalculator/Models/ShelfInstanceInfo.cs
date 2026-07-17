using Autodesk.Revit.DB;

namespace BinhShelfCalculator.Models
{
    public class ShelfInstanceInfo
    {
        public ElementId ElementId { get; set; }
        public string FamilyName { get; set; }
        public string TypeName { get; set; }
        public double LengthMm { get; set; }
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }

        public string SizeText
        {
            get
            {
                return string.Format("{0:0} x {1:0} x {2:0} mm", LengthMm, WidthMm, HeightMm);
            }
        }
    }
}
