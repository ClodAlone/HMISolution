
var DeployServerApp = angular.module(DEPLOYSERVERAPP, ['dx']);

(function ($) {
    $.event.special.destroyed = {
        remove: function (o) {
            if (o.handler) {
                o.handler();
            }
        }
    };
})(jQuery);

DeployServerApp.controller(MAINCONTROLLER, ['$scope', '$compile', 
    function ($scope, $compile) {

        initBusyIndicator($scope);

        $scope.bIsMobile = false;
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            $scope.bIsMobile = true;
        }

        i18next
            .use(i18nextXHRBackend)
            .use(i18nextBrowserLanguageDetector)
            .init({
                fallbackLng: 'en',
                //debug: true,
                //ns: ['special', 'common'],
                //defaultNS: 'special',
                backend: {
                    // for all available options read the backend's repository readme file
                    loadPath: '/locales/{{lng}}/{{ns}}.json',
                    crossDomain: false
                }
            }, function (err, t) {

                $scope.busyVisible = true;
                $scope.$apply();

                jqueryI18next.init(i18next, $);

                initSignalR($scope);

                $scope.$watch('corehubready', function (v) {

                    try {
                        initMainPage($scope);
                    } catch { }

                    $scope.busyVisible = false;
                    $scope.$apply();
                });
            });

        $scope.$on(onError, function (ev, arg) {

            DevExpress.ui.notify(arg.value, "error", 6000);
        });

        $scope.showMesageBox = function (errorText, title) {
            DevExpress.ui.dialog.alert(errorText, title);
        };
        $scope.showConfirmBox = function (errorText, title) {
            return DevExpress.ui.dialog.confirm(errorText, title);
        };
    }]);
