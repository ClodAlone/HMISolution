

function showPopup(arg/*
    { $scope,
        parent,
        id,
        content,
        showtitle = false,
        titletxt = "",
        dragenabled = false,
        closeonoutsideclick = false,
        showOk = false,
        showCancel = false,
        checkresult = false,
        showClose = true,
        resizeenabled = false,
        w = undefined,
        h = undefined}*/) {

    var positionHandlers = {};
    var resizeHandlers = {};

    var repositionPopup = function (jparent, ip, overlayContent) {
        return positionHandlers[jparent] || (positionHandlers[jparent] = function (event) {
            ip.option("position", ip.option("position"));

            restoreOverlaySize(overlayContent);
        });
    }

    var onRotateResizePopup = function (jparent, requestw, requesth, initialh, ip, popupContent, overlayContent) {
        return resizeHandlers[jparent] || (resizeHandlers[jparent] = function (event) {
            if (requesth) {
                ip.option('height', Math.min(window.innerHeight, requesth));
                if (parseFloat(popupContent.css("height")) < initialh) {
                    var toolbar = overlayContent.find(".dx-toolbar");
                    popupContent.css("height", overlayContent.height() - (toolbar.length > 0 ? toolbar[0].clientHeight : 0) - parseFloat(popupContent.css("paddingTop")) - parseFloat(popupContent.css("paddingBottom")));
                }
            }

            restoreOverlaySize(overlayContent);
        });
    }

    var restoreOverlaySize = function (overlayContent) {
        var customW = overlayContent.attr("initialwidth");
        if (typeof customW !== 'undefined' && customW !== false)
            overlayContent.width(customW);

        var customH = overlayContent.attr("initialheight");
        if (typeof customH !== 'undefined' && customH !== false)
            overlayContent.height(customH);
    }

    if (!arg.parent) {
        var d = new Date();
        arg.parent = "parentPopup" + d.getTime();
    }

    arg.content.data(popupparameters, arg.parent);

    var jparent = '#' + arg.parent;

    var iDiv = jQuery('<div/>', {
        id: arg.parent,
        class: 'popup'
    }).appendTo(ROOT);

    var requestw;
    var requesth;
    var lastX = NaN;
    var lastY = NaN;
    var bFirstAnim = true;
    var bPopupMoving = false;
    var bFits = false;
    var bPopupContentDataReady = false;
    var bToBeResized;

    var deregisterListenerPopup = arg.$scope.$on(onPopupSizeRequest + arg.parent, function (value, obj) {
        if (!bPopupContentDataReady)
            bPopupContentDataReady = true;
        requestw = obj.w;
        requesth = obj.h;
        windowState = obj.windowState;
        windowStyle = obj.windowStyle;
        //console.log("POPUPSIZEREQUEST", obj, arg, requestw, requesth, windowState, windowStyle, bFits);
        bFits = obj.bFitInWindow;
        bHasKeep = obj.bHasKeep;
        
        if (requesth && arg.showtitle)
            requesth += 100;
        if (requesth && (arg.showOk || arg.showCancel))
            requesth += 100;

        try { var ip = $(jparent).dxPopup("instance"); } catch (e) { }

        if (ip) {
            if (obj.titletxt)
                ip.option("title", obj.titletxt);
            if (bFits && !bHasKeep && !arg.notScrollable && arg.resizeenabled)
                unscroll(ip, arg);

            var popupContent = ip.content();
            var overlayContent = popupContent.parent();

            var popupContent = ip.content();
            var overlayContent = popupContent.parent();
            var bWidthResized;

            if (requestw) {
                var widthPadding = parseFloat(popupContent.css("paddingLeft")) * 2;
                if (bFits && window.innerWidth < requestw + widthPadding)
                    bWidthResized = true;
                ip.option('width', Math.min(window.innerWidth, requestw + widthPadding));
            }
            if (requesth) {
                ip.option('height', Math.min(window.innerHeight, requesth));
                if (parseFloat(popupContent.css("height")) < obj.h) {
                    var toolbar = overlayContent.find(".dx-toolbar");
                    popupContent.css("height", overlayContent.height() - (toolbar.length > 0 ? toolbar[0].clientHeight : 0) - parseFloat(popupContent.css("paddingTop")) - parseFloat(popupContent.css("paddingBottom")));
                }
            }

            if ((requesth || requestw) && !ip.bHasRotationHandler) {
                ip.bHasRotationHandler = true;
                window.addEventListener('resize', onRotateResizePopup(jparent, requestw, requesth, obj.h, ip, popupContent, overlayContent));
            }

            var overlayContentSize = {
                w: null,
                h: null
            };
            if (windowStyle === "none") {
                if (!overlayContent.hasClass("toolbarHidden")) {
                    overlayContent.addClass("toolbarHidden");
                    var toolbar = overlayContent.find(".dx-toolbar");
                    popupContent.attr({
                        "originalPadding": parseFloat(popupContent.css("padding")) * 2,
                        "originalToolbarHeight": toolbar.length > 0 ? parseFloat(toolbar[0].clientHeight) : 0
                    });
                    overlayContent.find(".dx-toolbar").hide();
                    popupContent.css("padding", "4px");
                }
                overlayContentSize.w = overlayContent.width() - parseFloat(popupContent.attr("originalPadding")) + parseFloat(popupContent.css("padding")) * 2;
                overlayContentSize.h = overlayContent.height() - parseFloat(popupContent.attr("originalPadding")) - parseFloat(popupContent.attr("originalToolbarHeight"));
            }
            if (bHasKeep) {
                overlayContentSize.w = overlayContentSize.w || overlayContent.width();
                overlayContentSize.h = overlayContentSize.h || overlayContent.width();
                var originalAspectRatio = obj.w / obj.h;
                var currentAspectRatio = overlayContentSize.w / overlayContentSize.h;
                var aspectRatioLoweringFactor = Math.min(originalAspectRatio / currentAspectRatio, currentAspectRatio / originalAspectRatio);
                
                if (aspectRatioLoweringFactor !== 1) { //greater dimension must be lowered mantaining aspect ratio
                    if (currentAspectRatio > originalAspectRatio) {
                        if (bFits && arg.$scope.biOS)
                            overlayContentSize.w = Math.min(window.innerWidth - (isSafari() ? 65 : 110), overlayContentSize.w);
                        var newW = overlayContentSize.w * aspectRatioLoweringFactor;
                        overlayContent.css("left", (overlayContentSize.w - newW) / 2);
                        overlayContentSize.w = newW;
                    }
                    else {
                        if (bFits && arg.$scope.biOS)
                            overlayContentSize.h = Math.min(window.innerHeight - (isSafari() ? 65 : 110), overlayContentSize.h);
                        var newH = overlayContentSize.h * aspectRatioLoweringFactor;
                        overlayContent.css("top", (overlayContentSize.h - newH) / 2);
                        overlayContentSize.h = newH;
                    }
                }
            }

            var newX;
            var newY;
            
            if (!isNaN(parseFloat(obj.x)) && !isNaN(parseFloat(obj.y))) {
                if (!arg.disableanimation) {
                    newX = obj.x;
                    newY = obj.y;
                }
                else
                    $(jparent).dxPopup("instance").option("position",
                        {
                            my: "left top", at: "left top", of: $(ROOT), offset: { x: parseFloat(obj.x), y: parseFloat(obj.y) }
                        });
            }
            else if (!isNaN(parseFloat(arg.x)) && !isNaN(parseFloat(arg.y))) {
                if (!arg.disableanimation) {
                    newX = arg.x;
                    newY = arg.y;
                }
                else
                    $(jparent).dxPopup("instance").option("position",
                        {
                            my: "left top", at: "left top", of: $(ROOT), offset: { x: parseFloat(arg.x), y: parseFloat(arg.y) }
                        });
            }
            else if (!isNaN(parseFloat(obj.x))) {
                if (!arg.disableanimation)
                    newX = obj.x;
                else
                    $(jparent).dxPopup("instance").option("position",
                        {
                            my: "left", at: "left", of: $(ROOT), offset: { x: parseFloat(obj.x) }
                        });
            }
            else if (!isNaN(parseFloat(obj.y))) {
                if (!arg.disableanimation)
                    newY = obj.y;
                else
                    $(jparent).dxPopup("instance").option("position",
                        {
                            my: "top", at: "top", of: $(ROOT), offset: { y: parseFloat(obj.y) }
                        });
            }

            var newOverlayContentOptions = {};
            var newWidth;
            if (overlayContentSize.w !== null) {
                if (arg.$scope.biOS) {
                    newWidth = (!bHasKeep && bFits) ? Math.min(window.innerWidth - (isSafari() ? 65 : 110), overlayContentSize.w) : overlayContentSize.w;
                    ip.option('width', newWidth);
                    if (!isSafari() && !ip.bHasRepositionHandler) {
                        ip.bHasRepositionHandler = true;
                        window.addEventListener('resize', repositionPopup(jparent, ip, overlayContent));
                    }
                }
                else {
                    newWidth = overlayContentSize.w;
                    newOverlayContentOptions.width = + parseFloat(newWidth).toFixed(2);
                }
            }
            if (overlayContentSize.h !== null) {
                var heightReductionRatio = 1;
                if (bFits && bWidthResized)
                    try { heightReductionRatio = requestw / newWidth; } catch { }

                if (arg.$scope.biOS) {
                    var newHeight = (!bHasKeep && bFits) ? Math.min(window.innerHeight - (isSafari() ? 65 : 110), overlayContentSize.h / heightReductionRatio) : overlayContentSize.h;
                    ip.option('height', newHeight);
                }
                else {
                    newOverlayContentOptions.height = + parseFloat(overlayContentSize.h / heightReductionRatio).toFixed(2);
                }
            }
            
            if (newOverlayContentOptions.width)
                overlayContent.width(newOverlayContentOptions.width);
            if (newOverlayContentOptions.height)
                overlayContent.height(newOverlayContentOptions.height);
            
            var contentScrollContainer = popupContent.find(".contentScrollContainer");
            if (contentScrollContainer.length > 0) {
                var contentMargin = parseFloat(overlayContent.css("border-width")) * 2 + parseFloat(popupContent.css("padding")) * 2;
                contentScrollContainer.dxScrollView("instance").option({
                    "width": newOverlayContentOptions.width - contentMargin,
                    "height": newOverlayContentOptions.height - contentMargin
                });
            }

            if (arg.avoidCentering) {
                var overlayRect = overlayContent[0].getBoundingClientRect();
                var bExceedsHeight = (overlayRect.y + overlayRect.height) > window.innerHeight;
                var bExceedsWidth = (overlayRect.x + overlayRect.width) > window.innerWidth;
                if (bExceedsHeight || bExceedsWidth) {
                    let overlaySize = {
                        width: overlayContent.width() + parseFloat(overlayContent.css("border-width")) * 2,
                        height: overlayContent.height() + parseFloat(overlayContent.css("border-width")) * 2
                    };
                    if (bExceedsHeight) {
                        var newYOffset;
                        if (overlayRect.y - overlayRect.height < 0)
                            newYOffset = 0;
                        else
                            newYOffset = ip.option("position.offset.y") - overlayRect.height;
                        ip.option("position.offset.y", newYOffset);
                    }
                    if (bExceedsWidth) {
                        var newXOffset;
                        if (overlayRect.x - overlayRect.width < 0)
                            newXOffset = 0;
                        else
                            newXOffset = ip.option("position.offset.x") - overlayRect.width
                        ip.option("position.offset.x", newXOffset);
                    }
                    overlayContent.css(overlaySize);
                }
            }

            if (!arg.disableanimation && (newX || newY)) {
                setTimeout(function () {
                    var bMovePopup = true;
                    var popupContent = ip.content();
                    if (obj.videoW && obj.videoH) {
                        var sensibilityX = obj.videoW * 0.005;
                        var sensibilityY = obj.videoH * 0.005;
                        //newX = newX - Math.abs(obj.boxW - $(popupContent).width()) / 2;
                        var bForceMoving = isNaN(lastX) && isNaN(lastY);
                        if (!bForceMoving) {
                            var popupParent = popupContent.parent();
                            var popupParentOffset = popupParent.offset();
                            if (popupParentOffset.left < 0)
                                newX = 0;
                            else {
                                var finalPopupMaxX = popupParentOffset.left + popupParent.width() + 10 + (newX - lastX);
                                if (finalPopupMaxX > obj.windowSize.w)
                                    newX -= finalPopupMaxX - obj.windowSize.w;
                            }
                            if (popupParentOffset.top < 0)
                                newY = 0;
                            else {
                                var finalPopupMaxY = popupParentOffset.top + popupParent.height() + (newY - lastY);
                                if (finalPopupMaxY > obj.windowSize.h)
                                    newY -= finalPopupMaxY - obj.windowSize.h;
                            }
                        }
                        bMovePopup = bForceMoving || Math.abs(newX - lastX) >= sensibilityX || Math.abs(newY - lastY) >= sensibilityY;
                    }
                    if (bMovePopup) {
                        if (!bPopupMoving) {
                            bPopupMoving = true;
                            var moveobj = {};
                            if (newX) {
                                lastX = newX;
                                moveobj.left = String(newX);
                            }
                            if (newY) {
                                lastY = newY;
                                moveobj.top = String(newY);
                            }
                            popupContent.parent().animate(moveobj, bFirstAnim ? 0 : 200, function () {
                                bPopupMoving = false;
                            });
                            if (bFirstAnim)
                                bFirstAnim = false;
                        }
                    }
                }, 200);
            }

            overlayContent.attr("initialwidth", overlayContent.width());
            overlayContent.attr("initialheight", overlayContent.height());

            popupContent.css({
                "max-width": "calc(100% - " + parseFloat(popupContent.css("paddingLeft")) * 2 + "px)",
                "max-height": "calc(100% - " + parseFloat(popupContent.css("paddingTop")) * 2 + "px)"
            });

            if (bToBeResized) {
                if (!arg.notScrollable)
                    initPopupScrollbars(popupContent);
                resizePopupContent(ip, popupContent);
            }
        }
    });

    var deregisterListenerClosePopup = arg.$scope.$on(onPopupCloseRequest + arg.parent, function (value) {
        try {
            $(jparent).dxPopup("instance").hide();
        } catch (e) {
            if (arg.$scope.biOS && !isSafari()) {
                try {
                    if (jparent in positionHandlers) {
                        window.removeEventListener('resize', positionHandlers[jparent]);
                        delete positionHandlers[jparent];
                    }
                    var ip = $(jparent).dxPopup("instance");
                    ip.bHasRepositionHandler = false;
                } catch (e) { }
            }

            try {
                if (jparent in resizeHandlers) {
                    window.removeEventListener('resize', resizeHandlers[jparent]);
                    delete resizeHandlers[jparent];
                }
                var ip = $(jparent).dxPopup("instance");
                ip.bHasRotationHandler = false;
            }
            catch (e) { }

            iDiv.remove();
            if (deregisterListenerPopup)
                deregisterListenerPopup();
            if (deregisterListenerClosePopup)
                deregisterListenerClosePopup();
            if (deregisterListenerMovePopup)
                deregisterListenerMovePopup();
        }
    });

    var deregisterListenerMovePopup = arg.$scope.$on(onMovePopup + arg.parent, function (value) {
        try { $(jparent).dxPopup("instance").option("position.offset", value.x + " " + value.y); } catch (e) { }
    });

    var closeConfirmed = false;
    var dialogresult = false;

    var popupContainer;
    if (!arg.notScrollable)
        popupContainer = $('<div class="contentScrollContainer" />');

    var popupOptions = {
        container: ROOT,
        shadingColor: defShadingColor,
        shading: !arg.noshading,
        showTitle: arg.showtitle,
        title: arg.titletxt,
        height: 'auto',
        width: 'auto',
        dragEnabled: arg.dragenabled,
        resizeEnabled: arg.resizeenabled,
        closeOnOutsideClick: arg.closeonoutsideclick,
        showCloseButton: arg.showclose,
        animation: {
            "show": { duration: 0 },
            "hide": { duration: 0 }
        },
        contentTemplate: function (contentElement) {

            if (!popupContainer)
                popupContainer = contentElement;

            if (arg.showOk || arg.showCancel)
                var buttonsContainer = $("<div class='popupButtonsContainer' style='text-align:center;'><div class='button' id='showButton'></div></div>").appendTo(popupContainer);
            
            if (arg.showOk) {
                var onOkChosen = function () {
                    dialogresult = true;
                    $(jparent).dxPopup("instance").hide();
                };
                var btnOk = $("<div id='btnOk'>").dxButton({
                    text: i18next.t('OkText'),
                    icon: "check",
                    onClick: onOkChosen
                });
                popupContainer.off("keypress");
                popupContainer.on("keypress", function (e) {
                    if (e.keyCode === 13) {
                        onOkChosen();
                    }
                });
                buttonsContainer.append(btnOk);
            }

            if (arg.showCancel) {
                var btnCancel = $("<div id='btnCancel'>").dxButton({
                    text: i18next.t(arg.showOk ? 'CancelText' : 'CloseText'),
                    icon: "close",
                    onClick: function () {
                        dialogresult = false;
                        $(jparent).dxPopup("instance").hide();
                    }
                });
                buttonsContainer.append(btnCancel);
            }
        },
        onShown: function (e) {
            popupContainer.prepend(arg.content);
            var popupContent = e.component.content();

            if (!bPopupContentDataReady) {
                bToBeResized = true;
                if (!arg.notScrollable)
                    initPopupScrollbars(popupContent);
            }
            
            if (arg.hideBorder) {
                popupContent.parent().css({
                    "border-style": "none",
                    "box-shadow": "none"
                });
            }
        },
        onHidden: function (options) {
            if (arg.$scope.biOS && !isSafari()) {
                try {
                    if (jparent in positionHandlers) {
                        window.removeEventListener('resize', positionHandlers[jparent]);
                        delete positionHandlers[jparent];
                    }
                    var ip = $(jparent).dxPopup("instance");
                    ip.bHasRepositionHandler = false;
                } catch (e) { }
            }

            try {
                if (jparent in resizeHandlers) {
                    window.removeEventListener('resize', resizeHandlers[jparent]);
                    delete resizeHandlers[jparent];
                }
                var ip = $(jparent).dxPopup("instance");
                ip.bHasRotationHandler = false;
            }
            catch (e) { }

            iDiv.remove();
            deregisterListenerPopup();
            deregisterListenerClosePopup();
            deregisterListenerMovePopup();
            if (arg.onHiddenFunc)
                arg.onHiddenFunc(jparent);
        },
        onContentReady: function (e) {
            arg.$scope.$broadcast(contentReady + arg.id, e);
            if (arg.onContentReadyFunc) {
                arg.onContentReadyFunc(e.component.content().parent(), jparent);
            }
        },
        onHiding: function (options) {
            if (arg.checkresult && !closeConfirmed) {
                options.cancel = true;

                var closeoption = { canClose: true, dialogResult: dialogresult };

                var deregisterListener = arg.$scope.$on(hideEvent + arg.id, function (value) {

                    if (closeoption.canClose) { // Yes: confirm close
                        closeConfirmed = true;
                        deregisterListener();
                        setTimeout(function () {

                            $(jparent).dxPopup("instance").hide();
                        }, 10);
                    } else { // No: don't close popup
                        dialogresult = false;
                        closeConfirmed = false;
                    }
                });
                arg.$scope.$broadcast(hidingEvent + arg.id, closeoption);
            }
        },
        onResize: function (e) {
            arg.$scope.$broadcast(onPopupResize + arg.parent, arg.parent);
        }
    };
    if (arg.h)
        popupOptions["height"] = isNaN(arg.h) ? arg.h : Math.min(window.innerHeight, arg.h);
    if (arg.w)
        popupOptions["width"] = isNaN(arg.w) ? arg.w : Math.min(window.innerWidth, arg.w);

    $(jparent).dxPopup(popupOptions);

    try { var popupinstance = $(jparent).dxPopup("instance"); } catch (e) { }
    if (popupinstance) {
        //popupinstance.option("position",
        //    {
        //        my: "left top", at: "left top", of: $(ROOT) /*, offset: String(newX) + " " + String(newY)*/
        //    });
        popupinstance.show();

        if (arg.transparent) {
            var popupcontent = popupinstance.content();
            var parent = popupcontent.parent();
            parent.css({
                "background": "transparent"
            });
        }
    }

    var unscroll = function (popupinstance, arg) {
        arg.notScrollable = true;
        var scrollContainer = popupinstance.element().find(".contentScrollContainer").first();
        if (scrollContainer.length > 0) {
            arg.content.detach().prependTo(scrollContainer.parent());
            scrollContainer.remove();
        }
    }

    var initPopupScrollbars = function (popupContent) {
        var sv;
        try {
            sv = popupContainer.dxScrollView("instance");
        }
        catch { }
        if (!sv) {
            popupContent.append(popupContainer);
            var scrollOptions = {
                width: '100%',
                height: '100%'
            };
            popupContainer.dxScrollView(scrollOptions);
            sv = popupContainer.dxScrollView("instance");
        }
        if (!bFits) {
            sv.option({
                direction: 'both',
                useNative: false,
                bounceEnabled: false
                //scrollByContent: true;
            });
        }
    };

    var resizePopupContent = function (component, popupContent) {
        var widthPadding = parseInt(popupContent.css("paddingLeft"), 10) * 2;
        if (requesth && requestw) {
            if (requestw + widthPadding > window.innerWidth) {
                if (!arg.$scope.biOS)
                    component.option("width", window.innerWidth);
                if (bFits && !arg.notScrollable)
                    popupContent.find(".dx-scrollview-content, .dx-scrollable-content").css({ "width": "100%" });
            }
            if (requesth > window.innerHeight) {
                if (!arg.$scope.biOS)
                    component.option("height", window.innerHeight);
                if (bFits && !arg.notScrollable)
                    popupContent.find(".dx-scrollview-content, .dx-scrollable-content").css({ "height": "100%" });
            }
        }
    }
}