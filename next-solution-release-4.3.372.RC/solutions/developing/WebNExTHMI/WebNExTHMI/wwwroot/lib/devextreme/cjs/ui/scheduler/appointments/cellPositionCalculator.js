/**
 * DevExtreme (cjs/ui/scheduler/appointments/cellPositionCalculator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.CellPositionCalculator = void 0;
var _type = require("../../../core/utils/type");
var _date = _interopRequireDefault(require("../../../core/utils/date"));

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
var BaseStrategy = function() {
    function BaseStrategy(options) {
        this.options = options
    }
    var _proto = BaseStrategy.prototype;
    _proto.calculateCellPositions = function(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var _this = this;
        var result = [];
        this.appointments.forEach((function(dateSetting, index) {
            var coordinates = _this.getCoordinateInfos({
                appointment: dateSetting,
                groupIndices: groupIndices,
                isAllDayRowAppointment: isAllDayRowAppointment,
                isRecurrentAppointment: isRecurrentAppointment
            });
            coordinates.forEach((function(item) {
                !!item && result.push(_this._prepareObject(item, index))
            }))
        }));
        return result
    };
    _proto.getCoordinateInfos = function(options) {
        var appointment = options.appointment,
            isAllDayRowAppointment = options.isAllDayRowAppointment,
            groupIndices = options.groupIndices,
            recurrent = options.recurrent;
        var startDate = appointment.startDate;
        var groupIndex = !recurrent ? appointment.source.groupIndex : void 0;
        return this.getCoordinatesByDateInGroup(startDate, groupIndices, isAllDayRowAppointment, groupIndex)
    };
    _proto._prepareObject = function(position, dateSettingIndex) {
        position.dateSettingIndex = dateSettingIndex;
        return {
            coordinates: position,
            dateSettingIndex: dateSettingIndex
        }
    };
    _proto.getCoordinatesByDate = function(date, groupIndex, inAllDayRow) {
        var validGroupIndex = groupIndex || 0;
        var cellInfo = {
            groupIndex: validGroupIndex,
            startDate: date,
            isAllDay: inAllDayRow
        };
        var positionByMap = this.viewDataProvider.findCellPositionInMap(cellInfo);
        if (!positionByMap) {
            return
        }
        var position = this.getCellPosition(positionByMap, inAllDayRow && !this.isVerticalGrouping);
        var timeShift = inAllDayRow ? 0 : this.getTimeShift(date);
        var shift = this.getPositionShift(timeShift, inAllDayRow);
        var horizontalHMax = this.positionHelper.getHorizontalMax(validGroupIndex, date);
        var verticalMax = this.positionHelper.getVerticalMax({
            groupIndex: validGroupIndex,
            isVirtualScrolling: this.isVirtualScrolling,
            showAllDayPanel: this.showAllDayPanel,
            supportAllDayRow: this.supportAllDayRow,
            isGroupedAllDayPanel: this.isGroupedAllDayPanel,
            isVerticalGrouping: this.isVerticalGrouping
        });
        return {
            positionByMap: positionByMap,
            cellPosition: position.left + shift.cellPosition,
            top: position.top + shift.top,
            left: position.left + shift.left,
            rowIndex: position.rowIndex,
            columnIndex: position.columnIndex,
            hMax: horizontalHMax,
            vMax: verticalMax,
            groupIndex: validGroupIndex
        }
    };
    _proto.getCoordinatesByDateInGroup = function(startDate, groupIndices, inAllDayRow, groupIndex) {
        var _this2 = this;
        var result = [];
        if (this.viewDataProvider.isSkippedDate(startDate)) {
            return result
        }
        var validGroupIndices = [groupIndex];
        if (!(0, _type.isDefined)(groupIndex)) {
            validGroupIndices = this.groupCount ? groupIndices : [0]
        }
        validGroupIndices.forEach((function(groupIndex) {
            var coordinates = _this2.getCoordinatesByDate(startDate, groupIndex, inAllDayRow);
            if (coordinates) {
                result.push(coordinates)
            }
        }));
        return result
    };
    _proto.getCellPosition = function(cellCoordinates, isAllDayPanel) {
        var _this$DOMMetaData = this.DOMMetaData,
            dateTableCellsMeta = _this$DOMMetaData.dateTableCellsMeta,
            allDayPanelCellsMeta = _this$DOMMetaData.allDayPanelCellsMeta;
        var columnIndex = cellCoordinates.columnIndex,
            rowIndex = cellCoordinates.rowIndex;
        var position = isAllDayPanel ? allDayPanelCellsMeta[columnIndex] : dateTableCellsMeta[rowIndex][columnIndex];
        var validPosition = _extends({}, position);
        if (this.rtlEnabled) {
            validPosition.left += position.width
        }
        if (validPosition) {
            validPosition.rowIndex = cellCoordinates.rowIndex;
            validPosition.columnIndex = cellCoordinates.columnIndex
        }
        return validPosition
    };
    _proto.getTimeShift = function(date) {
        var currentDayStart = new Date(date);
        var currentDayEndHour = new Date(new Date(date).setHours(this.viewEndDayHour, 0, 0));
        if (date.getTime() <= currentDayEndHour.getTime()) {
            currentDayStart.setHours(this.viewStartDayHour, 0, 0, 0)
        }
        var timeZoneDifference = _date.default.getTimezonesDifference(date, currentDayStart);
        var currentDateTime = date.getTime();
        var currentDayStartTime = currentDayStart.getTime();
        var minTime = this.startViewDate.getTime();
        return currentDateTime > minTime ? (currentDateTime - currentDayStartTime + timeZoneDifference) % this.cellDuration / this.cellDuration : 0
    };
    _createClass(BaseStrategy, [{
        key: "DOMMetaData",
        get: function() {
            return this.options.DOMMetaData
        }
    }, {
        key: "appointments",
        get: function() {
            return this.options.dateSettings
        }
    }, {
        key: "viewDataProvider",
        get: function() {
            return this.options.viewDataProvider
        }
    }, {
        key: "positionHelper",
        get: function() {
            return this.options.positionHelper
        }
    }, {
        key: "startViewDate",
        get: function() {
            return this.options.startViewDate
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
        key: "cellDuration",
        get: function() {
            return this.options.cellDuration
        }
    }, {
        key: "getPositionShift",
        get: function() {
            return this.options.getPositionShiftCallback
        }
    }, {
        key: "groupCount",
        get: function() {
            return this.options.groupCount
        }
    }, {
        key: "rtlEnabled",
        get: function() {
            return this.options.rtlEnabled
        }
    }, {
        key: "isVerticalGrouping",
        get: function() {
            return this.options.isVerticalOrientation
        }
    }, {
        key: "showAllDayPanel",
        get: function() {
            return this.options.showAllDayPanel
        }
    }, {
        key: "supportAllDayRow",
        get: function() {
            return this.options.supportAllDayRow
        }
    }, {
        key: "isGroupedAllDayPanel",
        get: function() {
            return this.options.isGroupedAllDayPanel
        }
    }, {
        key: "isVirtualScrolling",
        get: function() {
            return false
        }
    }]);
    return BaseStrategy
}();
var VirtualStrategy = function(_BaseStrategy) {
    _inheritsLoose(VirtualStrategy, _BaseStrategy);

    function VirtualStrategy() {
        return _BaseStrategy.apply(this, arguments) || this
    }
    var _proto2 = VirtualStrategy.prototype;
    _proto2.calculateCellPositions = function(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var _this3 = this;
        var appointments = isAllDayRowAppointment ? this.appointments : this.appointments.filter((function(_ref) {
            var source = _ref.source,
                startDate = _ref.startDate,
                endDate = _ref.endDate;
            return _this3.viewDataProvider.isGroupIntersectDateInterval(source.groupIndex, startDate, endDate)
        }));
        if (isRecurrentAppointment) {
            return this.createRecurrentAppointmentInfos(appointments, isAllDayRowAppointment)
        }
        return _BaseStrategy.prototype.calculateCellPositions.call(this, groupIndices, isAllDayRowAppointment, isRecurrentAppointment)
    };
    _proto2.createRecurrentAppointmentInfos = function(dateSettings, isAllDayRowAppointment) {
        var _this4 = this;
        var result = [];
        dateSettings.forEach((function(_ref2, index) {
            var source = _ref2.source,
                startDate = _ref2.startDate;
            var coordinate = _this4.getCoordinatesByDate(startDate, source.groupIndex, isAllDayRowAppointment);
            if (coordinate) {
                result.push(_this4._prepareObject(coordinate, index))
            }
        }));
        return result
    };
    _createClass(VirtualStrategy, [{
        key: "isVirtualScrolling",
        get: function() {
            return true
        }
    }]);
    return VirtualStrategy
}(BaseStrategy);
var CellPositionCalculator = function() {
    function CellPositionCalculator(options) {
        this.options = options
    }
    var _proto3 = CellPositionCalculator.prototype;
    _proto3.calculateCellPositions = function(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var strategy = this.options.isVirtualScrolling ? new VirtualStrategy(this.options) : new BaseStrategy(this.options);
        return strategy.calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment)
    };
    return CellPositionCalculator
}();
exports.CellPositionCalculator = CellPositionCalculator;
