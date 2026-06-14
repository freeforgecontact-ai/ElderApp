module Domain

type Onglet =
    | Accueil
    | Checklist
    | Annuaire
    | Courriers

type Statut =
    | RefugieReinstalle
    | DemandeurAsile
    | ResidentPermanent
    | CUAET
    | TravailleurTemporaire
    | Etudiant
module Statut =
    let label =
        function
        | RefugieReinstalle     -> "Réfugié réinstallé"
        | DemandeurAsile        -> "Demandeur d'asile"
        | ResidentPermanent     -> "Résident permanent"
        | CUAET                 -> "CUAET (Ukraine)"
        | TravailleurTemporaire -> "Travailleur temporaire"
        | Etudiant              -> "Étudiant"

    let icone =
        function
        | RefugieReinstalle     -> "🏠"
        | DemandeurAsile        -> "📋"
        | ResidentPermanent     -> "🍁"
        | CUAET                 -> "🌻"
        | TravailleurTemporaire -> "💼"
        | Etudiant              -> "🎓"

    let description =
        function
        | RefugieReinstalle     -> "Sélectionné à l'étranger par le HCR."
        | DemandeurAsile        -> "Demande de protection déposée au Canada."
        | ResidentPermanent     -> "Droit de résidence permanente obtenu."
        | CUAET                 -> "Autorisation de voyage d'urgence Canada-Ukraine."
        | TravailleurTemporaire -> "Permis de travail temporaire."
        | Etudiant              -> "Permis d'études valide."

    let tous = [ RefugieReinstalle; DemandeurAsile; ResidentPermanent; CUAET; TravailleurTemporaire; Etudiant ]

type Horizon =
    | JPlusUn
    | JPlusSept
    | UnMois
    | TroisMois
    | SixMois

module Horizon =
    let label =
        function
        | JPlusUn   -> "Dès l'arrivée (J+1)"
        | JPlusSept -> "Première semaine (J+7)"
        | UnMois    -> "Premier mois"
        | TroisMois -> "3 premiers mois"
        | SixMois   -> "6 premiers mois"
    let tous = [ JPlusUn; JPlusSept; UnMois; TroisMois; SixMois ]

type Etape =
    { Id: string
      Titre: string
      Detail: string
      Horizon: Horizon
      Statuts: Statut list
      Faite: bool }

type Categorie211 =
    | Hebergement
    | AideAlimentaire
    | Francisation
    | ServiceJuridique

module Categorie211 =
    let label =
        function
        | Hebergement      -> "Hébergement"
        | AideAlimentaire  -> "Aide alimentaire"
        | Francisation     -> "Francisation"
        | ServiceJuridique -> "Aide juridique"

    let icone =
        function
        | Hébergement      -> "🏠"
        | AideAlimentaire  -> "🍽️"
        | Francisation     -> "🗣️"
        | ServiceJuridique -> "⚖️"

    let toutes = [ Hebergement; AideAlimentaire; Francisation; ServiceJuridique ]

type Ressource211 =
    { Id: string; Nom: string; Categorie: Categorie211
      Region: string; Telephone: string; Site: string; Description: string }

type TypeCourrier =
    | Convocation | DecisionPositive | DecisionNegative
    | DemandeDocument | RenouvellerPermis | AvisGeneral

module TypeCourrier =
    let label =
        function
        | Convocation       -> "Convocation"
        | DecisionPositive  -> "Décision favorable"
        | DecisionNegative  -> "Décision défavorable"
        | DemandeDocument   -> "Demande de documents"
        | RenouvellerPermis -> "Avis de renouvellement"
        | AvisGeneral       -> "Avis général"

    let icone =
        function
        | Convocation       -> "📅"
        | DecisionPositive  -> "✅"
        | DecisionNegative  -> "❌"
        | DemandeDocument   -> "📎"
        | RenouvellerPermis -> "🔁"
        | AvisGeneral       -> "✉️"

    let explication =
        function
        | Convocation       -> "IRCC vous demande de vous présenter. Apportez vos documents."
        | DecisionPositive  -> "Votre demande a été acceptée. Conservez ce document."
        | DecisionNegative  -> "Votre demande a été refusée. Consultez un conseiller juridique."
        | DemandeDocument   -> "IRCC a besoin de documents. Respectez le délai indiqué."
        | RenouvellerPermis -> "Votre permis expire. Renouvelez 90 jours avant."
        | AvisGeneral       -> "IRCC vous envoie une information. Lisez la lettre."

    let tous = [ Convocation; DecisionPositive; DecisionNegative; DemandeDocument; RenouvellerPermis; AvisGeneral ]
