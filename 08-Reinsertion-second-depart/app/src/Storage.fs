module Storage

open Fable.Core
open Domain

// Persistance locale (localStorage). Aucune donnée ne quitte l'appareil.
// TODO : Les données de contacts et de rappels sont potentiellement sensibles
//        (obligations légales, probation). Envisager un chiffrement AES-GCM
//        côté client pour les protéger si un tiers accède à l'appareil.

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative // peut renvoyer null

// ---- Clés de stockage ----

let private CLE_SITUATION   = "depart.situation"
let private CLE_ETAPES_48   = "depart.etapes48"
let private CLE_ETAPES      = "depart.etapes"
let private CLE_CONTACTS    = "depart.contacts"
let private CLE_RAPPELS     = "depart.rappels"

// ---- Situation ----

let private situationVersChaine =
    function
    | Detention  -> "detention"
    | Refuge     -> "refuge"
    | Dependance -> "dependance"

let private chaineVersSituation =
    function
    | "detention"  -> Some Detention
    | "refuge"     -> Some Refuge
    | "dependance" -> Some Dependance
    | _            -> None

let sauverSituation (s: Situation) =
    setItem CLE_SITUATION (situationVersChaine s)

let chargerSituation () : Situation option =
    let v = getItem CLE_SITUATION
    if isNull v || v = "" then None
    else chaineVersSituation v

// ---- Étapes (Set d'identifiants) ----

let private serSet (ids: Set<string>) =
    ids |> Set.toList |> String.concat "\n"

let private deserSet (v: string) : Set<string> =
    if isNull v || v = "" then Set.empty
    else v.Split('\n') |> Array.filter (fun s -> s <> "") |> Set.ofArray

let sauverEtapes48Faites (faites: Set<string>) =
    setItem CLE_ETAPES_48 (serSet faites)

let chargerEtapes48Faites () : Set<string> =
    deserSet (getItem CLE_ETAPES_48)

let sauverEtapesFaites (faites: Set<string>) =
    setItem CLE_ETAPES (serSet faites)

let chargerEtapesFaites () : Set<string> =
    deserSet (getItem CLE_ETAPES)

// ---- Contacts d'urgence ----

let private serContact (c: ContactUrgence) =
    sprintf "%s\x1F%s\x1F%s" (c.Role.Replace("\x1F","")) (c.Nom.Replace("\x1F","")) (c.Telephone.Replace("\x1F",""))

let private deserContact (ligne: string) : ContactUrgence option =
    let parts = ligne.Split('\x1F')
    if parts.Length >= 3 then
        Some { Role = parts.[0]; Nom = parts.[1]; Telephone = parts.[2] }
    else None

let sauverContacts (contacts: ContactUrgence list) =
    contacts |> List.map serContact |> String.concat "\n" |> setItem CLE_CONTACTS

let chargerContacts () : ContactUrgence list =
    let v = getItem CLE_CONTACTS
    if isNull v || v = "" then []
    else
        v.Split('\n')
        |> Array.filter (fun s -> s <> "")
        |> Array.choose deserContact
        |> Array.toList

// ---- Rappels ----

let private serRappel (r: Rappel) =
    sprintf "%s\x1F%s\x1F%s\x1F%b" (r.Id.Replace("\x1F","")) (r.Quoi.Replace("\x1F","")) (r.Quand.Replace("\x1F","")) r.Recurrent

let private deserRappel (ligne: string) : Rappel option =
    let parts = ligne.Split('\x1F')
    if parts.Length >= 4 then
        let recurrent = parts.[3] = "true"
        Some { Id = parts.[0]; Quoi = parts.[1]; Quand = parts.[2]; Recurrent = recurrent }
    else None

let sauverRappels (rappels: Rappel list) =
    rappels |> List.map serRappel |> String.concat "\n" |> setItem CLE_RAPPELS

let chargerRappels () : Rappel list =
    let v = getItem CLE_RAPPELS
    if isNull v || v = "" then []
    else
        v.Split('\n')
        |> Array.filter (fun s -> s <> "")
        |> Array.choose deserRappel
        |> Array.toList
