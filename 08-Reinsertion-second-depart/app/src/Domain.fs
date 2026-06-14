module Domain

/// Onglets de navigation principale.
type Onglet =
    | Accueil
    | PremierJour
    | Parcours
    | Annuaire
    | Contacts

/// Situation de départ de la personne accompagnée.
type Situation =
    | Detention
    | Refuge
    | Dependance

module Situation =
    let label =
        function
        | Detention  -> "Sortie de détention"
        | Refuge     -> "Quitter un refuge"
        | Dependance -> "Fin de thérapie"

    let description =
        function
        | Detention  -> "Tu sors d'un établissement correctionnel et tu veux organiser ton retour."
        | Refuge     -> "Tu quittes un refuge ou un hébergement d'urgence vers plus de stabilité."
        | Dependance -> "Tu termines une thérapie et tu veux structurer tes premières semaines."

    let icone =
        function
        | Detention  -> "🚪"
        | Refuge     -> "🏠"
        | Dependance -> "🌱"

/// Catégories d'étapes dans le parcours.
type Categorie =
    | PapierIdentite
    | Logement
    | Sante
    | AideSociale
    | Emploi
    | Conditions

module Categorie =
    let label =
        function
        | PapierIdentite -> "Papiers"
        | Logement       -> "Logement"
        | Sante          -> "Santé"
        | AideSociale    -> "Aide sociale"
        | Emploi         -> "Emploi"
        | Conditions     -> "Conditions"

    let icone =
        function
        | PapierIdentite -> "🪪"
        | Logement       -> "🏠"
        | Sante          -> "🏥"
        | AideSociale    -> "🤝"
        | Emploi         -> "💼"
        | Conditions     -> "⚖️"

    let toutes = [ PapierIdentite; Logement; Sante; AideSociale; Emploi; Conditions ]

/// Une étape du parcours (48 h ou général).
type Etape =
    { Id: string
      Titre: string
      Details: string
      Categorie: Categorie
      Situations: Situation list
      Echeance: string option
      Faite: bool }

/// Un organisme ou ressource répertorié.
type Ressource =
    { Nom: string
      Description: string
      Region: string
      Telephone: string
      Url: string option
      Situations: Situation list }

/// Contact d'urgence personnalisé.
type ContactUrgence =
    { Role: string
      Nom: string
      Telephone: string }

/// Rappel personnel.
type Rappel =
    { Id: string
      Quoi: string
      Quand: string
      Recurrent: bool }

// ---- Données statiques ----

let premierJourEtapes (situation: Situation) : Etape list =
    let commun =
        [ { Id = "pj-signaler"; Titre = "Signaler ton arrivée à ton intervenant·e"; Details = "Contacte ton travailleur social ou agent de probation dans les 24 h."; Categorie = Conditions; Situations = [Detention; Refuge; Dependance]; Echeance = Some "24 h"; Faite = false }
          { Id = "pj-211"; Titre = "Appeler le 211"; Details = "Le 211 connecte gratuitement avec les services sociaux locaux, hébergement, nourriture, aide."; Categorie = AideSociale; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
          { Id = "pj-nourrit"; Titre = "Trouver à manger aujourd'hui"; Details = "Banque alimentaire, soupes populaires — le 211 peut t'indiquer le plus proche."; Categorie = AideSociale; Situations = [Detention; Refuge; Dependance]; Echeance = Some "Aujourd'hui"; Faite = false }
          { Id = "pj-medecin"; Titre = "Médicaments / ordonnances"; Details = "Si tu as des prescriptions, assure-toi d'avoir au moins 48 h de médicaments."; Categorie = Sante; Situations = [Detention; Refuge; Dependance]; Echeance = Some "Aujourd'hui"; Faite = false } ]
    let specifiques =
        match situation with
        | Detention ->
            [ { Id = "pj-det-probation"; Titre = "Rencontrer ton agent de probation"; Details = "Obligatoire. Confirme ton adresse et tes conditions de libération."; Categorie = Conditions; Situations = [Detention]; Echeance = Some "24-48 h"; Faite = false }
              { Id = "pj-det-nas"; Titre = "Vérifier ta carte d'assurance sociale"; Details = "Si tu ne l'as pas, fais une demande Service Canada en ligne ou en succursale."; Categorie = PapierIdentite; Situations = [Detention]; Echeance = Some "48 h"; Faite = false } ]
        | Refuge ->
            [ { Id = "pj-ref-hebergement"; Titre = "Confirmer ton hébergement temporaire"; Details = "Assure-toi d'avoir un endroit où dormir ce soir. Sinon, appelle le 211."; Categorie = Logement; Situations = [Refuge]; Echeance = Some "Aujourd'hui"; Faite = false }
              { Id = "pj-ref-effets"; Titre = "Récupérer tes effets personnels"; Details = "Si tes affaires sont encore au refuge ou ailleurs, planifie la récupération."; Categorie = Logement; Situations = [Refuge]; Echeance = Some "48 h"; Faite = false } ]
        | Dependance ->
            [ { Id = "pj-dep-suivi"; Titre = "Confirmer ton rendez-vous de suivi"; Details = "Prends contact avec ton thérapeute ou coordonnateur de suivi post-thérapie."; Categorie = Sante; Situations = [Dependance]; Echeance = Some "48 h"; Faite = false }
              { Id = "pj-dep-groupe"; Titre = "Localiser un groupe de soutien"; Details = "AA, NA, ou groupe de soutien recommandé par ta thérapie. Trouver une réunion cette semaine."; Categorie = Sante; Situations = [Dependance]; Echeance = Some "Cette semaine"; Faite = false } ]
    commun @ specifiques

let toutesLesEtapes () : Etape list =
    [ { Id = "e-nas"; Titre = "Obtenir ou renouveler ta carte NAS"; Details = "Service Canada — gratuit. Nécessaire pour travailler et accéder aux prestations."; Categorie = PapierIdentite; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-piece-id"; Titre = "Obtenir une pièce d'identité gouvernementale"; Details = "Permis de conduire ou carte d'identité provinciale."; Categorie = PapierIdentite; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-ramq"; Titre = "S'inscrire à la RAMQ (carte soleil)"; Details = "Si tu n'as pas de couverture santé, fais la demande dès que possible."; Categorie = Sante; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-logement"; Titre = "Sécuriser un logement stable"; Details = "HLM, coopératives, chambres — commence les démarches avec un travailleur social."; Categorie = Logement; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-adresse"; Titre = "Mettre à jour ton adresse"; Details = "Auprès de la RAMQ, banque, Service Canada, probation si applicable."; Categorie = Logement; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-aide-soc"; Titre = "Faire une demande d'aide sociale si nécessaire"; Details = "Revenu Québec — le délai de traitement peut être de 1 à 2 semaines."; Categorie = AideSociale; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-banque"; Titre = "Ouvrir un compte bancaire de base"; Details = "Les banques sont obligées d'offrir un compte de base sans frais."; Categorie = AideSociale; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-emploi"; Titre = "Rencontrer un conseiller en emploi"; Details = "CLE local — aide à mettre à jour le CV, chercher un emploi, formation."; Categorie = Emploi; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-ae"; Titre = "Vérifier l'admissibilité à l'assurance-emploi"; Details = "Si tu as travaillé récemment, une demande AE peut être possible."; Categorie = Emploi; Situations = [Detention; Refuge]; Echeance = None; Faite = false }
      { Id = "e-conditions"; Titre = "Respecter tes conditions de libération"; Details = "Note toutes tes dates de rendez-vous avec la probation dans le calendrier."; Categorie = Conditions; Situations = [Detention]; Echeance = None; Faite = false }
      { Id = "e-avocat"; Titre = "Contacter aide juridique si besoin"; Details = "Pour les questions légales liées à ta libération."; Categorie = Conditions; Situations = [Detention]; Echeance = None; Faite = false }
      { Id = "e-medecin"; Titre = "Trouver un médecin de famille"; Details = "Inscris-toi sur le guichet d'accès de ta région."; Categorie = Sante; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false }
      { Id = "e-sante-mentale"; Titre = "Accéder à du soutien en santé mentale"; Details = "CLSC, Tel-Aide (1-888-935-1101), ou ressources recommandées par ton suivi."; Categorie = Sante; Situations = [Detention; Refuge; Dependance]; Echeance = None; Faite = false } ]

let ressourcesInitiales : Ressource list =
    [ { Nom = "211 Québec"; Description = "Référence vers tous les services sociaux et communautaires. Gratuit, 24/7."; Region = "Québec"; Telephone = "211"; Url = Some "https://www.211qc.ca"; Situations = [Detention; Refuge; Dependance] }
      { Nom = "Maison Crossroads"; Description = "Hébergement de transition pour personnes sortant de détention."; Region = "Montréal"; Telephone = "514 933-4440"; Url = None; Situations = [Detention] }
      { Nom = "Projet Chance"; Description = "Accompagnement à la réinsertion professionnelle et sociale."; Region = "Montréal"; Telephone = "514 527-0997"; Url = None; Situations = [Detention; Refuge] }
      { Nom = "Tel-Aide"; Description = "Écoute et soutien émotionnel, 24 h/24."; Region = "Québec"; Telephone = "1 888 935-1101"; Url = Some "https://www.telaide.org"; Situations = [Detention; Refuge; Dependance] }
      { Nom = "RAMQ — Santé"; Description = "Inscription à l'assurance maladie du Québec."; Region = "Québec"; Telephone = "1 800 561-9749"; Url = Some "https://www.ramq.gouv.qc.ca"; Situations = [Detention; Refuge; Dependance] }
      { Nom = "Aide juridique Québec"; Description = "Représentation légale gratuite ou à faible coût selon revenus."; Region = "Québec"; Telephone = "1 800 842-2213"; Url = Some "https://www.csj.qc.ca"; Situations = [Detention] }
      { Nom = "Maison Héritage"; Description = "Hébergement d'urgence et transition pour femmes sortant de thérapie."; Region = "Québec"; Telephone = "418 522-6543"; Url = None; Situations = [Dependance] }
      { Nom = "Narcotiques Anonymes Québec"; Description = "Groupes de soutien NA partout au Québec."; Region = "Québec"; Telephone = "1 800 879-0333"; Url = Some "https://naquebec.org"; Situations = [Dependance] }
      { Nom = "Alcooliques Anonymes Québec"; Description = "Groupes de soutien AA — réunions quotidiennes partout."; Region = "Québec"; Telephone = "514 376-9230"; Url = Some "https://www.aafrancais.com"; Situations = [Dependance] }
      { Nom = "Réseau Accès Logement"; Description = "Banque de logements abordables et accompagnement pour trouver un logement."; Region = "Québec"; Telephone = "1 800 463-6238"; Url = None; Situations = [Detention; Refuge; Dependance] } ]
