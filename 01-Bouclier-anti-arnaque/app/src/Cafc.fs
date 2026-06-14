module Cafc

open Domain

/// Une signature de fraude : des indices textuels + sa typologie.
type Entree =
    { Indices: string list
      Type: TypeFraude
      Note: string }

/// Échantillon représentatif des schémas les plus courants au Québec.
/// En production, on charge le jeu de données ouvert du Centre antifraude
/// du Canada et la liste des numéros signalés (voir PLAN.md §6) ; ici on
/// embarque un noyau pour fonctionner 100 % hors-ligne dès la 1re ouverture.
let echantillon: Entree list =
    [ { Indices = [ "petit-fils"; "petite-fille"; "caution"; "avocat"; "accident"; "prison" ]
        Type = GrandsParents
        Note = "Quelqu'un se fait passer pour un proche en détresse et réclame de l'argent vite et en secret." }
      { Indices = [ "carte-cadeau"; "carte cadeau"; "google play"; "itunes"; "steam" ]
        Type = FausseInstitution
        Note = "On exige un paiement en cartes-cadeaux : c'est presque toujours une arnaque." }
      { Indices = [ "arc"; "revenu"; "impot"; "impôt"; "mandat"; "arrestation"; "police" ]
        Type = FausseInstitution
        Note = "Menace d'arrestation pour des impôts impayés : l'ARC ne procède jamais ainsi." }
      { Indices = [ "microsoft"; "virus"; "ordinateur"; "accès à distance"; "anydesk"; "teamviewer" ]
        Type = FauxSoutien
        Note = "Faux soutien technique qui veut prendre le contrôle de ton appareil." }
      { Indices = [ "amour"; "rencontre"; "militaire"; "western union"; "crypto"; "bitcoin" ]
        Type = Amoureuse
        Note = "Une relation en ligne qui finit par demander de l'argent." } ]

let private normaliser (s: string) =
    s.ToLowerInvariant().Replace("’", "'")

/// Évalue une saisie (texto, courriel, description d'appel) contre l'échantillon.
let verifier (saisie: string) : Verdict =
    let t = normaliser saisie
    if t.Trim() = "" then
        Inconnu
    else
        let trouve =
            echantillon
            |> List.tryFind (fun e ->
                e.Indices |> List.exists (fun ind -> t.Contains(normaliser ind)))
        match trouve with
        | Some e -> Suspect(e.Type, e.Note)
        | None -> Inconnu
