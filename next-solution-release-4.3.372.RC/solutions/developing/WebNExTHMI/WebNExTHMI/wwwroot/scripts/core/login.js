const loginPage = "loginPage";

define(function () {
    return {
        show: function ($scope) {

            try {
                if ($scope.configurationSettings.isExternalAuthActive) {
                    loginWithExternalIdentityProvider($scope);
                } else {
                    var page = $scope.loadHtmlOnDemand(loginPage, true);
                    showLoginWindow($scope, page, loginPage);
                }
            }
            catch (e) {

                if (e === true) {

                    var deregisterListener = $scope.$on(onloadedHtmlOnDemand + loginPage, function (e, value) {

                        deregisterListener();

                        page = $scope.loadHtmlOnDemand(loginPage, true);
                        showLoginWindow($scope, page, loginPage);
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

function loginWithExternalIdentityProvider($scope) {
    $scope.corehubconnection.invoke("CreateUrlForExternalIdPLogin")
        .then((loginUrl) => { window.open(loginUrl) });
}

counter = 0;
function showLoginWindow($scope, page, loginPage) {

    var username = $scope.username;
    var password = $scope.password;
    $scope.username = "";
    $scope.password = "";

    var deregisterListener = $scope.$on(hidingEvent + loginPage, function (e, value) {

        if (value.dialogResult) {
            value.canClose = false;

            if (!$scope.username || !$scope.password) {

                $scope.showMesageBox(i18next.t('UsernamePasswordEmpty'));
                return;
            }

            $scope.busyVisible = true;
            $scope.$apply();

            $scope.corehubconnection.invoke("LoginUser", $scope.username, $scope.password, i18next.language)
                .then((ret) => {
                    if (ret.status === 0) {
                        if (++counter > 3)
                            setTimeout(function () {
                                $scope.busyVisible = false;
                                $scope.$apply();
                                $scope.showMesageBox(i18next.t('LoginFailed'));
                            }, 1000 * counter);
                        else {
                            $scope.busyVisible = false;
                            $scope.$apply();
                            $scope.showMesageBox(i18next.t('LoginFailed'));
                        }
                    }
                    else
                        onLoginSuccessful(ret);

                    if (ret.status < 0)
                        onPasswordExpired();
                    else if (ret.status === 2) {
                        DevExpress.ui.dialog.confirm(i18next.t1("PasswordIsExpiring").replace("{0}", ret.passwordDaysLeft), i18next.t1("Warning"))
                            .then(function (res) {
                                if (res)
                                    onPasswordExpired();
                            });
                    }

                    $scope.$apply();
                })
                .catch(err => {
                    console.error(err.toString());
                    $scope.busyVisible = false;
                    $scope.$apply();
                    $scope.$broadcast(onError, { ex: err, value: "Error connecting : " + err.toString() });
                });
        }
        else {

            if ($scope.configurationSettings.requireLogin) {

                value.canClose = false;

                $scope.showMesageBox(i18next.t('RequiredLogin'));
                return;
            }

            $scope.username = username;
            $scope.password = password;

            deregisterListener();
            $scope.$broadcast(hideEvent + loginPage, loginPage);
        }

        function onPasswordExpired() {
            value.canClose = true;
            $scope.$broadcast(hideEvent + loginPage, loginPage);
            $scope.busyVisible = false;
            $scope.$apply();
            deregisterListener();
            counter = 0;

            require(['./scripts/core/passwordExpired.js'], function (passwordExpired) {
                passwordExpired.show($scope);
            });
        }

        function onLoginSuccessful(ret) {
            $scope.currentUser = {
                name: $scope.username, level: ret.level,
                mask: ret.mask, role: ret.role, signature: ret.signature
            };

            value.canClose = true;
            $scope.$broadcast(hideEvent + loginPage, loginPage);
            $scope.busyVisible = false;
            $scope.$apply();
            deregisterListener();
            counter = 0;
        }
    });

    showPopup({
        $scope: $scope,
        id: loginPage,
        content: page,
        showOk: true,
        showCancel: true,
        checkresult: true,
        dragenabled: true,
        showtitle: true,
        titletxt: i18next.t("LOGIN"),
        notScrollable: true,
        onContentReadyFunc: function (popupContainer) {
            var titleBar = popupContainer.find(".dx-toolbar-before");
            var label = popupContainer.find(".dx-toolbar-label");
            if (label.parent()[0] != titleBar[0])
                label.detach().appendTo(titleBar);
            popupContainer.find(".dx-toolbar-label .dx-toolbar-item-content > div").addClass("login100-form-title p-b-51");
            titleBar.css({
                "width": titleBar[0].clientWidth > 0 ? "100%" : "auto",
                "text-align": "center"
            });
            popupContainer.find(".dx-popup-title").css("border-bottom", "none");
        }
    });
}
