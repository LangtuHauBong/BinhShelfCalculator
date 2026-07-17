using System;
using System.Runtime.Serialization;

namespace BinhShelfCalculator.Models
{
    [DataContract]
    public class ItemBoxType
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public double LengthMm { get; set; }
        [DataMember] public double WidthMm { get; set; }
        [DataMember] public double HeightMm { get; set; }
        [DataMember] public int QuantityPerBox { get; set; }
        [DataMember] public string Note { get; set; }

        public ItemBoxType()
        {
            Id = Guid.NewGuid().ToString();
            Name = "Vật dụng mới";
            LengthMm = 100;
            WidthMm = 100;
            HeightMm = 100;
            QuantityPerBox = 1;
            Note = "";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}