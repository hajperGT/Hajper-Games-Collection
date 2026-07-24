using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class MicrosoftHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // =========================
                // MICROSOFT - XBOX
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox",
                    DisplayName = "Xbox",
                    ModelCode = "Original Xbox",
                    Revision = "",
                    Region = "Multiple"
                },

                // =========================
                // MICROSOFT - XBOX 360
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 Core",
                    ModelCode = "Core",
                    Revision = "Fat",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 Premium / Pro",
                    ModelCode = "Premium / Pro",
                    Revision = "Fat",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 Arcade",
                    ModelCode = "Arcade",
                    Revision = "Fat",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 Elite",
                    ModelCode = "Elite",
                    Revision = "Fat",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 S",
                    ModelCode = "Xbox 360 S",
                    Revision = "Slim",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Xbox 360 E",
                    ModelCode = "Xbox 360 E",
                    Revision = "E",
                    Region = "Multiple"
                },

                // =========================
                // MICROSOFT - XBOX ONE
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox One",
                    DisplayName = "Xbox One",
                    ModelCode = "1540",
                    Revision = "",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox One S",
                    DisplayName = "Xbox One S",
                    ModelCode = "1681",
                    Revision = "S",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox One X",
                    DisplayName = "Xbox One X",
                    ModelCode = "1787",
                    Revision = "X",
                    Region = "Multiple"
                },

                // =========================
                // MICROSOFT - XBOX SERIES
                // =========================

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series S",
                    DisplayName = "Xbox Series S 512 GB",
                    ModelCode = "1881",
                    Revision = "Series S",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series X",
                    DisplayName = "Xbox Series X 1 TB",
                    ModelCode = "1882",
                    Revision = "Series X",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series S",
                    DisplayName = "Xbox Series S 1 TB",
                    ModelCode = "1883",
                    Revision = "Series S",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series X",
                    DisplayName = "Xbox Series X Digital 1 TB",
                    ModelCode = "Series X Digital",
                    Revision = "Series X",
                    Region = "Multiple"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series X",
                    DisplayName = "Xbox Series X 2 TB Galaxy Black",
                    ModelCode = "Series X 2 TB",
                    Revision = "Series X",
                    Region = "Multiple"
                },

                // Custom

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox 360",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox One",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                },

                new HardwareModel
                {
                    Manufacturer = "Microsoft",
                    Family = "Xbox",
                    Platform = "Xbox Series X",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                }
            };
        }
    }
}