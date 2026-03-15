function registerCheckboxValueHandlers(scope, thisParent, thisPage, parameters, styleData, stylesEnablingRules, webKitRenderCorrection) {
    var onSettingValue;
    var bIsBoolean = null;
    let _methods = {
        "checkboxClicked": function (e) {
            if (!scope.dataValues[parameters.screenId] || !scope.dataValues[parameters.screenId][parameters.SVGReferenceId])
                return;
            let enabled = !onSettingValue && scope.dataValues[parameters.screenId][parameters.SVGReferenceId] && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood && !thisParent.hasClass(disabledClass) && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isWritable && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isUserWritable;
            if (enabled) {
                if (onSettingValue)
                    return;
                if (!(parameters.screenId in scope.dataValues) || !(parameters.SVGReferenceId in scope.dataValues[parameters.screenId]))
                    return;
                if (!scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood)
                    return;

                var val = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].value.toLowerCase();
                if (bIsBoolean == null)
                    bIsBoolean = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].bIsBoolean;
                if (!parameters.IsThreeState) {
                    if (val === 'true' || val === '1')
                        val = 0;
                    else
                        val = 1;
                }
                else {
                    if (bIsBoolean) {
                        if (val === 'true')
                            val = undefined;
                        if (val === 'undefined')
                            val = false;
                        if (val === 'false')
                            val = true;
                    }
                    else {
                        var v = parseInt(val, 10);
                        val = v < 2 ? v + 1 : 0;
                    }
                }

                scope.corehubconnection.invoke("SetValue", parameters.SVGReferenceId, String(val), parameters.screenId, false)
                    .then((ret) => {
                        onSettingValue = false;
                    })
                    .catch(err => {
                        console.error(err.toString());

                        onSettingValue = false;
                        scope.$broadcast(onError, { ex: err, value: "Error setting value : " + err.toString() + " for item : " + parameters.SVGItemId });
                    });
            }
        },
        "updateControlValue": function () {
            onSettingValue = true;
            try {
                if (!scope.dataValues[parameters.screenId] || !scope.dataValues[parameters.screenId][parameters.SVGReferenceId])
                    return;
                if (scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood) {
                    if (bIsBoolean == null)
                        bIsBoolean = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].bIsBoolean;
                    var val = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].value.toLowerCase();
                    if (stylesEnablingRules.ON(val, bIsBoolean))
                        styleData.activateStyleON();
                    else if (stylesEnablingRules.OFF(val, bIsBoolean))
                        styleData.activateStyleOFF();
                    else if (stylesEnablingRules.NULL(val, bIsBoolean))
                        styleData.activateStyleNULL();
                }
            }
            finally {
                onSettingValue = false;
            }
        }
    }

    return (function (_methods) {
        var drls = [];
        if (isFirefox()) {
            var onContainerShown = scope.$on("containerShown" + parameters.screenId, function (e, value) {
                var innerSvg = buttoncontainer.find("svg");
                if (innerSvg.length > 0) {
                    innerSvg.remove();
                    setTimeout(function () {
                        innerSvg.appendTo(buttoncontainer);
                    }, 1);
                }
            });
            drls.push(onContainerShown);
        }

        _methods.updateControlValue();

        var drl = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
            applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId);
            _methods.updateControlValue();
        });
        drls.push(drl);
        styleData.eventsTarget.on('mouseup keyup', _methods.checkboxClicked);
        return drls;
    })(_methods);
}