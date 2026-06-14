module State

open Fable.Core
open Elmish
open Domain

// État réseau, lu sans dépendance externe (offline-first).
[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

type Model =
    { Onglet: Onglet
      Saisie: string
      Verdict: Verdict option
      Proche: Proche option
      Brouillon: Proche
      Pause: ReponsePause
      PauseProchePrevenu: bool
      QuizReponse: int option
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | SaisieChangee of string
    | LancerVerification
    | OuvrirPause
    | BasculePauseQuestion of int
    | PrevenirProche
    | BrouillonNom of string
    | BrouillonTel of string
    | BrouillonAlerte of bool
    | EnregistrerProche
    | RepondreQuiz of int
    | ReseauChange of bool

let init () : Model * Cmd<Msg> =
    let proche = Storage.chargerProche ()
    let brouillon =
        proche |> Option.defaultValue { Nom = ""; Telephone = ""; AlerteActive = true }
    { Onglet = Accueil
      Saisie = ""
      Verdict = None
      Proche = proche
      Brouillon = brouillon
      Pause = ReponsePause.vide
      PauseProchePrevenu = false
      QuizReponse = None
      EnLigne = estEnLigne () },
    Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Aller o -> { model with Onglet = o }, Cmd.none
    | SaisieChangee s -> { model with Saisie = s; Verdict = None }, Cmd.none
    | LancerVerification -> { model with Verdict = Some(Cafc.verifier model.Saisie) }, Cmd.none
    | OuvrirPause ->
        { model with Onglet = Verifier; Pause = ReponsePause.vide; PauseProchePrevenu = false }, Cmd.none
    | BasculePauseQuestion i ->
        let p = model.Pause
        let p2 =
            match i with
            | 0 -> { p with CartesCadeaux = not p.CartesCadeaux }
            | 1 -> { p with Urgent = not p.Urgent }
            | _ -> { p with Secret = not p.Secret }
        { model with Pause = p2 }, Cmd.none
    | PrevenirProche -> { model with PauseProchePrevenu = true }, Cmd.none
    | BrouillonNom v -> { model with Brouillon = { model.Brouillon with Nom = v } }, Cmd.none
    | BrouillonTel v -> { model with Brouillon = { model.Brouillon with Telephone = v } }, Cmd.none
    | BrouillonAlerte v -> { model with Brouillon = { model.Brouillon with AlerteActive = v } }, Cmd.none
    | EnregistrerProche ->
        Storage.sauverProche model.Brouillon
        { model with Proche = Some model.Brouillon }, Cmd.none
    | RepondreQuiz i -> { model with QuizReponse = Some i }, Cmd.none
    | ReseauChange b -> { model with EnLigne = b }, Cmd.none
