/**
 * DevExtreme (renovation/ui/scheduler/common.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.isViewDataProviderConfigValid = exports.createDataAccessors = void 0;
var _utils = require("../../../ui/scheduler/utils");
var _utils2 = require("../../../ui/scheduler/resources/utils");

function _slicedToArray(arr, i) {
    return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest()
}

function _nonIterableRest() {
    throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _iterableToArrayLimit(arr, i) {
    var _i = null == arr ? null : "undefined" !== typeof Symbol && arr[Symbol.iterator] || arr["@@iterator"];
    if (null == _i) {
        return
    }
    var _arr = [];
    var _n = true;
    var _d = false;
    var _s, _e;
    try {
        for (_i = _i.call(arr); !(_n = (_s = _i.next()).done); _n = true) {
            _arr.push(_s.value);
            if (i && _arr.length === i) {
                break
            }
        }
    } catch (err) {
        _d = true;
        _e = err
    } finally {
        try {
            if (!_n && null != _i.return) {
                _i.return()
            }
        } finally {
            if (_d) {
                throw _e
            }
        }
    }
    return _arr
}

function _arrayWithHoles(arr) {
    if (Array.isArray(arr)) {
        return arr
    }
}
var createDataAccessors = function(props) {
    var forceIsoDateParsing = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
    var dataAccessors = _utils.utils.dataAccessors.create({
        startDate: props.startDateExpr,
        endDate: props.endDateExpr,
        startDateTimeZone: props.startDateTimeZoneExpr,
        endDateTimeZone: props.endDateTimeZoneExpr,
        allDay: props.allDayExpr,
        text: props.textExpr,
        description: props.descriptionExpr,
        recurrenceRule: props.recurrenceRuleExpr,
        recurrenceException: props.recurrenceExceptionExpr
    }, null, forceIsoDateParsing, props.dateSerializationFormat);
    dataAccessors.resources = (0, _utils2.createExpressions)(props.resources);
    return dataAccessors
};
exports.createDataAccessors = createDataAccessors;
var isViewDataProviderConfigValid = function(viewDataProviderConfig, currentViewOptions) {
    if (!viewDataProviderConfig) {
        return false
    }
    var result = true;
    Object.entries(viewDataProviderConfig).forEach((function(_ref) {
        var _ref2 = _slicedToArray(_ref, 2),
            key = _ref2[0],
            value = _ref2[1];
        if (value !== currentViewOptions[key]) {
            result = false
        }
    }));
    return result
};
exports.isViewDataProviderConfigValid = isViewDataProviderConfigValid;
