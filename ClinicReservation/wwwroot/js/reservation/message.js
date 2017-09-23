(function (window) {
    "use strict";
    var pages = (function () {
        var page_click = function (e) {
            var p = e.currentTarget;
            var page = parseInt(p.getAttribute("data-for"), 10);
            if (isNaN(page)) return;
            $("#actionform").find("[name='page']").val(page);
            $("#actionform").submit();
        };
        var init_page = function (min, max, current) {
            var pages = $(".page-container > .pages")[0];
            pages.innerHTML = "";
            var p;
            p = document.createElement("p");
            p.innerText = window.lang.first;
            p.setAttribute("data-for", 1);
            if (current === min)
                p.className = "disabled";
            else
                p.addEventListener("click", page_click, false);
            pages.appendChild(p);

            var cpage = current;
            var minpage = Math.max(current - 2, min);
            var maxpage = Math.min(current + 2, max);
            for (var i = minpage; i <= maxpage; i++) {
                p = document.createElement("p");
                p.innerText = i;
                p.setAttribute("data-for", i);
                if (i === current)
                    p.className = "current-page";
                else
                    p.addEventListener("click", page_click, false);
                pages.appendChild(p);
            }

            p = document.createElement("p");
            p.innerText = window.lang.last;
            p.setAttribute("data-for", max);
            if (current === max)
                p.className = "disabled";
            else
                p.addEventListener("click", page_click, false);
            pages.appendChild(p);
        };
        var load = function () {
            init_page(window.page.min, window.page.max, window.page.cur);
        };
        return {
            load: load
        };
    })();

    var siteLanguageSpecifier;
    var load = function () {
        if (!window.siteLanguageSpecifier)
            siteLanguageSpecifier = "";
        else
            siteLanguageSpecifier = window.siteLanguageSpecifier;
        $("#actionform").attr("action", siteLanguageSpecifier + $("#actionform").attr("action"));

        pages.load();
    };

    window.addEventListener("load", load, false);
})(window);