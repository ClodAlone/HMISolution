/**
 * DevExtreme (cjs/renovation/utils/subscribe_to_event.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}
exports.subscribeToDxInactiveEvent = exports.subscribeToDxHoverStartEvent = exports.subscribeToDxHoverEndEvent = exports.subscribeToDxFocusOutEvent = exports.subscribeToDxFocusInEvent = exports.subscribeToDxActiveEvent = exports.subscribeToDXScrollStopEvent = exports.subscribeToDXScrollStartEvent = exports.subscribeToDXScrollMoveEvent = exports.subscribeToDXScrollEndEvent = exports.subscribeToDXScrollCancelEvent = exports.subscribeToDXPointerUpEvent = exports.subscribeToDXPointerDownEvent = exports.subscribeToClickEvent = void 0;
exports.subscribeToEvent = subscribeToEvent;
exports.subscribeToScrollInitEvent = exports.subscribeToScrollEvent = exports.subscribeToMouseLeaveEvent = exports.subscribeToMouseEnterEvent = exports.subscribeToKeyDownEvent = void 0;
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var clickEvent = _interopRequireWildcard(require("../../events/click"));
var _index = require("../../events/utils/index");
var _emitterGesture = _interopRequireDefault(require("../../events/gesture/emitter.gesture.scroll"));
var _pointer = _interopRequireDefault(require("../../events/pointer"));

function _getRequireWildcardCache(nodeInterop) {
    if ("function" !== typeof WeakMap) {
        return null
    }
    var cacheBabelInterop = new WeakMap;
    var cacheNodeInterop = new WeakMap;
    return (_getRequireWildcardCache = function(nodeInterop) {
        return nodeInterop ? cacheNodeInterop : cacheBabelInterop
    })(nodeInterop)
}

function _interopRequireWildcard(obj, nodeInterop) {
    if (!nodeInterop && obj && obj.__esModule) {
        return obj
    }
    if (null === obj || "object" !== _typeof(obj) && "function" !== typeof obj) {
        return {
            default: obj
        }
    }
    var cache = _getRequireWildcardCache(nodeInterop);
    if (cache && cache.has(obj)) {
        return cache.get(obj)
    }
    var newObj = {};
    var hasPropertyDescriptor = Object.defineProperty && Object.getOwnPropertyDescriptor;
    for (var key in obj) {
        if ("default" !== key && Object.prototype.hasOwnProperty.call(obj, key)) {
            var desc = hasPropertyDescriptor ? Object.getOwnPropertyDescriptor(obj, key) : null;
            if (desc && (desc.get || desc.set)) {
                Object.defineProperty(newObj, key, desc)
            } else {
                newObj[key] = obj[key]
            }
        }
    }
    newObj.default = obj;
    if (cache) {
        cache.set(obj, newObj)
    }
    return newObj
}

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function subscribeToEvent(eventName) {
    return function(element, handler, eventData, namespace) {
        var event = namespace ? (0, _index.addNamespace)(eventName, namespace) : eventName;
        if (handler) {
            _events_engine.default.on(element, event, eventData, handler);
            return function() {
                _events_engine.default.off(element, event, handler)
            }
        }
        return
    }
}
var subscribeToClickEvent = subscribeToEvent(clickEvent.name);
exports.subscribeToClickEvent = subscribeToClickEvent;
var subscribeToScrollEvent = subscribeToEvent(_emitterGesture.default.scroll);
exports.subscribeToScrollEvent = subscribeToScrollEvent;
var subscribeToScrollInitEvent = subscribeToEvent(_emitterGesture.default.init);
exports.subscribeToScrollInitEvent = subscribeToScrollInitEvent;
var subscribeToDXScrollStartEvent = subscribeToEvent(_emitterGesture.default.start);
exports.subscribeToDXScrollStartEvent = subscribeToDXScrollStartEvent;
var subscribeToDXScrollMoveEvent = subscribeToEvent(_emitterGesture.default.move);
exports.subscribeToDXScrollMoveEvent = subscribeToDXScrollMoveEvent;
var subscribeToDXScrollEndEvent = subscribeToEvent(_emitterGesture.default.end);
exports.subscribeToDXScrollEndEvent = subscribeToDXScrollEndEvent;
var subscribeToDXScrollStopEvent = subscribeToEvent(_emitterGesture.default.stop);
exports.subscribeToDXScrollStopEvent = subscribeToDXScrollStopEvent;
var subscribeToDXScrollCancelEvent = subscribeToEvent(_emitterGesture.default.cancel);
exports.subscribeToDXScrollCancelEvent = subscribeToDXScrollCancelEvent;
var subscribeToDXPointerDownEvent = subscribeToEvent(_pointer.default.down);
exports.subscribeToDXPointerDownEvent = subscribeToDXPointerDownEvent;
var subscribeToDXPointerUpEvent = subscribeToEvent(_pointer.default.up);
exports.subscribeToDXPointerUpEvent = subscribeToDXPointerUpEvent;
var subscribeToMouseEnterEvent = subscribeToEvent("mouseenter");
exports.subscribeToMouseEnterEvent = subscribeToMouseEnterEvent;
var subscribeToMouseLeaveEvent = subscribeToEvent("mouseleave");
exports.subscribeToMouseLeaveEvent = subscribeToMouseLeaveEvent;
var subscribeToKeyDownEvent = subscribeToEvent("keydown");
exports.subscribeToKeyDownEvent = subscribeToKeyDownEvent;
var subscribeToDxActiveEvent = subscribeToEvent("dxactive");
exports.subscribeToDxActiveEvent = subscribeToDxActiveEvent;
var subscribeToDxInactiveEvent = subscribeToEvent("dxinactive");
exports.subscribeToDxInactiveEvent = subscribeToDxInactiveEvent;
var subscribeToDxHoverStartEvent = subscribeToEvent("dxhoverstart");
exports.subscribeToDxHoverStartEvent = subscribeToDxHoverStartEvent;
var subscribeToDxHoverEndEvent = subscribeToEvent("dxhoverend");
exports.subscribeToDxHoverEndEvent = subscribeToDxHoverEndEvent;
var subscribeToDxFocusInEvent = subscribeToEvent("focusin");
exports.subscribeToDxFocusInEvent = subscribeToDxFocusInEvent;
var subscribeToDxFocusOutEvent = subscribeToEvent("focusout");
exports.subscribeToDxFocusOutEvent = subscribeToDxFocusOutEvent;
