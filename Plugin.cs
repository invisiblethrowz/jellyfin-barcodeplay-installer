using MediaBrowser.Common.Plugins;
using System.Collections.Generic;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Barcode Play";
        public override Guid Id { get; } = Guid.Parse("c3b769e0-1e2a-4f32-bf8b-4d6f4b7a9e6a");

        public Plugin(IApplicationPaths applicationPaths) : base(applicationPaths) { }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "barcodeplay.html",
                    EmbeddedResourcePath = GetType().Namespace + ".Web.config.html"
                },
                new PluginPageInfo
                {
                    Name = "barcodeplay.js",
                    EmbeddedResourcePath = GetType().Namespace + ".Web.config.js"
                }
            };
        }
    }
}
