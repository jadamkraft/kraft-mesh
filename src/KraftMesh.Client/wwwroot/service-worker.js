// KraftMesh PWA service worker - cache-first for assets
const cacheName = 'kraftmesh-v1';
self.addEventListener('install', (e) => {
  e.waitUntil(caches.open(cacheName).then(() => self.skipWaiting()));
});
self.addEventListener('activate', (e) => {
  e.waitUntil(self.clients.claim());
});
self.addEventListener('fetch', (e) => {
  e.respondWith(caches.match(e.request).then((r) => r || fetch(e.request)));
});
