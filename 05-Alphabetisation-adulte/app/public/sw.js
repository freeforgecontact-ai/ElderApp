/* Service worker — offline-first (app shell precache + cache-first).
   Volontairement simple et sans dépendance. Incrémente CACHE à chaque
   release pour invalider proprement l'ancien cache. */
const CACHE = 'mots-v1';
const APP_SHELL = [
  '/', '/index.html',
  '/css/tokens.css', '/css/components.css', '/css/app.css',
  '/manifest.webmanifest', '/icons/icon.svg'
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
  // Navigation : renvoie l'app shell hors-ligne.
  if (request.mode === 'navigate') {
    e.respondWith(fetch(request).catch(() => caches.match('/index.html')));
    return;
  }
  // Ressources : cache d'abord, réseau en repli, puis mise en cache.
  e.respondWith(
    caches.match(request).then((hit) => hit || fetch(request).then((res) => {
      const copie = res.clone();
      caches.open(CACHE).then((c) => c.put(request, copie)).catch(() => {});
      return res;
    }).catch(() => hit))
  );
});
