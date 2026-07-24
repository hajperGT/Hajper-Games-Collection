using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class AtariHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                // =========================
                // ATARI - HOME CONSOLES
                // =========================

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Atari VCS / 2600 Heavy Sixer", ModelCode = "CX2600", Revision = "Heavy Sixer", Region = "USA" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Atari 2600 Light Sixer", ModelCode = "CX2600", Revision = "Light Sixer", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Atari 2600 4-Switch Woody", ModelCode = "CX2600A", Revision = "4-Switch Woody", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Atari 2600 Vader", ModelCode = "CX2600A", Revision = "Vader", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Atari 2600 Jr.", ModelCode = "2600 Jr.", Revision = "Jr.", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Sears Tele-Games Video Arcade", ModelCode = "Sears VCS", Revision = "Sears Variant", Region = "USA" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 2600", DisplayName = "Sears Tele-Games Video Arcade II", ModelCode = "Sears Arcade II", Revision = "Sears Variant", Region = "USA" },

                new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Atari Consoles",
    Platform = "Atari 2800",
    DisplayName = "Atari 2800",
    ModelCode = "CX-2800",
    Revision = "Japan",
    Region = "Japan"
},
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 5200", DisplayName = "Atari 5200 4-Port", ModelCode = "5200-4", Revision = "4 Controller Ports", Region = "USA" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 5200", DisplayName = "Atari 5200 2-Port", ModelCode = "5200-2", Revision = "2 Controller Ports", Region = "USA" },

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari 7800", DisplayName = "Atari 7800 ProSystem", ModelCode = "Atari 7800", Revision = "ProSystem", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari XEGS", DisplayName = "Atari XE Game System", ModelCode = "XEGS", Revision = "Standard", Region = "Multiple" },

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari Jaguar", DisplayName = "Atari Jaguar", ModelCode = "Jaguar", Revision = "", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Consoles", Platform = "Atari Jaguar", DisplayName = "Jaguar CD", ModelCode = "Jaguar CD", Revision = "", Region = "Multiple" },

                // =========================
                // ATARI - HANDHELDS
                // =========================

                new HardwareModel { Manufacturer = "Atari", Family = "Atari Handhelds", Platform = "Atari Lynx", DisplayName = "Atari Lynx", ModelCode = "Lynx I", Revision = "Model 1", Region = "Multiple" },
                new HardwareModel { Manufacturer = "Atari", Family = "Atari Handhelds", Platform = "Atari Lynx", DisplayName = "Atari Lynx II", ModelCode = "PAG3201", Revision = "Model 2", Region = "Multiple" },

// =========================
// MODERN ATARI
// =========================

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback",
    ModelCode = "Flashback",
    Revision = "1",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 2",
    ModelCode = "Flashback 2",
    Revision = "2",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 3",
    ModelCode = "Flashback 3",
    Revision = "3",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 4",
    ModelCode = "Flashback 4",
    Revision = "4",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 5",
    ModelCode = "Flashback 5",
    Revision = "5",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 6",
    ModelCode = "Flashback 6",
    Revision = "6",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 7",
    ModelCode = "Flashback 7",
    Revision = "7",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 8",
    ModelCode = "Flashback 8",
    Revision = "8",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback 9",
    ModelCode = "Flashback 9",
    Revision = "9",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari Flashback",
    DisplayName = "Atari Flashback X",
    ModelCode = "Flashback X",
    Revision = "10",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari VCS",
    DisplayName = "Atari VCS 800",
    ModelCode = "VCS 800",
    Revision = "Modern",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari VCS",
    DisplayName = "Atari VCS 400",
    ModelCode = "VCS 400",
    Revision = "Modern",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari 2600+",
    DisplayName = "Atari 2600+",
    ModelCode = "2600+",
    Revision = "Modern Reissue",
    Region = "Multiple"
},

new HardwareModel
{
    Manufacturer = "Atari",
    Family = "Modern Atari",
    Platform = "Atari 7800+",
    DisplayName = "Atari 7800+",
    ModelCode = "7800+",
    Revision = "Modern Reissue",
    Region = "Multiple"
},


                // Custom

                new HardwareModel { Manufacturer = "Atari", Family = "Atari", Platform = "Atari", DisplayName = "Other / Custom", ModelCode = "Other", Revision = "Other", Region = "Other" },


            };
        }
    }
}