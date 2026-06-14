# Design system — Apps-Solidaires

Source de vérité visuelle des 8 applications. Trois fichiers, zéro dépendance.

| Fichier | Rôle |
|---------|------|
| `tokens.css` | Couleurs, typographie, espacement, ombres, rayons, durées. **Un seul endroit à modifier.** |
| `components.css` | Composants accessibles préfixés `.sol-` (boutons, cartes, alertes, champs, checklist…). |
| `demo.html` | Vitrine : ouvre-la dans un navigateur pour tout voir (fonctionne hors-ligne, sans build). |

## Couleurs

- **Primaire `#0F4C81`** — bleu PGRG (theme-color officiel de ia.pgrg.ca).
- **Accent `#F4B41A`** — or chaleureux (les 💛 de la marque).
- Le reste de l'échelle (bleus, or, neutres, sémantique) est dérivé pour rester cohérent et accessible.

> Pour coller les valeurs **exactes** du site : change seulement `--c-primary` et `--c-accent` dans `tokens.css`.

## Réglages d'un coup

- **Design 100 % carré** (« 0 coin rond » littéral) : `--radius-base: 0;`
- **Mode sombre** : `data-theme="sombre"` sur `<html>` (sinon auto selon le système).
- **Gros texte** (aînés, faible littératie) : `data-echelle="grande"` ou `"enorme"`.

## Accessibilité (intégrée)

WCAG 2.2 AA/AAA : contrastes élevés, focus visible (anneau or), cibles ≥ 44 px, `prefers-reduced-motion`, `prefers-contrast`, lecture vocale prévue (bouton `.sol-vocal`).

## Usage dans une app

```html
<link rel="stylesheet" href="./css/tokens.css">
<link rel="stylesheet" href="./css/components.css">
<link rel="stylesheet" href="./css/app.css"> <!-- styles propres à l'app -->
```

Chaque app embarque sa copie des deux fichiers partagés (via `scripts/copy-ds.mjs`) pour fonctionner **100 % hors-ligne**.
