using backend.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace frontend
{
    public static class GameApiClient                    // itt kotjuk ossze a frontendet a backendel, ez az osztaly felelos az API hivasokert
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://localhost:7175/"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<List<Game>> GetGamesAsync() =>
            await client.GetFromJsonAsync<List<Game>>("api/games", jsonOptions) ?? new();   //azert kell a Game a list be hogy meghatarozzuk hogy milyen adatok kerulhetnek a listaba

        public static async Task<List<string>> GetPlatformsAsync() =>
            await client.GetFromJsonAsync<List<string>>("api/games/platforms", jsonOptions) ?? new();

        public static async Task<List<string>> GetCategoriesAsync() =>
            await client.GetFromJsonAsync<List<string>>("api/games/categories", jsonOptions) ?? new();

        public static async Task<List<Game>> SearchGamesAsync(string searchTerm)
        {
            string encodedQuery = Uri.EscapeDataString(searchTerm ?? string.Empty);
            return await client.GetFromJsonAsync<List<Game>>($"api/games/search?q={encodedQuery}", jsonOptions) ?? new();
        }

        public static async Task AddGameAsync(GameRequest game) =>
            await SendAsync(() => client.PostAsJsonAsync("api/games", game));

        public static async Task<List<player>> GetPlayersAsync() =>
            await client.GetFromJsonAsync<List<player>>("api/players", jsonOptions) ?? new();

        public static async Task AddPlayerAsync(player item) =>
            await SendAsync(() => client.PostAsJsonAsync("api/players", item));

        public static async Task UpdatePlayerAsync(int id, player item) =>
            await SendAsync(() => client.PutAsJsonAsync($"api/players/{id}", item));

        public static async Task DeletePlayerAsync(int id) =>
            await SendAsync(() => client.DeleteAsync($"api/players/{id}"));

        public static async Task<List<Rewiev>> GetReviewsAsync() =>
            await client.GetFromJsonAsync<List<Rewiev>>("api/reviews", jsonOptions) ?? new();

        public static async Task AddReviewAsync(Rewiev item) =>
            await SendAsync(() => client.PostAsJsonAsync("api/reviews", item));

        public static async Task DeleteReviewAsync(int id) =>
            await SendAsync(() => client.DeleteAsync($"api/reviews/{id}"));

        public static async Task<ReviewStatistics?> GetReviewStatisticsAsync() =>
            await client.GetFromJsonAsync<ReviewStatistics>("api/reviews/statistics", jsonOptions);

        public static async Task<List<PlayerRatedGame>> GetRatedGamesAsync(int playerId) =>
            await client.GetFromJsonAsync<List<PlayerRatedGame>>($"api/players/{playerId}/games", jsonOptions) ?? new();

        private static async Task SendAsync(Func<Task<HttpResponseMessage>> send)
        {
            using var response = await send();
            if (response.IsSuccessStatusCode) return;
            string details = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API-hiba ({(int)response.StatusCode} {response.ReasonPhrase}): {details}");
        }
    }

    public class GameRequest
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public string? Platform { get; set; }
        public string? Type { get; set; }
    }

    public class ReviewStatistics
    {
        public int ReviewCount { get; set; }
        public double? AveragePoints { get; set; }
        public int ReviewedGames { get; set; }
    }

    public class PlayerRatedGame
    {
        public int GameId { get; set; }
        public string? GameName { get; set; }
        public int Points { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewReleaseDate { get; set; }
    }
}
