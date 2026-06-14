module Domain

/// Onglets de navigation principale.
type Onglet =
    | Accueil
    | Publier
    | Surplus
    | MesDons

/// Rôle de l'utilisateur dans l'app.
type Role =
    | Donneur
    | Organisme

// ---- Domaine métier ----

/// Contrainte de conservation du don.
/// Ambiant = température pièce ; Réfrigéré = 0-4 °C ; Congelé = -18 °C et moins.
type ContrainteFroid =
    | Ambiant
    | Refrigere    // Réfrigéré — sans accents dans le discriminant pour la sérialisation simple
    | Congele      // Congelé

module ContrainteFroid =
    let label =
        function
        | Ambiant   -> "Ambiant (température pièce)"
        | Refrigere -> "Réfrigéré (0–4 °C)"
        | Congele   -> "Congelé (−18 °C ou moins)"

    let labelCourt =
        function
        | Ambiant   -> "Ambiant"
        | Refrigere -> "Réfrigéré"
        | Congele   -> "Congelé"

    let emoji =
        function
        | Ambiant   -> "🌿"
        | Refrigere -> "❄️"
        | Congele   -> "🧊"

    let cssVariante =
        function
        | Ambiant   -> "badge-froid--ambiant"
        | Refrigere -> "badge-froid--refrigere"
        | Congele   -> "badge-froid--congele"

    /// Toutes les valeurs, pour les sélecteurs.
    let toutes = [ Ambiant; Refrigere; Congele ]

    /// Désérialise depuis la chaîne stockée.
    let ofString =
        function
        | "Refrigere" -> Refrigere
        | "Congele" -> Congele
        | _ -> Ambiant

// ---- Catégories d'aliments ----
type CategorieAliment =
    | FruitsLegumes
    | PainViennoiserie
    | PlatsCuisines
    | ProduitsLaitiers
    | ViandePoisson
    | EpicerieSeche
    | AutreCategorie

module CategorieAliment =
    let label =
        function
        | FruitsLegumes    -> "Fruits et légumes"
        | PainViennoiserie -> "Pain et viennoiserie"
        | PlatsCuisines    -> "Plats cuisinés"
        | ProduitsLaitiers -> "Produits laitiers"
        | ViandePoisson    -> "Viande / poisson"
        | EpicerieSeche    -> "Épicerie sèche"
        | AutreCategorie   -> "Autre"
    let emoji =
        function
        | FruitsLegumes    -> "🥦"
        | PainViennoiserie -> "🥖"
        | PlatsCuisines    -> "🍲"
        | ProduitsLaitiers -> "🥛"
        | ViandePoisson    -> "🍗"
        | EpicerieSeche    -> "🥫"
        | AutreCategorie   -> "📦"
    let froidParDefaut =
        function
        | PlatsCuisines | ProduitsLaitiers -> Refrigere
        | ViandePoisson -> Congele
        | _ -> Ambiant
    let toutes =
        [ FruitsLegumes; PainViennoiserie; PlatsCuisines; ProduitsLaitiers; ViandePoisson; EpicerieSeche; AutreCategorie ]

// ---- Statut d'un don ----
type StatutDon =
    | Disponible
    | Reserve of string
    | Recupere
    | Expire

module StatutDon =
    /// Libellé complet du statut.
    let label =
        function
        | Disponible  -> "Disponible"
        | Reserve org -> sprintf "Réservé par %s" org
        | Recupere    -> "Récupéré"
        | Expire      -> "Expiré"

    /// Libellé court pour les badges.
    let labelCourt =
        function
        | Disponible -> "Disponible"
        | Reserve _  -> "Réservé"
        | Recupere   -> "Récupéré"
        | Expire     -> "Expiré"

    /// Pictogramme du statut.
    let emoji =
        function
        | Disponible -> "🟢"
        | Reserve _  -> "🟡"
        | Recupere   -> "✅"
        | Expire     -> "⏳"

    /// Variante CSS du badge (cf. badge-froid--…).
    let cssVariante =
        function
        | Disponible -> "badge-statut--disponible"
        | Reserve _  -> "badge-statut--reserve"
        | Recupere   -> "badge-statut--recupere"
        | Expire     -> "badge-statut--expire"

// ---- Don ----
type Don =
    { Id: string
      Donneur: string
      TypeAliment: string
      Quantite: string
      Froid: ContrainteFroid
      FenetreFin: System.DateTime
      Lieu: string
      Statut: StatutDon }
