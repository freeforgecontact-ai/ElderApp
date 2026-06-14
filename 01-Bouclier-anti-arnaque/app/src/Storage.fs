module Storage

open Fable.Core
open Domain

// Persistance locale simple (localStorage). Aucune donnée ne quitte l'appareil.
[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative // peut renvoyer null

let private CLE_PROCHE = "bouclier.proche"

let sauverProche (p: Proche) =
    // Format simple ; en production, chiffrer (voir 00-FONDATIONS §6).
    let v = sprintf "%s|%s|%b" p.Nom p.Telephone p.AlerteActive
    setItem CLE_PROCHE v

let chargerProche () : Proche option =
    let v = getItem CLE_PROCHE
    if isNull v || v = "" then
        None
    else
        match v.Split('|') with
        | [| nom; tel; actif |] ->
            Some { Nom = nom; Telephone = tel; AlerteActive = (actif = "true") }
        | _ -> None
