(function (window) {
    "use strict";

    var target_sizes;
    var size_sources;

    var onresize = function () {
        for (var i = 0; i < size_sources.length; i++) {
            target_sizes[i].style.width = size_sources[i].offsetWidth + "px";
        }
    };

    var load = function () {
        var table = $("table.items-list")[0];
        if (!table)
            return;

        size_sources = $(".items-title")[0].children;
        var thead = document.createElement("thead");
        var tr = document.createElement("tr");
        for (var i = 0; i < size_sources.length; i++) {
            var td = document.createElement("td");
            tr.appendChild(td);
        }
        thead.appendChild(tr);
        table.tHead = thead;
        target_sizes = [];
        for (i = 0; i < tr.children.length; i++)
            target_sizes.push(tr.children[i]);

        window.addEventListener("resize", $.throttle(50, onresize), false);
        onresize();
    };

    window.addEventListener("load", load, false);
})(window);