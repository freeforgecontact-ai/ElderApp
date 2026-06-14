# 05 — Alphabétisation adulte (voix d'abord)

> **Mission** : permettre aux adultes qui lisent mal d'apprendre à leur rythme, dans la dignité — une interface entièrement vocale et illustrée, sans jamais afficher le mot « alphabétisation ».

**Vague de construction : 2.**

---

## 1. Le problème

Selon le PEICA 2022 (Statistique Canada), **51,6 % des adultes québécois se situent sous le niveau 3 de littératie** (le seuil considéré nécessaire pour fonctionner pleinement dans la société), et jusqu'à **64 % dans certaines régions**. Ces personnes peinent à lire une ordonnance, un bail, un avis officiel. Le frein principal n'est pas seulement pédagogique : c'est la **honte**. Les adultes concernés évitent activement tout ce qui est étiqueté « cours de lecture ».

---

## 2. Personas

- **Réjean, 47 ans** — travaille de ses mains, cache qu'il déchiffre mal. Ne s'inscrira jamais à un « cours d'alphabétisation », mais utiliserait une app discrète sur son téléphone.
- **Formatrice en groupe populaire (RGPAQ)** — accompagne des adultes ; cherche un outil que les apprenants peuvent utiliser seuls entre les rencontres.
- **Nouvelle arrivante peu scolarisée** — doit apprendre à lire le français du quotidien.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| J'apprends (FR) | Propriétaire | Conçue en France ; vocabulaire/contexte non québécois ; pas open source |
| LÉKOL | Propriétaire | Ciblage emploi (exclut les plus éloignés) ; interface textuelle ; pas de version QC |
| Literacy Planet | Payant (AUS) | Anglais ; interface enfantine stigmatisante ; abonnement |
| Khan Academy | Gratuit | Interface texte-lourde inaccessible aux faibles lecteurs ; pas de mode tout-audio |

**Le trou** : aucune app **gratuite, québécoise, voix-d'abord et non stigmatisante** pour adultes peu alphabétisés.

---

## 4. Vision et principes

- **Voix d'abord** : on peut tout faire sans lire un seul mot.
- **Zéro stigmatisation** : l'app ressemble à un outil de vie quotidienne, jamais à une salle de classe. Le mot « alphabétisation » n'apparaît nulle part.
- **Ancrage dans le réel** : on apprend en lisant ce qui compte vraiment (ordonnance, formulaire, avis).
- **Co-conçu avec le milieu** (RGPAQ) pour la justesse pédagogique.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Mode « sans lecture »** — navigation 100 % par **pictogrammes (ARASAAC)** + **voix de synthèse (Coqui TTS)**. Aucun texte requis pour trouver ou lancer une activité.
2. **Leçons du quotidien** — modules ancrés sur des actes réels : lire une étiquette de médicament, comprendre un avis, remplir un chèque, lire un nom de rue (contexte québécois).
3. **Progression invisible** — pas de « niveau 1-2-3 » : un jardin ou un avatar qui évolue (gamification non stigmatisante).
4. **Hors-ligne complet** — modules téléchargeables (zones rurales, Nord, sans forfait données).

### v1
5. **Reconnaissance vocale** — l'apprenant répète un mot, l'app valide la prononciation.
6. **Mode formateur (RGPAQ)** — tableau de bord pour les intervenants, suivi **anonymisé** des progressions, attribution de modules.
7. **Mes documents** — l'apprenant photographie un document réel et travaille dessus (lecture guidée).

### v2
8. **Parcours écriture** — du tracé des lettres à l'écriture de mots utiles.
9. **Banque de contenus communautaire** — formateurs partagent leurs modules sous licence ouverte.

---

## 6. Architecture technique

- **PWA Fable 5 / Elmish**, pensée **audio d'abord** : chaque élément d'interface a un libellé vocal ; rien ne dépend de la lecture.
- **Pictogrammes** : intégration de l'**API ARASAAC** (40 000+ pictos, CC BY-NC-SA), mis en cache localement pour l'usage hors-ligne.
- **Synthèse vocale** : **Coqui TTS** (voix française, open source) embarquée/auto-hébergeable, pour ne pas dépendre d'un service payant et fonctionner hors-ligne.
- **Reconnaissance vocale** (v1) : Web Speech API quand dispo + repli local.
- **Contenu** : modules en format ouvert (métadonnées + audio + pictos), versionnés, contribuables par les groupes RGPAQ.
- **Anonymisation** : le suivi formateur n'utilise aucune donnée identifiante directe ; identifiants opaques, agrégation.

---

## 7. Modèle de données

```fsharp
type Module =
    { Id: string
      Thème: string                 // "médicaments", "transport"...
      Pictos: string list           // ids ARASAAC
      Audio: string                 // piste TTS pré-générée ou à la volée
      Niveau: int }                 // interne, jamais affiché

type Progression =
    { ModuleId: string
      Réussites: int
      DernierAccès: System.DateTime }

type ProfilApprenant =
    { IdOpaque: string              // anonyme
      ModulesActifs: string list
      VitesseVoix: float }
```

---

## 8. Accessibilité (cible AAA)

- **Contraste 7:1**, cibles ≥ 44 px, police 20 px+.
- Tout contenu audio est sous-titré ; tout contenu visuel est doublé en audio.
- Aucune contrainte de temps ; on peut réécouter à l'infini.
- Vocabulaire de l'interface : images + voix, jamais de jargon.
- Compatible lecteurs d'écran (NVDA, VoiceOver).

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Stigmatisation** | Critique | Aucune mention « alpha » ; design « vie quotidienne » ; progression invisible |
| **Fracture numérique** | Élevée | Navigation 100 % vocale ; hors-ligne ; matériel modeste |
| **Droits d'auteur du contenu** | Moyenne | Contenus sous licence ouverte (CC) ; pictos ARASAAC (CC) |
| **Justesse pédagogique** | Élevée | Co-conception et validation RGPAQ / CDEACF |
| **Qualité de la voix FR** | Moyenne | Coqui TTS affiné ; audios clés pré-enregistrés |

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Co-conception | Ateliers RGPAQ, choix des thèmes du quotidien | 2 sem. |
| 1 — Socle voix | PWA audio-first, intégration ARASAAC + Coqui | 2–3 sem. |
| 2 — Modules MVP | 8–10 modules du quotidien, progression invisible | 2–3 sem. |
| 3 — Hors-ligne | Téléchargement de packs, cache pictos/audio | 1 sem. |
| 4 — Test terrain | Apprenants en groupe populaire | 2 sem. |
| **MVP** | **Public, FR, hors-ligne** | **~8–10 sem.** |
| 5 — v1/v2 | Reco vocale, mode formateur, écriture, banque communautaire | +5–7 sem. |

---

## 11. Plan de lancement open source

- **Licence** : MIT (app) ; contenus CC ; pictos ARASAAC (CC BY-NC-SA).
- **Partenaires visés** : RGPAQ (78 groupes membres), ABC Alpha pour la vie, CDEACF, bibliothèques.
- **Distribution** : via les groupes populaires (usage autonome entre les rencontres) ; PWA discrète ; bouche-à-oreille.
- **Communauté** : formateurs créateurs de modules, traducteurs, voix communautaires.

---

## 12. Sources

- PEICA 2022 — littératie des adultes (Statistique Canada) — https://www150.statcan.gc.ca/n1/daily-quotidien/241210/dq241210a-fra.htm
- RGPAQ — https://rgpaq.qc.ca/
- ARASAAC (pictogrammes, API) — https://api.arasaac.org/
- Coqui TTS (synthèse vocale open source) — https://github.com/coqui-ai/TTS
- ABC Alpha pour la vie — https://abcalphapourlavie.ca/
- CDEACF — https://cdeacf.ca/