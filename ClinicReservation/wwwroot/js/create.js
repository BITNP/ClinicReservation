"use strict";

(function (window) {
    "use strict";

    var submit_button_click = function submit_button_click(sender, args) {
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

    var load = function load() {
        $("#input_detail")[0].context.set_value($("#hidden_problemdetail").val());

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
    };

    window.addEventListener("load", load, false);
})(window);

