/**
 * DevExtreme (cjs/ui/scheduler/workspaces/view_model/view_data_generator_day.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ViewDataGeneratorDay = void 0;
var _day = require("../../../../renovation/ui/scheduler/view_model/to_test/views/utils/day");
var _view_data_generator = require("./view_data_generator");

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
var ViewDataGeneratorDay = function(_ViewDataGenerator) {
    _inheritsLoose(ViewDataGeneratorDay, _ViewDataGenerator);

    function ViewDataGeneratorDay() {
        return _ViewDataGenerator.apply(this, arguments) || this
    }
    var _proto = ViewDataGeneratorDay.prototype;
    _proto._calculateStartViewDate = function(options) {
        return (0, _day.calculateStartViewDate)(options.currentDate, options.startDayHour, options.startDate, this._getIntervalDuration(options.intervalCount))
    };
    return ViewDataGeneratorDay
}(_view_data_generator.ViewDataGenerator);
exports.ViewDataGeneratorDay = ViewDataGeneratorDay;
