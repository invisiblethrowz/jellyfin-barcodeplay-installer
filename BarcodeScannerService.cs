using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class BarcodeScannerService
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger<BarcodeScannerService> _logger;

        public BarcodeScannerService(ISessionManager sessionManager, ILibraryManager libraryManager, ILogger<BarcodeScannerService> logger)
        {
            _sessionManager = sessionManager;
            _libraryManager = libraryManager;
            _logger = logger;
        }

        public async Task ScanAsync(string barcode, string? sessionId = null)
        {
            _logger.LogInformation("BarcodePlay: Scanned barcode {Barcode}", barcode);
            // Lookup stub (connect UPCItemDb → TMDb pipeline here)
            var title = "Stub Movie";
            var item = _libraryManager.GetItemList(new MediaBrowser.Controller.Entities.InternalItemsQuery { Name = title });
            if (item.Count > 0)
            {
                var targetSession = string.IsNullOrEmpty(sessionId)
                    ? _sessionManager.Sessions[0]
                    : _sessionManager.Sessions.Find(s => s.Id.ToString() == sessionId);

                if (targetSession != null)
                {
                    await _sessionManager.SendPlayCommand(targetSession.Id, item[0]);
                    _logger.LogInformation("BarcodePlay: Played {Title} for session {Session}", title, targetSession.Id);
                }
            }
            else
            {
                _logger.LogWarning("BarcodePlay: Could not find movie {Title}", title);
            }
        }
    }
}
