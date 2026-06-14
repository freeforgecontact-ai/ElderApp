module State

open Fable.Core
open Elmish
open Domain

[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

// Sortie rapide : remplace la page par un site neutre, sans laisser
// d'entrée dans l'historique. Geste de sécurité de base.
[<Emit("location.replace('https://www.google.com')")>]
let sortirViteJs () : unit = jsNative

type Model =
    { Onglet: Onglet
      Etapes: EtapeSortie list
      NomAffiche: string
      IconeId: string
      // --- verrou / chiffrement ---
      NipActif: bool
      Deverrouille: bool
      Cle: obj option
      SelB64: string
      BlobAttente: string
      SaisieNip: string
      SaisieNipConfirm: string
      ErreurNip: string option
      // --- réglages / effacement ---
      BrouillonNom: string
      EffacementConfirme: bool
      EffacementEnCours: bool
      // --- divers ---
      FiltreRegion: string option
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | BasculerEtape of int
    | BrouillonNomChange of string
    | EnregistrerCamouflage
    | SaisieNipChange of string
    | SaisieNipConfirmChange of string
    | TenterDeverrouillage
    | CleDerivee of obj
    | DeverrouillageResultat of obj * string
    | DefinirNip
    | NipDefini of obj * string
    | RetirerNip
    | PersistOk
    | DemanderEffacement
    | ConfirmerEffacement
    | AnnulerEffacement
    | EffacerEtQuitter
    | SortieRapide
    | FiltrerRegion of string option
    | ReseauChange of bool

// Persiste l'état courant : chiffré si un NIP est actif, sinon en clair.
let private persister (m: Model) : Cmd<Msg> =
    let json = Storage.serialiser m.Etapes m.NomAffiche m.IconeId
    match m.Cle with
    | Some cle -> Cmd.OfPromise.attempt (fun () -> Storage.chiffrerEtStocker cle json m.SelB64) () (fun _ -> PersistOk)
    | None -> Storage.sauverClair json; Cmd.none

let init () : Model * Cmd<Msg> =
    let baseModel etapes nom icone nipActif deverrouille sel blob =
        { Onglet = Accueil
          Etapes = etapes
          NomAffiche = nom
          IconeId = icone
          NipActif = nipActif
          Deverrouille = deverrouille
          Cle = None
          SelB64 = sel
          BlobAttente = blob
          SaisieNip = ""
          SaisieNipConfirm = ""
          ErreurNip = None
          BrouillonNom = nom
          EffacementConfirme = false
          EffacementEnCours = false
          FiltreRegion = None
          EnLigne = estEnLigne () }
    match Storage.charger () with
    | Storage.Vide ->
        baseModel EtapeSortie.defaut Camouflage.defaut.NomAffiche Camouflage.defaut.IconeId false true "" "", Cmd.none
    | Storage.Clair json ->
        let e, n, i = Storage.parser json
        baseModel e n i false true "" "", Cmd.none
    | Storage.Chiffre (sel, blob) ->
        baseModel EtapeSortie.defaut Camouflage.defaut.NomAffiche Camouflage.defaut.IconeId true false sel blob, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with

    | Aller o ->
        { model with Onglet = o; EffacementConfirme = false; EffacementEnCours = false }, Cmd.none

    | BasculerEtape id ->
        let etapes2 = model.Etapes |> List.map (fun e -> if e.Id = id then { e with Faite = not e.Faite } else e)
        let m = { model with Etapes = etapes2 }
        m, persister m

    | BrouillonNomChange v -> { model with BrouillonNom = v }, Cmd.none

    | EnregistrerCamouflage ->
        let m = { model with NomAffiche = (if model.BrouillonNom.Trim() = "" then Camouflage.defaut.NomAffiche else model.BrouillonNom) }
        m, persister m

    | SaisieNipChange v        -> { model with SaisieNip = v; ErreurNip = None }, Cmd.none
    | SaisieNipConfirmChange v -> { model with SaisieNipConfirm = v; ErreurNip = None }, Cmd.none

    | TenterDeverrouillage ->
        if model.SaisieNip = "" then model, Cmd.none
        else
            let nip = model.SaisieNip
            model, Cmd.OfPromise.perform (fun () -> Storage.deriverCle nip model.SelB64) () CleDerivee

    | CleDerivee cle ->
        model, Cmd.OfPromise.perform (fun () -> Storage.dechiffrer cle model.BlobAttente) () (fun json -> DeverrouillageResultat(cle, json))

    | DeverrouillageResultat (cle, json) ->
        if json = "" then
            { model with ErreurNip = Some "NIP incorrect. Réessayez."; SaisieNip = "" }, Cmd.none
        else
            let e, n, i = Storage.parser json
            { model with Deverrouille = true; Cle = Some cle; Etapes = e; NomAffiche = n; IconeId = i
                         BrouillonNom = n; SaisieNip = ""; ErreurNip = None }, Cmd.none

    | DefinirNip ->
        if model.SaisieNip.Length < 4 then
            { model with ErreurNip = Some "Choisissez un NIP d'au moins 4 chiffres." }, Cmd.none
        elif model.SaisieNip <> model.SaisieNipConfirm then
            { model with ErreurNip = Some "Les deux NIP ne correspondent pas." }, Cmd.none
        else
            let sel = Storage.nouveauSel ()
            model, Cmd.OfPromise.perform (fun () -> Storage.deriverCle model.SaisieNip sel) () (fun cle -> NipDefini(cle, sel))

    | NipDefini (cle, sel) ->
        let m = { model with Cle = Some cle; SelB64 = sel; NipActif = true; Deverrouille = true
                             SaisieNip = ""; SaisieNipConfirm = ""; ErreurNip = None }
        let json = Storage.serialiser m.Etapes m.NomAffiche m.IconeId
        m, Cmd.OfPromise.attempt (fun () -> Storage.chiffrerEtStocker cle json sel) () (fun _ -> PersistOk)

    | RetirerNip ->
        let json = Storage.serialiser model.Etapes model.NomAffiche model.IconeId
        Storage.sauverClair json
        { model with Cle = None; NipActif = false; SelB64 = ""; SaisieNip = ""; SaisieNipConfirm = "" }, Cmd.none

    | PersistOk -> model, Cmd.none

    | DemanderEffacement -> { model with EffacementConfirme = true }, Cmd.none

    | ConfirmerEffacement ->
        Storage.effacerTout ()
        { model with EffacementEnCours = true; EffacementConfirme = false
                     Etapes = EtapeSortie.defaut; NomAffiche = Camouflage.defaut.NomAffiche; IconeId = Camouflage.defaut.IconeId
                     BrouillonNom = Camouflage.defaut.NomAffiche; Cle = None; NipActif = false; SelB64 = "" }, Cmd.none

    | AnnulerEffacement -> { model with EffacementConfirme = false }, Cmd.none

    | EffacerEtQuitter ->
        Storage.effacerTout ()
        sortirViteJs ()
        model, Cmd.none

    | SortieRapide ->
        sortirViteJs ()
        model, Cmd.none

    | FiltrerRegion r   -> { model with FiltreRegion = r }, Cmd.none
    | ReseauChange enLigne -> { model with EnLigne = enLigne }, Cmd.none
