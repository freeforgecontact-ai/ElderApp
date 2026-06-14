module State

open Fable.Core
open Elmish
open Domain

// ---- Interop JS minimal ----

[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

/// Déclenche le téléchargement d'un fichier texte dans le navigateur.
[<Emit("""
(function(nom, contenu) {
  var blob = new Blob([contenu], { type: 'text/csv;charset=utf-8;' });
  var url  = URL.createObjectURL(blob);
  var a    = document.createElement('a');
  a.href = url; a.download = nom; a.click();
  setTimeout(function() { URL.revokeObjectURL(url); }, 1000);
})($0, $1)
""")>]
let private telechargerCsv (nom: string) (contenu: string) : unit = jsNative

// ---- Sous-états pour les exercices ----

/// État de l'exercice de respiration 4-7-8 en cours.
type EtatResp =
    { Phase:    PhaseResp
      Compteur: int    // secondes restantes dans la phase courante
      Cycles:   int    // nombre de cycles complets effectués
    }

/// État de l'exercice 5-4-3-2-1.
type EtatSens =
    { EtapeIdx: int    // 0 à 4 ; -1 = terminé
      Faites:   bool list }  // une entrée par étape déjà validée

// ---- Sous-états pour le plan de sécurité (édition) ----

type SectionPlan =
    | SigAl | Strat | PersS | Prof | SecEnv

type EtatPlan =
    { Plan:         PlanSecurite
      SaisieNom:    string                    // pour l'ajout d'un contact
      SaisieTel:    string
      SaisiesSec:   Map<SectionPlan, string>  // saisie texte par section
    }

module EtatPlan =
    let saisie (sec: SectionPlan) (ep: EtatPlan) =
        ep.SaisiesSec |> Map.tryFind sec |> Option.defaultValue ""

    let init () =
        { Plan       = Storage.chargerPlan ()
          SaisieNom  = ""
          SaisieTel  = ""
          SaisiesSec = Map.empty }

// ---- Sous-état journal ----

type EtatJournal =
    { Entrees:        EntreeJournal list
      HumeurBrouillon: Humeur option
      NoteBrouillon:   string }

module EtatJournal =
    let init () =
        { Entrees         = Storage.chargerJournal ()
          HumeurBrouillon = None
          NoteBrouillon   = "" }

// ---- Modèle global ----

type Model =
    { Onglet:          Onglet
      EnLigne:         bool
      ModaleOuverte:   bool    // modale ressources de crise
      // Ancrage
      ExoActif:        Exercice option
      Resp:            EtatResp option
      Sens:            EtatSens option
      // Plan de sécurité
      PlanEtat:        EtatPlan
      // Journal
      JournalEtat:     EtatJournal }

// ---- Messages ----

type Msg =
    // Navigation générale
    | Aller of Onglet
    | ReseauChange of bool
    // Modale de crise
    | OuvrirModale
    | FermerModale
    // Exercices
    | ChoisirExo     of Exercice
    | QuitterExo
    // Respiration 4-7-8
    | TickResp
    // 5-4-3-2-1
    | ValiderEtape
    | RecommencerSens
    // Plan de sécurité — items texte
    | SaisieTexteChange  of SectionPlan * string
    | AjouterItemPlan    of SectionPlan
    | SupprimerItemPlan  of SectionPlan * int
    // Plan de sécurité — contacts
    | SaisieNomChange    of string
    | SaisieTelChange    of string
    | AjouterContactPlan of SectionPlan
    | SupprimerContactPlan of SectionPlan * int
    // Journal
    | ChoisirHumeur   of Humeur
    | NoteChange      of string
    | EnregistrerHumeur
    | ExporterCsv

// ---- Init ----

let init () : Model * Cmd<Msg> =
    { Onglet        = Accueil
      EnLigne       = estEnLigne ()
      ModaleOuverte = false
      ExoActif      = None
      Resp          = None
      Sens          = None
      PlanEtat      = EtatPlan.init ()
      JournalEtat   = EtatJournal.init () },
    Cmd.none

// ---- Helpers respiration ----

let private phaseInitiale (p: PhaseResp) =
    { Phase = p; Compteur = PhaseResp.duree p; Cycles = 0 }

let private prochainPhase (e: EtatResp) : EtatResp =
    let cycles' = if e.Phase = Expirer then e.Cycles + 1 else e.Cycles
    let phase' =
        match e.Phase with
        | Inspirer -> Retenir
        | Retenir  -> Expirer
        | Expirer  -> Pause
        | Pause    -> Inspirer
    { Phase = phase'; Compteur = PhaseResp.duree phase'; Cycles = cycles' }

// ---- Update ----

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with

    | Aller o ->
        // Quitter un exo si on change d'onglet
        let model' =
            if o <> Ancrage then { model with ExoActif = None; Resp = None; Sens = None }
            else model
        { model' with Onglet = o }, Cmd.none

    | ReseauChange b -> { model with EnLigne = b }, Cmd.none

    | OuvrirModale -> { model with ModaleOuverte = true  }, Cmd.none
    | FermerModale -> { model with ModaleOuverte = false }, Cmd.none

    // ---- Exercices ----
    | ChoisirExo exo ->
        let resp = if exo = Respiration478 then Some (phaseInitiale Inspirer) else None
        let sens =
            if exo = Ancrage54321 then
                Some { EtapeIdx = 0; Faites = List.replicate 5 false }
            else None
        { model with ExoActif = Some exo; Resp = resp; Sens = sens }, Cmd.none

    | QuitterExo ->
        { model with ExoActif = None; Resp = None; Sens = None }, Cmd.none

    // ---- Tick respiration ----
    | TickResp ->
        match model.Resp with
        | None -> model, Cmd.none
        | Some e ->
            let e' =
                if e.Compteur > 1 then { e with Compteur = e.Compteur - 1 }
                else prochainPhase e
            { model with Resp = Some e' }, Cmd.none

    // ---- 5-4-3-2-1 ----
    | ValiderEtape ->
        match model.Sens with
        | None -> model, Cmd.none
        | Some s ->
            if s.EtapeIdx >= 5 then model, Cmd.none
            else
                let faites' =
                    s.Faites
                    |> List.mapi (fun i v -> if i = s.EtapeIdx then true else v)
                let idx' = s.EtapeIdx + 1
                { model with Sens = Some { s with Faites = faites'; EtapeIdx = idx' } }, Cmd.none

    | RecommencerSens ->
        { model with
            Sens = Some { EtapeIdx = 0; Faites = List.replicate 5 false } },
        Cmd.none

    // ---- Plan de sécurité — texte ----
    | SaisieTexteChange (sec, v) ->
        let ep = model.PlanEtat
        { model with PlanEtat = { ep with SaisiesSec = ep.SaisiesSec |> Map.add sec v } }, Cmd.none

    | AjouterItemPlan sec ->
        let ep = model.PlanEtat
        let txt = (EtatPlan.saisie sec ep).Trim()
        if txt = "" then model, Cmd.none
        else
            let plan' =
                match sec with
                | SigAl  -> { ep.Plan with SignauxAlerte          = ep.Plan.SignauxAlerte          @ [ txt ] }
                | Strat  -> { ep.Plan with StrategiesApaisantes   = ep.Plan.StrategiesApaisantes   @ [ txt ] }
                | SecEnv -> { ep.Plan with SecuriserEnvironnement = ep.Plan.SecuriserEnvironnement @ [ txt ] }
                | _      -> ep.Plan
            Storage.sauverPlan plan'
            { model with PlanEtat = { ep with Plan = plan'; SaisiesSec = ep.SaisiesSec |> Map.add sec "" } }, Cmd.none

    | SupprimerItemPlan (sec, idx) ->
        let ep = model.PlanEtat
        let retirer lst =
            lst |> List.mapi (fun i v -> i, v) |> List.choose (fun (i, v) -> if i = idx then None else Some v)
        let plan' =
            match sec with
            | SigAl  -> { ep.Plan with SignauxAlerte       = retirer ep.Plan.SignauxAlerte }
            | Strat  -> { ep.Plan with StrategiesApaisantes = retirer ep.Plan.StrategiesApaisantes }
            | SecEnv -> { ep.Plan with SecuriserEnvironnement = retirer ep.Plan.SecuriserEnvironnement }
            | _      -> ep.Plan
        Storage.sauverPlan plan'
        { model with PlanEtat = { ep with Plan = plan' } }, Cmd.none

    // ---- Plan de sécurité — contacts ----
    | SaisieNomChange v ->
        let ep = model.PlanEtat
        { model with PlanEtat = { ep with SaisieNom = v } }, Cmd.none

    | SaisieTelChange v ->
        let ep = model.PlanEtat
        { model with PlanEtat = { ep with SaisieTel = v } }, Cmd.none

    | AjouterContactPlan sec ->
        let ep = model.PlanEtat
        let nom = ep.SaisieNom.Trim()
        let tel = ep.SaisieTel.Trim()
        if nom = "" then model, Cmd.none
        else
            let contact = { Nom = nom; Telephone = tel }
            let plan' =
                match sec with
                | PersS -> { ep.Plan with PersonnesSoutien = ep.Plan.PersonnesSoutien @ [ contact ] }
                | Prof  -> { ep.Plan with Professionnels   = ep.Plan.Professionnels   @ [ contact ] }
                | _     -> ep.Plan
            Storage.sauverPlan plan'
            { model with PlanEtat = { ep with Plan = plan'; SaisieNom = ""; SaisieTel = "" } }, Cmd.none

    | SupprimerContactPlan (sec, idx) ->
        let ep = model.PlanEtat
        let retirer lst =
            lst |> List.mapi (fun i v -> i, v) |> List.choose (fun (i, v) -> if i = idx then None else Some v)
        let plan' =
            match sec with
            | PersS -> { ep.Plan with PersonnesSoutien = retirer ep.Plan.PersonnesSoutien }
            | Prof  -> { ep.Plan with Professionnels   = retirer ep.Plan.Professionnels }
            | _     -> ep.Plan
        Storage.sauverPlan plan'
        { model with PlanEtat = { ep with Plan = plan' } }, Cmd.none

    // ---- Journal ----
    | ChoisirHumeur h ->
        let je = model.JournalEtat
        { model with JournalEtat = { je with HumeurBrouillon = Some h } }, Cmd.none

    | NoteChange v ->
        let je = model.JournalEtat
        { model with JournalEtat = { je with NoteBrouillon = v } }, Cmd.none

    | EnregistrerHumeur ->
        let je = model.JournalEtat
        match je.HumeurBrouillon with
        | None -> model, Cmd.none
        | Some h ->
            let _ = Storage.ajouterEntree h je.NoteBrouillon
            let entrees' = Storage.chargerJournal ()
            { model with
                JournalEtat =
                    { Entrees         = entrees'
                      HumeurBrouillon = None
                      NoteBrouillon   = "" } },
            Cmd.none

    | ExporterCsv ->
        let csv = Storage.exporterJournalCsv ()
        telechargerCsv "journal-humeur.csv" csv
        model, Cmd.none
