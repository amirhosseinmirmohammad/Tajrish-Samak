/* legacy contact scripts (map init) */
$(document).ready(function () {
            $("#myModal").modal();
        });
    

    
    
        (function () {
            var mapBox = document.getElementById("map");

            if (!mapBox || typeof L === "undefined") {
                return;
            }

            var lat = 35.806270;
            var lng = 51.430387;

            var map = L.map("map", {
                scrollWheelZoom: false,
                zoomControl: true
            }).setView([lat, lng], 18);

            L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                attribution: "کلینیک شنوایی و سمعک شکوه تجریش",
                maxZoom: 19
            }).addTo(map);

            var popupHtml =
                "<div class='gp-map-popup'>" +
                "<img src='/images/logo_purple.webp' alt='کلینیک شنوایی و سمعک شکوه تجریش' />" +
                "<strong>کلینیک شنوایی و سمعک شکوه تجریش</strong>" +
                "<span>میدان تجریش، خیابان شهرداری</span>" +
                "<span>021 2273 5588</span>" +
                "</div>";

            L.marker([lat, lng]).addTo(map).bindPopup(popupHtml).openPopup();

            window.setTimeout(function () {
                map.invalidateSize();
            }, 450);
        })();
    
}