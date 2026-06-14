#!/usr/bin/env bash
# Build des 8 PWA pour GitHub Pages (chaque app sous /<NN>/, chemins relatifs).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$ROOT/_site"
rm -rf "$OUT"; mkdir -p "$OUT"

apps=(
  "01-Bouclier-anti-arnaque|01|Bouclier anti-arnaque"
  # "02-Securite-violence-conjugale|02|Notes perso"   # RETIREE du public : audit de securite en cours
  "03-Boussole-nouveaux-arrivants|03|Boussole"
  "04-Aines-anti-isolement|04|Lien"
  "05-Alphabetisation-adulte|05|Mots du quotidien"
  "06-Anti-gaspillage-alimentaire|06|Récolte"
  "07-Premiers-soins-psychologiques|07|Ancrage"
  "08-Reinsertion-second-depart|08|Second départ"
)

for entry in "${apps[@]}"; do
  IFS='|' read -r dir n title <<< "$entry"
  echo "=== build $dir ($n) ==="
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
  npx vite build --base=./ --outDir "$OUT/$n"
  popd >/dev/null
done

# Page d'accueil + feuilles partagees
cp "$ROOT/design-system/tokens.css" "$OUT/tokens.css"
cp "$ROOT/design-system/components.css" "$OUT/components.css"
cp "$ROOT/pages/index.html" "$OUT/index.html"
echo ">>> build termine : $(ls "$OUT")"
