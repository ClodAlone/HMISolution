/**
 * DevExtreme (cjs/ui/scheduler/appointments/settingsGenerator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.DateGeneratorVirtualStrategy = exports.DateGeneratorBaseStrategy = exports.AppointmentSettingsGenerator = void 0;
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _type = require("../../../core/utils/type");
var _extend = require("../../../core/utils/extend");
var _recurrence = require("../recurrence");
var _utils = _interopRequireDefault(require("../utils.timeZone"));
var _utils2 = require("../resources/utils");
var _appointmentAdapter = require("../appointmentAdapter");
var _cellPositionCalculator = require("./cellPositionCalculator");
var _expressionUtils = require("../expressionUtils");
var _base = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");
var _textUtils = require("./textUtils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
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

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
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
var toMs = _date.default.dateToMilliseconds;
var APPOINTMENT_DATE_TEXT_FORMAT = "TIME";
var DateGeneratorBaseStrategy = function() {
    function DateGeneratorBaseStrategy(options) {
        this.options = options
    }
    var _proto = DateGeneratorBaseStrategy.prototype;
    _proto.getIntervalDuration = function() {
        return this.appointmentTakesAllDay ? this.options.allDayIntervalDuration : this.options.intervalDuration
    };
    _proto.generate = function(appointmentAdapter) {
        var itemGroupIndices = this._getGroupIndices(this.rawAppointment);
        var appointmentList = this._createAppointments(appointmentAdapter, itemGroupIndices);
        appointmentList = this._getProcessedByAppointmentTimeZone(appointmentList, appointmentAdapter);
        if (this._canProcessNotNativeTimezoneDates(appointmentAdapter)) {
            appointmentList = this._getProcessedNotNativeTimezoneDates(appointmentList, appointmentAdapter)
        }
        var dateSettings = this._createGridAppointmentList(appointmentList, appointmentAdapter);
        dateSettings = this._cropAppointmentsByStartDayHour(dateSettings, this.rawAppointment);
        dateSettings = this._fillNormalizedEndDate(dateSettings, this.rawAppointment);
        if (this._needSeparateLongParts()) {
            dateSettings = this._separateLongParts(dateSettings, appointmentAdapter)
        }
        var isRecurrent = appointmentAdapter.isRecurrent;
        return {
            dateSettings: dateSettings,
            itemGroupIndices: itemGroupIndices,
            isRecurrent: isRecurrent
        }
    };
    _proto._getProcessedByAppointmentTimeZone = function(appointmentList, appointment) {
        var _this = this;
        var hasAppointmentTimeZone = !(0, _type.isEmptyObject)(appointment.startDateTimeZone) || !(0, _type.isEmptyObject)(appointment.endDateTimeZone);
        if (hasAppointmentTimeZone) {
            var appointmentOffsets = {
                startDate: this.timeZoneCalculator.getOffsets(appointment.startDate, appointment.startDateTimeZone),
                endDate: this.timeZoneCalculator.getOffsets(appointment.endDate, appointment.endDateTimeZone)
            };
            appointmentList.forEach((function(a) {
                var sourceOffsets_startDate = _this.timeZoneCalculator.getOffsets(a.startDate, appointment.startDateTimeZone),
                    sourceOffsets_endDate = _this.timeZoneCalculator.getOffsets(a.endDate, appointment.endDateTimeZone);
                var startDateOffsetDiff = appointmentOffsets.startDate.appointment - sourceOffsets_startDate.appointment;
                var endDateOffsetDiff = appointmentOffsets.endDate.appointment - sourceOffsets_endDate.appointment;
                if (sourceOffsets_startDate.appointment !== sourceOffsets_startDate.common) {
                    a.startDate = new Date(a.startDate.getTime() + startDateOffsetDiff * toMs("hour"))
                }
                if (sourceOffsets_endDate.appointment !== sourceOffsets_endDate.common) {
                    a.endDate = new Date(a.endDate.getTime() + endDateOffsetDiff * toMs("hour"))
                }
            }))
        }
        return appointmentList
    };
    _proto._createAppointments = function(appointment, groupIndices) {
        var appointments = this._createRecurrenceAppointments(appointment, groupIndices);
        if (!appointment.isRecurrent && 0 === appointments.length) {
            appointments.push({
                startDate: appointment.startDate,
                endDate: appointment.endDate
            })
        }
        appointments = appointments.map((function(item) {
            var _item$endDate;
            var resultEndTime = null === (_item$endDate = item.endDate) || void 0 === _item$endDate ? void 0 : _item$endDate.getTime();
            if (item.startDate.getTime() === resultEndTime) {
                item.endDate.setTime(resultEndTime + toMs("minute"))
            }
            return _extends({}, item, {
                exceptionDate: new Date(item.startDate)
            })
        }));
        return appointments
    };
    _proto._canProcessNotNativeTimezoneDates = function(appointment) {
        var isTimeZoneSet = !(0, _type.isEmptyObject)(this.timeZone);
        if (!isTimeZoneSet) {
            return false
        }
        if (!appointment.isRecurrent) {
            return false
        }
        return !_utils.default.isEqualLocalTimeZone(this.timeZone, appointment.startDate)
    };
    _proto._getProcessedNotNativeDateIfCrossDST = function(date, offset) {
        if (offset < 0) {
            var newDate = new Date(date);
            var newDateMinusOneHour = new Date(newDate);
            newDateMinusOneHour.setHours(newDateMinusOneHour.getHours() - 1);
            var newDateOffset = this.timeZoneCalculator.getOffsets(newDate).common;
            var newDateMinusOneHourOffset = this.timeZoneCalculator.getOffsets(newDateMinusOneHour).common;
            if (newDateOffset !== newDateMinusOneHourOffset) {
                return 0
            }
        }
        return offset
    };
    _proto._getCommonOffset = function(date) {
        return this.timeZoneCalculator.getOffsets(date).common
    };
    _proto._getProcessedNotNativeTimezoneDates = function(appointmentList, appointment) {
        var _this2 = this;
        return appointmentList.map((function(item) {
            var diffStartDateOffset = _this2._getCommonOffset(appointment.startDate) - _this2._getCommonOffset(item.startDate);
            var diffEndDateOffset = _this2._getCommonOffset(appointment.endDate) - _this2._getCommonOffset(item.endDate);
            if (0 === diffStartDateOffset && 0 === diffEndDateOffset) {
                return item
            }
            diffStartDateOffset = _this2._getProcessedNotNativeDateIfCrossDST(item.startDate, diffStartDateOffset);
            diffEndDateOffset = _this2._getProcessedNotNativeDateIfCrossDST(item.endDate, diffEndDateOffset);
            var newStartDate = new Date(item.startDate.getTime() + diffStartDateOffset * toMs("hour"));
            var newEndDate = new Date(item.endDate.getTime() + diffEndDateOffset * toMs("hour"));
            var testNewStartDate = _this2.timeZoneCalculator.createDate(newStartDate, {
                path: "toGrid"
            });
            var testNewEndDate = _this2.timeZoneCalculator.createDate(newEndDate, {
                path: "toGrid"
            });
            if (appointment.duration > testNewEndDate.getTime() - testNewStartDate.getTime()) {
                newEndDate = new Date(newStartDate.getTime() + appointment.duration)
            }
            return _extends({}, item, {
                startDate: newStartDate,
                endDate: newEndDate,
                exceptionDate: new Date(newStartDate)
            })
        }))
    };
    _proto._needSeparateLongParts = function() {
        return this.isVerticalOrientation ? this.isGroupedByDate : this.isGroupedByDate && this.appointmentTakesAllDay
    };
    _proto.normalizeEndDateByViewEnd = function(rawAppointment, endDate) {
        var result = new Date(endDate.getTime());
        var isAllDay = (0, _base.isDateAndTimeView)(this.viewType) && this.appointmentTakesAllDay;
        if (!isAllDay) {
            var roundedEndViewDate = _date.default.roundToHour(this.endViewDate);
            if (result > roundedEndViewDate) {
                result = roundedEndViewDate
            }
        }
        var endDayHour = this.viewEndDayHour;
        var allDay = _expressionUtils.ExpressionUtils.getField(this.dataAccessors, "allDay", rawAppointment);
        var currentViewEndTime = new Date(new Date(endDate.getTime()).setHours(endDayHour, 0, 0, 0));
        if (result.getTime() > currentViewEndTime.getTime() || allDay && result.getHours() < endDayHour) {
            result = currentViewEndTime
        }
        return result
    };
    _proto._fillNormalizedEndDate = function(dateSettings, rawAppointment) {
        var _this3 = this;
        return dateSettings.map((function(item) {
            var endDate = item.endDate;
            var normalizedEndDate = _this3.normalizeEndDateByViewEnd(rawAppointment, endDate);
            return _extends({}, item, {
                normalizedEndDate: normalizedEndDate
            })
        }))
    };
    _proto._separateLongParts = function(gridAppointmentList, appointmentAdapter) {
        var _this4 = this;
        var result = [];
        gridAppointmentList.forEach((function(gridAppointment) {
            var maxDate = new Date(_this4.dateRange[1]);
            var endDateOfPart = gridAppointment.normalizedEndDate;
            var longStartDateParts = _date.default.getDatesOfInterval(gridAppointment.startDate, endDateOfPart, {
                milliseconds: _this4.getIntervalDuration(_this4.appointmentTakesAllDay)
            });
            var list = longStartDateParts.filter((function(startDatePart) {
                return new Date(startDatePart) < maxDate
            })).map((function(date) {
                var endDate = new Date(new Date(date).setMilliseconds(appointmentAdapter.duration));
                var normalizedEndDate = _this4.normalizeEndDateByViewEnd(_this4.rawAppointment, endDate);
                return {
                    startDate: date,
                    endDate: endDate,
                    normalizedEndDate: normalizedEndDate,
                    source: gridAppointment.source
                }
            }));
            result = result.concat(list)
        }));
        return result
    };
    _proto._createGridAppointmentList = function(appointmentList, appointment) {
        var _this5 = this;
        return appointmentList.map((function(source) {
            var offsetDifference = appointment.startDate.getTimezoneOffset() - source.startDate.getTimezoneOffset();
            if (0 !== offsetDifference && _this5._canProcessNotNativeTimezoneDates(appointment)) {
                source.startDate = new Date(source.startDate.getTime() + offsetDifference * toMs("minute"));
                source.endDate = new Date(source.endDate.getTime() + offsetDifference * toMs("minute"));
                source.exceptionDate = new Date(source.startDate)
            }
            var startDate = _this5.timeZoneCalculator.createDate(source.startDate, {
                path: "toGrid"
            });
            var endDate = _this5.timeZoneCalculator.createDate(source.endDate, {
                path: "toGrid"
            });
            return {
                startDate: startDate,
                endDate: endDate,
                source: source
            }
        }))
    };
    _proto._createExtremeRecurrenceDates = function() {
        var startViewDate = this.appointmentTakesAllDay ? _date.default.trimTime(this.dateRange[0]) : this.dateRange[0];
        var endViewDateByEndDayHour = this.dateRange[1];
        if (this.timeZone) {
            startViewDate = this.timeZoneCalculator.createDate(startViewDate, {
                path: "fromGrid"
            });
            endViewDateByEndDayHour = this.timeZoneCalculator.createDate(endViewDateByEndDayHour, {
                path: "fromGrid"
            });
            var daylightOffset = _utils.default.getDaylightOffsetInMs(startViewDate, endViewDateByEndDayHour);
            if (daylightOffset) {
                endViewDateByEndDayHour = new Date(endViewDateByEndDayHour.getTime() + daylightOffset)
            }
        }
        return [startViewDate, endViewDateByEndDayHour]
    };
    _proto._createRecurrenceOptions = function(appointment, groupIndex) {
        var _this6 = this;
        var _this$_createExtremeR = this._createExtremeRecurrenceDates(groupIndex),
            _this$_createExtremeR2 = _slicedToArray(_this$_createExtremeR, 2),
            minRecurrenceDate = _this$_createExtremeR2[0],
            maxRecurrenceDate = _this$_createExtremeR2[1];
        return {
            rule: appointment.recurrenceRule,
            exception: appointment.recurrenceException,
            min: minRecurrenceDate,
            max: maxRecurrenceDate,
            firstDayOfWeek: this.firstDayOfWeek,
            start: appointment.startDate,
            end: appointment.endDate,
            getPostProcessedException: function(date) {
                if ((0, _type.isEmptyObject)(_this6.timeZone) || _utils.default.isEqualLocalTimeZone(_this6.timeZone, date)) {
                    return date
                }
                var appointmentOffset = _this6.timeZoneCalculator.getOffsets(appointment.startDate).common;
                var exceptionAppointmentOffset = _this6.timeZoneCalculator.getOffsets(date).common;
                var diff = appointmentOffset - exceptionAppointmentOffset;
                diff = _this6._getProcessedNotNativeDateIfCrossDST(date, diff);
                return new Date(date.getTime() - diff * _date.default.dateToMilliseconds("hour"))
            }
        }
    };
    _proto._createRecurrenceAppointments = function(appointment, groupIndices) {
        var duration = appointment.duration;
        var option = this._createRecurrenceOptions(appointment);
        var generatedStartDates = (0, _recurrence.getRecurrenceProcessor)().generateDates(option);
        return generatedStartDates.map((function(date) {
            var utcDate = _utils.default.createUTCDateWithLocalOffset(date);
            utcDate.setTime(utcDate.getTime() + duration);
            var endDate = _utils.default.createDateFromUTCWithLocalOffset(utcDate);
            return {
                startDate: new Date(date),
                endDate: endDate
            }
        }))
    };
    _proto._cropAppointmentsByStartDayHour = function(appointments, rawAppointment) {
        var _this7 = this;
        return appointments.filter((function(appointment) {
            var firstViewDate = _this7._getAppointmentFirstViewDate(appointment);
            if (!firstViewDate) {
                return false
            }
            var startDayHour = _this7._getViewStartDayHour(firstViewDate);
            var startDate = new Date(appointment.startDate);
            appointment.startDate = _this7._getAppointmentResultDate({
                appointment: appointment,
                rawAppointment: rawAppointment,
                startDate: startDate,
                startDayHour: startDayHour,
                firstViewDate: firstViewDate
            });
            return !_this7.isAllDayRowAppointment ? appointment.endDate > appointment.startDate : true
        }))
    };
    _proto._getViewStartDayHour = function() {
        return this.viewStartDayHour
    };
    _proto._getAppointmentResultDate = function(options) {
        var appointment = options.appointment,
            startDayHour = options.startDayHour,
            firstViewDate = options.firstViewDate;
        var startDate = options.startDate;
        var resultDate = new Date(appointment.startDate);
        if (this.appointmentTakesAllDay) {
            resultDate = _date.default.normalizeDate(startDate, firstViewDate)
        } else {
            if (startDate < firstViewDate) {
                startDate = firstViewDate
            }
            resultDate = _date.default.normalizeDate(appointment.startDate, startDate)
        }
        return !this.isDateAppointment ? _date.default.roundDateByStartDayHour(resultDate, startDayHour) : resultDate
    };
    _proto._getAppointmentFirstViewDate = function(appointment) {
        var groupIndex = appointment.source.groupIndex || 0;
        var startDate = appointment.startDate,
            endDate = appointment.endDate;
        return this.viewDataProvider.findGroupCellStartDate(groupIndex, startDate, endDate, this.isAllDayRowAppointment, this.isDateAppointment)
    };
    _proto._getGroupIndices = function(rawAppointment) {
        var _this8 = this;
        var result = [];
        if (rawAppointment && this.loadedResources.length) {
            var tree = (0, _utils2.createResourcesTree)(this.loadedResources);
            result = (0, _utils2.getResourceTreeLeaves)((function(field, action) {
                return (0, _utils2.getDataAccessors)(_this8.options.dataAccessors.resources, field, action)
            }), tree, rawAppointment)
        }
        return result
    };
    _createClass(DateGeneratorBaseStrategy, [{
        key: "rawAppointment",
        get: function() {
            return this.options.rawAppointment
        }
    }, {
        key: "timeZoneCalculator",
        get: function() {
            return this.options.timeZoneCalculator
        }
    }, {
        key: "viewDataProvider",
        get: function() {
            return this.options.viewDataProvider
        }
    }, {
        key: "appointmentTakesAllDay",
        get: function() {
            return this.options.appointmentTakesAllDay
        }
    }, {
        key: "supportAllDayRow",
        get: function() {
            return this.options.supportAllDayRow
        }
    }, {
        key: "isAllDayRowAppointment",
        get: function() {
            return this.options.isAllDayRowAppointment
        }
    }, {
        key: "timeZone",
        get: function() {
            return this.options.timeZone
        }
    }, {
        key: "dateRange",
        get: function() {
            return this.options.dateRange
        }
    }, {
        key: "firstDayOfWeek",
        get: function() {
            return this.options.firstDayOfWeek
        }
    }, {
        key: "viewStartDayHour",
        get: function() {
            return this.options.viewStartDayHour
        }
    }, {
        key: "viewEndDayHour",
        get: function() {
            return this.options.viewEndDayHour
        }
    }, {
        key: "endViewDate",
        get: function() {
            return this.options.endViewDate
        }
    }, {
        key: "viewType",
        get: function() {
            return this.options.viewType
        }
    }, {
        key: "isGroupedByDate",
        get: function() {
            return this.options.isGroupedByDate
        }
    }, {
        key: "isVerticalOrientation",
        get: function() {
            return this.options.isVerticalOrientation
        }
    }, {
        key: "dataAccessors",
        get: function() {
            return this.options.dataAccessors
        }
    }, {
        key: "loadedResources",
        get: function() {
            return this.options.loadedResources
        }
    }, {
        key: "isDateAppointment",
        get: function() {
            return !(0, _base.isDateAndTimeView)(this.viewType) && this.appointmentTakesAllDay
        }
    }]);
    return DateGeneratorBaseStrategy
}();
exports.DateGeneratorBaseStrategy = DateGeneratorBaseStrategy;
var DateGeneratorVirtualStrategy = function(_DateGeneratorBaseStr) {
    _inheritsLoose(DateGeneratorVirtualStrategy, _DateGeneratorBaseStr);

    function DateGeneratorVirtualStrategy() {
        return _DateGeneratorBaseStr.apply(this, arguments) || this
    }
    var _proto2 = DateGeneratorVirtualStrategy.prototype;
    _proto2._createRecurrenceAppointments = function(appointment, groupIndices) {
        var _this9 = this;
        var duration = appointment.duration;
        var result = [];
        var validGroupIndices = this.groupCount ? groupIndices : [0];
        validGroupIndices.forEach((function(groupIndex) {
            var option = _this9._createRecurrenceOptions(appointment, groupIndex);
            var generatedStartDates = (0, _recurrence.getRecurrenceProcessor)().generateDates(option);
            var recurrentInfo = generatedStartDates.map((function(date) {
                var startDate = new Date(date);
                var utcDate = _utils.default.createUTCDateWithLocalOffset(date);
                utcDate.setTime(utcDate.getTime() + duration);
                var endDate = _utils.default.createDateFromUTCWithLocalOffset(utcDate);
                return {
                    startDate: startDate,
                    endDate: endDate,
                    groupIndex: groupIndex
                }
            }));
            result.push.apply(result, _toConsumableArray(recurrentInfo))
        }));
        return result
    };
    _proto2._getViewStartDayHour = function(firstViewDate) {
        return firstViewDate.getHours()
    };
    _proto2._updateGroupIndices = function(appointments, groupIndices) {
        var _this10 = this;
        var result = [];
        groupIndices.forEach((function(groupIndex) {
            var groupStartDate = _this10.viewDataProvider.getGroupStartDate(groupIndex);
            if (groupStartDate) {
                appointments.forEach((function(appointment) {
                    var appointmentCopy = (0, _extend.extend)({}, appointment);
                    appointmentCopy.groupIndex = groupIndex;
                    result.push(appointmentCopy)
                }))
            }
        }));
        return result
    };
    _proto2._getGroupIndices = function(resources) {
        var _groupIndices;
        var groupIndices = _DateGeneratorBaseStr.prototype._getGroupIndices.call(this, resources);
        var viewDataGroupIndices = this.viewDataProvider.getGroupIndices();
        if (!(null !== (_groupIndices = groupIndices) && void 0 !== _groupIndices && _groupIndices.length)) {
            groupIndices = [0]
        }
        return groupIndices.filter((function(groupIndex) {
            return -1 !== viewDataGroupIndices.indexOf(groupIndex)
        }))
    };
    _proto2._createAppointments = function(appointment, groupIndices) {
        var appointments = _DateGeneratorBaseStr.prototype._createAppointments.call(this, appointment, groupIndices);
        return !appointment.isRecurrent ? this._updateGroupIndices(appointments, groupIndices) : appointments
    };
    _createClass(DateGeneratorVirtualStrategy, [{
        key: "groupCount",
        get: function() {
            return (0, _utils2.getGroupCount)(this.loadedResources)
        }
    }]);
    return DateGeneratorVirtualStrategy
}(DateGeneratorBaseStrategy);
exports.DateGeneratorVirtualStrategy = DateGeneratorVirtualStrategy;
var AppointmentSettingsGenerator = function() {
    function AppointmentSettingsGenerator(options) {
        this.options = options;
        this.appointmentAdapter = (0, _appointmentAdapter.createAppointmentAdapter)(this.rawAppointment, this.dataAccessors, this.timeZoneCalculator)
    }
    var _proto3 = AppointmentSettingsGenerator.prototype;
    _proto3.create = function() {
        var _this$_generateDateSe = this._generateDateSettings(),
            dateSettings = _this$_generateDateSe.dateSettings,
            itemGroupIndices = _this$_generateDateSe.itemGroupIndices,
            isRecurrent = _this$_generateDateSe.isRecurrent;
        var cellPositions = this._calculateCellPositions(dateSettings, itemGroupIndices);
        var result = this._prepareAppointmentInfos(dateSettings, cellPositions, isRecurrent);
        return result
    };
    _proto3._generateDateSettings = function() {
        return this.dateSettingsStrategy.generate(this.appointmentAdapter)
    };
    _proto3._calculateCellPositions = function(dateSettings, itemGroupIndices) {
        var cellPositionCalculator = new _cellPositionCalculator.CellPositionCalculator(_extends({}, this.options, {
            dateSettings: dateSettings
        }));
        return cellPositionCalculator.calculateCellPositions(itemGroupIndices, this.isAllDayRowAppointment, this.appointmentAdapter.isRecurrent)
    };
    _proto3._prepareAppointmentInfos = function(dateSettings, cellPositions, isRecurrent) {
        var _this11 = this;
        var infos = [];
        cellPositions.forEach((function(_ref) {
            var coordinates = _ref.coordinates,
                dateSettingIndex = _ref.dateSettingIndex;
            var dateSetting = dateSettings[dateSettingIndex];
            var dateText = _this11._getAppointmentDateText(dateSetting);
            var info = {
                appointment: dateSetting,
                sourceAppointment: dateSetting.source,
                dateText: dateText,
                isRecurrent: isRecurrent
            };
            _this11._setResourceColor(info, coordinates.groupIndex);
            infos.push(_extends({}, coordinates, {
                info: info
            }))
        }));
        return infos
    };
    _proto3._getAppointmentDateText = function(sourceAppointment) {
        var startDate = sourceAppointment.startDate,
            endDate = sourceAppointment.endDate,
            allDay = sourceAppointment.allDay;
        return (0, _textUtils.createFormattedDateText)({
            startDate: startDate,
            endDate: endDate,
            allDay: allDay,
            format: APPOINTMENT_DATE_TEXT_FORMAT
        })
    };
    _proto3._setResourceColor = function(info, groupIndex) {
        var appointmentConfig = {
            itemData: this.rawAppointment,
            groupIndex: groupIndex,
            groups: this.modelGroups
        };
        this.options.getAppointmentColor(appointmentConfig).done((function(color) {
            return info.resourceColor = color
        }))
    };
    _createClass(AppointmentSettingsGenerator, [{
        key: "rawAppointment",
        get: function() {
            return this.options.rawAppointment
        }
    }, {
        key: "dataAccessors",
        get: function() {
            return this.options.dataAccessors
        }
    }, {
        key: "timeZoneCalculator",
        get: function() {
            return this.options.timeZoneCalculator
        }
    }, {
        key: "isAllDayRowAppointment",
        get: function() {
            return this.options.appointmentTakesAllDay && this.options.supportAllDayRow
        }
    }, {
        key: "modelGroups",
        get: function() {
            return this.options.modelGroups
        }
    }, {
        key: "dateSettingsStrategy",
        get: function() {
            var options = _extends({}, this.options, {
                isAllDayRowAppointment: this.isAllDayRowAppointment
            });
            return this.options.isVirtualScrolling ? new DateGeneratorVirtualStrategy(options) : new DateGeneratorBaseStrategy(options)
        }
    }]);
    return AppointmentSettingsGenerator
}();
exports.AppointmentSettingsGenerator = AppointmentSettingsGenerator;
