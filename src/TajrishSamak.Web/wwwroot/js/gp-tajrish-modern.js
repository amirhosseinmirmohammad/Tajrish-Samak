(function () {
    "use strict";

    function ready(fn) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", fn);
        } else {
            fn();
        }
    }

    function showIsland(text) {
        var island = document.getElementById("gpDynamicIsland");
        var islandText = document.getElementById("gpIslandText");
        if (!island || !islandText) {
            return;
        }
        islandText.textContent = text || "انجام شد";
        island.classList.add("is-visible");
        window.clearTimeout(showIsland._timer);
        showIsland._timer = window.setTimeout(function () {
            island.classList.remove("is-visible");
        }, 1700);
    }

    function getThemeToggles() {
        return document.querySelectorAll(".gp-theme-toggle");
    }

    function paintThemeToggles(isDark) {
        var toggles = getThemeToggles();
        for (var i = 0; i < toggles.length; i++) {
            if (!toggles[i].querySelector(".gp-ios-theme-track")) {
                toggles[i].innerHTML = '<span class="gp-ios-theme-track" aria-hidden="true"><span class="gp-ios-theme-sun">☀</span><span class="gp-ios-theme-moon">☾</span><span class="gp-ios-theme-thumb"></span></span><span class="gp-theme-toggle-text"></span>';
            }
            var label = toggles[i].querySelector(".gp-theme-toggle-text");
            if (label) { label.textContent = isDark ? "حالت روز" : "حالت شب"; }
            toggles[i].setAttribute("aria-pressed", isDark ? "true" : "false");
            toggles[i].setAttribute("title", isDark ? "تغییر به حالت روز" : "تغییر به حالت شب");
        }
    }

    function setSavedTheme(mode) {
        var isDark = mode === "dark";
        document.documentElement.classList.toggle("gp-dark-mode", isDark);
        try {
            localStorage.setItem("gpTajrishTheme", isDark ? "dark" : "light");
        } catch (e) { }
        paintThemeToggles(isDark);
    }

    function applyInitialTheme() {
        var saved = null;
        try {
            saved = localStorage.getItem("gpTajrishTheme");
        } catch (e) { }
        if (saved === "dark") {
            document.documentElement.classList.add("gp-dark-mode");
        }
    }

    function ensureHeaderThemeToggle() {
        var existingTop = document.querySelector(".gp-header-tools .gp-theme-toggle");
        if (existingTop) {
            return;
        }

        var header = document.querySelector(".header-bot .header");
        if (!header) {
            return;
        }

        var tools = document.querySelector(".gp-header-tools");
        if (!tools) {
            tools = document.createElement("div");
            tools.className = "gp-header-tools";
            var search = header.querySelector(".agileits_search");
            if (search && search.parentNode === header) {
                if (search.nextSibling) {
                    header.insertBefore(tools, search.nextSibling);
                } else {
                    header.appendChild(tools);
                }
            } else {
                header.appendChild(tools);
            }
        }

        var topButton = document.createElement("button");
        topButton.type = "button";
        topButton.className = "gp-ios-theme-switch gp-theme-toggle gp-theme-toggle-top";
        topButton.setAttribute("data-gp-tip", "تغییر حالت شب و روز");
        topButton.setAttribute("aria-label", "تغییر حالت شب و روز");
        tools.appendChild(topButton);
    }

    function wireThemeToggles() {
        var toggles = getThemeToggles();
        for (var i = 0; i < toggles.length; i++) {
            if (toggles[i].getAttribute("data-gp-theme-bound") === "1") {
                continue;
            }
            toggles[i].setAttribute("data-gp-theme-bound", "1");
            toggles[i].addEventListener("click", function () {
                var next = document.documentElement.classList.contains("gp-dark-mode") ? "light" : "dark";
                setSavedTheme(next);
                showIsland(next === "dark" ? "حالت شب فعال شد" : "حالت روز فعال شد");
            });
        }
        paintThemeToggles(document.documentElement.classList.contains("gp-dark-mode"));
    }

    applyInitialTheme();

    ready(function () {
        ensureHeaderThemeToggle();
        wireThemeToggles();

        var navItems = document.querySelectorAll(".gp-mobile-nav-item");
        var currentPath = (window.location.pathname || "/").toLowerCase();

        for (var i = 0; i < navItems.length; i++) {
            var item = navItems[i];
            var target = (item.getAttribute("data-gp-path") || "").toLowerCase();
            if ((target === "/" && currentPath === "/") || (target !== "/" && currentPath.indexOf(target) === 0)) {
                item.classList.add("is-active");
            }
        }

        var progress = document.getElementById("gpPageProgress");
        function updateProgress() {
            if (!progress) {
                return;
            }
            var max = Math.max(1, document.documentElement.scrollHeight - window.innerHeight);
            var percent = Math.min(100, Math.max(0, (window.pageYOffset / max) * 100));
            progress.style.width = percent + "%";
        }
        updateProgress();
        window.addEventListener("scroll", updateProgress, { passive: true });
        window.addEventListener("resize", updateProgress, { passive: true });

        var revealItems = document.querySelectorAll("#mainheader, .picturebox, #myCarousel, .product-sec1, .why-choose-agile, .product-grid5, .men-pro-item, .wthree_agile_us, .brands a, .gp-footer-card, .gp-footer-column, .gp-footer-bottom-panel");
        for (var r = 0; r < revealItems.length; r++) {
            revealItems[r].classList.add("gp-reveal");
            revealItems[r].style.setProperty("--gp-stagger", String(r % 8));
        }

        if ("IntersectionObserver" in window) {
            var observer = new IntersectionObserver(function (entries) {
                for (var e = 0; e < entries.length; e++) {
                    if (entries[e].isIntersecting) {
                        entries[e].target.classList.add("is-visible");
                        observer.unobserve(entries[e].target);
                    }
                }
            }, { rootMargin: "0px 0px -8% 0px", threshold: 0.08 });

            for (var j = 0; j < revealItems.length; j++) {
                observer.observe(revealItems[j]);
            }
        } else {
            for (var k = 0; k < revealItems.length; k++) {
                revealItems[k].classList.add("is-visible");
            }
        }



        var verticalSliders = document.querySelectorAll("[data-gp-auto-slider]");
        for (var vs = 0; vs < verticalSliders.length; vs++) {
            (function (sliderBox) {
                if (sliderBox.getAttribute("data-gp-slider-ready") === "1") {
                    return;
                }
                sliderBox.setAttribute("data-gp-slider-ready", "1");

                var slides = sliderBox.querySelectorAll(".gp-auto-slide");
                var dotsBox = sliderBox.querySelector(".gp-v8-slider-dots, .gp-v10-slider-dots");
                var activeIndex = 0;
                var timer = null;
                var intervalMs = parseInt(sliderBox.getAttribute("data-gp-interval") || "3800", 10);

                if (!slides || slides.length === 0) {
                    return;
                }

                if (slides.length > 6) {
                    sliderBox.classList.add("has-many-slides");
                }

                if (slides.length <= 1) {
                    slides[0].classList.add("is-active");
                    if (dotsBox) {
                        dotsBox.style.display = "none";
                    }
                    return;
                }

                if (dotsBox) {
                    dotsBox.innerHTML = "";
                    for (var d = 0; d < slides.length; d++) {
                        var dot = document.createElement("button");
                        dot.type = "button";
                        dot.setAttribute("aria-label", "اسلاید " + (d + 1));
                        dot.setAttribute("data-gp-slide-to", d);
                        dotsBox.appendChild(dot);
                    }
                }

                var dots = dotsBox ? dotsBox.querySelectorAll("button") : [];

                function paint(nextIndex, isManual) {
                    var previousIndex = activeIndex;
                    activeIndex = (nextIndex + slides.length) % slides.length;

                    for (var s = 0; s < slides.length; s++) {
                        slides[s].classList.remove("is-active");
                        slides[s].classList.remove("is-leaving");
                    }

                    if (slides[previousIndex] && previousIndex !== activeIndex) {
                        slides[previousIndex].classList.add("is-leaving");
                    }

                    slides[activeIndex].classList.add("is-active");

                    for (var dd = 0; dd < dots.length; dd++) {
                        dots[dd].classList.toggle("is-active", dd === activeIndex);
                    }

                    window.setTimeout(function () {
                        for (var x = 0; x < slides.length; x++) {
                            if (!slides[x].classList.contains("is-active")) {
                                slides[x].classList.remove("is-leaving");
                            }
                        }
                    }, 760);

                    if (isManual) {
                        restart();
                    }
                }

                function restart() {
                    window.clearInterval(timer);
                    timer = window.setInterval(function () {
                        paint(activeIndex + 1, false);
                    }, intervalMs);
                }

                for (var i = 0; i < dots.length; i++) {
                    dots[i].addEventListener("click", function () {
                        var targetIndex = parseInt(this.getAttribute("data-gp-slide-to"), 10);
                        if (!isNaN(targetIndex)) {
                            paint(targetIndex, true);
                        }
                    });
                }

                var touchStartY = 0;
                sliderBox.addEventListener("touchstart", function (event) {
                    if (event.touches && event.touches.length) {
                        touchStartY = event.touches[0].clientY;
                    }
                }, { passive: true });

                sliderBox.addEventListener("touchend", function (event) {
                    if (!event.changedTouches || !event.changedTouches.length) {
                        return;
                    }
                    var diff = event.changedTouches[0].clientY - touchStartY;
                    if (Math.abs(diff) > 36) {
                        paint(activeIndex + (diff < 0 ? 1 : -1), true);
                    }
                }, { passive: true });

                sliderBox.addEventListener("mouseenter", function () {
                    window.clearInterval(timer);
                });

                sliderBox.addEventListener("mouseleave", restart);

                paint(0, false);
                restart();
            })(verticalSliders[vs]);
        }

        var rippleTargets = document.querySelectorAll(".gp-footer-cta, .gp-newsletter-form button, .gp-mobile-nav-item, .brands a, .agileits_search button, .gp-theme-toggle");
        for (var rp = 0; rp < rippleTargets.length; rp++) {
            rippleTargets[rp].addEventListener("click", function (event) {
                var target = event.currentTarget;
                if (!target || !target.getBoundingClientRect) {
                    return;
                }
                var rect = target.getBoundingClientRect();
                var ripple = document.createElement("span");
                ripple.className = "gp-ripple";
                ripple.style.left = (event.clientX - rect.left) + "px";
                ripple.style.top = (event.clientY - rect.top) + "px";
                target.appendChild(ripple);
                window.setTimeout(function () {
                    if (ripple && ripple.parentNode) {
                        ripple.parentNode.removeChild(ripple);
                    }
                }, 650);
            });
        }

        var deferredPrompt = null;
        var pwaCard = document.getElementById("gpPwaHearingCard");
        var installBtn = document.getElementById("gpPwaInstall");
        var closeBtn = document.getElementById("gpPwaClose");

        function isStandalone() {
            return window.matchMedia && window.matchMedia("(display-mode: standalone)").matches;
        }

        function showPwaCard() {
            if (!pwaCard || isStandalone()) {
                return;
            }
            try {
                if (window.localStorage.getItem("gpPwaDismissed") === "1") {
                    return;
                }
            } catch (e) { }
            pwaCard.classList.add("is-visible");
            pwaCard.setAttribute("aria-hidden", "false");
        }

        function hidePwaCard(remember) {
            if (!pwaCard) {
                return;
            }
            pwaCard.classList.remove("is-visible");
            pwaCard.setAttribute("aria-hidden", "true");
            if (remember) {
                try {
                    window.localStorage.setItem("gpPwaDismissed", "1");
                } catch (e) { }
            }
        }

        window.addEventListener("beforeinstallprompt", function (event) {
            event.preventDefault();
            deferredPrompt = event;
            window.setTimeout(showPwaCard, 1300);
        });

        if (installBtn) {
            installBtn.addEventListener("click", function () {
                if (!deferredPrompt) {
                    hidePwaCard(false);
                    showIsland("برای نصب، مرورگر را بررسی کنید");
                    return;
                }
                deferredPrompt.prompt();
                deferredPrompt.userChoice.finally(function () {
                    deferredPrompt = null;
                    hidePwaCard(true);
                    showIsland("انتخاب شما ذخیره شد");
                });
            });
        }

        if (closeBtn) {
            closeBtn.addEventListener("click", function () {
                hidePwaCard(true);
                showIsland("فعلاً نمایش داده نمی‌شود");
            });
        }

        window.addEventListener("appinstalled", function () {
            hidePwaCard(true);
            showIsland("اپلیکیشن وب نصب شد");
        });

        if ("serviceWorker" in navigator) {
            window.addEventListener("load", function () {
                fetch("/service-worker.js", { method: "GET", cache: "no-store" })
                    .then(function (response) {
                        if (response && response.ok) {
                            return navigator.serviceWorker.register("/service-worker.js");
                        }
                        return null;
                    })
                    .catch(function () {
                        // سرویس‌ورکر اختیاری است؛ نبودنش نباید کنسول یا صفحه را خراب کند.
                    });
            });
        }

        if (window.matchMedia && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
            var orb = document.createElement("span");
            orb.className = "gp-cursor-orb";
            document.body.appendChild(orb);

            var active = false;
            var targets = ".picturebox, #myCarousel, .brands a, .men-pro-item, .product-grid5, .wthree_agile_us, .gp-footer-card, .gp-footer-socials a, .gp-footer-cta, .gp-theme-toggle";

            document.addEventListener("mouseover", function (event) {
                if (event.target && event.target.closest && event.target.closest(targets)) {
                    active = true;
                    orb.classList.add("is-visible");
                }
            });

            document.addEventListener("mouseout", function (event) {
                if (event.target && event.target.closest && event.target.closest(targets)) {
                    active = false;
                    orb.classList.remove("is-visible");
                }
            });

            document.addEventListener("mousemove", function (event) {
                if (!active) {
                    return;
                }
                orb.style.left = event.clientX + "px";
                orb.style.top = event.clientY + "px";
            }, { passive: true });
        }

        if (!sessionStorage.getItem("gpTajrishIslandShown")) {
            sessionStorage.setItem("gpTajrishIslandShown", "1");
            window.setTimeout(function () {
                showIsland("شنیدن بهتر از همین‌جا شروع می‌شود");
            }, 650);
        }
    });
})();
