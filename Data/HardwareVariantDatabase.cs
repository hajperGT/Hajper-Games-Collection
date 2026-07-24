using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareVariantDatabase
    {
        public static List<HardwareVariant> GetVariants()
        {
            return new()
            {
                // Sony - PlayStation 3 Slim / Super Slim
                // A/B/C najczęściej oznacza wariant pojemności dysku.

                V("Sony", "PlayStation 3", "CECH-20xx", "A", "120 GB"),
                V("Sony", "PlayStation 3", "CECH-20xx", "B", "250 GB"),

                V("Sony", "PlayStation 3", "CECH-21xx", "A", "120 GB"),
                V("Sony", "PlayStation 3", "CECH-21xx", "B", "250 GB"),

                V("Sony", "PlayStation 3", "CECH-25xx", "A", "160 GB"),
                V("Sony", "PlayStation 3", "CECH-25xx", "B", "320 GB"),

                V("Sony", "PlayStation 3", "CECH-30xx", "A", "160 GB"),
                V("Sony", "PlayStation 3", "CECH-30xx", "B", "320 GB"),

                V("Sony", "PlayStation 3", "CECH-40xx", "A", "12 GB Flash"),
                V("Sony", "PlayStation 3", "CECH-40xx", "B", "250 GB"),
                V("Sony", "PlayStation 3", "CECH-40xx", "C", "500 GB"),

                V("Sony", "PlayStation 3", "CECH-42xx", "A", "12 GB Flash"),
                V("Sony", "PlayStation 3", "CECH-42xx", "B", "250 GB"),
                V("Sony", "PlayStation 3", "CECH-42xx", "C", "500 GB"),

                V("Sony", "PlayStation 3", "CECH-43xx", "A", "12 GB Flash"),
                V("Sony", "PlayStation 3", "CECH-43xx", "B", "250 GB"),
                V("Sony", "PlayStation 3", "CECH-43xx", "C", "500 GB")
            };
        }

        private static HardwareVariant V(
            string manufacturer,
            string platform,
            string modelCode,
            string variantCode,
            string variantName)
        {
            return new HardwareVariant
            {
                Manufacturer = manufacturer,
                Platform = platform,
                ModelCode = modelCode,
                VariantCode = variantCode,
                VariantName = variantName
            };
        }
    }
}