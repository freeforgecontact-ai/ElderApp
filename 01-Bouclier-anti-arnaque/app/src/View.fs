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
                          [ Html.span [ prop.className "sol-pastille"; prop.text "🛡" ]
                            Html.text (I18n.t "app.titre") ] ]
                reseau model.EnLigne ] ]

let private grosBouton (ico: string) (txt: string) (variante: string) (onClick: unit -> unit) =
    Html.button
        [ prop.className (sprintf "sol-grosbouton %s" variante)
          prop.onClick (fun _ -> onClick ())
          prop.children
              [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text ico ]
                Html.span [ prop.text txt ] ] ]

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

let private question (txt: string) (coche: bool) (onToggle: unit -> unit) =
    Html.label
        [ prop.className "sol-switch"
          prop.children
              [ Html.input
                    [ prop.type' "checkbox"
                      prop.isChecked coche
                      prop.onChange (fun (_: bool) -> onToggle ()) ]
                Html.span
                    [ prop.className "sol-switch__piste"
                      prop.children [ Html.span [ prop.className "sol-switch__bille" ] ] ]
                Html.span [ prop.text txt ] ] ]

// ---- Pause « avant de payer » ----

let private vuePause (model: Model) (dispatch: Msg -> unit) =
    let r = model.Pause
    let n = ReponsePause.risque r
    Html.div
        [ prop.className "sol-carte"
          prop.children
              [ Html.h2 (I18n.t "pause.titre")
                Html.p "Avant d'envoyer de l'argent, prends 10 secondes :"
                Html.div
                    [ prop.className "sol-pile"
                      prop.children
                          [ question "On me demande des cartes-cadeaux" r.CartesCadeaux (fun () -> dispatch (BasculePauseQuestion 0))
                            question "On me dit que c'est très urgent" r.Urgent (fun () -> dispatch (BasculePauseQuestion 1))
                            question "On me demande de garder le secret" r.Secret (fun () -> dispatch (BasculePauseQuestion 2)) ] ]
                (if n >= 1 then
                     Html.div
                         [ prop.className "sol-pile"
                           prop.style [ style.marginTop (length.rem 1) ]
                           prop.children
                               [ alerte "sol-alerte--danger" "🛑" "Ce sont des signes d'arnaque." "N'envoie rien. Parle d'abord à un proche de confiance."
                                 (if model.PauseProchePrevenu then
                                      alerte "sol-alerte--succes" "✅" "C'est noté." "Prends le temps d'en parler avec ton proche avant tout paiement."
                                  else
                                      Html.button
                                          [ prop.className "sol-btn sol-btn--danger sol-btn--bloc sol-btn--lg"
                                            prop.onClick (fun _ -> dispatch PrevenirProche)
                                            prop.text "Prévenir mon proche maintenant" ]) ] ]
                 else
                     Html.none) ] ]

// ---- Verdict de vérification ----

let private vueVerdict (model: Model) (dispatch: Msg -> unit) =
    match model.Verdict with
    | None -> Html.none
    | Some Inconnu ->
        alerte "sol-alerte--succes" "✅" "Aucun signe connu." "Reste prudent : si on te demande de payer vite ou en secret, vérifie avec un proche."
    | Some Sur -> alerte "sol-alerte--succes" "✅" "Rien de suspect." "Aucun indice d'arnaque détecté."
    | Some (Suspect (typ, note)) ->
        Html.div
            [ prop.className "sol-pile"
              prop.children
                  [ alerte "sol-alerte--danger" "🛑" (TypeFraude.label typ) note
                    grosBouton "📞" "Appeler mon proche" "sol-grosbouton--accent" (fun () -> dispatch (Aller Proches)) ] ]

// ---- Onglets ----

let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "bouclier-hero"
                      prop.children
                          [ Html.h1 "Ton bouclier contre les arnaques"
                            Html.p "Vérifie un appel, un texto ou un courriel. Apprends à reconnaître les pièges. Préviens un proche en un geste." ] ]
                Html.div
                    [ prop.className "sol-contenu"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-grille sol-grille--2"
                                  prop.children
                                      [ grosBouton "🔎" "Vérifier un message" "" (fun () -> dispatch (Aller Verifier))
                                        grosBouton "🛑" "Je vérifie avant de payer" "sol-grosbouton--danger" (fun () -> dispatch OuvrirPause)
                                        grosBouton "🎓" "Apprendre (quiz)" "" (fun () -> dispatch (Aller Apprendre))
                                        grosBouton "👤" "Mon proche de confiance" "sol-grosbouton--accent" (fun () -> dispatch (Aller Proches)) ] ] ] ] ] ]

let private vueVerifier (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "verifier.titre")
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "saisie"; prop.text "Message reçu" ]
                                        Html.span [ prop.className "sol-aide"; prop.id "saisie-aide"; prop.text (I18n.t "verifier.aide") ]
                                        Html.textarea
                                            [ prop.className "sol-textarea"
                                              prop.id "saisie"
                                              prop.value model.Saisie
                                              prop.custom ("aria-describedby", "saisie-aide")
                                              prop.placeholder "Ex. : « Allô grand-maman, j'ai eu un accident, envoie des cartes-cadeaux et garde ça secret... »"
                                              prop.onChange (fun (v: string) -> dispatch (SaisieChangee v)) ] ] ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                  prop.onClick (fun _ -> dispatch LancerVerification)
                                  prop.text (I18n.t "verifier.bouton") ] ] ]
                vueVerdict model dispatch
                vuePause model dispatch ] ]

let private vueApprendre (model: Model) (dispatch: Msg -> unit) =
    let options =
        [ "Je raccroche et j'appelle ma banque au numéro officiel", true
          "Je donne mon NIP pour « vérifier mon identité »", false
          "Je clique sur le lien dans le texto", false ]
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 "Quiz du jour"
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ Html.h2 "Un « conseiller de ta banque » t'appelle et demande ton NIP. Que fais-tu ?"
                            Html.div
                                [ prop.className "quiz-options"
                                  prop.children
                                      [ for (i, (txt, bonne)) in List.indexed options do
                                            let choisi = model.QuizReponse = Some i
                                            yield
                                                Html.button
                                                    [ prop.className (
                                                          if not choisi then "sol-btn sol-btn--fantome sol-btn--bloc"
                                                          elif bonne then "sol-btn sol-btn--succes sol-btn--bloc"
                                                          else "sol-btn sol-btn--danger sol-btn--bloc")
                                                      prop.onClick (fun _ -> dispatch (RepondreQuiz i))
                                                      prop.text txt ] ] ]
                            (match model.QuizReponse with
                             | Some i ->
                                 let (_, bonne) = List.item i options
                                 if bonne then
                                     alerte "sol-alerte--succes" "✅" "Exact !" "Ta banque ne demande jamais ton NIP. Raccroche et rappelle le numéro au dos de ta carte."
                                 else
                                     alerte "sol-alerte--alerte" "⚠️" "Attention." "Ne donne jamais ton NIP. Raccroche et rappelle ta banque au numéro officiel."
                             | None -> Html.none) ] ] ] ]

let private vueProches (model: Model) (dispatch: Msg -> unit) =
    let b = model.Brouillon
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "proches.titre")
                Html.p "Choisis une personne de confiance. On pourra la prévenir si tu fais face à une arnaque. Tu gardes le contrôle : tu peux désactiver l'alerte à tout moment."
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ champTexte "p-nom" "Nom" b.Nom (fun v -> dispatch (BrouillonNom v))
                            champTexte "p-tel" "Téléphone" b.Telephone (fun v -> dispatch (BrouillonTel v))
                            Html.label
                                [ prop.className "sol-switch"
                                  prop.style [ style.marginBottom (length.rem 1) ]
                                  prop.children
                                      [ Html.input
                                            [ prop.type' "checkbox"
                                              prop.isChecked b.AlerteActive
                                              prop.onChange (fun (v: bool) -> dispatch (BrouillonAlerte v)) ]
                                        Html.span
                                            [ prop.className "sol-switch__piste"
                                              prop.children [ Html.span [ prop.className "sol-switch__bille" ] ] ]
                                        Html.span [ prop.text "Activer l'alerte à ce proche" ] ] ]
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                  prop.disabled (b.Nom = "" || b.Telephone = "")
                                  prop.onClick (fun _ -> dispatch EnregistrerProche)
                                  prop.text "Enregistrer" ]
                            (match model.Proche with
                             | Some p ->
                                 alerte "sol-alerte--succes" "✅" "Proche enregistré." (sprintf "%s — %s%s" p.Nom p.Telephone (if p.AlerteActive then " (alerte active)" else " (alerte désactivée)"))
                             | None -> Html.none) ] ] ] ]

// ---- Navigation basse ----

let private nav (model: Model) (dispatch: Msg -> unit) =
    let item (o: Onglet) (ico: string) (cle: string) =
        let attrs =
            [ prop.className "sol-nav__item"
              prop.onClick (fun _ -> dispatch (Aller o))
              prop.children
                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text ico ]
                    Html.span [ prop.text (I18n.t cle) ] ] ]
        Html.button (if model.Onglet = o then attrs @ [ prop.custom ("aria-current", "page") ] else attrs)

    Html.nav
        [ prop.className "sol-nav"
          prop.ariaLabel "Navigation principale"
          prop.children
              [ item Accueil "🏠" "nav.accueil"
                item Verifier "🔎" "nav.verifier"
                item Apprendre "🎓" "nav.apprendre"
                item Proches "👤" "nav.proches" ] ]

// ---- Vue racine ----

let view (model: Model) (dispatch: Msg -> unit) =
    let contenu =
        match model.Onglet with
        | Accueil -> vueAccueil model dispatch
        | Verifier -> vueVerifier model dispatch
        | Apprendre -> vueApprendre model dispatch
        | Proches -> vueProches model dispatch
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model
                Html.main
                    [ prop.id "contenu"
                      prop.style [ style.flexGrow 1 ]
                      prop.children [ contenu ] ]
                nav model dispatch ] ]
