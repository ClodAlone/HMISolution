/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.work_space_week.js)
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
var _uiScheduler = _interopRequireDefault(require("./ui.scheduler.work_space_vertical"));
var _week = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/week");

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
var WEEK_CLASS = "dx-scheduler-work-space-week";
var SchedulerWorkSpaceWeek = function(_SchedulerWorkSpaceVe) {
    _inheritsLoose(SchedulerWorkSpaceWeek, _SchedulerWorkSpaceVe);

    function SchedulerWorkSpaceWeek() {
        return _SchedulerWorkSpaceVe.apply(this, arguments) || this
    }
    var _proto = SchedulerWorkSpaceWeek.prototype;
    _proto._getElementClass = function() {
        return WEEK_CLASS
    };
    _proto._calculateViewStartDate = function() {
        return (0, _week.calculateViewStartDate)(this.option("startDate"), this._firstDayOfWeek())
    };
    _createClass(SchedulerWorkSpaceWeek, [{
        key: "type",
        get: function() {
            return _constants.VIEWS.WEEK
        }
    }]);
    return SchedulerWorkSpaceWeek
}(_uiScheduler.default);
(0, _component_registrator.default)("dxSchedulerWorkSpaceWeek", SchedulerWorkSpaceWeek);
var _default = SchedulerWorkSpaceWeek;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
