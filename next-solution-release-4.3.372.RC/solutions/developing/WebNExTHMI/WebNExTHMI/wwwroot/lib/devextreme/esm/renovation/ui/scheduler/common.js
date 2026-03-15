/**
 * DevExtreme (esm/renovation/ui/scheduler/common.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    utils
} from "../../../ui/scheduler/utils";
import {
    createExpressions
} from "../../../ui/scheduler/resources/utils";
export var createDataAccessors = function(props) {
    var forceIsoDateParsing = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
    var dataAccessors = utils.dataAccessors.create({
        startDate: props.startDateExpr,
        endDate: props.endDateExpr,
        startDateTimeZone: props.startDateTimeZoneExpr,
        endDateTimeZone: props.endDateTimeZoneExpr,
        allDay: props.allDayExpr,
        text: props.textExpr,
        description: props.descriptionExpr,
        recurrenceRule: props.recurrenceRuleExpr,
        recurrenceException: props.recurrenceExceptionExpr
    }, null, forceIsoDateParsing, props.dateSerializationFormat);
    dataAccessors.resources = createExpressions(props.resources);
    return dataAccessors
};
export var isViewDataProviderConfigValid = (viewDataProviderConfig, currentViewOptions) => {
    if (!viewDataProviderConfig) {
        return false
    }
    var result = true;
    Object.entries(viewDataProviderConfig).forEach(_ref => {
        var [key, value] = _ref;
        if (value !== currentViewOptions[key]) {
            result = false
        }
    });
    return result
};
