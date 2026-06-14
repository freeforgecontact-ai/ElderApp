module Exemples

open Domain

// Données d'exemple embarquées pour le scaffold local (hors-ligne).
// TODO : remplacer par des appels API vers le back léger (Cloudflare Workers + D1)
// et l'intégration Open Food Network Canada (AGPL, bilingue).

let private maintenant = System.DateTime.Now

/// Génère un ID simple (en production : UUID v4 côté serveur).
let private makeId prefix i = sprintf "%s-%03d" prefix i

let donsExemples : Don list =
    [ { Id = makeId "don" 1
        Donneur = "Épicerie Voisin — Rosemont"
        TypeAliment = "Fruits et légumes"
        Quantite = "12 kg (carottes, pommes, courgettes)"
        Froid = Ambiant
        FenetreFin = maintenant.AddHours(6.0)
        Lieu = "4200, rue Masson, Montréal (Rosemont)"
        Statut = Disponible }

      { Id = makeId "don" 2
        Donneur = "Boulangerie La Miche"
        TypeAliment = "Pain et viennoiserie"
        Quantite = "30 pains (baguettes, miches, croissants)"
        Froid = Ambiant
        FenetreFin = maintenant.AddHours(3.0)
        Lieu = "Rue Saint-Denis, Plateau-Mont-Royal"
        Statut = Reserve "Moisson Montréal" }

      { Id = makeId "don" 3
        Donneur = "Restaurant Le Terroir"
        TypeAliment = "Plats cuisinés"
        Quantite = "15 portions de soupe + 8 portions de ragout"
        Froid = Refrigere
        FenetreFin = maintenant.AddHours(4.0)
        Lieu = "Avenue du Mont-Royal Est, Montréal"
        Statut = Disponible }

      { Id = makeId "don" 4
        Donneur = "Jardins communautaires Saint-Michel"
        TypeAliment = "Fruits et légumes"
        Quantite = "5 kg tomates, 3 kg haricots verts, courges"
        Froid = Ambiant
        FenetreFin = maintenant.AddHours(24.0)
        Lieu = "Saint-Michel, Montréal"
        Statut = Disponible }

      { Id = makeId "don" 5
        Donneur = "IGA Extra Côte-des-Neiges"
        TypeAliment = "Produits laitiers"
        Quantite = "Yogourts (DLC demain), fromage cottage, lait"
        Froid = Refrigere
        FenetreFin = maintenant.AddHours(18.0)
        Lieu = "Chemin de la Côte-des-Neiges, Montréal"
        Statut = Disponible }

      { Id = makeId "don" 6
        Donneur = "Metro Ahuntsic"
        TypeAliment = "Viande / poisson"
        Quantite = "Poulet entier (10 unités), saumon (3 kg)"
        Froid = Congele
        FenetreFin = maintenant.AddDays(2.0)
        Lieu = "Boulevard Henri-Bourassa, Ahuntsic"
        Statut = Reserve "Tablée des Chefs" }

      { Id = makeId "don" 7
        Donneur = "Particulier — jardin de Verdun"
        TypeAliment = "Fruits et légumes"
        Quantite = "Courgettes (surplus abondant !), herbes fraîches"
        Froid = Ambiant
        FenetreFin = maintenant.AddHours(48.0)
        Lieu = "Verdun, Montréal"
        Statut = Recupere }

      { Id = makeId "don" 8
        Donneur = "Café de la Place"
        TypeAliment = "Épicerie sèche"
        Quantite = "Sachets de café, sucre, farine (fins de stocks)"
        Froid = Ambiant
        FenetreFin = maintenant.AddDays(7.0)
        Lieu = "Place des Arts, Montréal"
        Statut = Disponible } ]

/// Filtre : dons actuellement disponibles (pas expirés selon fenêtreFin).
let donsDispo (maintenant: System.DateTime) (dons: Don list) =
    dons
    |> List.filter (fun d ->
        d.Statut = Disponible && d.FenetreFin > maintenant)

/// Tri par urgence (fenêtre la plus courte d'abord).
let triParUrgence (maintenant: System.DateTime) (dons: Don list) =
    dons |> List.sortBy (fun d -> (d.FenetreFin - maintenant).TotalSeconds)

/// Tri par contrainte de froid.
let triParFroid (froid: Domain.ContrainteFroid) (dons: Don list) =
    dons |> List.filter (fun d -> d.Froid = froid)

/// Formate la fenêtre restante de façon humaine.
let fenetreRestante (maintenant: System.DateTime) (fin: System.DateTime) =
    let delta = fin - maintenant
    if delta.TotalMinutes < 0.0 then "Expiré"
    elif delta.TotalMinutes < 60.0 then sprintf "⏰ %d min" (int delta.TotalMinutes)
    elif delta.TotalHours < 24.0 then sprintf "⏱ %dh" (int delta.TotalHours)
    else sprintf "📅 %d j" (int delta.TotalDays)
