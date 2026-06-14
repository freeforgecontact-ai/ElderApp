# Fix 1: Add missing module Categorie211 to Domain.fs
# It should go between the Categorie211 type definition and Ressource211 type

domain = open("/tmp/b3/src/Domain.fs", "r").read()

# Insert module Categorie211 after the type Categorie211 block
insertion = """
module Categorie211 =
    let label =
        function
        | Hebergement      -> "Hebergement"
        | AideAlimentaire  -> "Aide alimentaire"
        | Francisation     -> "Francisation"
        | ServiceJuridique -> "Aide juridique"

    let icone =
        function
        | Hebergement      -> "XH"
        | AideAlimentaire  -> "XA"
        | Francisation     -> "XF"
        | ServiceJuridique -> "XJ"

    let toutes = [ Hebergement; AideAlimentaire; Francisation; ServiceJuridique ]

"""

# Find the insertion point: after "    | ServiceJuridique\n\n" and before "type Ressource211"
marker = "    | ServiceJuridique\n\ntype Ressource211"
replacement = "    | ServiceJuridique\n" + insertion + "type Ressource211"

if marker in domain:
    domain = domain.replace(marker, replacement)
    open("/tmp/b3/src/Domain.fs", "w").write(domain)
    lines = domain.count("\n")
    print("Domain.fs fixed: " + str(lines) + " lines, Categorie211 module added")
else:
    print("ERROR: marker not found in Domain.fs")
    # Print the area around ServiceJuridique
    idx = domain.find("ServiceJuridique")
    print("Context: " + repr(domain[idx-20:idx+100]))