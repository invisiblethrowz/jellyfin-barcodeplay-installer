# Jellyfin.Plugin.BarcodePlay (for Jellyfin 10.10.7)

Scan a DVD/Blu-ray UPC/EAN → resolve Title/Year (UPCitemdb → TMDb) → find and play in your Jellyfin library.

- Admin UI: Jellyfin Dashboard → Plugins → **Barcode Play**
- API endpoint: `POST /BarcodePlay/scan` with JSON `{ "code": "012345678905", "sessionId": "optional-session-id" }`
- Startup log: `[BarcodePlay] 🍿 Running with demo API keys — please update them in plugin settings for full movie magic!`

## One-liner install (Ubuntu 24.04+)
```bash
bash <(curl -s https://raw.githubusercontent.com/invisiblethrowz/jellyfin-barcodeplay-installer/main/install_barcodeplay.sh)
```

## Uninstall
```bash
bash <(curl -s https://raw.githubusercontent.com/invisiblethrowz/jellyfin-barcodeplay-installer/main/uninstall_barcodeplay.sh)
```

## Build manually
```bash
dotnet nuget add source https://repo.jellyfin.org/releases/plugin/api/v3/nuget/index.json --name jellyfin
dotnet restore Jellyfin.Plugin.BarcodePlay.csproj
dotnet build -c Release Jellyfin.Plugin.BarcodePlay.csproj
sudo mkdir -p /var/lib/jellyfin/plugins/Jellyfin.Plugin.BarcodePlay
sudo cp bin/Release/net8.0/Jellyfin.Plugin.BarcodePlay.dll /var/lib/jellyfin/plugins/Jellyfin.Plugin.BarcodePlay/
sudo systemctl restart jellyfin
```
