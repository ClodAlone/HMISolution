
function initSignalR($scope) {

    var url = "/deployserverhub";
    if (proxyPath)
        url = proxyPath + url;

    $scope.corehubconnection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .build();

    $scope.notconnected = true;
    $scope.corehubready = false;
    $scope.corehubconnection.onclose(reconnect);

    $scope.connectingShowIndicator = true;
    $scope.connectingShowPane = true;
    $scope.connectingShading = true;
    $scope.connectingMessage = "Connecting to the server...";

    $scope.connectingOptions = {
        shadingColor: "rgba(0,0,0,0.4)",
        position: { my: "center" },
        bindingOptions: {
            visible: "notconnected",
            showIndicator: "connectingShowIndicator",
            showPane: "connectingShowPane",
            shading: "connectingShading",
            message: "connectingMessage",
            closeOnOutsideClick: false
        }
    };

    $scope.corehubconnection.on("cputotalload", (value) => {

        $scope.cputotalload = value;
        $scope.$apply();
    });

    $scope.$apply();

    startConnection();

    function startConnection() {
        console.log('connecting...');

        $scope.corehubconnection.start().then(function() {
                console.log('connected!');
                $scope.notconnected = false;
                $scope.corehubready = true;
                $scope.$apply();
            }).catch(function(err) {
                reconnect();
                $scope.$broadcast(onError, { ex: err, value: "Error connecting : " + err.toString() });
            });
    }

    function reconnect() {
        $scope.notconnected = true;

        console.log('reconnecting...');
        setTimeout(startConnection, 2000);
    }
}