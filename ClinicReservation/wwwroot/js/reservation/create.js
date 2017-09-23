(function () {
    "use strict";

    //TODO: 完善cookies中保存个人信息逻辑 懒得写了
    var save_personaldata = function () {
        var data = "";
        var name = $("#input_name")[0].context.get_value();
        var phone = $("#input_phonenumber")[0].context.get_value();
        var mail = $("#input_email")[0].context.get_value();
        var qq = $("#input_qqnumber")[0].context.get_value();
        var school = $("#input_posterschool")[0].context.selected_index();
        data = escape(name) + "|xx|" + escape(phone) + "|xx|" + escape(mail) + "|xx|" + escape(qq) + "|xx|" + school;

        var Days = 30;
        var exp = new Date();
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = "pdata=" + data + ";expires=" + exp.toGMTString();
    };
    var clear_personaldata = function () {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        document.cookie = "pdata=val;expires=" + exp.toGMTString();
    };
    var read_personaldata = function () {
        var arr, reg = new RegExp("(^| )" + "pdata" + "=([^;]*)(;|$)");
        var chs = $("#checkbox_savepdata");
        if ((arr = document.cookie.match(reg))) {
            var data = arr[2].split("|xx|");
            if (chs.length > 0)
                $("#checkbox_savepdata")[0].context.set_checked(true);
            $("#input_name")[0].context.set_value(unescape(data[0]));
            $("#input_phonenumber")[0].context.set_value(unescape(data[1]));
            $("#input_email")[0].context.set_value(unescape(data[2]));
            $("#input_qqnumber")[0].context.set_value(unescape(data[3]));
            $("#input_posterschool")[0].context.set_selected(parseInt(data[4], 10));
        }
        else {
            if (chs.length > 0)
                chs[0].context.set_checked(false);
        }
    };

    var check_all = function () {
        var detail = $("#input_detail")[0].context.get_value();
        var enablesubmit = true;
        if (detail === undefined || detail.length <= 5) {
            enablesubmit = false;
        }

        if (check_email(1) === false) {
            enablesubmit = false;
        }
        if (check_phonenumber(1) === false) {
            enablesubmit = false;
        }

        if (enablesubmit)
            $("#btn_submit")[0].context.set_enabled(true);
        else
            $("#btn_submit")[0].context.set_enabled(false);
    };

    var copy_values = function () {
        var form = $("#hidden_submiter");
        form.find("[name='postername']")[0].value = $("#input_name")[0].context.get_value();
        form.find("[name='posterphone']")[0].value = $("#input_phonenumber")[0].context.get_value();
        form.find("[name='posteremail']")[0].value = $("#input_email")[0].context.get_value();
        form.find("[name='posterqq']")[0].value = $("#input_qqnumber")[0].context.get_value();
        form.find("[name='posterschool']")[0].value = $("#input_posterschool")[0].context.selected_item().innerText;
        form.find("[name='problemtype']")[0].value = $("#input_questiontype")[0].context.selected_item().innerText;
        form.find("[name='problemdetail']")[0].value = $("#input_detail")[0].context.get_value();
        form.find("[name='location']")[0].value = $("#input_location")[0].context.selected_item().innerText;
        if (form.find("[name='captchaToken']").length > 0) {
            form.find("[name='captchaToken']")[0].value = $("#DNTCaptchaText")[0].value;
            form.find("[name='captchaText']")[0].value = $("#input_captcha")[0].context.get_value();
        }
        var date = $("#input_bookdate")[0].context.get_date();
        var date_str = date.year + "/" + date.month + "/" + date.day;
        form.find("[name='bookdate']")[0].value = date_str;
    };

    var check_delay = 1500;
    var focus_name = function () {
        $("#input_name_hint").addClass("show");
    };
    var blur_name = function () {
        $("#input_name_hint").removeClass("show");
    };
    var focus_phonenumber = function () {
        $("#input_phone_hint").addClass("show");
    };
    var blur_phonenumber = function () {
        $("#input_phone_hint").removeClass("show");
        check_phonenumber();
    };
    var check_phonenumber = function () {
        var reg = /^\d+$/gi;
        var text = $("#input_phonenumber")[0].context.get_value();
        if (text === null || text === undefined || text.length === 0) {
            $("#input_phonenumber")[0].removeAttribute("data-error");
            $("#input_phone_err").removeClass("show");
            return true;
        }
        else if (reg.test(text)) {
            $("#input_phonenumber")[0].removeAttribute("data-error");
            $("#input_phone_err").removeClass("show");
            return true;
        }
        else {
            $("#input_phonenumber")[0].setAttribute("data-error", true);
            $("#input_phone_err").addClass("show");
            return false;
        }
    };
    var focus_email = function () {
        $("#input_mail_hint").addClass("show");
        $("#input_email")[0].removeAttribute("data-error");
        $("#input_mail_err").removeClass("show");
        $.debounce(check_delay, check_email)();
    };
    var blur_email = function () {
        $("#input_mail_hint").removeClass("show");
        check_email();
    };
    var check_email = function () {
        var reg = /^([a-zA-Z0-9]+[_|\-|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\-|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/gi;
        var text = $("#input_email")[0].context.get_value();
        if (text === null || text === undefined || text.length === 0) {
            $("#input_email")[0].removeAttribute("data-error");
            $("#input_mail_err").removeClass("show");
            return true;
        }
        else if (reg.test(text)) {
            $("#input_email")[0].removeAttribute("data-error");
            $("#input_mail_err").removeClass("show");
            return true;
        }
        else {
            $("#input_email")[0].setAttribute("data-error", true);
            $("#input_mail_err").addClass("show");
            return false;
        }
    };
    var focus_qq = function () {
        $("#input_qq_hint").addClass("show");
    };
    var blur_qq = function () {
        $("#input_qq_hint").removeClass("show");
    };

    var lastindex = -1;
    var datepickclick = function (e) {
        var pitem = e.currentTarget;
        var parent = null;
        if (pitem === null) {
            parent = e.parentElement;
        }
        else {
            parent = pitem.parentElement;
        }
        if (lastindex >= 0) {
            $(parent.children[lastindex]).removeClass("selected");
        }
        lastindex = -1;
        if (pitem !== null) {
            $(pitem).addClass("selected");
            for (var i = 0; i < parent.children.length; i++)
                if (parent.children[i] === pitem) {
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
    var datechanged = function (sender, args) {
        var val = args.date.year + "/" + args.date.month + "/" + args.date.day;
        var tdate = new Date(val);
        setinputdate(tdate);
    };
    var setinputdate = function (date) {
        var cdate = new Date();
        var diff = parseInt(Math.ceil((date - cdate) / 24.0 / 60 / 60 / 1000));

        var pparent = $(".date-quick-pick > p")[0].parentElement;
        if (diff < 0) {
            datepickclick({
                currentTarget: null,
                parentElement: pparent,
                change: false
            });
        }
        else if (diff < pparent.children.length) {
            datepickclick({
                currentTarget: $(".date-quick-pick > p")[0].parentElement.children[diff],
                change: false
            });
        }
        else {
            datepickclick({
                currentTarget: null,
                parentElement: pparent,
                change: false
            });
        }
    };

    var submit_click = function (sender) {
        sender.context.set_enabled(false);
        $("#load_ring")[0].style.opacity = 1;

        var save = false;
        if ($("#checkbox_savepdata")[0])
            save = $("#checkbox_savepdata")[0].context.is_checked();
        if (save)
            save_personaldata();
        else
            clear_personaldata();

        $.debounce(1000, function () {
            copy_values();
            $("#hidden_submiter")[0].submit();
        })();
    };

    var siteLanguageSpecifier;
    var loaded = function () {
        if (!window.siteLanguageSpecifier)
            siteLanguageSpecifier = "";
        else
            siteLanguageSpecifier = window.siteLanguageSpecifier;

        $("#hidden_submiter").attr("action", siteLanguageSpecifier + $("#hidden_submiter").attr("action"));

        if (!window.showCaptchaError)
            read_personaldata();
        $(".date-quick-pick > p").click(datepickclick);
        $("#input_bookdate")[0].context.events.add("changed", datechanged);
        var picks = $(".date-quick-pick > p");
        for (var i = 0; i < picks.length; i++) {
            if ($(picks[i]).hasClass("selected")) {
                lastindex = i;
                break;
            }
        }

        $($("#input_name")[0].context.input).focus(focus_name);
        $($("#input_name")[0].context.input).blur(blur_name);
        $($("#input_qqnumber")[0].context.input).focus(focus_qq);
        $($("#input_qqnumber")[0].context.input).blur(blur_qq);

        $($("#input_email")[0].context.input).focus(focus_email);
        $($("#input_email")[0].context.input).blur(blur_email);
        $($("#input_email")[0].context.input).keyup($.debounce(check_delay, check_email));
        $($("#input_email")[0].context.input).keydown(function () {
            $("#input_email")[0].removeAttribute("data-error");
            $("#input_mail_err").removeClass("show");
        });
        $($("#input_email")[0].context.input).change(check_email);

        $($("#input_phonenumber")[0].context.input).focus(focus_phonenumber);
        $($("#input_phonenumber")[0].context.input).blur(blur_phonenumber);
        $($("#input_phonenumber")[0].context.input).keyup($.debounce(check_delay, check_phonenumber));
        $($("#input_phonenumber")[0].context.input).keydown(function () {
            $("#input_phonenumber")[0].removeAttribute("data-error");
            $("#input_phone_err").removeClass("show");
        });
        $($("#input_phonenumber")[0].context.input).change(check_phonenumber);

        $($("#input_detail")[0].context.input).change(check_all);
        $($("#input_detail")[0].context.input).keyup($.debounce(check_delay, check_all));

        check_all();

        $("#btn_submit")[0].context.events.add("click", submit_click);

        if (window.showCaptchaError) {
            $("#btn_submit")[0].context.set_enabled(true);
            $("#input_captcha")[0].context.set_error("验证码错误");
            $($("#input_captcha")[0].context.input).focus();
            $($("#input_captcha")[0].context.input).keydown(function () {
                $("#input_captcha")[0].context.clear_error();
                $($("#input_captcha")[0].context.input).unbind("keydown");
            });
        }
        $("#input_detail")[0].context.set_value($("#hidden_problemdetail").val());
    };

    window.addEventListener("load", loaded, false);
})();