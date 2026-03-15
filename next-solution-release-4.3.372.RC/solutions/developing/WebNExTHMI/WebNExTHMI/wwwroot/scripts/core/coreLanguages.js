
var alreadyInitializedLanguages = new Array();

function initLanguages($scope, bActivateUserFromUrl) {

    var localstorageCurrentLanguageKey = $scope.configurationSettings.projectTitle + 'CurrentLanguage';

    $scope.currentLanguage = navigator.language;
    DevExpress.localization.locale($scope.currentLanguage);
    $scope.changeLanguage = function (language) {

        $scope.busyVisible = true;
        $scope.currentLanguage = language;
        $scope.$apply();

        var arraycontainsLanguage = alreadyInitializedLanguages.indexOf(language) > -1;
        if (arraycontainsLanguage) {

            i18next
                .changeLanguage(language, (err, t) => {
                    if (err) {
                        $scope.busyVisible = false;
                        $scope.$apply();
                        return console.log('something went wrong loading', err);
                    }

                    localStorage.setItem(localstorageCurrentLanguageKey, language);
                    
                    $scope.corehubconnection.invoke("SetCurrentLocale", language);

                    $scope.$broadcast(onActiveLanguageChanged, language);

                    $scope.busyVisible = false;
                    $scope.$apply();
                });
            return;
        }

        var localstorageCurrentLanguageHashKey = $scope.configurationSettings.projectTitle + 'CurrentLanguageHash' + language;
        var localstorageCurrentLanguageTableKey = $scope.configurationSettings.projectTitle + 'CurrentLanguageTable' + language;

        var langHash;
        var langTable = localStorage.getItem(localstorageCurrentLanguageTableKey);
        if (langTable)
            langHash = localStorage.getItem(localstorageCurrentLanguageHashKey);

        $scope.corehubconnection.invoke("GetListStringForCulture", language, langHash ? parseInt(langHash) : 0)
            .then((ret) => {

                if (!ret.hashOk) {
                    localStorage.setItem(localstorageCurrentLanguageHashKey, ret.hash);
                    localStorage.setItem(localstorageCurrentLanguageTableKey, JSON.stringify(ret.listStrings));
                }
                else {
                    ret.listStrings = JSON.parse(langTable);
                }

                i18next.addResourceBundle(language, 'translation', ret.listStrings, true, true);

                alreadyInitializedLanguages.push(language);

                i18next
                    .changeLanguage(language, (err, t) => {
                        //if (err) {
                        //    $scope.busyVisible = false;
                        //    $scope.$apply();
                        //    return console.log('something went wrong loading', err);
                        //}

                        localStorage.setItem(localstorageCurrentLanguageKey, language);

                        $scope.corehubconnection.invoke("SetCurrentLocale", language);

                        $scope.$broadcast(onActiveLanguageChanged, language);

                        $scope.busyVisible = false;
                        $scope.$apply();
                    });

            }).catch(err => {
                console.error(err.toString());
                $scope.busyVisible = false;
                $scope.$apply();
                $scope.$broadcast(onError, { ex: err, value: "Error opening : " + err.toString() });
            });
    };

    if ($scope.configurationSettings.forceStartupCultureName && $scope.configurationSettings.projectCulture)
        $scope.changeLanguage($scope.configurationSettings.projectCulture);
    else {
        var currentLang = localStorage.getItem(localstorageCurrentLanguageKey);
        if (currentLang)
            $scope.changeLanguage(currentLang);
        else if ($scope.configurationSettings.availableLanguages.length > 0) {
            let clientCulture = $scope.configurationSettings.projectCulture || $scope.currentLanguage;
            let selectedCulture = $scope.configurationSettings.availableLanguages.find(item => item.indexOf(clientCulture) !== -1);
            $scope.changeLanguage(selectedCulture || $scope.configurationSettings.availableLanguages[0]);
        }
    }

    $scope.setTranslations = function (placeholder, translations) {
        var ret = {};
        for (var i = 0; i < translations.length; i++) {
            ret[translations[i].id] = (function (o) {
                return function () {
                    return i18next.t((!undefinedOrNull(o.customPrefix) ? o.customPrefix : "_" + placeholder + "_") + o.stringSuffix, o.defValue);
                };
            })(translations[i]);
        }
        return ret;
    };
}