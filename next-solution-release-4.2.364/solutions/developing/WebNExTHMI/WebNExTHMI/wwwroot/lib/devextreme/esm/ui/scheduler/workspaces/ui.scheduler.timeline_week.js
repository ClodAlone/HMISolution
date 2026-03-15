/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.timeline_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import SchedulerTimeline from "./ui.scheduler.timeline";
import {
    getBoundingRect
} from "../../../core/utils/position";
import {
    VIEWS
} from "../constants";
var TIMELINE_CLASS = "dx-scheduler-timeline-week";
export default class SchedulerTimelineWeek extends SchedulerTimeline {
    get type() {
        return VIEWS.TIMELINE_WEEK
    }
    _getElementClass() {
        return TIMELINE_CLASS
    }
    _getHeaderPanelCellWidth($headerRow) {
        return getBoundingRect($headerRow.children().first().get(0)).width
    }
    _needRenderWeekHeader() {
        return true
    }
    _incrementDate(date) {
        date.setDate(date.getDate() + 1)
    }
}
registerComponent("dxSchedulerTimelineWeek", SchedulerTimelineWeek);
