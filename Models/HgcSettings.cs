using System.Collections.Generic;

namespace HGC.Models
{
    public class HgcSettings
    {
        // Ogólne
        public string Language { get; set; } = "pl";
        public string Theme { get; set; } = "Default";

        // Przezroczystość programu
        public int WindowOpacity { get; set; } = 100;

        // Waluty
        public string DefaultCurrency { get; set; } = "PLN";
        public Dictionary<string, decimal> CurrencyRates { get; set; } = new();
               
        // Widok kolekcji 
        public bool ShowRecentlyAddedPanel { get; set; } = true;
        public string CollectionCategoryOrder { get; set; } = "Hardware,Accessories,Games";
        public bool ShowHardwareCategory { get; set; } = true;
        public bool ShowAccessoriesCategory { get; set; } = true;
        public bool ShowGamesCategory { get; set; } = true;
        public int CollectionCategoryLines { get; set; } = 1;
        public bool CollectionCategoryLinesAuto { get; set; } = true;
        public string CollectionTileSize { get; set; } = "Medium";
        public string CollectionSortMode { get; set; } = "AZ";
        public int CollectionItemsPerPage { get; set; } = 40;
        public string CollectionItemsPerPageMode { get; set; } = "Auto";

        // Wygląd kart
        public double GameCardOpacity { get; set; } = 1.0;
        public double HardwareCardOpacity { get; set; } = 1.0;
        public double PcCardOpacity { get; set; } = 1.0;
        public double AccessoryCardOpacity { get; set; } = 1.0;

        // Tło rekordów
        public bool UsePageBackground { get; set; } = true;
    }
}