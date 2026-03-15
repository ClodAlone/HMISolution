/**
 * DevExtreme (esm/viz/tree_map/colorizing.range.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    addColorizer,
    createColorCodeGetter as _createColorCodeGetter
} from "./colorizing";

function getPaletteIndex(value, items) {
    var start = 0;
    var end = items.length - 1;
    var index = -1;
    var middle;
    if (items[start] <= value && value <= items[end]) {
        if (value === items[end]) {
            index = end - 1
        } else {
            while (end - start > 1) {
                middle = start + end >> 1;
                if (value < items[middle]) {
                    end = middle
                } else {
                    start = middle
                }
            }
            index = start
        }
    }
    return index
}

function rangeColorizer(options, themeManager) {
    var range = options.range || [];
    var palette = themeManager.createDiscretePalette(options.palette, range.length - 1);
    var getValue = _createColorCodeGetter(options);
    return function(node) {
        return palette.getColor(getPaletteIndex(getValue(node), range))
    }
}
addColorizer("range", rangeColorizer);
export default rangeColorizer;
