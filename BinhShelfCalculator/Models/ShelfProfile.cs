using System;
using System.Runtime.Serialization;

namespace BinhShelfCalculator.Models
{
    [DataContract]
    public class ShelfProfile
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public double BoardThicknessMm { get; set; }
        [DataMember] public double ClearHeightBetweenShelvesMm { get; set; }
        [DataMember] public double SideClearanceMm { get; set; }
        [DataMember] public double FrontBackClearanceMm { get; set; }
        [DataMember] public double HandlingClearanceMm { get; set; }
        [DataMember] public double SafetyFactor { get; set; }

        public ShelfProfile()
        {
            Id = Guid.NewGuid().ToString();
            Name = "Loại kệ mới";
            BoardThicknessMm = 20;
            ClearHeightBetweenShelvesMm = 200;
            SideClearanceMm = 0;
            FrontBackClearanceMm = 0;
            HandlingClearanceMm = 80;
            SafetyFactor = 1.0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}