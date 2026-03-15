/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.work_space.indicator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _uiScheduler = _interopRequireDefault(require("./ui.scheduler.work_space"));
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _extend = require("../../../core/utils/extend");
var _position = require("../../../core/utils/position");
var _window = require("../../../core/utils/window");
var _classes = require("../classes");
var _base = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");
var _utils = _interopRequireDefault(require("../utils.timeZone"));

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

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
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
var toMs = _date.default.dateToMilliseconds;
var SCHEDULER_DATE_TIME_INDICATOR_CLASS = "dx-scheduler-date-time-indicator";
var TIME_PANEL_CURRENT_TIME_CELL_CLASS = "dx-scheduler-time-panel-current-time-cell";
var SchedulerWorkSpaceIndicator = function(_SchedulerWorkSpace) {
    _inheritsLoose(SchedulerWorkSpaceIndicator, _SchedulerWorkSpace);

    function SchedulerWorkSpaceIndicator() {
        return _SchedulerWorkSpace.apply(this, arguments) || this
    }
    var _proto = SchedulerWorkSpaceIndicator.prototype;
    _proto._getToday = function() {
        return (0, _base.getToday)(this.option("indicatorTime"), this.timeZoneCalculator)
    };
    _proto.isIndicationOnView = function() {
        if (this.option("showCurrentTimeIndicator")) {
            var today = this._getToday();
            var endViewDate = _date.default.trimTime(this.getEndViewDate());
            return _date.default.dateInRange(today, this.getStartViewDate(), new Date(endViewDate.getTime() + toMs("day")))
        }
        return false
    };
    _proto.isIndicationAvailable = function() {
        if (!(0, _window.hasWindow)()) {
            return false
        }
        var today = this._getToday();
        return today >= _date.default.trimTime(new Date(this.getStartViewDate()))
    };
    _proto.isIndicatorVisible = function() {
        var today = this._getToday();
        var endViewDate = new Date(this.getEndViewDate().getTime() + toMs("minute") - 1);
        var firstViewDate = new Date(this.getStartViewDate());
        firstViewDate.setFullYear(today.getFullYear(), today.getMonth(), today.getDate());
        endViewDate.setFullYear(today.getFullYear(), today.getMonth(), today.getDate());
        return _date.default.dateInRange(today, firstViewDate, endViewDate)
    };
    _proto._renderDateTimeIndication = function() {
        if (this.isIndicationAvailable()) {
            if (this.option("shadeUntilCurrentTime")) {
                this._shader.render()
            }
            if (this.isIndicationOnView() && this.isIndicatorVisible()) {
                var groupCount = this._getGroupCount() || 1;
                var $container = this._dateTableScrollable.$content();
                var height = this.getIndicationHeight();
                var rtlOffset = this._getRtlOffset(this.getCellWidth());
                this._renderIndicator(height, rtlOffset, $container, groupCount);
                this._setCurrentTimeCells()
            }
        }
    };
    _proto._renderIndicator = function(height, rtlOffset, $container, groupCount) {
        var groupedByDate = this.isGroupedByDate();
        var repeatCount = groupedByDate ? 1 : groupCount;
        for (var i = 0; i < repeatCount; i++) {
            var $indicator = this._createIndicator($container);
            (0, _size.setWidth)($indicator, groupedByDate ? this.getCellWidth() * groupCount : this.getCellWidth());
            this._groupedStrategy.shiftIndicator($indicator, height, rtlOffset, i)
        }
    };
    _proto._createIndicator = function($container) {
        var $indicator = (0, _renderer.default)("<div>").addClass(SCHEDULER_DATE_TIME_INDICATOR_CLASS);
        $container.append($indicator);
        return $indicator
    };
    _proto._getRtlOffset = function(width) {
        return this.option("rtlEnabled") ? (0, _position.getBoundingRect)(this._dateTableScrollable.$content().get(0)).width - this.getTimePanelWidth() - width : 0
    };
    _proto._setIndicationUpdateInterval = function() {
        if (!this.option("showCurrentTimeIndicator") || 0 === this.option("indicatorUpdateInterval")) {
            return
        }
        this._clearIndicatorUpdateInterval();
        this._indicatorInterval = setInterval(function() {
            this._refreshDateTimeIndication()
        }.bind(this), this.option("indicatorUpdateInterval"))
    };
    _proto._clearIndicatorUpdateInterval = function() {
        if (this._indicatorInterval) {
            clearInterval(this._indicatorInterval);
            delete this._indicatorInterval
        }
    };
    _proto._isVerticalShader = function() {
        return true
    };
    _proto.getIndicationWidth = function(groupIndex) {
        var maxWidth = this.getCellWidth() * this._getCellCount();
        var difference = this._getIndicatorDuration();
        if (difference > this._getCellCount()) {
            difference = this._getCellCount()
        }
        var width = difference * this.getRoundedCellWidth(groupIndex, groupIndex * this._getCellCount(), difference);
        return maxWidth < width ? maxWidth : width
    };
    _proto.getIndicatorOffset = function(groupIndex) {
        var difference = this._getIndicatorDuration() - 1;
        var offset = difference * this.getRoundedCellWidth(groupIndex, groupIndex * this._getCellCount(), difference);
        return offset
    };
    _proto._getIndicatorDuration = function() {
        var today = this._getToday();
        var firstViewDate = new Date(this.getStartViewDate());
        var timeDiff = today.getTime() - firstViewDate.getTime();
        if ("workWeek" === this.option("type")) {
            timeDiff -= this._getWeekendsCount(Math.round(timeDiff / toMs("day"))) * toMs("day")
        }
        return Math.ceil((timeDiff + 1) / toMs("day"))
    };
    _proto.getIndicationHeight = function() {
        var today = _utils.default.getDateWithoutTimezoneChange(this._getToday());
        var cellHeight = this.getCellHeight();
        var date = new Date(this.getStartViewDate());
        if (this.isIndicationOnView()) {
            date.setFullYear(today.getFullYear(), today.getMonth(), today.getDate())
        }
        var duration = today.getTime() - date.getTime();
        var cellCount = duration / this.getCellDuration();
        return cellCount * cellHeight
    };
    _proto._dispose = function() {
        this._clearIndicatorUpdateInterval();
        _SchedulerWorkSpace.prototype._dispose.apply(this, arguments)
    };
    _proto._refreshDateTimeIndication = function() {
        var _this$_shader;
        this._cleanDateTimeIndicator();
        this._cleanCurrentTimeCells();
        null === (_this$_shader = this._shader) || void 0 === _this$_shader ? void 0 : _this$_shader.clean();
        this._renderDateTimeIndication()
    };
    _proto._setCurrentTimeCells = function() {
        var timePanelCells = this._getTimePanelCells();
        var currentTimeCellIndices = this._getCurrentTimePanelCellIndices();
        currentTimeCellIndices.forEach((function(timePanelCellIndex) {
            timePanelCells.eq(timePanelCellIndex).addClass(TIME_PANEL_CURRENT_TIME_CELL_CLASS)
        }))
    };
    _proto._isCurrentTimeHeaderCell = function(headerIndex) {
        if (this.isIndicationOnView()) {
            var completeDateHeaderMap = this.viewDataProvider.completeDateHeaderMap;
            var date = completeDateHeaderMap[completeDateHeaderMap.length - 1][headerIndex].startDate;
            return _date.default.sameDate(date, this._getToday())
        }
        return false
    };
    _proto._getHeaderPanelCellClass = function(i) {
        var cellClass = _SchedulerWorkSpace.prototype._getHeaderPanelCellClass.call(this, i);
        if (this._isCurrentTimeHeaderCell(i)) {
            return cellClass + " " + _classes.HEADER_CURRENT_TIME_CELL_CLASS
        }
        return cellClass
    };
    _proto._cleanView = function() {
        _SchedulerWorkSpace.prototype._cleanView.call(this);
        this._cleanDateTimeIndicator()
    };
    _proto._dimensionChanged = function() {
        _SchedulerWorkSpace.prototype._dimensionChanged.call(this);
        this._refreshDateTimeIndication()
    };
    _proto._cleanDateTimeIndicator = function() {
        this.$element().find("." + SCHEDULER_DATE_TIME_INDICATOR_CLASS).remove()
    };
    _proto._cleanCurrentTimeCells = function() {
        this.$element().find(".".concat(TIME_PANEL_CURRENT_TIME_CELL_CLASS)).removeClass(TIME_PANEL_CURRENT_TIME_CELL_CLASS)
    };
    _proto._cleanWorkSpace = function() {
        _SchedulerWorkSpace.prototype._cleanWorkSpace.call(this);
        this._renderDateTimeIndication();
        this._setIndicationUpdateInterval()
    };
    _proto._optionChanged = function(args) {
        switch (args.name) {
            case "showCurrentTimeIndicator":
            case "indicatorTime":
                this._cleanWorkSpace();
                break;
            case "indicatorUpdateInterval":
                this._setIndicationUpdateInterval();
                break;
            case "showAllDayPanel":
            case "allDayExpanded":
            case "crossScrollingEnabled":
                _SchedulerWorkSpace.prototype._optionChanged.call(this, args);
                this._refreshDateTimeIndication();
                break;
            case "shadeUntilCurrentTime":
                this._refreshDateTimeIndication();
                break;
            default:
                _SchedulerWorkSpace.prototype._optionChanged.call(this, args)
        }
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_SchedulerWorkSpace.prototype._getDefaultOptions.call(this), {
            showCurrentTimeIndicator: true,
            indicatorTime: new Date,
            indicatorUpdateInterval: 5 * toMs("minute"),
            shadeUntilCurrentTime: true
        })
    };
    _proto._getCurrentTimePanelCellIndices = function() {
        var rowCountPerGroup = this._getTimePanelRowCount();
        var today = this._getToday();
        var index = this.getCellIndexByDate(today);
        var _this$_getCellCoordin = this._getCellCoordinatesByIndex(index),
            currentTimeRowIndex = _this$_getCellCoordin.rowIndex;
        if (void 0 === currentTimeRowIndex) {
            return []
        }
        var cellIndices;
        if (0 === currentTimeRowIndex) {
            cellIndices = [currentTimeRowIndex]
        } else {
            cellIndices = currentTimeRowIndex % 2 === 0 ? [currentTimeRowIndex - 1, currentTimeRowIndex] : [currentTimeRowIndex, currentTimeRowIndex + 1]
        }
        var verticalGroupCount = this._isVerticalGroupedWorkSpace() ? this._getGroupCount() : 1;
        return _toConsumableArray(new Array(verticalGroupCount)).reduce((function(currentIndices, _, groupIndex) {
            return [].concat(_toConsumableArray(currentIndices), _toConsumableArray(cellIndices.map((function(cellIndex) {
                return rowCountPerGroup * groupIndex + cellIndex
            }))))
        }), [])
    };
    return SchedulerWorkSpaceIndicator
}(_uiScheduler.default);
(0, _component_registrator.default)("dxSchedulerWorkSpace", SchedulerWorkSpaceIndicator);
var _default = SchedulerWorkSpaceIndicator;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
