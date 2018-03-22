(function (window) {
    "use strict";

    var create_click = function () {
        //set_location("/create");
    };
    var login_click = function () {
        //set_location("/login");
    };
    var board_click = function () {
        //set_location("/board");
    };

    var load = function () {

        add_event($("#btn_login"), "click", login_click);

        add_event($("#btn_create"), "click", create_click);

        add_event($("#btn_board"), "click", board_click);
    };
    window.addEventListener("load", load, false);

})(window);