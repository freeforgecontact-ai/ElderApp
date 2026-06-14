# Second départ — application

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Compagnon de poche pour les personnes qui sortent de détention, d'un refuge ou d'une thérapie. Fonctionne **100 % hors-ligne** après la première ouverture.

## Prérequis

- **.NET SDK 8** — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** installé localement via l'outil .NET

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5 (.config/dotnet-tools.json)
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, sert l'app
```

Ouvre http://localhost:5173. Le script `predev` copie `tokens.css` et `components.css` depuis `/design-system` vers `app/css`.

## Construire pour la production

```bash
npm run build              # -> dossier dist/ (statique, déployable partout)
npm run preview            # prévisualise le build
```

`dist/` est un site statique : héberge-le sur GitHub Pages, Cloudflare Pages ou Netlify. Aucun serveur requis.

## Structure

```
app/
  index.html                 point d'entrée + enregistrement du service worker
  vite.config.mjs            build statique
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie le design system partagé
  css/                       tokens.css + components.css (copiés) + app.css
  public/                    manifest, service worker (depart-v1), icônes
  src/
    Domain.fs                types métier : Situation, Etape, Ressource,
                             ContactUrgence, Rappel + données initiales
    I18n.fs                  traductions FR/EN
    Storage.fs               persistance locale (localStorage, sans compte)
    State.fs                 Model / Msg / update (logique pure, Elmish)
    View.fs                  interface Feliz (design system .sol- / .depart-)
    Main.fs                  amorçage Elmish + abonnement réseau
    Depart.fsproj            projet F# (Cafc.fs exclu)
```

## Écrans

| Onglet | Description |
|--------|-------------|
| Accueil | Choix de situation (Détention / Refuge / Dépendance) + raccourcis |
| 48 h | Checklist des premières 48 heures, filtrée selon la situation |
| Parcours | Étapes séquentielles par catégorie (papiers, logement, santé…) avec progression |
| Ressources | Annuaire hors-ligne (211, ASRSQ, AA, John Howard, etc.) avec filtre texte |
| Contacts | Contacts d'urgence personnels + rappels (intervenant·e, probation, médecin) |

## Données et confidentialité

- Toutes les données restent sur l'appareil (localStorage).
- Aucun compte ni connexion requis.
- **TODO production** : chiffrer les contacts et rappels (AES-GCM) — ces données peuvent être sensibles si un tiers accède à l'appareil (probation, employeur).
- **TODO contenu** : enrichir l'annuaire avec les données 211 Québec (donneesquebec.ca), validées par ASRSQ / YMCA-Espadrille.

## Partenaires cibles

ASRSQ · YMCA-Espadrille · Société John Howard · 211 Québec · Maisons de transition
