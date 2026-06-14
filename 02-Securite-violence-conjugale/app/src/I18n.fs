module I18n

type Lang =
    | Fr
    | En

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre",            "Notes perso"
          "nav.accueil",          "Accueil"
          "nav.plan",             "Mon plan"
          "nav.ressources",       "Aide"
          "nav.reglages",         "Réglages"
          "plan.titre",           "Plan de sortie"
          "plan.progression",     "Progression"
          "ressources.titre",     "Ressources d'aide"
          "reglages.titre",       "Réglages"
          "reglages.camouflage",  "Camouflage"
          "reglages.effacement",  "Effacement rapide"
          "accueil.bienvenue",    "Ici en sécurité"
          "accueil.mise_en_garde","Cet outil est un point de départ." ]

let private en =
    dict
        [ "app.titre",            "Personal Notes"
          "nav.accueil",          "Home"
          "nav.plan",             "My plan"
          "nav.ressources",       "Help"
          "nav.reglages",         "Settings"
          "plan.titre",           "Safety plan"
          "plan.progression",     "Progress"
          "ressources.titre",     "Help resources"
          "reglages.titre",       "Settings"
          "reglages.camouflage",  "Disguise"
          "reglages.effacement",  "Quick erase"
          "accueil.bienvenue",    "Safe here"
          "accueil.mise_en_garde","This tool is a starting point." ]

let t (cle: string) : string =
    let dico =
        match courante with
        | Fr -> fr
        | En -> en
    match dico.TryGetValue(cle) with
    | true, v -> v
    | _       -> cle
