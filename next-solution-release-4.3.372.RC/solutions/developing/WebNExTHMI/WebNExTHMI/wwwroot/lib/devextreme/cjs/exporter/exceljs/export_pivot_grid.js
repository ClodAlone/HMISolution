/**
 * DevExtreme (cjs/exporter/exceljs/export_pivot_grid.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.exportPivotGrid = exportPivotGrid;
var _type = require("../../core/utils/type");
var _export = require("./export");
var _common = require("../../core/utils/common");
var helpers = {
    _getWorksheetFrozenState: function(dataProvider, cellRange) {
        return {
            state: "frozen",
            xSplit: cellRange.from.column + dataProvider.getFrozenArea().x - 1,
            ySplit: cellRange.from.row + dataProvider.getFrozenArea().y - 1
        }
    },
    _getCustomizeCellOptions: function(excelCell, pivotCell) {
        return {
            excelCell: excelCell,
            pivotCell: pivotCell
        }
    },
    _isFrozenZone: function() {
        return true
    },
    _isHeaderCell: function(dataProvider, rowIndex, cellIndex) {
        return rowIndex < dataProvider.getColumnAreaRowCount() || cellIndex < dataProvider.getRowAreaColCount()
    },
    _allowToMergeRange: function(dataProvider, rowIndex, cellIndex, rowspan, colspan, mergeRowFieldValues, mergeColumnFieldValues) {
        return !(dataProvider.isColumnAreaCell(rowIndex, cellIndex) && !mergeColumnFieldValues && !!colspan || dataProvider.isRowAreaCell(rowIndex, cellIndex) && !mergeRowFieldValues && !!rowspan)
    },
    _getLoadPanelTargetElement: function(component) {
        return component._dataArea.groupElement()
    },
    _getLoadPanelContainer: function(component) {
        return component.$element()
    },
    _trySetAutoFilter: _common.noop,
    _trySetFont: _common.noop,
    _trySetOutlineLevel: _common.noop
};

function exportPivotGrid(options) {
    return _export.Export.export(_getFullOptions(options), helpers)
}

function _getFullOptions(options) {
    if (!((0, _type.isDefined)(options) && (0, _type.isObject)(options))) {
        throw Error('The "exportPivotGrid" method requires a configuration object.')
    }
    if (!((0, _type.isDefined)(options.component) && (0, _type.isObject)(options.component) && "dxPivotGrid" === options.component.NAME)) {
        throw Error('The "component" field must contain a PivotGrid instance.')
    }
    if (!(0, _type.isDefined)(options.mergeRowFieldValues)) {
        options.mergeRowFieldValues = true
    }
    if (!(0, _type.isDefined)(options.mergeColumnFieldValues)) {
        options.mergeColumnFieldValues = true
    }
    return _export.Export.getFullOptions(options)
}
