/**
 * DevExtreme (esm/ui/filter_builder/ui.filter_operations_dictionary.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var OPERATION_ICONS = {
    "=": "equal",
    "<>": "notequal",
    "<": "less",
    "<=": "lessorequal",
    ">": "greater",
    ">=": "greaterorequal",
    notcontains: "doesnotcontain",
    contains: "contains",
    startswith: "startswith",
    endswith: "endswith",
    isblank: "isblank",
    isnotblank: "isnotblank"
};
var OPERATION_NAME = {
    "=": "equal",
    "<>": "notEqual",
    "<": "lessThan",
    "<=": "lessThanOrEqual",
    ">": "greaterThan",
    ">=": "greaterThanOrEqual",
    startswith: "startsWith",
    contains: "contains",
    notcontains: "notContains",
    endswith: "endsWith",
    isblank: "isBlank",
    isnotblank: "isNotBlank",
    between: "between"
};
export default {
    getIconByFilterOperation: function(filterOperation) {
        return OPERATION_ICONS[filterOperation]
    },
    getNameByFilterOperation: function(filterOperation) {
        return OPERATION_NAME[filterOperation]
    }
};
