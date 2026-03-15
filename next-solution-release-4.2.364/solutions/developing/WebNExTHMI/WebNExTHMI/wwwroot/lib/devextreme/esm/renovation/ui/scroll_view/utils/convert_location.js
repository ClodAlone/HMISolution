/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/convert_location.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined,
    isPlainObject
} from "../../../../core/utils/type";
import {
    ensureDefined
} from "../../../../core/utils/common";
import {
    ScrollDirection
} from "./scroll_direction";
export function convertToLocation(location, direction) {
    if (isPlainObject(location)) {
        var left = ensureDefined(location.left, location.x);
        var top = ensureDefined(location.top, location.y);
        return {
            left: isDefined(left) ? left : void 0,
            top: isDefined(top) ? top : void 0
        }
    }
    var {
        isHorizontal: isHorizontal,
        isVertical: isVertical
    } = new ScrollDirection(direction);
    return {
        left: isHorizontal && isDefined(location) ? location : void 0,
        top: isVertical && isDefined(location) ? location : void 0
    }
}
