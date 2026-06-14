module View

open Feliz
open Domain
open State

// ============================================================
// Composants réutilisables
// ============================================================

let private reseau (enLigne: bool) =
    Html.span
        [ prop.className "sol-offline"
          prop.children
              [ Html.span [ prop.className (if enLigne then "sol-point" else "sol-point sol-point--hors") ]
                Html.text (if enLigne then "en ligne" else "hors-ligne") ] ]

/// Alerte informative (variante = sol-alerte--succes / --alerte / --danger).
let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div
                    [ prop.children
                          [ Html.strong (titre + " ")
                            Html.text corps ] ] ] ]

let private champTexte (id: string) (label: string) (valeur: string) (ph: string) (onChange: string -> unit) =
    Html.div
        [ prop.className "sol-champ"
          prop.children
              [ Html.label [ prop.className "sol-label"; prop.htmlFor id; prop.text label ]
                Html.input
                    [ prop.className "sol-input"
                      prop.id id
                      prop.value valeur
                      prop.placeholder ph
                      prop.onChange (fun (v: string) -> onChange v) ] ] ]

/// Item de liste éditable avec bouton de suppression.
let private itemEditable (texte: string) (onSupp: unit -> unit) =
    Html.li
        [ prop.className "ancrage-item-edit"
          prop.children
              [ Html.span [ prop.className "ancrage-item-edit__txt"; prop.text texte ]
                Html.button
                    [ prop.className "ancrage-item-edit__sup"
                      prop.ariaLabel (sprintf "Supprimer « %s »" texte)
                      prop.onClick (fun _ -> onSupp ())
                      prop.text "×" ] ] ]

// ============================================================
// En-tête — bouton de crise TOUJOURS visible
// ============================================================

let private entete (model: Model) (dispatch: Msg -> unit) =
    Html.header
        [ prop.className "sol-barre"
          prop.children
              [ Html.span
                    [ prop.className "sol-marque"
                      prop.children
                          [ Html.span [ prop.className "sol-pastille"; prop.ariaHidden true; prop.text "〰" ]
                            Html.text (I18n.t "app.titre") ] ]
                Html.div
                    [ prop.className "sol-rangee sol-gap-2"
                      prop.children
                          [ reseau model.EnLigne
                            Html.button
                                [ prop.className "ancrage-btn-crise"
                                  prop.ariaLabel "Aide maintenant — ressources de crise"
                                  prop.onClick (fun _ -> dispatch OuvrirModale)
                                  prop.children
                                      [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🆘" ]
                                        Html.text (I18n.t "crise.titre") ] ] ] ] ] ]

// ============================================================
// Modale ressources de crise (préchargées, hors-ligne)
// ============================================================

let private modaleCrise (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "ancrage-modale-fond"
          prop.role "dialog"
          prop.custom ("aria-modal", "true")
          prop.ariaLabel (I18n.t "crise.titre")
          prop.onClick (fun _ -> dispatch FermerModale)
          prop.children
              [ Html.div
                    [ prop.className "ancrage-modale"
                      prop.onClick (fun e -> e.stopPropagation ())
                      prop.children
                          [ Html.h2 (I18n.t "crise.sous-titre")
                            Html.p (I18n.t "crise.intro")
                            Html.ul
                                [ prop.className "ancrage-ressource-liste"
                                  prop.ariaLabel "Ressources de crise"
                                  prop.children
                                      [ for r in RessourcesCrise.toutes do
                                            yield
                                                Html.li
                                                    [ prop.className "ancrage-ressource-item"
                                                      prop.children
                                                          [ Html.a
                                                                [ prop.className "ancrage-ressource-lien"
                                                                  prop.href r.Lien
                                                                  prop.ariaLabel (sprintf "%s — %s" r.Nom r.Description)
                                                                  prop.children
                                                                      [ Html.span [ prop.className "ancrage-ressource-nom"; prop.text r.Nom ]
                                                                        Html.span [ prop.className "ancrage-ressource-desc"; prop.text r.Description ] ] ] ] ] ] ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                                  prop.style [ style.marginTop (length.rem 1.5) ]
                                  prop.onClick (fun _ -> dispatch FermerModale)
                                  prop.text (I18n.t "crise.fermer") ] ] ] ] ]

// ============================================================
// Onglet Accueil
// ============================================================

let private vueAccueil (dispatch: Msg -> unit) =
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "ancrage-hero"
                      prop.children
                          [ Html.h1 (I18n.t "accueil.titre")
                            Html.p  (I18n.t "accueil.intro") ] ]
                Html.div
                    [ prop.className "ancrage-cadre"
                      prop.role "note"
                      prop.children
                          [ Html.span [ prop.ariaHidden true; prop.text "ℹ️  " ]
                            Html.text (I18n.t "accueil.cadre") ] ]
                Html.div
                    [ prop.className "sol-contenu"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-grille sol-grille--2"
                                  prop.children
                                      [ Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller Ancrage))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🌊" ]
                                                    Html.span [ prop.text "Exercices d'ancrage" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton sol-grosbouton--accent"
                                              prop.onClick (fun _ -> dispatch (Aller PlanSecurite))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "📋" ]
                                                    Html.span [ prop.text "Mon plan de sécurité" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller Journal))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "📓" ]
                                                    Html.span [ prop.text "Journal d'humeur" ] ] ] ] ] ] ] ] ]

// ============================================================
// Onglet Ancrage — exercices
// ============================================================

// ---- Respiration 4-7-8 (avec minuterie animée) ----

let private vueResp478 (e: EtatResp) (dispatch: Msg -> unit) =
    let classeResp =
        match e.Phase with
        | Inspirer -> "ancrage-cercle-resp ancrage-cercle-resp--inspirer"
        | Retenir  -> "ancrage-cercle-resp ancrage-cercle-resp--retenir"
        | Expirer  -> "ancrage-cercle-resp ancrage-cercle-resp--expirer"
        | Pause    -> "ancrage-cercle-resp"
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.h2 (Exercice.label Respiration478)
                            Html.p  (Exercice.description Respiration478)
                            Html.div
                                [ prop.className "ancrage-respiration"
                                  prop.children
                                      [ Html.div
                                            [ prop.className classeResp
                                              prop.role "img"
                                              prop.ariaLabel (PhaseResp.label e.Phase)
                                              prop.children
                                                  [ Html.span [ prop.className "ancrage-phase-compteur"; prop.text (string e.Compteur) ] ] ]
                                        Html.p [ prop.className "ancrage-phase-label"; prop.text (PhaseResp.label e.Phase) ]
                                        Html.p [ prop.className "ancrage-cycles sol-muet"; prop.text (sprintf "Cycles complétés : %d" e.Cycles) ] ] ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                                  prop.onClick (fun _ -> dispatch QuitterExo)
                                  prop.text "Terminer l'exercice" ] ] ] ] ]

// ---- 5-4-3-2-1 ----

let private vueSens (s: EtatSens) (dispatch: Msg -> unit) =
    let etapes = EtapesSens.toutes
    let terminee = s.EtapeIdx >= 5
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.h2 (Exercice.label Ancrage54321)
                            Html.p  (Exercice.description Ancrage54321)
                            Html.ol
                                [ prop.className "ancrage-sens-etapes"
                                  prop.children
                                      [ for (i, etape) in List.indexed etapes do
                                            let estActive = i = s.EtapeIdx
                                            let estFaite  = List.item i s.Faites
                                            let classe =
                                                if estFaite        then "ancrage-sens-etape ancrage-sens-etape--faite"
                                                elif estActive     then "ancrage-sens-etape ancrage-sens-etape--active"
                                                else                    "ancrage-sens-etape"
                                            yield
                                                Html.li
                                                    [ prop.className classe
                                                      prop.children
                                                          [ Html.span [ prop.className "ancrage-sens-num"; prop.text (string etape.Numero) ]
                                                            Html.div
                                                                [ prop.children
                                                                      [ Html.strong (etape.Icone + " " + etape.Verbe)
                                                                        Html.br []
                                                                        Html.text (sprintf "Nomme %d %s" etape.Numero etape.Sens) ] ] ] ] ] ]
                            (if terminee then
                                 Html.div
                                     [ prop.className "sol-pile"
                                       prop.children
                                           [ alerte "sol-alerte--succes" "✅" "Exercice terminé." "Comment te sens-tu maintenant ? Prends un moment pour remarquer le changement."
                                             Html.button
                                                 [ prop.className "sol-btn sol-btn--accent sol-btn--bloc"
                                                   prop.onClick (fun _ -> dispatch RecommencerSens)
                                                   prop.text "Recommencer" ] ] ]
                             elif s.EtapeIdx < 5 then
                                 Html.button
                                     [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                       prop.onClick (fun _ -> dispatch ValiderEtape)
                                       prop.text (sprintf "J'ai nommé %d %s  ✓" (List.item s.EtapeIdx etapes).Numero (List.item s.EtapeIdx etapes).Sens) ]
                             else Html.none)
                            Html.button
                                [ prop.className "sol-btn sol-btn--fantome sol-btn--bloc"
                                  prop.style [ style.marginTop (length.rem 0.5) ]
                                  prop.onClick (fun _ -> dispatch QuitterExo)
                                  prop.text "Terminer l'exercice" ] ] ] ] ]

// ---- Sélecteur d'exercice ----

let private vueAncrage (model: Model) (dispatch: Msg -> unit) =
    match model.ExoActif with
    | Some Respiration478 ->
        match model.Resp with
        | Some e -> vueResp478 e dispatch
        | None   -> Html.none
    | Some Ancrage54321 ->
        match model.Sens with
        | Some s -> vueSens s dispatch
        | None   -> Html.none
    | None ->
        Html.div
            [ prop.className "sol-contenu sol-pile"
              prop.children
                  [ Html.h1 (I18n.t "ancrage.titre")
                    Html.p  (I18n.t "ancrage.intro")
                    Html.div
                        [ prop.className "ancrage-exo-grille"
                          prop.children
                              [ Html.button
                                    [ prop.className "sol-grosbouton"
                                      prop.onClick (fun _ -> dispatch (ChoisirExo Respiration478))
                                      prop.children
                                          [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🌬" ]
                                            Html.span [ prop.text "Respiration 4-7-8" ]
                                            Html.span [ prop.className "sol-muet"; prop.style [ style.fontSize (length.rem 0.9) ]; prop.text "Inspire 4 s · retiens 7 s · expire 8 s" ] ] ]
                                Html.button
                                    [ prop.className "sol-grosbouton"
                                      prop.onClick (fun _ -> dispatch (ChoisirExo Ancrage54321))
                                      prop.children
                                          [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "👁" ]
                                            Html.span [ prop.text "Technique 5-4-3-2-1" ]
                                            Html.span [ prop.className "sol-muet"; prop.style [ style.fontSize (length.rem 0.9) ]; prop.text "Revenir au moment présent par les sens" ] ] ] ] ] ] ]

// ============================================================
// Onglet Plan de sécurité — modèle Stanley-Brown
// ============================================================

let private sectionTexte
    (titre: string)
    (items: string list)
    (sec: SectionPlan)
    (saisie: string)
    (dispatch: Msg -> unit)
    =
    Html.div
        [ prop.className "ancrage-plan-section"
          prop.children
              [ Html.h3 titre
                Html.ul
                    [ prop.className "ancrage-liste-edit"
                      prop.children
                          [ for (i, item) in List.indexed items do
                                yield itemEditable item (fun () -> dispatch (SupprimerItemPlan (sec, i))) ] ]
                Html.div
                    [ prop.className "ancrage-ajouter-rangee"
                      prop.children
                          [ Html.input
                                [ prop.className "sol-input"
                                  prop.placeholder "Ajouter…"
                                  prop.value saisie
                                  prop.onChange (fun (v: string) -> dispatch (SaisieTexteChange (sec, v)))
                                  prop.onKeyDown (fun e ->
                                      if e.key = "Enter" then dispatch (AjouterItemPlan sec)) ]
                            Html.button
                                [ prop.className "sol-btn"
                                  prop.disabled (saisie.Trim() = "")
                                  prop.onClick (fun _ -> dispatch (AjouterItemPlan sec))
                                  prop.text "+" ] ] ] ] ]

let private sectionContacts
    (titre: string)
    (contacts: Contact list)
    (sec: SectionPlan)
    (nom: string)
    (tel: string)
    (dispatch: Msg -> unit)
    =
    Html.div
        [ prop.className "ancrage-plan-section"
          prop.children
              [ Html.h3 titre
                Html.ul
                    [ prop.className "ancrage-liste-edit"
                      prop.children
                          [ for (i, c) in List.indexed contacts do
                                let texte = if c.Telephone = "" then c.Nom else sprintf "%s — %s" c.Nom c.Telephone
                                yield itemEditable texte (fun () -> dispatch (SupprimerContactPlan (sec, i))) ] ]
                Html.div
                    [ prop.className "sol-rangee"
                      prop.children
                          [ Html.input
                                [ prop.className "sol-input"
                                  prop.style [ style.flexGrow 2 ]
                                  prop.placeholder "Nom"
                                  prop.value nom
                                  prop.onChange (fun (v: string) -> dispatch (SaisieNomChange v)) ]
                            Html.input
                                [ prop.className "sol-input"
                                  prop.style [ style.flexGrow 1 ]
                                  prop.placeholder "Tél. (optionnel)"
                                  prop.type' "tel"
                                  prop.value tel
                                  prop.onChange (fun (v: string) -> dispatch (SaisieTelChange v)) ]
                            Html.button
                                [ prop.className "sol-btn"
                                  prop.disabled (nom.Trim() = "")
                                  prop.onClick (fun _ -> dispatch (AjouterContactPlan sec))
                                  prop.text "+" ] ] ] ] ]

let private vuePlanSecurite (model: Model) (dispatch: Msg -> unit) =
    let ep = model.PlanEtat
    let p  = ep.Plan
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "plan.titre")
                Html.p  (I18n.t "plan.intro")
                alerte "sol-alerte" "ℹ️" "" (I18n.t "plan.cadre")
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ sectionTexte
                                "⚠️  Mes signaux d'alerte"
                                p.SignauxAlerte
                                SigAl (EtatPlan.saisie SigAl ep) dispatch
                            sectionTexte
                                "🌿  Mes stratégies d'apaisement"
                                p.StrategiesApaisantes
                                Strat (EtatPlan.saisie Strat ep) dispatch
                            sectionContacts
                                "🤝  Personnes de soutien"
                                p.PersonnesSoutien
                                PersS ep.SaisieNom ep.SaisieTel dispatch
                            sectionContacts
                                "🩺  Professionnel·le·s à contacter"
                                p.Professionnels
                                Prof ep.SaisieNom ep.SaisieTel dispatch
                            sectionTexte
                                "🔒  Sécuriser mon environnement"
                                p.SecuriserEnvironnement
                                SecEnv (EtatPlan.saisie SecEnv ep) dispatch ] ]
                alerte "sol-alerte--alerte" "🆘" "En cas de crise immédiate :" "appuie sur « Aide maintenant » en haut de l'écran." ] ]

// ============================================================
// Onglet Journal d'humeur
// ============================================================

let private vueJournal (model: Model) (dispatch: Msg -> unit) =
    let je = model.JournalEtat
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "journal.titre")
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.p (I18n.t "journal.intro")
                            Html.div
                                [ prop.className "ancrage-humeur-echelle"
                                  prop.role "group"
                                  prop.ariaLabel "Choisir ton humeur de 1 (très difficile) à 5 (très bien)"
                                  prop.children
                                      [ for h in 1..5 do
                                            let actif = je.HumeurBrouillon = Some h
                                            yield
                                                Html.button
                                                    [ prop.className (if actif then "ancrage-humeur-btn ancrage-humeur-btn--actif" else "ancrage-humeur-btn")
                                                      prop.ariaLabel (sprintf "%s — %s" (Humeur.emoji h) (Humeur.label h))
                                                      prop.custom ("aria-pressed", string actif)
                                                      prop.onClick (fun _ -> dispatch (ChoisirHumeur h))
                                                      prop.text (Humeur.emoji h) ] ] ]
                            (match je.HumeurBrouillon with
                             | Some h ->
                                 Html.p
                                     [ prop.className "sol-centre sol-muet"
                                       prop.text (sprintf "%s  %s" (Humeur.emoji h) (Humeur.label h)) ]
                             | None -> Html.none)
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "journal-note"; prop.text (I18n.t "journal.note") ]
                                        Html.textarea
                                            [ prop.className "sol-textarea"
                                              prop.id "journal-note"
                                              prop.value je.NoteBrouillon
                                              prop.placeholder "Ce que je vis, ce que je pense…"
                                              prop.onChange (fun (v: string) -> dispatch (NoteChange v)) ] ] ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                  prop.disabled (je.HumeurBrouillon.IsNone)
                                  prop.onClick (fun _ -> dispatch EnregistrerHumeur)
                                  prop.text (I18n.t "journal.enregistrer") ] ] ]
                Html.div
                    [ prop.className "ancrage-historique"
                      prop.children
                          [ Html.h2 (I18n.t "journal.historique")
                            if je.Entrees = [] then
                                alerte "" "📓" "" (I18n.t "journal.vide")
                            else
                                Html.div
                                    [ prop.className "ancrage-graphique"
                                      prop.ariaLabel "Graphique d'humeur"
                                      prop.role "img"
                                      prop.children
                                          [ for e in List.rev (je.Entrees |> List.truncate 30) do
                                                yield
                                                    Html.div
                                                        [ prop.className (sprintf "ancrage-barre-humeur ancrage-barre-humeur--%d" e.Humeur)
                                                          prop.title (sprintf "%s — %s %s" (e.Le.ToString("dd MMM")) (Humeur.emoji e.Humeur) (Humeur.label e.Humeur)) ] ] ]
                                Html.div
                                    [ prop.className "sol-pile"
                                      prop.children
                                          [ for e in je.Entrees |> List.truncate 10 do
                                                yield
                                                    Html.div
                                                        [ prop.className "ancrage-entree"
                                                          prop.children
                                                              [ Html.span [ prop.className "ancrage-entree__date"; prop.text (e.Le.ToString("dd MMM yyyy")) ]
                                                                Html.span [ prop.ariaHidden true; prop.text (Humeur.emoji e.Humeur) ]
                                                                if e.Note <> "" then
                                                                    Html.span [ prop.className "ancrage-entree__note"; prop.text e.Note ] ] ] ] ]
                                Html.button
                                    [ prop.className "sol-btn sol-btn--fantome"
                                      prop.style [ style.marginTop (length.rem 1) ]
                                      prop.onClick (fun _ -> dispatch ExporterCsv)
                                      prop.text (I18n.t "journal.exporter") ] ] ] ] ]

// ============================================================
// Navigation basse
// ============================================================

let private nav (model: Model) (dispatch: Msg -> unit) =
    let item (o: Onglet) (ico: string) (cle: string) =
        let actif = model.Onglet = o
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
              [ item Accueil     "🏠" "nav.accueil"
                item Ancrage     "🌊" "nav.ancrage"
                item PlanSecurite "📋" "nav.plan"
                item Journal     "📓" "nav.journal" ] ]

// ============================================================
// Vue racine
// ============================================================

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model dispatch
                Html.main
                    [ prop.id "contenu"
                      prop.style [ style.flexGrow 1 ]
                      prop.children
                          [ match model.Onglet with
                            | Accueil      -> vueAccueil dispatch
                            | Ancrage      -> vueAncrage model dispatch
                            | PlanSecurite -> vuePlanSecurite model dispatch
                            | Journal      -> vueJournal model dispatch ] ]
                nav model dispatch
                if model.ModaleOuverte then modaleCrise dispatch ] ]
