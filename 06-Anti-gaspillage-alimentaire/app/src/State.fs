module State

open Fable.Core
open Elmish
open Domain

// État réseau, lu sans dépendance externe (offline-first).
[<Emit("navigator.onLine")>]
let estEnLigne () : bool = jsNative

[<Emit("window.addEventListener('online', $0); window.addEventListener('offline', $0)")>]
let ecouterReseau (cb: unit -> unit) : unit = jsNative

// ---- Brouillon de don (formulaire publier) ----
type BrouillonDon =
    { Donneur: string
      TypeAliment: string        // libellé libre ou catégorie
      Quantite: string
      Froid: ContrainteFroid
      LieuTexte: string
      FenetreHeures: int         // durée de validité souhaitée (en heures)
      CategorieChoisie: CategorieAliment option }

module BrouillonDon =
    let vide nom =
        { Donneur         = nom
          TypeAliment     = ""
          Quantite        = ""
          Froid           = Ambiant
          LieuTexte       = ""
          FenetreHeures   = 6
          CategorieChoisie = None }

// ---- Tri de la liste des surplus ----
type TriSurplus =
    | ParUrgence
    | ParFroid of ContrainteFroid

// ---- Modèle principal ----
type Model =
    { Onglet: Onglet
      RoleChoisi: Role option
      NomDonneur: string
      Brouillon: BrouillonDon
      BrouillonPublie: bool          // confirmation après publication
      DonsLocaux: Don list           // dons publiés dans cette session (outbox)
      DonsExemples: Don list         // données d'exemple embarquées
      TriActif: TriSurplus
      FiltreCategorie: string option  // filtre texte optionnel
      EnLigne: bool }

// ---- Messages ----
type Msg =
    | Aller of Onglet
    | ChoisirRole of Role
    | BrouillonDonneurNom of string
    | BrouillonCategorie of CategorieAliment
    | BrouillonTypeAliment of string
    | BrouillonQuantite of string
    | BrouillonFroid of ContrainteFroid
    | BrouillonLieu of string
    | BrouillonFenetreHeures of int
    | PublierDon
    | ReserverDon of donId: string
    | MarquerRecupere of donId: string
    | ChangerTri of TriSurplus
    | ReiniRole
    | ReseauChange of bool

// ---- Init ----
let init () : Model * Cmd<Msg> =
    let role = Storage.chargerRole ()
    let nom  = Storage.chargerNom ()
    let dons = Storage.chargerDons ()
    { Onglet           = Accueil
      RoleChoisi        = role
      NomDonneur        = nom
      Brouillon         = BrouillonDon.vide nom
      BrouillonPublie   = false
      DonsLocaux        = dons
      DonsExemples      = Exemples.donsExemples
      TriActif          = ParUrgence
      FiltreCategorie   = None
      EnLigne           = estEnLigne () },
    Cmd.none

// ---- Update ----
let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Aller o ->
        { model with Onglet = o; BrouillonPublie = false }, Cmd.none

    | ChoisirRole r ->
        Storage.sauverRole r
        { model with RoleChoisi = Some r }, Cmd.none

    | BrouillonDonneurNom v ->
        Storage.sauverNom v
        { model with NomDonneur = v
                     Brouillon  = { model.Brouillon with Donneur = v } }, Cmd.none

    | BrouillonCategorie cat ->
        let label = CategorieAliment.label cat
        let froid = CategorieAliment.froidParDefaut cat
        { model with
            Brouillon =
                { model.Brouillon with
                    CategorieChoisie = Some cat
                    TypeAliment      = label
                    Froid            = froid } }, Cmd.none

    | BrouillonTypeAliment v ->
        { model with Brouillon = { model.Brouillon with TypeAliment = v } }, Cmd.none

    | BrouillonQuantite v ->
        { model with Brouillon = { model.Brouillon with Quantite = v } }, Cmd.none

    | BrouillonFroid v ->
        { model with Brouillon = { model.Brouillon with Froid = v } }, Cmd.none

    | BrouillonLieu v ->
        { model with Brouillon = { model.Brouillon with LieuTexte = v } }, Cmd.none

    | BrouillonFenetreHeures h ->
        { model with Brouillon = { model.Brouillon with FenetreHeures = h } }, Cmd.none

    | PublierDon ->
        let b = model.Brouillon
        if b.TypeAliment = "" || b.Quantite = "" || b.LieuTexte = "" then
            model, Cmd.none    // validation minimale — l'UI met les champs en évidence
        else
            let don =
                { Id          = Storage.genId ()
                  Donneur     = if b.Donneur = "" then "Anonyme" else b.Donneur
                  TypeAliment = b.TypeAliment
                  Quantite    = b.Quantite
                  Froid       = b.Froid
                  FenetreFin  = System.DateTime.Now.AddHours(float b.FenetreHeures)
                  Lieu        = b.LieuTexte
                  Statut      = Disponible }
            let dons' = don :: model.DonsLocaux
            Storage.sauverDons dons'
            { model with
                DonsLocaux      = dons'
                Brouillon       = BrouillonDon.vide model.NomDonneur
                BrouillonPublie = true }, Cmd.none

    | ReserverDon id ->
        // Scaffold local : marque réservé par le rôle actif.
        let org = (match model.RoleChoisi with Some Organisme -> "Mon organisme" | _ -> "Organisme")
        let maj (d: Don) = if d.Id = id then { d with Statut = Reserve org } else d
        let exemples' = model.DonsExemples |> List.map maj
        let locaux'   = model.DonsLocaux   |> List.map maj
        Storage.sauverDons locaux'
        { model with DonsExemples = exemples'; DonsLocaux = locaux' }, Cmd.none

    | MarquerRecupere id ->
        let maj (d: Don) = if d.Id = id then { d with Statut = Recupere } else d
        let exemples' = model.DonsExemples |> List.map maj
        let locaux'   = model.DonsLocaux   |> List.map maj
        Storage.sauverDons locaux'
        { model with DonsExemples = exemples'; DonsLocaux = locaux' }, Cmd.none

    | ChangerTri tri ->
        { model with TriActif = tri }, Cmd.none

    | ReiniRole ->
        // Réinitialise le choix de rôle -> re-affiche l'écran de sélection au prochain accueil.
        Storage.sauverRole Donneur   // efface en forçant valeur neutre (le choix sera re-fait)
        { model with RoleChoisi = None; Onglet = Accueil }, Cmd.none

    | ReseauChange b ->
        { model with EnLigne = b }, Cmd.none

// ---- Sélecteurs (purs) ----

let tousDons (model: Model) : Don list =
    model.DonsLocaux @ model.DonsExemples

let donsVisibles (model: Model) : Don list =
    let now = System.DateTime.Now
    let tous = tousDons model
    // On garde les dons encore valides : disponibles ou réservés, et non expirés.
    let pertinents =
        tous
        |> List.filter (fun d ->
            d.FenetreFin > now
            && (match d.Statut with
                | Disponible -> true
                | Reserve _  -> true
                | Recupere   -> false
                | Expire     -> false))
    // Filtre texte optionnel (catégorie / mot-clé saisi).
    let filtres =
        match model.FiltreCategorie with
        | Some terme when terme <> "" ->
            let t = terme.ToLowerInvariant()
            pertinents
            |> List.filter (fun d -> d.TypeAliment.ToLowerInvariant().Contains t)
        | _ -> pertinents
    // Tri / filtre selon le mode actif.
    match model.TriActif with
    | ParUrgence ->
        filtres |> List.sortBy (fun d -> (d.FenetreFin - now).TotalSeconds)
    | ParFroid froid ->
        filtres
        |> List.filter (fun d -> d.Froid = froid)
        |> List.sortBy (fun d -> (d.FenetreFin - now).TotalSeconds)
