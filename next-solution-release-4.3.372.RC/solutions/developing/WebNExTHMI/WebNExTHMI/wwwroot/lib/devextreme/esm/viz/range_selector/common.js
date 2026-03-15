/**
 * DevExtreme (esm/viz/range_selector/common.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    smartFormatter as _format
} from "../axes/smart_formatter";
import {
    isFunction
} from "../../core/utils/type";
export var HEIGHT_COMPACT_MODE = 24;
var POINTER_SIZE = 4;
var EMPTY_SLIDER_MARKER_TEXT = ". . .";
export var utils = {
    trackerSettings: {
        fill: "grey",
        stroke: "grey",
        opacity: 1e-4
    },
    animationSettings: {
        duration: 250
    }
};
export var consts = {
    emptySliderMarkerText: EMPTY_SLIDER_MARKER_TEXT,
    pointerSize: POINTER_SIZE
};
export var formatValue = function(value, formatOptions, tickIntervalsInfo, valueType, type, logarithmBase) {
    var formatObject = {
        value: value,
        valueText: _format(value, {
            labelOptions: formatOptions,
            ticks: tickIntervalsInfo ? tickIntervalsInfo.ticks : [],
            tickInterval: tickIntervalsInfo ? tickIntervalsInfo.tickInterval : void 0,
            dataType: valueType,
            type: type,
            logarithmBase: logarithmBase
        })
    };
    return String(isFunction(formatOptions.customizeText) ? formatOptions.customizeText.call(formatObject, formatObject) : formatObject.valueText)
};
