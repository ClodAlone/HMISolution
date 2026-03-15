/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.timeline.js)
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
var _common = require("../../../core/utils/common");
var _extend = require("../../../core/utils/extend");
var _position = require("../../../core/utils/position");
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _uiSchedulerWork_space = _interopRequireDefault(require("./ui.scheduler.work_space.indicator"));
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _table_creator = _interopRequireDefault(require("../table_creator"));
var _uiSchedulerCurrent_time_shader = _interopRequireDefault(require("../shaders/ui.scheduler.current_time_shader.horizontal"));
var _classes = require("../classes");
var _timeline_week = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/timeline_week");
var _utils = _interopRequireDefault(require("../utils.timeZone"));
var _layout = _interopRequireDefault(require("../../../renovation/ui/scheduler/workspaces/timeline/header_panel/layout.j"));
var _base = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");

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
var tableCreator = _table_creator.default.tableCreator;
var TIMELINE_CLASS = "dx-scheduler-timeline";
var GROUP_TABLE_CLASS = "dx-scheduler-group-table";
var HORIZONTAL_GROUPED_WORKSPACE_CLASS = "dx-scheduler-work-space-horizontal-grouped";
var HEADER_PANEL_CELL_CLASS = "dx-scheduler-header-panel-cell";
var HEADER_PANEL_WEEK_CELL_CLASS = "dx-scheduler-header-panel-week-cell";
var HEADER_ROW_CLASS = "dx-scheduler-header-row";
var HORIZONTAL = "horizontal";
var DATE_TABLE_CELL_BORDER = 1;
var DATE_TABLE_HEADER_MARGIN = 10;
var toMs = _date.default.dateToMilliseconds;
var SchedulerTimeline = function(_SchedulerWorkSpace) {
    _inheritsLoose(SchedulerTimeline, _SchedulerWorkSpace);

    function SchedulerTimeline() {
        return _SchedulerWorkSpace.apply(this, arguments) || this
    }
    var _proto = SchedulerTimeline.prototype;
    _proto.getGroupTableWidth = function() {
        return this._$sidebarTable ? (0, _size.getOuterWidth)(this._$sidebarTable) : 0
    };
    _proto._getTotalRowCount = function(groupCount) {
        if (this._isHorizontalGroupedWorkSpace()) {
            return this._getRowCount()
        } else {
            groupCount = groupCount || 1;
            return this._getRowCount() * groupCount
        }
    };
    _proto._getFormat = function() {
        return "shorttime"
    };
    _proto._getWorkSpaceHeight = function() {
        if (this.option("crossScrollingEnabled")) {
            return (0, _position.getBoundingRect)(this._$dateTable.get(0)).height
        }
        return (0, _position.getBoundingRect)(this.$element().get(0)).height
    };
    _proto._dateTableScrollableConfig = function() {
        var config = _SchedulerWorkSpace.prototype._dateTableScrollableConfig.call(this);
        var timelineConfig = {
            direction: HORIZONTAL
        };
        return this.option("crossScrollingEnabled") ? config : (0, _extend.extend)(config, timelineConfig)
    };
    _proto._needCreateCrossScrolling = function() {
        return true
    };
    _proto._headerScrollableConfig = function() {
        var config = _SchedulerWorkSpace.prototype._headerScrollableConfig.call(this);
        return (0, _extend.extend)(config, {
            scrollByContent: true
        })
    };
    _proto.supportAllDayRow = function() {
        return false
    };
    _proto._getGroupHeaderContainer = function() {
        if (this._isHorizontalGroupedWorkSpace()) {
            return this._$thead
        }
        return this._$sidebarTable
    };
    _proto._insertAllDayRowsIntoDateTable = function() {
        return false
    };
    _proto._needRenderWeekHeader = function() {
        return false
    };
    _proto._incrementDate = function(date) {
        date.setDate(date.getDate() + 1)
    };
    _proto.getIndicationCellCount = function() {
        var timeDiff = this._getTimeDiff();
        return this._calculateDurationInCells(timeDiff)
    };
    _proto._getTimeDiff = function() {
        var today = this._getToday();
        var date = this._getIndicationFirstViewDate();
        var startViewDate = this.getStartViewDate();
        var dayLightOffset = _utils.default.getDaylightOffsetInMs(startViewDate, today);
        if (dayLightOffset) {
            today = new Date(today.getTime() + dayLightOffset)
        }
        return today.getTime() - date.getTime()
    };
    _proto._calculateDurationInCells = function(timeDiff) {
        var today = this._getToday();
        var differenceInDays = Math.floor(timeDiff / toMs("day"));
        var duration = (timeDiff - differenceInDays * toMs("day") - this.option("startDayHour") * toMs("hour")) / this.getCellDuration();
        if (today.getHours() > this.option("endDayHour")) {
            duration = this._getCellCountInDay()
        }
        if (duration < 0) {
            duration = 0
        }
        return differenceInDays * this._getCellCountInDay() + duration
    };
    _proto.getIndicationWidth = function() {
        if (this.isGroupedByDate()) {
            var cellCount = this.getIndicationCellCount();
            var integerPart = Math.floor(cellCount);
            var fractionPart = cellCount - integerPart;
            return this.getCellWidth() * (integerPart * this._getGroupCount() + fractionPart)
        } else {
            return this.getIndicationCellCount() * this.getCellWidth()
        }
    };
    _proto._isVerticalShader = function() {
        return false
    };
    _proto._isCurrentTimeHeaderCell = function() {
        return false
    };
    _proto._setTableSizes = function() {
        var minHeight = this._getWorkSpaceMinHeight();
        (0, _size.setHeight)(this._$sidebarTable, minHeight);
        (0, _size.setHeight)(this._$dateTable, minHeight);
        _SchedulerWorkSpace.prototype._setTableSizes.call(this);
        this.virtualScrollingDispatcher.updateDimensions()
    };
    _proto._getWorkSpaceMinHeight = function() {
        var minHeight = this._getWorkSpaceHeight();
        var workspaceContainerHeight = (0, _size.getOuterHeight)(this.$element(), true) - this.getHeaderPanelHeight() - 2 * DATE_TABLE_CELL_BORDER - DATE_TABLE_HEADER_MARGIN;
        if (minHeight < workspaceContainerHeight) {
            minHeight = workspaceContainerHeight
        }
        return minHeight
    };
    _proto._getCellCoordinatesByIndex = function(index) {
        return {
            columnIndex: index % this._getCellCount(),
            rowIndex: 0
        }
    };
    _proto._getCellByCoordinates = function(cellCoordinates, groupIndex) {
        var indexes = this._groupedStrategy.prepareCellIndexes(cellCoordinates, groupIndex);
        return this._$dateTable.find("tr").eq(indexes.rowIndex).find("td").eq(indexes.columnIndex)
    };
    _proto._getWorkSpaceWidth = function() {
        return (0, _size.getOuterWidth)(this._$dateTable, true)
    };
    _proto._getIndicationFirstViewDate = function() {
        return _date.default.trimTime(new Date(this.getStartViewDate()))
    };
    _proto._getIntervalBetween = function(currentDate, allDay) {
        var startDayHour = this.option("startDayHour");
        var endDayHour = this.option("endDayHour");
        var firstViewDate = this.getStartViewDate();
        var firstViewDateTime = firstViewDate.getTime();
        var hiddenInterval = (24 - endDayHour + startDayHour) * toMs("hour");
        var timeZoneOffset = _date.default.getTimezonesDifference(firstViewDate, currentDate);
        var apptStart = currentDate.getTime();
        var fullInterval = apptStart - firstViewDateTime - timeZoneOffset;
        var fullDays = Math.floor(fullInterval / toMs("day"));
        var tailDuration = fullInterval - fullDays * toMs("day");
        var tailDelta = 0;
        var cellCount = this._getCellCountInDay() * (fullDays - this._getWeekendsCount(fullDays));
        var gapBeforeAppt = apptStart - _date.default.trimTime(new Date(currentDate)).getTime();
        var result = cellCount * this.option("hoursInterval") * toMs("hour");
        if (!allDay) {
            if (currentDate.getHours() < startDayHour) {
                tailDelta = tailDuration - hiddenInterval + gapBeforeAppt
            } else if (currentDate.getHours() >= startDayHour && currentDate.getHours() < endDayHour) {
                tailDelta = tailDuration
            } else if (currentDate.getHours() >= startDayHour && currentDate.getHours() >= endDayHour) {
                tailDelta = tailDuration - (gapBeforeAppt - endDayHour * toMs("hour"))
            } else if (!fullDays) {
                result = fullInterval
            }
            result += tailDelta
        }
        return result
    };
    _proto._getWeekendsCount = function() {
        return 0
    };
    _proto.getAllDayContainer = function() {
        return null
    };
    _proto.getTimePanelWidth = function() {
        return 0
    };
    _proto.getIntervalDuration = function(allDay) {
        return this.getCellDuration()
    };
    _proto.getCellMinWidth = function() {
        return 0
    };
    _proto.getWorkSpaceLeftOffset = function() {
        return 0
    };
    _proto.scrollToTime = function(hours, minutes, date) {
        var coordinates = this._getScrollCoordinates(hours, minutes, date);
        var scrollable = this.getScrollable();
        var offset = this.option("rtlEnabled") ? (0, _position.getBoundingRect)(this.getScrollableContainer().get(0)).width : 0;
        if (this.option("templatesRenderAsynchronously")) {
            setTimeout((function() {
                scrollable.scrollBy({
                    left: coordinates.left - scrollable.scrollLeft() - offset,
                    top: 0
                })
            }))
        } else {
            scrollable.scrollBy({
                left: coordinates.left - scrollable.scrollLeft() - offset,
                top: 0
            })
        }
    };
    _proto.renderRAllDayPanel = function() {};
    _proto.renderRTimeTable = function() {};
    _proto._renderGroupAllDayPanel = function() {};
    _proto.generateRenderOptions = function() {
        var options = _SchedulerWorkSpace.prototype.generateRenderOptions.call(this, true);
        return _extends({}, options, {
            isGenerateWeekDaysHeaderData: this._needRenderWeekHeader(),
            getDateForHeaderText: _timeline_week.getDateForHeaderText
        })
    };
    _proto._init = function() {
        _SchedulerWorkSpace.prototype._init.call(this);
        this.$element().addClass(TIMELINE_CLASS);
        this._$sidebarTable = (0, _renderer.default)("<div>").addClass(GROUP_TABLE_CLASS)
    };
    _proto._getDefaultGroupStrategy = function() {
        return "vertical"
    };
    _proto._toggleGroupingDirectionClass = function() {
        this.$element().toggleClass(HORIZONTAL_GROUPED_WORKSPACE_CLASS, this._isHorizontalGroupedWorkSpace())
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_SchedulerWorkSpace.prototype._getDefaultOptions.call(this), {
            groupOrientation: "vertical"
        })
    };
    _proto._createWorkSpaceElements = function() {
        this._createWorkSpaceScrollableElements()
    };
    _proto._toggleAllDayVisibility = function() {
        return (0, _common.noop)()
    };
    _proto._changeAllDayVisibility = function() {
        return (0, _common.noop)()
    };
    _proto._getDateHeaderTemplate = function() {
        return this.option("timeCellTemplate")
    };
    _proto._renderView = function() {
        var groupCellTemplates;
        if (!this.isRenovatedRender()) {
            groupCellTemplates = this._renderGroupHeader()
        }
        this.renderWorkSpace();
        this._shader = new _uiSchedulerCurrent_time_shader.default(this);
        this._$sidebarTable.appendTo(this._sidebarScrollable.$content());
        if (this.isRenovatedRender() && this._isVerticalGroupedWorkSpace()) {
            this.renderRGroupPanel()
        }
        this.updateHeaderEmptyCellWidth();
        this._applyCellTemplates(groupCellTemplates)
    };
    _proto._setHorizontalGroupHeaderCellsHeight = function() {
        return (0, _common.noop)()
    };
    _proto._setCurrentTimeCells = function() {
        var timePanelCells = this._getTimePanelCells();
        var currentTimeCellIndices = this._getCurrentTimePanelCellIndices();
        currentTimeCellIndices.forEach((function(timePanelCellIndex) {
            timePanelCells.eq(timePanelCellIndex).addClass(_classes.HEADER_CURRENT_TIME_CELL_CLASS)
        }))
    };
    _proto._cleanCurrentTimeCells = function() {
        this.$element().find(".".concat(_classes.HEADER_CURRENT_TIME_CELL_CLASS)).removeClass(_classes.HEADER_CURRENT_TIME_CELL_CLASS)
    };
    _proto._getTimePanelCells = function() {
        return this.$element().find(".".concat(HEADER_PANEL_CELL_CLASS, ":not(.").concat(HEADER_PANEL_WEEK_CELL_CLASS, ")"))
    };
    _proto._getCurrentTimePanelCellIndices = function() {
        var columnCountPerGroup = this._getCellCount();
        var today = this._getToday();
        var index = this.getCellIndexByDate(today);
        var _this$_getCellCoordin = this._getCellCoordinatesByIndex(index),
            currentTimeColumnIndex = _this$_getCellCoordin.columnIndex;
        if (void 0 === currentTimeColumnIndex) {
            return []
        }
        var horizontalGroupCount = this._isHorizontalGroupedWorkSpace() && !this.isGroupedByDate() ? this._getGroupCount() : 1;
        return _toConsumableArray(new Array(horizontalGroupCount)).map((function(_, groupIndex) {
            return columnCountPerGroup * groupIndex + currentTimeColumnIndex
        }))
    };
    _proto._renderTimePanel = function() {
        return (0, _common.noop)()
    };
    _proto._renderAllDayPanel = function() {
        return (0, _common.noop)()
    };
    _proto._createAllDayPanelElements = function() {
        return (0, _common.noop)()
    };
    _proto._renderDateHeader = function() {
        var $headerRow = _SchedulerWorkSpace.prototype._renderDateHeader.call(this);
        if (this._needRenderWeekHeader()) {
            var firstViewDate = new Date(this.getStartViewDate());
            var currentDate = new Date(firstViewDate);
            var $cells = [];
            var groupCount = this._getGroupCount();
            var cellCountInDay = this._getCellCountInDay();
            var colSpan = this.isGroupedByDate() ? cellCountInDay * groupCount : cellCountInDay;
            var cellTemplate = this.option("dateCellTemplate");
            var horizontalGroupCount = this._isHorizontalGroupedWorkSpace() && !this.isGroupedByDate() ? groupCount : 1;
            var cellsInGroup = this.viewDataProvider.viewDataGenerator.daysInInterval * this.option("intervalCount");
            var cellsCount = cellsInGroup * horizontalGroupCount;
            for (var templateIndex = 0; templateIndex < cellsCount; templateIndex++) {
                var $th = (0, _renderer.default)("<th>");
                var text = (0, _base.formatWeekdayAndDay)(currentDate);
                if (cellTemplate) {
                    var templateOptions = {
                        model: _extends({
                            text: text,
                            date: new Date(currentDate)
                        }, this._getGroupsForDateHeaderTemplate(templateIndex, colSpan)),
                        container: $th,
                        index: templateIndex
                    };
                    cellTemplate.render(templateOptions)
                } else {
                    $th.text(text)
                }
                $th.addClass(HEADER_PANEL_CELL_CLASS).addClass(HEADER_PANEL_WEEK_CELL_CLASS).attr("colSpan", colSpan);
                $cells.push($th);
                if (templateIndex % cellsInGroup === cellsInGroup - 1) {
                    currentDate = new Date(firstViewDate)
                } else {
                    this._incrementDate(currentDate)
                }
            }
            var $row = (0, _renderer.default)("<tr>").addClass(HEADER_ROW_CLASS).append($cells);
            $headerRow.before($row)
        }
    };
    _proto._renderIndicator = function(height, rtlOffset, $container, groupCount) {
        var $indicator;
        var width = this.getIndicationWidth();
        if ("vertical" === this.option("groupOrientation")) {
            $indicator = this._createIndicator($container);
            (0, _size.setHeight)($indicator, (0, _position.getBoundingRect)($container.get(0)).height);
            $indicator.css("left", rtlOffset ? rtlOffset - width : width)
        } else {
            for (var i = 0; i < groupCount; i++) {
                var offset = this.isGroupedByDate() ? i * this.getCellWidth() : this._getCellCount() * this.getCellWidth() * i;
                $indicator = this._createIndicator($container);
                (0, _size.setHeight)($indicator, (0, _position.getBoundingRect)($container.get(0)).height);
                $indicator.css("left", rtlOffset ? rtlOffset - width - offset : width + offset)
            }
        }
    };
    _proto._makeGroupRows = function(groups, groupByDate) {
        var tableCreatorStrategy = "vertical" === this.option("groupOrientation") ? tableCreator.VERTICAL : tableCreator.HORIZONTAL;
        return tableCreator.makeGroupedTable(tableCreatorStrategy, groups, {
            groupRowClass: _classes.GROUP_ROW_CLASS,
            groupHeaderRowClass: _classes.GROUP_ROW_CLASS,
            groupHeaderClass: this._getGroupHeaderClass.bind(this),
            groupHeaderContentClass: _classes.GROUP_HEADER_CONTENT_CLASS
        }, this._getCellCount() || 1, this.option("resourceCellTemplate"), this._getTotalRowCount(this._getGroupCount()), groupByDate)
    };
    _createClass(SchedulerTimeline, [{
        key: "verticalGroupTableClass",
        get: function() {
            return GROUP_TABLE_CLASS
        }
    }, {
        key: "viewDirection",
        get: function() {
            return "horizontal"
        }
    }, {
        key: "renovatedHeaderPanelComponent",
        get: function() {
            return _layout.default
        }
    }]);
    return SchedulerTimeline
}(_uiSchedulerWork_space.default);
(0, _component_registrator.default)("dxSchedulerTimeline", SchedulerTimeline);
var _default = SchedulerTimeline;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
