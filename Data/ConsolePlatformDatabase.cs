using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class ConsolePlatformDatabase
    {
        public static List<Manufacturer> GetManufacturers()
        {
            return new()
            {
                new Manufacturer { Name = "Sony" },
                new Manufacturer { Name = "Microsoft" },
                new Manufacturer { Name = "Nintendo" },
                new Manufacturer { Name = "SEGA" },
                new Manufacturer { Name = "Atari" },
                new Manufacturer { Name = "SNK" },
                new Manufacturer { Name = "NEC" },
                new Manufacturer { Name = "Panasonic" },
                new Manufacturer { Name = "Philips" },
                new Manufacturer { Name = "Commodore" },
                new Manufacturer { Name = "Amiga (Commodore)" },

                new Manufacturer { Name = "Bandai" },
                new Manufacturer { Name = "Apple" },
                new Manufacturer { Name = "Coleco" },
                new Manufacturer { Name = "Mattel" },
                new Manufacturer { Name = "Magnavox" },
                new Manufacturer { Name = "Fairchild" },
                new Manufacturer { Name = "RCA" },
                new Manufacturer { Name = "Bally / Midway" },
                new Manufacturer { Name = "GCE / Vectrex" },

                new Manufacturer { Name = "Nokia" },
                new Manufacturer { Name = "Tiger" },
                new Manufacturer { Name = "Watara" },
                new Manufacturer { Name = "GamePark" },
                new Manufacturer { Name = "Zeebo" },
                new Manufacturer { Name = "Epoch" },
                new Manufacturer { Name = "Casio" },
                new Manufacturer { Name = "Fujitsu" },
                new Manufacturer { Name = "Interton" },
                new Manufacturer { Name = "APF" },
                new Manufacturer { Name = "Emerson" },

                new Manufacturer { Name = "Evercade" },
                new Manufacturer { Name = "Analogue" },
                new Manufacturer { Name = "OUYA" },
                new Manufacturer { Name = "NVIDIA" },

                new Manufacturer { Name = "Sinclair" },               
                new Manufacturer { Name = "Developer Hardware" },
                new Manufacturer { Name = "Other" }
            };
        }

        public static List<PlatformFamily> GetFamilies()
        {
            return new()
            {
                // Sony
                new PlatformFamily { Name = "PlayStation", Manufacturer = "Sony", BackgroundGroup = "Sony" },
                new PlatformFamily { Name = "PSP", Manufacturer = "Sony", BackgroundGroup = "Sony" },
                new PlatformFamily { Name = "PlayStation Vita", Manufacturer = "Sony", BackgroundGroup = "Sony" },

                // Microsoft
                new PlatformFamily { Name = "Xbox", Manufacturer = "Microsoft", BackgroundGroup = "Microsoft" },

                // Nintendo
                new PlatformFamily { Name = "Home Consoles", Manufacturer = "Nintendo", BackgroundGroup = "Nintendo" },
                new PlatformFamily { Name = "Handhelds", Manufacturer = "Nintendo", BackgroundGroup = "Nintendo" },

                // SEGA
                new PlatformFamily { Name = "SEGA Consoles", Manufacturer = "SEGA", BackgroundGroup = "SEGA" },
                new PlatformFamily { Name = "SEGA Handhelds", Manufacturer = "SEGA", BackgroundGroup = "SEGA" },
                //new PlatformFamily { Name = "SEGA Add-ons", Manufacturer = "SEGA", BackgroundGroup = "SEGA" },
                new PlatformFamily { Name = "SEGA Computers", Manufacturer = "SEGA", BackgroundGroup = "SEGA" },
                new PlatformFamily { Name = "LaserActive", Manufacturer = "SEGA", BackgroundGroup = "SEGA" },

                // Atari
                new PlatformFamily { Name = "Atari Consoles", Manufacturer = "Atari", BackgroundGroup = "Atari" },
                new PlatformFamily { Name = "Atari Handhelds", Manufacturer = "Atari", BackgroundGroup = "Atari" },
                new PlatformFamily { Name = "Modern Atari", Manufacturer = "Atari", BackgroundGroup = "Atari" },

                // SNK / NEC
                new PlatformFamily { Name = "Neo Geo", Manufacturer = "SNK", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Neo Geo Pocket", Manufacturer = "SNK", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "PC Engine", Manufacturer = "NEC", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "TurboGrafx", Manufacturer = "NEC", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "PC Engine Duo", Manufacturer = "NEC", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "PC Engine CD", Manufacturer = "NEC", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "PC-FX", Manufacturer = "NEC", BackgroundGroup = "Default" },

                // 3DO / CD-i
                new PlatformFamily { Name = "3DO", Manufacturer = "Panasonic", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "CD-i", Manufacturer = "Philips", BackgroundGroup = "Default" },

                // Commodore / Amiga
                new PlatformFamily { Name = "Commodore", Manufacturer = "Commodore", BackgroundGroup = "Commodore" },
                new PlatformFamily { Name = "Amiga", Manufacturer = "Amiga (Commodore)", BackgroundGroup = "Amiga" },

                // Other known manufacturers
                new PlatformFamily { Name = "WonderSwan", Manufacturer = "Bandai", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Playdia", Manufacturer = "Bandai", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Pippin", Manufacturer = "Apple", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Coleco", Manufacturer = "Coleco", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Intellivision", Manufacturer = "Mattel", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Odyssey", Manufacturer = "Magnavox", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Channel F", Manufacturer = "Fairchild", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Studio II", Manufacturer = "RCA", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Astrocade", Manufacturer = "Bally / Midway", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Vectrex", Manufacturer = "GCE / Vectrex", BackgroundGroup = "Default" },

                new PlatformFamily { Name = "N-Gage", Manufacturer = "Nokia", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Game.com", Manufacturer = "Tiger", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Supervision", Manufacturer = "Watara", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "GP32", Manufacturer = "GamePark", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "GP2X", Manufacturer = "GamePark", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Zeebo", Manufacturer = "Zeebo", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Cassette Vision", Manufacturer = "Epoch", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Loopy", Manufacturer = "Casio", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "FM Towns", Manufacturer = "Fujitsu", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "VC4000", Manufacturer = "Interton", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "APF", Manufacturer = "APF", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Arcadia", Manufacturer = "Emerson", BackgroundGroup = "Default" },

                new PlatformFamily { Name = "Evercade", Manufacturer = "Evercade", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Analogue", Manufacturer = "Analogue", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "OUYA", Manufacturer = "OUYA", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "NVIDIA Shield", Manufacturer = "NVIDIA", BackgroundGroup = "Default" },

                new PlatformFamily { Name = "ZX Series", Manufacturer = "Sinclair", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "PC", Manufacturer = "PC", BackgroundGroup = "PC" },
                new PlatformFamily { Name = "Developer Hardware", Manufacturer = "Developer Hardware", BackgroundGroup = "Default" },
                new PlatformFamily { Name = "Other", Manufacturer = "Other", BackgroundGroup = "Default" }
            };
        }

        public static List<Platform> GetPlatforms()
        {
            return new()
            {
                // =========================
                // SONY
                // =========================

                P("PlayStation", "PlayStation", "Sony", "Sony"),
                P("PS One", "PlayStation", "Sony", "Sony"),
                P("PlayStation 2", "PlayStation", "Sony", "Sony"),
                P("PlayStation 3", "PlayStation", "Sony", "Sony"),
                P("PlayStation 4", "PlayStation", "Sony", "Sony"),
                P("PlayStation 4 Slim", "PlayStation", "Sony", "Sony"),
                P("PlayStation 4 Pro", "PlayStation", "Sony", "Sony"),
                P("PlayStation 5", "PlayStation", "Sony", "Sony"),
                P("PlayStation 5 Digital", "PlayStation", "Sony", "Sony"),
                P("PlayStation 5 Slim", "PlayStation", "Sony", "Sony"),
                P("PlayStation 5 Pro", "PlayStation", "Sony", "Sony"),

                P("PSP", "PSP", "Sony", "Sony"),
                P("PSP Go", "PSP", "Sony", "Sony"),
                P("PSP Street", "PSP", "Sony", "Sony"),

                P("PlayStation Vita", "PlayStation Vita", "Sony", "Sony"),
                P("PlayStation TV", "PlayStation Vita", "Sony", "Sony"),

                // =========================
                // MICROSOFT
                // =========================

                P("Xbox", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox 360", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox One", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox One S", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox One X", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox Series S", "Xbox", "Microsoft", "Microsoft"),
                P("Xbox Series X", "Xbox", "Microsoft", "Microsoft"),

                // =========================
                // NINTENDO - HOME
                // =========================

                P("Color TV-Game", "Home Consoles", "Nintendo", "Nintendo"),
                P("Famicom", "Home Consoles", "Nintendo", "Nintendo"),
                P("NES", "Home Consoles", "Nintendo", "Nintendo"),
                P("Super Famicom", "Home Consoles", "Nintendo", "Nintendo"),
                P("SNES", "Home Consoles", "Nintendo", "Nintendo"),
                P("Virtual Boy", "Home Consoles", "Nintendo", "Nintendo"),
                P("Nintendo 64", "Home Consoles", "Nintendo", "Nintendo"),
                P("GameCube", "Home Consoles", "Nintendo", "Nintendo"),
                P("Panasonic Q", "Home Consoles", "Nintendo", "Nintendo"),
                P("Wii", "Home Consoles", "Nintendo", "Nintendo"),
                P("Wii Mini", "Home Consoles", "Nintendo", "Nintendo"),
                P("Wii U", "Home Consoles", "Nintendo", "Nintendo"),
                P("Nintendo Switch", "Home Consoles", "Nintendo", "Nintendo"),
                P("Nintendo Switch Lite", "Home Consoles", "Nintendo", "Nintendo"),
                P("Nintendo Switch OLED", "Home Consoles", "Nintendo", "Nintendo"),
                P("Nintendo Switch 2", "Home Consoles", "Nintendo", "Nintendo"),

                // =========================
                // NINTENDO - HANDHELDS
                // =========================

                P("Game Boy", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Pocket", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Light", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Color", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Advance", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Advance SP", "Handhelds", "Nintendo", "Nintendo"),
                P("Game Boy Micro", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo DS", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo DS Lite", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo DSi", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo DSi XL", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo 3DS", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo 3DS XL", "Handhelds", "Nintendo", "Nintendo"),
                P("New Nintendo 3DS", "Handhelds", "Nintendo", "Nintendo"),
                P("New Nintendo 3DS XL", "Handhelds", "Nintendo", "Nintendo"),
                P("Nintendo 2DS", "Handhelds", "Nintendo", "Nintendo"),
                P("New Nintendo 2DS XL", "Handhelds", "Nintendo", "Nintendo"),

                // =========================
                // SEGA
                // =========================

                P("SG-1000", "SEGA Consoles", "SEGA", "SEGA"),
                P("SG-1000 II", "SEGA Consoles", "SEGA", "SEGA"),
                P("SC-3000", "SEGA Computers", "SEGA", "SEGA"),
                P("SC-3000H", "SEGA Computers", "SEGA", "SEGA"),
                P("SEGA Mark III", "SEGA Consoles", "SEGA", "SEGA"),
                P("Master System", "SEGA Consoles", "SEGA", "SEGA"),
                P("Master System II", "SEGA Consoles", "SEGA", "SEGA"),
                P("Mega Drive", "SEGA Consoles", "SEGA", "SEGA"),
                P("Genesis", "SEGA Consoles", "SEGA", "SEGA"),
                P("Mega Jet", "SEGA Consoles", "SEGA", "SEGA"),
                P("Multi-Mega", "SEGA Consoles", "SEGA", "SEGA"),
                P("Genesis CDX", "SEGA Consoles", "SEGA", "SEGA"),
                P("Wondermega", "SEGA Consoles", "SEGA", "SEGA"),
                P("X'Eye", "SEGA Consoles", "SEGA", "SEGA"),
                P("TeraDrive", "SEGA Computers", "SEGA", "SEGA"),
                P("Saturn", "SEGA Consoles", "SEGA", "SEGA"),
                P("Dreamcast", "SEGA Consoles", "SEGA", "SEGA"),
                P("Pico", "SEGA Consoles", "SEGA", "SEGA"),
                P("Advanced Pico Beena", "SEGA Consoles", "SEGA", "SEGA"),

                //P("Mega-CD", "SEGA Add-ons", "SEGA", "SEGA"),
                //P("Sega CD", "SEGA Add-ons", "SEGA", "SEGA"),
                //P("32X", "SEGA Add-ons", "SEGA", "SEGA"),

                P("Pioneer LaserActive", "LaserActive", "SEGA", "SEGA"),
                P("Mega-LD PAC-S1", "LaserActive", "SEGA", "SEGA"),
                P("Sega PAC-S10", "LaserActive", "SEGA", "SEGA"),

                P("Game Gear", "SEGA Handhelds", "SEGA", "SEGA"),
                P("Nomad", "SEGA Handhelds", "SEGA", "SEGA"),

                // =========================
                // ATARI
                // =========================

                P("Atari 2600", "Atari Consoles", "Atari", "Atari"),
                P("Atari 2800", "Atari Consoles", "Atari", "Atari"),
                P("Atari 5200", "Atari Consoles", "Atari", "Atari"),
                P("Atari 7800", "Atari Consoles", "Atari", "Atari"),
                P("Atari XEGS", "Atari Consoles", "Atari", "Atari"),
                P("Atari Jaguar", "Atari Consoles", "Atari", "Atari"),
                //P("Jaguar CD", "Atari Consoles", "Atari", "Atari"),

                P("Atari Lynx", "Atari Handhelds", "Atari", "Atari"),

                P("Atari Flashback", "Modern Atari", "Atari", "Atari"),
                P("Atari VCS", "Modern Atari", "Atari", "Atari"),
                P("Atari 2600+", "Modern Atari", "Atari", "Atari"),
                P("Atari 7800+", "Modern Atari", "Atari", "Atari"),

                // =========================
                // SNK
                // =========================

                P("Neo Geo AES", "Neo Geo", "SNK", "Default"),
                P("Neo Geo MVS", "Neo Geo", "SNK", "Default"),
                P("Neo Geo CD", "Neo Geo", "SNK", "Default"),
                P("Neo Geo CDZ", "Neo Geo", "SNK", "Default"),
                P("Hyper Neo Geo 64", "Neo Geo", "SNK", "Default"),
                P("Neo Geo X Gold", "Neo Geo", "SNK", "Default"),
                P("Neo Geo Mini", "Neo Geo", "SNK", "Default"),
                P("Neo Geo Mini International", "Neo Geo", "SNK", "Default"),
                P("Neo Geo Mini Christmas Edition", "Neo Geo", "SNK", "Default"),
                P("Neo Geo Arcade Stick Pro", "Neo Geo", "SNK", "Default"),

                P("Neo Geo Pocket", "Neo Geo Pocket", "SNK", "Default"),
                P("Neo Geo Pocket Color", "Neo Geo Pocket", "SNK", "Default"),

                // =========================
                // NEC
                // =========================

                P("PC Engine", "PC Engine", "NEC", "Default"),
                P("PC Engine Shuttle", "PC Engine", "NEC", "Default"),
                P("PC Engine CoreGrafx", "PC Engine", "NEC", "Default"),
                P("PC Engine CoreGrafx II", "PC Engine", "NEC", "Default"),
                P("PC Engine SuperGrafx", "PC Engine", "NEC", "Default"),
                P("PC Engine LT", "PC Engine", "NEC", "Default"),
                P("PC Engine GT", "PC Engine", "NEC", "Default"),

                P("TurboGrafx-16", "TurboGrafx", "NEC", "Default"),
                P("TurboExpress", "TurboGrafx", "NEC", "Default"),

                P("PC Engine Duo", "PC Engine Duo", "NEC", "Default"),
                P("PC Engine Duo-R", "PC Engine Duo", "NEC", "Default"),
                P("PC Engine Duo-RX", "PC Engine Duo", "NEC", "Default"),
                P("TurboDuo", "PC Engine Duo", "NEC", "Default"),

                P("PC Engine CD-ROM²", "PC Engine CD", "NEC", "Default"),
                P("PC Engine Super CD-ROM²", "PC Engine CD", "NEC", "Default"),

                P("PC-FX", "PC-FX", "NEC", "Default"),
                P("PC-FXGA", "PC-FX", "NEC", "Default"),

                // =========================
                // 3DO / CD-i
                // =========================

                P("3DO", "3DO", "Panasonic", "Default"),

                P("CD-i", "CD-i", "Philips", "Default"),
                P("Philips CD-i 180", "CD-i", "Philips", "Default"),
                P("Philips CD-i 181", "CD-i", "Philips", "Default"),
                P("Philips CD-i 182", "CD-i", "Philips", "Default"),
                P("Philips CD-i 205", "CD-i", "Philips", "Default"),
                P("Philips CD-i 210", "CD-i", "Philips", "Default"),
                P("Philips CD-i 220", "CD-i", "Philips", "Default"),
                P("Philips CD-i 310", "CD-i", "Philips", "Default"),
                P("Philips CD-i 350", "CD-i", "Philips", "Default"),
                P("Philips CD-i 360", "CD-i", "Philips", "Default"),
                P("Philips CD-i 370", "CD-i", "Philips", "Default"),
                P("Philips CD-i 450", "CD-i", "Philips", "Default"),
                P("Philips CD-i 470", "CD-i", "Philips", "Default"),
                P("Philips CD-i 490", "CD-i", "Philips", "Default"),
                P("Philips CD-i 550", "CD-i", "Philips", "Default"),
                P("Philips CD-i 601", "CD-i", "Philips", "Default"),
                P("Philips CD-i 602", "CD-i", "Philips", "Default"),
                P("Philips CD-i 604", "CD-i", "Philips", "Default"),
                P("Philips CD-i 605", "CD-i", "Philips", "Default"),
                P("Philips CD-i 615", "CD-i", "Philips", "Default"),
                P("Philips CD-i 660", "CD-i", "Philips", "Default"),
                P("Philips CD-i 670", "CD-i", "Philips", "Default"),
                P("Philips CD-i 740", "CD-i", "Philips", "Default"),
                P("Magnavox CD-i 910", "CD-i", "Philips", "Default"),
                P("Magnavox CD-i 450", "CD-i", "Philips", "Default"),
                P("Magnavox CD-i 550", "CD-i", "Philips", "Default"),
                P("GoldStar GDI-700", "CD-i", "Philips", "Default"),
                P("GoldStar GDI-1000", "CD-i", "Philips", "Default"),
                P("GoldStar GPI-1200M", "CD-i", "Philips", "Default"),
                P("Kyocera Pro 1000S", "CD-i", "Philips", "Default"),
                P("Sony Intelligent Discman CD-i", "CD-i", "Philips", "Default"),
                P("BeoCenter AV5", "CD-i", "Philips", "Default"),
                P("Philips FW380i", "CD-i", "Philips", "Default"),
                P("Philips 21TCDi30", "CD-i", "Philips", "Default"),
                P("Philips CD-i/PC 2.0", "CD-i", "Philips", "Default"),

                // =========================
                // COMMODORE
                // =========================

                P("PET 2001", "Commodore", "Commodore", "Commodore"),
                P("VIC-20", "Commodore", "Commodore", "Commodore"),
                P("Commodore MAX Machine", "Commodore", "Commodore", "Commodore"),
                P("Commodore 16", "Commodore", "Commodore", "Commodore"),
                P("Commodore Plus/4", "Commodore", "Commodore", "Commodore"),
                P("Commodore 64", "Commodore", "Commodore", "Commodore"),
                P("Commodore 64C", "Commodore", "Commodore", "Commodore"),
                P("Commodore SX-64", "Commodore", "Commodore", "Commodore"),
                P("Commodore 128", "Commodore", "Commodore", "Commodore"),
                P("Commodore 128D", "Commodore", "Commodore", "Commodore"),
                P("Commodore 65", "Commodore", "Commodore", "Commodore"),

                // =========================
                // AMIGA
                // =========================

                P("Amiga 1000", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 500", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 500 Plus", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 600", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 1200", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 1500", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 2000", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 2500", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 3000", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 3000T", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 3000UX", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 4000", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga 4000T", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga CDTV", "Amiga", "Amiga (Commodore)", "Amiga"),
                P("Amiga CD32", "Amiga", "Amiga (Commodore)", "Amiga"),
                //P("CD32 FMV Module", "Amiga", "Amiga (Commodore)", "Amiga"),

                // =========================
                // SINCLAIR
                // =========================

                P("ZX80", "ZX Series", "Sinclair", "Default"),
                P("ZX81", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum+", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum 128K", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum +2", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum +2A", "ZX Series", "Sinclair", "Default"),
                P("ZX Spectrum +3", "ZX Series", "Sinclair", "Default"),

                // =========================
                // BANDAI / APPLE
                // =========================

                P("WonderSwan", "WonderSwan", "Bandai", "Default"),
                P("WonderSwan Color", "WonderSwan", "Bandai", "Default"),
                P("SwanCrystal", "WonderSwan", "Bandai", "Default"),
                P("Playdia", "Playdia", "Bandai", "Default"),

                P("Apple Pippin", "Pippin", "Apple", "Default"),

                // =========================
                // OTHER RETRO
                // =========================

                P("Coleco Telstar", "Coleco", "Coleco", "Default"),
                P("ColecoVision", "Coleco", "Coleco", "Default"),
                P("Coleco Gemini", "Coleco", "Coleco", "Default"),
                P("Coleco Adam", "Coleco", "Coleco", "Default"),

                P("Intellivision", "Intellivision", "Mattel", "Default"),
                P("Intellivision II", "Intellivision", "Mattel", "Default"),
                P("Intellivision III Prototype", "Intellivision", "Mattel", "Default"),
                P("Intellivision Keyboard Component", "Intellivision", "Mattel", "Default"),

                P("Magnavox Odyssey", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey 100", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey 200", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey 300", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey 400", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey 500", "Odyssey", "Magnavox", "Default"),
                P("Magnavox Odyssey²", "Odyssey", "Magnavox", "Default"),

                P("Fairchild Channel F", "Channel F", "Fairchild", "Default"),
                P("Fairchild Channel F System II", "Channel F", "Fairchild", "Default"),

                P("RCA Studio II", "Studio II", "RCA", "Default"),
                P("Bally Professional Arcade", "Astrocade", "Bally / Midway", "Default"),
                P("Bally Astrocade", "Astrocade", "Bally / Midway", "Default"),
                P("Vectrex", "Vectrex", "GCE / Vectrex", "Default"),

                P("Nokia N-Gage", "N-Gage", "Nokia", "Default"),
                P("Nokia N-Gage QD", "N-Gage", "Nokia", "Default"),

                P("Tiger Game.com", "Game.com", "Tiger", "Default"),
                P("Tiger Game.com Pocket Pro", "Game.com", "Tiger", "Default"),

                P("Watara Supervision", "Supervision", "Watara", "Default"),

                P("GP32", "GP32", "GamePark", "Default"),
                P("GP2X", "GP2X", "GamePark", "Default"),
                P("GP2X Wiz", "GP2X", "GamePark", "Default"),
                P("Caanoo", "GP2X", "GamePark", "Default"),

                P("Zeebo", "Zeebo", "Zeebo", "Default"),

                P("Cassette Vision", "Cassette Vision", "Epoch", "Default"),
                P("Super Cassette Vision", "Cassette Vision", "Epoch", "Default"),

                P("Casio Loopy", "Loopy", "Casio", "Default"),

                P("FM Towns Marty", "FM Towns", "Fujitsu", "Default"),
                P("FM Towns Marty 2", "FM Towns", "Fujitsu", "Default"),

                P("Interton VC4000", "VC4000", "Interton", "Default"),

                P("APF-M1000", "APF", "APF", "Default"),
                P("APF Imagination Machine", "APF", "APF", "Default"),

                P("Emerson Arcadia 2001", "Arcadia", "Emerson", "Default"),

                // =========================
                // MODERN RETRO
                // =========================

                P("Evercade", "Evercade", "Evercade", "Default"),
                P("Evercade VS", "Evercade", "Evercade", "Default"),
                P("Analogue Pocket", "Analogue", "Analogue", "Default"),
                P("OUYA", "OUYA", "OUYA", "Default"),
                P("NVIDIA Shield", "NVIDIA Shield", "NVIDIA", "Default"),

                // =========================
                // DEVELOPER HARDWARE
                // =========================

                P("PlayStation Net Yaroze", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 2 TOOL", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 2 TEST", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 3 TEST", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 3 TOOL", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 4 TestKit", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 4 DevKit", "Developer Hardware", "Developer Hardware", "Default"),
                P("PlayStation 5 DevKit", "Developer Hardware", "Developer Hardware", "Default"),

                P("Xbox Debug Kit", "Developer Hardware", "Developer Hardware", "Default"),
                P("Xbox 360 XDK", "Developer Hardware", "Developer Hardware", "Default"),
                P("Xbox 360 Test Kit", "Developer Hardware", "Developer Hardware", "Default"),
                P("Xbox One Dev Kit", "Developer Hardware", "Developer Hardware", "Default"),
                P("Xbox Series X Dev Kit", "Developer Hardware", "Developer Hardware", "Default"),

                P("Nintendo GameCube NR Reader", "Developer Hardware", "Developer Hardware", "Default"),
                P("Nintendo Wii RVT-R Reader", "Developer Hardware", "Developer Hardware", "Default"),
                P("Nintendo Wii RVT-H Reader", "Developer Hardware", "Developer Hardware", "Default"),
                P("Nintendo Wii U CAT-DEV", "Developer Hardware", "Developer Hardware", "Default"),
                P("Nintendo Switch DevKit", "Developer Hardware", "Developer Hardware", "Default"),

                P("SEGA Katana Dev Box", "Developer Hardware", "Developer Hardware", "Default"),

                // =========================
                // PC / OTHER
                // =========================

                P("PC", "PC", "PC", "PC"),
                P("Other", "Other", "Other", "Default")
            };
        }

        private static Platform P(string name, string family, string manufacturer, string backgroundGroup)
        {
            return new Platform
            {
                Name = name,
                Family = family,
                Manufacturer = manufacturer,
                BackgroundGroup = backgroundGroup
            };
        }
    }
}