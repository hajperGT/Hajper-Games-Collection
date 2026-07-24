using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareRevisionDatabase
    {
        public static List<HardwareRevision> GetRevisions()
        {
            return new()
            {
                // Sony - PlayStation

                R("Sony", "PlayStation", "PlayStation", ""),
                R("Sony", "PlayStation", "PS One", "PS One"),

                R("Sony", "PlayStation", "PlayStation 2", "Fat"),
                R("Sony", "PlayStation", "PlayStation 2", "Slim"),

                R("Sony", "PlayStation", "PlayStation 3", "Fat"),
                R("Sony", "PlayStation", "PlayStation 3", "Slim"),
                R("Sony", "PlayStation", "PlayStation 3", "Super Slim"),

                R("Sony", "PlayStation", "PlayStation 4", "Standard"),
                R("Sony", "PlayStation", "PlayStation 4 Slim", "Slim"),
                R("Sony", "PlayStation", "PlayStation 4 Pro", "Pro"),

                R("Sony", "PlayStation", "PlayStation 5", "Standard"),
                R("Sony", "PlayStation", "PlayStation 5 Digital", "Digital"),
                R("Sony", "PlayStation", "PlayStation 5 Slim", "Slim"),
                R("Sony", "PlayStation", "PlayStation 5 Pro", "Pro"),

                // Sony - PSP

                R("Sony", "PSP", "PSP", "1000"),
                R("Sony", "PSP", "PSP", "2000"),
                R("Sony", "PSP", "PSP", "3000"),
                R("Sony", "PSP", "PSP Go", "Go"),
                R("Sony", "PSP", "PSP Street", "Street"),

                // Sony - Vita

                R("Sony", "PlayStation Vita", "PlayStation Vita", "1000"),
                R("Sony", "PlayStation Vita", "PlayStation Vita", "2000"),
                R("Sony", "PlayStation Vita", "PlayStation TV", "TV"),

                // Microsoft

                R("Microsoft", "Xbox", "Xbox", "Original"),

                R("Microsoft", "Xbox", "Xbox 360", "Fat"),
                R("Microsoft", "Xbox", "Xbox 360", "Slim"),
                R("Microsoft", "Xbox", "Xbox 360", "E"),

                R("Microsoft", "Xbox", "Xbox One", "Standard"),
                R("Microsoft", "Xbox", "Xbox One S", "S"),
                R("Microsoft", "Xbox", "Xbox One X", "X"),

                R("Microsoft", "Xbox", "Xbox Series S", "Series S"),
                R("Microsoft", "Xbox", "Xbox Series X", "Series X"),

                // Nintendo - Home

                R("Nintendo", "Home Consoles", "NES", "Standard"),
                R("Nintendo", "Home Consoles", "SNES", "Standard"),
                R("Nintendo", "Home Consoles", "Nintendo 64", "Standard"),
                R("Nintendo", "Home Consoles", "GameCube", "Standard"),
                R("Nintendo", "Home Consoles", "Wii", "Standard"),
                R("Nintendo", "Home Consoles", "Wii Mini", "Mini"),
                R("Nintendo", "Home Consoles", "Wii U", "Standard"),

                R("Nintendo", "Home Consoles", "Nintendo Switch", "Standard"),
                R("Nintendo", "Home Consoles", "Nintendo Switch Lite", "Lite"),
                R("Nintendo", "Home Consoles", "Nintendo Switch OLED", "OLED"),

                // Nintendo - Handhelds

                R("Nintendo", "Handhelds", "Game Boy", "DMG"),
                R("Nintendo", "Handhelds", "Game Boy Pocket", "Pocket"),
                R("Nintendo", "Handhelds", "Game Boy Light", "Light"),
                R("Nintendo", "Handhelds", "Game Boy Color", "Color"),

                R("Nintendo", "Handhelds", "Game Boy Advance", "Standard"),
                R("Nintendo", "Handhelds", "Game Boy Advance SP", "SP"),
                R("Nintendo", "Handhelds", "Game Boy Micro", "Micro"),

                R("Nintendo", "Handhelds", "Nintendo DS", "Standard"),
                R("Nintendo", "Handhelds", "Nintendo DS Lite", "Lite"),
                R("Nintendo", "Handhelds", "Nintendo DSi", "DSi"),
                R("Nintendo", "Handhelds", "Nintendo DSi XL", "DSi XL"),

                R("Nintendo", "Handhelds", "Nintendo 3DS", "Standard"),
                R("Nintendo", "Handhelds", "Nintendo 3DS XL", "XL"),
                R("Nintendo", "Handhelds", "New Nintendo 3DS", "New"),
                R("Nintendo", "Handhelds", "New Nintendo 3DS XL", "New XL"),
                R("Nintendo", "Handhelds", "Nintendo 2DS", "2DS"),
                R("Nintendo", "Handhelds", "New Nintendo 2DS XL", "New 2DS XL"),

                // SEGA

                R("SEGA", "SEGA Consoles", "SG-1000", "Standard"),
                R("SEGA", "SEGA Consoles", "Master System", "Standard"),
                R("SEGA", "SEGA Consoles", "Mega Drive", "Standard"),
                R("SEGA", "SEGA Consoles", "Genesis", "Standard"),
                R("SEGA", "SEGA Consoles", "Saturn", "Standard"),
                R("SEGA", "SEGA Consoles", "Dreamcast", "Standard"),
                R("SEGA", "SEGA Consoles", "Pico", "Standard"),

                R("SEGA", "SEGA Handhelds", "Game Gear", "Standard"),
                R("SEGA", "SEGA Handhelds", "Nomad", "Standard"),

                // Atari

                R("Atari", "Atari Consoles", "Atari 2600", "Standard"),
                R("Atari", "Atari Consoles", "Atari 5200", "Standard"),
                R("Atari", "Atari Consoles", "Atari 7800", "Standard"),
                R("Atari", "Atari Consoles", "Atari XEGS", "Standard"),
                R("Atari", "Atari Consoles", "Atari Jaguar", "Standard"),
                R("Atari", "Atari Handhelds", "Atari Lynx", "Standard"),

                // PC / Other

                R("PC", "PC", "PC", "Standard"),
                R("Other", "Other", "Other", "Standard")
            };
        }

        private static HardwareRevision R(
            string manufacturer,
            string family,
            string platform,
            string revision)
        {
            return new HardwareRevision
            {
                Manufacturer = manufacturer,
                Family = family,
                Platform = platform,
                Revision = revision
            };
        }
    }
}