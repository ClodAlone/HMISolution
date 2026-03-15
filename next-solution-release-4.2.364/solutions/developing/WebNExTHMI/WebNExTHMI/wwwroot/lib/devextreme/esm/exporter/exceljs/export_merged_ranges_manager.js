/**
 * DevExtreme (esm/exporter/exceljs/export_merged_ranges_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
class MergedRangesManager {
    constructor(dataProvider, helpers, mergeRowFieldValues, mergeColumnFieldValues) {
        this.mergedCells = [];
        this.mergedRanges = [];
        this.dataProvider = dataProvider;
        this.helpers = helpers;
        this.mergeRowFieldValues = mergeRowFieldValues;
        this.mergeColumnFieldValues = mergeColumnFieldValues
    }
    updateMergedRanges(excelCell, rowIndex, cellIndex) {
        if (this.helpers._isHeaderCell(this.dataProvider, rowIndex, cellIndex)) {
            if (!this.isCellInMergedRanges(rowIndex, cellIndex)) {
                var {
                    rowspan: rowspan,
                    colspan: colspan
                } = this.dataProvider.getCellMerging(rowIndex, cellIndex);
                var isMasterCellOfMergedRange = colspan || rowspan;
                if (isMasterCellOfMergedRange) {
                    var allowToMergeRange = this.helpers._allowToMergeRange(this.dataProvider, rowIndex, cellIndex, rowspan, colspan, this.mergeRowFieldValues, this.mergeColumnFieldValues);
                    this.updateMergedCells(excelCell, rowIndex, cellIndex, rowspan, colspan, allowToMergeRange);
                    if (allowToMergeRange) {
                        this.mergedRanges.push(_extends({
                            masterCell: excelCell
                        }, {
                            rowspan: rowspan,
                            colspan: colspan
                        }))
                    }
                }
            }
        }
    }
    isCellInMergedRanges(rowIndex, cellIndex) {
        return this.mergedCells[rowIndex] && this.mergedCells[rowIndex][cellIndex]
    }
    findMergedCellInfo(rowIndex, cellIndex) {
        if (this.helpers._isHeaderCell(this.dataProvider, rowIndex, cellIndex)) {
            if (this.isCellInMergedRanges(rowIndex, cellIndex)) {
                return this.mergedCells[rowIndex][cellIndex]
            }
        }
    }
    updateMergedCells(excelCell, rowIndex, cellIndex, rowspan, colspan, allowToMergeRange) {
        for (var i = rowIndex; i <= rowIndex + rowspan; i++) {
            for (var j = cellIndex; j <= cellIndex + colspan; j++) {
                if (!this.mergedCells[i]) {
                    this.mergedCells[i] = []
                }
                this.mergedCells[i][j] = {
                    masterCell: excelCell,
                    unmerged: !allowToMergeRange
                }
            }
        }
    }
    applyMergedRages(worksheet) {
        this.mergedRanges.forEach(range => {
            var startRowIndex = range.masterCell.fullAddress.row;
            var startColumnIndex = range.masterCell.fullAddress.col;
            var endRowIndex = startRowIndex + range.rowspan;
            var endColumnIndex = startColumnIndex + range.colspan;
            worksheet.mergeCells(startRowIndex, startColumnIndex, endRowIndex, endColumnIndex)
        })
    }
}
export {
    MergedRangesManager
};
