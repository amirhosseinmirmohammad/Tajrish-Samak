/* Tajrish Samak safe service worker - v17 */
const GP_CACHE = "tajrish-samak-v17";
const GP_ASSETS = [
    "/",
    "/manifest.webmanifest",
    "/images/logo_purple.webp",
    "/css/gp-tajrish-modern.css",
    "/js/gp-tajrish-modern.js"
];

self.addEventListener("install", event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(GP_CACHE).then(cache => cache.addAll(GP_ASSETS).catch(() => undefined))
    );
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(keys.filter(key => key !== GP_CACHE).map(key => caches.delete(key))))
            .then(() => self.clients.claim())
    );
});

self.addEventListener("fetch", event => {
    const request = event.request;

    if (request.method !== "GET") {
        return;
    }

    if (!request.url.startsWith(self.location.origin)) {
        return;
    }

    event.respondWith(
        caches.match(request).then(cached => {
            return fetch(request)
                .then(response => {
                    if (!response || !response.ok) {
                        return cached || response;
                    }

                    const responseForCache = response.clone();
                    caches.open(GP_CACHE)
                        .then(cache => cache.put(request, responseForCache))
                        .catch(() => undefined);

                    return response;
                })
                .catch(() => cached || Response.error());
        })
    );
});
