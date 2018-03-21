"use strict";

(function (window) {
    "use strict";

    window.addEventListener("load", function () {
        $(".nppage-header > div.right > div.floatmenu").focus(function () {
            var floatmenu = this;
            $(floatmenu).addClass("expand");
            var menu = $(floatmenu).find(".menu");
            if (menu.length > 0) {
                menu = menu[0];
                menu.style.height = menu.children[0].offsetHeight + "px";
            }
        });
        $(".nppage-header > div.right > div.floatmenu").blur(function () {
            var floatmenu = this;
            var menu = $(floatmenu).find(".menu");
            if (menu.length > 0) {
                menu = menu[0];
                menu.style.height = 0 + "px";
            }
            window.setTimeout(function () {
                $(floatmenu).removeClass("expand");
            }, 200);
        });
    }, false);
})(window);

