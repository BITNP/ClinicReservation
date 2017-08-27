(function () {
    "use strict";

    var login_click = function () {
        $("#input_hiddenpwd").val($.md5($("#input_password")[0].context.get_value()));
        $("#form_memberlogin").submit();
    };
    var apply_click = function () {
        window.location.href = "/member/application";
    };

    var load = function () {
        $("#btn_login")[0].context.events.add("click", login_click);
        $("#btn_apply")[0].context.events.add("click", apply_click);
    };

    window.addEventListener("load", load, false);
})();