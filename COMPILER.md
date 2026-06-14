# Compiler et lancer les applications

> **Statut de validation** : les 8 applications compilent avec **Fable 5.2** (testé une à une). Les apps **01 (Bouclier)** et **06 (Récolte)** ont été buildées en **PWA complète** (`dist/`) de bout en bout. Les 6 autres partagent exactement la même configuration de build.

## Prérequis (versions validées)

- **.NET SDK 10** — Fable 5.2 est un outil .NET 10. (Le SDK 8 seul ne suffit pas pour lancer Fable 5.)
- **Node.js 18+** (validé avec Node 22).
- **Fable 5.2** — déjà épinglé dans chaque `app/.config/dotnet-tools.json` ; `dotnet tool restore` l'installe.

## Lancer une app en développement

```bash
cd 01-Bouclier-anti-arnaque/app
dotnet tool restore      # installe Fable 5.2
npm install              # installe react, react-dom, vite
npm run dev              # compile F# -> JS et sert sur http://localhost:5173
```

`npm run dev` exécute `dotnet fable watch src --run vite` (le script `predev` copie d'abord le design system partagé dans `app/css`).

## Build de production (PWA installable)

```bash
npm run build            # -> dossier dist/  (dotnet fable src && vite build)
npm run preview          # prévisualise le build
```

`dist/` est un site **statique** : déploie-le gratuitement sur **GitHub Pages**, **Cloudflare Pages** ou **Netlify**. Aucun serveur requis. L'app est installable (PWA) et fonctionne **hors-ligne** (service worker + manifest).

Exemple de sortie de build validée (app 1) :

```
dist/index.html                 1.11 kB
dist/assets/index-*.css        13.8 kB   (design system)
dist/assets/index-*.js        189.7 kB   (Fable + Elmish + Feliz + React)
dist/manifest.webmanifest, dist/sw.js, dist/icons/
```

## Dépendances de chaque app

| Paquet | Version | Rôle |
|--------|---------|------|
| Fable | 5.2.0 (outil .NET) | F# → JavaScript |
| Fable.Core | 5.0.0 | runtime Fable |
| Feliz | 3.3.3 | DSL React en F# |
| Fable.Elmish | 5.0.2 | architecture MVU |
| Fable.Elmish.React | 5.6.0 | rendu React |
| react / react-dom | 18.3.1 | requis par Elmish.React |
| vite | 5.4.x | bundler / serveur de dev |

## Construire toutes les apps d'un coup

```bash
for d in 0*-*/app; do ( cd "$d" && dotnet tool restore && npm install && npm run build ); done
```

## Restes mineurs

- App **06 (Récolte)** : un avertissement CSS bénin (`app.css`) au build — non bloquant, à nettoyer à l'occasion.
- Les `TargetFramework` des `.fsproj` sont en `net8.0` (sert au restore NuGet) ; Fable transpile en JS, donc cela n'affecte pas l'exécution. Le **runtime .NET 10** reste nécessaire pour lancer l'outil Fable.
