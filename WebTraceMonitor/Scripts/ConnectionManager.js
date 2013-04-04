wtmConnectionManager = {
    Connection: {},
    
    init: function() {
        wtmConnectionManager.Connection = $.connection('/trace');
        
        $(window).load(function () {
            wtmConnectionManager.Connection.start();
        });
        
        wtmConnectionManager.Connection.stateChanged(function (change) {
            if (change.newState === $.signalR.connectionState.disconnected) {
                $("#btnConnection").button("option", "label", "Connect");
                $("#btnConnection").button("option", "icons", { primary: "menue-icon-start" });
                window.setTimeout(function () {
                    $("#lblConnection").removeClass("lblConnection-receiving");
                    $("#lblConnection").removeClass("lblConnection-connected");
                    $("#lblConnection").addClass("lblConnection-disconnected");
                    $("#lblConnection").html("Offline");
                }, 100);
            }
            else if (change.newState === $.signalR.connectionState.connected) {
                $("#btnConnection").button("option", "label", "Disconnect");
                $("#btnConnection").button("option", "icons", { primary: "menue-icon-stop" });
                $("#lblConnection").removeClass("lblConnection-receiving");
                $("#lblConnection").removeClass("lblConnection-disconnected");
                $("#lblConnection").addClass("lblConnection-connected");
                $("#lblConnection").html("Connected");
            }
        });

        wtmConnectionManager.Connection.received(function (item) {
            wtm.processItems(item);
        });

        wtmConnectionManager.Connection.error(function (error) {
            $("#lblConnection").attr("title", error);
        });
    },
    
    start: function() {
        wtmConnectionManager.Connection.start();
    },
    
    stop: function () {
        wtmConnectionManager.Connection.stop();
    }
    
}