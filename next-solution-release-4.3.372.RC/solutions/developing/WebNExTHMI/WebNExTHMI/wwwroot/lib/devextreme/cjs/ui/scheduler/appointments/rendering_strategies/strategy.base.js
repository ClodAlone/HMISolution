/**
 * DevExtreme (cjs/ui/scheduler/appointments/rendering_strategies/strategy.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _appointmentsPositioning_strategy_base = _interopRequireDefault(require("./appointmentsPositioning_strategy_base"));
var _appointmentsPositioning_strategy_adaptive = _interopRequireDefault(require("./appointmentsPositioning_strategy_adaptive"));
var _extend = require("../../../../core/utils/extend");
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _type = require("../../../../core/utils/type");
var _themes = require("../../../themes");
var _settingsGenerator = require("../settingsGenerator");
var _utils = _interopRequireDefault(require("../../utils.timeZone"));
var _appointmentAdapter = require("../../appointmentAdapter");
var _utils2 = require("../dataProvider/utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
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
var APPOINTMENT_MIN_SIZE = 2;
var APPOINTMENT_DEFAULT_HEIGHT = 20;
var COMPACT_THEME_APPOINTMENT_DEFAULT_HEIGHT = 18;
var DROP_DOWN_BUTTON_ADAPTIVE_SIZE = 28;
var WEEK_VIEW_COLLECTOR_OFFSET = 5;
var COMPACT_THEME_WEEK_VIEW_COLLECTOR_OFFSET = 1;
var BaseRenderingStrategy = function() {
    function BaseRenderingStrategy(options) {
        this.options = options;
        this._initPositioningStrategy()
    }
    var _proto = BaseRenderingStrategy.prototype;
    _proto._correctCollectorCoordinatesInAdaptive = function(coordinates, isAllDay) {
        coordinates.top = coordinates.top + this.getCollectorTopOffset(isAllDay);
        coordinates.left = coordinates.left + this.getCollectorLeftOffset()
    };
    _proto._initPositioningStrategy = function() {
        this._positioningStrategy = this.isAdaptive ? new _appointmentsPositioning_strategy_adaptive.default(this) : new _appointmentsPositioning_strategy_base.default(this)
    };
    _proto.getPositioningStrategy = function() {
        return this._positioningStrategy
    };
    _proto.getAppointmentMinSize = function() {
        return APPOINTMENT_MIN_SIZE
    };
    _proto.keepAppointmentSettings = function() {
        return false
    };
    _proto.getDeltaTime = function() {};
    _proto.getAppointmentGeometry = function(coordinates) {
        return coordinates
    };
    _proto.needCorrectAppointmentDates = function() {
        return true
    };
    _proto.getDirection = function() {
        return "horizontal"
    };
    _proto.createTaskPositionMap = function(items) {
        var _this = this;
        delete this._maxAppointmentCountPerCell;
        var length = null === items || void 0 === items ? void 0 : items.length;
        if (!length) {
            return
        }
        var map = [];
        for (var i = 0; i < length; i++) {
            var coordinates = this._getItemPosition(items[i]);
            if (coordinates.length && this.rtlEnabled) {
                coordinates = this._correctRtlCoordinates(coordinates)
            }
            coordinates.forEach((function(item) {
                item.leftVirtualCellCount = _this.leftVirtualCellCount;
                item.topVirtualCellCount = _this.topVirtualCellCount;
                item.leftVirtualWidth = _this.leftVirtualCellCount * _this.cellWidth;
                item.topVirtualHeight = _this.topVirtualCellCount * _this.cellHeight
            }));
            map.push(coordinates)
        }
        var positionArray = this._getSortedPositions(map);
        var resultPositions = this._getResultPositions(positionArray);
        return this._getExtendedPositionMap(map, resultPositions)
    };
    _proto._getDeltaWidth = function(args, initialSize) {
        var intervalWidth = this.resizableStep || this.getAppointmentMinSize();
        var initialWidth = initialSize.width;
        return Math.round((args.width - initialWidth) / intervalWidth)
    };
    _proto._correctRtlCoordinates = function(coordinates) {
        var width = coordinates[0].width || this._getAppointmentMaxWidth();
        coordinates.forEach((function(coordinate) {
            if (!coordinate.appointmentReduced) {
                coordinate.left -= width
            }
        }));
        return coordinates
    };
    _proto._getAppointmentMaxWidth = function() {
        return this.cellWidth
    };
    _proto._getItemPosition = function(appointment) {
        var position = this.generateAppointmentSettings(appointment);
        var allDay = this.isAllDay(appointment);
        var result = [];
        for (var j = 0; j < position.length; j++) {
            var height = this.calculateAppointmentHeight(appointment, position[j]);
            var width = this.calculateAppointmentWidth(appointment, position[j]);
            var resultWidth = width;
            var appointmentReduced = null;
            var multiWeekAppointmentParts = [];
            var initialRowIndex = position[j].rowIndex;
            var initialColumnIndex = position[j].columnIndex;
            if (this._needVerifyItemSize() || allDay) {
                var currentMaxAllowedPosition = position[j].hMax;
                if (this.isAppointmentGreaterThan(currentMaxAllowedPosition, {
                        left: position[j].left,
                        width: width
                    })) {
                    appointmentReduced = "head";
                    initialRowIndex = position[j].rowIndex;
                    initialColumnIndex = position[j].columnIndex;
                    resultWidth = this._reduceMultiWeekAppointment(width, {
                        left: position[j].left,
                        right: currentMaxAllowedPosition
                    });
                    multiWeekAppointmentParts = this._getAppointmentParts({
                        sourceAppointmentWidth: width,
                        reducedWidth: resultWidth,
                        height: height
                    }, position[j]);
                    if (this.rtlEnabled) {
                        position[j].left = currentMaxAllowedPosition
                    }
                }
            }(0, _extend.extend)(position[j], {
                height: height,
                width: resultWidth,
                allDay: allDay,
                rowIndex: initialRowIndex,
                columnIndex: initialColumnIndex,
                appointmentReduced: appointmentReduced
            });
            result = this._getAppointmentPartsPosition(multiWeekAppointmentParts, position[j], result)
        }
        return result
    };
    _proto._getAppointmentPartsPosition = function(appointmentParts, position, result) {
        if (appointmentParts.length) {
            appointmentParts.unshift(position);
            result = result.concat(appointmentParts)
        } else {
            result.push(position)
        }
        return result
    };
    _proto.getAppointmentSettingsGenerator = function(rawAppointment) {
        return new _settingsGenerator.AppointmentSettingsGenerator(_extends({
            rawAppointment: rawAppointment,
            appointmentTakesAllDay: this.isAppointmentTakesAllDay(rawAppointment),
            getPositionShiftCallback: this.getPositionShift.bind(this)
        }, this.options))
    };
    _proto.generateAppointmentSettings = function(rawAppointment) {
        return this.getAppointmentSettingsGenerator(rawAppointment).create()
    };
    _proto.isAppointmentTakesAllDay = function(rawAppointment) {
        var adapter = (0, _appointmentAdapter.createAppointmentAdapter)(rawAppointment, this.dataAccessors, this.timeZoneCalculator);
        return (0, _utils2.getAppointmentTakesAllDay)(adapter, this.viewStartDayHour, this.viewEndDayHour)
    };
    _proto._getAppointmentParts = function() {
        return []
    };
    _proto._getCompactAppointmentParts = function(appointmentWidth) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        return Math.round(appointmentWidth / cellWidth)
    };
    _proto._reduceMultiWeekAppointment = function(sourceAppointmentWidth, bound) {
        if (this.rtlEnabled) {
            sourceAppointmentWidth = Math.floor(bound.left - bound.right)
        } else {
            sourceAppointmentWidth = bound.right - Math.floor(bound.left)
        }
        return sourceAppointmentWidth
    };
    _proto.calculateAppointmentHeight = function() {
        return 0
    };
    _proto.calculateAppointmentWidth = function() {
        return 0
    };
    _proto.isAppointmentGreaterThan = function(etalon, comparisonParameters) {
        var result = comparisonParameters.left + comparisonParameters.width - etalon;
        if (this.rtlEnabled) {
            result = etalon + comparisonParameters.width - comparisonParameters.left
        }
        return result > this.cellWidth / 2
    };
    _proto.isAllDay = function() {
        return false
    };
    _proto.cropAppointmentWidth = function(width, cellWidth) {
        return this.isGroupedByDate ? cellWidth : width
    };
    _proto._getSortedPositions = function(positionList) {
        var _this2 = this;
        var result = [];
        var round = function(value) {
            return Math.round(100 * value) / 100
        };
        var createItem = function(rowIndex, columnIndex, top, left, bottom, right, position, allDay) {
            return {
                i: rowIndex,
                j: columnIndex,
                top: round(top),
                left: round(left),
                bottom: round(bottom),
                right: round(right),
                cellPosition: position,
                allDay: allDay
            }
        };
        for (var rowIndex = 0, rowCount = positionList.length; rowIndex < rowCount; rowIndex++) {
            for (var columnIndex = 0, cellCount = positionList[rowIndex].length; columnIndex < cellCount; columnIndex++) {
                var _positionList$rowInde = positionList[rowIndex][columnIndex],
                    top = _positionList$rowInde.top,
                    left = _positionList$rowInde.left,
                    height = _positionList$rowInde.height,
                    width = _positionList$rowInde.width,
                    cellPosition = _positionList$rowInde.cellPosition,
                    allDay = _positionList$rowInde.allDay;
                result.push(createItem(rowIndex, columnIndex, top, left, top + height, left + width, cellPosition, allDay))
            }
        }
        return result.sort((function(a, b) {
            return _this2._sortCondition(a, b)
        }))
    };
    _proto._sortCondition = function() {};
    _proto._getConditions = function(a, b) {
        var isSomeEdge = this._isSomeEdge(a, b);
        return {
            columnCondition: isSomeEdge || this._normalizeCondition(a.left, b.left),
            rowCondition: isSomeEdge || this._normalizeCondition(a.top, b.top),
            cellPositionCondition: isSomeEdge || this._normalizeCondition(a.cellPosition, b.cellPosition)
        }
    };
    _proto._rowCondition = function(a, b) {
        var conditions = this._getConditions(a, b);
        return conditions.columnCondition || conditions.rowCondition
    };
    _proto._columnCondition = function(a, b) {
        var conditions = this._getConditions(a, b);
        return conditions.rowCondition || conditions.columnCondition
    };
    _proto._isSomeEdge = function(a, b) {
        return a.i === b.i && a.j === b.j
    };
    _proto._normalizeCondition = function(first, second) {
        var result = first - second;
        return Math.abs(result) > 1 ? result : 0
    };
    _proto._isItemsCross = function(firstItem, secondItem) {
        var areItemsInTheSameTable = !!firstItem.allDay === !!secondItem.allDay;
        var areItemsAllDay = firstItem.allDay && secondItem.allDay;
        if (areItemsInTheSameTable) {
            var orientation = this._getOrientation(areItemsAllDay);
            return this._checkItemsCrossing(firstItem, secondItem, orientation)
        } else {
            return false
        }
    };
    _proto._checkItemsCrossing = function(firstItem, secondItem, orientation) {
        var firstItemSide_1 = Math.floor(firstItem[orientation[0]]);
        var firstItemSide_2 = Math.floor(firstItem[orientation[1]]);
        var secondItemSide_1 = Math.ceil(secondItem[orientation[0]]);
        var secondItemSide_2 = Math.ceil(secondItem[orientation[1]]);
        var isItemCross = Math.abs(firstItem[orientation[2]] - secondItem[orientation[2]]) <= 1;
        return isItemCross && (firstItemSide_1 <= secondItemSide_1 && firstItemSide_2 > secondItemSide_1 || firstItemSide_1 < secondItemSide_2 && firstItemSide_2 >= secondItemSide_2 || firstItemSide_1 === secondItemSide_1 && firstItemSide_2 === secondItemSide_2)
    };
    _proto._getOrientation = function(isAllDay) {
        return isAllDay ? ["left", "right", "top"] : ["top", "bottom", "left"]
    };
    _proto._getResultPositions = function(sortedArray) {
        var _this3 = this;
        var result = [];
        var i;
        var sortedIndex = 0;
        var currentItem;
        var indexes;
        var itemIndex;
        var maxIndexInStack = 0;
        var stack = {};
        var findFreeIndex = function findFreeIndex(indexes, index) {
            var isFind = indexes.some((function(item) {
                return item === index
            }));
            if (isFind) {
                return findFreeIndex(indexes, ++index)
            } else {
                return index
            }
        };
        var createItem = function(currentItem, index) {
            var currentIndex = index || 0;
            return {
                index: currentIndex,
                i: currentItem.i,
                j: currentItem.j,
                left: currentItem.left,
                right: currentItem.right,
                top: currentItem.top,
                bottom: currentItem.bottom,
                allDay: currentItem.allDay,
                sortedIndex: _this3._skipSortedIndex(currentIndex) ? null : sortedIndex++
            }
        };
        var startNewStack = function(currentItem) {
            stack.items = [createItem(currentItem)];
            stack.left = currentItem.left;
            stack.right = currentItem.right;
            stack.top = currentItem.top;
            stack.bottom = currentItem.bottom;
            stack.allDay = currentItem.allDay
        };
        var pushItemsInResult = function(items) {
            items.forEach((function(item) {
                result.push({
                    index: item.index,
                    count: maxIndexInStack + 1,
                    i: item.i,
                    j: item.j,
                    sortedIndex: item.sortedIndex
                })
            }))
        };
        for (i = 0; i < sortedArray.length; i++) {
            currentItem = sortedArray[i];
            indexes = [];
            if (!stack.items) {
                startNewStack(currentItem)
            } else if (this._isItemsCross(stack, currentItem)) {
                stack.items.forEach((function(item, index) {
                    if (_this3._isItemsCross(item, currentItem)) {
                        indexes.push(item.index)
                    }
                }));
                itemIndex = indexes.length ? findFreeIndex(indexes, 0) : 0;
                stack.items.push(createItem(currentItem, itemIndex));
                maxIndexInStack = Math.max(itemIndex, maxIndexInStack);
                stack.left = Math.min(stack.left, currentItem.left);
                stack.right = Math.max(stack.right, currentItem.right);
                stack.top = Math.min(stack.top, currentItem.top);
                stack.bottom = Math.max(stack.bottom, currentItem.bottom);
                stack.allDay = currentItem.allDay
            } else {
                pushItemsInResult(stack.items);
                stack = {};
                startNewStack(currentItem);
                maxIndexInStack = 0
            }
        }
        if (stack.items) {
            pushItemsInResult(stack.items)
        }
        return result.sort((function(a, b) {
            var columnCondition = a.j - b.j;
            var rowCondition = a.i - b.i;
            return rowCondition ? rowCondition : columnCondition
        }))
    };
    _proto._skipSortedIndex = function(index) {
        return index > this._getMaxAppointmentCountPerCell() - 1
    };
    _proto._findIndexByKey = function(arr, iKey, jKey, iValue, jValue) {
        var result = 0;
        for (var i = 0, len = arr.length; i < len; i++) {
            if (arr[i][iKey] === iValue && arr[i][jKey] === jValue) {
                result = i;
                break
            }
        }
        return result
    };
    _proto._getExtendedPositionMap = function(map, positions) {
        var positionCounter = 0;
        var result = [];
        for (var i = 0, mapLength = map.length; i < mapLength; i++) {
            var resultString = [];
            for (var j = 0, itemLength = map[i].length; j < itemLength; j++) {
                map[i][j].index = positions[positionCounter].index;
                map[i][j].sortedIndex = positions[positionCounter].sortedIndex;
                map[i][j].count = positions[positionCounter++].count;
                resultString.push(map[i][j]);
                this._checkLongCompactAppointment(map[i][j], resultString)
            }
            result.push(resultString)
        }
        return result
    };
    _proto._checkLongCompactAppointment = function(item, result) {
        this._splitLongCompactAppointment(item, result);
        return result
    };
    _proto._splitLongCompactAppointment = function(item, result) {
        var appointmentCountPerCell = this._getMaxAppointmentCountPerCellByType(item.allDay);
        var compactCount = 0;
        if (void 0 !== appointmentCountPerCell && item.index > appointmentCountPerCell - 1) {
            item.isCompact = true;
            compactCount = this._getCompactAppointmentParts(item.width);
            for (var k = 1; k < compactCount; k++) {
                var compactPart = (0, _extend.extend)(true, {}, item);
                compactPart.left = this._getCompactLeftCoordinate(item.left, k);
                compactPart.columnIndex = compactPart.columnIndex + k;
                compactPart.sortedIndex = null;
                result.push(compactPart)
            }
        }
        return result
    };
    _proto._adjustDurationByDaylightDiff = function(duration, startDate, endDate) {
        var daylightDiff = _utils.default.getDaylightOffset(startDate, endDate);
        return this._needAdjustDuration(daylightDiff) ? this._calculateDurationByDaylightDiff(duration, daylightDiff) : duration
    };
    _proto._needAdjustDuration = function(diff) {
        return 0 !== diff
    };
    _proto._calculateDurationByDaylightDiff = function(duration, diff) {
        return duration + diff * toMs("minute")
    };
    _proto._getCollectorLeftOffset = function(isAllDay) {
        if (isAllDay || !this.isApplyCompactAppointmentOffset()) {
            return 0
        }
        var dropDownButtonWidth = this.getDropDownAppointmentWidth(this.intervalCount, isAllDay);
        var rightOffset = this._isCompactTheme() ? COMPACT_THEME_WEEK_VIEW_COLLECTOR_OFFSET : WEEK_VIEW_COLLECTOR_OFFSET;
        return this.cellWidth - dropDownButtonWidth - rightOffset
    };
    _proto._markAppointmentAsVirtual = function(coordinates) {
        var isAllDay = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
        var countFullWidthAppointmentInCell = this._getMaxAppointmentCountPerCellByType(isAllDay);
        if (coordinates.count - countFullWidthAppointmentInCell > 0) {
            var top = coordinates.top,
                left = coordinates.left;
            var compactRender = this.isAdaptive || !isAllDay && this.supportCompactDropDownAppointments();
            coordinates.virtual = {
                left: left + this._getCollectorLeftOffset(isAllDay),
                top: top,
                width: this.getDropDownAppointmentWidth(this.intervalCount, isAllDay),
                height: this.getDropDownAppointmentHeight(),
                index: this._generateAppointmentCollectorIndex(coordinates, isAllDay),
                isAllDay: isAllDay,
                isCompact: compactRender
            }
        }
    };
    _proto.isApplyCompactAppointmentOffset = function() {
        return this.supportCompactDropDownAppointments()
    };
    _proto.supportCompactDropDownAppointments = function() {
        return true
    };
    _proto._generateAppointmentCollectorIndex = function(_ref, isAllDay) {
        var groupIndex = _ref.groupIndex,
            rowIndex = _ref.rowIndex,
            columnIndex = _ref.columnIndex;
        return "".concat(groupIndex, "-").concat(rowIndex, "-").concat(columnIndex, "-").concat(isAllDay)
    };
    _proto._getMaxAppointmentCountPerCellByType = function(isAllDay) {
        var appointmentCountPerCell = this._getMaxAppointmentCountPerCell();
        if ((0, _type.isObject)(appointmentCountPerCell)) {
            return isAllDay ? appointmentCountPerCell.allDay : appointmentCountPerCell.simple
        } else {
            return appointmentCountPerCell
        }
    };
    _proto.getDropDownAppointmentWidth = function(intervalCount, isAllDay) {
        return this.getPositioningStrategy().getDropDownAppointmentWidth(intervalCount, isAllDay)
    };
    _proto.getDropDownAppointmentHeight = function() {
        return this.getPositioningStrategy().getDropDownAppointmentHeight()
    };
    _proto.getDropDownButtonAdaptiveSize = function() {
        return DROP_DOWN_BUTTON_ADAPTIVE_SIZE
    };
    _proto.getCollectorTopOffset = function(allDay) {
        return this.getPositioningStrategy().getCollectorTopOffset(allDay)
    };
    _proto.getCollectorLeftOffset = function() {
        return this.getPositioningStrategy().getCollectorLeftOffset()
    };
    _proto.getAppointmentDataCalculator = function() {};
    _proto._customizeCoordinates = function(coordinates, height, appointmentCountPerCell, topOffset, isAllDay) {
        var index = coordinates.index;
        var appointmentHeight = height / appointmentCountPerCell;
        var appointmentTop = coordinates.top + index * appointmentHeight;
        var top = appointmentTop + topOffset;
        var width = coordinates.width;
        var left = coordinates.left;
        if (coordinates.isCompact) {
            this.isAdaptive && this._correctCollectorCoordinatesInAdaptive(coordinates, isAllDay);
            this._markAppointmentAsVirtual(coordinates, isAllDay)
        }
        return {
            height: appointmentHeight,
            width: width,
            top: top,
            left: left,
            empty: this._isAppointmentEmpty(height, width)
        }
    };
    _proto._isAppointmentEmpty = function(height, width) {
        return height < this._getAppointmentMinHeight() || width < this._getAppointmentMinWidth()
    };
    _proto._calculateGeometryConfig = function(coordinates) {
        var overlappingMode = this.maxAppointmentsPerCell;
        var offsets = this._getOffsets();
        var appointmentDefaultOffset = this._getAppointmentDefaultOffset();
        var appointmentCountPerCell = this._getAppointmentCount(overlappingMode, coordinates);
        var ratio = this._getDefaultRatio(coordinates, appointmentCountPerCell);
        var maxHeight = this._getMaxHeight();
        if (!(0, _type.isNumeric)(appointmentCountPerCell)) {
            appointmentCountPerCell = coordinates.count;
            ratio = (maxHeight - offsets.unlimited) / maxHeight
        }
        var topOffset = (1 - ratio) * maxHeight;
        if ("auto" === overlappingMode || (0, _type.isNumeric)(overlappingMode)) {
            ratio = 1;
            maxHeight -= appointmentDefaultOffset;
            topOffset = appointmentDefaultOffset
        }
        return {
            height: ratio * maxHeight,
            appointmentCountPerCell: appointmentCountPerCell,
            offset: topOffset
        }
    };
    _proto._getAppointmentCount = function() {};
    _proto._getDefaultRatio = function() {};
    _proto._getOffsets = function() {};
    _proto._getMaxHeight = function() {};
    _proto._needVerifyItemSize = function() {
        return false
    };
    _proto._getMaxAppointmentCountPerCell = function() {
        if (!this._maxAppointmentCountPerCell) {
            var overlappingMode = this.maxAppointmentsPerCell;
            var appointmentCountPerCell;
            if ((0, _type.isNumeric)(overlappingMode)) {
                appointmentCountPerCell = overlappingMode
            }
            if ("auto" === overlappingMode) {
                appointmentCountPerCell = this._getDynamicAppointmentCountPerCell()
            }
            if ("unlimited" === overlappingMode) {
                appointmentCountPerCell = void 0
            }
            this._maxAppointmentCountPerCell = appointmentCountPerCell
        }
        return this._maxAppointmentCountPerCell
    };
    _proto._getDynamicAppointmentCountPerCell = function() {
        return this.getPositioningStrategy().getDynamicAppointmentCountPerCell()
    };
    _proto.allDaySupported = function() {
        return false
    };
    _proto._isCompactTheme = function() {
        return "compact" === ((0, _themes.current)() || "").split(".").pop()
    };
    _proto._getAppointmentDefaultOffset = function() {
        return this.getPositioningStrategy().getAppointmentDefaultOffset()
    };
    _proto._getAppointmentDefaultHeight = function() {
        return this._getAppointmentHeightByTheme()
    };
    _proto._getAppointmentMinHeight = function() {
        return this._getAppointmentDefaultHeight()
    };
    _proto._getAppointmentHeightByTheme = function() {
        return this._isCompactTheme() ? COMPACT_THEME_APPOINTMENT_DEFAULT_HEIGHT : APPOINTMENT_DEFAULT_HEIGHT
    };
    _proto._getAppointmentDefaultWidth = function() {
        return this.getPositioningStrategy()._getAppointmentDefaultWidth()
    };
    _proto._getAppointmentMinWidth = function() {
        return this._getAppointmentDefaultWidth()
    };
    _proto._needVerticalGroupBounds = function() {
        return false
    };
    _proto._needHorizontalGroupBounds = function() {
        return false
    };
    _proto.getAppointmentDurationInMs = function(startDate, endDate, allDay) {
        var appointmentDuration = endDate.getTime() - startDate.getTime();
        var dayDuration = toMs("day");
        var visibleDayDuration = this.visibleDayDuration;
        var result = 0;
        if (allDay) {
            var ceilQuantityOfDays = Math.ceil(appointmentDuration / dayDuration);
            result = ceilQuantityOfDays * visibleDayDuration
        } else {
            var isDifferentDates = !_utils.default.isSameAppointmentDates(startDate, endDate);
            var floorQuantityOfDays = Math.floor(appointmentDuration / dayDuration);
            var tailDuration;
            if (isDifferentDates) {
                var startDateEndHour = new Date(new Date(startDate).setHours(this.endDayHour, 0, 0));
                var hiddenDayDuration = dayDuration - visibleDayDuration - (startDate.getTime() > startDateEndHour.getTime() ? startDate.getTime() - startDateEndHour.getTime() : 0);
                tailDuration = appointmentDuration - (floorQuantityOfDays ? floorQuantityOfDays * dayDuration : hiddenDayDuration);
                var startDayTime = this.startDayHour * toMs("hour");
                var endPartDuration = endDate - _date.default.trimTime(endDate);
                if (endPartDuration < startDayTime) {
                    if (floorQuantityOfDays) {
                        tailDuration -= hiddenDayDuration
                    }
                    tailDuration += startDayTime - endPartDuration
                }
            } else {
                tailDuration = appointmentDuration % dayDuration
            }
            if (tailDuration > visibleDayDuration) {
                tailDuration = visibleDayDuration
            }
            result = floorQuantityOfDays * visibleDayDuration + tailDuration || toMs("minute")
        }
        return result
    };
    _proto.getPositionShift = function(timeShift, isAllDay) {
        return {
            top: timeShift * this.cellHeight,
            left: 0,
            cellPosition: 0
        }
    };
    _createClass(BaseRenderingStrategy, [{
        key: "isAdaptive",
        get: function() {
            return this.options.adaptivityEnabled
        }
    }, {
        key: "rtlEnabled",
        get: function() {
            return this.options.rtlEnabled
        }
    }, {
        key: "startDayHour",
        get: function() {
            return this.options.startDayHour
        }
    }, {
        key: "endDayHour",
        get: function() {
            return this.options.endDayHour
        }
    }, {
        key: "maxAppointmentsPerCell",
        get: function() {
            return this.options.maxAppointmentsPerCell
        }
    }, {
        key: "cellWidth",
        get: function() {
            return this.options.cellWidth
        }
    }, {
        key: "cellHeight",
        get: function() {
            return this.options.cellHeight
        }
    }, {
        key: "allDayHeight",
        get: function() {
            return this.options.allDayHeight
        }
    }, {
        key: "resizableStep",
        get: function() {
            return this.options.resizableStep
        }
    }, {
        key: "isGroupedByDate",
        get: function() {
            return this.options.isGroupedByDate
        }
    }, {
        key: "visibleDayDuration",
        get: function() {
            return this.options.visibleDayDuration
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
        key: "cellDurationInMinutes",
        get: function() {
            return this.options.cellDurationInMinutes
        }
    }, {
        key: "leftVirtualCellCount",
        get: function() {
            return this.options.leftVirtualCellCount
        }
    }, {
        key: "topVirtualCellCount",
        get: function() {
            return this.options.topVirtualCellCount
        }
    }, {
        key: "positionHelper",
        get: function() {
            return this.options.positionHelper
        }
    }, {
        key: "showAllDayPanel",
        get: function() {
            return this.options.showAllDayPanel
        }
    }, {
        key: "isGroupedAllDayPanel",
        get: function() {
            return this.options.isGroupedAllDayPanel
        }
    }, {
        key: "groupOrientation",
        get: function() {
            return this.options.groupOrientation
        }
    }, {
        key: "rowCount",
        get: function() {
            return this.options.rowCount
        }
    }, {
        key: "groupCount",
        get: function() {
            return this.options.groupCount
        }
    }, {
        key: "currentDate",
        get: function() {
            return this.options.currentDate
        }
    }, {
        key: "appointmentCountPerCell",
        get: function() {
            return this.options.appointmentCountPerCell
        }
    }, {
        key: "appointmentOffset",
        get: function() {
            return this.options.appointmentOffset
        }
    }, {
        key: "allowResizing",
        get: function() {
            return this.options.allowResizing
        }
    }, {
        key: "allowAllDayResizing",
        get: function() {
            return this.options.allowAllDayResizing
        }
    }, {
        key: "viewDataProvider",
        get: function() {
            return this.options.viewDataProvider
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
        key: "intervalCount",
        get: function() {
            return this.options.intervalCount
        }
    }, {
        key: "isVirtualScrolling",
        get: function() {
            return this.options.isVirtualScrolling
        }
    }]);
    return BaseRenderingStrategy
}();
var _default = BaseRenderingStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
