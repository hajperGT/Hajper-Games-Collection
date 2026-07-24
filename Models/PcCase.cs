using System;

namespace HGC.Models
{
    public class PcCase
    {
        // ================= BASIC =================

        public string Manufacturer { get; set; } = "";

        public string Model { get; set; } = "";

        // ================= TYPE =================

        public string CaseType { get; set; } = "";

        // ================= COLOR =================

        public string Color { get; set; } = "";

        // ================= FEATURES =================

        public bool HasSideWindow { get; set; }

        public bool HasTemperedGlass { get; set; }

        public bool HasRgb { get; set; }

        public bool HasVerticalGpuMount { get; set; }

        public bool HasHotSwap { get; set; }

        public bool HasSoundDamping { get; set; }

        // ================= COMPATIBILITY =================

        public string MaxMotherboardSize { get; set; } = "";

        public string MaxGpuLength { get; set; } = "";

        public string MaxCpuCoolerHeight { get; set; } = "";

        // ================= PURCHASE =================

        public DateTime? PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }
    }
}