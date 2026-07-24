using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class DeveloperHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // SONY

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation", DisplayName = "PlayStation Net Yaroze", ModelCode = "DTL-H3000", Revision = "Development", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 2", DisplayName = "PlayStation 2 TOOL", ModelCode = "DTL-T10000", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 2", DisplayName = "PlayStation 2 TEST", ModelCode = "DTL-H", Revision = "Test Kit", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 3", DisplayName = "PlayStation 3 TEST", ModelCode = "DECH", Revision = "Test Kit", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 3", DisplayName = "PlayStation 3 TOOL", ModelCode = "DECR", Revision = "Development", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 4", DisplayName = "PlayStation 4 TestKit", ModelCode = "DUH-T", Revision = "Test Kit", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 4", DisplayName = "PlayStation 4 DevKit", ModelCode = "DUH-D", Revision = "Development", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Sony", Platform = "PlayStation 5", DisplayName = "PlayStation 5 DevKit", ModelCode = "PS5 DevKit", Revision = "Development", Region = "Multiple" },

                // MICROSOFT

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Microsoft", Platform = "Xbox", DisplayName = "Xbox Debug Kit", ModelCode = "Debug Kit", Revision = "Development", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Microsoft", Platform = "Xbox 360", DisplayName = "Xbox 360 XDK", ModelCode = "XDK", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Microsoft", Platform = "Xbox 360", DisplayName = "Xbox 360 Test Kit", ModelCode = "Test Kit", Revision = "Test Kit", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Microsoft", Platform = "Xbox One", DisplayName = "Xbox One Dev Kit", ModelCode = "Dev Kit", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Microsoft", Platform = "Xbox Series X", DisplayName = "Xbox Series X Dev Kit", ModelCode = "Project Scarlett Dev Kit", Revision = "Development", Region = "Multiple" },

                // NINTENDO

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Nintendo", Platform = "GameCube", DisplayName = "Nintendo GameCube NR Reader", ModelCode = "NR Reader", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Nintendo", Platform = "Wii", DisplayName = "Nintendo Wii RVT-R Reader", ModelCode = "RVT-R", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Nintendo", Platform = "Wii", DisplayName = "Nintendo Wii RVT-H Reader", ModelCode = "RVT-H", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Nintendo", Platform = "Wii U", DisplayName = "Nintendo Wii U CAT-DEV", ModelCode = "CAT-DEV", Revision = "Development", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Nintendo", Platform = "Nintendo Switch", DisplayName = "Nintendo Switch DevKit", ModelCode = "SDEV", Revision = "Development", Region = "Multiple" },

                // SEGA

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "SEGA", Platform = "Dreamcast", DisplayName = "SEGA Katana Dev Box", ModelCode = "Katana", Revision = "Development", Region = "Multiple" },

                // Custom

                new HardwareModel { Manufacturer = "Developer Hardware", Family = "Developer Hardware", Platform = "Developer Hardware", DisplayName = "Other / Custom", ModelCode = "Other", Revision = "Other", Region = "Other" }
            };
        }
    }
}