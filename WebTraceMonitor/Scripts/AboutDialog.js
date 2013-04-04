aboutDialog = {
    show: function (item) {
        $("#aboutDialog").dialog({
            resizable: false,
            modal: true,
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