using System;

namespace HGC.Models
{
    public class PcPowerSupply
    {
        // ================= BASIC =================

        public string Manufacturer { get; set; } = "";

        public string Model { get; set; } = "";

        public string Power { get; set; } = "";

        // ================= FORMAT / STANDARD =================

        public string FormFactor { get; set; } = "";

        public string StandardVersion { get; set; } = "";

        // ================= CERTIFICATE / MODULARITY =================

        public string Certificate { get; set; } = "";

        public string Modularity { get; set; } = "";

        // ================= PROTECTIONS =================

        public bool HasOcp { get; set; }

        public bool HasOvp { get; set; }

        public bool HasUvp { get; set; }

        public bool HasOpp { get; set; }

        public bool HasOtp { get; set; }

        public bool HasScp { get; set; }

        public bool HasSip { get; set; }

        public bool HasNlo { get; set; }

        // ================= POWER RAILS =================

        public bool HasPowerRails { get; set; }

        public string Rail12V1 { get; set; } = "";

        public string Rail12V2 { get; set; } = "";

        public string Rail12V3 { get; set; } = "";

        public string Rail12V4 { get; set; } = "";

        public string Rail5V { get; set; } = "";

        public string Rail33V { get; set; } = "";

        // ================= PURCHASE =================

        public DateTime? PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }
    }
}