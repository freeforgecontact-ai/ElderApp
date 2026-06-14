# Handoff — Apps-Solidaires

Document de transfert complet : ce qui existe, comment chaque application fonctionne, comment la lancer, et ce qui reste à faire. Tout le code est dans `D:\Apps-Solidaires`.

---

## 1. Ce que c'est

Un portfolio de **8 applications libres, gratuites et hors-ligne** à fort impact social (Québec/Canada d'abord, multilingue à terme). Chacune est une **PWA** (application web installable) écrite en **F#** compilé en JavaScript par **Fable 5.2**, avec l'architecture **Elmish** (Model-View-Update) et l'interface **Feliz** (React en F#). Toutes partagent un **design system** unique aux couleurs de `ia.pgrg.ca`.

| # | Dossier | Nom de travail | Public |
|---|---------|----------------|--------|
| 1 | `01-Bouclier-anti-arnaque` | Bouclier | Aînés, proches aidants |
| 2 | `02-Securite-violence-conjugale` | Notes / Sécurité | Victimes de violence conjugale |
| 3 | `03-Boussole-nouveaux-arrivants` | Boussole | Immigrants, réfugiés |
| 4 | `04-Aines-anti-isolement` | Lien | Aînés isolés et proches |
| 5 | `05-Alphabetisation-adulte` | Mots du quotidien | Adultes à faible littératie |
| 6 | `06-Anti-gaspillage-alimentaire` | Récolte | Commerces, organismes, citoyens |
| 7 | `07-Premiers-soins-psychologiques` | Ancrage | Personnes en détresse |
| 8 | `08-Reinsertion-second-depart` | Second départ | Sortie de détention/refuge/dépendance |

---

## 2. État de validation (en toute transparence)

| Vérification | Statut |
|--------------|--------|
| **Compilation Fable (F# → JS)** | ✅ **8/8** validées (chaîne réelle .NET 10 + Fable 5.2, compilées une à une depuis le disque) |
| **Build PWA complet (`dist/`)** | ✅ **8/8** buildées de bout en bout sur Windows (.NET 10 + Fable 5.2 + Vite) |
| **Rendu visuel + interactions** | ✅ **Les 8 apps testées dans Chrome** (écran d'accueil + interactions). Tous les défauts trouvés — accents manquants et emojis corrompus introduits par les agents — ont été **corrigés et re-vérifiés à l'écran**. App 1 testée à fond (navigation, détection d'arnaque, quiz, formulaire). |
| **Encodage / cohérence fichiers** | ✅ vérifié (UTF-8, projets cohérents, zéro octet nul) |

**À faire côté toi avant un lancement public** : lancer chaque app (`npm run dev`), cliquer dans chaque écran, et — pour les apps sensibles (2 et 7) — la revue sécurité/clinique décrite dans leur `PLAN.md`.

---

## 3. Architecture commune

```
Apps-Solidaires/
  design-system/        tokens.css (couleurs/typo/espacement), components.css (.sol-*), demo.html (vitrine)
  0X-.../
    PLAN.md             le plan produit + technique complet de l'app
    app/
      src/*.fs          F# : Domain (types), I18n, State (Model/Msg/update), View (Feliz), Main, + données
      index.html        page hôte (charge le JS compilé + le service worker)
      public/           manifest.webmanifest, sw.js (offline), icons/
      css/              copie du design system (pour l'offline)
      package.json      scripts npm + dépendances (react, react-dom, vite)
      .config/          Fable 5.2 (dotnet tool)
  README.md             vue d'ensemble + priorisation
  00-FONDATIONS-COMMUNES.md   socle technique détaillé
  COMPILER.md           prérequis et commandes de build validés
```

**Principe de chaque app (identique partout)** : un `Model` (état immuable) + des `Msg` (événements) + une fonction `update` pure (logique) + une `view` Feliz (interface). Tout l'état vit sur l'appareil (localStorage), rien n'est envoyé sur un serveur par défaut. Le service worker met l'app en cache → elle s'ouvre et fonctionne **sans réseau** après la première visite.

**Navigation** : chaque app a une barre d'onglets en bas (`.sol-nav`) qui change l'`Onglet` dans le `Model` ; la `view` affiche l'écran correspondant.

---

## 4. Fonctionnement détaillé, app par app

### App 1 — Bouclier anti-arnaque  *(vérifiée à l'écran)*

**But** : aider un aîné à reconnaître une fraude et prévenir un proche avant de payer.

**4 onglets :**
- **Accueil** — 4 gros boutons illustrés : Vérifier un message, « Je vérifie avant de payer » (rouge), Apprendre, Mon proche.
- **Vérifier** — l'usager colle un texto/courriel ou décrit l'appel, tape **Vérifier** → l'app compare le texte à des signatures de fraude (`Cafc.fs`) et affiche un **verdict** : vert « aucun signe » ou rouge avec le type de fraude et son explication. En dessous, la pause **« avant de payer »** : 3 interrupteurs (cartes-cadeaux ? urgent ? secret ?) ; si l'un est coché, alerte rouge + bouton « Prévenir mon proche ».
- **Apprendre** — un quiz : on choisit une réponse, la bonne devient verte avec une explication, les mauvaises rouges.
- **Proches** — formulaire nom + téléphone + interrupteur « activer l'alerte » ; **Enregistrer** sauvegarde localement (le bouton reste désactivé tant que nom/téléphone sont vides).

**Données** (`Domain.fs`) : `TypeFraude`, `Verdict`, `Proche`, `ReponsePause`. **Détection** : `Cafc.verifier` (échantillon embarqué ; en production, brancher les données ouvertes du Centre antifraude du Canada). **Limite réelle** : une PWA ne peut pas bloquer les appels ; le blocage en temps réel nécessite une extension native (voir `PLAN.md` §6).

### App 2 — Sécurité (violence conjugale)

**But** : préparer une sortie, documenter les incidents, trouver de l'aide — discrètement. **L'app la plus sensible.**

**4 onglets :** **Accueil discret** (accès direct à SOS violence conjugale) ; **Plan de sortie** (checklist par catégorie : documents, sac d'urgence, compte séparé, contacts — avec barre de progression) ; **Ressources** (numéros et maisons d'hébergement du Québec, hors-ligne, filtrables par région) ; **Réglages** (camouflage : nom/icône factice + NIP ; **effacement rapide** en 2 étapes).

**Données** : `EtapeSortie`, `Ressource`, `Camouflage`. **À faire avant diffusion (marqué TODO dans le code)** : chiffrement réel des données, camouflage natif robuste, et **un audit de sécurité indépendant**. Le scaffold ne doit pas être présenté comme sûr en l'état.

### App 3 — Boussole nouveaux arrivants

**But** : guider les démarches d'établissement selon le statut de la personne.

**4 onglets :** **Onboarding statut** (6 statuts : réfugié réinstallé, demandeur d'asile, résident permanent, CUAET, travailleur temporaire, étudiant) ; **Checklist** d'établissement filtrée par statut et par horizon (J+1, J+7, 1/3/6 mois), cochable, avec progression ; **Annuaire 211** (ressources locales, échantillon) ; **Courriers IRCC** (choisir un type de lettre → explication en langage simple). Disclaimer « information générale, pas un avis juridique » présent partout.

**Données** (`Domain.fs` + `Contenu.fs`) : `Statut`, `Horizon`, `Etape`, `Ressource211`, `TypeCourrier`. Multilingue prévu (FR/EN + RTL).

### App 4 — Lien (aînés anti-isolement)

**But** : rompre l'isolement, mode **kiosque ultra-simple** (gros boutons, texte agrandi).

**4 onglets :** **Accueil** avec un énorme bouton **« Je vais bien aujourd'hui »** (check-in du jour) ; **Appeler un proche** (contacts avec photo, un seul appui → lien `tel:`) ; **Humeur du jour** (3 grands émojis) ; **Réglages famille** (seuil d'inactivité, contacts d'escalade).

**Données** : `CheckIn`, `Contact`, `Humeur`, `RegleInactivite`. Le check-in fonctionne hors-ligne ; un **repli SMS** est prévu (TODO) pour les aînés sans Internet. Données de santé → consentement et chiffrement à ajouter (Loi 25).

### App 5 — Mots du quotidien (alphabétisation adulte)

**But** : apprendre à lire/écrire **sans stigmatisation** ; interface **voix d'abord** (rien à lire pour naviguer). Le mot « alphabétisation » n'apparaît nulle part.

**3 onglets :** **Accueil** (grille de modules du quotidien : médicaments, transport, argent, formulaires… avec pictogrammes) ; **Module** (étapes illustrées + bouton **haut-parleur** qui lit à voix haute via la synthèse vocale du navigateur) ; **Jardin** (progression « invisible » : une plante qui pousse, pas de niveaux affichés).

**Données** : `Module`, `Etape`, `Jardin`. Pictogrammes = emojis en attendant l'**API ARASAAC** (production) ; voix = Web Speech en attendant **Coqui TTS** (vrai hors-ligne). 8 modules × 5 étapes embarqués.

### App 6 — Récolte (anti-gaspillage)  *(buildée en PWA)*

**But** : relier les surplus alimentaires aux organismes par le **don gratuit** (pas de vente).

**Écrans :** **Choix de rôle** (donneur / organisme) ; **Publier un don** (type d'aliment, quantité, contrainte de froid, fenêtre de récupération) ; **Surplus** (liste des dons disponibles, tri par urgence/froid) ; **Mes dons** (statuts : disponible / réservé / récupéré). Le caractère **gratuit** du don est martelé dans l'interface.

**Données** : `CategorieAliment` (7 catégories), `ContrainteFroid`, `StatutDon`, `Don`. Scaffold local (8 dons d'exemple) ; en production : back léger (Cloudflare Workers + D1) + intégration Open Food Network (TODO marqués).

### App 7 — Ancrage (premiers soins psychologiques)

**But** : soutien immédiat en détresse, **100 % hors-ligne**. **Outil de soutien, PAS un dispositif médical ni un substitut à une aide professionnelle** (cadré explicitement dans l'UI).

**Élément permanent :** un bouton **« Aide maintenant »** (rouge, toujours visible) ouvre les ressources de crise préchargées : **988**, **1-866-APPELLE**, texto **53 53 53**, suicide.ca. **Onglets :** **Ancrage** (respiration 4-7-8 animée, technique 5-4-3-2-1) ; **Plan de sécurité** (modèle Stanley-Brown, éditable, exportable en PDF chiffré) ; **Journal d'humeur** (échelle + note, graphique local, export CSV).

**Données** : `Exercice`, `PlanSecurite`, `Humeur`, `EntreeJournal`, `RessourceCrise`. **Avant lancement** : revue clinique des libellés + chiffrement local.

### App 8 — Second départ (réinsertion)

**But** : accompagner pas à pas la sortie de détention, d'un refuge ou d'une dépendance.

**5 onglets :** **Choix de situation** (détention / refuge / dépendance) ; **Premier jour** (checklist des 48 h : dormir, manger, qui appeler) ; **Parcours** (étapes par catégorie — papiers, logement, santé, emploi — filtrées par situation, cochables, avec progression) ; **Annuaire** hors-ligne (ressources 211/ASRSQ) ; **Contacts** d'urgence + rappels.

**Données** : `Situation`, `Etape`, `Ressource`, `ContactUrgence`, `Rappel`. Offline-first, sans compte. Disclaimer « informations indicatives ». Chiffrement à ajouter (données potentiellement demandées par un tiers).

---

## 5. Build et déploiement

Prérequis : **.NET SDK 10**, **Node 18+**, **Fable 5.2** (déjà épinglé). Détails dans `COMPILER.md`.

```bash
cd 01-Bouclier-anti-arnaque/app
dotnet tool restore     # Fable 5.2
npm install             # react, react-dom, vite
npm run dev             # http://localhost:5173 (dev)
npm run build           # -> dist/  (PWA statique, à héberger gratuitement)
```

`dist/` se déploie tel quel sur GitHub Pages, Cloudflare Pages ou Netlify. L'app est installable et fonctionne hors-ligne.

---

## 6. Captures (app 1, navigateur réel)

Dans `D:\Apps-Solidaires\_captures\` : `cap-1-accueil.png`, `cap-2-verifier.png` (détection d'arnaque en action), `cap-3-apprendre.png` (quiz), `cap-4-proches.png`. Le rendu respecte les couleurs PGRG ; les icônes y apparaissent en carrés uniquement parce que le navigateur de test (Linux headless) n'avait pas de police emoji — elles s'affichent normalement sur Windows/Mac/Android.

---

## 7. Limites connues et prochaines étapes

- **Vérification visuelle** des apps 2 à 8 : à faire en local (`npm run dev`) — non bloquant, design system déjà validé à l'écran.
- **Apps sensibles (2, 7)** : ne pas diffuser sans la revue sécurité/clinique de leur `PLAN.md`.
- **Données réelles** : remplacer les échantillons embarqués par les sources ouvertes (CAFC, 211, ARASAAC, Open Food Network, lignes de crise).
- **Chiffrement** : activer le chiffrement local (AES-GCM) là où c'est marqué TODO (apps 2, 4, 7, 8).
- **App 6** : un avertissement CSS bénin reste à nettoyer.
- **Suite possible** : finir les features v1/v2 des `PLAN.md`, préparer le dépôt GitHub (licences, CI), empaqueter en apps natives (Tauri desktop / Android) si souhaité.
