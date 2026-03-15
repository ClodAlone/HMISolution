/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.timeline_work_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _constants = require("../constants");
var _uiScheduler = _interopRequireDefault(require("./ui.scheduler.timeline_week"));
var _work_week = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/work_week");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
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
var TIMELINE_CLASS = "dx-scheduler-timeline-work-week";
var LAST_DAY_WEEK_INDEX = 5;
var SchedulerTimelineWorkWeek = function(_SchedulerTimelineWee) {
    _inheritsLoose(SchedulerTimelineWorkWeek, _SchedulerTimelineWee);

    function SchedulerTimelineWorkWeek() {
        var _this;
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key]
        }
        _this = _SchedulerTimelineWee.call.apply(_SchedulerTimelineWee, [this].concat(args)) || this;
        _this._getWeekendsCount = _work_week.getWeekendsCount;
        return _this
    }
    var _proto = SchedulerTimelineWorkWeek.prototype;
    _proto._getElementClass = function() {
        return TIMELINE_CLASS
    };
    _proto._incrementDate = function(date) {
        var day = date.getDay();
        if (day === LAST_DAY_WEEK_INDEX) {
            date.setDate(date.getDate() + 2)
        }
        _SchedulerTimelineWee.prototype._incrementDate.call(this, date)
    };
    _createClass(SchedulerTimelineWorkWeek, [{
        key: "type",
        get: function() {
            return _constants.VIEWS.TIMELINE_WORK_WEEK
        }
    }]);
    return SchedulerTimelineWorkWeek
}(_uiScheduler.default);
(0, _component_registrator.default)("dxSchedulerTimelineWorkWeek", SchedulerTimelineWorkWeek);
var _default = SchedulerTimelineWorkWeek;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
