/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.timeline.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    getOuterWidth,
    setHeight,
    getOuterHeight
} from "../../../core/utils/size";
import $ from "../../../core/renderer";
import {
    noop
} from "../../../core/utils/common";
import {
    extend
} from "../../../core/utils/extend";
import {
    getBoundingRect
} from "../../../core/utils/position";
import registerComponent from "../../../core/component_registrator";
import SchedulerWorkSpace from "./ui.scheduler.work_space.indicator";
import dateUtils from "../../../core/utils/date";
import tableCreatorModule from "../table_creator";
var {
    tableCreator: tableCreator
} = tableCreatorModule;
import HorizontalShader from "../shaders/ui.scheduler.current_time_shader.horizontal";
import {
    HEADER_CURRENT_TIME_CELL_CLASS,
    GROUP_ROW_CLASS,
    GROUP_HEADER_CONTENT_CLASS
} from "../classes";
import {
    getDateForHeaderText
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/timeline_week";
import timezoneUtils from "../utils.timeZone";
import dxrTimelineDateHeader from "../../../renovation/ui/scheduler/workspaces/timeline/header_panel/layout.j";
import {
    formatWeekdayAndDay
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
var TIMELINE_CLASS = "dx-scheduler-timeline";
var GROUP_TABLE_CLASS = "dx-scheduler-group-table";
var HORIZONTAL_GROUPED_WORKSPACE_CLASS = "dx-scheduler-work-space-horizontal-grouped";
var HEADER_PANEL_CELL_CLASS = "dx-scheduler-header-panel-cell";
var HEADER_PANEL_WEEK_CELL_CLASS = "dx-scheduler-header-panel-week-cell";
var HEADER_ROW_CLASS = "dx-scheduler-header-row";
var HORIZONTAL = "horizontal";
var DATE_TABLE_CELL_BORDER = 1;
var DATE_TABLE_HEADER_MARGIN = 10;
var toMs = dateUtils.dateToMilliseconds;
class SchedulerTimeline extends SchedulerWorkSpace {
    get verticalGroupTableClass() {
        return GROUP_TABLE_CLASS
    }
    get viewDirection() {
        return "horizontal"
    }
    get renovatedHeaderPanelComponent() {
        return dxrTimelineDateHeader
    }
    getGroupTableWidth() {
        return this._$sidebarTable ? getOuterWidth(this._$sidebarTable) : 0
    }
    _getTotalRowCount(groupCount) {
        if (this._isHorizontalGroupedWorkSpace()) {
            return this._getRowCount()
        } else {
            groupCount = groupCount || 1;
            return this._getRowCount() * groupCount
        }
    }
    _getFormat() {
        return "shorttime"
    }
    _getWorkSpaceHeight() {
        if (this.option("crossScrollingEnabled")) {
            return getBoundingRect(this._$dateTable.get(0)).height
        }
        return getBoundingRect(this.$element().get(0)).height
    }
    _dateTableScrollableConfig() {
        var config = super._dateTableScrollableConfig();
        var timelineConfig = {
            direction: HORIZONTAL
        };
        return this.option("crossScrollingEnabled") ? config : extend(config, timelineConfig)
    }
    _needCreateCrossScrolling() {
        return true
    }
    _headerScrollableConfig() {
        var config = super._headerScrollableConfig();
        return extend(config, {
            scrollByContent: true
        })
    }
    supportAllDayRow() {
        return false
    }
    _getGroupHeaderContainer() {
        if (this._isHorizontalGroupedWorkSpace()) {
            return this._$thead
        }
        return this._$sidebarTable
    }
    _insertAllDayRowsIntoDateTable() {
        return false
    }
    _needRenderWeekHeader() {
        return false
    }
    _incrementDate(date) {
        date.setDate(date.getDate() + 1)
    }
    getIndicationCellCount() {
        var timeDiff = this._getTimeDiff();
        return this._calculateDurationInCells(timeDiff)
    }
    _getTimeDiff() {
        var today = this._getToday();
        var date = this._getIndicationFirstViewDate();
        var startViewDate = this.getStartViewDate();
        var dayLightOffset = timezoneUtils.getDaylightOffsetInMs(startViewDate, today);
        if (dayLightOffset) {
            today = new Date(today.getTime() + dayLightOffset)
        }
        return today.getTime() - date.getTime()
    }
    _calculateDurationInCells(timeDiff) {
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
    }
    getIndicationWidth() {
        if (this.isGroupedByDate()) {
            var cellCount = this.getIndicationCellCount();
            var integerPart = Math.floor(cellCount);
            var fractionPart = cellCount - integerPart;
            return this.getCellWidth() * (integerPart * this._getGroupCount() + fractionPart)
        } else {
            return this.getIndicationCellCount() * this.getCellWidth()
        }
    }
    _isVerticalShader() {
        return false
    }
    _isCurrentTimeHeaderCell() {
        return false
    }
    _setTableSizes() {
        var minHeight = this._getWorkSpaceMinHeight();
        setHeight(this._$sidebarTable, minHeight);
        setHeight(this._$dateTable, minHeight);
        super._setTableSizes();
        this.virtualScrollingDispatcher.updateDimensions()
    }
    _getWorkSpaceMinHeight() {
        var minHeight = this._getWorkSpaceHeight();
        var workspaceContainerHeight = getOuterHeight(this.$element(), true) - this.getHeaderPanelHeight() - 2 * DATE_TABLE_CELL_BORDER - DATE_TABLE_HEADER_MARGIN;
        if (minHeight < workspaceContainerHeight) {
            minHeight = workspaceContainerHeight
        }
        return minHeight
    }
    _getCellCoordinatesByIndex(index) {
        return {
            columnIndex: index % this._getCellCount(),
            rowIndex: 0
        }
    }
    _getCellByCoordinates(cellCoordinates, groupIndex) {
        var indexes = this._groupedStrategy.prepareCellIndexes(cellCoordinates, groupIndex);
        return this._$dateTable.find("tr").eq(indexes.rowIndex).find("td").eq(indexes.columnIndex)
    }
    _getWorkSpaceWidth() {
        return getOuterWidth(this._$dateTable, true)
    }
    _getIndicationFirstViewDate() {
        return dateUtils.trimTime(new Date(this.getStartViewDate()))
    }
    _getIntervalBetween(currentDate, allDay) {
        var startDayHour = this.option("startDayHour");
        var endDayHour = this.option("endDayHour");
        var firstViewDate = this.getStartViewDate();
        var firstViewDateTime = firstViewDate.getTime();
        var hiddenInterval = (24 - endDayHour + startDayHour) * toMs("hour");
        var timeZoneOffset = dateUtils.getTimezonesDifference(firstViewDate, currentDate);
        var apptStart = currentDate.getTime();
        var fullInterval = apptStart - firstViewDateTime - timeZoneOffset;
        var fullDays = Math.floor(fullInterval / toMs("day"));
        var tailDuration = fullInterval - fullDays * toMs("day");
        var tailDelta = 0;
        var cellCount = this._getCellCountInDay() * (fullDays - this._getWeekendsCount(fullDays));
        var gapBeforeAppt = apptStart - dateUtils.trimTime(new Date(currentDate)).getTime();
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
    }
    _getWeekendsCount() {
        return 0
    }
    getAllDayContainer() {
        return null
    }
    getTimePanelWidth() {
        return 0
    }
    getIntervalDuration(allDay) {
        return this.getCellDuration()
    }
    getCellMinWidth() {
        return 0
    }
    getWorkSpaceLeftOffset() {
        return 0
    }
    scrollToTime(hours, minutes, date) {
        var coordinates = this._getScrollCoordinates(hours, minutes, date);
        var scrollable = this.getScrollable();
        var offset = this.option("rtlEnabled") ? getBoundingRect(this.getScrollableContainer().get(0)).width : 0;
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
    }
    renderRAllDayPanel() {}
    renderRTimeTable() {}
    _renderGroupAllDayPanel() {}
    generateRenderOptions() {
        var options = super.generateRenderOptions(true);
        return _extends({}, options, {
            isGenerateWeekDaysHeaderData: this._needRenderWeekHeader(),
            getDateForHeaderText: getDateForHeaderText
        })
    }
    _init() {
        super._init();
        this.$element().addClass(TIMELINE_CLASS);
        this._$sidebarTable = $("<div>").addClass(GROUP_TABLE_CLASS)
    }
    _getDefaultGroupStrategy() {
        return "vertical"
    }
    _toggleGroupingDirectionClass() {
        this.$element().toggleClass(HORIZONTAL_GROUPED_WORKSPACE_CLASS, this._isHorizontalGroupedWorkSpace())
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            groupOrientation: "vertical"
        })
    }
    _createWorkSpaceElements() {
        this._createWorkSpaceScrollableElements()
    }
    _toggleAllDayVisibility() {
        return noop()
    }
    _changeAllDayVisibility() {
        return noop()
    }
    _getDateHeaderTemplate() {
        return this.option("timeCellTemplate")
    }
    _renderView() {
        var groupCellTemplates;
        if (!this.isRenovatedRender()) {
            groupCellTemplates = this._renderGroupHeader()
        }
        this.renderWorkSpace();
        this._shader = new HorizontalShader(this);
        this._$sidebarTable.appendTo(this._sidebarScrollable.$content());
        if (this.isRenovatedRender() && this._isVerticalGroupedWorkSpace()) {
            this.renderRGroupPanel()
        }
        this.updateHeaderEmptyCellWidth();
        this._applyCellTemplates(groupCellTemplates)
    }
    _setHorizontalGroupHeaderCellsHeight() {
        return noop()
    }
    _setCurrentTimeCells() {
        var timePanelCells = this._getTimePanelCells();
        var currentTimeCellIndices = this._getCurrentTimePanelCellIndices();
        currentTimeCellIndices.forEach(timePanelCellIndex => {
            timePanelCells.eq(timePanelCellIndex).addClass(HEADER_CURRENT_TIME_CELL_CLASS)
        })
    }
    _cleanCurrentTimeCells() {
        this.$element().find(".".concat(HEADER_CURRENT_TIME_CELL_CLASS)).removeClass(HEADER_CURRENT_TIME_CELL_CLASS)
    }
    _getTimePanelCells() {
        return this.$element().find(".".concat(HEADER_PANEL_CELL_CLASS, ":not(.").concat(HEADER_PANEL_WEEK_CELL_CLASS, ")"))
    }
    _getCurrentTimePanelCellIndices() {
        var columnCountPerGroup = this._getCellCount();
        var today = this._getToday();
        var index = this.getCellIndexByDate(today);
        var {
            columnIndex: currentTimeColumnIndex
        } = this._getCellCoordinatesByIndex(index);
        if (void 0 === currentTimeColumnIndex) {
            return []
        }
        var horizontalGroupCount = this._isHorizontalGroupedWorkSpace() && !this.isGroupedByDate() ? this._getGroupCount() : 1;
        return [...new Array(horizontalGroupCount)].map((_, groupIndex) => columnCountPerGroup * groupIndex + currentTimeColumnIndex)
    }
    _renderTimePanel() {
        return noop()
    }
    _renderAllDayPanel() {
        return noop()
    }
    _createAllDayPanelElements() {
        return noop()
    }
    _renderDateHeader() {
        var $headerRow = super._renderDateHeader();
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
                var $th = $("<th>");
                var text = formatWeekdayAndDay(currentDate);
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
            var $row = $("<tr>").addClass(HEADER_ROW_CLASS).append($cells);
            $headerRow.before($row)
        }
    }
    _renderIndicator(height, rtlOffset, $container, groupCount) {
        var $indicator;
        var width = this.getIndicationWidth();
        if ("vertical" === this.option("groupOrientation")) {
            $indicator = this._createIndicator($container);
            setHeight($indicator, getBoundingRect($container.get(0)).height);
            $indicator.css("left", rtlOffset ? rtlOffset - width : width)
        } else {
            for (var i = 0; i < groupCount; i++) {
                var offset = this.isGroupedByDate() ? i * this.getCellWidth() : this._getCellCount() * this.getCellWidth() * i;
                $indicator = this._createIndicator($container);
                setHeight($indicator, getBoundingRect($container.get(0)).height);
                $indicator.css("left", rtlOffset ? rtlOffset - width - offset : width + offset)
            }
        }
    }
    _makeGroupRows(groups, groupByDate) {
        var tableCreatorStrategy = "vertical" === this.option("groupOrientation") ? tableCreator.VERTICAL : tableCreator.HORIZONTAL;
        return tableCreator.makeGroupedTable(tableCreatorStrategy, groups, {
            groupRowClass: GROUP_ROW_CLASS,
            groupHeaderRowClass: GROUP_ROW_CLASS,
            groupHeaderClass: this._getGroupHeaderClass.bind(this),
            groupHeaderContentClass: GROUP_HEADER_CONTENT_CLASS
        }, this._getCellCount() || 1, this.option("resourceCellTemplate"), this._getTotalRowCount(this._getGroupCount()), groupByDate)
    }
}
registerComponent("dxSchedulerTimeline", SchedulerTimeline);
export default SchedulerTimeline;
