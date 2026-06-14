module I18n

/// Langues supportées. FR/EN d'abord (voir 00-FONDATIONS-COMMUNES.md §8).
type Lang =
    | Fr
    | En

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre",          "Récolte — dons alimentaires"
          "nav.accueil",        "Accueil"
          "nav.publier",        "Publier"
          "nav.surplus",        "Surplus"
          "nav.mesdons",        "Mes dons"
          "accueil.titre",      "Surplus alimentaires → organismes"
          "accueil.sous",       "Don gratuit. Zéro transaction. Zéro argent qui circule."
          "publier.titre",      "Publier un don"
          "publier.aide",       "Décris ton surplus. Les organismes à portée seront notifiés."
          "surplus.titre",      "Surplus disponibles"
          "surplus.aide",       "Dons gratuits à proximité — clique pour réserver."
          "mesdons.titre",      "Mes dons"
          "mesdons.aide",       "Historique de tes dons publiés."
          "gratuit.bandeau",    "100 % GRATUIT — Aucune transaction, aucune commission"
          "role.titre",         "Je suis…"
          "role.donneur",       "Je donne"
          "role.donneur.desc",  "Commerce, restaurant, jardin, particulier"
          "role.organisme",     "Je reçois"
          "role.organisme.desc","Banque alimentaire, organisme communautaire"
          "todo.scaffold",      "TODO (scaffold local) — brancher Open Food Network / back léger (Workers + D1) pour la mise en relation temps réel et les notifications push." ]

let private en =
    dict
        [ "app.titre",          "Récolte — food donations"
          "nav.accueil",        "Home"
          "nav.publier",        "Donate"
          "nav.surplus",        "Surplus"
          "nav.mesdons",        "My gifts"
          "accueil.titre",      "Food surplus → organizations"
          "accueil.sous",       "Free gift. Zero transaction. No money involved."
          "publier.titre",      "Post a donation"
          "publier.aide",       "Describe your surplus. Nearby organizations will be notified."
          "surplus.titre",      "Available surplus"
          "surplus.aide",       "Free donations nearby — tap to reserve."
          "mesdons.titre",      "My donations"
          "mesdons.aide",       "History of your posted donations."
          "gratuit.bandeau",    "100% FREE — No transaction, no commission"
          "role.titre",         "I am..."
          "role.donneur",       "I donate"
          "role.donneur.desc",  "Store, restaurant, garden, individual"
          "role.organisme",     "I receive"
          "role.organisme.desc","Food bank, community organization"
          "todo.scaffold",      "TODO (local scaffold) — connect Open Food Network / lightweight back (Workers + D1) for real-time matching and push notifications." ]

/// Traduit une clé selon la langue courante (renvoie la clé si absente).
let t (cle: string) =
    let d = match courante with Fr -> fr | En -> en
    match d.TryGetValue cle with
    | true, v -> v
    | _ -> cle
