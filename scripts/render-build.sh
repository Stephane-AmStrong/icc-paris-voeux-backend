#!/usr/bin/env bash
# Installer les dépendances requises
apt-get update && apt-get install -y \
    wget \
    fonts-liberation \
    libasound2 \
    libnss3 \
    libxss1 \
    libappindicator3-1 \
    xdg-utils \
    chromium-browser

# Lancer l'installation des packages Node.js
npm install
npm run build
