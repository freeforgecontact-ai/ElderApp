# Ancrage — Premiers soins psychologiques

> **Outil de soutien et de bien-être uniquement. PAS un dispositif médical. Ne remplace ni un thérapeute ni une ligne de crise.**
> En cas d'urgence, composez le **988** (Canada) ou le **1-866-APPELLE** (1-866-277-3553).

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Fonctionne **100 % hors-ligne** après la première ouverture. Aucun serveur, aucun compte, aucune donnée transmise.

---

## Fonctionnalités

| Écran | Ce qu'on y fait |
|-------|----------------|
| **Accueil** | Point d'entrée calme, rappel du cadre, navigation vers les outils |
| **Ancrage** | Respiration 4-7-8 animée par minuterie + technique sensorielle 5-4-3-2-1 |
| **Mon plan de sécurité** | Modèle Stanley-Brown : signaux d'alerte, stratégies, personnes et professionnels, sécurisation de l'environnement — tout éditable et sauvegardé localement |
| **Journal d'humeur** | Échelle 1-5, note libre, graphique d'historique, export CSV |

**Bouton « Aide maintenant » (rouge) en permanence dans la barre du haut** : donne accès hors-ligne au 988, au 1-866-APPELLE, au texto 53 53 53, et à suicide.ca.

---

## Cadre responsable

- Positionnement explicite **« bien-être général »** : aucune fonction de diagnostic ou de traitement.
- Conçu pour rester **hors champ SaMD** (Santé Canada / Loi sur les dispositifs médicaux).
- **Zéro télémétrie** : les données de santé mentale restent sur l'appareil (localStorage), conformément à la **Loi 25** (Québec) et à la **LPRPDE**.
- Ressources de crise **toujours accessibles en un geste**, même en mode avion.

---

## Prérequis

- **.NET SDK 8** — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** (installé via `.config/dotnet-tools.json`)

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, sert l'app
```

Ouvre http://localhost:5173. Le script `predev` copie automatiquement `tokens.css` et `components.css` depuis `/design-system` vers `app/css`.

## Construire pour la production

```bash
npm run build              # -> dossier dist/ (statique)
npm run preview            # prévisualise
```

`dist/` est entièrement statique : déployable sur **GitHub Pages**, **Cloudflare Pages** ou **Netlify**. Aucun serveur requis.

## Structure

```
app/
  index.html                 point d'entrée + service worker
  vite.config.mjs            build statique
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie le design system partagé
  css/                       tokens.css + components.css (copiés) + app.css
  public/                    manifest, sw.js, icônes
  src/
    Domain.fs                types métier (Exercice, PlanSecurite, EntreeJournal, RessourceCrise)
    I18n.fs                  traductions FR
    Storage.fs               persistance localStorage (plan + journal)
    State.fs                 Model / Msg / update (Elmish MVU)
    View.fs                  interface Feliz (classes .sol- du design system)
    Main.fs                  amorçage Elmish + abonnements (réseau + minuterie)
    Ancrage.fsproj           projet Fable
```

## Points d'attention pour la production

- **Chiffrement** : les données de santé mentale (plan, journal) devraient être chiffrées (AES-GCM) avec un code choisi par l'usager — voir `00-FONDATIONS-COMMUNES.md §6`.
- **Révision clinique** : le contenu (ressources de crise, libellés du plan Stanley-Brown) doit être revu par des professionnel·le·s avant toute diffusion publique.
- **Révision juridique** : les mentions légales et le positionnement « bien-être » doivent être validés par un juriste en droit de la santé (Québec / Canada) avant lancement.
- **Export PDF du plan** : à implémenter (v1) pour partager le plan avec un intervenant.
- **Mode discret** : à implémenter (v1) pour les contextes sensibles.
