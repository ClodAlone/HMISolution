
var WebHMIApp = angular.module(WEBHMIAPP, ['dx']);
var currentRoot = ROOT;
var fontsAlreadyLoading = [];
var localstorageZoomKey;
var localstorageLastScreenKey;
var bZoomInit;
var demoModePopup;
var timerDemoCountDown;
var demoCountDown;
var lastActive = [];
var dotNotDestroyView = [];
var toDestroyView = [];
var layoutBarsHiddenScreens = [];
var lastViewType;
var lastOption;
var lastParentPath;
var activeView;
var intervalUpdateDestroyView;
var registerScreenDataUpdate;
var delayUnloadMSecsMap = {};
var layoutBars = [];
var bFirstConnection;
var translationRegexCache = {};
var popupOpened = [];
var currentAutologoffTimeout = 0;
var waitInactive;

parent.postMessage("WebHMIReady", "*");

function handleLoad(e) {
    console.log('Loaded import: ' + e.target.href);
}
function handleError(e) {
    console.log('Error loading import: ' + e.target.href);
}

(function ($) {
    $.event.special.destroyed = {
        remove: function (o) {
            if (o.handler) {
                o.handler();
            }
        }
    };
})(jQuery);

function clearInactiveTimeout() {
    if (waitInactive)
        clearTimeout(waitInactive);
    waitInactive = undefined;
}

if (!Modernizr.canvas)
    DevExpress.ui.dialog.alert('HTML5 support is required !');

function getUrlVars() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
    });
    return vars;
}

window.addEventListener('contextmenu', event => event.preventDefault());

WebHMIApp.controller(MAINCONTROLLER, ['$scope', '$compile', 
    function ($scope, $compile) {

        $scope.$watch('notconnected', function (v) {

            if (v === true) {

                $scope.busyMessage = i18next.t("Lost connection...Reconnecting");
                $scope.busyVisible = true;
            }
            else if (v === false) {
                if (typeof bFirstConnection == "undefined")
                    bFirstConnection = true;
                else if (bFirstConnection)
                    bFirstConnection = false;
                if (!bFirstConnection) {
                    //location.reload();
                    CleanAppData();
                }
                $scope.busyVisible = false;
                $scope.restoreDefaultBusyMessage();
            }
        });

        initPlugin($scope, $compile);
        initBusyIndicator($scope);

        $scope.bIsMobile = false;
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            $scope.bIsMobile = true;
        }

        $scope.biOS = false;
        if (['iPad Simulator', 'iPhone Simulator', 'iPod Simulator', 'iPad', 'iPhone', 'iPod', 'MacIntel'].includes(navigator.platform)
            // iPad on iOS 13 detection
            || (navigator.userAgent.includes("Mac") && "ontouchend" in document)) {
            $scope.biOS = true;
        }

        $scope.$on(onError, function (ev, arg) {
            DevExpress.ui.notify(arg.value, "error", 6000);
        });

        $scope.showMesageBox = function (errorText, title = null) {
            if (title == null)
                title = i18next.t1("ErrorText");
            DevExpress.ui.dialog.alert(errorText, title);
        };
        $scope.showConfirmBox = function (errorText, title) {
            return DevExpress.ui.dialog.confirm(errorText, title);
        };

        var loginPath = './scripts/core/login.js';
        require([loginPath], function (login) {
        });
        $scope.showLogin = function () {

            if ($scope.configurationSettings.enableUserManager) {
                require([loginPath], function (login) {
                    login.show($scope);
                });
            }
        };

        $scope.showNumericPad = function (scope, ispassword, min, max, defaultValue, decimalDigits) {

            return showNumericPad(scope, min, max, ispassword, defaultValue, decimalDigits);
        };
        $scope.showAlphanumericPad = function (scope, ispassword, defaultValue, max) {

            return showAlphanumericPad(scope, ispassword, defaultValue, max);
        };

        $scope.activateView = function (viewtype, option) {
            attachView($scope, currentRoot, viewtype, option);
        };
        $scope.activateLastView = function () {
            if (lastActive.length > 0) {
                var lastview = lastActive.pop();
                attachView($scope, currentRoot, lastview.view, lastview.option, true);
            }
        };
        $scope.activatePopup = function (viewtype, option) {
            attachPopup($scope, viewtype, option);
        };
        $scope.showMainPage = function () {
            showMainPage($scope);
        };

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
                    loadPath: 'locales/{{lng}}/{{ns}}.json',
                    crossDomain: false
                },
                nsSeparator: false,
                keySeparator: false,
                pluralSeparator: false,
                contextSeparator: false
            }, function (err, t) {

                $scope.busyVisible = true;
                $scope.$apply();

                jqueryI18next.init(i18next, $);

                initSignalR($scope);

                $scope.$watch('corehubready', function (v) {

                    if ($scope.corehubready) {
                        $scope.corehubconnection.invoke("GetConfiguration", $scope.bIsMobile, new Date().getTimezoneOffset())
                            .then((ret) => {

                                if (Modernizr.indexeddb) {

                                    var deregisterListener = $scope.$on(onindexedDBReady, function (e, value) {

                                        deregisterListener();

                                        SetCurrentTheme($scope, ret.projectTheme);

                                        DevExpress.ui.notify(i18next.t("Welcome to WebHMI"), "success", 2000);

                                        StartApp($scope, ret);
                                    });

                                    initIndexedDBSupported($scope, ret.projectTitle);
                                }
                                else {
                                    DevExpress.ui.notify(i18next.t('IndexedDBNotSupported'), "error", 5000);
                                    StartApp($scope, ret);
                                }
                            })
                            .catch(err => {
                                console.error(err.toString());
                                $scope.busyVisible = false;
                                $scope.$apply();
                                $scope.$broadcast(onError, { ex: err, value: "Error connecting : " + err.toString() });
                            });
                    }
                });
            });
    }]);

function CleanAppData() {
    lastActive = [];
    dotNotDestroyView = [];
    toDestroyView = [];
    lastViewType = undefined;
    lastOption = undefined;
    lastParentPath = undefined;
    activeView = undefined;
    intervalUpdateDestroyView = undefined;
    delayUnloadMSecsMap = {};
    layoutBars = [];
    layoutBarsHiddenScreens = [];
    $.each(toDestroyView, function (index, item) {
        item.viewpage.remove();
        toDestroyView.splice(index, 1);
    });
    demoModePopup = undefined;
    $(ROOTAppBars).empty();
    $(ROOT).empty();
}

function SetCurrentTheme($scope, projectTheme) {
    $scope.theme = { isLight: false };

    switch (projectTheme) {
        case "None":
        case "VS2017Dark":
        case "TouchlineDark":
        case "Blend":
            DevExpress.ui.themes.current("generic.dark");
            $scope.theme.lightIcons = true;
            break;
        default:
            DevExpress.ui.themes.current("generic.light");
            $scope.theme.isLight = true;
            $scope.theme.lightIcons = false;
            break;
    }
}

function getThemeColors($scope) {
    return {
        "background": $scope.theme.isLight ? "#FAFAFA" : "#363636",
        "foreground": $scope.theme.isLight ? "#363636" : "#FAFAFA"
    };
}

function activateUserFromUrl($scope) {

    var user = getUrlVars()["user"];
    if (user) {

        var psw = getUrlVars()["psw"];
        if (psw) {

            $scope.username = user;
            $scope.password = psw;
            $scope.corehubconnection.invoke("LoginUser", $scope.username, $scope.password, i18next.language)
                .then((ret) => {
                    if (ret.status === 0)
                        $scope.showMesageBox(i18next.t('LoginFailed'));
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
                    showMainPage($scope, true);
                })
                .catch(err => {
                    console.error(err.toString());
                    $scope.$broadcast(onError, { ex: err, value: "Error connecting : " + err.toString() });
                    showMainPage($scope, true);
                 });

            return true;
        }
    }

    return false;

    function onPasswordExpired() {
        require(['./scripts/core/passwordExpired.js'], function (passwordExpired) {
            passwordExpired.show($scope);
        });
    }

    function onLoginSuccessful(ret) {
        $scope.currentUser = {
            name: $scope.username, level: ret.level,
            mask: ret.mask, role: ret.role, signature: ret.signature
        };
    }
}

function showMainPage($scope, startup) {

    if ($scope.configurationSettings.forceMainPageAR) {

        $scope.activateView('arView');
        return;
    }

    var page = getUrlVars()["page"];
    if (page) {
        var options = { name: page };
        var parameterfile = getUrlVars()["parameter"];
        if (!parameterfile) {
            $scope.activateView('screenView', JSON.stringify(options));
            return;
        }
        options.parameter = parameterfile;
        $scope.corehubconnection.invoke("SetDefaultParameterFile", parameterfile)
            .then((ret) => {

            })
            .catch((err) => {
                console.error(err.toString());
                $scope.$broadcast(onError, { ex: err, value: "Error setting default URL parameters : " + err.toString() });
            })
            .finally(() => {
                $scope.activateView('screenView', JSON.stringify(options));
            });
        return;
    }

    if (startup && $scope.configurationSettings.restoreLastOpenScreenAndZoom) {

        var lastoption = localStorage.getItem(localstorageLastScreenKey);
        if (lastoption) {
            try {
                var optobject = JSON.parse(lastoption);
                optobject.isStartupRestoreScreen = true;
                $scope.activateView('screenView', JSON.stringify(optobject));
                return;
            }
            catch (err) {}
        }
    }

    switch ($scope.configurationSettings.startType) {

        case 0 /*MainScreen*/:
            var options = { name: $scope.configurationSettings.startupScreen };
            $scope.activateView('screenView', JSON.stringify(options));
            break;
        case 1 /*TilePage*/:
            $scope.activateView('tileView');
            break;
        case 2 /*GeoPage*/:
            $scope.activateView('mapView');
            break;
        case 3 /*GalleryPage*/:
            $scope.activateView('tileView');
            break;
    }
}

function StartApp($scope, ret) {

    $scope.configurationSettings = ret;
    $scope.dataValues = {};

    if (ret.requireLogin) {
        $scope.showLogin();
    }

    var bActivateUserFromUrl = activateUserFromUrl($scope);
    initLanguages($scope, bActivateUserFromUrl);
    initCommands($scope);
    initAnimations($scope);
    initStyles($scope);
    if ($scope.configurationSettings.preloadedToolboxComponents.length > 0)
        $scope.preloadToolbox($scope);

    $scope.corehubconnection.on("userLoggedIn", (user, id, autoLogoffTimeout, culture) => {

        clearInactiveTimeout();
        currentAutologoffTimeout = autoLogoffTimeout;
        
        if (currentAutologoffTimeout > 0) {
            var logoutFn = function () {
                $scope.corehubconnection.invoke("LogoutUser", $scope.currentLanguage);
            };
            waitInactive = setTimeout(logoutFn, currentAutologoffTimeout * 1000);

            document.onmousemove = document.mousedown = document.mouseup = document.onkeydown = document.onkeyup = document.focus = function () {
                if (currentAutologoffTimeout == 0 || $scope.currentUser == null)
                    return;
                clearTimeout(waitInactive);
                waitInactive = setTimeout(logoutFn, currentAutologoffTimeout * 1000);
            };
        }

        if (culture)
            $scope.changeLanguage(culture);
    });

    $scope.corehubconnection.on("userLoggedOut", (user, id) => {
        $scope.currentUser = null;
        $scope.$apply();

        clearInactiveTimeout();

        if (ret.requireLogin) {
            $scope.showLogin();
        }
    });

    $scope.corehubconnection.on("executeCommands", (commandListString) => {

        try {
            var commands = JSON.parse(commandListString);

            jQuery.each(commands, function (index, item) {
                if ($scope.canExecuteCommand(item)) {
                    $scope.executeCommand(item);
                }
                else {
                    setTimeout(function () {
                        if ($scope.canExecuteCommand(item)) 
                            $scope.executeCommand(item);
                    }, 500);
                }
            });
        }
        catch (err) {
            console.error(err.toString());
        }
    });
    
    function startDemoModeCountDown() {
        demoCountDown = 5;
        demoModePopup.option({
            "toolbarItems[0].options.text": demoCountDown,
            "toolbarItems[0].options.disabled": true
        });
        timerDemoCountDown = setInterval(function () {
            if (--demoCountDown > 0)
                demoModePopup.option("toolbarItems[0].options.text", demoCountDown);
            else {
                clearInterval(timerDemoCountDown);
                demoModePopup.option({
                    "toolbarItems[0].options.text": i18next.t("OkText"),
                    "toolbarItems[0].options.disabled": false
                });
            }
        }, 1000);
    }

    function onShowDemoMode() {
        if (!demoModePopup) {
            var popupContainer = $("<div>").appendTo($(ROOT));
            demoModePopup = popupContainer.dxPopup({
                contentTemplate: function () {
                    return $("<div>").append("<p>" + i18next.t("DemoModeMessage") + "</p>");
                },
                width: 'auto',
                height: 'auto',
                showTitle: true,
                title: i18next.t("DemoModeTitle"),
                visible: false,
                dragEnabled: false,
                closeOnOutsideClick: false,
                showCloseButton: false,
                position: {
                    my: "center",
                    at: "center",
                    of: window
                },
                onShowing: function (e) {
                    startDemoModeCountDown();
                },
                toolbarItems: [
                    {
                        widget: "dxButton",
                        toolbar: "bottom",
                        location: "center",
                        options: {
                            disabled: true,
                            text: i18next.t("OkText"),
                            onClick: function (e) {
                                demoModePopup.hide();
                            }
                        }
                    }]
            }).dxPopup("instance");
        }
        if (!demoModePopup.option("visible"))
            demoModePopup.show();
    }

    //if (ret.inDemoMode)
    //    onShowDemoMode();
    $scope.corehubconnection.on("showDemoMode", onShowDemoMode);

    if (typeof window.orientation !== 'undefined') {
        var dxMenu = $("#menu").dxMenu({
            adaptivityEnabled: true,
            hideSubmenuOnMouseLeave: false,
            displayExpr: "name",
            onItemClick: function (data) {
            }
        }).dxMenu("instance");
    }

    
    if (ret.topScreen || ret.bottomScreen || ret.leftScreen ||
        ret.rightScreen || ret.appbartopScreen || ret.appbarbottomScreen) {

        var rootAppBar = jQuery('<div/>', {
            id: ROOTAppBarsID
        });

        currentRoot = ROOTAppBars;

        if (ret.topScreen) {
            addScreenBar($scope, ret.topScreen, 'ui-layout-north', 'topScreen', ret.topScreenHeight, true);
            rootAppBar.css("top", ret.topScreenHeight + "px");
        }
        if (ret.bottomScreen)
            addScreenBar($scope, ret.bottomScreen, 'ui-layout-south', 'bottomScreen', ret.bottomScreenHeight, true);
        if (ret.topScreen || ret.bottomScreen) {
            var barsHeight = (ret.topScreen ? ret.topScreenHeight : 0) + (ret.bottomScreen ? ret.bottomScreenHeight : 0);
            var h = $(ROOT).height() - barsHeight;
            rootAppBar.css("height", h + "px");
            window.addEventListener("resize", function () {
                h = $(ROOT).height() - barsHeight;
                rootAppBar.css("height", h + "px");
            });
        }

        if (ret.leftScreen) {
            addScreenBar($scope, ret.leftScreen, 'ui-layout-west', 'leftScreen', ret.leftScreenWidth, false);
            rootAppBar.css("left", ret.leftScreenWidth + "px");
        }
        if (ret.rightScreen)
            addScreenBar($scope, ret.rightScreen, 'ui-layout-east', 'rightScreen', ret.rightScreenWidth, false);
        if (ret.leftScreen || ret.rightScreen) {
            var barsWidth = (ret.leftScreen ? ret.leftScreenWidth : 0) + (ret.rightScreen ? ret.rightScreenWidth : 0);
            var w = $(ROOT).width() - barsWidth;
            rootAppBar.css("width", w + "px");
            window.addEventListener("resize", function () {
                w = $(ROOT).width() - barsWidth;
                rootAppBar.css("width", w + "px");
            });
        }

        rootAppBar.appendTo($(ROOT));
    }

    if ($scope.biOS && (typeof window.orientation !== 'undefined')) {
        if (!isSafari()) {
            window.onresize = function () {
                if (activeView && lastViewType === "screenView") {
                    var screenLoadedId = activeView.find("svg").attr("id");
                    var ids = [screenLoadedId];
                    $.each(layoutBars, function (index, item) {
                        ids.push($(item).find("svg").attr("id"));
                    });
                    $.each(ids, function (index, item) {
                        $scope.$broadcast("iOSDeviceRotated" + item, { isLayoutBar: item != screenLoadedId });
                    });
                }
            }
        }
        else {
            window.onorientationchange = function () {
                if (activeView && lastViewType === "screenView") {
                    var screenLoadedId = activeView.find("svg").attr("id");
                    var ids = [screenLoadedId];
                    $.each(layoutBars, function (index, item) {
                        ids.push($(item).find("svg").attr("id"));
                    });
                    var afterOrientationChange = function () {
                        window.removeEventListener('resize', afterOrientationChange);
                        $.each(ids, function (index, item) {
                            $scope.$broadcast("iOSDeviceRotated" + item, { isLayoutBar: item != screenLoadedId });
                        });
                    };
                    window.addEventListener('resize', afterOrientationChange);
                }
            }
        }
    }

    if ($scope.configurationSettings.restoreLastOpenScreenAndZoom) {

        localstorageZoomKey = $scope.configurationSettings.projectTitle + 'LastZoom';
        localstorageLastScreenKey = $scope.configurationSettings.projectTitle + 'LastScreen';

        if (!isMobile.any()) {
            if (!isChrome() || isTouchDevice()) {
                var lastZoom = localStorage.getItem(localstorageZoomKey);
                if (lastZoom) {
                    try {
                        document.body.style.zoom = parseFloat(lastZoom);
                    } catch (err) { }
                }
            }
        }
        //else {
            //var lastZoom = localStorage.getItem(localstorageZoomKey);
            //if (lastZoom) {
            //    let viewportNode = document.querySelector("meta[name=viewport]");
            //    let content = 'width=device-width, initial-scale=' + lastZoom;
            //    viewportNode.setAttribute('content', content);
            //}
        //}

        var delayResize;
        var delayResizeTime = 250;
        if (!isTouchDevice()) {
            $(window).on('resize', function (e) {
                if (delayResize) {
                    clearTimeout(delayResize);
                    delayResize = undefined;
                }
                delayResize = setTimeout(function () {
                    delayResize = undefined;

                    var zoomlevel = 1;
                    try {
                        zoomlevel = window.detectZoom.device();
                    }
                    catch (err) {
                        try {
                            zoomlevel = window.visualViewport.scale;
                        }
                        catch (err) { }
                    }
                    localStorage.setItem(localstorageZoomKey, zoomlevel);
                }, delayResizeTime);
            });
        }
        else if (!isMobile.any()) {
            if (delayResize) {
                clearTimeout(delayResize);
                delayResize = undefined;
            }
            $(window.visualViewport).on('resize', function (e) {
                delayResize = setTimeout(function () {
                    delayResize = undefined;
                    if (!bZoomInit /*&& !isMobile.any()*/) {
                        try {
                            if (parseFloat(document.body.style.zoom) > 1)
                                document.body.style.zoom = 1;
                        } catch (err) { }
                    }
                    bZoomInit = true;
                    localStorage.setItem(localstorageZoomKey, window.visualViewport.scale);
                }, delayResizeTime);
            });
        }
    }
    else if (isMobile.iOS() && isChrome()) {
        document.querySelector('meta[name="viewport"]').setAttribute('content', "width=device-width, initial-scale=1, maximum-scale=1");
        if (window.visualViewport.scale !== 1)
            window.scrollTo(0, 0);
    }

    if (!bActivateUserFromUrl) 
        showMainPage($scope, true);
    $('#imgBackground').remove();
}

function addScreenBar($scope, screenName, className, id, size, isHeight) {

    var iDiv = jQuery('<div/>', {
        id: id,
        class: className
    }).appendTo($(ROOT));

    layoutBars.push(iDiv);

    if (isHeight)
        iDiv.css("height", size + "px");
    else
        iDiv.css("width", size + "px");

    var options = { name: screenName, isdocked: true, isLayoutBar: true };
    attachView($scope, iDiv, 'screenView', JSON.stringify(options), false, true);
}

function hideLayoutBars(rootAppBars) {
    if (layoutBars.length == 0 || rootAppBars.hasClass("hiddenBar")) {
        return;
    }
    $.each(layoutBars, function (index, elem) {
        $(elem).hide();
    });
    rootAppBars.attr("originalstyle", rootAppBars.attr("style"));
    rootAppBars.attr("style", "").addClass("hiddenBar");
}

function showLayoutBars(rootAppBars) {
    if (layoutBars.length == 0 || !rootAppBars.hasClass("hiddenBar"))
        return;

    $.each(layoutBars, function (index, elem) {
        $(elem).show();
    });
    if (rootAppBars.attr("originalstyle"))
        rootAppBars.attr("style", rootAppBars.attr("originalstyle"));
    rootAppBars.removeClass("hiddenBar");
}

function attachView($scope, parent, viewType, option, activatingLast, isdocked) {
    if (!registerScreenDataUpdate)
        registerScreenDataUpdate = $scope.$on(onUpdateScreenSettings, function (e, screenData) {
            if (layoutBars.length > 0) {
                var rootAppBars = $(ROOTAppBars);
                if (screenData.hideLayoutScreens) {
                    hideLayoutBars(rootAppBars);
                    if (layoutBarsHiddenScreens.indexOf(screenData.name) === -1)
                        layoutBarsHiddenScreens.push(screenData.name);
                }
                else
                    showLayoutBars(rootAppBars);
            }
            if (screenData.keepAlwaysInMemory && !isNotDestroyable(screenData.name, screenData.parameter))
                dotNotDestroyView.push({ name: screenData.name, parameter: screenData.parameter });
            else
                delayUnloadMSecsMap[screenData.name] = screenData.delayUnloadSecs * 1000;
            if (!screenData.isdocked) {
                if (lastActive.length > 0 && lastParentPath) {
                    try {
                        var elemToOverride = lastActive[lastActive.length - 1];
                        var temp = JSON.parse(elemToOverride.option);
                        if (!isAbsolutePath(temp.name)) {
                            temp.name = lastParentPath;
                            elemToOverride.option = JSON.stringify(temp);
                        }
                    }
                    catch { }
                }
                lastParentPath = screenData.fullPath;
            }
        });

    if (!isdocked) {
        if (!activatingLast) {
            if (lastViewType) {
                lastActive.push({
                    view: lastViewType, option: lastOption
                });
                while (lastActive.length > maxBackHistoryCount)
                    lastActive.shift();
            }
        }
    }

    if (!isdocked) {
        if (option && lastOption && viewsCompare(option, lastOption, viewType, lastViewType))
            return;

        if ($scope.configurationSettings.restoreLastOpenScreenAndZoom && viewType === "screenView")
            localStorage.setItem(localstorageLastScreenKey, option);

        var foundHidden = findHiddenView(viewType, option);
        if (foundHidden) {
            if (layoutBars.length > 0) {
                var hiddenBarsIndex = layoutBarsHiddenScreens.indexOf(JSON.parse(foundHidden.option).name);
                var rootAppBars = $(ROOTAppBars);
                if (hiddenBarsIndex !== -1)
                    hideLayoutBars(rootAppBars);
                else
                    showLayoutBars(rootAppBars);
            }
            var index = toDestroyView.indexOf(foundHidden);
            toDestroyView.splice(index, 1);

            var view = foundHidden.viewpage.appendTo(parent);
            $(view).show();

            if (isFirefox())
                $scope.$broadcast("containerShown" + view.find("svg").attr("id"));

            if ($scope.biOS && (typeof window.orientation !== 'undefined') && viewType === "screenView") {
                var currentOrientation = $(window).height() > $(window).width() ? "Portrait" : "Landscape";
                if (view.attr("currentOrientation") !== currentOrientation) {
                    var screenLoadedId = view.find("svg").attr("id");
                    var ids = [screenLoadedId];
                    $.each(layoutBars, function (index, item) {
                        ids.push($(item).find("svg").attr("id"));
                    });
                    $.each(ids, function (index, item) {
                        $scope.$broadcast("iOSDeviceRotated" + item, { isLayoutBar: item != screenLoadedId, bForce: true });
                    });
                }
            }

            if (activeView) {
                $(activeView).hide();
                toDestroyView.push({
                    view: lastViewType, option: lastOption,
                    viewpage: activeView,
                    lastTime: Date.now()
                })

                if (!intervalUpdateDestroyView)
                    intervalUpdateDestroyView = setInterval(function () {
                        delayDestroyView();
                        }, $scope.configurationSettings.screenDelayUnloadMSecs);
            }
            activeView = view;
            lastViewType = viewType;
            lastOption = option;
            return;
        }
    }

    $scope.busyVisible = true;
    $scope.$apply();
    try {
        var page = $scope.loadHtmlOnDemand(viewType, true, false, option);

        var view = page.appendTo(parent);

        if (!isdocked) {
            if (activeView) {
                $(activeView).hide();
                toDestroyView.push({
                    view: lastViewType, option: lastOption,
                    viewpage: activeView,
                    lastTime: Date.now()
                })

                if (!intervalUpdateDestroyView)
                    intervalUpdateDestroyView = setInterval(function () {
                        delayDestroyView();
                        }, $scope.configurationSettings.screenDelayUnloadMSecs);
            }
            activeView = view;
            lastViewType = viewType;
            lastOption = option;
        }
    }
    catch (e) {

        if (e === true) {

            var deregisterListener = $scope.$on(onloadedHtmlOnDemand + viewType, function (e, value) {

                page = $scope.loadHtmlOnDemand(viewType, true, false, option);

                deregisterListener();

                var view = page.appendTo(parent);

                if (!isdocked) {
                    if (activeView) {
                        $(activeView).hide();
                        toDestroyView.push({
                            view: lastViewType, option: lastOption,
                            viewpage: activeView,
                            lastTime: Date.now()
                        })

                        if (!intervalUpdateDestroyView)
                            intervalUpdateDestroyView = setInterval(function () {
                                delayDestroyView();
                                }, $scope.configurationSettings.screenDelayUnloadMSecs);
                    }
                    activeView = view;
                    lastViewType = viewType;
                    lastOption = option;
                }
            });
        }
        else
            throw e;
    }
}

function equalsEmpty(a, b) {
    return a === b || (undefinedOrNull(a) && undefinedOrNull(b));
}

function undefinedOrNull(a) {
    return a === undefined || a === null;
}

function isNotDestroyable(name, parameter) {

    for (var i = 0; i < dotNotDestroyView.length; ++i) {

        if (equalsEmpty(dotNotDestroyView[i].name, name) &&
            equalsEmpty(dotNotDestroyView[i].parameter, parameter))
            return true;
    }

    return false;
}

function findHiddenView(viewType, option) {

    if (!option)
        return;

    for (var i = 0; i < toDestroyView.length; ++i) {
        if (viewsCompare(option, toDestroyView[i].option, toDestroyView[i].view, viewType))
            return toDestroyView[i];
    }
}

function viewsCompare(v1, v2, v1type, v2type) {
    try {
        v1 = JSON.parse(v1);
        v2 = JSON.parse(v2);
    }
    catch (e) {
        console.error(e);
        return false;
    }
    return equalsEmpty(v1type, v2type) &&
        equalsEmpty(v1.name, v2.name) &&
        equalsEmpty(v1.parameter, v2.parameter);
}

function delayDestroyView() {

    if (toDestroyView.length == 0) {

        if (intervalUpdateDestroyView) {
            clearInterval(intervalUpdateDestroyView);
            intervalUpdateDestroyView = undefined;
        }
        return;
    }

    var anydestroy = false;
    for (var i = 0; i < toDestroyView.length; ++i) {

        if (toDestroyView[i].option) {

            var arrayparameters = JSON.parse(toDestroyView[i].option);

            if (!isNotDestroyable(arrayparameters.name, arrayparameters.parameter)) {

                anydestroy = true;
                var destroy = false;
                var msecs = delayUnloadMSecsMap[arrayparameters.name]
                if (msecs) {
                    var dt = toDestroyView[i].lastTime + msecs;
                    destroy = dt < Date.now();
                }
                else
                    destroy = true;

                if (destroy) {

                    toDestroyView[i].viewpage.remove();
                    cleanLayoutBarsHiddenScreen(toDestroyView[i]);
                    toDestroyView.splice(i, 1);
                    break;
                }
            }
        }
        else {

            anydestroy = true;
            toDestroyView[i].viewpage.remove();
            cleanLayoutBarsHiddenScreen(toDestroyView[i]);
            toDestroyView.splice(i, 1);
            break;
        }
    }

    if (!anydestroy) {
        clearInterval(intervalUpdateDestroyView);
        intervalUpdateDestroyView = undefined;
    }
}

function cleanLayoutBarsHiddenScreen(destroyingView) {
    if (destroyingView.option) {
        var name = JSON.parse(destroyingView.option).name;
        var screenIndex = layoutBarsHiddenScreens.indexOf(name);
        if (screenIndex !== -1)
            layoutBarsHiddenScreens.splice(screenIndex, 1);
    }
}

function attachPopupWithParameters($scope, viewType, page, parameters, popupKey, bOpenAtCoords) {
    showPopup({
        $scope: $scope,
        id: viewType,
        content: page,
        showtitle: true,
        showclose: true,
        dragenabled: true,
        resizeenabled: !parameters.contextPopup,
        titletxt: parameters.titletxt,
        w: parameters.width,
        h: parameters.height,
        x: parameters.x,
        y: parameters.y,
        avoidCentering: bOpenAtCoords,
        noshading: !parameters.modal,
        closeonoutsideclick: parameters.contextPopup,
        noshowhidetransition: parameters.contextPopup,
        notScrollable: parameters.notScrollable,
        disableanimation: bOpenAtCoords,
        onContentReadyFunc: function (popupContainer, jparent) {
            var popupIndex = popupOpened.findIndex(function (elem) { return elem.key == popupKey });
            if (popupIndex !== -1) {
                popupOpened[popupIndex].jparent = jparent;
            }
        },
        onHiddenFunc: function (jparent) {
            var popup = popupOpened.filter(function (elem) { return elem.key == popupKey }).splice(-1);
            if (popup.length == 1)
                popupOpened.splice(popupOpened.indexOf(popup), 1);
        }
    });
}

function attachPopup($scope, viewType, option) {

    var parameters = JSON.parse(option);

    var popupKey = viewType + "|" + parameters.name + "|" + parameters.parameter;
    var popupIndex = popupOpened.findIndex(function (elem) { return elem.key == popupKey });
    if (popupIndex !== -1) {
        if (!parameters.contextPopup || !popupOpened[popupIndex].jparent)
            return;
        $(popupOpened[popupIndex].jparent).dxPopup("instance").hide();
    }
    popupOpened.push({ "key": popupKey });

    var bOpenAtCoords = !isNaN(parameters.x) && !isNaN(parameters.y);

    $scope.busyVisible = true;
    $scope.$apply();
    try {
        var page = $scope.loadHtmlOnDemand(viewType, true, false, option);

        attachPopupWithParameters($scope, viewType, page, parameters, popupKey, bOpenAtCoords);
    }
    catch (e) {

        if (e === true) {

            var deregisterListener = $scope.$on(onloadedHtmlOnDemand + viewType, function (e, value) {

                page = $scope.loadHtmlOnDemand(viewType, true, false, option);

                deregisterListener();

                attachPopupWithParameters($scope, viewType, page, parameters, popupKey, bOpenAtCoords);
            });
        }
        else
            throw e;
    }
}

var setUniqueId = function () {
    var counter = 0;
    return function (elem, prefix) {
        prefix = prefix || "defaultPrefix";
        var theId;
        while (document.getElementById(theId = prefix + counter++));
        $(elem).attr("id", theId);
        return theId;
    };
}();

var padString = function (str, finalLength,  bFillChar, bRight) {
    var ret = String(str);
    while (ret.length < finalLength) {
        if (!bRight)
            ret = bFillChar + ret;
        else
            ret = ret + bFillChar;
    }
    return ret;
};

function lowercaseFirstLetter(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}

var updateThresholdElementColor = function (data, element, thresholds, severityColorMap) {
    if (!element)
        return;

    var findColorsBySeverity = function () {
        if (data.severity in severityColorMap)
            return severityColorMap[data.severity];

        var finalThreshold = thresholds[0];
        $(thresholds).each(function (i, e) {
            if (parseInt(e.ThresholdValue, 10) > parseInt(data.severity, 10))
                return false;
            finalThreshold = e;
        });
        var colorsObj = {
            WaitAckONBack: finalThreshold.ThresholdColor,
            WaitAckONFore: finalThreshold.ThresholdForeColor,
            WaitAckOFFBack: finalThreshold.ThresholdOffColor,
            WaitAckOFFFore: finalThreshold.ThresholdOffForeColor,
            WaitResetONBack: finalThreshold.ThresholdAckColor,
            WaitResetONFore: finalThreshold.ThresholdAckForeColor,
            WaitResetOFFBack: finalThreshold.ThresholdOffAckColor,
            WaitResetOFFFore: finalThreshold.ThresholdOffAckForeColor
        };
        severityColorMap[data.severity] = colorsObj;
        return colorsObj;
    };

    var backcolor = "";
    var forecolor = "";
    var thresoldColors = findColorsBySeverity();
    //console.log("ALARM DATA:", data);
    var isActive = !data.enabledState.includes("Inactive");
    if (isActive && data.needsAcknoledge) {
        backcolor = thresoldColors.WaitAckONBack;
        forecolor = thresoldColors.WaitAckONFore;
    }
    else if (isActive) {
        backcolor = thresoldColors.WaitResetONBack;
        forecolor = thresoldColors.WaitResetONFore;
    }
    else if (data.needsAcknoledge) {
        backcolor = thresoldColors.WaitAckOFFBack;
        forecolor = thresoldColors.WaitAckOFFFore;
    }
    else {
        backcolor = thresoldColors.WaitResetOFFBack;
        forecolor = thresoldColors.WaitResetOFFFore;
    }
    fastdom.measure(function () {
        var isBlinking = $(element).hasClass("blinkingRow");
        fastdom.mutate(function () {
            $(element).attr("thresholdColor", backcolor);
            if (!isBlinking) {
                $(element).css({
                    'background': backcolor,
                });
            }
            $(element).css('color', forecolor);
        });
    });
};

function addOrUpdateObjectProps(myobj, nestedProps, finalValue) {
    var lastNestedObj = myobj;
    for (var i = 0; i < nestedProps.length; i++) {
        if (!(nestedProps[i] in lastNestedObj))
            lastNestedObj[nestedProps[i]] = {};
        if (i === nestedProps.length - 1)
            lastNestedObj[nestedProps[i]] = finalValue;
        lastNestedObj = lastNestedObj[nestedProps[i]];
    }
}

// #region Date utils
function ticksToTimestamp(ticks) {
    return (parseInt(ticks, 10) - 621355968000000000) / 10000;
}

function getDayTicks(date) {
    return (date.getSeconds() + date.getMinutes() * 60 + date.getHours() * 3600) * 10000000;
}

function momentToDate(obj) {
    if (moment.isMoment(obj))
        obj = obj["_d"];
    return obj;
}
// #endregion

function getClientErrorText(serverError, clientErrorPrefix = "", separator = ": ") {
    var innerExceptionMsg = serverError.toString().split(": ").pop();
    var errorSplitted = innerExceptionMsg.split("|");
    var errorText = i18next.t(errorSplitted[0]);
    if (clientErrorPrefix)
        errorText = i18next.t(clientErrorPrefix) + separator + errorText;
    if (errorSplitted.length > 1)
        errorText += errorSplitted.splice(1).join(" - ");
    return errorText;
}

function notNumeric(val) {
    return $.trim(val) === "" || isNaN(val);
}

function sanitizedImageUrl(url) {
    return url.replace(/\\/g, '/').replace(/"/g, '\\"').replace(/\n/g, '\\n');
}

function getDecimalSeparator($scope, lang) {
    if (!lang)
        lang = navigator.language;
    const numberWithDecimalSeparator = 1.1;
    return Intl.NumberFormat(lang)
        .formatToParts(numberWithDecimalSeparator)
        .find(part => part.type === 'decimal')
        .value;
}

(function (mutateHandler) {
    var _pendingMutates = {};
    var _mutateDeferred = {};
    mutateHandler.preparePageMutate = function (pageId, svgnode, webKitRenderCorrection, controlsNo) {
        _pendingMutates[pageId] = { controlsNo: controlsNo, svgNode: svgnode, webKitRenderCorrection: webKitRenderCorrection, mutateCallbacks: [], scheduledMutates: [] };
        if (pageId in _mutateDeferred)
            _mutateDeferred[pageId].reject();
        _mutateDeferred[pageId] = new $.Deferred();
        return _mutateDeferred[pageId].promise();
    };
    mutateHandler.terminatePageMutate = function (pageId) {
        if (pageId in _pendingMutates) {
            try {
                $.each(_pendingMutates[pageId].scheduledMutates, function (index, item) {
                    fastdom.clear(item);
                });
            }
            catch { }
            delete _pendingMutates[pageId];
        }
        if (pageId in _mutateDeferred) {
            _mutateDeferred[pageId].reject();
            delete _mutateDeferred[pageId];
        }
    };
    mutateHandler.scheduleMutate = function (pageId, func) {
        if (!(pageId in _pendingMutates)) {
            console.error("Mutations not scheduled for the page ID:", pageId);
            return;
        }
        _pendingMutates[pageId].mutateCallbacks.push(func);
        if (_pendingMutates[pageId].mutateCallbacks.length == _pendingMutates[pageId].controlsNo) {
            var mutateID = fastdom.mutate(function () {
                $.each(_pendingMutates[pageId].mutateCallbacks, function (index, item) { item(); });
                _pendingMutates[pageId].mutateCallbacks = [];
                _mutateDeferred[pageId].resolve();
            });
            _pendingMutates[pageId].scheduledMutates.push(mutateID);
        }
    }
})(window.mutateHandler = window.mutateHandler || {});

function getGroupSeparator($scope, lang) {
    if (!lang)
        lang = navigator.language;
    const numberWithDecimalSeparator = 1000;
    return Intl.NumberFormat(lang)
        .formatToParts(numberWithDecimalSeparator)
        .find(part => part.type === 'group')
        .value;
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

function invokeDelayedAction(action, periodMS = 0) {
    var startMS;
    function checkExecuteAction(nowMS) {
        if (startMS == undefined)
            startMS = nowMS;
        let timeSinceStart = nowMS - startMS;
        if (timeSinceStart < periodMS)
            requestAnimationFrame(checkExecuteAction);
        else {
            action();
            bInvoking = false;
            startMS = undefined;
        }
    }
    requestAnimationFrame(checkExecuteAction);
}

function normalizeDataValue(dataValue, bInvariant = true) {
    var ret = { dataValue: bInvariant ? dataValue.invariantValue : dataValue.value, bNormalized: false };
    if (!dataValue.bIsString) {
        if (dataValue.value.toLowerCase() === "false") {
            ret.bNormalized = true;
            ret.dataValue = 0;
        }
        else if (dataValue.value.toLowerCase() === "true") {
            ret.bNormalized = true;
            ret.dataValue = 1;
        }
    }
    return ret;
}

function normalizeBoolean(value) {
    var floatVal = parseFloat(value);
    if (!isNaN(floatVal))
        return floatVal;
    var strVal = $.trim(value.toLowerCase());
    if (strVal === "true")
        return 1;
    else if (strVal === "false")
        return 0;
    return value;
}

function toFixedNumber(num, digits, base) {
    var pow = Math.pow(base || 10, digits); return Math.round(num * pow) / pow;
}

function radian(ux, uy, vx, vy) {
    var dot = ux * vx + uy * vy;
    var mod = Math.sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
    var rad = Math.acos(dot / mod);
    if (ux * vy - uy * vx < 0.0) {
        rad = -rad;
    }
    return rad;
}

function setObjectBorder(cssStyleObject, parameters, thisPage, mainContainer) {
    if (!parameters || !cssStyleObject)
        return;

    var bMustSetNonSolidBorderBrush = false;
    if (parameters.BorderThickness && (parameters.BorderBrush.Color || parameters.BorderBrush.Source)) {
        if (parameters.BorderThickness.Top > 0 || parameters.BorderThickness.Right > 0 || parameters.BorderThickness.Bottom > 0 || parameters.BorderThickness.Left > 0) {
            cssStyleObject["border-width"] = parameters.BorderThickness.Top + "px " + parameters.BorderThickness.Right + "px " + parameters.BorderThickness.Bottom + "px " + parameters.BorderThickness.Left + "px";
            cssStyleObject["border-style"] = "solid";
            if (parameters.IsSolidColorBorder)
                cssStyleObject["border-color"] = parameters.BorderBrush.Color;
            else
                bMustSetNonSolidBorderBrush = true;
        }
        else
            cssStyleObject["border"] = "none";
    }
    else
        cssStyleObject["border"] = "none";

    if (parameters.CornerRadius) {
        cssStyleObject["border-radius"] = parameters.CornerRadius.TopLeft + "px" + " " + parameters.CornerRadius.TopRight + "px" + " " + parameters.CornerRadius.BottomRight + "px" + " " + parameters.CornerRadius.BottomLeft + "px";
        if (bMustSetNonSolidBorderBrush && thisPage && mainContainer)
        {
            var borderBrush = parameters.BorderBrush.Color ? parameters.BorderBrush.Color : "url(\"" + sanitizedImageUrl(parameters.BorderBrush.Source) + "\")";
            fastdom.measure(function () {
                var mainContainerHeight = mainContainer.height();
                fastdom.mutate(function () {
                    thisPage.css("padding", cssStyleObject["border-width"]);
                    mainContainer.css("height", mainContainerHeight - (parameters.BorderThickness.Top + parameters.BorderThickness.Bottom));
                    mainContainer.after("<div style='background:" + borderBrush + "; position: absolute; top: 0px; bottom: 0px; left: 0px; right: 0px; z-index: -1; border-radius: " + cssStyleObject["border-radius"] + ";'></div>");
                });
            })
            cssStyleObject["border-width"] = 0;
        }
    }
    else if (bMustSetNonSolidBorderBrush)
        cssStyleObject["border-image"] = (parameters.BorderBrush.Color ? parameters.BorderBrush.Color : "url(\"" + sanitizedImageUrl(parameters.BorderBrush.Source) + "\")") + " " + parameters.BorderThickness.Top + " " + parameters.BorderThickness.Right + " " + parameters.BorderThickness.Bottom + " " + parameters.BorderThickness.Left + " stretch stretch";
}

// svg : [A | a] (rx ry x-axis-rotation large-arc-flag sweep-flag x y)+
//conversion_from_endpoint_to_center_parameterization
//sample :  svgArcToCenterParam(200,200,50,50,0,1,1,300,200)
// x1 y1 rx ry φ fA fS x2 y2
function svgArcToCenterParam(x1, y1, rx, ry, phi, fA, fS, x2, y2) {
    var cx, cy, startAngle, deltaAngle, endAngle;
    var PIx2 = Math.PI * 2.0;

    if (rx < 0) {
        rx = -rx;
    }
    if (ry < 0) {
        ry = -ry;
    }
    if (rx == 0.0 || ry == 0.0) { // invalid arguments
        throw Error('rx and ry can not be 0');
    }

    var s_phi = Math.sin(phi);
    var c_phi = Math.cos(phi);
    var hd_x = (x1 - x2) / 2.0; // half diff of x
    var hd_y = (y1 - y2) / 2.0; // half diff of y
    var hs_x = (x1 + x2) / 2.0; // half sum of x
    var hs_y = (y1 + y2) / 2.0; // half sum of y

    // F6.5.1
    var x1_ = c_phi * hd_x + s_phi * hd_y;
    var y1_ = c_phi * hd_y - s_phi * hd_x;

    // F.6.6 Correction of out-of-range radii
    //   Step 3: Ensure radii are large enough
    var lambda = (x1_ * x1_) / (rx * rx) + (y1_ * y1_) / (ry * ry);
    if (lambda > 1) {
        rx = rx * Math.sqrt(lambda);
        ry = ry * Math.sqrt(lambda);
    }

    var rxry = rx * ry;
    var rxy1_ = rx * y1_;
    var ryx1_ = ry * x1_;
    var sum_of_sq = rxy1_ * rxy1_ + ryx1_ * ryx1_; // sum of square
    if (!sum_of_sq) {
        throw Error('start point can not be same as end point');
    }
    var coe = Math.sqrt(Math.abs((rxry * rxry - sum_of_sq) / sum_of_sq));
    if (fA == fS) { coe = -coe; }

    // F6.5.2
    var cx_ = coe * rxy1_ / ry;
    var cy_ = -coe * ryx1_ / rx;

    // F6.5.3
    cx = c_phi * cx_ - s_phi * cy_ + hs_x;
    cy = s_phi * cx_ + c_phi * cy_ + hs_y;

    var xcr1 = (x1_ - cx_) / rx;
    var xcr2 = (x1_ + cx_) / rx;
    var ycr1 = (y1_ - cy_) / ry;
    var ycr2 = (y1_ + cy_) / ry;

    // F6.5.5
    startAngle = radian(1.0, 0.0, xcr1, ycr1);

    // F6.5.6
    deltaAngle = radian(xcr1, ycr1, -xcr2, -ycr2);
    while (deltaAngle > PIx2) { deltaAngle -= PIx2; }
    while (deltaAngle < 0.0) { deltaAngle += PIx2; }
    if (fS == false || fS == 0) { deltaAngle -= PIx2; }
    endAngle = startAngle + deltaAngle;
    while (endAngle > PIx2) { endAngle -= PIx2; }
    while (endAngle < 0.0) { endAngle += PIx2; }

    var outputObj = { /* cx, cy, startAngle, deltaAngle */
        cx: cx,
        cy: cy,
        startAngle: startAngle,
        deltaAngle: deltaAngle,
        endAngle: endAngle,
        clockwise: (fS == true || fS == 1)
    }

    return outputObj;
}

var isMobile = {
    Windows: function () {
        return /IEMobile/i.test(navigator.userAgent);
    },
    Android: function () {
        return /Android/i.test(navigator.userAgent);
    },
    BlackBerry: function () {
        return /BlackBerry/i.test(navigator.userAgent);
    },
    iOS: function () {
        return /iPhone|iPad|iPod/i.test(navigator.userAgent);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Windows());
    }
};

var isTouchDevice = function () {
    return (('ontouchstart' in window) ||
        (navigator.maxTouchPoints > 0) ||
        (navigator.msMaxTouchPoints > 0));
}

var isChrome = function () {
    return (/Chrome/i.test(navigator.userAgent) || navigator.userAgent.match('CriOS'));
}

var isSafari = function () {
    return navigator.vendor && navigator.vendor.indexOf('Apple') > -1 &&
        navigator.userAgent &&
        navigator.userAgent.indexOf('CriOS') === -1 &&
        navigator.userAgent.indexOf('FxiOS') === -1;
};

var isFirefox = function () {
    return navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
}

var isEventBound = function (control, type /*, fn*/) {
    var data = jQuery._data(control, 'events');

    if (data === undefined || data.length === 0) {
        return false;
    }
    if (!data[type])
        return false;

    return true; //return (-1 !== $.inArray(fn, data[type]));
};

i18next.t1 = function (str, defValue) {
    if (!str || str.indexOf("{") === -1)
        return i18next.t(str, defValue);
    let matches;
    if (str in translationRegexCache)
        matches = translationRegexCache[str];
    else {
        matches = Array.from(str.matchAll(concatTranslationsRegexp));
        translationRegexCache[str] = matches;
    }
    if (matches.length == 0)
        return i18next.t(str, defValue);
    var finalStr = str;
    var replacedPart = "";
    $(matches).each(function (i, match) {
        let prefix;
        if (i == 0)
            prefix = finalStr.substr(0, match.index);
        else
            prefix = finalStr.substr(0, finalStr.indexOf(match[0]));
        let suffix = str.substr(match.index + match[0].length);
        if (i18next.exists(match[1]))
            replacedPart = prefix + i18next.t(match[1]);
        else
            replacedPart = prefix + match[0];
        finalStr = replacedPart + suffix;
    });
    return finalStr;
}

function urlReferenceReplace(urlReference, screenId) {
    var ret = urlReference;
    try {
        ret = urlReference.replace(/url\(#(.*?)\)/ig, "url(#" + screenId + "$1)");
    }
    catch { }
    return ret;
}

function getBrowserLangISO3166() {
    var re = /^(?:(en-GB-oed|i-ami|i-bnn|i-default|i-enochian|i-hak|i-klingon|i-lux|i-mingo|i-navajo|i-pwn|i-tao|i-tay|i-tsu|sgn-BE-FR|sgn-BE-NL|sgn-CH-DE)|(art-lojban|cel-gaulish|no-bok|no-nyn|zh-guoyu|zh-hakka|zh-min|zh-min-nan|zh-xiang))$|^((?:[a-z]{2,3}(?:(?:-[a-z]{3}){1,3})?)|[a-z]{4}|[a-z]{5,8})(?:-([a-z]{4}))?(?:-([a-z]{2}|\d{3}))?((?:-(?:[\da-z]{5,8}|\d[\da-z]{3}))*)?((?:-[\da-wy-z](?:-[\da-z]{2,8})+)*)?(-x(?:-[\da-z]{1,8})+)?$|^(x(?:-[\da-z]{1,8})+)$/i;
    var reg = re.exec(navigator.language);
    var ret = reg[5];
    if (undefinedOrNull(ret))
        ret = !undefinedOrNull(reg[3]) ? reg[3].toUpperCase() : "US";
    return ret;
}

function transformMatrixToScale(node) {
    var obj = jQuerize(node);
    var x = 1;
    var y = 1;
    transformMatrix = obj.attr("transform");
    try {
        let matrix = transformMatrix.match(/.*?scale\((.*?)\)/)[1].replace(/[^0-9\-\.\s]/g, "").split(" ");
        x = parseFloat(matrix[0], 10);
        y = parseFloat(matrix[1], 10);
    }
    catch { }

    return {
        x: isNaN(x) ? 1 : x,
        y: isNaN(y) ? 1 : y
    }
}

function transformMatrixToTranslate(node) {
    var obj = jQuerize(node);
    var transformMatrix = obj.css("-webkit-transform") ||
        obj.css("-moz-transform") ||
        obj.css("-ms-transform") ||
        obj.css("-o-transform") ||
        obj.css("transform");

    var x = 0;
    var y = 0;
    if (transformMatrix === "none") {
        transformMatrix = obj.attr("transform") || "none";
        let matrix = transformMatrix.replace(/[^0-9\-.\s]/g, '').split(' ');
        x = parseFloat(matrix[0], 10);
        y = parseFloat(matrix[1], 10);
    }
    else {
        let matrix = transformMatrix.replace(/[^0-9\-.,]/g, '').split(',');
        x = parseFloat(matrix[12] || matrix[4], 10);
        y = parseFloat(matrix[13] || matrix[5], 10);
    }

    return {
        x: isNaN(x) ? 0 : x,
        y: isNaN(y) ? 0 : y
    }
}

function jQuerize(node) {
    return node instanceof jQuery ? node : $(node);
}

function isInFittedParent(node) {
    var elem = jQuerize(node);
    return elem.closest(".fullSizeSVG").length > 0;
}

function safelyRemoveFobj($sourceNode, $targetNode, scope, webKitRenderCorrection) {
    var parentOpacity = $sourceNode.attr("opacity");
    if (scope.biOS && parentOpacity)
        $targetNode.css("opacity", parentOpacity);

    var tr = { x: 0, y: 0 };
    if (scope.biOS) {
        $.each($sourceNode.parents("g[transform]"), function (i, gr) {
            if (webKitRenderCorrection)
                $(gr).removeAttr("transform");
            else if (gr.transform.baseVal[0].type == 2) {
                tr.x += gr.transform.baseVal[0].matrix.e;
                tr.y += gr.transform.baseVal[0].matrix.f;
            }
        });
    }

    fastdom.mutate(function () {
        try {
            var svgTransform = $sourceNode[0].transform.baseVal[0];
            if (svgTransform.type == 4) {
                $targetNode[0].setAttribute("transform", $sourceNode[0].attributes['transform'].value);
                //$sourceNode[0].transform.baseVal.removeItem(0);
            }
        }
        catch (e) { }
        $sourceNode.remove();
    });
    updateControlSecurity($sourceNode[0], $targetNode[0]);
    return tr;
}

function webkitMoveRotateTransform(thisParent, thisPage, container) {
    fastdom.mutate(function () {
        try {
            var svgTransform = thisParent[0].transform.baseVal[0];
            if (svgTransform.type == 4) {
                thisPage.css("overflow", "visible");
                thisParent[0].transform.baseVal.removeItem(0);
                jQuerize(container).css("transform", "rotate(" + svgTransform.angle + "deg)");
            }
        }
        catch (e) { }
    });
}

function getMinMaxControlValues(scope, parameters, bTagHasDynamicLimits, bTagHasEU) {
    var minValue;
    var maxValue;
    if (!bTagHasDynamicLimits && !bTagHasEU) {
        minValue = parameters.MinValue;
        maxValue = parameters.MaxValue;
    }
    else if (bTagHasDynamicLimits) {
        if (!parameters.TagMinValue)
            minValue = parameters.MinValue;
        else if (!scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId])
            minValue = parameters.MinValue;
        if (!parameters.TagMaxValue)
            maxValue = parameters.MaxValue;
        else if (!scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId])
            maxValue = parameters.MaxValue;

        if (!bTagHasEU) {
            if (parameters.TagMinValue)
                if (scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId])
                    minValue = scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].invariantValue;
            if (parameters.TagMaxValue)
                if (scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId])
                    maxValue = scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].invariantValue;
        }
        else if (scope.dataValues[parameters.screenId][parameters.SVGReferenceId]) {
            if (parameters.TagMinValue)
                if (scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId])
                    minValue = Math.max(scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].invariantValue, scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeLow);
            if (parameters.TagMaxValue)
                if (scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId])
                    maxValue = Math.min(scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].invariantValue, scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeHigh);
        }
    }
    else if (scope.dataValues[parameters.screenId][parameters.SVGReferenceId]) { //bTagHasEU && !bTagHasDynamicLimits
        if (parameters.UseEUnit) {
            minValue = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeLow;
            maxValue = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeHigh;
        }
        else {
            minValue = Math.max(parameters.MinValue, scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeLow);
            maxValue = Math.min(parameters.MaxValue, scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeHigh);
        }
    }
    return { "min": parseFloat(minValue), "max": parseFloat(maxValue) };
}

function setOpacity(scope, thisParent, thisPage) {
    fastdom.measure(function () {
        var parentOpacity = thisParent.attr("opacity");
        if (scope.biOS && parentOpacity) {
            fastdom.mutate(function () {
                thisPage.css("opacity", parentOpacity);
            });
        }
    });
}

function loadCustomFonts(fontFamilies) {
    $(fontFamilies).each(function (index, item) {
        if (fontsAlreadyLoading.indexOf(item) === -1) {
            fontsAlreadyLoading.push(item);
            var new_font = new FontFace(item, 'url(fonts/' + encodeURI(item) + '.ttf)');
            new_font.load().then(function (loaded_face) {
                document.fonts.add(loaded_face);
            }).catch(function (error) { });
        }
    });
}

function applyFontSettingList(elements, fontProperty) {
    var langFontOpts = buildFontOptions(fontProperty);
    fastdom.mutate(function () {
        for (var i = 0; i < elements.length; i++) {
            if (elements[i])
                elements[i].css(langFontOpts);
        }
    });
    return langFontOpts;
}

function buildFontOptions(fontProp, fontOpts) {
    if (!fontOpts)
        fontOpts = {};
    if (fontProp["FontFamily"])
        fontOpts["font-family"] = fontProp["FontFamily"];
    if (fontProp["FontSize"])
        fontOpts["font-size"] = parseFloat(fontProp["FontSize"]) + "px";
    if (fontProp["FontWeight"])
        fontOpts["font-weight"] = fontWeightConverter(fontProp["FontWeight"], true);
    if (fontProp["FontStyle"]) {
        var valfontStyle = fontProp["FontStyle"].toLowerCase();
        if (valfontStyle != "normal")
            fontOpts["font-style"] = valfontStyle;
    }
    return fontOpts;
}

function buildFontOptionsDevExtreme(fontProp, fontOpts) {
    if (!fontOpts)
        fontOpts = {};
    if (fontProp["FontFamily"])
        fontOpts["family"] = fontProp["FontFamily"];
    if (fontProp["FontSize"])
        fontOpts["size"] = fontProp["FontSize"];
    if (fontProp["FontWeight"])
        fontOpts["weight"] = fontWeightConverter(fontProp["FontWeight"], true);
    return fontOpts;
}

function fontWeightConverter(fontWeight, bNumericFontWeight) {
    switch (fontWeight.toLowerCase()) {
        case "thin":
        case "extralight":
            return bNumericFontWeight ? 200 : "lighter";
        case "light":
        case "normal":
            return bNumericFontWeight ? 400 : "normal";
        case "medium":
        case "semibold":
        case "bold":
            return bNumericFontWeight ? 600 : "bold";
        case "extrabold":
        case "black":
        case "extrablack":
            return bNumericFontWeight ? 800 : "bolder";
    }
    return bNumericFontWeight ? 400 : "normal";
};

function isAbsolutePath(path) {
    return /^([A-Za-z]+:(\/+|\\+)|\/)/.test(path);
}

function getGroupTranslate(node) {
    var obj = jQuerize(node);
    var ret = null;

    try {
        let matches = obj.attr("transform").match(/translate\(([0-9\.]+)\s+([0-9\.]+)/);
        if (matches !== null && matches.length > 2) {
            ret = {
                x: parseFloat(matches[1]),
                y: parseFloat(matches[2])
            };
        }
    }
    catch (err) { }
    return ret;
}

function getParentsGroupTranslate(fobj) {
    var parentGroups = fobj.parents("g[transform]");
    if (parentGroups.length == 0)
        return null;

    var ret = {
        x: 0,
        y: 0
    };
    $(parentGroups).each(function (i, g) {
        var tr = getGroupTranslate(g);
        if (tr != null) {
            ret.x += tr.x;
            ret.y += tr.y;
        }
    });
    return ret;
}

function getParentGroupScale(node) {
    var obj = jQuerize(node);
    var ret = null;

    try {
        var parentGroup = obj.parent("g").first();
        let matches = parentGroup.attr("transform").match(/scale\(([0-9\.]+)\s+([0-9\.]+)/);
        if (matches !== null && matches.length > 2) {
            ret = {
                group: parentGroup,
                x: parseFloat(matches[1]),
                y: parseFloat(matches[2])
            };
        }
    }
    catch (err) { }
    return ret;
}

function getWebkitOriginalXY(fobj) {
    return {
        x: parseFloat(typeof (fobj.attr("originalX")) !== "undefined" ? fobj.attr("originalX") : fobj.attr("x")),
        y: parseFloat(typeof (fobj.attr("originalY")) !== "undefined" ? fobj.attr("originalY") : fobj.attr("y"))
    }
}

function fixGroupedPositionWebkit(fobj, page, webKitRenderCorrection) {
    if (page.hasClass("notScaledGroup"))
        return;

    var fobj = jQuerize(fobj);
    var gx = null;
    var gy = null;
    var fobjX;
    var fobjY;
    var bFixPositioning = false;
    if (page.attr("gx") == undefined) {
        var gs = getParentGroupScale(fobj);
        if (gs) {
            var oc = getWebkitOriginalXY(fobj);
            fobjX = oc.x;
            fobjY = oc.y;
            var group = gs.group;

            $.each(group.find("foreignObject[groupscalingx]"), function (index, item) {
                var $item = $(item);
                var fx = $item == fobj ? fobjX : getWebkitOriginalXY($item).x;
                var fy = $item == fobj ? fobjY : getWebkitOriginalXY($item).y;
                if (gx == null || fx < gx)
                    gx = fx;
                if (gy == null || fy < gy)
                    gy = fy;
            });
            page.attr({ "gx": gx, "gy": gy, "groupScaleX": gs.x, "groupScaleY": gs.y });

            bFixPositioning = true;
        }
        else
            page.addClass("notScaledGroup");
    }
    else {
        var oc = getWebkitOriginalXY(fobj);
        fobjX = oc.x;
        fobjY = oc.y;
        gx = parseFloat(page.attr("gx"));
        gy = parseFloat(page.attr("gy"));

        bFixPositioning = true;
    }
    if (bFixPositioning) {
        page.css({
            "left": ((gx - fobjX) * (1 - parseFloat(page.attr("groupScaleX")))) * webKitRenderCorrection.getWidthFactor() + webKitRenderCorrection.centerLeftDelta,
            "top": ((gy - fobjY) * (1 - parseFloat(page.attr("groupScaleY")))) * webKitRenderCorrection.getHeightFactor() + webKitRenderCorrection.centerTopDelta
        });
    }
}

function fixGroupedPositionAndScale(fobj, page, popupparameter) {
    fobj = jQuerize(fobj);
    var parentGroup = fobj.parent("g");
    var bIsGrouped = parentGroup.length > 0;
    if (!bIsGrouped)
        return;

    var ret = getParentsGroupTranslate(fobj);
    var fobjX = parseFloat(fobj.attr("x")); 
    var fobjY = parseFloat(fobj.attr("y")); 
    if (ret == null)
        ret = {
            x: fobjX,
            y: fobjY
        };
    else {
        ret.x += fobjX;
        ret.y += fobjY
    }
    fobj.attr(ret);
    if (popupparameter) {
        var parentGTranslate = getGroupTranslate(parentGroup);
        fobj.attr("transform", "translate(-" + parentGTranslate.x + " -" + parentGTranslate.y + ")");
    }
    var gs = getParentGroupScale(fobj);
    if (gs) {
        page.css({
            "transform": "scale(" + gs.x + ", " + gs.y + ")",
            "transform-origin": + (-fobjX) + "px " + (-fobjY) + "px"
        });
    }
}

function overlaps(a, b) {
    let rect1 = a.getBoundingClientRect();
    let rect2 = b.getBoundingClientRect();
    let isInHorizontalBounds =
        rect1.x < rect2.x + rect2.width && rect1.x + rect1.width > rect2.x;
    let isInVerticalBounds =
        rect1.y < rect2.y + rect2.height && rect1.y + rect1.height > rect2.y;
    return isInHorizontalBounds && isInVerticalBounds;
}

function getTextWidth(text, elem) {
    // re-use canvas object for better performance
    const canvas = getTextWidth.canvas || (getTextWidth.canvas = document.createElement("canvas"));
    const context = canvas.getContext("2d");
    context.font = getCanvasFontSize(elem);
    const metrics = context.measureText(text);
    return metrics.width;

    function getCssStyle(element, prop) {
        return window.getComputedStyle(element, null).getPropertyValue(prop);
    }

    function getCanvasFontSize(el = document.body) {
        const fontWeight = getCssStyle(el, 'font-weight') || 'normal';
        const fontSize = getCssStyle(el, 'font-size') || '16px';
        const fontFamily = getCssStyle(el, 'font-family') || 'Times New Roman';

        return `${fontWeight} ${fontSize} ${fontFamily}`;
    }
}

function disableSVGControl(control, disable) {
    control = jQuerize(control);
    if (disable) {
        fastdom.measure(function () {
            var hasOldOpacity = control.is("[originalOpacity]");
            if (!hasOldOpacity)
                var oldOpacity = control.css("opacity");
            fastdom.mutate(function () {
                if (!hasOldOpacity)
                    control.attr("originalOpacity", oldOpacity);
                control.css("opacity", disabledControlOpacity);
            })
        });
    }
    else {
        fastdom.measure(function () {
            var oldOpacity = control.attr("originalOpacity");
            fastdom.mutate(function () {
                control.css("opacity", oldOpacity == undefined ? 1 : oldOpacity);
            });
        });
    }
}

function setTextWrappingOptions(btnContentCss, textWrapping, bHasNewLines, biOS) {
    switch (textWrapping) {
        case 0:
            btnContentCss["white-space"] = "break-spaces";
            break;
        case 1:
            btnContentCss["white-space"] = "nowrap";
            if (biOS)
                btnContentCss["overflow"] = "hidden";
            break;
        case 2:
            btnContentCss["white-space"] = "break-spaces";
            btnContentCss["overflow-wrap"] = "break-word";
            break;
    }
    if (textWrapping == 1 && bHasNewLines)
        btnContentCss["white-space"] = "break-spaces";
}

function applyTagQualityStyle(container, scope, screenId, SVGReferenceId, bApplySVGFilter = false, innerElement = null, bSkipBorder = false) {
    if (!SVGReferenceId || !container.length)
        return;

    var tagReference;
    var cssClass;
    if (bApplySVGFilter)
        cssClass = scope.biOS ? "badQualityOutline" : "badQualityFilter";
    else {
        if (scope.biOS) {
            if (container.hasClass("webKitRenderCorrection"))
                cssClass = "badQualityFilter" + (bSkipBorder ? "" : " badQualityBorder");
            else
                cssClass = "badQuality badQuality_iOS";
        }
        else
            cssClass = bSkipBorder ? "badQualityFilter" : "badQuality";
    }

    var styledElement = innerElement ? innerElement : container;
    if (bApplySVGFilter && scope.biOS && styledElement.is("g"))
        styledElement = styledElement.children().first();
    try {
        tagReference = scope.dataValues[screenId][SVGReferenceId];
    }
    catch {
        styledElement.addClass(cssClass);
        return;
    }
    if (tagReference == null)
        return;

    if (!tagReference.isGood)
        styledElement.addClass(cssClass);
    else
        styledElement.removeClass(cssClass);
}

(function (colorHelpers) {
    colorHelpers.hex2rgba = function (hex) {
        var c;
        if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
            c = hex.substring(1).split('');
            if (c.length == 3) {
                c = [c[0], c[0], c[1], c[1], c[2], c[2]];
            }
            c = '0x' + c.join('');
            return { "R": (c >> 16) & 255, "G": (c >> 8) & 255, "B": c & 255, "A": 1 };
        }
        else if (/^#([A-Fa-f0-9]{4}){1,2}$/.test(hex)) {
            c = hex.substring(1).split('');
            if (c.length == 4) {
                c = [c[0], c[0], c[1], c[1], c[2], c[2], c[3], c[3]];
            }
            c = '0x' + c.join('');
            return { "A": (c >> 24) & 255, "R": (c >> 16) & 255, "G": (c >> 8) & 255, "B": c & 255 };
        }
        throw new Error('hexToRgbA: Bad Hex: ' + hex);
    };
    colorHelpers.rgba2hex = function (rgba, bWithAlpha = true) {
        var hex = (rgba.R | 1 << 8).toString(16).slice(1) +
            (rgba.G | 1 << 8).toString(16).slice(1) +
            (rgba.B | 1 << 8).toString(16).slice(1);
        if (bWithAlpha)
            hex += (rgba.A | 1 << 8).toString(16).slice(1);

        return "#" + hex;
    };
    colorHelpers.interpolateColors = function (color1, color2, percentage, bWithAlpha = true) {
        color1 = colorHelpers.hex2rgba(color1);
        color2 = colorHelpers.hex2rgba(color2);
        var a1 = color1.A / 255.0;
        var r1 = color1.R / 255.0;
        var g1 = color1.G / 255.0;
        var b1 = color1.B / 255.0;

        var a2 = color2.A / 255.0;
        var r2 = color2.R / 255.0;
        var g2 = color2.G / 255.0;
        var b2 = color2.B / 255.0;

        var a3 = (a1 + (a2 - a1) * percentage) * 255;
        var r3 = (r1 + (r2 - r1) * percentage) * 255;
        var g3 = (g1 + (g2 - g1) * percentage) * 255;
        var b3 = (b1 + (b2 - b1) * percentage) * 255;
        return colorHelpers.rgba2hex({ "A": a3, "R": r3, "G": g3, "B": b3 }, bWithAlpha);
    };
}) (window.colorHelpers = window.colorHelpers || {});

(function (mathHelpers) {
    mathHelpers.isNum = function (args) {
        args = args.toString();
        if (args.length === 0) return false;
        for (var i = 0; i < args.length; i++) {
            if ((args.substring(i, i + 1) < "0" || args.substring(i, i + 1) > "9")
                && args.substring(i, i + 1) !== "." && args.substring(i, i + 1) !== "-") { return false; }
        }
        return true;
    };
    mathHelpers.variance = function (arr) {
        var len = 0;
        var sum = 0;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] === "") { }
            else if (!mathHelpers.isNum(arr[i])) {
                console.err("mathHelpers.variance: " + arr[i] + " is not number, Variance Calculation failed!");
                return 0;
            }
            else {
                len = len + 1;
                sum = sum + parseFloat(arr[i]);
            }
        }
        var v = 0;
        if (len > 1) {
            var mean = sum / len;
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] == "") { }
                else { v = v + (arr[i] - mean) * (arr[i] - mean); }
            }
            return v / len;
        }
        else { return 0; }
    };
    mathHelpers.median = function (arr) {
        arr.sort(function (a, b) { return a - b });
        var median = 0;
        if (arr.length % 2 == 1) {
            median = arr[(arr.length + 1) / 2 - 1];
        }
        else {
            median = (1 * arr[arr.length / 2 - 1] + 1 * arr[arr.length / 2]) / 2;
        }
        return median;
    };
    mathHelpers.standardDeviation = function (arr) {
        return Math.sqrt(mathHelpers.variance(arr));
    };
    mathHelpers.roundToSignificantDigits = function (value, decimalDigits) {
        if (notNumeric(decimalDigits))
            return value;

        let fvFactor = Math.pow(10, decimalDigits);
        return ((value + Number.EPSILON) * fvFactor / fvFactor).toFixed(decimalDigits);
    };
})(window.mathHelpers = window.mathHelpers || {});

(function (devExtremeHelpers) {
    devExtremeHelpers.getLabelsFormat = function (labelStringFormat) {
        if (labelStringFormat) {
            var digits = labelStringFormat.match(/.*?\.(0+)/);
            if (digits)
                return "#." + digits[1];
        }
        return null;
    };
    devExtremeHelpers.getCustomTicks = function (parameters, minValue, maxValue, minorIntervalCount, majorIntervalCount, bMinorTicks, majorCustomTicks) {
        var tickInterval;
        if (maxValue == minValue)
            return !bMinorTicks ? [] : [minValue, maxValue];
        if (majorIntervalCount == 0 || (bMinorTicks && minorIntervalCount == 0))
            return [];
        majortickInterval = (maxValue - minValue) / majorIntervalCount;
        tickInterval = majortickInterval;
        if (bMinorTicks) {
            var roundedInterval = majortickInterval / minorIntervalCount;
            if (roundedInterval > 0)
                tickInterval = roundedInterval;
        }
        var ticks = [minValue];
        var iteration = 0;
        var reduceTicks = false;
        if (!bMinorTicks) {
            if (parameters.ScaleOptions) {
                if (parameters.ScaleOptions.ScaleFactor < 1)
                    reduceTicks = true;
            }
        }
        for (var i = minValue; ticks[ticks.length - 1] < maxValue; i += tickInterval) {
            iteration++;
            if (reduceTicks && iteration % 2 === 0)
                continue;
            if (i >= maxValue)
                break;
            var tickValue = parseFloat(i.toFixed(2));
            if (!bMinorTicks || majorCustomTicks.indexOf(tickValue) === -1)
                ticks.push(tickValue);
        }
        if (!bMinorTicks)
            ticks.push(maxValue);
        else
            ticks.splice(0, 1);
        return ticks;
    }
})(window.devExtremeHelpers = window.devExtremeHelpers || {});

function updateControlSecurity(oldContainer, newContainer) {
    if (!window.currentEntitiesContext)
        return;

    for (var screenId in window.currentEntitiesContext) {
        var screenContext = window.currentEntitiesContext[screenId];
        var securityIndex = screenContext.controlInSecurity.indexOf(oldContainer);
        if (securityIndex !== -1) {
            screenContext.controlInSecurity[securityIndex] = newContainer;
            $(newContainer).hide();
        }
    }
}

function checkControlSetVisible(controlToShow) {
    if (!window.currentEntitiesContext)
        return;

    for (var screenId in window.currentEntitiesContext) {
        var screenContext = window.currentEntitiesContext[screenId];
        var entity = screenContext.entities.find(function (item) {
            var control = screenContext.svg.find('#' + item.SVGItemId);
            if (control.length <= 0)
                return false;
            var $controlNode = $(control[0].node);
            return controlToShow.node == control[0].node || ($controlNode.hasClass("skip_me") && controlToShow.node == $controlNode.children()[0]);
        });
        if (entity) {
            var zoomlevel = getZoomLevel();
            evaluateControlVisibility(screenContext.scope, screenContext.controlInSecurity, controlToShow, entity, zoomlevel);
        }
    }
}

function evaluateControlVisibility(scope, controlInSecurity, control, entity, zoomlevel) {
    control = Array.isArray(control) ? control[0].node : control.node;
    //var scope = angular.element(control.node).scope();
    var jqControl = $(control);
    var bControlHidden;
    var itemzoomLevel = Math.max(entity.ZoomLevelVisibilityX, entity.ZoomLevelVisibilityY);
    if (zoomlevel < itemzoomLevel) {
        if (controlInSecurity.indexOf(control) === -1)
            controlInSecurity.push(control);
        if (jqControl.hasClass(hiddenClass)) {
            jqControl.removeClass(hiddenClass);
        }
        jqControl.hide();
        bControlHidden = true;
    }
    else {
        var found = controlInSecurity.indexOf(control);
        if (found !== -1)
            controlInSecurity.splice(found, 1);
        if (!jqControl.hasClass(hiddenClass)) {
            jqControl.addClass(hiddenClass);
        }
        if (!jqControl.hasClass(animationHiddenClass))
            jqControl.show();
    }

    if (!bControlHidden && scope.configurationSettings.enableUserManager) {

        if (!hasAccessMask(scope, entity.ReadableAccessMask)) {
            if (controlInSecurity.indexOf(control) === -1)
                controlInSecurity.push(control);
            jqControl.hide();
        }
        else {

            var found = controlInSecurity.indexOf(control);
            if (found !== -1)
                controlInSecurity.splice(found, 1);
            if (!jqControl.hasClass(animationHiddenClass))
                jqControl.show();
        }
    }
}

function evaluateVisibilityZoom(controlToShow, bCheckSingleControl) {
    if (!window.currentEntitiesContext)
        return;

    for (var screenId in window.currentEntitiesContext) {
        var screenContext = window.currentEntitiesContext[screenId];
        var svg = screenContext.svg;
        var entities = screenContext.entities;

        var zoomlevel = getZoomLevel();
        jQuery.each(entities, function (index, item) {
            var control = svg.find('#' + item.SVGItemId);
            if (control.length > 0)
                evaluateControlVisibility(screenContext.scope, screenContext.controlInSecurity, control, item, zoomlevel);
        });
    }
}

function getZoomLevel() {
    var zoomlevel = 1;
    try {
        zoomlevel = window.detectZoom.device();
    }
    catch (err) {
        try {
            zoomlevel = window.visualViewport.scale;
        }
        catch (err) { }
    }
    return zoomlevel;
}

function buildStyleID(thisPage, thisParentId, screenName) {
    return (thisPage.attr("id") + thisParentId + screenName).replace(/\W+/g, "_");
}

function getKeepAspectRatioOffset(viewboxRatio) {
    var ret = { x: 0, y: 0 };
    var windowRatio = window.innerWidth / window.innerHeight;
    if (windowRatio < viewboxRatio) {
        var desiredHeight = window.innerWidth / viewboxRatio;
        ret.y = (window.innerHeight - desiredHeight) / 2;
    }
    else if (windowRatio > viewboxRatio) {
        var desiredWidth = window.innerHeight * viewboxRatio;
        ret.x = (window.innerWidth - desiredWidth) / 2;
    }
    return ret;
}

function setColumnOrdering(item, col) {
    if (item.SortIndex != -1 && item.ColumnOrder != 0) {
        col.sortIndex = item.SortIndex;
        col.sortOrder = item.ColumnOrder == 1 ? "asc" : "desc";
    }
}

function isTransparent(color) {
    var alpha = parseFloat(color.split(',')[3]);
    return alpha === 0;
}