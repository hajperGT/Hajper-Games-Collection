using System;

namespace HGC.Models
{
    public class PcRam
    {
        // ================= BASIC =================

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public int ModuleCount { get; set; }

        public string TotalCapacity { get; set; } = "";

        public string MemoryType { get; set; } = "";

        // ================= FEATURES =================

        public bool IsEcc { get; set; }

        public bool IsRegistered { get; set; }

        public string Profile { get; set; } = "";

        // ================= STOCK =================

        public string Clock { get; set; } = "";

        public string Voltage { get; set; } = "";

        // Primary Timings

        public string CL { get; set; } = "";

        public string TRCD { get; set; } = "";

        public string TRP { get; set; } = "";

        public string TRAS { get; set; } = "";

        public string TRC { get; set; } = "";

        public string CommandRate { get; set; } = "";

        // Advanced Timings

        public string TRFC { get; set; } = "";

        public string TREFI { get; set; } = "";

        public string TFAW { get; set; } = "";

        public string TRRDS { get; set; } = "";

        public string TRRDL { get; set; } = "";

        public string TWTRS { get; set; } = "";

        public string TWTRL { get; set; } = "";

        public string TCWL { get; set; } = "";

        // ================= OVERCLOCK =================

        public bool HasOverclock { get; set; }

        public string OcClock { get; set; } = "";

        public string OcVoltage { get; set; } = "";

        // Primary OC Timings

        public string OcCL { get; set; } = "";

        public string OcTRCD { get; set; } = "";

        public string OcTRP { get; set; } = "";

        public string OcTRAS { get; set; } = "";

        public string OcTRC { get; set; } = "";

        public string OcCommandRate { get; set; } = "";

        // Advanced OC Timings

        public string OcTRFC { get; set; } = "";

        public string OcTREFI { get; set; } = "";

        public string OcTFAW { get; set; } = "";

        public string OcTRRDS { get; set; } = "";

        public string OcTRRDL { get; set; } = "";

        public string OcTWTRS { get; set; } = "";

        public string OcTWTRL { get; set; } = "";

        public string OcTCWL { get; set; } = "";

        // ================= PURCHASE =================

        public DateTime? PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }
    }
}