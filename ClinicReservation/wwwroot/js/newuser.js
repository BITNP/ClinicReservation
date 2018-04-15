﻿"use strict";

(function (window) {
    "use strict";

    var clear_error = function clear_error() {
        this.parentElement.context.clear_error();
    };
    var is_valid_format = function is_valid_format() {
        var name = $("#input_name")[0].context.get_value().trim();
        var phone = $("#input_phone")[0].context.get_value().trim();
        var email = $("#input_email")[0].context.get_value().trim();
        var im = $("#input_im")[0].context.get_value().trim();
        var github = $("#input_github")[0].context.get_value().trim();

        var phone_reg = /(\+\d+)?\s*1\d{10}/;
        var email_reg = /^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
        var github_reg = /^https?:\/\/github.com\/.+/;
        var succ = true;
        if (name.length > 0) {
            $("#input_name")[0].context.set_value(name);
        } else {
            $("#input_name")[0].context.set_error(window.message.name.error);
            succ = false;
        }

        if (phone.length <= 0 || phone_reg.exec(phone)) {
            $("#input_phone")[0].context.set_value(phone);
        } else {
            $("#input_phone")[0].context.set_error(window.message.phone.error);
            succ = false;
        }

        if (email.length <= 0 || email_reg.exec(email)) {
            $("#input_email")[0].context.set_value(email);
        } else {
            $("#input_email")[0].context.set_error(window.message.email.error);
            succ = false;
        }

        $("#input_im")[0].context.set_value(im);

        if (github.length <= 0 || github_reg.exec(github)) {
            $("#input_github")[0].context.set_value(github);
        } else {
            $("#input_github")[0].context.set_error(window.message.github.error);
            succ = false;
        }

        $("#btn_submit")[0].context.set_enabled(succ);
        return succ;
    };

    var submit_button_click = function submit_button_click(sender, args) {
        if (!is_valid_format()) {
            args.handled = true;
            return;
        }

        $("#btn_submit")[0].context.set_enabled(false);
        $("#load_ring")[0].style.opacity = 1;
        var form = args.form;
        $.debounce(1000, function () {
            form.submit();
        })();
        args.handled = true;
    };

    var check_delay = 1000;
    var load = function load() {
        $("#input_name")[0].context.set_hint("focus", "", window.message.name.hint);
        $("#input_phone")[0].context.set_hint("focus", "", window.message.phone.hint);
        $("#input_email")[0].context.set_hint("focus", "", window.message.email.hint);
        $("#input_im")[0].context.set_hint("focus", "", window.message.im.hint);
        $("#input_github")[0].context.set_hint("focus", "", window.message.github.hint);
        $("#input_secret_code")[0].context.set_hint("focus", "", window.message.code.hint);

        add_event($("#btn_submit"), "click", submit_button_click);

        add_event($("#input_name"), "blur", is_valid_format);
        add_event($("#input_phone"), "blur", is_valid_format);
        add_event($("#input_email"), "blur", is_valid_format);
        add_event($("#input_im"), "blur", is_valid_format);
        add_event($("#input_github"), "blur", is_valid_format);

        $($("#input_name")[0].context.input).keydown(clear_error);
        $($("#input_phone")[0].context.input).keydown(clear_error);
        $($("#input_email")[0].context.input).keydown(clear_error);
        $($("#input_im")[0].context.input).keydown(clear_error);
        $($("#input_github")[0].context.input).keydown(clear_error);

        $($("#input_name")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
        $($("#input_phone")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
        $($("#input_email")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
        $($("#input_im")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
        $($("#input_github")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
    };

    window.addEventListener("load", load, false);
})(window);

