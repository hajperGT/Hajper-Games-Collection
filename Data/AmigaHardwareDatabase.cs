using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class AmigaHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // =========================
                // AMIGA
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 1000",
                    DisplayName = "Amiga 1000",
                    ModelCode = "A1000",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 1500",
                    DisplayName = "Amiga 1500",
                    ModelCode = "A1500",
                    Revision = "UK Variant",
                    Region = "Europe"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 3000UX",
                    DisplayName = "Amiga 3000UX",
                    ModelCode = "A3000UX",
                    Revision = "Unix",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 500",
                    DisplayName = "Amiga 500",
                    ModelCode = "A500",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 500 Plus",
                    DisplayName = "Amiga 500 Plus",
                    ModelCode = "A500+",
                    Revision = "Plus",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 600",
                    DisplayName = "Amiga 600",
                    ModelCode = "A600",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 1200",
                    DisplayName = "Amiga 1200",
                    ModelCode = "A1200",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 2000",
                    DisplayName = "Amiga 2000",
                    ModelCode = "A2000",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 2500",
                    DisplayName = "Amiga 2500",
                    ModelCode = "A2500",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 3000",
                    DisplayName = "Amiga 3000",
                    ModelCode = "A3000",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 3000T",
                    DisplayName = "Amiga 3000 Tower",
                    ModelCode = "A3000T",
                    Revision = "Tower",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 4000",
                    DisplayName = "Amiga 4000",
                    ModelCode = "A4000",
                    Revision = "Desktop",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Computers",
                    Platform = "Amiga 4000T",
                    DisplayName = "Amiga 4000 Tower",
                    ModelCode = "A4000T",
                    Revision = "Tower",
                    Region = "Multiple"
                },

                // =========================
                // AMIGA MULTIMEDIA
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Multimedia",
                    Platform = "Amiga CDTV",
                    DisplayName = "Amiga CDTV",
                    ModelCode = "CDTV",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Multimedia",
                    Platform = "Amiga CD32",
                    DisplayName = "Amiga CD32",
                    ModelCode = "CD32",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga Multimedia",
                    Platform = "Amiga CD32",
                    DisplayName = "CD32 Full Motion Video Module",
                    ModelCode = "FMV",
                    Revision = "",
                    Region = "Multiple"
                },

                // Custom

                new HardwareModel
                {
                    Manufacturer = "Amiga",
                    Family = "Amiga",
                    Platform = "Amiga",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                }
            };
        }
    }
}