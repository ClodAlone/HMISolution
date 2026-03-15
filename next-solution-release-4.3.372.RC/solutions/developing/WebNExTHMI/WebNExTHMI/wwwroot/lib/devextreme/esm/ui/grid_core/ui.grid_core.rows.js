/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.rows.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    getHeight,
    getOuterHeight,
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
import eventsEngine from "../../events/core/events_engine";
import {
    deferRender,
    deferUpdate
} from "../../core/utils/common";
import {
    setHeight
} from "../../core/utils/style";
import {
    isDefined,
    isNumeric,
    isString
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    getBoundingRect,
    getDefaultAlignment
} from "../../core/utils/position";
import {
    isEmpty
} from "../../core/utils/string";
import {
    compileGetter
} from "../../core/utils/data";
import gridCoreUtils from "./ui.grid_core.utils";
import {
    ColumnsView
} from "./ui.grid_core.columns_view";
import Scrollable from "../scroll_view/ui.scrollable";
import {
    removeEvent
} from "../../events/remove";
import messageLocalization from "../../localization/message";
import browser from "../../core/utils/browser";
var ROWS_VIEW_CLASS = "rowsview";
var CONTENT_CLASS = "content";
var NOWRAP_CLASS = "nowrap";
var GROUP_ROW_CLASS = "dx-group-row";
var GROUP_CELL_CLASS = "dx-group-cell";
var DATA_ROW_CLASS = "dx-data-row";
var FREE_SPACE_CLASS = "dx-freespace-row";
var ROW_LINES_CLASS = "dx-row-lines";
var COLUMN_LINES_CLASS = "dx-column-lines";
var ROW_ALTERNATION_CLASS = "dx-row-alt";
var LAST_ROW_BORDER = "dx-last-row-border";
var EMPTY_CLASS = "dx-empty";
var ROW_INSERTED_ANIMATION_CLASS = "row-inserted-animation";
var LOADPANEL_HIDE_TIMEOUT = 200;

function getMaxHorizontalScrollOffset(scrollable) {
    return scrollable ? Math.round(scrollable.scrollWidth() - scrollable.clientWidth()) : 0
}
export var rowsModule = {
    defaultOptions: function() {
        return {
            hoverStateEnabled: false,
            scrolling: {
                useNative: "auto"
            },
            loadPanel: {
                enabled: "auto",
                text: messageLocalization.format("Loading"),
                width: 200,
                height: 90,
                showIndicator: true,
                indicatorSrc: "",
                showPane: true
            },
            dataRowTemplate: null,
            columnAutoWidth: false,
            noDataText: messageLocalization.format("dxDataGrid-noDataText"),
            wordWrapEnabled: false,
            showColumnLines: true,
            showRowLines: false,
            rowAlternationEnabled: false,
            activeStateEnabled: false,
            twoWayBindingEnabled: true
        }
    },
    views: {
        rowsView: ColumnsView.inherit(function() {
            var defaultCellTemplate = function($container, options) {
                var isDataTextEmpty = isEmpty(options.text) && "data" === options.rowType;
                var text = options.text;
                var container = $container.get(0);
                if (isDataTextEmpty) {
                    gridCoreUtils.setEmptyText($container)
                } else if (options.column.encodeHtml) {
                    container.textContent = text
                } else {
                    container.innerHTML = text
                }
            };
            return {
                _getDefaultTemplate: function(column) {
                    switch (column.command) {
                        case "empty":
                            return function(container) {
                                container.html("&nbsp;")
                            };
                        default:
                            return defaultCellTemplate
                    }
                },
                _getDefaultGroupTemplate: function(column) {
                    var summaryTexts = this.option("summary.texts");
                    return function($container, options) {
                        var data = options.data;
                        var text = options.column.caption + ": " + options.text;
                        var container = $container.get(0);
                        if (options.summaryItems && options.summaryItems.length) {
                            text += " " + gridCoreUtils.getGroupRowSummaryText(options.summaryItems, summaryTexts)
                        }
                        if (data) {
                            if (options.groupContinuedMessage && options.groupContinuesMessage) {
                                text += " (" + options.groupContinuedMessage + ". " + options.groupContinuesMessage + ")"
                            } else if (options.groupContinuesMessage) {
                                text += " (" + options.groupContinuesMessage + ")"
                            } else if (options.groupContinuedMessage) {
                                text += " (" + options.groupContinuedMessage + ")"
                            }
                        }
                        $container.addClass(GROUP_CELL_CLASS);
                        if (column.encodeHtml) {
                            container.textContent = text
                        } else {
                            container.innerHTML = text
                        }
                    }
                },
                _update: function() {},
                _getCellTemplate: function(options) {
                    var column = options.column;
                    var template;
                    if ("group" === options.rowType && isDefined(column.groupIndex) && !column.showWhenGrouped && !column.command) {
                        template = column.groupCellTemplate || {
                            allowRenderToDetachedContainer: true,
                            render: this._getDefaultGroupTemplate(column)
                        }
                    } else if (("data" === options.rowType || column.command) && column.cellTemplate) {
                        template = column.cellTemplate
                    } else {
                        template = {
                            allowRenderToDetachedContainer: true,
                            render: this._getDefaultTemplate(column)
                        }
                    }
                    return template
                },
                _createRow: function(row) {
                    var $row = this.callBase.apply(this, arguments);
                    if (row) {
                        var isGroup = "group" === row.rowType;
                        var isDataRow = "data" === row.rowType;
                        isDataRow && $row.addClass(DATA_ROW_CLASS);
                        isDataRow && this.option("showRowLines") && $row.addClass(ROW_LINES_CLASS);
                        this.option("showColumnLines") && $row.addClass(COLUMN_LINES_CLASS);
                        if (false === row.visible) {
                            $row.hide()
                        }
                        if (isGroup) {
                            $row.addClass(GROUP_ROW_CLASS);
                            var isRowExpanded = row.isExpanded;
                            this.setAria("role", "row", $row);
                            this.setAria("expanded", isDefined(isRowExpanded) && isRowExpanded.toString(), $row)
                        }
                    }
                    return $row
                },
                _rowPrepared: function($row, rowOptions, row) {
                    if ("data" === rowOptions.rowType) {
                        if (this.option("rowAlternationEnabled")) {
                            this._isAltRow(row) && $row.addClass(ROW_ALTERNATION_CLASS);
                            rowOptions.watch && rowOptions.watch(() => this._isAltRow(row), value => {
                                $row.toggleClass(ROW_ALTERNATION_CLASS, value)
                            })
                        }
                        this._setAriaRowIndex(rowOptions, $row);
                        rowOptions.watch && rowOptions.watch(() => rowOptions.rowIndex, () => this._setAriaRowIndex(rowOptions, $row))
                    }
                    this.callBase.apply(this, arguments)
                },
                _setAriaRowIndex: function(row, $row) {
                    var component = this.component;
                    var isPagerMode = "standard" === component.option("scrolling.mode") && !gridCoreUtils.isVirtualRowRendering(component);
                    var rowIndex = row.rowIndex + 1;
                    if (isPagerMode) {
                        rowIndex = component.pageIndex() * component.pageSize() + rowIndex
                    } else {
                        rowIndex += this._dataController.getRowIndexOffset()
                    }
                    this.setAria("rowindex", rowIndex, $row)
                },
                _afterRowPrepared: function(e) {
                    var arg = e.args[0];
                    var dataController = this._dataController;
                    var row = dataController.getVisibleRows()[arg.rowIndex];
                    var watch = this.option("integrationOptions.watchMethod");
                    if (!arg.data || "data" !== arg.rowType || arg.isNewRow || !this.option("twoWayBindingEnabled") || !watch || !row) {
                        return
                    }
                    var dispose = watch(() => dataController.generateDataValues(arg.data, arg.columns), () => {
                        dataController.repaintRows([row.rowIndex], this.option("repaintChangesOnly"))
                    }, {
                        deep: true,
                        skipImmediate: true
                    });
                    eventsEngine.on(arg.rowElement, removeEvent, dispose)
                },
                _renderScrollable: function(force) {
                    var $element = this.element();
                    if (!$element.children().length) {
                        $element.append("<div>")
                    }
                    if (force || !this._loadPanel) {
                        this._renderLoadPanel($element, $element.parent(), this._dataController.isLocalStore())
                    }
                    if ((force || !this.getScrollable()) && this._dataController.isLoaded()) {
                        var columns = this.getColumns();
                        var allColumnsHasWidth = true;
                        for (var i = 0; i < columns.length; i++) {
                            if (!columns[i].width && !columns[i].minWidth) {
                                allColumnsHasWidth = false;
                                break
                            }
                        }
                        if (this.option("columnAutoWidth") || this._hasHeight || allColumnsHasWidth || this._columnsController._isColumnFixing()) {
                            this._renderScrollableCore($element)
                        }
                    }
                },
                _handleScroll: function(e) {
                    var rtlEnabled = this.option("rtlEnabled");
                    var isNativeScrolling = e.component.option("useNative");
                    this._scrollTop = e.scrollOffset.top;
                    this._scrollLeft = e.scrollOffset.left;
                    var scrollLeft = e.scrollOffset.left;
                    if (rtlEnabled) {
                        this._scrollRight = getMaxHorizontalScrollOffset(e.component) - this._scrollLeft;
                        if (isNativeScrolling) {
                            scrollLeft = -this._scrollRight
                        }
                        if (!this.isScrollbarVisible(true)) {
                            this._scrollLeft = -1
                        }
                    }
                    this.scrollChanged.fire(_extends({}, e.scrollOffset, {
                        left: scrollLeft
                    }), this.name)
                },
                _renderScrollableCore: function($element) {
                    var dxScrollableOptions = this._createScrollableOptions();
                    var scrollHandler = this._handleScroll.bind(this);
                    dxScrollableOptions.onScroll = scrollHandler;
                    this._scrollable = this._createComponent($element, Scrollable, dxScrollableOptions);
                    this._scrollableContainer = this._scrollable && $(this._scrollable.container())
                },
                _renderLoadPanel: gridCoreUtils.renderLoadPanel,
                _renderContent: function(contentElement, tableElement) {
                    contentElement.empty().append(tableElement);
                    return this._findContentElement()
                },
                _updateContent: function(newTableElement, change) {
                    var that = this;
                    var tableElement = that.getTableElement();
                    var contentElement = that._findContentElement();
                    var changeType = change && change.changeType;
                    var executors = [];
                    var highlightChanges = this.option("highlightChanges");
                    var rowInsertedClass = this.addWidgetPrefix(ROW_INSERTED_ANIMATION_CLASS);
                    switch (changeType) {
                        case "update":
                            each(change.rowIndices, (function(index, rowIndex) {
                                var $newRowElement = that._getRowElements(newTableElement).eq(index);
                                var changeType = change.changeTypes && change.changeTypes[index];
                                var item = change.items && change.items[index];
                                executors.push((function() {
                                    var $rowsElement = that._getRowElements();
                                    var $rowElement = $rowsElement.eq(rowIndex);
                                    switch (changeType) {
                                        case "update":
                                            if (item) {
                                                var columnIndices = change.columnIndices && change.columnIndices[index];
                                                if (isDefined(item.visible) && item.visible !== $rowElement.is(":visible")) {
                                                    $rowElement.toggle(item.visible)
                                                } else if (columnIndices) {
                                                    that._updateCells($rowElement, $newRowElement, columnIndices)
                                                } else {
                                                    $rowElement.replaceWith($newRowElement)
                                                }
                                            }
                                            break;
                                        case "insert":
                                            if (!$rowsElement.length) {
                                                if (tableElement) {
                                                    var target = $newRowElement.is("tbody") ? tableElement : tableElement.children("tbody");
                                                    $newRowElement.prependTo(target)
                                                }
                                            } else if ($rowElement.length) {
                                                $newRowElement.insertBefore($rowElement)
                                            } else {
                                                $newRowElement.insertAfter($rowsElement.last())
                                            }
                                            if (highlightChanges && change.isLiveUpdate) {
                                                $newRowElement.addClass(rowInsertedClass)
                                            }
                                            break;
                                        case "remove":
                                            $rowElement.remove()
                                    }
                                }))
                            }));
                            each(executors, (function() {
                                this()
                            }));
                            newTableElement.remove();
                            break;
                        default:
                            that.setTableElement(newTableElement);
                            contentElement.addClass(that.addWidgetPrefix(CONTENT_CLASS));
                            that._renderContent(contentElement, newTableElement)
                    }
                },
                _createEmptyRow: function(className, isFixed, height) {
                    var $cell;
                    var $row = this._createRow();
                    var columns = isFixed ? this.getFixedColumns() : this.getColumns();
                    $row.addClass(className).toggleClass(COLUMN_LINES_CLASS, this.option("showColumnLines"));
                    for (var i = 0; i < columns.length; i++) {
                        $cell = this._createCell({
                            column: columns[i],
                            rowType: "freeSpace",
                            columnIndex: i,
                            columns: columns
                        });
                        isNumeric(height) && $cell.css("height", height);
                        $row.append($cell)
                    }
                    this.setAria("role", "presentation", $row);
                    return $row
                },
                _appendEmptyRow: function($table, $emptyRow, location) {
                    var $tBodies = this._getBodies($table);
                    var isTableContainer = !$tBodies.length || $emptyRow.is("tbody");
                    var $container = isTableContainer ? $table : $tBodies;
                    if ("top" === location) {
                        $container.first().prepend($emptyRow);
                        if (isTableContainer) {
                            var $colgroup = $container.children("colgroup");
                            $container.prepend($colgroup)
                        }
                    } else {
                        $container.last().append($emptyRow)
                    }
                },
                _renderFreeSpaceRow: function($tableElement) {
                    var $freeSpaceRowElement = this._createEmptyRow(FREE_SPACE_CLASS);
                    $freeSpaceRowElement = this._wrapRowIfNeed($tableElement, $freeSpaceRowElement);
                    this._appendEmptyRow($tableElement, $freeSpaceRowElement)
                },
                _checkRowKeys: function(options) {
                    var that = this;
                    var rows = that._getRows(options);
                    var keyExpr = that._dataController.store() && that._dataController.store().key();
                    keyExpr && rows.some((function(row) {
                        if ("data" === row.rowType && void 0 === row.key) {
                            that._dataController.fireError("E1046", keyExpr);
                            return true
                        }
                    }))
                },
                _needUpdateRowHeight: function(itemsCount) {
                    return itemsCount > 0 && !this._rowHeight
                },
                _getRowsHeight: function($tableElement) {
                    var $rowElements = $tableElement.children("tbody").children().not(".dx-virtual-row").not("." + FREE_SPACE_CLASS);
                    return $rowElements.toArray().reduce((function(sum, row) {
                        return sum + getBoundingRect(row).height
                    }), 0)
                },
                _updateRowHeight: function() {
                    var $tableElement = this.getTableElement();
                    var itemsCount = this._dataController.items().length;
                    if ($tableElement && this._needUpdateRowHeight(itemsCount)) {
                        var rowsHeight = this._getRowsHeight($tableElement);
                        this._rowHeight = rowsHeight / itemsCount
                    }
                },
                _findContentElement: function() {
                    var $content = this.element();
                    var scrollable = this.getScrollable();
                    if ($content) {
                        if (scrollable) {
                            $content = $(scrollable.content())
                        }
                        return $content.children().first()
                    }
                },
                _getRowElements: function(tableElement) {
                    var $rows = this.callBase(tableElement);
                    return $rows && $rows.not("." + FREE_SPACE_CLASS)
                },
                _getFreeSpaceRowElements: function($table) {
                    var tableElements = $table || this.getTableElements();
                    return tableElements && tableElements.children("tbody").children("." + FREE_SPACE_CLASS)
                },
                _getNoDataText: function() {
                    return this.option("noDataText")
                },
                _rowClick: function(e) {
                    var item = this._dataController.items()[e.rowIndex] || {};
                    this.executeAction("onRowClick", extend({
                        evaluate: function(expr) {
                            var getter = compileGetter(expr);
                            return getter(item.data)
                        }
                    }, e, item))
                },
                _rowDblClick: function(e) {
                    var item = this._dataController.items()[e.rowIndex] || {};
                    this.executeAction("onRowDblClick", extend({}, e, item))
                },
                _getColumnsCountBeforeGroups: function(columns) {
                    for (var i = 0; i < columns.length; i++) {
                        if ("groupExpand" === columns[i].type) {
                            return i
                        }
                    }
                    return 0
                },
                _getGroupCellOptions: function(options) {
                    var columnsCountBeforeGroups = this._getColumnsCountBeforeGroups(options.columns);
                    var columnIndex = (options.row.groupIndex || 0) + columnsCountBeforeGroups;
                    return {
                        columnIndex: columnIndex,
                        colspan: options.columns.length - columnIndex - 1
                    }
                },
                _renderCells: function($row, options) {
                    if ("group" === options.row.rowType) {
                        this._renderGroupedCells($row, options)
                    } else if (options.row.values) {
                        this.callBase($row, options)
                    }
                },
                _renderGroupedCells: function($row, options) {
                    var row = options.row;
                    var expandColumn;
                    var columns = options.columns;
                    var rowIndex = row.rowIndex;
                    var isExpanded;
                    var groupCellOptions = this._getGroupCellOptions(options);
                    for (var i = 0; i <= groupCellOptions.columnIndex; i++) {
                        if (i === groupCellOptions.columnIndex && columns[i].allowCollapsing && "infinite" !== options.scrollingMode) {
                            isExpanded = !!row.isExpanded;
                            expandColumn = columns[i]
                        } else {
                            isExpanded = null;
                            expandColumn = {
                                command: "expand",
                                cssClass: columns[i].cssClass
                            }
                        }
                        if (this._needRenderCell(i, options.columnIndices)) {
                            this._renderCell($row, {
                                value: isExpanded,
                                row: row,
                                rowIndex: rowIndex,
                                column: expandColumn,
                                columnIndex: i,
                                columnIndices: options.columnIndices
                            })
                        }
                    }
                    var groupColumnAlignment = getDefaultAlignment(this.option("rtlEnabled"));
                    var groupColumn = extend({}, columns[groupCellOptions.columnIndex], {
                        command: null,
                        type: null,
                        cssClass: null,
                        width: null,
                        showWhenGrouped: false,
                        alignment: groupColumnAlignment
                    });
                    if (groupCellOptions.colspan > 1) {
                        groupColumn.colspan = groupCellOptions.colspan
                    }
                    if (this._needRenderCell(groupCellOptions.columnIndex + 1, options.columnIndices)) {
                        this._renderCell($row, {
                            value: row.values[row.groupIndex],
                            row: row,
                            rowIndex: rowIndex,
                            column: groupColumn,
                            columnIndex: groupCellOptions.columnIndex + 1,
                            columnIndices: options.columnIndices
                        })
                    }
                },
                _renderRows: function($table, options) {
                    var scrollingMode = this.option("scrolling.mode");
                    this.callBase($table, extend({
                        scrollingMode: scrollingMode
                    }, options));
                    this._checkRowKeys(options.change);
                    this._renderFreeSpaceRow($table);
                    if (!this._hasHeight) {
                        this.updateFreeSpaceRowHeight($table)
                    }
                },
                _renderDataRowByTemplate($table, options, dataRowTemplate) {
                    var row = options.row;
                    var rowOptions = extend({
                        columns: options.columns
                    }, row);
                    var $tbody = this._createRow(row, "tbody");
                    $tbody.appendTo($table);
                    this.renderTemplate($tbody, dataRowTemplate, rowOptions, true, options.change);
                    this._rowPrepared($tbody, rowOptions, options.row)
                },
                _renderRow: function($table, options) {
                    var row = options.row;
                    var rowTemplate = this.option().rowTemplate;
                    var dataRowTemplate = this.option("dataRowTemplate");
                    if ("data" === row.rowType && dataRowTemplate) {
                        this._renderDataRowByTemplate($table, options, dataRowTemplate)
                    } else if (("data" === row.rowType || "group" === row.rowType) && !isDefined(row.groupIndex) && rowTemplate) {
                        this.renderTemplate($table, rowTemplate, extend({
                            columns: options.columns
                        }, row), true)
                    } else {
                        this.callBase($table, options)
                    }
                },
                _renderTable: function(options) {
                    var that = this;
                    var $table = that.callBase(options);
                    if (!isDefined(that.getTableElement())) {
                        that.setTableElement($table);
                        that._renderScrollable(true);
                        that.resizeCompleted.add((function resizeCompletedHandler() {
                            var scrollableInstance = that.getScrollable();
                            if (scrollableInstance && that.element().closest(getWindow().document).length) {
                                that.resizeCompleted.remove(resizeCompletedHandler);
                                scrollableInstance._visibilityChanged(true)
                            }
                        }))
                    } else {
                        that._renderScrollable()
                    }
                    return $table
                },
                _createTable: function() {
                    var $table = this.callBase.apply(this, arguments);
                    if (this.option().rowTemplate || this.option().dataRowTemplate) {
                        $table.appendTo(this.component.$element())
                    }
                    return $table
                },
                _renderCore: function(change) {
                    var $element = this.element();
                    $element.addClass(this.addWidgetPrefix(ROWS_VIEW_CLASS)).toggleClass(this.addWidgetPrefix(NOWRAP_CLASS), !this.option("wordWrapEnabled"));
                    $element.toggleClass(EMPTY_CLASS, this._dataController.isEmpty());
                    this.setAria("role", "presentation", $element);
                    var $table = this._renderTable({
                        change: change
                    });
                    this._updateContent($table, change);
                    this.callBase(change);
                    this._lastColumnWidths = null
                },
                _getRows: function(change) {
                    return change && change.items || this._dataController.items()
                },
                _getCellOptions: function(options) {
                    var column = options.column;
                    var row = options.row;
                    var data = row.data;
                    var summaryCells = row && row.summaryCells;
                    var value = options.value;
                    var displayValue = gridCoreUtils.getDisplayValue(column, value, data, row.rowType);
                    var parameters = this.callBase(options);
                    parameters.value = value;
                    parameters.oldValue = options.oldValue;
                    parameters.displayValue = displayValue;
                    parameters.row = row;
                    parameters.key = row.key;
                    parameters.data = data;
                    parameters.rowType = row.rowType;
                    parameters.values = row.values;
                    parameters.text = !column.command ? gridCoreUtils.formatValue(displayValue, column) : "";
                    parameters.rowIndex = row.rowIndex;
                    parameters.summaryItems = summaryCells && summaryCells[options.columnIndex];
                    parameters.resized = column.resizedCallbacks;
                    if (isDefined(column.groupIndex) && !column.command) {
                        var groupingTextsOptions = this.option("grouping.texts");
                        var scrollingMode = this.option("scrolling.mode");
                        if ("virtual" !== scrollingMode && "infinite" !== scrollingMode) {
                            parameters.groupContinuesMessage = data && data.isContinuationOnNextPage && groupingTextsOptions && groupingTextsOptions.groupContinuesMessage;
                            parameters.groupContinuedMessage = data && data.isContinuation && groupingTextsOptions && groupingTextsOptions.groupContinuedMessage
                        }
                    }
                    return parameters
                },
                _setRowsOpacityCore: function($rows, visibleColumns, columnIndex, value) {
                    var columnsController = this._columnsController;
                    var columns = columnsController.getColumns();
                    var column = columns && columns[columnIndex];
                    var columnID = column && column.isBand && column.index;
                    each($rows, (function(rowIndex, row) {
                        if (!$(row).hasClass(GROUP_ROW_CLASS)) {
                            for (var i = 0; i < visibleColumns.length; i++) {
                                if (isNumeric(columnID) && columnsController.isParentBandColumn(visibleColumns[i].index, columnID) || visibleColumns[i].index === columnIndex) {
                                    $rows.eq(rowIndex).children().eq(i).css({
                                        opacity: value
                                    });
                                    if (!isNumeric(columnID)) {
                                        break
                                    }
                                }
                            }
                        }
                    }))
                },
                _getDevicePixelRatio: function() {
                    return getWindow().devicePixelRatio
                },
                renderNoDataText: gridCoreUtils.renderNoDataText,
                getCellOptions: function(rowIndex, columnIdentifier) {
                    var rowOptions = this._dataController.items()[rowIndex];
                    var cellOptions;
                    var column;
                    if (rowOptions) {
                        if (isString(columnIdentifier)) {
                            column = this._columnsController.columnOption(columnIdentifier)
                        } else {
                            column = this._columnsController.getVisibleColumns()[columnIdentifier]
                        }
                        if (column) {
                            cellOptions = this._getCellOptions({
                                value: column.calculateCellValue(rowOptions.data),
                                rowIndex: rowOptions.rowIndex,
                                row: rowOptions,
                                column: column
                            })
                        }
                    }
                    return cellOptions
                },
                getRow: function(index) {
                    if (index >= 0) {
                        var rows = this._getRowElements();
                        if (rows.length > index) {
                            return $(rows[index])
                        }
                    }
                },
                updateFreeSpaceRowHeight: function($table) {
                    var dataController = this._dataController;
                    var itemCount = dataController.items(true).length;
                    var contentElement = this._findContentElement();
                    var freeSpaceRowElements = this._getFreeSpaceRowElements($table);
                    if (freeSpaceRowElements && contentElement && dataController.totalCount() >= 0) {
                        var isFreeSpaceRowVisible = false;
                        if (itemCount > 0) {
                            if (!this._hasHeight) {
                                var freeSpaceRowCount = dataController.pageSize() - itemCount;
                                var scrollingMode = this.option("scrolling.mode");
                                if (freeSpaceRowCount > 0 && dataController.pageCount() > 1 && "virtual" !== scrollingMode && "infinite" !== scrollingMode) {
                                    setHeight(freeSpaceRowElements, freeSpaceRowCount * this._rowHeight);
                                    isFreeSpaceRowVisible = true
                                }
                                if (!isFreeSpaceRowVisible && $table) {
                                    setHeight(freeSpaceRowElements, 0)
                                } else {
                                    freeSpaceRowElements.toggle(isFreeSpaceRowVisible)
                                }
                                this._updateLastRowBorder(isFreeSpaceRowVisible)
                            } else {
                                freeSpaceRowElements.hide();
                                deferUpdate(() => {
                                    var scrollbarWidth = this.getScrollbarWidth(true);
                                    var elementHeightWithoutScrollbar = getHeight(this.element()) - scrollbarWidth;
                                    var contentHeight = getOuterHeight(contentElement);
                                    var showFreeSpaceRow = elementHeightWithoutScrollbar - contentHeight > 0;
                                    var rowsHeight = this._getRowsHeight(contentElement.children().first());
                                    var $tableElement = $table || this.getTableElements();
                                    var borderTopWidth = Math.ceil(parseFloat($tableElement.css("borderTopWidth")));
                                    var heightCorrection = this._getHeightCorrection();
                                    var resultHeight = elementHeightWithoutScrollbar - rowsHeight - borderTopWidth - heightCorrection;
                                    if (showFreeSpaceRow) {
                                        deferRender(() => {
                                            freeSpaceRowElements.css("height", resultHeight);
                                            isFreeSpaceRowVisible = true;
                                            freeSpaceRowElements.show()
                                        })
                                    }
                                    deferRender(() => this._updateLastRowBorder(isFreeSpaceRowVisible))
                                })
                            }
                        } else {
                            freeSpaceRowElements.css("height", 0);
                            freeSpaceRowElements.show();
                            this._updateLastRowBorder(true)
                        }
                    }
                },
                _getHeightCorrection: function() {
                    var isZoomedWebkit = browser.webkit && this._getDevicePixelRatio() >= 2;
                    var isChromeLatest = browser.chrome && browser.version >= 91;
                    var hasExtraBorderTop = browser.mozilla && browser.version >= 70 && !this.option("showRowLines");
                    return isZoomedWebkit || hasExtraBorderTop || isChromeLatest ? 1 : 0
                },
                _columnOptionChanged: function(e) {
                    var optionNames = e.optionNames;
                    if (e.changeTypes.grouping) {
                        return
                    }
                    if (optionNames.width || optionNames.visibleWidth) {
                        this.callBase(e);
                        this._fireColumnResizedCallbacks()
                    }
                },
                getScrollable: function() {
                    return this._scrollable
                },
                init: function() {
                    var that = this;
                    var dataController = that.getController("data");
                    that.callBase();
                    that._editorFactoryController = that.getController("editorFactory");
                    that._rowHeight = 0;
                    that._scrollTop = 0;
                    that._scrollLeft = -1;
                    that._scrollRight = 0;
                    that._hasHeight = false;
                    dataController.loadingChanged.add((function(isLoading, messageText) {
                        that.setLoading(isLoading, messageText)
                    }));
                    dataController.dataSourceChanged.add((function() {
                        if (that._scrollLeft >= 0) {
                            that._handleScroll({
                                component: that.getScrollable(),
                                scrollOffset: {
                                    top: that._scrollTop,
                                    left: that._scrollLeft
                                }
                            })
                        }
                    }))
                },
                _handleDataChanged: function(change) {
                    switch (change.changeType) {
                        case "refresh":
                        case "prepend":
                        case "append":
                        case "update":
                            this.render(null, change);
                            break;
                        default:
                            this._update(change)
                    }
                },
                publicMethods: function() {
                    return ["isScrollbarVisible", "getTopVisibleRowData", "getScrollbarWidth", "getCellElement", "getRowElement", "getScrollable"]
                },
                contentWidth: function() {
                    return getWidth(this.element()) - this.getScrollbarWidth()
                },
                getScrollbarWidth: function(isHorizontal) {
                    var scrollableContainer = this._scrollableContainer && this._scrollableContainer.get(0);
                    var scrollbarWidth = 0;
                    if (scrollableContainer) {
                        if (!isHorizontal) {
                            scrollbarWidth = scrollableContainer.clientWidth ? scrollableContainer.offsetWidth - scrollableContainer.clientWidth : 0
                        } else {
                            scrollbarWidth = scrollableContainer.clientHeight ? scrollableContainer.offsetHeight - scrollableContainer.clientHeight : 0;
                            scrollbarWidth += (that = this, scrollable = that.getScrollable(), scrollable ? Math.ceil(parseFloat($(scrollable.content()).css("paddingBottom"))) : 0)
                        }
                    }
                    var that, scrollable;
                    return scrollbarWidth > 0 ? scrollbarWidth : 0
                },
                _fireColumnResizedCallbacks: function() {
                    var lastColumnWidths = this._lastColumnWidths || [];
                    var columnWidths = [];
                    var columns = this.getColumns();
                    for (var i = 0; i < columns.length; i++) {
                        columnWidths[i] = columns[i].visibleWidth;
                        if (columns[i].resizedCallbacks && !isDefined(columns[i].groupIndex) && lastColumnWidths[i] !== columnWidths[i]) {
                            columns[i].resizedCallbacks.fire(columnWidths[i])
                        }
                    }
                    this._lastColumnWidths = columnWidths
                },
                _updateLastRowBorder: function(isFreeSpaceRowVisible) {
                    if (this.option("showBorders") && this.option("showRowLines") && !isFreeSpaceRowVisible) {
                        this.element().addClass(LAST_ROW_BORDER)
                    } else {
                        this.element().removeClass(LAST_ROW_BORDER)
                    }
                },
                _updateScrollable: function() {
                    var dxScrollable = Scrollable.getInstance(this.element());
                    if (dxScrollable) {
                        dxScrollable.update();
                        this._updateHorizontalScrollPosition()
                    }
                },
                _updateHorizontalScrollPosition: function() {
                    var scrollable = this.getScrollable();
                    var scrollLeft = scrollable && scrollable.scrollOffset().left;
                    var rtlEnabled = this.option("rtlEnabled");
                    if (rtlEnabled) {
                        var maxHorizontalScrollOffset = getMaxHorizontalScrollOffset(scrollable);
                        var scrollRight = maxHorizontalScrollOffset - scrollLeft;
                        if (scrollRight !== this._scrollRight) {
                            this._scrollLeft = maxHorizontalScrollOffset - this._scrollRight
                        }
                    }
                    if (this._scrollLeft >= 0 && scrollLeft !== this._scrollLeft) {
                        scrollable.scrollTo({
                            x: this._scrollLeft
                        })
                    }
                },
                _resizeCore: function() {
                    var that = this;
                    that._fireColumnResizedCallbacks();
                    that._updateRowHeight();
                    deferRender((function() {
                        that._renderScrollable();
                        that.renderNoDataText();
                        that.updateFreeSpaceRowHeight();
                        deferUpdate((function() {
                            that._updateScrollable()
                        }))
                    }))
                },
                scrollTo: function(location) {
                    var $element = this.element();
                    var dxScrollable = $element && Scrollable.getInstance($element);
                    if (dxScrollable) {
                        dxScrollable.scrollTo(location)
                    }
                },
                height: function(_height, hasHeight) {
                    var that = this;
                    var $element = this.element();
                    if (0 === arguments.length) {
                        return $element ? getOuterHeight($element, true) : 0
                    }
                    that._hasHeight = void 0 === hasHeight ? "auto" !== _height : hasHeight;
                    if (isDefined(_height) && $element) {
                        setHeight($element, _height)
                    }
                },
                setLoading: function(isLoading, messageText) {
                    var loadPanel = this._loadPanel;
                    var dataController = this._dataController;
                    var loadPanelOptions = this.option("loadPanel") || {};
                    var animation = dataController.isLoaded() ? loadPanelOptions.animation : null;
                    var $element = this.element();
                    if (!hasWindow()) {
                        return
                    }
                    if (!loadPanel && void 0 !== messageText && dataController.isLocalStore() && "auto" === loadPanelOptions.enabled && $element) {
                        this._renderLoadPanel($element, $element.parent());
                        loadPanel = this._loadPanel
                    }
                    if (loadPanel) {
                        var visibilityOptions = {
                            message: messageText || loadPanelOptions.text,
                            animation: animation,
                            visible: isLoading
                        };
                        if (isLoading) {
                            visibilityOptions.position = gridCoreUtils.calculateLoadPanelPosition($element)
                        }
                        clearTimeout(this._hideLoadingTimeoutID);
                        if (loadPanel.option("visible") && !isLoading) {
                            this._hideLoadingTimeoutID = setTimeout((function() {
                                loadPanel.option(visibilityOptions)
                            }), LOADPANEL_HIDE_TIMEOUT)
                        } else {
                            loadPanel.option(visibilityOptions)
                        }
                    }
                },
                setRowsOpacity: function(columnIndex, value) {
                    var $rows = this._getRowElements().not("." + GROUP_ROW_CLASS) || [];
                    this._setRowsOpacityCore($rows, this.getColumns(), columnIndex, value)
                },
                _getCellElementsCore: function(rowIndex) {
                    var $cells = this.callBase.apply(this, arguments);
                    if ($cells) {
                        var groupCellIndex = $cells.filter("." + GROUP_CELL_CLASS).index();
                        if (groupCellIndex >= 0 && $cells.length > groupCellIndex + 1) {
                            return $cells.slice(0, groupCellIndex + 1)
                        }
                    }
                    return $cells
                },
                _getBoundaryVisibleItemIndex: function(isTop, isFloor) {
                    var itemIndex = 0;
                    var prevOffset = 0;
                    var offset = 0;
                    var viewportBoundary = this._scrollTop;
                    var $contentElement = this._findContentElement();
                    var contentElementOffsetTop = $contentElement && $contentElement.offset().top;
                    var dataController = this.getController("data");
                    var items = dataController.items();
                    var tableElement = this.getTableElement();
                    if (items.length && tableElement) {
                        var rowElements = this._getRowElements(tableElement).filter(":visible");
                        if (!isTop) {
                            var height = this._hasHeight ? getOuterHeight(this.element()) : $(getWindow()).outerHeight();
                            viewportBoundary += height
                        }
                        for (itemIndex = 0; itemIndex < items.length; itemIndex++) {
                            prevOffset = offset;
                            var $rowElement = $(rowElements).eq(itemIndex);
                            if ($rowElement.length) {
                                offset = $rowElement.offset();
                                offset = (isTop ? offset.top : offset.top + getOuterHeight($rowElement)) - contentElementOffsetTop;
                                if (offset > viewportBoundary) {
                                    if (itemIndex) {
                                        if (isFloor || 2 * viewportBoundary < Math.round(offset + prevOffset)) {
                                            itemIndex--
                                        }
                                    }
                                    break
                                }
                            }
                        }
                        if (itemIndex && itemIndex === items.length) {
                            itemIndex--
                        }
                    }
                    return itemIndex
                },
                getTopVisibleItemIndex: function(isFloor) {
                    return this._getBoundaryVisibleItemIndex(true, isFloor)
                },
                getBottomVisibleItemIndex: function(isFloor) {
                    return this._getBoundaryVisibleItemIndex(false, isFloor)
                },
                getTopVisibleRowData: function() {
                    var itemIndex = this.getTopVisibleItemIndex();
                    var items = this._dataController.items();
                    if (items[itemIndex]) {
                        return items[itemIndex].data
                    }
                },
                _scrollToElement: function($element, offset) {
                    var scrollable = this.getScrollable();
                    scrollable && scrollable.scrollToElement($element, offset)
                },
                optionChanged: function(args) {
                    this.callBase(args);
                    switch (args.name) {
                        case "wordWrapEnabled":
                        case "showColumnLines":
                        case "showRowLines":
                        case "rowAlternationEnabled":
                        case "rowTemplate":
                        case "dataRowTemplate":
                        case "twoWayBindingEnabled":
                            this._invalidate(true, true);
                            args.handled = true;
                            break;
                        case "scrolling":
                            this._rowHeight = null;
                            this._tableElement = null;
                            args.handled = true;
                            break;
                        case "rtlEnabled":
                            this._rowHeight = null;
                            this._tableElement = null;
                            break;
                        case "loadPanel":
                            this._tableElement = null;
                            this._invalidate(true, "loadPanel.enabled" !== args.fullName);
                            args.handled = true;
                            break;
                        case "noDataText":
                            this.renderNoDataText();
                            args.handled = true
                    }
                },
                dispose: function() {
                    clearTimeout(this._hideLoadingTimeoutID);
                    this._scrollable && this._scrollable.dispose()
                },
                setScrollerSpacing: function() {},
                _restoreErrorRow: function() {}
            }
        }())
    }
};
