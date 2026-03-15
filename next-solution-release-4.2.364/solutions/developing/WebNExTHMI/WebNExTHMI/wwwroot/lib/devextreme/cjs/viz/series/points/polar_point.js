/**
 * DevExtreme (cjs/viz/series/points/polar_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.polarSymbolPoint = exports.polarBarPoint = void 0;
var _extend2 = require("../../../core/utils/extend");
var _symbol_point = _interopRequireDefault(require("./symbol_point"));
var _bar_point = _interopRequireDefault(require("./bar_point"));
var _pie_point = _interopRequireDefault(require("./pie_point"));
var _type = require("../../../core/utils/type");
var _utils = require("../../core/utils");
var _consts = _interopRequireDefault(require("../../components/consts"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _extend = _extend2.extend;
var _math = Math;
var _max = _math.max;
var RADIAL_LABEL_INDENT = _consts.default.radialLabelIndent;
var ERROR_BARS_ANGLE_OFFSET = 90;
var CANVAS_POSITION_START = "canvas_position_start";
var CANVAS_POSITION_END = "canvas_position_end";
var CANVAS_POSITION_DEFAULT = "canvas_position_default";
var polarSymbolPoint = _extend({}, _symbol_point.default, {
    _getLabelCoords: _pie_point.default._getLabelCoords,
    _getElementCoords: _pie_point.default._getElementCoords,
    _moveLabelOnCanvas: function(coord, visibleArea, labelBBox) {
        var x = coord.x;
        var y = coord.y;
        if (visibleArea.minX > x) {
            x = visibleArea.minX
        }
        if (visibleArea.maxX < x + labelBBox.width) {
            x = visibleArea.maxX - labelBBox.width
        }
        if (visibleArea.minY > y) {
            y = visibleArea.minY
        }
        if (visibleArea.maxY < y + labelBBox.height) {
            y = visibleArea.maxY - labelBBox.height
        }
        return {
            x: x,
            y: y
        }
    },
    _getLabelPosition: function() {
        return "outside"
    },
    _getCoords: function(argument, value) {
        var axis = this.series.getValueAxis();
        var startAngle = axis.getAngles()[0];
        var angle = this._getArgTranslator().translate(argument);
        var radius = this._getValTranslator().translate(value);
        var coords = (0, _utils.convertPolarToXY)(axis.getCenter(), axis.getAngles()[0], angle, radius);
        coords.angle = angle + startAngle - 90, coords.radius = radius;
        return coords
    },
    _translate: function() {
        var center = this.series.getValueAxis().getCenter();
        var coord = this._getCoords(this.argument, this.value);
        var translator = this._getValTranslator();
        var maxRadius = translator.isInverted() ? translator.translate(CANVAS_POSITION_START) : translator.translate(CANVAS_POSITION_END);
        var normalizedRadius = (0, _type.isDefined)(coord.radius) && coord.radius >= 0 ? coord.radius : null;
        this.vx = (0, _utils.normalizeAngle)(coord.angle);
        this.vy = this.radiusOuter = this.radiusLabels = normalizedRadius;
        this.radiusLabels += RADIAL_LABEL_INDENT;
        this.radius = normalizedRadius;
        this.middleAngle = -coord.angle;
        this.angle = -coord.angle;
        this.x = coord.x;
        this.y = coord.y;
        this.defaultX = this.centerX = center.x;
        this.defaultY = this.centerY = center.y;
        this._translateErrorBars();
        this.inVisibleArea = this._checkRadiusForVisibleArea(normalizedRadius, maxRadius)
    },
    _checkRadiusForVisibleArea: function(radius, maxRadius) {
        return (0, _type.isDefined)(radius) && radius <= maxRadius
    },
    _translateErrorBars: function() {
        var errorBars = this._options.errorBars;
        var translator = this._getValTranslator();
        if (!errorBars) {
            return
        }(0, _type.isDefined)(this.lowError) && (this._lowErrorCoord = this.centerY - translator.translate(this.lowError));
        (0, _type.isDefined)(this.highError) && (this._highErrorCoord = this.centerY - translator.translate(this.highError));
        this._errorBarPos = this.centerX;
        this._baseErrorBarPos = "stdDeviation" === errorBars.type ? this._lowErrorCoord + (this._highErrorCoord - this._lowErrorCoord) / 2 : this.centerY - this.radius
    },
    _getTranslates: function(animationEnabled) {
        return animationEnabled ? this.getDefaultCoords() : {
            x: this.x,
            y: this.y
        }
    },
    getDefaultCoords: function() {
        var cosSin = (0, _utils.getCosAndSin)(-this.angle);
        var radius = this._getValTranslator().translate(CANVAS_POSITION_DEFAULT);
        var x = this.defaultX + radius * cosSin.cos;
        var y = this.defaultY + radius * cosSin.sin;
        return {
            x: x,
            y: y
        }
    },
    _addLabelAlignmentAndOffset: function(label, coord) {
        return coord
    },
    _checkLabelPosition: function(label, coord) {
        var visibleArea = this._getVisibleArea();
        var graphicBBox = this._getGraphicBBox();
        if (this._isPointInVisibleArea(visibleArea, graphicBBox)) {
            coord = this._moveLabelOnCanvas(coord, visibleArea, label.getBoundingRect())
        }
        return coord
    },
    _getErrorBarSettings: function(errorBarOptions, animationEnabled) {
        var settings = _symbol_point.default._getErrorBarSettings.call(this, errorBarOptions, animationEnabled);
        settings.rotate = ERROR_BARS_ANGLE_OFFSET - this.angle;
        settings.rotateX = this.centerX;
        settings.rotateY = this.centerY;
        return settings
    },
    getCoords: function(min) {
        return min ? this.getDefaultCoords() : {
            x: this.x,
            y: this.y
        }
    }
});
exports.polarSymbolPoint = polarSymbolPoint;
var polarBarPoint = _extend({}, _bar_point.default, {
    _translateErrorBars: polarSymbolPoint._translateErrorBars,
    _getErrorBarSettings: polarSymbolPoint._getErrorBarSettings,
    _moveLabelOnCanvas: polarSymbolPoint._moveLabelOnCanvas,
    _getLabelCoords: _pie_point.default._getLabelCoords,
    _getElementCoords: _pie_point.default._getElementCoords,
    _getLabelConnector: _pie_point.default._getLabelConnector,
    getTooltipParams: _pie_point.default.getTooltipParams,
    _getLabelPosition: _pie_point.default._getLabelPosition,
    _getCoords: polarSymbolPoint._getCoords,
    _translate: function() {
        var translator = this._getValTranslator();
        var businessRange = translator.getBusinessRange();
        var maxRadius = translator.isInverted() ? translator.translate(CANVAS_POSITION_START) : translator.translate(CANVAS_POSITION_END);
        this.radiusInner = translator.translate(this.minValue);
        polarSymbolPoint._translate.call(this);
        if (null === this.radiusInner) {
            this.radiusInner = this.radius = maxRadius
        } else if (null === this.radius) {
            this.radius = this.value >= businessRange.minVisible ? maxRadius : 0
        } else if (this.radius > maxRadius) {
            this.radius = maxRadius
        }
        this.radiusOuter = this.radiusLabels = _max(this.radiusInner, this.radius);
        this.radiusLabels += RADIAL_LABEL_INDENT;
        this.radiusInner = this.defaultRadius = _math.min(this.radiusInner, this.radius);
        this.middleAngle = this.angle = -(0, _utils.normalizeAngle)(this.middleAngleCorrection - this.angle)
    },
    _checkRadiusForVisibleArea: function(radius) {
        return (0, _type.isDefined)(radius) || this._getValTranslator().translate(this.minValue) > 0
    },
    _getErrorBarBaseEdgeLength: function() {
        var coord = this.getMarkerCoords();
        return _math.PI * coord.outerRadius * _math.abs(coord.startAngle - coord.endAngle) / 180
    },
    getMarkerCoords: function() {
        return {
            x: this.centerX,
            y: this.centerY,
            outerRadius: this.radiusOuter,
            innerRadius: this.defaultRadius,
            startAngle: this.middleAngle - this.interval / 2,
            endAngle: this.middleAngle + this.interval / 2
        }
    },
    _drawMarker: function(renderer, group, animationEnabled) {
        var styles = this._getStyle();
        var coords = this.getMarkerCoords();
        var innerRadius = coords.innerRadius;
        var outerRadius = coords.outerRadius;
        var start = this._getCoords(this.argument, CANVAS_POSITION_DEFAULT);
        var x = coords.x;
        var y = coords.y;
        if (animationEnabled) {
            innerRadius = 0;
            outerRadius = 0;
            x = start.x;
            y = start.y
        }
        this.graphic = renderer.arc(x, y, innerRadius, outerRadius, coords.startAngle, coords.endAngle).attr(styles).data({
            "chart-data-point": this
        }).append(group)
    },
    _checkLabelPosition: function(label, coord) {
        var visibleArea = this._getVisibleArea();
        var angleFunctions = (0, _utils.getCosAndSin)(this.middleAngle);
        var x = this.centerX + this.defaultRadius * angleFunctions.cos;
        var y = this.centerY - this.defaultRadius * angleFunctions.sin;
        if (x > visibleArea.minX && x < visibleArea.maxX && y > visibleArea.minY && y < visibleArea.maxY) {
            coord = this._moveLabelOnCanvas(coord, visibleArea, label.getBoundingRect())
        }
        return coord
    },
    _addLabelAlignmentAndOffset: function(label, coord) {
        return coord
    },
    correctCoordinates: function(correctOptions) {
        this.middleAngleCorrection = correctOptions.offset;
        this.interval = correctOptions.width
    },
    coordsIn: function(x, y) {
        var val = (0, _utils.convertXYToPolar)(this.series.getValueAxis().getCenter(), x, y);
        var coords = this.getMarkerCoords();
        var isBetweenAngles = coords.startAngle < coords.endAngle ? -val.phi >= coords.startAngle && -val.phi <= coords.endAngle : -val.phi <= coords.startAngle && -val.phi >= coords.endAngle;
        return val.r >= coords.innerRadius && val.r <= coords.outerRadius && isBetweenAngles
    }
});
exports.polarBarPoint = polarBarPoint;
