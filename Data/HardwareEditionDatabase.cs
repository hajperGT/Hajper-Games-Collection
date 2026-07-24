using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareEditionDatabase
    {
        public static List<HardwareEdition> GetEditions()
        {
            return new()
            {
               
// =========================
// SONY - PLAYSTATION
// =========================

E("Sony", "PlayStation", "Net Yaroze", "Official developer edition"),

// =========================
// SONY - PLAYSTATION 2
// =========================

E("Sony", "PlayStation 2", "Final Fantasy X Edition", ""),
E("Sony", "PlayStation 2", "Final Fantasy XII Edition", ""),
E("Sony", "PlayStation 2", "Dragon Quest VIII Edition", ""),
E("Sony", "PlayStation 2", "Gran Turismo Edition", ""),
E("Sony", "PlayStation 2", "Resident Evil Code Veronica Edition", ""),
E("Sony", "PlayStation 2", "Gundam Edition", ""),
E("Sony", "PlayStation 2", "Metal Gear Solid Edition", ""),

// =========================
// SONY - PLAYSTATION 3
// =========================

E("Sony", "PlayStation 3", "Final Fantasy XIII Lightning Edition", ""),
E("Sony", "PlayStation 3", "Final Fantasy XIII-2 Lightning Edition Ver.2", ""),
E("Sony", "PlayStation 3", "Final Fantasy X/X-2 Edition", ""),
E("Sony", "PlayStation 3", "Tales of Xillia Edition", ""),
E("Sony", "PlayStation 3", "Ni No Kuni Edition", ""),
E("Sony", "PlayStation 3", "Yakuza Edition", ""),
E("Sony", "PlayStation 3", "One Piece Edition", ""),
E("Sony", "PlayStation 3", "Gundam Edition", ""),
E("Sony", "PlayStation 3", "Metal Gear Solid 4 Edition", ""),
E("Sony", "PlayStation 3", "Metal Gear Rising Edition", ""),
E("Sony", "PlayStation 3", "Gran Turismo 5 Edition", ""),
E("Sony", "PlayStation 3", "God of War III Edition", ""),
E("Sony", "PlayStation 3", "Ni No Kuni Magical Edition", ""),

// =========================
// SONY - PSP
// =========================

E("Sony", "PSP", "Final Fantasy VII Crisis Core Edition", ""),
E("Sony", "PSP", "Final Fantasy Dissidia Edition", ""),
E("Sony", "PSP", "Monster Hunter Portable 3rd Edition", ""),
E("Sony", "PSP", "Metal Gear Solid Peace Walker Edition", ""),
E("Sony", "PSP", "Kingdom Hearts Birth by Sleep Edition", ""),
E("Sony", "PSP", "Star Ocean First Departure Edition", ""),
E("Sony", "PSP", "Phantasy Star Portable Edition", ""),
E("Sony", "PSP", "Hatsune Miku Project Diva Edition", ""),
E("Sony", "PSP", "God Eater Edition", ""),
E("Sony", "PSP", "Gundam vs Gundam Edition", ""),
E("Sony", "PSP", "Tales of the World Edition", ""),
E("Sony", "PSP", "AKB48 Edition", ""),

// =========================
// SONY - PLAYSTATION VITA
// =========================

E("Sony", "PlayStation Vita", "Persona 4 Golden Edition", ""),
E("Sony", "PlayStation Vita", "Persona 4 Dancing All Night Edition", ""),
E("Sony", "PlayStation Vita", "Final Fantasy X/X-2 Edition", ""),
E("Sony", "PlayStation Vita", "God Eater 2 Edition", ""),
E("Sony", "PlayStation Vita", "Soul Sacrifice Edition", ""),
E("Sony", "PlayStation Vita", "Dragon Quest Metal Slime Edition", ""),
E("Sony", "PlayStation Vita", "Freedom Wars Edition", ""),
E("Sony", "PlayStation Vita", "Hatsune Miku Limited Edition", ""),
E("Sony", "PlayStation Vita", "Minecraft Special Edition", ""),
E("Sony", "PlayStation Vita", "Gundam Breaker Edition", ""),
E("Sony", "PlayStation Vita", "Phantasy Star Nova Edition", ""),
E("Sony", "PlayStation Vita", "Attack on Titan Edition", ""),

// =========================
// SONY - PLAYSTATION 4
// =========================

E("Sony", "PlayStation 4", "20th Anniversary Edition", ""),
E("Sony", "PlayStation 4", "Metal Gear Solid V Limited Edition", ""),
E("Sony", "PlayStation 4", "Destiny The Taken King Limited Edition", ""),
E("Sony", "PlayStation 4", "Star Wars Battlefront Limited Edition", ""),
E("Sony", "PlayStation 4", "Uncharted 4 Limited Edition", ""),
E("Sony", "PlayStation 4", "Call of Duty Black Ops III Limited Edition", ""),
E("Sony", "PlayStation 4", "Days of Play Limited Edition", ""),

// =========================
// SONY - PLAYSTATION 4 PRO
// =========================

E("Sony", "PlayStation 4 Pro", "500 Million Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "Marvel's Spider-Man Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "God of War Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "Monster Hunter World Rathalos Edition", ""),
E("Sony", "PlayStation 4 Pro", "Kingdom Hearts III Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "Death Stranding Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "The Last of Us Part II Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "Star Wars Battlefront II Limited Edition", ""),
E("Sony", "PlayStation 4 Pro", "Ghost of Tsushima Limited Edition", ""),

// =========================
// SONY - PLAYSTATION 5
// =========================

E("Sony", "PlayStation 5", "Marvel's Spider-Man 2 Limited Edition", ""),
E("Sony", "PlayStation 5", "30th Anniversary Limited Edition", ""),

// =========================
// SONY - PLAYSTATION 5 PRO
// =========================

E("Sony", "PlayStation 5 Pro", "30th Anniversary Limited Edition", ""),

// =========================
// MICROSOFT - XBOX
// =========================

E("Microsoft", "Xbox", "Crystal Pack", ""),

// =========================
// MICROSOFT - XBOX 360
// =========================

E("Microsoft", "Xbox 360", "Halo 3 Special Edition", ""),
E("Microsoft", "Xbox 360", "Resident Evil 5 Limited Edition", ""),
E("Microsoft", "Xbox 360", "Call of Duty Modern Warfare 2 Edition", ""),
E("Microsoft", "Xbox 360", "Call of Duty Modern Warfare 3 Edition", ""),
E("Microsoft", "Xbox 360", "Star Wars Kinect Edition", ""),
E("Microsoft", "Xbox 360", "Halo Reach Limited Edition", ""),
E("Microsoft", "Xbox 360", "Halo 4 Limited Edition", ""),
E("Microsoft", "Xbox 360", "Gears of War 3 Limited Edition", ""),

// =========================
// MICROSOFT - XBOX ONE
// =========================

E("Microsoft", "Xbox One", "Day One Edition", ""),
E("Microsoft", "Xbox One", "Titanfall Edition", ""),

// =========================
// MICROSOFT - XBOX ONE S
// =========================

E("Microsoft", "Xbox One S", "Battlefield 1 Military Green Special Edition", ""),
E("Microsoft", "Xbox One S", "Gears of War 4 Limited Edition", ""),
E("Microsoft", "Xbox One S", "Minecraft Limited Edition", ""),
E("Microsoft", "Xbox One S", "Fortnite Purple Special Edition", ""),

// =========================
// MICROSOFT - XBOX ONE X
// =========================

E("Microsoft", "Xbox One X", "Project Scorpio Edition", ""),
E("Microsoft", "Xbox One X", "Gold Rush Special Edition", ""),
E("Microsoft", "Xbox One X", "Hyperspace Special Edition", ""),
E("Microsoft", "Xbox One X", "Fallout 76 Robot White Edition", ""),
E("Microsoft", "Xbox One X", "Gears 5 Limited Edition", ""),
E("Microsoft", "Xbox One X", "Cyberpunk 2077 Limited Edition", ""),
E("Microsoft", "Xbox One X", "Taco Bell Eclipse Limited Edition", ""),

// =========================
// MICROSOFT - XBOX SERIES X
// =========================

E("Microsoft", "Xbox Series X", "Halo Infinite Limited Edition", ""),
E("Microsoft", "Xbox Series X", "Galaxy Black Special Edition", ""),

// =========================
// NINTENDO 64
// =========================

E("Nintendo", "Nintendo 64", "Pikachu Edition", ""),
E("Nintendo", "Nintendo 64", "Pokémon Stadium Edition", ""),
E("Nintendo", "Nintendo 64", "Daiei Hawks Edition", ""),
E("Nintendo", "Nintendo 64", "Clear Blue Edition", ""),
E("Nintendo", "Nintendo 64", "Hello Kitty Edition", ""),

// =========================
// GAMECUBE
// =========================

E("Nintendo", "GameCube", "Pokémon XD Gale of Darkness Edition", ""),
E("Nintendo", "GameCube", "Gundam Char Custom Edition", ""),
E("Nintendo", "GameCube", "Tales of Symphonia Edition", ""),
E("Nintendo", "GameCube", "Hanshin Tigers Edition", ""),
E("Nintendo", "GameCube", "Mobile Suit Gundam Edition", ""),

E("Panasonic", "Panasonic Q", "Standard Edition", ""),

// =========================
// GAME BOY COLOR
// =========================

E("Nintendo", "Game Boy Color", "Pokémon Center Edition", ""),
E("Nintendo", "Game Boy Color", "Pokémon Gold & Silver Edition", ""),

// =========================
// GAME BOY ADVANCE
// =========================

E("Nintendo", "Game Boy Advance", "Pokémon Center Edition", ""),
E("Nintendo", "Game Boy Advance", "Mario Edition", ""),

// =========================
// GAME BOY ADVANCE SP
// =========================

E("Nintendo", "Game Boy Advance SP", "NES Classic Edition", ""),
E("Nintendo", "Game Boy Advance SP", "Tribal Edition", ""),
E("Nintendo", "Game Boy Advance SP", "Kingdom Hearts Edition", ""),
E("Nintendo", "Game Boy Advance SP", "Pokémon Center Edition", ""),
E("Nintendo", "Game Boy Advance SP", "Rayquaza Edition", ""),
E("Nintendo", "Game Boy Advance SP", "Charizard Edition", ""),

// =========================
// NINTENDO DS
// =========================

E("Nintendo", "Nintendo DS", "Nintendogs Edition", ""),
E("Nintendo", "Nintendo DS", "Mario Kart Edition", ""),

// =========================
// NINTENDO DS LITE
// =========================

E("Nintendo", "Nintendo DS Lite", "The Legend of Zelda Phantom Hourglass Edition", ""),
E("Nintendo", "Nintendo DS Lite", "Pokémon Dialga & Palkia Edition", ""),
E("Nintendo", "Nintendo DS Lite", "Final Fantasy III Edition", ""),
E("Nintendo", "Nintendo DS Lite", "Guitar Hero Edition", ""),
E("Nintendo", "Nintendo DS Lite", "Crimson Black Edition", ""),
E("Nintendo", "Nintendo DS Lite", "Pokémon Center Edition", ""),

// =========================
// NINTENDO DSi
// =========================

E("Nintendo", "Nintendo DSi", "Kingdom Hearts 358/2 Days Edition", ""),
E("Nintendo", "Nintendo DSi", "Pokémon Black Edition", ""),
E("Nintendo", "Nintendo DSi", "Pokémon White Edition", ""),

// =========================
// NINTENDO DSi XL
// =========================

E("Nintendo", "Nintendo DSi XL", "Super Mario Bros 25th Anniversary Edition", ""),

// =========================
// NINTENDO 3DS
// =========================

E("Nintendo", "Nintendo 3DS", "The Legend of Zelda Ocarina of Time Edition", ""),
E("Nintendo", "Nintendo 3DS", "The Legend of Zelda 25th Anniversary Edition", ""),
E("Nintendo", "Nintendo 3DS", "Mario Edition", ""),
E("Nintendo", "Nintendo 3DS", "Fire Emblem Awakening Edition", ""),
E("Nintendo", "Nintendo 3DS", "Kingdom Hearts Edition", ""),
E("Nintendo", "Nintendo 3DS", "Monster Hunter Edition", ""),

// =========================
// NINTENDO 3DS XL
// =========================

E("Nintendo", "Nintendo 3DS XL", "Pikachu Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Pokémon X Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Pokémon Y Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Animal Crossing Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Luigi Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Year of Luigi Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Zelda A Link Between Worlds Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Super Smash Bros Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Persona Q Edition", ""),
E("Nintendo", "Nintendo 3DS XL", "Monster Hunter 4 Ultimate Edition", ""),

// =========================
// NEW NINTENDO 3DS
// =========================

E("Nintendo", "New Nintendo 3DS", "Super Mario Edition", ""),
E("Nintendo", "New Nintendo 3DS", "Animal Crossing Happy Home Designer Edition", ""),

// =========================
// NEW NINTENDO 3DS XL
// =========================

E("Nintendo", "New Nintendo 3DS XL", "Majora's Mask Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Monster Hunter 4 Ultimate Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Monster Hunter Generations Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Hyrule Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Pokémon Solgaleo Lunala Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Super Nintendo Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Samus Edition", ""),
E("Nintendo", "New Nintendo 3DS XL", "Fire Emblem Fates Edition", ""),

// =========================
// NINTENDO 2DS
// =========================

E("Nintendo", "Nintendo 2DS", "Super Mario Maker Edition", ""),
E("Nintendo", "Nintendo 2DS", "Pokémon Sun Edition", ""),
E("Nintendo", "Nintendo 2DS", "Pokémon Moon Edition", ""),
E("Nintendo", "Nintendo 2DS", "Tomodachi Life Edition", ""),

// =========================
// NEW NINTENDO 2DS XL
// =========================

E("Nintendo", "New Nintendo 2DS XL", "Poké Ball Edition", ""),
E("Nintendo", "New Nintendo 2DS XL", "Pikachu Edition", ""),
E("Nintendo", "New Nintendo 2DS XL", "Hylian Shield Edition", ""),
E("Nintendo", "New Nintendo 2DS XL", "Minecraft Creeper Edition", ""),
E("Nintendo", "New Nintendo 2DS XL", "Animal Crossing Edition", ""),
E("Nintendo", "New Nintendo 2DS XL", "Mario Kart 7 Edition", ""),

// =========================
// NINTENDO SWITCH
// =========================

E("Nintendo", "Nintendo Switch", "Super Mario Odyssey Edition", ""),
E("Nintendo", "Nintendo Switch", "Pokémon Let's Go Pikachu Edition", ""),
E("Nintendo", "Nintendo Switch", "Pokémon Let's Go Eevee Edition", ""),
E("Nintendo", "Nintendo Switch", "Diablo III Eternal Collection Edition", ""),
E("Nintendo", "Nintendo Switch", "Dragon Quest XI S Edition", ""),
E("Nintendo", "Nintendo Switch", "Disney Tsum Tsum Festival Edition", ""),
E("Nintendo", "Nintendo Switch", "Monster Hunter Rise Edition", ""),
E("Nintendo", "Nintendo Switch", "Fortnite Wildcat Edition", ""),
E("Nintendo", "Nintendo Switch", "Mario Red & Blue Edition", ""),
E("Nintendo", "Nintendo Switch", "Animal Crossing New Horizons Edition", ""),
E("Nintendo", "Nintendo Switch", "Splatoon 2 Edition", ""),
E("Nintendo", "Nintendo Switch", "Splatoon 3 Edition", ""),

// =========================
// NINTENDO SWITCH LITE
// =========================

E("Nintendo", "Nintendo Switch Lite", "Pokémon Dialga & Palkia Edition", ""),
E("Nintendo", "Nintendo Switch Lite", "Zacian & Zamazenta Edition", ""),
E("Nintendo", "Nintendo Switch Lite", "Animal Crossing Isabelle's Aloha Edition", ""),

// =========================
// NINTENDO SWITCH OLED
// =========================

E("Nintendo", "Nintendo Switch OLED", "Pokémon Scarlet & Violet Edition", ""),
E("Nintendo", "Nintendo Switch OLED", "The Legend of Zelda Tears of the Kingdom Edition", ""),
E("Nintendo", "Nintendo Switch OLED", "Splatoon 3 Edition", ""),
E("Nintendo", "Nintendo Switch OLED", "Super Smash Bros Ultimate Edition", ""),

// =========================
// SEGA - GAME GEAR
// =========================

E("SEGA", "Game Gear", "Coca-Cola Edition", ""),
E("SEGA", "Game Gear", "Magic Knight Rayearth Edition", ""),

// =========================
// SEGA - SATURN
// =========================

E("SEGA", "Saturn", "This Is Cool Edition", ""),
E("SEGA", "Saturn", "Derby Stallion Edition", ""),
E("SEGA", "Saturn", "Skeleton Saturn Edition", ""),
E("SEGA", "Saturn", "Virtua Fighter Remix Edition", ""),
E("SEGA", "Saturn", "Sakura Wars Edition", ""),
E("SEGA", "Saturn", "Hitachi Hi-Saturn Navi Edition", ""),
E("SEGA", "Saturn", "Victor V-Saturn Edition", ""),

// =========================
// SEGA - DREAMCAST
// =========================

E("SEGA", "Dreamcast", "Hello Kitty Edition", ""),
E("SEGA", "Dreamcast", "Hello Kitty Blue Edition", ""),
E("SEGA", "Dreamcast", "Hello Kitty Pink Edition", ""),
E("SEGA", "Dreamcast", "Sakura Wars Edition", ""),
E("SEGA", "Dreamcast", "Resident Evil Code Veronica Edition", ""),
E("SEGA", "Dreamcast", "Biohazard Code Veronica Edition", ""),
E("SEGA", "Dreamcast", "Seaman Christmas Edition", ""),
E("SEGA", "Dreamcast", "Maziora Dreamcast Edition", ""),
E("SEGA", "Dreamcast", "R7 Edition", ""),
E("SEGA", "Dreamcast", "Pearl Blue Edition", ""),
E("SEGA", "Dreamcast", "Black Dreamcast Edition", ""),
E("SEGA", "Dreamcast", "Toyota Netz Edition", ""),
E("SEGA", "Dreamcast", "CX-1 Edition", ""),
E("SEGA", "Dreamcast", "Divers 2000 CX-1 Edition", ""),

// =========================
// ATARI
// =========================

E("Atari", "Atari 2600", "Darth Vader Edition", ""),

E("Atari", "Atari Lynx", "Telegames Edition", ""),

E("Atari", "Atari Jaguar", "Jaguar Kiosk Edition", ""),
E("Atari", "Jaguar CD", "Jaguar CD Promotional Edition", ""),

// =========================
// SNK
// =========================

E("SNK", "Neo Geo AES", "SNK Gold System Edition", ""),

E("SNK", "Neo Geo Pocket Color", "SNK Camouflage Edition", ""),
E("SNK", "Neo Geo Pocket Color", "Neo 21 Edition", ""),
E("SNK", "Neo Geo Pocket Color", "King of Fighters R-2 Edition", ""),

// =========================
// NEC
// =========================

E("NEC", "PC Engine", "Shuttle Edition", ""),

E("NEC", "PC Engine Duo", "PC Engine Duo Gundam Edition", ""),

E("NEC", "TurboGrafx-16", "TurboExpress Limited Edition", ""),

// =========================
// COMMODORE
// =========================

E("Commodore", "Commodore 64", "Aldi Edition", ""),

// =========================
// AMIGA
// =========================

E("Amiga", "Amiga 500", "Batman Pack", ""),
E("Amiga", "Amiga 500", "Cartoon Classics Pack", ""),
E("Amiga", "Amiga 1200", "Magic Pack", ""),


            };
        }

        private static HardwareEdition E(
            string manufacturer,
            string platform,
            string editionName,
            string notes)
        {
            return new HardwareEdition
            {
                Manufacturer = manufacturer,
                Platform = platform,
                EditionName = editionName,
                Notes = notes
            };
        }
    }
}