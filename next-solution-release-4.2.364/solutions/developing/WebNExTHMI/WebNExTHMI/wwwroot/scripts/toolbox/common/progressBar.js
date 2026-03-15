function onProgressBarLoaded(obj) {
    var deregisterListener;
    var deregisterListenerMin;
    var deregisterListenerMax;

    var thisPage = $(obj).parent();
    thisPage.localize();
    var scope = thisPage.scope();
    if (!scope)
        return;
    var paratt = thisPage.data(dataparameters);
    if (!paratt)
        return;

    var thisParent = thisPage.parent();

    setOpacity(scope, thisParent, thisPage);

    var thisParentHeight = thisParent.attr("height");
    var thisParentWidth = thisParent.attr("width");
    var barStyle;
    var fillRect;
    var indicatorStartingY;
    var originalRectHeight;
    var originalRectWidth;
    var bTagHasDynamicLimits;
    var bTagHasEU;
    var valueDisplay;
    var euDisplay;
    var lastValue = 0;
    var bEUnitSet;

    $(obj).remove();

    var deregisterListener;
    var deregisterListenerActiveLanguage;
    $(thisPage).bind('destroyed', function () {
        if (deregisterListener)
            deregisterListener();
        if (deregisterListenerMin)
            deregisterListenerMin();
        if (deregisterListenerMax)
            deregisterListenerMax();
        if (deregisterListenerActiveLanguage)
            deregisterListenerActiveLanguage();
    });

    var parameters = JSON.parse(paratt);
    if (parameters.bFitInWindow)
        thisParent.css("overflow", "visible");

    if (parameters.FontFamily)
        loadCustomFonts([parameters.FontFamily]);

    var minmax = {
        "min": parameters.MinValue, "max": parameters.MaxValue
    };

    var container = thisPage.find("#progressbar");
    var outerContainer = thisPage.find("#progressbarcontainer");

    if (scope.biOS)
        webkitMoveRotateTransform(thisParent, thisPage, outerContainer);

    var isVertical = parameters.Orientation === 1;

    var mutateFunc = function () {
        if (parameters.TagMinValue && parameters.TagMinValue.SVGReferenceId) {
            deregisterListenerMin = scope.$on(onDataChanged + parameters.screenId + parameters.TagMinValue.SVGReferenceId, function (e, value) {
                if (scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].isGood) {
                    bTagHasDynamicLimits = true;
                    updateBarScale();
                }
            });
        }

        if (parameters.TagMinValue) {
            if (parameters.screenId in scope.dataValues && parameters.TagMinValue.SVGReferenceId in scope.dataValues[parameters.screenId] &&
                scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].isGood) {
                bTagHasDynamicLimits = true;
                updateBarScale();
            }
        }

        if (parameters.TagMaxValue && parameters.TagMaxValue.SVGReferenceId) {
            deregisterListenerMax = scope.$on(onDataChanged + parameters.screenId + parameters.TagMaxValue.SVGReferenceId, function (e, value) {
                if (scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].isGood) {
                    bTagHasDynamicLimits = true;
                    updateBarScale();
                }
            });
        }

        if (parameters.TagMaxValue) {
            if (parameters.screenId in scope.dataValues && parameters.TagMaxValue.SVGReferenceId in scope.dataValues[parameters.screenId] &&
                scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].isGood) {
                bTagHasDynamicLimits = true;
                updateBarScale();
            }
        }

        var styleSvg = SVG(scope.Styles[parameters.screenId]);
        barStyle = styleSvg.find('#' + parameters.StyleOFF);
        if (barStyle.length > 0)
            barStyle = barStyle[0]; //.clone();

        if (parameters.ShowValue) {
            valueDisplay = $('<div class="barValueDisplay" style="background: transparent; border: none; position: fixed; z-index: 1;">0</div>').appendTo(outerContainer);
            var cssEdits = {};
            if (parameters.FontFamily)
                cssEdits["font-family"] = parameters.FontFamily;
            if (parameters.FontWeight)
                cssEdits["font-weight"] = fontWeightConverter(parameters.FontWeight);
            if (parameters.FontSize)
                cssEdits["font-size"] = parameters.FontSize;
            if (parameters.FontStyle) {
                var fontStyle = parameters.FontStyle.toLowerCase();
                if (fontStyle !== "normal")
                    cssEdits["font-style"] = fontStyle;
            }
            if (parameters.Foreground.Color)
                cssEdits["color"] = parameters.Foreground.Color;
            if (isVertical) {
                cssEdits["transform"] = "translateY(-50%);";
                cssEdits["top"] = "50%";
                cssEdits["left"] = "40%";
                cssEdits["margin-top"] = (valueDisplay.height() / 2 * -1) + "px";
            }
            else {
                cssEdits["transform"] = "rotate(90deg) translateY(50%)";
                cssEdits["top"] = "35%";
                cssEdits["left"] = "50%";
                cssEdits["margin-left"] = (valueDisplay.width() / 2 * -1) + "px";
            }
            valueDisplay.css(cssEdits);
        }

        if (parameters.Opacity)
            outerContainer.css("opacity", parameters.Opacity);

        fillRect = $(barStyle.node).find("#PART_Indicator");
        indicatorStartingY = parseFloat(fillRect.attr("y"), 10);
        if (fillRect.length > 0) {
            fillRect = fillRect[0];
        }
        var fillRectSize = {
            w: $(fillRect).attr("width"),
            h: $(fillRect).attr("height")
        };

        if (parameters.SvgBackground)
            if (parameters.SvgBackground.Color) {
                var tagsBG = $(barStyle.node).find("[tag='BG']");
                for (var i = 0; i < tagsBG.length; i++) {
                    $(tagsBG[i]).css("fill", parameters.SvgBackground.Color);
                }
            }

        if (parameters.Animation != 1) {
            originalRectHeight = parseFloat(fillRectSize.h, 10);
            originalRectWidth = parseFloat(fillRectSize.w, 10);
            if (isVertical)
                $(fillRect).attr("height", 0);
            else
                $(fillRect).attr("width", 0);
        }
        else {
            $(fillRect).css({
                "transform-origin": "50% 50%",
                "transform-box": "fill-box"
            });
        }

        $(barStyle.node).appendTo(container).css({
            "position": "absolute",
            "left": 0,
            "top": 0,
            "width": thisParent.width(),
            "height": thisParent.height()
        });

        if (parameters.ShowEngeneeringUnit && (parameters.EngeneeringUnit || parameters.UseEUnit)) {
            var eUnitText = parameters.EngeneeringUnit ? parameters.EngeneeringUnit : "";
            euDisplay = $('<div class="barEUDisplay" style="background: transparent; white-space: nowrap; border: none; position: fixed; z-index: 1;">' + i18next.t1(eUnitText) + '</div>').appendTo(outerContainer);
            if (!bEUnitSet && eUnitText)
                styleEUnitText();

            deregisterListenerActiveLanguage = scope.$on(onActiveLanguageChanged, function (e, value) {
                if (euDisplay && eUnitText)
                    euDisplay.text(i18next.t1(eUnitText));
            });
        }

        onBarDataChanged();

        deregisterListener = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
            onBarDataChanged();
        });
    };

    mutateHandler.scheduleMutate(parameters.screenId, mutateFunc);

    function styleEUnitText() {
        bEUnitSet = true;
        var cssEdits = {};
        if (parameters.FontFamily)
            cssEdits["font-family"] = parameters.FontFamily;
        if (parameters.FontWeight)
            cssEdits["font-weight"] = fontWeightConverter(parameters.FontWeight);
        if (parameters.FontSize)
            cssEdits["font-size"] = parameters.FontSize;
        if (parameters.FontStyle) {
            var fontStyle = parameters.FontStyle.toLowerCase();
            if (fontStyle !== "normal")
                cssEdits["font-style"] = fontStyle;
        }
        if (parameters.Foreground.Color)
            cssEdits["color"] = parameters.Foreground.Color;
        if (isVertical) {
            cssEdits["transform"] = "scale(0.7) translateY(-50%)";
            cssEdits["top"] = "50%";
            cssEdits["left"] = (thisParentWidth - euDisplay.width() - 10) + "px";
            //cssEdits["margin-top"] = valueDisplay ? valueDisplay.css("margin-top") : (euDisplay.height() / 2 * -1) + "px";
        }
        else {
            cssEdits["transform"] = "scale(0.7) rotate(90deg) translateY(50%)";
            cssEdits["top"] = (thisParentHeight - euDisplay.width() - 10) + "px";
            cssEdits["left"] = "50%";
            //cssEdits["margin-left"] = valueDisplay ? valueDisplay.css("margin-left") : (euDisplay.width() / 2 * -1) + "px";
        }
        euDisplay.css(cssEdits);
    }

    function onBarDataChanged() {
        onSettingValue = true;
        try {
            var newValue = Number.NaN;
            if (parameters.screenId in scope.dataValues && parameters.SVGReferenceId in scope.dataValues[parameters.screenId] &&
                scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood) {
                var referenceObject = scope.dataValues[parameters.screenId][parameters.SVGReferenceId];
                newInvariantValue = referenceObject.invariantValue;
                newValue = referenceObject.value;
                var n = normalizeDataValue(referenceObject, false);
                if (n.bNormalized)
                    newValue = newInvariantValue = n.dataValue.toString();

                lastValue = parseFloat(newInvariantValue, 10);
                updateBarFillSize();

                if (!notNumeric(scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeLow)) {
                    bTagHasEU = true;
                    updateBarScale();
                }
                if (!notNumeric(scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeHigh)) {
                    bTagHasEU = true;
                    updateBarScale();
                }

                if (parameters.ShowValue && valueDisplay)
                    valueDisplay.text(newValue);

                if (euDisplay) {
                    eUnitText = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].displayName;
                    if (eUnitText) {
                        euDisplay.text(i18next.t1(eUnitText));
                        if (!bEUnitSet)
                            styleEUnitText();
                    }
                }
            }
        }
        finally {
            onSettingValue = false;
        }
    }

    function updateBarScale() {
        var mm = getMinMaxControlValues(scope, parameters, bTagHasDynamicLimits, bTagHasEU);

        if (minmax)
            if ((mm.min == minmax.min && mm.max == minmax.max) || isNaN(mm.min) || isNaN(mm.max))
                return;
        minmax = mm;

        updateBarFillSize();
    }

    function updateBarFillSize() {
        var valueChanged = false;
        if (lastValue < minmax.min) {
            lastValue = newValue = minmax.min;
            valueChanged = true;
        }
        if (lastValue > minmax.max) {
            lastValue = newValue = minmax.max;
            valueChanged = true;
        }
        if (valueChanged && parameters.ShowValue && valueDisplay) {
            valueDisplay.text(lastValue);
        }

        if (parameters.Animation == 1) { //rotation
            var newRectAngleDegrees = (lastValue - minmax.min) / (minmax.max - minmax.min) * 90;
            $(fillRect).css("transform", "rotate(" + newRectAngleDegrees + "deg)");
        }
        else {
            if (isVertical) {
                var newRectHeight = originalRectHeight / (minmax.max - minmax.min) * (lastValue - minmax.min);
                $(fillRect).attr({
                    "height": newRectHeight,
                    "y": indicatorStartingY + (originalRectHeight - newRectHeight)
                });
            }
            else
                $(fillRect).attr("width", originalRectWidth / (minmax.max - minmax.min) * (lastValue - minmax.min));
        }
    }
}