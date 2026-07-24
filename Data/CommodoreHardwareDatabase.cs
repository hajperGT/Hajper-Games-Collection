using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class CommodoreHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // =========================
                // COMMODORE
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore 64",
                    DisplayName = "Commodore 64",
                    ModelCode = "C64",
                    Revision = "Breadbin",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore 64C",
                    DisplayName = "Commodore 64C",
                    ModelCode = "C64C",
                    Revision = "C",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore 128",
                    DisplayName = "Commodore 128",
                    ModelCode = "C128",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore 128D",
                    DisplayName = "Commodore 128D",
                    ModelCode = "C128D",
                    Revision = "Desktop",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "VIC-20",
                    DisplayName = "VIC-20",
                    ModelCode = "VIC-20",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore 16",
                    DisplayName = "Commodore 16",
                    ModelCode = "C16",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore Computers",
                    Platform = "Commodore Plus/4",
                    DisplayName = "Commodore Plus/4",
                    ModelCode = "Plus/4",
                    Revision = "Standard",
                    Region = "Multiple"
                },

                // Custom

                new HardwareModel
                {
                    Manufacturer = "Commodore",
                    Family = "Commodore",
                    Platform = "Commodore",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                }
            };
        }
    }
}