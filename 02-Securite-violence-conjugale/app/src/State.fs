module State

open Fable.Core
open Elmish
open Domain

[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

type Model =
    { Onglet: Onglet
      Etapes: EtapeSortie list
      Camouflage: Camouflage
      BrouillonNom: string
      BrouillonIconeId: string
      BrouillonNip: string
      BrouillonNipConfirm: string
      EffacementConfirme: bool
      EffacementEnCours: bool
      FiltreRegion: string option
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | BasculerEtape of int
    | BrouillonNomChange of string
    | BrouillonIconeChange of string
    | BrouillonNipChange of string
    | BrouillonNipConfirmChange of string
    | EnregistrerCamouflage
    | DemanderEffacement
    | ConfirmerEffacement
    | AnnulerEffacement
    | FiltrerRegion of string option
    | ReseauChange of bool

let init () : Model * Cmd<Msg> =
    let etapes     = Storage.chargerEtapes ()
    let camouflage = Storage.chargerCamouflage ()
    { Onglet              = Accueil
      Etapes              = etapes
      Camouflage          = camouflage
      BrouillonNom        = camouflage.NomAffiche
      BrouillonIconeId    = camouflage.IconeId
      BrouillonNip        = ""
      BrouillonNipConfirm = ""
      EffacementConfirme  = false
      EffacementEnCours   = false
      FiltreRegion        = None
      EnLigne             = estEnLigne () },
    Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with

    | Aller o ->
        { model with Onglet = o; EffacementConfirme = false; EffacementEnCours = false }, Cmd.none

    | BasculerEtape id ->
        let etapes2 = model.Etapes |> List.map (fun e -> if e.Id = id then { e with Faite = not e.Faite } else e)
        Storage.sauverEtapes etapes2
        { model with Etapes = etapes2 }, Cmd.none

    | BrouillonNomChange v        -> { model with BrouillonNom = v }, Cmd.none
    | BrouillonIconeChange v      -> { model with BrouillonIconeId = v }, Cmd.none
    | BrouillonNipChange v        -> { model with BrouillonNip = v }, Cmd.none
    | BrouillonNipConfirmChange v -> { model with BrouillonNipConfirm = v }, Cmd.none

    | EnregistrerCamouflage ->
        let cam2 = { model.Camouflage with NomAffiche = model.BrouillonNom; IconeId = model.BrouillonIconeId }
        Storage.sauverCamouflage cam2
        { model with Camouflage = cam2; BrouillonNip = ""; BrouillonNipConfirm = "" }, Cmd.none

    | DemanderEffacement -> { model with EffacementConfirme = true }, Cmd.none

    | ConfirmerEffacement ->
        Storage.effacerTout ()
        { model with EffacementEnCours = true; EffacementConfirme = false
                     Etapes = EtapeSortie.defaut; Camouflage = Camouflage.defaut
                     BrouillonNom = Camouflage.defaut.NomAffiche; BrouillonIconeId = Camouflage.defaut.IconeId
                     BrouillonNip = ""; BrouillonNipConfirm = "" }, Cmd.none

    | AnnulerEffacement -> { model with EffacementConfirme = false }, Cmd.none
    | FiltrerRegion r   -> { model with FiltreRegion = r }, Cmd.none
    | ReseauChange enLigne -> { model with EnLigne = enLigne }, Cmd.none
