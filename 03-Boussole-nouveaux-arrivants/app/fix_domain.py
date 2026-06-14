import sys

# These are the lines currently in /tmp/b3/src/Domain.fs (75 lines, ending at Categorie211 module)
# We need to append the rest

rest = """
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
        | DecisionPositive  -> "Decision favorable"
        | DecisionNegative  -> "Decision defavorable"
        | DemandeDocument   -> "Demande de documents"
        | RenouvellerPermis -> "Avis de renouvellement"
        | AvisGeneral       -> "Avis general"

    let icone =
        function
        | Convocation       -> "XC"
        | DecisionPositive  -> "XP"
        | DecisionNegative  -> "XN"
        | DemandeDocument   -> "XD"
        | RenouvellerPermis -> "XR"
        | AvisGeneral       -> "XG"

    let explication =
        function
        | Convocation       -> "IRCC vous demande de vous presenter. Apportez vos documents."
        | DecisionPositive  -> "Votre demande a ete acceptee. Conservez ce document."
        | DecisionNegative  -> "Votre demande a ete refusee. Consultez un conseiller juridique."
        | DemandeDocument   -> "IRCC a besoin de documents. Respectez le delai indique."
        | RenouvellerPermis -> "Votre permis expire. Renouvelez 90 jours avant."
        | AvisGeneral       -> "IRCC vous envoie une information. Lisez la lettre."

    let tous = [ Convocation; DecisionPositive; DecisionNegative; DemandeDocument; RenouvellerPermis; AvisGeneral ]
"""

with open("/tmp/b3/src/Domain.fs", "a") as f:
    f.write(rest)

lines = sum(1 for _ in open("/tmp/b3/src/Domain.fs"))
print("Domain.fs now has " + str(lines) + " lines")