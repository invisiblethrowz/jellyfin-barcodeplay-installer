using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "BarcodePlay";
        public override string Description => "Scan DVD barcodes and play movies in Jellyfin";
        public Plugin(ILogger<Plugin> logger)
        {
            logger.LogInformation("BarcodePlay plugin loaded. Ready to scan.");
        }

        public PluginPageInfo[] GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "barcodeplay",
                    EmbeddedResourcePath = GetType().Namespace + ".AdminWeb.index.html"
                }
            };
        }
    }
}
