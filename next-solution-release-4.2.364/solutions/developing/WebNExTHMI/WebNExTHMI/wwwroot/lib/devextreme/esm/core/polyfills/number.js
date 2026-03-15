/**
 * DevExtreme (esm/core/polyfills/number.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    hasWindow,
    getWindow
} from "../../core/utils/window";
var number = hasWindow() ? getWindow().Number : Number;
number.isFinite = number.isFinite || function(value) {
    return "number" === typeof value && isFinite(value)
};
export default number;
