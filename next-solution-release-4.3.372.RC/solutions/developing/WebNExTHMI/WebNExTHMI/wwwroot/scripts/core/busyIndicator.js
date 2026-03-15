

function initBusyIndicator($scope) {

    var busyVisibileCounter = 0;
    $scope.busyVisibleCounter = false;
    $scope.$watch('busyVisible', function (v) {

        if (v === true) {
            if (++busyVisibileCounter === 1) {

                $scope.busyVisibleCounter = true;
            }
        }
        else {
            if (busyVisibileCounter > 0) {
                if (--busyVisibileCounter === 0)
                    $scope.busyVisibleCounter = false;
            }
        }
    });

    $scope.busyVisible = false;
    $scope.busyShowIndicator = true;
    $scope.busyShowPane = true;
    $scope.busyShading = true;

    $scope.restoreDefaultBusyMessage = function () {
        $scope.busyMessage = i18next.t("Please wait...");
    };
    $scope.restoreDefaultBusyMessage();

    $scope.busyOptions = {
        shadingColor: "rgba(0,0,0,0.4)",
        position: { my: "center" },
        bindingOptions: {
            visible: "busyVisibleCounter",
            showIndicator: "busyShowIndicator",
            showPane: "busyShowPane",
            shading: "busyShading",
            message: "busyMessage",
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
}
