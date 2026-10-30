/* legacy contact scripts (map init) */
$(document).ready(function () {
            $("#myModal").modal();
        });
    

    
    
        (function () {
            var mapBox = document.getElementById("map");
            if (!mapBox) return;

            var initialized = false;
            var map = null;

            function initMap() {
                if (initialized || typeof L === "undefined") return;
                initialized = true;

                var lat = 35.806270;
                var lng = 51.430387;

                map = L.map("map", {
                    scrollWheelZoom: false,
                    zoomControl: true
                }).setView([lat, lng], 18);

                L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                    attribution: "?????? ?????? ? ???? ???? ?????",
                    maxZoom: 19
                }).addTo(map);

                var popupHtml =
                    "<div class='gp-map-popup'>" +
                    "<img src='/images/logo_purple.webp' alt='?????? ?????? ? ???? ???? ?????' />" +
                    "<strong>?????? ?????? ? ???? ???? ?????</strong>" +
                    "<span>????? ?????? ?????? ???????</span>" +
                    "<span dir='ltr' style='direction:ltr;unicode-bidi:isolate;display:inline-block;'>021 2273 5588</span>" +
                    "</div>";

                L.marker([lat, lng]).addTo(map).bindPopup(popupHtml).openPopup();

                window.setTimeout(function () { map.invalidateSize(); }, 250);
                window.setTimeout(function () { map.invalidateSize(); }, 1200);
                if (window.ResizeObserver) {
                    new ResizeObserver(function () { map.invalidateSize(); }).observe(mapBox);
                }
                window.addEventListener("load", function () { map.invalidateSize(); });
            }

            if (document.readyState === "complete") { initMap(); }
            else { window.addEventListener("load", initMap); }
        })();
