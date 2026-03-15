/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.work_space_work_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import {
    getWeekendsCount
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/work_week";
import SchedulerWorkSpaceWeek from "./ui.scheduler.work_space_week";
import {
    VIEWS
} from "../constants";
var WORK_WEEK_CLASS = "dx-scheduler-work-space-work-week";
class SchedulerWorkSpaceWorkWeek extends SchedulerWorkSpaceWeek {
    get type() {
        return VIEWS.WORK_WEEK
    }
    constructor() {
        super(...arguments);
        this._getWeekendsCount = getWeekendsCount
    }
    _getElementClass() {
        return WORK_WEEK_CLASS
    }
}
registerComponent("dxSchedulerWorkSpaceWorkWeek", SchedulerWorkSpaceWorkWeek);
export default SchedulerWorkSpaceWorkWeek;
