"use strict";

(function (window) {
    "use strict";

    window.addEventListener("load", function () {
        $(".nppage-header > div.right > div.float-site-navi").click(function () {
            var floatmenu = this;
            var $menu = $(this);
            $menu.focus();
            if ($menu.hasClass("expand")) {
                $menu.blur();
                return;
            }
            $(floatmenu).addClass("expand");
            var menu = $(floatmenu).find(".menu");
            if (menu.length > 0) {
                menu = menu[0];
                menu.style.height = menu.children[0].offsetHeight + "px";
            }
        });
        $(".nppage-header > div.right > div.float-site-navi").blur(function () {
            var floatmenu = this;
            var source = event.relatedTarget;
            if (source !== undefined && source !== null && source.tagName.toLowerCase() === "a") {
                var parent = source.parentElement;
                while (parent !== undefined && parent !== null) {
                    if (parent === floatmenu) {
                        return;
                    }
                    parent = parent.parentElement;
                }
            }
            var menu = $(floatmenu).find(".menu");
            if (menu.length > 0) {
                menu = menu[0];
                menu.style.height = 0 + "px";
            }
            window.setTimeout(function () {
                $(floatmenu).removeClass("expand");
            }, 200);
        });

        $(".nppage-header > div.right > div.float-user-control").click(function () {
            var floatmenu = this;
            var $menu = $(this);
            $menu.focus();
            if ($menu.hasClass("expand")) {
                $menu.blur();
                return;
            }
            $(floatmenu).addClass("expand");
            var menu = $(floatmenu).find(".menu");
            if (menu.length > 0) {
                menu = menu[0];
                var height = 0;
                for (var i = 0; i < menu.children.length; i++) {
                    height += menu.children[i].offsetHeight;
                }
                menu.style.height = height + "px";
            }
        });
        $(".nppage-header > div.right > div.float-user-control").blur(function () {
            var floatmenu = this;
            var source = event.relatedTarget;
            if (source !== undefined && source !== null && source.tagName.toLowerCase() === "a") {
                var parent = source.parentElement;
                while (parent !== undefined && parent !== null) {
                    if (parent === floatmenu) {
                        return;
                    }
                    parent = parent.parentElement;
                }
            }
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

