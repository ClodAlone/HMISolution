function onButtonControlLoaded(obj) {
    var thisPage = $(obj).parent();
    thisPage.localize();

    var scope = thisPage.scope();
    if (!scope)
        return;
    var paratt = thisPage.data(dataparameters);
    if (!paratt)
        return;

    $(obj).remove();
    var thisParent = thisPage.parent();
    var thisParentId = thisParent.attr("id");
    var thisParentPos;
    var thisParentSize;

    var deregisterListeners = new Array();

    var parameters = JSON.parse(paratt);
    var webKitRenderCorrection = $(thisPage).hasClass("webKitRenderCorrection");
    var buttonText = parameters.Text;
    if (!buttonText)
        buttonText = parameters.Content;
    var bHasNewLine = buttonText.indexOf("\n") != -1;

    var textNode;
    var titleNode;
    var $fobjReplacerContainer;
    var textBox;
    var rectBox;
    var pressedBox;
    var alignedTextPos;
    var clipPath;
    var bNeedsWrapping = parameters.TextWrapping != 1 || bHasNewLine;
    var textAlignment = parameters.HorizontalContentAlignment == 0 ? "left" : (parameters.HorizontalContentAlignment == 2 ? "right" : "center");
    var buttonStartingCoords;
    var horizontalMargin = 5;
    var verticalMargin = 5;
    var clipPathID = thisParentId + parameters.screenId + "_WrappingClipPath";
    var pressedPatternID = thisParentId + parameters.screenId + "_PressedIcon";
    var releasedPatternID = thisParentId + parameters.screenId + "_ReleasedIcon";
    var releasedBox;
    var originalFill = "none";

    if (parameters.FontFamily)
        loadCustomFonts([parameters.FontFamily]);

    setOpacity(scope, thisParent, thisPage);

    var styleData;
    var textRectNode;
    var qualityElem;

    var mutateFunc = function () {
        removeForeignObject();
        configureTextParameters();
        textBox = textNode[0].getBBox();

        onTextChanged();

        if (parameters.SVGReferenceId) {
            qualityElem = styleData.styleApplied ? $fobjReplacerContainer.children("svg").first() : textRectNode;
            applyTagQualityStyle(qualityElem, scope, parameters.screenId, parameters.SVGReferenceId, true);
            var drl = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
                if (styleData.styleApplied)
                    qualityElem = $fobjReplacerContainer.children("svg").first();
                applyTagQualityStyle(qualityElem, scope, parameters.screenId, parameters.SVGReferenceId, true);
            });
            deregisterListeners.push(drl);
        }

        if (styleData.styleApplied) {

            checkEnableControl();
            styleData.eventsTarget.on('mousedown touchstart keydown mouseup touchend keyup', buttonClicked);

            styleData.setStrokeAndFill(true);
            
            $fobjReplacerContainer.bind(disabledClassEvent, function () {
                styleData.enableDisableStyles(!thisParent.hasClass(disabledClass), webKitRenderCorrection);
            });
        }
        else {
            $fobjReplacerContainer.on("mousedown touchstart", function () {
                if ($fobjReplacerContainer.hasClass(disabledClass))
                    return;

                textRectNode.addClass("pressedState");
            }).on("mouseup mouseleave touchend", function () {
                if ($fobjReplacerContainer.hasClass(disabledClass))
                    return;

                textRectNode.removeClass("pressedState");
            });

            $(thisParent).bind(disabledClassEvent, function () {
                if ($fobjReplacerContainer.hasClass(disabledClass))
                    $fobjReplacerContainer.addClass("disableStyle");
                else
                    $fobjReplacerContainer.removeClass("disableStyle");
            });
        }
        registerStateImages();
    }

    function handleTextWrapping() {
        if (bNeedsWrapping)
            textNode = autoWrapSVGTextToWidth(textNode[0], textRectNode.attr("width") - horizontalMargin * 2, textAlignment == "justify");
        clipPath = configureTextClipPath(clipPath, clipPathID, { x: textBox.x, y: buttonStartingCoords.y + parameters.BorderThickness.Top, width: rectBox.width, height: rectBox.height }, textNode, $fobjReplacerContainer);
    }

    function handleTextAlignment() {
        var tspans = textNode.find("tspan");
        //Horizontal alignment
        if (textAlignment != "left") {
            if (textAlignment == "center") {
                textNode.css("text-anchor", "middle");
                if (tspans.length > 0)
                    tspans.each(function (i, el) {
                        var $el = $(el);
                        $el.attr("x", parseFloat($el.attr("x")) + rectBox.width / 2);
                    });
                else
                    textNode.attr("x", parseFloat(buttonStartingCoords.x) + rectBox.width / 2);
            }
            if (textAlignment == "right") {
                textNode.css("text-anchor", "end");
                if (tspans.length > 0)
                    tspans.each(function (i, el) {
                        var $el = $(el);
                        $el.attr("x", parseFloat($el.attr("x")) + rectBox.width);
                    });
                else
                    textNode.attr("x", parseFloat(buttonStartingCoords.x) + rectBox.width);
            }
        }
        else {
            if (tspans.length > 0)
                tspans.each(function (i, el) {
                    var $el = $(el);
                    $el.attr("x", parseFloat($el.attr("x")) + horizontalMargin);
                });
            else
                textNode.attr("x", parseFloat(buttonStartingCoords.x) + horizontalMargin);
        }

        //Value label valignment
        textBox = checkVAlignment(textNode, textBox, tspans, rectBox, parameters.VerticalContentAlignment, bHasNewLine, verticalMargin);
        alignedTextPos = {
            x: parseFloat(textNode.attr("x")),
            y: parseFloat(textNode.attr("y"))
        };
    }

    fastdom.measure(() => {
        thisParentPos = { x: webKitRenderCorrection ? 0 : thisParent.attr("x"), y: webKitRenderCorrection ? 0 : thisParent.attr("y") };
        thisParentSize = { w: thisParent.attr("width"), h: thisParent.attr("height") };
        mutateHandler.scheduleMutate(parameters.screenId, mutateFunc);
    });

    var deregisterListenerActiveLanguage = scope.$on(onActiveLanguageChanged, function (e, value) {
        let translatedText = i18next.t1(parameters.Text);
        if (textNode && textNode[0].textContent !== translatedText) {
            textNode[0].textContent = translatedText;
            onTextChanged();
        }
        if (titleNode && titleNode.length > 0)
            titleNode[0].textContent = i18next.t1(parameters.ToolTip);
    });
    deregisterListeners.push(deregisterListenerActiveLanguage);

    function buttonClicked(e) {
        var bOn = e.type === "mousedown" || e.type === "keydown" || e.type === "touchstart";
        if (bOn && thisParent.hasClass(disabledClass))
            return;
        styleData.toggleButtonStyle(bOn);
        if (bOn)
            applyTagQualityStyle($fobjReplacerContainer.children("svg").first(), scope, parameters.screenId, parameters.SVGReferenceId, true);
    }

    function onTextChanged() {
        handleTextWrapping();
        handleTextAlignment();
    }

    function checkEnableControl() {
        var enabled = true;
        if (parameters.screenId in scope.dataValues && parameters.SVGReferenceId in scope.dataValues[parameters.screenId])
            enabled = scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood && !thisParent.hasClass(disabledClass) && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isWritable && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isUserWritable;

        if (styleData && styleData.styleApplied)
            styleData.enableDisableStyles(enabled, webKitRenderCorrection);
        else if (enabled)
            $fobjReplacerContainer.removeClass("disableStyle");
        else
            $fobjReplacerContainer.addClass("disableStyle");
    }

    function registerStateImages() {
        if (!parameters.ReleasedIcon && !parameters.PressedIcon)
            return;

        let newDefs = [];
        let iconMargin = 4;
        let pressReleasedRectHeight = (thisParentSize.h - iconMargin * 2);
        let pressReleasedRect = $(SVG("<rect style='display: none;' x='" + (buttonStartingCoords.x + iconMargin) + "' y='" + (buttonStartingCoords.y + iconMargin) + "' width='" + (thisParentSize.w - iconMargin * 2) + "' height='" + pressReleasedRectHeight + "' />").node);
        let imgStretchAttrs = "";

        if (parameters.ImageStretch != 0) { // != None
            imgStretchAttrs = " height = '" + (thisParentSize.h - iconMargin * 2) + "' preserveAspectRatio = 'none' ";
        }

        textRectNode.after(pressReleasedRect);
        if (parameters.PressedIcon) {
            var pressedPattern = $(SVG("<defs><pattern id='" + pressedPatternID + "' width='1' height='1'><image " + imgStretchAttrs + " xmlns:xlink='http://www.w3.org/1999/xlink' xlink:href='" + sanitizedImageUrl(parameters.PressedIcon) + "'></image></pattern></defs>").node);
            newDefs.push(pressedPattern);
        }

        if (parameters.ReleasedIcon) {
            var releasedPattern = $(SVG("<defs><pattern id='" + releasedPatternID + "' width='1' height='1'><image " + imgStretchAttrs + " xmlns:xlink='http://www.w3.org/1999/xlink' xlink:href='" + sanitizedImageUrl(parameters.ReleasedIcon) + "'></image></pattern></defs>").node);
            newDefs.push(releasedPattern);
            pressReleasedRect.attr({
                "fill": "url(\"#" + releasedPatternID + "\")",
            }).show();   
        }

        $fobjReplacerContainer.on("mousedown touchstart", function (e) {
            if ($fobjReplacerContainer.hasClass(disabledClass))
                return;

            fastdom.mutate(function () {
                textNode.attr("x", alignedTextPos.x + (parameters.PressedIcon ? pressedBox.width + horizontalMargin : 0));
                if (parameters.PressedIcon)
                    pressReleasedRect.attr({
                        "fill": "url(\"#" + pressedPatternID + "\")"
                    }).show();
                else if (parameters.ReleasedIcon)
                    pressReleasedRect.hide();
            });
        }).on("mouseup mouseleave touchend", function (e) {
            if ($fobjReplacerContainer.hasClass(disabledClass))
                return;

            fastdom.mutate(function () {
                if (parameters.ReleasedIcon) {
                    textNode.attr("x", alignedTextPos.x + releasedBox.width + horizontalMargin);
                    pressReleasedRect.attr({
                        "fill": "url(\"#" + releasedPatternID + "\")",
                    }).show();
                }
                else {
                    pressReleasedRect.hide();
                    textNode.attr("x", alignedTextPos.x);
                }
            });
        });

        if (newDefs.length > 0)
            fastdom.mutate(function () {
                $(newDefs).each(function (i, el) {
                    el.prependTo($fobjReplacerContainer);
                });
                if (parameters.ReleasedIcon) {
                    let image = releasedPattern.find("image");
                    function onReleaseImageLoaded() {
                        image[0].removeEventListener('load', onReleaseImageLoaded);
                        releasedBox = image[0].getBBox();
                        var newTextPos = alignedTextPos.x + releasedBox.width + horizontalMargin;
                        textNode.attr("x", newTextPos);
                        if (scope.biOS && newTextPos >= rectBox.x + rectBox.width) {
                            textNode.hide();
                        }
                        pressReleasedRect.attr("y", parseFloat(pressReleasedRect.attr("y")) + pressReleasedRectHeight / 2 - releasedBox.height / 2);
                    }
                    image[0].addEventListener('load', onReleaseImageLoaded);
                }
                if (parameters.PressedIcon) {
                    let image = pressedPattern.find("image");
                    pressedBox = image[0].getBBox();
                    pressReleasedRect.attr("y", parseFloat(pressReleasedRect.attr("y")) + pressReleasedRectHeight / 2 - pressedBox.height / 2);
                }
            });
    }

    function configureTextParameters() {
        var inputOptions = {};
        if (parameters.Foreground)
            inputOptions["fill"] = getSolidColor(parameters.Foreground);
        if (parameters.FontFamily)
            inputOptions["font-family"] = parameters.FontFamily;
        if (parameters.FontSize)
            inputOptions["font-size"] = parameters.FontSize;
        if (parameters.FontWeight)
            inputOptions["font-weight"] = fontWeightConverter(parameters.FontWeight);
        var fontStyle = parameters.FontStyle.toLowerCase();
        if (fontStyle != "normal")
            inputOptions["font-style"] = fontStyle;
        if (!$.isEmptyObject(inputOptions))
            textNode.css(inputOptions);
    }

    function removeForeignObject() {
        fobjReplacerContainer = document.createElementNS("http://www.w3.org/2000/svg", "g");
        fobjReplacerContainer.setAttribute("id", thisParentId);
        fobjReplacerContainer.classList.add("fobjReplacer");
        fobjReplacerContainer.classList.add("ButtonControl");
        thisParent.after(fobjReplacerContainer);

        textNode = $(SVG("<text dy='1em' style='user-select: none;'>" + i18next.t1(buttonText) + "</text>").node);

        $fobjReplacerContainer = $(fobjReplacerContainer);

        $fobjReplacerContainer.bind('destroyed', function () {
            deregisterListeners.forEach(function (item, index, array) {
                item();
            });
            if (styleData && styleData.styleApplied)
                styleData.eventsTarget.off('mousedown touchstart keydown mouseup touchend keyup', buttonClicked);
        });

        let tr = safelyRemoveFobj(thisParent, $fobjReplacerContainer, scope, webKitRenderCorrection);
        let translatedPos = { x: thisParentPos.x - tr.x, y: thisParentPos.y - tr.y };

        buttonStartingCoords = {
            "x": translatedPos.x,
            "y": translatedPos.y
        };

        var textStartingAttrs = {
            "x": translatedPos.x,
            "y": translatedPos.y + verticalMargin
        };

        var attrs = {
            "x": translatedPos.x,
            "y": translatedPos.y,
            "width": thisParentSize.w,
            "height": thisParentSize.h
        };
        if (parameters.Opacity) {
            textStartingAttrs["opacity"] = parameters.Opacity;
            attrs["opacity"] = parameters.Opacity;
        }

        textNode.attr(textStartingAttrs).appendTo($fobjReplacerContainer);
        titleNode = $(SVG("<title>" + i18next.t1(parameters.ToolTip) + "</title>").node);
        titleNode.appendTo($fobjReplacerContainer);

        styleData = scope.applyControlStyle(scope, $fobjReplacerContainer, $fobjReplacerContainer, parameters, false, attrs, true);

        textRectNode = $(SVG("<rect width='100%' height='100%' fill='none'></rect>").node);

        if (!styleData.styleApplied) {
            var borderCss = {};
            setObjectBorder(borderCss, parameters, null, null, parameters.screenId);
            if (!$.isEmptyObject(borderCss))
                textRectNode.css(borderCss);

            var borderRadius = 0;
            if (parameters.CornerRadius) {
                borderRadius = Math.max(parameters.CornerRadius.TopLeft, parameters.CornerRadius.BottomLeft, parameters.CornerRadius.TopRight, parameters.CornerRadius.BottomRight);
                attrs["rx"] = borderRadius;
            }

            if (parameters.Background && parameters.Background.Color) {
                originalFill = urlReferenceReplace(parameters.Background.Color, parameters.screenId);
                attrs["fill"] = originalFill;
            }
        }

        textRectNode.attr(attrs).prependTo($fobjReplacerContainer);

        rectBox = textRectNode[0].getBBox();
    }
}