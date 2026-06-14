# Audit de sécurité — App 02 « Notes perso » (Sécurité / violence conjugale)

- **Date** : 2026-06-14
- **Version auditée** : dépôt `freeforgecontact-ai/ElderApp`, app `02-Securite-violence-conjugale` (PWA Fable 5 / F#)
- **Type d'audit** : revue de code statique + analyse du modèle de menace, menée par l'assistant FreeForge
- **Référentiel** : le `PLAN.md` de l'app (§5, §6, §9) et les cadres Tech Safety Canada cités en sources

> ⚠️ **Portée et limite de cet audit.** Il s'agit d'une **revue technique automatisée**, pas d'un audit de sécurité indépendant ni d'une co-conception avec des intervenantes spécialisées. Le `PLAN.md` (§9-§10) pose ces deux conditions comme **obligatoires avant toute diffusion grand public**. Cet audit corrige des failles techniques concrètes, mais **ne lève pas** l'exigence d'un audit humain externe et d'un test terrain encadré avant de recommander l'app à de vraies usagères.

---

## 1. Modèle de menace

L'adversaire principal n'est pas un pirate distant, mais **une personne qui a un accès physique et social à l'appareil** : le conjoint qui consulte le téléphone, connaît parfois les codes, et peut surveiller le réseau domestique. Le risque vital est **la découverte de l'outil**. Conséquences pour l'évaluation :

- Tout ce qui est stocké en clair est considéré comme **lisible par l'adversaire**.
- Tout nom, cache, titre ou trace visible (DevTools, historique, liste d'apps, relevé réseau) est une **fuite**.
- Toute requête réseau non sollicitée est **traçable**.
- L'usagère doit pouvoir **fuir l'écran et effacer instantanément**.

---

## 2. Synthèse des constats

| # | Gravité | Constat | Emplacement |
|---|---------|---------|-------------|
| 1 | 🔴 Critique | Données stockées **en clair** (aucun chiffrement) | `src/Storage.fs` |
| 2 | 🔴 Critique | **Noms révélateurs** : clés `sec_etapes_v1` / `sec_camouflage_v1`, cache `securite-v1` | `src/Storage.fs`, `public/sw.js` |
| 3 | 🔴 Critique | **Verrou NIP annoncé mais non fonctionnel** (aucune UI de définition, aucun écran de déverrouillage) | `src/Domain.fs`, `src/State.fs`, `src/View.fs` |
| 4 | 🟠 Majeur | **Aucune sortie rapide** vers un site neutre | `src/View.fs` |
| 5 | 🟠 Majeur | **Effacement non « d'urgence »** : 3 étapes, pas de redirection neutre | `src/View.fs`, `src/State.fs` |
| 6 | 🟠 Majeur | **Service worker à comportement réseau** (network-first navigation, repli réseau) | `public/sw.js`, `index.html` |
| 7 | 🟡 Moyen | **Camouflage superficiel** : ne renomme que l'en-tête ; libellés internes explicites | `src/View.fs`, `src/I18n.fs` |
| 8 | 🟢 Mineur | Parsing JSON artisanal fragile | `src/Storage.fs` |

**Constats positifs** : aucun appel réseau de données dans le code F# (vérifié : zéro `fetch`/`XMLHttpRequest`/URL externe) ; liens `tel:`/`sms:` standards sans fuite ; manifest, `short_name` et `<title>` déjà neutres (« Notes perso ») ; avertissement de sécurité honnête déjà affiché à l'accueil.

---

## 3. Détail des constats et recommandations

### 🔴 1 — Données en clair
Le plan de sortie (cases cochées : « Contacter une maison d'hébergement », « Alerter une personne de confiance »…) et les préférences sont sérialisés en JSON et écrits tels quels dans `localStorage`. Quiconque ouvre les outils de développement, inspecte le profil du navigateur ou extrait les fichiers lit l'intention de partir.
**Reco** : chiffrement **AES-GCM 256** au repos, clé dérivée d'un NIP via **PBKDF2** ; sans NIP, données illisibles.

### 🔴 2 — Noms révélateurs
Les identifiants techniques `sec_*` et le cache `securite-v1` apparaissent en clair dans l'onglet Application des DevTools. Le mot « sécurité » trahit la nature de l'app.
**Reco** : renommer en identifiants neutres cohérents avec « Notes perso » (`np_*`, cache `notes-cache`).

### 🔴 3 — Verrou NIP non fonctionnel
`Domain.Camouflage` porte `NipActif`/`NipHash`, `State.Model` porte `BrouillonNip`/`BrouillonNipConfirm`, mais **aucune vue** ne permet de saisir un NIP et **aucun écran** ne le vérifie au démarrage. La porte d'entrée est ouverte.
**Reco** : écran de déverrouillage si NIP actif ; UI de création/changement de NIP ; le NIP sert aussi à dériver la clé de chiffrement (constat 1).

### 🟠 4 — Pas de sortie rapide
Standard de base des applications pour victimes : un bouton **toujours visible** qui quitte l'écran immédiatement vers une page anodine.
**Reco** : bouton « Quitter » permanent + touche **Échap**, faisant `location.replace` vers un site neutre (météo) — sans laisser d'entrée dans l'historique.

### 🟠 5 — Effacement non « d'urgence »
L'effacement exige d'aller dans Réglages, cliquer, confirmer ; il ne referme pas l'écran.
**Reco** : effacement accessible rapidement, puis **redirection neutre** immédiate.

### 🟠 6 — Service worker réseau
`sw.js` fait du *network-first* pour la navigation et un repli réseau pour les ressources. En PWA installée (l'`index.html` enregistre le SW), chaque ouverture peut générer une requête vers le domaine d'origine, visible sur le réseau surveillé.
**Reco** : **cache-only après installation** (offline strict), cache renommé ; aucune requête réseau spontanée.

### 🟡 7 — Camouflage superficiel
Le réglage « camouflage » ne change que le titre de l'en-tête. Dès l'app ouverte, « Plan de sortie », « Ici en sécurité », « Effacement rapide » révèlent tout.
**Reco** : à court terme, neutraliser les libellés les plus explicites ; à terme, **vrai camouflage = version native** (cf. `PLAN.md §6`).

### 🟢 8 — Parsing JSON artisanal
`getField` lit le JSON à la main ; fragile mais sans impact sécurité direct. Devient sans objet une fois le contenu chiffré/sérialisé proprement.

---

## 4. Corrections appliquées (cette itération)

| Constat | Correction | Vérifié |
|---------|-----------|---------|
| 1 — données en clair | **Chiffrement AES-GCM 256** au repos ; clé dérivée du NIP par **PBKDF2** (150 000 itérations, SHA-256), sel aléatoire. Implémenté via WebCrypto (`Storage.fs`). | ✅ stockage `E:…` observé |
| 2 — noms révélateurs | Clé unique renommée **`np_v1`** ; cache du service worker renommé **`notes-cache-v2`** ; plus aucune clé `sec_*`. | ✅ `sec_*` absentes |
| 3 — verrou NIP absent | **Écran de déverrouillage** au démarrage si NIP actif ; **UI de création/retrait du NIP** dans Réglages ; sans NIP correct, données indéchiffrables. | ✅ bon NIP ouvre / mauvais rejeté |
| 4 — pas de sortie rapide | **Bouton « Quitter » permanent** (en-tête) + **touche Échap** → `location.replace` vers un site neutre, sans entrée d'historique. | ✅ redirige vers le site neutre |
| 5 — effacement non urgent | Bouton **« Effacer et quitter »** (efface tout puis sort) en plus de l'effacement classique. | ✅ présent |
| 6 — service worker réseau | SW en **cache-first strict** (navigation comprise) : aucune requête réseau spontanée après installation ; cache renommé. | ✅ code revu |
| 7 — camouflage superficiel | Nom/onglet/manifest neutres (« Notes perso ») confirmés ; libellé d'accueil adouci. Camouflage complet = version native (hors PWA). | ⚠️ partiel (limite PWA assumée) |

**Tests fonctionnels réalisés (Chrome, build de production) :** activation du NIP → données chiffrées ; rechargement → écran de verrouillage, contenu masqué ; bon NIP → ouverture ; mauvais NIP → refus ; bouton « Quitter » → site neutre. Compilation Fable sans erreur.

> Le chiffrement protège les données **au repos** sur l'appareil. Il ne protège pas contre un appareil déjà déverrouillé et observé en direct, ni contre un logiciel espion installé par l'agresseur — d'où l'importance de la sortie rapide et, à terme, de la version native.

---

## 5. Conditions de remise en ligne

1. ✅ Correctifs techniques 1 à 7 implémentés et testés.
2. ⛔ **Toujours requis avant promotion grand public** (non couvert par cet audit) :
   - audit de sécurité **humain et indépendant** (pentest + revue du chiffrement) ;
   - **co-conception et test terrain** avec des intervenantes (SOS violence conjugale, Regroupement des maisons) ;
   - revue juridique pour la recevabilité des preuves (si le journal de preuves est ajouté).
3. **Recommandation** : remettre l'app en ligne comme **démonstration technique**, avec son avertissement intégré, et **ne pas la promouvoir** auprès d'usagères réelles via un canal grand public tant que le point 2 n'est pas satisfait.

---

## 6. Verdict

Le scaffold est **honnête** (il prévient lui-même de ses limites) et **sain côté réseau** (100 % local). Ses manques étaient ceux d'un prototype : pas de chiffrement, pas de verrou, pas de sortie rapide, noms parlants. Ces points sont **corrigeables** et corrigés dans cette itération. La **diffusion à de vraies victimes** reste toutefois conditionnée à un audit humain et à une co-conception — règle posée par le projet lui-même.
