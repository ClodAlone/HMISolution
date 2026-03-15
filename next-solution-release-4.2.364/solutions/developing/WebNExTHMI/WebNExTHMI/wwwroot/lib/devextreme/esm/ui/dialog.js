/**
 * DevExtreme (esm/ui/dialog.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    getWidth
} from "../core/utils/size";
import $ from "../core/renderer";
import {
    Component
} from "../core/component";
import Action from "../core/action";
import devices from "../core/devices";
import config from "../core/config";
import {
    resetActiveElement
} from "../core/utils/dom";
import {
    Deferred
} from "../core/utils/deferred";
import {
    isPlainObject
} from "../core/utils/type";
import {
    each
} from "../core/utils/iterator";
import {
    extend
} from "../core/utils/extend";
import {
    getWindow
} from "../core/utils/window";
import eventsEngine from "../events/core/events_engine";
import {
    value as getViewport
} from "../core/utils/view_port";
import messageLocalization from "../localization/message";
import errors from "./widget/ui.errors";
import Popup from "./popup";
import {
    ensureDefined
} from "../core/utils/common";
var window = getWindow();
var DEFAULT_BUTTON = {
    text: "OK",
    onClick: function() {
        return true
    }
};
var DX_DIALOG_CLASSNAME = "dx-dialog";
var DX_DIALOG_WRAPPER_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-wrapper");
var DX_DIALOG_ROOT_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-root");
var DX_DIALOG_CONTENT_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-content");
var DX_DIALOG_MESSAGE_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-message");
var DX_DIALOG_BUTTONS_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-buttons");
var DX_DIALOG_BUTTON_CLASSNAME = "".concat(DX_DIALOG_CLASSNAME, "-button");
var DX_BUTTON_CLASSNAME = "dx-button";
export var FakeDialogComponent = Component.inherit({
    ctor: function(element, options) {
        this.callBase(options)
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: {
                platform: "ios"
            },
            options: {
                width: 276
            }
        }])
    }
});
export var custom = function(options) {
    var _options$title;
    var deferred = new Deferred;
    var defaultOptions = (new FakeDialogComponent).option();
    options = extend(defaultOptions, options);
    var $element = $("<div>").addClass(DX_DIALOG_CLASSNAME).appendTo(getViewport());
    var isMessageDefined = "message" in options;
    var isMessageHtmlDefined = "messageHtml" in options;
    if (isMessageDefined) {
        errors.log("W1013")
    }
    var messageHtml = String(isMessageHtmlDefined ? options.messageHtml : options.message);
    var $message = $("<div>").addClass(DX_DIALOG_MESSAGE_CLASSNAME).html(messageHtml);
    var popupToolbarItems = [];
    each(options.buttons || [DEFAULT_BUTTON], (function() {
        var action = new Action(this.onClick, {
            context: popupInstance
        });
        popupToolbarItems.push({
            toolbar: "bottom",
            location: devices.current().android ? "after" : "center",
            widget: "dxButton",
            options: extend({}, this, {
                onClick: function() {
                    var result = action.execute(...arguments);
                    hide(result)
                }
            })
        })
    }));
    var popupInstance = new Popup($element, extend({
        title: null !== (_options$title = options.title) && void 0 !== _options$title ? _options$title : "",
        showTitle: ensureDefined(options.showTitle, true),
        dragEnabled: ensureDefined(options.dragEnabled, true),
        height: "auto",
        width: options.width,
        showCloseButton: options.showCloseButton || false,
        ignoreChildEvents: false,
        onContentReady: function(args) {
            args.component.$content().addClass(DX_DIALOG_CONTENT_CLASSNAME).append($message)
        },
        onShowing: function(e) {
            e.component.bottomToolbar().addClass(DX_DIALOG_BUTTONS_CLASSNAME).find(".".concat(DX_BUTTON_CLASSNAME)).addClass(DX_DIALOG_BUTTON_CLASSNAME);
            resetActiveElement()
        },
        onShown: function(e) {
            var $firstButton = e.component.bottomToolbar().find(".".concat(DX_BUTTON_CLASSNAME)).first();
            eventsEngine.trigger($firstButton, "focus")
        },
        onHiding: function() {
            deferred.reject()
        },
        toolbarItems: popupToolbarItems,
        animation: {
            show: {
                type: "pop",
                duration: 400
            },
            hide: {
                type: "pop",
                duration: 400,
                to: {
                    opacity: 0,
                    scale: 0
                },
                from: {
                    opacity: 1,
                    scale: 1
                }
            }
        },
        rtlEnabled: config().rtlEnabled,
        position: {
            boundaryOffset: {
                h: 10,
                v: 0
            }
        }
    }, options.popupOptions));
    popupInstance.$wrapper().addClass(DX_DIALOG_WRAPPER_CLASSNAME);
    if (options.position) {
        popupInstance.option("position", options.position)
    }
    popupInstance.$wrapper().addClass(DX_DIALOG_ROOT_CLASSNAME);

    function hide(value) {
        deferred.resolve(value);
        popupInstance.hide().done((function() {
            popupInstance.$element().remove()
        }))
    }
    return {
        show: function() {
            if ("android" === devices.real().platform) {
                var isPortrait = getHeight(window) > getWidth(window);
                var width = isPortrait ? "80%" : "60%";
                popupInstance.option({
                    width: width
                })
            }
            popupInstance.show();
            return deferred.promise()
        },
        hide: hide
    }
};
export var alert = function(messageHtml) {
    var title = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : "";
    var showTitle = arguments.length > 2 ? arguments[2] : void 0;
    var options = isPlainObject(messageHtml) ? messageHtml : {
        title: title,
        messageHtml: messageHtml,
        showTitle: showTitle,
        dragEnabled: showTitle
    };
    return custom(options).show()
};
export var confirm = function(messageHtml) {
    var title = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : "";
    var showTitle = arguments.length > 2 ? arguments[2] : void 0;
    var options = isPlainObject(messageHtml) ? messageHtml : {
        title: title,
        messageHtml: messageHtml,
        showTitle: showTitle,
        buttons: [{
            text: messageLocalization.format("Yes"),
            onClick: function() {
                return true
            }
        }, {
            text: messageLocalization.format("No"),
            onClick: function() {
                return false
            }
        }],
        dragEnabled: showTitle
    };
    return custom(options).show()
};
