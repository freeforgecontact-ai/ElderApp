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
      Situation: Situation option
      Etapes48: Etape list
      Etapes: Etape list
      FiltreCategorie: Categorie option
      Ressources: Ressource list
      FiltreRessources: string
      Contacts: ContactUrgence list
      Rappels: Rappel list
      BrouillonContact: ContactUrgence
      BrouillonRappel: Rappel
      AjoutContactOuvert: bool
      AjoutRappelOuvert: bool
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | ChoisirSituation of Situation
    | BasculerEtape48 of string
    | BasculerEtape of string
    | ChangerFiltreCategorie of Categorie option
    | ChangerFiltreRessources of string
    | OuvrirAjoutContact
    | FermerAjoutContact
    | BrouillonContactRole of string
    | BrouillonContactNom of string
    | BrouillonContactTel of string
    | EnregistrerContact
    | SupprimerContact of int
    | OuvrirAjoutRappel
    | FermerAjoutRappel
    | BrouillonRappelQuoi of string
    | BrouillonRappelQuand of string
    | BrouillonRappelRecurrent of bool
    | EnregistrerRappel
    | SupprimerRappel of string
    | ReseauChange of bool

let private brouillonContactVide = { Role = ""; Nom = ""; Telephone = "" }
let private brouillonRappelVide  = { Id = ""; Quoi = ""; Quand = ""; Recurrent = false }

let private etapesAvecEtat (etapes: Etape list) (faites: Set<string>) =
    etapes |> List.map (fun e -> { e with Faite = Set.contains e.Id faites })

let init () : Model * Cmd<Msg> =
    let situation   = Storage.chargerSituation ()
    let faites48    = Storage.chargerEtapes48Faites ()
    let faitesPar   = Storage.chargerEtapesFaites ()
    let etapes48 =
        situation
        |> Option.map (fun s -> premierJourEtapes s |> fun es -> etapesAvecEtat es faites48)
        |> Option.defaultValue []
    let etapes =
        situation
        |> Option.map (fun s ->
            toutesLesEtapes ()
            |> List.filter (fun e -> List.contains s e.Situations)
            |> fun es -> etapesAvecEtat es faitesPar)
        |> Option.defaultValue []
    let contacts = Storage.chargerContacts ()
    let rappels  = Storage.chargerRappels ()
    { Onglet              = Accueil
      Situation           = situation
      Etapes48            = etapes48
      Etapes              = etapes
      FiltreCategorie     = None
      Ressources          = ressourcesInitiales
      FiltreRessources    = ""
      Contacts            = contacts
      Rappels             = rappels
      BrouillonContact    = brouillonContactVide
      BrouillonRappel     = brouillonRappelVide
      AjoutContactOuvert  = false
      AjoutRappelOuvert   = false
      EnLigne             = false }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Aller onglet ->
        { model with Onglet = onglet }, Cmd.none

    | ChoisirSituation sit ->
        Storage.sauverSituation sit
        let faites48  = Storage.chargerEtapes48Faites ()
        let faitesPar = Storage.chargerEtapesFaites ()
        let etapes48 = premierJourEtapes sit |> fun es -> etapesAvecEtat es faites48
        let etapes   = toutesLesEtapes () |> List.filter (fun e -> List.contains sit e.Situations) |> fun es -> etapesAvecEtat es faitesPar
        { model with Situation = Some sit; Etapes48 = etapes48; Etapes = etapes; FiltreCategorie = None }, Cmd.none

    | BasculerEtape48 id ->
        let faites = model.Etapes48 |> List.filter (fun e -> e.Faite) |> List.map (fun e -> e.Id) |> Set.ofList
        let nouvellesFaites =
            if Set.contains id faites then Set.remove id faites
            else Set.add id faites
        Storage.sauverEtapes48Faites nouvellesFaites
        { model with Etapes48 = etapesAvecEtat model.Etapes48 nouvellesFaites }, Cmd.none

    | BasculerEtape id ->
        let faites = model.Etapes |> List.filter (fun e -> e.Faite) |> List.map (fun e -> e.Id) |> Set.ofList
        let nouvellesFaites =
            if Set.contains id faites then Set.remove id faites
            else Set.add id faites
        Storage.sauverEtapesFaites nouvellesFaites
        { model with Etapes = etapesAvecEtat model.Etapes nouvellesFaites }, Cmd.none

    | ChangerFiltreCategorie cat ->
        { model with FiltreCategorie = cat }, Cmd.none

    | ChangerFiltreRessources texte ->
        { model with FiltreRessources = texte }, Cmd.none

    | OuvrirAjoutContact ->
        { model with AjoutContactOuvert = true; BrouillonContact = brouillonContactVide }, Cmd.none

    | FermerAjoutContact ->
        { model with AjoutContactOuvert = false; BrouillonContact = brouillonContactVide }, Cmd.none

    | BrouillonContactRole v ->
        { model with BrouillonContact = { model.BrouillonContact with Role = v } }, Cmd.none

    | BrouillonContactNom v ->
        { model with BrouillonContact = { model.BrouillonContact with Nom = v } }, Cmd.none

    | BrouillonContactTel v ->
        { model with BrouillonContact = { model.BrouillonContact with Telephone = v } }, Cmd.none

    | EnregistrerContact ->
        if model.BrouillonContact.Nom.Trim() = "" then
            model, Cmd.none
        else
            let nouveaux = model.Contacts @ [ model.BrouillonContact ]
            Storage.sauverContacts nouveaux
            { model with Contacts = nouveaux; BrouillonContact = brouillonContactVide; AjoutContactOuvert = false }, Cmd.none

    | SupprimerContact idx ->
        let nouveaux = model.Contacts |> List.indexed |> List.choose (fun (i, c) -> if i = idx then None else Some c)
        Storage.sauverContacts nouveaux
        { model with Contacts = nouveaux }, Cmd.none

    | OuvrirAjoutRappel ->
        { model with AjoutRappelOuvert = true; BrouillonRappel = brouillonRappelVide }, Cmd.none

    | FermerAjoutRappel ->
        { model with AjoutRappelOuvert = false; BrouillonRappel = brouillonRappelVide }, Cmd.none

    | BrouillonRappelQuoi v ->
        { model with BrouillonRappel = { model.BrouillonRappel with Quoi = v } }, Cmd.none

    | BrouillonRappelQuand v ->
        { model with BrouillonRappel = { model.BrouillonRappel with Quand = v } }, Cmd.none

    | BrouillonRappelRecurrent v ->
        { model with BrouillonRappel = { model.BrouillonRappel with Recurrent = v } }, Cmd.none

    | EnregistrerRappel ->
        if model.BrouillonRappel.Quoi.Trim() = "" then
            model, Cmd.none
        else
            let id = sprintf "r-%d" (System.DateTime.Now.Ticks)
            let nouveau = { model.BrouillonRappel with Id = id }
            let nouveaux = model.Rappels @ [ nouveau ]
            Storage.sauverRappels nouveaux
            { model with Rappels = nouveaux; BrouillonRappel = brouillonRappelVide; AjoutRappelOuvert = false }, Cmd.none

    | SupprimerRappel id ->
        let nouveaux = model.Rappels |> List.filter (fun r -> r.Id <> id)
        Storage.sauverRappels nouveaux
        { model with Rappels = nouveaux }, Cmd.none

    | ReseauChange enLigne ->
        { model with EnLigne = enLigne }, Cmd.none
