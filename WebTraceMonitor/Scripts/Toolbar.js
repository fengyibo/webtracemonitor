wtmToolbar = {
    toggleSelection: function (levelToToggle, el) {
        var enabled = wtm.toggleLevel(levelToToggle);
        if (enabled) {
            el.addClass("menu-icon-selected");
        } else {
            el.removeClass("menu-icon-selected");
        }
        wtm.grid.focus();
        wtm.updateFilters(wtm);
    },

    bindMachines: function () {
        var selectMachines = $("#btnMachines").empty();
        $.each(wtm.machines, function (key, value) {
            if (key != "all") {
                var opt = $('<option />');
                opt.addClass("ui-button ui-button-text-only toolbar-button");
                opt.html(key);
                if (wtm.machines[key]) {
                    opt.attr('selected', 'selected');
                }

                opt.appendTo(selectMachines);
            }
        });
        selectMachines.multiselect("refresh");
    },


    init: function () {
        $("#help-icon").on("click", aboutDialog.show);
        $("#btnScroll").on("click", wtm.toggleAutoscroll);
        $("#btnSave").on("click", wtm.savefile);
        $("#btnSave").button({ icons: { primary: "menue-icon-save" } });
        
        $("#btnScroll").button({ icons: { primary: "menue-icon-pause" } });
        $("#btnConnection").button({ icons: { primary: "menue-icon-stop" } }).click(function (event) {
            if ($("#btnConnection").button("option", "label") == "Connect" ||
            $("#btnConnection").button("option", "label") == "Connect") {
                wtmConnectionManager.start();
            } else {
                wtmConnectionManager.stop();
            }
            wtm.grid.focus();
        });

        $("#btnClear").button({
            icons: { primary: "menue-icon-clear" }
        }).click(function (event) {
            wtm.clear();
        });

        $("#btnError").button({
            icons: { primary: "menue-icon-trace-level-error" }
        }).click(function (event) {
            wtmToolbar.toggleSelection("Error", $("#btnError"));
        });

        $("#btnWarning").button({
            icons: { primary: "menue-icon-trace-level-warning" }
        }).click(function (event) {
            wtmToolbar.toggleSelection("Warning", $("#btnWarning"));
        });

        $("#btnInformation").button({
            icons: { primary: "menue-icon-trace-level-information" }
        }).click(function (event) {
            wtmToolbar.toggleSelection("Information", $("#btnInformation"));
        });

        $("#btnVerbose").button({
            icons: { primary: "menue-icon-trace-level-verbose" }
        }).click(function (event) {
            wtmToolbar.toggleSelection("Verbose", $("#btnVerbose"));
        });

        $("#toolbarGroupControl").buttonset();

        $("#btnMachines").multiselect({
            noneSelectedText: "Machines",
            selectedText: "# Machine(s) selected",
            classes: "ui-button ui-widget ui-state-default toolbar-button",
            minWidth: "225"
        }).bind("multiselectclick", function (event, ui) {
            wtm.machines[ui.text] = ui.checked;
            wtm.machines.all = false;
            wtm.updateFilters();
        }).bind("multiselectcheckall", function () {
            wtm.machines.all = true;
            $.each(wtm.machines, function (key) { wtm.machines[key] = true; });
            wtm.updateFilters();
        }).bind("multiselectuncheckall", function () {
            wtm.machines.all = false;
            $.each(wtm.machines, function (key) { wtm.machines[key] = false; });
            wtm.updateFilters();
        });

    }
}