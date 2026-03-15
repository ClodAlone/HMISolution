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

    var deregisterListeners = new Array();
    $(thisPage).bind('destroyed', function () {
        deregisterListeners.forEach(function (item, index, array) {
            item();
        });
        if (styleData && styleData.styleApplied)
            styleData.eventsTarget.off('mousedown keydown mouseup keyup', buttonClicked);
    });

    var parameters = JSON.parse(paratt);
    var webKitRenderCorrection = $(thisPage).hasClass("webKitRenderCorrection");
    var buttonText = parameters.Text;
    if (!buttonText)
        buttonText = parameters.Content;

    if (parameters.FontFamily)
        loadCustomFonts([parameters.FontFamily]);

    var buttoncontainer = thisPage.children().first();
    if (scope.biOS) {
        webkitMoveRotateTransform(thisParent, thisPage, buttoncontainer);
        var bIsInFittedParent = isInFittedParent(thisPage);
        if (webKitRenderCorrection || bIsInFittedParent || ($.trim(buttonText) && parameters.currentPopupId && !parameters.bFitInWindow)) {
            buttoncontainer.css("position", "relative");
            if (webKitRenderCorrection)
                thisPage.css("will-change", "opacity");
        }
    }

    setOpacity(scope, thisParent, thisPage);

    var el;
    var textDiv;
    var styleData;
    var button;

    var mutateFunc = function () {
        styleData = scope.applyControlStyle(scope, thisParent, buttoncontainer, parameters);
        if (parameters.SVGReferenceId) {
            applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId);
            var drl = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
                applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId);
            });
            deregisterListeners.push(drl);
        }

        if (styleData.styleApplied) {

            checkEnableControl(null, thisParent, parameters.SVGReferenceId, parameters.screenId);
            styleData.eventsTarget.on('mousedown keydown mouseup keyup', buttonClicked);

            if (isFirefox()) {
                var onContainerShown = scope.$on("containerShown" + parameters.screenId, function (e, value) {
                    fastdom.measure(function () {
                        var innerSvg = buttoncontainer.find("svg");
                        if (innerSvg.length > 0)
                            fastdom.mutate(function () {
                                innerSvg.remove();
                                //setTimeout(function () {
                                innerSvg.appendTo(buttoncontainer);
                                //}, 1);
                            });
                    });
                });
                deregisterListeners.push(onContainerShown);
            }

            styleData.setStrokeAndFill(true);

            $(thisParent).bind(disabledClassEvent, function () {
                styleData.enableDisableStyles(!thisParent.hasClass(disabledClass), webKitRenderCorrection);
            });

            registerStateImages(parameters, buttoncontainer, 10);

            var textAlignment = parameters.HorizontalContentAlignment == 0 ? "left" : (parameters.HorizontalContentAlignment == 2 ? "right" : "center");
            var cssProps = {
                "font-size": parameters.FontSize,
                "font-family": parameters.FontFamily,
                "font-weight": parameters.FontWeight,
                "cursor": "default",
                "text-align": textAlignment,
                "padding": "5px"
            };
            if (parameters.FontStyle) {
                var fontStyle = parameters.FontStyle.toLowerCase();
                if (fontStyle != "normal")
                    cssProps["font-style"] = fontStyle;
            }
            if (parameters.Foreground && parameters.Foreground.Color)
                cssProps["color"] = parameters.Foreground.Color;
            fastdom.mutate(function () {
                var btnContentCss = {};
                setTextWrappingOptions(btnContentCss, parameters.TextWrapping, buttonText.indexOf("\n") !== -1, scope.biOS);

                textDiv = $("<div title=\"" + i18next.t1(parameters.ToolTip).replace(/"/g, "&quot;") + "\" style='position:" + (scope.biOS && !parameters.bFitInWindow && !bIsInFittedParent && !parameters.currentPopupId ? "fixed" : "absolute") + ";z-index:2;text-align:center;width:100%;'>" + i18next.t1(buttonText) + "</div>");
                buttoncontainer.css({
                    "display": "flex",
                    "align-items": parameters.VerticalContentAlignment == 0 || parameters.VerticalContentAlignment == 3 ? "flex-start" : (parameters.VerticalContentAlignment == 2 ? "flex-end" : "center")
                });
                $(textDiv).css(btnContentCss).appendTo(buttoncontainer).css(cssProps);
            });
        }
        else {
            var buttonOptions = {
                text: i18next.t1(buttonText),
                type: "normal",
                icon: parameters.icon ? imageFolder + parameters.icon : null,
                height: thisParent.attr("height"),
                width: thisParent.attr("width"),
                disabled: thisParent.hasClass(disabledClass),
                hint: i18next.t1(parameters.ToolTip),
                onContentReady: function (e) {
                    el = e.component.element();
                    var options = { "text-align": parameters.HorizontalContentAlignment == 0 ? "left" : (parameters.HorizontalContentAlignment == 2 ? "right" : "center") };
                    if (parameters.FontFamily)
                        options["font-family"] = parameters.FontFamily;
                    if (parameters.FontSize)
                        options["font-size"] = parameters.FontSize;
                    if (parameters.Background) {
                        if (parameters.Background.Color)
                            options["background"] = parameters.Background.Color;
                        else if (parameters.Background.Source) {
                            options["background"] = "url(\"" + sanitizedImageUrl(parameters.Background.Source) + "\")";
                            options["background-size"] = "100% 100%";
                            options["background-repeat"] = "no-repeat";
                            options["background-position"] = "center center";
                            options["image-rendering"] = "-webkit-optimize-contrast";
                            buttoncontainer.on("mousedown", function (e) {
                                fastdom.mutate(function () {
                                    el.css("background-size", "95% 95%");
                                });
                            }).on("mouseup mouseleave", function (e) {
                                fastdom.mutate(function () {
                                    el.css("background-size", "100% 100%");
                                });
                            });
                        }
                    }

                    registerStateImages(parameters, el, 5);

                    if (parameters.Foreground)
                        options["color"] = parameters.Foreground.Color;
                    if (parameters.FontWeight)
                        options["font-weight"] = fontWeightConverter(parameters.FontWeight);
                    if (parameters.FontStyle) {
                        var fontStyle = parameters.FontStyle.toLowerCase();
                        if (fontStyle != "normal")
                            options["font-style"] = fontStyle;
                    }

                    setObjectBorder(options, parameters, thisPage, buttoncontainer);
                    fastdom.mutate(function () {
                        el.css(options);
                    });

                    var btnContentCss = {
                        "padding": "5px"
                    };
                    setTextWrappingOptions(btnContentCss, parameters.TextWrapping, buttonText.indexOf("\n") !== -1, scope.biOS);

                    fastdom.measure(function () {
                        var btnContent = el.find(".dx-button-content");
                        var btnText = el.find(".dx-button-text");
                        fastdom.mutate(function () {
                            btnContent.css(btnContentCss);
                            btnText.css("vertical-align", parameters.VerticalContentAlignment == 0 || parameters.VerticalContentAlignment == 3 ? "top" : (parameters.VerticalContentAlignment == 2 ? "bottom" : "middle"));
                            fastdom.measure(function () {
                                var btnTextWidth = btnText.width();
                                var btnContentWidth = btnContent.width();
                                var btnTextHeight = btnText.height();
                                var btnContentHeight = btnContent.height();
                                var twcwdiff = btnTextWidth - btnContentWidth;
                                if (twcwdiff > 0)
                                    fastdom.mutate(function () {
                                        btnText.css("margin-left", - (twcwdiff / 2) + "px");
                                    });
                                if (btnTextHeight > btnContentHeight)
                                    fastdom.mutate(function () {
                                        btnContent.addClass("wrap-active");
                                    });
                            });
                        });
                    });
                }
            };
            button = buttoncontainer.dxButton(buttonOptions).dxButton("instance");
            
            $(thisParent).bind(disabledClassEvent, function () {
                if (button) {
                    button.option("disabled", thisParent.hasClass(disabledClass));
                }
            });
        }
    }

    mutateHandler.scheduleMutate(parameters.screenId, mutateFunc);

    var deregisterListenerActiveLanguage = scope.$on(onActiveLanguageChanged, function (e, value) {
        if (button) {
            button.option("text", i18next.t1(buttonText));
            button.option("hint", i18next.t1(parameters.ToolTip));
        }
        if (textDiv) {
            fastdom.mutate(function () {
                $(textDiv).text(i18next.t1(buttonText));
                $(textDiv).attr("title", i18next.t1(parameters.ToolTip).replace(/"/g, "&quot;"));
            });
        }
    });
    deregisterListeners.push(deregisterListenerActiveLanguage);

    function buttonClicked(e) {
        var bOn = e.type === "mousedown" || e.type === "keydown";
        if (bOn && thisParent.hasClass(disabledClass))
            return;
        styleData.toggleButtonStyle(bOn);
    }

    function checkEnableControl(button, thisParent, SVGReferenceId, screenId) {

        var enabled = true;
        if (screenId in scope.dataValues && SVGReferenceId in scope.dataValues[screenId])
            enabled = scope.dataValues[screenId][SVGReferenceId].isGood && !thisParent.hasClass(disabledClass) && scope.dataValues[screenId][SVGReferenceId].isWritable && scope.dataValues[screenId][SVGReferenceId].isUserWritable;
        
        if (button !== null)
            button.option("disabled", enabled);
        else
            styleData.enableDisableStyles(enabled, webKitRenderCorrection);
    }

    function registerStateImages(parameters, parentElement, percentMargin) {
        if (!parameters.ReleasedIcon && !parameters.PressedIcon)
            return;

        var widthHeight = 100 - percentMargin;
        var topLeftMargin = percentMargin / 2;
        var imageDiv = $("<div class='btnImageDiv' style='width:" + widthHeight + "%;height:" + widthHeight + "%;position:absolute;top:" + topLeftMargin + "%;left:" + topLeftMargin + "%;'></div>");
        var imageDivOpts = { "background-repeat": "no-repeat", "image-rendering": "-webkit-optimize-contrast" };
        if (parameters.ImageStretch == 0) { // None
            imageDivOpts["background-position"] = "center center";
        }
        if (parameters.ImageStretch == 1) { // Fill
            imageDivOpts["background-size"] = "100% 100%";
            imageDivOpts["background-position"] = "center center";
        }
        if (parameters.ImageStretch == 2) { // Uniform
            imageDivOpts["background-size"] = "contain";
            imageDivOpts["background-position"] = "center center";
        }
        if (parameters.ImageStretch == 3) { // UniformFill
            imageDivOpts["background-size"] = "cover";
            imageDivOpts["background-position"] = "left top";
        }
        if (parameters.ReleasedIcon)
            imageDivOpts["background-image"] = "url(\"" + sanitizedImageUrl(parameters.ReleasedIcon) + "\")";

        buttoncontainer.on("mousedown", function (e) {
            fastdom.mutate(function () {
                imageDiv.css("background-image", parameters.PressedIcon ? "url(\"" + sanitizedImageUrl(parameters.PressedIcon) + "\")" : "none");
            });
        }).on("mouseup mouseleave", function (e) {
            fastdom.mutate(function () {
                imageDiv.css("background-image", parameters.ReleasedIcon ? "url(\"" + sanitizedImageUrl(parameters.ReleasedIcon) + "\")" : "none");
            });
        });

        fastdom.mutate(function () {
            imageDiv.appendTo(parentElement).css(imageDivOpts);
        });
    }
}