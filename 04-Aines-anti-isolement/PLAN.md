# 04 — Aînés anti-isolement

> **Mission** : rompre l'isolement des aînés avec un outil ultra-simple — un appel vidéo d'un seul geste, un « ça va aujourd'hui ? » quotidien, et une alerte aux proches si quelque chose cloche — gratuit et sans abonnement.

**Vague de construction : 1 (faisabilité la plus haute du portfolio).**

---

## 1. Le problème

Au Québec, environ **25 % de la population aura 65 ans et plus en 2031**, et une large part des aînés est à risque d'isolement social — un facteur de risque de santé comparable au tabagisme. Pourtant **près de 40 % des 75 ans et plus n'utilisent pas Internet** : les solutions « high-tech » les excluent. L'isolement n'est pas qu'une question de solitude : entre deux visites, personne ne sait si la personne va bien.

---

## 2. Personas

- **Gilles, 81 ans** — vit seul, peu à l'aise avec la technologie, voit ses petits-enfants trop rarement. Une interface compliquée le décourage immédiatement.
- **Nadia, 52 ans (fille)** — veut savoir que son père va bien chaque jour, sans le surveiller ni l'appeler dix fois.
- **Bénévole FADOQ / Petits Frères** — accompagne des aînés ; pourrait être un contact de confiance « élargi ».

---

## 3. Solutions existantes et lacune

| Solution | Type | Lacune |
|----------|------|--------|
| GrandPad | Propriétaire fermé | 299 $ + ~40 $/mois ; écosystème verrouillé ; pas de FR |
| ElliQ 2.0 | Propriétaire | 249 $ + abonnement ; données de santé centralisées chez un privé ; pas de FR |
| FaceTime / Zoom / WhatsApp | Généralistes | Trop complexes pour aînés peu technophiles ; aucun check-in ; aucune détection d'inactivité |
| Programmes de visites virtuelles | Humain | Non scalable ; pas d'outil intégré ; rien entre deux visites |

**Le trou** : aucun outil **gratuit, en français, ultra-simple** ne réunit appel vidéo d'un geste + check-in quotidien + détection d'inactivité + alerte proches.

---

## 4. Vision et principes

La simplicité radicale comme fonctionnalité principale : **un écran, un geste**. L'aîné n'a ni compte, ni mot de passe, ni menu. Le numérique s'efface ; il reste le lien. Respect absolu de la dignité : on outille, on ne surveille pas ; les proches sont un filet, pas un œil.

---

## 5. Fonctionnalités

### MVP (v0.1)
1. **Bouton check-in unique** — « Je vais bien », un seul appui par jour. Fonctionne **sans connexion** (repli **SMS**) pour les appareils/zones sans Internet.
2. **Appel vidéo ultra-simplifié** — la photo d'un proche, un appui, l'appel démarre (moteur **Jitsi Meet**), sans compte ni mot de passe pour l'aîné.
3. **Détection d'inactivité** — si aucun check-in / aucune activité pendant un seuil **personnalisable** (ex. 24 h), alerte au(x) proche(s), avec escalade (famille → bénévole → service).

### v1
4. **Humeur du jour** — 3 émojis simples accompagnant le check-in.
5. **Tableau de bord famille** (mobile) — derniers check-ins, dernière activité, humeur ; sobre et respectueux.
6. **Rappels doux** — médicaments, rendez-vous, sous forme vocale et visuelle.

### v2
7. **Entourage élargi** — bénévoles communautaires (FADOQ, Petits Frères) comme contacts de confiance avec droits limités.
8. **Mur de photos** — les proches déposent des photos qui s'affichent en plein écran (lien sans effort).

---

## 6. Architecture technique

- **Côté aîné** : PWA Fable 5 en **mode kiosque** (un seul écran, énormes cibles), installable sur tablette/téléphone bas de gamme. Conçue pour fonctionner sur du matériel ancien.
- **Appel vidéo** : **Jitsi Meet** (open source, WebRTC, chiffré, auto-hébergeable) intégré ; aucun compte requis ; salle pré-générée par contact.
- **Repli SMS** : pour les 40 % hors-ligne, le check-in et l'alerte passent par SMS (passerelle à bas coût ou via l'appareil d'un proche). La logique de check-in est la même (F# partagé), seul le transport change.
- **Détection d'inactivité** : minuterie locale + signaux d'activité (ouverture de l'app, check-in) ; seuils et escalade configurés par la famille ; **anti-faux-positifs** (fenêtres de sommeil, délais de grâce).
- **Vie privée** : données de check-in = renseignements de santé sous **Loi 25** → consentement explicite, minimisation, chiffrement, co-signature possible par un proche en cas de troubles cognitifs.

---

## 7. Modèle de données

```fsharp
type Humeur = Bien | CommeCiCommeÇa | PasFort

type CheckIn = { Le: System.DateTime; Humeur: Humeur option }

type Contact =
    { Nom: string; Photo: string; Téléphone: string
      Rôle: RôleContact }            // Famille | Bénévole | Service
and RôleContact = Famille | Bénévole | Service

type RègleInactivité =
    { SeuilHeures: int
      FenêtreSilencieuse: (int * int)   // ex. 22h–7h
      Escalade: Contact list }
```

---

## 8. Accessibilité (cible AAA, public très âgé)

- Un seul écran principal, **2 à 3 actions maximum**, pictogrammes + mots simples.
- Police 24 px+, contraste maximal, boutons pleine largeur.
- **Tout est parlé** : instructions et confirmations vocales.
- Aucune notion technique exposée ; aucune mise à jour à gérer par l'aîné.
- Parcours d'installation fait *par le proche* (guide dédié).

---

## 9. Risques et contraintes

| Risque | Gravité | Mitigation |
|--------|---------|-----------|
| **Faux positif d'inactivité** | Élevée | Seuils calibrés, fenêtres de sommeil, délai de grâce, escalade graduée |
| **Vie privée (Loi 25, santé)** | Élevée | Consentement explicite, chiffrement, minimisation, co-signature si besoin |
| **Fracture numérique** | Élevée | Repli SMS, mode hors-ligne, matériel bas de gamme supporté |
| **Capacité / consentement (démence)** | Moyenne | Co-décision proche/représentant, réglages verrouillés côté aîné |
| **Dépendance à un proche unique** | Moyenne | Escalade multi-contacts, entourage élargi (bénévoles) |

---

## 10. Feuille de route MVP

| Phase | Contenu | Effort indicatif |
|-------|---------|------------------|
| 0 — Cadrage | Tests d'utilisabilité avec aînés, choix matériel | 1 sem. |
| 1 — Socle kiosque | PWA mono-écran AAA, repli SMS | 1–2 sem. |
| 2 — Check-in + inactivité | Logique, seuils, escalade, alertes | 2 sem. |
| 3 — Appel vidéo | Intégration Jitsi sans compte | 1–2 sem. |
| 4 — Test terrain | 5–10 duos aîné/proche | 1–2 sem. |
| **MVP** | **Public, FR, hors-ligne** | **~6–8 sem.** |
| 5 — v1/v2 | Humeur, tableau famille, entourage élargi, mur photos | +4–6 sem. |

---

## 11. Plan de lancement open source

- **Licence** : MIT.
- **Partenaires visés** : FADOQ, Les Petits Frères, CIUSSS (soutien à domicile), bibliothèques et centres communautaires (installation assistée).
- **Distribution** : PWA + programme de tablettes reconditionnées avec les organismes ; ateliers d'installation famille.
- **Communauté** : guides d'installation, traductions, bénévoles « contacts de confiance ».

---

## 12. Sources

- Isolement des aînés au Québec (Petits Frères) — https://petitsfreres.ca/lisolement-des-aines-au-quebec-des-facteurs-et-des-statistiques-en-hausse/
- Jitsi Meet (visioconférence open source) — https://jitsi.org/
- WCAG 2.2 (W3C) — https://www.w3.org/TR/WCAG22/
- Commissariat à la vie privée (LPRPDE) — https://www.priv.gc.ca/
- Statistique Canada — vieillissement — https://www150.statcan.gc.ca/