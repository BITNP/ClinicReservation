(function (window) {
    "use strict";

    Array.prototype.copy = function () {
        var source = this;
        var result = [];
        for (var i = 0; i < source.length; i++) {
            result.push(source[i]);
        }
        return result;
    };
    Array.prototype.clear = function () {
        this.splice(0, this.length);
    };

    var on_filter_changed = function () {
        var val = $(this).val();
        var i;
        if (val === undefined || val === null || val.trim().length <= 0) {
            app.groups.clear();
            for (i = 0; i < groups.length; i++) {
                app.groups.push(groups[i]);
            }
        }
        else {
            app.groups.clear();
            val = val.trim();
            for (i = 0; i < groups.length; i++) {
                if (groups[i].code.indexOf(val) >= 0)
                    app.groups.push(groups[i]);
            }
        }
    };

    var btn_edit;
    var btn_add;
    var $selected = undefined;
    var row_clicked = function (group, event) {
        var row = event.currentTarget;
        if ($selected !== undefined && row === $selected[0])
            return;
        if ($selected !== undefined)
            $selected.removeClass("selected");
        $selected = $(row);
        $selected.addClass("selected");
        if (!btn_edit.context.is_enabled()) {
            btn_edit.context.set_enabled(true);
        }
        btn_edit.context.set_href("/groups/modify?code=" + group.code);
    };

    var button_add_clicked = function () {
        show_context(add_context);
    };

    var create_add_context = function (elem) {
        var btn_cancel;
        var code_elem;
        var $context_elem = $(elem);
        var context_elem = elem;
        var cancel_click = function () {
            hide_context();
        };
        var _update_data = function () {
        };
        var _on_hide = function () {
        };
        var _on_show = function () {
            code_elem.context.set_value("");
        };
        var cancel_click = function () {
            hide_context();
        };
        var _ok_click = function () {
        };
        var _init = function () {
            code_elem = $context_elem.find("div[data-control='input']")[0];
            btn_cancel = $context_elem.find(".btn-cancel")[0];
            btn_cancel.context.events.add("click", cancel_click);
        };
        var context = {
            update: _update_data,
            init: _init,
            on_hide: _on_hide,
            on_show: _on_show
        };
        return context;
    };
    var hide_context = function () {
        if (!current_context)
            return;
        current_context.style.opacity = 0;
        context_cotainer.style.height = current_context.offsetHeight + "px";
        setTimeout(function () {
            var old_context = current_context;
            context_cotainer.style.height = "0px";
            current_context.style.zIndex = 0;
            setTimeout(function () {
                old_context.style.position = "absolute";
            }, 200);
            current_context.context.on_hide();
            current_context = undefined;
        }, 0);
    };
    var show_context = function (context) {
        if (current_context === context) 
            return;
        var old_context = undefined;
        if (current_context) {
            old_context = current_context;
            current_context.style.opacity = 0;
            current_context.style.zIndex = 0;
            current_context.context.on_hide();
            context_cotainer.style.height = current_context.offsetHeight + "px";
        }
        context.style.opacity = 1;
        context.style.zIndex = 2;
        context.style.position = "relative";
        context.context.on_show();
        setTimeout(function () {
            context_cotainer.style.height = context.offsetHeight + "px";
            current_context = context;
            setTimeout(function () {
                if (old_context)
                    old_context.style.position = "absolute";
                context_cotainer.style.height = "auto";
            }, 200);
        }, 0);
    };
    var current_context;
    var add_context;
    var context_cotainer;

    var groups;
    var app;
    var load = function () {
        groups = window.groups;
        app = new Vue({
            el: ".items-list",
            data: {
                groups: groups.copy()
            },
            methods: {
                row_clicked: row_clicked
            }
        });

        $(".search-input").each(function () {
            $(this.context.input).keyup($.throttle(1000, on_filter_changed));
        });

        btn_edit = $("#btn-edit")[0];
        btn_add = $("#btn-add")[0];
        btn_add.context.events.add("click", button_add_clicked);

        add_context = $(".items-control-add-context")[0];
        context_cotainer = $(".items-control-context")[0];
        add_context.context = create_add_context(add_context);
        add_context.context.init();
    };

    window.addEventListener("load", load, false);

})(window);