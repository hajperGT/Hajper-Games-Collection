using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareRegionDatabase
    {
        public static List<HardwareRegion> GetRegions()
        {
            return new()
            {
                // Sony

                R("Sony", "PlayStation", "00", "Japan"),
                R("Sony", "PlayStation", "01", "USA"),
                R("Sony", "PlayStation", "02", "Australia"),
                R("Sony", "PlayStation", "03", "United Kingdom"),
                R("Sony", "PlayStation", "04", "Europe"),
                R("Sony", "PlayStation", "05", "Korea"),
                R("Sony", "PlayStation", "06", "Asia"),
                R("Sony", "PlayStation", "07", "Taiwan"),
                R("Sony", "PlayStation", "08", "Russia")
            };
        }

        private static HardwareRegion R(
            string manufacturer,
            string platform,
            string regionCode,
            string regionName)
        {
            return new HardwareRegion
            {
                Manufacturer = manufacturer,
                Platform = platform,
                RegionCode = regionCode,
                RegionName = regionName
            };
        }
    }
}