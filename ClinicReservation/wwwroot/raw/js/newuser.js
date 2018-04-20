(function (window) {
    "use strict";

    var submit_button_click = function (sender, args) {
        $("#btn_submit")[0].context.set_enabled(false);
        $("#load_ring")[0].style.opacity = 1;
        var form = args.form;
        $.debounce(1000, function () {
            form.submit();
        })();
        args.handled = true;
    };

    var load = function () {
        add_event($("#btn_submit"), "click", submit_button_click);

    };

    window.addEventListener("load", load, false);
})(window);