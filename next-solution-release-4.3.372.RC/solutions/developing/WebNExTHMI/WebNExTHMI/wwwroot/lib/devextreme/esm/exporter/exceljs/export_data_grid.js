/**
 * DevExtreme (esm/exporter/exceljs/export_data_grid.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined,
    isObject
} from "../../core/utils/type";
import {
    Export
} from "./export";
import errors from "../../core/errors";
var helpers = {
    _trySetAutoFilter(dataProvider, worksheet, cellRange, autoFilterEnabled) {
        if (autoFilterEnabled) {
            if (!isDefined(worksheet.autoFilter) && dataProvider.getRowsCount() > 0) {
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
    _trySetFont(excelCell, bold) {
        if (isDefined(bold)) {
            excelCell.font = excelCell.font || {};
            excelCell.font.bold = bold
        }
    },
    _getWorksheetFrozenState: (dataProvider, cellRange) => ({
        state: "frozen",
        ySplit: cellRange.from.row + dataProvider.getFrozenArea().y - 1
    }),
    _trySetOutlineLevel(dataProvider, row, rowIndex) {
        if (rowIndex >= dataProvider.getHeaderRowCount()) {
            row.outlineLevel = dataProvider.getGroupLevel(rowIndex)
        }
    },
    _getCustomizeCellOptions(excelCell, gridCell) {
        var options = {
            excelCell: excelCell,
            gridCell: gridCell
        };
        Object.defineProperty(options, "cell", {
            get() {
                errors.log("W0003", "CustomizeCell handler argument", "cell", "20.1", "Use the 'excelCell' field instead");
                return excelCell
            }
        });
        return options
    },
    _isFrozenZone: dataProvider => dataProvider.getHeaderRowCount() > 0,
    _isHeaderCell: (dataProvider, rowIndex) => rowIndex < dataProvider.getHeaderRowCount(),
    _allowToMergeRange: () => true,
    _getLoadPanelTargetElement: component => component.getView("rowsView").element(),
    _getLoadPanelContainer: component => component.getView("rowsView").element().parent()
};

function exportDataGrid(options) {
    return Export.export(_getFullOptions(options), helpers)
}

function _getFullOptions(options) {
    if (!(isDefined(options) && isObject(options))) {
        throw Error('The "exportDataGrid" method requires a configuration object.')
    }
    if (!(isDefined(options.component) && isObject(options.component) && "dxDataGrid" === options.component.NAME)) {
        throw Error('The "component" field must contain a DataGrid instance.')
    }
    if (!isDefined(options.selectedRowsOnly)) {
        options.selectedRowsOnly = false
    }
    if (!isDefined(options.autoFilterEnabled)) {
        options.autoFilterEnabled = false
    }
    return Export.getFullOptions(options)
}
export {
    exportDataGrid
};
