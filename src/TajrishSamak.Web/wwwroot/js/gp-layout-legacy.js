/* Legacy layout scripts (loader, language toggle, service worker, newsletter) - ported from _Layout.cshtml */

        $("#btntext").click(function (e) {
            e.preventDefault()
            $.ajax({
                url: "@Url.Action("NewsLetter","Home")",
                contentType: "application/json; charset=utf-8",
                data: { text: $("#textnews").val() },
                dataType: "json",
                type: "GET",
                success: function (Data) {
                    if (Data.status == "1") {
                        $("#msg").text(Data.text)
                        $("#msg").css("color", "#4b1684");
                        $("#msg").css("font-size", "larger");
                        $("#msg").parent("h6").css("padding-top", "20px");
                        $("#msg").parent("h6").css("text-align", "center");
                        $("#msg").parent("h6").css("direction", "rtl");
                    }
                    if (Data.status == "0") {
                        $("#msg").text(Data.text)
                        $("#msg").css("color", "rgb(255, 0, 0)");
                        $("#msg").css("font-size", "larger");
                        $("#msg").parent("h6").css("padding-top", "20px");
                        $("#msg").parent("h6").css("text-align", "center");
                        $("#msg").parent("h6").css("direction", "rtl");
                    }
                },
                error: function (jqhxr, statusText, error) { alert("خطایی رخ داده است لطفا دوباره امتحان کنید ."); }
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
    