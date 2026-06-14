module View

open Feliz
open Domain
open State

// ---- Petits composants ----

let private reseau (enLigne: bool) =
    Html.span
        [ prop.className "sol-offline"
          prop.children
              [ Html.span [ prop.className (if enLigne then "sol-point" else "sol-point sol-point--hors") ]
                Html.text (if enLigne then "en ligne" else "hors-ligne") ] ]

let private entete (model: Model) =
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "🌅" ]
                            Html.text (I18n.t "app.titre") ] ]
                reseau model.EnLigne ] ]

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

let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div [ prop.children [ Html.strong (titre + " "); Html.text corps ] ] ] ]

let private grosBouton (ico: string) (txt: string) (variante: string) (onClick: unit -> unit) =
    Html.button
        [ prop.className (sprintf "sol-grosbouton %s" variante)
          prop.onClick (fun _ -> onClick ())
          prop.children
              [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text ico ]
                Html.span [ prop.text txt ] ] ]

// ---- Progression ----

let private barreProgres (etapes: Etape list) =
    if etapes |> List.isEmpty then Html.none
    else
        let total = List.length etapes
        let faites = etapes |> List.filter (fun e -> e.Faite) |> List.length
        let pct = (float faites / float total) * 100.0
        Html.div
            [ prop.children
                  [ Html.p
                        [ prop.className "depart-progres-label"
                          prop.text (sprintf "%d / %d étape%s complétée%s" faites total (if total > 1 then "s" else "") (if faites > 1 then "s" else "")) ]
                    Html.div
                        [ prop.className "sol-progres"
                          prop.children
                              [ Html.div
                                    [ prop.className "sol-progres__barre"
                                      prop.style [ style.width (length.percent pct) ]
                                      prop.custom ("aria-valuenow", faites)
                                      prop.custom ("aria-valuemin", 0)
                                      prop.custom ("aria-valuemax", total)
                                      prop.role "progressbar"
                                      prop.ariaLabel (sprintf "%d sur %d étapes faites" faites total) ] ] ] ] ]

// ---- Étape cochable ----

let private vueEtape (e: Etape) (onToggle: unit -> unit) =
    let echeanceElem =
        match e.Echeance with
        | Some ech -> Html.p [ prop.className "depart-etape__echeance"; prop.text (sprintf "⏱ %s" ech) ]
        | None     -> Html.none
    Html.div
        [ prop.className (if e.Faite then "depart-etape depart-etape--faite" else "depart-etape")
          prop.onClick (fun _ -> onToggle ())
          prop.role "checkbox"
          prop.custom ("aria-checked", e.Faite)
          prop.tabIndex 0
          prop.onKeyDown (fun ev -> if ev.key = "Enter" || ev.key = " " then onToggle ())
          prop.children
              [ Html.div
                    [ prop.className "depart-etape__check"
                      prop.ariaHidden true
                      prop.children
                          [ if e.Faite then Html.text "✓" else Html.none ] ]
                Html.div
                    [ prop.className "depart-etape__corps"
                      prop.children
                          [ Html.p [ prop.className "depart-etape__cat"; prop.text (sprintf "%s  %s" (Categorie.icone e.Categorie) (Categorie.label e.Categorie)) ]
                            Html.p [ prop.className "depart-etape__titre"; prop.text e.Titre ]
                            Html.p [ prop.className "sol-aide"; prop.text e.Details ]
                            echeanceElem ] ] ] ]

// ---- Accueil ----

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    let alerteSituation =
        match model.Situation with
        | Some s ->
            alerte "sol-alerte--info" (Situation.icone s) (sprintf "Situation : %s" (Situation.label s)) "Tu peux la changer à tout moment."
        | None ->
            alerte "sol-alerte--alerte" "👋" "Bienvenue !" "Commence par choisir ta situation pour personnaliser ton parcours."
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "depart-hero"
                      prop.children
                          [ Html.h1 (I18n.t "accueil.titre")
                            Html.p (I18n.t "accueil.sous-titre") ] ]
                Html.div
                    [ prop.className "sol-contenu sol-pile"
                      prop.children
                          [ alerteSituation
                            Html.h2 (I18n.t "accueil.choisir")
                            Html.div
                                [ prop.className "depart-situations"
                                  prop.children
                                      [ for sit in [ Detention; Refuge; Dependance ] do
                                            yield
                                                Html.button
                                                    [ prop.className (
                                                          if model.Situation = Some sit
                                                          then "depart-situation-btn depart-situation-btn--actif"
                                                          else "depart-situation-btn")
                                                      prop.onClick (fun _ -> dispatch (ChoisirSituation sit))
                                                      prop.children
                                                          [ Html.span [ prop.className "depart-situation-btn__ico"; prop.ariaHidden true; prop.text (Situation.icone sit) ]
                                                            Html.span [ prop.className "depart-situation-btn__nom";  prop.text (Situation.label sit) ]
                                                            Html.span [ prop.className "depart-situation-btn__desc"; prop.text (Situation.description sit) ] ] ] ] ]
                            Html.div
                                [ prop.className "sol-grille sol-grille--2"
                                  prop.children
                                      [ grosBouton "📋" "48 premières heures" "" (fun () -> dispatch (Aller PremierJour))
                                        grosBouton "🗺" "Voir mon parcours"   "sol-grosbouton--accent" (fun () -> dispatch (Aller Parcours))
                                        grosBouton "📞" "Ressources"          "" (fun () -> dispatch (Aller Annuaire))
                                        grosBouton "🆘" "Mes contacts"        "sol-grosbouton--danger" (fun () -> dispatch (Aller Contacts)) ] ]
                            Html.p [ prop.className "depart-disclaimer"; prop.text (I18n.t "disclaimer") ] ] ] ] ]

// ---- Premier jour (48 h) ----

let private vuePremierJour (model: Model) (dispatch: Msg -> unit) =
    let contenu =
        match model.Situation with
        | None ->
            alerte "sol-alerte--alerte" "👆" "Choisis d'abord ta situation" "Retourne à l'accueil pour sélectionner ta situation."
        | Some _ ->
            Html.div
                [ prop.className "sol-pile"
                  prop.children
                      [ barreProgres model.Etapes48
                        Html.div
                            [ prop.className "sol-pile"
                              prop.children
                                  [ for e in model.Etapes48 do
                                        yield vueEtape e (fun () -> dispatch (BasculerEtape48 e.Id)) ] ]
                        alerte "sol-alerte--info" "ℹ️" "Rappel." "Ces étapes ne remplacent pas un·e intervenant·e. Appelle le 211 pour trouver un organisme près de toi." ] ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "premier-jour.titre")
                Html.span [ prop.className "depart-48h-badge"; prop.text "Checklist des 48 premières heures" ]
                contenu ] ]

// ---- Parcours ----

let private vueParcours (model: Model) (dispatch: Msg -> unit) =
    let etapesFiltrees =
        match model.FiltreCategorie with
        | None   -> model.Etapes
        | Some c -> model.Etapes |> List.filter (fun e -> e.Categorie = c)

    let contenu =
        match model.Situation with
        | None ->
            alerte "sol-alerte--alerte" "👆" "Choisis d'abord ta situation" "Retourne à l'accueil pour sélectionner ta situation."
        | Some s ->
            let etapesVide =
                if etapesFiltrees |> List.isEmpty then
                    alerte "sol-alerte--info" "👍" "Tout est fait ici !" "Passe à une autre catégorie ou reviens vérifier plus tard."
                else
                    Html.div
                        [ prop.className "sol-pile"
                          prop.children
                              [ for e in etapesFiltrees do
                                    yield vueEtape e (fun () -> dispatch (BasculerEtape e.Id)) ] ]
            Html.div
                [ prop.className "sol-pile"
                  prop.children
                      [ Html.p [ prop.className "sol-aide"; prop.text (sprintf "Parcours pour : %s %s" (Situation.icone s) (Situation.label s)) ]
                        barreProgres model.Etapes
                        Html.div
                            [ prop.className "depart-filtres"
                              prop.role "group"
                              prop.ariaLabel "Filtrer par catégorie"
                              prop.children
                                  [ yield
                                        Html.button
                                            [ prop.className (if model.FiltreCategorie = None then "depart-filtre-btn depart-filtre-btn--actif" else "depart-filtre-btn")
                                              prop.onClick (fun _ -> dispatch (ChangerFiltreCategorie None))
                                              prop.text "Tout" ]
                                    for c in Categorie.toutes do
                                        let nbCat = model.Etapes |> List.filter (fun e -> e.Categorie = c) |> List.length
                                        if nbCat > 0 then
                                            yield
                                                Html.button
                                                    [ prop.className (if model.FiltreCategorie = Some c then "depart-filtre-btn depart-filtre-btn--actif" else "depart-filtre-btn")
                                                      prop.onClick (fun _ -> dispatch (ChangerFiltreCategorie (Some c)))
                                                      prop.text (sprintf "%s %s" (Categorie.icone c) (Categorie.label c)) ] ] ]
                        etapesVide ] ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "parcours.titre")
                contenu ] ]

// ---- Annuaire ----

let private vueRessource (r: Ressource) =
    let lienSite =
        match r.Url with
        | Some u ->
            Html.a
                [ prop.className "sol-lien"
                  prop.href u
                  prop.target "_blank"
                  prop.rel "noopener noreferrer"
                  prop.text "Voir le site →" ]
        | None -> Html.none
    Html.div
        [ prop.className "depart-ressource"
          prop.children
              [ Html.p [ prop.className "depart-ressource__region"; prop.text (sprintf "📍 %s" r.Region) ]
                Html.p [ prop.className "depart-ressource__nom";    prop.text r.Nom ]
                Html.p [ prop.className "depart-ressource__detail"; prop.text r.Description ]
                Html.a
                    [ prop.className "depart-ressource__tel"
                      prop.href (sprintf "tel:%s" (r.Telephone.Replace(" ", "")))
                      prop.text (sprintf "📞 %s" r.Telephone) ]
                lienSite ] ]

let private vueAnnuaire (model: Model) (dispatch: Msg -> unit) =
    let texte = model.FiltreRessources.ToLowerInvariant()
    let ressourcesFiltrees =
        model.Ressources
        |> List.filter (fun r ->
            let situOk =
                match model.Situation with
                | None   -> true
                | Some s -> List.contains s r.Situations
            let texteOk =
                texte = ""
                || r.Nom.ToLowerInvariant().Contains(texte)
                || r.Region.ToLowerInvariant().Contains(texte)
                || r.Description.ToLowerInvariant().Contains(texte)
            situOk && texteOk)
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "annuaire.titre")
                Html.p [ prop.className "sol-aide"; prop.text "Organismes et ressources disponibles hors-ligne." ]
                champTexte "recherche-res" "Rechercher" model.FiltreRessources (fun v -> dispatch (ChangerFiltreRessources v))
                if ressourcesFiltrees |> List.isEmpty then
                    alerte "sol-alerte--info" "🔍" "Aucun résultat" "Essaie un autre terme ou efface le filtre."
                else
                    Html.div
                        [ prop.className "sol-pile"
                          prop.children
                              [ for r in ressourcesFiltrees do
                                    yield vueRessource r ] ] ] ]

// ---- Contacts ----

let private vueContact (idx: int) (c: ContactUrgence) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "depart-contact"
          prop.children
              [ Html.div
                    [ prop.className "depart-contact__info"
                      prop.children
                          [ Html.p [ prop.className "depart-contact__role"; prop.text c.Role ]
                            Html.p [ prop.className "depart-contact__nom";  prop.text c.Nom ]
                            Html.a
                                [ prop.className "depart-contact__tel"
                                  prop.href (sprintf "tel:%s" (c.Telephone.Replace(" ", "")))
                                  prop.text (sprintf "📞 %s" c.Telephone) ] ] ]
                Html.button
                    [ prop.className "sol-bouton-icone sol-bouton-icone--danger"
                      prop.ariaLabel (sprintf "Supprimer %s" c.Nom)
                      prop.onClick (fun _ -> dispatch (SupprimerContact idx))
                      prop.text "🗑" ] ] ]

let private vueRappel (r: Rappel) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "depart-rappel"
          prop.children
              [ Html.div
                    [ prop.className "depart-rappel__info"
                      prop.children
                          [ Html.p [ prop.className "depart-rappel__quoi";  prop.text r.Quoi ]
                            Html.p [ prop.className "depart-rappel__quand"; prop.text (sprintf "🕐 %s%s" r.Quand (if r.Recurrent then " (récurrent)" else "")) ] ] ]
                Html.button
                    [ prop.className "sol-bouton-icone sol-bouton-icone--danger"
                      prop.ariaLabel (sprintf "Supprimer le rappel %s" r.Quoi)
                      prop.onClick (fun _ -> dispatch (SupprimerRappel r.Id))
                      prop.text "🗑" ] ] ]

let private vueContacts (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "contacts.titre")
                // --- Contacts d'urgence ---
                if model.Contacts |> List.isEmpty then
                    alerte "sol-alerte--info" "📋" "Aucun contact" "Ajoute un contact d'urgence (intervenant·e, probation, proche...)."
                else
                    Html.div
                        [ prop.className "sol-pile"
                          prop.children
                              [ for (idx, c) in model.Contacts |> List.indexed do
                                    yield vueContact idx c dispatch ] ]
                if model.AjoutContactOuvert then
                    Html.div
                        [ prop.className "sol-carte sol-pile"
                          prop.children
                              [ champTexte "contact-role" (I18n.t "contacts.role") model.BrouillonContact.Role (fun v -> dispatch (BrouillonContactRole v))
                                champTexte "contact-nom"  (I18n.t "contacts.nom")  model.BrouillonContact.Nom  (fun v -> dispatch (BrouillonContactNom v))
                                champTexte "contact-tel"  (I18n.t "contacts.tel")  model.BrouillonContact.Telephone (fun v -> dispatch (BrouillonContactTel v))
                                Html.div
                                    [ prop.className "sol-grille sol-grille--2"
                                      prop.children
                                          [ Html.button [ prop.className "sol-bouton sol-bouton--accent"; prop.onClick (fun _ -> dispatch EnregistrerContact); prop.text (I18n.t "contacts.enregistrer") ]
                                            Html.button [ prop.className "sol-bouton"; prop.onClick (fun _ -> dispatch FermerAjoutContact); prop.text "Annuler" ] ] ] ] ]
                else
                    Html.button
                        [ prop.className "sol-bouton sol-bouton--accent"
                          prop.onClick (fun _ -> dispatch OuvrirAjoutContact)
                          prop.text (I18n.t "contacts.ajouter") ]
                // --- Rappels ---
                Html.h2 (I18n.t "rappels.titre")
                if model.Rappels |> List.isEmpty then
                    alerte "sol-alerte--info" "⏰" "Aucun rappel" "Ajoute un rappel pour tes rendez-vous importants."
                else
                    Html.div
                        [ prop.className "sol-pile"
                          prop.children
                              [ for r in model.Rappels do
                                    yield vueRappel r dispatch ] ]
                if model.AjoutRappelOuvert then
                    Html.div
                        [ prop.className "sol-carte sol-pile"
                          prop.children
                              [ champTexte "rappel-quoi"  (I18n.t "rappels.quoi")  model.BrouillonRappel.Quoi  (fun v -> dispatch (BrouillonRappelQuoi v))
                                champTexte "rappel-quand" (I18n.t "rappels.quand") model.BrouillonRappel.Quand (fun v -> dispatch (BrouillonRappelQuand v))
                                Html.div
                                    [ prop.className "sol-champ-case"
                                      prop.children
                                          [ Html.input
                                                [ prop.type' "checkbox"
                                                  prop.id "rappel-recurrent"
                                                  prop.isChecked model.BrouillonRappel.Recurrent
                                                  prop.onChange (fun (v: bool) -> dispatch (BrouillonRappelRecurrent v)) ]
                                            Html.label [ prop.htmlFor "rappel-recurrent"; prop.text (I18n.t "rappels.recurrent") ] ] ]
                                Html.div
                                    [ prop.className "sol-grille sol-grille--2"
                                      prop.children
                                          [ Html.button [ prop.className "sol-bouton sol-bouton--accent"; prop.onClick (fun _ -> dispatch EnregistrerRappel); prop.text (I18n.t "contacts.enregistrer") ]
                                            Html.button [ prop.className "sol-bouton"; prop.onClick (fun _ -> dispatch FermerAjoutRappel); prop.text "Annuler" ] ] ] ] ]
                else
                    Html.button
                        [ prop.className "sol-bouton"
                          prop.onClick (fun _ -> dispatch OuvrirAjoutRappel)
                          prop.text (I18n.t "rappels.ajouter") ] ] ]

// ---- Navigation ----

let private navigation (model: Model) (dispatch: Msg -> unit) =
    Html.nav
        [ prop.className "sol-nav"
          prop.role "navigation"
          prop.ariaLabel "Navigation principale"
          prop.children
              [ for (ong, ico, cle) in [ (Accueil, "🏠", "nav.accueil"); (PremierJour, "⏱", "nav.premier-jour"); (Parcours, "🗺", "nav.parcours"); (Annuaire, "📞", "nav.annuaire"); (Contacts, "🆘", "nav.contacts") ] do
                    yield
                        Html.button
                            [ prop.className (if model.Onglet = ong then "sol-nav__item sol-nav__item--actif" else "sol-nav__item")
                              prop.onClick (fun _ -> dispatch (Aller ong))
                              prop.ariaLabel (I18n.t cle)
                              prop.children
                                  [ Html.span [ prop.className "sol-nav__ico"; prop.ariaHidden true; prop.text ico ]
                                    Html.span [ prop.className "sol-nav__label"; prop.text (I18n.t cle) ] ] ] ] ]

// ---- Vue principale ----

let view (model: Model) (dispatch: Msg -> unit) =
    let contenuOnglet =
        match model.Onglet with
        | Accueil    -> vueAccueil model dispatch
        | PremierJour -> vuePremierJour model dispatch
        | Parcours   -> vueParcours model dispatch
        | Annuaire   -> vueAnnuaire model dispatch
        | Contacts   -> vueContacts model dispatch
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model
                Html.main [ prop.className "sol-main"; prop.children [ contenuOnglet ] ]
                navigation model dispatch ] ]
