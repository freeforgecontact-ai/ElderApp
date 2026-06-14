/// Données de contenu de la Boussole : étapes de la checklist et ressources 211.
/// Ce fichier remplace Cafc.fs du gabarit — même position dans la chaîne de compilation.
/// En production, ce contenu est chargé depuis un fichier JSON versionné et validé par
/// des partenaires (TCRI, organismes d'accueil). Ici : échantillon embarqué MVP.
module Contenu

open Domain

// ---------------------------------------------------------------------------
// Checklist d'établissement (échantillon validé)
// ---------------------------------------------------------------------------

let etapes : Etape list =
    [
      // ---- J+1 ----
      { Id = "j1-hebergement"; Titre = "Trouver un hébergement d'urgence si nécessaire"
        Detail = "Contacter le 211 ou un organisme d'accueil (CSAI, CRIR, PROMIS). Les réfugiés réinstallés sont pris en charge par IRCC à l'arrivée."
        Horizon = JPlusUn; Statuts = []; Faite = false }

      { Id = "j1-argent"; Titre = "Accéder à vos fonds de voyage"
        Detail = "Pour les réfugiés gouvernementaux : l'agent IRCC vous remet vos premiers fonds à l'aéroport. Conservez tous les reçus."
        Horizon = JPlusUn; Statuts = [ RefugieReinstalle ]; Faite = false }

      { Id = "j1-sim"; Titre = "Obtenir une carte SIM locale"
        Detail = "Un numéro canadien est nécessaire pour presque toutes les démarches. Vidéotron, Koodo, Public Mobile offrent des forfaits abordables."
        Horizon = JPlusUn; Statuts = []; Faite = false }

      { Id = "j1-cuaet-enreg"; Titre = "Enregistrer votre CUAET à la frontière ou en ligne"
        Detail = "Présentez votre autorisation CUAET à un agent frontalier ou connectez-vous au portail IRCC pour confirmer votre statut."
        Horizon = JPlusUn; Statuts = [ CUAET ]; Faite = false }

      // ---- J+7 ----
      { Id = "j7-nas"; Titre = "Demander votre Numéro d'assurance sociale (NAS)"
        Detail = "Service Canada (en ligne ou en personne). Requis pour travailler légalement et recevoir des prestations. Gratuit."
        Horizon = JPlusSept; Statuts = [ RefugieReinstalle; ResidentPermanent; TravailleurTemporaire; CUAET ]; Faite = false }

      { Id = "j7-ramq"; Titre = "S'inscrire à la RAMQ (assurance maladie Québec)"
        Detail = "Apportez preuve de statut et preuve de résidence au Québec. Délai de carence de 3 mois pour certains statuts — vérifiez si vous êtes exempté(e)."
        Horizon = JPlusSept; Statuts = []; Faite = false }

      { Id = "j7-compte"; Titre = "Ouvrir un compte bancaire"
        Detail = "Desjardins, BMO et TD acceptent les nouveaux arrivants sans historique de crédit canadien. Apportez votre passeport et preuve de statut."
        Horizon = JPlusSept; Statuts = []; Faite = false }

      { Id = "j7-ecole"; Titre = "Inscrire les enfants à l'école"
        Detail = "Contacter la commission scolaire de votre secteur. Tout enfant a droit à l'école publique, quel que soit son statut. Classes d'accueil disponibles."
        Horizon = JPlusSept; Statuts = []; Faite = false }

      { Id = "j7-permis-travail"; Titre = "Confirmer ou demander un permis de travail"
        Detail = "Les demandeurs d'asile peuvent demander un permis de travail après dépôt de leur demande. Vérifiez votre admissibilité sur le site d'IRCC."
        Horizon = JPlusSept; Statuts = [ DemandeurAsile ]; Faite = false }

      // ---- 1 mois ----
      { Id = "1m-francisation"; Titre = "S'inscrire à la francisation (cours de français)"
        Detail = "Au Québec : cours gratuits offerts par le MIFI. Plusieurs formules (temps plein, soir, en ligne). Chercher via l'annuaire ou appeler le 1 877 341-6433."
        Horizon = UnMois; Statuts = []; Faite = false }

      { Id = "1m-permis-conduire"; Titre = "Échange ou demande de permis de conduire"
        Detail = "La SAAQ reconnaît certains permis étrangers (ententes de réciprocité). Apportez votre permis, preuve de statut et résidence."
        Horizon = UnMois; Statuts = []; Faite = false }

      { Id = "1m-besoins"; Titre = "Rencontrer l'école pour les besoins particuliers"
        Detail = "Informer l'école des allergies, besoins religieux, soutien linguistique. Les écoles ont des ressources d'intégration."
        Horizon = UnMois; Statuts = []; Faite = false }

      { Id = "1m-aide-sociale"; Titre = "Vérifier votre droit aux prestations sociales"
        Detail = "Aide sociale (MIFI, pour demandeurs d'asile), Allocation canadienne pour enfants (ARC), prestation de logement. Chaque statut a des règles différentes."
        Horizon = UnMois; Statuts = [ DemandeurAsile; RefugieReinstalle ]; Faite = false }

      // ---- 3 mois ----
      { Id = "3m-credit"; Titre = "Commencer à bâtir votre historique de crédit"
        Detail = "Demander une carte de crédit sécurisée (dépôt de garantie). Un bon historique facilite la location d'appartement et les prêts futurs."
        Horizon = TroisMois; Statuts = []; Faite = false }

      { Id = "3m-ramq-active"; Titre = "Activer votre carte d'assurance maladie (si délai de carence écoulé)"
        Detail = "Après 3 mois de résidence au Québec pour la plupart des statuts. Vérifier auprès de la RAMQ si vous avez droit à une exemption."
        Horizon = TroisMois; Statuts = []; Faite = false }

      { Id = "3m-logement"; Titre = "Chercher un logement permanent"
        Detail = "Contacter un organisme d'aide au logement (OCI, OMHM, coopératives). Les listes de logements sociaux peuvent être longues — s'inscrire tôt."
        Horizon = TroisMois; Statuts = []; Faite = false }

      { Id = "3m-cisr"; Titre = "Préparer votre audience à la CISR"
        Detail = "Rassembler toutes les preuves (identité, récit, preuves de persécution). Obtenir un représentant (aide juridique ou avocat accrédité). Ne ratez pas votre audience."
        Horizon = TroisMois; Statuts = [ DemandeurAsile ]; Faite = false }

      // ---- 6 mois ----
      { Id = "6m-renouvellement"; Titre = "Vérifier les dates d'expiration de vos permis"
        Detail = "Permis de travail, permis d'études, carte de résident permanent (renouveler 6 mois avant expiration). Un permis expiré = statut illégal."
        Horizon = SixMois; Statuts = [ TravailleurTemporaire; Etudiant; ResidentPermanent ]; Faite = false }

      { Id = "6m-citoyen"; Titre = "Vérifier votre admissibilité à la citoyenneté"
        Detail = "En général, 3 ans de résidence sur 5 ans comme résident permanent. Demande en ligne via le portail IRCC."
        Horizon = SixMois; Statuts = [ ResidentPermanent ]; Faite = false }

      { Id = "6m-emploi"; Titre = "Faire reconnaître vos diplômes et expériences"
        Detail = "Contacter l'ordre professionnel concerné (médecins, ingénieurs, etc.) ou des organismes de reconnaissance (PRIIME, MIFI). Processus parfois long — commencer tôt."
        Horizon = SixMois; Statuts = []; Faite = false }
    ]

// ---------------------------------------------------------------------------
// Ressources 211 (échantillon — Québec)
// ---------------------------------------------------------------------------

let ressources211 : Ressource211 list =
    [
      // Hébergement
      { Id = "r01"; Nom = "OMHM — Office municipal d'habitation de Montréal"
        Categorie = Hebergement; Region = "Montréal"
        Telephone = "514 872-6442"; Site = "https://www.omhm.qc.ca"
        Description = "Logements sociaux et abordables pour personnes à faible revenu, incluant les nouveaux arrivants." }

      { Id = "r02"; Nom = "Maison du Père"
        Categorie = Hebergement; Region = "Montréal"
        Telephone = "514 845-0168"; Site = "https://maisondupere.org"
        Description = "Hébergement d'urgence et ressources pour hommes en situation de vulnérabilité." }

      { Id = "r03"; Nom = "Y des femmes de Montréal"
        Categorie = Hebergement; Region = "Montréal"
        Telephone = "514 866-9941"; Site = "https://www.ydesfemmesmtl.org"
        Description = "Hébergement temporaire pour femmes et enfants, dont les réfugiées et demandeuses d'asile." }

      // Aide alimentaire
      { Id = "r04"; Nom = "Moisson Montréal"
        Categorie = AideAlimentaire; Region = "Montréal"
        Telephone = "514 344-4494"; Site = "https://www.moissonmontreal.org"
        Description = "Banque alimentaire desservant plus de 230 organismes communautaires à Montréal." }

      { Id = "r05"; Nom = "Dépannage Hochelaga"
        Categorie = AideAlimentaire; Region = "Montréal"
        Telephone = "514 523-1039"; Site = "https://www.depannage-hochelaga.org"
        Description = "Épicerie communautaire, dépannage alimentaire d'urgence et cuisines collectives." }

      { Id = "r06"; Nom = "Moisson Québec"
        Categorie = AideAlimentaire; Region = "Québec"
        Telephone = "418 529-4484"; Site = "https://www.moissonquebec.com"
        Description = "Aide alimentaire d'urgence et accompagnement dans la région de Québec." }

      // Francisation
      { Id = "r07"; Nom = "CSDM — Cours de français pour adultes"
        Categorie = Francisation; Region = "Montréal"
        Telephone = "514 596-6000"; Site = "https://www.csdm.ca"
        Description = "Cours de français langue seconde gratuits pour adultes immigrants, plusieurs niveaux." }

      { Id = "r08"; Nom = "MIFI — Francisation en ligne"
        Categorie = Francisation; Region = "Québec (province)"
        Telephone = "1 877 341-6433"; Site = "https://www.quebec.ca/education/apprendre-le-francais"
        Description = "Cours de français gratuits offerts par le MIFI, en présentiel et en ligne." }

      { Id = "r09"; Nom = "Cégep Marie-Victorin — Francisation"
        Categorie = Francisation; Region = "Montréal"
        Telephone = "514 325-0150"; Site = "https://www.collegemv.qc.ca"
        Description = "Programmes d'intégration linguistique à temps plein pour adultes immigrants." }

      // Services juridiques
      { Id = "r10"; Nom = "Aide juridique — CSAJ Montréal"
        Categorie = ServiceJuridique; Region = "Montréal"
        Telephone = "514 864-4555"; Site = "https://www.csj.qc.ca"
        Description = "Aide juridique gratuite pour les personnes à faible revenu, incluant les dossiers d'immigration." }

      { Id = "r11"; Nom = "Clinique de droit des réfugiés — UQAM"
        Categorie = ServiceJuridique; Region = "Montréal"
        Telephone = "514 987-3000"; Site = "https://cqlc.ca"
        Description = "Services juridiques gratuits pour demandeurs d'asile : préparation d'audience CISR, appels, etc." }

      { Id = "r12"; Nom = "Action Réfugiés Montréal"
        Categorie = ServiceJuridique; Region = "Montréal"
        Telephone = "514 935-7799"; Site = "https://actionrefugies.ca"
        Description = "Soutien légal et accompagnement pour réfugiés : parrainage privé, CISR, appels de visa." }

      { Id = "r13"; Nom = "TCRI — Table de concertation des réfugiés et immigrants"
        Categorie = ServiceJuridique; Region = "Montréal"
        Telephone = "514 279-4001"; Site = "https://tcri.qc.ca"
        Description = "Réseau de plus de 140 organismes. Orientation et référence vers des services spécialisés." }
    ]

/// Filtre les étapes selon le statut sélectionné.
/// Statuts = [] signifie « applicable à tous ».
let etapesPourStatut (statut: Statut) : Etape list =
    etapes
    |> List.filter (fun e ->
        e.Statuts = [] || List.contains statut e.Statuts)

/// Filtre les ressources selon la catégorie sélectionnée.
let ressourcesPourCategorie (cat: Categorie211) : Ressource211 list =
    ressources211 |> List.filter (fun r -> r.Categorie = cat)
