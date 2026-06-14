# Boussole nouveaux arrivants — application

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

Ouvre ensuite http://localhost:5173. Le script `predev` copie automatiquement `tokens.css` et `components.css` depuis `/design-system` vers `app/css`.

## Construire pour la production

```bash
npm run build              # -> dossier dist/ (statique, déployable partout)
npm run preview            # prévisualise le build
```

## Fonctionnalités MVP

| Écran | Description |
|-------|-------------|
| **Accueil / Mon statut** | Sélection du statut parmi 6 catégories ; description et guide personnalisé |
| **Ma checklist** | Étapes filtrées par statut et horizon (J+1, J+7, 1 mois, 3 mois, 6 mois) ; cochables hors-ligne ; barre de progression |
| **Annuaire 211** | Ressources filtrées par catégorie (hébergement, aide alimentaire, francisation, aide juridique) ; liens téléphoniques directs |
| **Courriers IRCC** | Sélection du type de lettre reçue ; explication en langage simple ; renvoi à l'aide juridique |

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
    Domain.fs                types métier (Statut, Etape, Ressource211, TypeCourrier)
    I18n.fs                  traductions FR/EN (RTL prévu en v1)
    Cafc.fs                  données embarquées : checklist + annuaire 211
    Storage.fs               persistance locale (statut choisi, étapes cochées)
    State.fs                 Model / Msg / update (logique pure)
    View.fs                  interface Feliz (design system .sol-)
    Main.fs                  amorçage Elmish + abonnement réseau
```

## Vie privée

Tout reste sur l'appareil de l'utilisateur. Aucun profil centralisé. Conforme au principe **privacy-by-design** (critique pour les demandeurs d'asile). Voir `00-FONDATIONS-COMMUNES.md` §6 pour le chiffrement AES-GCM en production.

## Notes de production (voir PLAN.md)

- Connecter l'**API 211 Québec** (données ouvertes) pour un annuaire dynamique et géolocalisé.
- Ajouter les langues additionnelles (EN, ar, fa, uk, es, zh) et le support RTL.
- Implémenter la **traduction offline** de documents (LibreTranslate / Argos Translate + OCR Tesseract WASM) — v1.
- Rappels d'échéances locaux (via Notifications API) — v2.
- Chiffrer les données locales (AES-GCM).
