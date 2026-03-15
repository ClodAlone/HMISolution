
var pendingLoading = 0;
var pendingRequests = new Array();

function loadNextPending($scope, $compile) {

    if (pendingRequests.length === 0)
        return;

    var el = pendingRequests[0];
    pendingRequests.splice(0, 1);

    $scope.$broadcast(onloadedHtmlOnDemand + el.t, el.t);
    setTimeout(function () {
        loadNextPending($scope, $compile);
    }, 50);
}

function initPlugin($scope, $compile) {

    $scope.preloadToolbox = function () {
        $.each($scope.configurationSettings.preloadedToolboxComponents, function (index, elem) {
            var templatetoload = elem.replace(".html", "");
            var linkid = templatetoload + 'link';
            var folder = toolboxPath;
            var filetoload = folder + elem;
            link = document.createElement('div');
            link.id = linkid;
            var queryName = '#' + linkid;
            var content = document.querySelector(queryName);
            if (content === null) {
                $(link).load(filetoload, function (responseText, textStatus, req) {
                    //var error = false;
                    if (textStatus === "error") {
                        //error = true;
                        $scope.showMesageBox(i18next.t('ErrorLoading') + ' : ' + filetoload);
                    }
                    else {
                        var scripts = link.getElementsByTagName("script");
                        for (var i = 0; i < scripts.length; ++i) {
                            var script = scripts[i];

                            $(script).remove();
                            var newscript = document.createElement('script');
                            newscript.type = 'text/javascript';
                            newscript.code = script.innerHTML;
                            link.children[0].append(newscript);
                        }
                    }
                    //if (!error)
                    //    $scope.$broadcast(onloadedHtmlOnDemand + templatetoload, templatetoload);
                });
                document.head.appendChild(link);
            }
        });
    };

    $scope.loadHtmlOnDemand = function (templatetoload, syscomponent, toolbox, option) {

        if (pendingLoading > 0) {
            pendingRequests[pendingRequests.length] = { t: templatetoload, s: syscomponent, tbx: toolbox, o: option };
            throw true;
        }

        var linkid = templatetoload + 'link';
        var queryName = '#' + linkid;
        var content = document.querySelector(queryName);
        if (content === null) {

            pendingLoading++;
            var folder = toolbox ? toolboxPath : syscomponent ? syscomponentsPath : componentsPath;
            var filetoload = folder + templatetoload + '.html';

            link = document.createElement('div');
            link.id = linkid;

            $(link).load(filetoload, function (responseText, textStatus, req) {

                var error = false;
                if (textStatus === "error") {
                    error = true;
                    $scope.showMesageBox(i18next.t('ErrorLoading') + ' : ' + filetoload);
                }
                else {
                    var scripts = link.getElementsByTagName("script");
                    for (var i = 0; i < scripts.length; ++i) {
                        var script = scripts[i];

                        script.remove();
                        var newscript = document.createElement('script');
                        newscript.type = 'text/javascript';
                        newscript.code = script.innerHTML;
                        link.children[0].append(newscript);
                    }

                    /*
                    var script = document.createElement('script');
                    script.type = 'text/javascript';
                    script.src = folder + templatetoload + '.js';
                    link.children[0].append(script);
                    */
                }

                pendingLoading--;
                if (!error)
                    $scope.$broadcast(onloadedHtmlOnDemand + templatetoload, templatetoload);
                else {
                    do {

                        var found = pendingRequests.findIndex(function (value) {
                            return value.t === templatetoload;
                        });
                        if (found !== -1)
                            pendingRequests.splice(found, 1);

                    } while (found !== -1);
                }
                if (pendingLoading === 0)
                    loadNextPending($scope, $compile);
            });

            /*
            link = document.createElement('link');
            link.rel = 'import';
            link.id = linkid;
            link.href = filetoload;
            link.setAttribute('async', 'true'); // make it async!

            var handler = function () {
                pendingLoading--;
                $scope.$broadcast(onloadedHtmlOnDemand + templatetoload, templatetoload);
                if (pendingLoading === 0)
                    loadNextPending($scope, $compile);
                document.removeEventListener('HTMLImportsLoaded', handler);
            };
            document.addEventListener('HTMLImportsLoaded', handler);

            link.onload = function (e) {
                pendingLoading--;
                $scope.$broadcast(onloadedHtmlOnDemand + templatetoload, templatetoload);
                if (pendingLoading === 0)
                    loadNextPending($scope, $compile);
                document.removeEventListener('HTMLImportsLoaded', handler);
            };
            link.onerror = function (ex) {
                pendingLoading--;
                $scope.$broadcast(onError, { e: ex, value: templatetoload });
                if (pendingLoading === 0)
                    loadNextPending($scope, $compile);
                document.removeEventListener('HTMLImportsLoaded', handler);
            };
            */

            document.head.appendChild(link);
            throw true;
        }

        try {
            // var control = content.import.querySelector('#' + templatetoload);
            var control = content.querySelector('#' + templatetoload);
            var refreshloaded = $(control).children().first();
            if ($(refreshloaded)[0].nodeName.toLowerCase() === 'img') {
                $(refreshloaded)[0].src = "";
            }

            var ret = $(control).clone(true, true);
            ret.detach();

            refreshloaded = ret.children().first();
            if ($(refreshloaded)[0].nodeName.toLowerCase() === 'img' && $(refreshloaded)[0].src.indexOf("?ver=") === -1) {
                var d = new Date();
                $(refreshloaded)[0].src = "cl.gif?ver=" + d.getTime();
            }

            var retval = $compile(ret)($scope);
            if (option)
                retval.data(dataparameters, option);
            return retval;
        }
        catch (ex) {
            $scope.$broadcast(onError, { e: ex, value: templatetoload });
        }
    };
}