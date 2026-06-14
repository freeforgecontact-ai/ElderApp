module Domain

/// Onglets de navigation principale.
type Onglet =
    | Accueil
    | Ancrage
    | PlanSecurite
    | Journal

/// Exercice d'ancrage disponible.
type Exercice =
    | Respiration478
    | Ancrage54321

module Exercice =
    let label =
        function
        | Respiration478 -> "Respiration 4-7-8"
        | Ancrage54321   -> "Technique sensorielle 5-4-3-2-1"

    let description =
        function
        | Respiration478 ->
            "Inspire 4 secondes, retiens 7 secondes, expire lentement 8 secondes. Répète 3 à 4 cycles pour calmer le système nerveux."
        | Ancrage54321 ->
            "Nomme 5 choses que tu vois, 4 que tu touches, 3 que tu entends, 2 que tu sens, 1 que tu goûtes. Ça te ramène au moment présent."

/// Phase de la respiration 4-7-8.
type PhaseResp =
    | Inspirer    // 4 s
    | Retenir     // 7 s
    | Expirer     // 8 s
    | Pause       // 1 s entre cycles

module PhaseResp =
    let label =
        function
        | Inspirer -> "Inspire..."
        | Retenir  -> "Retiens..."
        | Expirer  -> "Expire doucement..."
        | Pause    -> "Respire librement"

    let duree =
        function
        | Inspirer -> 4
        | Retenir  -> 7
        | Expirer  -> 8
        | Pause    -> 1

/// Étape de la technique 5-4-3-2-1.
type EtapeSens =
    { Numero: int
      Sens:   string
      Verbe:  string
      Icone:  string }

module EtapesSens =
    let toutes =
        [ { Numero = 5; Sens = "choses que tu vois";    Verbe = "Regarde autour de toi";   Icone = "👁" }
          { Numero = 4; Sens = "choses que tu touches"; Verbe = "Touche quelque chose";     Icone = "✋" }
          { Numero = 3; Sens = "sons que tu entends";   Verbe = "Écoute attentivement";     Icone = "👂" }
          { Numero = 2; Sens = "odeurs que tu sens";    Verbe = "Respire et remarque";      Icone = "👃" }
          { Numero = 1; Sens = "goût que tu perçois";   Verbe = "Remarque en bouche";       Icone = "👅" } ]

/// Personne de soutien dans le plan de sécurité (Stanley-Brown).
type Contact =
    { Nom:       string
      Telephone: string }

/// Plan de sécurité personnel — modèle Stanley-Brown.
/// Toutes les sections sont des listes éditables sauvegardées localement.
type PlanSecurite =
    { SignauxAlerte:         string list
      StrategiesApaisantes:  string list
      PersonnesSoutien:      Contact list
      Professionnels:        Contact list
      SecuriserEnvironnement: string list }

module PlanSecurite =
    let vide =
        { SignauxAlerte          = []
          StrategiesApaisantes   = []
          PersonnesSoutien       = []
          Professionnels         = []
          SecuriserEnvironnement = [] }

/// Niveau d'humeur sur une échelle 1 à 5.
type Humeur = int

module Humeur =
    let emoji =
        function
        | 1 -> "😔"
        | 2 -> "😕"
        | 3 -> "😐"
        | 4 -> "🙂"
        | _ -> "😊"

    let label =
        function
        | 1 -> "Très difficile"
        | 2 -> "Difficile"
        | 3 -> "Neutre"
        | 4 -> "Bien"
        | _ -> "Très bien"

/// Entrée du journal d'humeur.
type EntreeJournal =
    { Id:     int
      Le:     System.DateTime
      Humeur: Humeur
      Note:   string }

/// Ressource de crise préchargée, accessible hors-ligne.
type RessourceCrise =
    { Nom:        string
      Numero:     string
      Lien:       string
      Description: string }

module RessourcesCrise =
    let toutes =
        [ { Nom         = "Ligne 988 — Prévention du suicide"
            Numero      = "988"
            Lien        = "tel:988"
            Description = "Disponible 24 h/24, 7 j/7 au Canada" }
          { Nom         = "1-866-APPELLE (Québec)"
            Numero      = "1-866-277-3553"
            Lien        = "tel:18662773553"
            Description = "Ligne québécoise de prévention du suicide" }
          { Nom         = "Texto 53 53 53 (Québec)"
            Numero      = "53 53 53"
            Lien        = "sms:535353"
            Description = "Envoie un texto si tu ne peux pas parler" }
          { Nom         = "suicide.ca — clavardage"
            Numero      = "suicide.ca"
            Lien        = "https://suicide.ca"
            Description = "Soutien par clavardage en ligne" } ]
