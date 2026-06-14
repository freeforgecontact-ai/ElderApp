content = """module Storage

open Fable.Core
open Domain

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative

let private CLE_STATUT = "boussole.statut"
let private CLE_ETAPES = "boussole.etapes"

let private statutVersChaine =
    function
    | RefugieReinstalle     -> "refugie"
    | DemandeurAsile        -> "demandeur"
    | ResidentPermanent     -> "rp"
    | CUAET                 -> "cuaet"
    | TravailleurTemporaire -> "travailleur"
    | Etudiant              -> "etudiant"

let private chaineVersStatut =
    function
    | "refugie"     -> Some RefugieReinstalle
    | "demandeur"   -> Some DemandeurAsile
    | "rp"          -> Some ResidentPermanent
    | "cuaet"       -> Some CUAET
    | "travailleur" -> Some TravailleurTemporaire
    | "etudiant"    -> Some Etudiant
    | _             -> None

let sauverStatut (s: Statut) =
    setItem CLE_STATUT (statutVersChaine s)

let chargerStatut () : Statut option =
    let v = getItem CLE_STATUT
    if isNull v || v = "" then None
    else chaineVersStatut v

let private separateur = "|"

let sauverEtapesFaites (ids: string list) =
    setItem CLE_ETAPES (ids |> String.concat separateur)

let chargerEtapesFaites () : Set<string> =
    let v = getItem CLE_ETAPES
    if isNull v || v = "" then Set.empty
    else
        v.Split(separateur) |> Array.filter (fun s -> s <> "") |> Set.ofArray
"""
with open("/tmp/b3/src/Storage.fs", "w") as f:
    f.write(content)
lines = sum(1 for _ in open("/tmp/b3/src/Storage.fs"))
print("Storage.fs: " + str(lines) + " lines")