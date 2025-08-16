#!/bin/bash
set -e

BUILD_DIR="$HOME/.barcodeplay_build"
REPO_DIR="$BUILD_DIR/repo"
PLUGIN_DIR="/var/lib/jellyfin/plugins/BarcodePlay"
JELLYFIN_LIB="/usr/lib/jellyfin/bin"

echo "[*] Cleaning build dir"
rm -rf "$BUILD_DIR"
mkdir -p "$REPO_DIR"

echo "[*] Cloning repo"
git clone https://github.com/invisiblethrowz/jellyfin-barcodeplay-installer.git "$REPO_DIR"

cd "$REPO_DIR"

echo "[*] Copying Jellyfin assemblies from $JELLYFIN_LIB"
mkdir -p lib
cp "$JELLYFIN_LIB"/MediaBrowser.*.dll lib/ || true
cp "$JELLYFIN_LIB"/Microsoft.Extensions.Logging*.dll lib/ || true


echo "[*] Building plugin"
dotnet build -c Release

echo "[*] Installing plugin"
sudo mkdir -p "$PLUGIN_DIR"
sudo cp "$REPO_DIR/bin/Release/net8.0/Jellyfin.Plugin.BarcodePlay.dll" "$PLUGIN_DIR/"

echo "[*] Restarting Jellyfin"
sudo systemctl restart jellyfin

echo "[✓] Installed Jellyfin.Plugin.BarcodePlay"
