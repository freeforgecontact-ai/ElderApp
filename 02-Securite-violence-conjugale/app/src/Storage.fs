module Storage

open Fable.Core
open Domain

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative

[<Emit("localStorage.clear()")>]
let private clearAll () : unit = jsNative

[<Literal>]
let private cleEtapes     = "sec_etapes_v1"
[<Literal>]
let private cleCamouflage = "sec_camouflage_v1"

let private boolStr (b: bool) = if b then "true" else "false"
let private parseBool (s: string) = s.Trim() = "true"

let private getField (s: string) (key: string) : string =
    let prefix = sprintf "\"%s\":" key
    match s.IndexOf(prefix) with
    | -1 -> ""
    | i  ->
        let rest = s.Substring(i + prefix.Length).Trim()
        if rest.StartsWith("\"") then
            let end2 = rest.IndexOf("\"", 1)
            if end2 > 0 then rest.Substring(1, end2 - 1) else ""
        else
            let end2 = rest.IndexOfAny([|','; '}'; ' '|])
            if end2 > 0 then rest.Substring(0, end2) else rest

let chargerEtapes () : EtapeSortie list =
    let raw = getItem cleEtapes
    if isNull raw || raw = "" then EtapeSortie.defaut
    else
        try
            let inner = raw.Trim().TrimStart('[').TrimEnd(']')
            if inner = "" then EtapeSortie.defaut
            else
                let items =
                    inner.Split([|"},"|], System.StringSplitOptions.RemoveEmptyEntries)
                    |> Array.toList
                    |> List.map (fun s -> s.TrimStart('{').TrimEnd('}').Trim())
                    |> List.choose (fun s ->
                        try
                            Some { Id = int (getField s "id"); Libelle = getField s "libelle"; Faite = parseBool (getField s "faite"); Categorie = getField s "categorie" }
                        with _ -> None)
                if List.isEmpty items then EtapeSortie.defaut else items
        with _ -> EtapeSortie.defaut

let chargerCamouflage () : Camouflage =
    let raw = getItem cleCamouflage
    if isNull raw || raw = "" then Camouflage.defaut
    else
        try
            let s = raw.Trim().TrimStart('{').TrimEnd('}')
            { CamouflageActif = parseBool (getField s "camouflageActif")
              NomAffiche      = getField s "nomAffiche"
              IconeId         = getField s "iconeId"
              NipActif        = parseBool (getField s "nipActif")
              NipHash         = getField s "nipHash" }
        with _ -> Camouflage.defaut

let sauverEtapes (etapes: EtapeSortie list) : unit =
    let items =
        etapes
        |> List.map (fun e ->
            sprintf "{\"id\":%d,\"libelle\":\"%s\",\"faite\":%s,\"categorie\":\"%s\"}"
                e.Id e.Libelle (boolStr e.Faite) e.Categorie)
        |> String.concat ","
    setItem cleEtapes (sprintf "[%s]" items)

let sauverCamouflage (c: Camouflage) : unit =
    let json =
        sprintf "{\"camouflageActif\":%s,\"nomAffiche\":\"%s\",\"iconeId\":\"%s\",\"nipActif\":%s,\"nipHash\":\"%s\"}"
            (boolStr c.CamouflageActif) c.NomAffiche c.IconeId (boolStr c.NipActif) c.NipHash
    setItem cleCamouflage json

let effacerTout () : unit = clearAll ()
