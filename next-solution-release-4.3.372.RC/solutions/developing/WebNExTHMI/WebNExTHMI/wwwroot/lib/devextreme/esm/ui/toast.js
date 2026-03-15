/**
 * DevExtreme (esm/ui/toast.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import {
    getWindow
} from "../core/utils/window";
var window = getWindow();
import domAdapter from "../core/dom_adapter";
import eventsEngine from "../events/core/events_engine";
import readyCallbacks from "../core/utils/ready_callbacks";
import {
    noop
} from "../core/utils/common";
import {
    isString
} from "../core/utils/type";
import {
    extend
} from "../core/utils/extend";
import {
    inArray
} from "../core/utils/array";
import pointerEvents from "../events/pointer";
import registerComponent from "../core/component_registrator";
import Overlay from "./overlay/ui.overlay";
import {
    isMaterial
} from "./themes";
var ready = readyCallbacks.add;
var TOAST_CLASS = "dx-toast";
var TOAST_CLASS_PREFIX = TOAST_CLASS + "-";
var TOAST_WRAPPER_CLASS = TOAST_CLASS_PREFIX + "wrapper";
var TOAST_CONTENT_CLASS = TOAST_CLASS_PREFIX + "content";
var TOAST_MESSAGE_CLASS = TOAST_CLASS_PREFIX + "message";
var TOAST_ICON_CLASS = TOAST_CLASS_PREFIX + "icon";
var WIDGET_NAME = "dxToast";
var toastTypes = ["info", "warning", "error", "success"];
var TOAST_STACK = [];
var FIRST_Z_INDEX_OFFSET = 8e3;
var visibleToastInstance = null;
var POSITION_ALIASES = {
    top: {
        my: "top",
        at: "top",
        of: null,
        offset: "0 0"
    },
    bottom: {
        my: "bottom",
        at: "bottom",
        of: null,
        offset: "0 -20"
    },
    center: {
        my: "center",
        at: "center",
        of: null,
        offset: "0 0"
    },
    right: {
        my: "center right",
        at: "center right",
        of: null,
        offset: "0 0"
    },
    left: {
        my: "center left",
        at: "center left",
        of: null,
        offset: "0 0"
    }
};
var DEFAULT_BOUNDARY_OFFSET = {
    h: 0,
    v: 0
};
ready((function() {
    eventsEngine.subscribeGlobal(domAdapter.getDocument(), pointerEvents.down, (function(e) {
        for (var i = TOAST_STACK.length - 1; i >= 0; i--) {
            if (!TOAST_STACK[i]._proxiedDocumentDownHandler(e)) {
                return
            }
        }
    }))
}));
var Toast = Overlay.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            message: "",
            type: "info",
            displayTime: 2e3,
            position: "bottom center",
            animation: {
                show: {
                    type: "fade",
                    duration: 400,
                    from: 0,
                    to: 1
                },
                hide: {
                    type: "fade",
                    duration: 400,
                    to: 0
                }
            },
            shading: false,
            height: "auto",
            hideTopOverlayHandler: null,
            closeOnSwipe: true,
            closeOnClick: false,
            resizeEnabled: false
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: {
                platform: "android"
            },
            options: {
                closeOnOutsideClick: true,
                width: "auto",
                position: {
                    at: "bottom left",
                    my: "bottom left",
                    offset: "20 -20"
                },
                animation: {
                    show: {
                        type: "slide",
                        duration: 200,
                        from: {
                            position: {
                                my: "top",
                                at: "bottom",
                                of: window
                            }
                        }
                    },
                    hide: {
                        type: "slide",
                        duration: 200,
                        to: {
                            position: {
                                my: "top",
                                at: "bottom",
                                of: window
                            }
                        }
                    }
                }
            }
        }, {
            device: function(_device) {
                var isPhone = "phone" === _device.deviceType;
                var isAndroid = "android" === _device.platform;
                return isPhone && isAndroid
            },
            options: {
                width: "100vw",
                position: {
                    at: "bottom center",
                    my: "bottom center",
                    offset: "0 0"
                }
            }
        }, {
            device: function(_device2) {
                return "phone" === _device2.deviceType
            },
            options: {
                width: "100vw"
            }
        }, {
            device: function() {
                return isMaterial()
            },
            options: {
                minWidth: 344,
                maxWidth: 568,
                displayTime: 4e3
            }
        }])
    },
    _init: function() {
        this.callBase();
        this._posStringToObject()
    },
    _renderContentImpl: function() {
        if (this.option("message")) {
            this._message = $("<div>").addClass(TOAST_MESSAGE_CLASS).text(this.option("message")).appendTo(this.$content())
        }
        this.setAria("role", "alert", this._message);
        if (inArray(this.option("type").toLowerCase(), toastTypes) > -1) {
            this.$content().prepend($("<div>").addClass(TOAST_ICON_CLASS))
        }
        this.callBase()
    },
    _render: function() {
        this.callBase();
        this.$element().addClass(TOAST_CLASS);
        this.$wrapper().addClass(TOAST_WRAPPER_CLASS);
        this.$content().addClass(TOAST_CLASS_PREFIX + String(this.option("type")).toLowerCase());
        this.$content().addClass(TOAST_CONTENT_CLASS);
        this._toggleCloseEvents("Swipe");
        this._toggleCloseEvents("Click")
    },
    _renderScrollTerminator: noop,
    _toggleCloseEvents: function(event) {
        var dxEvent = "dx" + event.toLowerCase();
        eventsEngine.off(this.$content(), dxEvent);
        this.option("closeOn" + event) && eventsEngine.on(this.$content(), dxEvent, this.hide.bind(this))
    },
    _posStringToObject: function() {
        if (!isString(this.option("position"))) {
            return
        }
        var verticalPosition = this.option("position").split(" ")[0];
        var horizontalPosition = this.option("position").split(" ")[1];
        this.option("position", extend({
            boundaryOffset: DEFAULT_BOUNDARY_OFFSET
        }, POSITION_ALIASES[verticalPosition]));
        switch (horizontalPosition) {
            case "center":
            case "left":
            case "right":
                this.option("position").at += " " + horizontalPosition;
                this.option("position").my += " " + horizontalPosition
        }
    },
    _show: function() {
        if (visibleToastInstance && visibleToastInstance !== this) {
            clearTimeout(visibleToastInstance._hideTimeout);
            visibleToastInstance.hide()
        }
        visibleToastInstance = this;
        return this.callBase.apply(this, arguments).done(function() {
            clearTimeout(this._hideTimeout);
            this._hideTimeout = setTimeout(this.hide.bind(this), this.option("displayTime"))
        }.bind(this))
    },
    _hide: function() {
        visibleToastInstance = null;
        return this.callBase.apply(this, arguments)
    },
    _overlayStack: function() {
        return TOAST_STACK
    },
    _zIndexInitValue: function() {
        return this.callBase() + FIRST_Z_INDEX_OFFSET
    },
    _dispose: function() {
        clearTimeout(this._hideTimeout);
        visibleToastInstance = null;
        this.callBase()
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "type":
                this.$content().removeClass(TOAST_CLASS_PREFIX + args.previousValue);
                this.$content().addClass(TOAST_CLASS_PREFIX + String(args.value).toLowerCase());
                break;
            case "message":
                if (this._message) {
                    this._message.text(args.value)
                }
                break;
            case "closeOnSwipe":
                this._toggleCloseEvents("Swipe");
                break;
            case "closeOnClick":
                this._toggleCloseEvents("Click");
                break;
            case "displayTime":
                break;
            default:
                this.callBase(args)
        }
    }
});
registerComponent(WIDGET_NAME, Toast);
export default Toast;
