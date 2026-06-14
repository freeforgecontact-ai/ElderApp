# Récolte — dons alimentaires

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Relie les surplus alimentaires aux organismes et aux personnes dans le besoin par le **don gratuit** — aucune transaction, aucune commission, aucun argent qui circule.

## Prérequis

- **.NET SDK 8** (pour Fable) — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** : installé localement via l'outil .NET (`.config/dotnet-tools.json`)

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
npm run build              # -> dossier dist/ (statique, déployable partout)
npm run preview            # prévisualise le build
```

## Hors-ligne (PWA)

- `public/sw.js` (cache `recolte-v1`) met en cache l'app shell en *cache-first*.
- `public/manifest.webmanifest` rend l'app installable.
- Toutes les données de session restent **sur l'appareil** (localStorage).

## Structure

```
app/
  index.html                 point d'entrée + service worker
  vite.config.mjs            build statique
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie tokens.css + components.css depuis /design-system
  css/app.css                styles propres à Récolte (classes .recolte-, .don-carte, etc.)
  public/                    manifest, sw.js, icône panier/feuille
  src/
    Domain.fs     types métier : ContrainteFroid, StatutDon, Don, Role, CategorieAliment
    I18n.fs       traductions FR/EN
    Exemples.fs   données d'exemple embarquées + helpers de tri/filtrage
    Storage.fs    persistance locale via [<Emit>] localStorage
    State.fs      Model / Msg / update (logique pure Elmish)
    View.fs       interface Feliz (design system .sol-)
    Main.fs       amorçage Elmish + abonnement réseau
    Recolte.fsproj
```

## Écrans

| Onglet | Rôle |
|--------|------|
| Accueil | Choix de rôle (premier lancement), raccourcis, compteur de dons dispo |
| Publier | Formulaire de don : catégorie, aliment, quantité, froid, lieu, fenêtre |
| Surplus | Liste filtrée/triée des surplus disponibles ; réservation et confirmation |
| Mes dons | Historique des dons publiés + exemples illustratifs |

## Notes de production (voir PLAN.md)

- **Back léger à brancher** (marqué `TODO` dans le code) : Cloudflare Workers + D1 ou Supabase — mise en relation temps réel, notifications push, géo-requêtes.
- **Open Food Network Canada** (AGPL) — synchronisation logistique.
- **Open Food Facts** — enrichissement par code-barres / catégories.
- **Annuaire des organismes** : Banques alimentaires du Québec / données Québec.
- Licence composante serveur : **AGPL-3.0**.
- Zéro donnée de bénéficiaire stockée ; accès anonyme pour recevoir.
