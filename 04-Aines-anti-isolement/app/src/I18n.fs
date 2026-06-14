module I18n

/// Langues supportées. Seul le français est exposé dans cette version kiosque.
type Lang =
    | Fr

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre", "Lien"
          "nav.accueil", "Accueil"
          "nav.appeler", "Appeler"
          "nav.humeur", "Humeur"
          "nav.reglages", "Réglages"
          "accueil.bonjour", "Bonjour !"
          "accueil.checkin", "Je vais bien aujourd'hui"
          "accueil.dejafait", "C'est noté — merci !"
          "accueil.dejafait.sous", "Reviens demain pour ton prochain check-in."
          "appeler.titre", "Appeler un proche"
          "appeler.vide", "Aucun contact enregistré."
          "appeler.vide.aide", "Demande à un proche d'ouvrir les Réglages et d'ajouter ses coordonnées."
          "humeur.titre", "Comment tu te sens ?"
          "humeur.dejafait", "Humeur enregistrée."
          "humeur.changer", "Changer l'humeur"
          "reglages.titre", "Réglages famille"
          "reglages.seuil", "Alerte si pas de nouvelles depuis (heures)"
          "reglages.contacts", "Contacts d'escalade"
          "reglages.ajouter", "Ajouter un contact"
          "reglages.sauver", "Enregistrer"
          "reglages.sauve", "Réglages enregistrés."
          "contact.nom", "Nom"
          "contact.tel", "Téléphone"
          "contact.role", "Rôle" ]

let t (cle: string) : string =
    match courante with
    | Fr ->
        match fr.TryGetValue(cle) with
        | true, v -> v
        | _ -> cle
