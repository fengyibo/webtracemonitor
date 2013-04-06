var wtm = {
    data: [],
    filteredData: [],
    autoScroll: true,
    index: 0,
    machines: { all: true },
    levels: { Error: true, Warning: true, Information: true, Verbose: true },
    errorCount: 0,
    selectedPosition: 0,
    hasChanged: false,

    init: function () {
        var positionFormatter = function(row, cell, value, columnDef, dataContext) {
            return "<a class='ui-state-default cell-index ui-button' onclick='wtm.showDialog(" + row + ");'>" + value + "</a>";
        };

        var traceLevelIconFormatter = function(row, cell, value, columnDef, dataContext) {
            switch (value) {
            case "Error":
                return "<span class='trace-level trace-error-image' title='Error'>&nbsp;</span>";
            case "Warning":
                return "<span class='trace-level trace-warning-image' title='Warning'>&nbsp;</span>";
            case "Information":
                return "<span class='trace-level trace-information-image' title='Information'>&nbsp;</span>";
            case "Verbose":
                return "<span class='trace-level trace-verbose-image' title='Verbose'>&nbsp;</span>";
            default:
                throw new Error("Unknown Trace Level");
            }
        };

        var options = {
            enableCellNavigation: true,
            forceFitColumns: true,
            selectedCellCssClass: "cell-selection",
            multiSelect: false,
            enableColumnReorder: true,
            rowHeight: 21
        };
        
        var columns = [
            { id: "no", name: "Position", field: "id", width: 45, selectable: false, cssClass: "ui-state-default cell-text cell-index", formatter: positionFormatter },
            { id: "level", name: "Level", field: "Level", width: 20, formatter: traceLevelIconFormatter, cssClass: "cell-trace-level", selectable: true },
            { id: "timestamp", name: "Time Stamp", field: "Timestamp", width: 110, selectable: true, cssClass: "cell-text cell-date" },
            { id: "machine", name: "Machine", field: "Machine", width: 150, selectable: true, cssClass: "cell-text" },
            { id: "category", name: "Category", field: "Category", width: 120, selectable: true, cssClass: "cell-text" },
            { id: "source", name: "Source", field: "Source", width: 120, selectable: true, cssClass: "cell-text" },
            { id: "process", name: "Process", field: "ProcessId", width: 120, selectable: true, cssClass: "cell-text" },
            { id: "thread", name: "Thread", field: "ThreadId", width: 120, selectable: true, cssClass: "cell-text" },
            { id: "text", name: "Text", field: "Message", width: 800, selectable: true, cssClass: "cell-text" },
            { id: "eventid", name: "Event Id", field: "EventId", width: 120, selectable: true, cssClass: "cell-text" }
        ];
        
        wtm.grid = new Slick.Grid("#dataGrid", wtm.filteredData, [columns[0], columns[1], columns[2], columns[3], columns[8]], options);
        wtm.grid.setSelectionModel(new Slick.RowSelectionModel());

        $(window).resize(function () {
            wtm.grid.resizeCanvas();
        });
        
        wtm.grid.onDblClick.subscribe(function(e, args) {
            wtm.showDialog(args.row);
        });

        wtm.grid.onKeyDown.subscribe(function(e, args) {
            if (e.keyCode === 13) {
                wtm.showDialog(args.row);
            }
        });
        wtm.columnpicker = new Slick.Controls.ColumnPicker(columns, wtm.grid, options);
        detailsDialog.init(this);
        wtm.start();
    },

    showDialog: function(row) {
        wtm.selectedPosition = row;
        detailsDialog.show(wtm.filteredData[row]);
    },

    doAutoScrollIfEnabled: function() {
        if (wtm.autoScroll) {
            if (wtm.filteredData.length > 0) {
                wtm.grid.scrollRowIntoView(wtm.filteredData.length - 1, true);
            }
        }
    },

    showItem: function(item, machines) {
        if (!(((item.Level == 'Error') && wtm.levels["Error"]) ||
            ((item.Level == 'Warning') && wtm.levels["Warning"]) ||
            ((item.Level == 'Information') && wtm.levels["Information"]) ||
            ((item.Level == 'Verbose') && wtm.levels["Verbose"])))
            return false;

        if (!(wtm.machines.all || wtm.machines[item.Machine]))
            return false;

        return true;
    },
   
    clear: function() {
        wtm.data = [];
        wtm.filteredData = [];
        wtm.index = 0;
        $("#spanMessageCount").html(0);
        wtm.grid.setData(wtm.filteredData);
        wtm.errorCount = 0;
        wtm.machines = new Object();
        wtm.machines.all = true;
        wtm.grid.focus();
    },

    updateFilters: function() {
        $("#waitOverlay").toggle();
        window.setTimeout(function() {
            wtm.filteredData = wtm.data.filter(wtm.showItem, wtm.machines);
            $("#waitOverlay").toggle();
            wtm.grid.setData(wtm.filteredData);
            wtm.grid.invalidate();
        }, 1);
    },

    toggleAutoscroll: function() {
        if (wtm.autoScroll) {
            $("#btnScroll").button("option", "icons", { primary: "menue-icon-start" });
            wtm.autoScroll = false;
        } else {
            $("#btnScroll").button("option", "icons", { primary: "menue-icon-pause" });
            wtm.autoScroll = true;
            wtm.doAutoScrollIfEnabled();
        }
        wtm.grid.focus();
        wtm.grid.resizeCanvas();
    },

    toggleLevel: function(levelToToggle) {
        wtm.levels[levelToToggle] = !wtm.levels[levelToToggle];
        return wtm.levels[levelToToggle];
    },

    showPrevious: function() {
        if (wtm.selectedPosition > 0) {
            wtm.selectedPosition--;
            wtm.grid.gotoCell(wtm.selectedPosition, 0);
            wtm.grid.scrollRowIntoView(wtm.selectedPosition, true);
            detailsDialog.show(wtm.filteredData[wtm.selectedPosition]);
        }
    },

    showNext: function() {
        if (wtm.selectedPosition < wtm.filteredData.length - 1) {
            wtm.selectedPosition++;
            wtm.grid.gotoCell(wtm.selectedPosition, 0);
            wtm.grid.scrollRowIntoView(wtm.selectedPosition, true);
            detailsDialog.show(wtm.filteredData[wtm.selectedPosition]);
        }
    },

    processItems: function(item) {

        function pad(number, length) {
            var str = '' + number;
            while (str.length < length) {
                str = '0' + str;
            }
            return str;
        }

        var arr = JSON.parse(item);
        for (var k = 0; k < arr.length; k++) {
            wtm.index++;
            var obj = arr[k];
            obj.id = pad(wtm.index, 8);
            obj.Timestamp = wtm.formatDateTime(new Date(parseInt(obj.Timestamp.substr(6)))),
            wtm.data.push(obj);
            if (obj.Level === "Error") {
                wtm.errorCount++;
            }
            if (wtm.machines[obj.Machine] == undefined) {
                wtm.machines[obj.Machine] = wtm.machines["all"];
            }
            if (wtm.showItem(obj, wtm.machines)) {
                wtm.filteredData.push(obj);
            }
        }

        if (arr.length > 0) {
            wtm.hasChanged = true;
        }
    },
    
    addZero: function (i) {
        i = i + "";
        if (i.length == 1) {
            i = "0" + i;
        }
        return i;
    },
    
    addTwoZeros: function (i) {
        i = i + "";
        if (i.length == 1) {
            i = "0" + i;
        }
        if (i.length == 2) {
            i = "0" + i;
        }
        return i;
    },
    
    formatDateTime: function (d) {
        return d.getFullYear() + "-" + wtm.addZero(d.getMonth() + 1) + "-" + wtm.addZero(d.getDate()) + " " + wtm.addZero(d.getHours()) + ":" + wtm.addZero(d.getMinutes()) + ":" + wtm.addZero(d.getSeconds()) + "." + wtm.addTwoZeros(d.getMilliseconds());
    },
    
    start: function() {
        wtm.updateGridView();
    },
    
    updateGridView: function () {
        if (wtm.hasChanged) {
            wtmToolbar.bindMachines();
            $("#spanMessageCount").html(wtm.data.length);
            $("#spanErrorCount").html(wtm.errorCount);
            wtm.grid.updateRowCount();
            wtm.grid.render();
            wtm.doAutoScrollIfEnabled();
        }
       window.setTimeout(function () { wtm.updateGridView(); }, 500);
    }
};

   