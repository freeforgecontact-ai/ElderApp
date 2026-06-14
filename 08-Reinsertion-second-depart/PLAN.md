# 08 — Réinsertion / second départ

> **Mission** : accompagner pas à pas les personnes qui sortent de détention, d'un refuge ou d'une dépendance — checklist concrète, annuaire de ressources hors-ligne et rappels — pour transformer une sortie en vrai nouveau départ.

**Vague de construction : 2.**

---

## 1. Le problème

La sortie est le moment le plus fragile : en quelques jours, il faut récupérer des papiers, trouver où dormir, ouvrir un compte, demander l'aide sociale, voir un médecin, parfois respecter des conditions de probation — souvent sans téléphone à jour, sans argent et avec une faible aisance numérique. Les ressources existent (ASRSQ, YMCA-Espadrille, 211) mais sont **dispersées**, sans outil mobile qui les réunisse et **suive les démarches** de la personne. Un oubli peut coûter une prestation, un logement, ou une violation de conditions.

---

## 2. Personas

- **Daniel, 38 ans** — sort de détention ; n'a plus de pièce d'identité valide ; doit voir son agent de probation et trouver un hébergement transitoire.
- **Karine, 30 ans** — quitte une thérapie en dépendance ; veut structurer ses premières semaines pour ne pas rechuter.
- **Intervenant ASRSQ / YMCA** — accompagne plusieurs personnes ; cherche un outil qu'elles peuvent garder dans leur poche.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| PlanStreet | Propriétaire SaaS | Pour gestionnaires de cas, pas pour la personne ; payant ; anglais |
| John Howard Society (web) | Annuaire statique | Pas d'app ; pas de suivi ; surtout anglais |
| National Reentry Resource Center | Ressources statiques (US) | Contexte américain ; pas d'app ; pas en français |
| 211 Québec | Service gratuit | Pas d'app dédiée avec suivi/checklist/rappels |

**Le trou** : aucune app mobile **libre, en français**, qui combine **checklist par situation + annuaire hors-ligne + rappels** pour la personne elle-même.

---

## 4. Vision et principes

Un compagnon de poche, concret et sans jugement. **Offline-first et sans compte obligatoire** — parce que les données peuvent être sensibles (probation, surveillance). Informations **indicatives** qui orientent toujours vers les organismes officiels. Pensé pour une **faible littératie numérique**.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Mode « premier jour » (48 h)** — checklist des toutes premières heures (où dormir, où manger, qui appeler), inspirée des guides ASRSQ / YMCA-Espadrille.
2. **Parcours personnalisé par situation** — 3 entrées (sortie de **détention** / **refuge** / **dépendance**) avec étapes séquentielles adaptées : pièce d'identité → compte bancaire → aide sociale → hébergement transitoire → santé → emploi.
3. **Annuaire hors-ligne par région** — ressources 211 mises en cache par MRC / code postal, consultables **sans connexion**.

### v1
4. **Suivi des démarches + rappels** — cases à cocher par étape ; rappels configurables (rendez-vous probation, renouvellement de médicaments, entrevues) ; export en fichier texte simple.
5. **Carnet de contacts d'urgence** — intervenant, maison de transition, médecin, numéro de probation — accessible depuis l'accueil, hors-ligne.

### v2
6. **Documents** — coffre local chiffré pour photos de papiers importants.
7. **Mode intervenant** — un accompagnateur peut préparer un parcours et le remettre à la personne (sans surveillance continue).

---

## 6. Architecture technique

- **PWA Fable 5 / Elmish**, **offline-first**, installable sur appareil modeste.
- **Sans compte** pour l'usage de base ; données locales ; **chiffrement** (AES-GCM) du coffre de documents et des contacts.
- **Annuaire 211** : import des données 211 Québec (donneesquebec.ca), format pivot commun (partagé avec apps 3 et 6), cache par région, rafraîchissement en Wi-Fi.
- **Contenu des parcours** : étapes en format ouvert, **versionnées et validées** par des partenaires (ASRSQ, YMCA-Espadrille) ; logique de séquencement en F# (états typés).
- **Rappels** : notifications locales (pas de serveur) ; rien ne dépend du réseau.
- **Minimisation** : aucune donnée envoyée ; conscience que des tiers (probation, employeur) pourraient demander l'accès → tout reste sous le contrôle et le chiffrement de l'usager.

---

## 7. Modèle de données

```fsharp
type Situation = Détention | Refuge | Dépendance

type Étape =
    { Titre: string
      Catégorie: string             // "papiers","logement","santé","emploi"
      Situations: Situation list
      Faite: bool
      Échéance: System.DateTime option
      Ressources: RessourceId list }

type Rappel =
    { Quoi: string; Quand: System.DateTime; Récurrent: bool }

type ContactUrgence =
    { Nom: string; Rôle: string; Téléphone: string }   // chiffré
```

---

## 8. Accessibilité (faible littératie numérique)

- Interface ultra-simplifiée, **pictogrammes + texte court**, lecture vocale.
- Police ajustable, fort contraste, gros boutons.
- Fonctionne hors-ligne, sur vieux téléphone, sans forfait de données.
- Aucun jargon administratif non expliqué.

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Données saisissables par un tiers** (probation, employeur) | Élevée | Offline-first, chiffrement local, aucun compte obligatoire, effacement |
| **Information légale/administrative erronée** | Élevée | Contenu « indicatif » + renvoi aux organismes ; versionnage daté ; validation partenaires |
| **Faible littératie numérique** | Élevée | Pictos, voix, simplicité radicale, hors-ligne |
| **Durabilité du contenu** | Moyenne | Partenariat ASRSQ / YMCA / John Howard pour la maintenance |
| **Stigmatisation** | Moyenne | App neutre, discrète, sans étiquette visible |

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Cadrage | Parcours par situation validés avec ASRSQ / YMCA-Espadrille | 2 sem. |
| 1 — Socle | PWA offline, design simple/accessible | 1–2 sem. |
| 2 — Parcours + premier jour | 3 situations, étapes, checklist 48 h | 2–3 sem. |
| 3 — Annuaire 211 | Import, cache par région, hors-ligne | 1–2 sem. |
| 4 — Test terrain | Avec des personnes accompagnées | 2 sem. |
| **MVP** | **Public, FR, hors-ligne** | **~7–9 sem.** |
| 5 — v1/v2 | Rappels, contacts, coffre documents, mode intervenant | +4–6 sem. |

---

## 11. Plan de lancement open source

- **Licence** : MIT (app) ; contenus sous CC.
- **Partenaires visés** : ASRSQ (Association des services de réhabilitation sociale du Québec), YMCA-Espadrille (accompagnement postdétention), Société John Howard, maisons de transition, 211 Québec.
- **Distribution** : remise à la sortie (détention, refuge, fin de thérapie), via les intervenants ; PWA installable ; fiches imprimables.
- **Communauté** : mainteneurs de contenu par situation, traductions, partenaires terrain.

---

## 12. Sources

- ASRSQ — https://asrsq.ca/
- YMCA Québec — réinsertion postdétention (Espadrille) — https://www.ymcaquebec.org/fr/programmes-aide/reinsertion-sociale/postdetention
- 211 Québec (données) — https://www.211quebecregions.ca/pages/donnees
- Gouvernement du Québec — réinsertion sociale — https://www.quebec.ca/securite-situations-urgence/services-correctionnels/reinsertion-sociale
- Données Québec — https://www.donneesquebec.ca/
- Société John Howard — https://johnhoward.ca/