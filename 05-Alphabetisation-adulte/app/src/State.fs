module State

open Fable.Core
open Elmish
open Domain

// ---- Interop JS (offline-first, sans dependance externe) ----

[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

/// Lecture vocale via Web Speech API (synthese vocale -- fr-CA).
/// En production : remplacer par Coqui TTS embarque pour le vrai hors-ligne.
/// Ref. PLAN.md §6 -- architecture technique.
[<Emit("""(function(){
  var u = new SpeechSynthesisUtterance($0);
  u.lang = 'fr-CA';
  u.rate = 0.88;
  window.speechSynthesis.cancel();
  window.speechSynthesis.speak(u);
})()""")>]
let lireVoix (texte: string) : unit = jsNative

// ---- Modele ----

/// Phase active a l'interieur d'un module (navigation par etapes).
type PhaseModule =
    | EnCours of etapeIdx: int
    | TermineAvecBravo

type Model =
    { Onglet: Onglet
      /// Module actuellement ouvert (None = grille d'accueil visible).
      ModuleActif: Module option
      /// Phase a l'interieur du module actif.
      PhaseModule: PhaseModule
      /// Jardin de progression (persiste localement).
      Jardin: Jardin
      /// Etat de la connexion reseau.
      EnLigne: bool }

// ---- Messages ----

type Msg =
    | Aller of Onglet
    | OuvrirModule of Module
    | EtapeSuivante
    | EtapePrecedente
    | LireVoix of string
    | RetourAccueil
    | ReseauChange of bool

// ---- Init ----

let init () : Model * Cmd<Msg> =
    let jardin = Storage.chargerJardin ()
    { Onglet      = Accueil
      ModuleActif  = None
      PhaseModule  = EnCours 0
      Jardin       = jardin
      EnLigne      = estEnLigne () },
    Cmd.none

// ---- Update ----

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with

    | Aller o ->
        { model with Onglet = o; ModuleActif = None; PhaseModule = EnCours 0 }, Cmd.none

    | OuvrirModule m ->
        // Reprendre depuis la derniere etape si memorisee.
        let idxReprise =
            match Storage.chargerModuleEnCours () with
            | Some (id, idx) when id = m.Id -> idx
            | _ -> 0
        Storage.sauverModuleEnCours m.Id idxReprise
        { model with Onglet = Module; ModuleActif = Some m; PhaseModule = EnCours idxReprise }, Cmd.none

    | EtapeSuivante ->
        match model.ModuleActif, model.PhaseModule with
        | Some m, EnCours idx ->
            let nEtapes = List.length m.Etapes
            // Enregistrer l'etape dans le jardin.
            let jardinMaj = Jardin.enregistrerEtape model.Jardin
            if idx + 1 >= nEtapes then
                // Toutes les etapes terminees -> bravo + completer le module.
                let jardinFinal = Jardin.completerModule m.Id jardinMaj
                Storage.sauverJardin jardinFinal
                Storage.effacerModuleEnCours ()
                { model with PhaseModule = TermineAvecBravo; Jardin = jardinFinal }, Cmd.none
            else
                let idxSuivant = idx + 1
                Storage.sauverModuleEnCours m.Id idxSuivant
                Storage.sauverJardin jardinMaj
                { model with PhaseModule = EnCours idxSuivant; Jardin = jardinMaj }, Cmd.none
        | _ -> model, Cmd.none

    | EtapePrecedente ->
        match model.PhaseModule with
        | EnCours idx when idx > 0 ->
            let idxPrec = idx - 1
            match model.ModuleActif with
            | Some m -> Storage.sauverModuleEnCours m.Id idxPrec
            | None -> ()
            { model with PhaseModule = EnCours idxPrec }, Cmd.none
        | _ -> model, Cmd.none

    | LireVoix texte ->
        lireVoix texte
        model, Cmd.none

    | RetourAccueil ->
        Storage.effacerModuleEnCours ()
        { model with Onglet = Accueil; ModuleActif = None; PhaseModule = EnCours 0 }, Cmd.none

    | ReseauChange b ->
        { model with EnLigne = b }, Cmd.none
