#!/bin/bash
set -e

PLUGINDIR="/var/lib/jellyfin/plugins/Jellyfin.Plugin.BarcodePlay"

echo "[*] Removing BarcodePlay plugin..."
sudo rm -rf "$PLUGINDIR"

echo "[*] Restarting Jellyfin..."
sudo systemctl restart jellyfin

echo "[+] BarcodePlay removed."
