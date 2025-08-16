namespace Jellyfin.Plugin.BarcodePlay
{
    public class TMDbClient
    {
        private readonly string _apiKey;
        public TMDbClient(string apiKey) { _apiKey = apiKey; }

        public string LookupMovie(string title)
        {
            // TODO: call TMDb API
            return title;
        }
    }
}
