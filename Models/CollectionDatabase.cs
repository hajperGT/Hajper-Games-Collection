using System.Collections.Generic;

namespace HGC.Models
{
    internal class CollectionDatabase
    {
        public List<Hardware> HardwareItems { get; set; } = new();

        public List<Game> Games { get; set; } = new();

        public List<Hardware> ArchivedHardwareItems { get; set; } = new();

        public List<Game> ArchivedGames { get; set; } = new();

        public List<PcSystem> PcSystems { get; set; } = new();

        public List<PcSystem> ArchivedPcSystems { get; set; } = new();

        public List<Accessory> Accessories { get; set; } = new();

        public List<Accessory> ArchivedAccessories { get; set; } = new();

        public List<CurrencyRate> CurrencyRates { get; set; } = new();
    }
}