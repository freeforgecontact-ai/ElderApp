// Copie les feuilles partagées du design system dans ./css
// pour que l'app fonctionne 100 % hors-ligne, tout en gardant
// une source de vérité unique dans /design-system.
import { copyFile, mkdir } from 'node:fs/promises'
import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const ici = dirname(fileURLToPath(import.meta.url))
const ds = resolve(ici, '../../../design-system')
const dest = resolve(ici, '../css')

await mkdir(dest, { recursive: true })
for (const f of ['tokens.css', 'components.css']) {
  await copyFile(resolve(ds, f), resolve(dest, f))
  console.log('copié:', f)
}
