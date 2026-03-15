function initStyles($scope) {
    $scope.applyControlStyle = function ($scope, thisParent, controlcontainer, parameters, webKitRenderCorrection = false) {
        var bStyleApplied = parameters.StyleON && parameters.StyleOFF && $scope.Styles && $scope.Styles[parameters.screenId];
        var controlON;
        var controlOFF;
        var controlNULL;
        var fobjReplacerContainer;
        var bHasStyleNULL = parameters.StyleNULL !== "";

        function iOS_StyleNodePositionFix($styleNode) {
            fastdom.mutate(function () {
                $styleNode.css("transform", "translate(0px, 0px)");
            });
        }

        return (function () {
            if (bStyleApplied) {

                var styleSvg = SVG($scope.Styles[parameters.screenId]);
                
                if (bHasStyleNULL) {
                    controlNULL = styleSvg.find('#' + parameters.StyleNULL);
                    if (controlNULL.length > 0) {
                        controlNULL = controlNULL[0]; //.clone();
                        if ($scope.biOS && !webKitRenderCorrection)
                            iOS_StyleNodePositionFix($(controlNULL.node));
                    }
                }
                controlON = styleSvg.find('#' + parameters.StyleON);
                if (controlON.length > 0) {
                    controlON = controlON[0]; //.clone();
                    if (webKitRenderCorrection) {
                        var nodeON = $(controlON.node);
                        if (nodeON.find("animateTransform").length > 0) {
                            fobjReplacerContainer = document.createElementNS("http://www.w3.org/2000/svg", "g");
                            fobjReplacerContainer.setAttribute("id", thisParent.attr("id"));
                            controlcontainer = fobjReplacerContainer;
                            fastdom.mutate(function () {
                                thisParent.after(fobjReplacerContainer);
                                nodeON.attr({
                                    "width": thisParent.attr("width"),
                                    "height": thisParent.attr("height"),
                                    "x": thisParent.attr("exportedX"),
                                    "y": thisParent.attr("exportedY"),
                                });
                                if (controlNULL && controlNULL.length > 0) {
                                    $(controlNULL.node).attr({
                                        "width": thisParent.attr("width"),
                                        "height": thisParent.attr("height"),
                                        "x": thisParent.attr("exportedX"),
                                        "y": thisParent.attr("exportedY"),
                                    });
                                }
                                nodeON.detach().appendTo(fobjReplacerContainer);
                                //fobjReplacerContainer.html(fobjReplacerContainer.html());
                                thisParent.remove();
                                updateControlSecurity(thisParent[0], fobjReplacerContainer);
                            });
                        }
                    }
                    if ($scope.biOS && !webKitRenderCorrection)
                        iOS_StyleNodePositionFix($(controlON.node));
                }

                if (parameters.Background && parameters.Background.Color) {
                    fastdom.measure(function () {
                        var tagsBG = styleSvg.find("[tag='BG']");
                        fastdom.mutate(function () {
                            for (var i = 0; i < tagsBG.length; i++) {
                                tagsBG[i].attr("fill", parameters.Background.Color);
                            }
                        });
                    });
                }

                controlOFF = styleSvg.find('#' + parameters.StyleOFF);
                if (controlOFF.length > 0) {
                    controlOFF = controlOFF[0]; //.clone();
                    if (webKitRenderCorrection && fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlOFF.node).attr({
                                "width": thisParent.attr("width"),
                                "height": thisParent.attr("height"),
                                "x": thisParent.attr("exportedX"),
                                "y": thisParent.attr("exportedY"),
                            });
                        });
                    }
                    if ($scope.biOS && !webKitRenderCorrection)
                        iOS_StyleNodePositionFix($(controlOFF.node));
                }

                fastdom.mutate(function () {
                    $(controlOFF.node).appendTo(controlcontainer);
                });

                //$("<img src='data:image/svg+xml;base64," + btoa(controlOFF.node.outerHTML) + "'/>").appendTo(controlcontainer);

                var controlStyles = { "ON": controlON, "OFF": controlOFF, "NULL": controlNULL };
            }
            return {
                "eventsTarget": webKitRenderCorrection && fobjReplacerContainer ? $(fobjReplacerContainer) : thisParent,
                "styleApplied": bStyleApplied,
                "enableDisableStyles": function (enabled, webKitRenderCorrection) {
                    if (!enabled) {
                        controlStyles.ON.filterWith(function (add) {
                            add.gaussianBlur(5);
                        });
                        controlStyles.OFF.filterWith(function (add) {
                            add.gaussianBlur(5);
                        });
                        if (controlStyles.NULL)
                            controlStyles.NULL.filterWith(function (add) {
                                add.gaussianBlur(5);
                            });
                    }
                    else {
                        controlStyles.ON.unfilter();
                        controlStyles.OFF.unfilter();
                        if (controlStyles.NULL)
                            controlStyles.NULL.unfilter();
                    }
                },
                "activateStyleON": function () {
                    if ($scope.biOS && !fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlON.node).prependTo(controlcontainer);
                            $(controlON.node).css("width", "100%");
                        });
                    }
                    else {
                        if (!isFirefox() || controlcontainer.find($(controlON.node)).length == 0)
                            fastdom.mutate(function () {
                                $(controlON.node).appendTo(controlcontainer);
                            });
                    }

                    if (parameters.BackgroundOn && parameters.BackgroundOn.Color) {
                        var tagsBG_ON = $(controlON.node).find("[tag='BG']");
                        for (var i = 0; i < tagsBG_ON.length; i++) {
                            $(tagsBG_ON[i]).attr("fill", parameters.BackgroundOn.Color);
                        }
                    }
                    if (!$scope.biOS || fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlOFF.node).remove();
                            if (bHasStyleNULL)
                                $(controlNULL.node).remove();
                        });
                    }
                    else {
                        fastdom.mutate(function () {
                            $(controlOFF.node).css("width", "0px");
                            if (bHasStyleNULL)
                                $(controlNULL.node).css("width", "0px");
                        });
                    }
                },
                "activateStyleOFF": function () {
                    if ($scope.biOS && !fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlOFF.node).prependTo(controlcontainer);
                            $(controlOFF.node).css("width", "100%");
                        });
                    }
                    else
                        fastdom.mutate(function () {
                            $(controlOFF.node).appendTo(controlcontainer);
                        });
                    var bHasColor = (parameters.SourceSymbolLinked && parameters.SvgBackground && parameters.SvgBackground.Color) || (!parameters.SourceSymbolLinked && parameters.Background && parameters.Background.Color);
                    if (bHasColor) {
                        var bgColor = parameters.SourceSymbolLinked ? parameters.SvgBackground.Color : parameters.Background.Color;
                        var tagsBG_OFF = $(controlOFF.node).find("[tag='BG']");
                        for (var j = 0; j < tagsBG_OFF.length; j++) {
                            $(tagsBG_OFF[j]).attr("fill", urlReferenceReplace(bgColor, parameters.screenId));
                        }
                    }
                    if (!$scope.biOS || fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlON.node).remove();
                            if (bHasStyleNULL)
                                $(controlNULL.node).remove();
                        });
                    }
                    else {
                        fastdom.mutate(function () {
                            $(controlON.node).css("width", "0px");
                            if (bHasStyleNULL)
                                $(controlNULL.node).css("width", "0px");
                        });
                    }
                },
                "activateStyleNULL": function () {
                    if ($scope.biOS && !fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlNULL.node).prependTo(controlcontainer);
                            $(controlNULL.node).css("width", "100%");
                        });
                    }
                    else
                        fastdom.mutate(function () {
                            $(controlNULL.node).appendTo(controlcontainer);
                        });
                    if (parameters.BackgroundOn && parameters.BackgroundOn.Color) {
                        var tagsBG_NULL = $(controlNULL.node).find("[tag='BG']");
                        for (var i = 0; i < tagsBG_NULL.length; i++) {
                            $(tagsBG_NULL[i]).attr("fill", parameters.BackgroundOn.Color);
                        }
                    }
                    if (!$scope.biOS || fobjReplacerContainer) {
                        fastdom.mutate(function () {
                            $(controlON.node).remove();
                            $(controlOFF.node).remove();
                        });
                    }
                    else {
                        fastdom.mutate(function () {
                            $(controlON.node).css("width", "0px");
                            $(controlOFF.node).css("width", "0px");
                        });
                    }
                },
                "toggleButtonStyle": function (bOn) {
                    var styleNode = bOn ? $(controlON.node) : $(controlOFF.node);
                    styleNode.appendTo(controlcontainer);
                    var bHasColor = (parameters.SourceSymbolLinked && parameters.SvgBackground && parameters.SvgBackground.Color) || (!parameters.SourceSymbolLinked && parameters.Background && parameters.Background.Color);
                    if (bHasColor) {
                        var bgColor = parameters.SourceSymbolLinked ? parameters.SvgBackground.Color : parameters.Background.Color;
                        var tagsBG = styleNode.find("[tag='BG']");
                        for (var i = 0; i < tagsBG.length; i++) {
                            $(tagsBG[i]).attr("fill", urlReferenceReplace(bgColor, parameters.screenId));
                        }
                    }
                    (bOn ? $(controlOFF.node) : $(controlON.node)).remove();
                },
                "setStrokeAndFill": function (bIsButton) {
                    if (bIsButton) {
                        if (parameters.SourceSymbolLinked && parameters.SvgBackground && parameters.SvgBackground.Color) {
                            for (let styleKey in controlStyles) {
                                let style = controlStyles[styleKey];
                                if (style)
                                    $(style.node).find("[tag='BG']").attr("fill", urlReferenceReplace(parameters.SvgBackground.Color, parameters.screenId));
                            }
                        }

                        if (parameters.BorderBrush && parameters.BorderBrush.Color) {
                            for (let styleKey in controlStyles) {
                                let style = controlStyles[styleKey];
                                if (style)
                                    $(style.node).find("[tag='BG']").attr("stroke", urlReferenceReplace(parameters.BorderBrush.Color, parameters.screenId));
                            }
                        }
                    }
                    else {
                        if (parameters.SourceSymbolLinked) {
                            if (controlStyles.OFF && parameters.SvgBackground && parameters.SvgBackground.Color)
                                $(controlStyles.OFF.node).find("[tag='BG']").attr("fill", urlReferenceReplace(parameters.SvgBackground.Color, parameters.screenId));
                            if ((controlStyles.ON || controlStyles.NULL) && parameters.SvgBackgroundOn && parameters.SvgBackgroundOn.Color) {
                                if (controlStyles.ON)
                                    $(controlStyles.ON.node).find("[tag='BG']").attr("fill", urlReferenceReplace(parameters.SvgBackgroundOn.Color, parameters.screenId));
                                if (controlStyles.NULL)
                                    $(controlStyles.NULL.node).find("[tag='BG']").attr("fill", urlReferenceReplace(parameters.SvgBackgroundOn.Color, parameters.screenId));
                            }
                            if (parameters.BorderBrush && parameters.BorderBrush.Color) {
                                if (controlStyles.ON)
                                    $(controlStyles.ON.node).find("[tag='BG']").attr("stroke", urlReferenceReplace(parameters.BorderBrush.Color, parameters.screenId));
                                if (controlStyles.OFF)
                                    $(controlStyles.OFF.node).find("[tag='BG']").attr("stroke", urlReferenceReplace(parameters.BorderBrush.Color, parameters.screenId));
                                if (controlStyles.NULL)
                                    $(controlStyles.NULL.node).find("[tag='BG']").attr("stroke", urlReferenceReplace(parameters.BorderBrush.Color, parameters.screenId));
                            }
                        }
                    }
                }
            };
        })();
    };
}