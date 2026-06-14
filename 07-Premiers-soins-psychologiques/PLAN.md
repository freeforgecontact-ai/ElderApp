# 07 — Premiers soins psychologiques (hors-ligne)

> **Mission** : offrir un soutien immédiat en cas de détresse — journal d'humeur, exercices d'ancrage, plan de sécurité et ressources de crise — qui fonctionne **sans réseau** et en français.

**Vague de construction : 3 (haute sensibilité).**

> ⚠️ **Cadre** : cette app est un **outil de soutien et de bien-être, PAS un dispositif médical** et **PAS un substitut** à une aide professionnelle. Elle n'établit aucun diagnostic et ne remplace ni un thérapeute ni une ligne de crise. Les ressources d'urgence (988) sont **toujours accessibles en un geste**.

---

## 1. Le problème

En situation de détresse, l'aide n'est pas toujours joignable : il est 3 h du matin, il n'y a pas de réseau, ou la personne n'ose pas appeler. Les outils existants sont souvent anglophones, payants, ou monétisent des données parmi les plus intimes qui soient. Il manque un compagnon **hors-ligne, gratuit, francophone**, qui propose des gestes concrets *maintenant* et oriente vers les bonnes ressources.

La ligne **988** (prévention du suicide) est active au Canada depuis novembre 2023 ; au Québec s'ajoutent **1-866-APPELLE** et **suicide.ca** (clavardage et texto). Aucun outil libre ne les met à portée immédiate dans une app de soutien.

---

## 2. Personas

- **Élodie, 23 ans** — anxiété, crises nocturnes ; a besoin d'un exercice de respiration immédiat, sans devoir chercher.
- **Personne en région éloignée** — réseau intermittent ; veut des ressources qui marchent hors-ligne.
- **Intervenant** — souhaite recommander un outil de soutien entre les rendez-vous, fiable et non commercial.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| PTSD Coach (VA) | Partiellement OSS | Anglais ; ciblé vétérans US ; données aux USA |
| Wysa | Propriétaire, freemium | Fonctions avancées payantes ; données de santé mentale exploitées |
| MindShift (Anxiety Canada) | Gratuit, fermé | Anxiété seulement ; pas de plan de sécurité ; FR limité |
| if-me.org / Medito / Aware (29k) | Open source | Web only ou méditation seule ; **pas** de plan de sécurité ni de ressources de crise locales |

**Le trou** : aucune app OSS ne réunit **journal d'humeur + ancrage hors-ligne + plan de sécurité + numéros de crise canadiens en français**.

---

## 4. Vision et principes

- **Sécurité et prudence avant tout** : aucune promesse thérapeutique, ressources de crise omniprésentes.
- **Hors-ligne d'abord** : les gestes d'urgence fonctionnent en mode avion.
- **Données ultra-privées** : tout reste sur l'appareil, l'usager est propriétaire et peut tout exporter ou effacer.
- **Encadrement clinique** : conçu avec des professionnels, sur des protocoles validés.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Bouton ressources de crise — toujours visible** — 988, 1-866-APPELLE, texto 53 53 53, suicide.ca ; liste préchargée par province, accessible **hors-ligne**, en un geste.
2. **Exercices d'ancrage en mode avion** — respiration 4-7-8, cohérence cardiaque animée, technique sensorielle 5-4-3-2-1 ; tout fonctionne sans réseau.
3. **Plan de sécurité personnel** — basé sur le modèle validé **Stanley-Brown** (signaux d'alerte, stratégies, personnes et ressources à contacter, sécurisation de l'environnement) ; modifiable ; partageable en **PDF chiffré** avec un proche de confiance.

### v1
4. **Journal d'humeur** — suivi simple, graphiques **locaux**, **export CSV** (l'usager possède ses données) ; aucune analytique transmise.
5. **Mode discret** — icône neutre, rien sur l'écran de verrouillage, accès par code court (utile en contexte familial/professionnel peu sûr).

### v2
6. **Boîte à outils personnalisée** — l'usager épingle ses exercices efficaces.
7. **Rappels bienveillants** — opt-in, doux, jamais culpabilisants.

---

## 6. Architecture technique

- **PWA Fable 5 / Elmish**, **100 % offline-first** : tout le cœur (ressources, ancrage, plan de sécurité) fonctionne sans serveur.
- **Aucun backend** : pas de compte, pas de cloud. Les données restent en IndexedDB, chiffrées (AES-GCM) si l'usager active un code.
- **Ressources de crise** : préchargées et mises en cache ; mises à jour opportunistes en Wi-Fi, mais **jamais nécessaires** au fonctionnement.
- **Plan de sécurité** : structuré selon Stanley-Brown ; export PDF généré localement, chiffrable.
- **Zéro télémétrie** : aucune donnée de santé mentale ne sort de l'appareil, par conception (et par obligation Loi 25 / LPRPDE).
- **Statut réglementaire** : positionnement explicite « bien-être général » pour rester **hors champ SaMD** (Santé Canada) — aucune fonction de diagnostic ou de traitement.

---

## 7. Modèle de données (local, chiffrable)

```fsharp
type Humeur = int      // échelle simple 1..5

type EntréeJournal =
    { Le: System.DateTime; Humeur: Humeur; Note: string option }

type PlanSécurité =
    { SignauxAlerte: string list
      StratégiesApaisantes: string list
      PersonnesSoutien: Contact list
      Professionnels: Contact list
      SécuriserEnvironnement: string list }   // modèle Stanley-Brown

type Exercice =
    | Respiration478 | CohérenceCardiaque | Ancrage54321
```

---

## 8. Accessibilité

- **WCAG 2.1 AA minimum** ; pensé pour une cognition altérée (détresse, médication) : peu d'éléments, gros texte, langage doux.
- Tout est lisible/jouable à voix haute ; animations calmes ; mode sombre.
- Aucune contrainte de temps ; sortie toujours possible.

---

## 9. Risques et contraintes — sensibles

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Risque suicidaire** | **Vitale** | Ressources de crise omniprésentes ; plan Stanley-Brown ; jamais de gamification de la crise ; orientation claire vers 988 |
| **Prétention médicale (SaMD)** | Élevée | Cadrage « bien-être », aucun diagnostic/traitement ; mentions explicites |
| **Données sensibles** | Élevée | Local-only, chiffrement, zéro vente, conformité Loi 25/LPRPDE |
| **Responsabilité civile** | Élevée | Revue clinique et juridique ; avertissements ; pas de conseil personnalisé automatisé |
| **Accès par un tiers** | Moyenne | Mode discret, code, effacement |

---

## 10. Feuille de route MVP

| Phase | Contenu | Garde-fou |
|-------|---------|-----------|
| 0 — Cadrage clinique | Protocoles (PFA/OMS, Stanley-Brown) validés avec des pros | Aucun lancement sans cette base |
| 1 — Ressources + ancrage | Bouton crise hors-ligne, 3 exercices | Revue clinique |
| 2 — Plan de sécurité | Stanley-Brown, export PDF chiffré | Revue clinique + juridique |
| 3 — Journal + mode discret | Suivi local, code, effacement | Test d'utilisabilité |
| **MVP** | **Public, FR, hors-ligne** | **~7–9 sem. + validations** |

---

## 11. Plan de lancement open source

- **Licence** : MIT (app) ; protocoles publics (OMS PFA, Stanley-Brown).
- **Partenaires visés** : Association québécoise de prévention du suicide / suicide.ca, eSantéMentale.ca, INSPQ, milieux communautaires en santé mentale.
- **Distribution** : recommandation par intervenants, organismes, lignes d'aide ; PWA ; jamais de marketing agressif.
- **Communauté** : contenu clinique révisé par des pros, traductions, transparence du code.

---

## 12. Sources

- Ligne 988 Canada — https://988.ca/
- suicide.ca (Québec) — https://suicide.ca/
- eSantéMentale.ca — ressources de crise QC — https://www.esantementale.ca/Quebec/Ressources-en-cas-de-crise-ou-durgence/
- Aware / 29k (OSS) — https://github.com/29ki/29k
- if-me.org (OSS) — https://github.com/ifmeorg/ifme
- Santé Canada — logiciels comme dispositifs médicaux (SaMD) — https://www.canada.ca/en/health-canada/services/drugs-health-products/medical-devices/application-information/guidance-documents/software-medical-device-guidance/notice.html
- INSPQ — santé mentale (données) — https://www.inspq.qc.ca/sante-mentale/donnees

---

> Ce projet touche des vies. Aucune fonctionnalité ne passe avant la sécurité de la personne, et rien ne se lance sans validation clinique.