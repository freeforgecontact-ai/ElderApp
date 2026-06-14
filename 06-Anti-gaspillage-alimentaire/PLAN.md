# 06 — Anti-gaspillage alimentaire ↔ banques alimentaires

> **Mission** : relier en temps réel les surplus alimentaires (commerces, restos, jardins, particuliers) aux organismes et aux personnes dans le besoin — par le **don gratuit**, pas la revente.

**Vague de construction : 1.**

---

## 1. Le problème

Le Canada gaspille des millions de tonnes de nourriture par an pendant que la demande d'aide alimentaire explose : le **Bilan-Faim 2024** des Banques alimentaires du Québec recense **près de 2,9 millions de demandes d'aide en un seul mois (mars 2024)**. Le réseau (18 Moisson + ~1 300 organismes locaux) peine à capter les surplus, faute d'outil de mise en relation rapide. Les apps existantes ont choisi le créneau **commercial** (rabais), pas le **don**.

---

## 2. Personas

- **Gérant d'épicerie** — jette chaque soir des invendus encore bons ; veut un moyen *simple et sans risque légal* de les donner.
- **Coordonnatrice d'un organisme** — cherche des dons frais à proximité, mais n'a ni temps ni outil pour suivre qui a quoi.
- **Bénévole-chauffeur** — disponible pour récupérer et livrer si on lui dit où et quand.
- **Jardinier / particulier** — a un surplus de potager et aimerait le donner plutôt que le perdre.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| Too Good To Go | Commercial | Commission sur ventes ; pas de don gratuit ; données revendues |
| Flashfood | Commercial | Grandes chaînes seulement ; pas de don ; carte bancaire requise |
| Sauvegarde (MTL) | Commercial | Vente à rabais ; fermé ; pas de lien direct banques alimentaires |
| Food Rescue Robot | Open source | Anglais ; contexte US ; pas d'app mobile ; peu actif |

**Le trou** : aucune plateforme **libre, gratuite, bilingue** qui fait le **pont de don direct** vers le réseau des banques alimentaires, ouverte aussi aux **particuliers**.

---

## 4. Vision et principes

Le don, pas la transaction. Zéro argent qui circule, zéro donnée de bénéficiaire monétisée. **Accès anonyme** pour les personnes qui reçoivent (la dignité avant l'inscription). Interopérable avec l'infrastructure existante plutôt que concurrente.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Pont don → organisme** — un commerce publie un surplus (type, quantité, fenêtre de récupération) ; les organismes à portée sont notifiés ; confirmation de récupération. **Aucune transaction.**
2. **Carte temps réel des surplus** — offres actives géolocalisées, filtrables par type d'aliment et contrainte de froid.
3. **Alerte « surplus urgent »** — notification push aux organismes/bénévoles à proximité quand un don à délai court (< 3 h) est déposé.

### v1
4. **Portail particuliers** — jardins communautaires et citoyens peuvent aussi offrir des surplus.
5. **Coordination bénévoles-chauffeurs** — appariement récupération/livraison, itinéraires simples.
6. **Avertissements chaîne du froid** — règles de sécurité sanitaire selon le type d'aliment ; refus guidé des dons à risque sans logistique adaptée.

### v2
7. **Tableau de bord d'impact** — kg sauvés, repas équivalents, par région.
8. **Synchronisation Open Food Network** — pour ne pas dupliquer la logistique déjà déployée.

---

## 6. Architecture technique

Contrairement aux 5 premières apps, celle-ci a **besoin d'un dos serveur léger** (mise en relation temps réel, géolocalisation, notifications). D'où le choix **AGPL** (garder les améliorations ouvertes).

- **Front** : PWA Fable 5 / Elmish (donneur, organisme, bénévole : 3 vues d'une même base).
- **Back léger** : **Cloudflare Workers + D1** (ou Supabase, palier gratuit) — API d'annonces, géo-requêtes, Web Push. Reste gratuit jusqu'à une grande échelle.
- **Données** : intégration **Open Food Network Canada** (AGPL, bilingue) pour la logistique ; **Open Food Facts** (codes-barres, catégories) ; annuaire des organismes (Banques alimentaires du Québec, données Québec).
- **Géo** : géocodage + requêtes de proximité ; carte via tuiles ouvertes (OpenStreetMap).
- **Anonymat bénéficiaires** : aucune inscription requise pour recevoir ; pas de profilage.
- **Hors-ligne** : consultation des offres en cache ; publication mise en file (outbox) si réseau instable.

---

## 7. Modèle de données

```fsharp
type ContrainteFroid = Ambiant | Réfrigéré | Congelé

type Don =
    { Id: string
      Donneur: string
      TypeAliment: string
      Quantité: string
      Froid: ContrainteFroid
      FenêtreFin: System.DateTime    // après quoi périmé/indisponible
      Position: float * float
      Statut: StatutDon }
and StatutDon = Disponible | Réservé of orga:string | Récupéré | Expiré

type Organisme =
    { Nom: string; Région: string; Rayon_km: int
      AccepteFroid: ContrainteFroid list }
```

---

## 8. Accessibilité et langue

- Bilingue FR/EN ; interface simple pour des bénévoles de tous âges.
- Gros boutons, carte lisible, pictogrammes par type d'aliment.
- WCAG 2.2 AA ; mode contraste ; compatible mobile bas de gamme.

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Responsabilité légale du don** | Élevée | Clause de renonciation ; cadre via **La Tablée des Chefs** ; conformité MAPAQ (Loi P-29) |
| **Chaîne du froid / sécurité sanitaire** | Élevée | Règles strictes par type ; refus guidé des dons à risque ; traçage température v1 |
| **Données des bénéficiaires** | Élevée | Accès anonyme, aucune inscription pour recevoir, pas de profilage |
| **Disparité géographique** | Moyenne | Priorité géo aux zones rurales sous-desservies, pas seulement aux grands centres |
| **Coût serveur à l'échelle** | Moyenne | Paliers gratuits ; architecture frugale ; auto-hébergement possible |

> Veille : un **projet de loi anti-gaspillage** (déposé au QC en 2026) pourrait obliger les grandes chaînes à conclure des accords de récupération — opportunité d'adoption.

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Cadrage | Validation avec une Moisson + La Tablée des Chefs (cadre légal) | 2 sem. |
| 1 — Socle | PWA 3 rôles + back léger (Workers/D1) | 2–3 sem. |
| 2 — Pont don | Publication, notification organismes, confirmation | 2–3 sem. |
| 3 — Carte + urgent | Géo, carte temps réel, alerte < 3 h | 2 sem. |
| 4 — Pilote | 1 quartier : quelques commerces + 1 organisme | 2–3 sem. |
| **MVP** | **Public, bilingue** | **~9–11 sem.** |
| 5 — v1/v2 | Particuliers, chauffeurs, froid, OFN, impact | +5–8 sem. |

---

## 11. Plan de lancement open source

- **Licence** : AGPL-3.0 (composante serveur).
- **Partenaires visés** : Banques alimentaires du Québec et les Moisson, La Tablée des Chefs (cadre juridique de récupération), Open Food Network Canada, municipalités (plans de réduction des déchets).
- **Distribution** : déploiement quartier par quartier ; intégration aux processus des organismes ; PWA + affichage en commerce.
- **Communauté** : contributeurs OSS, connecteurs de données régionales, bénévoles.

---

## 12. Sources

- Bilan-Faim 2024 (Banques alimentaires du Québec) — https://banquesalimentaires.org/wp-content/uploads/2024/10/Bilan-Faim_2024.pdf
- Open Food Network Canada — https://openfoodnetwork.ca/
- Open Food Facts (données) — https://world.openfoodfacts.org/data
- Food Rescue Robot (OSS) — https://github.com/boulder-food-rescue/food-rescue-robot
- La Tablée des Chefs — récupération alimentaire — https://www.tableedeschefs.org/fr/recuperation-alimentaire/
- Données Québec — https://www.donneesquebec.ca/