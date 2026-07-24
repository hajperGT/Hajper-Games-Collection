namespace Hajper_Game_Collection.Data
{
    internal static class ConditionDatabase
    {
        public static string Translate(string condition, string language)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return condition;

            if (language == "pl")
            {
                return condition switch
                {
                    "New" => "Nowy",
                    "Mint" => "Idealny",

                    "Very Good" => "Bardzo dobry",
                    "Very good" => "Bardzo dobry",

                    "Good" => "Dobry",
                    "Fair" => "Dostateczny",
                    "Acceptable" => "Akceptowalny",
                    "Poor" => "Słaby",
                    "Damaged" => "Uszkodzony",

                    _ => condition
                };
            }

            return condition switch
            {
                // Sprzęt
                "Nowy" => "New",
                "Idealny" => "Mint",
                "Bardzo dobry" => "Very good",
                "Dobry" => "Good",
                "Dostateczny" => "Fair",
                "Akceptowalny" => "Acceptable",
                "Słaby" => "Poor",
                "Uszkodzony" => "Damaged",
                "Uszkodzone" => "Damaged",
                "Uszkodzona" => "Damaged",

                // Nośniki / pudełka
                "Bardzo dobra" => "Very good",
                "Dobra" => "Good",
                "Dostateczna" => "Fair",
                "Słaba" => "Poor",

                _ => condition
            };
        }
    }
}