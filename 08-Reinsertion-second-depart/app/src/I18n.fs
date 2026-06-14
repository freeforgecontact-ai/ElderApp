module I18n

/// Langues supportées. FR d'abord (voir 00-FONDATIONS-COMMUNES.md §8).
type Lang =
    | Fr
    | En

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre",              "Second départ"
          "nav.accueil",            "Accueil"
          "nav.premier-jour",       "48 h"
          "nav.parcours",           "Parcours"
          "nav.annuaire",           "Ressources"
          "nav.contacts",           "Contacts"
          "accueil.titre",          "Ton nouveau départ commence ici"
          "accueil.sous-titre",     "Checklist des premières heures, étapes par situation, ressources et contacts — tout hors-ligne."
          "accueil.choisir",        "Choisir ta situation"
          "premier-jour.titre",     "Les 48 premières heures"
          "parcours.titre",         "Ton parcours"
          "annuaire.titre",         "Ressources et organismes"
          "contacts.titre",         "Mes contacts d'urgence"
          "contacts.ajouter",       "Ajouter un contact"
          "contacts.enregistrer",   "Enregistrer"
          "contacts.role",          "Rôle (ex. : Intervenant·e, Probation)"
          "contacts.nom",           "Nom"
          "contacts.tel",           "Téléphone"
          "rappels.titre",          "Rappels"
          "rappels.ajouter",        "Ajouter un rappel"
          "rappels.quoi",           "Quoi (ex. : Rendez-vous probation)"
          "rappels.quand",          "Quand (ex. : Lundi 9 h)"
          "rappels.recurrent",      "Récurrent (hebdomadaire)"
          "disclaimer",             "Cette application ne remplace pas un·e professionnel·le. En cas de crise, compose le 211 ou le 9-1-1." ]

let private en =
    dict
        [ "app.titre",              "Fresh Start"
          "nav.accueil",            "Home"
          "nav.premier-jour",       "48 h"
          "nav.parcours",           "Journey"
          "nav.annuaire",           "Resources"
          "nav.contacts",           "Contacts"
          "accueil.titre",          "Your new start begins here"
          "accueil.sous-titre",     "First-hours checklist, steps by situation, resources and contacts — all offline."
          "accueil.choisir",        "Choose your situation"
          "premier-jour.titre",     "The first 48 hours"
          "parcours.titre",         "Your journey"
          "annuaire.titre",         "Resources and organizations"
          "contacts.titre",         "My emergency contacts"
          "contacts.ajouter",       "Add a contact"
          "contacts.enregistrer",   "Save"
          "contacts.role",          "Role (e.g.: Worker, Probation)"
          "contacts.nom",           "Name"
          "contacts.tel",           "Phone"
          "rappels.titre",          "Reminders"
          "rappels.ajouter",        "Add a reminder"
          "rappels.quoi",           "What (e.g.: Probation meeting)"
          "rappels.quand",          "When (e.g.: Monday 9 AM)"
          "rappels.recurrent",      "Recurring (weekly)"
          "disclaimer",             "This app does not replace a professional. In a crisis, call 211 or 9-1-1." ]

/// Résout une clé de traduction dans la langue courante.
let t (cle: string) : string =
    let dico =
        match courante with
        | Fr -> fr
        | En -> en
    if dico.ContainsKey(cle) then dico[cle]
    else cle
