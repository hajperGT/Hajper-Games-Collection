using System;
using System.Collections.Generic;

namespace HGC.Models
{
    public class Accessory
    {
        public List<CollectionPhoto> Photos { get; set; } = new();
        public Guid Id { get; set; } = Guid.NewGuid();
        public int CardOpacity { get; set; } = 100;
        public string CustomBackgroundPath { get; set; } = "";
        public int CustomBackgroundOpacity { get; set; } = 30;
        public string AccessoryType { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";
        public string AssignedManufacturer { get; set; } = "";
        public string AssignedFamily { get; set; } = "";
        public string AssignedPlatform { get; set; } = "";
        public string ControllerManufacturer { get; set; } = "";
        public string ControllerModel { get; set; } = "";
        public string DisplaySize { get; set; } = "";
        public string DisplayResolution { get; set; } = "";
        public string DisplayType { get; set; } = "";
        public string DisplayPanelType { get; set; } = "";
        public string DisplayBacklightType { get; set; } = "";
        public string RefreshRate { get; set; } = "";
        public string AspectRatio { get; set; } = "";
        public string ResponseTime { get; set; } = "";
        public string SyncTechnology { get; set; } = "";
        public bool HdrSupport { get; set; }
        public bool SmartTv { get; set; }
        public string TvSystem { get; set; } = "";
        public bool TvTuner { get; set; }
        public string ProjectionTechnology { get; set; } = "";
        public string AnsiLumens { get; set; } = "";
        public string LampLife { get; set; } = "";
        public string Condition { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; } = "PLN";
        public string Notes { get; set; } = "";
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
    }
}