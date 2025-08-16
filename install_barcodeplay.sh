#!/bin/bash
set -e

echo "[*] Installing BarcodePlay plugin..."

WORKDIR="$HOME/.barcodeplay_build"
REPO="https://github.com/invisiblethrowz/jellyfin-barcodeplay-installer.git"
PLUGINDIR="/var/lib/jellyfin/plugins/Jellyfin.Plugin.BarcodePlay"

mkdir -p "$WORKDIR"
cd "$WORKDIR"

if [ ! -d repo ]; then
  git clone "$REPO" repo
else
  cd repo
  git pull
  cd ..
fi

cd repo

# Copy Jellyfin assemblies
mkdir -p lib
cp -v /usr/lib/jellyfin/*.dll lib/ || true

# Build plugin
dotnet build -c Release

# Install plugin
sudo mkdir -p "$PLUGINDIR"
sudo cp -v bin/Release/net8.0/Jellyfin.Plugin.BarcodePlay.dll "$PLUGINDIR"

# Restart Jellyfin
echo "[*] Restarting Jellyfin..."
sudo systemctl restart jellyfin

echo "[+] BarcodePlay installed successfully."
