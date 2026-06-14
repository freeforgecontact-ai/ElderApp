# Lien — aînés anti-isolement

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Mode kiosque ultra-simple, conçue pour des aînés peu à l'aise avec la technologie. Installable sur tablette ou téléphone. Fonctionne **100 % hors-ligne** après la première ouverture.

## Prérequis

- **.NET SDK 8** — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** : installé localement via `.config/dotnet-tools.json`

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, sert l'app
```

Ouvre ensuite http://localhost:5173.

## Construire pour la production

```bash
npm run build              # -> dist/ (statique)
npm run preview
```

## Fonctionnalités (MVP)

| Écran | Ce que fait l'aîné |
|-------|-------------------|
| Accueil | Un grand bouton « Je vais bien » — check-in du jour, confirmation visuelle |
| Appeler | Photo/initiale + prénom — un seul appui lance l'appel (`tel:`) |
| Humeur | 3 grands émojis : Bien / Comme ci comme ça / Pas fort |
| Réglages | Famille : contacts d'escalade, seuil d'inactivité (heures) |

## Structure

```
app/
  index.html                 point d'entrée (data-echelle="grande" pour AAA)
  vite.config.mjs
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie le design system partagé
  css/                       tokens.css + components.css + app.css
  public/                    manifest, service worker, icônes
  src/
    Domain.fs                types métier (CheckIn, Contact, RegleInactivite…)
    I18n.fs                  traductions (FR — v1 : EN)
    Storage.fs               persistance locale (localStorage, [<Emit>])
    State.fs                 Model / Msg / update (logique pure Elmish)
    View.fs                  interface Feliz (.sol-, kiosque grande échelle)
    Main.fs                  amorçage Elmish + abonnement réseau
    Lien.fsproj
```

## Notes de production

- **Repli SMS** : quand `navigator.onLine = false`, le check-in est sauvé localement. TODO (v1) : envoyer un SMS via passerelle à bas coût pour alerter la famille.
- **Vie privée / Loi 25** : les données de check-in sont des renseignements de santé. En production : chiffrement AES-GCM, consentement explicite, co-signature si besoin.
- **Inactivité** : la règle est stockée localement ; la détection se fait à l'ouverture de l'app. En v1 : service worker + notification push (si permission) ou SMS d'escalade.
- **Appel vidéo** (v1) : Jitsi Meet, salle pré-générée par contact, sans compte requis.
