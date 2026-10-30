/* Tajrish Samak - Service Worker v21 (installable, zero-interference)
   - "fetch" handles ONLY top navigations with a pure network passthrough
     (this satisfies the PWA installability check without caching bugs).
   - videos, range requests, SW script: never intercepted. */
const GP_CACHE = "tajrish-samak-v21";

self.addEventListener("install", event => {
    self.skipWaiting();
    event.waitUntil(caches.keys().then(keys =>
        Promise.all(keys.filter(k => k !== GP_CACHE).map(k => caches.delete(k)))
    ).catch(() => undefined));
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(keys.filter(k => k !== GP_CACHE).map(k => caches.delete(k))))
            .then(() => self.clients.claim())
    );
});

self.addEventListener("fetch", event => {
    if (event.request.method !== "GET") return;
    if (event.request.mode === "navigate") {
        event.respondWith(fetch(event.request));
    }
});