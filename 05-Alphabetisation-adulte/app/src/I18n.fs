module I18n

/// Langues supportées. Français canadien d'abord (voir 00-FONDATIONS-COMMUNES.md §8).
type Lang =
    | Fr
    | En

let mutable courante = Fr

// NOTE : tous les libellés de l'interface sont aussi lus vocalement (synthèse vocale).
// Aucun texte n'est exigé pour naviguer — l'interface fonctionne entièrement
// par pictogrammes + voix.

let private fr =
    dict
        [ "app.titre",         "Mots du quotidien"
          "app.soustitre",     "Apprends à ton rythme, avec des images et ta voix."
          "nav.accueil",       "Accueil"
          "nav.module",        "Exercice"
          "nav.jardin",        "Mon jardin"
          "accueil.intro",     "Choisis quelque chose de ta vie de tous les jours."
          "module.ecouter",    "Écouter"
          "module.suivant",    "Suivant"
          "module.precedent",  "Précédent"
          "module.bravo",      "Bien joué !"
          "module.continuer",  "Continuer"
          "module.retour",     "Retour à l'accueil"
          "jardin.titre",      "Ton jardin"
          "jardin.desc",       "Ton jardin grandit chaque fois que tu pratiques."
          "jardin.etapes",     "exercices complétés"
          "vocal.lire",        "Lire à voix haute" ]

let private en =
    dict
        [ "app.titre",         "Everyday Words"
          "app.soustitre",     "Learn at your own pace, with pictures and your voice."
          "nav.accueil",       "Home"
          "nav.module",        "Practice"
          "nav.jardin",        "My Garden"
          "accueil.intro",     "Choose something from your everyday life."
          "module.ecouter",    "Listen"
          "module.suivant",    "Next"
          "module.precedent",  "Back"
          "module.bravo",      "Well done!"
          "module.continuer",  "Continue"
          "module.retour",     "Back to home"
          "jardin.titre",      "Your Garden"
          "jardin.desc",       "Your garden grows every time you practice."
          "jardin.etapes",     "exercises completed"
          "vocal.lire",        "Read aloud" ]

/// Traduit une clé selon la langue courante (renvoie la clé si absente).
let t (cle: string) =
    let d = match courante with Fr -> fr | En -> en
    match d.TryGetValue cle with
    | true, v -> v
    | _ -> cle
