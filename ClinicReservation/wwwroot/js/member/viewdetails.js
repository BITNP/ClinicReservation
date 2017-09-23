(function () {
    "use strict";

    var btn_msg_viewall_click = function () {
        
    };

    var btn_nextpage_click = function () {
        var loc = window.location.origin + window.location.pathname;
        var search = window.location.search;
        var match = search.match(/page=([^&]*)/);
        if (match) {
            var page = match[1];
            if (page.length > 0)
                page = parseInt(page, 10) + 1;
            else
                page = 2;
            search = search.replace(/page=[^&]*/, "page=" + page);
        }
        else if (search.length === 0)
            search = "?page=2";
        else
            search = search + "&page=2";
        var url = loc + search;
        window.location.href = url;
    };
    var btn_id_filter_click = function () {
        var btn = this.self;
        var val = "id=" + parseInt($("#input_id_filter")[0].context.get_value(), 10);

        var loc = window.location.origin + window.location.pathname;
        var search = window.location.search;

        if (search.indexOf("id") > 0)
            search = search.replace(/id=[^&]*/, val);
        else if (search.length === 0)
            search = "?" + val;
        else
            search = search + "&" + val;

        var url = loc + search;
        window.location.href = url;
    };

    var btn_submit_message_flyoutshow = function (sender, args) {
        var flyoutcontext = args.flyout.context;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function (sender) {
            var ispublic = $(flyout).find(".checkbox_ispublic")[0].context.is_checked();
            var text = $(sender.parentElement).find("textarea").val();
            if (text === undefined || text.length === 0)
                return;

            $.post("/member/AddDetailMessage", {
                id: window.modelId,
                token: window.actionToken,
                message: text,
                ispublic: ispublic
            }, function (ret) {
                if (ret.result)
                    window.location.reload();
                else
                    alert("false");
            });
        });
    };

    var btn_markcancel_flyoutshow = function (sender, args) {
        var flyoutcontext = args.flyout.context;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function (sender) {
            $.post("/member/CloseDetail", {
                id: window.modelId,
                token: window.actionToken
            }, function (ret) {
                if (ret.result)
                    window.location.reload();
                else
                    alert("false");
            });
        });
    };

    var btn_markcomplete_flyoutshow = function (sender, args) {
        var flyoutcontext = args.flyout.context;
        var flyout = flyoutcontext.item;
        var button = $(flyout).find("div[data-control='button']")[0];
        button.context.events.clear("click");
        button.context.events.add("click", function (sender) {
            $.post("/member/CompleteDetail", {
                id: window.modelId,
                token: window.actionToken
            }, function (ret) {
                if (ret.result)
                    window.location.reload();
                else
                    alert("false");
            });
        });
    };

    var accept_detail_click = function (sender, args) {
        var btn = sender.context;
        $.post("/member/AcceptDetail", {
            id: window.modelId,
            token: window.actionToken
        }, function (ret) {
            if (ret.result)
                window.location.reload();
            else
                alert("false");
        });
    };
    var cancelaccept_detail_click = function (sender, args) {
        var btn = sender.context;
        $.post("/member/CancelAcceptDetail", {
            id: window.modelId,
            token: window.actionToken
        }, function (ret) {
            if (ret.result)
                window.location.reload();
            else
                alert("false");
        });
    };

    var select_filter_changed = function () {
        var loc = window.location.origin + window.location.pathname;
        var index = $("#select_filter")[0].context.selected_index();
        var search = window.location.search;
        var list = ["uncompleted", "me", "closed", "completed", "all"];
        var filterval = "filter=" + list[index];

        if (search.indexOf("filter") > 0)
            search = search.replace(/filter=[^&]*/, filterval);
        else if (search.length === 0)
            search = "?" + filterval;
        else
            search = search + "&" + filterval;

        var url = loc + search;
        window.location.href = url;
    };

    var cover_click = function (sender, args) {
        var cover = this;
        var datacontainer = cover.previousElementSibling;
        var data = datacontainer.children[0].innerText;
        var id = data.match(/ID:\s*(\d+)/)[1];

        var loc = window.location.origin + window.location.pathname;
        var search = window.location.search;
        if (search.indexOf("id") > 0)
            search = search.replace(/id=[^&]*/, "id=" + id);
        else if (search.length === 0)
            search = "?id=" + id;
        else
            search = search + "&id=" + id;

        var url = loc + search;
        window.location.href = url;
    };

    var login_heartbeat_timer = undefined;
    var login_heartbeat = function () {
        $.post("/member/LoginHeartbeat", {}, function (ret) {
            if (!ret.result)
                console.error("心跳保持失败");
        });
    };
    var load = function () {
        $("#select_filter")[0].context.events.add("changed", select_filter_changed);

        $(".itemslistview .itemcover").click(cover_click);

        $(".submit_message").each(function () {
            this.context.events.add("flyout_showing", btn_submit_message_flyoutshow);
        });
        $(".btn_markcancel").each(function () {
            this.context.events.add("flyout_showing", btn_markcancel_flyoutshow);
        });
        $(".accept_detail").each(function () {
            this.context.events.add("click", accept_detail_click);
        });
        $(".cancelaccept_detail").each(function () {
            this.context.events.add("click", cancelaccept_detail_click);
        });
        $(".complete_detail").each(function () {
            this.context.events.add("flyout_showing", btn_markcomplete_flyoutshow);
        });

        $("#btn_msg_viewall").each(function () {
            this.context.events.add("click", btn_msg_viewall_click);
        });

        $("#btn_id_filter")[0].context.events.add("click", btn_id_filter_click);

        if ($("#btn_nextpage")[0])
            $("#btn_nextpage")[0].context.events.add("click", btn_nextpage_click);

        // 每隔4分钟发送心跳请求保持登陆状态
        login_heartbeat_timer = setInterval(login_heartbeat, 240 * 1000);
    };
    window.addEventListener("load", load, false);
})();