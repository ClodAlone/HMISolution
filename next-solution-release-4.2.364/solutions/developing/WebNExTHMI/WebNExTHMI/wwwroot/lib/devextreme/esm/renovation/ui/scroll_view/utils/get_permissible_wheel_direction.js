/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_permissible_wheel_direction.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    DIRECTION_HORIZONTAL,
    DIRECTION_VERTICAL
} from "../common/consts";
export function permissibleWheelDirection(direction, isShiftKey) {
    switch (direction) {
        case DIRECTION_HORIZONTAL:
            return DIRECTION_HORIZONTAL;
        case DIRECTION_VERTICAL:
            return DIRECTION_VERTICAL;
        default:
            return isShiftKey ? DIRECTION_HORIZONTAL : DIRECTION_VERTICAL
    }
}
