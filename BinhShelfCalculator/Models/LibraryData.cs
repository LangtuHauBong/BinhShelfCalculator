using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BinhShelfCalculator.Models
{
    [DataContract]
    public class LibraryData
    {
        [DataMember] public List<ShelfProfile> ShelfProfiles { get; set; }
        [DataMember] public List<ItemBoxType> ItemBoxTypes { get; set; }

        public LibraryData()
        {
            ShelfProfiles = new List<ShelfProfile>();
            ItemBoxTypes = new List<ItemBoxType>();
        }
    }
}