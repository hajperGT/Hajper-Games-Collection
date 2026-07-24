using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HGC.Services
{
    internal static class RawgService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<List<RawgGameResult>> SearchGamesAsync(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return new List<RawgGameResult>();

            string url =
                $"https://api.rawg.io/api/games?key={RawgConfig.ApiKey}&search={Uri.EscapeDataString(gameName)}";

            try
            {
                string json = await client.GetStringAsync(url);

                RawgSearchResponse? response =
                    JsonSerializer.Deserialize<RawgSearchResponse>(json);

                return response?.Results ?? new List<RawgGameResult>();
            }
            catch
            {
                return new List<RawgGameResult>();
            }
        }
    }
}