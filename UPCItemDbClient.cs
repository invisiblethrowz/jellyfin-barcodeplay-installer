namespace Jellyfin.Plugin.BarcodePlay
{
    public class UPCItemDbClient
    {
        private readonly string _apiKey;
        public UPCItemDbClient(string apiKey) { _apiKey = apiKey; }

        public string LookupTitle(string barcode)
        {
            // TODO: call UPCitemdb API
            return "Stub Movie";
        }
    }
}
