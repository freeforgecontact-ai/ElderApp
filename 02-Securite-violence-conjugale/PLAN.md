# 02 — Sécurité (violence conjugale)

> **Mission** : donner aux personnes victimes de violence conjugale un outil discret, gratuit et en français pour préparer leur sécurité, documenter ce qu'elles vivent et trouver de l'aide — sans jamais les mettre en danger.

**Vague de construction : 3 (sensibilité maximale).**

> ⚠️ **Principe directeur absolu** : la sécurité physique de l'usagère prime sur toute fonctionnalité. *Perdre des données est toujours préférable à les exposer.* Cette app ne doit **jamais** être diffusée sans co-conception avec des intervenantes spécialisées et **un audit de sécurité indépendant** (voir §9 et §10).

---

## 1. Le problème

La violence conjugale tue et blesse, et la technologie est à double tranchant : un téléphone surveillé par l'agresseur devient une arme. Les victimes ont besoin de préparer une sortie, de **documenter les incidents** (preuves admissibles en cour) et d'accéder aux ressources — mais toute trace visible peut déclencher une escalade. Les organismes spécialisés sont unanimes : **le risque numéro un, c'est que l'agresseur découvre l'outil.**

Au Québec, SOS violence conjugale opère une ligne 24/7, et le Regroupement des maisons regroupe 46 maisons d'hébergement — mais aucun outil numérique libre, en français, ne relie plan de sortie, preuves et ressources de façon sécuritaire.

---

## 2. Personas

- **Sophie, 34 ans** — vit avec un conjoint contrôlant qui vérifie son téléphone. A besoin d'un accès *invisible* aux ressources et d'un moyen de noter les incidents.
- **Intervenante en maison d'hébergement** — accompagne des dizaines de femmes ; veut un outil qu'elle peut recommander en confiance, qui ne crée pas de risque.
- **Proche de confiance** — sœur, amie, prête à recevoir une alerte discrète.

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| myPlan Canada | Gratuit, fermé | Pas de version française QC stable ; pas de journal de preuves ; camouflage limité |
| Aspire News | Gratuit, propriétaire | Camouflage basique ; pas de preuves ; US, pas de ressources QC |
| Bright Sky (UK) | Gratuit, propriétaire | Anglais ; répertoire non canadien ; pas d'effacement rapide |
| bSafe / StaySafe | Payant | Coût prohibitif ; collecte GPS ; abonnement visible (relevé bancaire) |

**Le trou** : aucune solution open source FR n'offre ensemble (a) **journal de preuves horodaté et chiffré**, (b) **camouflage robuste + effacement rapide**, (c) **annuaire des ressources québécoises** hors-ligne.

---

## 4. Vision et principes

- **Sécurité d'abord, fonctionnalités ensuite.** Chaque choix est évalué par « est-ce que ça peut la mettre en danger ? ».
- **Invisible par défaut** : rien qui révèle la nature de l'app.
- **Sous le contrôle total de l'usagère** : elle décide ce qui est gardé, partagé, effacé.
- **Co-construit avec le milieu** : intervenantes et survivantes dans la boucle dès la conception.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Camouflage** — icône et nom configurables (apparence de météo, calculatrice ou recettes) ; **NIP secret** distinct du déverrouillage du téléphone ; **aucune notification visible**.
2. **Effacement d'urgence** — en **2 tapotements** ou par un geste (secouer) : suppression instantanée de toutes les données, retour à un écran neutre.
3. **Plan de sortie guidé** — checklist personnalisable (documents à rassembler, sac d'urgence, compte bancaire séparé, contacts) entièrement hors-ligne.
4. **Carte des ressources QC hors-ligne** — 46 maisons d'hébergement, SOS violence conjugale (1 800 363-9010 / texto 438 601-1211 / clavardage), numéros locaux, mis en cache.

### v1
5. **Journal de preuves chiffré et horodaté** — notes, photos, audios ; chaque entrée scellée par un horodatage cryptographique (hash SHA-256) ; chiffrement AES-256 local ; **export sécurisé** vers un tiers de confiance (avocate, maison) conforme au cadre de Tech Safety Canada (recevabilité en cour).
6. **Mode « je vais bien »** — check-in discret quotidien vers une personne de confiance ; en l'absence de confirmation, un message codé pré-convenu part automatiquement.

### v2 (uniquement après audit)
7. **Bouton d'alerte discret** — geste secret envoyant la localisation à un contact opt-in.
8. **Effacement à distance** déclenché par un mot-code.

---

## 6. Architecture technique

**Honnêteté technique sur le camouflage.** Une PWA installée permet une icône et un nom neutres sur l'écran d'accueil, et fonctionne hors-ligne — mais le **navigateur garde un historique** et la liste des apps installées peut trahir. Conséquences sur l'architecture :

- **MVP en PWA déguisée** : accès rapide aux ressources + plan de sortie, sous une URL et un nom anodins, sans rien d'évocateur. Ouverture en mode standalone (pas d'historique d'onglet visible). Effacement rapide du stockage local.
- **Camouflage maximal et journal de preuves : version native recommandée** (icône réellement substituée, pas d'historique de navigation, stockage chiffré au niveau OS, déclencheurs natifs). Le noyau de logique (chiffrement, scellé horodaté, modèle de plan de sortie) est écrit en **F#** et partagé entre PWA et natif.
- **Chiffrement** : AES-GCM 256, clé dérivée du NIP secret (PBKDF2/Argon2). **Aucune sauvegarde cloud automatique** (ni iCloud ni Google Drive). Sans le NIP, les données sont irrécupérables — c'est voulu.
- **Scellé de preuve** : chaque entrée = contenu + métadonnées (date, appareil) + hash SHA-256 chaîné à l'entrée précédente (journal infalsifiable), pour soutenir la recevabilité.
- **Réseau** : par défaut, aucune connexion. Les seules sorties réseau (export, check-in) sont explicites, déclenchées par l'usagère, chiffrées.

---

## 7. Modèle de données (entièrement chiffré au repos)

```fsharp
type TypeIncident = Verbal | Physique | Financier | Numérique | Autre of string

type Preuve =
    { Le: System.DateTime
      Type: TypeIncident
      Note: string
      PièceJointe: byte[] option   // photo/audio chiffrés
      Hash: string                 // SHA-256 chaîné (scellé)
      HashPrécédent: string }

type ÉtapeSortie = { Libellé: string; Faite: bool }

type Camouflage =
    { NomAffiché: string           // ex. "Météo"
      IcôneId: string
      NipHash: string }
```

---

## 8. Accessibilité et discrétion

- Interface neutre, sobre, qui ressemble à l'app dont elle prend l'apparence.
- Sortie texte/vocale optionnelle **désactivée par défaut** (le son peut trahir).
- Multilingue prioritaire (FR/EN + langues d'immigration : isolement linguistique = vulnérabilité accrue).
- Aucune capture d'écran promotionnelle révélant l'usage réel.

---

## 9. Risques et contraintes — CRITIQUES

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Découverte par l'agresseur** | **Vitale** | Camouflage, NIP secret, aucune notif, effacement 2 tapotements, pas de trace bancaire (gratuit) |
| **Sauvegarde cloud qui expose** | **Vitale** | Aucune sauvegarde auto ; export uniquement manuel et chiffré |
| **Faux sentiment de sécurité** | Élevée | Messages prudents ; co-conception avec intervenantes ; ne jamais survendre l'outil (mise en garde de Tech Safety Canada) |
| **Preuves non recevables** | Élevée | Horodatage chaîné, métadonnées, cadre Tech Safety Canada / chaîne de possession |
| **Alerte involontaire** | Élevée | Tiers de confiance strictement opt-in et réversible |
| **Vulnérabilité logicielle** | **Vitale** | **Audit de sécurité indépendant obligatoire avant toute diffusion** |

---

## 10. Feuille de route (jalons de sûreté intégrés)

| Phase | Contenu | Garde-fou |
|-------|---------|-----------|
| 0 — Co-conception | Ateliers avec intervenantes et survivantes | Aucun code avant ce point |
| 1 — Ressources + plan de sortie | PWA déguisée hors-ligne, carte QC | Revue par le milieu |
| 2 — Effacement + camouflage | NIP secret, effacement 2 tapotements | Tests adversariaux |
| 3 — Journal de preuves | Chiffrement, scellé horodaté, export | Revue juridique (recevabilité) |
| **AUDIT** | **Audit de sécurité externe + test terrain encadré** | **Bloque la diffusion publique** |
| 4 — Diffusion | Publication seulement après audit réussi | Partenariat maisons/SOS VC |

---

## 11. Plan de lancement open source

- **Licence** : MIT pour le code ; **diffusion conditionnée à l'audit**.
- **Partenaires indispensables** : SOS violence conjugale, Regroupement des maisons pour femmes victimes de violence conjugale, Tech Safety Canada (cadre preuve), Conseil du statut de la femme.
- **Distribution** : jamais par publicité grand public ; via les intervenantes, les maisons, les lignes d'aide, en main à main, avec consignes de sécurité.
- **Gouvernance** : code public et auditable, mais communication prudente ; documentation de sécurité pour les intervenantes.

---

## 12. Sources

- SOS violence conjugale — https://www.sosviolenceconjugale.ca/
- Regroupement des maisons — https://maisons-femmes.qc.ca/
- Tech Safety Canada (preuve numérique) — https://techsafety.ca/resources/toolkits/preserving-digital-evidence-toolkit
- Tech Safety Canada (apps de sécurité) — https://techsafety.ca/resources/toolkits/choosing-and-using-safety-apps-designed-for-women-experiencing-violence
- myPlan Canada — https://myplanapp.ca/
- 211 Québec — https://qc.211.ca/
- Revue systématique (apps VC) — https://pmc.ncbi.nlm.nih.gov/articles/PMC10094623/

---

> **Note finale** : si une seule fonctionnalité de ce plan crée un risque pour une seule usagère, elle est retirée. C'est la règle qui prime sur le calendrier et sur l'ambition.