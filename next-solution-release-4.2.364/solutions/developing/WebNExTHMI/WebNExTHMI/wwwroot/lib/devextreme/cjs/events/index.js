/**
 * DevExtreme (cjs/events/index.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.triggerHandler = exports.trigger = exports.one = exports.on = exports.off = exports.Event = void 0;
var _events_engine = _interopRequireDefault(require("./core/events_engine"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var on = _events_engine.default.on;
exports.on = on;
var one = _events_engine.default.one;
exports.one = one;
var off = _events_engine.default.off;
exports.off = off;
var trigger = _events_engine.default.trigger;
exports.trigger = trigger;
var triggerHandler = _events_engine.default.triggerHandler;
exports.triggerHandler = triggerHandler;
var Event = _events_engine.default.Event;
exports.Event = Event;
