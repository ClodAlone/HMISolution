/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.columns_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getWidth,
    getOuterHeight,
    getHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import {
    getWindow
} from "../../core/utils/window";
import eventsEngine from "../../events/core/events_engine";
import {
    data as elementData
} from "../../core/element_data";
import pointerEvents from "../../events/pointer";
import {
    name as clickEventName
} from "../../events/click";
import {
    name as dblclickEvent
} from "../../events/double_click";
import browser from "../../core/utils/browser";
import {
    noop
} from "../../core/utils/common";
import {
    setWidth
} from "../../core/utils/style";
import {
    getPublicElement
} from "../../core/element";
import {
    isRenderer,
    isFunction,
    isString,
    isDefined,
    isNumeric
} from "../../core/utils/type";
import {
    getBoundingRect,
    getDefaultAlignment
} from "../../core/utils/position";
import * as iteratorUtils from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import modules from "./ui.grid_core.modules";
import gridCoreUtils from "./ui.grid_core.utils";
import columnStateMixin from "./ui.grid_core.column_state_mixin";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    nativeScrolling
} from "../../core/utils/support";
var SCROLL_CONTAINER_CLASS = "scroll-container";
var SCROLLABLE_SIMULATED_CLASS = "scrollable-simulated";
var GROUP_SPACE_CLASS = "group-space";
var CONTENT_CLASS = "content";
var TABLE_CLASS = "table";
var TABLE_FIXED_CLASS = "table-fixed";
var CONTENT_FIXED_CLASS = "content-fixed";
var ROW_CLASS = "dx-row";
var GROUP_ROW_CLASS = "dx-group-row";
var DETAIL_ROW_CLASS = "dx-master-detail-row";
var FILTER_ROW_CLASS = "filter-row";
var ERROR_ROW_CLASS = "dx-error-row";
var CELL_UPDATED_ANIMATION_CLASS = "cell-updated-animation";
var HIDDEN_COLUMNS_WIDTH = "0.0001px";
var CELL_HINT_VISIBLE = "dxCellHintVisible";
var FORM_FIELD_ITEM_CONTENT_CLASS = "dx-field-item-content";
var appendElementTemplate = {
    render: function(options) {
        options.container.append(options.content)
    }
};
var subscribeToRowEvents = function(that, $table) {
    var touchTarget;
    var touchCurrentTarget;
    var timeoutId;

    function clearTouchTargets(timeout) {
        return setTimeout((function() {
            touchTarget = touchCurrentTarget = null
        }), timeout)
    }
    eventsEngine.on($table, "touchstart touchend", ".dx-row", (function(e) {
        clearTimeout(timeoutId);
        if ("touchstart" === e.type) {
            touchTarget = e.target;
            touchCurrentTarget = e.currentTarget;
            timeoutId = clearTouchTargets(1e3)
        } else {
            timeoutId = clearTouchTargets()
        }
    }));
    eventsEngine.on($table, [clickEventName, dblclickEvent, pointerEvents.down].join(" "), ".dx-row", {
        useNative: that._isNativeClick()
    }, that.createAction((function(e) {
        var event = e.event;
        if (touchTarget) {
            event.target = touchTarget;
            event.currentTarget = touchCurrentTarget
        }
        if (!$(event.target).closest("a").length) {
            e.rowIndex = that.getRowIndex(event.currentTarget);
            if (e.rowIndex >= 0) {
                e.rowElement = getPublicElement($(event.currentTarget));
                e.columns = that.getColumns();
                if (event.type === pointerEvents.down) {
                    that._rowPointerDown(e)
                } else if (event.type === clickEventName) {
                    that._rowClick(e)
                } else {
                    that._rowDblClick(e)
                }
            }
        }
    })))
};
var getWidthStyle = function(width) {
    if ("auto" === width) {
        return ""
    }
    return isNumeric(width) ? width + "px" : width
};
var setCellWidth = function(cell, column, width) {
    cell.style.width = cell.style.maxWidth = "auto" === column.width ? "" : width
};
var copyAttributes = function(element, newElement) {
    if (!element || !newElement) {
        return
    }
    var oldAttributes = element.attributes;
    var newAttributes = newElement.attributes;
    var i;
    for (i = 0; i < oldAttributes.length; i++) {
        var name = oldAttributes[i].nodeName;
        if (!newElement.hasAttribute(name)) {
            element.removeAttribute(name)
        }
    }
    for (i = 0; i < newAttributes.length; i++) {
        element.setAttribute(newAttributes[i].nodeName, newAttributes[i].nodeValue)
    }
};
export var ColumnsView = modules.View.inherit(columnStateMixin).inherit({
    _createScrollableOptions: function() {
        var scrollingOptions = this.option("scrolling");
        var useNativeScrolling = this.option("scrolling.useNative");
        var options = extend({}, scrollingOptions, {
            direction: "both",
            bounceEnabled: false,
            useKeyboard: false
        });
        if (void 0 === useNativeScrolling) {
            useNativeScrolling = true
        }
        if ("auto" === useNativeScrolling) {
            delete options.useNative;
            delete options.useSimulatedScrollbar
        } else {
            options.useNative = !!useNativeScrolling;
            options.useSimulatedScrollbar = !useNativeScrolling
        }
        return options
    },
    _updateCell: function($cell, parameters) {
        if (parameters.rowType) {
            this._cellPrepared($cell, parameters)
        }
    },
    _createCell: function(options) {
        var column = options.column;
        var alignment = column.alignment || getDefaultAlignment(this.option("rtlEnabled"));
        var cell = domAdapter.createElement("td");
        cell.style.textAlign = alignment;
        var $cell = $(cell);
        if ("data" === options.rowType && column.headerId && !column.type) {
            if (this.component.option("showColumnHeaders")) {
                this.setAria("describedby", column.headerId, $cell)
            }
        }
        if (column.cssClass) {
            $cell.addClass(column.cssClass)
        }
        if ("expand" === column.command) {
            $cell.addClass(column.cssClass);
            $cell.addClass(this.addWidgetPrefix(GROUP_SPACE_CLASS))
        }
        if (column.colspan > 1) {
            $cell.attr("colSpan", column.colspan)
        } else if (!column.isBand && "auto" !== column.visibleWidth && this.option("columnAutoWidth")) {
            if (column.width || column.minWidth) {
                cell.style.minWidth = getWidthStyle(column.minWidth || column.width)
            }
            if (column.width) {
                setCellWidth(cell, column, getWidthStyle(column.width))
            }
        }
        return $cell
    },
    _createRow: function(rowObject, tagName) {
        tagName = tagName || "tr";
        var $element = $("<".concat(tagName, ">")).addClass(ROW_CLASS);
        this.setAria("role", "row", $element);
        return $element
    },
    _isAltRow: function(row) {
        return row && row.dataIndex % 2 === 1
    },
    _createTable: function(columns, isAppend) {
        var that = this;
        var $table = $("<table>").addClass(that.addWidgetPrefix(TABLE_CLASS)).addClass(that.addWidgetPrefix(TABLE_FIXED_CLASS));
        if (columns && !isAppend) {
            $table.append(that._createColGroup(columns));
            if (browser.safari) {
                $table.append($("<thead>").append("<tr>"))
            }
            that.setAria("role", "presentation", $table)
        } else {
            that.setAria("hidden", true, $table)
        }
        this.setAria("role", "presentation", $("<tbody>").appendTo($table));
        if (isAppend) {
            return $table
        }
        if (browser.mozilla) {
            eventsEngine.on($table, "mousedown", "td", (function(e) {
                if (e.ctrlKey) {
                    e.preventDefault()
                }
            }))
        }
        if (that.option("cellHintEnabled")) {
            eventsEngine.on($table, "mousemove", ".dx-row > td", this.createAction((function(args) {
                var e = args.event;
                var $element = $(e.target);
                var $cell = $(e.currentTarget);
                var $row = $cell.parent();
                var isDataRow = $row.hasClass("dx-data-row");
                var isHeaderRow = $row.hasClass("dx-header-row");
                var isGroupRow = $row.hasClass(GROUP_ROW_CLASS);
                var isMasterDetailRow = $row.hasClass(DETAIL_ROW_CLASS);
                var isFilterRow = $row.hasClass(that.addWidgetPrefix(FILTER_ROW_CLASS));
                var visibleColumns = that._columnsController.getVisibleColumns();
                var rowOptions = $row.data("options");
                var columnIndex = $cell.index();
                var cellOptions = rowOptions && rowOptions.cells && rowOptions.cells[columnIndex];
                var column = cellOptions ? cellOptions.column : visibleColumns[columnIndex];
                if (!isMasterDetailRow && !isFilterRow && (!isDataRow || isDataRow && column && !column.cellTemplate) && (!isHeaderRow || isHeaderRow && column && !column.headerCellTemplate) && (!isGroupRow || isGroupRow && column && (void 0 === column.groupIndex || !column.groupCellTemplate))) {
                    if ($element.data(CELL_HINT_VISIBLE)) {
                        $element.removeAttr("title");
                        $element.data(CELL_HINT_VISIBLE, false)
                    }
                    var difference = $element[0].scrollWidth - $element[0].clientWidth;
                    if (difference > 0 && !isDefined($element.attr("title"))) {
                        $element.attr("title", $element.text());
                        $element.data(CELL_HINT_VISIBLE, true)
                    }
                }
            })))
        }
        var getOptions = function(event) {
            var $cell = $(event.currentTarget);
            var $fieldItemContent = $(event.target).closest("." + FORM_FIELD_ITEM_CONTENT_CLASS);
            var $row = $cell.parent();
            var rowOptions = $row.data("options");
            var options = rowOptions && rowOptions.cells && rowOptions.cells[$cell.index()];
            if (!$cell.closest("table").is(event.delegateTarget)) {
                return
            }
            var resultOptions = extend({}, options, {
                cellElement: getPublicElement($cell),
                event: event,
                eventType: event.type
            });
            resultOptions.rowIndex = that.getRowIndex($row);
            if ($fieldItemContent.length) {
                var formItemOptions = $fieldItemContent.data("dx-form-item");
                if (formItemOptions.column) {
                    resultOptions.column = formItemOptions.column;
                    resultOptions.columnIndex = that._columnsController.getVisibleIndex(resultOptions.column.index)
                }
            }
            return resultOptions
        };
        eventsEngine.on($table, "mouseover", ".dx-row > td", (function(e) {
            var options = getOptions(e);
            options && that.executeAction("onCellHoverChanged", options)
        }));
        eventsEngine.on($table, "mouseout", ".dx-row > td", (function(e) {
            var options = getOptions(e);
            options && that.executeAction("onCellHoverChanged", options)
        }));
        eventsEngine.on($table, clickEventName, ".dx-row > td", (function(e) {
            var options = getOptions(e);
            options && that.executeAction("onCellClick", options)
        }));
        eventsEngine.on($table, dblclickEvent, ".dx-row > td", (function(e) {
            var options = getOptions(e);
            options && that.executeAction("onCellDblClick", options)
        }));
        subscribeToRowEvents(that, $table);
        return $table
    },
    _isNativeClick: noop,
    _rowPointerDown: noop,
    _rowClick: noop,
    _rowDblClick: noop,
    _createColGroup: function(columns) {
        var colgroupElement = $("<colgroup>");
        for (var i = 0; i < columns.length; i++) {
            var colspan = columns[i].colspan || 1;
            for (var j = 0; j < colspan; j++) {
                colgroupElement.append(this._createCol(columns[i]))
            }
        }
        return colgroupElement
    },
    _createCol: function(column) {
        var width = column.visibleWidth || column.width;
        if ("adaptiveHidden" === width) {
            width = HIDDEN_COLUMNS_WIDTH
        }
        var col = $("<col>");
        setWidth(col, width);
        return col
    },
    renderDelayedTemplates: function(change) {
        var delayedTemplates = this._delayedTemplates;
        var syncTemplates = delayedTemplates.filter(template => !template.async);
        var asyncTemplates = delayedTemplates.filter(template => template.async);
        this._delayedTemplates = [];
        this._renderDelayedTemplatesCore(syncTemplates, false, change);
        this._renderDelayedTemplatesCoreAsync(asyncTemplates)
    },
    _renderDelayedTemplatesCoreAsync: function(templates) {
        var that = this;
        if (templates.length) {
            getWindow().setTimeout((function() {
                that._renderDelayedTemplatesCore(templates, true)
            }))
        }
    },
    _renderDelayedTemplatesCore: function(templates, isAsync, change) {
        var date = new Date;
        while (templates.length) {
            var templateParameters = templates.shift();
            var options = templateParameters.options;
            var doc = domAdapter.getDocument();
            if (!isAsync || $(options.container).closest(doc).length) {
                if (change) {
                    options.change = change
                }
                templateParameters.template.render(options)
            }
            if (isAsync && new Date - date > 30) {
                this._renderDelayedTemplatesCoreAsync(templates);
                break
            }
        }
        if (!templates.length && this._delayedTemplates.length) {
            this.renderDelayedTemplates()
        }
    },
    _processTemplate: function(template) {
        var renderingTemplate;
        if (template && template.render && !isRenderer(template)) {
            renderingTemplate = {
                allowRenderToDetachedContainer: template.allowRenderToDetachedContainer,
                render: function(options) {
                    template.render(options.container, options.model, options.change);
                    options.deferred && options.deferred.resolve()
                }
            }
        } else if (isFunction(template)) {
            renderingTemplate = {
                render: function(options) {
                    var renderedTemplate = template(getPublicElement(options.container), options.model, options.change);
                    if (renderedTemplate && (renderedTemplate.nodeType || isRenderer(renderedTemplate))) {
                        options.container.append(renderedTemplate)
                    }
                    options.deferred && options.deferred.resolve()
                }
            }
        } else {
            var templateID = isString(template) ? template : $(template).attr("id");
            if (!templateID) {
                renderingTemplate = this.getTemplate(template)
            } else {
                if (!this._templatesCache[templateID]) {
                    this._templatesCache[templateID] = this.getTemplate(template)
                }
                renderingTemplate = this._templatesCache[templateID]
            }
        }
        return renderingTemplate
    },
    renderTemplate: function(container, template, options, allowRenderToDetachedContainer, change) {
        var renderingTemplate = this._processTemplate(template, options);
        var column = options.column;
        var isDataRow = "data" === options.rowType;
        var templateDeferred = new Deferred;
        var templateOptions = {
            container: container,
            model: options,
            deferred: templateDeferred,
            onRendered: () => {
                templateDeferred.resolve()
            }
        };
        if (renderingTemplate) {
            options.component = this.component;
            var async = column && (column.renderAsync && isDataRow || this.option("renderAsync") && (false !== column.renderAsync && (column.command || column.showEditorAlways) && isDataRow || "filter" === options.rowType));
            if ((renderingTemplate.allowRenderToDetachedContainer || allowRenderToDetachedContainer) && !async) {
                renderingTemplate.render(templateOptions)
            } else {
                this._delayedTemplates.push({
                    template: renderingTemplate,
                    options: templateOptions,
                    async: async
                })
            }
            if (change) {
                change.templateDeferreds = change.templateDeferreds || [];
                change.templateDeferreds.push(templateDeferred)
            }
        } else {
            templateDeferred.reject()
        }
        return templateDeferred.promise()
    },
    _getBodies: function(tableElement) {
        return $(tableElement).children("tbody").not(".dx-header").not(".dx-footer")
    },
    _wrapRowIfNeed: function($table, $row) {
        var hasDataRowTemplate = this.option().rowTemplate || this.option("dataRowTemplate");
        var $tBodies = hasDataRowTemplate && this._getBodies(this._tableElement || $table);
        if ($tBodies && $tBodies.filter("." + ROW_CLASS).length) {
            var $tbody = $("<tbody>").addClass($row.attr("class"));
            this.setAria("role", "presentation", $tbody);
            return $tbody.append($row)
        }
        return $row
    },
    _appendRow: function($table, $row, appendTemplate) {
        appendTemplate = appendTemplate || appendElementTemplate;
        appendTemplate.render({
            content: $row,
            container: $table
        })
    },
    _resizeCore: function() {
        var scrollLeft = this._scrollLeft;
        if (scrollLeft >= 0) {
            this._scrollLeft = 0;
            this.scrollTo({
                left: scrollLeft
            })
        }
    },
    _renderCore: function(e) {
        var $root = this.element().parent();
        if (!$root || $root.parent().length) {
            this.renderDelayedTemplates(e)
        }
    },
    _renderTable: function(options) {
        options = options || {};
        options.columns = this._columnsController.getVisibleColumns();
        var changeType = options.change && options.change.changeType;
        var $table = this._createTable(options.columns, "append" === changeType || "prepend" === changeType || "update" === changeType);
        this._renderRows($table, options);
        return $table
    },
    _renderRows: function($table, options) {
        var rows = this._getRows(options.change);
        var columnIndices = options.change && options.change.columnIndices || [];
        var changeTypes = options.change && options.change.changeTypes || [];
        for (var i = 0; i < rows.length; i++) {
            this._renderRow($table, extend({
                row: rows[i],
                columnIndices: columnIndices[i],
                changeType: changeTypes[i]
            }, options))
        }
    },
    _renderRow: function($table, options) {
        if (!options.columnIndices) {
            options.row.cells = []
        }
        var $row = this._createRow(options.row);
        var $wrappedRow = this._wrapRowIfNeed($table, $row);
        if ("remove" !== options.changeType) {
            this._renderCells($row, options)
        }
        this._appendRow($table, $wrappedRow);
        var rowOptions = extend({
            columns: options.columns
        }, options.row);
        this._addWatchMethod(rowOptions, options.row);
        this._rowPrepared($wrappedRow, rowOptions, options.row)
    },
    _needRenderCell: function(columnIndex, columnIndices) {
        return !columnIndices || columnIndices.indexOf(columnIndex) >= 0
    },
    _renderCells: function($row, options) {
        var columnIndex = 0;
        var row = options.row;
        var columns = options.columns;
        for (var i = 0; i < columns.length; i++) {
            if (this._needRenderCell(i, options.columnIndices)) {
                this._renderCell($row, extend({
                    column: columns[i],
                    columnIndex: columnIndex,
                    value: row.values && row.values[columnIndex],
                    oldValue: row.oldValues && row.oldValues[columnIndex]
                }, options))
            }
            if (columns[i].colspan > 1) {
                columnIndex += columns[i].colspan
            } else {
                columnIndex++
            }
        }
    },
    _updateCells: function($rowElement, $newRowElement, columnIndices) {
        var $cells = $rowElement.children();
        var $newCells = $newRowElement.children();
        var highlightChanges = this.option("highlightChanges");
        var cellUpdatedClass = this.addWidgetPrefix(CELL_UPDATED_ANIMATION_CLASS);
        columnIndices.forEach((function(columnIndex, index) {
            var $cell = $cells.eq(columnIndex);
            var $newCell = $newCells.eq(index);
            $cell.replaceWith($newCell);
            if (highlightChanges && !$newCell.hasClass("dx-command-expand")) {
                $newCell.addClass(cellUpdatedClass)
            }
        }));
        copyAttributes($rowElement.get(0), $newRowElement.get(0))
    },
    _setCellAriaAttributes: function($cell, cellOptions) {
        if ("freeSpace" !== cellOptions.rowType) {
            this.setAria("selected", false, $cell);
            this.setAria("role", "gridcell", $cell);
            var columnIndexOffset = this._columnsController.getColumnIndexOffset();
            var ariaColIndex = cellOptions.columnIndex + columnIndexOffset + 1;
            this.setAria("colindex", ariaColIndex, $cell)
        }
    },
    _renderCell: function($row, options) {
        var cellOptions = this._getCellOptions(options);
        if (options.columnIndices) {
            if (options.row.cells) {
                options.row.cells[cellOptions.columnIndex] = cellOptions
            }
        } else {
            options.row.cells.push(cellOptions)
        }
        var $cell = this._createCell(cellOptions);
        this._setCellAriaAttributes($cell, cellOptions);
        this._renderCellContent($cell, cellOptions);
        $row.get(0).appendChild($cell.get(0));
        return $cell
    },
    _renderCellContent: function($cell, options) {
        var template = this._getCellTemplate(options);
        when(!template || this.renderTemplate($cell, template, options)).done(() => {
            this._updateCell($cell, options)
        })
    },
    _getCellTemplate: function() {},
    _getRows: function() {
        return []
    },
    _getCellOptions: function(options) {
        var cellOptions = {
            column: options.column,
            columnIndex: options.columnIndex,
            rowType: options.row.rowType,
            isAltRow: this._isAltRow(options.row)
        };
        this._addWatchMethod(cellOptions);
        return cellOptions
    },
    _addWatchMethod: function(options, source) {
        if (!this.option("repaintChangesOnly")) {
            return
        }
        var watchers = [];
        source = source || options;
        source.watch = source.watch || function(getter, updateValueFunc, updateRowFunc) {
            var oldValue = getter(source.data);
            var watcher = function(row) {
                if (row && updateRowFunc) {
                    updateRowFunc(row)
                }
                var newValue = getter(source.data);
                if (JSON.stringify(oldValue) !== JSON.stringify(newValue)) {
                    if (row) {
                        updateValueFunc(newValue)
                    }
                    oldValue = newValue
                }
            };
            watchers.push(watcher);
            return function() {
                var index = watchers.indexOf(watcher);
                if (index >= 0) {
                    watchers.splice(index, 1)
                }
            }
        };
        source.update = source.update || function(row, keepRow) {
            if (row) {
                this.data = options.data = row.data;
                this.rowIndex = options.rowIndex = row.rowIndex;
                this.dataIndex = options.dataIndex = row.dataIndex;
                this.isExpanded = options.isExpanded = row.isExpanded;
                if (options.row && !keepRow) {
                    options.row = row
                }
            }
            watchers.forEach((function(watcher) {
                watcher(row)
            }))
        };
        if (source !== options) {
            options.watch = source.watch.bind(source)
        }
        return options
    },
    _cellPrepared: function(cell, options) {
        options.cellElement = getPublicElement($(cell));
        this.executeAction("onCellPrepared", options)
    },
    _rowPrepared: function($row, options) {
        elementData($row.get(0), "options", options);
        options.rowElement = getPublicElement($row);
        this.executeAction("onRowPrepared", options)
    },
    _columnOptionChanged: function(e) {
        var optionNames = e.optionNames;
        if (gridCoreUtils.checkChanges(optionNames, ["width", "visibleWidth"])) {
            var visibleColumns = this._columnsController.getVisibleColumns();
            var widths = iteratorUtils.map(visibleColumns, (function(column) {
                var width = column.visibleWidth || column.width;
                return isDefined(width) ? width : "auto"
            }));
            this.setColumnWidths({
                widths: widths,
                optionNames: optionNames
            });
            return
        }
        if (!this._requireReady) {
            this.render()
        }
    },
    getCellIndex: function($cell) {
        var cellIndex = $cell.length ? $cell[0].cellIndex : -1;
        return cellIndex
    },
    getTableElements: function() {
        return this._tableElement || $()
    },
    getTableElement: function() {
        return this._tableElement
    },
    setTableElement: function(tableElement) {
        this._tableElement = tableElement
    },
    optionChanged: function(args) {
        this.callBase(args);
        switch (args.name) {
            case "cellHintEnabled":
            case "onCellPrepared":
            case "onRowPrepared":
            case "onCellHoverChanged":
                this._invalidate(true, true);
                args.handled = true
        }
    },
    init: function() {
        var that = this;
        that._scrollLeft = -1;
        that._columnsController = that.getController("columns");
        that._dataController = that.getController("data");
        that._delayedTemplates = [];
        that._templatesCache = {};
        that.createAction("onCellClick");
        that.createAction("onRowClick");
        that.createAction("onCellDblClick");
        that.createAction("onRowDblClick");
        that.createAction("onCellHoverChanged", {
            excludeValidators: ["disabled", "readOnly"]
        });
        that.createAction("onCellPrepared", {
            excludeValidators: ["disabled", "readOnly"],
            category: "rendering"
        });
        that.createAction("onRowPrepared", {
            excludeValidators: ["disabled", "readOnly"],
            category: "rendering",
            afterExecute: function(e) {
                that._afterRowPrepared(e)
            }
        });
        that._columnsController.columnsChanged.add(that._columnOptionChanged.bind(that));
        that._dataController && that._dataController.changed.add(that._handleDataChanged.bind(that))
    },
    _afterRowPrepared: noop,
    _handleDataChanged: function() {},
    callbackNames: function() {
        return ["scrollChanged"]
    },
    _updateScrollLeftPosition: function() {
        var scrollLeft = this._scrollLeft;
        if (scrollLeft >= 0) {
            this._scrollLeft = 0;
            this.scrollTo({
                left: scrollLeft
            })
        }
    },
    scrollTo: function(pos) {
        var $element = this.element();
        var $scrollContainer = $element && $element.children("." + this.addWidgetPrefix(SCROLL_CONTAINER_CLASS)).not("." + this.addWidgetPrefix(CONTENT_FIXED_CLASS));
        if (isDefined(pos) && isDefined(pos.left) && this._scrollLeft !== pos.left) {
            this._scrollLeft = pos.left;
            $scrollContainer && $scrollContainer.scrollLeft(pos.left)
        }
    },
    _wrapTableInScrollContainer: function($table) {
        var $scrollContainer = $("<div>");
        var useNative = this.option("scrolling.useNative");
        if (false === useNative || "auto" === useNative && !nativeScrolling) {
            $scrollContainer.addClass(this.addWidgetPrefix(SCROLLABLE_SIMULATED_CLASS))
        }
        eventsEngine.on($scrollContainer, "scroll", () => {
            var scrollLeft = $scrollContainer.scrollLeft();
            if (scrollLeft !== this._scrollLeft) {
                this.scrollChanged.fire({
                    left: scrollLeft
                }, this.name)
            }
        });
        $scrollContainer.addClass(this.addWidgetPrefix(CONTENT_CLASS)).addClass(this.addWidgetPrefix(SCROLL_CONTAINER_CLASS)).append($table).appendTo(this.element());
        this.setAria("role", "presentation", $scrollContainer);
        return $scrollContainer
    },
    _updateContent: function($newTableElement) {
        this.setTableElement($newTableElement);
        this._wrapTableInScrollContainer($newTableElement)
    },
    _findContentElement: noop,
    _getWidths: function($cellElements) {
        var result = [];
        var width;
        if ($cellElements) {
            iteratorUtils.each($cellElements, (function(index, item) {
                width = item.offsetWidth;
                if (item.getBoundingClientRect) {
                    var clientRect = getBoundingRect(item);
                    if (clientRect.width > width - 1) {
                        width = clientRect.width
                    }
                }
                result.push(width)
            }))
        }
        return result
    },
    getColumnWidths: function($tableElement) {
        var result = [];
        var $rows;
        var $cells;
        (this.option("forceApplyBindings") || noop)();
        $tableElement = $tableElement || this.getTableElement();
        if ($tableElement) {
            $rows = $tableElement.children("tbody:not(.dx-header)").children();
            for (var i = 0; i < $rows.length; i++) {
                var $row = $rows.eq(i);
                var isRowVisible = "none" !== $row.get(0).style.display && !$row.hasClass("dx-state-invisible");
                if (!$row.is("." + GROUP_ROW_CLASS) && !$row.is("." + DETAIL_ROW_CLASS) && !$row.is("." + ERROR_ROW_CLASS) && isRowVisible) {
                    $cells = $row.children("td");
                    break
                }
            }
            result = this._getWidths($cells)
        }
        return result
    },
    getVisibleColumnIndex: function(columnIndex, rowIndex) {
        return columnIndex
    },
    setColumnWidths: function(_ref) {
        var {
            widths: widths,
            $tableElement: $tableElement,
            columns: columns,
            fixed: fixed
        } = _ref;
        var $cols;
        var width;
        var minWidth;
        var columnIndex;
        var columnAutoWidth = this.option("columnAutoWidth");
        $tableElement = $tableElement || this.getTableElement();
        if ($tableElement && $tableElement.length && widths) {
            columnIndex = 0;
            $cols = $tableElement.children("colgroup").children("col");
            setWidth($cols, "auto");
            columns = columns || this.getColumns(null, $tableElement);
            for (var i = 0; i < columns.length; i++) {
                if (columnAutoWidth && !fixed) {
                    width = columns[i].width;
                    if (width && !columns[i].command) {
                        width = columns[i].visibleWidth || width;
                        width = getWidthStyle(width);
                        minWidth = getWidthStyle(columns[i].minWidth || width);
                        var $rows = $rows || $tableElement.children().children(".dx-row").not("." + DETAIL_ROW_CLASS);
                        for (var rowIndex = 0; rowIndex < $rows.length; rowIndex++) {
                            var row = $rows[rowIndex];
                            var cell = void 0;
                            var visibleIndex = this.getVisibleColumnIndex(i, rowIndex);
                            if (row.classList.contains(GROUP_ROW_CLASS)) {
                                if (1 !== visibleIndex) {
                                    cell = row.querySelector("td[aria-colindex='".concat(visibleIndex + 1, "']"))
                                }
                            } else {
                                cell = row.cells[visibleIndex]
                            }
                            if (cell) {
                                setCellWidth(cell, columns[i], width);
                                cell.style.minWidth = minWidth
                            }
                        }
                    }
                }
                if (columns[i].colspan) {
                    columnIndex += columns[i].colspan;
                    continue
                }
                width = widths[columnIndex];
                if ("adaptiveHidden" === width) {
                    width = HIDDEN_COLUMNS_WIDTH
                }
                if ("number" === typeof width) {
                    width = width.toFixed(3) + "px"
                }
                setWidth($cols.eq(columnIndex), isDefined(width) ? width : "auto");
                columnIndex++
            }
        }
    },
    getCellElements: function(rowIndex) {
        return this._getCellElementsCore(rowIndex)
    },
    _getCellElementsCore: function(rowIndex) {
        var $row = this._getRowElements().eq(rowIndex);
        return $row.children()
    },
    _getCellElement: function(rowIndex, columnIdentifier) {
        var $cell;
        var $cells = this.getCellElements(rowIndex);
        var columnVisibleIndex = this._getVisibleColumnIndex($cells, rowIndex, columnIdentifier);
        if ($cells.length && columnVisibleIndex >= 0) {
            $cell = $cells.eq(columnVisibleIndex)
        }
        if ($cell && $cell.length) {
            return $cell
        }
    },
    _getRowElement: function(rowIndex) {
        var that = this;
        var $rowElement = $();
        var $tableElements = that.getTableElements();
        iteratorUtils.each($tableElements, (function(_, tableElement) {
            $rowElement = $rowElement.add(that._getRowElements($(tableElement)).eq(rowIndex))
        }));
        if ($rowElement.length) {
            return $rowElement
        }
    },
    getCellElement: function(rowIndex, columnIdentifier) {
        return getPublicElement(this._getCellElement(rowIndex, columnIdentifier))
    },
    getRowElement: function(rowIndex) {
        var $rows = this._getRowElement(rowIndex);
        var elements = [];
        if ($rows && !getPublicElement($rows).get) {
            for (var i = 0; i < $rows.length; i++) {
                elements.push($rows[i])
            }
        } else {
            elements = $rows
        }
        return elements
    },
    _getVisibleColumnIndex: function($cells, rowIndex, columnIdentifier) {
        if (isString(columnIdentifier)) {
            var columnIndex = this._columnsController.columnOption(columnIdentifier, "index");
            return this._columnsController.getVisibleIndex(columnIndex)
        }
        return columnIdentifier
    },
    getColumnElements: function() {},
    getColumns: function(rowIndex) {
        return this._columnsController.getVisibleColumns(rowIndex)
    },
    getCell: function(cellPosition, rows) {
        var $rows = rows || this._getRowElements();
        var $cells;
        if ($rows.length > 0 && cellPosition.rowIndex >= 0) {
            if ("virtual" !== this.option("scrolling.mode") && "virtual" !== this.option("scrolling.rowRenderingMode")) {
                cellPosition.rowIndex = cellPosition.rowIndex < $rows.length ? cellPosition.rowIndex : $rows.length - 1
            }
            $cells = this.getCellElements(cellPosition.rowIndex);
            if ($cells && $cells.length > 0) {
                return $cells.eq($cells.length > cellPosition.columnIndex ? cellPosition.columnIndex : $cells.length - 1)
            }
        }
    },
    getRowsCount: function() {
        var tableElement = this.getTableElement();
        if (tableElement && 1 === tableElement.length) {
            return tableElement[0].rows.length
        }
        return 0
    },
    _getRowElementsCore: function(tableElement) {
        tableElement = tableElement || this.getTableElement();
        if (tableElement) {
            var hasRowTemplate = this.option().rowTemplate || this.option("dataRowTemplate");
            var tBodies = hasRowTemplate && tableElement.find("> tbody." + ROW_CLASS);
            return tBodies && tBodies.length ? tBodies : tableElement.find("> tbody > ." + ROW_CLASS + ", > ." + ROW_CLASS)
        }
        return $()
    },
    _getRowElements: function(tableElement) {
        return this._getRowElementsCore(tableElement)
    },
    getRowIndex: function($row) {
        return this._getRowElements().index($row)
    },
    getBoundingRect: function() {},
    getName: function() {},
    setScrollerSpacing: function(width) {
        var $element = this.element();
        var rtlEnabled = this.option("rtlEnabled");
        $element && $element.css({
            paddingLeft: rtlEnabled ? width : "",
            paddingRight: !rtlEnabled ? width : ""
        })
    },
    isScrollbarVisible: function(isHorizontal) {
        var $element = this.element();
        var $tableElement = this._tableElement;
        if ($element && $tableElement) {
            return isHorizontal ? getOuterWidth($tableElement) - getWidth($element) > 0 : getOuterHeight($tableElement) - getHeight($element) > 0
        }
        return false
    }
});
