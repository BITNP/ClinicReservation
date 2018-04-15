"use strict";

(function (window) {
    "use strict";

    var clear_error = function clear_error() {
        this.parentElement.context.clear_error();
    };
    var is_valid_format = function is_valid_format() {
        var detail = $("#input_detail")[0].context.get_value().trim();
        var captcha = $("#input_captcha")[0].context.get_value().trim();
        var captcha_reg = /^\d+$/;

        var succ = true;
        if (detail.length > 0) {
            $("#input_detail")[0].context.set_value(detail);
            if (detail.length < 5) {
                $("#input_detail")[0].context.set_error(window.message.detail.error);
                succ = false;
            }
        }

        if (captcha.length <= 0) {
            $("#input_captcha")[0].context.set_error(window.message.captcha_empty.error);
            succ = false;
        } else if (captcha_reg.exec(captcha)) {
            $("#input_captcha")[0].context.set_value(captcha);
        } else {
            $("#input_captcha")[0].context.set_error(window.message.captcha.error);
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
        var form = $(args.form);
        $.debounce(1000, function () {
            if (form.find("[name='captchaToken']").length > 0) {
                form.find("[name='captchaText']")[0].value = $("#DNTCaptchaText")[0].value;
                form.find("[name='captchaToken']")[0].value = $("#DNTCaptchaToken")[0].value;
            }
            form.submit();
        })();
        args.handled = true;
    };

    var lastindex = -1;
    var datepickclick = function datepickclick(e) {
        var pitem = e.currentTarget;
        var parent = null;
        if (pitem === null) {
            parent = e.parentElement;
        } else {
            parent = pitem.parentElement;
        }
        if (lastindex >= 0) {
            $(parent.children[lastindex]).removeClass("selected");
        }
        lastindex = -1;
        if (pitem !== null) {
            $(pitem).addClass("selected");
            for (var i = 0; i < parent.children.length; i++) if (parent.children[i] === pitem) {
                lastindex = i;
            }
            if (e.change === undefined) {
                var date = new Date();
                var milltime = date.getTime() + lastindex * 24 * 60 * 60 * 1000;
                var resultdate = new Date(milltime);
                $("#input_bookdate")[0].context.set_date(resultdate);
            }
        }
    };
    var datechanged = function datechanged(sender, args) {
        var val = args.date.year + "/" + args.date.month + "/" + args.date.day;
        var tdate = new Date(val);
        setinputdate(tdate);
    };
    var setinputdate = function setinputdate(date) {
        var cdate = new Date();
        var diff = parseInt(Math.ceil((date - cdate) / 24.0 / 60 / 60 / 1000));

        var pparent = $(".date-quick-pick > p")[0].parentElement;
        if (diff < 0) {
            datepickclick({
                currentTarget: null,
                parentElement: pparent,
                change: false
            });
        } else if (diff < pparent.children.length) {
            datepickclick({
                currentTarget: $(".date-quick-pick > p")[0].parentElement.children[diff],
                change: false
            });
        } else {
            datepickclick({
                currentTarget: null,
                parentElement: pparent,
                change: false
            });
        }
    };

    var check_delay = 1000;
    var load = function load() {
        $("#input_detail")[0].context.set_value($("#hidden_problemdetail").val());
        $("#input_detail")[0].context.set_hint("focus", "", window.message.detail.hint);
        if (window.detail_error) {
            $("#input_detail")[0].context.set_error(window.detail_error);
        }

        $(".date-quick-pick > p").click(datepickclick);
        $("#input_bookdate")[0].context.events.add("changed", datechanged);
        var picks = $(".date-quick-pick > p");
        for (var i = 0; i < picks.length; i++) {
            if ($(picks[i]).hasClass("selected")) {
                lastindex = i;
                break;
            }
        }

        add_event($("#btn_submit"), "click", submit_button_click);

        add_event($("#input_detail"), "blur", is_valid_format);
        add_event($("#input_captcha"), "blur", is_valid_format);

        $($("#input_detail")[0].context.input).keydown(clear_error);
        $($("#input_captcha")[0].context.input).keydown(clear_error);

        $($("#input_detail")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
        $($("#input_captcha")[0].context.input).keyup($.debounce(check_delay, is_valid_format));
    };

    window.addEventListener("load", load, false);
})(window);

