#!/usr/bin/env bash
set -e

GITHUB_USER="invisiblethrowz"
ZIP_URL="https://raw.githubusercontent.com/$GITHUB_USER/jellyfin-barcodeplay-installer/main/Jellyfin.Plugin.BarcodePlay-10.10.7.zip"
ZIP_FILE="$HOME/Jellyfin.Plugin.BarcodePlay-10.10.7.zip"
PLUGIN_NAME="BarcodePlay"
JELLYFIN_PLUGIN_DIR="/var/lib/jellyfin/plugins/$PLUGIN_NAME"

echo "=== Step 1: Installing prerequisites ==="
sudo apt update && sudo apt upgrade -y
sudo apt install -y wget curl git unzip

# Install Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET 8 SDK
sudo apt update
sudo apt install -y dotnet-sdk-8.0

echo "=== Step 2: Downloading plugin source ==="
wget -O "$ZIP_FILE" "$ZIP_URL"

echo "=== Step 3: Unpacking source ZIP ==="
unzip -o "$ZIP_FILE" -d "$HOME/Jellyfin.Plugin.BarcodePlay"
cd "$HOME/Jellyfin.Plugin.BarcodePlay/src"

echo "=== Step 4: Restoring and building ==="
dotnet restore
dotnet build -c Release

echo "=== Step 5: Installing into Jellyfin plugin folder ==="
sudo mkdir -p "$JELLYFIN_PLUGIN_DIR"
sudo cp bin/Release/net8.0/Jellyfin.Plugin.BarcodePlay.dll "$JELLYFIN_PLUGIN_DIR/"

echo "=== Step 6: Restarting Jellyfin ==="
sudo systemctl restart jellyfin

echo "=== Done! ==="
echo "Plugin installed. Open Jellyfin → Dashboard → Plugins → Barcode Play to set TMDb & UPCitemdb keys."
