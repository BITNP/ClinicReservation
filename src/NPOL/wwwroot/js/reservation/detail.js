var detail_script = (function (map) {
    "use strict";

    var btn_rat_show = function (sender, args) {
        var flyout = args.flyout;
        var flyoutcontext = flyout.context;
        var select = $(flyout).find(".rate")[0];
        setTimeout(function () {
            select.context.set_selection(2);
        }, 5);
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function (_sender, _args) {
            var rate = parseInt(select.context.selected_index(), 10);
            var text = $(flyout).find("textarea")[0].value;
            if (text === undefined || text === null)
                text = "";
            setformdata(map.complete + rate, text);
            $("#actionform").submit();
        });
    };

    var setformdata = function (action, content) {
        $("#actionform input[name='action']").val(action);
        if (content)
            $("#actionform input[name='content']").val(content);
        else
            $("#actionform input[name='content']").val("");
    };
    var btn_msg_viewall_click = function () {
        setformdata(map.viewmsg, "");
        $("#actionform").submit();
    };
    var btn_click = function (sender, args) {
        var btn = this;
        var text = btn.context.get_text();

        switch (text) {
            case "填写留言":
                break;
            case "恢复预约":
                btn.context.set_enabled(false);
                setformdata(map.restore, "");
                $("#actionform").submit();
                break;
            case "填写评价":
                break;
            case "修改预约单":
                setformdata(map.modify, "");
                $("#actionform").submit();
                break;

        }
    };
    var btn_cancel_flyoutshow = function (sender, args) {
        var flyoutcontext = $("#btn_cancelpost")[0].context.flyout;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function () {
            setformdata(map.cancel, "");
            $("#actionform").submit();
        });
    };
    var btn_stop_flyoutshow = function (sender, args) {
        var flyoutcontext = $("#btn_stoppost")[0].context.flyout;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function () {
            setformdata(map.stop, "");
            $("#actionform").submit();
        });
    };
    var btn_postmessage_flyoutshow = function (sender, args) {
        var flyoutcontext = args.flyout.context;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function (_sender, _args) {
            var text = $(_sender.parentElement).find("textarea")[0].value;
            if (text === undefined || text.length === 0)
                return;
            setformdata(map.submitmessage, text);
            $("#actionform").submit();
        });
    };

    var load = function () {
        $(".cmdbtns").click(btn_click);
        var btn_cancelpost = $("#btn_cancelpost");
        if (btn_cancelpost.length > 0)
            btn_cancelpost[0].context.events.add("flyout_showing", btn_cancel_flyoutshow);

        var btn_stoppost = $("#btn_stoppost");
        if (btn_stoppost.length > 0)
            btn_stoppost[0].context.events.add("flyout_showing", btn_stop_flyoutshow);

        var btn_postmsg = $(".btn_postmessage");
        btn_postmsg.each(function () {
            this.context.events.add("flyout_showing", btn_postmessage_flyoutshow);
        });

        $("#btn_msg_viewall").each(function () {
            this.context.events.add("click", btn_msg_viewall_click);
        });

        $(".btn_markdone").each(function () {
            this.context.events.add("flyout_showing", btn_rat_show);
        });
    };
    load();
});