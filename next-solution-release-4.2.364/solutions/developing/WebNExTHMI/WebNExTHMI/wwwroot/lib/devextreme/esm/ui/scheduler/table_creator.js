/**
 * DevExtreme (esm/ui/scheduler/table_creator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import {
    data as elementData
} from "../../core/element_data";
import {
    isFunction
} from "../../core/utils/type";
import {
    getPublicElement
} from "../../core/element";
var ROW_SELECTOR = "tr";
class SchedulerTableCreator {
    constructor() {
        this.VERTICAL = "vertical";
        this.HORIZONTAL = "horizontal"
    }
    insertAllDayRow(allDayElements, tableBody, index) {
        if (allDayElements[index]) {
            var row = allDayElements[index].find(ROW_SELECTOR);
            if (!row.length) {
                row = $(domAdapter.createElement(ROW_SELECTOR));
                row.append(allDayElements[index].get(0))
            }
            tableBody.appendChild(row.get ? row.get(0) : row)
        }
    }
    makeTable(options) {
        var tableBody = domAdapter.createElement("tbody");
        var templateCallbacks = [];
        var row;
        var rowCountInGroup = options.groupCount ? options.rowCount / options.groupCount : options.rowCount;
        var allDayElementIndex = 0;
        var allDayElements = options.allDayElements;
        var groupIndex = options.groupIndex;
        var rowCount = options.rowCount;
        $(options.container).append(tableBody);
        if (allDayElements) {
            this.insertAllDayRow(allDayElements, tableBody, 0);
            allDayElementIndex++
        }
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
            row = domAdapter.createElement(ROW_SELECTOR);
            tableBody.appendChild(row);
            var isLastRowInGroup = (rowIndex + 1) % rowCountInGroup === 0;
            if (options.rowClass) {
                row.className = options.rowClass
            }
            for (var columnIndex = 0; columnIndex < options.cellCount; columnIndex++) {
                var _options$setAdditiona;
                var td = domAdapter.createElement("td");
                row.appendChild(td);
                if (options.cellClass) {
                    if (isFunction(options.cellClass)) {
                        td.className = options.cellClass(rowIndex, columnIndex)
                    } else {
                        td.className = options.cellClass
                    }
                }
                var cellDataObject = void 0;
                var dataKey = void 0;
                var dataValue = void 0;
                if (options.getCellData) {
                    cellDataObject = options.getCellData(td, rowIndex, columnIndex, groupIndex);
                    dataKey = cellDataObject.key;
                    dataValue = cellDataObject.value;
                    dataKey && elementData(td, dataKey, dataValue)
                }
                null === (_options$setAdditiona = options.setAdditionalClasses) || void 0 === _options$setAdditiona ? void 0 : _options$setAdditiona.call(options, $(td), dataValue);
                if (options.cellTemplate && options.cellTemplate.render) {
                    var additionalTemplateData = options.getTemplateData ? options.getTemplateData(rowIndex) : {};
                    var templateOptions = {
                        model: _extends({
                            text: options.getCellText ? options.getCellText(rowIndex, columnIndex) : "",
                            date: options.getCellDate ? options.getCellDate(rowIndex) : void 0
                        }, additionalTemplateData),
                        container: getPublicElement($(td)),
                        index: rowIndex * options.cellCount + columnIndex
                    };
                    if (dataValue) {
                        if (dataValue.startDate) {
                            templateOptions.model.startDate = dataValue.startDate
                        }
                        if (dataValue.endDate) {
                            templateOptions.model.endDate = dataValue.endDate
                        }
                        if (dataValue.groups) {
                            templateOptions.model.groups = dataValue.groups
                        }
                        if (dataValue.allDay) {
                            templateOptions.model.allDay = dataValue.allDay
                        }
                    }
                    templateCallbacks.push(options.cellTemplate.render.bind(options.cellTemplate, templateOptions))
                } else if (options.getCellText) {
                    $("<div>").text(options.getCellText(rowIndex, columnIndex)).addClass(options.getCellTextClass).appendTo($(td))
                }
            }
            if (allDayElements && isLastRowInGroup) {
                this.insertAllDayRow(allDayElements, tableBody, allDayElementIndex);
                allDayElementIndex++
            }
        }
        return templateCallbacks
    }
    makeGroupedTable(type, groups, cssClasses, cellCount, cellTemplate, rowCount, groupByDate) {
        var rows = [];
        if (type === this.VERTICAL) {
            rows = this._makeVerticalGroupedRows(groups, cssClasses, cellTemplate, rowCount)
        } else {
            rows = this._makeHorizontalGroupedRows(groups, cssClasses, cellCount, cellTemplate, groupByDate)
        }
        return rows
    }
    makeGroupedTableFromJSON(type, data, config) {
        var table;
        var cellStorage = [];
        var rowIndex = 0;
        config = config || {};
        var cellTag = config.cellTag || "td";
        var childrenField = config.childrenField || "children";
        var titleField = config.titleField || "title";
        var groupTableClass = config.groupTableClass;
        var groupRowClass = config.groupRowClass;
        var groupCellClass = config.groupCellClass;
        var groupCellCustomContent = config.groupCellCustomContent;

        function getChildCount(item) {
            if (item[childrenField]) {
                return item[childrenField].length
            }
            return 0
        }

        function createCell(text, childCount, index, data) {
            var cell = {
                element: domAdapter.createElement(cellTag),
                childCount: childCount
            };
            if (groupCellClass) {
                cell.element.className = groupCellClass
            }
            var cellText = domAdapter.createTextNode(text);
            if ("function" === typeof groupCellCustomContent) {
                groupCellCustomContent(cell.element, cellText, index, data)
            } else {
                cell.element.appendChild(cellText)
            }
            return cell
        }! function() {
            table = domAdapter.createElement("table");
            if (groupTableClass) {
                table.className = groupTableClass
            }
        }();
        ! function generateCells(data) {
            for (var i = 0; i < data.length; i++) {
                var childCount = getChildCount(data[i]);
                var cell = createCell(data[i][titleField], childCount, i, data[i]);
                if (!cellStorage[rowIndex]) {
                    cellStorage[rowIndex] = []
                }
                cellStorage[rowIndex].push(cell);
                if (childCount) {
                    generateCells(data[i][childrenField])
                } else {
                    rowIndex++
                }
            }
        }(data);
        void cellStorage.forEach((function(cells) {
            var row = domAdapter.createElement(ROW_SELECTOR);
            if (groupRowClass) {
                row.className = groupRowClass
            }
            var rowspans = [];
            for (var i = cells.length - 1; i >= 0; i--) {
                var prev = cells[i + 1];
                var rowspan = cells[i].childCount;
                if (prev && prev.childCount) {
                    rowspan *= prev.childCount
                }
                rowspans.push(rowspan)
            }
            rowspans.reverse();
            cells.forEach((function(cell, index) {
                if (rowspans[index]) {
                    cell.element.setAttribute("rowSpan", rowspans[index])
                }
                row.appendChild(cell.element)
            }));
            table.appendChild(row)
        }));
        return table
    }
    _makeFlexGroupedRowCells(group, repeatCount, cssClasses, cellTemplate) {
        var repeatByDate = arguments.length > 4 && void 0 !== arguments[4] ? arguments[4] : 1;
        var cells = [];
        var items = group.items;
        var itemCount = items.length;
        for (var i = 0; i < repeatCount * repeatByDate; i++) {
            for (var j = 0; j < itemCount; j++) {
                var $container = $("<div>");
                var cell = {};
                if (cellTemplate && cellTemplate.render) {
                    var templateOptions = {
                        model: items[j],
                        container: getPublicElement($container),
                        index: i * itemCount + j
                    };
                    if (group.data) {
                        templateOptions.model.data = group.data[j]
                    }
                    cell.template = cellTemplate.render.bind(cellTemplate, templateOptions)
                } else {
                    $container.text(items[j].text).attr("title", items[j].text).addClass("dx-scheduler-group-header-content");
                    $container = $("<div>").append($container)
                }
                var cssClass = isFunction(cssClasses.groupHeaderClass) ? cssClasses.groupHeaderClass(j) : cssClasses.groupHeaderClass;
                cell.element = $container.addClass(cssClass);
                cells.push(cell)
            }
        }
        return cells
    }
    _makeVerticalGroupedRows(groups, cssClasses, cellTemplate) {
        var cellTemplates = [];
        var repeatCount = 1;
        var cellsArray = [];
        var cellIterator = function(cell) {
            if (cell.template) {
                cellTemplates.push(cell.template)
            }
        };
        for (var i = 0; i < groups.length; i++) {
            if (i > 0) {
                repeatCount = groups[i - 1].items.length * repeatCount
            }
            var cells = this._makeFlexGroupedRowCells(groups[i], repeatCount, cssClasses, cellTemplate);
            cells.forEach(cellIterator);
            cellsArray.push(cells)
        }
        var rows = [];
        var groupCount = cellsArray.length;
        for (var _i = 0; _i < groupCount; _i++) {
            rows.push($("<div>").addClass(cssClasses.groupHeaderRowClass))
        }
        for (var _i2 = groupCount - 1; _i2 >= 0; _i2--) {
            var currentColumnLength = cellsArray[_i2].length;
            for (var j = 0; j < currentColumnLength; j++) {
                rows[_i2].append(cellsArray[_i2][j].element)
            }
        }
        return {
            elements: $("<div>").addClass("dx-scheduler-group-flex-container").append(rows),
            cellTemplates: cellTemplates
        }
    }
    _makeHorizontalGroupedRows(groups, cssClasses, cellCount, cellTemplate, groupByDate) {
        var repeatCount = 1;
        var groupCount = groups.length;
        var rows = [];
        var cellTemplates = [];
        var repeatByDate = groupByDate ? cellCount : 1;
        var cellIterator = function(cell) {
            if (cell.template) {
                cellTemplates.push(cell.template)
            }
            return cell.element
        };
        for (var i = 0; i < groupCount; i++) {
            if (i > 0) {
                repeatCount = groups[i - 1].items.length * repeatCount
            }
            var cells = this._makeGroupedRowCells(groups[i], repeatCount, cssClasses, cellTemplate, repeatByDate);
            rows.push($("<tr>").addClass(cssClasses.groupRowClass).append(cells.map(cellIterator)))
        }
        var maxCellCount = rows[groupCount - 1].find("th").length;
        for (var j = 0; j < groupCount; j++) {
            var $cell = rows[j].find("th");
            var colspan = maxCellCount / $cell.length;
            if (!groupByDate) {
                colspan *= cellCount
            }
            if (colspan > 1 && 1 === repeatByDate || groupByDate && groupCount > 1) {
                $cell.attr("colSpan", colspan)
            }
        }
        return {
            elements: rows,
            cellTemplates: cellTemplates
        }
    }
    _makeGroupedRowCells(group, repeatCount, cssClasses, cellTemplate, repeatByDate) {
        repeatByDate = repeatByDate || 1;
        repeatCount *= repeatByDate;
        var cells = [];
        var items = group.items;
        var itemCount = items.length;
        for (var i = 0; i < repeatCount; i++) {
            for (var j = 0; j < itemCount; j++) {
                var $container = $("<div>");
                var cell = {};
                if (cellTemplate && cellTemplate.render) {
                    var templateOptions = {
                        model: items[j],
                        container: getPublicElement($container),
                        index: i * itemCount + j
                    };
                    if (group.data) {
                        templateOptions.model.data = group.data[j]
                    }
                    cell.template = cellTemplate.render.bind(cellTemplate, templateOptions)
                } else {
                    $container.text(items[j].text);
                    $container = $("<div>").append($container)
                }
                $container.addClass(cssClasses.groupHeaderContentClass);
                var cssClass = void 0;
                if (isFunction(cssClasses.groupHeaderClass)) {
                    cssClass = cssClasses.groupHeaderClass(j)
                } else {
                    cssClass = cssClasses.groupHeaderClass
                }
                cell.element = $("<th>").addClass(cssClass).append($container);
                cells.push(cell)
            }
        }
        return cells
    }
}
export default {
    tableCreator: new SchedulerTableCreator
};
