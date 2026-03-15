/**
 * DevExtreme (esm/ui/scheduler/expressionUtils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
export var ExpressionUtils = {
    getField: (dataAccessors, field, obj) => {
        if (!isDefined(dataAccessors.getter[field])) {
            return
        }
        return dataAccessors.getter[field](obj)
    },
    setField: (dataAccessors, field, obj, value) => {
        if (!isDefined(dataAccessors.setter[field])) {
            return
        }
        dataAccessors.setter[field](obj, value);
        return obj
    }
};
