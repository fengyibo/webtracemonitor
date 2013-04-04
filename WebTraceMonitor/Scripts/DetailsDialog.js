detailsDialog = {
    init: function (wtm) {
        $("#imgDetailsUp").on("click", wtm.showPrevious);
        $("#imgDetailsDown").on("click", wtm.showNext);
        $("#detailsDialog").on("dialogclose", function () { wtm.grid.focus(); });
    },

    show: function (item) {
        ko.applyBindings(item);
        $("#detailsDialog").dialog({
            title: "Trace Entry #" + item.id,
            resizable: false,
            modal: true,
            height: 600,
            width: 800,
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            },
            show: {
                effect: "clip",
                duration: 150
            },
            hide: {
                effect: "clip",
                duration: 150
            }
        });
    }
};