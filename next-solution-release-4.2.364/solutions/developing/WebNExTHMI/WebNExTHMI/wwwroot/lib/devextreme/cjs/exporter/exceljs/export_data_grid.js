/**
 * DevExtreme (cjs/exporter/exceljs/export_data_grid.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.exportDataGrid = exportDataGrid;
var _type = require("../../core/utils/type");
var _export = require("./export");
var _errors = _interopRequireDefault(require("../../core/errors"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var helpers = {
    _trySetAutoFilter: function(dataProvider, worksheet, cellRange, autoFilterEnabled) {
        if (autoFilterEnabled) {
            if (!(0, _type.isDefined)(worksheet.autoFilter) && dataProvider.getRowsCount() > 0) {
                var dataRange = {
                    from: {
                        row: cellRange.from.row + dataProvider.getHeaderRowCount() - 1,
                        column: cellRange.from.column
                    },
                    to: cellRange.to
                };
                worksheet.autoFilter = dataRange
            }
        }
    },
    _trySetFont: function(excelCell, bold) {
        if ((0, _type.isDefined)(bold)) {
            excelCell.font = excelCell.font || {};
            excelCell.font.bold = bold
        }
    },
    _getWorksheetFrozenState: function(dataProvider, cellRange) {
        return {
            state: "frozen",
            ySplit: cellRange.from.row + dataProvider.getFrozenArea().y - 1
        }
    },
    _trySetOutlineLevel: function(dataProvider, row, rowIndex) {
        if (rowIndex >= dataProvider.getHeaderRowCount()) {
            row.outlineLevel = dataProvider.getGroupLevel(rowIndex)
        }
    },
    _getCustomizeCellOptions: function(excelCell, gridCell) {
        var options = {
            excelCell: excelCell,
            gridCell: gridCell
        };
        Object.defineProperty(options, "cell", {
            get: function() {
                _errors.default.log("W0003", "CustomizeCell handler argument", "cell", "20.1", "Use the 'excelCell' field instead");
                return excelCell
            }
        });
        return options
    },
    _isFrozenZone: function(dataProvider) {
        return dataProvider.getHeaderRowCount() > 0
    },
    _isHeaderCell: function(dataProvider, rowIndex) {
        return rowIndex < dataProvider.getHeaderRowCount()
    },
    _allowToMergeRange: function() {
        return true
    },
    _getLoadPanelTargetElement: function(component) {
        return component.getView("rowsView").element()
    },
    _getLoadPanelContainer: function(component) {
        return component.getView("rowsView").element().parent()
    }
};

function exportDataGrid(options) {
    return _export.Export.export(_getFullOptions(options), helpers)
}

function _getFullOptions(options) {
    if (!((0, _type.isDefined)(options) && (0, _type.isObject)(options))) {
        throw Error('The "exportDataGrid" method requires a configuration object.')
    }
    if (!((0, _type.isDefined)(options.component) && (0, _type.isObject)(options.component) && "dxDataGrid" === options.component.NAME)) {
        throw Error('The "component" field must contain a DataGrid instance.')
    }
    if (!(0, _type.isDefined)(options.selectedRowsOnly)) {
        options.selectedRowsOnly = false
    }
    if (!(0, _type.isDefined)(options.autoFilterEnabled)) {
        options.autoFilterEnabled = false
    }
    return _export.Export.getFullOptions(options)
}
