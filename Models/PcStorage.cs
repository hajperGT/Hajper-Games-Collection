using System;

namespace HGC.Models
{
    public class PcStorage
    {
        // ================= BASIC =================

        public string Manufacturer { get; set; } = "";

        public string Model { get; set; } = "";

        public string Capacity { get; set; } = "";

        // ================= TYPE =================

        public string DriveType { get; set; } = "";

        // HDD
        // SSD
        // SSHD
        // CF
        // SD
        // MicroSD
        // ZIP
        // MO
        // Other

        // ================= CONNECTION =================

        public string Interface { get; set; } = "";

        // IDE
        // EIDE
        // SATA
        // mSATA
        // M.2 SATA
        // M.2 NVMe
        // U.2
        // SCSI
        // SAS
        // USB
        // FireWire
        // PCI
        // PCI-E
        // AGP
        // Other

        public string FormFactor { get; set; } = "";

        // 1.8
        // 2.5
        // 3.5
        // 2230
        // 2242
        // 2260
        // 2280
        // 22110
        // CF
        // SD
        // MicroSD

        // ================= FLAGS =================

        public bool IsBootDrive { get; set; }

        // ================= HDD =================

        public string Rpm { get; set; } = "";

        public string Cache { get; set; } = "";

        // ================= SSD =================

        public string Controller { get; set; } = "";

        public string NandType { get; set; } = "";

        public bool HasDramCache { get; set; }

        // ================= PURCHASE =================

        public DateTime? PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }
    }
}