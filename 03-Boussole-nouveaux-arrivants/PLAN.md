# 03 — Boussole nouveaux arrivants

> **Mission** : guider chaque personne immigrante, réfugiée ou demandeuse d'asile dans ses démarches d'établissement — selon son statut réel — traduire ses documents et trouver les services autour d'elle, gratuitement et dans sa langue.

**Vague de construction : 2.**

---

## 1. Le problème

Arriver dans un nouveau pays, c'est affronter un labyrinthe administratif dans une langue qu'on ne maîtrise pas toujours, avec des **droits et des démarches qui changent radicalement selon le statut** (réfugié réinstallé, demandeur d'asile, résident permanent, travailleur temporaire, étudiant). Une mauvaise orientation peut priver quelqu'un d'un service auquel il a droit — ou lui faire rater un délai aux conséquences graves (refus, renvoi).

Les outils existants sont fragmentés, souvent anglophones, rarement à jour, et aucun ne traduit les documents officiels reçus.

---

## 2. Personas

- **Amir, 29 ans, demandeur d'asile** — reçoit des lettres d'IRCC qu'il ne comprend pas ; ne sait pas quels services lui sont ouverts.
- **Famille Nguyen, résidents permanents** — arrivés depuis 2 semaines ; cherchent francisation, école, carte d'assurance maladie, logement.
- **Organisme d'accueil** — accompagne des centaines de personnes ; veut un outil fiable à mettre entre leurs mains.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| Welcome to Canada (PeaceGeeks) | Gratuit, fermé | FR partiel ; pas de traduction de documents ; financement institutionnel instable |
| O-Canada (OIM/IRCC) | Gratuit, propriétaire | Réservé aux réfugiés réinstallés ; pas de services locaux QC |
| ArriveCAN | Gratuit, propriétaire | Conçu pour la douane, pas l'établissement ; quasi abandonné |
| ArriveON / UpRow (COSTI) | Gratuit, propriétaire | Ontario ; pas de version FR adaptée QC ; non interopérable avec 211 |

**Le trou** : aucune solution bilingue+ n'offre ensemble (a) **parcours personnalisés par statut**, (b) **traduction de documents**, (c) **annuaire 211 dynamique**, (d) explication des courriers IRCC en langage simple.

---

## 4. Vision et principes

Une boussole, pas un guichet : elle oriente, explique et rassure, sans jamais se substituer à un conseil juridique. **Privacy-by-design** (les demandeurs d'asile peuvent être en danger). Contenu **versionné et validé** par des partenaires, parce qu'une information périmée est dangereuse.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Parcours « Mon statut, mes droits »** — onboarding par statut (6 catégories : réfugié réinstallé, demandeur d'asile, résident permanent, CUAET, travailleur temporaire, étudiant) qui filtre automatiquement les étapes et délais pertinents.
2. **Checklist d'établissement personnalisée** — par horizon (J+1, J+7, 1 mois, 3 mois, 6 mois), cases cochables hors-ligne, exportable en PDF.
3. **Annuaire 211 Québec intégré** — ressources géolocalisées (hébergement, aide alimentaire, francisation, services juridiques) en cache local.

### v1
4. **Traducteur de documents hors-ligne** — OCR + traduction locale (LibreTranslate / Argos) des lettres, baux, formulaires médicaux, avec mention claire « traduction non officielle ».
5. **Aide à la compréhension des courriers IRCC** — l'usager photographie une lettre ; l'app identifie le type (convocation, décision, demande de pièce) et explique en langage simple les prochaines étapes, **sans donner d'avis juridique**.

### v2
6. **Rappels d'échéances** — dates limites (renouvellements, rendez-vous), locaux et privés.
7. **Mode hors-ligne complet par région** — packs téléchargeables (utile sans forfait data à l'arrivée).

---

## 6. Architecture technique

- **PWA Fable 5 / Elmish**, multilingue dès le départ (FR/EN + es, ar, fa, uk, zh), avec support **RTL**.
- **Traduction offline** : moteur **Argos Translate** embarqué côté client / **LibreTranslate** auto-hébergeable — aucune dépendance à Google/DeepL, aucune fuite des documents vers un tiers. OCR via Tesseract (WebAssembly).
- **Annuaire 211** : import des données 211 Québec (donneesquebec.ca), format pivot commun, cache IndexedDB, rafraîchissement périodique.
- **Contenu des parcours** : fichiers structurés (Markdown + métadonnées), **versionnés et datés**, modifiables par les partenaires via un wiki contrôlé (pull request). La logique de filtrage par statut est en F# (unions discriminées = pas d'état incohérent).
- **Vie privée** : tout local ; aucun profil centralisé ; les documents traduits ne quittent jamais l'appareil.

---

## 7. Modèle de données

```fsharp
type Statut =
    | RéfugiéRéinstallé | DemandeurAsile | RésidentPermanent
    | CUAET | TravailleurTemporaire | Étudiant

type Étape =
    { Titre: string
      Horizon: string            // "J+7", "3 mois"...
      Statuts: Statut list       // à qui elle s'applique
      Faite: bool
      Ressources: RessourceId list }

type Ressource211 =
    { Nom: string; Catégorie: string; Région: string
      Téléphone: string; MajLe: System.DateTime }

type DocumentTraduit =
    { Type: string; LangueSource: string; TexteTraduit: string }  // jamais envoyé au réseau
```

---

## 8. Accessibilité et langue

- **Multilingue + pictogrammes** : navigation compréhensible même avec une lecture limitée.
- Langage simplifié, phrases courtes, lecture vocale dans la langue choisie.
- Avertissements clairs : « information générale », « pour un avis juridique, voir un organisme ».
- WCAG 2.2 AA ; gros boutons ; mode contraste.

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Information juridique erronée/périmée** | Élevée | Contenu versionné et validé par partenaires ; disclaimer ; renvoi aux organismes officiels |
| **Vie privée des demandeurs d'asile** | Élevée | Données locales, aucun profil, documents jamais envoyés au réseau |
| **Mauvaise orientation par statut** | Élevée | Onboarding rigoureux ; filtrage typé ; option « je ne sais pas » → vers un organisme |
| **Obsolescence du contenu** | Élevée | Modèle de contribution communautaire + partenaire mainteneur |
| **Traduction trompeuse** | Moyenne | Mention « non officielle » systématique ; ne jamais traduire un formulaire à signer sans avertissement |

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Cadrage | Parcours par statut validés avec un organisme d'accueil | 1–2 sem. |
| 1 — Socle | PWA multilingue, RTL, design accessible | 2 sem. |
| 2 — Parcours + checklist | 6 statuts, étapes datées, export PDF | 2–3 sem. |
| 3 — Annuaire 211 | Import, cache, géolocalisation | 1–2 sem. |
| **MVP** | **Public, FR/EN, hors-ligne** | **~7–9 sem.** |
| 4 — v1 | Traduction offline + lecture des courriers IRCC | +4–6 sem. |
| 5 — v2 | Rappels, packs régionaux, langues additionnelles | +3–4 sem. |

---

## 11. Plan de lancement open source

- **Licence** : MIT (app) ; contenus sous CC.
- **Partenaires visés** : organismes d'accueil financés par IRCC/MIFI, TCRI (Table de concertation des réfugiés et immigrants), 211 Québec, bibliothèques.
- **Distribution** : PWA + QR dans les organismes, classes de francisation, points de service ; packs régionaux.
- **Communauté** : traducteurs bénévoles, mainteneurs de contenu par statut, gouvernance avec le milieu.

---

## 12. Sources

- Apps d'établissement au Canada (panorama) — https://km4s.ca/2022/01/smartphone-apps-for-migration-and-settlement-in-canada-the-current-landscape/
- Données ouvertes IRCC — https://ouvert.canada.ca/data/fr/dataset/b6cbcf4d-f763-4924-a2fb-8cc4a06e3de4
- 211 Québec (données) — https://www.211quebecregions.ca/pages/donnees
- Services aux nouveaux arrivants (IRCC) — https://ircc.canada.ca/english/newcomers/services/index.asp
- Données Québec — https://www.donneesquebec.ca/
- LibreTranslate (traduction open source) — https://libretranslate.com/