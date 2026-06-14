/* Service worker — hors-ligne strict, nom neutre.
   Apres la premiere visite, AUCUNE requete reseau spontanee : la navigation
   et les ressources sont servies depuis le cache. Le reseau ne sert qu'une
   seule fois, pour mettre en cache une ressource encore absente. */
const CACHE = 'notes-cache-v2';
const APP_SHELL = [
  './', './index.html',
  './css/tokens.css', './css/components.css', './css/app.css',
  './manifest.webmanifest', './icons/icon.svg'
];

self.addEventListener('install', (e) => {
  e.waitUntil(caches.open(CACHE).then((c) => c.addAll(APP_SHELL)).then(() => self.skipWaiting()));
});

self.addEventListener('activate', (e) => {
  e.waitUntil(
    caches.keys().then((ks) => Promise.all(ks.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
      .then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', (e) => {
  const { request } = e;
  if (request.method !== 'GET') return;
  // Cache-first partout (navigation comprise) : pas de requete reseau
  // spontanee a l'ouverture. Repli sur l'app shell si hors cache et hors ligne.
  e.respondWith(
    caches.match(request).then((hit) => {
      if (hit) return hit;
      return fetch(request).then((res) => {
        const copie = res.clone();
        caches.open(CACHE).then((c) => c.put(request, copie)).catch(() => {});
        return res;
      }).catch(() => caches.match('./index.html'));
    })
  );
});
