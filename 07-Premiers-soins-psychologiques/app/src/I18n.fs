module I18n

/// Seul le français est supporté dans cette version (FR d'abord, voir fondations §8).
/// La structure est conservée pour faciliter l'ajout de l'anglais ultérieurement.
type Lang = | Fr

let mutable courante = Fr

let private fr =
    dict
        [ "app.titre",           "Ancrage"
          "nav.accueil",         "Accueil"
          "nav.ancrage",         "Ancrage"
          "nav.plan",            "Mon plan"
          "nav.journal",         "Journal"
          "accueil.titre",       "Tu n'es pas seul·e"
          "accueil.intro",       "Cet outil t'accompagne dans les moments difficiles. Choisis une action ci-dessous."
          "accueil.cadre",       "Outil de soutien et de bien-être — PAS un dispositif médical. Ne remplace ni un thérapeute ni une ligne de crise."
          "ancrage.titre",       "Exercices d'ancrage"
          "ancrage.intro",       "Ces exercices t'aident à revenir au moment présent quand tu te sens débordé·e."
          "plan.titre",          "Mon plan de sécurité"
          "plan.intro",          "Ce plan t'aide à traverser une crise. Remplis-le quand tu vas mieux, utilise-le quand c'est difficile."
          "plan.cadre",          "Ce plan suit le modèle Stanley-Brown, élaboré avec des professionnel·le·s. Il complète — sans remplacer — l'aide professionnelle."
          "journal.titre",       "Journal d'humeur"
          "journal.intro",       "Comment te sens-tu aujourd'hui ?"
          "journal.note",        "Note ou pensée (facultatif)"
          "journal.enregistrer", "Enregistrer"
          "journal.historique",  "Historique"
          "journal.exporter",    "Exporter en CSV"
          "journal.vide",        "Aucune entrée pour l'instant."
          "crise.titre",         "Aide maintenant"
          "crise.sous-titre",    "Ressources de crise — disponibles hors-ligne"
          "crise.intro",         "Si tu traverses une crise, tu n'es pas obligé·e de l'affronter seul·e. Ces ressources sont là pour toi."
          "crise.fermer",        "Fermer" ]

/// Traduit une clé selon la langue courante (renvoie la clé si absente).
let t (cle: string) =
    let d = match courante with Fr -> fr
    match d.TryGetValue cle with
    | true, v -> v
    | _       -> cle
