# Apps-Solidaires
## Portfolio d'applications libres et gratuites à fort impact social

> 8 applications, 100 % open source, gratuites pour toujours. Conçues pour les personnes que le marché ignore ou fait payer le plus cher. Ancrage initial : Québec / Canada francophone. Vocation : multilingue et mondial.

---

## Pourquoi ce projet

Le marché logiciel laisse de côté — ou fait payer le plus cher — celles et ceux qui ont le moins : aînés, personnes en situation de précarité, victimes de violence, nouveaux arrivants, personnes peu alphabétisées. Ces huit applications comblent des manques **réels et documentés**, là où il n'existe aujourd'hui aucune solution à la fois libre, gratuite et en français.

Chaque dossier contient un plan complet (`PLAN.md`) : problème chiffré, personas, analyse concurrentielle, vision produit, spécifications fonctionnelles, architecture technique (Fable 5 / F# / PWA), modèle de données, accessibilité, risques, feuille de route MVP et plan de lancement open source.

---

## Principes directeurs (non négociables)

1. **Gratuit pour toujours** — aucune publicité, aucun abonnement, aucune revente de données.
2. **Vie privée par conception** — offline-first, données locales chiffrées, zéro compte obligatoire quand c'est possible. Conforme à la Loi 25 (Québec) et à la LPRPDE (fédéral).
3. **Accessibilité d'abord** — WCAG 2.2 niveau AA au minimum, AAA quand le public l'exige (aînés, faible littératie).
4. **Hors-ligne d'abord** — tout ce qui est vital fonctionne sans réseau.
5. **Souveraineté des données** — l'usager possède, exporte et efface ses données à tout moment.
6. **Gouvernance ouverte** — code public, contenu maintenu avec les organismes du milieu, contributions communautaires.

---

## Les 8 applications

| # | Application | Public visé | Manque comblé | Dossier |
|---|-------------|-------------|---------------|---------|
| 1 | Bouclier anti-arnaque | Aînés, proches aidants | Détection FR + alerte d'un proche avant un paiement suspect | `01-Bouclier-anti-arnaque` |
| 2 | Sécurité (violence conjugale) | Victimes de violence conjugale | App camouflée FR, journal de preuves chiffré, effacement rapide | `02-Securite-violence-conjugale` |
| 3 | Boussole nouveaux arrivants | Immigrants, réfugiés, demandeurs d'asile | Parcours par statut + traduction de documents + annuaire 211 | `03-Boussole-nouveaux-arrivants` |
| 4 | Aînés anti-isolement | Aînés isolés et leurs proches | Check-in quotidien + détection d'inactivité + appel vidéo ultra-simple | `04-Aines-anti-isolement` |
| 5 | Alphabétisation adulte | Adultes à faible littératie | Interface 100 % voix + pictogrammes, sans stigmatisation | `05-Alphabetisation-adulte` |
| 6 | Anti-gaspillage alimentaire | Commerces, organismes, citoyens | Pont de DON gratuit vers les banques alimentaires (pas de revente) | `06-Anti-gaspillage-alimentaire` |
| 7 | Premiers soins psychologiques | Personnes en détresse | Journal + ancrage + plan de sécurité + 988, hors-ligne et en FR | `07-Premiers-soins-psychologiques` |
| 8 | Réinsertion / second départ | Sortie de détention, refuge, dépendance | Compagnon FR : checklist, annuaire hors-ligne, rappels | `08-Reinsertion-second-depart` |

---

## Socle technique commun

Toutes les apps partagent une même fondation, détaillée dans **`00-FONDATIONS-COMMUNES.md`** :

- **Langage** : F# compilé en JavaScript via **Fable 5**.
- **Front** : Elmish (architecture Model-View-Update) + Feliz, rendu PWA installable.
- **Hors-ligne** : Service Worker + IndexedDB, fonctionnement complet sans réseau.
- **Stockage sécurisé** : chiffrement local (WebCrypto, AES-GCM) pour les données sensibles.
- **Design system** : composants accessibles partagés (gros boutons, fort contraste, synthèse vocale).
- **i18n** : fichiers de traduction communautaires, FR/EN d'abord.
- **Coût d'hébergement** : nul ou quasi nul (hébergement statique : GitHub Pages, Cloudflare Pages, Netlify).

---

## Ordre de construction recommandé

Priorisation par **impact x faisabilité x créneau libre x risque**. Échelle 1 (faible) à 5 (fort).

| App | Impact | Faisabilité | Risque | Données prêtes | Vague |
|-----|:------:|:-----------:|:------:|:--------------:|:-----:|
| 1 — Anti-arnaque | 5 | 4 | Faible | CAFC ouvert | **1** |
| 4 — Anti-isolement | 4 | 5 | Faible | Jitsi/Linphone | **1** |
| 6 — Anti-gaspillage | 4 | 3 | Moyen | Open Food Network | **1** |
| 3 — Nouveaux arrivants | 5 | 3 | Moyen | 211 / IRCC | **2** |
| 8 — Réinsertion | 4 | 4 | Moyen | 211 / ASRSQ | **2** |
| 5 — Alphabétisation | 4 | 3 | Faible | ARASAAC | **2** |
| 7 — Soins psychologiques | 5 | 3 | Élevé | 988 / suicide.ca | **3** |
| 2 — Sécurité (VC) | 5 | 2 | Critique | SOS VC / 211 | **3** |

**Vague 1 — gagner vite, prouver le modèle.** Fort impact, technique maîtrisée, peu de risque juridique. Le Bouclier anti-arnaque est le meilleur point de départ : données ouvertes prêtes, créneau francophone vide, valeur immédiate.

**Vague 2 — densité de contenu et partenariats.** La valeur dépend d'un contenu juste et à jour (services, droits, ressources) : exige des partenaires du milieu (211, organismes communautaires) pour la fiabilité et la durabilité.

**Vague 3 — haute sensibilité, rigueur maximale.** Soins psychologiques et Sécurité touchent à la vie et à la sécurité physique. À ne lancer qu'avec un cadrage clinique/juridique sérieux et, pour l'app Sécurité, **un audit de sécurité indépendant obligatoire avant toute diffusion publique**.

---

## Structure du dépôt

```
Apps-Solidaires/
  README.md                        <- ce fichier
  00-FONDATIONS-COMMUNES.md         <- stack, design, sécurité, i18n, gouvernance
  01-Bouclier-anti-arnaque/PLAN.md
  02-Securite-violence-conjugale/PLAN.md
  03-Boussole-nouveaux-arrivants/PLAN.md
  04-Aines-anti-isolement/PLAN.md
  05-Alphabetisation-adulte/PLAN.md
  06-Anti-gaspillage-alimentaire/PLAN.md
  07-Premiers-soins-psychologiques/PLAN.md
  08-Reinsertion-second-depart/PLAN.md
```

## Licence

- **MIT** par défaut pour les apps clientes (réutilisation maximale).
- **AGPL-3.0** pour les apps à composante serveur (ex. Anti-gaspillage) afin de garder les améliorations ouvertes.
- App **Sécurité (violence conjugale)** : audit de sécurité indépendant **avant** toute publication ; voir son PLAN.md.

> « Apps-Solidaires » est un nom de chantier. Chaque app peut recevoir son propre nom public au lancement.