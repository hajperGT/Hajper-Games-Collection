using HGC.Models;
using System;
using System.IO;
using System.Text.Json;

namespace HGC.Services
{
    internal static class HgcStorageService
    {
        private const string DataFileName = "Collection.hgc";

        public static string GetDataFolder()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HGC");

            Directory.CreateDirectory(folder);
            return folder;
        }

        public static string GetCollectionFilePath()
        {
            return Path.Combine(GetDataFolder(), DataFileName);
        }

        public static void SaveCollection(CollectionDatabase collection)
        {
            string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetCollectionFilePath(), json);
        }

        public static CollectionDatabase? LoadCollection()
        {
            string path = GetCollectionFilePath();

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<CollectionDatabase>(json);
        }
    }
}