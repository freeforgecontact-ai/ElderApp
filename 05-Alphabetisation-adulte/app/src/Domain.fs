module Domain

/// Onglets de navigation principale.
/// Noms volontairement neutres : aucune référence à la lecture ou à l’école.
type Onglet =
    | Accueil
    | Module
    | Jardin

/// Identifiant de thème (clé des modules du quotidien).
type ThemeId = string

/// Une étape à l’intérieur d’un module (image + mot + son).
type Etape =
    { /// Texte du mot ou de la phrase — affiché en grand, jamais exigé pour avancer.
      Mot: string
      /// Emoji substitut MVP. En production : URL pictogramme ARASAAC (CC BY-NC-SA).
      /// API : https://api.arasaac.org/v1/pictograms/{id}
      Picto: string
      /// Texte lu par la synthèse vocale (peut différer légèrement du mot affiché).
      TexteVocal: string }

/// Un module d’apprentissage ancré dans la vie réelle.
type Module =
    { Id: ThemeId
      /// Picto principal affiché dans la grille d’accueil.
      Picto: string
      /// Libellé vocal du module (lu par le bouton 🔊, jamais exigé pour naviguer).
      LibelleVocal: string
      /// Étapes illustrées (1–6 max pour le MVP).
      Etapes: Etape list }

/// Progression de l’apprenant : opaque, invisible à l’apprenant, non stigmatisante.
/// Elle alimente uniquement le « jardin » (avatar qui grandit) — jamais un niveau chiffré.
type Jardin =
    { /// Nombre total d’étapes complétées depuis le début.
      EtapesTotal: int
      /// Identifiants des modules entièrement complétés au moins une fois.
      ModulesCompletes: ThemeId list }

module Jardin =
    let vide = { EtapesTotal = 0; ModulesCompletes = [] }

    /// Stade de croissance (usage interne — jamais affiché en texte à l’apprenant).
    /// 0-4 = graine, 5-14 = pousse, 15-29 = plante, 30-59 = arbuste, 60+ = arbre.
    let stade (j: Jardin) =
        match j.EtapesTotal with
        | n when n < 5  -> 0
        | n when n < 15 -> 1
        | n when n < 30 -> 2
        | n when n < 60 -> 3
        | _             -> 4

    /// Picto du stade courant (emojis MVP — pictos ARASAAC en production).
    let pictoStade (j: Jardin) =
        match stade j with
        | 0 -> "🌱"
        | 1 -> "🌿"
        | 2 -> "🌳"
        | 3 -> "🌲"
        | _ -> "🏡"

    /// Couleur d’ambiance associée au stade (animation progressive).
    let couleurStade (j: Jardin) =
        match stade j with
        | 0 -> "#C8E6C9"
        | 1 -> "#81C784"
        | 2 -> "#388E3C"
        | 3 -> "#1B5E20"
        | _ -> "#0F4C81"

    let enregistrerEtape (j: Jardin) =
        { j with EtapesTotal = j.EtapesTotal + 1 }

    let completerModule (moduleId: ThemeId) (j: Jardin) =
        if List.contains moduleId j.ModulesCompletes then j
        else { j with ModulesCompletes = moduleId :: j.ModulesCompletes }

/// Catalogue de modules du quotidien — MVP (8 modules).
/// En production : chargés depuis un fichier JSON versionné contribuable (CC-BY).
/// Les emojis sont des substituts temporaires ; les pictogrammes ARASAAC
/// (API : https://api.arasaac.org/) les remplaceront — mis en cache hors-ligne.
let catalogueModules : Module list =
    [ { Id = "medicaments"
        Picto = "💊"
        LibelleVocal = "Tes médicaments"
        Etapes =
          [ { Mot = "médicament";  Picto = "💊"; TexteVocal = "un médicament" }
            { Mot = "ordonnance";  Picto = "📋"; TexteVocal = "une ordonnance" }
            { Mot = "pharmacie";   Picto = "🏥"; TexteVocal = "la pharmacie" }
            { Mot = "matin";       Picto = "🌅"; TexteVocal = "le matin" }
            { Mot = "soir";        Picto = "🌙"; TexteVocal = "le soir" } ] }

      { Id = "transport"
        Picto = "🚌"
        LibelleVocal = "Prendre l’autobus"
        Etapes =
          [ { Mot = "autobus";     Picto = "🚌"; TexteVocal = "l’autobus" }
            { Mot = "arrêt";       Picto = "🚏"; TexteVocal = "l’arrêt d’autobus" }
            { Mot = "billet";      Picto = "🎫"; TexteVocal = "un billet" }
            { Mot = "horaire";     Picto = "🕐"; TexteVocal = "l’horaire" }
            { Mot = "destination"; Picto = "📍"; TexteVocal = "la destination" } ] }

      { Id = "argent"
        Picto = "💵"
        LibelleVocal = "Gérer ton argent"
        Etapes =
          [ { Mot = "chèque";  Picto = "📄"; TexteVocal = "un chèque" }
            { Mot = "banque";  Picto = "🏦"; TexteVocal = "la banque" }
            { Mot = "dépôt";   Picto = "💰"; TexteVocal = "un dépôt" }
            { Mot = "retrait"; Picto = "💳"; TexteVocal = "un retrait" }
            { Mot = "reçu";    Picto = "🧾"; TexteVocal = "un reçu" } ] }

      { Id = "formulaires"
        Picto = "📝"
        LibelleVocal = "Remplir un formulaire"
        Etapes =
          [ { Mot = "nom";        Picto = "🪪"; TexteVocal = "ton nom" }
            { Mot = "adresse";    Picto = "🏠"; TexteVocal = "ton adresse" }
            { Mot = "date";       Picto = "📅"; TexteVocal = "la date" }
            { Mot = "signature";  Picto = "✍️"; TexteVocal = "ta signature" }
            { Mot = "envoyer";    Picto = "📮"; TexteVocal = "envoyer le formulaire" } ] }

      { Id = "epicerie"
        Picto = "🛒"
        LibelleVocal = "Faire l’épicerie"
        Etapes =
          [ { Mot = "liste";   Picto = "📋"; TexteVocal = "une liste d’épicerie" }
            { Mot = "prix";    Picto = "🏷️"; TexteVocal = "le prix" }
            { Mot = "caisse";  Picto = "🧾"; TexteVocal = "la caisse" }
            { Mot = "monnaie"; Picto = "🪙"; TexteVocal = "la monnaie" }
            { Mot = "sac";     Picto = "🛍️"; TexteVocal = "un sac" } ] }

      { Id = "urgences"
        Picto = "🚨"
        LibelleVocal = "En cas d’urgence"
        Etapes =
          [ { Mot = "urgence";    Picto = "🚨"; TexteVocal = "une urgence" }
            { Mot = "9-1-1";      Picto = "📞"; TexteVocal = "composer le neuf-un-un" }
            { Mot = "adresse";    Picto = "📍"; TexteVocal = "dire ton adresse" }
            { Mot = "ambulance";  Picto = "🚑"; TexteVocal = "une ambulance" }
            { Mot = "pompiers";   Picto = "🚒"; TexteVocal = "les pompiers" } ] }

      { Id = "courrier"
        Picto = "✉️"
        LibelleVocal = "Lire ton courrier"
        Etapes =
          [ { Mot = "enveloppe";   Picto = "✉️"; TexteVocal = "une enveloppe" }
            { Mot = "lettre";      Picto = "📩"; TexteVocal = "une lettre" }
            { Mot = "avis";        Picto = "⚠️"; TexteVocal = "un avis important" }
            { Mot = "date limite"; Picto = "📅"; TexteVocal = "la date limite" }
            { Mot = "répondre";    Picto = "↩️"; TexteVocal = "répondre" } ] }

      { Id = "sante"
        Picto = "🩺"
        LibelleVocal = "Ta santé"
        Etapes =
          [ { Mot = "médecin";      Picto = "🩺"; TexteVocal = "le médecin" }
            { Mot = "rendez-vous";  Picto = "📅"; TexteVocal = "un rendez-vous" }
            { Mot = "carte soleil"; Picto = "🪪"; TexteVocal = "ta carte soleil" }
            { Mot = "symptôme";     Picto = "🤢"; TexteVocal = "un symptôme" }
            { Mot = "guérir";       Picto = "😊"; TexteVocal = "guérir" } ] } ]
