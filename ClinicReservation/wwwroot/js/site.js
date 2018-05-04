"use strict";

var add_event = function add_event(elements, event, handler) {
    var i = 0;
    var len = elements.length;
    for (i; i < len; i++) {
        elements[i].context.events.add(event, handler);
    }
};

var set_location = function set_location(url) {
    var culture = window.culture;
    if (!culture) window.culture = culture = "";
    window.location = culture + url;
};

(function (window) {
    var langreg = /^[a-zA-Z]{2}(\-(\*|([a-zA-Z]{2})))?(\/|$)/;

    var startsWidthCulture = function startsWidthCulture(url) {
        url = url.substr(1);
        if (langreg.exec(url) !== null && langreg.exec(url) !== undefined) return true;
        return false;
    };
    var load = function load() {
        var culture = window.culture;
        if (!culture) window.culture = culture = "";

        if (culture.length > 0) {
            $("a").each(function () {
                var href = this.href;
                if (href === undefined || href === null || href === "undefined" || href === "null") return;
                if (href.startsWith(window.location.origin)) {
                    href = href.substr(window.location.origin.length);
                }
                if (href.startsWith('/') && !startsWidthCulture(href)) {
                    href = culture + href;
                    this.href = href;
                }
            });
            $("form").each(function () {
                var action = this.action;
                if (action === undefined || action === null || action === "undefined" || action === "null") return;
                if (action.startsWith('/') && !startsWidthCulture(action)) {
                    action = culture + action;
                    this.action = action;
                }
            });
            $("div[data-control='button']").each(function () {
                var href = this.context.get_href();
                if (href === undefined || href === null || href === "undefined" || href === "null") return;
                if (href.startsWith('/') && !startsWidthCulture(href)) {
                    href = culture + href;
                    this.context.set_href(href);
                }
            });
        }
    };

    window.addEventListener("load", load, false);
})(window);

