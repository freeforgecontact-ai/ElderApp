# Mots du quotidien — application

PWA **voix-d'abord, offline-first** en **F# / Fable 5** (Elmish + Feliz). Installable sur Android, iOS et ordinateur. Fonctionne **100 % hors-ligne** après la première ouverture.

L'interface est pilotée par **pictogrammes + synthèse vocale** : aucun texte n'est exigé pour naviguer ou progresser. Aucune mention explicite de « lecture » ou d' « apprentissage ».

## Prérequis

- **.NET SDK 8** — https://dotnet.microsoft.com/download
- **Node.js 18+**
- **Fable 5** : installé localement via l'outil .NET (`.config/dotnet-tools.json`)

## Démarrer

```bash
cd app
dotnet tool restore        # installe Fable 5
npm install                # installe Vite
npm run dev                # copie le design system, compile F# -> JS, lance le dev server
```

Ouvre http://localhost:5173. Le script `predev` copie `tokens.css` et `components.css` depuis `/design-system` (source de vérité unique).

## Construire pour la production

```bash
npm run build              # -> dist/ (statique, déployable partout)
npm run preview            # prévisualise le build
```

`dist/` se déploie sans serveur sur **GitHub Pages**, **Cloudflare Pages** ou **Netlify**.

## Hors-ligne (PWA)

- `public/sw.js` précache l'app shell (`mots-v1`) et sert en *cache-first*.
- `public/manifest.webmanifest` rend l'app installable.
- Toute la progression est stockée **localement** (localStorage). Aucune donnée ne quitte l'appareil.

## Structure des fichiers

```
app/
  index.html                 point d'entrée + enregistrement du service worker
  vite.config.mjs            build statique
  .config/dotnet-tools.json  Fable 5
  scripts/copy-ds.mjs        copie le design system partagé
  css/
    tokens.css               design tokens (copié depuis /design-system)
    components.css           composants .sol- (copié depuis /design-system)
    app.css                  styles propres à l'app (préfixe .mots-)
  public/
    manifest.webmanifest     PWA installable
    sw.js                    service worker offline-first (cache mots-v1)
    icons/icon.svg           icône de l'app (plante / jardin)
  src/
    Domain.fs                types métier : Module, Etape, Jardin, catalogueModules
    I18n.fs                  libellés FR/EN (tous lisibles vocalement)
    Storage.fs               persistance locale [<Emit>] localStorage
    State.fs                 Model / Msg / update + interop Web Speech API
    View.fs                  interface Feliz (design system .sol- + .mots-)
    Main.fs                  amorçage Elmish + abonnement réseau
    Alpha.fsproj             projet F# (sans référence à Cafc)
```

## Feuille de route (voir PLAN.md)

- **v1** : reconnaissance vocale (l'apprenant répète le mot, l'app valide).
- **v1** : mode formateur RGPAQ (tableau de bord anonymisé).
- **v1** : remplacer les emojis par les pictos **ARASAAC** (API CC BY-NC-SA, mis en cache).
- **v1** : remplacer Web Speech API par **Coqui TTS** embarqué (hors-ligne réel, voix française).
- **v2** : parcours écriture (tracé de lettres).
- **v2** : banque de modules communautaire (formateurs contributeurs).
