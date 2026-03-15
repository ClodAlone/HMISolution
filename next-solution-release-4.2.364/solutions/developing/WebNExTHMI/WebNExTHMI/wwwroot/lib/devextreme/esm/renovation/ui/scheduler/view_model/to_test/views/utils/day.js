/**
 * DevExtreme (esm/renovation/ui/scheduler/view_model/to_test/views/utils/day.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getViewStartByOptions,
    setOptionHour
} from "./base";
export var calculateStartViewDate = (currentDate, startDayHour, startDate, intervalDuration) => {
    var firstViewDate = getViewStartByOptions(startDate, currentDate, intervalDuration, startDate);
    return setOptionHour(firstViewDate, startDayHour)
};
