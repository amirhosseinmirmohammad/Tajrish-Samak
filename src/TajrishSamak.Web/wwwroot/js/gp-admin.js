/* =========================================================
   gp-admin.js — admin widgets: Persian digits, searchable select,
   jalali date picker, drag&drop uploader
   ========================================================= */
(function () {
    "use strict";

    var FA = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    function toFa(s) { return String(s).replace(/\d/g, function (d) { return FA[+d]; }); }

    /* ---------- 1) convert digits inside admin text (skip inputs/textarea) ---------- */
    function faDigitsInText(root) {
        var walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, null);
        var node;
        while ((node = walker.nextNode())) {
            var parent = node.parentElement;
            if (!parent || parent.closest("input, textarea, select, script, style")) continue;
            if (/\d/.test(node.nodeValue)) {
                node.nodeValue = toFa(node.nodeValue);
            }
        }
    }

    /* ---------- 2) searchable select ---------- */
    function makeSearchable(select) {
        if (select.getAttribute("data-gp-searchable") === "1") return;
        select.setAttribute("data-gp-searchable", "1");
        select.style.display = "none";

        var wrap = document.createElement("div");
        wrap.className = "gpc-select";
        var input = document.createElement("input");
        input.type = "text";
        input.className = "gpc-select-input";
        input.placeholder = "جستجو یا انتخاب کنید...";
        input.autocomplete = "off";
        var list = document.createElement("div");
        list.className = "gpc-select-list";
        wrap.appendChild(input);
        wrap.appendChild(list);
        select.parentNode.insertBefore(wrap, select);

        var options = [];
        Array.prototype.forEach.call(select.options, function (opt) {
            options.push({ value: opt.value, text: opt.textContent.trim(), disabled: opt.disabled });
        });

        function renderList(filter) {
            list.innerHTML = "";
            var f = (filter || "").trim().toLowerCase();
            var count = 0;
            options.forEach(function (opt) {
                if (f && opt.text.toLowerCase().indexOf(f) === -1 && opt.value.toLowerCase().indexOf(f) === -1) return;
                var item = document.createElement("div");
                item.className = "gpc-select-item" + (opt.value === select.value ? " is-selected" : "");
                item.textContent = opt.text;
                item.addEventListener("mousedown", function (e) {
                    e.preventDefault();
                    select.value = opt.value;
                    input.value = opt.text;
                    list.classList.remove("is-open");
                });
                list.appendChild(item);
                count++;
            });
            if (count === 0) {
                var empty = document.createElement("div");
                empty.className = "gpc-select-item gpc-select-empty";
                empty.textContent = "موردی یافت نشد";
                list.appendChild(empty);
            }
        }

        input.addEventListener("focus", function () { renderList(input.value); list.classList.add("is-open"); });
        input.addEventListener("input", function () { renderList(input.value); list.classList.add("is-open"); });
        input.addEventListener("blur", function () {
            setTimeout(function () { list.classList.remove("is-open"); }, 150);
            // restore display text for current value
            var current = options.find(function (o) { return o.value === select.value; });
            input.value = current ? current.text : input.value;
        });
        renderList("");
        if (select.value) {
            var cur = options.find(function (o) { return o.value === select.value; });
            if (cur) input.value = cur.text;
        }
    }

    /* ---------- 3) jalali date picker ---------- */
    var JALALI_BREAKS = [-61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178];

    function jalCal(jy) {
        var bl = JALALI_BREAKS.length, gy = jy + 621, leapJ = -14, jp = JALALI_BREAKS[0], jm, jump, leap, n, i;
        for (i = 1; i < bl; i += 1) {
            jm = JALALI_BREAKS[i];
            jump = jm - jp;
            if (jy < jm) break;
            leapJ = leapJ + div(jump, 33) * 8 + div(mod(jump, 33), 4);
            jp = jm;
        }
        n = jy - jp;
        leapJ = leapJ + div(n, 33) * 8 + div(mod(n, 33) + 3, 4);
        if (mod(jump, 33) === 4 && jump - n === 4) leapJ += 1;
        var leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150;
        var march = 20 + leapG - leapJ;
        if (jump - n < 6) n = n - jump + div(jump + 4, 33) * 33;
        leap = mod(mod(n + 1, 33) - 1, 4);
        if (leap === -1) leap = 4;
        return { leap: leap, gy: gy, march: march };
    }
    function div(a, b) { return ~~(a / b); }
    function mod(a, b) { return a - ~~(a / b) * b; }
    function g2d(gy, gm, gd) {
        var d = div((gy + div(gm - 8, 6) + 100100) * 1461, 4) + div(153 * mod(gm + 9, 12) + 2, 5) + gd - 34840408;
        d = d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752;
        return d;
    }
    function d2g(jdn) {
        var j = 4 * jdn + 139361631;
        j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
        var i = div(mod(j, 1461), 4) * 5 + 308;
        var gd = div(mod(i, 153), 5) + 1;
        var gm = mod(div(i, 153), 12) + 1;
        var gy = div(j, 1461) - 100100 + div(8 - gm, 6);
        return { gy: gy, gm: gm, gd: gd };
    }
    function j2d(jy, jm, jd) {
        var r = jalCal(jy);
        return g2d(r.gy, 3, r.march) + (jm - 1) * 31 - div(jm, 7) * (jm - 7) + jd - 1;
    }
    function d2j(jdn) {
        var gy = d2g(jdn).gy, jy = gy - 621, r = jalCal(jy), jdn1f = g2d(gy, 3, r.march), jd, jm, k;
        k = jdn - jdn1f;
        if (k >= 0) {
            if (k <= 185) { jm = 1 + div(k, 31); jd = mod(k, 31) + 1; return { jy: jy, jm: jm, jd: jd }; }
            k = k - 186;
        } else {
            jy -= 1;
            k = k + 179;
            if (r.leap === 1) k = k + 1;
        }
        jm = 7 + div(k, 30);
        jd = mod(k, 30) + 1;
        return { jy: jy, jm: jm, jd: jd };
    }
    function toJalaali(date) {
        return d2j(g2d(date.getFullYear(), date.getMonth() + 1, date.getDate()));
    }
    function toGregorian(jy, jm, jd) {
        return d2g(j2d(jy, jm, jd));
    }
    function jalaliMonthLength(jy, jm) {
        if (jm <= 6) return 31;
        if (jm <= 11) return 30;
        var r = jalCal(jy);
        return r.leap === 1 ? 30 : 29;
    }

    var WEEKDAYS = ["ش", "ی", "د", "س", "چ", "پ", "ج"];
    var MONTHS = ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"];

    function attachJalali(input) {
        if (input.getAttribute("data-gp-jalali") === "1") return;
        input.setAttribute("data-gp-jalali", "1");
        input.setAttribute("readonly", "readonly");
        input.classList.add("gp-jalali-input");

        var hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = input.name || "";
        input.removeAttribute("name");
        input.parentNode.insertBefore(hidden, input);

        var state = input.value
            ? parseIso(input.value)
            : toJalaali(new Date());

        function parseIso(iso) {
            var p = iso.split("-");
            if (p.length === 3) {
                var g = new Date(+p[0], +p[1] - 1, +p[2]);
                return toJalaali(g);
            }
            return toJalaali(new Date());
        }

        function writeBack(jy, jm, jd) {
            var g = toGregorian(jy, jm, jd);
            hidden.value = g.gy + "-" + ("0" + g.gm).slice(-2) + "-" + ("0" + g.gd).slice(-2);
            input.value = toFa(jy) + "/" + toFa(("0" + jm).slice(-2)) + "/" + toFa(("0" + jd).slice(-2));
        }
        writeBack(state.jy, state.jm, state.jd);

        var popup = null;

        function render() {
            var first = toGregorian(state.jy, state.jm, 1);
            var firstDow = (new Date(first.gy, first.gm - 1, first.gd).getDay() + 1) % 7;
            var len = jalaliMonthLength(state.jy, state.jm);
            var todayJ = toJalaali(new Date());
            var selected = state.jd;

            var html = '<div class="gp-jal-head"><button type="button" class="gp-jal-nav" data-nav="-1">›</button><b>' + MONTHS[state.jm - 1] + " " + toFa(state.jy) + '</b><button type="button" class="gp-jal-nav" data-nav="1">‹</button></div>';
            html += '<div class="gp-jal-week">';
            for (var w = 0; w < 7; w++) html += '<span>' + WEEKDAYS[w] + "</span>";
            html += "</div><div class='gp-jal-days'>";
            for (var e = 0; e < firstDow; e++) html += "<span></span>";
            for (var d = 1; d <= len; d++) {
                var isToday = todayJ.jy === state.jy && todayJ.jm === state.jm && d === todayJ.jd;
                var isSel = d === selected;
                html += '<button type="button" data-day="' + d + '" class="' + (isSel ? "is-sel " : "") + (isToday ? "is-today" : "") + '">' + toFa(d) + "</button>";
            }
            html += "</div>";

            popup.innerHTML = html;
            popup.querySelectorAll("[data-day]").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    state.jd = +btn.getAttribute("data-day");
                    writeBack(state.jy, state.jm, state.jd);
                    closePopup();
                });
            });
            popup.querySelectorAll(".gp-jal-nav").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    var delta = +btn.getAttribute("data-nav");
                    state.jm += delta;
                    if (state.jm > 12) { state.jm = 1; state.jy += 1; }
                    if (state.jm < 1) { state.jm = 12; state.jy -= 1; }
                    var maxD = jalaliMonthLength(state.jy, state.jm);
                    if (state.jd > maxD) state.jd = maxD;
                    render();
                });
            });
        }

        function openPopup() {
            if (!popup) {
                popup = document.createElement("div");
                popup.className = "gp-jal-popup";
                document.body.appendChild(popup);
                document.addEventListener("click", function (event) {
                    if (!popup.contains(event.target) && event.target !== input) { closePopup(); }
                });
            }
            var r = input.getBoundingClientRect();
            popup.style.top = r.bottom + window.scrollY + 6 + "px";
            popup.style.right = window.innerWidth - r.right + "px";
            popup.classList.add("is-open");
            render();
        }
        function closePopup() { if (popup) popup.classList.remove("is-open"); }

        input.addEventListener("click", function () {
            if (popup && popup.classList.contains("is-open")) { closePopup(); } else { openPopup(); }
        });
        input.addEventListener("keydown", function (e) { if (e.key === "Escape") closePopup(); });
    }

    /* ---------- 4) drag & drop uploader ---------- */
    function attachUploader(zone) {
        if (zone.getAttribute("data-gp-uploader") === "1") return;
        zone.setAttribute("data-gp-uploader", "1");
        var input = zone.querySelector("input[type=text]");
        var folder = zone.getAttribute("data-folder") || "uploads";
        var fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.accept = "image/*";
        fileInput.style.display = "none";
        zone.appendChild(fileInput);

        var preview = zone.querySelector(".gpc-upload-preview img") || null;

        function uploadFile(file) {
            if (!file || file.type.indexOf("image") !== 0) return;
            var data = new FormData();
            data.append("file", file);
            data.append("folder", folder);
            zone.classList.add("is-uploading");
            fetch("/admin/upload", { method: "POST", body: data })
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    zone.classList.remove("is-uploading");
                    if (data.ok && data.url) {
                        if (input) { input.value = data.url; input.dispatchEvent(new Event("change")); }
                        if (preview) { preview.src = data.url; }
                        zone.classList.add("has-image");
                    }
                })
                .catch(function () { zone.classList.remove("is-uploading"); });
        }

        zone.addEventListener("click", function (e) {
            if (e.target.closest("input")) return;
            fileInput.click();
        });
        fileInput.addEventListener("change", function () { if (fileInput.files[0]) uploadFile(fileInput.files[0]); });

        var pick = document.createElement("button");
        pick.type = "button";
        pick.className = "gpc-upload-pick";
        pick.textContent = "انتخاب فایل";
        pick.addEventListener("click", function (e) {
            e.stopPropagation();
            fileInput.click();
        });
        zone.appendChild(pick);

        ["dragover", "dragenter"].forEach(function (ev) {
            zone.addEventListener(ev, function (e) { e.preventDefault(); zone.classList.add("is-drag"); });
        });
        ["dragleave", "drop"].forEach(function (ev) {
            zone.addEventListener(ev, function (e) { e.preventDefault(); zone.classList.remove("is-drag"); });
        });
        zone.addEventListener("drop", function (e) {
            if (e.dataTransfer.files && e.dataTransfer.files[0]) uploadFile(e.dataTransfer.files[0]);
        });
    }

    /* ---------- 5) tag chips editor (Enter یا ویرگول = چیپ جدید) ---------- */
    function makeTags(container) {
        if (container.getAttribute("data-gp-tags") === "1") return;
        container.setAttribute("data-gp-tags", "1");
        var name = container.getAttribute("data-name") || "tags";
        var initial = container.getAttribute("data-values") || "";
        var hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = name;
        container.appendChild(hidden);

        var chipsBox = document.createElement("div");
        chipsBox.className = "gp-tags";
        var input = document.createElement("input");
        input.type = "text";
        input.className = "gp-tags-input";
        input.placeholder = "تایپ کنید، Enter یا , = افزودن";
        chipsBox.appendChild(input);
        container.appendChild(chipsBox);

        var values = initial.split(/[،,]/).map(function (s) { return s.trim(); }).filter(function (s) { return s.length > 0; });

        function sync() { hidden.value = values.join("،"); }
        function render() {
            chipsBox.querySelectorAll(".gp-tag").forEach(function (t) { t.remove(); });
            values.forEach(function (v, index) {
                var chip = document.createElement("span");
                chip.className = "gp-tag";
                chip.textContent = v;
                var x = document.createElement("button");
                x.type = "button";
                x.className = "gp-tag-x";
                x.textContent = "×";
                x.addEventListener("click", function () {
                    values.splice(index, 1);
                    sync();
                    render();
                });
                chip.appendChild(x);
                chipsBox.insertBefore(chip, input);
            });
            sync();
        }
        input.addEventListener("keydown", function (e) {
            if (e.key === "Enter" || e.key === "," || e.key === "،") {
                e.preventDefault();
                var v = input.value.replace(/[،,]/g, "").trim();
                if (v && values.indexOf(v) === -1) { values.push(v); sync(); render(); }
                input.value = "";
            } else if (e.key === "Backspace" && input.value === "" && values.length > 0) {
                values.pop();
                sync();
                render();
            }
        });
        render();
    }

    /* ---------- boot ---------- */
    function init() {
        var adminContent = document.querySelector(".adm-main") || document.body;
        faDigitsInText(adminContent);

        document.querySelectorAll("select.gp-searchable").forEach(makeSearchable);
        document.querySelectorAll("input.gp-jalali").forEach(attachJalali);
        document.querySelectorAll(".gpc-upload-zone").forEach(attachUploader);
        document.querySelectorAll(".gp-tags-editor").forEach(makeTags);

        // convert after reveal animations too (late content)
        var mo = new MutationObserver(function () { faDigitsInText(adminContent); });
        mo.observe(adminContent, { childList: true, subtree: true });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
