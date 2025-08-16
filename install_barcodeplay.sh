#!/usr/bin/env bash
set -euo pipefail

PLUGIN_ASM="Jellyfin.Plugin.BarcodePlay"
PLUGIN_DIR="/var/lib/jellyfin/plugins/${PLUGIN_ASM}"
WORKDIR="${HOME}/.barcodeplay_build"
REPO_ARCHIVE_URL="https://github.com/invisiblethrowz/jellyfin-barcodeplay-installer/archive/refs/heads/main.zip"

echo "=== [BarcodePlay] Preparing build environment ==="
mkdir -p "$WORKDIR"
cd "$WORKDIR"

echo "=== [BarcodePlay] Ensuring system packages (.NET 8, unzip, curl) ==="
if ! command -v dotnet >/dev/null 2>&1; then
  sudo apt update
  sudo apt install -y dotnet-sdk-8.0
fi
sudo apt install -y unzip curl

echo "=== [BarcodePlay] Downloading latest repo archive ==="
rm -f repo.zip
curl -L "$REPO_ARCHIVE_URL" -o repo.zip

echo "=== [BarcodePlay] Unpacking source ==="
rm -rf repo
mkdir -p repo
unzip -q repo.zip -d repo
cd "$(find repo -maxdepth 1 -type d -name '*-main' | head -n1)"

echo "=== [BarcodePlay] Adding Jellyfin NuGet source (if missing) ==="
if ! dotnet nuget list source | grep -q "https://repo.jellyfin.org/releases/plugin/api/v3/nuget/index.json"; then
  dotnet nuget add source https://repo.jellyfin.org/releases/plugin/api/v3/nuget/index.json --name jellyfin
fi

echo "=== [BarcodePlay] Stopping Jellyfin service (if running) ==="
if systemctl list-units --type=service | grep -q jellyfin.service; then
  sudo systemctl stop jellyfin || true
fi

echo "=== [BarcodePlay] Restoring & building plugin ==="
dotnet restore Jellyfin.Plugin.BarcodePlay.csproj
dotnet build -c Release Jellyfin.Plugin.BarcodePlay.csproj

echo "=== [BarcodePlay] Installing plugin DLL ==="
sudo mkdir -p "$PLUGIN_DIR"
sudo cp -v bin/Release/net8.0/${PLUGIN_ASM}.dll "$PLUGIN_DIR"/

echo "=== [BarcodePlay] Restarting Jellyfin ==="
if systemctl list-units --type=service | grep -q jellyfin.service; then
  sudo systemctl restart jellyfin
  echo "[BarcodePlay] 🍿 Installed & ready! Open Jellyfin → Dashboard → Plugins → Barcode Play."
else
  echo "[BarcodePlay] Note: Jellyfin systemd service not found. Start Jellyfin manually to load the plugin."
fi
