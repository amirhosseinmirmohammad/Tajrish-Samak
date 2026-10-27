/* legacy home scripts: modal, auto slider + swipe, geo finder */

        $(document).ready(function () {
            $("#myModal").modal();
        });


        (function () {
            var sliders = document.querySelectorAll("[data-gp-auto-slider]");
            if (!sliders || !sliders.length) {
                return;
            }

            function getSlides(slider) {
                return slider.querySelectorAll(".gp-auto-slide");
            }

            function getActiveIndex(slides) {
                for (var i = 0; i < slides.length; i++) {
                    if (slides[i].classList.contains("is-active")) {
                        return i;
                    }
                }
                return 0;
            }

            function goToSlide(slider, targetIndex) {
                var slides = getSlides(slider);
                if (!slides || slides.length <= 1) {
                    return;
                }

                var dots = slider.querySelectorAll(".gp-v8-slider-dots button, .gp-v10-slider-dots button");
                var activeIndex = getActiveIndex(slides);
                var nextIndex = (targetIndex + slides.length) % slides.length;

                if (activeIndex === nextIndex) {
                    return;
                }

                for (var i = 0; i < slides.length; i++) {
                    slides[i].classList.remove("is-active");
                    slides[i].classList.remove("is-leaving");
                }

                slides[activeIndex].classList.add("is-leaving");
                slides[nextIndex].classList.add("is-active");

                for (var d = 0; d < dots.length; d++) {
                    dots[d].classList.toggle("is-active", d === nextIndex);
                }

                window.setTimeout(function () {
                    if (slides[activeIndex]) {
                        slides[activeIndex].classList.remove("is-leaving");
                    }
                }, 750);
            }

            for (var s = 0; s < sliders.length; s++) {
                (function (slider) {
                    var startX = 0;
                    var startY = 0;
                    var moved = false;
                    var threshold = 38;

                    function onStart(x, y) {
                        startX = x;
                        startY = y;
                        moved = false;
                        slider.classList.add("is-dragging");
                    }

                    function onEnd(x, y) {
                        slider.classList.remove("is-dragging");

                        var dx = x - startX;
                        var dy = y - startY;
                        var distance = Math.max(Math.abs(dx), Math.abs(dy));

                        if (distance < threshold) {
                            return;
                        }

                        moved = true;
                        var slides = getSlides(slider);
                        var current = getActiveIndex(slides);

                        if (Math.abs(dy) >= Math.abs(dx)) {
                            goToSlide(slider, current + (dy < 0 ? 1 : -1));
                        } else {
                            goToSlide(slider, current + (dx < 0 ? 1 : -1));
                        }
                    }

                    slider.addEventListener("touchstart", function (event) {
                        if (event.touches && event.touches.length) {
                            onStart(event.touches[0].clientX, event.touches[0].clientY);
                        }
                    }, { passive: true });

                    slider.addEventListener("touchend", function (event) {
                        if (event.changedTouches && event.changedTouches.length) {
                            onEnd(event.changedTouches[0].clientX, event.changedTouches[0].clientY);
                        }
                    }, { passive: true });

                    slider.addEventListener("mousedown", function (event) {
                        if (event.button !== 0) {
                            return;
                        }
                        onStart(event.clientX, event.clientY);

                        function upHandler(upEvent) {
                            document.removeEventListener("mouseup", upHandler);
                            onEnd(upEvent.clientX, upEvent.clientY);
                        }

                        document.addEventListener("mouseup", upHandler);
                    });

                    slider.addEventListener("click", function (event) {
                        if (moved) {
                            event.preventDefault();
                            event.stopPropagation();
                            moved = false;
                        }
                    }, true);
                })(sliders[s]);
            }
        })();




        /* Home Finder v85 - service / brand / area smart router */
        (function () {
            "use strict";

            var serviceSelect = document.getElementById("gpFinderServiceV85");
            var areaSelect = document.getElementById("gpFinderAreaV85");
            var brandSelect = document.getElementById("gpFinderBrandV85");
            var searchInput = document.getElementById("gpFinderSearchV85");
            var goBtn = document.getElementById("gpFinderGoV85");
            var buildBtn = document.getElementById("gpFinderBuildV85");
            var preview = document.getElementById("gpFinderPreviewV85");
            var suggestions = document.getElementById("gpFinderSuggestionsV85");

            if (!serviceSelect || !areaSelect || !brandSelect || !searchInput || !preview) {
                return;
            }

            var searchMap = [
                { keys: ["بهترین کلینیک شنوایی", "بهترین کلینیک", "بهترین مرکز شنوایی", "کلینیک معتبر شنوایی"], service: "best-audiology-clinic" },
                { keys: ["بهترین مرکز سمعک", "بهترین کلینیک سمعک"], service: "best-hearing-aid-clinic" },
                { keys: ["قیمت سمعک سال ۱۴۰۵", "قیمت سمعک 1405", "قیمت سمعک ۱۴۰۵"], service: "hearing-aid-price-1405" },
                { keys: ["قیمت سمعک", "سمعک چنده"], service: "hearing-aid-price" },
                { keys: ["سمعک قسطی", "اقساط"], service: "installment-hearing-aid" },
                { keys: ["باتری سمعک"], service: "hearing-aid-battery" },
                { keys: ["فیلتر سمعک"], service: "hearing-aid-filter" },
                { keys: ["تعمیر سمعک", "تعمیرات سمعک"], service: "hearing-aid-repair" },
                { keys: ["گارانتی سمعک", "ضمانت سمعک"], service: "hearing-aid-warranty" },
                { keys: ["سمعک با بیمه"], service: "insurance-hearing-aid" },
                { keys: ["تعرفه سمعک با بیمه"], service: "hearing-aid-insurance-tariff" },
                { keys: ["سمعک دولتی", "سمعک دولتی تهران", "سمعک دولتی شمال تهران"], service: "government-hearing-aid" },
                { keys: ["سمعک رایگان بهزیستی", "سمعک رایگان", "سمعک بهزیستی"], service: "free-behzisti-hearing-aid" },
                { keys: ["حمایت ناشنوایان", "حمایت کم شنوایان", "حمایت کم‌شنوایان"], service: "deaf-support" },
                { keys: ["قالب سمعک"], service: "earmold" },
                { keys: ["قالب ضد آب"], service: "waterproof-earmold" },
                { keys: ["قالب سیلیکونی"], service: "silicone-earmold" },
                { keys: ["تعویض قالب"], service: "earmold-replacement" },
                { keys: ["هزینه تست شنوایی"], service: "hearing-test-price" },
                { keys: ["آزمایش شنوایی چنده"], service: "audiometry-price" },
                { keys: ["سمعک شارژی"], service: "rechargeable-hearing-aid" },
                { keys: ["سمعک بدون باتری"], service: "battery-free-hearing-aid" },
                { keys: ["سمعک نامرئی"], service: "invisible-hearing-aid" },
                { keys: ["ویزیت در منزل"], service: "home-visit-hearing-aid" },
                { keys: ["تجویز سمعک در منزل"], service: "in-home-hearing-aid-prescription" },
                { keys: ["تنظیم سمعک در منزل"], service: "in-home-hearing-aid-adjustment" },
                { keys: ["سمعک سیگنیا", "قیمت سمعک سیگنیا"], service: "signia-hearing-aid-price" },
                { keys: ["سمعک زیمنس", "قیمت سمعک زیمنس"], service: "siemens-hearing-aid-price" },
                { keys: ["سمعک ویدکس", "قیمت سمعک ویدکس"], service: "widex-hearing-aid-price" },
                { keys: ["سمعک فوناک", "قیمت سمعک فوناک"], service: "phonak-hearing-aid-price" },
                { keys: ["تست شنوایی"], service: "hearing-test" },
                { keys: ["تنظیم سمعک"], service: "hearing-aid-adjustment" },
                { keys: ["خرید سمعک"], service: "hearing-aid" },
                { keys: ["کلینیک شنوایی"], service: "audiology-clinic" }
            ];

            var areaMap = [
                { keys: ["تجریش", "بازار تجریش", "پل تجریش"], area: "tajrish" },
                { keys: ["تهران"], area: "tehran" },
                { keys: ["شمیران"], area: "shemiran" },
                { keys: ["شمیرانات"], area: "shemiranat" },
                { keys: ["شمال تهران"], area: "north-tehran" },
                { keys: ["شمال شرق تهران"], area: "northeast-tehran" },
                { keys: ["نزدیک من"], area: "near-me" },
                { keys: ["دارآباد", "داراباد"], area: "darabad" },
                { keys: ["کاشانک"], area: "kashanak" },
                { keys: ["جماران"], area: "jamaran" },
                { keys: ["دزاشیب"], area: "dezashib" },
                { keys: ["فرمانیه"], area: "farmanieh" },
                { keys: ["نیاوران"], area: "niavaran" },
                { keys: ["قیطریه"], area: "qeytarieh" },
                { keys: ["زعفرانیه"], area: "zaferanieh" },
                { keys: ["الهیه"], area: "elahiyeh" },
                { keys: ["ولنجک"], area: "velenjak" },
                { keys: ["پاسداران"], area: "pasdaran" },
                { keys: ["هروی"], area: "heravi" },
                { keys: ["اقدسیه"], area: "aghdasiyeh" },
                { keys: ["لواسان"], area: "lavasan" },
                { keys: ["فشم"], area: "feshm" }
            ];

            function normalize(value) {
                return (value || "")
                    .toString()
                    .trim()
                    .replace(/ي/g, "ی")
                    .replace(/ك/g, "ک")
                    .replace(/\s+/g, " ");
            }

            function findService(text) {
                var q = normalize(text);
                for (var i = 0; i < searchMap.length; i++) {
                    for (var k = 0; k < searchMap[i].keys.length; k++) {
                        if (q.indexOf(normalize(searchMap[i].keys[k])) >= 0) {
                            return searchMap[i].service;
                        }
                    }
                }
                return serviceSelect.value || "best-audiology-clinic";
            }

            function findArea(text) {
                var q = normalize(text);
                for (var i = 0; i < areaMap.length; i++) {
                    for (var k = 0; k < areaMap[i].keys.length; k++) {
                        if (q.indexOf(normalize(areaMap[i].keys[k])) >= 0) {
                            return areaMap[i].area;
                        }
                    }
                }
                return areaSelect.value || "tajrish";
            }

            function buildUrl() {
                var typed = normalize(searchInput.value);
                var brand = brandSelect.value;
                var service = typed ? findService(typed) : (serviceSelect.value || "");
                var area = typed ? findArea(typed) : (areaSelect.value || "tajrish");

                if (brand && !service && !typed) {
                    return "/hearing-aid-brands/" + brand;
                }

                if (!service && brand) {
                    var brandServiceMap = {
                        signia: "signia-hearing-aid-price",
                        widex: "widex-hearing-aid-price",
                        phonak: "phonak-hearing-aid-price",
                        resound: "hearing-aid"
                    };
                    service = brandServiceMap[brand] || "hearing-aid";
                }

                if (!service) {
                    service = "best-audiology-clinic";
                }

                return "/" + area + "/" + service;
            }

            function updatePreview() {
                var url = buildUrl();
                var link = preview.querySelector("a");
                if (link) {
                    link.setAttribute("href", url);
                    link.textContent = url;
                }
                return url;
            }

            function go() {
                window.location.href = updatePreview();
            }

            function renderSuggestions() {
                var q = normalize(searchInput.value);
                suggestions.innerHTML = "";

                if (!q || q.length < 2) {
                    suggestions.classList.remove("is-visible");
                    return;
                }

                var found = [];
                for (var i = 0; i < searchMap.length; i++) {
                    for (var k = 0; k < searchMap[i].keys.length; k++) {
                        var label = searchMap[i].keys[k];
                        if (normalize(label).indexOf(q) >= 0 || q.indexOf(normalize(label)) >= 0) {
                            found.push({ text: label, service: searchMap[i].service });
                            break;
                        }
                    }
                    if (found.length >= 5) {
                        break;
                    }
                }

                if (!found.length) {
                    suggestions.classList.remove("is-visible");
                    return;
                }

                for (var j = 0; j < found.length; j++) {
                    (function (item) {
                        var btn = document.createElement("button");
                        btn.type = "button";
                        btn.textContent = item.text;
                        btn.addEventListener("click", function () {
                            searchInput.value = item.text;
                            serviceSelect.value = item.service;
                            suggestions.classList.remove("is-visible");
                            updatePreview();
                        });
                        suggestions.appendChild(btn);
                    })(found[j]);
                }

                suggestions.classList.add("is-visible");
            }

            serviceSelect.addEventListener("change", updatePreview);
            areaSelect.addEventListener("change", updatePreview);
            brandSelect.addEventListener("change", updatePreview);
            searchInput.addEventListener("input", function () {
                renderSuggestions();
                updatePreview();
            });

            if (goBtn) {
                goBtn.addEventListener("click", go);
            }

            if (buildBtn) {
                buildBtn.addEventListener("click", go);
            }

            var chips = document.querySelectorAll(".gp-finder-v85-chips button");
            for (var i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    var area = this.getAttribute("data-area") || "tajrish";
                    var service = this.getAttribute("data-service") || "best-audiology-clinic";
                    areaSelect.value = area;
                    serviceSelect.value = service;
                    brandSelect.value = "";
                    searchInput.value = this.textContent || "";
                    updatePreview();
                });
            }

            updatePreview();
        })();

    