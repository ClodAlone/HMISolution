/**
 * DevExtreme (cjs/ui/scheduler/header/calendar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _devices = _interopRequireDefault(require("../../../core/devices"));
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _ui = _interopRequireDefault(require("../../widget/ui.widget"));
var _popover = _interopRequireDefault(require("../../popover"));
var _popup = _interopRequireDefault(require("../../popup"));
var _calendar = _interopRequireDefault(require("../../calendar"));
var _ui2 = _interopRequireDefault(require("../../scroll_view/ui.scrollable"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var CALENDAR_CLASS = "dx-scheduler-navigator-calendar";
var CALENDAR_POPOVER_CLASS = "dx-scheduler-navigator-calendar-popover";
var SchedulerCalendar = function(_Widget) {
    _inheritsLoose(SchedulerCalendar, _Widget);

    function SchedulerCalendar() {
        return _Widget.apply(this, arguments) || this
    }
    var _proto = SchedulerCalendar.prototype;
    _proto.show = function(target) {
        if (!this._isMobileLayout()) {
            this._overlay.option("target", target)
        }
        this._overlay.show()
    };
    _proto.hide = function() {
        this._overlay.hide()
    };
    _proto._keyboardHandler = function(opts) {
        var _this$_calendar;
        null === (_this$_calendar = this._calendar) || void 0 === _this$_calendar ? void 0 : _this$_calendar._keyboardHandler(opts)
    };
    _proto._init = function() {
        _Widget.prototype._init.call(this);
        this.$element()
    };
    _proto._render = function() {
        _Widget.prototype._render.call(this);
        this._renderOverlay()
    };
    _proto._renderOverlay = function() {
        var _this = this;
        this.$element().addClass(CALENDAR_POPOVER_CLASS);
        var isMobileLayout = this._isMobileLayout();
        var overlayType = isMobileLayout ? _popup.default : _popover.default;
        this._overlay = this._createComponent(this.$element(), overlayType, {
            contentTemplate: function() {
                return _this._createOverlayContent()
            },
            onShown: function() {
                return _this._calendar.focus()
            },
            defaultOptionsRules: [{
                device: function() {
                    return isMobileLayout
                },
                options: {
                    fullScreen: true,
                    showCloseButton: false,
                    toolbarItems: [{
                        shortcut: "cancel"
                    }]
                }
            }]
        })
    };
    _proto._createOverlayContent = function() {
        var result = (0, _renderer.default)("<div>").addClass(CALENDAR_CLASS);
        this._calendar = this._createComponent(result, _calendar.default, this._getCalendarOptions());
        if (this._isMobileLayout()) {
            var scrollable = this._createScrollable(result);
            return scrollable.$element()
        }
        return result
    };
    _proto._createScrollable = function(content) {
        var result = this._createComponent("<div>", _ui2.default, {
            direction: "vertical"
        });
        result.$content().append(content);
        return result
    };
    _proto._getCalendarOptions = function() {
        return {
            value: this.option("date"),
            min: this.option("min"),
            max: this.option("max"),
            firstDayOfWeek: this.option("firstDayOfWeek"),
            focusStateEnabled: this.option("focusStateEnabled"),
            onValueChanged: this.option("onValueChanged"),
            skipFocusCheck: true,
            tabIndex: this.option("tabIndex"),
            width: "100%"
        }
    };
    _proto._isMobileLayout = function() {
        return !_devices.default.current().generic
    };
    return SchedulerCalendar
}(_ui.default);
exports.default = SchedulerCalendar;
(0, _component_registrator.default)("dxSchedulerCalendarPopup", SchedulerCalendar);
module.exports = exports.default;
module.exports.default = exports.default;
