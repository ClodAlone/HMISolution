/**
 * DevExtreme (cjs/viz/series/points/range_symbol_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _iterator = require("../../../core/utils/iterator");
var _extend2 = require("../../../core/utils/extend");
var _common = require("../../../core/utils/common");
var _label = require("./label");
var _symbol_point = _interopRequireDefault(require("./symbol_point"));
var _type = require("../../../core/utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _extend = _extend2.extend;
var _math = Math;
var _abs = _math.abs;
var _min = _math.min;
var _max = _math.max;
var _round = _math.round;
var DEFAULT_IMAGE_WIDTH = 20;
var DEFAULT_IMAGE_HEIGHT = 20;
var _default = _extend({}, _symbol_point.default, {
    deleteLabel: function() {
        this._topLabel.dispose();
        this._topLabel = null;
        this._bottomLabel.dispose();
        this._bottomLabel = null
    },
    hideMarker: function(type) {
        var graphic = this.graphic;
        var marker = graphic && graphic[type + "Marker"];
        var label = this["_" + type + "Label"];
        if (marker && "hidden" !== marker.attr("visibility")) {
            marker.attr({
                visibility: "hidden"
            })
        }
        label.draw(false)
    },
    setInvisibility: function() {
        this.hideMarker("top");
        this.hideMarker("bottom")
    },
    clearVisibility: function() {
        var graphic = this.graphic;
        var topMarker = graphic && graphic.topMarker;
        var bottomMarker = graphic && graphic.bottomMarker;
        if (topMarker && topMarker.attr("visibility")) {
            topMarker.attr({
                visibility: null
            })
        }
        if (bottomMarker && bottomMarker.attr("visibility")) {
            bottomMarker.attr({
                visibility: null
            })
        }
    },
    clearMarker: function() {
        var graphic = this.graphic;
        var topMarker = graphic && graphic.topMarker;
        var bottomMarker = graphic && graphic.bottomMarker;
        var emptySettings = this._emptySettings;
        topMarker && topMarker.attr(emptySettings);
        bottomMarker && bottomMarker.attr(emptySettings)
    },
    _getLabelPosition: function(markerType) {
        var position;
        var labelsInside = "inside" === this._options.label.position;
        if (!this._options.rotated) {
            position = "top" === markerType ^ labelsInside ? "top" : "bottom"
        } else {
            position = "top" === markerType ^ labelsInside ? "right" : "left"
        }
        return position
    },
    _getLabelMinFormatObject: function() {
        return {
            index: 0,
            argument: this.initialArgument,
            value: this.initialMinValue,
            seriesName: this.series.name,
            originalValue: this.originalMinValue,
            originalArgument: this.originalArgument,
            point: this
        }
    },
    _updateLabelData: function() {
        var maxFormatObject = this._getLabelFormatObject();
        maxFormatObject.index = 1;
        this._topLabel.setData(maxFormatObject);
        this._bottomLabel.setData(this._getLabelMinFormatObject())
    },
    _updateLabelOptions: function() {
        var options = this._options.label;
        (!this._topLabel || !this._bottomLabel) && this._createLabel();
        this._topLabel.setOptions(options);
        this._bottomLabel.setOptions(options)
    },
    _createLabel: function() {
        var options = {
            renderer: this.series._renderer,
            labelsGroup: this.series._labelsGroup,
            point: this
        };
        this._topLabel = new _label.Label(options);
        this._bottomLabel = new _label.Label(options)
    },
    _getGraphicBBox: function(location) {
        var options = this._options;
        var images = this._getImage(options.image);
        var image = "top" === location ? this._checkImage(images.top) : this._checkImage(images.bottom);
        var bBox;
        var coord = this._getPositionFromLocation(location);
        if (options.visible) {
            bBox = image ? this._getImageBBox(coord.x, coord.y) : this._getSymbolBBox(coord.x, coord.y, options.styles.normal.r)
        } else {
            bBox = {
                x: coord.x,
                y: coord.y,
                width: 0,
                height: 0
            }
        }
        return bBox
    },
    _getPositionFromLocation: function(location) {
        var x;
        var y;
        var isTop = "top" === location;
        if (!this._options.rotated) {
            x = this.x;
            y = isTop ? _min(this.y, this.minY) : _max(this.y, this.minY)
        } else {
            x = isTop ? _max(this.x, this.minX) : _min(this.x, this.minX);
            y = this.y
        }
        return {
            x: x,
            y: y
        }
    },
    _checkOverlay: function(bottomCoord, topCoord, topValue) {
        return bottomCoord < topCoord + topValue
    },
    _getOverlayCorrections: function(topCoords, bottomCoords) {
        var rotated = this._options.rotated;
        var coordSelector = !rotated ? "y" : "x";
        var valueSelector = !rotated ? "height" : "width";
        var visibleArea = this.series.getValueAxis().getVisibleArea();
        var minBound = visibleArea[0];
        var maxBound = visibleArea[1];
        var delta = _round((topCoords[coordSelector] + topCoords[valueSelector] - bottomCoords[coordSelector]) / 2);
        var coord1 = topCoords[coordSelector] - delta;
        var coord2 = bottomCoords[coordSelector] + delta;
        if (coord1 < minBound) {
            delta = minBound - coord1;
            coord1 += delta;
            coord2 += delta
        } else if (coord2 + bottomCoords[valueSelector] > maxBound) {
            delta = maxBound - coord2 - bottomCoords[valueSelector];
            coord1 += delta;
            coord2 += delta
        }
        return {
            coord1: coord1,
            coord2: coord2
        }
    },
    _checkLabelsOverlay: function(topLocation) {
        var topCoords = this._topLabel.getBoundingRect();
        var bottomCoords = this._bottomLabel.getBoundingRect();
        var corrections = {};
        if (!this._options.rotated) {
            if ("top" === topLocation) {
                if (this._checkOverlay(bottomCoords.y, topCoords.y, topCoords.height)) {
                    corrections = this._getOverlayCorrections(topCoords, bottomCoords);
                    this._topLabel.shift(topCoords.x, corrections.coord1);
                    this._bottomLabel.shift(bottomCoords.x, corrections.coord2)
                }
            } else if (this._checkOverlay(topCoords.y, bottomCoords.y, bottomCoords.height)) {
                corrections = this._getOverlayCorrections(bottomCoords, topCoords);
                this._topLabel.shift(topCoords.x, corrections.coord2);
                this._bottomLabel.shift(bottomCoords.x, corrections.coord1)
            }
        } else if ("top" === topLocation) {
            if (this._checkOverlay(topCoords.x, bottomCoords.x, bottomCoords.width)) {
                corrections = this._getOverlayCorrections(bottomCoords, topCoords);
                this._topLabel.shift(corrections.coord2, topCoords.y);
                this._bottomLabel.shift(corrections.coord1, bottomCoords.y)
            }
        } else if (this._checkOverlay(bottomCoords.x, topCoords.x, topCoords.width)) {
            corrections = this._getOverlayCorrections(topCoords, bottomCoords);
            this._topLabel.shift(corrections.coord1, topCoords.y);
            this._bottomLabel.shift(corrections.coord2, bottomCoords.y)
        }
    },
    _drawLabel: function() {
        var labels = [];
        var notInverted = this._options.rotated ? this.x >= this.minX : this.y < this.minY;
        var customVisibility = this._getCustomLabelVisibility();
        var topLabel = this._topLabel;
        var bottomLabel = this._bottomLabel;
        topLabel.pointPosition = notInverted ? "top" : "bottom";
        bottomLabel.pointPosition = notInverted ? "bottom" : "top";
        if ((this.series.getLabelVisibility() || customVisibility) && this.hasValue() && false !== customVisibility) {
            false !== this.visibleTopMarker && labels.push(topLabel);
            false !== this.visibleBottomMarker && labels.push(bottomLabel);
            (0, _iterator.each)(labels, (function(_, label) {
                label.draw(true)
            }));
            this._checkLabelsOverlay(this._topLabel.pointPosition)
        } else {
            topLabel.draw(false);
            bottomLabel.draw(false)
        }
    },
    _getImage: function(imageOption) {
        var image = {};
        if ((0, _type.isDefined)(imageOption)) {
            if ("string" === typeof imageOption) {
                image.top = image.bottom = imageOption
            } else {
                image.top = {
                    url: "string" === typeof imageOption.url ? imageOption.url : imageOption.url && imageOption.url.rangeMaxPoint,
                    width: "number" === typeof imageOption.width ? imageOption.width : imageOption.width && imageOption.width.rangeMaxPoint,
                    height: "number" === typeof imageOption.height ? imageOption.height : imageOption.height && imageOption.height.rangeMaxPoint
                };
                image.bottom = {
                    url: "string" === typeof imageOption.url ? imageOption.url : imageOption.url && imageOption.url.rangeMinPoint,
                    width: "number" === typeof imageOption.width ? imageOption.width : imageOption.width && imageOption.width.rangeMinPoint,
                    height: "number" === typeof imageOption.height ? imageOption.height : imageOption.height && imageOption.height.rangeMinPoint
                }
            }
        }
        return image
    },
    _checkSymbol: function(oldOptions, newOptions) {
        var oldSymbol = oldOptions.symbol;
        var newSymbol = newOptions.symbol;
        var symbolChanged = "circle" === oldSymbol && "circle" !== newSymbol || "circle" !== oldSymbol && "circle" === newSymbol;
        var oldImages = this._getImage(oldOptions.image);
        var newImages = this._getImage(newOptions.image);
        var topImageChanged = this._checkImage(oldImages.top) !== this._checkImage(newImages.top);
        var bottomImageChanged = this._checkImage(oldImages.bottom) !== this._checkImage(newImages.bottom);
        return symbolChanged || topImageChanged || bottomImageChanged
    },
    _getSettingsForTwoMarkers: function(style) {
        var options = this._options;
        var settings = {};
        var x = options.rotated ? _min(this.x, this.minX) : this.x;
        var y = options.rotated ? this.y : _min(this.y, this.minY);
        var radius = style.r;
        var points = this._populatePointShape(options.symbol, radius);
        settings.top = _extend({
            translateX: x + this.width,
            translateY: y,
            r: radius
        }, style);
        settings.bottom = _extend({
            translateX: x,
            translateY: y + this.height,
            r: radius
        }, style);
        if (points) {
            settings.top.points = settings.bottom.points = points
        }
        return settings
    },
    _hasGraphic: function() {
        return this.graphic && this.graphic.topMarker && this.graphic.bottomMarker
    },
    _drawOneMarker: function(renderer, markerType, imageSettings, settings) {
        var graphic = this.graphic;
        if (graphic[markerType]) {
            this._updateOneMarker(markerType, settings)
        } else {
            graphic[markerType] = this._createMarker(renderer, graphic, imageSettings, settings)
        }
    },
    _drawMarker: function(renderer, group, animationEnabled, firstDrawing, style) {
        var settings = this._getSettingsForTwoMarkers(style || this._getStyle());
        var image = this._getImage(this._options.image);
        if (this._checkImage(image.top)) {
            settings.top = this._getImageSettings(settings.top, image.top)
        }
        if (this._checkImage(image.bottom)) {
            settings.bottom = this._getImageSettings(settings.bottom, image.bottom)
        }
        this.graphic = this.graphic || renderer.g().append(group);
        this.visibleTopMarker && this._drawOneMarker(renderer, "topMarker", image.top, settings.top);
        this.visibleBottomMarker && this._drawOneMarker(renderer, "bottomMarker", image.bottom, settings.bottom)
    },
    _getSettingsForTracker: function(radius) {
        var rotated = this._options.rotated;
        return {
            translateX: rotated ? _min(this.x, this.minX) - radius : this.x - radius,
            translateY: rotated ? this.y - radius : _min(this.y, this.minY) - radius,
            width: this.width + 2 * radius,
            height: this.height + 2 * radius
        }
    },
    isInVisibleArea: function() {
        var rotated = this._options.rotated;
        var argument = !rotated ? this.x : this.y;
        var maxValue = !rotated ? _max(this.minY, this.y) : _max(this.minX, this.x);
        var minValue = !rotated ? _min(this.minY, this.y) : _min(this.minX, this.x);
        var tmp;
        var visibleTopMarker;
        var visibleBottomMarker;
        var visibleRangeArea = true;
        var visibleArgArea = this.series.getArgumentAxis().getVisibleArea();
        var visibleValArea = this.series.getValueAxis().getVisibleArea();
        var notVisibleByArg = visibleArgArea[1] < argument || visibleArgArea[0] > argument;
        var notVisibleByVal = visibleValArea[0] > minValue && visibleValArea[0] > maxValue || visibleValArea[1] < minValue && visibleValArea[1] < maxValue;
        if (notVisibleByArg || notVisibleByVal) {
            visibleTopMarker = visibleBottomMarker = visibleRangeArea = false
        } else {
            visibleTopMarker = visibleValArea[0] <= minValue && visibleValArea[1] > minValue;
            visibleBottomMarker = visibleValArea[0] < maxValue && visibleValArea[1] >= maxValue;
            if (rotated) {
                tmp = visibleTopMarker;
                visibleTopMarker = visibleBottomMarker;
                visibleBottomMarker = tmp
            }
        }
        this.visibleTopMarker = visibleTopMarker;
        this.visibleBottomMarker = visibleBottomMarker;
        return visibleRangeArea
    },
    getTooltipParams: function() {
        var x;
        var y;
        var rotated = this._options.rotated;
        var minValue = !rotated ? _min(this.y, this.minY) : _min(this.x, this.minX);
        var side = !rotated ? "height" : "width";
        var visibleArea = this._getVisibleArea();
        var minVisible = rotated ? visibleArea.minX : visibleArea.minY;
        var maxVisible = rotated ? visibleArea.maxX : visibleArea.maxY;
        var min = _max(minVisible, minValue);
        var max = _min(maxVisible, minValue + this[side]);
        if (!rotated) {
            x = this.x;
            y = min + (max - min) / 2
        } else {
            y = this.y;
            x = min + (max - min) / 2
        }
        return {
            x: x,
            y: y,
            offset: 0
        }
    },
    _translate: function() {
        var rotated = this._options.rotated;
        _symbol_point.default._translate.call(this);
        this.height = rotated ? 0 : _abs(this.minY - this.y);
        this.width = rotated ? _abs(this.x - this.minX) : 0
    },
    hasCoords: function() {
        return _symbol_point.default.hasCoords.call(this) && !(null === this.minX || null === this.minY)
    },
    _updateData: function(data) {
        _symbol_point.default._updateData.call(this, data);
        this.minValue = this.initialMinValue = this.originalMinValue = data.minValue
    },
    _getImageSettings: function(settings, image) {
        return {
            href: image.url || image.toString(),
            width: image.width || DEFAULT_IMAGE_WIDTH,
            height: image.height || DEFAULT_IMAGE_HEIGHT,
            translateX: settings.translateX,
            translateY: settings.translateY
        }
    },
    getCrosshairData: function(x, y) {
        var rotated = this._options.rotated;
        var minX = this.minX;
        var minY = this.minY;
        var vx = this.vx;
        var vy = this.vy;
        var value = this.value;
        var minValue = this.minValue;
        var argument = this.argument;
        var coords = {
            axis: this.series.axis,
            x: vx,
            y: vy,
            yValue: value,
            xValue: argument
        };
        if (rotated) {
            coords.yValue = argument;
            if (_abs(vx - x) < _abs(minX - x)) {
                coords.xValue = value
            } else {
                coords.x = minX;
                coords.xValue = minValue
            }
        } else if (_abs(vy - y) >= _abs(minY - y)) {
            coords.y = minY;
            coords.yValue = minValue
        }
        return coords
    },
    _updateOneMarker: function(markerType, settings) {
        this.graphic && this.graphic[markerType] && this.graphic[markerType].attr(settings)
    },
    _updateMarker: function(animationEnabled, style) {
        this._drawMarker(void 0, void 0, false, false, style)
    },
    _getFormatObject: function(tooltip) {
        var initialMinValue = this.initialMinValue;
        var initialValue = this.initialValue;
        var initialArgument = this.initialArgument;
        var minValue = tooltip.formatValue(initialMinValue);
        var value = tooltip.formatValue(initialValue);
        return {
            argument: initialArgument,
            argumentText: tooltip.formatValue(initialArgument, "argument"),
            valueText: minValue + " - " + value,
            rangeValue1Text: minValue,
            rangeValue2Text: value,
            rangeValue1: initialMinValue,
            rangeValue2: initialValue,
            seriesName: this.series.name,
            point: this,
            originalMinValue: this.originalMinValue,
            originalValue: this.originalValue,
            originalArgument: this.originalArgument
        }
    },
    getLabel: function() {
        return [this._topLabel, this._bottomLabel]
    },
    getLabels: function() {
        return [this._topLabel, this._bottomLabel]
    },
    getBoundingRect: _common.noop,
    coordsIn: function(x, y) {
        var trackerRadius = this._storeTrackerR();
        var xCond = x >= this.x - trackerRadius && x <= this.x + trackerRadius;
        var yCond = y >= this.y - trackerRadius && y <= this.y + trackerRadius;
        if (this._options.rotated) {
            return yCond && (xCond || x >= this.minX - trackerRadius && x <= this.minX + trackerRadius)
        } else {
            return xCond && (yCond || y >= this.minY - trackerRadius && y <= this.minY + trackerRadius)
        }
    },
    getMaxValue: function() {
        if ("discrete" !== this.series.valueAxisType) {
            return this.minValue > this.value ? this.minValue : this.value
        }
        return this.value
    },
    getMinValue: function() {
        if ("discrete" !== this.series.valueAxisType) {
            return this.minValue < this.value ? this.minValue : this.value
        }
        return this.minValue
    }
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
