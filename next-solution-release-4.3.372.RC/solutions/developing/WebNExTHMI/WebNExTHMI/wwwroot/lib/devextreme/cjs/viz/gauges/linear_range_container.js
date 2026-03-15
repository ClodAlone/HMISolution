/**
 * DevExtreme (cjs/viz/gauges/linear_range_container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _base_range_container = _interopRequireDefault(require("./base_range_container"));
var _utils = require("../core/utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _Number = Number;
var _max = Math.max;
var LinearRangeContainer = _base_range_container.default.inherit({
    _processOptions: function() {
        this.vertical = this._options.vertical;
        this._inner = this._outer = 0;
        if (this.vertical) {
            switch ((0, _utils.normalizeEnum)(this._options.horizontalOrientation)) {
                case "left":
                    this._inner = 1;
                    break;
                case "center":
                    this._inner = this._outer = .5;
                    break;
                default:
                    this._outer = 1
            }
        } else {
            switch ((0, _utils.normalizeEnum)(this._options.verticalOrientation)) {
                case "top":
                    this._inner = 1;
                    break;
                case "center":
                    this._inner = this._outer = .5;
                    break;
                default:
                    this._outer = 1
            }
        }
    },
    _isVisible: function() {
        return true
    },
    _createRange: function(range, layout) {
        var inner = this._inner;
        var outer = this._outer;
        var startPosition = this._translator.translate(range.start);
        var endPosition = this._translator.translate(range.end);
        var points;
        var x = layout.x;
        var y = layout.y;
        var startWidth = range.startWidth;
        var endWidth = range.endWidth;
        if (this.vertical) {
            points = [x - startWidth * inner, startPosition, x - endWidth * inner, endPosition, x + endWidth * outer, endPosition, x + startWidth * outer, startPosition]
        } else {
            points = [startPosition, y + startWidth * outer, startPosition, y - startWidth * inner, endPosition, y - endWidth * inner, endPosition, y + endWidth * outer]
        }
        return this._renderer.path(points, "area")
    },
    measure: function(layout) {
        var result = {};
        var width;
        result.min = result.max = layout[this.vertical ? "x" : "y"];
        width = this._options.width;
        width = _Number(width) || _max(_Number(width.start), _Number(width.end));
        result.min -= this._inner * width;
        result.max += this._outer * width;
        return result
    }
});
var _default = LinearRangeContainer;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
