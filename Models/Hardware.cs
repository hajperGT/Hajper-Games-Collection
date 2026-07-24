using System;
using System.Collections.Generic;

namespace HGC.Models
{
    internal class Hardware
    {
        public List<CollectionPhoto> Photos { get; set; } = new();
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Manufacturer { get; set; } = "";
        public string Family { get; set; } = "";
        public string Platform { get; set; } = "";
        public string CustomName { get; set; } = "";
        public string HardwareType { get; set; } = "";
        public string Model { get; set; } = "";
        public string Variant { get; set; } = "";
        public string Revision { get; set; } = "";
        public string BoardRevision { get; set; } = "";
        public string Region { get; set; } = "";
        public string Color { get; set; } = "";
        public string SpecialEdition { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Condition { get; set; } = "";
        public bool HasBox { get; set; }
        public string BoxCondition { get; set; } = "";
        public bool HasManual { get; set; }
        public bool HasInserts { get; set; }
        public bool HasPowerSupply { get; set; }
        public bool HasCable { get; set; }
        public bool HasController { get; set; }
        public bool HasMemoryCard { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; } = "PLN";
        public List<string> PhotoPaths { get; set; } = new();
        public string CustomBackgroundPath { get; set; } = "";
        public bool CustomBackgroundFitToPage { get; set; } = true;
        public int CustomBackgroundOpacity { get; set; } = 30;
        public int CardOpacity { get; set; } = 100;
        public string Notes { get; set; } = "";
        public List<ServiceEntry> ServiceHistory { get; set; } = new();
        public bool IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public string ArchiveReason { get; set; } = "";
    }
}