/**
 * DevExtreme (esm/viz/tree_map/common.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    patchFontOptions as _patchFontOptions
} from "../core/utils";
export function buildRectAppearance(option) {
    var border = option.border || {};
    return {
        fill: option.color,
        opacity: option.opacity,
        stroke: border.color,
        "stroke-width": border.width,
        "stroke-opacity": border.opacity,
        hatching: option.hatching
    }
}
export function buildTextAppearance(options, filter) {
    return {
        attr: {
            filter: filter
        },
        css: _patchFontOptions(options.font)
    }
}
