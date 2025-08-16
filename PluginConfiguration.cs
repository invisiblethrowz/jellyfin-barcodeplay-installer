using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string UPCItemDbApiKey { get; set; } = "demo-upc-key";
        public string TMDbApiKey { get; set; } = "demo-tmdb-key";
    }
}
