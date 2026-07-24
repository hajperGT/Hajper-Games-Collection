using System;
using System.Collections.Generic;

namespace HGC.Models
{
    internal class PcSystem
    {
        public List<CollectionPhoto> Photos { get; set; } = new();
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Name { get; set; } = "";
        public string ComputerType { get; set; } = "";
        public string PortableType { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public bool AddModelAndSerialForCustomBuild { get; set; }
        public string Model { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string OperatingSystem { get; set; } = "";
        public string OperatingSystemEdition { get; set; } = "";
        public string OperatingSystemRevision { get; set; } = "";
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; } = "PLN";
        public List<string> PhotoPaths { get; set; } = new();
        public int CardOpacity { get; set; } = 100;
        public string CustomBackgroundPath { get; set; } = "";
        public bool CustomBackgroundFitToPage { get; set; } = true;
        public int CustomBackgroundOpacity { get; set; } = 30;
        public string Notes { get; set; } = "";
        public List<ServiceEntry> ServiceHistory { get; set; } = new();
        public List<PcCpu> Cpus { get; set; } = new();
        public PcMotherboard? Motherboard { get; set; }
        public List<PcGpu> Gpus { get; set; } = new();
        public List<PcRam> RamModules { get; set; } = new();
        public List<PcStorage> StorageDevices { get; set; } = new();
        public List<PcPowerSupply> PowerSupplies { get; set; } = new();
        public PcCase? Case { get; set; }
        public PcCooling? Cooling { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public string ArchiveReason { get; set; } = "";
    }
}