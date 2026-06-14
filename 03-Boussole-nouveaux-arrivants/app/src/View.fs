module View

open Feliz
open Domain
open State

let private reseau (enLigne: bool) =
    Html.span
        [ prop.className "sol-offline"
          prop.children
              [ Html.span [ prop.className (if enLigne then "sol-point" else "sol-point sol-point--hors") ]
                Html.text (if enLigne then "en ligne" else "hors-ligne") ] ]

let private entete (model: Model) (dispatch: Msg -> unit) =
    let boutonStatut =
        match model.Statut with
        | Some s ->
            Html.button
                [ prop.className "sol-btn sol-btn--fantome"
                  prop.onClick (fun _ -> dispatch OuvrirSelection)
                  prop.ariaLabel (sprintf "Changer de statut : %s actuellement" (Statut.label s))
                  prop.children
                      [ Html.span [ prop.ariaHidden true; prop.text (Statut.icone s) ]
                        Html.text " "
                        Html.span [ prop.className "sol-badge"; prop.text (Statut.label s) ] ] ]
        | None ->
            Html.button
                [ prop.className "sol-btn sol-btn--accent"
                  prop.onClick (fun _ -> dispatch OuvrirSelection)
                  prop.text (I18n.t "accueil.statut") ]
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "🧭" ]
                            Html.text (I18n.t "app.titre") ] ]
                Html.div
                    [ prop.className "sol-rangee"
                      prop.children
                          [ reseau model.EnLigne
                            boutonStatut ] ] ] ]

let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div [ prop.children [ Html.strong (titre + " "); Html.text corps ] ] ] ]

let private disclaimer () =
    Html.p [ prop.className "boussole-disclaimer"; prop.text (I18n.t "disclaimer") ]

let private vueSelection (model: Model) (dispatch: Msg -> unit) =
    let boutonsFermer =
        match model.Statut with
        | Some _ ->
            Html.button
                [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                  prop.onClick (fun _ -> dispatch AnnulerSelection)
                  prop.text "Annuler" ]
        | None ->
            Html.div
                [ prop.className "sol-alerte sol-alerte--alerte"
                  prop.children
                      [ Html.span [ prop.ariaHidden true; prop.text "ℹ️" ]
                        Html.div
                            [ prop.children
                                  [ Html.strong "Pas encore de statut ? "
                                    Html.text "Vous pouvez explorer l'annuaire et l'aide aux courriers." ] ] ] ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.role "dialog"
          prop.custom ("aria-modal", "true")
          prop.ariaLabelledBy "sel-titre"
          prop.children
              [ Html.h1 [ prop.id "sel-titre"; prop.text (I18n.t "accueil.q.statut") ]
                Html.p  [ prop.className "sol-aide"; prop.text (I18n.t "accueil.aide.statut") ]
                Html.div
                    [ prop.className "boussole-statuts"
                      prop.children
                          ( Statut.tous |> List.map (fun s ->
                                Html.button
                                    [ prop.className "boussole-statut-btn"
                                      prop.custom ("aria-pressed", (model.Statut = Some s) |> string |> box)
                                      prop.onClick (fun _ -> dispatch (ChoisirStatut s))
                                      prop.children
                                          [ Html.span [ prop.className "boussole-ico"; prop.ariaHidden true; prop.text (Statut.icone s) ]
                                            Html.span [ prop.text (Statut.label s) ] ] ] ) ) ]
                boutonsFermer ] ]

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    let carteStatut =
        match model.Statut with
        | Some s ->
            Html.div
                [ prop.className "sol-carte"
                  prop.children
                      [ Html.div
                            [ prop.className "sol-entre"
                              prop.children
                                  [ Html.div
                                        [ prop.children
                                              [ Html.span [ prop.ariaHidden true; prop.text (sprintf "%s " (Statut.icone s)) ]
                                                Html.strong (Statut.label s) ] ]
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--fantome"
                                          prop.onClick (fun _ -> dispatch OuvrirSelection)
                                          prop.text (I18n.t "accueil.changer") ] ] ]
                        Html.p [ prop.className "sol-aide"; prop.text (Statut.description s) ] ] ]
        | None ->
            Html.div
                [ prop.className "sol-alerte sol-alerte--alerte"
                  prop.children
                      [ Html.span [ prop.ariaHidden true; prop.text "🧭" ]
                        Html.div
                            [ prop.children
                                  [ Html.strong "Commencez ici. "
                                    Html.text "Choisissez votre statut pour un guide personnalisé."
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--accent sol-btn--bloc"
                                          prop.style [ style.marginTop (length.rem 0.75) ]
                                          prop.onClick (fun _ -> dispatch OuvrirSelection)
                                          prop.text (I18n.t "accueil.q.statut") ] ] ] ] ]
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "boussole-hero"
                      prop.children
                          [ Html.h1 (I18n.t "app.titre.long")
                            Html.p (I18n.t "accueil.soustitre") ] ]
                Html.div
                    [ prop.className "sol-contenu sol-pile"
                      prop.children
                          [ carteStatut
                            Html.div
                                [ prop.className "sol-grille sol-grille--2"
                                  prop.children
                                      [ Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller Checklist))
                                              prop.children
                                                  [ Html.span [ prop.ariaHidden true; prop.text "✅" ]
                                                    Html.span [ prop.text "Ma checklist" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton sol-grosbouton--accent"
                                              prop.onClick (fun _ -> dispatch (Aller Annuaire))
                                              prop.children
                                                  [ Html.span [ prop.ariaHidden true; prop.text "📍" ]
                                                    Html.span [ prop.text "Annuaire 211" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller Courriers))
                                              prop.children
                                                  [ Html.span [ prop.ariaHidden true; prop.text "✉️" ]
                                                    Html.span [ prop.text "Courriers IRCC" ] ] ]
                                        Html.a
                                            [ prop.className "sol-grosbouton sol-grosbouton--danger"
                                              prop.href "tel:211"
                                              prop.ariaLabel "Appeler le 211"
                                              prop.children
                                                  [ Html.span [ prop.ariaHidden true; prop.text "📞" ]
                                                    Html.span [ prop.text "Appeler le 211" ] ] ] ] ]
                            disclaimer () ] ] ] ]

let private vueChecklist (model: Model) (dispatch: Msg -> unit) =
    let etapesFilterees =
        match model.Statut with
        | Some s -> Contenu.etapesPourStatut s
        | None   -> Contenu.etapes
    let etapesDuHorizon =
        etapesFilterees |> List.filter (fun e -> e.Horizon = model.HorizonActif)
    let totalFiltre = etapesFilterees |> List.length
    let totalFait   = etapesFilterees |> List.filter (fun e -> Set.contains e.Id model.EtapesFaites) |> List.length
    let pct         = if totalFiltre = 0 then 0 else (totalFait * 100) / totalFiltre
    let listeEtapes =
        etapesDuHorizon |> List.map (fun e ->
            let faite = Set.contains e.Id model.EtapesFaites
            Html.li
                [ prop.className (if faite then "sol-liste__item sol-liste__item--fait" else "sol-liste__item")
                  prop.children
                      [ Html.label
                            [ prop.className "sol-check"
                              prop.children
                                  [ Html.input
                                        [ prop.type' "checkbox"
                                          prop.isChecked faite
                                          prop.ariaLabel e.Titre
                                          prop.onChange (fun (_: bool) -> dispatch (BasculerEtape e.Id)) ]
                                    Html.span
                                        [ prop.children
                                              [ Html.strong e.Titre
                                                Html.p [ prop.className "sol-aide"; prop.text e.Detail ] ] ] ] ] ] ] )
    let contenuHorizon =
        if etapesDuHorizon = [] then
            alerte "sol-alerte--succes" "✅" (I18n.t "checklist.vide") ""
        else
            Html.ul
                [ prop.className "sol-liste"
                  prop.children listeEtapes ]
    let boutonsHorizon =
        Horizon.tous |> List.map (fun h ->
            let actif = model.HorizonActif = h
            Html.button
                [ prop.className "boussole-categorie-btn"
                  prop.role "tab"
                  prop.ariaSelected actif
                  prop.custom ("aria-pressed", actif |> string |> box)
                  prop.onClick (fun _ -> dispatch (ChangerHorizon h))
                  prop.text (Horizon.label h) ] )
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "checklist.titre")
                Html.div
                    [ prop.className "sol-progres"
                      prop.ariaLabel (sprintf "%d / %d %s" totalFait totalFiltre (I18n.t "checklist.progression"))
                      prop.children
                          [ Html.div
                                [ prop.className "sol-progres__barre"
                                  prop.style [ style.width (length.percent pct) ] ] ] ]
                Html.p
                    [ prop.className "sol-aide"
                      prop.text (sprintf "%d / %d %s" totalFait totalFiltre (I18n.t "checklist.progression")) ]
                Html.div
                    [ prop.className "boussole-categorie-tabs"
                      prop.role "tablist"
                      prop.ariaLabel "Horizons temporels — filtrer la checklist"
                      prop.children boutonsHorizon ]
                Html.div
                    [ prop.className "boussole-horizon"
                      prop.children
                          [ Html.p [ prop.className "boussole-horizon__titre"; prop.text (Horizon.label model.HorizonActif) ]
                            contenuHorizon ] ]
                disclaimer () ] ]

let private vueAnnuaire (model: Model) (dispatch: Msg -> unit) =
    let ressources = Contenu.ressourcesPourCategorie model.CategorieActive
    let boutonsCategorie =
        Categorie211.toutes |> List.map (fun c ->
            let actif = model.CategorieActive = c
            Html.button
                [ prop.className "boussole-categorie-btn"
                  prop.role "tab"
                  prop.ariaSelected actif
                  prop.custom ("aria-pressed", actif |> string |> box)
                  prop.onClick (fun _ -> dispatch (ChangerCategorie c))
                  prop.children
                      [ Html.span [ prop.ariaHidden true; prop.text (sprintf "%s " (Categorie211.icone c)) ]
                        Html.text (Categorie211.label c) ] ] )
    let cartes =
        ressources |> List.map (fun r ->
            Html.div
                [ prop.className "boussole-ressource"
                  prop.role "listitem"
                  prop.children
                      [ Html.div [ prop.className "boussole-ressource__nom"; prop.text r.Nom ]
                        Html.div [ prop.className "boussole-ressource__meta"; prop.text (sprintf "📍 %s" r.Region) ]
                        Html.p   [ prop.className "sol-aide"; prop.text r.Description ]
                        Html.a
                            [ prop.className "boussole-ressource__tel"
                              prop.href (sprintf "tel:%s" (r.Telephone.Replace(" ", "")))
                              prop.ariaLabel (sprintf "Appeler %s au %s" r.Nom r.Telephone)
                              prop.children
                                  [ Html.span [ prop.ariaHidden true; prop.text "📞 " ]
                                    Html.text r.Telephone ] ] ] ] )
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "annuaire.titre")
                Html.p [ prop.className "sol-aide"; prop.text (I18n.t "annuaire.sous") ]
                Html.div
                    [ prop.className "boussole-categorie-tabs"
                      prop.role "tablist"
                      prop.ariaLabel "Catégories de ressources"
                      prop.children boutonsCategorie ]
                Html.div
                    [ prop.role "list"
                      prop.ariaLabel (sprintf "Ressources : %s" (Categorie211.label model.CategorieActive))
                      prop.children cartes ]
                alerte "sol-alerte--alerte" "ℹ️" "Données 211 Québec." "Ces données sont un échantillon."
                disclaimer () ] ]

let private vueCourriers (model: Model) (dispatch: Msg -> unit) =
    let contenuCourrier =
        match model.CourrierChoisi with
        | None -> Html.none
        | Some tc ->
            Html.div
                [ prop.className "sol-carte sol-pile"
                  prop.children
                      [ Html.h2
                            [ prop.children
                                  [ Html.span [ prop.ariaHidden true; prop.text (sprintf "%s " (TypeCourrier.icone tc)) ]
                                    Html.text (TypeCourrier.label tc) ] ]
                        Html.p (TypeCourrier.explication tc)
                        alerte "sol-alerte--alerte" "⚖️" "Besoin d'aide juridique ?" "Contactez l'aide juridique ou un organisme accrédité."
                        Html.button
                            [ prop.className "sol-btn sol-btn--accent sol-btn--bloc"
                              prop.onClick (fun _ -> dispatch (Aller Annuaire))
                              prop.text "Trouver un organisme d'aide (annuaire)" ] ] ]
    let boutonsCourrier =
        TypeCourrier.tous |> List.map (fun tc ->
            let choisi = model.CourrierChoisi = Some tc
            Html.button
                [ prop.className
                      (if choisi then "sol-btn sol-btn--accent sol-btn--bloc"
                       else "sol-btn sol-btn--fantome sol-btn--bloc")
                  prop.ariaPressed choisi
                  prop.onClick (fun _ -> dispatch (ChoisirCourrier tc))
                  prop.children
                      [ Html.span [ prop.ariaHidden true; prop.text (sprintf "%s " (TypeCourrier.icone tc)) ]
                        Html.text (TypeCourrier.label tc) ] ] )
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "courriers.titre")
                Html.p [ prop.className "sol-aide"; prop.text (I18n.t "courriers.sous") ]
                Html.div
                    [ prop.className "boussole-type-lettre"
                      prop.children boutonsCourrier ]
                contenuCourrier
                disclaimer () ] ]

let private nav (model: Model) (dispatch: Msg -> unit) =
    let item (o: Onglet) (ico: string) (cle: string) =
        let actif = model.Onglet = o && not model.EnSelection
        let attrs =
            [ prop.className "sol-nav__item"
              prop.onClick (fun _ -> dispatch (Aller o))
              prop.children
                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text ico ]
                    Html.span [ prop.text (I18n.t cle) ] ] ]
        Html.button (if actif then attrs @ [ prop.custom ("aria-current", "page") ] else attrs)
    Html.nav
        [ prop.className "sol-nav"
          prop.ariaLabel "Navigation principale"
          prop.children
              [ item Accueil   "🏠" "nav.accueil"
                item Checklist "✅" "nav.checklist"
                item Annuaire  "📍" "nav.annuaire"
                item Courriers "✉️" "nav.courriers" ] ]

let view (model: Model) (dispatch: Msg -> unit) =
    let contenuPrincipal =
        if model.EnSelection then
            vueSelection model dispatch
        else
            match model.Onglet with
            | Accueil   -> vueAccueil   model dispatch
            | Checklist -> vueChecklist model dispatch
            | Annuaire  -> vueAnnuaire  model dispatch
            | Courriers -> vueCourriers model dispatch
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model dispatch
                Html.main
                    [ prop.id "contenu"
                      prop.style [ style.flexGrow 1 ]
                      prop.children [ contenuPrincipal ] ]
                nav model dispatch ] ]
