/**
 * DevExtreme (esm/exporter/jspdf/v3/normalizeOptions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isNumeric
} from "../../../core/utils/type";

function normalizeBoundaryValue(value) {
    var _value$top, _value$right, _value$bottom, _value$left;
    if (isNumeric(value)) {
        return {
            top: value,
            right: value,
            bottom: value,
            left: value
        }
    }
    return {
        top: null !== (_value$top = null === value || void 0 === value ? void 0 : value.top) && void 0 !== _value$top ? _value$top : 0,
        right: null !== (_value$right = null === value || void 0 === value ? void 0 : value.right) && void 0 !== _value$right ? _value$right : 0,
        bottom: null !== (_value$bottom = null === value || void 0 === value ? void 0 : value.bottom) && void 0 !== _value$bottom ? _value$bottom : 0,
        left: null !== (_value$left = null === value || void 0 === value ? void 0 : value.left) && void 0 !== _value$left ? _value$left : 0
    }
}

function normalizeRowsInfo(rowsInfo) {
    rowsInfo.forEach(row => {
        row.cells.forEach(_ref => {
            var {
                pdfCell: pdfCell
            } = _ref;
            pdfCell.padding = normalizeBoundaryValue(pdfCell.padding)
        })
    })
}
export {
    normalizeRowsInfo,
    normalizeBoundaryValue
};
