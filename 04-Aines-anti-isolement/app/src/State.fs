module State

open Fable.Core
open Elmish
open Domain

// ---- Interop JS ----

[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

// ---- Modele ----

type EcranReglagages =
    | ListeContacts
    | FormulaireContact of Contact option

type Model =
    { Onglet: Onglet
      DernierCheckIn: CheckIn option
      Contacts: Contact list
      Regle: RegleInactivite
      SousEcranReglages: EcranReglagages
      BrouillonContact: Contact
      BrouillonSeuil: string
      ReglagesSauves: bool
      HumeurSelectionnee: Humeur option
      EnLigne: bool }

type Msg =
    | Aller of Onglet
    | FaireCheckIn
    | ChoisirHumeur of Humeur
    | ConfirmerHumeur
    | OuvrirFormulaireContact of Contact option
    | BrouillonNom of string
    | BrouillonTel of string
    | BrouillonPhoto of string
    | BrouillonRole of RoleContact
    | SauverContact
    | SupprimerContact of string
    | AnnulerFormulaire
    | SeuilChange of string
    | SauverReglages
    | ReseauChange of bool

let brouillonVide : Contact =
    { Nom = ""; Telephone = ""; Photo = ""; Role = Famille }

let init () : Model * Cmd<Msg> =
    let contacts    = Storage.chargerContacts ()
    let regle       = Storage.chargerRegle ()
    let dernierCI   = Storage.chargerCheckIn ()
    { Onglet = Accueil
      DernierCheckIn = dernierCI
      Contacts = contacts
      Regle = regle
      SousEcranReglages = ListeContacts
      BrouillonContact = brouillonVide
      BrouillonSeuil = string regle.SeuilHeures
      ReglagesSauves = false
      HumeurSelectionnee = dernierCI |> Option.bind _.Humeur
      EnLigne = estEnLigne () },
    Cmd.none

/// Vrai si un check-in a deja ete enregistre aujourd'hui.
let checkInDuJour (ci: CheckIn option) =
    match ci with
    | None -> false
    | Some c -> c.Le.Date = System.DateTime.Today

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with

    | Aller o ->
        { model with Onglet = o; ReglagesSauves = false }, Cmd.none

    | FaireCheckIn ->
        if checkInDuJour model.DernierCheckIn then
            model, Cmd.none
        else
            let ci = { Le = System.DateTime.Now; Humeur = None }
            Storage.sauverCheckIn ci
            { model with DernierCheckIn = Some ci }, Cmd.none

    | ChoisirHumeur h ->
        { model with HumeurSelectionnee = Some h }, Cmd.none

    | ConfirmerHumeur ->
        match model.HumeurSelectionnee with
        | None -> model, Cmd.none
        | Some h ->
            let ci = { Le = System.DateTime.Now; Humeur = Some h }
            Storage.sauverCheckIn ci
            { model with DernierCheckIn = Some ci }, Cmd.none

    | OuvrirFormulaireContact contact ->
        let b = contact |> Option.defaultValue brouillonVide
        { model with
            SousEcranReglages = FormulaireContact contact
            BrouillonContact = b }, Cmd.none

    | BrouillonNom v ->
        { model with BrouillonContact = { model.BrouillonContact with Nom = v } }, Cmd.none

    | BrouillonTel v ->
        { model with BrouillonContact = { model.BrouillonContact with Telephone = v } }, Cmd.none

    | BrouillonPhoto v ->
        { model with BrouillonContact = { model.BrouillonContact with Photo = v } }, Cmd.none

    | BrouillonRole r ->
        { model with BrouillonContact = { model.BrouillonContact with Role = r } }, Cmd.none

    | SauverContact ->
        let b = model.BrouillonContact
        if b.Nom = "" || b.Telephone = "" then
            model, Cmd.none
        else
            let contacts =
                match model.SousEcranReglages with
                | FormulaireContact (Some ancien) ->
                    model.Contacts |> List.map (fun c -> if c.Nom = ancien.Nom then b else c)
                | _ ->
                    model.Contacts @ [ b ]
            Storage.sauverContacts contacts
            { model with
                Contacts = contacts
                SousEcranReglages = ListeContacts
                BrouillonContact = brouillonVide }, Cmd.none

    | SupprimerContact nom ->
        let contacts = model.Contacts |> List.filter (fun c -> c.Nom <> nom)
        Storage.sauverContacts contacts
        { model with Contacts = contacts }, Cmd.none

    | AnnulerFormulaire ->
        { model with SousEcranReglages = ListeContacts; BrouillonContact = brouillonVide }, Cmd.none

    | SeuilChange v ->
        { model with BrouillonSeuil = v }, Cmd.none

    | SauverReglages ->
        let h =
            match System.Int32.TryParse(model.BrouillonSeuil) with
            | true, v -> max 1 v
            | _ -> model.Regle.SeuilH
        let regle = { SeuilH = h; SeuilHeures = h }
        Storage.sauverRegle regle
        { model with Regle = regle; ReglagesSauves = true }, Cmd.none

    | ReseauChange en ->
        { model with EnLigne = en }, Cmd.none
