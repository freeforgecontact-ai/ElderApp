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
                          [ Html.span [ prop.className "sol-pastille"; prop.text "🌿" ]
                            Html.text (I18n.t "app.titre") ] ]
                reseau model.EnLigne ] ]

let private alerte (variante: string) (ico: string) (titre: string) (corps: string) =
    Html.div
        [ prop.className (sprintf "sol-alerte %s" variante)
          prop.children
              [ Html.span [ prop.className "sol-alerte__ico"; prop.ariaHidden true; prop.text ico ]
                Html.div [ prop.children [ Html.strong (titre + " "); Html.text corps ] ] ] ]

let private champTexte (id: string) (label: string) (placeholder: string) (valeur: string) (onChange: string -> unit) =
    Html.div
        [ prop.className "sol-champ"
          prop.children
              [ Html.label [ prop.className "sol-label"; prop.htmlFor id; prop.text label ]
                Html.input
                    [ prop.className "sol-input"
                      prop.id id
                      prop.placeholder placeholder
                      prop.value valeur
                      prop.onChange (fun (v: string) -> onChange v) ] ] ]

let private badgeFroid (froid: ContrainteFroid) =
    Html.span
        [ prop.className (sprintf "badge-froid %s" (ContrainteFroid.cssVariante froid))
          prop.children
              [ Html.span [ prop.ariaHidden true; prop.text (ContrainteFroid.emoji froid) ]
                Html.text (" " + ContrainteFroid.labelCourt froid) ] ]

let private badgeStatut (statut: StatutDon) =
    Html.span
        [ prop.className (sprintf "badge-statut %s" (StatutDon.cssVariante statut))
          prop.text (StatutDon.labelCourt statut) ]

let private fenetreRestante (fin: System.DateTime) =
    Exemples.fenetreRestante System.DateTime.Now fin

// ---- Bandeau gratuit ----
let private bandeauGratuit () =
    Html.div
        [ prop.className "recolte-gratuit"
          prop.children
              [ Html.span [ prop.ariaHidden true; prop.text "🎁 " ]
                Html.text (I18n.t "gratuit.bandeau") ] ]

// ---- Carte de don dans la liste ----
let private carteDon (don: Don) (role: Role option) (dispatch: Msg -> unit) =
    let categorieEmoji =
        CategorieAliment.toutes
        |> List.tryFind (fun c -> CategorieAliment.label c = don.TypeAliment)
        |> Option.map CategorieAliment.emoji
        |> Option.defaultValue "📦"

    Html.article
        [ prop.className "don-carte"
          prop.ariaLabel (sprintf "Don de %s par %s" don.TypeAliment don.Donneur)
          prop.children
              [ Html.span [ prop.className "don-carte__ico"; prop.ariaHidden true; prop.text categorieEmoji ]
                Html.div
                    [ prop.className "don-carte__corps"
                      prop.children
                          [ Html.p [ prop.className "don-carte__titre"; prop.text don.TypeAliment ]
                            Html.p [ prop.className "don-carte__meta"; prop.text (sprintf "%s — %s" don.Quantite don.Donneur) ]
                            Html.p [ prop.className "don-carte__meta"; prop.text (sprintf "📍 %s" don.Lieu) ]
                            Html.div
                                [ prop.className "sol-inline sol-inline--gap2"
                                  prop.style [ style.marginTop (length.rem 0.5) ]
                                  prop.children
                                      [ badgeFroid don.Froid
                                        badgeStatut don.Statut
                                        Html.span
                                            [ prop.className "sol-aide"
                                              prop.text (fenetreRestante don.FenetreFin) ] ] ]
                            // Actions selon role et statut
                            (let actionBtn =
                                match don.Statut, role with
                                | Disponible, Some Organisme ->
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--accent sol-btn--sm"
                                          prop.style [ style.marginTop (length.rem 0.75) ]
                                          prop.onClick (fun _ -> dispatch (ReserverDon don.Id))
                                          prop.text "Reserver ce don" ]
                                | Reserve _, Some Organisme ->
                                    Html.button
                                        [ prop.className "sol-btn sol-btn--succes sol-btn--sm"
                                          prop.style [ style.marginTop (length.rem 0.75) ]
                                          prop.onClick (fun _ -> dispatch (MarquerRecupere don.Id))
                                          prop.text "Marquer récupéré" ]
                                | _ -> Html.none
                             actionBtn) ] ] ] ]

// ---- Écran : Choix de rôle ----
let private vueChoixRole (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "role.titre")
                Html.p "Choisis ton rôle pour personnaliser l'expérience. Tu pourras changer en tout temps."
                Html.div
                    [ prop.className "role-grille"
                      prop.children
                          [ Html.button
                                [ prop.className "role-btn"
                                  prop.onClick (fun _ -> dispatch (ChoisirRole Donneur))
                                  prop.children
                                      [ Html.span [ prop.className "role-btn__ico"; prop.ariaHidden true; prop.text "🛒" ]
                                        Html.span [ prop.text (I18n.t "role.donneur") ]
                                        Html.span [ prop.className "sol-aide"; prop.text (I18n.t "role.donneur.desc") ] ] ]
                            Html.button
                                [ prop.className "role-btn"
                                  prop.onClick (fun _ -> dispatch (ChoisirRole Organisme))
                                  prop.children
                                      [ Html.span [ prop.className "role-btn__ico"; prop.ariaHidden true; prop.text "🏠" ]
                                        Html.span [ prop.text (I18n.t "role.organisme") ]
                                        Html.span [ prop.className "sol-aide"; prop.text (I18n.t "role.organisme.desc") ] ] ] ] ]
                Html.p
                    [ prop.className "recolte-todo"
                      prop.text (I18n.t "todo.scaffold") ] ] ]

// ---- Écran : Accueil ----
let private vueAccueil (model: Model) (dispatch: Msg -> unit) =
    // Compteur rapide (extrait hors prop.children pour eviter le let dans une liste)
    let nbDispo =
        model.DonsExemples @ model.DonsLocaux
        |> List.filter (fun d -> d.Statut = Disponible && d.FenetreFin > System.DateTime.Now)
        |> List.length
    let alerteCompteur =
        if nbDispo > 0 then
            alerte "sol-alerte--succes" "🌿" (sprintf "%d don(s) disponible(s) en ce moment." nbDispo) "Consultez la liste des surplus."
        else Html.none
    Html.div
        [ prop.children
              [ Html.section
                    [ prop.className "recolte-hero"
                      prop.children
                          [ Html.h1 (I18n.t "accueil.titre")
                            Html.p (I18n.t "accueil.sous") ] ]
                bandeauGratuit ()
                Html.div
                    [ prop.className "sol-contenu"
                      prop.children
                          [ Html.div
                                [ prop.className "sol-grille sol-grille--2"
                                  prop.children
                                      [ Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller Surplus))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🗺" ]
                                                    Html.span [ prop.text "Voir les surplus" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton sol-grosbouton--accent"
                                              prop.onClick (fun _ -> dispatch (Aller Publier))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🎁" ]
                                                    Html.span [ prop.text "Publier un don" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch (Aller MesDons))
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "📋" ]
                                                    Html.span [ prop.text "Mes dons" ] ] ]
                                        Html.button
                                            [ prop.className "sol-grosbouton"
                                              prop.onClick (fun _ -> dispatch ReiniRole)
                                              prop.children
                                                  [ Html.span [ prop.className "sol-ico"; prop.ariaHidden true; prop.text "🔄" ]
                                                    Html.span
                                                        [ prop.text
                                                            (match model.RoleChoisi with
                                                             | Some Donneur    -> "Role : donneur"
                                                             | Some Organisme  -> "Role : organisme"
                                                             | None            -> "Choisir mon role") ] ] ] ] ]
                            alerteCompteur ] ] ] ]

// ---- Écran : Publier un don ----
let private vuePublier (model: Model) (dispatch: Msg -> unit) =
    let b = model.Brouillon
    let champManquant s = s = ""

    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "publier.titre")
                alerte "sol-alerte--succes" "🎁" "Don gratuit." "Zero transaction, zero argent qui circule. Tu offres de la nourriture, c'est tout."
                Html.div
                    [ prop.className "sol-carte"
                      prop.children
                          [ // Nom du donneur
                            champTexte "pub-nom" "Votre nom ou raison sociale" "Ex. : Epicerie Voisin, Marie-Claire..." b.Donneur (fun v -> dispatch (BrouillonDonneurNom v))

                            // Categorie rapide
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.text "Categorie d'aliment" ]
                                        Html.div
                                            [ prop.className "sol-inline sol-inline--gap2"
                                              prop.style [ style.flexWrap.wrap ]
                                              prop.children
                                                  [ for cat in CategorieAliment.toutes do
                                                        yield
                                                            Html.button
                                                                [ prop.className
                                                                      (if b.CategorieChoisie = Some cat
                                                                       then "sol-btn sol-btn--accent sol-btn--sm"
                                                                       else "sol-btn sol-btn--fantome sol-btn--sm")
                                                                  prop.onClick (fun _ -> dispatch (BrouillonCategorie cat))
                                                                  prop.text (sprintf "%s %s" (CategorieAliment.emoji cat) (CategorieAliment.label cat)) ] ] ] ] ]

                            // Type d'aliment (libelle libre)
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "pub-type"; prop.text "Description de l'aliment *" ]
                                        Html.input
                                            [ prop.className (if champManquant b.TypeAliment then "sol-input sol-input--erreur" else "sol-input")
                                              prop.id "pub-type"
                                              prop.placeholder "Ex. : Pommes, yogourt, soupe du jour..."
                                              prop.value b.TypeAliment
                                              prop.onChange (fun (v: string) -> dispatch (BrouillonTypeAliment v)) ]
                                        if champManquant b.TypeAliment then
                                            Html.span [ prop.className "sol-erreur"; prop.text "Champ requis." ]
                                        else Html.none ] ]

                            // Quantite
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "pub-qte"; prop.text "Quantite *" ]
                                        Html.input
                                            [ prop.className (if champManquant b.Quantite then "sol-input sol-input--erreur" else "sol-input")
                                              prop.id "pub-qte"
                                              prop.placeholder "Ex. : 5 kg, 3 boites, 1 caisse..."
                                              prop.value b.Quantite
                                              prop.onChange (fun (v: string) -> dispatch (BrouillonQuantite v)) ]
                                        if champManquant b.Quantite then
                                            Html.span [ prop.className "sol-erreur"; prop.text "Champ requis." ]
                                        else Html.none ] ]

                            // Contrainte de froid
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "pub-froid"; prop.text "Conservation requise" ]
                                        Html.select
                                            [ prop.className "sol-select"
                                              prop.id "pub-froid"
                                              prop.value (string b.Froid)
                                              prop.onChange (fun (v: string) ->
                                                  dispatch (BrouillonFroid (ContrainteFroid.ofString v)))
                                              prop.children
                                                  [ for f in ContrainteFroid.toutes do
                                                        yield
                                                            Html.option
                                                                [ prop.value (string f)
                                                                  prop.text (sprintf "%s %s" (ContrainteFroid.emoji f) (ContrainteFroid.label f)) ] ] ] ] ]

                            // Lieu
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "pub-lieu"; prop.text "Lieu de récupération *" ]
                                        Html.input
                                            [ prop.className (if champManquant b.LieuTexte then "sol-input sol-input--erreur" else "sol-input")
                                              prop.id "pub-lieu"
                                              prop.placeholder "Adresse ou quartier (ex. : 123, rue Exemple, Montréal)"
                                              prop.value b.LieuTexte
                                              prop.onChange (fun (v: string) -> dispatch (BrouillonLieu v)) ]
                                        if champManquant b.LieuTexte then
                                            Html.span [ prop.className "sol-erreur"; prop.text "Champ requis." ]
                                        else Html.none ] ]

                            // Fenêtre de récupération
                            Html.div
                                [ prop.className "sol-champ"
                                  prop.children
                                      [ Html.label [ prop.className "sol-label"; prop.htmlFor "pub-fenetre"; prop.text "Disponible pendant..." ]
                                        Html.select
                                            [ prop.className "sol-select"
                                              prop.id "pub-fenetre"
                                              prop.value (string b.FenetreHeures)
                                              prop.onChange (fun (v: string) ->
                                                  match System.Int32.TryParse(v) with
                                                  | true, h -> dispatch (BrouillonFenetreHeures h)
                                                  | _ -> ())
                                              prop.children
                                                  [ Html.option [ prop.value "2";  prop.text "2 heures" ]
                                                    Html.option [ prop.value "4";  prop.text "4 heures" ]
                                                    Html.option [ prop.value "6";  prop.text "6 heures" ]
                                                    Html.option [ prop.value "12"; prop.text "12 heures" ]
                                                    Html.option [ prop.value "24"; prop.text "1 journee" ]
                                                    Html.option [ prop.value "48"; prop.text "2 jours" ]
                                                    Html.option [ prop.value "168"; prop.text "1 semaine" ] ] ] ] ]

                            // Bouton publier
                            Html.button
                                [ prop.className "sol-btn sol-btn--accent sol-btn--bloc sol-btn--lg"
                                  prop.disabled (b.TypeAliment = "" || b.Quantite = "" || b.LieuTexte = "")
                                  prop.onClick (fun _ -> dispatch PublierDon)
                                  prop.text "Publier ce don gratuitement" ] ] ]

                // Confirmation
                if model.BrouillonPublie then
                    alerte "sol-alerte--succes" "✅" "Don publié !" "Les organismes à portée peuvent maintenant le voir et le réserver. Merci !"
                else Html.none ] ]

// ---- Écran : Surplus disponibles ----
let private vueSurplus (model: Model) (dispatch: Msg -> unit) =
    let dons = donsVisibles model
    let nbTotal = List.length dons

    Html.div
        [ prop.className "sol-pile"
          prop.children
              [ Html.div
                    [ prop.className "recolte-filtres"
                      prop.ariaLabel "Filtres et tri"
                      prop.children
                          [ yield Html.span [ prop.className "sol-label"; prop.text "Tri : " ]
                            yield Html.button
                                [ prop.className
                                      (if model.TriActif = ParUrgence
                                       then "sol-btn sol-btn--accent sol-btn--sm"
                                       else "sol-btn sol-btn--fantome sol-btn--sm")
                                  prop.onClick (fun _ -> dispatch (ChangerTri ParUrgence))
                                  prop.text "⏰ Urgence" ]
                            for froid in ContrainteFroid.toutes do
                                yield
                                    Html.button
                                        [ prop.className
                                              (if model.TriActif = ParFroid froid
                                               then "sol-btn sol-btn--accent sol-btn--sm"
                                               else "sol-btn sol-btn--fantome sol-btn--sm")
                                          prop.onClick (fun _ -> dispatch (ChangerTri (ParFroid froid)))
                                          prop.text (sprintf "%s %s" (ContrainteFroid.emoji froid) (ContrainteFroid.labelCourt froid)) ] ] ]

                Html.div
                    [ prop.className "sol-contenu sol-pile"
                      prop.children
                          [ Html.h1 (I18n.t "surplus.titre")
                            Html.p (sprintf "%d don(s) en ce moment" nbTotal)
                            Html.p
                                [ prop.className "recolte-todo"
                                  prop.text (I18n.t "todo.scaffold") ]
                            if nbTotal = 0 then
                                alerte "sol-alerte--alerte" "📭" "Aucun surplus pour ce filtre." "Essaie un autre tri ou reviens dans quelques instants."
                            else
                                Html.div
                                    [ prop.className "dons-liste"
                                      prop.children
                                          [ for don in dons do
                                                yield carteDon don model.RoleChoisi dispatch ] ] ] ] ] ]

// ---- Écran : Mes dons ----
let private vueMesDons (model: Model) (dispatch: Msg -> unit) =
    let tousDons = model.DonsLocaux @ model.DonsExemples
    // Affiche surtout les dons de la session locale (+ exemples filtres Recupere/Reserve pour illustration)
    let mesDons =
        model.DonsLocaux
        @ (model.DonsExemples |> List.filter (fun d -> d.Statut = Recupere || (match d.Statut with Reserve _ -> true | _ -> false)))
    // Resume chiffre (extrait hors prop.children pour eviter les let dans une liste)
    let nbDispo = tousDons |> List.filter (fun d -> d.Statut = Disponible) |> List.length
    let nbRec   = tousDons |> List.filter (fun d -> d.Statut = Recupere) |> List.length
    let _ = mesDons // reference pour eviter warning inutilise

    Html.div
        [ prop.className "sol-contenu sol-pile"
          prop.children
              [ Html.h1 (I18n.t "mesdons.titre")
                Html.p (I18n.t "mesdons.aide")
                if List.isEmpty model.DonsLocaux then
                    Html.div
                        [ prop.className "sol-pile"
                          prop.children
                              [ alerte "sol-alerte--alerte" "📋" "Aucun don publie dans cette session." "Publie ton premier surplus via l'onglet Publier."
                                // Montre quelques exemples pour l'illustration
                                Html.h2 [ prop.style [ style.marginTop (length.rem 1) ]; prop.text "Exemples de dons" ]
                                Html.div
                                    [ prop.className "dons-liste"
                                      prop.children
                                          (model.DonsExemples |> List.truncate 4 |> List.map (fun don -> carteDon don model.RoleChoisi dispatch)) ] ] ]
                else
                    Html.div
                        [ prop.className "dons-liste"
                          prop.children
                              (model.DonsLocaux |> List.map (fun don -> carteDon don model.RoleChoisi dispatch)) ]

                // Resume chiffre
                Html.div
                    [ prop.className "sol-carte"
                      prop.style [ style.marginTop (length.rem 1) ]
                      prop.children
                          [ Html.h2 "Impact (session locale)"
                            Html.p (sprintf "Disponibles : %d  |  Récupérés : %d" nbDispo nbRec)
                            Html.p [ prop.className "recolte-todo"; prop.text "Le suivi d'impact cumulé (kilos sauvés, repas équivalents) arrivera avec la version connectée." ] ] ] ] ]

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
                item Publier "🎁" "nav.publier"
                item Surplus "🗺" "nav.surplus"
                item MesDons "📋" "nav.mesdons" ] ]

// ---- Vue racine ----
let view (model: Model) (dispatch: Msg -> unit) =
    let contenu =
        match model.RoleChoisi, model.Onglet with
        | None, Accueil -> vueChoixRole dispatch
        | _, Accueil    -> vueAccueil model dispatch
        | _, Publier    -> vuePublier model dispatch
        | _, Surplus    -> vueSurplus model dispatch
        | _, MesDons    -> vueMesDons model dispatch
    Html.div
        [ prop.className "sol-app"
          prop.children
              [ entete model
                Html.main
                    [ prop.id "contenu"
                      prop.style [ style.flexGrow 1 ]
                      prop.children [ contenu ] ]
                nav model dispatch ] ]
