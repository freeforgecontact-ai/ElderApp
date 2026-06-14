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

let private entete (model: Model) =
    let titre =
        if model.Camouflage.CamouflageActif && model.Camouflage.NomAffiche <> ""
        then model.Camouflage.NomAffiche
        else I18n.t "app.titre"
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "N" ]
                            Html.text titre ] ]
                reseau model.EnLigne ] ]

let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div [ prop.children [ Html.strong (titre + " "); Html.text corps ] ] ] ]

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

let private barreProgression (pct: int) =
    Html.div
        [ prop.className "sol-progres"
          prop.children
              [ Html.div
                    [ prop.className "sol-progres__piste"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-progres__barre"
                                  prop.style [ style.width (length.percent pct) ]
                                  prop.custom ("aria-valuenow", string pct)
                                  prop.custom ("aria-valuemin", "0")
                                  prop.custom ("aria-valuemax", "100")
                                  prop.role "progressbar"
                                  prop.ariaLabel (sprintf "Progression : %d %%" pct) ] ] ] ] ]

let private miseEnGardeSecurite () =
    Html.details
        [ prop.className "sec-mise-en-garde"
          prop.children
              [ Html.summary [ prop.text "Avertissement de sécurité — lire avant d'utiliser" ]
                Html.div
                    [ prop.className "sec-mise-en-garde__corps"
                      prop.children
                          [ Html.p "Cet outil est un point de départ. Il ne remplace pas :"
                            Html.ul
                                [ prop.children
                                      [ Html.li [ prop.text "L'accompagnement d'une intervenante spécialisée" ]
                                        Html.li [ prop.text "Un chiffrement réel des données" ]
                                        Html.li [ prop.text "Un audit de sécurité indépendant" ] ] ]
                            Html.p "En cas de doute, utilisez l'effacement rapide (onglet Réglages)." ] ] ] ]

// ============ Onglet 1 : Accueil ============

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    let pct = EtapeSortie.progression model.Etapes
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "sec-hero"
                      prop.children
                          [ Html.h1 (I18n.t "accueil.bienvenue")
                            Html.p "Prépare-toi à ton rythme. Tout reste sur cet appareil." ] ]
                Html.div
                    [ prop.className "sol-contenu sol-pile"
                      prop.children
                          [ miseEnGardeSecurite ()
                            Html.div
                                [ prop.className "sol-carte"
                                  prop.children
                                      [ Html.div
                                            [ prop.className "sol-entre"
                                              prop.children
                                                  [ Html.span [ prop.text "Mon plan de sortie" ]
                                                    Html.span [ prop.className "sol-badge"; prop.text (sprintf "%d %%" pct) ] ] ]
                                        barreProgression pct
                                        Html.button
                                            [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                              prop.style [ style.marginTop (length.rem 1) ]
                                              prop.onClick (fun _ -> dispatch (Aller PlanSortie))
                                              prop.text "Voir mon plan de sortie" ] ] ]
                            Html.div
                                [ prop.className "sol-carte"
                                  prop.children
                                      [ Html.h2 "Besoin d'aide maintenant ?"
                                        Html.p "SOS violence conjugale - disponible 24h/24, 7j/7."
                                        Html.a
                                            [ prop.className "sol-btn sol-btn--danger sol-btn--bloc sol-btn--lg"
                                              prop.href "tel:18003639010"
                                              prop.ariaLabel "Appeler SOS violence conjugale au 1 800 363-9010"
                                              prop.text "Appeler : 1 800 363-9010" ]
                                        Html.a
                                            [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                                              prop.style [ style.marginTop (length.rem 0.5) ]
                                              prop.href "sms:4386011211"
                                              prop.ariaLabel "Envoyer un texto au 438 601-1211"
                                              prop.text "Texto : 438 601-1211" ] ] ] ] ] ] ]

// ============ Onglet 2 : Plan de sortie ============

let private vueEtapeParCategorie (etapes: EtapeSortie list) (categorie: string) (dispatch: Msg -> unit) =
    let items = etapes |> List.filter (fun e -> e.Categorie = categorie)
    if List.isEmpty items then Html.none
    else
        Html.div
            [ prop.className "sol-carte"
              prop.children
                  [ Html.h2 [ prop.className "sec-categorie"; prop.text categorie ]
                    Html.ul
                        [ prop.className "sol-liste"
                          prop.children
                              [ for e in items do
                                    yield
                                        Html.li
                                            [ prop.className (sprintf "sol-liste__item%s" (if e.Faite then " sol-liste__item--fait" else ""))
                                              prop.children
                                                  [ Html.label
                                                        [ prop.className "sol-check"
                                                          prop.children
                                                              [ Html.input
                                                                    [ prop.type' "checkbox"
                                                                      prop.isChecked e.Faite
                                                                      prop.id (sprintf "etape-%d" e.Id)
                                                                      prop.onChange (fun (_: bool) -> dispatch (BasculerEtape e.Id)) ]
                                                                Html.span [ prop.className "sol-check__case"; prop.ariaHidden true ]
                                                                Html.span [ prop.text e.Libelle ] ] ] ] ] ] ] ] ]

let private vuePlanSortie (model: Model) (dispatch: Msg -> unit) =
    let pct = EtapeSortie.progression model.Etapes
    let categories = model.Etapes |> List.map (fun e -> e.Categorie) |> List.distinct
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ yield Html.h1 (I18n.t "plan.titre")
                yield Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-entre"
                                  prop.children
                                      [ Html.span [ prop.text (I18n.t "plan.progression") ]
                                        Html.span [ prop.className "sol-badge sol-badge--accent"; prop.text (sprintf "%d %%" pct) ] ] ]
                            barreProgression pct ] ]
                yield alerte "sol-alerte--alerte" "!" "Note de sécurité :"
                          "Prépare ces éléments progressivement. Il n'est pas nécessaire de tout faire d'un coup."
                for cat in categories do
                    yield vueEtapeParCategorie model.Etapes cat dispatch ] ]

// ============ Onglet 3 : Ressources ============

let private carteRessource (r: Ressource) =
    let actionsListe =
        [ match r.Telephone with
          | Some tel ->
              yield Html.a
                        [ prop.className "sol-btn sol-btn--accent sol-btn--bloc"
                          prop.href (sprintf "tel:%s" (tel.Replace(" ", "").Replace("-", "")))
                          prop.ariaLabel (sprintf "Appeler %s au %s" r.Nom tel)
                          prop.text (sprintf "Appeler : %s" tel) ]
          | None -> ()
          match r.Texto with
          | Some txt ->
              yield Html.a
                        [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                          prop.href (sprintf "sms:%s" (txt.Replace(" ", "")))
                          prop.ariaLabel (sprintf "Texto a %s" r.Nom)
                          prop.text (sprintf "Texto : %s" txt) ]
          | None -> () ]
    Html.div
        [ prop.className "sol-carte sec-ressource"
          prop.children
              [ Html.h2 [ prop.className "sec-ressource__nom"; prop.text r.Nom ]
                Html.p [ prop.className "sec-ressource__desc"; prop.text r.Description ]
                Html.p [ prop.className "sol-badge"; prop.text r.Region ]
                Html.div [ prop.className "sol-pile sec-ressource__actions"; prop.children actionsListe ] ] ]

let private vueRessources (model: Model) (dispatch: Msg -> unit) =
    let regions = Ressource.liste |> List.map (fun r -> r.Region) |> List.distinct |> List.sort
    let filtrees =
        match model.FiltreRegion with
        | None   -> Ressource.liste
        | Some r -> Ressource.liste |> List.filter (fun res -> res.Region = r)
    let options =
        [ yield Html.option [ prop.value ""; prop.text "Toutes les régions" ]
          yield! (regions |> List.map (fun r -> Html.option [ prop.value r; prop.text r ])) ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ yield Html.h1 (I18n.t "ressources.titre")
                yield alerte "sol-alerte--alerte" "!" "Données hors-ligne :"
                          "Ces ressources sont disponibles sans connexion. Vérifier sur sosviolenceconjugale.ca."
                yield Html.div
                    [ prop.className "sol-champ"
                      prop.children
                          [ Html.label [ prop.className "sol-label"; prop.htmlFor "filtre-region"; prop.text "Filtrer par région" ]
                            Html.select
                                [ prop.className "sol-input"
                                  prop.id "filtre-region"
                                  prop.onChange (fun (v: string) -> dispatch (FiltrerRegion (if v = "" then None else Some v)))
                                  prop.children options ] ] ]
                yield! (filtrees |> List.map carteRessource) ] ]

// ============ Onglet 4 : Reglages ============

let private vueReglages (model: Model) (dispatch: Msg -> unit) =
    let contenuEffacement =
        if model.EffacementEnCours then
            Html.p [ prop.className "sol-alerte sol-alerte--succes"; prop.text "Toutes les données ont été effacées." ]
        elif model.EffacementConfirme then
            Html.div
                [ prop.className "sol-pile"
                  prop.children
                      [ Html.p "Êtes-vous certaine de vouloir effacer toutes les données ?"
                        Html.button
                            [ prop.className "sol-btn sol-btn--danger"
                              prop.onClick (fun _ -> dispatch ConfirmerEffacement)
                              prop.text "Oui, tout effacer" ]
                        Html.button
                            [ prop.className "sol-btn sol-btn--fantome"
                              prop.onClick (fun _ -> dispatch AnnulerEffacement)
                              prop.text "Annuler" ] ] ]
        else
            Html.button
                [ prop.className "sol-btn sol-btn--danger"
                  prop.onClick (fun _ -> dispatch DemanderEffacement)
                  prop.text "Effacer toutes les données" ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "reglages.titre")
                Html.section
                    [ prop.className "sol-carte sol-pile"
                      prop.children
                          [ Html.h2 (I18n.t "reglages.camouflage")
                            Html.p "Choisissez un nom discret pour l'en-tête de l'application."
                            champTexte "brouillon-nom" "Nom affiché dans l'en-tête" model.BrouillonNom
                                (fun v -> dispatch (BrouillonNomChange v))
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent"
                                  prop.onClick (fun _ -> dispatch EnregistrerCamouflage)
                                  prop.text "Enregistrer le camouflage" ] ] ]
                Html.section
                    [ prop.className "sol-carte sol-pile"
                      prop.children
                          [ Html.h2 (I18n.t "reglages.effacement")
                            alerte "sol-alerte--alerte" "!" "Attention :"
                                "Cette action efface TOUTES les données (plan de sortie, réglages). Irréversible."
                            contenuEffacement ] ] ] ]

let private navItem (oc: Onglet) (o: Onglet) (ico: string) (lbl: string) (dispatch: Msg -> unit) =
    Html.button
        [ prop.className (sprintf "sol-nav__item%s" (if oc = o then " sol-nav__item--actif" else ""))
          prop.onClick (fun _ -> dispatch (Aller o))
          prop.ariaLabel lbl
          prop.children
              [ Html.span [ prop.className "sol-nav__ico"; prop.ariaHidden true; prop.text ico ]
                Html.span [ prop.className "sol-nav__label"; prop.text lbl ] ] ]

let private navigation (model: Model) (dispatch: Msg -> unit) =
    Html.nav
        [ prop.className "sol-nav"
          prop.role "navigation"
          prop.ariaLabel "Navigation principale"
          prop.children
              [ navItem model.Onglet Accueil    "H" (I18n.t "nav.accueil")    dispatch
                navItem model.Onglet PlanSortie "P" (I18n.t "nav.plan")       dispatch
                navItem model.Onglet Ressources "A" (I18n.t "nav.ressources") dispatch
                navItem model.Onglet Reglages   "R" (I18n.t "nav.reglages")   dispatch ] ]

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model
                Html.main
                    [ prop.className "sol-main"
                      prop.children
                          [ match model.Onglet with
                            | Accueil    -> vueAccueil    model dispatch
                            | PlanSortie -> vuePlanSortie model dispatch
                            | Ressources -> vueRessources model dispatch
                            | Reglages   -> vueReglages   model dispatch ] ]
                navigation model dispatch ] ]
