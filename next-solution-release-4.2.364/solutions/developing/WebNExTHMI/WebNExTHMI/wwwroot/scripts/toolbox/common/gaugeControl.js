function onGaugeControlLoaded(obj) {
    var deregisterListener;
    var deregisterListenerMin;
    var deregisterListenerMax;

    var thisPage = $(obj).parent();
    thisPage.localize();

    var scope = thisPage.scope();
    if (!scope)
        return;

    var paratt;
    var parameters;
    try {
        paratt = thisPage.data(dataparameters);
        parameters = JSON.parse(paratt);
    }
    catch {
        return;
    }

    $(obj).remove();
    var thisParent = thisPage.parent();
    var svgContainer = thisParent.closest("#svgcontainer")[0];
    var bDragging;
    var serverWriteTimeoutDefaultMS = scope.configurationSettings.serverInvokeMinFreqOnDrag;
    var writeTimeout;
    var nextWriteTimeout;
    var latestServerInvokeTime;
    var bDragNotifyingServer;
    var dragDetectTimeout;
    var latestDraggedValue;
    var gaugeSvgPoint;

    thisParent.css("overflow", "visible");

    var customFonts = [parameters.LabelFontSettings.FontFamily, parameters.ValueFontSettings.FontFamily, parameters.EngeneeringUnitFontSettings.FontFamily];
    if (parameters.FontSettingList)
        $.each(parameters.FontSettingList, function (index, item) {
            customFonts.push(item.FontFamily);
        });
    loadCustomFonts(customFonts);
    var dynamicFontElements = [];

    var interval;
    var deregisterListenerActiveLanguage;
    $(thisPage).bind('destroyed', function () {
        if (interval)
            clearInterval(interval);
        if (deregisterListener)
            deregisterListener();
        if (deregisterListenerMin)
            deregisterListenerMin();
        if (deregisterListenerMax)
            deregisterListenerMax();
        if (deregisterListenerActiveLanguage)
            deregisterListenerActiveLanguage();
        if (writeTimeout)
            clearTimeout(writeTimeout);
        if (nextWriteTimeout)
            clearTimeout(nextWriteTimeout);
        if (dragDetectTimeout)
            clearTimeout(dragDetectTimeout);
    });

    var floatingPointPos = parameters.ValueStringFormat.indexOf(invariantNumberDecimalSeparator);
    var floatingPointPosLabel = parameters.LabelStringFormat.indexOf(invariantNumberDecimalSeparator);
    var decimals = floatingPointPos == -1 ? 0 : parameters.ValueStringFormat.substring(floatingPointPos + 1).length;
    var labelDecimals = floatingPointPosLabel == -1 ? 0 : parameters.LabelStringFormat.substring(floatingPointPosLabel + 1).length;
    var decimalSeparator = getDecimalSeparator(scope);

    var parentHeight = parseFloat(thisParent.attr("height"));
    var parentWidth = parseFloat(thisParent.attr("width"));
    var gaugeSize = Math.min(parentHeight, parentWidth);
    var gaugeSvgScale = 1;

    var mainContainer = thisPage.find("#mainContainer");
    var container = thisPage.find("#gauge");
    if (scope.biOS)
        webkitMoveRotateTransform(thisParent, thisPage, mainContainer);

    var containerInner = thisPage.find("#gaugeInner");
    var bHasRangeBar = parameters.RangeBarVisible && !parameters.NeedleVisible;

    //setOpacity(scope, thisParent, thisPage);
    var parentOpacity = thisParent.attr("opacity");
    if (scope.biOS && parentOpacity)
        container.css("opacity", parentOpacity);

    var bTagHasEU;
    var bTagHasDynamicLimits;
    var bHasSVGBackground = parameters.GaugeBaseModel && scope.Styles && scope.Styles[parameters.screenId];
    var bHasRangeBarBackground = parseFloat(parameters.RangeBarBackground.Color.split(",")[3]) > 0;
    var baseModelName = parameters.GaugeBaseModel ? parameters.GaugeBaseModel.replace(/.*_/, "").toLowerCase() : "";
    var gaugeType = parameters.GaugeType.toLowerCase();

    var tickmarkLengthFactor = 6;
    var tickmarkWidthFactor = 1.5;

    var spindleStyleProp = parameters.IsFastGaugeControl ? parameters.CapType : parameters.SpindleCapPresentation;
    var spindleStyle;
    var spindleSize = { w: 0, h: 0, factor: 0 };
    if (scope.Styles && scope.Styles[parameters.screenId]) {
        if (!parameters.IsFastGaugeControl) {
            spindleSize.w = parseFloat(parameters.SpindleFactorWidth);
            spindleSize.h = parseFloat(parameters.SpindleFactorHeight);
            spindleSize.factor = Math.max(spindleSize.w, spindleSize.h) / gaugeSize;
        }
        else if (parameters.HasCap) {
            spindleSize.w = parseFloat(parameters.CapRadius);
            spindleSize.h = parseFloat(parameters.CapRadius);
            spindleSize.factor = Math.max(spindleSize.w, spindleSize.h) / gaugeSize;
        }
    }
    var bExpandedGaugeSize = !bHasSVGBackground && bHasRangeBar;
    var bIsQuarterType = gaugeType == "rightquarter" || gaugeType == "leftquarter";
    var innerGaugeScaleFactorX = bExpandedGaugeSize ? 1 : (parameters.IsFastGaugeControl ? 0.85 : (!bHasSVGBackground && !bIsQuarterType ? 0.85 : 0.77));
    var innerGaugeScaleFactorY = innerGaugeScaleFactorX;
    if (!bExpandedGaugeSize && (baseModelName == "leftquarter" || baseModelName == "rightquarter")) {
        innerGaugeScaleFactorX = 0.66;
        innerGaugeScaleFactorY = 0.7;
    }
    var rangeScaleFactor = 1;
    var gaugeEU;
    var gaugeValueDisplay;
    var gaugeValueDisplayTextbox;
    var minmax;
    var onSettingValue = false;
    var animationDuration = 1000;

    //var foreignObject = container.parent().parent();

    var webKitRenderCorrection = thisPage.hasClass("webKitRenderCorrection");
    if (webKitRenderCorrection)
        thisPage.css("will-change", "opacity");

    var viewboxRatio;
    if (webKitRenderCorrection && parameters.bKeepAspectRatio)
        viewboxRatio = parameters.parentViewbox.width / parameters.parentViewbox.height;

    if (parameters.ShowValue) {
        var bDisplayInit;
        var display = $('<div class="gaugeControlValueDisplay" style="pointer-events:none; background: transparent; width: 100%; border: none; position: ' + (scope.biOS && (parameters.IsFastGaugeControl || parameters.currentPopupId) && !webKitRenderCorrection ? "absolute" : "fixed") + ';"></div>');
        gaugeValueDisplay = display.dxTextBox({
            value: "?",
            readOnly: true,
            hint: i18next.t1(parameters.Tooltip),
            showSpinButtons: false,
            showClearButton: false,
            onContentReady: function (e) {
                if (bDisplayInit)
                    return;
                bDisplayInit = true;

                var component = e.component.element();
                el = component.find("input").eq(0);
                var displayCssOpt = {
                    "text-align": "center",
                    "background": "transparent",
                    "color": parameters.ValueForeground.Color,
                };
                if (parameters.ValueFontSettings.FontFamily)
                    displayCssOpt["font-family"] = parameters.ValueFontSettings.FontFamily;
                if (parameters.ValueFontSettings.FontSize)
                    displayCssOpt["font-size"] = parameters.ValueFontSettings.FontSize;
                if (parameters.ValueFontSettings.FontWeight)
                    displayCssOpt["font-weight"] = fontWeightConverter(parameters.ValueFontSettings.FontWeight);
                if (parameters.ValueFontSettings.FontStyle) {
                    var valfontStyle = parameters.ValueFontSettings.FontStyle.toLowerCase();
                    if (valfontStyle != "normal")
                        displayCssOpt["font-style"] = valfontStyle;
                }
                el.css(displayCssOpt);
                component.css({
                    "z-index": 3,
                });
            },
            placeholder: i18next.t1('ConnectingToServer')
        }).dxTextBox("instance");
    }

    var paramFontSize = parseInt(parameters.LabelFontSettings.FontSize, 10);
    var bFirstDrawn = false;
    var gaugeArcsCenter;
    var parentScreen = $("#" + parameters.screenId);
    var gaugeSvg;
    var majorCustomTicks = devExtremeHelpers.getCustomTicks(parameters, parseFloat(parameters.MinValue), parseFloat(parameters.MaxValue), parameters.MinorIntervalCount, parameters.MajorIntervalCount);

    var gaugeStartAngle = (360 - parameters.StartAngle) % 360;
    var gaugeEndAngle = parameters.EndAngle - parameters.StartAngle >= 360 ? ((360 - parameters.StartAngle) % 360) + 360 : (360 - parameters.EndAngle) % 360;
    var absAngle = Math.abs(gaugeEndAngle - gaugeStartAngle) % 360;
    if (absAngle == 0)
        absAngle = 360;
    
    var gaugeConfig = {
        animation: {
            enabled: true,
            duration: animationDuration
        },
        scale: {
            startValue: parameters.MinValue,
            endValue: parameters.MaxValue,
            tick: {
                color: parameters.MajorTickmarkFill.Color,
                length: Math.abs(parameters.MajorTickmarkFactorLength) * tickmarkLengthFactor * 2,
                width: parameters.MajorTickmarkFactorThickness * tickmarkWidthFactor * 0.7
            },
            minorTick: {
                color: parameters.MinorTickmarkFill.Color,
                visible: true,
                length: Math.abs(parameters.MinorTickmarkFactorLength) * tickmarkLengthFactor,
                width: parameters.MinorTickmarkFactorThickness * tickmarkWidthFactor * 0.7
            },
            customTicks: majorCustomTicks,
            customMinorTicks: devExtremeHelpers.getCustomTicks(parameters, parseFloat(parameters.MinValue), parseFloat(parameters.MaxValue), parameters.MinorIntervalCount, parameters.MajorIntervalCount, true, majorCustomTicks),
            allowDecimals: false,
            label: {
                overlappingBehavior: "none",
                indentFromTick: parameters.LabelOffset <= -30 ? -1 : 5,
                font: {
                    weight: fontWeightConverter(parameters.LabelFontSettings.FontWeight),
                    family: parameters.LabelFontSettings.FontFamily,
                    size: paramFontSize * (1 - (innerGaugeScaleFactorX + innerGaugeScaleFactorY) / 2) + paramFontSize,
                    color: (parameters.MajorIntervalCount > 1 || parameters.ShowFirstLabel) ? parameters.LabelForeground.Color : "transparent"
                },
                customizeText: function (arg) {
                    if (arg.valueText.charAt(0) === decimalSeparator)
                        return "0" + arg.valueText;
                    return labelDecimals <= 0 ? parseInt(arg.valueText, 10) : arg.valueText;
                }
            }
        },
        rangeContainer: {
            ranges: getRanges().ranges,
            backgroundColor: "transparent"
        },
        value: 0,
        subvalues: [0],
        geometry: {
            startAngle: gaugeStartAngle,
            endAngle: gaugeEndAngle,
        },
        valueIndicator: {
            type: "triangleNeedle",
            color: urlReferenceReplace(parameters.RangeBarVisible && !parameters.NeedleVisible && parameters.RangeBarFill ? parameters.RangeBarFill.Color : (parameters.NeedleVisible ? parameters.NeedleFill.Color : "transparent"), parameters.screenId),
            offset: parameters.RangeBarVisible && !parameters.NeedleVisible ? -4 : 10
        },
        subvalueIndicator: {
            type: "triangleMarker",
            color: parameters.MarkerFill.Color,
            length: parameters.MarkerVisible ? parameters.MarkerFactorHeight : 0,
            width: parameters.MarkerVisible ? parameters.MarkerFactorWidth : 0
        },
        onDrawn: function (e) {
            if (bFirstDrawn)
                return;
            bFirstDrawn = true;

            var el = e.component.element();

            if (!gaugeArcsCenter) {
                var needleHole = el.find(".dxg-spindle-hole");
                gaugeArcsCenter = {
                    x: parseFloat(needleHole.attr("cx")),
                    y: parseFloat(needleHole.attr("cy")),
                    bbox: needleHole[0].getBBox()
                };
                if (webKitRenderCorrection)
                    gaugeArcsCenter.rect = getGaugeSpindleCenterRect(e.component);
            }

            if (bHasRangeBar && el.dxCircularGauge("instance").option("valueIndicator.type") !== "rangebar") {
                bFirstDrawn = false;
                el.dxCircularGauge("instance").option({
                    "valueIndicator.type": "rangebar",
                    "valueIndicator.backgroundColor": parameters.RangeBarBackground.Color
                });
                return;
            }

            if (spindleStyleProp && scope.Styles && scope.Styles[parameters.screenId])
                applySpindleStyle(el);

            var rangeContainer = el.find("g.dxg-range-container")[0];
            var initialRangeContainerSize = rangeContainer.getBoundingClientRect();

            if (parameters.MarkerIsInteractive || parameters.NeedleIsInteractive || parameters.RangeBarIsInteractive) {
                container.on("mousedown touchstart", function (e) {
                    if (writeTimeout) {
                        clearTimeout(writeTimeout);
                        writeTimeout = null;
                    }
                    if (nextWriteTimeout)
                        clearTimeout(nextWriteTimeout);
                    if (!bDragging) {
                        container.on("touchmove mousemove", onDragging);
                        bDragging = true;
                    }
                }).on("mouseup touchend", function (e) {
                    bDragging = false;
                    container.off("touchmove mousemove", onDragging);
                    if (nextWriteTimeout)
                        clearTimeout(nextWriteTimeout);

                    if (!undefinedOrNull(latestDraggedValue)) {
                        setGaugeValue(latestDraggedValue);
                        latestDraggedValue = null;
                    }
                    else {
                        var cursorpt = getPointInSVGSpace(e);
                        if (cursorpt != null) {
                            var newClientValue = getNewClientValue({ x: cursorpt.x, y: cursorpt.y }, webKitRenderCorrection);
                            if (newClientValue != null)
                                setGaugeValue(newClientValue);
                        }
                    }
                });
            }

            for (var i = 0; i < gaugeConfig.rangeContainer.ranges.length; i++) {
                var rangeInfo = getRangeInfo(parameters, i);
                if (!rangeInfo)
                    continue;

                if (i === 0)
                    el.find("svg.dxg.dxg-circular-gauge").css("overflow", "visible");
                var rangeArc = el.find(".dxg-range.dxg-range-" + i)[0];

                if (rangeArc) {
                    var arcPath = $(rangeArc).attr("d");
                    var arcPathData = rangeArc.getPathData(); //"M" "A" "L" "A" "Z"

                    //Range Bars offset and scale
                    //if (!gaugeArcsCenter) {
                    //    var needleHole = el.find(".dxg-spindle-hole");
                    //    if (needleHole.length > 0)
                    //        gaugeArcsCenter = {
                    //            x: parseFloat(needleHole.attr("cx"), 10),
                    //            y: parseFloat(needleHole.attr("cy"), 10)
                    //        };
                    //    else
                    //        gaugeArcsCenter = svgArcToCenterParam(arcPathData[0].values[0], arcPathData[0].values[1], arcPathData[1].values[0], arcPathData[1].values[1], arcPathData[1].values[2], arcPathData[1].values[3], arcPathData[1].values[4], arcPathData[1].values[5], arcPathData[1].values[6], arcPathData[1].values[7]);
                    //}
                    var temparc = document.createElementNS("http://www.w3.org/2000/svg", "path");
                    temparc.setAttribute("d", arcPath.substr(0, arcPath.indexOf(" L")));
                    //var arcMidpoint = temparc.getPointAtLength(temparc.getTotalLength() / 2);
                    //var angleCenterToMidpoint = Math.atan2(gaugeArcsCenter.y - arcMidpoint.y, gaugeArcsCenter.x - arcMidpoint.x);
                    var sizeFactor = Math.min(gaugeConfig.size.height, gaugeConfig.size.width) / 230;
                    var rangeInfoNormalizedOffset = rangeInfo.offset + 29; //-29 = default gauge offset on desktop
                    var rangeOffset = rangeInfoNormalizedOffset * -36 / 29 /* * sizeFactor*/;

                    if (baseModelName == "leftquarter" || baseModelName == "rightquarter")
                        rangeScaleFactor = 0.4;
                    var scale = 1 + Math.abs(rangeOffset / sizeFactor * 0.45 / 36) * (rangeInfoNormalizedOffset >= 0 ? 1 : -1) * rangeScaleFactor;

                    $(rangeArc).attr("transform", "translate(" + gaugeArcsCenter.x + " " + gaugeArcsCenter.y + ") scale(" + scale + ") translate(" + (gaugeArcsCenter.x * -1) + " " + (gaugeArcsCenter.y * -1) + ")");

                    setRangeBarThickness(rangeArc, arcPathData, rangeInfo, scale);
                }
            }
            if (!bHasSVGBackground) {
                var finalRangeContainerSize = el.find("g.dxg-range-container")[0].getBoundingClientRect();
                var rangeContainerSizeVariationFactor = initialRangeContainerSize.height / finalRangeContainerSize.height;
                if (rangeContainerSizeVariationFactor < 1) {
                    innerGaugeScaleFactorX *= rangeContainerSizeVariationFactor;
                    innerGaugeScaleFactorY *= rangeContainerSizeVariationFactor;
                }
            }
        }
    };
    gaugeConfig.size = findGaugeSize();
    if (parameters.IsFastGaugeControl)
        gaugeConfig.subvalueIndicator.offset = parameters.MarkerOffset + parameters.MarkerFactorHeight / 2 - 3;
    else
        gaugeConfig.subvalueIndicator.offset = (parameters.MarkerOffset + 12) * -2;
    //else if (parameters.RangeBarOffset > parameters.LabelOffset && gaugeConfig.scale.label.indentFromTick < 0)
    //    gaugeConfig.valueIndicator.offset = -25;
    if (parameters.RangeBarVisible && !parameters.NeedleVisible && parameters.RangeBarThickness)
        gaugeConfig.valueIndicator.size = parameters.RangeBarThickness;

    var labelFormat = devExtremeHelpers.getLabelsFormat(parameters.LabelStringFormat);
    if (labelFormat)
        gaugeConfig.scale.label.format = labelFormat;

    var gauge;
    var mutateFunc = function () {
        gauge = containerInner.dxCircularGauge(gaugeConfig).dxCircularGauge("instance");

        if (!gaugeSvg) {
            gaugeSvg = containerInner.children(".dxg.dxg-circular-gauge").first();
            gaugeSvgPoint = gaugeSvg[0].createSVGPoint();
        }
        gaugeSvg.css({ "z-index": 2, "position": "relative", "margin": "auto", "overflow": "visible", "transform": "scale(" + gaugeSvgScale + ")" });

        fastdom.measure(function () {
            var firstSubvalueIndicator;
            var spindleHole;
            if (parameters.MarkerStroke.Color || parameters.IsFastGaugeControl)
                firstSubvalueIndicator = gaugeSvg.find(".dxg-subvalue-indicator").children().first();
            if (parameters.NeedleFill.Color)
                spindleHole = gaugeSvg.find(".dxg-spindle-hole");
            fastdom.mutate(function () {
                if (parameters.MarkerStroke.Color)
                    firstSubvalueIndicator.attr("stroke", parameters.MarkerStroke.Color)
                if (parameters.IsFastGaugeControl)
                    firstSubvalueIndicator.attr("stroke-width", parameters.MarkerBorderThickness);
                if (parameters.NeedleFill.Color)
                    spindleHole.attr("fill", urlReferenceReplace(parameters.NeedleFill.Color, parameters.screenId));
            });
        });
        
        fastdom.mutate(function () {
            gaugeSvg.find(".dxg-scale").first().detach().appendTo(gaugeSvg);
            gaugeSvg.find(".dxg-scale-elements").first().detach().appendTo(gaugeSvg);
            gaugeSvg.find(".dxg-subvalue-indicators").first().detach().appendTo(gaugeSvg);
        });

        if (bHasSVGBackground) {
            var gaugeInnerLeftOffset = 0;
            var gaugeInnerTopOffset = parameters.EndAngle - parameters.StartAngle >= 360 ? 0 : parseInt(gaugeSvg.css("height"), 10) * -0.05;

            switch (baseModelName) {
                case "threequarter":
                    break;
                case "leftquarter":
                    gaugeInnerTopOffset -= gaugeInnerTopOffset / 3;
                    gaugeInnerLeftOffset = parseInt(gaugeSvg.css("width"), 10) * -0.05;
                    break;
                case "rightquarter":
                    gaugeInnerTopOffset -= gaugeInnerTopOffset / 3;
                    gaugeInnerLeftOffset = -parseInt(gaugeSvg.css("width"), 10) * -0.05;
                    break;
                default:
                    break;
            }
            containerInner.css({
                "top": gaugeInnerTopOffset,
                "left": gaugeInnerLeftOffset
            });

            var styleSvg = SVG(scope.Styles[parameters.screenId]);
            var gaugeStyle = styleSvg.find('#' + parameters.GaugeBaseModel);
            if (gaugeStyle.length > 0)
                gaugeStyle = gaugeStyle[0]; //.clone();

            if (parameters.ArcScaleFill) {
                var tagsBG = styleSvg.find("[tag='BG']");
                for (var i = 0; i < tagsBG.length; i++)
                    tagsBG[i].addClass(parameters.ArcScaleFill);
            }

            var svgLeft = (container.width() - gaugeSize) / 2;
            var svgTop = (container.height() - gaugeSize) / 2;
            $(gaugeStyle.node).appendTo(container).css({
                "position": "absolute",
                "left": svgLeft,
                "top": svgTop,
                "width": gaugeSize,
                "height": gaugeSize
            });
            if (parameters.IsFastGaugeControl && parameters.BackgroundRadius && parameters.BackgroundRadius != 100) {
                var bgCircle = $(gaugeStyle.node).find("circle");
                if (bgCircle.length > 0) {
                    var backgroundCircle = bgCircle.first();
                    backgroundCircle.attr("r", Math.max(0, parseFloat(backgroundCircle.attr("r")) * parameters.BackgroundRadius / 100 - 6.5));
                }
            }
        }

        switch (baseModelName) {
            case "leftquarter":
            case "rightquarter":
                containerInner.css("transform", "scale(" + innerGaugeScaleFactorX + ", " + innerGaugeScaleFactorY + ")");
                break;
            default:
                containerInner.css("transform", "scale(" + innerGaugeScaleFactorX + ", " + innerGaugeScaleFactorY + ")");
                break;
        }

        var pageTop = 0;
        if (bHasSVGBackground || (gaugeType == "full" && bExpandedGaugeSize)) {
            pageTop = (parentHeight - thisPage.height()) / 2;
        }
        else if (bExpandedGaugeSize) {
            var heightBoundingClientFactor = thisParent[0].getBoundingClientRect().height / thisParent.attr("height");
            var scaleContainerBox = gaugeSvg.find("g.dxg-scale")[0].getBoundingClientRect();
            var rangeContainerBox = gaugeSvg.find("g.dxg-range-container")[0].getBoundingClientRect();
            var gaugeInnerHeight = Math.min(scaleContainerBox.y, rangeContainerBox.y) == scaleContainerBox.y ? scaleContainerBox.height : rangeContainerBox.height;
            var gaugeSvgTopOffset = (gaugeSvg[0].getBoundingClientRect().height - gaugeInnerHeight) / 2
            if (gaugeType == "threequarter" && (!scope.biOS || webKitRenderCorrection || !parameters.currentPopupId))
                gaugeSvgTopOffset = -gaugeSvgTopOffset + Math.max(0, (parentHeight - parentWidth) / 2);
            if (gaugeType == "half")
                gaugeSvgTopOffset = (parentHeight - thisPage.height()) / 3
            pageTop = gaugeSvgTopOffset / heightBoundingClientFactor;
        }
        if (webKitRenderCorrection) {
            var scaleY = new WebKitCSSMatrix(window.getComputedStyle(thisPage[0]).webkitTransform).d;
            pageTop *= scaleY;
        }
        thisPage.css({
            "position": "relative",
            "top": pageTop
        });

        var scaleTopCorrectionFactor = 0;
        if (parameters.ScaleOptions) {
            if (parameters.ScaleOptions.ScaleFactor < 1) {
                scaleTopCorrectionFactor = 1 - parameters.ScaleOptions.ScaleFactor;
                if (parameters.ScaleOptions.HorizontalAlignment !== "Center" && parameters.ScaleOptions.HorizontalAlignment !== "Stretch") {
                    var leftCorrection = parameters.ScaleOptions.HorizontalAlignment === "Left" ? 11 : -11;
                    gaugeSvg.css({
                        "margin": parameters.ScaleOptions.HorizontalAlignment === "Left" ? "inherit" : "0 0 0 50%",
                        "left": leftCorrection + "px"
                    });
                    var spindleLeftCorrection = gaugeSvg.width() / 2 * (parameters.ScaleOptions.HorizontalAlignment === "Left" ? 1 : -1) - leftCorrection;
                    if (spindleStyle)
                        $(spindleStyle.node).css("left", (parseFloat($(spindleStyle.node).css("left")) - spindleLeftCorrection) + "px");
                }
                if (parameters.ScaleOptions.VerticalAlignment !== "Center" && parameters.ScaleOptions.VerticalAlignment !== "Stretch") {
                    containerInner.css("top", (containerInner.height() / 2 - 15) * (parameters.ScaleOptions.VerticalAlignment === "Top" ? -1 : 1));
                }
            }
        }

        setValueLabelPosition(pageTop, scaleTopCorrectionFactor);

        //if (parameters.IsFastGaugeControl) {
        if (parameters.ScaleOptions && bHasSVGBackground) {
            if (parameters.ScaleOptions.VerticalAlignment === "Top" && (parameters.ScaleOptions.BackLayerAspect === "Threequarter" || parameters.ScaleOptions.BackLayerAspect === "Half")) {
                var scaleBox = gaugeSvg.find("g.dxg-scale")[0].getBoundingClientRect();
                var gaugeSvgBox = gaugeSvg[0].getBoundingClientRect();
                var gaugeSvgTopCorrection = scaleBox.y - gaugeSvgBox.y;
                if (parameters.IsFastGaugeControl && (parameters.bFitInWindow || parameters.bKeepAspectRatio)) {
                    gaugeSvgTopCorrection *= parameters.parentViewbox.height / window.innerHeight;
                    if (webKitRenderCorrection)
                        containerInner.css("transform-origin", "top center");
                }
                var angleDiff = Math.abs(parameters.EndAngle - parameters.StartAngle) - 180;
                var angleDiffTopFactor = angleDiff >= 90 ? 0 : 100 - (1 * angleDiff / 90); //over 90 (=>270°) correction factor is zero; below 90 it grows until 100%
                gaugeSvgTopCorrection *= angleDiffTopFactor / 100;
                gaugeSvg.css("top", "-" + gaugeSvgTopCorrection + "px");
                if (spindleStyle)
                    $(spindleStyle.node).css("top", (parseFloat($(spindleStyle.node).css("top")) - gaugeSvgTopCorrection) + "px");
            }
        }
        //}

        if (!parameters.bFitInWindow && isMobile.Android() && isChrome()) {
            container.css({
                "position": "fixed",
                "width": "100%"
            });
        }

        if (parameters.TagMinValue && parameters.TagMinValue.SVGReferenceId) {
            deregisterListenerMin = scope.$on(onDataChanged + parameters.screenId + parameters.TagMinValue.SVGReferenceId, function (e, value) {
                if (scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].isGood) {
                    bTagHasDynamicLimits = true;
                    updateGaugeScaleAndRanges();
                }
            });
        }

        if (parameters.TagMinValue) {
            if (parameters.screenId in scope.dataValues && parameters.TagMinValue.SVGReferenceId in scope.dataValues[parameters.screenId] &&
                scope.dataValues[parameters.screenId][parameters.TagMinValue.SVGReferenceId].isGood) {
                bTagHasDynamicLimits = true;
                updateGaugeScaleAndRanges();
            }
        }

        if (parameters.TagMaxValue && parameters.TagMaxValue.SVGReferenceId) {
            deregisterListenerMax = scope.$on(onDataChanged + parameters.screenId + parameters.TagMaxValue.SVGReferenceId, function (e, value) {
                if (scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].isGood) {
                    bTagHasDynamicLimits = true;
                    updateGaugeScaleAndRanges();
                }
            });
        }

        if (parameters.TagMaxValue) {
            if (parameters.screenId in scope.dataValues && parameters.TagMaxValue.SVGReferenceId in scope.dataValues[parameters.screenId] &&
                scope.dataValues[parameters.screenId][parameters.TagMaxValue.SVGReferenceId].isGood) {
                bTagHasDynamicLimits = true;
                updateGaugeScaleAndRanges();
            }
        }

        onGaugeDataChanged();

        if (parameters.SVGReferenceId)
            applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId, !scope.biOS, scope.biOS ? null : gaugeSvg);
        deregisterListener = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
            applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId, !scope.biOS, scope.biOS ? null : gaugeSvg);
            onGaugeDataChanged();
        });
    };
    mutateHandler.scheduleMutate(parameters.screenId, mutateFunc);

    function findGaugeSize() {
        //starting gaugeSize = min(fobjW, fobjH)
        var ret = {
            height: gaugeSize,
            width: gaugeType == "half" ? parentWidth : gaugeSize
        };
        if (!bHasSVGBackground) {
            var bTicksVisible = (parameters.MajorIntervalCount > 1 && parameters.MajorTickmarkFactorLength > 0 && parameters.MajorTickmarkFactorThickness > 0) || (parameters.MinorIntervalCount > 1 && parameters.MinorTickmarkFactorLength > 0 && parameters.MinorTickmarkFactorThickness > 0);
            //Adjusting gauge size accorting to parameters
            var visibleOffsets = [parameters.RangeBarOffset + parameters.RangeBarThickness / 2];
            if (parameters.Range1Visible)
                visibleOffsets.push(parameters.Range1Offset);
            if (parameters.Range2Visible)
                visibleOffsets.push(parameters.Range2Offset);
            if (parameters.Range3Visible)
                visibleOffsets.push(parameters.Range3Offset);
            var gaugeLimit = Math.max.apply(Math, visibleOffsets);
            ret.width += gaugeLimit;
            ret.height += gaugeLimit;
            if (bTicksVisible && gaugeConfig.scale.label.indentFromTick > 0) {
                ret.width *= 1.1;
                ret.height *= 1.1;
            }
        }
        if (parameters.ScaleOptions) {
            if (parameters.ScaleOptions.ScaleFactor !== 1) {
                ret.width *= parameters.ScaleOptions.ScaleFactor;
                ret.height *= parameters.ScaleOptions.ScaleFactor;
            }
        }
        return ret;
    }

    function setValueLabelPosition(pageTop, scaleTopCorrectionFactor) {
        if (!gaugeValueDisplay && !parameters.ShowEngeneeringUnit)
            return;

        var bottomValueDisplayCorrection = 0;
        var displayParent = containerInner;
        var startingGaugeTop;
        var displayScale = "";
        if (gaugeValueDisplay)
            gaugeValueDisplayElement = gaugeValueDisplay.element();
        if (gaugeType == "half" || bIsQuarterType) {
            startingGaugeTop = gaugeArcsCenter.bbox.y + gaugeArcsCenter.bbox.height;
            bottomValueDisplayCorrection = 50;
            displayScale = " scale(" + (1 + (1 - innerGaugeScaleFactorX)) + ", " + (1 + (1 - innerGaugeScaleFactorY)) + ")";
            fastdom.mutate(function () {
                if (gaugeValueDisplay)
                    gaugeValueDisplay.element().detach().appendTo(containerInner);
            });
        }
        else {
            displayParent = mainContainer;
            if (gaugeValueDisplay) {
                fastdom.mutate(function () {
                    if (scope.biOS && parentOpacity)
                        gaugeValueDisplay.element().css("opacity", parentOpacity);
                    gaugeValueDisplay.element().detach().appendTo(mainContainer);
                });
            }
            startingGaugeTop = thisPage.height() + (baseModelName ? 0 : 20) + (webKitRenderCorrection ? 0 : Math.max(0, pageTop));
        }

        var gaugeTop = startingGaugeTop + bottomValueDisplayCorrection;

        if (gaugeValueDisplay) {
            gaugeValueDisplayTextbox = gaugeValueDisplay.element().find(".dx-texteditor-input");
            dynamicFontElements.push(gaugeValueDisplayTextbox);
            gaugeValueDisplayTextbox.css("padding", 0);
            if (gaugeType == "threequarter")
                gaugeTop -= bHasSVGBackground ? 0 : (webKitRenderCorrection ? - thisPage.height() / 12 : thisPage.height() / 20 - 5);
            if (bIsQuarterType)
                gaugeTop -= 5;
            var gaugeOffset = - parseInt(parameters.ValueOffset.Bottom, 10) + parseInt(parameters.ValueOffset.Top, 10);
            fastdom.mutate(function () {
                gaugeValueDisplay.element().css({
                    "margin-left": "50%",
                    "height": "auto",
                    "transform": "translateX(-50%)" + displayScale,
                    "left": (parseFloat(parameters.ValueOffset.Left) - parseFloat(parameters.ValueOffset.Right)) / 2 + "px"
                });
                var displayLabelTop = (gaugeTop + gaugeOffset - gaugeValueDisplay.element().height()) * (1 + scaleTopCorrectionFactor);
                gaugeValueDisplay.element().css("top", displayLabelTop - displayLabelTop * (gaugeSvgScale - 1));
            });
        }

        if (parameters.ShowEngeneeringUnit) {
            gaugeEU = $("<div class='gaugeEUDisplay' style='background: transparent; white-space: nowrap; border: none; margin-left:50%; text-align: center; position: " + (scope.biOS && (parameters.IsFastGaugeControl || parameters.currentPopupId) && !webKitRenderCorrection ? "absolute" : "fixed") + "; color: " + parameters.EngeneeringUnitForeground.Color + "; font-family: " + parameters.EngeneeringUnitFontSettings.FontFamily + "; font-size: " + parameters.EngeneeringUnitFontSettings.FontSize + "px; font-weight:" + fontWeightConverter(parameters.EngeneeringUnitFontSettings.FontWeight) + "; z-index: 3;'>" + i18next.t1(parameters.EngeneeringUnit) + "</div>")
                .appendTo(displayParent);
            dynamicFontElements.push(gaugeEU);
            var gaugeEUCssOptions = {
                "margin-left": "50%",
                "height": "auto",
                "transform": "translateX(-50%)" + displayScale,
                "left": (parseFloat(parameters.EngeneeringOffset.Left) - parseFloat(parameters.EngeneeringOffset.Right)) / 2 + "px"
            };
            if (scope.biOS && parentOpacity && displayParent == thisPage)
                gaugeEUCssOptions["opacity"] = parentOpacity;
            var euOffset = - parseInt(parameters.EngeneeringOffset.Bottom, 10) + /*parseInt(parameters.EngeneeringOffset.Top, 10)*/ - 30;
            if (!bHasSVGBackground)
                euOffset += gaugeType != "half" ? (gaugeType == "threequarter" ? 10 : 20) : 0;
            else if (gaugeType == "half")
                euOffset += 5;
            if (parameters.EngeneeringUnitFontSettings.FontStyle) {
                var eufontStyle = parameters.EngeneeringUnitFontSettings.FontStyle.toLowerCase();
                if (eufontStyle != "normal")
                    gaugeEUCssOptions["font-style"] = eufontStyle;
            }
            fastdom.mutate(function () {
                gaugeEU.css(gaugeEUCssOptions);
                gaugeEU.css("top", (gaugeTop + euOffset) * (1 + scaleTopCorrectionFactor));
            });

            deregisterListenerActiveLanguage = scope.$on(onActiveLanguageChanged, function (e, value) {
                decimalSeparator = getDecimalSeparator(scope, value);
                if (gaugeEU)
                    gaugeEU.text(i18next.t1(parameters.EngeneeringUnit));
                if (parameters.FontSettingList && (value in parameters.FontSettingList) !== -1)
                    applyFontSettingList(dynamicFontElements, parameters.FontSettingList[value]);
                if (gauge)
                    gauge.option({
                        "scale.label.font.family": parameters.FontSettingList[value].FontFamily,
                        "scale.label.font.size": parameters.FontSettingList[value].FontSize
                    });
            });
        }
    }

    function applySpindleStyle(gaugeEl) {
        var styleSvg = SVG(scope.Styles[parameters.screenId]);
        spindleStyle = styleSvg.find('#' + spindleStyleProp);
        if (spindleStyle.length > 0)
            spindleStyle = spindleStyle[0]; //.clone();

        if (parameters.ArcScaleFill) {
            var tagsBG = styleSvg.find("[tag='BG']");
            for (var i = 0; i < tagsBG.length; i++)
                tagsBG[i].addClass(parameters.SpindleFill);
        }

        var needleHole = gaugeEl.find(".dxg-spindle-hole");
        if (needleHole.length > 0)
            needleHole.css("display", "none");
        gaugeEl.find(".dxg-spindle-border").css("display", "none");

        var spindleStyleNode = $(spindleStyle.node);
        spindleStyleNode.appendTo(containerInner);

        spindleStyleNode.css({
            "position": "absolute",
            "z-index": bHasRangeBar ? 2 : 1,
            "width": spindleSize.w,
            "height": spindleSize.h,
            "transform": "scale(1.12)"
        });
        var containerInnerSize = {
            w: containerInner.width(),
            h: containerInner.height()
        };
        var pos = {};
        pos = {
            "top": gaugeArcsCenter.bbox.y - spindleSize.h / 2 + gaugeArcsCenter.bbox.height / 2,
            "left": (containerInnerSize.w - containerInner.children(".dxg.dxg-circular-gauge").first().width()) / 2 + gaugeArcsCenter.bbox.x - spindleSize.w / 2 + gaugeArcsCenter.bbox.width / 2
        };
        spindleStyleNode.css(pos);

        if (spindleSize.w > 0 && spindleSize.h > 0) {
            var spindleSizeFactor = (containerInnerSize.w * containerInnerSize.h) / (spindleSize.w * spindleSize.h);
            if (spindleSizeFactor < 2) {
                var firstTick = gaugeEl.find(".dxg-line path:first");
                if (firstTick.length > 0) {
                    if (overlaps(spindleStyle.node, firstTick[0]))
                        gaugeSvgScale = 1.15;
                }
            }
        }
    }

    function updateGaugeScaleAndRanges() {
        var rangesObj = getRanges();

        majorCustomTicks = devExtremeHelpers.getCustomTicks(parameters, rangesObj.min, rangesObj.max, parameters.MinorIntervalCount, parameters.MajorIntervalCount);
        gauge.option("animation.enabled", false);
        gauge.option({
            "min": rangesObj.min,
            "max": rangesObj.max,
            "scale.startValue": rangesObj.min,
            "scale.endValue": rangesObj.max,
            "scale.customTicks": majorCustomTicks,
            "scale.customMinorTicks": devExtremeHelpers.getCustomTicks(parameters, rangesObj.min, rangesObj.max, parameters.MinorIntervalCount, parameters.MajorIntervalCount, true, majorCustomTicks),
            "rangeContainer.ranges": rangesObj.ranges
        });
        gauge.option("animation.enabled", true);
    }

    function getRanges() {
        var mm = getMinMaxControlValues(scope, parameters, bTagHasDynamicLimits, bTagHasEU);
        var bKeepOldLimits = false;

        if (minmax)
            if ((mm.min == minmax.min && mm.max == minmax.max) || isNaN(mm.min) || isNaN(mm.max))
                bKeepOldLimits = true;

        if (!bKeepOldLimits)
            minmax = mm;

        var ranges = [];
        if (parameters.Range1Visible && parameters.Range1Fill)
            ranges.push({
                "startValue": (minmax.max - minmax.min) * parameters.Range1StartValue * 0.01 + parseFloat(minmax.min),
                "endValue": (minmax.max - minmax.min) * parameters.Range1EndValue * 0.01 + parseFloat(minmax.min),
                "color": parameters.Range1Fill.Color
            });
        if (parameters.Range2Visible && parameters.Range2Fill)
            ranges.push({
                "startValue": (minmax.max - minmax.min) * parameters.Range2StartValue * 0.01 + parseFloat(minmax.min),
                "endValue": (minmax.max - minmax.min) * parameters.Range2EndValue * 0.01 + parseFloat(minmax.min),
                "color": parameters.Range2Fill.Color
            });
        if (parameters.Range3Visible && parameters.Range3Fill)
            ranges.push({
                "startValue": (minmax.max - minmax.min) * parameters.Range3StartValue * 0.01 + parseFloat(minmax.min),
                "endValue": (minmax.max - minmax.min) * parameters.Range3EndValue * 0.01 + parseFloat(minmax.min),
                "color": parameters.Range3Fill.Color
            });
        return {
            ranges: ranges,
            min: minmax.min,
            max: minmax.max
        };
    }

    function getRangeInfo(parameters, rangeIndex) {
        var bIsVisible = false;
        var index = rangeIndex;
        while (!bIsVisible && index <= 2) {
            index++;
            bIsVisible = parameters["Range" + index + "Visible"];
        }

        if (!bIsVisible)
            return null;

        return {
            offset: parameters["Range" + index + "Offset"],
            thickness: parameters["Range" + index + "Thickness"]
        };
    }

    function setRangeBarThickness(rangeArc, arcPathData, rangeInfo, scale) {
        var moveCoords = arcPathData[0].values;
        var firstArcEndCoords = arcPathData[1].values.slice(arcPathData[1].values.length - 2);
        var lineCoords = arcPathData[2].values;
        var secondArcEndCoords = arcPathData[3].values.slice(arcPathData[1].values.length - 2);
        var leftAngle = Math.atan2(lineCoords[1] - firstArcEndCoords[1], lineCoords[0] - firstArcEndCoords[0])
        var rightAngle = Math.atan2(moveCoords[1] - secondArcEndCoords[1], moveCoords[0] - secondArcEndCoords[0]);

        var thicknessFactor = rangeInfo.thickness * 3 / 5 - 2;
        var thicknessVariationLeft = { x: thicknessFactor * toFixedNumber(Math.cos(leftAngle), 6) * 1 / scale, y: thicknessFactor * toFixedNumber(Math.sin(leftAngle), 6) * 1 / scale };
        var thicknessVariationRight = { x: thicknessFactor * toFixedNumber(Math.cos(rightAngle), 6) * 1 / scale, y: thicknessFactor * toFixedNumber(Math.sin(rightAngle), 6) * 1 / scale };

        //L (x,y)
        arcPathData[2].values[0] += thicknessVariationLeft.x;
        arcPathData[2].values[1] += thicknessVariationLeft.y;

        //A1
        arcPathData[1].values[5] -= thicknessVariationLeft.x;
        arcPathData[1].values[6] -= thicknessVariationLeft.y;
        arcPathData[1].values[0] += thicknessFactor / (1 - (1 - scale) * 1.3);
        arcPathData[1].values[1] += thicknessFactor / (1 - (1 - scale) * 1.3);

        //M (x,y)
        arcPathData[0].values[0] += thicknessVariationRight.x;
        arcPathData[0].values[1] += thicknessVariationRight.y;

        //A2
        var innerArcVariation = thicknessFactor / (1.05 - (1 - scale));
        if (innerArcVariation < arcPathData[3].values[0])
            arcPathData[3].values[0] -= innerArcVariation;
        if (innerArcVariation < arcPathData[3].values[1])
            arcPathData[3].values[1] -= innerArcVariation;
        arcPathData[3].values[5] -= thicknessVariationRight.x;
        arcPathData[3].values[6] -= thicknessVariationRight.y;

        rangeArc.setPathData(arcPathData);
    }

    function onGaugeDataChanged() {
        if (bDragging)
            return;

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
                updateClientValue(newValue, newInvariantValue);
            }
            else if (parameters.ShowValue && gaugeValueDisplay) {
                gaugeValueDisplay.option("value", "?");
            }
        }
        finally {
            gauge.option("animation.enabled", true);
            onSettingValue = false;
        }
    }

    function setGaugeClientValue(newValue, endValue) {
        updateClientValue(newValue, endValue, false)
    }

    function updateClientValue(newValue, newInvariantValue, bCheckUpdateEU = true) {
        gauge.option({
            "value": newInvariantValue,
            "subvalues": [newInvariantValue]
        });

        if (bCheckUpdateEU)
            checkUpdateEU();

        updateValueLabel(newValue);
    }

    function checkUpdateEU() {
        if (!notNumeric(scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeLow)) {
            bTagHasEU = true;
            updateGaugeScaleAndRanges();
        }
        if (!notNumeric(scope.dataValues[parameters.screenId][parameters.SVGReferenceId].rangeHigh)) {
            bTagHasEU = true;
            updateGaugeScaleAndRanges();
        }

        if (gaugeEU && !parameters.EngeneeringUnit && parameters.UseEUnit && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].displayName) {
            gaugeEU.text(scope.dataValues[parameters.screenId][parameters.SVGReferenceId].displayName);
        }
    }

    function setGaugeValue(newInvariantValue) {
        if (onSettingValue)
            return;
        if (!(parameters.screenId in scope.dataValues) || !(parameters.SVGReferenceId in scope.dataValues[parameters.screenId]))
            return;
        if (!scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood)
            return;

        gauge.option("disabled", true);

        scope.corehubconnection.invoke("SetValue", parameters.SVGReferenceId, String(newInvariantValue), parameters.screenId)
            .then((ret) => {
                gauge.option("disabled", false);
            })
            .catch(err => {
                console.error(err.toString());

                gauge.option("disabled", false);
                
                scope.$broadcast(onError, { ex: err, value: "Error setting value : " + err.toString() + " for item : " + parameters.SVGItemId });
            });
    }

    function getGaugeSpindleCenterRect(component, bRotated) {
        if (!component || !containerInner)
            return null;
        var gaugeType = component.option("valueIndicator.type");
        component.option("valueIndicator.type", "triangleNeedle");
        var rect = mainContainer.find(".dxg-spindle-hole")[0].getBoundingClientRect();
        component.option("valueIndicator.type", gaugeType);
        return rect;
    }

    function getNewClientValue(clickedPoint, bUseBoundingRect = false) {
        if (!gaugeArcsCenter)
            return null;

        if (bUseBoundingRect) {
            var boundingRect;
            if (!parentScreen.hasClass("rotated")) {
                if (!gaugeArcsCenter.rect)
                    gaugeArcsCenter.rect = getGaugeSpindleCenterRect(gauge);
                boundingRect = gaugeArcsCenter.rect;
            }
            else {
                if (!gaugeArcsCenter.rotatedRect)
                    gaugeArcsCenter.rotatedRect = getGaugeSpindleCenterRect(gauge, true);
                boundingRect = gaugeArcsCenter.rotatedRect;
            }
        }

        var gaugeCenter = {
            x: bUseBoundingRect ? boundingRect.x + boundingRect.width / 2 : gaugeArcsCenter.x,
            y: bUseBoundingRect ? boundingRect.y + boundingRect.height / 2 : gaugeArcsCenter.y
        };
        var angleCenterToClicked = Math.atan2(gaugeCenter.y - clickedPoint.y, gaugeCenter.x - clickedPoint.x);
        var valuePerDegree = (minmax.max - minmax.min) / absAngle;

        var clickedDxAngle = 180 - angleCenterToClicked * 180 / Math.PI;
        if (clickedDxAngle > gaugeStartAngle)
            clickedDxAngle = clickedDxAngle - 360;
        
        if (absAngle == 360 || (clickedDxAngle <= gaugeStartAngle && clickedDxAngle >= gaugeEndAngle))
            return Math.abs(clickedDxAngle - gaugeStartAngle) * valuePerDegree + minmax.min;
        return null;
    }

    function updateValueLabel(newValue) {
        if (parameters.ShowValue || parameters.ShowEngeneeringUnit) {
            var finalDisplayValue = "";
            if (parameters.ShowValue) {
                var newValFormatted = newValue;
                if (decimalSeparator != invariantNumberDecimalSeparator)
                    newValFormatted = parseFloat(newValue.replace(decimalSeparator, invariantNumberDecimalSeparator)).toFixed(decimals).replace(invariantNumberDecimalSeparator, decimalSeparator);
                else
                    newValFormatted = parseFloat(newValue).toFixed(decimals);
                finalDisplayValue += newValFormatted;
            }
            //if (parameters.ShowEngeneeringUnit && gaugeConfig.valueIndicator.type != "rangebar")
            //    finalDisplayValue += (finalDisplayValue != "" ? " " + parameters.EngeneeringUnit : parameters.EngeneeringUnit);

            gaugeValueDisplay.option("value", finalDisplayValue);
        }
    }

    function onDragging(e) {
        e.preventDefault();

        if (!gaugeArcsCenter) {
            var styleRect = gaugeStyleNode[0].getBoundingClientRect();
            gaugeArcsCenter = {
                x: styleRect.x + styleRect.width / 2,
                y: styleRect.y + styleRect.height / 2,
            };
        }

        var cursorpt = getPointInSVGSpace(e);
        if (cursorpt != null) {
            latestDraggedValue = getNewClientValue({ x: cursorpt.x, y: cursorpt.y }, webKitRenderCorrection);
            if (latestDraggedValue != null) {
                gauge.option("animation.enabled", false);
                setGaugeClientValue(latestDraggedValue.toString(), latestDraggedValue);
            }

            bDragNotifyingServer = true;
            if (dragDetectTimeout)
                clearTimeout(dragDetectTimeout);
            dragDetectTimeout = setTimeout(function () {
                if (writeTimeout) {
                    clearTimeout(writeTimeout);
                    writeTimeout = null;
                }
                bDragNotifyingServer = false;
            }, serverWriteTimeoutDefaultMS * 2);

            if (!writeTimeout)
                writeTimeout = setTimeout(notifyServerOnDragging, serverWriteTimeoutDefaultMS);
        }
    }

    function notifyServerOnDragging() {
        latestServerInvokeTime = Date.now();
        scope.corehubconnection.invoke("SetValue", parameters.SVGReferenceId, String(latestDraggedValue), parameters.screenId).finally(() => {
            if (!bDragging || !bDragNotifyingServer)
                return;

            var nextInvokeTimeout = serverWriteTimeoutDefaultMS - (Date.now() - latestServerInvokeTime);
            if (nextInvokeTimeout <= 0)
                notifyServerOnDragging();
            else {
                if (nextWriteTimeout)
                    clearTimeout(nextWriteTimeout);
                nextWriteTimeout = setTimeout(notifyServerOnDragging, nextInvokeTimeout);
            }
        });
    }

    function getPointInSVGSpace(e) {
        if (undefinedOrNull(e.clientX) || undefinedOrNull(e.clientY))
            return null;

        gaugeSvgPoint.x = e.clientX;
        gaugeSvgPoint.y = e.clientY;
        var cursorpt;
        if (!webKitRenderCorrection)
            cursorpt = gaugeSvgPoint.matrixTransform(gaugeSvg[0].getScreenCTM().inverse());
        else {
            var scaleX = 1;
            var scaleY = 1;
            var transformMatrix = new WebKitCSSMatrix(window.getComputedStyle(thisPage[0]).webkitTransform);
            scaleY = transformMatrix.d;
            scaleX = transformMatrix.a;

            var px = e.pageX;
            var py = e.pageY;

            cursorpt = {
                x: px * scaleX, y: py * scaleY
            };

            if (webKitRenderCorrection && parameters.bKeepAspectRatio) {
                var karOffset = getKeepAspectRatioOffset(viewboxRatio);
                cursorpt.y += karOffset.y;
                cursorpt.x += karOffset.x;
            }
        }
        return cursorpt;
    }
}