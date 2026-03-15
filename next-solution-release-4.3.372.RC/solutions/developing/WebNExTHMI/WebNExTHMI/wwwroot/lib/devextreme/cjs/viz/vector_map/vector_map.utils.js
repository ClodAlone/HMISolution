/**
 * DevExtreme (cjs/viz/vector_map/vector_map.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.generateDataKey = generateDataKey;
var nextDataKey = 1;

function generateDataKey() {
    return "vectormap-data-" + nextDataKey++
}
