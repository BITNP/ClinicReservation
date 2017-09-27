(function () {
    "use strict";

    var trim = function trim(str) {
        return str.replace(/(^\s*)|(\s*$)/g, "");
    };
    var invalid_userreg = /[^A-Za-z0-9_@.]+/;
    var NETWORK_QUERY_DEBOUNCE = 1000;
    var LOCAL_CHECK_DEBOUNCE = 100;

    var username_needverify = true;
    var username_lastok = false;
    var check_username = function (name) {
        var dtd = $.Deferred();
        if (username_needverify === false) {
            dtd.resolve({
                succ: username_lastok,
                err: null
            });
        } else if (name.length <= 0) {
            username_needverify = false;
            username_lastok = false;
            dtd.resolve({
                succ: username_lastok,
                err: "用户名不能为空"
            });
        } else if (name.match(invalid_userreg)) {
            username_needverify = false;
            username_lastok = false;
            dtd.resolve({
                succ: username_lastok,
                err: "用户名格式错误"
            });
        } else {
            var param = { name: name };
            var start = new Date();
            var report_result = function report_result(data) {
                username_needverify = false;
                if (data) username_lastok = true; else username_lastok = false;
                dtd.resolve({
                    succ: username_lastok,
                    err: null
                });
            };
            $.ajax({
                url: "/member/queryloginname",
                type: "POST",
                data: JSON.stringify(param),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function (data) {
                var end = new Date();
                var diff = end - start;
                if (diff < 1000) setTimeout(report_result, 1000 - diff, data); else report_result(data);
            }).fail(function (err) {
                username_needverify = true;
                username_lastok = false;
                dtd.resolve({
                    succ: false,
                    err: err
                });
            });
        }
        return dtd.promise();
    };
    var username_keyup_blur = function (e) {
        if (e.type === "blur" && username_needverify === false) return;

        var div = e.currentTarget.parentElement;
        var value = trim(div.context.get_value());

        div.context.set_hint("always", null, "正在检测用户名");
        div.context.clear_error();
        username_needverify = true;
        check_username(value).done(function (result) {
            if (result.err !== null) {
                div.context.clear_hint();
                div.context.set_error(result.err);
            } else if (result.succ) {
                div.context.clear_hint();
                div.context.clear_error();
            } else {
                div.context.clear_hint();
                div.context.set_error("该用户名已被使用");
            }
        });
    };
    var notnull_keyup_blur = function notnull_keyup_blur(e) {
        var div = e.currentTarget.parentElement;
        var value = trim(div.context.get_value());
        if (value.length <= 0) {
            div.context.set_error("输入不能为空");
        } else {
            div.context.clear_error();
        }
    };
    var password_keyup_blur = function (e) {
        var divps = div_password.context.get_value();
        var conps = div_confirmpwd.context.get_value();
        if (divps === conps) {
            if (divps.length <= 0) {
                div_password.context.set_error("密码不能为空");
                div_confirmpwd.context.set_error("密码不能为空");
            } else {
                div_password.context.clear_error();
                div_confirmpwd.context.clear_error();
            }
        } else {
            div_password.context.set_error("密码不一致");
            div_confirmpwd.context.set_error("密码不一致");
        }
    };


    var copy_values = function () {
        var form = $("#hidden_submiter");
        form.find("[name='ticket']")[0].value = $("#input_ticket")[0].context.get_value();
        form.find("[name='loginname']")[0].value = $("#input_loginname")[0].context.get_value();
        form.find("[name='password']")[0].value = $.md5($("#input_pwd")[0].context.get_value());
        form.find("[name='name']")[0].value = $("#input_name")[0].context.get_value();
        form.find("[name='contact']")[0].value = $("#input_contact")[0].context.get_value();
        form.find("[name='grade']")[0].value = $("#input_grade")[0].context.get_value();
        form.find("[name='sexual']")[0].value = $("#input_sexual")[0].context.selected_item().innerText;
        form.find("[name='school']")[0].value = $("#input_school")[0].context.selected_item().innerText;
    };

    var submit_click = function (sender) {
        sender.context.set_enabled(false);
        $("#load_ring")[0].style.opacity = 1;

        $.debounce(1000, function () {
            copy_values();
            $("#hidden_submiter")[0].submit();
        })();
    };

    var div_password;
    var div_confirmpwd;

    var load = function () {
        var loginname = $($("#input_loginname")[0].context.input);
        loginname.blur(username_keyup_blur);
        loginname.keyup($.debounce(NETWORK_QUERY_DEBOUNCE, username_keyup_blur));

        div_password = $("#input_pwd")[0];
        div_confirmpwd = $("#input_pwd_confirm")[0];
        $(div_password.context.input).keyup($.debounce(LOCAL_CHECK_DEBOUNCE, password_keyup_blur));
        $(div_password.context.input).blur(password_keyup_blur);
        $(div_confirmpwd.context.input).keyup($.debounce(LOCAL_CHECK_DEBOUNCE, password_keyup_blur));
        $(div_confirmpwd.context.input).blur(password_keyup_blur);

        $($("#input_name")[0].context.input).blur(notnull_keyup_blur);
        $($("#input_name")[0].context.input).keyup($.debounce(LOCAL_CHECK_DEBOUNCE, notnull_keyup_blur));

        $("#btn_submit")[0].context.events.add("click", submit_click);

        $("#userpic").change(function () {
            var files = $("#userpic")[0].files;
            var windowURL = window.URL || window.webkitURL;
            if (files.length <= 0) {
                var dataurl = "";
                $("#preview_img")[0].src = dataurl;
            }
            else {
                var dataurl = windowURL.createObjectURL(this.files[0]);
                $("#preview_img")[0].src = dataurl;
            }
        });
        $("#btn_pickpic")[0].context.events.add("click", function () {
            $("#userpic").click();
        });
    };

    window.addEventListener("load", load, false);
})();