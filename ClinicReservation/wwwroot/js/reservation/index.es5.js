"use strict";

(function (window) {
    "use strict";

    var query_click = function query_click() {
        $("#form_queryreservation").submit();
    };
    var create_click = function create_click() {
        window.location.href = "/reservation/create";
    };
    var load = function load() {
        $("#btn_query")[0].context.events.add("click", query_click);
        $("#btn_create")[0].context.events.add("click", create_click);
    };
    window.addEventListener("load", load, false);
})(window);

