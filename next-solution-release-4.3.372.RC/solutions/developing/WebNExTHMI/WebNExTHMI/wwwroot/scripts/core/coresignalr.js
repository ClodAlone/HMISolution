
function initSignalR($scope) {

    var url = "/corehub";
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
        }/*,
        onShown: function () {
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.loadingVisible = false;
                });
            }, 3000);
        },
        onHidden: function () {
            $scope.employeeInfo = employee;
        }
        */
    };

    $scope.corehubconnection.on("ReferenceUpdate", (screenId, listdata) => {
        $scope.$broadcast("ReferenceUpdate" + screenId, listdata);
    });

    $scope.corehubconnection.on("CanAckResetCommandUpdate", (screenId, bCanExecute) => {
        $scope.$broadcast("CanAckResetCommandUpdate" + screenId, bCanExecute);
    });

    var alarmsound = new Howl({
        src: ['audio/alarm.mp3'],
        loop: true
    });

    $scope.corehubconnection.on("AlarmBuzzing", (value) => {
        if (!value)
            alarmsound.stop();
        else
            alarmsound.play();
    });

    $scope.corehubconnection.on("WriteValueError", (screenId, humanReadable, value, exMessage) => {
        $scope.$broadcast("WriteValueError" + screenId, { "humanReadable": humanReadable, "value": value, "exMessage": exMessage });
    });

    $scope.corehubconnection.on("ExpressionParserError", (formula, exMessage) => {
        $scope.showMesageBox(i18next.t1('ExpressionTagMissing').replace("{0}", "'" + formula + "'") + ' : ' + exMessage);
    });

    $scope.corehubconnection.on("ErrorLoadingScreen", (screenId, humanReadable, value, exMessage) => {
        $scope.$broadcast("ErrorLoadingScreen" + screenId, { "humanReadable": humanReadable, "value": value, "exMessage": exMessage });
    });

    $scope.corehubconnection.on("ImageProcessed", (jsonData) => {
        var res_json = JSON.parse(jsonData);
        $scope.$broadcast("ImageProcessed" + res_json.pageID, res_json);
    });

    $scope.corehubconnection.on("PositionUpdate", (pageID, listdata) => {
        $scope.$broadcast("PositionUpdate" + pageID, listdata);
    });

    $scope.corehubconnection.on("RecipeCommandsInvalidate", (recipePath, recipeCommandsAllowedEventArgs) => {
        $scope.$broadcast("RecipeCommandsInvalidate" + recipePath, recipeCommandsAllowedEventArgs);
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
        $scope.corehubready = false;
        $scope.$apply();

        console.log('reconnecting...');
        setTimeout(startConnection, 2000);
    }
}