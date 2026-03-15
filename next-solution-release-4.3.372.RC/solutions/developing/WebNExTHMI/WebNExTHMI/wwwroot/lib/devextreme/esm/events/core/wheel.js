/**
 * DevExtreme (esm/events/core/wheel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import registerEvent from "./event_registrator";
import {
    addNamespace,
    fireEvent
} from "../utils/index";
var EVENT_NAME = "dxmousewheel";
var EVENT_NAMESPACE = "dxWheel";
var NATIVE_EVENT_NAME = "wheel";
var PIXEL_MODE = 0;
var DELTA_MUTLIPLIER = 30;
var wheel = {
    setup: function(element) {
        var $element = $(element);
        eventsEngine.on($element, addNamespace(NATIVE_EVENT_NAME, EVENT_NAMESPACE), wheel._wheelHandler.bind(wheel))
    },
    teardown: function(element) {
        eventsEngine.off(element, ".".concat(EVENT_NAMESPACE))
    },
    _wheelHandler: function(e) {
        var {
            deltaMode: deltaMode,
            deltaY: deltaY,
            deltaX: deltaX,
            deltaZ: deltaZ
        } = e.originalEvent;
        fireEvent({
            type: EVENT_NAME,
            originalEvent: e,
            delta: this._normalizeDelta(deltaY, deltaMode),
            deltaX: deltaX,
            deltaY: deltaY,
            deltaZ: deltaZ,
            deltaMode: deltaMode,
            pointerType: "mouse"
        });
        e.stopPropagation()
    },
    _normalizeDelta(delta) {
        var deltaMode = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : PIXEL_MODE;
        if (deltaMode === PIXEL_MODE) {
            return -delta
        } else {
            return -DELTA_MUTLIPLIER * delta
        }
    }
};
registerEvent(EVENT_NAME, wheel);
export {
    EVENT_NAME as name
};
