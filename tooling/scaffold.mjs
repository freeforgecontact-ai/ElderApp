import fs from 'fs'; import path from 'path';
// usage: scaffold.mjs <slug> <android|electron> <outDir>
// Empaquette web/<slug>/ (HTML/JS/CSS statique) en projet Electron (bureau) ou Capacitor (Android).
// Repris du tooling FreeForge (freeforge-build).
const [, , slug, target, outDir] = process.argv;
if (!slug || !target || !outDir) { console.error('usage: scaffold.mjs <slug> <android|electron> <outDir>'); process.exit(2); }
const NAMES = {
  bouclier: 'Bouclier anti-arnaque', boussole: 'Boussole', lien: 'Lien',
  mots: 'Mots du quotidien', recolte: 'Recolte', ancrage: 'Ancrage', depart: 'Second depart'
};
const appName = NAMES[slug] || ('Apps-Solidaires ' + slug);
const bundle = 'ca.pgrg.solidaires.' + slug;
const webDir = path.join(process.cwd(), 'web', slug);
if (!fs.existsSync(webDir)) { console.error('web introuvable:', webDir); process.exit(1); }
fs.rmSync(outDir, { recursive: true, force: true });
fs.mkdirSync(path.join(outDir, 'www'), { recursive: true });
function cp(src, dst) {
  for (const e of fs.readdirSync(src, { withFileTypes: true })) {
    const s = path.join(src, e.name), d = path.join(dst, e.name);
    if (e.isDirectory()) { fs.mkdirSync(d, { recursive: true }); cp(s, d); }
    else fs.copyFileSync(s, d);
  }
}
cp(webDir, path.join(outDir, 'www'));

if (target === 'android') {
  fs.writeFileSync(path.join(outDir, 'capacitor.config.json'),
    JSON.stringify({ appId: bundle, appName, webDir: 'www', server: { androidScheme: 'https' } }, null, 2));
  fs.writeFileSync(path.join(outDir, 'package.json'),
    JSON.stringify({ name: 'sol-' + slug, version: '1.0.0', private: true,
      devDependencies: { '@capacitor/cli': '^6.1.2' },
      dependencies: { '@capacitor/core': '^6.1.2', '@capacitor/android': '^6.1.2' } }, null, 2));
} else {
  fs.writeFileSync(path.join(outDir, 'main.js'),
    "const{app,BrowserWindow}=require('electron');const path=require('path');" +
    "function w(){const win=new BrowserWindow({width:1120,height:840,title:" + JSON.stringify(appName) +
    ",backgroundColor:'#0F4C81',webPreferences:{contextIsolation:true,nodeIntegration:false,sandbox:true}});" +
    "win.setMenuBarVisibility(false);win.loadFile(path.join(__dirname,'www','index.html'));}" +
    "app.whenReady().then(()=>{w();app.on('activate',()=>{if(BrowserWindow.getAllWindows().length===0)w();});});" +
    "app.on('window-all-closed',()=>{if(process.platform!=='darwin')app.quit();});");
  fs.writeFileSync(path.join(outDir, 'package.json'),
    JSON.stringify({ name: 'sol-' + slug, version: '1.0.0', description: appName, main: 'main.js',
      scripts: { dist: 'electron-builder' },
      devDependencies: { electron: '^31.7.7', 'electron-builder': '^24.13.3' },
      build: { appId: bundle, productName: appName, directories: { output: 'dist' },
        files: ['main.js', 'www/**/*'],
        win: { target: 'nsis' },
        mac: { target: 'dmg', category: 'public.app-category.utilities' },
        linux: { target: 'AppImage', category: 'Utility' } } }, null, 2));
}
console.log('scaffolded', slug, target, '->', outDir, '(' + appName + ')');
