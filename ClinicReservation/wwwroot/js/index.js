"use strict";

(function (window) {
    "use strict";

    var create_click = function create_click() {
        //set_location("/create");
    };
    var login_click = function login_click() {
        //set_location("/login");
    };
    var board_click = function board_click() {
        //set_location("/board");
    };

    var load = function load() {

        add_event($("#btn_login"), "click", login_click);

        add_event($("#btn_create"), "click", create_click);

        add_event($("#btn_board"), "click", board_click);
    };
    window.addEventListener("load", load, false);
})(window);

