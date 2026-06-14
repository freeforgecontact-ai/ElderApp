module Domain

/// Onglets de navigation principale (mode kiosque ultra-simple).
type Onglet =
    | Accueil
    | Appeler
    | Humeur
    | Reglages

/// Humeur quotidienne exprimee par l'aine (3 etats, pas plus).
type Humeur =
    | Bien
    | CommeCiCommeCa
    | PasFort

module Humeur =
    let emoji =
        function
        | Bien -> "😊"
        | CommeCiCommeCa -> "😐"
        | PasFort -> "😔"

    let label =
        function
        | Bien -> "Je vais bien"
        | CommeCiCommeCa -> "Comme ci, comme ça"
        | PasFort -> "Pas fort aujourd'hui"

/// Check-in quotidien : date et humeur choisie (optionnelle).
type CheckIn =
    { Le: System.DateTime
      Humeur: Humeur option }

/// Role du contact dans l'entourage de l'aine.
type RoleContact =
    | Famille
    | Benevole
    | Service

module RoleContact =
    let label =
        function
        | Famille -> "Famille"
        | Benevole -> "Bénévole"
        | Service -> "Service"

/// Contact joignable d'un seul appui (lien tel:).
type Contact =
    { Nom: string
      Photo: string
      Telephone: string
      Role: RoleContact }

/// Regle d'inactivite configuree par la famille.
type RegleInactivite =
    { SeuilH: int
      SeuilHeures: int }
