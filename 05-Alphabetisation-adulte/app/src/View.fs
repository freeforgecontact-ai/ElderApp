module View

open Feliz
open Domain
open State

// ============================================================
// COMPOSANTS DE BASE
// ============================================================

/// Indicateur réseau (coin supérieur droit de l'en-tête).
let private reseau (enLigne: bool) =
    Html.span
        [ prop.className "sol-offline"
          prop.ariaHidden true
          prop.children
              [ Html.span [ prop.className (if enLigne then "sol-point" else "sol-point sol-point--hors") ]
                Html.text (if enLigne then "en ligne" else "hors-ligne") ] ]

/// En-tête commune à toutes les pages.
let private entete (model: Model) =
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "🌱" ]
                            Html.text (I18n.t "app.titre") ] ]
                reseau model.EnLigne ] ]

/// Bouton vocal : lit un texte à voix haute via la synthèse vocale (Web Speech API).
let private boutonVocal (texte: string) (dispatch: Msg -> unit) =
    Html.button
        [ prop.className "sol-vocal"
          prop.ariaLabel (sprintf "%s : %s" (I18n.t "vocal.lire") texte)
          prop.title (I18n.t "vocal.lire")
          prop.onClick (fun _ -> dispatch (LireVoix texte))
          prop.children
              [ Html.span [ prop.ariaHidden true; prop.text "🔊" ] ] ]

/// Gros bouton illustré (pictogramme + libellé vocal).
let private grosBouton (picto: string) (libelle: string) (variante: string) (ariaLabel: string) (onClick: unit -> unit) =
    Html.button
        [ prop.className (sprintf "sol-grosbouton %s" variante)
          prop.ariaLabel ariaLabel
          prop.onClick (fun _ -> onClick ())
          prop.children
              [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text picto ]
                Html.span [ prop.className "sol-grosbouton__libelle"; prop.text libelle ] ] ]

// ============================================================
// VUE ACCUEIL — grille de modules par pictogrammes
// ============================================================

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "mots-hero"
                      prop.children
                          [ Html.h1
                                [ prop.className "mots-hero__titre"
                                  prop.children
                                      [ Html.span [ prop.ariaHidden true; prop.text "🌱 " ]
                                        Html.text (I18n.t "app.titre") ] ]
                            Html.p
                                [ prop.className "mots-hero__sous"
                                  prop.text (I18n.t "accueil.intro") ]
                            boutonVocal (I18n.t "accueil.intro") dispatch ] ]

                Html.div
                    [ prop.className "sol-contenu"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-grille sol-grille--2 mots-grille"
                                  prop.role "list"
                                  prop.children
                                      [ for m in catalogueModules do
                                            let fait = List.contains m.Id model.Jardin.ModulesCompletes
                                            yield
                                                Html.div
                                                    [ prop.role "listitem"
                                                      prop.children
                                                          [ grosBouton
                                                                m.Picto
                                                                m.LibelleVocal
                                                                (if fait then "sol-grosbouton--accent mots-module--fait" else "")
                                                                (sprintf "Ouvrir : %s%s" m.LibelleVocal (if fait then " (complété)" else ""))
                                                                (fun () -> dispatch (OuvrirModule m)) ] ] ] ] ] ] ] ]

// ============================================================
// VUE MODULE — étapes illustrées (image + mot + bouton haut-parleur)
// ============================================================

let private vueModule (model: Model) (dispatch: Msg -> unit) =
    match model.ModuleActif with
    | None ->
        Html.div
            [ prop.className "sol-contenu sol-pile"
              prop.children
                  [ Html.p "Aucun exercice sélectionné."
                    Html.button
                        [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                          prop.onClick (fun _ -> dispatch RetourAccueil)
                          prop.text (I18n.t "module.retour") ] ] ]

    | Some m ->
        let nEtapes = List.length m.Etapes

        match model.PhaseModule with

        | TermineAvecBravo ->
            Html.div
                [ prop.className "sol-contenu sol-pile sol-centre"
                  prop.children
                      [ Html.div
                            [ prop.className "mots-bravo"
                              prop.role "status"
                              prop.ariaLive.polite
                              prop.children
                                  [ Html.span [ prop.className "mots-bravo__ico"; prop.ariaHidden true; prop.text "🌟" ]
                                    Html.h2 (I18n.t "module.bravo")
                                    Html.p (sprintf "Tu as exploré « %s » !" m.LibelleVocal)
                                    Html.span [ prop.ariaHidden true; prop.text (Jardin.pictoStade model.Jardin) ]
                                    boutonVocal (sprintf "%s. %s." (I18n.t "module.bravo") m.LibelleVocal) dispatch ] ]
                        Html.button
                            [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                              prop.onClick (fun _ -> dispatch RetourAccueil)
                              prop.text (I18n.t "module.retour") ] ] ]

        | EnCours etapeIdx ->
            let etape = List.item etapeIdx m.Etapes
            let estPremiere = etapeIdx = 0
            Html.div
                [ prop.className "sol-contenu sol-pile"
                  prop.children
                      [ Html.div
                            [ prop.className "mots-progres-zone"
                              prop.ariaLabel (sprintf "Étape %d sur %d" (etapeIdx + 1) nEtapes)
                              prop.children
                                  [ Html.div
                                        [ prop.className "sol-progres"
                                          prop.role "progressbar"
                                          prop.custom ("aria-valuenow", string (etapeIdx + 1))
                                          prop.custom ("aria-valuemin", "1")
                                          prop.custom ("aria-valuemax", string nEtapes)
                                          prop.children
                                              [ Html.div
                                                    [ prop.className "sol-progres__barre"
                                                      prop.style [ style.width (length.percent (int (float (etapeIdx + 1) / float nEtapes * 100.0))) ] ] ] ] ] ]
                        Html.div
                            [ prop.className "sol-carte mots-etape"
                              prop.ariaLabel (sprintf "Image : %s" etape.TexteVocal)
                              prop.children
                                  [ Html.div
                                        [ prop.className "mots-picto"
                                          prop.ariaHidden true
                                          prop.text etape.Picto ]
                                    Html.p
                                        [ prop.className "mots-mot"
                                          prop.lang "fr-CA"
                                          prop.text etape.Mot ]
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--xl sol-btn--accent mots-btn-ecouter"
                                          prop.ariaLabel (sprintf "%s : %s" (I18n.t "module.ecouter") etape.TexteVocal)
                                          prop.onClick (fun _ -> dispatch (LireVoix etape.TexteVocal))
                                          prop.children
                                              [ Html.span [ prop.ariaHidden true; prop.text "🔊" ]
                                                Html.span [ prop.text (I18n.t "module.ecouter") ] ] ] ] ]
                        Html.div
                            [ prop.className "mots-nav-etapes"
                              prop.children
                                  [ Html.button
                                        [ prop.className "sol-btn sol-btn--fantome mots-nav-etapes__prec"
                                          prop.ariaLabel (I18n.t "module.precedent")
                                          prop.disabled estPremiere
                                          prop.onClick (fun _ -> dispatch EtapePrecedente)
                                          prop.children
                                              [ Html.span [ prop.ariaHidden true; prop.text "←" ]
                                                Html.span [ prop.text (I18n.t "module.precedent") ] ] ]
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--accent sol-btn--lg mots-nav-etapes__suiv"
                                          prop.ariaLabel (I18n.t "module.suivant")
                                          prop.onClick (fun _ -> dispatch EtapeSuivante)
                                          prop.children
                                              [ Html.span [ prop.text (I18n.t "module.suivant") ]
                                                Html.span [ prop.ariaHidden true; prop.text " →" ] ] ] ] ] ] ]

// ============================================================
// VUE JARDIN — progression invisible (avatar/plante qui grandit)
// ============================================================

let private vueJardin (model: Model) (dispatch: Msg -> unit) =
    let j = model.Jardin
    let stade = Jardin.stade j
    let picto = Jardin.pictoStade j
    let couleur = Jardin.couleurStade j
    let nCompletes = List.length j.ModulesCompletes

    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.div
                    [ prop.className "mots-jardin"
                      prop.children
                          [ Html.div
                                [ prop.className "mots-jardin__avatar"
                                  prop.ariaHidden true
                                  prop.style [ style.backgroundColor couleur ]
                                  prop.text picto ]
                            Html.h2 (I18n.t "jardin.titre")
                            Html.p (I18n.t "jardin.desc")
                            boutonVocal (I18n.t "jardin.desc") dispatch

                            Html.p
                                [ prop.className "mots-jardin__etapes"
                                  prop.children
                                      [ Html.strong [ prop.text (string j.EtapesTotal) ]
                                        Html.text (sprintf " %s" (I18n.t "jardin.etapes")) ] ]

                            if nCompletes > 0 then
                                Html.div
                                    [ prop.className "mots-jardin__completes"
                                      prop.ariaLabel (sprintf "%d thème%s exploré%s" nCompletes (if nCompletes > 1 then "s" else "") (if nCompletes > 1 then "s" else ""))
                                      prop.children
                                          [ for id in j.ModulesCompletes do
                                                let m = catalogueModules |> List.tryFind (fun m -> m.Id = id)
                                                match m with
                                                | Some m ->
                                                    yield Html.span
                                                        [ prop.className "mots-jardin__badge"
                                                          prop.ariaLabel m.LibelleVocal
                                                          prop.title m.LibelleVocal
                                                          prop.text m.Picto ]
                                                | None -> () ] ] ] ] ] ]

// ============================================================
// NAVIGATION BASSE
// ============================================================

let private nav (model: Model) (dispatch: Msg -> unit) =
    let item (o: Onglet) (ico: string) (cle: string) =
        let actif = model.Onglet = o
        let attrs =
            [ prop.className "sol-nav__item"
              prop.ariaLabel (I18n.t cle)
              prop.onClick (fun _ -> dispatch (Aller o))
              prop.children
                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text ico ]
                    Html.span [ prop.text (I18n.t cle) ] ] ]
        Html.button (if actif then attrs @ [ prop.custom ("aria-current", "page") ] else attrs)

    Html.nav
        [ prop.className "sol-nav"
          prop.ariaLabel "Navigation principale"
          prop.children
              [ item Accueil "🏠" "nav.accueil"
                item Module  "▶️" "nav.module"
                item Jardin  "🌱" "nav.jardin" ] ]

// ============================================================
// VUE RACINE
// ============================================================

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model
                Html.main
                    [ prop.id "contenu"
                      prop.style [ style.flexGrow 1 ]
                      prop.children
                          [ match model.Onglet with
                            | Accueil -> vueAccueil model dispatch
                            | Module  -> vueModule  model dispatch
                            | Jardin  -> vueJardin  model dispatch ] ]
                nav model dispatch ] ]
