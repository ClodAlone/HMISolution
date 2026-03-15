/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_allowed_direction.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    DIRECTION_BOTH,
    DIRECTION_HORIZONTAL,
    DIRECTION_VERTICAL
} from "../common/consts";
import {
    ScrollDirection
} from "./scroll_direction";
export function allowedDirection(direction, scrollTopMax, scrollLeftMax, bounceEnabled) {
    var {
        isBoth: isBoth,
        isHorizontal: isHorizontal,
        isVertical: isVertical
    } = new ScrollDirection(direction);
    var vDirectionAllowed = isVertical && (scrollTopMax > 0 || bounceEnabled);
    var hDirectionAllowed = isHorizontal && (scrollLeftMax > 0 || bounceEnabled);
    if (isBoth && vDirectionAllowed && hDirectionAllowed) {
        return DIRECTION_BOTH
    }
    if (isHorizontal && hDirectionAllowed) {
        return DIRECTION_HORIZONTAL
    }
    if (isVertical && vDirectionAllowed) {
        return DIRECTION_VERTICAL
    }
    return
}
