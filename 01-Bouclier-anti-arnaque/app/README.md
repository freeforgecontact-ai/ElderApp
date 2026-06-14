# Bouclier anti-arnaque — application

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Installable sur Mac, Windows, Linux et Android via le navigateur. Fonctionne **100 % hors-ligne** après la première ouverture.

## Prérequis

- **.NET SDK 8** (pour Fable) — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** : installé localement via l'outil .NET (voir ci-dessous)

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5 (.config/dotnet-tools.json)
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, sert l'app
```

Ouvre ensuite http://localhost:5173. Le script `predev` copie automatiquement `tokens.css` et `components.css` depuis `/design-system` vers `app/css` (source de vérité unique).

## Construire pour la production

```bash
npm run build              # -> dossier dist/ (statique, déployable partout)
npm run preview            # prévisualise le build
```

`dist/` est un site statique : héberge-le gratuitement sur **GitHub Pages**, **Cloudflare Pages** ou **Netlify**. Aucun serveur requis.

## Hors-ligne (PWA)

- `public/sw.js` met en cache l'app shell et sert tout en *cache-first*.
- `public/manifest.webmanifest` rend l'app installable (icône sur le bureau / l'écran d'accueil).
- Toutes les données (proche de confiance, etc.) restent **sur l'appareil** (localStorage ; à chiffrer en production, voir `00-FONDATIONS-COMMUNES.md` §6).

## Structure

```
app/
  index.html                 point d'entrée + enregistrement du service worker
  vite.config.mjs            build statique
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie le design system partagé
  css/                       tokens.css + components.css (copiés) + app.css
  public/                    manifest, service worker, icônes
  src/
    Domain.fs                types métier (états illégaux impossibles)
    I18n.fs                  traductions FR/EN
    Cafc.fs                  détection (échantillon — données ouvertes CAFC en prod)
    Storage.fs               persistance locale
    State.fs                 Model / Msg / update (logique pure)
    View.fs                  interface Feliz (design system .sol-)
    Main.fs                  amorçage Elmish + abonnement réseau
```

## Notes de production (voir PLAN.md)

- Brancher les **données ouvertes du CAFC** (numéros + typologies) en plus de l'échantillon embarqué.
- Chiffrer les données sensibles (AES-GCM).
- Volet **filtrage d'appels** en temps réel = extension native (Android `CallScreeningService` / iOS `Call Directory`), partageant ce noyau F#.

## Adapter ce gabarit à une autre app

Cette app sert de **patron** aux 7 autres : copie `app/`, renomme, garde `index.html`, `vite.config.mjs`, `sw.js`, `scripts/`, `css/`, et remplace le contenu de `src/` par le domaine de l'app.
