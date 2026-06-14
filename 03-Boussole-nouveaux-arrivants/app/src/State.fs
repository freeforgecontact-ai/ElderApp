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
      Statut: Statut option          // statut choisi par l'utilisateur
      EnSelection: bool              // onboarding : écran de sélection visible ?
      EtapesFaites: Set<string>      // IDs des étapes cochées
      HorizonActif: Horizon          // horizon affiché dans la checklist
      CategorieActive: Categorie211  // catégorie filtrée dans l'annuaire
      CourrierChoisi: TypeCourrier option
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | OuvrirSelection
    | ChoisirStatut of Statut
    | AnnulerSelection
    | BasculerEtape of string
    | ChangerHorizon of Horizon
    | ChangerCategorie of Categorie211
    | ChoisirCourrier of TypeCourrier
    | ReseauChange of bool

let private etapesInitiales (faites: Set<string>) : Set<string> = faites

let init () : Model * Cmd<Msg> =
    let statut   = Storage.chargerStatut ()
    let faites   = Storage.chargerEtapesFaites ()
    { Onglet         = Accueil
      Statut         = statut
      EnSelection    = statut.IsNone   // onboarding direct si pas encore de statut
      EtapesFaites   = etapesInitiales faites
      HorizonActif   = JPlusUn
      CategorieActive = Hebergement
      CourrierChoisi = None
      EnLigne        = estEnLigne () },
    Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Aller o ->
        { model with Onglet = o; EnSelection = false }, Cmd.none

    | OuvrirSelection ->
        { model with EnSelection = true }, Cmd.none

    | ChoisirStatut s ->
        Storage.sauverStatut s
        { model with Statut = Some s; EnSelection = false; Onglet = Accueil }, Cmd.none

    | AnnulerSelection ->
        { model with EnSelection = false }, Cmd.none

    | BasculerEtape id ->
        let faites =
            if Set.contains id model.EtapesFaites
            then Set.remove id model.EtapesFaites
            else Set.add    id model.EtapesFaites
        Storage.sauverEtapesFaites (faites |> Set.toList)
        { model with EtapesFaites = faites }, Cmd.none

    | ChangerHorizon h ->
        { model with HorizonActif = h }, Cmd.none

    | ChangerCategorie c ->
        { model with CategorieActive = c }, Cmd.none

    | ChoisirCourrier t ->
        let nouveau =
            match model.CourrierChoisi with
            | Some t2 when t2 = t -> None   // désélectionne
            | _                   -> Some t
        { model with CourrierChoisi = nouveau }, Cmd.none

    | ReseauChange b ->
        { model with EnLigne = b }, Cmd.none
