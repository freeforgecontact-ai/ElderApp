# 00 — Fondations communes

Base technique, de conception et de gouvernance partagée par les 8 applications. Chaque `PLAN.md` y fait référence au lieu de répéter ces choix.

---

## 1. Principes d'architecture

- **Local-first / offline-first** : l'app fonctionne d'abord sur l'appareil ; le réseau est un bonus, jamais un prérequis pour l'essentiel.
- **Privacy-by-design** : pas de compte par défaut, données locales, chiffrement des données sensibles, zéro télémétrie.
- **Accessibilité dès la première ligne** : un composant inaccessible n'est pas « fini ».
- **Un seul langage, du domaine à l'écran** : F# partout, modèles typés partagés.
- **Coût marginal nul** : hébergement statique gratuit, aucune dépendance payante.

---

## 2. Stack technique

| Couche | Choix | Rôle |
|--------|-------|------|
| Langage | **F#** (.NET) | Logique métier et UI, fortement typé |
| Compilation | **Fable 5** | F# → JavaScript (ES2020), tree-shaking |
| Architecture UI | **Elmish** (Model-View-Update) | Flux d'état prévisible, unidirectionnel |
| Rendu | **Feliz** (DSL React en F#) | Composants React 18 écrits en F# |
| Build / dev | **Vite** + plugin Fable | Hot reload, bundle optimisé |
| Styles | **Design tokens CSS** + utilitaires maison | Léger, thémable, sans dépendance lourde |
| Persistance | **IndexedDB** (binding Fable / localForage) | Données structurées hors-ligne |
| Hors-ligne | **Service Worker** (Workbox) + manifest PWA | App installable, cache applicatif |
| Cryptographie | **WebCrypto** (AES-GCM, PBKDF2) | Chiffrement local des données sensibles |
| Voix | **Web Speech API** + repli **Coqui TTS** | Synthèse et reconnaissance vocale |
| i18n | Fichiers **JSON** + résolveur typé | FR/EN d'abord, extensible |
| Tests | **Fable.Mocha** / Expecto, **Playwright**, **axe-core** | Unitaire, E2E, accessibilité |
| CI/CD | **GitHub Actions** → Pages / Cloudflare | Build et déploiement automatiques |

---

## 3. Pourquoi F# + Fable 5

- **Modélisation par types** : les unions discriminées rendent les états illégaux *impossibles à compiler*. Moins de bugs dans des apps où une erreur peut blesser (santé, sécurité).
- **Un langage unique** : la même logique de validation tourne côté UI et, si besoin, côté serveur (.NET). Pas de duplication JS/back.
- **MVU/Elmish** : un état, des messages, une fonction `update` pure. Comportement reproductible, facile à tester et à déboguer.
- **Sortie web standard** : Fable produit du JavaScript ordinaire → PWA installable partout (Android, iOS, desktop), sans store ni frais.

Exemple — un état sensible qu'on ne peut pas lire par accident :

```fsharp
type DonnéeSensible =
    | Verrouillée
    | Déverrouillée of contenu: string

// Impossible d'accéder au `contenu` sans avoir géré le cas Verrouillée.
let afficher d =
    match d with
    | Verrouillée -> "•••"
    | Déverrouillée contenu -> contenu
```

---

## 4. Architecture applicative (MVU)

Chaque app suit le même squelette Elmish : un `Model` immuable, des `Msg`, une fonction `update` pure, une `view`.

```fsharp
type Model = { Étape: int; EnLigne: bool }

type Msg =
    | ÉtapeSuivante
    | RéseauChangé of bool

let init () = { Étape = 0; EnLigne = false }, Cmd.none

let update msg model =
    match msg with
    | ÉtapeSuivante -> { model with Étape = model.Étape + 1 }, Cmd.none
    | RéseauChangé b -> { model with EnLigne = b }, Cmd.none
```

Avantages : état unique sérialisable (sauvegarde/restauration triviale), logique testable sans navigateur, rendu découplé.

---

## 5. Hors-ligne et PWA

- **App shell precache** : HTML/JS/CSS et icônes mis en cache à l'installation (Workbox `precacheAndRoute`). L'app s'ouvre sans réseau.
- **Contenu** : stratégie *stale-while-revalidate* pour les ressources non critiques ; *cache-first* pour les données figées (pictogrammes, annuaires).
- **Données utilisateur** : IndexedDB, jamais le réseau par défaut.
- **File d'attente (outbox)** : quand une action a besoin du réseau (envoi d'un signalement, sync d'un don), elle est mise en file et rejouée au retour de la connexion (Background Sync).
- **Installation** : `manifest.webmanifest` complet (icônes maskables, `display: standalone`, langue, thème) → bouton « Installer » natif.

---

## 6. Sécurité et vie privée

**Modèle de menace générique** : appareil partagé ou perdu, réseau hostile, accès non désiré aux données (proche violent, tiers judiciaire, fraudeur).

- **Chiffrement local** : données sensibles chiffrées en **AES-GCM 256**, clé dérivée d'un NIP via **PBKDF2** (ou Argon2 si dispo). Rien en clair sur le disque.
- **Aucune télémétrie, aucun pisteur, aucune pub.** Pas de SDK tiers de collecte.
- **Pas de cloud par défaut** : si une sauvegarde est offerte, elle est chiffrée de bout en bout et déclenchée par l'usager.
- **Effacement** : suppression complète des données locales en une action (important pour les apps 2 et 7).
- **Conformité** : Loi 25 (Québec) et LPRPDE — minimisation, consentement explicite, droit d'accès et d'effacement, registre de traitement.
- **Audit** : pour toute app à risque (Sécurité VC), revue de sécurité indépendante avant publication.

---

## 7. Design system et accessibilité

Cible : **WCAG 2.2 AA** partout, **AAA** pour aînés et faible littératie.

- **Tokens** : contraste minimum 7:1, taille de police de base 18–20 px et ajustable, cibles tactiles ≥ 44×44 px, focus visible.
- **Composants partagés** (bibliothèque `Solidaire.UI`) :
  - `GrosBouton` — large, libellé + icône + couleur, lisible de loin.
  - `TexteVocal` — tout texte peut être lu à voix haute (Web Speech), bouton haut-parleur intégré.
  - `ÉtiquettePicto` — texte toujours doublé d'un pictogramme (ARASAAC) pour les publics non lecteurs.
  - `ChampSimple` — saisie tolérante, messages d'erreur clairs et parlés.
- **Navigation** : 100 % clavier et lecteur d'écran (NVDA, VoiceOver, TalkBack) ; ordre de tabulation logique ; `aria-*` systématique.
- **Modes** : clair / sombre / fort contraste ; option « interface simple » (moins d'éléments).
- **Voix** : sortie (TTS) et, pour l'alphabétisation, entrée (reconnaissance) ; repli local Coqui si pas de réseau.

---

## 8. Internationalisation

- Fichiers `i18n/<lang>.json` à plat, clés stables (`accueil.bouton.continuer`).
- Résolveur typé en F# : une clé inconnue est détectée à la compilation/au test, pas en production.
- Pluriels et genres gérés ; support **RTL** prévu (arabe, dari, ourdou — utile pour l'app 3).
- **FR et EN** au lancement ; langues d'immigration prioritaires ensuite (es, ar, fa, uk, zh).
- Traductions ouvertes à la contribution communautaire (fichiers texte simples, pas de code).

---

## 9. Données et intégrations communes

Module partagé `Solidaire.Data` pour ne pas réécrire la même plomberie :

- **Annuaire 211 Québec** : import des ressources communautaires par région, format pivot commun (réutilisé par apps 3, 6, 8).
- **Cache de jeux ouverts** : CAFC (fraude), IRCC (immigration), ARASAAC (pictos), Open Food Network — téléchargés, normalisés, mis en cache local, rafraîchis périodiquement en Wi-Fi.
- **Traduction hors-ligne** : LibreTranslate / Argos Translate auto-hébergeable (app 3), sans dépendance Google/DeepL.
- **Format pivot** : toutes les ressources externes sont converties dans un schéma interne stable, pour isoler l'app des changements de format des sources.

---

## 10. Hébergement et coûts

| Besoin | Solution gratuite | Coût |
|--------|-------------------|------|
| App PWA statique | GitHub Pages / Cloudflare Pages / Netlify | 0 $ |
| Données ouvertes | Fichiers en cache (pas de serveur) | 0 $ |
| Backend léger (app 6, relais) | Cloudflare Workers + D1, ou Supabase (palier gratuit) | 0 $ jusqu'à l'échelle |
| Traduction offline | Argos embarqué côté client | 0 $ |
| Nom de domaine | Optionnel | ~15 $/an |

La règle : **pas de backend tant que ce n'est pas indispensable.** 6 des 8 apps peuvent vivre 100 % côté client.

---

## 11. Qualité et CI/CD

- **Unitaire** : logique `update` et validations testées en F# (Fable.Mocha / Expecto).
- **E2E** : Playwright sur les parcours critiques (onboarding, action principale, effacement).
- **Accessibilité** : axe-core en CI ; échec du build si régression a11y.
- **PWA / perf** : Lighthouse en CI (score PWA et offline vérifiés).
- **Pipeline** : GitHub Actions → tests → build Fable/Vite → déploiement Pages. Releases versionnées, journal de modifications.

---

## 12. Gouvernance open source

- **Licences** : MIT (apps clientes) ; AGPL-3.0 (apps à serveur) ; contenus et pictos sous CC adaptée.
- **Dépôt** : mono-repo `apps-solidaires` avec un dossier par app + paquets partagés (`Solidaire.UI`, `Solidaire.Data`, `Solidaire.Crypto`).
- **Contribution** : `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, gabarits d'issues, étiquette « good first issue ».
- **Fiabilité du contenu** : chaque app à contenu sensible (droits, ressources, santé) est adossée à un partenaire du milieu qui valide et met à jour ; versionnage daté des contenus.
- **Durabilité** : dons (open collective), subventions (fondations, programmes publics d'inclusion numérique), bénévolat encadré. Objectif : aucune dépendance à un acteur unique.

---

## 13. Squelette réutilisable d'une app

```
app-xxx/
  src/
    Domain.fs        // types métier (modèle, messages)
    State.fs         // init + update (logique pure)
    View.fs          // Feliz/Elmish
    Storage.fs       // IndexedDB + chiffrement
    I18n.fs          // résolveur de traductions
  public/
    manifest.webmanifest
    sw.js            // Service Worker (Workbox)
    icons/
  i18n/ fr.json en.json
  tests/
  README.md
```

Démarrage type : `dotnet tool restore` puis `npm install` puis `npm run dev` (Vite + Fable). Déploiement : `npm run build` → dossier statique publié.

> Ce socle est volontairement simple et durable : peu de dépendances, des standards web, un langage typé. C'est ce qui permet à une petite équipe de maintenir 8 apps sans budget.