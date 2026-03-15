/**
 * DevExtreme (esm/ui/scheduler/appointments/rendering_strategies/strategy.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import AppointmentPositioningStrategy from "./appointmentsPositioning_strategy_base";
import AdaptivePositioningStrategy from "./appointmentsPositioning_strategy_adaptive";
import {
    extend
} from "../../../../core/utils/extend";
import dateUtils from "../../../../core/utils/date";
import {
    isNumeric,
    isObject
} from "../../../../core/utils/type";
import {
    current as currentTheme
} from "../../../themes";
import {
    AppointmentSettingsGenerator
} from "../settingsGenerator";
import timeZoneUtils from "../../utils.timeZone";
import {
    createAppointmentAdapter
} from "../../appointmentAdapter";
import {
    getAppointmentTakesAllDay
} from "../dataProvider/utils";
var toMs = dateUtils.dateToMilliseconds;
var APPOINTMENT_MIN_SIZE = 2;
var APPOINTMENT_DEFAULT_HEIGHT = 20;
var COMPACT_THEME_APPOINTMENT_DEFAULT_HEIGHT = 18;
var DROP_DOWN_BUTTON_ADAPTIVE_SIZE = 28;
var WEEK_VIEW_COLLECTOR_OFFSET = 5;
var COMPACT_THEME_WEEK_VIEW_COLLECTOR_OFFSET = 1;
class BaseRenderingStrategy {
    constructor(options) {
        this.options = options;
        this._initPositioningStrategy()
    }
    get isAdaptive() {
        return this.options.adaptivityEnabled
    }
    get rtlEnabled() {
        return this.options.rtlEnabled
    }
    get startDayHour() {
        return this.options.startDayHour
    }
    get endDayHour() {
        return this.options.endDayHour
    }
    get maxAppointmentsPerCell() {
        return this.options.maxAppointmentsPerCell
    }
    get cellWidth() {
        return this.options.cellWidth
    }
    get cellHeight() {
        return this.options.cellHeight
    }
    get allDayHeight() {
        return this.options.allDayHeight
    }
    get resizableStep() {
        return this.options.resizableStep
    }
    get isGroupedByDate() {
        return this.options.isGroupedByDate
    }
    get visibleDayDuration() {
        return this.options.visibleDayDuration
    }
    get viewStartDayHour() {
        return this.options.viewStartDayHour
    }
    get viewEndDayHour() {
        return this.options.viewEndDayHour
    }
    get cellDuration() {
        return this.options.cellDuration
    }
    get cellDurationInMinutes() {
        return this.options.cellDurationInMinutes
    }
    get leftVirtualCellCount() {
        return this.options.leftVirtualCellCount
    }
    get topVirtualCellCount() {
        return this.options.topVirtualCellCount
    }
    get positionHelper() {
        return this.options.positionHelper
    }
    get showAllDayPanel() {
        return this.options.showAllDayPanel
    }
    get isGroupedAllDayPanel() {
        return this.options.isGroupedAllDayPanel
    }
    get groupOrientation() {
        return this.options.groupOrientation
    }
    get rowCount() {
        return this.options.rowCount
    }
    get groupCount() {
        return this.options.groupCount
    }
    get currentDate() {
        return this.options.currentDate
    }
    get appointmentCountPerCell() {
        return this.options.appointmentCountPerCell
    }
    get appointmentOffset() {
        return this.options.appointmentOffset
    }
    get allowResizing() {
        return this.options.allowResizing
    }
    get allowAllDayResizing() {
        return this.options.allowAllDayResizing
    }
    get viewDataProvider() {
        return this.options.viewDataProvider
    }
    get dataAccessors() {
        return this.options.dataAccessors
    }
    get timeZoneCalculator() {
        return this.options.timeZoneCalculator
    }
    get intervalCount() {
        return this.options.intervalCount
    }
    get isVirtualScrolling() {
        return this.options.isVirtualScrolling
    }
    _correctCollectorCoordinatesInAdaptive(coordinates, isAllDay) {
        coordinates.top = coordinates.top + this.getCollectorTopOffset(isAllDay);
        coordinates.left = coordinates.left + this.getCollectorLeftOffset()
    }
    _initPositioningStrategy() {
        this._positioningStrategy = this.isAdaptive ? new AdaptivePositioningStrategy(this) : new AppointmentPositioningStrategy(this)
    }
    getPositioningStrategy() {
        return this._positioningStrategy
    }
    getAppointmentMinSize() {
        return APPOINTMENT_MIN_SIZE
    }
    keepAppointmentSettings() {
        return false
    }
    getDeltaTime() {}
    getAppointmentGeometry(coordinates) {
        return coordinates
    }
    needCorrectAppointmentDates() {
        return true
    }
    getDirection() {
        return "horizontal"
    }
    createTaskPositionMap(items) {
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
            coordinates.forEach(item => {
                item.leftVirtualCellCount = this.leftVirtualCellCount;
                item.topVirtualCellCount = this.topVirtualCellCount;
                item.leftVirtualWidth = this.leftVirtualCellCount * this.cellWidth;
                item.topVirtualHeight = this.topVirtualCellCount * this.cellHeight
            });
            map.push(coordinates)
        }
        var positionArray = this._getSortedPositions(map);
        var resultPositions = this._getResultPositions(positionArray);
        return this._getExtendedPositionMap(map, resultPositions)
    }
    _getDeltaWidth(args, initialSize) {
        var intervalWidth = this.resizableStep || this.getAppointmentMinSize();
        var initialWidth = initialSize.width;
        return Math.round((args.width - initialWidth) / intervalWidth)
    }
    _correctRtlCoordinates(coordinates) {
        var width = coordinates[0].width || this._getAppointmentMaxWidth();
        coordinates.forEach(coordinate => {
            if (!coordinate.appointmentReduced) {
                coordinate.left -= width
            }
        });
        return coordinates
    }
    _getAppointmentMaxWidth() {
        return this.cellWidth
    }
    _getItemPosition(appointment) {
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
            }
            extend(position[j], {
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
    }
    _getAppointmentPartsPosition(appointmentParts, position, result) {
        if (appointmentParts.length) {
            appointmentParts.unshift(position);
            result = result.concat(appointmentParts)
        } else {
            result.push(position)
        }
        return result
    }
    getAppointmentSettingsGenerator(rawAppointment) {
        return new AppointmentSettingsGenerator(_extends({
            rawAppointment: rawAppointment,
            appointmentTakesAllDay: this.isAppointmentTakesAllDay(rawAppointment),
            getPositionShiftCallback: this.getPositionShift.bind(this)
        }, this.options))
    }
    generateAppointmentSettings(rawAppointment) {
        return this.getAppointmentSettingsGenerator(rawAppointment).create()
    }
    isAppointmentTakesAllDay(rawAppointment) {
        var adapter = createAppointmentAdapter(rawAppointment, this.dataAccessors, this.timeZoneCalculator);
        return getAppointmentTakesAllDay(adapter, this.viewStartDayHour, this.viewEndDayHour)
    }
    _getAppointmentParts() {
        return []
    }
    _getCompactAppointmentParts(appointmentWidth) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        return Math.round(appointmentWidth / cellWidth)
    }
    _reduceMultiWeekAppointment(sourceAppointmentWidth, bound) {
        if (this.rtlEnabled) {
            sourceAppointmentWidth = Math.floor(bound.left - bound.right)
        } else {
            sourceAppointmentWidth = bound.right - Math.floor(bound.left)
        }
        return sourceAppointmentWidth
    }
    calculateAppointmentHeight() {
        return 0
    }
    calculateAppointmentWidth() {
        return 0
    }
    isAppointmentGreaterThan(etalon, comparisonParameters) {
        var result = comparisonParameters.left + comparisonParameters.width - etalon;
        if (this.rtlEnabled) {
            result = etalon + comparisonParameters.width - comparisonParameters.left
        }
        return result > this.cellWidth / 2
    }
    isAllDay() {
        return false
    }
    cropAppointmentWidth(width, cellWidth) {
        return this.isGroupedByDate ? cellWidth : width
    }
    _getSortedPositions(positionList) {
        var result = [];
        var round = value => Math.round(100 * value) / 100;
        var createItem = (rowIndex, columnIndex, top, left, bottom, right, position, allDay) => ({
            i: rowIndex,
            j: columnIndex,
            top: round(top),
            left: round(left),
            bottom: round(bottom),
            right: round(right),
            cellPosition: position,
            allDay: allDay
        });
        for (var rowIndex = 0, rowCount = positionList.length; rowIndex < rowCount; rowIndex++) {
            for (var columnIndex = 0, cellCount = positionList[rowIndex].length; columnIndex < cellCount; columnIndex++) {
                var {
                    top: top,
                    left: left,
                    height: height,
                    width: width,
                    cellPosition: cellPosition,
                    allDay: allDay
                } = positionList[rowIndex][columnIndex];
                result.push(createItem(rowIndex, columnIndex, top, left, top + height, left + width, cellPosition, allDay))
            }
        }
        return result.sort((a, b) => this._sortCondition(a, b))
    }
    _sortCondition() {}
    _getConditions(a, b) {
        var isSomeEdge = this._isSomeEdge(a, b);
        return {
            columnCondition: isSomeEdge || this._normalizeCondition(a.left, b.left),
            rowCondition: isSomeEdge || this._normalizeCondition(a.top, b.top),
            cellPositionCondition: isSomeEdge || this._normalizeCondition(a.cellPosition, b.cellPosition)
        }
    }
    _rowCondition(a, b) {
        var conditions = this._getConditions(a, b);
        return conditions.columnCondition || conditions.rowCondition
    }
    _columnCondition(a, b) {
        var conditions = this._getConditions(a, b);
        return conditions.rowCondition || conditions.columnCondition
    }
    _isSomeEdge(a, b) {
        return a.i === b.i && a.j === b.j
    }
    _normalizeCondition(first, second) {
        var result = first - second;
        return Math.abs(result) > 1 ? result : 0
    }
    _isItemsCross(firstItem, secondItem) {
        var areItemsInTheSameTable = !!firstItem.allDay === !!secondItem.allDay;
        var areItemsAllDay = firstItem.allDay && secondItem.allDay;
        if (areItemsInTheSameTable) {
            var orientation = this._getOrientation(areItemsAllDay);
            return this._checkItemsCrossing(firstItem, secondItem, orientation)
        } else {
            return false
        }
    }
    _checkItemsCrossing(firstItem, secondItem, orientation) {
        var firstItemSide_1 = Math.floor(firstItem[orientation[0]]);
        var firstItemSide_2 = Math.floor(firstItem[orientation[1]]);
        var secondItemSide_1 = Math.ceil(secondItem[orientation[0]]);
        var secondItemSide_2 = Math.ceil(secondItem[orientation[1]]);
        var isItemCross = Math.abs(firstItem[orientation[2]] - secondItem[orientation[2]]) <= 1;
        return isItemCross && (firstItemSide_1 <= secondItemSide_1 && firstItemSide_2 > secondItemSide_1 || firstItemSide_1 < secondItemSide_2 && firstItemSide_2 >= secondItemSide_2 || firstItemSide_1 === secondItemSide_1 && firstItemSide_2 === secondItemSide_2)
    }
    _getOrientation(isAllDay) {
        return isAllDay ? ["left", "right", "top"] : ["top", "bottom", "left"]
    }
    _getResultPositions(sortedArray) {
        var result = [];
        var i;
        var sortedIndex = 0;
        var currentItem;
        var indexes;
        var itemIndex;
        var maxIndexInStack = 0;
        var stack = {};
        var findFreeIndex = (indexes, index) => {
            var isFind = indexes.some(item => item === index);
            if (isFind) {
                return findFreeIndex(indexes, ++index)
            } else {
                return index
            }
        };
        var createItem = (currentItem, index) => {
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
                sortedIndex: this._skipSortedIndex(currentIndex) ? null : sortedIndex++
            }
        };
        var startNewStack = currentItem => {
            stack.items = [createItem(currentItem)];
            stack.left = currentItem.left;
            stack.right = currentItem.right;
            stack.top = currentItem.top;
            stack.bottom = currentItem.bottom;
            stack.allDay = currentItem.allDay
        };
        var pushItemsInResult = items => {
            items.forEach(item => {
                result.push({
                    index: item.index,
                    count: maxIndexInStack + 1,
                    i: item.i,
                    j: item.j,
                    sortedIndex: item.sortedIndex
                })
            })
        };
        for (i = 0; i < sortedArray.length; i++) {
            currentItem = sortedArray[i];
            indexes = [];
            if (!stack.items) {
                startNewStack(currentItem)
            } else if (this._isItemsCross(stack, currentItem)) {
                stack.items.forEach((item, index) => {
                    if (this._isItemsCross(item, currentItem)) {
                        indexes.push(item.index)
                    }
                });
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
    }
    _skipSortedIndex(index) {
        return index > this._getMaxAppointmentCountPerCell() - 1
    }
    _findIndexByKey(arr, iKey, jKey, iValue, jValue) {
        var result = 0;
        for (var i = 0, len = arr.length; i < len; i++) {
            if (arr[i][iKey] === iValue && arr[i][jKey] === jValue) {
                result = i;
                break
            }
        }
        return result
    }
    _getExtendedPositionMap(map, positions) {
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
    }
    _checkLongCompactAppointment(item, result) {
        this._splitLongCompactAppointment(item, result);
        return result
    }
    _splitLongCompactAppointment(item, result) {
        var appointmentCountPerCell = this._getMaxAppointmentCountPerCellByType(item.allDay);
        var compactCount = 0;
        if (void 0 !== appointmentCountPerCell && item.index > appointmentCountPerCell - 1) {
            item.isCompact = true;
            compactCount = this._getCompactAppointmentParts(item.width);
            for (var k = 1; k < compactCount; k++) {
                var compactPart = extend(true, {}, item);
                compactPart.left = this._getCompactLeftCoordinate(item.left, k);
                compactPart.columnIndex = compactPart.columnIndex + k;
                compactPart.sortedIndex = null;
                result.push(compactPart)
            }
        }
        return result
    }
    _adjustDurationByDaylightDiff(duration, startDate, endDate) {
        var daylightDiff = timeZoneUtils.getDaylightOffset(startDate, endDate);
        return this._needAdjustDuration(daylightDiff) ? this._calculateDurationByDaylightDiff(duration, daylightDiff) : duration
    }
    _needAdjustDuration(diff) {
        return 0 !== diff
    }
    _calculateDurationByDaylightDiff(duration, diff) {
        return duration + diff * toMs("minute")
    }
    _getCollectorLeftOffset(isAllDay) {
        if (isAllDay || !this.isApplyCompactAppointmentOffset()) {
            return 0
        }
        var dropDownButtonWidth = this.getDropDownAppointmentWidth(this.intervalCount, isAllDay);
        var rightOffset = this._isCompactTheme() ? COMPACT_THEME_WEEK_VIEW_COLLECTOR_OFFSET : WEEK_VIEW_COLLECTOR_OFFSET;
        return this.cellWidth - dropDownButtonWidth - rightOffset
    }
    _markAppointmentAsVirtual(coordinates) {
        var isAllDay = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
        var countFullWidthAppointmentInCell = this._getMaxAppointmentCountPerCellByType(isAllDay);
        if (coordinates.count - countFullWidthAppointmentInCell > 0) {
            var {
                top: top,
                left: left
            } = coordinates;
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
    }
    isApplyCompactAppointmentOffset() {
        return this.supportCompactDropDownAppointments()
    }
    supportCompactDropDownAppointments() {
        return true
    }
    _generateAppointmentCollectorIndex(_ref, isAllDay) {
        var {
            groupIndex: groupIndex,
            rowIndex: rowIndex,
            columnIndex: columnIndex
        } = _ref;
        return "".concat(groupIndex, "-").concat(rowIndex, "-").concat(columnIndex, "-").concat(isAllDay)
    }
    _getMaxAppointmentCountPerCellByType(isAllDay) {
        var appointmentCountPerCell = this._getMaxAppointmentCountPerCell();
        if (isObject(appointmentCountPerCell)) {
            return isAllDay ? appointmentCountPerCell.allDay : appointmentCountPerCell.simple
        } else {
            return appointmentCountPerCell
        }
    }
    getDropDownAppointmentWidth(intervalCount, isAllDay) {
        return this.getPositioningStrategy().getDropDownAppointmentWidth(intervalCount, isAllDay)
    }
    getDropDownAppointmentHeight() {
        return this.getPositioningStrategy().getDropDownAppointmentHeight()
    }
    getDropDownButtonAdaptiveSize() {
        return DROP_DOWN_BUTTON_ADAPTIVE_SIZE
    }
    getCollectorTopOffset(allDay) {
        return this.getPositioningStrategy().getCollectorTopOffset(allDay)
    }
    getCollectorLeftOffset() {
        return this.getPositioningStrategy().getCollectorLeftOffset()
    }
    getAppointmentDataCalculator() {}
    _customizeCoordinates(coordinates, height, appointmentCountPerCell, topOffset, isAllDay) {
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
    }
    _isAppointmentEmpty(height, width) {
        return height < this._getAppointmentMinHeight() || width < this._getAppointmentMinWidth()
    }
    _calculateGeometryConfig(coordinates) {
        var overlappingMode = this.maxAppointmentsPerCell;
        var offsets = this._getOffsets();
        var appointmentDefaultOffset = this._getAppointmentDefaultOffset();
        var appointmentCountPerCell = this._getAppointmentCount(overlappingMode, coordinates);
        var ratio = this._getDefaultRatio(coordinates, appointmentCountPerCell);
        var maxHeight = this._getMaxHeight();
        if (!isNumeric(appointmentCountPerCell)) {
            appointmentCountPerCell = coordinates.count;
            ratio = (maxHeight - offsets.unlimited) / maxHeight
        }
        var topOffset = (1 - ratio) * maxHeight;
        if ("auto" === overlappingMode || isNumeric(overlappingMode)) {
            ratio = 1;
            maxHeight -= appointmentDefaultOffset;
            topOffset = appointmentDefaultOffset
        }
        return {
            height: ratio * maxHeight,
            appointmentCountPerCell: appointmentCountPerCell,
            offset: topOffset
        }
    }
    _getAppointmentCount() {}
    _getDefaultRatio() {}
    _getOffsets() {}
    _getMaxHeight() {}
    _needVerifyItemSize() {
        return false
    }
    _getMaxAppointmentCountPerCell() {
        if (!this._maxAppointmentCountPerCell) {
            var overlappingMode = this.maxAppointmentsPerCell;
            var appointmentCountPerCell;
            if (isNumeric(overlappingMode)) {
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
    }
    _getDynamicAppointmentCountPerCell() {
        return this.getPositioningStrategy().getDynamicAppointmentCountPerCell()
    }
    allDaySupported() {
        return false
    }
    _isCompactTheme() {
        return "compact" === (currentTheme() || "").split(".").pop()
    }
    _getAppointmentDefaultOffset() {
        return this.getPositioningStrategy().getAppointmentDefaultOffset()
    }
    _getAppointmentDefaultHeight() {
        return this._getAppointmentHeightByTheme()
    }
    _getAppointmentMinHeight() {
        return this._getAppointmentDefaultHeight()
    }
    _getAppointmentHeightByTheme() {
        return this._isCompactTheme() ? COMPACT_THEME_APPOINTMENT_DEFAULT_HEIGHT : APPOINTMENT_DEFAULT_HEIGHT
    }
    _getAppointmentDefaultWidth() {
        return this.getPositioningStrategy()._getAppointmentDefaultWidth()
    }
    _getAppointmentMinWidth() {
        return this._getAppointmentDefaultWidth()
    }
    _needVerticalGroupBounds() {
        return false
    }
    _needHorizontalGroupBounds() {
        return false
    }
    getAppointmentDurationInMs(startDate, endDate, allDay) {
        var appointmentDuration = endDate.getTime() - startDate.getTime();
        var dayDuration = toMs("day");
        var visibleDayDuration = this.visibleDayDuration;
        var result = 0;
        if (allDay) {
            var ceilQuantityOfDays = Math.ceil(appointmentDuration / dayDuration);
            result = ceilQuantityOfDays * visibleDayDuration
        } else {
            var isDifferentDates = !timeZoneUtils.isSameAppointmentDates(startDate, endDate);
            var floorQuantityOfDays = Math.floor(appointmentDuration / dayDuration);
            var tailDuration;
            if (isDifferentDates) {
                var startDateEndHour = new Date(new Date(startDate).setHours(this.endDayHour, 0, 0));
                var hiddenDayDuration = dayDuration - visibleDayDuration - (startDate.getTime() > startDateEndHour.getTime() ? startDate.getTime() - startDateEndHour.getTime() : 0);
                tailDuration = appointmentDuration - (floorQuantityOfDays ? floorQuantityOfDays * dayDuration : hiddenDayDuration);
                var startDayTime = this.startDayHour * toMs("hour");
                var endPartDuration = endDate - dateUtils.trimTime(endDate);
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
    }
    getPositionShift(timeShift, isAllDay) {
        return {
            top: timeShift * this.cellHeight,
            left: 0,
            cellPosition: 0
        }
    }
}
export default BaseRenderingStrategy;
