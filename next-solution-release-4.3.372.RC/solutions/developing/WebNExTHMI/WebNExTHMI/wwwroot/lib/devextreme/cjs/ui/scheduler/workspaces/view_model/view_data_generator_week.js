/**
 * DevExtreme (cjs/ui/scheduler/workspaces/view_model/view_data_generator_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ViewDataGeneratorWeek = void 0;
var _week = require("../../../../renovation/ui/scheduler/view_model/to_test/views/utils/week");
var _view_data_generator = require("./view_data_generator");

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
var ViewDataGeneratorWeek = function(_ViewDataGenerator) {
    _inheritsLoose(ViewDataGeneratorWeek, _ViewDataGenerator);

    function ViewDataGeneratorWeek() {
        return _ViewDataGenerator.apply(this, arguments) || this
    }
    var _proto = ViewDataGeneratorWeek.prototype;
    _proto._getIntervalDuration = function(intervalCount) {
        return (0, _week.getIntervalDuration)(intervalCount)
    };
    _proto._calculateStartViewDate = function(options) {
        return (0, _week.calculateStartViewDate)(options.currentDate, options.startDayHour, options.startDate, this._getIntervalDuration(options.intervalCount), this.getFirstDayOfWeek(options.firstDayOfWeek))
    };
    _createClass(ViewDataGeneratorWeek, [{
        key: "daysInInterval",
        get: function() {
            return 7
        }
    }]);
    return ViewDataGeneratorWeek
}(_view_data_generator.ViewDataGenerator);
exports.ViewDataGeneratorWeek = ViewDataGeneratorWeek;
