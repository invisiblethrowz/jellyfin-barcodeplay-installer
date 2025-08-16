using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class TMDbClient
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<TMDbClient> _log;

        public TMDbClient(IHttpClientFactory httpFactory, ILogger<TMDbClient> log)
        {
            _httpFactory = httpFactory;
            _log = log;
        }

        public async Task<(string? Title, int? Year)> RefineAsync(string rawTitle, string? apiKey, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _log.LogInformation("[TMDb] No API key set; using UPC title only '{Title}'", rawTitle);
                return (rawTitle, null);
            }

            var url = $"https://api.themoviedb.org/3/search/movie?api_key={Uri.EscapeDataString(apiKey)}&query={Uri.EscapeDataString(rawTitle)}";
            var http = _httpFactory.CreateClient();
            using var resp = await http.GetAsync(url, ct).ConfigureAwait(false);
            var body = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            _log.LogDebug("[TMDb] {Status} body: {Body}", (int)resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode) return (rawTitle, null);

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array || results.GetArrayLength() == 0)
                return (rawTitle, null);

            string? finalTitle = rawTitle;
            int? year = null;

            foreach (var r in results.EnumerateArray())
            {
                var t = r.TryGetProperty("title", out var tt) ? tt.GetString() : null;
                var rd = r.TryGetProperty("release_date", out var rdEl) ? rdEl.GetString() : null;
                if (!string.IsNullOrWhiteSpace(rd) && rd!.Length >= 4 && int.TryParse(rd.Substring(0, 4), out var y))
                    year = y;

                if (!string.IsNullOrWhiteSpace(t) && string.Equals(t, rawTitle, StringComparison.OrdinalIgnoreCase))
                {
                    finalTitle = t;
                    break;
                }
                if (finalTitle == rawTitle)
                    finalTitle = t ?? rawTitle;
            }
            return (finalTitle, year);
        }
    }
}
