/**
 * DevExtreme (esm/ui/pivot_grid/remote_store.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isString,
    isDefined
} from "../../core/utils/type";
import Class from "../../core/class";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import {
    DataSource
} from "../../data/data_source/data_source";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    getFiltersByPath,
    capitalizeFirstLetter,
    getExpandedLevel,
    discoverObjectFields,
    setDefaultFieldValueFormatting
} from "./ui.pivot_grid.utils";
import {
    forEachGroup
} from "./remote_store.utils";
import dateSerialization from "../../core/utils/date_serialization";
import {
    normalizeLoadResult
} from "../../data/data_source/utils";

function createGroupingOptions(dimensionOptions, useSortOrder) {
    var groupingOptions = [];
    each(dimensionOptions, (function(index, dimensionOption) {
        groupingOptions.push({
            selector: dimensionOption.dataField,
            groupInterval: dimensionOption.groupInterval,
            desc: useSortOrder && "desc" === dimensionOption.sortOrder,
            isExpanded: index < dimensionOptions.length - 1
        })
    }));
    return groupingOptions
}

function getFieldFilterSelector(field) {
    var selector = field.dataField;
    var groupInterval = field.groupInterval;
    if ("date" === field.dataType && "string" === typeof groupInterval) {
        if ("quarter" === groupInterval.toLowerCase()) {
            groupInterval = "Month"
        }
        selector = selector + "." + capitalizeFirstLetter(groupInterval)
    }
    return selector
}

function getIntervalFilterExpression(selector, numericInterval, numericValue, isExcludedFilterType) {
    var startFilterValue = [selector, isExcludedFilterType ? "<" : ">=", numericValue];
    var endFilterValue = [selector, isExcludedFilterType ? ">=" : "<", numericValue + numericInterval];
    return [startFilterValue, isExcludedFilterType ? "or" : "and", endFilterValue]
}

function getFilterExpressionForFilterValue(field, filterValue) {
    var selector = getFieldFilterSelector(field);
    var isExcludedFilterType = "exclude" === field.filterType;
    var expression = [selector, isExcludedFilterType ? "<>" : "=", filterValue];
    if (isDefined(field.groupInterval)) {
        if ("string" === typeof field.groupInterval && "quarter" === field.groupInterval.toLowerCase()) {
            expression = getIntervalFilterExpression(selector, 3, 3 * (filterValue - 1) + 1, isExcludedFilterType)
        } else if ("number" === typeof field.groupInterval && "date" !== field.dataType) {
            expression = getIntervalFilterExpression(selector, field.groupInterval, filterValue, isExcludedFilterType)
        }
    }
    return expression
}

function createFieldFilterExpressions(field, operation) {
    var fieldFilterExpressions = [];
    if (field.searchValue) {
        return [field.dataField, "contains", field.searchValue]
    }
    if ("exclude" === field.filterType) {
        operation = operation || "and"
    } else {
        operation = operation || "or"
    }
    each(field.filterValues, (function(index, filterValue) {
        var currentExpression = [];
        if (Array.isArray(filterValue)) {
            var parseLevelsRecursive = field.levels && field.levels.length;
            if (parseLevelsRecursive) {
                currentExpression = createFieldFilterExpressions({
                    filterValues: filterValue,
                    filterType: field.filterType,
                    levels: field.levels
                }, "and")
            }
        } else {
            var currentField = field.levels ? field.levels[index] : field;
            currentExpression = getFilterExpressionForFilterValue(currentField, filterValue)
        }
        if (!currentExpression.length) {
            return
        }
        if (fieldFilterExpressions.length) {
            fieldFilterExpressions.push(operation)
        }
        fieldFilterExpressions.push(currentExpression)
    }));
    return fieldFilterExpressions
}

function createFilterExpressions(fields) {
    var filterExpressions = [];
    each(fields, (function(_, field) {
        var fieldExpressions = createFieldFilterExpressions(field);
        if (!fieldExpressions.length) {
            return []
        }
        if (filterExpressions.length) {
            filterExpressions.push("and")
        }
        filterExpressions.push(fieldExpressions)
    }));
    if (1 === filterExpressions.length) {
        filterExpressions = filterExpressions[0]
    }
    return filterExpressions
}

function mergeFilters(filter1, filter2) {
    var mergedFilter;
    var notEmpty = function(filter) {
        return filter && filter.length
    };
    if (notEmpty(filter1) && notEmpty(filter2)) {
        mergedFilter = [filter1, "and", filter2]
    } else {
        mergedFilter = notEmpty(filter1) ? filter1 : filter2
    }
    return mergedFilter
}

function createLoadOptions(options, externalFilterExpr, hasRows) {
    var filterExpressions = createFilterExpressions(options.filters);
    var groupingOptions = createGroupingOptions(options.rows, options.rowTake).concat(createGroupingOptions(options.columns, options.columnTake));
    var loadOptions = {
        groupSummary: [],
        totalSummary: [],
        group: groupingOptions.length ? groupingOptions : void 0,
        take: groupingOptions.length ? void 0 : 1
    };
    if (options.rows.length && options.rowTake) {
        loadOptions.skip = options.rowSkip;
        loadOptions.take = options.rowTake;
        loadOptions.requireGroupCount = true
    } else if (options.columns.length && options.columnTake && !hasRows) {
        loadOptions.skip = options.columnSkip;
        loadOptions.take = options.columnTake;
        loadOptions.requireGroupCount = true
    }
    if (externalFilterExpr) {
        filterExpressions = mergeFilters(filterExpressions, externalFilterExpr)
    }
    if (filterExpressions.length) {
        loadOptions.filter = filterExpressions
    }
    each(options.values, (function(_, value) {
        var summaryOption = {
            selector: value.dataField,
            summaryType: value.summaryType || "count"
        };
        loadOptions.groupSummary.push(summaryOption);
        options.includeTotalSummary && loadOptions.totalSummary.push(summaryOption)
    }));
    return loadOptions
}

function setValue(valuesArray, value, rowIndex, columnIndex, dataIndex) {
    valuesArray[rowIndex] = valuesArray[rowIndex] || [];
    valuesArray[rowIndex][columnIndex] = valuesArray[rowIndex][columnIndex] || [];
    if (!isDefined(valuesArray[rowIndex][columnIndex][dataIndex])) {
        valuesArray[rowIndex][columnIndex][dataIndex] = value
    }
}

function parseValue(value, field) {
    if (field && "number" === field.dataType && isString(value)) {
        return Number(value)
    }
    if (field && "date" === field.dataType && !field.groupInterval && !(value instanceof Date)) {
        return dateSerialization.deserializeDate(value)
    }
    return value
}

function parseResult(data, total, descriptions, result) {
    var rowPath = [];
    var columnPath = [];
    var rowHash = result.rowHash;
    var columnHash = result.columnHash;
    if (total && total.summary) {
        each(total.summary, (function(index, summary) {
            setValue(result.values, summary, result.grandTotalRowIndex, result.grandTotalColumnIndex, index)
        }))
    }
    if (total && total.groupCount >= 0) {
        var skip = descriptions.rows.length ? descriptions.rowSkip : descriptions.columnSkip;
        data = [...Array(skip)].concat(data);
        data.length = total.groupCount
    }

    function getItem(dataItem, dimensionName, path, level, field) {
        var dimensionHash = result[dimensionName + "Hash"];
        var parentItem;
        var parentItemChildren;
        var item;
        var pathValue = path.slice(0, level + 1).join("/");
        var parentPathValue;
        if (void 0 !== dimensionHash[pathValue]) {
            item = dimensionHash[pathValue]
        } else {
            item = {
                value: parseValue(dataItem.key, field),
                index: result[dimensionName + "Index"]++,
                displayText: dataItem.displayText
            };
            parentPathValue = path.slice(0, level).join("/");
            if (level > 0 && void 0 !== dimensionHash[parentPathValue]) {
                parentItem = dimensionHash[parentPathValue];
                parentItemChildren = parentItem.children = parentItem.children || []
            } else {
                parentItemChildren = result[dimensionName + "s"]
            }
            parentItemChildren.push(item);
            dimensionHash[pathValue] = item
        }
        return item
    }
    forEachGroup(data, (function(item, level) {
        var rowLevel = level >= descriptions.rows.length ? descriptions.rows.length : level;
        var columnLevel = level >= descriptions.rows.length ? level - descriptions.rows.length : 0;
        var columnItem;
        var rowItem;
        if (level >= descriptions.rows.length && columnLevel >= descriptions.columns.length) {
            return
        }
        if (level < descriptions.rows.length) {
            columnPath = []
        }
        if (level >= descriptions.rows.length) {
            if (item) {
                columnPath[columnLevel] = item.key + "";
                columnItem = getItem(item, "column", columnPath, columnLevel, descriptions.columns[columnLevel]);
                rowItem = rowHash[rowPath.slice(0, rowLevel + 1).join("/")]
            } else {
                result.columns.push({})
            }
        } else if (item) {
            rowPath[rowLevel] = item.key + "";
            rowItem = getItem(item, "row", rowPath, rowLevel, descriptions.rows[rowLevel]);
            columnItem = columnHash[columnPath.slice(0, columnLevel + 1).join("/")]
        } else {
            result.rows.push({})
        }
        var currentRowIndex = rowItem && rowItem.index || result.grandTotalRowIndex;
        var currentColumnIndex = columnItem && columnItem.index || result.grandTotalColumnIndex;
        each(item && item.summary || [], (function(i, summary) {
            setValue(result.values, summary, currentRowIndex, currentColumnIndex, i)
        }))
    }));
    return result
}

function getFiltersForDimension(fields) {
    return (fields || []).filter(f => f.filterValues && f.filterValues.length || f.searchValue)
}

function getExpandedIndex(options, axis) {
    if (options.headerName) {
        if (axis === options.headerName) {
            return options.path.length
        } else if (options.oppositePath) {
            return options.oppositePath.length
        }
    }
    return 0
}

function getFiltersForExpandedDimension(options) {
    return getFiltersByPath(options[options.headerName], options.path).concat(getFiltersByPath(options["rows" === options.headerName ? "columns" : "rows"], options.oppositePath || []))
}

function getExpandedPathSliceFilter(options, dimensionName, level, firstCollapsedFieldIndex) {
    var result = [];
    var startSliceIndex = level > firstCollapsedFieldIndex ? 0 : firstCollapsedFieldIndex;
    var fields = options.headerName !== dimensionName ? options[dimensionName].slice(startSliceIndex, level) : [];
    var paths = "rows" === dimensionName ? options.rowExpandedPaths : options.columnExpandedPaths;
    each(fields, (function(index, field) {
        var filterValues = [];
        each(paths, (function(_, path) {
            path = path.slice(startSliceIndex, level);
            if (index < path.length) {
                var filterValue = path[index];
                if (-1 === filterValues.indexOf(filterValue)) {
                    filterValues.push(filterValue)
                }
            }
        }));
        if (filterValues.length) {
            result.push(extend({}, field, {
                filterType: "include",
                filterValues: filterValues
            }))
        }
    }));
    return result
}

function getGrandTotalRequest(options, dimensionName, expandedIndex, expandedLevel, commonFilters, firstCollapsedFieldIndex) {
    var expandedPaths = ("columns" === dimensionName ? options.columnExpandedPaths : options.rowExpandedPaths) || [];
    var oppositeDimensionName = "columns" === dimensionName ? "rows" : "columns";
    var fields = options[dimensionName];
    var result = [];
    var newOptions;
    if (expandedPaths.length) {
        for (var i = expandedIndex; i < expandedLevel + 1; i++) {
            newOptions = {
                filters: commonFilters.concat(getExpandedPathSliceFilter(options, dimensionName, i, firstCollapsedFieldIndex))
            };
            newOptions[dimensionName] = fields.slice(expandedIndex, i + 1);
            newOptions[oppositeDimensionName] = [];
            result.push(extend({}, options, newOptions))
        }
    } else {
        newOptions = {
            filters: commonFilters
        };
        newOptions[dimensionName] = fields.slice(expandedIndex, expandedLevel + 1);
        newOptions[oppositeDimensionName] = [];
        result.push(extend({}, options, newOptions))
    }
    result[0].includeTotalSummary = true;
    return result
}

function getFirstCollapsedIndex(fields) {
    var firstCollapsedIndex = 0;
    each(fields, (function(index, field) {
        if (!field.expanded) {
            firstCollapsedIndex = index;
            return false
        }
    }));
    return firstCollapsedIndex
}

function getRequestsData(options) {
    var rowExpandedLevel = getExpandedLevel(options, "rows");
    var columnExpandedLevel = getExpandedLevel(options, "columns");
    var filters = options.filters || [];
    var columnExpandedIndex = getExpandedIndex(options, "columns");
    var firstCollapsedColumnIndex = getFirstCollapsedIndex(options.columns);
    var firstCollapsedRowIndex = getFirstCollapsedIndex(options.rows);
    var rowExpandedIndex = getExpandedIndex(options, "rows");
    var data = [];
    filters = filters.concat(getFiltersForDimension(options.rows)).concat(getFiltersForDimension(options.columns)).concat(getFiltersForExpandedDimension(options));
    var columnTotalsOptions = getGrandTotalRequest(options, "columns", columnExpandedIndex, columnExpandedLevel, filters, firstCollapsedColumnIndex);
    if (options.rows.length && options.columns.length) {
        if ("rows" !== options.headerName) {
            data = data.concat(columnTotalsOptions)
        }
        for (var i = rowExpandedIndex; i < rowExpandedLevel + 1; i++) {
            var rows = options.rows.slice(rowExpandedIndex, i + 1);
            var rowFilterByExpandedPaths = getExpandedPathSliceFilter(options, "rows", i, firstCollapsedRowIndex);
            for (var j = columnExpandedIndex; j < columnExpandedLevel + 1; j++) {
                var preparedOptions = extend({}, options, {
                    columns: options.columns.slice(columnExpandedIndex, j + 1),
                    rows: rows,
                    filters: filters.concat(getExpandedPathSliceFilter(options, "columns", j, firstCollapsedColumnIndex)).concat(rowFilterByExpandedPaths)
                });
                data.push(preparedOptions)
            }
        }
    } else {
        data = options.columns.length ? columnTotalsOptions : getGrandTotalRequest(options, "rows", rowExpandedIndex, rowExpandedLevel, filters, firstCollapsedRowIndex)
    }
    return data
}

function prepareFields(fields) {
    each(fields || [], (function(_, field) {
        var levels = field.levels;
        if (levels) {
            prepareFields(levels)
        }
        setDefaultFieldValueFormatting(field)
    }))
}
export default Class.inherit({
    ctor: function(options) {
        this._dataSource = new DataSource(options);
        this._store = this._dataSource.store()
    },
    getFields: function(fields) {
        var d = new Deferred;
        this._store.load({
            skip: 0,
            take: 20
        }).done((function(data) {
            var normalizedArguments = normalizeLoadResult(data);
            d.resolve(discoverObjectFields(normalizedArguments.data, fields))
        })).fail(d.reject);
        return d
    },
    key: function() {
        return this._store.key()
    },
    load: function(options) {
        var that = this;
        var d = new Deferred;
        var result = {
            rows: [],
            columns: [],
            values: [],
            grandTotalRowIndex: 0,
            grandTotalColumnIndex: 0,
            rowHash: {},
            columnHash: {},
            rowIndex: 1,
            columnIndex: 1
        };
        var requestsData = getRequestsData(options);
        var deferreds = [];
        prepareFields(options.rows);
        prepareFields(options.columns);
        prepareFields(options.filters);
        each(requestsData, (function(_, dataItem) {
            deferreds.push(that._store.load(createLoadOptions(dataItem, that.filter(), options.rows.length)))
        }));
        when.apply(null, deferreds).done((function() {
            var args = deferreds.length > 1 ? arguments : [arguments];
            each(args, (function(index, argument) {
                var normalizedArguments = normalizeLoadResult(argument[0], argument[1]);
                parseResult(normalizedArguments.data, normalizedArguments.extra, requestsData[index], result)
            }));
            d.resolve({
                rows: result.rows,
                columns: result.columns,
                values: result.values,
                grandTotalRowIndex: result.grandTotalRowIndex,
                grandTotalColumnIndex: result.grandTotalColumnIndex
            })
        })).fail(d.reject);
        return d
    },
    filter: function() {
        return this._dataSource.filter.apply(this._dataSource, arguments)
    },
    supportPaging: function() {
        return false
    },
    createDrillDownDataSource: function(loadOptions, params) {
        loadOptions = loadOptions || {};
        params = params || {};
        var store = this._store;
        var filters = getFiltersByPath(loadOptions.rows, params.rowPath).concat(getFiltersByPath(loadOptions.columns, params.columnPath)).concat(getFiltersForDimension(loadOptions.rows)).concat(loadOptions.filters || []).concat(getFiltersForDimension(loadOptions.columns));
        var filterExp = createFilterExpressions(filters);
        return new DataSource({
            load: function(loadOptions) {
                return store.load(extend({}, loadOptions, {
                    filter: mergeFilters(filterExp, loadOptions.filter),
                    select: params.customColumns
                }))
            }
        })
    }
});
