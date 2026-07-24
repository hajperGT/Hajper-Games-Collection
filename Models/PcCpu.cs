using System;

namespace HGC.Models
{
    public class PcCpu
    {
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public bool HasSerialNumber { get; set; }
        public string SerialNumber { get; set; } = "";

        public bool HasOverclock { get; set; }
        public string CurrentClock { get; set; } = "";
        public string CurrentVoltage { get; set; } = "";

        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
    }
}