module View

open Feliz
open Domain
open State

let private reseau (enLigne: bool) =
    Html.span
        [ prop.className "sol-offline"
          prop.children
              [ Html.span
                    [ prop.className (if enLigne then "sol-point" else "sol-point sol-point--hors") ]
                Html.text (if enLigne then "en ligne" else "hors-ligne") ] ]

let private entete (model: Model) =
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "💛" ]
                            Html.text (I18n.t "app.titre") ] ]
                reseau model.EnLigne ] ]

let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div
                    [ prop.children
                          [ Html.strong (titre + " ")
                            Html.text corps ] ] ] ]

let private champTexte (id: string) (label: string) (valeur: string) (onChange: string -> unit) =
    Html.div
        [ prop.className "sol-champ"
          prop.children
              [ Html.label [ prop.className "sol-label"; prop.htmlFor id; prop.text label ]
                Html.input
                    [ prop.className "sol-input"
                      prop.id id
                      prop.value valeur
                      prop.onChange (fun (v: string) -> onChange v) ] ] ]

let private avatar (contact: Contact) =
    let initiale =
        if contact.Photo <> "" then ""
        elif contact.Nom.Length > 0 then string contact.Nom.[0]
        else "?"
    Html.div
        [ prop.className "lien-avatar"
          prop.ariaHidden true
          prop.children
              [ if contact.Photo <> "" then
                    yield Html.img [ prop.src contact.Photo; prop.alt ""; prop.className "lien-avatar__img" ]
                else
                    yield Html.span [ prop.text (initiale.ToUpperInvariant()) ] ] ]

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    let dejafait = checkInDuJour model.DernierCheckIn
    let contenuAccueil =
        if dejafait then
            Html.div
                [ prop.className "sol-carte lien-confirmation"
                  prop.role "status"
                  prop.custom ("aria-live", "polite")
                  prop.children
                      [ Html.span [ prop.className "lien-grosico"; prop.ariaHidden true; prop.text "✅" ]
                        Html.p [ prop.className "lien-confirmation__titre"; prop.text (I18n.t "accueil.dejafait") ]
                        Html.p [ prop.className "lien-confirmation__sous"; prop.text (I18n.t "accueil.dejafait.sous") ] ] ]
        else
            Html.button
                [ prop.className "sol-btn sol-btn--accent sol-btn--xl sol-btn--bloc lien-checkin-btn"
                  prop.onClick (fun _ -> dispatch FaireCheckIn)
                  prop.ariaLabel (I18n.t "accueil.checkin")
                  prop.children
                      [ Html.span [ prop.className "lien-grosico"; prop.ariaHidden true; prop.text "☀️" ]
                        Html.span [ prop.text (I18n.t "accueil.checkin") ] ] ]
    Html.div
        [ prop.className "sol-contenu lien-kiosque"
          prop.children
              [ Html.h1
                    [ prop.className "lien-bonjour"
                      prop.children [ Html.text (I18n.t "accueil.bonjour") ] ]
                contenuAccueil ] ]

let private carteContact (c: Contact) =
    Html.a
        [ prop.className "lien-contact-carte"
          prop.href (sprintf "tel:%s" (c.Telephone.Replace(" ", "")))
          prop.ariaLabel (sprintf "Appeler %s" c.Nom)
          prop.children
              [ avatar c
                Html.div
                    [ prop.className "lien-contact-info"
                      prop.children
                          [ Html.span [ prop.className "lien-contact-nom"; prop.text c.Nom ]
                            Html.span [ prop.className "lien-contact-role"; prop.text (RoleContact.label c.Role) ] ] ]
                Html.span [ prop.className "lien-contact-appel"; prop.ariaHidden true; prop.text "📞" ] ] ]

let private vueAppeler (model: Model) (dispatch: Msg -> unit) =
    let contenuAppeler =
        if model.Contacts = [] then
            alerte "sol-alerte--alerte" "ℹ️" (I18n.t "appeler.vide") (I18n.t "appeler.vide.aide")
        else
            Html.div
                [ prop.className "lien-contacts-liste"
                  prop.children
                      [ for c in model.Contacts do
                            yield carteContact c ] ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "appeler.titre")
                contenuAppeler ] ]

let private vueHumeur (model: Model) (dispatch: Msg -> unit) =
    let dejafait =
        model.DernierCheckIn
        |> Option.map (fun ci -> ci.Le.Date = System.DateTime.Today && ci.Humeur.IsSome)
        |> Option.defaultValue false

    let contenuHumeur =
        if dejafait && model.HumeurSelectionnee.IsSome then
            Html.div
                [ prop.className "sol-carte lien-confirmation"
                  prop.role "status"
                  prop.children
                      [ Html.span
                            [ prop.className "lien-grosico"
                              prop.ariaHidden true
                              prop.text (Humeur.emoji model.HumeurSelectionnee.Value) ]
                        Html.p [ prop.className "lien-confirmation__titre"; prop.text (I18n.t "humeur.dejafait") ]
                        Html.button
                            [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                              prop.onClick (fun _ -> dispatch (ChoisirHumeur Bien))
                              prop.text (I18n.t "humeur.changer") ] ] ]
        else
            Html.div
                [ prop.className "lien-humeur-grille"
                  prop.role "group"
                  prop.custom ("aria-labelledby", "h-titre")
                  prop.children
                      [ for h in [ Bien; CommeCiCommeCa; PasFort ] do
                            let selectionne = model.HumeurSelectionnee = Some h
                            yield
                                Html.button
                                    [ prop.className
                                          (if selectionne
                                           then "lien-humeur-btn lien-humeur-btn--actif"
                                           else "lien-humeur-btn")
                                      prop.ariaPressed selectionne
                                      prop.onClick (fun _ -> dispatch (ChoisirHumeur h))
                                      prop.children
                                          [ Html.span
                                                [ prop.className "lien-humeur-ico"
                                                  prop.ariaHidden true
                                                  prop.text (Humeur.emoji h) ]
                                            Html.span
                                                [ prop.className "lien-humeur-label"
                                                  prop.text (Humeur.label h) ] ] ] ] ]

    let boutonConfirmer =
        if (not dejafait) && model.HumeurSelectionnee.IsSome then
            Html.button
                [ prop.className "sol-btn sol-btn--accent sol-btn--xl sol-btn--bloc"
                  prop.style [ style.marginTop (length.rem 1.5) ]
                  prop.onClick (fun _ -> dispatch ConfirmerHumeur)
                  prop.text "Confirmer" ]
        else
            Html.none

    Html.div
        [ prop.className "sol-contenu lien-kiosque"
          prop.children
              [ Html.h1 (I18n.t "humeur.titre")
                contenuHumeur
                boutonConfirmer ] ]

let private formulaireContact (b: Contact) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-carte sol-pile"
          prop.children
              [ Html.h2 "Ajouter un contact"
                champTexte "c-nom" (I18n.t "contact.nom") b.Nom (fun v -> dispatch (BrouillonNom v))
                champTexte "c-tel" (I18n.t "contact.tel") b.Telephone (fun v -> dispatch (BrouillonTel v))
                Html.div
                    [ prop.className "sol-champ"
                      prop.children
                          [ Html.label
                                [ prop.className "sol-label"
                                  prop.htmlFor "c-role"
                                  prop.text (I18n.t "contact.role") ]
                            Html.select
                                [ prop.className "sol-input"
                                  prop.id "c-role"
                                  prop.value
                                      (match b.Role with
                                       | Famille  -> "famille"
                                       | Benevole -> "benevole"
                                       | Service  -> "service")
                                  prop.onChange (fun (v: string) ->
                                      let r =
                                          match v with
                                          | "benevole" -> Benevole
                                          | "service"  -> Service
                                          | _          -> Famille
                                      dispatch (BrouillonRole r))
                                  prop.children
                                      [ Html.option [ prop.value "famille";  prop.text "Famille" ]
                                        Html.option [ prop.value "benevole"; prop.text "Bénévole" ]
                                        Html.option [ prop.value "service";  prop.text "Service" ] ] ] ] ]
                Html.div
                    [ prop.className "sol-rangee sol-entre"
                      prop.children
                          [ Html.button
                                [ prop.className "sol-btn sol-btn--fantome"
                                  prop.onClick (fun _ -> dispatch AnnulerFormulaire)
                                  prop.text "Annuler" ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent"
                                  prop.disabled (b.Nom = "" || b.Telephone = "")
                                  prop.onClick (fun _ -> dispatch SauverContact)
                                  prop.text (I18n.t "reglages.sauver") ] ] ] ] ]

let private listeContactsReglages (contacts: Contact list) (dispatch: Msg -> unit) =
    let contenuListe =
        if contacts = [] then
            Html.p
                [ prop.className "sol-texte-faible"
                  prop.text "Aucun contact d'escalade pour l'instant." ]
        else
            Html.ul
                [ prop.className "sol-liste"
                  prop.children
                      [ for c in contacts do
                            yield
                                Html.li
                                    [ prop.className "sol-liste__item sol-rangee sol-entre"
                                      prop.children
                                          [ Html.span
                                                [ prop.children
                                                      [ Html.strong (c.Nom + " ")
                                                        Html.text (sprintf "(%s) %s" (RoleContact.label c.Role) c.Telephone) ] ]
                                            Html.button
                                                [ prop.className "sol-btn sol-btn--danger"
                                                  prop.ariaLabel (sprintf "Supprimer %s" c.Nom)
                                                  prop.onClick (fun _ -> dispatch (SupprimerContact c.Nom))
                                                  prop.text "Supprimer" ] ] ] ] ]
    Html.div
        [ prop.className "sol-pile"
          prop.children
              [ contenuListe
                Html.button
                    [ prop.className "sol-btn sol-btn--secondaire"
                      prop.onClick (fun _ -> dispatch (OuvrirFormulaireContact None))
                      prop.text (I18n.t "reglages.ajouter") ] ] ]

let private vueReglages (model: Model) (dispatch: Msg -> unit) =
    let sousEcran =
        match model.SousEcranReglages with
        | FormulaireContact _ ->
            formulaireContact model.BrouillonContact dispatch
        | ListeContacts ->
            Html.div
                [ prop.className "sol-pile"
                  prop.children
                      [ Html.h2 (I18n.t "reglages.contacts")
                        listeContactsReglages model.Contacts dispatch
                        Html.div
                            [ prop.className "sol-carte sol-pile"
                              prop.children
                                  [ Html.h2 (I18n.t "reglages.seuil")
                                    champTexte
                                        "seuil"
                                        (I18n.t "reglages.seuil")
                                        model.BrouillonSeuil
                                        (fun v -> dispatch (SeuilChange v))
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--accent"
                                          prop.onClick (fun _ -> dispatch SauverReglages)
                                          prop.text (I18n.t "reglages.sauver") ]
                                    if model.ReglagesSauves then
                                        Html.p
                                            [ prop.className "sol-texte-succes"
                                              prop.text (I18n.t "reglages.sauve") ]
                                    else
                                        Html.none ] ] ] ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "reglages.titre")
                sousEcran ] ]

let private navItem (onglet: Onglet) (label: string) (ico: string) (actif: bool) (dispatch: Msg -> unit) =
    Html.button
        [ prop.className (if actif then "sol-nav__item sol-nav__item--actif" else "sol-nav__item")
          prop.onClick (fun _ -> dispatch (Aller onglet))
          prop.ariaLabel label
          prop.children
              [ Html.span [ prop.className "sol-nav__ico"; prop.ariaHidden true; prop.text ico ]
                Html.span [ prop.className "sol-nav__label"; prop.text label ] ] ]

let private navigation (model: Model) (dispatch: Msg -> unit) =
    Html.nav
        [ prop.className "sol-nav"
          prop.role "navigation"
          prop.children
              [ navItem Accueil  (I18n.t "nav.accueil")  "🏠" (model.Onglet = Accueil)  dispatch
                navItem Appeler  (I18n.t "nav.appeler")  "📞" (model.Onglet = Appeler)  dispatch
                navItem Humeur   (I18n.t "nav.humeur")   "🙂" (model.Onglet = Humeur)   dispatch
                navItem Reglages (I18n.t "nav.reglages") "⚙️" (model.Onglet = Reglages) dispatch ] ]

let view (model: Model) (dispatch: Msg -> unit) =
    let ecran =
        match model.Onglet with
        | Accueil  -> vueAccueil  model dispatch
        | Appeler  -> vueAppeler  model dispatch
        | Humeur   -> vueHumeur   model dispatch
        | Reglages -> vueReglages model dispatch
    Html.div
        [ prop.className "sol-coquille"
          prop.children
              [ entete model
                ecran
                navigation model dispatch ] ]
