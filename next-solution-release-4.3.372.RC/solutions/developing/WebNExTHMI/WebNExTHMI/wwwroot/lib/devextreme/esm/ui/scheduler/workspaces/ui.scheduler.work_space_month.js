/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.work_space_month.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    noop
} from "../../../core/utils/common";
import registerComponent from "../../../core/component_registrator";
import SchedulerWorkSpace from "./ui.scheduler.work_space.indicator";
import dateUtils from "../../../core/utils/date";
import {
    getBoundingRect
} from "../../../core/utils/position";
import {
    utils
} from "../utils";
import {
    hasWindow
} from "../../../core/utils/window";
import dxrMonthDateTableLayout from "../../../renovation/ui/scheduler/workspaces/month/date_table/layout.j";
import {
    getViewStartByOptions,
    getCellText
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/month";
import {
    formatWeekday
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    VIEWS
} from "../constants";
var MONTH_CLASS = "dx-scheduler-work-space-month";
var DATE_TABLE_CURRENT_DATE_CLASS = "dx-scheduler-date-table-current-date";
var DATE_TABLE_CELL_TEXT_CLASS = "dx-scheduler-date-table-cell-text";
var DATE_TABLE_FIRST_OF_MONTH_CLASS = "dx-scheduler-date-table-first-of-month";
var DATE_TABLE_OTHER_MONTH_DATE_CLASS = "dx-scheduler-date-table-other-month";
var toMs = dateUtils.dateToMilliseconds;
class SchedulerWorkSpaceMonth extends SchedulerWorkSpace {
    get type() {
        return VIEWS.MONTH
    }
    _getElementClass() {
        return MONTH_CLASS
    }
    _getFormat() {
        return formatWeekday
    }
    _getIntervalBetween(currentDate) {
        var firstViewDate = this.getStartViewDate();
        var timeZoneOffset = dateUtils.getTimezonesDifference(firstViewDate, currentDate);
        return currentDate.getTime() - (firstViewDate.getTime() - 36e5 * this.option("startDayHour")) - timeZoneOffset
    }
    _getDateGenerationOptions() {
        return _extends({}, super._getDateGenerationOptions(), {
            cellCountInDay: 1
        })
    }
    getCellWidth() {
        return this.cache.get("cellWidth", () => {
            var averageWidth = 0;
            var cells = this._getCells().slice(0, 7);
            cells.each((index, element) => {
                averageWidth += hasWindow() ? getBoundingRect(element).width : 0
            });
            return 0 === cells.length ? void 0 : averageWidth / 7
        })
    }
    _insertAllDayRowsIntoDateTable() {
        return false
    }
    _getCellCoordinatesByIndex(index) {
        var rowIndex = Math.floor(index / this._getCellCount());
        var columnIndex = index - this._getCellCount() * rowIndex;
        return {
            rowIndex: rowIndex,
            columnIndex: columnIndex
        }
    }
    _needCreateCrossScrolling() {
        return this.option("crossScrollingEnabled") || this._isVerticalGroupedWorkSpace()
    }
    _getViewStartByOptions() {
        return getViewStartByOptions(this.option("startDate"), this.option("currentDate"), this.option("intervalCount"), dateUtils.getFirstMonthDate(this.option("startDate")))
    }
    _updateIndex(index) {
        return index
    }
    isIndicationAvailable() {
        return false
    }
    getIntervalDuration() {
        return toMs("day")
    }
    getTimePanelWidth() {
        return 0
    }
    supportAllDayRow() {
        return false
    }
    keepOriginalHours() {
        return true
    }
    getWorkSpaceLeftOffset() {
        return 0
    }
    needApplyCollectorOffset() {
        return true
    }
    _getHeaderDate() {
        return this._getViewStartByOptions()
    }
    scrollToTime() {
        return noop()
    }
    renderRAllDayPanel() {}
    renderRTimeTable() {}
    renderRDateTable() {
        utils.renovation.renderComponent(this, this._$dateTable, dxrMonthDateTableLayout, "renovatedDateTable", this._getRDateTableProps())
    }
    _createWorkSpaceElements() {
        if (this._isVerticalGroupedWorkSpace()) {
            this._createWorkSpaceScrollableElements()
        } else {
            super._createWorkSpaceElements()
        }
    }
    _toggleAllDayVisibility() {
        return noop()
    }
    _changeAllDayVisibility() {
        return noop()
    }
    _renderTimePanel() {
        return noop()
    }
    _renderAllDayPanel() {
        return noop()
    }
    _setMonthClassesToCell($cell, data) {
        $cell.toggleClass(DATE_TABLE_CURRENT_DATE_CLASS, data.isCurrentDate).toggleClass(DATE_TABLE_FIRST_OF_MONTH_CLASS, data.firstDayOfMonth).toggleClass(DATE_TABLE_OTHER_MONTH_DATE_CLASS, data.otherMonth)
    }
    _createAllDayPanelElements() {}
    _renderTableBody(options) {
        options.getCellText = (rowIndex, columnIndex) => {
            var date = this.viewDataProvider.completeViewDataMap[rowIndex][columnIndex].startDate;
            return getCellText(date, this.option("intervalCount"))
        };
        options.getCellTextClass = DATE_TABLE_CELL_TEXT_CLASS;
        options.setAdditionalClasses = this._setMonthClassesToCell.bind(this), super._renderTableBody(options)
    }
}
registerComponent("dxSchedulerWorkSpaceMonth", SchedulerWorkSpaceMonth);
export default SchedulerWorkSpaceMonth;
