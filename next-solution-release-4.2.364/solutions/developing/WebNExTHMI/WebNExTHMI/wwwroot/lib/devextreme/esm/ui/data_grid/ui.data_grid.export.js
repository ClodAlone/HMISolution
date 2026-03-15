/**
 * DevExtreme (esm/ui/data_grid/ui.data_grid.export.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import $ from "../../core/renderer";
import Class from "../../core/class";
import {
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    getDefaultAlignment
} from "../../core/utils/position";
import {
    merge
} from "../../core/utils/array";
import dataGridCore from "./ui.data_grid.core";
import exportMixin from "../grid_core/ui.grid_core.export_mixin";
import {
    export as clientExport,
    excel
} from "../../exporter";
import messageLocalization from "../../localization/message";
import "../button";
import "../drop_down_button";
import List from "../list_light";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
var DATAGRID_EXPORT_MENU_CLASS = "dx-datagrid-export-menu";
var DATAGRID_EXPORT_BUTTON_CLASS = "dx-datagrid-export-button";
var DATAGRID_EXPORT_TOOLBAR_BUTTON_NAME = "exportButton";
var DATAGRID_EXPORT_ICON = "export-to";
var DATAGRID_EXPORT_EXCEL_ICON = "xlsxfile";
var DATAGRID_EXPORT_SELECTED_ICON = "exportselected";
var DATAGRID_EXPORT_EXCEL_BUTTON_ICON = "export-excel-button";
export var DataProvider = Class.inherit({
    ctor: function(exportController, initialColumnWidthsByColumnIndex, selectedRowsOnly) {
        this._exportController = exportController;
        this._initialColumnWidthsByColumnIndex = initialColumnWidthsByColumnIndex;
        this._selectedRowsOnly = selectedRowsOnly
    },
    _getGroupValue: function(item) {
        var {
            key: key,
            data: data,
            rowType: rowType,
            groupIndex: groupIndex,
            summaryCells: summaryCells
        } = item;
        var groupColumn = this._options.groupColumns[groupIndex];
        var value = dataGridCore.getDisplayValue(groupColumn, groupColumn.deserializeValue ? groupColumn.deserializeValue(key[groupIndex]) : key[groupIndex], data, rowType);
        var result = groupColumn.caption + ": " + dataGridCore.formatValue(value, groupColumn);
        if (summaryCells && summaryCells[0] && summaryCells[0].length) {
            result += " " + dataGridCore.getGroupRowSummaryText(summaryCells[0], this._options.summaryTexts)
        }
        return result
    },
    _correctCellIndex: function(cellIndex) {
        return cellIndex
    },
    _initOptions: function() {
        var exportController = this._exportController;
        var groupColumns = exportController._columnsController.getGroupColumns();
        var excelWrapTextEnabled = exportController.option("export.excelWrapTextEnabled");
        this._options = {
            columns: exportController._getColumns(this._initialColumnWidthsByColumnIndex),
            groupColumns: groupColumns,
            items: this._selectedRowsOnly || exportController._selectionOnly ? exportController._getSelectedItems() : exportController._getAllItems(),
            getVisibleIndex: exportController._columnsController.getVisibleIndex.bind(exportController._columnsController),
            isHeadersVisible: exportController.option("showColumnHeaders"),
            summaryTexts: exportController.option("summary.texts"),
            customizeExportData: exportController.option("customizeExportData"),
            rtlEnabled: exportController.option("rtlEnabled"),
            wrapTextEnabled: isDefined(excelWrapTextEnabled) ? excelWrapTextEnabled : !!exportController.option("wordWrapEnabled"),
            customizeExcelCell: exportController.option("export.customizeExcelCell")
        }
    },
    hasCustomizeExcelCell: function() {
        return isDefined(this._options.customizeExcelCell)
    },
    customizeExcelCell: function(e, cellSourceData) {
        if (this._options.customizeExcelCell) {
            e.gridCell = cellSourceData;
            if (isDefined(this._exportController) && isDefined(this._exportController.component)) {
                e.component = this._exportController.component
            }
            this._options.customizeExcelCell(e)
        }
    },
    getHeaderStyles: () => [{
        bold: true,
        alignment: "center",
        wrapText: true
    }, {
        bold: true,
        alignment: "left",
        wrapText: true
    }, {
        bold: true,
        alignment: "right",
        wrapText: true
    }],
    getGroupRowStyle() {
        return {
            bold: true,
            wrapText: false,
            alignment: getDefaultAlignment(this._options.rtlEnabled)
        }
    },
    getColumnStyles() {
        var wrapTextEnabled = this._options.wrapTextEnabled;
        var columnStyles = [];
        this.getColumns().forEach(column => {
            columnStyles.push({
                alignment: column.alignment || "left",
                format: column.format,
                wrapText: wrapTextEnabled,
                dataType: column.dataType
            })
        });
        return columnStyles
    },
    getStyles: function() {
        return [...this.getHeaderStyles(), ...this.getColumnStyles(), this.getGroupRowStyle()]
    },
    _getTotalCellStyleId: function(cellIndex) {
        var _this$getColumns$cell;
        var alignment = (null === (_this$getColumns$cell = this.getColumns()[cellIndex]) || void 0 === _this$getColumns$cell ? void 0 : _this$getColumns$cell.alignment) || "right";
        return this.getHeaderStyles().map(style => style.alignment).indexOf(alignment)
    },
    getStyleId: function(rowIndex, cellIndex) {
        if (rowIndex < this.getHeaderRowCount()) {
            return 0
        } else if (this.isTotalCell(rowIndex - this.getHeaderRowCount(), cellIndex)) {
            return this._getTotalCellStyleId(cellIndex)
        } else if (this.isGroupRow(rowIndex - this.getHeaderRowCount())) {
            return this.getHeaderStyles().length + this.getColumns().length
        } else {
            return cellIndex + this.getHeaderStyles().length
        }
    },
    getColumns: function(getColumnsByAllRows) {
        var {
            columns: columns
        } = this._options;
        return getColumnsByAllRows ? columns : columns[columns.length - 1]
    },
    getColumnsWidths: function() {
        var columns = this.getColumns();
        return isDefined(columns) ? columns.map(c => c.width) : void 0
    },
    getRowsCount: function() {
        return this._options.items.length + this.getHeaderRowCount()
    },
    getHeaderRowCount: function() {
        if (this.isHeadersVisible()) {
            return this._options.columns.length - 1
        }
        return 0
    },
    isGroupRow: function(rowIndex) {
        return rowIndex < this._options.items.length && "group" === this._options.items[rowIndex].rowType
    },
    getGroupLevel: function(rowIndex) {
        var item = this._options.items[rowIndex - this.getHeaderRowCount()];
        var groupIndex = item && item.groupIndex;
        if (item && "totalFooter" === item.rowType) {
            return 0
        }
        return isDefined(groupIndex) ? groupIndex : this._options.groupColumns.length
    },
    getCellType: function(rowIndex, cellIndex) {
        var columns = this.getColumns();
        if (rowIndex < this.getHeaderRowCount()) {
            return "string"
        } else {
            rowIndex -= this.getHeaderRowCount()
        }
        if (cellIndex < columns.length) {
            var item = this._options.items.length && this._options.items[rowIndex];
            var column = columns[cellIndex];
            if (item && "data" === item.rowType) {
                if (isFinite(item.values[this._correctCellIndex(cellIndex)]) && !isDefined(column.customizeText)) {
                    return isDefined(column.lookup) ? column.lookup.dataType : column.dataType
                }
            }
            return "string"
        }
    },
    ready: function() {
        var that = this;
        that._initOptions();
        var options = that._options;
        return when(options.items).done((function(items) {
            options.customizeExportData && options.customizeExportData(that.getColumns(that.getHeaderRowCount() > 1), items);
            options.items = items
        })).fail((function() {
            options.items = []
        }))
    },
    _convertFromGridGroupSummaryItems: function(gridGroupSummaryItems) {
        if (isDefined(gridGroupSummaryItems) && gridGroupSummaryItems.length > 0) {
            return gridGroupSummaryItems.map((function(item) {
                return {
                    value: item.value,
                    name: item.name
                }
            }))
        }
    },
    getCellData: function(rowIndex, cellIndex, isExcelJS) {
        var value;
        var column;
        var result = {
            cellSourceData: {},
            value: value
        };
        var columns = this.getColumns();
        var correctedCellIndex = this._correctCellIndex(cellIndex);
        if (rowIndex < this.getHeaderRowCount()) {
            var columnsRow = this.getColumns(true)[rowIndex];
            column = columnsRow[cellIndex];
            result.cellSourceData.rowType = "header";
            result.cellSourceData.column = column && column.gridColumn;
            result.value = column && column.caption
        } else {
            rowIndex -= this.getHeaderRowCount();
            var item = this._options.items.length && this._options.items[rowIndex];
            if (item) {
                var itemValues = item.values;
                result.cellSourceData.rowType = item.rowType;
                result.cellSourceData.column = columns[cellIndex] && columns[cellIndex].gridColumn;
                switch (item.rowType) {
                    case "groupFooter":
                    case "totalFooter":
                        if (correctedCellIndex < itemValues.length) {
                            value = itemValues[correctedCellIndex];
                            if (isDefined(value)) {
                                result.cellSourceData.value = value.value;
                                result.cellSourceData.totalSummaryItemName = value.name;
                                result.value = dataGridCore.getSummaryText(value, this._options.summaryTexts)
                            } else {
                                result.cellSourceData.value = void 0
                            }
                        }
                        break;
                    case "group":
                        result.cellSourceData.groupIndex = item.groupIndex;
                        if (cellIndex < 1) {
                            result.cellSourceData.column = this._options.groupColumns[item.groupIndex];
                            result.cellSourceData.value = item.key[item.groupIndex];
                            result.cellSourceData.groupSummaryItems = this._convertFromGridGroupSummaryItems(item.summaryCells[0]);
                            result.value = this._getGroupValue(item)
                        } else {
                            var summaryItems = item.values[correctedCellIndex];
                            if (Array.isArray(summaryItems)) {
                                result.cellSourceData.groupSummaryItems = this._convertFromGridGroupSummaryItems(summaryItems);
                                value = "";
                                for (var i = 0; i < summaryItems.length; i++) {
                                    value += (i > 0 ? isExcelJS ? "\n" : " \n " : "") + dataGridCore.getSummaryText(summaryItems[i], this._options.summaryTexts)
                                }
                                result.value = value
                            } else {
                                result.cellSourceData.value = void 0
                            }
                        }
                        break;
                    default:
                        column = columns[cellIndex];
                        if (column) {
                            var _value = itemValues[correctedCellIndex];
                            var displayValue = dataGridCore.getDisplayValue(column, _value, item.data, item.rowType);
                            if (!isFinite(displayValue) || isDefined(column.customizeText)) {
                                if (isExcelJS && isDefined(column.customizeText) && column.customizeText === this._exportController._columnsController.getCustomizeTextByDataType("boolean")) {
                                    result.value = displayValue
                                } else {
                                    result.value = dataGridCore.formatValue(displayValue, column)
                                }
                            } else {
                                result.value = displayValue
                            }
                            result.cellSourceData.value = _value
                        }
                        result.cellSourceData.data = item.data
                }
            }
        }
        return result
    },
    isHeadersVisible: function() {
        return this._options.isHeadersVisible
    },
    isTotalCell: function(rowIndex, cellIndex) {
        var items = this._options.items;
        var item = items[rowIndex];
        var correctCellIndex = this._correctCellIndex(cellIndex);
        var isSummaryAlignByColumn = item.summaryCells && item.summaryCells[correctCellIndex] && item.summaryCells[correctCellIndex].length > 0 && item.summaryCells[correctCellIndex][0].alignByColumn;
        return item && "groupFooter" === item.rowType || "totalFooter" === item.rowType || isSummaryAlignByColumn
    },
    getCellMerging: function(rowIndex, cellIndex) {
        var columns = this._options.columns;
        var column = columns[rowIndex] && columns[rowIndex][cellIndex];
        return column ? {
            colspan: (column.exportColspan || 1) - 1,
            rowspan: (column.rowspan || 1) - 1
        } : {
            colspan: 0,
            rowspan: 0
        }
    },
    getFrozenArea: function() {
        return {
            x: 0,
            y: this.getHeaderRowCount()
        }
    }
});
export var ExportController = dataGridCore.ViewController.inherit({}).include(exportMixin).inherit({
    _getEmptyCell: function() {
        return {
            caption: "",
            colspan: 1,
            rowspan: 1
        }
    },
    _updateColumnWidth: function(column, width) {
        column.width = width
    },
    _getColumns: function(initialColumnWidthsByColumnIndex) {
        var result = [];
        var i;
        var columns;
        var columnsController = this._columnsController;
        var rowCount = columnsController.getRowCount();
        for (i = 0; i <= rowCount; i++) {
            var currentHeaderRow = [];
            columns = columnsController.getVisibleColumns(i, true);
            var columnWidthsByColumnIndex = void 0;
            if (i === rowCount) {
                if (this._updateLockCount) {
                    columnWidthsByColumnIndex = initialColumnWidthsByColumnIndex
                } else {
                    var columnWidths = this._getColumnWidths(this._headersView, this._rowsView);
                    if (columnWidths && columnWidths.length) {
                        columnWidthsByColumnIndex = {};
                        for (var _i = 0; _i < columns.length; _i++) {
                            columnWidthsByColumnIndex[columns[_i].index] = columnWidths[_i]
                        }
                    }
                }
            }
            for (var j = 0; j < columns.length; j++) {
                var column = extend({}, columns[j], {
                    dataType: "datetime" === columns[j].dataType ? "date" : columns[j].dataType,
                    gridColumn: columns[j]
                });
                if (this._needColumnExporting(column)) {
                    var currentColspan = this._calculateExportColspan(column);
                    if (isDefined(currentColspan)) {
                        column.exportColspan = currentColspan
                    }
                    if (columnWidthsByColumnIndex) {
                        this._updateColumnWidth(column, columnWidthsByColumnIndex[column.index])
                    }
                    currentHeaderRow.push(column)
                }
            }
            result.push(currentHeaderRow)
        }
        columns = result[rowCount];
        result = this._prepareItems(result.slice(0, -1));
        result.push(columns);
        return result
    },
    _calculateExportColspan: function(column) {
        if (!column.isBand) {
            return
        }
        var childColumns = this._columnsController.getChildrenByBandColumn(column.index, true);
        if (!isDefined(childColumns)) {
            return
        }
        return childColumns.reduce((result, childColumn) => {
            if (this._needColumnExporting(childColumn)) {
                return result + (this._calculateExportColspan(childColumn) || 1)
            } else {
                return result
            }
        }, 0)
    },
    _needColumnExporting: function(column) {
        return !column.command && (column.allowExporting || void 0 === column.allowExporting)
    },
    _getFooterSummaryItems: function(summaryCells, isTotal) {
        var result = [];
        var estimatedItemsCount = 1;
        var i = 0;
        do {
            var values = [];
            for (var j = 0; j < summaryCells.length; j++) {
                var summaryCell = summaryCells[j];
                var itemsLength = summaryCell.length;
                if (estimatedItemsCount < itemsLength) {
                    estimatedItemsCount = itemsLength
                }
                values.push(summaryCell[i])
            }
            result.push({
                values: values,
                rowType: isTotal ? "totalFooter" : "groupFooter"
            })
        } while (i++ < estimatedItemsCount - 1);
        return result
    },
    _hasSummaryGroupFooters: function() {
        var groupItems = this.option("summary.groupItems");
        if (isDefined(groupItems)) {
            for (var i = 0; i < groupItems.length; i++) {
                if (groupItems[i].showInGroupFooter) {
                    return true
                }
            }
        }
        return false
    },
    _getItemsWithSummaryGroupFooters: function(sourceItems) {
        var result = [];
        var beforeGroupFooterItems = [];
        var groupFooterItems = [];
        for (var i = 0; i < sourceItems.length; i++) {
            var item = sourceItems[i];
            if ("groupFooter" === item.rowType) {
                groupFooterItems = this._getFooterSummaryItems(item.summaryCells);
                result = result.concat(beforeGroupFooterItems, groupFooterItems);
                beforeGroupFooterItems = []
            } else {
                beforeGroupFooterItems.push(item)
            }
        }
        return result.length ? result : beforeGroupFooterItems
    },
    _updateGroupValuesWithSummaryByColumn: function(sourceItems) {
        var summaryValues = [];
        for (var i = 0; i < sourceItems.length; i++) {
            var item = sourceItems[i];
            var summaryCells = item.summaryCells;
            if ("group" === item.rowType && summaryCells && summaryCells.length > 1) {
                var groupColumnCount = item.values.length;
                for (var j = 1; j < summaryCells.length; j++) {
                    for (var k = 0; k < summaryCells[j].length; k++) {
                        var summaryItem = summaryCells[j][k];
                        if (summaryItem && summaryItem.alignByColumn) {
                            if (!Array.isArray(summaryValues[j - groupColumnCount])) {
                                summaryValues[j - groupColumnCount] = []
                            }
                            summaryValues[j - groupColumnCount].push(summaryItem)
                        }
                    }
                }
                if (summaryValues.length > 0) {
                    merge(item.values, summaryValues);
                    summaryValues = []
                }
            }
        }
    },
    _processUnExportedItems: function(items) {
        var columns = this._columnsController.getVisibleColumns(null, true);
        var groupColumns = this._columnsController.getGroupColumns();
        var values;
        var summaryCells;
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var isDetailExpandColumn = false;
            values = [];
            summaryCells = [];
            for (var j = 0; j < columns.length; j++) {
                var column = columns[j];
                isDetailExpandColumn = isDetailExpandColumn || "detailExpand" === column.type;
                if (this._needColumnExporting(column)) {
                    if (item.values) {
                        if ("group" === item.rowType && !values.length) {
                            values.push(item.key[item.groupIndex])
                        } else {
                            values.push(item.values[j])
                        }
                    }
                    if (item.summaryCells) {
                        if ("group" === item.rowType && !summaryCells.length) {
                            var index = j - groupColumns.length + item.groupIndex;
                            summaryCells.push(item.summaryCells[isDetailExpandColumn ? index : index + 1])
                        } else {
                            summaryCells.push(item.summaryCells[j])
                        }
                    }
                }
            }
            if (values.length) {
                item.values = values
            }
            if (summaryCells.length) {
                item.summaryCells = summaryCells
            }
        }
    },
    _getAllItems: function(data) {
        var that = this;
        var d = new Deferred;
        var dataController = this.getController("data");
        var footerItems = dataController.footerItems();
        var totalItem = footerItems.length && footerItems[0];
        var summaryTotalItems = that.option("summary.totalItems");
        var summaryCells;
        when(data).done((function(data) {
            dataController.loadAll(data).done((function(sourceItems, totalAggregates) {
                that._updateGroupValuesWithSummaryByColumn(sourceItems);
                if (that._hasSummaryGroupFooters()) {
                    sourceItems = that._getItemsWithSummaryGroupFooters(sourceItems)
                }
                summaryCells = totalItem && totalItem.summaryCells;
                if (isDefined(totalAggregates) && summaryTotalItems) {
                    summaryCells = that._getSummaryCells(summaryTotalItems, totalAggregates)
                }
                var summaryItems = totalItem && that._getFooterSummaryItems(summaryCells, true);
                if (summaryItems) {
                    sourceItems = sourceItems.concat(summaryItems)
                }
                that._processUnExportedItems(sourceItems);
                d.resolve(sourceItems)
            })).fail(d.reject)
        })).fail(d.reject);
        return d
    },
    _getSummaryCells: function(summaryTotalItems, totalAggregates) {
        var dataController = this.getController("data");
        var columnsController = dataController._columnsController;
        return dataController._calculateSummaryCells(summaryTotalItems, totalAggregates, columnsController.getVisibleColumns(null, true), (function(summaryItem, column) {
            return dataController._isDataColumn(column) ? column.index : -1
        }))
    },
    _getSelectedItems: function() {
        var selectionController = this.getController("selection");
        var selectedRowData = selectionController.getSelectedRowsData();
        return this._getAllItems(selectedRowData)
    },
    _getColumnWidths: function(headersView, rowsView) {
        return headersView && headersView.isVisible() ? headersView.getColumnWidths() : rowsView.getColumnWidths()
    },
    init: function() {
        this._columnsController = this.getController("columns");
        this._rowsView = this.getView("rowsView");
        this._headersView = this.getView("columnHeadersView");
        this.createAction("onExporting", {
            excludeValidators: ["disabled", "readOnly"]
        });
        this.createAction("onExported", {
            excludeValidators: ["disabled", "readOnly"]
        });
        this.createAction("onFileSaving", {
            excludeValidators: ["disabled", "readOnly"]
        })
    },
    callbackNames: function() {
        return ["selectionOnlyChanged"]
    },
    getExportFormat: function() {
        return ["EXCEL"]
    },
    getDataProvider: function(selectedRowsOnly) {
        var columnWidths = this._getColumnWidths(this._headersView, this._rowsView);
        var initialColumnWidthsByColumnIndex;
        if (columnWidths && columnWidths.length) {
            initialColumnWidthsByColumnIndex = {};
            var columnsLastRowVisibleColumns = this._columnsController.getVisibleColumns(this._columnsController.getRowCount(), true);
            for (var i = 0; i < columnsLastRowVisibleColumns.length; i++) {
                initialColumnWidthsByColumnIndex[columnsLastRowVisibleColumns[i].index] = columnWidths[i]
            }
        }
        return new DataProvider(this, initialColumnWidthsByColumnIndex, selectedRowsOnly)
    },
    exportToExcel: function(selectionOnly) {
        this._selectionOnly = selectionOnly;
        clientExport(this.component.getDataProvider(), {
            fileName: this.option("export.fileName"),
            proxyUrl: this.option("export.proxyUrl"),
            format: "EXCEL",
            autoFilterEnabled: !!this.option("export.excelFilterEnabled"),
            rtlEnabled: this.option("rtlEnabled"),
            ignoreErrors: this.option("export.ignoreExcelErrors"),
            exportingAction: this.getAction("onExporting"),
            exportedAction: this.getAction("onExported"),
            fileSavingAction: this.getAction("onFileSaving")
        }, excel.getData)
    },
    publicMethods: function() {
        return ["getDataProvider", "getExportFormat", "exportToExcel"]
    },
    selectionOnly: function(value) {
        if (isDefined(value)) {
            this._isSelectedRows = value;
            this.selectionOnlyChanged.fire()
        } else {
            return this._isSelectedRows
        }
    }
});
dataGridCore.registerModule("export", {
    defaultOptions: function() {
        return {
            export: {
                enabled: false,
                fileName: "DataGrid",
                excelFilterEnabled: false,
                excelWrapTextEnabled: void 0,
                proxyUrl: void 0,
                allowExportSelectedData: false,
                ignoreExcelErrors: true,
                texts: {
                    exportTo: messageLocalization.format("dxDataGrid-exportTo"),
                    exportAll: messageLocalization.format("dxDataGrid-exportAll"),
                    exportSelectedRows: messageLocalization.format("dxDataGrid-exportSelectedRows")
                }
            }
        }
    },
    controllers: {
        export: ExportController
    },
    extenders: {
        controllers: {
            editing: {
                callbackNames: function() {
                    var callbackList = this.callBase();
                    return isDefined(callbackList) ? callbackList.push("editingChanged") : ["editingChanged"]
                },
                _updateEditButtons: function() {
                    this.callBase();
                    this.editingChanged.fire(this.hasChanges())
                }
            }
        },
        views: {
            headerPanel: {
                _getToolbarItems: function() {
                    var items = this.callBase();
                    var exportButton = this._getExportToolbarButton();
                    if (exportButton) {
                        items.push(exportButton);
                        this._correctItemsPosition(items)
                    }
                    return items
                },
                _getExportToolbarButton: function() {
                    var items = this._getExportToolbarItems();
                    if (0 === items.length) {
                        return null
                    }
                    var toolbarButtonOptions = {
                        name: DATAGRID_EXPORT_TOOLBAR_BUTTON_NAME,
                        location: "after",
                        locateInMenu: "auto",
                        sortIndex: 30,
                        options: {
                            items: items
                        }
                    };
                    if (1 === items.length) {
                        var widgetOptions = _extends({}, items[0], {
                            hint: items[0].text,
                            elementAttr: {
                                class: DATAGRID_EXPORT_BUTTON_CLASS
                            }
                        });
                        toolbarButtonOptions.widget = "dxButton";
                        toolbarButtonOptions.showText = "inMenu";
                        toolbarButtonOptions.options = widgetOptions
                    } else {
                        var _widgetOptions = {
                            icon: DATAGRID_EXPORT_ICON,
                            displayExpr: "text",
                            items: items,
                            hint: this.option("export.texts.exportTo"),
                            elementAttr: {
                                class: DATAGRID_EXPORT_BUTTON_CLASS
                            },
                            dropDownOptions: {
                                wrapperAttr: {
                                    class: DATAGRID_EXPORT_MENU_CLASS
                                },
                                width: "auto"
                            }
                        };
                        toolbarButtonOptions.options = _widgetOptions;
                        toolbarButtonOptions.widget = "dxDropDownButton";
                        toolbarButtonOptions.menuItemTemplate = (_data, _index, container) => {
                            this._createComponent($(container), List, {
                                items: items
                            })
                        }
                    }
                    return toolbarButtonOptions
                },
                _getExportToolbarItems: function() {
                    var exportOptions = this.option("export");
                    var texts = this.option("export.texts");
                    var items = [];
                    if (exportOptions.enabled) {
                        items.push({
                            text: texts.exportAll,
                            icon: DATAGRID_EXPORT_EXCEL_ICON,
                            onClick: () => {
                                this._exportController.exportToExcel()
                            }
                        });
                        if (exportOptions.allowExportSelectedData) {
                            items.push({
                                text: texts.exportSelectedRows,
                                icon: DATAGRID_EXPORT_SELECTED_ICON,
                                onClick: () => {
                                    this._exportController.exportToExcel(true)
                                }
                            })
                        }
                    }
                    return items
                },
                _correctItemsPosition: function(items) {
                    items.sort((function(itemA, itemB) {
                        return itemA.sortIndex - itemB.sortIndex
                    }))
                },
                _isExportButtonVisible: function() {
                    return this.option("export.enabled")
                },
                _getButtonOptions: function(allowExportSelected) {
                    var that = this;
                    var texts = that.option("export.texts");
                    var options;
                    if (allowExportSelected) {
                        options = {
                            hint: texts.exportTo,
                            icon: DATAGRID_EXPORT_ICON
                        }
                    } else {
                        options = {
                            hint: texts.exportAll,
                            icon: DATAGRID_EXPORT_EXCEL_BUTTON_ICON,
                            onClick: function() {
                                that._exportController.exportToExcel()
                            }
                        }
                    }
                    return options
                },
                optionChanged: function(args) {
                    this.callBase(args);
                    if ("export" === args.name) {
                        args.handled = true;
                        this._invalidate()
                    }
                },
                init: function() {
                    var that = this;
                    this.callBase();
                    this._exportController = this.getController("export");
                    this._editingController = this.getController("editing");
                    this._editingController.editingChanged.add((function(hasChanges) {
                        that.setToolbarItemDisabled("exportButton", hasChanges)
                    }))
                },
                isVisible: function() {
                    return this.callBase() || this._isExportButtonVisible()
                }
            }
        }
    }
});
