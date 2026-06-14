module Domain

/// Onglets de navigation principale.
type Onglet =
    | Accueil
    | Verifier
    | Apprendre
    | Proches

/// Typologies de fraude documentées par le Centre antifraude du Canada.
type TypeFraude =
    | GrandsParents
    | Amoureuse
    | FauxSoutien
    | FausseInstitution
    | AutreFraude

module TypeFraude =
    let label =
        function
        | GrandsParents -> "Fraude « grands-parents »"
        | Amoureuse -> "Fraude amoureuse"
        | FauxSoutien -> "Faux soutien technique"
        | FausseInstitution -> "Fausse institution (banque, gouvernement, police)"
        | AutreFraude -> "Autre arnaque"

/// Résultat d'une vérification.
type Verdict =
    | Inconnu
    | Suspect of TypeFraude * string
    | Sur

/// Proche de confiance prévenu en cas d'alerte (opt-in).
type Proche =
    { Nom: string
      Telephone: string
      AlerteActive: bool }

/// Réponses aux 3 questions de la pause « avant de payer ».
type ReponsePause =
    { CartesCadeaux: bool
      Urgent: bool
      Secret: bool }

module ReponsePause =
    let vide = { CartesCadeaux = false; Urgent = false; Secret = false }
    /// Nombre de signaux d'alerte cochés (0 à 3).
    let risque (r: ReponsePause) =
        (if r.CartesCadeaux then 1 else 0)
        + (if r.Urgent then 1 else 0)
        + (if r.Secret then 1 else 0)
