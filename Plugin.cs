using System;
using System.Collections.Generic;
using Jellyfin.Plugin.BarcodePlay.Web;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BarcodePlay
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IPluginServiceRegistrator, IServerEntryPoint
    {
        private readonly ILogger<Plugin> _log;
        public static Plugin Instance { get; private set; } = null!;

        public override string Name => "Barcode Play";
        public override Guid Id { get; } = Guid.Parse("c3b769e0-1e2a-4f32-bf8b-4d6f4b7a9e6a");

        public Plugin(IApplicationPaths applicationPaths, ILogger<Plugin> logger) : base(applicationPaths)
        {
            _log = logger;
            Instance = this;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<UPCItemDbClient>();
            services.AddSingleton<TMDbClient>();
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "barcodeplay.html",
                    EmbeddedResourcePath = "Jellyfin.Plugin.BarcodePlay.AdminWeb.config.html"
                },
                new PluginPageInfo
                {
                    Name = "barcodeplay.js",
                    EmbeddedResourcePath = "Jellyfin.Plugin.BarcodePlay.AdminWeb.config.js"
                }
            };
        }

        public void Run()
        {
            if (string.Equals(Configuration.TMDbApiKey, "demo-tmdb-key", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Configuration.UPCItemDbApiKey, "demo-upcitemdb-key", StringComparison.OrdinalIgnoreCase))
            {
                _log.LogWarning("[BarcodePlay] 🍿 Running with demo API keys — please update them in plugin settings for full movie magic!");
            }
            else
            {
                _log.LogInformation("[BarcodePlay] Plugin loaded. Ready to scan!");
            }
        }

        public void Dispose() { }
    }
}
