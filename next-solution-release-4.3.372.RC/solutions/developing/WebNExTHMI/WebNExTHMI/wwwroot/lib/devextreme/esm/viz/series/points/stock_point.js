/**
 * DevExtreme (esm/viz/series/points/stock_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../../core/utils/extend";
import {
    isNumeric
} from "../../../core/utils/type";
import candlestickPoint from "./candlestick_point";
var _extend = extend;
var _isNumeric = isNumeric;
export default _extend({}, candlestickPoint, {
    _getPoints: function() {
        var createPoint = this._options.rotated ? function(x, y) {
            return [y, x]
        } : function(x, y) {
            return [x, y]
        };
        var openYExist = _isNumeric(this.openY);
        var closeYExist = _isNumeric(this.closeY);
        var x = this.x;
        var width = this.width;
        var points = [].concat(createPoint(x, this.highY));
        openYExist && (points = points.concat(createPoint(x, this.openY)));
        openYExist && (points = points.concat(createPoint(x - width / 2, this.openY)));
        openYExist && (points = points.concat(createPoint(x, this.openY)));
        closeYExist && (points = points.concat(createPoint(x, this.closeY)));
        closeYExist && (points = points.concat(createPoint(x + width / 2, this.closeY)));
        closeYExist && (points = points.concat(createPoint(x, this.closeY)));
        points = points.concat(createPoint(x, this.lowY));
        return points
    },
    _drawMarkerInGroup: function(group, attributes, renderer) {
        this.graphic = renderer.path(this._getPoints(), "line").attr({
            "stroke-linecap": "square"
        }).attr(attributes).data({
            "chart-data-point": this
        }).sharp().append(group)
    },
    _getMinTrackerWidth: function() {
        var width = 2 + this._styles.normal["stroke-width"];
        return width + width % 2
    }
});
