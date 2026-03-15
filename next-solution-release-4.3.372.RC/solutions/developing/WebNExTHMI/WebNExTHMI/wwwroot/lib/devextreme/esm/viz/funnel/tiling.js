/**
 * DevExtreme (esm/viz/funnel/tiling.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    normalizeEnum as _normalizeEnum
} from "../core/utils";
var algorithms = {};
var defaultAlgorithm;
export function getAlgorithm(name) {
    return algorithms[_normalizeEnum(name)] || defaultAlgorithm
}
export function addAlgorithm(name, callback, setDefault) {
    algorithms[name] = callback;
    if (setDefault) {
        defaultAlgorithm = algorithms[name]
    }
}
