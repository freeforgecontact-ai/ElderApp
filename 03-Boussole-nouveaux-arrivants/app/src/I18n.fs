module I18n

type Lang = Fr | En

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre",           "Boussole"
          "app.titre.long",      "Boussole nouveaux arrivants"
          "nav.accueil",         "Accueil"
          "nav.checklist",       "Ma liste"
          "nav.annuaire",        "Annuaire"
          "nav.courriers",       "Courriers"
          "accueil.soustitre",   "Votre guide d'établissement au Canada"
          "accueil.statut",      "Mon statut"
          "accueil.changer",     "Changer de statut"
          "accueil.q.statut",    "Quel est votre statut ?"
          "accueil.aide.statut", "Choisissez votre situation."
          "accueil.ignorer",     "Je ne sais pas"
          "checklist.titre",     "Ma checklist"
          "checklist.progression","étape(s) complétée(s)"
          "checklist.vide",      "Aucune étape pour cet horizon."
          "annuaire.titre",      "Annuaire 211"
          "annuaire.sous",       "Services près de vous"
          "courriers.titre",     "Aide aux courriers IRCC"
          "courriers.sous",      "Choisissez le type de lettre."
          "disclaimer",          "Information générale — pas un avis juridique." ]

let private en =
    dict
        [ "app.titre",           "Compass"
          "app.titre.long",      "Newcomers Compass"
          "nav.accueil",         "Home"
          "nav.checklist",       "My List"
          "nav.annuaire",        "Directory"
          "nav.courriers",       "Letters"
          "accueil.soustitre",   "Your settlement guide"
          "accueil.statut",      "My status"
          "accueil.changer",     "Change status"
          "accueil.q.statut",    "What is your status?"
          "accueil.aide.statut", "Choose your situation."
          "accueil.ignorer",     "I do not know"
          "checklist.titre",     "My settlement checklist"
          "checklist.progression","step(s) completed"
          "checklist.vide",      "No steps for this horizon."
          "annuaire.titre",      "211 Directory"
          "annuaire.sous",       "Services near you"
          "courriers.titre",     "IRCC Letter Help"
          "courriers.sous",      "Choose the type of letter."
          "disclaimer",          "General information only - not legal advice." ]

let t (cle: string) =
    let d = match courante with Fr -> fr | En -> en
    match d.TryGetValue cle with
    | true, v -> v
    | _ -> cle
