# 01 — Bouclier anti-arnaque

> **Mission** : protéger les aînés de la fraude téléphonique, par texto et par courriel — en français, gratuitement — et permettre à un proche d'intervenir *avant* qu'un paiement ne parte.

**Vague de construction : 1 (point de départ recommandé du portfolio).**

---

## 1. Le problème

En 2024, le Centre antifraude du Canada (CAFC) a recensé **638 M$ de pertes** déclarées ; au Québec, les pertes des aînés ont atteint **20 M$ (+17,7 % vs 2023)**. Le CAFC estime que **90 à 95 % des fraudes ne sont jamais signalées** : le chiffre réel est donc 10 à 20 fois supérieur.

Les aînés sont ciblés par des scénarios précis et documentés : fraude « grands-parents » (un faux petit-enfant en détresse), fraude amoureuse, fausse institution (banque, ARC, police), faux soutien technique. Le moment critique est toujours le même : **juste avant le virement, le paiement en cartes-cadeaux ou le retrait**. C'est là qu'il faut une friction protectrice.

---

## 2. Personas

- **Lucienne, 78 ans** — utilise un téléphone Android offert par sa fille. Reçoit 3-4 appels suspects/semaine. N'identifie pas toujours l'arnaque. A peur de « déranger » ses proches.
- **Marc, 49 ans (proche aidant)** — fils de Lucienne, veut être prévenu si quelque chose cloche, sans surveiller sa mère en continu ni envahir sa vie privée.
- **Centre communautaire FADOQ** — anime des ateliers et cherche un outil gratuit en français à recommander.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| Truecaller | Propriétaire | Téléverse les carnets de contacts sans consentement des tiers ; données monétisées ; pensé pour smartphones, pas pour aînés |
| Hiya | Propriétaire | Fonctions clés en mode payant ; ne couvre pas SMS ni courriels |
| RoboKiller | Propriétaire (~4 $/mois) | Abonnement ; centré USA ; pas de volet éducatif francophone |
| SpamBlocker (OSS, F-Droid) | Open source | Aucun volet éducatif, aucune alerte proche, pas de données canadiennes, Android seulement |

**Le trou** : aucune solution ne combine (a) détection ancrée Québec/Canada (données CAFC), (b) **alerte automatique à un proche avant un paiement suspect**, (c) **module éducatif francophone**. C'est notre positionnement.

---

## 4. Vision et principes

Un « ange gardien » discret et bienveillant, pas un logiciel de surveillance. L'aîné garde le contrôle ; le proche n'est qu'un filet de sécurité, strictement opt-in. Aucune donnée vendue, aucune liste de numéros téléversée sans consentement (le modèle Truecaller est explicitement **proscrit**).

---

## 5. Fonctionnalités

### MVP (v0.1) — cœur PWA, 100 % offline
1. **Base consultable de numéros et procédés signalés** — l'aîné ou le proche peut taper un numéro / coller un texto et savoir s'il correspond à un schéma connu (données CAFC en cache local, rafraîchies en Wi-Fi).
2. **« Je vérifie avant de payer »** — un grand bouton qui ouvre une pause guidée : 3 questions simples (« Vous demande-t-on des cartes-cadeaux ? un virement urgent ? le secret ? »), puis propose d'appeler un proche.
3. **Alerte proche (opt-in)** — si l'aîné active la pause « avant de payer », une notification est envoyée à un proche désigné avec un délai pour rappeler.
4. **Quiz fraude du jour** — micro-leçon de 60 s sur un scénario réel (grands-parents, ARC, amour), contenu tiré des typologies CAFC.

### v1
5. **Signalement simplifié au CAFC** — formulaire pré-rempli (numéro, heure) en un bouton.
6. **Liste blanche** — contacts de confiance (médecin, pharmacie, famille) jamais signalés ; évite les faux positifs dangereux.
7. **Analyse de courriels/textos collés** — repère les marqueurs d'hameçonnage (urgence, lien, demande de paiement atypique).

### v2
8. **Extension native de filtrage d'appels** (voir §6) — affichage d'avertissement en temps réel à l'arrivée d'un appel signalé.
9. **Tableau de bord proche aidant** — historique des alertes (consenti), avec respect strict de la vie privée.

---

## 6. Architecture technique

**Réalité des plateformes** : une PWA web **ne peut pas** intercepter les appels/SMS. Le filtrage d'appels en temps réel exige du code natif. D'où une architecture en deux cercles, partageant la logique F# :

- **Cœur PWA (Fable 5 / Elmish / Feliz)** — éducation, base consultable, pause « avant de payer », alerte proche (Web Push), signalement, quiz. Couvre 80 % de la valeur, installable partout, sans store.
- **Extension native optionnelle (v2)** :
  - **Android** : `CallScreeningService` + `RoleManager` (rôle « filtrage des appels ») pour étiqueter/avertir.
  - **iOS** : `Call Directory Extension` (liste de numéros à signaler/bloquer, mise à jour depuis l'app).
  - La logique de correspondance (matching) est écrite en F# et partagée via un module compilé ; la couche native ne fait qu'appeler ce noyau.

**Données CAFC** : import du jeu ouvert (open.canada.ca), normalisation en format pivot, cache IndexedDB, rafraîchissement trimestriel en Wi-Fi. Aucune donnée utilisateur n'est envoyée pour la consultation (tout est local).

**Alerte proche** : Web Push (VAPID) vers l'appareil du proche ; file d'attente offline (outbox) si l'aîné est hors-ligne au moment de l'alerte.

---

## 7. Modèle de données (local, chiffré si sensible)

```fsharp
type Signalement =
    | FraudeGrandsParents | FraudeAmoureuse | FauxSoutienTech
    | FausseInstitution | Autre of string

type EntréeBase =
    { Numéro: string option
      Motif: Signalement
      Région: string
      MajLe: System.DateTime }

type ProcheDeConfiance =
    { Nom: string; Téléphone: string; AlerteActive: bool }

type ÉvénementPause =
    { Le: System.DateTime
      Réponses: bool list      // réponses aux 3 questions
      ProchePrévenu: bool }
```

Stockage : `EntréeBase` (données publiques CAFC) en clair ; `ProcheDeConfiance` et `ÉvénementPause` chiffrés (AES-GCM, clé dérivée d'un NIP).

---

## 8. Accessibilité (cible AAA, public 65+)

- Police de base 20 px, ajustable jusqu'à 200 % sans casse.
- Contraste ≥ 7:1 ; boutons pleine largeur, libellé + icône.
- **Tout est lisible à voix haute** (TexteVocal) ; instructions parlées à chaque étape.
- Parcours d'installation guidé pas-à-pas (« Comment installer sur votre téléphone »).
- Zéro jargon : « arnaque », « payer », « appeler ma fille » — pas « phishing », « VAPID ».

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Faux positif** (bloquer un médecin) | Élevée | Liste blanche simple ; on **avertit**, on ne bloque jamais sans confirmation |
| **Vie privée** (Loi 25 / LPRPDE) | Élevée | Données locales ; alerte proche opt-in et réversible ; aucun téléversement de carnet |
| **Crowdsourcing abusif** (modèle Truecaller) | Élevée | Proscrit : on s'appuie sur des données officielles, pas sur les contacts des usagers |
| **Limites iOS** (téléphonie fermée) | Moyenne | Cœur PWA d'abord ; filtrage natif en extension v2, sans bloquer la valeur initiale |
| **Surveillance perçue** du proche | Moyenne | Cadrage « filet de sécurité », transparence totale envers l'aîné, contrôle par l'aîné |

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Cadrage | Personas validés avec un groupe FADOQ, maquettes accessibles | 1 sem. |
| 1 — Socle | Gabarit Fable 5/PWA, design system AAA, i18n FR | 1–2 sem. |
| 2 — Données CAFC | Import, normalisation, cache, base consultable | 1–2 sem. |
| 3 — Pause + alerte | « Avant de payer », Web Push proche, quiz | 2 sem. |
| 4 — Tests terrain | Test avec 5–10 aînés, ajustements a11y | 1–2 sem. |
| **MVP** | **Public, installable, FR** | **~6–8 sem.** |
| 5 — v1 | Signalement CAFC, liste blanche, analyse textos | +3–4 sem. |
| 6 — v2 | Extension native filtrage d'appels | +4–6 sem. |

---

## 11. Plan de lancement open source

- **Licence** : MIT.
- **Partenaires visés** : FADOQ et Petits Frères (diffusion, ateliers), Option Consommateurs (contenu fraude), CAFC (validation des typologies), bibliothèques publiques (installation assistée).
- **Distribution** : PWA (lien web + QR dans les ateliers) ; F-Droid puis stores pour la version native ; fiches imprimables pour les centres.
- **Communauté** : dépôt GitHub public, contenu éducatif ouvert à traduction, gabarits d'ateliers réutilisables.

---

## 12. Mesure d'impact

- Nombre de pauses « avant de payer » déclenchées et d'alertes proches envoyées.
- Numéros/scénarios consultés (anonyme, agrégé, local).
- Retours qualitatifs des partenaires (ateliers, témoignages).
- Aucune de ces mesures n'implique de pister l'usager : compteurs locaux, partage volontaire.

---

## 13. Sources

- Centre antifraude du Canada — https://antifraudcentre-centreantifraude.ca/index-fra.htm
- Jeu de données CAFC (Gouvernement ouvert) — https://open.canada.ca/data/en/dataset/6a09c998-cddb-4a22-beff-4dca67ab892f
- Rapports CAFC (ouvert.canada.ca) — https://ouvert.canada.ca/data/fr/dataset/69c68f22-8a2a-43d1-8f4e-4017e3ffebba
- CRTC — télémarketing et fraude — https://crtc.gc.ca/fra/phone/telemarketing/fraud.htm
- SpamBlocker (OSS) — https://github.com/aj3423/SpamBlocker
- Option Consommateurs — aînés et fraude — https://option-consommateurs.org/rapports-de-recherche/aines-la-fraude-canada