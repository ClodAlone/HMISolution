
define(function () {
    return {
        show: function ($scope) {
            var passwordExpiredPage = "passwordExpiredPage";

            try {
                var page = $scope.loadHtmlOnDemand(passwordExpiredPage, true);

                showPasswordExpiredWindow($scope, page, passwordExpiredPage);
            }
            catch (e) {

                if (e === true) {

                    var deregisterListener = $scope.$on(onloadedHtmlOnDemand + passwordExpiredPage, function (e, value) {

                        deregisterListener();

                        page = $scope.loadHtmlOnDemand(passwordExpiredPage, true);
                        showPasswordExpiredWindow($scope, page, passwordExpiredPage);
                    });
                    var deregisterListenerError = $scope.$on(onError, function (e, value) {

                        deregisterListenerError();
                    });
                }
                else
                    throw e;
            }
        }
    };
});

function showPasswordExpiredWindow($scope, page, passwordExpiredPage) {

    var oldPassword = $scope.oldPassword;
    var newPassword = $scope.newPassword;
    var verifyPassword = $scope.verifyPassword;
    $scope.oldPassword = "";
    $scope.newPassword = "";
    $scope.verifyPassword = "";

    var deregisterListener = $scope.$on(hidingEvent + passwordExpiredPage, function (e, value) {

        if (value.dialogResult) {
            value.canClose = false;

            if (!$scope.newPassword || !$scope.verifyPassword || 
                $scope.newPassword !== $scope.verifyPassword) {
                $scope.showMesageBox(i18next.t('PasswordMismatchEmpty'));
                return;
            }

            if ($scope.configurationSettings.minRequiredPasswordLength > 0 &&
                $scope.newPassword.length < $scope.configurationSettings.minRequiredPasswordLength) {
                $scope.showMesageBox(i18next.t('PasswordNotLongEnough'));
                return;
            }

            $scope.busyVisible = true;
            $scope.$apply();

            $scope.corehubconnection.invoke("UpdateUser", $scope.username, $scope.password, $scope.newPassword)
                .then((ret) => {
                    if (ret <= 0) {
                        $scope.busyVisible = false;
                        $scope.$apply();
                        if (ret === -1)
                            $scope.showMesageBox(i18next.t('PasswordCannotBeReused'));
                        else
                            $scope.showMesageBox(i18next.t('UpdateUserFailed'));
                    }
                    else {

                        $scope.corehubconnection.invoke("LoginUser", $scope.username, $scope.newPassword, i18next.language)
                            .then((ret) => {
                                if (ret.status > 0) {
                                    $scope.currentUser = {
                                        name: $scope.username, level: ret.level,
                                        mask: ret.mask, role: ret.role, signature: ret.signature
                                    };
                                }
                                value.canClose = true;
                                $scope.$broadcast(hideEvent + passwordExpiredPage, passwordExpiredPage);
                                $scope.busyVisible = false;
                                $scope.$apply();
                                deregisterListener();
                                counter = 0;
                            })
                            .catch(err => {
                                console.error(err.toString());
                                $scope.busyVisible = false;
                                $scope.$apply();
                                $scope.$broadcast(onError, { ex: err, value: "Error loggin user in : " + err.toString() });
                            });
                    }
                })
                .catch(err => {
                    console.error(err.toString());
                    $scope.busyVisible = false;
                    $scope.$apply();
                    $scope.$broadcast(onError, { ex: err, value: "Error Updating the password : " + err.toString() });
                });
        }
        else {
            $scope.oldPassword = oldPassword;
            $scope.newPassword = newPassword;
            $scope.verifyPassword = verifyPassword;

            deregisterListener();
            $scope.$broadcast(hideEvent + passwordExpiredPage, passwordExpiredPage);
        }
    });

    showPopup({
        $scope: $scope,
        id: passwordExpiredPage,
        content: page,
        showOk: true,
        showCancel: false,
        checkresult: true,
        dragenabled: true,
        showtitle: true,
        titletxt: i18next.t("PASSWORD EXPIRED"),
        notScrollable: true,
        onContentReadyFunc: function (popupContainer) {
            popupContainer.css("min-width", "430px");
            var titleBar = popupContainer.find(".dx-toolbar-before");
            var label = popupContainer.find(".dx-toolbar-label");
            if (label.parent()[0] != titleBar[0])
                label.detach().appendTo(titleBar);
            popupContainer.find(".dx-toolbar-label .dx-toolbar-item-content > div").addClass("login100-form-title p-b-51");
            titleBar.css({
                "width": "100%",
                "text-align": "center"
            });
            popupContainer.find(".dx-popup-title").css("border-bottom", "none");
        }
    });
}
