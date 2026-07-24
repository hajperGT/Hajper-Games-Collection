using HGC.Models;
using System;
using System.IO;
using Microsoft.Win32;
using System.Text.Json;

namespace HGC.Services
{
    internal static class HgcSettingsService
    {
        private const string SettingsFileName = "Settings.hgc";

        public static string GetSettingsFilePath()
        {
            return Path.Combine(
                HgcStorageService.GetDataFolder(),
                SettingsFileName);
        }

        public static void SaveSettings(HgcSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetSettingsFilePath(), json);
        }

        public static HgcSettings LoadSettings()
        {
            string path = GetSettingsFilePath();

            if (!File.Exists(path))
            {
                HgcSettings settings = new HgcSettings();

                using RegistryKey? key =
                    Registry.CurrentUser.OpenSubKey(@"Software\HGC");

                if (key != null)
                {
                    string? language =
                        key.GetValue("DefaultLanguage") as string;

                    string? currency =
                        key.GetValue("DefaultCurrency") as string;

                    if (language == "pl" || language == "en")
                        settings.Language = language;

                    if (!string.IsNullOrWhiteSpace(currency))
                        settings.DefaultCurrency = currency;
                }

                SaveSettings(settings);

                return settings;
            }

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<HgcSettings>(json)
                   ?? new HgcSettings();
        }
    }
}