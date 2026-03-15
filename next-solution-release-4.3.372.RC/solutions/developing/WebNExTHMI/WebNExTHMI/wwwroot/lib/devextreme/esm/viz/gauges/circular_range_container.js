/**
 * DevExtreme (esm/viz/gauges/circular_range_container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseRangeContainer from "./base_range_container";
var _Number = Number;
var _max = Math.max;
import {
    normalizeEnum as _normalizeEnum
} from "../core/utils";
var CircularRangeContainer = BaseRangeContainer.inherit({
    _processOptions: function() {
        this._inner = this._outer = 0;
        switch (_normalizeEnum(this._options.orientation)) {
            case "inside":
                this._inner = 1;
                break;
            case "center":
                this._inner = this._outer = .5;
                break;
            default:
                this._outer = 1
        }
    },
    _isVisible: function(layout) {
        var width = this._options.width;
        width = _Number(width) || _max(_Number(width.start), _Number(width.end));
        return layout.radius - this._inner * width > 0
    },
    _createRange: function(range, layout) {
        var width = (range.startWidth + range.endWidth) / 2;
        return this._renderer.arc(layout.x, layout.y, layout.radius - this._inner * width, layout.radius + this._outer * width, this._translator.translate(range.end), this._translator.translate(range.start)).attr({
            "stroke-linejoin": "round"
        })
    },
    measure: function(layout) {
        var width = this._options.width;
        width = _Number(width) || _max(_Number(width.start), _Number(width.end));
        return {
            min: layout.radius - this._inner * width,
            max: layout.radius + this._outer * width
        }
    }
});
export default CircularRangeContainer;
