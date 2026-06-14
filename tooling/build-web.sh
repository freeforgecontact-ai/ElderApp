#!/usr/bin/env bash
# Compile les 7 apps publiques en web statique (Fable + Vite, chemins relatifs)
# vers web/<slug>/, pret a etre empaquete en Electron/Capacitor. (App 02 exclue.)
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$ROOT/web"
rm -rf "$OUT"; mkdir -p "$OUT"

# slug|dossier|titre
apps=(
  "bouclier|01-Bouclier-anti-arnaque|Bouclier anti-arnaque"
  "boussole|03-Boussole-nouveaux-arrivants|Boussole"
  "lien|04-Aines-anti-isolement|Lien"
  "mots|05-Alphabetisation-adulte|Mots du quotidien"
  "recolte|06-Anti-gaspillage-alimentaire|Recolte"
  "ancrage|07-Premiers-soins-psychologiques|Ancrage"
  "depart|08-Reinsertion-second-depart|Second depart"
)

for entry in "${apps[@]}"; do
  IFS='|' read -r slug dir title <<< "$entry"
  echo "=== build web $slug ($dir) ==="
  pushd "$ROOT/$dir/app" >/dev/null
  cat > index.html <<HTML
<!DOCTYPE html>
<html lang="fr-CA">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover">
<meta name="theme-color" content="#0F4C81">
<title>$title</title>
<link rel="stylesheet" href="/css/tokens.css">
<link rel="stylesheet" href="/css/components.css">
<link rel="stylesheet" href="/css/app.css">
</head>
<body>
<a class="sol-skip" href="#contenu">Aller au contenu</a>
<div id="app" class="sol-app"></div>
<script type="module" src="/src/Main.fs.js"></script>
</body>
</html>
HTML
  dotnet tool restore
  npm install --no-audit --no-fund
  node scripts/copy-ds.mjs
  dotnet fable src
  npx vite build --base=./ --outDir "$OUT/$slug"
  popd >/dev/null
done
echo ">>> web build termine : $(ls "$OUT")"
