/**
 * DevExtreme (renovation/ui/scroll_view/utils/get_permissible_wheel_direction.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.permissibleWheelDirection = permissibleWheelDirection;
var _consts = require("../common/consts");

function permissibleWheelDirection(direction, isShiftKey) {
    switch (direction) {
        case _consts.DIRECTION_HORIZONTAL:
            return _consts.DIRECTION_HORIZONTAL;
        case _consts.DIRECTION_VERTICAL:
            return _consts.DIRECTION_VERTICAL;
        default:
            return isShiftKey ? _consts.DIRECTION_HORIZONTAL : _consts.DIRECTION_VERTICAL
    }
}
