module Storage

open Fable.Core
open Domain

// Persistance locale simple (localStorage). Aucune donnée ne quitte l'appareil.
// TODO (production) : chiffrer les données sensibles (AES-GCM, voir 00-FONDATIONS-COMMUNES §6).
[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative // peut renvoyer null

// Identifiant pseudo-unique (suffisant pour le scaffold local ; UUID v4 côté serveur en production).
[<Emit("(Date.now().toString(36) + Math.random().toString(36).slice(2, 8))")>]
let private uid () : string = jsNative

// ---- Clés de stockage ----
let private CLE_ROLE   = "recolte.role"
let private CLE_DONS   = "recolte.dons"
let private CLE_NOM    = "recolte.nom"

// ---- Rôle ----

let sauverRole (role: Role) =
    let v = match role with Donneur -> "Donneur" | Organisme -> "Organisme"
    setItem CLE_ROLE v

let chargerRole () : Role option =
    let v = getItem CLE_ROLE
    if isNull v || v = "" then None
    else
        match v with
        | "Donneur"   -> Some Donneur
        | "Organisme" -> Some Organisme
        | _           -> None

// ---- Nom du donneur ----

let sauverNom (nom: string) =
    setItem CLE_NOM nom

let chargerNom () : string =
    let v = getItem CLE_NOM
    if isNull v then "" else v

// ---- Génération d'identifiant ----

/// Génère un identifiant local unique pour un don publié hors-ligne.
let genId () : string =
    "don-loc-" + uid ()

// ---- Sérialisation des dons ----
// Format texte simple, sans dépendance JSON externe.
//   - les champs d'un don sont séparés par le caractère unité (U+241F encodé "")
//   - les dons sont séparés par le caractère d'enregistrement (U+241E encodé "")
// Ces séparateurs n'apparaissent pas dans une saisie clavier normale.

let private SEP_CHAMP = ""
let private SEP_DON   = ""

let private froidVersTexte =
    function
    | Ambiant   -> "Ambiant"
    | Refrigere -> "Refrigere"
    | Congele   -> "Congele"

let private statutVersTexte =
    function
    | Disponible  -> "Disponible|"
    | Reserve org -> "Reserve|" + org
    | Recupere    -> "Recupere|"
    | Expire      -> "Expire|"

let private statutDepuisTexte (s: string) : StatutDon =
    let i = s.IndexOf('|')
    let tag, reste =
        if i < 0 then s, ""
        else s.Substring(0, i), s.Substring(i + 1)
    match tag with
    | "Reserve"  -> Reserve reste
    | "Recupere" -> Recupere
    | "Expire"   -> Expire
    | _          -> Disponible

let private donVersTexte (d: Don) : string =
    String.concat SEP_CHAMP
        [ d.Id
          d.Donneur
          d.TypeAliment
          d.Quantite
          froidVersTexte d.Froid
          // ticks en chaîne -> reconstruction exacte de la date
          string d.FenetreFin.Ticks
          d.Lieu
          statutVersTexte d.Statut ]

let private donDepuisTexte (ligne: string) : Don option =
    let champs = ligne.Split([| SEP_CHAMP |], System.StringSplitOptions.None)
    if champs.Length < 8 then None
    else
        let ticks =
            match System.Int64.TryParse champs.[5] with
            | true, t -> t
            | _ -> System.DateTime.Now.Ticks
        Some
            { Id          = champs.[0]
              Donneur     = champs.[1]
              TypeAliment = champs.[2]
              Quantite    = champs.[3]
              Froid       = ContrainteFroid.ofString champs.[4]
              FenetreFin  = System.DateTime(ticks)
              Lieu        = champs.[6]
              Statut      = statutDepuisTexte champs.[7] }

/// Sauvegarde la liste des dons publiés localement.
let sauverDons (dons: Don list) : unit =
    let texte =
        dons
        |> List.map donVersTexte
        |> String.concat SEP_DON
    setItem CLE_DONS texte

/// Recharge les dons publiés localement (liste vide si rien de stocké).
let chargerDons () : Don list =
    let v = getItem CLE_DONS
    if isNull v || v = "" then []
    else
        v.Split([| SEP_DON |], System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.toList
        |> List.choose donDepuisTexte
