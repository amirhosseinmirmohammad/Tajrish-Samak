/* Legacy layout scripts (loader, language toggle, service worker, newsletter) - ported from _Layout.cshtml */
/* NOTE: /Home/NewsLetter is a minimal-API endpoint registered in Program.cs (port of HomeController.NewsLetter). */
/* This file loads in <head>, so any binding that targets footer markup must run on document.ready. */

        $(function () {
            $("#btntext").click(function (e) {
                e.preventDefault()
                $.ajax({                    url: "/Home/NewsLetter",
                    contentType: "application/json; charset=utf-8",
                    data: { text: $("#textnews").val() },
                    dataType: "json",
                    type: "GET",
                    success: function (Data) {
                        if (Data.status == "1") {
                            $("#msg").text(Data.text)
                            $("#msg").removeClass("is-error");
                            $("#msg").css("font-size", "larger");
                            $("#msg").parent("h6").css("padding-top", "20px");
                            $("#msg").parent("h6").css("text-align", "center");
                            $("#msg").parent("h6").css("direction", "rtl");
                        }
                        if (Data.status == "0") {
                            $("#msg").text(Data.text)
                            $("#msg").addClass("is-error");
                            $("#msg").css("font-size", "larger");
                            $("#msg").parent("h6").css("padding-top", "20px");
                            $("#msg").parent("h6").css("text-align", "center");
                            $("#msg").parent("h6").css("direction", "rtl");
                        }
                    },
                    error: function (jqhxr, statusText, error) { alert("خطایی رخ داده است لطفا دوباره امتحان کنید ."); }
                });
            });

            /* Enter in the newsletter input must sign up, not submit the
               wrapping /search form (that looked like "registration failed") */
            $("#textnews").on("keydown", function (e) {
                if (e.key === "Enter" || e.keyCode === 13) {
                    e.preventDefault();
                    $("#btntext").trigger("click");
                }
            });
        });
    
// ----

        /* V88: loader only on navigation click - smooth, short, safe */
        (function () {
            var loader = document.getElementById("gpPageLoader");
            var showAt = 0;
            var minVisibleTime = 220;
            var maxVisibleTime = 1800;
            var hardHideTimer = null;

            function hardHideLoader() {
                if (!loader) return;

                loader.classList.remove("is-visible");
                loader.classList.add("gp-force-hidden");
                loader.setAttribute("aria-hidden", "true");

                if (hardHideTimer) {
                    window.clearTimeout(hardHideTimer);
                    hardHideTimer = null;
                }
            }

            function softHideLoader() {
                if (!loader) return;

                var elapsed = Date.now() - showAt;
                var delay = Math.max(0, minVisibleTime - elapsed);

                window.setTimeout(function () {
                    if (!loader) return;

                    loader.classList.remove("is-visible");
                    loader.setAttribute("aria-hidden", "true");

                    window.setTimeout(function () {
                        loader.classList.add("gp-force-hidden");
                    }, 260);
                }, delay);
            }

            function showLoader() {
                if (!loader || !navigator.onLine) return;

                showAt = Date.now();

                loader.classList.remove("gp-force-hidden");

                window.requestAnimationFrame(function () {
                    loader.classList.add("is-visible");
                    loader.setAttribute("aria-hidden", "false");
                });

                if (hardHideTimer) {
                    window.clearTimeout(hardHideTimer);
                }

                hardHideTimer = window.setTimeout(function () {
                    softHideLoader();
                }, maxVisibleTime);
            }

            function shouldIgnoreLink(link, event) {
                if (!link) return true;

                var href = link.getAttribute("href");

                if (!href) return true;
                if (event && (event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || event.button !== 0)) return true;
                if (href.charAt(0) === "#") return true;
                if (href.indexOf("javascript:") === 0) return true;
                if (href.indexOf("tel:") === 0) return true;
                if (href.indexOf("mailto:") === 0) return true;
                if (link.target === "_blank") return true;
                if (link.hasAttribute("download")) return true;
                if (link.getAttribute("data-toggle")) return true;

                return false;
            }

            window.gpShowPageLoader = showLoader;
            window.gpHidePageLoader = hardHideLoader;

            document.addEventListener("DOMContentLoaded", function () {
                hardHideLoader();

                document.addEventListener("click", function (event) {
                    var link = event.target.closest ? event.target.closest("a[href]") : null;

                    if (shouldIgnoreLink(link, event)) return;

                    showLoader();
                }, true);

                var forms = document.querySelectorAll("form");
                for (var i = 0; i < forms.length; i++) {
                    forms[i].addEventListener("submit", function () {
                        showLoader();
                    });
                }
            });

            window.addEventListener("load", function () {
                window.setTimeout(hardHideLoader, 120);
            });

            window.addEventListener("pageshow", function () {
                window.setTimeout(hardHideLoader, 80);
            });

            window.addEventListener("online", hardHideLoader);
            window.addEventListener("offline", hardHideLoader);

            document.addEventListener("visibilitychange", function () {
                if (!document.hidden) {
                    window.setTimeout(hardHideLoader, 120);
                }
            });
        })();
    
// ----

        (function () {
            var storageKey = "gpTajrishLang";
            var defaultLang = "fa";
            var targetLang = "en";
            var translateReadyTimer = null;

            function getLang() {
                try {
                    return localStorage.getItem(storageKey) || defaultLang;
                } catch (e) {
                    return defaultLang;
                }
            }

            function setCookie(name, value, days) {
                var expires = "";
                if (days) {
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    expires = "; expires=" + date.toUTCString();
                }

                document.cookie = name + "=" + value + expires + "; path=/";
                document.cookie = name + "=" + value + expires + "; path=/; domain=" + window.location.hostname;
                if (window.location.hostname.indexOf("www.") === 0) {
                    document.cookie = name + "=" + value + expires + "; path=/; domain=" + window.location.hostname.replace(/^www\./, "");
                } else {
                    document.cookie = name + "=" + value + expires + "; path=/; domain=." + window.location.hostname;
                }
            }

            function clearTranslateCookies() {
                setCookie("googtrans", "", -1);
                setCookie("googtrans", "/fa/fa", -1);
            }

            function markLanguage(lang) {
                var isEn = lang === targetLang;
                var root = document.documentElement;

                root.setAttribute("lang", isEn ? "en" : "fa");
                root.setAttribute("xml:lang", isEn ? "en" : "fa");
                root.setAttribute("dir", isEn ? "ltr" : "rtl");
                root.classList.toggle("gp-lang-en", isEn);
                root.classList.toggle("gp-lang-fa", !isEn);

                if (document.body) {
                    document.body.setAttribute("dir", isEn ? "ltr" : "rtl");
                }

                var buttons = document.querySelectorAll("[data-gp-lang-toggle]");
                for (var i = 0; i < buttons.length; i++) {
                    buttons[i].setAttribute("aria-pressed", isEn ? "true" : "false");
                    buttons[i].setAttribute("title", isEn ? "نمایش فارسی" : "View in English");
                }
            }

            function triggerGoogleTranslate(lang) {
                var combo = document.querySelector(".goog-te-combo");

                if (!combo) {
                    window.clearTimeout(translateReadyTimer);
                    translateReadyTimer = window.setTimeout(function () {
                        triggerGoogleTranslate(lang);
                    }, 350);
                    return;
                }

                combo.value = lang;
                combo.dispatchEvent(new Event("change"));
            }

            function applyLanguage(lang, shouldReloadIfNeeded) {
                try {
                    localStorage.setItem(storageKey, lang);
                } catch (e) { }

                markLanguage(lang);

                if (lang === targetLang) {
                    setCookie("googtrans", "/fa/en", 365);
                    triggerGoogleTranslate(targetLang);
                } else {
                    clearTranslateCookies();

                    if (shouldReloadIfNeeded) {
                        window.location.reload();
                    }
                }
            }

            window.gpGoogleTranslateInit = function () {
                try {
                    new google.translate.TranslateElement({
                        pageLanguage: "fa",
                        includedLanguages: "en,fa",
                        autoDisplay: false,
                        multilanguagePage: true
                    }, "google_translate_element");

                    if (getLang() === targetLang) {
                        window.setTimeout(function () {
                            triggerGoogleTranslate(targetLang);
                        }, 500);
                    }
                } catch (e) { }
            };

            document.addEventListener("DOMContentLoaded", function () {
                var lang = getLang();
                markLanguage(lang);

                var buttons = document.querySelectorAll("[data-gp-lang-toggle]");
                for (var i = 0; i < buttons.length; i++) {
                    buttons[i].addEventListener("click", function () {
                        var next = getLang() === targetLang ? defaultLang : targetLang;
                        applyLanguage(next, true);
                    });
                }

                if (lang === targetLang) {
                    setCookie("googtrans", "/fa/en", 365);
                }
            });
        })();
    
// ----

        /* V82: service worker + live offline notice */
        (function () {
            var offlineMini = document.getElementById("gpOfflineMini");
            var offlineTitle = document.getElementById("gpOfflineMiniTitle");
            var offlineText = document.getElementById("gpOfflineMiniText");
            var retryBtn = document.getElementById("gpOfflineMiniRetry");
            var hideTimer = null;

            function showMini(title, text, autoHide) {
                if (!offlineMini) return;

                if (offlineTitle) offlineTitle.textContent = title;
                if (offlineText) offlineText.textContent = text;

                offlineMini.classList.add("is-visible");
                offlineMini.setAttribute("aria-hidden", "false");

                if (hideTimer) window.clearTimeout(hideTimer);
                if (autoHide) {
                    hideTimer = window.setTimeout(function () {
                        offlineMini.classList.remove("is-visible");
                        offlineMini.setAttribute("aria-hidden", "true");
                    }, 3500);
                }
            }

            function hideMini() {
                if (!offlineMini) return;
                offlineMini.classList.remove("is-visible");
                offlineMini.setAttribute("aria-hidden", "true");
            }

            function setRetryChecking(isChecking) {
                if (!retryBtn) return;

                if (isChecking) {
                    retryBtn.classList.add("is-checking");
                    retryBtn.setAttribute("aria-busy", "true");
                    retryBtn.textContent = "در حال بررسی";
                } else {
                    retryBtn.classList.remove("is-checking");
                    retryBtn.removeAttribute("aria-busy");
                    retryBtn.textContent = "بررسی مجدد";
                }
            }

            function checkConnection() {
                setRetryChecking(true);

                if (!navigator.onLine) {
                    window.setTimeout(function () {
                        showMini("هنوز اینترنت قطع است", " اتصال را بررسی کنید .", false);
                        setRetryChecking(false);
                        if (window.gpHidePageLoader) window.gpHidePageLoader();
                    }, 650);
                    return;
                }

                fetch("/", { method: "HEAD", cache: "no-store" })
                    .then(function () {
                        window.setTimeout(function () {
                            showMini("اتصال برگشت", "همه‌چیز دوباره آماده است.", true);
                            setRetryChecking(false);
                            if (window.gpHidePageLoader) window.gpHidePageLoader();
                        }, 650);
                    })
                    .catch(function () {
                        window.setTimeout(function () {
                            showMini("اتصال ناپایدار است", "دوباره بررسی کنید؛ شاید شبکه هنوز کامل وصل نشده.", false);
                            setRetryChecking(false);
                            if (window.gpHidePageLoader) window.gpHidePageLoader();
                        }, 650);
                    });
            }

            window.addEventListener("offline", function () {
                showMini("اینترنت قطع شده", "اگر صفحه‌ای باز نشد، نسخه آفلاین نمایش داده می‌شود.", false);
            });

            window.addEventListener("online", function () {
                checkConnection();
            });

            if (retryBtn) {
                retryBtn.addEventListener("click", checkConnection);
            }

            if (!navigator.onLine) {
                showMini("اینترنت قطع شده", "اگر صفحه‌ای باز نشد، نسخه آفلاین نمایش داده می‌شود.", false);
            }

            if ("serviceWorker" in navigator) {
                window.addEventListener("load", function () {
                    navigator.serviceWorker.register("/sw.js")
                        .catch(function () { });
                });
            }
        })();
    
/* =========================================================
   V94: header search autocomplete (vanilla JS, debounced,
   in-memory cache, keyboard navigation)
   ========================================================= */
(function () {
    "use strict";

    var KIND_CLASS = { product: "gp-suggest-product", service: "gp-suggest-service", blog: "gp-suggest-blog", brand: "gp-suggest-brand" };

    function escapeHtml(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    function init() {
        var input = document.getElementById("gpHeaderSearch");
        if (!input || input.getAttribute("data-gp-suggest") === "1") {
            return;
        }
        input.setAttribute("data-gp-suggest", "1");

        var box = input.closest(".gp-header-search-box") || input.parentElement;
        if (!box) return;
        box.classList.add("gp-suggest-host");

        var panel = document.createElement("div");
        panel.className = "gp-suggest-panel";
        panel.id = "gpSuggestPanel";
        panel.setAttribute("role", "listbox");
        box.appendChild(panel);

        var cache = {};
        var timer = null;
        var activeIndex = -1;
        var itemCount = 0;
        var controller = null;

        function close() {
            panel.classList.remove("is-open");
            panel.classList.remove("has-results");
            input.setAttribute("aria-expanded", "false");
            activeIndex = -1;
            itemCount = 0;
        }

        function renderEmpty(term) {
            currentItems = [];
            itemCount = 0;
            panel.innerHTML = '<div class="gp-suggest-empty"><span>🔍</span><b>نتیجه‌ای برای «' + escapeHtml(term) + '» پیدا نشد</b><small>Enter را بزنید تا جستجوی کامل در سایت انجام شود</small></div>';
            panel.classList.add("is-open");
            input.setAttribute("aria-expanded", "true");
        }

        function fallbackGlyph(kind) {
            if (kind === "service") return "✚";
            if (kind === "blog") return "▤";
            if (kind === "brand") return "★";
            return "◖";
        }

        function render(items) {
            if (!items || !items.length) {
                return; // caller decides (empty state)
            }
            var html = '<div class="gp-suggest-head">پیشنهادهای شکوه تجریش</div>';
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var cls = KIND_CLASS[item.kind] || "gp-suggest-product";
                var thumb = item.img
                    ? '<img class="gp-suggest-thumb" src="' + escapeHtml(item.img) + '" alt="" decoding="async" />'
                    : '<span class="gp-suggest-thumb gp-thumb-fallback" aria-hidden="true">' + fallbackGlyph(item.kind) + "</span>";
                html += '<a class="gp-suggest-item ' + cls + '" role="option" aria-label="' + escapeHtml(item.type) + ": " + escapeHtml(item.title) + '" href="' + escapeHtml(item.url) + '" data-i="' + i + '">' +
                    thumb +
                    "<b>" + escapeHtml(item.title) + "</b>" +
                    '<span class="gp-suggest-badge">' + escapeHtml(item.type) + "</span>" +
                    '<span class="gp-suggest-go" aria-hidden="true">←</span>' +
                    "</a>";
            }
            html += '<div class="gp-suggest-foot"><kbd>Enter</kbd> جستجوی کامل در سایت</div>';
            panel.innerHTML = html;
            panel.classList.add("is-open");
            panel.classList.add("has-results");
            input.setAttribute("aria-expanded", "true");
            activeIndex = -1;
            itemCount = items.length;
        }

        function request(term) {
            if (controller) { try { controller.abort(); } catch (e) { } }
            if (window.AbortController) { controller = new AbortController(); }
            var options = controller ? { signal: controller.signal } : {};
            fetch("/search/suggest?q=" + encodeURIComponent(term), options)
                .then(function (response) { return response.ok ? response.json() : []; })
                .then(function (data) {
                    cache[term.toLowerCase()] = data || [];
                    if ((input.value || "").trim().toLowerCase() === term.toLowerCase()) {
                        if (data && data.length) { render(data); } else { renderEmpty(term); }
                    }
                })
                .catch(function () { /* aborted or offline: keep previous state */ });
        }

        input.addEventListener("input", function () {
            var term = (input.value || "").trim();
            if (term.length < 2) { close(); return; }
            var key = term.toLowerCase();
            if (Object.prototype.hasOwnProperty.call(cache, key)) {
                if (cache[key] && cache[key].length) { render(cache[key]); } else { renderEmpty(term); }
                return;
            }
            // instant synchronous feedback while the debounced request is pending
            panel.innerHTML = '<div class="gp-suggest-loading"><span class="gp-suggest-spinner" aria-hidden="true"></span>در حال جستجو...</div>';
            panel.classList.add("is-open");
            panel.classList.remove("has-results");
            input.setAttribute("aria-expanded", "true");
            activeIndex = -1;
            itemCount = 0;
            if (timer) { window.clearTimeout(timer); }
            timer = window.setTimeout(function () { request(term); }, 170);
        });

        input.addEventListener("focus", function () {
            if ((input.value || "").trim().length >= 2 && itemCount > 0) {
                panel.classList.add("is-open");
                input.setAttribute("aria-expanded", "true");
            }
        });

        input.addEventListener("keydown", function (event) {
            if (!panel.classList.contains("is-open") || itemCount === 0) { return; }
            var items = panel.querySelectorAll(".gp-suggest-item");
            if (!items.length) { return; }
            if (event.key === "ArrowDown" || event.key === "ArrowUp") {
                event.preventDefault();
                activeIndex = event.key === "ArrowDown"
                    ? (activeIndex + 1) % itemCount
                    : (activeIndex - 1 + itemCount) % itemCount;
                for (var i = 0; i < items.length; i++) {
                    items[i].classList.toggle("is-active", i === activeIndex);
                }
                if (items[activeIndex]) {
                    items[activeIndex].scrollIntoView({ block: "nearest" });
                }
            } else if (event.key === "Enter" && activeIndex >= 0 && items[activeIndex]) {
                event.preventDefault();
                window.location.href = items[activeIndex].getAttribute("href");
            } else if (event.key === "Escape") {
                close();
            }
        });

        document.addEventListener("click", function (event) {
            if (!box.contains(event.target)) { close(); }
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
