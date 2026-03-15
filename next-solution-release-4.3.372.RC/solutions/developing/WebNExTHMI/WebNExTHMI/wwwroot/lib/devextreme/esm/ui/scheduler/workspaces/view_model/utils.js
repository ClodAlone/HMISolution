/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    VIEWS
} from "../../constants";
import {
    ViewDataGenerator
} from "./view_data_generator";
import {
    ViewDataGeneratorDay
} from "./view_data_generator_day";
import {
    ViewDataGeneratorMonth
} from "./view_data_generator_month";
import {
    ViewDataGeneratorTimelineMonth
} from "./view_data_generator_timeline_month";
import {
    ViewDataGeneratorWeek
} from "./view_data_generator_week";
import {
    ViewDataGeneratorWorkWeek
} from "./view_data_generator_work_week";
export var getViewDataGeneratorByViewType = viewType => {
    switch (viewType) {
        case VIEWS.MONTH:
            return new ViewDataGeneratorMonth;
        case VIEWS.TIMELINE_MONTH:
            return new ViewDataGeneratorTimelineMonth;
        case VIEWS.DAY:
        case VIEWS.TIMELINE_DAY:
            return new ViewDataGeneratorDay;
        case VIEWS.WEEK:
        case VIEWS.TIMELINE_WEEK:
            return new ViewDataGeneratorWeek;
        case VIEWS.WORK_WEEK:
        case VIEWS.TIMELINE_WORK_WEEK:
            return new ViewDataGeneratorWorkWeek;
        default:
            return new ViewDataGenerator
    }
};
