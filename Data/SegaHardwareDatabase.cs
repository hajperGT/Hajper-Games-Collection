using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class SegaHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // =========================
                // SEGA - HOME CONSOLES
                // =========================

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "SG-1000", DisplayName = "SG-1000", ModelCode = "SG-1000", Revision = "Standard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "SG-1000 II", DisplayName = "SG-1000 II", ModelCode = "SG-1000 II", Revision = "II", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Master System", DisplayName = "Master System", ModelCode = "Master System", Revision = "Standard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Master System II", DisplayName = "Master System II", ModelCode = "Master System II", Revision = "II", Region = "Multiple" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "Mega Drive", ModelCode = "Mega Drive", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "Mega Drive", ModelCode = "Mega Drive", Revision = "Model 2", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "Mega Drive", ModelCode = "Mega Drive", Revision = "Model 3", Region = "Multiple" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "Genesis", ModelCode = "Genesis", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "Genesis", ModelCode = "Genesis", Revision = "Model 2", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "Genesis", ModelCode = "Genesis", Revision = "Model 3", Region = "Multiple" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Saturn", DisplayName = "SEGA Saturn", ModelCode = "Saturn", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Saturn", DisplayName = "SEGA Saturn", ModelCode = "Saturn", Revision = "Model 2", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Dreamcast", DisplayName = "Dreamcast", ModelCode = "Dreamcast", Revision = "Standard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Pico", DisplayName = "SEGA Pico", ModelCode = "Pico", Revision = "Standard", Region = "Multiple" },

                // =========================
                // SEGA - MEGA DRIVE / GENESIS ADD-ONS
                // =========================

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "Mega-CD", ModelCode = "Mega-CD", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "Mega-CD", ModelCode = "Mega-CD", Revision = "Model 2", Region = "Multiple" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "Sega CD", ModelCode = "Sega CD", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "Sega CD", ModelCode = "Sega CD", Revision = "Model 2", Region = "Multiple" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Drive", DisplayName = "SEGA 32X", ModelCode = "32X", Revision = "", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis", DisplayName = "SEGA 32X", ModelCode = "32X", Revision = "", Region = "Multiple" },

                // =========================
                // SEGA - HANDHELDS
                // =========================

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Handhelds", Platform = "Game Gear", DisplayName = "Game Gear", ModelCode = "Game Gear", Revision = "Standard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Handhelds", Platform = "Nomad", DisplayName = "SEGA Nomad", ModelCode = "Nomad", Revision = "Standard", Region = "Multiple" },

                // =========================
                // SEGA - COMPUTERS / EARLY HARDWARE
                // =========================

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Computers", Platform = "SC-3000", DisplayName = "SC-3000", ModelCode = "SC-3000", Revision = "Standard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Computers", Platform = "SC-3000H", DisplayName = "SC-3000H", ModelCode = "SC-3000H", Revision = "Improved Keyboard", Region = "Multiple" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "SEGA Mark III", DisplayName = "SEGA Mark III", ModelCode = "Mark III", Revision = "Japan", Region = "Japan" },

                // =========================
                // SEGA - MEGA DRIVE / GENESIS DERIVED HARDWARE
                // =========================

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Mega Jet", DisplayName = "Mega Jet", ModelCode = "Mega Jet", Revision = "Portable / Airline", Region = "Japan" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Multi-Mega", DisplayName = "Multi-Mega", ModelCode = "Multi-Mega", Revision = "Mega Drive + CD", Region = "Europe / Asia" },
                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Genesis CDX", DisplayName = "Genesis CDX", ModelCode = "CDX", Revision = "Genesis + CD", Region = "USA" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Consoles", Platform = "Wondermega", DisplayName = "Wondermega", ModelCode = "Wondermega", Revision = "Mega Drive + CD", Region = "Japan" },
                new HardwareModel { Manufacturer = "JVC", Family = "SEGA Consoles", Platform = "Wondermega", DisplayName = "JVC Wondermega", ModelCode = "RG-M1", Revision = "Mega Drive + CD", Region = "Japan" },
                new HardwareModel { Manufacturer = "JVC", Family = "SEGA Consoles", Platform = "X'Eye", DisplayName = "JVC X'Eye", ModelCode = "X'Eye", Revision = "Genesis + CD", Region = "USA" },

                new HardwareModel { Manufacturer = "SEGA", Family = "SEGA Computers", Platform = "TeraDrive", DisplayName = "SEGA TeraDrive", ModelCode = "TeraDrive", Revision = "Mega Drive + PC", Region = "Japan" },

                // =========================
                // SEGA - LASERACTIVE
                // =========================

                new HardwareModel { Manufacturer = "Pioneer", Family = "LaserActive", Platform = "LaserActive", DisplayName = "Pioneer LaserActive", ModelCode = "CLD-A100", Revision = "Base Unit", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Pioneer", Family = "LaserActive", Platform = "LaserActive Mega-LD", DisplayName = "Mega-LD PAC-S1", ModelCode = "PAC-S1", Revision = "Mega Drive Module", Region = "Japan" },
                new HardwareModel { Manufacturer = "Pioneer", Family = "LaserActive", Platform = "LaserActive Mega-LD", DisplayName = "Sega PAC-S10", ModelCode = "PAC-S10", Revision = "Genesis Module", Region = "USA" },

                // Custom

                new HardwareModel
                {
                    Manufacturer = "SEGA",
                    Family = "SEGA",
                    Platform = "SEGA",
                    DisplayName = "Other / Custom",
                    ModelCode = "Other",
                    Revision = "Other",
                    Region = "Other"
                }
            };
        }
    }
}