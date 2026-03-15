/**
 * DevExtreme (cjs/events/visibility_change.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.triggerShownEvent = exports.triggerResizeEvent = exports.triggerHidingEvent = void 0;
var _renderer = _interopRequireDefault(require("../core/renderer"));
var _events_engine = _interopRequireDefault(require("./core/events_engine"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var triggerVisibilityChangeEvent = function(eventName) {
    return function(element) {
        var $element = (0, _renderer.default)(element || "body");
        var changeHandlers = $element.filter(".dx-visibility-change-handler").add($element.find(".dx-visibility-change-handler"));
        for (var i = 0; i < changeHandlers.length; i++) {
            _events_engine.default.triggerHandler(changeHandlers[i], eventName)
        }
    }
};
var triggerShownEvent = triggerVisibilityChangeEvent("dxshown");
exports.triggerShownEvent = triggerShownEvent;
var triggerHidingEvent = triggerVisibilityChangeEvent("dxhiding");
exports.triggerHidingEvent = triggerHidingEvent;
var triggerResizeEvent = triggerVisibilityChangeEvent("dxresize");
exports.triggerResizeEvent = triggerResizeEvent;
