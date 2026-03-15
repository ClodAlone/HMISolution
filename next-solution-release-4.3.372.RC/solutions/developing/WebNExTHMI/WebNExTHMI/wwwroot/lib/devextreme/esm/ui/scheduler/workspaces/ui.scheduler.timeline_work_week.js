/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.timeline_work_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import {
    VIEWS
} from "../constants";
import SchedulerTimelineWeek from "./ui.scheduler.timeline_week";
import {
    getWeekendsCount
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/work_week";
var TIMELINE_CLASS = "dx-scheduler-timeline-work-week";
var LAST_DAY_WEEK_INDEX = 5;
class SchedulerTimelineWorkWeek extends SchedulerTimelineWeek {
    get type() {
        return VIEWS.TIMELINE_WORK_WEEK
    }
    constructor() {
        super(...arguments);
        this._getWeekendsCount = getWeekendsCount
    }
    _getElementClass() {
        return TIMELINE_CLASS
    }
    _incrementDate(date) {
        var day = date.getDay();
        if (day === LAST_DAY_WEEK_INDEX) {
            date.setDate(date.getDate() + 2)
        }
        super._incrementDate(date)
    }
}
registerComponent("dxSchedulerTimelineWorkWeek", SchedulerTimelineWorkWeek);
export default SchedulerTimelineWorkWeek;
