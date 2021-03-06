﻿"use strict";

(function () {
    "use strict";
    var str_trim = function str_trim(str) {
        return str.replace(/(^\s+)|(\s+$)/g, '');
    };

    var dataset_helper = {
        set: function set(obj, property, val) {
            if (obj != undefined) {
                if (obj.dataset == undefined) {
                    obj.setAttribute("data-" + property, val);
                } else {
                    var ps = property.split("-");
                    if (ps.length <= 0) return undefined;
                    var prop = ps[0];
                    for (var i = 1; i < ps.length; i++) {
                        if (ps[i].length <= 1) prop += ps[i].toUpperCase();else {
                            prop += ps[i].substring(0, 1).toUpperCase();
                            prop += ps[i].substring(1);
                        }
                    }
                    obj.dataset[prop] = val;
                }
            }
        },
        read: function read(obj, property) {
            if (obj == undefined) return undefined;
            if (obj.dataset == undefined) return obj.getAttribute("data-" + property);else {
                var ps = property.split("-");
                if (ps.length <= 0) return undefined;
                var prop = ps[0];
                for (var i = 1; i < ps.length; i++) {
                    if (ps[i].length <= 1) prop += ps[i].toUpperCase();else {
                        prop += ps[i].substring(0, 1).toUpperCase();
                        prop += ps[i].substring(1);
                    }
                }
                return obj.dataset[prop];
            }
        },
        clear: function clear(obj, property) {
            if (obj == undefined) return;
            if (obj.hasAttribute("data-" + property)) obj.removeAttribute("data-" + property);
        }
    };

    function handler_base() {
        this.handlers = {};
    }
    handler_base.prototype = {
        constructor: handler_base,
        add: function add(type, handler) {
            if (typeof this.handlers[type] == 'undefined') {
                this.handlers[type] = new Array();
            }
            this.handlers[type].push(handler);
        },
        remove: function remove(type, handler) {
            if (this.handlers[type] instanceof Array) {
                var handlers = this.handlers[type];
                for (var i = 0, len = handlers.length; i < len; i++) {
                    if (handler[i] == handler) {
                        handlers.splice(i, 1);
                        break;
                    }
                }
            }
        },
        clear: function clear(type) {
            if (this.handlers[type] instanceof Array) {
                this.handlers[type] = undefined;
            }
        },
        trigger: function trigger(event, sender, args) {
            if (this.handlers[event] instanceof Array) {
                var handlers = this.handlers[event];
                for (var i = 0, len = handlers.length; i < len; i++) {
                    handlers[i].call(sender, sender, args);
                }
            }
        }
    };

    var getStyle = function getStyle(dom, attr) {
        return dom.currentStyle ? dom.currentStyle[attr] : getComputedStyle(dom, false)[attr];
    };

    //#region Flyout
    var flyout_curr = undefined;
    var flyout_pos_curr = undefined;

    var windowresizing = function windowresizing() {
        $.throttle(10, function () {
            var context = flyout_curr;
            if (context == undefined) return;
            var base = $("div[data-control='flyoutbase']")[0];
            var top = $(context.parent).offset().top + context.parent.offsetHeight + 5;
            if (flyout_pos_curr == "top") {
                top = $(context.parent).offset().top - base.offsetHeight - 5;
            }
            var left = $(context.parent).offset().left;
            var itemwidth = context.item.offsetWidth + parseInt(getStyle(base, "paddingLeft"), 10) + parseInt(getStyle(base, "paddingRight"), 10);
            if (left + itemwidth + 10 > $(window).width()) left = $(window).width() - 10 - itemwidth;
            base.style.top = top + "px";
            base.style.left = left + "px";
        })();
    };
    var flyout_cover_click = function flyout_cover_click() {
        flyout_hide();
    };
    var flyout_hide = function flyout_hide() {
        // hiding
        var flyout = $("div[data-control='flyoutbase']")[0].children[0];
        var parent = flyout_curr.parent;
        var hidingargs = {
            prevent_hide: false,
            flyout: flyout
        };
        if (parent.context != undefined && parent.context.events != undefined) parent.context.events.trigger("flyout_hiding", parent, hidingargs);
        if (hidingargs.prevent_hide) return;

        window.removeEventListener("resize", windowresizing, false);
        $("div[data-control='flyoutlayer']").removeClass("flyoutshow");

        if (parent.context != undefined && parent.context.events != undefined) parent.context.events.trigger("flyout_hide", parent, {});
        flyout_curr = undefined;
        flyout_pos_curr = undefined;
    };
    var flyout_show = function flyout_show() {
        var context = this;
        var position = "bottom";
        var _p = dataset_helper.read(context.item, "place");
        if (_p) position = _p;
        var parent = context.parent;
        // showing
        var showingargs = {
            flyout: context.item,
            prevent_show: false,
            position: position
        };
        if (parent.context != undefined && parent.context.events != undefined) parent.context.events.trigger("flyout_showing", parent, showingargs);
        if (showingargs.prevent_show) return;

        // show flyout
        flyout_curr = context;
        flyout_pos_curr = position;
        $("div[data-control='flyoutlayer']").addClass("flyoutshow");
        var base = $("div[data-control='flyoutbase']")[0];
        base.innerHTML = "";
        base.appendChild(context.item);
        var top = $(parent).offset().top + parent.offsetHeight + 5;
        if (position == "top") {
            top = $(parent).offset().top - base.offsetHeight - 5;
        }
        var left = $(parent).offset().left;
        var itemwidth = context.item.offsetWidth + parseInt(getStyle(base, "paddingLeft"), 10) + parseInt(getStyle(base, "paddingRight"), 10);
        if (left + itemwidth + 10 > $(window).width()) left = $(window).width() - 10 - itemwidth;
        base.style.top = top + "px";
        base.style.left = left + "px";

        // showed
        if (parent.context != undefined && parent.context.events != undefined) parent.context.events.trigger("flyout_showed", parent, {
            flyout: base.children[0]
        });
        window.addEventListener("resize", windowresizing, false);
    };
    var flyout_init = function flyout_init(obj, flyout) {
        var item = flyout.cloneNode(true);
        button_init(item);
        checkbox_init(item);
        listbox_init(item);
        datepicker_init(item);
        select_init(item);
        input_init(item);

        var context = {
            parent: obj,
            item: item,
            showFlyout: flyout_show,
            hideFlyout: flyout_hide
        };
        item.context = context;
        obj.context.flyout = context;
    };
    var flyout_global_init = function flyout_global_init() {
        var layer = document.createElement("div");
        dataset_helper.set(layer, "control", "flyoutlayer");
        var cover = document.createElement("div");
        cover.className = "cover";
        $(cover).click(flyout_cover_click);
        var base = document.createElement("div");
        dataset_helper.set(base, "control", "flyoutbase");
        layer.appendChild(cover);
        layer.appendChild(base);
        document.body.appendChild(layer);
    };
    //#endregion

    //#region Button
    var button_set_enabled = function button_set_enabled(is_enable) {
        var context = this;
        var button = context.self;
        if (is_enable) dataset_helper.set(button, "enabled", true);else dataset_helper.set(button, "enabled", false);
    };
    var button_is_enabled = function button_is_enabled() {
        var context = this;
        var button = context.self;
        return !(dataset_helper.read(button, "enabled") == "false");
    };
    var button_click = function button_click() {
        var button = this;
        if (!this.context.is_enabled()) return;
        if (button.context.flyout != undefined) button.context.flyout.showFlyout();
        button.context.events.trigger("click", button, {});
    };
    var button_set_text = function button_set_text(text) {
        var context = this;
        var button = context.self;
        dataset_helper.set(button, "text", text);
        button.children[0].innerText = text;
    };
    var button_get_text = function button_get_text() {
        var context = this;
        var button = context.self;
        return dataset_helper.read(button, "text");
    };
    var button_init = function button_init(parent) {
        $(parent).find("div[data-control='button']").each(function () {
            var button = this;
            if (button.hasAttribute("tabindex") == false) button.setAttribute("tabindex", -1);
            button.context = {
                self: button,
                events: new handler_base(),
                set_enabled: button_set_enabled,
                is_enabled: button_is_enabled,
                set_text: button_set_text,
                get_text: button_get_text
            };

            $(button).click(button_click);
            var span = document.createElement("span");
            span.innerText = dataset_helper.read(button, "text");
            var flyouts = $(button).children("div[data-control='flyout']");
            if (flyouts.length > 0) flyout_init(button, flyouts[0]);

            button.innerHTML = "";
            button.appendChild(span);
        });
    };
    //#endregion

    //#region ListBox
    var listbox_set_selected = function listbox_set_selected(i) {
        var box = this.self;
        var box_main = $(box).find(".main-list")[0];

        if (i < -1 || i >= box_main.children[0].children.length) return;

        var selected = box.context.selected_item();
        if (selected != undefined) $(selected).removeClass("selected");

        if (i == -1) {
            dataset_helper.set(box, "selected-index", -1);
        } else {
            dataset_helper.set(box, "selected-index", i);
            $(box_main.children[0].children[i]).addClass("selected");
        }

        if ($(box).hasClass("expand") == false) {
            if (i == -1) box_main.children[0].style.top = "2.1em";else box_main.children[0].style.top = "-" + box_main.children[0].children[i].offsetTop + "px";
        }

        if (i == -1) this.events.trigger("changed", box, { action: "programmatic", selected_index: -1, selected_item: undefined });else this.events.trigger("changed", box, { action: "programmatic", selected_index: i, selected_item: box.children[0].children[i] });
    };
    var listbox_span_click = function listbox_span_click(e) {
        var span = e.currentTarget;
        var listbox_div = span.parentElement;
        var listbox_main = listbox_div.parentElement;
        var listbox = listbox_main.parentElement;
        var index = 0;
        for (var i = 0; i < listbox_div.children.length; i++) {
            if (span == listbox_div.children[i]) {
                index = i;
                break;
            }
        }
        var lindex = dataset_helper.read(listbox, "selected-index");
        if (lindex != undefined) lindex = parseInt(lindex, 10);else lindex = -1;
        if (lindex == index) {
            listbox.blur();
            return;
        };

        if (lindex >= 0) $(listbox_div.children[lindex]).removeClass("selected");
        dataset_helper.set(listbox, "selected-index", index);
        $(listbox_div.children[index]).addClass("selected");
        listbox.blur();
        listbox.context.events.trigger("changed", listbox, {
            action: "user_selection",
            selected_index: index,
            selected_item: listbox_div.children[index]
        });
    };
    var listbox_collp = function listbox_collp(e) {
        var box = e.currentTarget;
        var $box = $(box);
        var box_main = $box.find(".main-list")[0];
        var index = parseInt(dataset_helper.read(box, "selected-index"));
        $box.removeClass("expand");

        if (index >= 0) box_main.children[0].style.top = "-" + box_main.children[0].children[index].offsetTop + "px";else box_main.children[0].style.top = "2.1em";
        box_main.style.height = "2.1em";
        var $spans = $(box_main.children[0]).children();
        $spans.unbind("click");
    };
    var listbox_expand = function listbox_expand() {
        var box = this;
        var $box = $(box);
        if (!box.context.is_enabled()) {
            $box.blur();
            return false;
        }
        var box_main = $box.find(".main-list")[0];
        box_main.children[0].style.top = 0;
        $box.addClass("expand");
        box_main.style.height = box_main.children[0].offsetHeight + "px";
        var $spans = $(box_main.children[0]).children();
        $spans.click(listbox_span_click);
    };
    var listbox_selected_index = function listbox_selected_index() {
        var box = this.self;
        return dataset_helper.read(box, "selected-index");
    };
    var listbox_selected_item = function listbox_selected_item() {
        var box = this.self;
        var selected = parseInt(this.selected_index());
        return $(box).find(".main-list")[0].children[0].children[selected];
    };
    var listbox_init = function listbox_init(parent) {
        $(parent).find("div[data-control='listbox']").each(function () {
            var box = this;
            var $box = $(box);
            if (box.hasAttribute("tabindex") == false) box.setAttribute("tabindex", -1);

            var childs = box.children;
            var box_main = document.createElement("div");
            box_main.className = "main-list";
            var div = document.createElement("div");

            var record = dataset_helper.read(box, "selected-item");
            var findrecord = undefined;
            var i = 0;
            while (box.children.length > 0) {
                if (childs[0].innerText == record) findrecord = i;
                div.appendChild(childs[0].cloneNode(true));
                box.removeChild(childs[0]);
                i++;
            }
            dataset_helper.clear(box, "selected-item");

            div.className = "itemholder";
            box_main.appendChild(div);

            var header_text = dataset_helper.read(box, "header");
            if (!header_text) {
                if (!$box.hasClass("no-header")) $box.addClass("no-header");
            } else {
                var p = document.createElement("p");
                p.className = "header";
                p.innerText = header_text;
                box.appendChild(p);
            }

            box.appendChild(box_main);
            var index = parseInt(dataset_helper.read(box, "selected-index"));
            if (!index && index != 0) {
                if (findrecord != undefined) {
                    dataset_helper.set(box, "selected-index", findrecord);
                    index = findrecord;
                } else {
                    dataset_helper.set(box, "selected-index", -1);
                    index = -1;
                }
            }
            if (index >= 0 && index < div.children.length) {
                div.style.top = "-" + div.children[index].offsetTop + "px";
                $(div.children[index]).addClass("selected");
            } else {
                dataset_helper.set(box, "selected-index", -1);
                div.style.top = "2.1em";
            }

            div = document.createElement("div");
            div.className = "cover";
            box_main.appendChild(div);

            box.context = {
                self: box,
                selected_item: listbox_selected_item,
                selected_index: listbox_selected_index,
                set_selection: listbox_set_selected,
                set_enabled: listbox_set_enabled,
                is_enabled: listbox_is_enabled,
                insert_item: listbox_insert_item,
                remove_item: listbox_remove_item,
                get_items: listbox_get_items,
                set_items: listbox_set_items,
                events: new handler_base()
            };

            $(box).focus(listbox_expand);
            $(box).blur(listbox_collp);
        });
    };
    var listbox_set_enabled = function listbox_set_enabled(is_enabled) {
        var context = this;
        var div = context.self;
        if (is_enabled) {
            dataset_helper.set(div, "enabled", true);
        } else {
            dataset_helper.set(div, "enabled", false);
        }
    };
    var listbox_is_enabled = function listbox_is_enabled() {
        var context = this;
        var div = context.self;
        var enabled = dataset_helper.read(div, "enabled");
        return !(enabled == "false");
    };
    var listbox_get_items = function listbox_get_items() {
        var ary = [];
        var box = this.self;
        var holder = $(box).find(".itemholder")[0];
        var childs = holder.children;
        for (var i = 0; i < childs.length; i++) {
            ary.push(childs[i].innerText);
        }
        return ary;
    };
    var listbox_set_items = function listbox_set_items(items) {
        var box = this.self;
        var holder = $(box).find(".itemholder")[0];
        var index = this.selected_index();
        dataset_helper.set(box, "selected-index", -1);
        holder.innerHTML = "";
        if (items instanceof Array) {
            var span;
            for (var i = 0; i < items.length; i++) {
                span = document.createElement("span");
                span.innerText = items[i];
                holder.appendChild(span);
            }
        }
        if (index >= 0) {
            if ($(box).hasClass("expand")) holder.style.top = 0;else holder.style.top = "2.1em";
            box.context.events.trigger("changed", box, {
                action: "item_removed",
                selected_index: -1,
                selected_item: undefined
            });
        }
    };
    var listbox_insert_item = function listbox_insert_item(index, item) {
        var box = this.self;
        var holder = $(box).find(".itemholder")[0];
        var childs = holder.children;
        var selected_index = this.selected_index();
        var span = document.createElement("span");
        span.innerText = item;
        index = Math.max(-1, index);
        index = Math.min(index, childs.length);
        for (var i = 0; i < childs.length; i++) {
            if (i == index) {
                holder.insertBefore(span, childs[i]);
                if (index <= selected_index) {
                    selected_index++;
                    dataset_helper.set(box, "selected-index", selected_index);
                    if ($(box).hasClass("expand") == false) holder.style.top = "-" + childs[selected_index].offsetTop + "px";
                }
                return;
            }
        }
        holder.appendChild(span);
    };
    var listbox_remove_item = function listbox_remove_item(index) {
        var box = this.self;
        var holder = $(box).find(".itemholder")[0];
        var selected_index = this.selected_index();
        var childs = holder.children;
        for (var i = 0; i < childs.length; i++) {
            if (i == index) {
                var text = childs[i].innerText;
                childs[i].remove();
                if (selected_index == index) {
                    dataset_helper.set(box, "selected-index", -1);
                    if ($(box).hasClass("expand") == false) holder.style.top = "2.1em";

                    box.context.events.trigger("changed", box, {
                        action: "item_removed",
                        selected_index: -1,
                        selected_item: undefined
                    });
                } else if (selected_index < index) {} else {
                    selected_index--;
                    dataset_helper.set(box, "selected-index", selected_index);
                    if ($(box).hasClass("expand") == false) holder.style.top = "-" + childs[selected_index].offsetTop + "px";
                }
                return text;
            }
        }
        return undefined;
    };
    //#endregion

    //#region Select
    var select_set_selected = function select_set_selected(i) {
        var select = this.self;
        var $select = $(select);
        var select_main = $select.find(".main-select")[0];
        var selected = select.context.selected_item();

        if (i < -1 || i >= select_main.children[0].children[0].children.length) return;

        if (selected != undefined) $(selected).removeClass("selected");

        var actualnone = false;
        if (i == -1) {
            dataset_helper.set(select, "selected-index", -1);
            actualnone = true;
            i = 0;
        } else {
            dataset_helper.set(select, "selected-index", i);
            $(select_main.children[0].children[0].children[i]).addClass("selected");
        }
        if ($select.hasClass("expand")) select_main.children[0].scrollTop = select_main.children[0].children[0].children[i].offsetTop;else select_main.children[0].children[0].style.top = "-" + select_main.children[0].children[0].children[i].offsetTop + "px";

        if (actualnone) select.context.events.trigger("changed", select, {
            action: "programmatic",
            selected_index: -1,
            selected_item: undefined
        });else select.context.events.trigger("changed", select, {
            action: "programmatic",
            selected_index: i,
            selected_item: select_main.children[0].children[0].children[i]
        });
    };
    var select_selected_item = function select_selected_item() {
        var select = this.self;
        var selected = parseInt(this.selected_index());
        if (selected == -1) return undefined;
        return $(select).find(".main-select")[0].children[0].children[0].children[selected];
    };
    var select_span_click = function select_span_click(e) {
        var span = e.currentTarget;
        var select_div = span.parentElement.parentElement;
        var select = select_div.parentElement.parentElement;
        var index = 0;
        for (var i = 0; i < select_div.children[0].children.length; i++) {
            if (span == select_div.children[0].children[i]) {
                index = i;
                break;
            }
        }
        var lindex = dataset_helper.read(select, "selected-index");
        if (lindex != undefined) lindex = parseInt(lindex, 10);else lindex = -1;

        if (lindex == index) {
            select.blur();
            return;
        }

        if (lindex >= 0) $(select_div.children[0].children[lindex]).removeClass("selected");

        dataset_helper.set(select, "selected-index", index);
        $(select_div.children[0].children[index]).addClass("selected");
        select.blur();
        select.context.events.trigger("changed", select, {
            action: "user_selection",
            selected_index: index,
            selected_item: select_div.children[0].children[index]
        });
    };
    var select_list_scroll = function select_list_scroll(e, delta) {
        if (delta > 0) {
            if (e.currentTarget.scrollTop <= 0) e.preventDefault();
        } else if (delta < 0) {
            if (e.currentTarget.scrollTop + e.currentTarget.offsetHeight >= e.currentTarget.children[0].offsetHeight) e.preventDefault();
        }
    };
    var select_focus = function select_focus() {
        var select = this;
        var $select = $(select);
        var select_main = $select.find(".main-select")[0];
        if (!select.context.is_enabled()) {
            $select.blur();
            return false;
        }
        select_main.children[0].children[0].style.top = "0";
        var height = select_main.children[0].children[0].offsetHeight;
        if (height > 200) height = 200;
        select_main.children[0].style.height = height + "px";
        var $spans = $(select_main.children[0].children[0]).children();
        $spans.click(select_span_click);
        var index = parseInt(dataset_helper.read(select, "selected-index"));
        if (index < 0 || isNaN(index)) index = 0;
        if (index > $spans.length - 4) index = $spans.length - 1;else index = index - 2;
        if (index < 0) index = 0;
        $select.addClass("expand");
        $select.addClass("expandimmeditly");
        setTimeout(function () {
            if (select_main.children[0].children[0].children[index]) select_main.children[0].scrollTop = select_main.children[0].children[0].children[index].offsetTop;else select_main.children[0].scrollTop = 0;
            $(select_main.children[0]).bind("mousewheel", select_list_scroll);
        }, 200);
    };
    var select_collpase = function select_collpase() {
        var select = this;
        var $select = $(select);
        var select_main = $select.find(".main-select")[0];
        var index = parseInt(dataset_helper.read(select, "selected-index"));
        select_main.children[0].scrollTop = 0;
        if (index >= 0) select_main.children[0].children[0].style.top = "-" + select_main.children[0].children[0].children[index].offsetTop + "px";else select_main.children[0].children[0].style.top = "0";
        select_main.children[0].style.height = select_main.offsetHeight + "px";

        $(select_main.children[0]).unbind("mousewheel", select_list_scroll);
        var $spans = $(select_main.children[0].children[0]).children();
        $spans.unbind("click");
        $select.removeClass("expandimmeditly");
        setTimeout(function () {
            $select.removeClass("expand");
        }, 200);
    };
    var select_init = function select_init(parent) {
        $(parent).find("div[data-control='select']").each(function () {
            var select = this;
            var $select = $(select);
            if (select.hasAttribute("tabindex") == false) select.setAttribute("tabindex", -1);

            var childs = select.children;
            var select_main = document.createElement("div");
            select_main.className = "main-select";

            var div = document.createElement("div");
            var cdiv = document.createElement("div");
            var record = dataset_helper.read(select, "selected-item");
            var findrecord = undefined;
            var i = 0;
            while (select.children.length > 0) {
                if (childs[0].innerText == record) findrecord = i;
                cdiv.appendChild(childs[0].cloneNode(true));
                select.removeChild(childs[0]);
                i++;
            }
            dataset_helper.clear(select, "selected-item");
            var index = parseInt(dataset_helper.read(select, "selected-index"));
            if (!index && index != 0) {
                if (findrecord != undefined) {
                    dataset_helper.set(select, "selected-index", findrecord);
                    index = findrecord;
                } else {
                    dataset_helper.set(select, "selected-index", -1);
                    index = -1;
                }
            }

            var header_text = dataset_helper.read(select, "header");
            if (!header_text) {
                if (!$select.hasClass("no-header")) $select.addClass("no-header");
            } else {
                var p = document.createElement("p");
                p.className = "header";
                p.innerText = header_text;
                select.appendChild(p);
            }

            div.appendChild(cdiv);
            div.className = "itemholder";
            select_main.appendChild(div);
            select.appendChild(select_main);
            div.style.height = select_main.offsetHeight + "px";

            select_main.children[0].scrollTop = 0;
            if (index >= 0 && index < cdiv.children.length) {
                select_main.children[0].children[0].style.top = "-" + select_main.children[0].children[0].children[index].offsetTop + "px";
                $(select_main.children[0].children[0].children[index]).addClass("selected");
                dataset_helper.set(select, "selected-index", index);
            } else {
                dataset_helper.set(select, "selected-index", -1);
                select_main.children[0].children[0].style.top = "0";
            }

            div = document.createElement("div");
            div.className = "cover";
            select_main.appendChild(div);

            select.context = {
                self: select,
                selected_item: select_selected_item,
                selected_index: listbox_selected_index,
                set_selection: select_set_selected,
                set_enabled: select_set_enabled,
                is_enabled: select_is_enabled,
                get_items: select_get_items,
                set_items: select_set_items,
                insert_item: select_insert_item,
                remove_item: select_remove_item,
                events: new handler_base()
            };
            var enabled = dataset_helper.read(select, "enabled");
            if (enabled == "false") {
                select.context.set_enabled(false);
            } else {
                select.context.set_enabled(true);
            }

            $(select).focus(select_focus);
            $(select).blur(select_collpase);
        });
    };
    var select_set_enabled = function select_set_enabled(is_enabled) {
        var context = this;
        var div = context.self;
        if (is_enabled) {
            dataset_helper.set(div, "enabled", true);
        } else {
            dataset_helper.set(div, "enabled", false);
        }
    };
    var select_is_enabled = function select_is_enabled() {
        var context = this;
        var div = context.self;
        var enabled = dataset_helper.read(div, "enabled");
        return !(enabled == "false");
    };
    var select_get_items = function select_get_items() {
        var ary = [];
        var select = this.self;
        var holder = $(select).find(".itemholder")[0];
        var childs = holder.children[0].children;
        for (var i = 0; i < childs.length; i++) {
            ary.push(childs[i].innerText);
        }
        return ary;
    };
    var select_set_items = function select_set_items(items) {
        var select = this.self;
        var holder = $(select).find(".itemholder")[0];
        var index = this.selected_index();
        dataset_helper.set(select, "selected-index", -1);
        holder.children[0].innerHTML = "";
        if (items instanceof Array) {
            var span;
            for (var i = 0; i < items.length; i++) {
                span = document.createElement("span");
                span.innerText = items[i];
                holder.children[0].appendChild(span);
            }
        }
        if (index >= 0) {
            holder.children[0].style.top = 0;

            select.context.events.trigger("changed", select, {
                action: "item_removed",
                selected_index: -1,
                selected_item: undefined
            });
        }
    };
    var select_insert_item = function select_insert_item(index, item) {
        var select = this.self;
        var holder = $(select).find(".itemholder")[0];
        var childs = holder.children[0].children;
        var selected_index = this.selected_index();
        var span = document.createElement("span");
        span.innerText = item;
        index = Math.max(-1, index);
        index = Math.min(index, childs.length);
        for (var i = 0; i < childs.length; i++) {
            if (i == index) {
                holder.children[0].insertBefore(span, childs[i]);
                if (index <= selected_index) {
                    selected_index++;
                    dataset_helper.set(select, "selected-index", selected_index);
                    if ($(select).hasClass("expand") == false) holder.children[0].style.top = "-" + childs[selected_index].offsetTop + "px";
                }
                return;
            }
        }
        holder.children[0].appendChild(span);
    };
    var select_remove_item = function select_remove_item(index) {
        var select = this.self;
        var holder = $(select).find(".itemholder")[0];
        var selected_index = this.selected_index();
        var childs = holder.children[0].children;
        for (var i = 0; i < childs.length; i++) {
            if (i == index) {
                var text = childs[i].innerText;
                childs[i].remove();
                if (selected_index == index) {
                    dataset_helper.set(select, "selected-index", -1);
                    if ($(select).hasClass("expand") == false) holder.children[0].style.top = "0";

                    select.context.events.trigger("changed", select, {
                        action: "item_removed",
                        selected_index: -1,
                        selected_item: undefined
                    });
                } else if (selected_index < index) {} else {
                    selected_index--;
                    dataset_helper.set(select, "selected-index", selected_index);
                    if ($(select).hasClass("expand") == false) holder.children[0].style.top = "-" + childs[selected_index].offsetTop + "px";
                }
                return text;
            }
        }
        return undefined;
    };
    //#endregion

    //#region Checkbox
    var checkbox_getchecked = function checkbox_getchecked() {
        var context = this;
        var checkbox = context.self;
        var ch = dataset_helper.read(checkbox, "checked");
        return ch == "true";
    };
    var checkbox_setchecked = function checkbox_setchecked(checked) {
        var context = this;
        var checkbox = context.self;
        var innerbox = checkbox.children[0].children[0];
        if (checked == false) {
            innerbox.checked = false;
            dataset_helper.set(checkbox, "checked", false);
        } else {
            innerbox.checked = true;
            dataset_helper.set(checkbox, "checked", true);
        }
        checkbox.context.events.trigger("changed", checkbox, {
            action: "programmatic",
            checked: innerbox.checked
        });
    };
    var checkbox_click = function checkbox_click(e) {
        var box = e.currentTarget;
        if (!box.context.is_enabled()) {
            return;
        }
        var checkbox = box.children[0].children[0];
        if (checkbox.checked) {
            checkbox.checked = false;
            dataset_helper.set(box, "checked", false);
        } else {
            checkbox.checked = true;
            dataset_helper.set(box, "checked", true);
        }
        box.context.events.trigger("changed", box, {
            action: "user_selection",
            checked: checkbox.checked
        });
    };
    var checkbox_init = function checkbox_init(parent) {
        var holder;
        var msg;
        var msgspan;
        var span;
        var checkbox;
        $(parent).find("div[data-control='checkbox']").each(function () {
            var div = this;
            if (div.hasAttribute("tabindex") == false) div.setAttribute("tabindex", -1);
            div.innerHTML = "";
            holder = document.createElement("div");
            msg = document.createElement("div");
            msgspan = document.createElement("span");
            msgspan.innerText = dataset_helper.read(div, "message");
            span = document.createElement("span");
            checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            holder.appendChild(checkbox);
            holder.appendChild(span);
            holder.className = "holder";
            msg.className = "message";
            msg.appendChild(msgspan);
            div.appendChild(holder);
            div.appendChild(msg);
            div.addEventListener("click", checkbox_click, false);

            div.context = {
                set_checked: checkbox_setchecked,
                self: div,
                events: new handler_base(),
                is_checked: checkbox_getchecked,
                is_enabled: checkbox_is_enabled,
                set_enabled: checkbox_set_enabled
            };
            div.context.set_checked(div.context.is_checked());
            div.context.set_enabled(div.context.is_enabled());
        });
    };
    var checkbox_is_enabled = function checkbox_is_enabled() {
        var context = this;
        var div = context.self;
        var enabled = dataset_helper.read(div, "enabled");
        return !(enabled == "false");
    };
    var checkbox_set_enabled = function checkbox_set_enabled(is_enabled) {
        var context = this;
        var div = context.self;
        if (is_enabled) {
            dataset_helper.set(div, "enabled", true);
        } else {
            dataset_helper.set(div, "enabled", false);
        }
    };
    //#endregion

    //#region DatePicker
    var datepicker_init = function datepicker_init(parent) {
        $(parent).find("div[data-control='datepicker']").each(function () {
            var picker_holder = this;
            var $picker_holder = $(picker_holder);
            if (picker_holder.hasAttribute("tabindex") == false) picker_holder.setAttribute("tabindex", -1);
            picker_holder.innerHTML = "";
            $picker_holder.focus(datepicker_focus);
            $picker_holder.blur(datepicker_blur);
            picker_holder.context = {
                set_date: datepicker_setdate,
                get_date: datepicker_getdate,
                set_enabled: datepicker_set_enabled,
                is_enabled: datepicker_is_enabled,
                self: picker_holder,
                events: new handler_base()
            };

            var enabled = dataset_helper.read(picker_holder, "enabled");
            if (enabled == "false") {
                picker_holder.context.set_enabled(false);
            } else {
                picker_holder.context.set_enabled(true);
            }

            var picker = document.createElement("div");
            picker.className = "main-picker";

            var header_text = dataset_helper.read(picker_holder, "header");
            if (!header_text) {
                if (!$picker_holder.hasClass("no-header")) $picker_holder.addClass("no-header");
            } else {
                var p = document.createElement("p");
                p.className = "header";
                p.innerText = header_text;
                picker_holder.appendChild(p);
            }

            var mindate = dataset_helper.read(picker_holder, "mindate");
            var maxdate = dataset_helper.read(picker_holder, "maxdate");
            var dateval = dataset_helper.read(picker_holder, "value");
            var date = new Date();
            if (mindate == null || mindate == undefined) mindate = "1990/01/01";
            if (maxdate == null || maxdate == undefined) maxdate = "2100/12/31";
            if (dateval == null || dateval == undefined) dateval = date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();

            var minresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(mindate);
            var maxresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(maxdate);
            var valresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(dateval);
            var minyear = 1990,
                minmonth = 1,
                minday = 1;
            var maxyear = 2100,
                maxmonth = 12,
                maxday = 31;
            var valyear = date.getFullYear(),
                valmonth = date.getMonth() + 1,
                valday = date.getDate();

            if (minresult != null) {
                minyear = parseInt(minresult["1"], 10);
                minmonth = parseInt(minresult["2"], 10);
                minday = parseInt(minresult["5"], 10);
                var daycheck = new Date(minyear, minmonth - 1, minday);
                minyear = daycheck.getFullYear();
                minmonth = daycheck.getMonth() + 1;
                minday = daycheck.getDate();
            }
            if (maxresult != null) {
                maxyear = parseInt(maxresult["1"], 10);
                maxmonth = parseInt(maxresult["2"], 10);
                maxday = parseInt(maxresult["5"], 10);
                var daycheck = new Date(maxyear, maxmonth - 1, maxday);
                maxyear = daycheck.getFullYear();
                maxmonth = daycheck.getMonth() + 1;
                maxday = daycheck.getDate();
            }
            if (valresult != null) {
                valyear = parseInt(valresult["1"], 10);
                valmonth = parseInt(valresult["2"], 10);
                valday = parseInt(valresult["5"], 10);
                var daycheck = new Date(valyear, valmonth - 1, valday);
                valyear = daycheck.getFullYear();
                valmonth = daycheck.getMonth() + 1;
                valday = daycheck.getDate();
            }
            mindate = new Date(minyear, minmonth - 1, minday);
            maxdate = new Date(maxyear, maxmonth - 1, maxday);
            dateval = new Date(valyear, valmonth - 1, valday);
            if (mindate.getTime() > dateval.getTime()) dateval.setTime(mindate.getTime());
            if (maxdate.getTime() < dateval.getTime()) dateval.setTime(maxdate.getTime());

            var div;
            div = document.createElement("div");

            div.className = "input-date-value";
            var p;
            p = document.createElement("p");
            p.innerText = dateval.getFullYear() + " 年 " + (dateval.getMonth() + 1) + " 月 " + dateval.getDate() + " 日";
            div.appendChild(p);
            picker.appendChild(div);

            div = document.createElement("div");
            div.className = "input-date-picker-container";
            var cdiv;
            cdiv = document.createElement("div");
            cdiv.className = "input-date-container-itemscrollview";
            $(cdiv).addClass("input-date-container-year");
            for (var i = mindate.getFullYear(); i <= maxdate.getFullYear(); i++) {
                var p = document.createElement("p");
                if (i == dateval.getFullYear()) {
                    p.className = "selected";
                }
                p.innerText = i.toString();
                cdiv.appendChild(p);
                $(p).click(datepicker_year_clicked);
            }
            div.appendChild(cdiv);
            cdiv = document.createElement("div");
            cdiv.className = "input-date-container-itemscrollview";
            $(cdiv).addClass("input-date-container-month");
            var monthbegin = 1;
            var monthend = 12;
            if (mindate.getFullYear() == dateval.getFullYear()) monthbegin = mindate.getMonth() + 1;
            if (maxdate.getFullYear() == dateval.getFullYear()) monthend = maxdate.getMonth() + 1;
            for (var i = monthbegin; i <= monthend; i++) {
                var p = document.createElement("p");
                if (i == dateval.getMonth() + 1) {
                    p.className = "selected";
                }
                p.innerText = i.toString();
                $(p).click(datepicker_month_clicked);
                cdiv.appendChild(p);
            }
            div.appendChild(cdiv);
            cdiv = document.createElement("div");
            cdiv.className = "input-date-container-itemscrollview";
            $(cdiv).addClass("input-date-container-date");
            var daybegin = 1;
            var dayend = new Date(dateval.getFullYear(), dateval.getMonth() + 1, 0).getDate();
            if (mindate.getFullYear() == dateval.getFullYear() && mindate.getMonth() == dateval.getMonth()) daybegin = mindate.getDate();
            if (maxdate.getFullYear() == dateval.getFullYear() && maxdate.getMonth() == dateval.getMonth()) dayend = maxdate.getDate();
            for (var i = daybegin; i <= dayend; i++) {
                var p = document.createElement("p");
                if (i == dateval.getDate()) {
                    p.className = "selected";
                }
                p.innerText = i.toString();
                $(p).click(datepicker_day_clicked);
                cdiv.appendChild(p);
            }
            div.appendChild(cdiv);
            picker.appendChild(div);
            div = document.createElement("div");
            div.className = "cover";
            picker.appendChild(div);
            picker_holder.appendChild(picker);

            dataset_helper.set(picker_holder, "value", dateval.getFullYear() + "/" + (dateval.getMonth() + 1) + "/" + dateval.getDate());
            dataset_helper.set(picker_holder, "mindate", mindate.getFullYear() + "/" + (mindate.getMonth() + 1) + "/" + mindate.getDate());
            dataset_helper.set(picker_holder, "maxdate", maxdate.getFullYear() + "/" + (maxdate.getMonth() + 1) + "/" + maxdate.getDate());
        });
    };
    var datepicker_year_clicked = function datepicker_year_clicked() {
        var yearp = this;
        if (yearp.className == "selected") return;
        var container = yearp.parentElement.parentElement;
        var datepicker = container.parentElement;
        var picker_holder = datepicker.parentElement;
        var mindate = dataset_helper.read(datepicker, "mindate");
        var maxdate = dataset_helper.read(datepicker, "maxdate");
        var dateval = dataset_helper.read(datepicker, "value");
        var minresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(mindate);
        var maxresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(maxdate);
        var valresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(dateval);
        var minyear = parseInt(minresult["1"], 10),
            minmonth = parseInt(minresult["2"], 10),
            minday = parseInt(minresult["5"], 10);
        var maxyear = parseInt(maxresult["1"], 10),
            maxmonth = parseInt(maxresult["2"], 10),
            maxday = parseInt(maxresult["5"], 10);
        var valyear = parseInt(valresult["1"], 10),
            valmonth = parseInt(valresult["2"], 10),
            valday = parseInt(valresult["5"], 10);
        var yearct = container.children[0];
        var monthct = container.children[1];
        var datect = container.children[2];
        for (var i = 0; i < yearct.children.length; i++) {
            if (yearct.children[i].className == "selected") yearct.children[i].className = "";
        }
        yearp.className = "selected";
        var newyear = parseInt(yearp.innerText, 10);
        monthct.innerHTML = "";
        datect.innerHTML = "";
        var monthbegin = 1,
            monthend = 12;
        if (newyear == minyear) monthbegin = minmonth;
        if (newyear == maxyear) monthend = maxmonth;
        for (var i = monthbegin; i <= monthend; i++) {
            var p = document.createElement("p");
            if (i == monthbegin) {
                p.className = "selected";
            }
            p.innerText = i.toString();
            $(p).click(datepicker_month_clicked);
            monthct.appendChild(p);
        }
        monthct.scrollTop = 0;
        var daybegin = 1,
            dayend = new Date(newyear, monthbegin, 0).getDate();
        if (newyear == minyear && minmonth == monthbegin) daybegin = minday;
        if (newyear == maxyear && monthbegin == maxmonth) dayend = maxday;
        datect.innerHTML = "";
        for (var i = daybegin; i <= dayend; i++) {
            var p = document.createElement("p");
            if (i == daybegin) {
                p.className = "selected";
            }
            p.innerText = i.toString();
            $(p).click(datepicker_day_clicked);
            datect.appendChild(p);
        }
        dataset_helper.set(picker_holder, "value", newyear + "/" + monthbegin + "/" + daybegin);
        datepicker.children[0].children[0].innerText = newyear + " 年 " + monthbegin + " 月 " + daybegin + " 日";
        picker_holder.context.events.trigger("changed", picker_holder, {
            action: "user_selection",
            date: { year: valyear, month: monthbegin, day: daybegin }
        });
    };
    var datepicker_month_clicked = function datepicker_month_clicked() {
        var monthp = this;
        if (monthp.className == "selected") return;
        var container = monthp.parentElement.parentElement;
        var datepicker = container.parentElement;
        var picker_holder = datepicker.parentElement;
        var mindate = dataset_helper.read(picker_holder, "mindate");
        var maxdate = dataset_helper.read(picker_holder, "maxdate");
        var dateval = dataset_helper.read(picker_holder, "value");
        var minresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(mindate);
        var maxresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(maxdate);
        var valresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(dateval);
        var minyear = parseInt(minresult["1"], 10),
            minmonth = parseInt(minresult["2"], 10),
            minday = parseInt(minresult["5"], 10);
        var maxyear = parseInt(maxresult["1"], 10),
            maxmonth = parseInt(maxresult["2"], 10),
            maxday = parseInt(maxresult["5"], 10);
        var valyear = parseInt(valresult["1"], 10),
            valmonth = parseInt(valresult["2"], 10),
            valday = parseInt(valresult["5"], 10);
        var monthct = container.children[1];
        var datect = container.children[2];
        for (var i = 0; i < monthct.children.length; i++) {
            if (monthct.children[i].className == "selected") monthct.children[i].className = "";
        }
        monthp.className = "selected";
        var newmonth = parseInt(monthp.innerText, 10);
        var daybegin = 1,
            dayend = new Date(valyear, newmonth, 0).getDate();
        if (valyear == minyear && minmonth == newmonth) daybegin = minday;
        if (valyear == maxyear && newmonth == maxmonth) dayend = maxday;
        datect.innerHTML = "";
        for (var i = daybegin; i <= dayend; i++) {
            var p = document.createElement("p");
            if (i == daybegin) {
                p.className = "selected";
            }
            p.innerText = i.toString();
            $(p).click(datepicker_day_clicked);
            datect.appendChild(p);
        }
        dataset_helper.set(picker_holder, "value", valyear + "/" + newmonth + "/" + daybegin);
        datect.scrollTop = 0;
        datepicker.children[0].children[0].innerText = valyear + " 年 " + newmonth + " 月 " + daybegin + " 日";
        picker_holder.context.events.trigger("changed", picker_holder, {
            action: "user_selection",
            date: { year: valyear, month: newmonth, day: daybegin }
        });
    };
    var datepicker_day_clicked = function datepicker_day_clicked() {
        var datep = this;
        if (datep.className == "selected") return;
        var container = datep.parentElement.parentElement;
        var datepicker = container.parentElement;
        var picker_holder = datepicker.parentElement;
        var dateval = dataset_helper.read(picker_holder, "value");
        var valresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(dateval);
        var valyear = parseInt(valresult["1"], 10),
            valmonth = parseInt(valresult["2"], 10),
            valday = parseInt(valresult["5"], 10);
        var datect = container.children[2];
        for (var i = 0; i < datect.children.length; i++) {
            if (datect.children[i].className == "selected") datect.children[i].className = "";
        }
        datep.className = "selected";
        dataset_helper.set(picker_holder, "value", valyear + "/" + valmonth + "/" + datep.innerText);
        datepicker.children[0].children[0].innerText = valyear + " 年 " + valmonth + " 月 " + datep.innerText + " 日";
        picker_holder.context.events.trigger("changed", picker_holder, {
            action: "user_selection",
            date: { year: valyear, month: valmonth, day: datep.innerText }
        });
    };
    var datepicker_scroll = function datepicker_scroll(e, delta) {
        if (delta > 0) {
            if (e.currentTarget.scrollTop <= 0) e.preventDefault();
        } else if (delta < 0) {
            if (e.currentTarget.scrollTop + e.currentTarget.offsetHeight >= e.currentTarget.lastChild.offsetHeight + e.currentTarget.lastChild.offsetTop) e.preventDefault();
        }
    };
    var datepicker_focus = function datepicker_focus() {
        var $picker_holder = $(this);
        var context = this.context;
        if (!context.is_enabled()) {
            $picker_holder.blur();
            return false;
        }

        var picker = $picker_holder.find(".main-picker")[0];
        var container = picker.children[1];
        var yearct = container.children[0];
        var monthct = container.children[1];
        var datect = container.children[2];
        for (var i = 0; i < yearct.children.length; i++) if ($(yearct.children[i]).hasClass("selected")) yearct.scrollTop = yearct.children[i].offsetTop;
        for (var i = 0; i < monthct.children.length; i++) if ($(monthct.children[i]).hasClass("selected")) monthct.scrollTop = monthct.children[i].offsetTop;
        for (var i = 0; i < datect.children.length; i++) if ($(datect.children[i]).hasClass("selected")) datect.scrollTop = datect.children[i].offsetTop;
        $(yearct).bind("mousewheel", datepicker_scroll);
        $(monthct).bind("mousewheel", datepicker_scroll);
        $(datect).bind("mousewheel", datepicker_scroll);
    };
    var datepicker_blur = function datepicker_blur() {
        var $picker_holder = $(this);
        var picker = $picker_holder.find(".main-picker")[0];
        var container = picker.children[1];
        var yearct = container.children[0];
        var monthct = container.children[1];
        var datect = container.children[2];
        $(yearct).unbind("mousewheel", datepicker_scroll);
        $(monthct).unbind("mousewheel", datepicker_scroll);
        $(datect).unbind("mousewheel", datepicker_scroll);
    };
    var datepicker_setdate = function datepicker_setdate(date) {
        var i = 0;
        var picker_holder = this.self;
        var picker = $(picker_holder).find(".main-picker")[0];
        var mindate = dataset_helper.read(picker_holder, "mindate");
        var maxdate = dataset_helper.read(picker_holder, "maxdate");
        if (mindate == null || mindate == undefined) mindate = "1990/01/01";
        if (maxdate == null || maxdate == undefined) maxdate = "2100/12/31";

        var minresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(mindate);
        var maxresult = /^(\d{4})\/((0?[1-9])|(1[0-2]))\/((0?[1-9])|(1\d)|(2\d)|(3[0-1]))$/g.exec(maxdate);
        var minyear = 1990,
            minmonth = 1,
            minday = 1;
        var maxyear = 2100,
            maxmonth = 12,
            maxday = 31;
        var valyear = date.getFullYear(),
            valmonth = date.getMonth() + 1,
            valday = date.getDate();

        if (minresult != null) {
            minyear = parseInt(minresult["1"], 10);
            minmonth = parseInt(minresult["2"], 10);
            minday = parseInt(minresult["5"], 10);
            var daycheck = new Date(minyear, minmonth - 1, minday);
            minyear = daycheck.getFullYear();
            minmonth = daycheck.getMonth() + 1;
            minday = daycheck.getDate();
        }
        if (maxresult != null) {
            maxyear = parseInt(maxresult["1"], 10);
            maxmonth = parseInt(maxresult["2"], 10);
            maxday = parseInt(maxresult["5"], 10);
            var daycheck = new Date(maxyear, maxmonth - 1, maxday);
            maxyear = daycheck.getFullYear();
            maxmonth = daycheck.getMonth() + 1;
            maxday = daycheck.getDate();
        }

        mindate = new Date(minyear, minmonth - 1, minday);
        maxdate = new Date(maxyear, maxmonth - 1, maxday);
        var dateval = date;
        if (mindate.getTime() > dateval.getTime()) return false;
        if (maxdate.getTime() < dateval.getTime()) return false;

        var container = picker.children[1];
        var yearct = container.children[0];
        var monthct = container.children[1];
        var datect = container.children[2];
        var index = 0;
        for (var i = 0; i < yearct.children.length; i++) {
            if (yearct.children[i].className == "selected") yearct.children[i].className = "";
            if (yearct.children[i].innerText == valyear.toString()) {
                yearct.children[i].className = "selected";
                index = i;
            }
        }
        yearct.scrollTop = yearct.children[index].offsetTop;

        monthct.innerHTML = "";
        datect.innerHTML = "";
        var monthbegin = 1,
            monthend = 12;
        if (valyear == minyear) monthbegin = minmonth;
        if (valyear == maxyear) monthend = maxmonth;
        index = monthbegin;
        for (var i = monthbegin; i <= monthend; i++) {
            var p = document.createElement("p");
            if (i == valmonth) {
                p.className = "selected";
                index = i;
            }
            p.innerText = i.toString();
            $(p).click(datepicker_month_clicked);
            monthct.appendChild(p);
        }
        monthct.scrollTop = monthct.children[index - monthbegin].offsetTop;

        var daybegin = 1,
            dayend = new Date(valyear, valmonth, 0).getDate();
        if (valyear == minyear && valmonth == monthbegin) daybegin = minday;
        if (valyear == maxyear && valmonth == maxmonth) dayend = maxday;
        index = daybegin;
        for (var i = daybegin; i <= dayend; i++) {
            var p = document.createElement("p");
            if (i == valday) {
                p.className = "selected";
                index = i;
            }
            p.innerText = i.toString();
            $(p).click(datepicker_day_clicked);
            datect.appendChild(p);
        }
        datect.scrollTop = datect.children[index - daybegin].offsetTop;

        dataset_helper.set(picker_holder, "value", valyear + "/" + valmonth + "/" + valday);
        picker.children[0].children[0].innerText = valyear + " 年 " + valmonth + " 月 " + valday + " 日";
        picker_holder.context.events.trigger("changed", picker_holder, {
            action: "programmatic",
            date: { year: valyear, month: valmonth, day: valday }
        });
        return true;
    };
    var datepicker_getdate = function datepicker_getdate() {
        var picker = this.self;
        var date = dataset_helper.read(picker, "value");
        var dates = date.split("/");
        var valyear = parseInt(dates[0], 10);
        var valmonth = parseInt(dates[1], 10);
        var valday = parseInt(dates[2], 10);
        return {
            year: valyear,
            month: valmonth,
            day: valday
        };
    };
    var datepicker_set_enabled = function datepicker_set_enabled(is_enabled) {
        var context = this;
        var div = context.self;
        if (is_enabled) {
            dataset_helper.set(div, "enabled", true);
        } else {
            dataset_helper.set(div, "enabled", false);
        }
    };
    var datepicker_is_enabled = function datepicker_is_enabled() {
        var context = this;
        var div = context.self;
        var enabled = dataset_helper.read(div, "enabled");
        return !(enabled == "false");
    };
    //#endregion

    // #region Input
    var input_set_error = function input_set_error(error) {
        var context = this;
        var div = context.self;
        var $div = $(div);
        if (div.hintobj.error_timeout !== undefined) window.clearTimeout(div.hintobj.error_timeout);

        if (error == null) {
            dataset_helper.set(div, "error", null);
            $div.removeClass("has-error");
            div.hintobj.error.style.height = "0px";
        } else {
            dataset_helper.set(div, "error", error);
            var error_text = "[错误] " + error;
            div.hintobj.error.innerText = error_text;
            div.hintobj.error.style.height = "auto";
            div.hintobj.error_timeout = setTimeout(function () {
                div.hintobj.error_timeout = undefined;
                var height = $(div.hintobj.error).height() + "px";
                div.hintobj.error.style.height = height;
            }, 0);
            if (!$div.hasClass("has-error")) $div.addClass("has-error");
        }
    };
    var input_clear_error = function input_clear_error() {
        this.set_error(null);
    };
    var input_set_enabled = function input_set_enabled(is_enabled) {
        var context = this;
        var div = context.self;
        var $div = $(div);
        if (is_enabled) {
            dataset_helper.set(div, "enabled", true);
            context.input.removeAttribute("disabled");
            var type = dataset_helper.read(div, "hint-type");
            if (type == "always") {
                div.hintobj.hint.style.height = "auto";
                setTimeout(function () {
                    var height = $(div.hintobj.hint).height() + "px";
                    div.hintobj.hint.style.height = height;
                }, 1);
            }
        } else {
            dataset_helper.set(div, "enabled", false);
            context.input.setAttribute("disabled", true);
        }
    };
    var input_is_enabled = function input_is_enabled() {
        var context = this;
        var div = context.self;
        return !(dataset_helper.read(div, "enabled") == "false");
    };
    var input_input_focus = function input_input_focus(e) {
        var div = e.currentTarget.parentElement;
        var $div = $(div);
        var hint = div.hintobj.hint;
        hint.style.height = "auto";
        setTimeout(function () {
            var height = $(hint).height() + "px";
            hint.style.height = height;
            if (!$div.hasClass("has-hint")) $div.addClass("has-hint");
        }, 0);
    };
    var input_input_blur = function input_input_blur(e) {
        var div = e.currentTarget.parentElement;
        var $div = $(div);
        var hint = div.hintobj.hint;
        hint.style.height = "0px";
        $div.removeClass("has-hint");
    };
    var input_set_hint = function input_set_hint(type, cate, hint) {
        var context = this;
        var div = context.self;
        var $div = $(div);
        var clear_events = function clear_events() {
            div.hintobj.hasEvent = false;
            div.context.input.removeEventListener("focus", input_input_focus);
            div.context.input.addEventListener("blur", input_input_blur);
        };
        var add_events = function add_events() {
            div.context.input.addEventListener("focus", input_input_focus, false);
            div.context.input.addEventListener("blur", input_input_blur, false);
            div.hintobj.hasEvent = true;
        };

        if (div.hintobj.hint_timeout !== undefined) window.clearTimeout(div.hintobj.hint_timeout);

        if (type == "hide") {
            dataset_helper.set(div, "hint-type", type);
            dataset_helper.set(div, "hint-cate", null);
            dataset_helper.set(div, "hint", null);

            $div.removeClass("has-hint");
            div.hintobj.hint.style.height = "0px";
            if (div.hintobj.hasEvent) clear_events();
        } else if (type == "focus") {
            dataset_helper.set(div, "hint-type", type);
            dataset_helper.set(div, "hint-cate", cate);
            dataset_helper.set(div, "hint", hint);

            var hint_text = hint;
            if (cate) hint_text = "[" + cate + "] " + hint_text;
            div.hintobj.hint.innerText = hint_text;

            if (document.activeElement == div.context.input) {
                div.hintobj.hint.style.height = "auto";

                div.hintobj.hint_timeout = setTimeout(function () {
                    div.hintobj.hint_timeout = undefined;
                    var height = $(div.hintobj.hint).height() + "px";
                    div.hintobj.hint.style.height = height;
                    if (!$div.hasClass("has-hint")) $div.addClass("has-hint");
                }, 0);
            } else {
                $div.removeClass("has-hint");
                div.hintobj.hint.style.height = "0px";
            }

            if (!div.hintobj.hasEvent) add_events();
        } else if (type == "always") {
            dataset_helper.set(div, "hint-type", type);
            dataset_helper.set(div, "hint-cate", cate);
            dataset_helper.set(div, "hint", hint);

            var hint_text = hint;
            if (cate) hint_text = "[" + cate + "] " + hint_text;

            div.hintobj.hint.innerText = hint_text;
            div.hintobj.hint.style.height = "auto";

            div.hintobj.hint_timeout = setTimeout(function () {
                div.hintobj.hint_timeout = undefined;
                var height = $(div.hintobj.hint).height() + "px";
                div.hintobj.hint.style.height = height;
                if (!$div.hasClass("has-hint")) $div.addClass("has-hint");
                if (div.hintobj.hasEvent) clear_events();
            }, 0);
        }
    };
    var input_change_hint = function input_change_hint(cate, hint) {
        var context = this;
        var div = context.self;
        var type = dataset_helper.read(div, "hint-type");
        context.set_hint(type, cate, hint);
    };
    var input_clear_hint = function input_clear_hint() {
        this.set_hint("hide", null, null);
    };
    var input_set_value = function input_set_value(value) {
        var context = this;
        var input = context.input;
        input.value = value;
    };
    var input_get_value = function input_get_value() {
        var context = this;
        var input = context.input;
        return input.value;
    };
    var input_value_change = function input_value_change(e) {
        var input = e.currentTarget;
        var div = input.parentElement;
        div.context.events.trigger("changed", div, {
            value: input.value
        });
    };
    var input_file_set_value = function input_file_set_value(files) {};
    var input_file_get_value = function input_file_get_value() {};
    var input_file_btn_click = function input_file_btn_click(sender, args) {
        var div = sender.parentElement;
        var input = div.context.input;
        $(input).click();
    };
    var input_file_value_changed = function input_file_value_changed(e) {
        var input = e.currentTarget;
        var div = input.parentElement;
        var files = input.files;
        if (files.length == 0) {
            div.input_name.value = "";
        } else if (files.length == 1) {
            div.context.input_name.value = files[0].name;
        } else {}
    };
    var input_file_init = function input_file_init(div, content) {
        var input_file = document.createElement("input");
        input_file.type = "file";
        var input_filename = document.createElement("input");
        input_filename.type = "text";
        input_filename.placeholder = "";

        var btn = document.createElement("div");
        var value = dataset_helper.read(div, "value");
        if (!value) value = dataset_helper.read(div, "header");
        if (!value) value = str_trim(content);
        if (!value) value = "选择文件";
        dataset_helper.set(btn, "control", "button");
        dataset_helper.set(btn, "text", value);
        div.appendChild(input_file);
        div.appendChild(input_filename);
        div.appendChild(btn);
        button_init(div);

        btn.context.events.add("click", input_file_btn_click);

        div.context = {
            self: div,
            input: input_file,
            input_name: input_filename,
            set_enabled: input_set_enabled,
            set_value: input_file_set_value,
            get_value: input_file_get_value,
            events: new handler_base()
        };

        input_file.addEventListener("change", input_file_value_changed, false);
    };
    var input_init = function input_init(parent) {
        $(parent).find("div[data-control='input']").each(function () {
            var div = this;
            var $div = $(div);
            var innerContent = div.innerHTML;
            div.innerHTML = "";
            var header = document.createElement("p");
            header.className = "header";
            div.appendChild(header);
            var data_header = dataset_helper.read(div, "header");
            if (data_header) header.innerText = data_header;else $div.addClass("no-header");

            var element;
            var type = dataset_helper.read(div, "type");
            if (!type) return;
            if (type == "textarea") {
                element = document.createElement("textarea");
            } else if (type == "file") {
                input_file_init(div, innerContent);
                return;
            } else {
                element = document.createElement("input");
                element.type = type;
                var placeholder = dataset_helper.read(div, "placeholder");
                if (placeholder) element.placeholder = placeholder;
            }
            div.appendChild(element);
            $(element).change(input_value_change);
            var value = dataset_helper.read(div, "value");
            if (value) dataset_helper.clear(div, "value");else value = str_trim(innerContent);
            element.value = value;
            var name = dataset_helper.read(div, "name");
            if (name) element.name = name;

            var hint = document.createElement("div");
            hint.className = "hint";
            hint.style.height = 0;
            div.appendChild(hint);
            var error = document.createElement("div");
            error.className = "error";
            error.style.height = 0;
            div.appendChild(error);

            div.context = {
                self: div,
                input: element,
                set_error: input_set_error,
                clear_error: input_clear_error,
                set_hint: input_set_hint,
                change_hint: input_change_hint,
                clear_hint: input_clear_hint,
                set_enabled: input_set_enabled,
                is_enabled: input_is_enabled,
                set_value: input_set_value,
                get_value: input_get_value,
                events: new handler_base()
            };
            div.hintobj = {
                hint: hint,
                error: error,
                hasEvent: false,
                hint_timeout: undefined,
                error_timeout: undefined
            };

            var data_error = dataset_helper.read(div, "error");
            if (data_error && data_error != "null") div.context.set_error(data_error);

            var hint_type = dataset_helper.read(div, "hint-type");
            var hint_cate = dataset_helper.read(div, "hint-cate");
            var hint_hint = dataset_helper.read(div, "hint");
            if (hint_hint) div.context.set_hint(hint_type, hint_cate, hint_hint);

            var enabled = dataset_helper.read(div, "enabled");
            if (enabled != undefined) {
                div.context.set_enabled(enabled != "false");
            } else {
                div.context.set_enabled(true);
            }
        });
    };
    // #endregion Input

    var load_components = function load_components(parent) {
        button_init(parent);
        checkbox_init(parent);
        listbox_init(parent);
        select_init(parent);
        datepicker_init(parent);
        input_init(parent);
    };

    var load = function load() {
        var body = document.body;
        flyout_global_init();
        load_components(body);
    };

    var npstrap = {
        load_components: load_components
    };
    window.npstrap = npstrap;

    window.addEventListener("load", load, false);
})();

