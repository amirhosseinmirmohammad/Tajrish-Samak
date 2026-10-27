/* Tajrish Samak safe service worker - v86 lottie fallback fix */
const GP_CACHE = "tajrish-samak-v86";
const GP_OFFLINE_URL = "/offline.html";

const GP_ASSETS = [
    GP_OFFLINE_URL,
    "/manifest.webmanifest",
    "/content/images/pwa/icon-192x192.png",
    "/content/images/pwa/icon-512x512.png",
    "/content/images/pwa/icon-maskable-192x192.png",
    "/content/images/pwa/icon-maskable-512x512.png",
    "/content/images/pwa/apple-touch-icon-180x180.png",
    "/content/images/pwa/favicon-32x32.png",
    "/content/images/pwa/favicon-16x16.png",
    "/images/logo_purple.webp"
];

self.addEventListener("install", event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(GP_CACHE).then(cache => {
            return Promise.all(GP_ASSETS.map(url => cache.add(url).catch(() => undefined)));
        })
    );
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(keys.filter(key => key !== GP_CACHE).map(key => caches.delete(key))))
            .then(() => self.clients.claim())
    );
});

self.addEventListener("message", event => {
    if (event.data && event.data.type === "SKIP_WAITING") {
        self.skipWaiting();
    }
});

function gpIsHtmlNavigation(request) {
    return request.mode === "navigate" ||
        (request.headers.get("accept") || "").includes("text/html");
}

function gpShouldSkip(request) {
    const url = new URL(request.url);
    if (request.method !== "GET") return true;
    if (url.origin !== self.location.origin) return true;

    const pathname = url.pathname.toLowerCase();
    return pathname.startsWith("/account/") ||
        pathname.startsWith("/admin") ||
        pathname.startsWith("/order") ||
        pathname.startsWith("/cart") ||
        pathname.includes("/submit") ||
        pathname.includes("/login") ||
        pathname.includes("/logout");
}

async function gpNetworkFirstNavigation(request) {
    try {
        const response = await fetch(request);
        return response;
    } catch (error) {
        const offline = await caches.match(GP_OFFLINE_URL);
        return offline || new Response("Offline", {
            status: 503,
            headers: { "Content-Type": "text/plain; charset=utf-8" }
        });
    }
}

async function gpCacheFirstStatic(request) {
    const cached = await caches.match(request);
    if (cached) {
        fetch(request)
            .then(response => {
                if (response && response.ok) {
                    caches.open(GP_CACHE).then(cache => cache.put(request, response.clone())).catch(() => undefined);
                }
            })
            .catch(() => undefined);

        return cached;
    }

    try {
        const response = await fetch(request);
        if (response && response.ok) {
            const url = new URL(request.url);
            const path = url.pathname.toLowerCase();
            if (
                path.endsWith(".css") ||
                path.endsWith(".js") ||
                path.endsWith(".png") ||
                path.endsWith(".jpg") ||
                path.endsWith(".jpeg") ||
                path.endsWith(".webp") ||
                path.endsWith(".svg") ||
                path.endsWith(".woff") ||
                path.endsWith(".woff2") ||
                path.endsWith(".webmanifest") ||
                path.endsWith(".json")
            ) {
                caches.open(GP_CACHE).then(cache => cache.put(request, response.clone())).catch(() => undefined);
            }
        }
        return response;
    } catch (error) {
        return cached || Response.error();
    }
}

self.addEventListener("fetch", event => {
    const request = event.request;

    if (gpShouldSkip(request)) {
        return;
    }

    if (gpIsHtmlNavigation(request)) {
        event.respondWith(gpNetworkFirstNavigation(request));
        return;
    }

    event.respondWith(gpCacheFirstStatic(request));
});
