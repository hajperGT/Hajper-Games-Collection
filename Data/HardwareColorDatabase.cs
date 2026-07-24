using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareColorDatabase
    {
        public static List<HardwareColor> GetColors()
        {
            return new()
            {
                // =========================
                // SONY - PLAYSTATION / PS ONE
                // =========================

                C("Sony", "PlayStation", "Grey"),
                C("Sony", "PS One", "White"),

                // =========================
                // SONY - PLAYSTATION 2
                // =========================

                C("Sony", "PlayStation 2", "Black"),
                C("Sony", "PlayStation 2", "Ceramic White"),
                C("Sony", "PlayStation 2", "Satin Silver"),
                C("Sony", "PlayStation 2", "Aqua Blue"),
                C("Sony", "PlayStation 2", "Ocean Blue"),
                C("Sony", "PlayStation 2", "Midnight Black"),
                C("Sony", "PlayStation 2", "Sakura Pink"),
                C("Sony", "PlayStation 2", "Pink"),

                // =========================
                // SONY - PLAYSTATION 3
                // =========================

                C("Sony", "PlayStation 3", "Piano Black"),
                C("Sony", "PlayStation 3", "Ceramic White"),
                C("Sony", "PlayStation 3", "Satin Silver"),
                C("Sony", "PlayStation 3", "Classic White"),
                C("Sony", "PlayStation 3", "Charcoal Black"),
                C("Sony", "PlayStation 3", "Scarlet Red"),
                C("Sony", "PlayStation 3", "Splash Blue"),
                C("Sony", "PlayStation 3", "Garnet Red"),
                C("Sony", "PlayStation 3", "Azurite Blue"),

                // =========================
                // SONY - PLAYSTATION 4
                // =========================

                C("Sony", "PlayStation 4", "Jet Black"),
                C("Sony", "PlayStation 4", "Glacier White"),

                C("Sony", "PlayStation 4 Slim", "Jet Black"),
                C("Sony", "PlayStation 4 Slim", "Glacier White"),
                C("Sony", "PlayStation 4 Slim", "Gold"),
                C("Sony", "PlayStation 4 Slim", "Silver"),

                C("Sony", "PlayStation 4 Pro", "Jet Black"),
                C("Sony", "PlayStation 4 Pro", "Glacier White"),

                // =========================
                // SONY - PLAYSTATION 5
                // =========================

                C("Sony", "PlayStation 5", "White"),
                C("Sony", "PlayStation 5", "Midnight Black"),
                C("Sony", "PlayStation 5", "Cosmic Red"),
                C("Sony", "PlayStation 5", "Nova Pink"),
                C("Sony", "PlayStation 5", "Starlight Blue"),
                C("Sony", "PlayStation 5", "Galactic Purple"),
                C("Sony", "PlayStation 5", "Grey Camouflage"),
                C("Sony", "PlayStation 5", "Volcanic Red"),
                C("Sony", "PlayStation 5", "Cobalt Blue"),
                C("Sony", "PlayStation 5", "Sterling Silver"),
                C("Sony", "PlayStation 5", "Chroma Pearl"),
                C("Sony", "PlayStation 5", "Chroma Indigo"),
                C("Sony", "PlayStation 5", "Chroma Teal"),

                C("Sony", "PlayStation 5 Slim", "White"),
                C("Sony", "PlayStation 5 Slim", "Midnight Black"),
                C("Sony", "PlayStation 5 Slim", "Cosmic Red"),
                C("Sony", "PlayStation 5 Slim", "Nova Pink"),
                C("Sony", "PlayStation 5 Slim", "Starlight Blue"),
                C("Sony", "PlayStation 5 Slim", "Galactic Purple"),
                C("Sony", "PlayStation 5 Slim", "Chroma Pearl"),
                C("Sony", "PlayStation 5 Slim", "Chroma Indigo"),
                C("Sony", "PlayStation 5 Slim", "Chroma Teal"),

                C("Sony", "PlayStation 5 Pro", "White"),

                // =========================
                // SONY - PSP
                // =========================

                C("Sony", "PSP", "Piano Black"),
                C("Sony", "PSP", "Ceramic White"),
                C("Sony", "PSP", "Ice Silver"),
                C("Sony", "PSP", "Mystic Silver"),
                C("Sony", "PSP", "Pearl White"),
                C("Sony", "PSP", "Radiant Red"),
                C("Sony", "PSP", "Vibrant Blue"),
                C("Sony", "PSP", "Felicia Blue"),
                C("Sony", "PSP", "Rose Pink"),
                C("Sony", "PSP", "Blossom Pink"),
                C("Sony", "PSP", "Lavender Purple"),
                C("Sony", "PSP", "Bright Yellow"),
                C("Sony", "PSP", "Spirited Green"),
                C("Sony", "PSP", "Mint Green"),
                C("Sony", "PSP", "Deep Red"),
                C("Sony", "PSP", "Matte Bronze"),
                C("Sony", "PSP", "Metallic Blue"),

                C("Sony", "PSP Go", "Piano Black"),
                C("Sony", "PSP Go", "Pearl White"),

                C("Sony", "PSP Street", "Charcoal Black"),
                C("Sony", "PSP Street", "Ice White"),

                // =========================
                // SONY - PLAYSTATION VITA
                // =========================

                C("Sony", "PlayStation Vita", "Black"),
                C("Sony", "PlayStation Vita", "Crystal White"),
                C("Sony", "PlayStation Vita", "Cosmic Red"),
                C("Sony", "PlayStation Vita", "Sapphire Blue"),
                C("Sony", "PlayStation Vita", "White"),
                C("Sony", "PlayStation Vita", "Light Blue / White"),
                C("Sony", "PlayStation Vita", "Lime Green / White"),
                C("Sony", "PlayStation Vita", "Pink / Black"),
                C("Sony", "PlayStation Vita", "Khaki / Black"),
                C("Sony", "PlayStation Vita", "Aqua Blue"),
                C("Sony", "PlayStation Vita", "Neon Orange"),

                C("Sony", "PlayStation TV", "White"),
                C("Sony", "PlayStation TV", "Black"),

                // =========================
// MICROSOFT - XBOX
// =========================

C("Microsoft", "Xbox", "Black"),
C("Microsoft", "Xbox", "Crystal"),

// =========================
// MICROSOFT - XBOX 360 FAT
// =========================

C("Microsoft", "Xbox 360", "White"),
C("Microsoft", "Xbox 360", "Black"),

// =========================
// MICROSOFT - XBOX 360 S
// =========================

C("Microsoft", "Xbox 360 S", "Gloss Black"),
C("Microsoft", "Xbox 360 S", "Matte Black"),
C("Microsoft", "Xbox 360 S", "White"),

// =========================
// MICROSOFT - XBOX 360 E
// =========================

C("Microsoft", "Xbox 360 E", "Black"),
C("Microsoft", "Xbox 360 E", "White"),

// =========================
// MICROSOFT - XBOX ONE
// =========================

C("Microsoft", "Xbox One", "Black"),

// =========================
// MICROSOFT - XBOX ONE S
// =========================

C("Microsoft", "Xbox One S", "Robot White"),

// =========================
// MICROSOFT - XBOX ONE X
// =========================

C("Microsoft", "Xbox One X", "Black"),
C("Microsoft", "Xbox One X", "Robot White"),

// =========================
// MICROSOFT - XBOX SERIES S
// =========================

C("Microsoft", "Xbox Series S", "Robot White"),
C("Microsoft", "Xbox Series S", "Carbon Black"),

// =========================
// MICROSOFT - XBOX SERIES X
// =========================

C("Microsoft", "Xbox Series X", "Black"),
C("Microsoft", "Xbox Series X", "Robot White"),

// =========================
// NINTENDO - NES
// =========================

C("Nintendo", "NES", "Grey"),

// =========================
// NINTENDO - SNES
// =========================

C("Nintendo", "SNES", "Grey"),

C("Nintendo", "Super Famicom", "Grey"),

// =========================
// NINTENDO - NINTENDO 64
// =========================

C("Nintendo", "Nintendo 64", "Charcoal Black"),
C("Nintendo", "Nintendo 64", "Jungle Green"),
C("Nintendo", "Nintendo 64", "Ice Blue"),
C("Nintendo", "Nintendo 64", "Fire Orange"),
C("Nintendo", "Nintendo 64", "Watermelon Red"),
C("Nintendo", "Nintendo 64", "Grape Purple"),
C("Nintendo", "Nintendo 64", "Smoke Black"),
C("Nintendo", "Nintendo 64", "Gold"),

// =========================
// NINTENDO - GAMECUBE
// =========================

C("Nintendo", "GameCube", "Indigo"),
C("Nintendo", "GameCube", "Jet Black"),
C("Nintendo", "GameCube", "Platinum"),
C("Nintendo", "GameCube", "Spice Orange"),

C("Panasonic", "Panasonic Q", "Silver"),

// =========================
// NINTENDO - WII
// =========================

C("Nintendo", "Wii", "White"),
C("Nintendo", "Wii", "Black"),
C("Nintendo", "Wii", "Red"),
C("Nintendo", "Wii", "Blue"),

C("Nintendo", "Wii Mini", "Red / Black"),

C("Nintendo", "Wii U", "White"),
C("Nintendo", "Wii U", "Black"),

// =========================
// NINTENDO - SWITCH
// =========================

C("Nintendo", "Nintendo Switch", "Neon Red / Neon Blue"),
C("Nintendo", "Nintendo Switch", "Grey"),

C("Nintendo", "Nintendo Switch Lite", "Yellow"),
C("Nintendo", "Nintendo Switch Lite", "Turquoise"),
C("Nintendo", "Nintendo Switch Lite", "Grey"),
C("Nintendo", "Nintendo Switch Lite", "Coral"),
C("Nintendo", "Nintendo Switch Lite", "Blue"),

C("Nintendo", "Nintendo Switch OLED", "White"),
C("Nintendo", "Nintendo Switch OLED", "Neon Red / Neon Blue"),

C("Nintendo", "Nintendo Switch 2", "Black"),

// =========================
// NINTENDO - GAME BOY
// =========================

C("Nintendo", "Game Boy", "Grey"),

C("Nintendo", "Game Boy Pocket", "Silver"),
C("Nintendo", "Game Boy Pocket", "Red"),
C("Nintendo", "Game Boy Pocket", "Green"),
C("Nintendo", "Game Boy Pocket", "Blue"),
C("Nintendo", "Game Boy Pocket", "Yellow"),
C("Nintendo", "Game Boy Pocket", "Pink"),
C("Nintendo", "Game Boy Pocket", "Black"),
C("Nintendo", "Game Boy Pocket", "White"),

C("Nintendo", "Game Boy Light", "Gold"),
C("Nintendo", "Game Boy Light", "Silver"),

// =========================
// NINTENDO - GAME BOY COLOR
// =========================

C("Nintendo", "Game Boy Color", "Atomic Purple"),
C("Nintendo", "Game Boy Color", "Berry"),
C("Nintendo", "Game Boy Color", "Teal"),
C("Nintendo", "Game Boy Color", "Kiwi"),
C("Nintendo", "Game Boy Color", "Dandelion"),
C("Nintendo", "Game Boy Color", "Grape"),
C("Nintendo", "Game Boy Color", "Blue"),
C("Nintendo", "Game Boy Color", "Green"),
C("Nintendo", "Game Boy Color", "Red"),
C("Nintendo", "Game Boy Color", "Yellow"),
C("Nintendo", "Game Boy Color", "Silver"),
C("Nintendo", "Game Boy Color", "Black"),

// =========================
// NINTENDO - GAME BOY ADVANCE
// =========================

C("Nintendo", "Game Boy Advance", "Glacier"),
C("Nintendo", "Game Boy Advance", "Indigo"),
C("Nintendo", "Game Boy Advance", "Arctic"),
C("Nintendo", "Game Boy Advance", "Fuchsia"),
C("Nintendo", "Game Boy Advance", "Orange"),
C("Nintendo", "Game Boy Advance", "Black"),
C("Nintendo", "Game Boy Advance", "White"),
C("Nintendo", "Game Boy Advance", "Pink"),
C("Nintendo", "Game Boy Advance", "Blue"),

// =========================
// NINTENDO - GAME BOY ADVANCE SP
// =========================

C("Nintendo", "Game Boy Advance SP", "Cobalt Blue"),
C("Nintendo", "Game Boy Advance SP", "Platinum Silver"),
C("Nintendo", "Game Boy Advance SP", "Pearl Blue"),
C("Nintendo", "Game Boy Advance SP", "Pearl Pink"),
C("Nintendo", "Game Boy Advance SP", "Graphite"),
C("Nintendo", "Game Boy Advance SP", "Onyx Black"),
C("Nintendo", "Game Boy Advance SP", "Flame Red"),
C("Nintendo", "Game Boy Advance SP", "Pearl Green"),

// =========================
// NINTENDO - GAME BOY MICRO
// =========================

C("Nintendo", "Game Boy Micro", "Silver"),
C("Nintendo", "Game Boy Micro", "Black"),
C("Nintendo", "Game Boy Micro", "Blue"),
C("Nintendo", "Game Boy Micro", "Green"),
C("Nintendo", "Game Boy Micro", "Pink"),
C("Nintendo", "Game Boy Micro", "Purple"),

// =========================
// NINTENDO - DS
// =========================

C("Nintendo", "Nintendo DS", "Silver"),
C("Nintendo", "Nintendo DS", "Blue"),
C("Nintendo", "Nintendo DS", "Pink"),
C("Nintendo", "Nintendo DS", "Red"),
C("Nintendo", "Nintendo DS", "White"),
C("Nintendo", "Nintendo DS", "Black"),

// =========================
// NINTENDO - DS LITE
// =========================

C("Nintendo", "Nintendo DS Lite", "Polar White"),
C("Nintendo", "Nintendo DS Lite", "Onyx Black"),
C("Nintendo", "Nintendo DS Lite", "Coral Pink"),
C("Nintendo", "Nintendo DS Lite", "Crimson Black"),
C("Nintendo", "Nintendo DS Lite", "Cobalt Blue"),
C("Nintendo", "Nintendo DS Lite", "Silver"),

// =========================
// NINTENDO - DSi
// =========================

C("Nintendo", "Nintendo DSi", "Black"),
C("Nintendo", "Nintendo DSi", "White"),
C("Nintendo", "Nintendo DSi", "Pink"),
C("Nintendo", "Nintendo DSi", "Blue"),
C("Nintendo", "Nintendo DSi", "Red"),
C("Nintendo", "Nintendo DSi", "Green"),

// =========================
// NINTENDO - DSi XL
// =========================

C("Nintendo", "Nintendo DSi XL", "Burgundy"),
C("Nintendo", "Nintendo DSi XL", "Bronze"),
C("Nintendo", "Nintendo DSi XL", "Blue"),
C("Nintendo", "Nintendo DSi XL", "Green"),
C("Nintendo", "Nintendo DSi XL", "Yellow"),
C("Nintendo", "Nintendo DSi XL", "Wine Red"),

// =========================
// NINTENDO - 3DS
// =========================

C("Nintendo", "Nintendo 3DS", "Aqua Blue"),
C("Nintendo", "Nintendo 3DS", "Cosmos Black"),
C("Nintendo", "Nintendo 3DS", "Flare Red"),
C("Nintendo", "Nintendo 3DS", "Pearl Pink"),
C("Nintendo", "Nintendo 3DS", "Ice White"),
C("Nintendo", "Nintendo 3DS", "Cobalt Blue"),

// =========================
// NINTENDO - 3DS XL
// =========================

C("Nintendo", "Nintendo 3DS XL", "Blue / Black"),
C("Nintendo", "Nintendo 3DS XL", "Red / Black"),
C("Nintendo", "Nintendo 3DS XL", "Silver / Black"),
C("Nintendo", "Nintendo 3DS XL", "Pink / White"),
C("Nintendo", "Nintendo 3DS XL", "White"),

// =========================
// NINTENDO - NEW 3DS
// =========================

C("Nintendo", "New Nintendo 3DS", "Black"),
C("Nintendo", "New Nintendo 3DS", "White"),

// =========================
// NINTENDO - NEW 3DS XL
// =========================

C("Nintendo", "New Nintendo 3DS XL", "Black"),
C("Nintendo", "New Nintendo 3DS XL", "Red"),
C("Nintendo", "New Nintendo 3DS XL", "Blue"),
C("Nintendo", "New Nintendo 3DS XL", "White"),
C("Nintendo", "New Nintendo 3DS XL", "Metallic Black"),
C("Nintendo", "New Nintendo 3DS XL", "Metallic Blue"),
C("Nintendo", "New Nintendo 3DS XL", "Metallic Red"),

// =========================
// NINTENDO - 2DS
// =========================

C("Nintendo", "Nintendo 2DS", "Red / White"),
C("Nintendo", "Nintendo 2DS", "Blue / Black"),
C("Nintendo", "Nintendo 2DS", "White / Red"),

// =========================
// NINTENDO - NEW 2DS XL
// =========================

C("Nintendo", "New Nintendo 2DS XL", "Black / Turquoise"),
C("Nintendo", "New Nintendo 2DS XL", "White / Orange"),
C("Nintendo", "New Nintendo 2DS XL", "Black / Lime"),
C("Nintendo", "New Nintendo 2DS XL", "Purple / Silver"),

// =========================
// SEGA - SG-1000
// =========================

C("SEGA", "SG-1000", "Black"),
C("SEGA", "SG-1000 II", "Black"),

// =========================
// SEGA - MASTER SYSTEM
// =========================

C("SEGA", "Master System", "Black"),
C("SEGA", "Master System II", "Black"),

// =========================
// SEGA - MEGA DRIVE
// =========================

C("SEGA", "Mega Drive", "Black"),
C("SEGA", "Mega Drive 2", "Black"),
C("SEGA", "Mega Drive 3", "Black"),

// =========================
// SEGA - GENESIS
// =========================

C("SEGA", "Genesis", "Black"),
C("SEGA", "Genesis 2", "Black"),
C("SEGA", "Genesis 3", "Black"),

// =========================
// SEGA - GAME GEAR
// =========================

C("SEGA", "Game Gear", "Black"),
C("SEGA", "Game Gear", "Blue"),
C("SEGA", "Game Gear", "Red"),
C("SEGA", "Game Gear", "Yellow"),
C("SEGA", "Game Gear", "White"),

// =========================
// SEGA - SATURN
// =========================

C("SEGA", "Saturn", "Grey"),
C("SEGA", "Saturn", "White"),
C("SEGA", "Saturn", "Black"),

// =========================
// SEGA - DREAMCAST
// =========================

C("SEGA", "Dreamcast", "White"),
C("SEGA", "Dreamcast", "Black"),

// =========================
// SEGA - PICO
// =========================

C("SEGA", "Pico", "Green"),
C("SEGA", "Pico", "Blue"),

// =========================
// SEGA - NOMAD
// =========================

C("SEGA", "Nomad", "Black"),

// =========================
// ATARI
// =========================

C("Atari", "Atari 2600", "Black"),
C("Atari", "Atari 2600 Jr.", "Black"),

C("Atari", "Atari 5200", "Black"),

C("Atari", "Atari 7800", "Black"),

C("Atari", "Atari XE Game System", "Grey"),

C("Atari", "Atari Lynx", "Black"),
C("Atari", "Atari Lynx II", "Black"),

C("Atari", "Atari Jaguar", "Black"),

C("Atari", "Jaguar CD", "Black"),

// =========================
// COMMODORE
// =========================

C("Commodore", "Commodore 64 Games System", "Beige"),

C("Commodore", "Commodore 64", "Beige"),

C("Commodore", "Commodore 128", "Beige"),

C("Commodore", "Commodore CDTV", "Black"),

C("Commodore", "Amiga CD32", "White"),

// =========================
// AMIGA
// =========================

C("Amiga", "Amiga 500", "Beige"),
C("Amiga", "Amiga 500 Plus", "Beige"),

C("Amiga", "Amiga 600", "Beige"),

C("Amiga", "Amiga 1200", "Beige"),

C("Amiga", "Amiga CD32", "White"),

// =========================
// SNK
// =========================

C("SNK", "Neo Geo AES", "Black"),

C("SNK", "Neo Geo CD", "White"),
C("SNK", "Neo Geo CDZ", "White"),

C("SNK", "Neo Geo Pocket", "Silver"),

C("SNK", "Neo Geo Pocket Color", "Silver"),
C("SNK", "Neo Geo Pocket Color", "Blue"),
C("SNK", "Neo Geo Pocket Color", "Green"),
C("SNK", "Neo Geo Pocket Color", "Red"),
C("SNK", "Neo Geo Pocket Color", "Yellow"),
C("SNK", "Neo Geo Pocket Color", "Black"),

// =========================
// NEC
// =========================

C("NEC", "PC Engine", "White"),

C("NEC", "CoreGrafx", "Grey"),
C("NEC", "CoreGrafx II", "Grey"),

C("NEC", "SuperGrafx", "Grey"),

C("NEC", "PC Engine Duo", "Black"),
C("NEC", "PC Engine Duo-R", "White"),
C("NEC", "PC Engine Duo-RX", "White"),

C("NEC", "TurboGrafx-16", "Black"),

C("NEC", "TurboDuo", "Black"),

C("NEC", "PC-FX", "White"),

// =========================
// OTHER
// =========================

C("Magnavox", "Magnavox Odyssey", "Brown"),
C("Magnavox", "Magnavox Odyssey²", "Black"),

C("Coleco", "ColecoVision", "Black"),

C("Mattel", "Intellivision", "Brown"),

C("GCE", "Vectrex", "Black"),

C("Panasonic", "3DO", "Black"),

C("Philips", "CD-i", "Black"),

C("Apple", "Apple Pippin", "White"),

C("Bandai", "WonderSwan", "White"),
C("Bandai", "WonderSwan Color", "Blue"),
C("Bandai", "WonderSwan Color", "Black"),
C("Bandai", "WonderSwan Color", "Pearl Blue"),

C("Bandai", "SwanCrystal", "Blue"),
C("Bandai", "SwanCrystal", "Red"),
C("Bandai", "SwanCrystal", "Violet"),

C("OUYA", "OUYA", "Silver"),

C("NVIDIA", "NVIDIA Shield", "Black"),

C("Evercade", "Evercade", "White"),
C("Evercade", "Evercade VS", "White"),

C("Analogue", "Analogue Pocket", "Black"),
C("Analogue", "Analogue Pocket", "White"),


            };
        }

        private static HardwareColor C(
            string manufacturer,
            string platform,
            string colorName,
            string notes = "")
        {
            return new HardwareColor
            {
                Manufacturer = manufacturer,
                Platform = platform,
                ColorName = colorName,
                Notes = notes
            };
        }
    }
}