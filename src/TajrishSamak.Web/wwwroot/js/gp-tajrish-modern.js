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

        var revealItems = document.querySelectorAll("#mainheader, .picturebox, #myCarousel, .product-sec1, .why-choose-agile, .product-grid5, .men-pro-item, .wthree_agile_us, .brands a, .gp-ai-answer-v85-shell, .gp-finder-v85-shell, .gp-v10-showcase-shell, .gp-v8-life-card, .gp-journey-step, .gp-stat-v93-card, .gp-service-v85-card, .gp-product-plan-card, .gp-magazine-card, .gp-popular-card, .gp-about-value, .gp-about-gallery-card, .gp-contact-quick-card, .gp-contact-item, .gp-about-story-card, .gp-about-video-card, .gp-contact-form-card, .gp-contact-info-card, .gp-contact-map-card, .gp-blog-card, .gp-article-hero, .gp-article-figure, .gp-comments-card, .gp-related-card, .gp-side-card, .gpb-card, .gpb-help-band, .gpb-product, .gpb-side-box");
        // NOTE: footer elements are intentionally NOT revealed - contact info and
        // SEO links must be visible immediately on every page, even if JS/IO fails.
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

            // Safety net: if the observer never fires (background tab, browser
            // quirk, missing paint), content must never stay invisible.
            window.setTimeout(function () {
                for (var f = 0; f < revealItems.length; f++) {
                    revealItems[f].classList.add("is-visible");
                }
            }, 2500);
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

        /* ================= PWA install card (Persian, frequency-capped) ================= */
        var deferredPrompt = null;
        var GP_PWA_REASK = 5 * 24 * 60 * 60 * 1000; /* 5 days */

        function pwaIsStandalone() {
            return (window.matchMedia && window.matchMedia("(display-mode: standalone)").matches) || window.navigator.standalone === true;
        }
        function pwaIsIOS() {
            return /iphone|ipad|ipod/i.test(navigator.userAgent) && !window.MSStream;
        }
        function pwaShouldAsk() {
            if (pwaIsStandalone()) return false;
            try {
                if (window.localStorage.getItem("gpPwaInstalled") === "1") return false;
                var dismissed = parseInt(window.localStorage.getItem("gpPwaDismissedAt") || "0", 10);
                if (dismissed && (Date.now() - dismissed) < GP_PWA_REASK) return false;
                if (window.sessionStorage.getItem("gpPwaShownThisSession") === "1") return false;
            } catch (e) { return false; }
            return true;
        }
        function pwaMarkDismissed() {
            try { window.localStorage.setItem("gpPwaDismissedAt", String(Date.now())); } catch (e) { }
        }
        function pwaMarkInstalled() {
            try { window.localStorage.setItem("gpPwaInstalled", "1"); } catch (e) { }
        }
        function pwaBuildCard(kind) {
            var card = document.createElement("div");
            card.id = "gpPwaHearingCard";
            card.setAttribute("dir", "rtl");
            card.style.cssText = "position:fixed;left:14px;right:14px;bottom:24px;max-width:430px;margin:0 auto;z-index:2147483000;" +
                "background:linear-gradient(165deg,#ffffff,#faf5ff);border:1px solid rgba(146,10,182,.22);border-radius:24px;" +
                "padding:20px 20px 16px;box-shadow:0 28px 70px rgba(44,8,66,.35);font-family:inherit;" +
                "transform:translateY(24px);opacity:0;transition:all .4s cubic-bezier(.2,.8,.2,1);";
            if (!document.getElementById("gpPwaCardStyle")) {
                var pst = document.createElement("style");
                pst.id = "gpPwaCardStyle";
                pst.textContent = "@media (max-width:900px){#gpPwaHearingCard{bottom:92px !important;left:10px !important;right:10px !important;}}@media (min-width:901px){#gpPwaHearingCard{bottom:24px !important;}}";
                document.head.appendChild(pst);
            }
            var top = document.createElement("div");
            top.style.cssText = "position:absolute;top:0;left:0;right:0;height:4px;border-radius:24px 24px 0 0;background:linear-gradient(90deg,#920ab6,#c60069,#d1f411);";
            card.appendChild(top);
            var row = document.createElement("div");
            row.style.cssText = "display:flex;align-items:center;gap:12px;";
            var logo = document.createElement("img");
            logo.src = "/images/logo_purple.webp"; logo.alt = "سمعک شکوه تجریش";
            logo.style.cssText = "width:96px;height:40px;object-fit:contain;flex:0 0 auto;";
            var txt = document.createElement("div");
            txt.style.cssText = "flex:1 1 auto;min-width:0;";
            var t1 = document.createElement("b");
            t1.textContent = "نصب اپلیکیشن سمعک شکوه تجریش";
            t1.style.cssText = "display:block;color:#33003f;font-size:13.5px;font-weight:950;line-height:1.7;";
            var t2 = document.createElement("span");
            t2.style.cssText = "display:block;color:rgba(46,22,70,.62);font-size:11.5px;font-weight:700;line-height:1.9;margin-top:2px;";
            if (kind === "ios") {
                t2.textContent = "در سافاری روی آیکون ⬆️ اشتراک‌گذاری بزنید و گزینه Add to Home Screen را انتخاب کنید.";
            } else if (kind === "desktop") {
                t2.textContent = "از منوی مرورگر خود گزینه Install / نصب برنامه را انتخاب کنید.";
            } else if (kind === "android") {
                t2.textContent = "در کروم روی منوی ⋮ بزنید و Add to Home screen را انتخاب کنید.";
            } else {
                t2.textContent = "سایت را مثل اپلیکیشن نصب کنید؛ سریع‌تر، تمام‌صفحه و بدون مرورگر.";
            }
            txt.appendChild(t1); txt.appendChild(t2);
            var close = document.createElement("button");
            close.type = "button"; close.textContent = "✕"; close.setAttribute("aria-label", "بستن");
            close.style.cssText = "position:absolute;top:10px;left:10px;width:30px;height:30px;border:0;border-radius:50%;background:rgba(146,10,182,.10);color:#7a0799;font-size:13px;cursor:pointer;";
            close.addEventListener("click", function () { pwaHide(true); });
            row.appendChild(logo); row.appendChild(txt); row.appendChild(close);
            var chips = document.createElement("div");
            chips.style.cssText = "display:flex;gap:6px;flex-wrap:wrap;margin:12px 0 2px;";
            ["⚡ سریع‌تر از مرورگر", "📱 تمام‌صفحه", "🔔 دسترسی سریع"].forEach(function (c) {
                var ch = document.createElement("span");
                ch.textContent = c;
                ch.style.cssText = "padding:4px 10px;border-radius:999px;background:rgba(146,10,182,.07);color:#7a0799;font-size:10.5px;font-weight:800;";
                chips.appendChild(ch);
            });
            card.appendChild(row); card.appendChild(chips);
            var actions = document.createElement("div");
            actions.style.cssText = "display:flex;gap:8px;margin-top:14px;";
            var install = document.createElement("button");
            install.type = "button";
            install.textContent = kind ? "متوجه شدم" : "📱 نصب اپلیکیشن";
            install.style.cssText = "flex:1 1 auto;min-height:46px;border:0;border-radius:14px;cursor:pointer;" +
                "background:linear-gradient(135deg,#920ab6,#c60069);color:#fff;font-size:13.5px;font-weight:900;font-family:inherit;" +
                "box-shadow:0 10px 24px rgba(146,10,182,.30);transition:transform .15s ease;";
            install.addEventListener("mouseenter", function () { install.style.transform = "translateY(-2px)"; });
            install.addEventListener("mouseleave", function () { install.style.transform = "none"; });
            var later = document.createElement("button");
            later.type = "button"; later.textContent = "فعلاً نه";
            later.style.cssText = "flex:0 0 auto;min-height:46px;padding:0 16px;border:1px solid rgba(146,10,182,.25);border-radius:14px;background:#fff;color:#7a0799;font-size:12.5px;font-weight:850;font-family:inherit;cursor:pointer;";
            actions.appendChild(install); actions.appendChild(later);
            card.appendChild(actions);
            install.addEventListener("click", function () {
                if (kind || !deferredPrompt) { pwaHide(true); return; }
                deferredPrompt.prompt();
                deferredPrompt.userChoice.finally(function () {
                    deferredPrompt = null; pwaHide(true);
                });
            });
            later.addEventListener("click", function () { pwaHide(true); });
            document.body.appendChild(card);
            window.setTimeout(function () {
                card.style.transform = "none"; card.style.opacity = "1";
            }, 40);
            return card;
        }

        function pwaHide(remember) {
            var card = document.getElementById("gpPwaHearingCard");
            if (card) {
                card.style.transform = "translateY(20px)"; card.style.opacity = "0";
                window.setTimeout(function () { if (card.parentNode) card.parentNode.removeChild(card); }, 380);
            }
            if (remember) { pwaMarkDismissed(); }
            try { window.sessionStorage.setItem("gpPwaShownThisSession", "1"); } catch (e) { }
        }

        window.addEventListener("beforeinstallprompt", function (event) {
            event.preventDefault();
            deferredPrompt = event;
            window.setTimeout(function () { if (pwaShouldAsk()) pwaBuildCard(false); }, 2000);
        });

        window.setTimeout(function () {
            if (deferredPrompt || !pwaShouldAsk() || pwaIsStandalone() || document.getElementById("gpPwaHearingCard")) return;
            pwaBuildCard(window.innerWidth <= 900 ? "android" : "desktop");
        }, 4000);
        if (pwaIsIOS() && !pwaIsStandalone()) {
            window.setTimeout(function () { if (pwaShouldAsk()) pwaBuildCard("ios"); }, 2500);
        }

        window.addEventListener("appinstalled", function () {
            pwaMarkInstalled();
            deferredPrompt = null;
            pwaHide(false);
        });

        if ("serviceWorker" in navigator) {
            window.addEventListener("load", function () {
                navigator.serviceWorker.getRegistrations().then(function (regs) {
                    var stale = (regs || []).filter(function (r) {
                        var u = ((r.active && r.active.scriptURL) || "").toLowerCase();
                        return u.indexOf("/service-worker-v20.js") < 0;
                    });
                    return stale.length ? Promise.all(stale.map(function (r) { return r.unregister(); })) : null;
                }).then(function () {
                    return navigator.serviceWorker.register("/service-worker-v20.js");
                }).catch(function () { });
            });
        }

        /* Lightbox for the About-page clinic gallery: click to enlarge,
           prev/next navigation, Esc/backdrop close, Persian counter */
        var aboutGallery = document.querySelector(".gp-about-gallery-grid");
        if (aboutGallery) {
            var lbCards = Array.prototype.slice.call(aboutGallery.querySelectorAll(".gp-about-gallery-card img"));
            var lbIndex = 0;
            var lightbox = null;
            var lbImg = null;
            var lbCounter = null;
            var faDigits = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];

            function faNum(n) {
                return String(n).replace(/\d/g, function (d) { return faDigits[+d]; });
            }

            function buildLightbox() {
                lightbox = document.createElement("div");
                lightbox.className = "gp-lightbox";
                lightbox.setAttribute("role", "dialog");
                lightbox.setAttribute("aria-modal", "true");
                lightbox.setAttribute("aria-label", "نمایش بزرگ تصویر کلینیک");
                lightbox.innerHTML =
                    '<button type="button" class="gp-lightbox-close" aria-label="بستن">×</button>' +
                    '<button type="button" class="gp-lightbox-nav gp-lightbox-prev" aria-label="تصویر قبلی">→</button>' +
                    '<figure class="gp-lightbox-figure"><img class="gp-lightbox-img" alt="" />' +
                    '<figcaption class="gp-lightbox-counter"></figcaption></figure>' +
                    '<button type="button" class="gp-lightbox-nav gp-lightbox-next" aria-label="تصویر بعدی">←</button>';
                document.body.appendChild(lightbox);
                lbImg = lightbox.querySelector(".gp-lightbox-img");
                lbCounter = lightbox.querySelector(".gp-lightbox-counter");

                lightbox.addEventListener("click", function (event) {
                    if (event.target === lightbox) { closeLightbox(); }
                });
                lightbox.querySelector(".gp-lightbox-close").addEventListener("click", closeLightbox);
                lightbox.querySelector(".gp-lightbox-prev").addEventListener("click", function () { stepLightbox(-1); });
                lightbox.querySelector(".gp-lightbox-next").addEventListener("click", function () { stepLightbox(1); });

                document.addEventListener("keydown", function (event) {
                    if (!lightbox.classList.contains("is-open")) { return; }
                    if (event.key === "Escape") { closeLightbox(); }
                    else if (event.key === "ArrowLeft") { stepLightbox(1); }
                    else if (event.key === "ArrowRight") { stepLightbox(-1); }
                });
            }

            function showLightbox(index) {
                if (!lightbox) { buildLightbox(); }
                lbIndex = (index + lbCards.length) % lbCards.length;
                var source = lbCards[lbIndex];
                lbImg.src = source.currentSrc || source.src;
                lbImg.alt = source.alt || "";
                lbCounter.textContent = faNum(lbIndex + 1) + " / " + faNum(lbCards.length);
                lightbox.classList.add("is-open");
                document.body.style.overflow = "hidden";
            }

            function closeLightbox() {
                lightbox.classList.remove("is-open");
                document.body.style.overflow = "";
            }

            function stepLightbox(delta) { showLightbox(lbIndex + delta); }

            lbCards.forEach(function (img, index) {
                img.closest(".gp-about-gallery-card").addEventListener("click", function (event) {
                    event.preventDefault();
                    showLightbox(index);
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
