module Storage

open Fable.Core
open Domain

// ---- Primitives localStorage (aucune donnée ne quitte l'appareil) ----

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative

[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative  // peut renvoyer null

[<Emit("localStorage.removeItem($0)")>]
let private removeItem (cle: string) : unit = jsNative

// ---- Clés ----
let private CLE_PLAN        = "ancrage.plan"
let private CLE_JOURNAL     = "ancrage.journal"
let private CLE_COMPTEUR    = "ancrage.journal.id"

// ---- Plan de sécurité ----

/// Sérialise une liste de chaînes en JSON minimal (séparateur \x1F pour éviter les conflits).
let private serListe (lst: string list) =
    lst |> List.map (fun s -> s.Replace("\x1F", " ").Replace("|", " ")) |> String.concat "\x1F"

let private deserListe (s: string) =
    if s = "" then []
    else s.Split('\x1F') |> Array.toList |> List.filter (fun x -> x <> "")

let private serContact (c: Contact) =
    sprintf "%s\x1E%s" (c.Nom.Replace("\x1E", " ")) (c.Telephone.Replace("\x1E", " "))

let private deserContact (s: string) =
    match s.Split('\x1E') with
    | [| n; t |] -> Some { Nom = n; Telephone = t }
    | _           -> None

let private serContacts (lst: Contact list) =
    lst |> List.map serContact |> String.concat "\x1F"

let private deserContacts (s: string) =
    if s = "" then []
    else
        s.Split('\x1F')
        |> Array.toList
        |> List.choose deserContact

let sauverPlan (p: PlanSecurite) =
    // Format : 5 sections séparées par || ; chaque section = items séparés par \x1F
    let v =
        [| serListe p.SignauxAlerte
           serListe p.StrategiesApaisantes
           serContacts p.PersonnesSoutien
           serContacts p.Professionnels
           serListe p.SecuriserEnvironnement |]
        |> String.concat "||"
    setItem CLE_PLAN v

let chargerPlan () : PlanSecurite =
    let v = getItem CLE_PLAN
    if isNull v || v = "" then
        PlanSecurite.vide
    else
        match v.Split([| "||" |], System.StringSplitOptions.None) with
        | [| sa; strat; ps; prof; env |] ->
            { SignauxAlerte          = deserListe sa
              StrategiesApaisantes   = deserListe strat
              PersonnesSoutien       = deserContacts ps
              Professionnels         = deserContacts prof
              SecuriserEnvironnement = deserListe env }
        | _ -> PlanSecurite.vide

// ---- Journal d'humeur ----

let private prochainId () =
    let v = getItem CLE_COMPTEUR
    let id = if isNull v || v = "" then 1 else (int v) + 1
    setItem CLE_COMPTEUR (string id)
    id

let private serEntree (e: EntreeJournal) =
    sprintf "%d\x1E%s\x1E%d\x1E%s"
        e.Id
        (e.Le.ToString("o"))   // ISO 8601
        e.Humeur
        (e.Note.Replace("\x1E", " ").Replace("||", "  "))

let private deserEntree (s: string) =
    match s.Split('\x1E') with
    | [| id; date; hum; note |] ->
        match System.Int32.TryParse id, System.DateTime.TryParse date, System.Int32.TryParse hum with
        | (true, iVal), (true, dVal), (true, hVal) ->
            Some { Id = iVal; Le = dVal; Humeur = hVal; Note = note }
        | _ -> None
    | _ -> None

let chargerJournal () : EntreeJournal list =
    let v = getItem CLE_JOURNAL
    if isNull v || v = "" then []
    else
        v.Split([| "||" |], System.StringSplitOptions.None)
        |> Array.toList
        |> List.choose deserEntree
        |> List.sortByDescending (fun e -> e.Le)

let ajouterEntree (humeur: Humeur) (note: string) : EntreeJournal =
    let entrees = chargerJournal ()
    let nouvelle =
        { Id     = prochainId ()
          Le     = System.DateTime.Now
          Humeur = humeur
          Note   = note }
    let toutes = nouvelle :: entrees |> List.sortByDescending (fun e -> e.Le)
    let v = toutes |> List.map serEntree |> String.concat "||"
    setItem CLE_JOURNAL v
    nouvelle

/// Export CSV (données brutes — l'usager est propriétaire).
let exporterJournalCsv () : string =
    let entrees = chargerJournal ()
    let lignes =
        entrees
        |> List.map (fun e ->
            sprintf "\"%s\",\"%d\",\"%s\""
                (e.Le.ToString("yyyy-MM-dd HH:mm"))
                e.Humeur
                (e.Note.Replace("\"", "\"\"")))
    "\"Date\",\"Humeur (1-5)\",\"Note\"\n" + (lignes |> String.concat "\n")
