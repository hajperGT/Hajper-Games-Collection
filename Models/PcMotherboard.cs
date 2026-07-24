using System;

namespace HGC.Models
{
    public class PcMotherboard
    {
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";
        public bool HasChipset { get; set; }
        public string Chipset { get; set; } = "";
        public bool HasBiosVersion { get; set; }
        public string BiosVersion { get; set; } = "";
        public bool HasRevision { get; set; }
        public string Revision { get; set; } = "";
        public bool HasSerialNumber { get; set; }
        public string SerialNumber { get; set; } = "";
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
    }
}