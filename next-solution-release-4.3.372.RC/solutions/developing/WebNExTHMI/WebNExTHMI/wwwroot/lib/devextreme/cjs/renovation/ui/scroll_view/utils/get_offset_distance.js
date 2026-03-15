/**
 * DevExtreme (cjs/renovation/ui/scroll_view/utils/get_offset_distance.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getOffsetDistance = getOffsetDistance;
var _common = require("../../../../core/utils/common");

function getOffsetDistance(targetLocation, scrollOffset) {
    return {
        top: (0, _common.ensureDefined)(targetLocation.top, scrollOffset.top) - scrollOffset.top,
        left: (0, _common.ensureDefined)(targetLocation.left, scrollOffset.left) - scrollOffset.left
    }
}
