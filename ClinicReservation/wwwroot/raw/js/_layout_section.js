(function (window) {
    "use strict";

    var THROTTLE_TIMEOUT = 60;
    var section_container;
    var section_holder;
    var $section;
    var itemcnt;

    var wnd_resize = function wnd_resize() {
        section_holder.style.width = "auto";
        var ceilwidth = $section.outerWidth(true);
        var parentwidth = section_container.offsetWidth;
        var ceilcnt = Math.max(Math.floor(parentwidth / ceilwidth), 1);
        if (ceilcnt > itemcnt) ceilcnt = itemcnt;
        section_holder.style.width = ceilcnt * ceilwidth + "px";
    };

    var load = function load() {
        section_container = $(".section-container")[0];
        section_holder = section_container.firstElementChild;
        itemcnt = section_holder.children.length;
        if (itemcnt > 0) {
            $section = $(section_holder.firstElementChild);
            window.addEventListener("resize", $.throttle(THROTTLE_TIMEOUT, wnd_resize), false);
            wnd_resize();
        }
    };
    window.addEventListener("load", load, false);
})(window);