using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string TMDbApiKey { get; set; } = "demo-tmdb-key";
        public string UPCItemDbApiKey { get; set; } = "demo-upcitemdb-key";
        public string? DefaultSessionId { get; set; }
        public bool PreferExactTitleMatch { get; set; } = true;
        public bool HasSeenConfigOnce { get; set; } = false;
    }
}
