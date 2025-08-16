using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class UPCItemDbClient
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<UPCItemDbClient> _log;

        public UPCItemDbClient(IHttpClientFactory httpFactory, ILogger<UPCItemDbClient> log)
        {
            _httpFactory = httpFactory;
            _log = log;
        }

        public async Task<string?> ResolveTitleAsync(string code, string? apiKey, CancellationToken ct)
        {
            string url = !string.IsNullOrWhiteSpace(apiKey)
                ? $"https://api.upcitemdb.com/prod/v1/lookup?upc={Uri.EscapeDataString(code)}&apikey={Uri.EscapeDataString(apiKey)}"
                : $"https://api.upcitemdb.com/prod/trial/lookup?upc={Uri.EscapeDataString(code)}";

            var http = _httpFactory.CreateClient();
            using var resp = await http.GetAsync(url, ct).ConfigureAwait(false);
            var body = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            _log.LogDebug("[UPCitemdb] {Status} body: {Body}", (int)resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode) return null;
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array || items.GetArrayLength() == 0)
                return null;

            var item = items[0];
            var title = item.TryGetProperty("title", out var tEl) ? tEl.GetString() : null;
            return string.IsNullOrWhiteSpace(title) ? null : title;
        }
    }
}
