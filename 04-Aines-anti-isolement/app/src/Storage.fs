module Storage

open Fable.Core
open Domain

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative

let private CLE_CHECKIN   = "lien.checkin"
let private CLE_CONTACTS  = "lien.contacts"
let private CLE_REGLE     = "lien.regle"

let sauverCheckIn (c: CheckIn) =
    let humeurStr =
        match c.Humeur with
        | Some Bien            -> "bien"
        | Some CommeCiCommeCa  -> "commeci"
        | Some PasFort         -> "pasfort"
        | None                 -> ""
    let v = sprintf "%s|%s" (c.Le.ToString("o")) humeurStr
    setItem CLE_CHECKIN v

let chargerCheckIn () : CheckIn option =
    let s = getItem CLE_CHECKIN
    if isNull s || s = "" then
        None
    else
        let parts = s.Split('|')
        if parts.Length >= 1 then
            match System.DateTime.TryParse(parts.[0]) with
            | true, d ->
                let humeur =
                    if parts.Length >= 2 then
                        match parts.[1] with
                        | "bien"    -> Some Bien
                        | "commeci" -> Some CommeCiCommeCa
                        | "pasfort" -> Some PasFort
                        | _         -> None
                    else None
                Some { Le = d; Humeur = humeur }
            | _ -> None
        else None

let sauverContacts (contacts: Contact list) =
    let lignes =
        contacts
        |> List.map (fun c ->
            let role =
                match c.Role with
                | Famille  -> "f"
                | Benevole -> "b"
                | Service  -> "s"
            sprintf "%s~%s~%s~%s" c.Nom c.Telephone role c.Photo)
        |> String.concat ";"
    setItem CLE_CONTACTS lignes

let chargerContacts () : Contact list =
    let s = getItem CLE_CONTACTS
    if isNull s || s = "" then []
    else
        s.Split(';')
        |> Array.toList
        |> List.choose (fun ligne ->
            let p : string[] = ligne.Split('~')
            if p.Length >= 3 then
                let role =
                    match p.[2] with
                    | "b" -> Benevole
                    | "s" -> Service
                    | _   -> Famille
                let photo = if p.Length >= 4 then p.[3] else ""
                Some { Nom = p.[0]; Telephone = p.[1]; Role = role; Photo = photo }
            else None)

let sauverRegle (r: RegleInactivite) =
    setItem CLE_REGLE (string r.SeuilH)

let chargerRegle () : RegleInactivite =
    let s = getItem CLE_REGLE
    let h =
        if isNull s || s = "" then 24
        else
            match System.Int32.TryParse(s) with
            | true, v -> v
            | _ -> 24
    { SeuilH = h; SeuilHeures = h }
