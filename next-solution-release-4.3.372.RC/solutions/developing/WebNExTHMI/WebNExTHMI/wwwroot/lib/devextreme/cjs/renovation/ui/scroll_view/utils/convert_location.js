/**
 * DevExtreme (cjs/renovation/ui/scroll_view/utils/convert_location.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.convertToLocation = convertToLocation;
var _type = require("../../../../core/utils/type");
var _common = require("../../../../core/utils/common");
var _scroll_direction = require("./scroll_direction");

function convertToLocation(location, direction) {
    if ((0, _type.isPlainObject)(location)) {
        var left = (0, _common.ensureDefined)(location.left, location.x);
        var top = (0, _common.ensureDefined)(location.top, location.y);
        return {
            left: (0, _type.isDefined)(left) ? left : void 0,
            top: (0, _type.isDefined)(top) ? top : void 0
        }
    }
    var _ScrollDirection = new _scroll_direction.ScrollDirection(direction),
        isHorizontal = _ScrollDirection.isHorizontal,
        isVertical = _ScrollDirection.isVertical;
    return {
        left: isHorizontal && (0, _type.isDefined)(location) ? location : void 0,
        top: isVertical && (0, _type.isDefined)(location) ? location : void 0
    }
}
