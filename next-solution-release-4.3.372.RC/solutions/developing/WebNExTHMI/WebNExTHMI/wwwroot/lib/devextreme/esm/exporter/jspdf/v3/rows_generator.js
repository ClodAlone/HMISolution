/**
 * DevExtreme (esm/exporter/jspdf/v3/rows_generator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../../core/utils/type";

function generateRowsInfo(dataProvider, dataGrid, headerBackgroundColor) {
    var result = [];
    var rowsCount = dataProvider.getRowsCount();
    var wordWrapEnabled = !!dataGrid.option("wordWrapEnabled");
    var columns = dataProvider.getColumns();
    for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++) {
        var rowType = dataProvider.getCellData(rowIndex, 0, true).cellSourceData.rowType;
        var indentLevel = "header" !== rowType ? dataProvider.getGroupLevel(rowIndex) : 0;
        var previousRow = result[rowIndex - 1];
        if ("groupFooter" === rowType && "groupFooter" === (null === previousRow || void 0 === previousRow ? void 0 : previousRow.rowType)) {
            indentLevel = previousRow.indentLevel - 1
        }
        result.push({
            rowType: rowType,
            indentLevel: indentLevel,
            cells: generateRowCells({
                dataProvider: dataProvider,
                rowIndex: rowIndex,
                wordWrapEnabled: wordWrapEnabled,
                columns: columns,
                rowType: rowType,
                backgroundColor: "header" === rowType ? headerBackgroundColor : void 0
            }),
            rowIndex: rowIndex
        })
    }
    return result
}

function generateRowCells(_ref) {
    var {
        dataProvider: dataProvider,
        rowIndex: rowIndex,
        wordWrapEnabled: wordWrapEnabled,
        columns: columns,
        rowType: rowType,
        backgroundColor: backgroundColor
    } = _ref;
    var result = [];
    for (var cellIndex = 0; cellIndex < columns.length; cellIndex++) {
        var _cellData$value, _columns$cellIndex$al;
        var cellData = dataProvider.getCellData(rowIndex, cellIndex, true);
        var cellInfo = {
            gridCell: cellData.cellSourceData,
            pdfCell: {
                text: null === (_cellData$value = cellData.value) || void 0 === _cellData$value ? void 0 : _cellData$value.toString(),
                verticalAlign: "middle",
                horizontalAlign: null !== (_columns$cellIndex$al = columns[cellIndex].alignment) && void 0 !== _columns$cellIndex$al ? _columns$cellIndex$al : "left",
                wordWrapEnabled: wordWrapEnabled,
                backgroundColor: backgroundColor,
                padding: 0,
                _rect: {}
            }
        };
        if ("header" === rowType) {
            var cellMerging = dataProvider.getCellMerging(rowIndex, cellIndex);
            if (cellMerging && cellMerging.rowspan > 0) {
                cellInfo.rowSpan = cellMerging.rowspan
            }
            if (cellMerging && cellMerging.colspan > 0) {
                cellInfo.colSpan = cellMerging.colspan
            }
        } else if ("group" === rowType) {
            cellInfo.pdfCell.drawLeftBorder = 0 === cellIndex;
            cellInfo.pdfCell.drawRightBorder = cellIndex === columns.length - 1;
            if (cellIndex > 0) {
                var isEmptyCellsExceptFirst = result.slice(1).reduce((accumulate, cellInfo) => accumulate && !isDefined(cellInfo.pdfCell.text), true);
                if (!isDefined(cellInfo.pdfCell.text) && isEmptyCellsExceptFirst) {
                    result[0].pdfCell.drawRightBorder = true;
                    for (var i = 0; i < result.length; i++) {
                        result[i].colSpan = result.length
                    }
                    cellInfo.colSpan = result.length
                }
            }
        }
        result.push(cellInfo)
    }
    return result
}
export {
    generateRowsInfo
};
