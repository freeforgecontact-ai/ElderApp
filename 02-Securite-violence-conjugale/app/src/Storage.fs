module Storage

open Fable.Core
open Domain

// ====================================================================
//  Stockage local — clé au nom NEUTRE (aucune mention de « sécurité »).
//  Les données sont soit en clair (préfixe "P:") soit chiffrées ("E:").
// ====================================================================

[<Emit("localStorage.setItem($0, $1)")>]
let private setItem (cle: string) (valeur: string) : unit = jsNative
[<Emit("localStorage.getItem($0)")>]
let private getItem (cle: string) : string = jsNative
[<Emit("localStorage.clear()")>]
let private clearAll () : unit = jsNative

[<Literal>]
let private cleDonnees = "np_v1"

// ====================================================================
//  Chiffrement — WebCrypto : AES-GCM 256, clé dérivée du NIP (PBKDF2).
//  Sans le NIP, les données sont irrécupérables — c'est voulu.
// ====================================================================

// Sel aléatoire (16 octets) en base64.
[<Emit("btoa(String.fromCharCode.apply(null, crypto.getRandomValues(new Uint8Array(16))))")>]
let nouveauSel () : string = jsNative

// Dérive une CryptoKey AES-GCM depuis le NIP et le sel (Promise<CryptoKey>).
[<Emit("""(async (pin, saltB64) => {
  const enc = new TextEncoder();
  const salt = Uint8Array.from(atob(saltB64), c => c.charCodeAt(0));
  const base = await crypto.subtle.importKey('raw', enc.encode(pin), {name:'PBKDF2'}, false, ['deriveKey']);
  return await crypto.subtle.deriveKey({name:'PBKDF2', salt, iterations:150000, hash:'SHA-256'},
                                       base, {name:'AES-GCM', length:256}, false, ['encrypt','decrypt']);
})($0, $1)""")>]
let deriverCle (nip: string) (selB64: string) : JS.Promise<obj> = jsNative

// Chiffre un JSON et le stocke ("E:sel|blob"). Promise<bool>.
[<Emit("""(async (key, json, sel) => {
  const enc = new TextEncoder();
  const iv = crypto.getRandomValues(new Uint8Array(12));
  const ct = await crypto.subtle.encrypt({name:'AES-GCM', iv}, key, enc.encode(json));
  const out = new Uint8Array(iv.length + ct.byteLength);
  out.set(iv, 0); out.set(new Uint8Array(ct), iv.length);
  const blob = btoa(String.fromCharCode.apply(null, out));
  localStorage.setItem('np_v1', 'E:' + sel + '|' + blob);
})($0, $1, $2)""")>]
let chiffrerEtStocker (cle: obj) (json: string) (sel: string) : JS.Promise<unit> = jsNative

// Déchiffre un blob -> JSON, ou "" si le NIP est faux. Promise<string>.
[<Emit("""(async (key, b64) => {
  try {
    const data = Uint8Array.from(atob(b64), c => c.charCodeAt(0));
    const iv = data.slice(0, 12); const ct = data.slice(12);
    const pt = await crypto.subtle.decrypt({name:'AES-GCM', iv}, key, ct);
    return new TextDecoder().decode(pt);
  } catch (e) { return ""; }
})($0, $1)""")>]
let dechiffrer (cle: obj) (b64: string) : JS.Promise<string> = jsNative

// ====================================================================
//  (de)sérialisation JSON maison (un seul objet : prefs + étapes).
// ====================================================================

let private boolStr (b: bool) = if b then "true" else "false"
let private parseBool (s: string) = s.Trim() = "true"

let private getField (s: string) (key: string) : string =
    let prefix = sprintf "\"%s\":" key
    match s.IndexOf(prefix) with
    | -1 -> ""
    | i  ->
        let rest = s.Substring(i + prefix.Length).Trim()
        if rest.StartsWith("\"") then
            let e = rest.IndexOf("\"", 1)
            if e > 0 then rest.Substring(1, e - 1) else ""
        else
            let e = rest.IndexOfAny([|','; '}'; ' '|])
            if e > 0 then rest.Substring(0, e) else rest

let serialiser (etapes: EtapeSortie list) (nom: string) (icone: string) : string =
    let items =
        etapes
        |> List.map (fun e ->
            sprintf "{\"id\":%d,\"libelle\":\"%s\",\"faite\":%s,\"categorie\":\"%s\"}"
                e.Id e.Libelle (boolStr e.Faite) e.Categorie)
        |> String.concat ","
    sprintf "{\"nomAffiche\":\"%s\",\"iconeId\":\"%s\",\"etapes\":[%s]}" nom icone items

let parser (json: string) : EtapeSortie list * string * string =
    let nom   = let v = getField json "nomAffiche" in if v = "" then Camouflage.defaut.NomAffiche else v
    let icone = let v = getField json "iconeId"    in if v = "" then Camouflage.defaut.IconeId    else v
    let etapes =
        let marqueur = "\"etapes\":["
        let i = json.IndexOf(marqueur)
        if i < 0 then EtapeSortie.defaut
        else
            let apres = json.Substring(i + marqueur.Length)
            let fin = apres.IndexOf(']')
            let inner = if fin >= 0 then apres.Substring(0, fin) else apres
            if inner.Trim() = "" then EtapeSortie.defaut
            else
                let items =
                    inner.Split([|"},"|], System.StringSplitOptions.RemoveEmptyEntries)
                    |> Array.toList
                    |> List.map (fun s -> s.TrimStart('{').TrimEnd('}').Trim())
                    |> List.choose (fun s ->
                        try Some { Id = int (getField s "id"); Libelle = getField s "libelle"; Faite = parseBool (getField s "faite"); Categorie = getField s "categorie" }
                        with _ -> None)
                if List.isEmpty items then EtapeSortie.defaut else items
    etapes, nom, icone

// ====================================================================
//  Lecture / écriture
// ====================================================================

type Charge =
    | Clair of string              // JSON en clair
    | Chiffre of string * string   // (selB64, blobB64)
    | Vide

let charger () : Charge =
    let raw = getItem cleDonnees
    if isNull raw || raw = "" then Vide
    elif raw.StartsWith("E:") then
        let corps = raw.Substring(2)
        let sep = corps.IndexOf('|')
        if sep > 0 then Chiffre(corps.Substring(0, sep), corps.Substring(sep + 1)) else Vide
    elif raw.StartsWith("P:") then Clair(raw.Substring(2))
    else Clair(raw)

let sauverClair (json: string) : unit = setItem cleDonnees ("P:" + json)
let effacerTout () : unit = clearAll ()
