/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_offset_distance.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    ensureDefined
} from "../../../../core/utils/common";
export function getOffsetDistance(targetLocation, scrollOffset) {
    return {
        top: ensureDefined(targetLocation.top, scrollOffset.top) - scrollOffset.top,
        left: ensureDefined(targetLocation.left, scrollOffset.left) - scrollOffset.left
    }
}
