module View

open Feliz
open Domain
open State

// ---- Sortie rapide : bouton toujours visible ----
let private boutonQuitter (dispatch: Msg -> unit) =
    Html.button
        [ prop.className "sec-quitter"
          prop.title "Quitter rapidement (ou touche Échap)"
          prop.ariaLabel "Quitter rapidement vers un site neutre"
          prop.onClick (fun _ -> dispatch SortieRapide)
          prop.text "Quitter" ]

let private entete (model: Model) (dispatch: Msg -> unit) =
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "N" ]
                            Html.text model.NomAffiche ] ]
                boutonQuitter dispatch ] ]

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
                    [ prop.className "sol-input"; prop.id id; prop.value valeur
                      prop.onChange (fun (v: string) -> onChange v) ] ] ]

let private champNip (id: string) (label: string) (valeur: string) (onChange: string -> unit) =
    Html.div
        [ prop.className "sol-champ"
          prop.children
              [ Html.label [ prop.className "sol-label"; prop.htmlFor id; prop.text label ]
                Html.input
                    [ prop.className "sol-input"; prop.id id; prop.value valeur
                      prop.type' "password"
                      prop.custom ("inputmode", "numeric")
                      prop.custom ("autocomplete", "off")
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
                                  prop.role "progressbar"
                                  prop.ariaLabel (sprintf "Progression : %d %%" pct) ] ] ] ] ]

// ============ Écran de déverrouillage (si NIP actif) ============

let private ecranVerrou (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model dispatch
                Html.main
                    [ prop.className "sol-main"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-contenu sol-pile"
                                  prop.children
                                      [ Html.h1 model.NomAffiche
                                        Html.p "Entrez votre NIP pour ouvrir."
                                        champNip "nip-ouvrir" "NIP" model.SaisieNip (fun v -> dispatch (SaisieNipChange v))
                                        (match model.ErreurNip with
                                         | Some e -> Html.p [ prop.className "sol-alerte sol-alerte--alerte"; prop.text e ]
                                         | None -> Html.none)
                                        Html.button
                                            [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                              prop.onClick (fun _ -> dispatch TenterDeverrouillage)
                                              prop.text "Ouvrir" ] ] ] ] ] ] ]

// ============ Onglet 1 : Accueil ============

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    let pct = EtapeSortie.progression model.Etapes
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "sec-hero"
                      prop.children
                          [ Html.h1 (I18n.t "accueil.bienvenue")
                            Html.p "Tout reste sur cet appareil. Le bouton « Quitter » (en haut) ou la touche Échap ferment l'écran immédiatement." ] ]
                Html.div
                    [ prop.className "sol-contenu sol-pile"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-carte"
                                  prop.children
                                      [ Html.div
                                            [ prop.className "sol-entre"
                                              prop.children
                                                  [ Html.span [ prop.text "Mon plan" ]
                                                    Html.span [ prop.className "sol-badge"; prop.text (sprintf "%d %%" pct) ] ] ]
                                        barreProgression pct
                                        Html.button
                                            [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                              prop.style [ style.marginTop (length.rem 1) ]
                                              prop.onClick (fun _ -> dispatch (Aller PlanSortie))
                                              prop.text "Voir mon plan" ] ] ]
                            Html.div
                                [ prop.className "sol-carte"
                                  prop.children
                                      [ Html.h2 "Besoin d'aide maintenant ?"
                                        Html.p "Ligne provinciale, disponible 24 h/24, 7 j/7."
                                        Html.a
                                            [ prop.className "sol-btn sol-btn--danger sol-btn--bloc sol-btn--lg"
                                              prop.href "tel:18003639010"
                                              prop.ariaLabel "Appeler la ligne d'aide au 1 800 363-9010"
                                              prop.text "Appeler : 1 800 363-9010" ]
                                        Html.a
                                            [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                                              prop.style [ style.marginTop (length.rem 0.5) ]
                                              prop.href "sms:4386011211"
                                              prop.ariaLabel "Envoyer un texto au 438 601-1211"
                                              prop.text "Texto : 438 601-1211" ] ] ] ] ] ] ]

// ============ Onglet 2 : Plan ============

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
                yield alerte "sol-alerte--alerte" "!" "À ton rythme :" "Prépare ces éléments progressivement. Rien ne presse."
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
                          prop.ariaLabel (sprintf "Texto à %s" r.Nom)
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
                yield alerte "sol-alerte--alerte" "!" "Hors-ligne :" "Ces ressources fonctionnent sans connexion."
                yield Html.div
                    [ prop.className "sol-champ"
                      prop.children
                          [ Html.label [ prop.className "sol-label"; prop.htmlFor "filtre-region"; prop.text "Filtrer par région" ]
                            Html.select
                                [ prop.className "sol-input"; prop.id "filtre-region"
                                  prop.onChange (fun (v: string) -> dispatch (FiltrerRegion (if v = "" then None else Some v)))
                                  prop.children options ] ] ]
                yield! (filtrees |> List.map carteRessource) ] ]

// ============ Onglet 4 : Réglages ============

let private sectionNip (model: Model) (dispatch: Msg -> unit) =
    if model.NipActif then
        Html.section
            [ prop.className "sol-carte sol-pile"
              prop.children
                  [ Html.h2 "Verrouillage par NIP"
                    Html.p "Un NIP protège l'ouverture et chiffre les données sur cet appareil."
                    alerte "sol-alerte--succes" "OK" "Protection active." "Les données sont chiffrées et demandent le NIP à l'ouverture."
                    Html.button
                        [ prop.className "sol-btn sol-btn--fantome"
                          prop.onClick (fun _ -> dispatch RetirerNip)
                          prop.text "Retirer le NIP (revenir en clair)" ] ] ]
    else
        Html.section
            [ prop.className "sol-carte sol-pile"
              prop.children
                  [ Html.h2 "Verrouillage par NIP"
                    Html.p "Ajoute un NIP pour chiffrer les données et exiger un code à l'ouverture. Sans le NIP, les données sont irrécupérables."
                    champNip "nip-1" "Choisir un NIP (4 chiffres ou plus)" model.SaisieNip (fun v -> dispatch (SaisieNipChange v))
                    champNip "nip-2" "Confirmer le NIP" model.SaisieNipConfirm (fun v -> dispatch (SaisieNipConfirmChange v))
                    (match model.ErreurNip with
                     | Some e -> Html.p [ prop.className "sol-alerte sol-alerte--alerte"; prop.text e ]
                     | None -> Html.none)
                    Html.button
                        [ prop.className "sol-btn sol-btn--accent"
                          prop.onClick (fun _ -> dispatch DefinirNip)
                          prop.text "Activer le NIP et chiffrer" ] ] ]

let private vueReglages (model: Model) (dispatch: Msg -> unit) =
    let contenuEffacement =
        if model.EffacementEnCours then
            Html.p [ prop.className "sol-alerte sol-alerte--succes"; prop.text "Toutes les données ont été effacées." ]
        elif model.EffacementConfirme then
            Html.div
                [ prop.className "sol-pile"
                  prop.children
                      [ Html.p "Effacer toutes les données de cet appareil ?"
                        Html.button [ prop.className "sol-btn sol-btn--danger"; prop.onClick (fun _ -> dispatch ConfirmerEffacement); prop.text "Oui, tout effacer" ]
                        Html.button [ prop.className "sol-btn sol-btn--fantome"; prop.onClick (fun _ -> dispatch AnnulerEffacement); prop.text "Annuler" ] ] ]
        else
            Html.button [ prop.className "sol-btn sol-btn--danger"; prop.onClick (fun _ -> dispatch DemanderEffacement); prop.text "Effacer toutes les données" ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "reglages.titre")
                sectionNip model dispatch
                Html.section
                    [ prop.className "sol-carte sol-pile"
                      prop.children
                          [ Html.h2 (I18n.t "reglages.camouflage")
                            Html.p "Nom affiché en haut de l'écran (par défaut « Notes perso »)."
                            champTexte "brouillon-nom" "Nom affiché" model.BrouillonNom (fun v -> dispatch (BrouillonNomChange v))
                            Html.button [ prop.className "sol-btn sol-btn--accent"; prop.onClick (fun _ -> dispatch EnregistrerCamouflage); prop.text "Enregistrer" ] ] ]
                Html.section
                    [ prop.className "sol-carte sol-pile"
                      prop.children
                          [ Html.h2 (I18n.t "reglages.effacement")
                            alerte "sol-alerte--alerte" "!" "Urgence :" "« Effacer et quitter » supprime tout et ouvre un site neutre, d'un seul geste."
                            Html.button
                                [ prop.className "sol-btn sol-btn--danger sol-btn--bloc sol-btn--lg"
                                  prop.onClick (fun _ -> dispatch EffacerEtQuitter)
                                  prop.text "Effacer et quitter" ]
                            Html.hr []
                            contenuEffacement ] ] ] ]

// ============ Navigation + vue racine ============

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
    if not model.Deverrouille then ecranVerrou model dispatch
    else
        Html.div
            [ prop.className "sol-app"
              prop.children
                  [ entete model dispatch
                    Html.main
                        [ prop.className "sol-main"
                          prop.children
                              [ match model.Onglet with
                                | Accueil    -> vueAccueil    model dispatch
                                | PlanSortie -> vuePlanSortie model dispatch
                                | Ressources -> vueRessources model dispatch
                                | Reglages   -> vueReglages   model dispatch ] ]
                    navigation model dispatch ] ]
