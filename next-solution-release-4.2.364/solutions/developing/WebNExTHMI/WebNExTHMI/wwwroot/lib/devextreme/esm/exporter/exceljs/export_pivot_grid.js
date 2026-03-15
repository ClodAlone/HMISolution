/**
 * DevExtreme (esm/exporter/exceljs/export_pivot_grid.js)
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
import {
    noop
} from "../../core/utils/common";
var helpers = {
    _getWorksheetFrozenState: (dataProvider, cellRange) => ({
        state: "frozen",
        xSplit: cellRange.from.column + dataProvider.getFrozenArea().x - 1,
        ySplit: cellRange.from.row + dataProvider.getFrozenArea().y - 1
    }),
    _getCustomizeCellOptions: (excelCell, pivotCell) => ({
        excelCell: excelCell,
        pivotCell: pivotCell
    }),
    _isFrozenZone: () => true,
    _isHeaderCell: (dataProvider, rowIndex, cellIndex) => rowIndex < dataProvider.getColumnAreaRowCount() || cellIndex < dataProvider.getRowAreaColCount(),
    _allowToMergeRange: (dataProvider, rowIndex, cellIndex, rowspan, colspan, mergeRowFieldValues, mergeColumnFieldValues) => !(dataProvider.isColumnAreaCell(rowIndex, cellIndex) && !mergeColumnFieldValues && !!colspan || dataProvider.isRowAreaCell(rowIndex, cellIndex) && !mergeRowFieldValues && !!rowspan),
    _getLoadPanelTargetElement: component => component._dataArea.groupElement(),
    _getLoadPanelContainer: component => component.$element(),
    _trySetAutoFilter: noop,
    _trySetFont: noop,
    _trySetOutlineLevel: noop
};

function exportPivotGrid(options) {
    return Export.export(_getFullOptions(options), helpers)
}

function _getFullOptions(options) {
    if (!(isDefined(options) && isObject(options))) {
        throw Error('The "exportPivotGrid" method requires a configuration object.')
    }
    if (!(isDefined(options.component) && isObject(options.component) && "dxPivotGrid" === options.component.NAME)) {
        throw Error('The "component" field must contain a PivotGrid instance.')
    }
    if (!isDefined(options.mergeRowFieldValues)) {
        options.mergeRowFieldValues = true
    }
    if (!isDefined(options.mergeColumnFieldValues)) {
        options.mergeColumnFieldValues = true
    }
    return Export.getFullOptions(options)
}
export {
    exportPivotGrid
};
