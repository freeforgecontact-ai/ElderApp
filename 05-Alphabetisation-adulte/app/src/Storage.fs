module Storage

open Fable.Core
open Domain

// Persistance locale via localStorage -- aucune donnee ne quitte l'appareil.
// Identifiant opaque : pas de nom, pas de donnees personnelles (voir PLAN.md §7).

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative  // peut renvoyer null JS

[<Emit("localStorage.removeItem($0)")>]
let private removeItem (cle: string) : unit = jsNative

// Cles de persistance.
let private CLE_JARDIN         = "mots.jardin"
let private CLE_MODULE_EN_COURS = "mots.module.encours"

// --- Jardin (progression) ---

let sauverJardin (j: Jardin) =
    // Format CSV minimal ; en production, serialiser en JSON chiffre.
    let completes = j.ModulesCompletes |> String.concat ","
    let v = sprintf "%d|%s" j.EtapesTotal completes
    setItem CLE_JARDIN v

let chargerJardin () : Jardin =
    let v = getItem CLE_JARDIN
    if isNull v || v = "" then
        Jardin.vide
    else
        try
            match v.Split('|') with
            | [| etapes; completes |] ->
                let ids =
                    if completes = "" then []
                    else completes.Split(',') |> Array.toList
                { EtapesTotal = int etapes; ModulesCompletes = ids }
            | _ -> Jardin.vide
        with _ -> Jardin.vide

// --- Module en cours (reprise apres fermeture) ---

let sauverModuleEnCours (moduleId: ThemeId) (etapeIdx: int) =
    setItem CLE_MODULE_EN_COURS (sprintf "%s|%d" moduleId etapeIdx)

let chargerModuleEnCours () : (ThemeId * int) option =
    let v = getItem CLE_MODULE_EN_COURS
    if isNull v || v = "" then None
    else
        match v.Split('|') with
        | [| id; idx |] -> try Some (id, int idx) with _ -> None
        | _ -> None

let effacerModuleEnCours () =
    removeItem CLE_MODULE_EN_COURS
