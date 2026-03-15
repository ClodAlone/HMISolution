/**
 * DevExtreme (cjs/viz/gauges/tracker.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _class = _interopRequireDefault(require("../../core/class"));
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _wheel = require("../../events/core/wheel");
var _ready_callbacks = _interopRequireDefault(require("../../core/utils/ready_callbacks"));
var _index = require("../../events/utils/index");
var _pointer = _interopRequireDefault(require("../../events/pointer"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var EVENT_NS = "gauge-tooltip";
var TOOLTIP_HIDE_DELAY = 100;
var ready = _ready_callbacks.default.add;
var Tracker = _class.default.inherit({
    ctor: function(parameters) {
        var that = this;
        that._element = parameters.renderer.g().attr({
            class: "dxg-tracker",
            stroke: "none",
            "stroke-width": 0,
            fill: "#000000",
            opacity: 1e-4
        }).linkOn(parameters.container, {
            name: "tracker",
            after: "peripheral"
        });
        that._showTooltipCallback = function() {
            var target = that._tooltipEvent.target;
            var data_target = target["gauge-data-target"];
            var data_info = target["gauge-data-info"];
            that._targetEvent = null;
            if (that._tooltipTarget !== target) {
                var callback = function(result) {
                    result && (that._tooltipTarget = target)
                };
                callback(that._callbacks["tooltip-show"](data_target, data_info, callback))
            }
        };
        that._hideTooltipCallback = function() {
            that._hideTooltipTimeout = null;
            that._targetEvent = null;
            if (that._tooltipTarget) {
                that._callbacks["tooltip-hide"]();
                that._tooltipTarget = null
            }
        };
        that._dispose = function() {
            clearTimeout(that._hideTooltipTimeout);
            that._showTooltipCallback = that._hideTooltipCallback = that._dispose = null
        }
    },
    dispose: function() {
        this._dispose();
        this.deactivate();
        this._element.off("." + EVENT_NS);
        this._element.linkOff();
        this._element = this._context = this._callbacks = null;
        return this
    },
    activate: function() {
        this._element.linkAppend();
        return this
    },
    deactivate: function() {
        this._element.linkRemove().clear();
        return this
    },
    attach: function(element, target, info) {
        element.data({
            "gauge-data-target": target,
            "gauge-data-info": info
        }).append(this._element);
        return this
    },
    detach: function(element) {
        element.remove();
        return this
    },
    setTooltipState: function(state) {
        this._element.off("." + EVENT_NS);
        if (state) {
            var data = {
                tracker: this
            };
            this._element.on((0, _index.addNamespace)([_pointer.default.move], EVENT_NS), data, handleTooltipMouseOver).on((0, _index.addNamespace)([_pointer.default.out], EVENT_NS), data, handleTooltipMouseOut).on((0, _index.addNamespace)([_pointer.default.down], EVENT_NS), data, handleTooltipTouchStart).on((0, _index.addNamespace)([_pointer.default.up], EVENT_NS), data, handleTooltipTouchEnd).on((0, _index.addNamespace)([_wheel.name], EVENT_NS), data, handleTooltipMouseWheel)
        }
        return this
    },
    setCallbacks: function(callbacks) {
        this._callbacks = callbacks;
        return this
    },
    _showTooltip: function(event) {
        clearTimeout(this._hideTooltipTimeout);
        this._hideTooltipTimeout = null;
        if (this._tooltipTarget === event.target) {
            return
        }
        this._tooltipEvent = event;
        this._showTooltipCallback()
    },
    _hideTooltip: function(delay) {
        clearTimeout(this._hideTooltipTimeout);
        if (delay) {
            this._hideTooltipTimeout = setTimeout(this._hideTooltipCallback, delay)
        } else {
            this._hideTooltipCallback()
        }
    }
});
var active_touch_tooltip_tracker = null;

function handleTooltipMouseOver(event) {
    var tracker = event.data.tracker;
    tracker._x = event.pageX;
    tracker._y = event.pageY;
    tracker._showTooltip(event)
}

function handleTooltipMouseOut(event) {
    event.data.tracker._hideTooltip(TOOLTIP_HIDE_DELAY)
}

function handleTooltipMouseWheel(event) {
    event.data.tracker._hideTooltip()
}

function handleTooltipTouchStart(event) {
    var tracker = active_touch_tooltip_tracker = event.data.tracker;
    tracker._touch = true;
    handleTooltipMouseOver(event)
}

function handleTooltipTouchEnd() {
    active_touch_tooltip_tracker._touch = false
}

function handleDocumentTooltipTouchStart(event) {
    var tracker = active_touch_tooltip_tracker;
    if (tracker && !tracker._touch) {
        tracker._hideTooltip(TOOLTIP_HIDE_DELAY);
        active_touch_tooltip_tracker = null
    }
}
ready((function() {
    _events_engine.default.subscribeGlobal(_dom_adapter.default.getDocument(), (0, _index.addNamespace)([_pointer.default.down], EVENT_NS), handleDocumentTooltipTouchStart)
}));
var _default = Tracker;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
