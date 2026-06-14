module I18n

/// Langues supportées. FR/EN d'abord (voir 00-FONDATIONS-COMMUNES.md §8).
type Lang =
    | Fr
    | En

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre", "Bouclier anti-arnaque"
          "nav.accueil", "Accueil"
          "nav.verifier", "Vérifier"
          "nav.apprendre", "Apprendre"
          "nav.proches", "Proches"
          "verifier.titre", "Vérifier un message ou un appel"
          "verifier.aide", "Colle le texto, le courriel ou décris l'appel reçu."
          "verifier.bouton", "Vérifier"
          "pause.titre", "Je vérifie avant de payer"
          "proches.titre", "Mon proche de confiance" ]

let private en =
    dict
        [ "app.titre", "Scam Shield"
          "nav.accueil", "Home"
          "nav.verifier", "Check"
          "nav.apprendre", "Learn"
          "nav.proches", "Contacts"
          "verifier.titre", "Check a message or a call"
          "verifier.aide", "Paste the text, the email, or describe the call."
          "verifier.bouton", "Check"
          "pause.titre", "I check before paying"
          "proches.titre", "My trusted contact" ]

/// Traduit une clé selon la langue courante (renvoie la clé si absente).
let t (cle: string) =
    let d = match courante with Fr -> fr | En -> en
    match d.TryGetValue cle with
    | true, v -> v
    | _ -> cle
