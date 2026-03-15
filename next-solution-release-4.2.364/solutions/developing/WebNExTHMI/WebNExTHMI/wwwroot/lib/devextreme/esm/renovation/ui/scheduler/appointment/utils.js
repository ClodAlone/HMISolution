/**
 * DevExtreme (esm/renovation/ui/scheduler/appointment/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    addToStyles
} from "../workspaces/utils";
export var getAppointmentStyles = item => {
    var {
        geometry: {
            height: height,
            left: left,
            top: top,
            width: width
        },
        info: {
            resourceColor: resourceColor
        }
    } = item;
    var result = addToStyles([{
        attr: "height",
        value: "".concat(height || 50, "px")
    }, {
        attr: "width",
        value: "".concat(width || 50, "px")
    }, {
        attr: "top",
        value: "".concat(top, "px")
    }, {
        attr: "left",
        value: "".concat(left, "px")
    }]);
    if (resourceColor) {
        result = addToStyles([{
            attr: "backgroundColor",
            value: resourceColor
        }], result)
    }
    return result
};
export var getAppointmentKey = geometry => {
    var {
        height: height,
        left: left,
        top: top,
        width: width
    } = geometry;
    return "".concat(left, "-").concat(top, "-").concat(width, "-").concat(height)
};
