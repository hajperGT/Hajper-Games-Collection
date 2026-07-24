namespace Hajper_Game_Collection.Data
{
    internal static class ServiceTypeDatabase
    {
        public static string Translate(string serviceType, string language)
        {
            if (string.IsNullOrWhiteSpace(serviceType))
                return serviceType;

            if (language == "pl")
            {
                return serviceType switch
                {
                    "Cleaning" => "Czyszczenie",
                    "Repair" => "Naprawa",
                    "Modification" => "Modyfikacja",
                    "Drive replacement" => "Wymiana dysku",
                    "Optical drive replacement" => "Wymiana napędu",
                    "Battery replacement" => "Wymiana baterii",
                    "Other" => "Inne",
                    _ => serviceType
                };
            }

            return serviceType switch
            {
                "Czyszczenie" => "Cleaning",
                "Naprawa" => "Repair",
                "Modyfikacja" => "Modification",
                "Wymiana dysku" => "Drive replacement",
                "Wymiana napędu" => "Optical drive replacement",
                "Wymiana baterii" => "Battery replacement",
                "Inne" => "Other",
                _ => serviceType
            };
        }
    }
}