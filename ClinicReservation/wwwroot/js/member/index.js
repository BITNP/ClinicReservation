(function () {
    "use strict";

    var login_click = function () {
        $("#input_hiddenpwd").val($.md5($("#input_password")[0].context.get_value()));
        $("#form_memberlogin").submit();
    };
    var apply_click = function () {
        window.location.href = $("#btn_apply").attr("data-href");
    };

    var siteLanguageSpecifier;
    var load = function () {
        if (!window.siteLanguageSpecifier)
            siteLanguageSpecifier = "";
        else
            siteLanguageSpecifier = window.siteLanguageSpecifier;
        $("#btn_apply").attr("data-href", siteLanguageSpecifier + $("#btn_apply").attr("data-href"));

        $("#btn_login")[0].context.events.add("click", login_click);
        $("#btn_apply")[0].context.events.add("click", apply_click);
    };

    window.addEventListener("load", load, false);
})();