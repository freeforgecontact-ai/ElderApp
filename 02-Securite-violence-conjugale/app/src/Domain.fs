module Domain

type Onglet =
    | Accueil
    | PlanSortie
    | Ressources
    | Reglages

type EtapeSortie =
    { Id: int
      Libelle: string
      Faite: bool
      Categorie: string }

module EtapeSortie =
    let defaut : EtapeSortie list =
        [ { Id = 1;  Libelle = "Carte identité / passeport";    Faite = false; Categorie = "Documents" }
          { Id = 2;  Libelle = "Acte de naissance";             Faite = false; Categorie = "Documents" }
          { Id = 3;  Libelle = "Carnet de vaccination";         Faite = false; Categorie = "Documents" }
          { Id = 4;  Libelle = "Documents d'immigration";       Faite = false; Categorie = "Documents" }
          { Id = 5;  Libelle = "Carte d'assurance maladie (RAMQ)";  Faite = false; Categorie = "Documents" }
          { Id = 6;  Libelle = "Numéro d'assurance sociale (NAS)";  Faite = false; Categorie = "Documents" }
          { Id = 7;  Libelle = "Carnet bancaire / chèques";     Faite = false; Categorie = "Finances" }
          { Id = 8;  Libelle = "Carte de débit / crédit";       Faite = false; Categorie = "Finances" }
          { Id = 9;  Libelle = "Épargne d'urgence";               Faite = false; Categorie = "Finances" }
          { Id = 10; Libelle = "Numéros d'assurances";            Faite = false; Categorie = "Finances" }
          { Id = 11; Libelle = "Médicaments pour 30 jours";     Faite = false; Categorie = "Santé" }
          { Id = 12; Libelle = "Ordonnances médicales";         Faite = false; Categorie = "Santé" }
          { Id = 13; Libelle = "Lunettes / appareils auditifs"; Faite = false; Categorie = "Santé" }
          { Id = 14; Libelle = "Vêtements (3 jours)";             Faite = false; Categorie = "Bagages" }
          { Id = 15; Libelle = "Jouets enfants";                Faite = false; Categorie = "Bagages" }
          { Id = 16; Libelle = "Téléphone chargé + chargeur";   Faite = false; Categorie = "Bagages" }
          { Id = 17; Libelle = "Clés (maison / voiture)";         Faite = false; Categorie = "Bagages" }
          { Id = 18; Libelle = "Contacter une maison d'hébergement";  Faite = false; Categorie = "Réseau" }
          { Id = 19; Libelle = "Alerter une personne de confiance";    Faite = false; Categorie = "Réseau" }
          { Id = 20; Libelle = "Connaître le 1 800 363-9010";   Faite = false; Categorie = "Réseau" } ]

    let progression (etapes: EtapeSortie list) : int =
        if List.isEmpty etapes then 0
        else
            let faites = etapes |> List.filter (fun e -> e.Faite) |> List.length
            int (float faites / float etapes.Length * 100.0)

type Camouflage =
    { CamouflageActif: bool
      NomAffiche: string
      IconeId: string
      NipActif: bool
      NipHash: string }

module Camouflage =
    let defaut : Camouflage =
        { CamouflageActif = false
          NomAffiche      = "Notes perso"
          IconeId         = "notes"
          NipActif        = false
          NipHash         = "" }

type Ressource =
    { Nom: string
      Description: string
      Region: string
      Telephone: string option
      Texto: string option }

module Ressource =
    let liste : Ressource list =
        [ { Nom = "SOS violence conjugale"
            Description = "Ligne provinciale 24h/24, 7j/7 — soutien, référence, hébergement."
            Region = "Québec (province)"
            Telephone = Some "1 800 363-9010"
            Texto = Some "438 601-1211" }
          { Nom = "Maison Flora Tristan"
            Description = "Hébergement sécuritaire pour femmes et enfants fuyant la violence."
            Region = "Montréal"
            Telephone = Some "514 939-3463"
            Texto = None }
          { Nom = "La Dauphinelle"
            Description = "Maison d'hébergement et appartements (2e étape)."
            Region = "Montréal"
            Telephone = Some "514 259-8112"
            Texto = None }
          { Nom = "Carrefour pour Elle"
            Description = "Hébergement et suivi pour Laval."
            Region = "Laval"
            Telephone = Some "450 686-8276"
            Texto = None }
          { Nom = "La Bouee"
            Description = "Soutien aux femmes victimes de violence en Lanaudière."
            Region = "Lanaudière"
            Telephone = Some "450 752-2111"
            Texto = None }
          { Nom = "Le Nid"
            Description = "Hébergement d'urgence dans la région de Québec."
            Region = "Québec (ville)"
            Telephone = Some "418 525-8086"
            Texto = None }
          { Nom = "La Sejournelle"
            Description = "Hébergement en Mauricie."
            Region = "Mauricie"
            Telephone = Some "819 379-1011"
            Texto = None }
          { Nom = "La Passerelle"
            Description = "Maison d'hébergement en Outaouais."
            Region = "Outaouais"
            Telephone = Some "819 771-6233"
            Texto = None } ]
