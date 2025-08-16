using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BarcodePlay
{
    [ApiController]
    [Route("BarcodePlay")]
    public class BarcodePlayController : ControllerBase
    {
        private readonly Plugin _plugin;
        private readonly UPCItemDbClient _upc;
        private readonly TMDbClient _tmdb;
        private readonly ILibraryManager _library;
        private readonly ISessionManager _sessions;
        private readonly ILogger<BarcodePlayController> _log;

        public BarcodePlayController(
            Plugin plugin,
            UPCItemDbClient upc,
            TMDbClient tmdb,
            ILibraryManager library,
            ISessionManager sessions,
            ILogger<BarcodePlayController> log)
        {
            _plugin = plugin;
            _upc = upc;
            _tmdb = tmdb;
            _library = library;
            _sessions = sessions;
            _log = log;
        }

        public record ScanRequest([Required] string Code, string? SessionId);
        public record ScanResponse(string Status, string? ItemId = null, string? ItemName = null, string? Message = null, string? Title = null, int? Year = null);

        [HttpPost("scan")]
        public async Task<ActionResult<ScanResponse>> Scan([FromBody] ScanRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ScanResponse("error", Message: "Invalid payload"));
            _log.LogInformation("[Scan] Received barcode {Code}", req.Code);

            var cfg = _plugin.Configuration;
            var upcTitle = await _upc.ResolveTitleAsync(req.Code, cfg.UPCItemDbApiKey, ct);
            if (string.IsNullOrWhiteSpace(upcTitle))
            {
                _log.LogWarning("[Scan] No UPC match for {Code}", req.Code);
                return NotFound(new ScanResponse("not_found", Message: "Barcode not recognized."));
            }

            var (title, year) = await _tmdb.RefineAsync(upcTitle!, cfg.TMDbApiKey, ct);
            title ??= upcTitle;

            var movie = _library.GetItemList(new InternalItemsQuery { IncludeItemTypes = new[] { nameof(Movie) }, Recursive = true })
                                .OfType<Movie>()
                                .Select(m => new { m, score = Score(m, title!, year, cfg.PreferExactTitleMatch) })
                                .OrderByDescending(x => x.score)
                                .FirstOrDefault()?.m;

            if (movie is null)
            {
                _log.LogWarning("[Scan] No library match for '{Title}' ({Year})", title, year);
                return NotFound(new ScanResponse("not_found", Message: $"No library match for '{title}' ({year?.ToString() ?? "n/a"})", Title: title, Year: year));
            }

            var session = string.IsNullOrWhiteSpace(req.SessionId) ? cfg.DefaultSessionId : req.SessionId;
            var ok = await PlayAsync(movie.Id, session, ct);
            if (!ok)
            {
                return StatusCode(409, new ScanResponse("no_session", movie.Id.ToString("N"), movie.Name, "No active/selected session to play on", Title: title, Year: year));
            }

            return Ok(new ScanResponse("playing", movie.Id.ToString("N"), movie.Name, Title: title, Year: year));
        }

        private int Score(Movie m, string title, int? year, bool preferExact)
        {
            int s = 0;
            if (preferExact)
            {
                if (string.Equals(m.Name?.Trim(), title.Trim(), StringComparison.OrdinalIgnoreCase)) s += 100;
            }
            if (m.Name?.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0) s += 50;
            if (year.HasValue && m.ProductionYear == year) s += 25;
            return s;
        }

        private async Task<bool> PlayAsync(Guid itemId, string? sessionId, CancellationToken ct)
        {
            var session = sessionId != null
                ? _sessions.Sessions.FirstOrDefault(s => string.Equals(s.Id, sessionId, StringComparison.OrdinalIgnoreCase))
                : _sessions.Sessions.OrderByDescending(s => s.LastActivityDate).FirstOrDefault();

            if (session is null)
            {
                _log.LogWarning("[Play] No active session found (requested SessionId={Req})", sessionId);
                return false;
            }

            var request = new MediaBrowser.Controller.Session.PlaybackRequest
            {
                ItemIds = new[] { itemId },
                StartPositionTicks = 0,
                PlayCommand = MediaBrowser.Controller.Session.PlayCommand.PlayNow
            };

            _log.LogInformation("[Play] Sending play for item {Item} to session {Session}", itemId, session.Id);
            await _sessions.SendPlayCommand(session, request, ct).ConfigureAwait(false);
            return true;
        }
    }
}
