(function (window) {
    "use strict";

    var siteLanguageSpecifier;
    var query_click = function () {
        $("#form_queryreservation").submit();
    };
    var create_click = function () {
        window.location.href = siteLanguageSpecifier + "/reservation/create";
    };
    var load = function () {
        if (!window.siteLanguageSpecifier)
            siteLanguageSpecifier = "";
        else
            siteLanguageSpecifier = window.siteLanguageSpecifier;

        $("#form_queryreservation").attr("action", siteLanguageSpecifier + $("#form_queryreservation").attr("action"));

        $("#btn_query")[0].context.events.add("click", query_click);
        $("#btn_create")[0].context.events.add("click", create_click);
    };
    window.addEventListener("load", load, false);
})(window);