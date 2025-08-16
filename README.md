# Jellyfin Barcode Play Plugin

A Jellyfin plugin to scan DVD barcodes and play movies from your library automatically.

## Features
- Scan UPC/EAN barcode and auto-play movie.
- UPCitemdb → TMDb lookup pipeline.
- Admin UI with API key configuration.
- Logging on startup and scan events.

## Installation (Ubuntu 24.04 / Jellyfin 10.10.7)

Run this one-liner:

```bash
bash <(curl -sL https://github.com/invisiblethrowz/jellyfin-barcodeplay-installer/archive/refs/heads/main.zip | busybox unzip -p - jellyfin-barcodeplay-installer-main/install_barcodeplay.sh)
