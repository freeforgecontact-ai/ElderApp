# Sécurité (violence conjugale) — application

> **AVERTISSEMENT CRITIQUE — lire avant tout.**
> Ce scaffold est une base de départ technique. Il ne doit **pas** être diffusé
> tel quel. Voir les TODO de sécurité ci-dessous et `PLAN.md §9`.

PWA **offline-first** en **F# / Fable 5** (Elmish + Feliz). Installable via le navigateur. Fonctionne **100 % hors-ligne** après la première ouverture.

## Mission

Offrir aux personnes victimes de violence conjugale un outil discret pour :
- préparer un plan de sortie (checklist personnalisable),
- accéder aux ressources québécoises hors-ligne (SOS VC, maisons d'hébergement),
- régler un nom et une icône discrets (camouflage),
- effacer toutes les données en quelques secondes.

## Sécurité — TODO obligatoires avant toute diffusion

| Priorité | TODO |
|----------|------|
| CRITIQUE | Chiffrement AES-GCM 256 de toutes les données stockées (clé dérivée d'un NIP via PBKDF2/Argon2) |
| CRITIQUE | Hachage cryptographique du NIP (PBKDF2/Argon2 — marqué TODO dans Storage.fs et State.fs) |
| CRITIQUE | Audit de sécurité indépendant (externe) avant toute diffusion publique |
| CRITIQUE | Co-conception et revue par des intervenantes spécialisées en violence conjugale |
| Élevée   | Le camouflage PWA ne masque pas l'historique du navigateur — informer l'usagère |
| Élevée   | Effacement rapide : évaluer l'ajout d'un geste (secouer l'appareil) via l'API DeviceMotion |
| Élevée   | Journalisation d'incidents chiffrée + horodatage chaîné (module Preuve, v1) |

## Prérequis

- **.NET SDK 8** — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** (installé localement via l'outil .NET)

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5 (.config/dotnet-tools.json)
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, sert l'app
```

Ouvre http://localhost:5173.

## Construire pour la production

```bash
npm run build              # -> dossier dist/ (statique)
npm run preview            # prévisualise le build
```

## Hors-ligne (PWA)

- `public/sw.js` — cache `securite-v1`, app shell complet en *cache-first*.
- `public/manifest.webmanifest` — nom discret « Notes perso » par défaut.
- Données 100 % locales (localStorage). Aucune sortie réseau sauf si l'usagère clique sur un lien externe.

## Structure des fichiers source

```
src/
  Domain.fs     Types : EtapeSortie, Ressource, Camouflage, TypeIncident
  I18n.fs       Traductions FR (+ EN stub)
  Storage.fs    Persistance localStorage — TODO : chiffrer
  State.fs      Model / Msg / update (Elmish MVU, logique pure)
  View.fs       Interface Feliz (classes .sol- du design system)
  Main.fs       Amorçage Elmish + abonnement réseau
  Securite.fsproj
```

## Onglets MVP

| Onglet | Contenu |
|--------|---------|
| Accueil | Accès rapide à SOS VC + progression du plan |
| Mon plan | Checklist en 5 catégories, barre de progression, persistante |
| Aide | 7 ressources QC hors-ligne, filtrables par région |
| Réglages | Camouflage (nom + icône), NIP (stub), effacement rapide |

## Ressources de référence

- SOS violence conjugale : https://www.sosviolenceconjugale.ca/
- Regroupement des maisons : https://maisons-femmes.qc.ca/
- Tech Safety Canada (apps de sécurité) : https://techsafety.ca/
