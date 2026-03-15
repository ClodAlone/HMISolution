/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.timeline_month.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import registerComponent from "../../../core/component_registrator";
import SchedulerTimeline from "./ui.scheduler.timeline";
import dateUtils from "../../../core/utils/date";
import dxrDateHeader from "../../../renovation/ui/scheduler/workspaces/base/header_panel/layout.j";
import {
    getViewStartByOptions
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/month";
import {
    formatWeekdayAndDay
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    VIEWS
} from "../constants";
var TIMELINE_CLASS = "dx-scheduler-timeline-month";
class SchedulerTimelineMonth extends SchedulerTimeline {
    get type() {
        return VIEWS.TIMELINE_MONTH
    }
    get viewDirection() {
        return "horizontal"
    }
    get renovatedHeaderPanelComponent() {
        return dxrDateHeader
    }
    _renderView() {
        super._renderView();
        this._updateScrollable()
    }
    _getElementClass() {
        return TIMELINE_CLASS
    }
    _getDateHeaderTemplate() {
        return this.option("dateCellTemplate")
    }
    _calculateDurationInCells(timeDiff) {
        return timeDiff / this.getCellDuration()
    }
    isIndicatorVisible() {
        return true
    }
    _getFormat() {
        return formatWeekdayAndDay
    }
    _getIntervalBetween(currentDate) {
        var firstViewDate = this.getStartViewDate();
        var timeZoneOffset = dateUtils.getTimezonesDifference(firstViewDate, currentDate);
        return currentDate.getTime() - (firstViewDate.getTime() - 36e5 * this.option("startDayHour")) - timeZoneOffset
    }
    _getViewStartByOptions() {
        return getViewStartByOptions(this.option("startDate"), this.option("currentDate"), this.option("intervalCount"), dateUtils.getFirstMonthDate(this.option("startDate")))
    }
    generateRenderOptions() {
        var options = super.generateRenderOptions(true);
        return _extends({}, options, {
            getDateForHeaderText: (_, date) => date
        })
    }
}
registerComponent("dxSchedulerTimelineMonth", SchedulerTimelineMonth);
export default SchedulerTimelineMonth;
