#!/usr/bin/env bash
set -euo pipefail

PLUGIN_ASM="Jellyfin.Plugin.BarcodePlay"
PLUGIN_DIR="/var/lib/jellyfin/plugins/${PLUGIN_ASM}"

echo "=== [BarcodePlay] Stopping Jellyfin ==="
if systemctl list-units --type=service | grep -q jellyfin.service; then
  sudo systemctl stop jellyfin || true
fi

echo "=== [BarcodePlay] Removing plugin folder ==="
sudo rm -rf "$PLUGIN_DIR"

echo "=== [BarcodePlay] Restarting Jellyfin ==="
if systemctl list-units --type=service | grep -q jellyfin.service; then
  sudo systemctl start jellyfin || true
  echo "[BarcodePlay] 🍿 Uninstalled successfully."
else
  echo "[BarcodePlay] Note: Jellyfin systemd service not found. Plugin files removed."
fi
