/**
 * DevExtreme (cjs/viz/series/points/bar_point.js)
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

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _extend = _extend2.extend;
var _math = Math;
var _floor = _math.floor;
var _abs = _math.abs;
var CANVAS_POSITION_DEFAULT = "canvas_position_default";
var DEFAULT_BAR_TRACKER_SIZE = 9;
var CORRECTING_BAR_TRACKER_VALUE = 4;
var RIGHT = "right";
var LEFT = "left";
var TOP = "top";
var BOTTOM = "bottom";

function getLabelOrientation(point) {
    var initialValue = point.initialValue;
    var invert = point._getValTranslator().getBusinessRange().invert;
    var isDiscreteValue = "discrete" === point.series.valueAxisType;
    var isFullStacked = point.series.isFullStackedSeries();
    var notAxisInverted = !isDiscreteValue && (initialValue >= 0 && !invert || initialValue < 0 && invert) || isDiscreteValue && !invert || isFullStacked;
    return notAxisInverted ? TOP : BOTTOM
}
var _default = _extend({}, _symbol_point.default, {
    correctCoordinates: function(correctOptions) {
        var correction = _floor(correctOptions.offset - correctOptions.width / 2);
        if (this._options.rotated) {
            this.height = correctOptions.width;
            this.yCorrection = correction;
            this.xCorrection = null
        } else {
            this.width = correctOptions.width;
            this.xCorrection = correction;
            this.yCorrection = null
        }
    },
    _getGraphicBBox: function(location) {
        var bBox = {
            x: this.x,
            y: this.y,
            width: this.width,
            height: this.height
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
    _getLabelConnector: function(location) {
        return this._getGraphicBBox(location)
    },
    _getLabelPosition: function() {
        var position = getLabelOrientation(this);
        if (this._options.rotated) {
            position = position === TOP ? RIGHT : LEFT
        }
        return position
    },
    _getLabelCoords: function(label) {
        var coords;
        if (0 === this.initialValue && this.series.isFullStackedSeries()) {
            if (!this._options.rotated) {
                coords = this._getLabelCoordOfPosition(label, TOP)
            } else {
                coords = this._getLabelCoordOfPosition(label, RIGHT)
            }
        } else if ("inside" === label.getLayoutOptions().position) {
            coords = this._getLabelCoordOfPosition(label, "inside")
        } else {
            coords = _symbol_point.default._getLabelCoords.call(this, label)
        }
        return coords
    },
    _drawLabel: function() {
        this._label.pointPosition = "inside" !== this._label.getLayoutOptions().position && getLabelOrientation(this);
        _symbol_point.default._drawLabel.call(this)
    },
    hideInsideLabel: function(label, coord) {
        var graphicBBox = this._getGraphicBBox();
        var labelBBox = label.getBoundingRect();
        if (this._options.resolveLabelsOverlapping) {
            if ((coord.y <= graphicBBox.y && coord.y + labelBBox.height >= graphicBBox.y + graphicBBox.height || coord.x <= graphicBBox.x && coord.x + labelBBox.width >= graphicBBox.x + graphicBBox.width) && !(coord.y > graphicBBox.y + graphicBBox.height || coord.y + labelBBox.height < graphicBBox.y || coord.x > graphicBBox.x + graphicBBox.width || coord.x + labelBBox.width < graphicBBox.x)) {
                label.draw(false);
                return true
            }
        }
        return false
    },
    _showForZeroValues: function() {
        return this._options.label.showForZeroValues || this.initialValue
    },
    _drawMarker: function(renderer, group, animationEnabled) {
        var style = this._getStyle();
        var r = this._options.cornerRadius;
        var rotated = this._options.rotated;
        var _that$getMarkerCoords = this.getMarkerCoords(),
            x = _that$getMarkerCoords.x,
            y = _that$getMarkerCoords.y,
            width = _that$getMarkerCoords.width,
            height = _that$getMarkerCoords.height;
        if (animationEnabled) {
            if (rotated) {
                width = 0;
                x = this.defaultX
            } else {
                height = 0;
                y = this.defaultY
            }
        }
        this.graphic = renderer.rect(x, y, width, height).attr({
            rx: r,
            ry: r
        }).smartAttr(style).data({
            "chart-data-point": this
        }).append(group)
    },
    _getSettingsForTracker: function() {
        var y = this.y;
        var height = this.height;
        var x = this.x;
        var width = this.width;
        if (this._options.rotated) {
            if (1 === width) {
                width = DEFAULT_BAR_TRACKER_SIZE;
                x -= CORRECTING_BAR_TRACKER_VALUE
            }
        } else if (1 === height) {
            height = DEFAULT_BAR_TRACKER_SIZE;
            y -= CORRECTING_BAR_TRACKER_VALUE
        }
        return {
            x: x,
            y: y,
            width: width,
            height: height
        }
    },
    getGraphicSettings: function() {
        var graphic = this.graphic;
        return {
            x: graphic.attr("x"),
            y: graphic.attr("y"),
            height: graphic.attr("height"),
            width: graphic.attr("width")
        }
    },
    _getEdgeTooltipParams: function() {
        var isPositive = this.value >= 0;
        var xCoord;
        var yCoord;
        var invertedBusinessRange = this._getValTranslator().getBusinessRange().invert;
        var x = this.x,
            y = this.y,
            width = this.width,
            height = this.height;
        if (this._options.rotated) {
            yCoord = y + height / 2;
            if (invertedBusinessRange) {
                xCoord = isPositive ? x : x + width
            } else {
                xCoord = isPositive ? x + width : x
            }
        } else {
            xCoord = x + width / 2;
            if (invertedBusinessRange) {
                yCoord = isPositive ? y + height : y
            } else {
                yCoord = isPositive ? y : y + height
            }
        }
        return {
            x: xCoord,
            y: yCoord,
            offset: 0
        }
    },
    getTooltipParams: function(location) {
        if ("edge" === location) {
            return this._getEdgeTooltipParams()
        }
        var center = this.getCenterCoord();
        center.offset = 0;
        return center
    },
    getCenterCoord: function() {
        var width = this.width,
            height = this.height,
            x = this.x,
            y = this.y;
        return {
            x: x + width / 2,
            y: y + height / 2
        }
    },
    _truncateCoord: function(coord, bounds) {
        if (null === coord) {
            return coord
        }
        if (coord < bounds[0]) {
            return bounds[0]
        }
        if (coord > bounds[1]) {
            return bounds[1]
        }
        return coord
    },
    _getErrorBarBaseEdgeLength: function() {
        return this._options.rotated ? this.height : this.width
    },
    _translateErrorBars: function(argVisibleArea) {
        _symbol_point.default._translateErrorBars.call(this);
        if (this._errorBarPos < argVisibleArea[0] || this._errorBarPos > argVisibleArea[1]) {
            this._errorBarPos = void 0
        }
    },
    _translate: function() {
        var rotated = this._options.rotated;
        var valAxis = rotated ? "x" : "y";
        var argAxis = rotated ? "y" : "x";
        var valIntervalName = rotated ? "width" : "height";
        var argIntervalName = rotated ? "height" : "width";
        var argTranslator = this._getArgTranslator();
        var valTranslator = this._getValTranslator();
        var argVisibleArea = this.series.getArgumentAxis().getVisibleArea();
        var valVisibleArea = this.series.getValueAxis().getVisibleArea();
        var arg = argTranslator.translate(this.argument);
        var val = valTranslator.translate(this.value, 1);
        var minVal = valTranslator.translate(this.minValue);
        this[argAxis] = arg = null === arg ? arg : arg + (this[argAxis + "Correction"] || 0);
        this["v" + valAxis] = val;
        this["v" + argAxis] = arg + this[argIntervalName] / 2;
        val = this._truncateCoord(val, valVisibleArea);
        minVal = this._truncateCoord(minVal, valVisibleArea);
        this[valIntervalName] = _abs(val - minVal);
        val = val < minVal ? val : minVal;
        this._calculateVisibility(rotated ? val : arg, rotated ? arg : val, this.width, this.height);
        this[valAxis] = null === val ? val : val + (this[valAxis + "Correction"] || 0);
        this["min" + valAxis.toUpperCase()] = null === minVal ? minVal : minVal + (this[valAxis + "Correction"] || 0);
        this["default" + valAxis.toUpperCase()] = valTranslator.translate(CANVAS_POSITION_DEFAULT);
        this._translateErrorBars(argVisibleArea);
        if (this.inVisibleArea && null !== this[argAxis]) {
            if (this[argAxis] < argVisibleArea[0]) {
                this[argIntervalName] = this[argIntervalName] - (argVisibleArea[0] - this[argAxis]);
                this[argAxis] = argVisibleArea[0]
            }
            if (this[argAxis] + this[argIntervalName] > argVisibleArea[1]) {
                this[argIntervalName] = argVisibleArea[1] - this[argAxis]
            }
        }
    },
    _updateMarker: function(animationEnabled, style) {
        this.graphic.smartAttr(_extend({}, style, !animationEnabled ? this.getMarkerCoords() : {}))
    },
    getMarkerCoords: function() {
        var x = this.x;
        var y = this.y;
        var width = this.width;
        var height = this.height;
        var argAxis = this.series.getArgumentAxis();
        var rotated = this._options.rotated;
        if (argAxis.getAxisPosition) {
            var axisOptions = argAxis.getOptions();
            var edgeOffset = Math.round(axisOptions.width / 2);
            var argAxisPosition = argAxis.getAxisPosition();
            if (axisOptions.visible) {
                if (!rotated) {
                    height -= this.minY === this.defaultY && this.minY === argAxisPosition - argAxis.getAxisShift() ? edgeOffset : 0;
                    height < 0 && (height = 0)
                } else {
                    var isStartFromAxis = this.minX === this.defaultX && this.minX === argAxisPosition - argAxis.getAxisShift();
                    x += isStartFromAxis ? edgeOffset : 0;
                    width -= isStartFromAxis ? edgeOffset : 0;
                    width < 0 && (width = 0)
                }
            }
        }
        return {
            x: x,
            y: y,
            width: width,
            height: height
        }
    },
    coordsIn: function(x, y) {
        return x >= this.x && x <= this.x + this.width && y >= this.y && y <= this.y + this.height
    }
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
