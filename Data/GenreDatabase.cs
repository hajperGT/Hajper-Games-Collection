using System;

namespace Hajper_Game_Collection.Data
{
    internal static class GenreDatabase
    {
        public static string TranslateGenre(string genre, string language)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return genre;

            if (language == "pl")
            {
                return genre switch
                {
                    "Action" => "Akcja",
                    "Adventure" => "Przygodowa",
                    "Strategy" => "Strategiczna",
                    "Tactical" => "Taktyczna",
                    "Simulation" => "Symulacja",
                    "Racing" => "Wyścigi",
                    "Sports" => "Sportowa",
                    "Fighting" => "Bijatyka",
                    "Platform" => "Platformowa",
                    "Puzzle" => "Logiczna",
                    "Rhythm" => "Rytmiczna",
                    "Educational" => "Edukacyjna",
                    "Party" => "Imprezowa",
                    "Other" => "Inna",
                    _ => genre
                };
            }

            return genre switch
            {
                "Akcja" => "Action",
                "Przygodowa" => "Adventure",
                "Strategiczna" => "Strategy",
                "Taktyczna" => "Tactical",
                "Symulacja" => "Simulation",
                "Wyścigi" => "Racing",
                "Sportowa" => "Sports",
                "Bijatyka" => "Fighting",
                "Platformowa" => "Platform",
                "Logiczna" => "Puzzle",
                "Rytmiczna" => "Rhythm",
                "Edukacyjna" => "Educational",
                "Imprezowa" => "Party",
                "Inna" => "Other",
                _ => genre
            };
        }
    }
}