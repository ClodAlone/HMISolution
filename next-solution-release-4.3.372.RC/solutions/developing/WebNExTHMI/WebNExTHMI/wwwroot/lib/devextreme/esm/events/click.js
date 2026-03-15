/**
 * DevExtreme (esm/events/click.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import eventsEngine from "../events/core/events_engine";
import devices from "../core/devices";
import domAdapter from "../core/dom_adapter";
import {
    resetActiveElement,
    contains,
    closestCommonParent
} from "../core/utils/dom";
import {
    requestAnimationFrame,
    cancelAnimationFrame
} from "../animation/frame";
import {
    addNamespace,
    fireEvent,
    eventDelta,
    eventData
} from "./utils/index";
import {
    subscribeNodesDisposing,
    unsubscribeNodesDisposing
} from "./utils/event_nodes_disposing";
import pointerEvents from "./pointer";
import Emitter from "./core/emitter";
import registerEmitter from "./core/emitter_registrator";
import {
    compare as compareVersions
} from "../core/utils/version";
var CLICK_EVENT_NAME = "dxclick";
var TOUCH_BOUNDARY = 10;
var abs = Math.abs;
var isInput = function(element) {
    return $(element).is("input, textarea, select, button ,:focus, :focus *")
};
var misc = {
    requestAnimationFrame: requestAnimationFrame,
    cancelAnimationFrame: cancelAnimationFrame
};
var ClickEmitter = Emitter.inherit({
    ctor: function(element) {
        this.callBase(element);
        this._makeElementClickable($(element))
    },
    _makeElementClickable: function($element) {
        if (!$element.attr("onclick")) {
            $element.attr("onclick", "void(0)")
        }
    },
    start: function(e) {
        this._blurPrevented = e.isDefaultPrevented();
        this._startTarget = e.target;
        this._startEventData = eventData(e)
    },
    end: function(e) {
        if (this._eventOutOfElement(e, this.getElement().get(0)) || e.type === pointerEvents.cancel) {
            this._cancel(e);
            return
        }
        if (!isInput(e.target) && !this._blurPrevented) {
            resetActiveElement()
        }
        this._accept(e);
        this._clickAnimationFrame = misc.requestAnimationFrame(function() {
            this._fireClickEvent(e)
        }.bind(this))
    },
    _eventOutOfElement: function(e, element) {
        var target = e.target;
        var targetChanged = !contains(element, target) && element !== target;
        var gestureDelta = eventDelta(eventData(e), this._startEventData);
        var boundsExceeded = abs(gestureDelta.x) > TOUCH_BOUNDARY || abs(gestureDelta.y) > TOUCH_BOUNDARY;
        return targetChanged || boundsExceeded
    },
    _fireClickEvent: function(e) {
        this._fireEvent(CLICK_EVENT_NAME, e, {
            target: closestCommonParent(this._startTarget, e.target)
        })
    },
    dispose: function() {
        misc.cancelAnimationFrame(this._clickAnimationFrame)
    }
});
var realDevice = devices.real();
var useNativeClick = realDevice.generic || realDevice.ios && compareVersions(realDevice.version, [9, 3]) >= 0 || realDevice.android && compareVersions(realDevice.version, [5]) >= 0;
! function() {
    var isNativeClickEvent = function(target) {
        return useNativeClick || $(target).closest(".dx-native-click").length
    };
    var prevented = null;
    var lastFiredEvent = null;

    function onNodeRemove() {
        lastFiredEvent = null
    }
    var clickHandler = function(e) {
        var originalEvent = e.originalEvent;
        var eventAlreadyFired = lastFiredEvent === originalEvent || originalEvent && originalEvent.DXCLICK_FIRED;
        var leftButton = !e.which || 1 === e.which;
        if (leftButton && !prevented && isNativeClickEvent(e.target) && !eventAlreadyFired) {
            if (originalEvent) {
                originalEvent.DXCLICK_FIRED = true
            }
            unsubscribeNodesDisposing(lastFiredEvent, onNodeRemove);
            lastFiredEvent = originalEvent;
            subscribeNodesDisposing(lastFiredEvent, onNodeRemove);
            fireEvent({
                type: CLICK_EVENT_NAME,
                originalEvent: e
            })
        }
    };
    ClickEmitter = ClickEmitter.inherit({
        _makeElementClickable: function($element) {
            if (!isNativeClickEvent($element)) {
                this.callBase($element)
            }
            eventsEngine.on($element, "click", clickHandler)
        },
        configure: function(data) {
            this.callBase(data);
            if (data.useNative) {
                this.getElement().addClass("dx-native-click")
            }
        },
        start: function(e) {
            prevented = null;
            if (!isNativeClickEvent(e.target)) {
                this.callBase(e)
            }
        },
        end: function(e) {
            if (!isNativeClickEvent(e.target)) {
                this.callBase(e)
            }
        },
        cancel: function() {
            prevented = true
        },
        dispose: function() {
            this.callBase();
            eventsEngine.off(this.getElement(), "click", clickHandler)
        }
    })
}();
! function() {
    var desktopDevice = devices.real().generic;
    if (!desktopDevice) {
        var startTarget = null;
        var blurPrevented = false;
        var document = domAdapter.getDocument();
        eventsEngine.subscribeGlobal(document, addNamespace(pointerEvents.down, "NATIVE_CLICK_FIXER"), (function(e) {
            startTarget = e.target;
            blurPrevented = e.isDefaultPrevented()
        }));
        eventsEngine.subscribeGlobal(document, addNamespace("click", "NATIVE_CLICK_FIXER"), (function(e) {
            var $target = $(e.target);
            if (!blurPrevented && startTarget && !$target.is(startTarget) && !$(startTarget).is("label") && isInput($target)) {
                resetActiveElement()
            }
            startTarget = null;
            blurPrevented = false
        }))
    }
}();
registerEmitter({
    emitter: ClickEmitter,
    bubble: true,
    events: [CLICK_EVENT_NAME]
});
export {
    CLICK_EVENT_NAME as name
};
