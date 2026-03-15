/**
 * DevExtreme (cjs/viz/series/points/candlestick_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _extend2 = require("../../../core/utils/extend");
var _symbol_point = _interopRequireDefault(require("./symbol_point"));
var _bar_point = _interopRequireDefault(require("./bar_point"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _math = Math;
var _abs = _math.abs;
var _min = _math.min;
var _max = _math.max;
var _round = _math.round;
var DEFAULT_FINANCIAL_TRACKER_MARGIN = 2;
var _default = (0, _extend2.extend)({}, _bar_point.default, {
    _getContinuousPoints: function(openCoord, closeCoord) {
        var x = this.x;
        var createPoint = this._options.rotated ? function(x, y) {
            return [y, x]
        } : function(x, y) {
            return [x, y]
        };
        var width = this.width;
        var highCoord = this.highY;
        var max = _abs(highCoord - openCoord) < _abs(highCoord - closeCoord) ? openCoord : closeCoord;
        var min = max === closeCoord ? openCoord : closeCoord;
        var points;
        if (min === max) {
            points = [].concat(createPoint(x, this.highY)).concat(createPoint(x, this.lowY)).concat(createPoint(x, this.closeY)).concat(createPoint(x - width / 2, this.closeY)).concat(createPoint(x + width / 2, this.closeY)).concat(createPoint(x, this.closeY))
        } else {
            points = [].concat(createPoint(x, this.highY)).concat(createPoint(x, max)).concat(createPoint(x + width / 2, max)).concat(createPoint(x + width / 2, min)).concat(createPoint(x, min)).concat(createPoint(x, this.lowY)).concat(createPoint(x, min)).concat(createPoint(x - width / 2, min)).concat(createPoint(x - width / 2, max)).concat(createPoint(x, max))
        }
        return points
    },
    _getCrockPoints: function(y) {
        var x = this.x;
        var createPoint = this._options.rotated ? function(x, y) {
            return [y, x]
        } : function(x, y) {
            return [x, y]
        };
        return [].concat(createPoint(x, this.highY)).concat(createPoint(x, this.lowY)).concat(createPoint(x, y)).concat(createPoint(x - this.width / 2, y)).concat(createPoint(x + this.width / 2, y)).concat(createPoint(x, y))
    },
    _getPoints: function() {
        var points;
        var closeCoord = this.closeY;
        var openCoord = this.openY;
        if (null !== closeCoord && null !== openCoord) {
            points = this._getContinuousPoints(openCoord, closeCoord)
        } else if (openCoord === closeCoord) {
            points = [this.x, this.highY, this.x, this.lowY]
        } else {
            points = this._getCrockPoints(null !== openCoord ? openCoord : closeCoord)
        }
        return points
    },
    getColor: function() {
        return this._isReduction ? this._options.reduction.color : this._styles.normal.stroke || this.series.getColor()
    },
    _drawMarkerInGroup: function(group, attributes, renderer) {
        this.graphic = renderer.path(this._getPoints(), "area").attr({
            "stroke-linecap": "square"
        }).attr(attributes).data({
            "chart-data-point": this
        }).sharp().append(group)
    },
    _fillStyle: function() {
        var styles = this._options.styles;
        if (this._isReduction && this._isPositive) {
            this._styles = styles.reductionPositive
        } else if (this._isReduction) {
            this._styles = styles.reduction
        } else if (this._isPositive) {
            this._styles = styles.positive
        } else {
            this._styles = styles
        }
    },
    _getMinTrackerWidth: function() {
        return 2 + 2 * this._styles.normal["stroke-width"]
    },
    correctCoordinates: function(correctOptions) {
        var minWidth = this._getMinTrackerWidth();
        var width = correctOptions.width;
        width = width < minWidth ? minWidth : width > 10 ? 10 : width;
        this.width = width + width % 2;
        this.xCorrection = correctOptions.offset
    },
    _getMarkerGroup: function(group) {
        var markerGroup;
        if (this._isReduction && this._isPositive) {
            markerGroup = group.reductionPositiveMarkersGroup
        } else if (this._isReduction) {
            markerGroup = group.reductionMarkersGroup
        } else if (this._isPositive) {
            markerGroup = group.defaultPositiveMarkersGroup
        } else {
            markerGroup = group.defaultMarkersGroup
        }
        return markerGroup
    },
    _drawMarker: function(renderer, group) {
        this._drawMarkerInGroup(this._getMarkerGroup(group), this._getStyle(), renderer)
    },
    _getSettingsForTracker: function() {
        var highY = this.highY;
        var lowY = this.lowY;
        var rotated = this._options.rotated;
        var x;
        var y;
        var width;
        var height;
        if (highY === lowY) {
            highY = rotated ? highY + DEFAULT_FINANCIAL_TRACKER_MARGIN : highY - DEFAULT_FINANCIAL_TRACKER_MARGIN;
            lowY = rotated ? lowY - DEFAULT_FINANCIAL_TRACKER_MARGIN : lowY + DEFAULT_FINANCIAL_TRACKER_MARGIN
        }
        if (rotated) {
            x = _min(lowY, highY);
            y = this.x - this.width / 2;
            width = _abs(lowY - highY);
            height = this.width
        } else {
            x = this.x - this.width / 2;
            y = _min(lowY, highY);
            width = this.width;
            height = _abs(lowY - highY)
        }
        return {
            x: x,
            y: y,
            width: width,
            height: height
        }
    },
    _getGraphicBBox: function(location) {
        var rotated = this._options.rotated;
        var x = this.x;
        var width = this.width;
        var lowY = this.lowY;
        var highY = this.highY;
        if (location) {
            var valVisibleArea = this.series.getValueAxis().getVisibleArea();
            highY = this._truncateCoord(highY, valVisibleArea);
            lowY = this._truncateCoord(lowY, valVisibleArea)
        }
        var bBox = {
            x: !rotated ? x - _round(width / 2) : lowY,
            y: !rotated ? highY : x - _round(width / 2),
            width: !rotated ? width : highY - lowY,
            height: !rotated ? lowY - highY : width
        };
        if (location) {
            var isTop = "top" === location;
            if (!this._options.rotated) {
                bBox.y = isTop ? bBox.y : bBox.y + bBox.height;
                bBox.height = 0
            } else {
                bBox.x = isTop ? bBox.x + bBox.width : bBox.x;
                bBox.width = 0
            }
        }
        return bBox
    },
    getTooltipParams: function(location) {
        if (this.graphic) {
            var minValue = _min(this.lowY, this.highY);
            var maxValue = _max(this.lowY, this.highY);
            var visibleArea = this._getVisibleArea();
            var rotated = this._options.rotated;
            var minVisible = rotated ? visibleArea.minX : visibleArea.minY;
            var maxVisible = rotated ? visibleArea.maxX : visibleArea.maxY;
            var min = _max(minVisible, minValue);
            var max = _min(maxVisible, maxValue);
            var centerCoord = this.getCenterCoord();
            if ("edge" === location) {
                centerCoord[rotated ? "x" : "y"] = rotated ? max : min
            }
            centerCoord.offset = 0;
            return centerCoord
        }
    },
    getCenterCoord: function() {
        if (this.graphic) {
            var x;
            var y;
            var minValue = _min(this.lowY, this.highY);
            var maxValue = _max(this.lowY, this.highY);
            var visibleArea = this._getVisibleArea();
            var rotated = this._options.rotated;
            var minVisible = rotated ? visibleArea.minX : visibleArea.minY;
            var maxVisible = rotated ? visibleArea.maxX : visibleArea.maxY;
            var min = _max(minVisible, minValue);
            var max = _min(maxVisible, maxValue);
            var center = min + (max - min) / 2;
            if (rotated) {
                y = this.x;
                x = center
            } else {
                x = this.x;
                y = center
            }
            return {
                x: x,
                y: y
            }
        }
    },
    hasValue: function() {
        return null !== this.highValue && null !== this.lowValue
    },
    hasCoords: function() {
        return null !== this.x && null !== this.lowY && null !== this.highY
    },
    _translate: function() {
        var rotated = this._options.rotated;
        var valTranslator = this._getValTranslator();
        var x = this._getArgTranslator().translate(this.argument);
        this.vx = this.vy = this.x = null === x ? x : x + (this.xCorrection || 0);
        this.openY = null !== this.openValue ? valTranslator.translate(this.openValue) : null;
        this.highY = valTranslator.translate(this.highValue);
        this.lowY = valTranslator.translate(this.lowValue);
        this.closeY = null !== this.closeValue ? valTranslator.translate(this.closeValue) : null;
        var centerValue = _min(this.lowY, this.highY) + _abs(this.lowY - this.highY) / 2;
        this._calculateVisibility(!rotated ? this.x : centerValue, !rotated ? centerValue : this.x)
    },
    getCrosshairData: function(x, y) {
        var rotated = this._options.rotated;
        var origY = rotated ? x : y;
        var yValue;
        var argument = this.argument;
        var coords;
        var coord = "low";
        if (_abs(this.lowY - origY) < _abs(this.closeY - origY)) {
            yValue = this.lowY
        } else {
            yValue = this.closeY;
            coord = "close"
        }
        if (_abs(yValue - origY) >= _abs(this.openY - origY)) {
            yValue = this.openY;
            coord = "open"
        }
        if (_abs(yValue - origY) >= _abs(this.highY - origY)) {
            yValue = this.highY;
            coord = "high"
        }
        if (rotated) {
            coords = {
                y: this.vy,
                x: yValue,
                xValue: this[coord + "Value"],
                yValue: argument
            }
        } else {
            coords = {
                x: this.vx,
                y: yValue,
                xValue: argument,
                yValue: this[coord + "Value"]
            }
        }
        coords.axis = this.series.axis;
        return coords
    },
    _updateData: function(data) {
        var label = this._label;
        var reductionColor = this._options.reduction.color;
        this.value = this.initialValue = data.reductionValue;
        this.originalValue = data.value;
        this.lowValue = this.originalLowValue = data.lowValue;
        this.highValue = this.originalHighValue = data.highValue;
        this.openValue = this.originalOpenValue = data.openValue;
        this.closeValue = this.originalCloseValue = data.closeValue;
        this._isPositive = data.openValue < data.closeValue;
        this._isReduction = data.isReduction;
        if (this._isReduction) {
            label.setColor(reductionColor)
        }
    },
    _updateMarker: function(animationEnabled, style, group) {
        var graphic = this.graphic;
        graphic.attr({
            points: this._getPoints()
        }).smartAttr(style).sharp();
        group && graphic.append(this._getMarkerGroup(group))
    },
    _getLabelFormatObject: function() {
        return {
            openValue: this.openValue,
            highValue: this.highValue,
            lowValue: this.lowValue,
            closeValue: this.closeValue,
            reductionValue: this.initialValue,
            argument: this.initialArgument,
            value: this.initialValue,
            seriesName: this.series.name,
            originalOpenValue: this.originalOpenValue,
            originalCloseValue: this.originalCloseValue,
            originalLowValue: this.originalLowValue,
            originalHighValue: this.originalHighValue,
            originalArgument: this.originalArgument,
            point: this
        }
    },
    _getFormatObject: function(tooltip) {
        var highValue = tooltip.formatValue(this.highValue);
        var openValue = tooltip.formatValue(this.openValue);
        var closeValue = tooltip.formatValue(this.closeValue);
        var lowValue = tooltip.formatValue(this.lowValue);
        var symbolMethods = _symbol_point.default;
        var formatObject = symbolMethods._getFormatObject.call(this, tooltip);
        return (0, _extend2.extend)({}, formatObject, {
            valueText: "h: " + highValue + ("" !== openValue ? " o: " + openValue : "") + ("" !== closeValue ? " c: " + closeValue : "") + " l: " + lowValue,
            highValueText: highValue,
            openValueText: openValue,
            closeValueText: closeValue,
            lowValueText: lowValue
        })
    },
    getMaxValue: function() {
        return this.highValue
    },
    getMinValue: function() {
        return this.lowValue
    }
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
