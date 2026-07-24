using System;
using System.Collections.Generic;

namespace HGC.Models
{
    internal class Game
    {
        public List<CollectionPhoto> Photos { get; set; } = new();       
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public CollectionPhoto? FrontCoverPhoto { get; set; }
        public CollectionPhoto? BackCoverPhoto { get; set; }
        public string Title { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Family { get; set; } = "";
        public string Platform { get; set; } = "";
        public string OperatingSystem { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string ReleaseType { get; set; } = "";      
        public string DigitalPlatform { get; set; } = ""; 
        public string Region { get; set; } = "";
        public string MediaType { get; set; } = "";        
        public string BoxType { get; set; } = ""; 
        public string BoxCondition { get; set; } = "";
        public double? UserRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPlanned { get; set; }
        public string Genre { get; set; } = "";
        public string Condition { get; set; } = "";
        public bool HasBox { get; set; }
        public bool HasManual { get; set; }
        public bool HasAdvertisements { get; set; }
        public bool HasMap { get; set; }
        public bool HasPoster { get; set; }
        public bool HasSoundtrack { get; set; }
        public bool HasArtbook { get; set; }
        public bool HasFigure { get; set; }
        public bool HasCertificate { get; set; }
        public bool HasOtherExtras { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; } = "PLN";
        public List<string> PhotoPaths { get; set; } = new();
        public string CustomBackgroundPath { get; set; } = "";
        public bool CustomBackgroundFitToPage { get; set; } = true;
        public int CustomBackgroundOpacity { get; set; } = 30;
        public int CardOpacity { get; set; } = 100;
        public string Notes { get; set; } = "";
        public bool IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public string ArchiveReason { get; set; } = "";
    }
}