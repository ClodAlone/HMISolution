/**
 * DevExtreme (esm/viz/series/points/pie_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../../core/utils/extend";
import symbolPoint from "./symbol_point";
var _extend = extend;
var _round = Math.round;
var _sqrt = Math.sqrt;
var _acos = Math.acos;
var DEG = 180 / Math.PI;
var _abs = Math.abs;
import {
    getVerticallyShiftedAngularCoords,
    normalizeAngle as _normalizeAngle,
    getCosAndSin as _getCosAndSin
} from "../../core/utils";
import {
    isDefined as _isDefined
} from "../../../core/utils/type";
import consts from "../../components/consts";
var RADIAL_LABEL_INDENT = consts.radialLabelIndent;
export default _extend({}, symbolPoint, {
    _updateData: function(data, argumentChanged) {
        symbolPoint._updateData.call(this, data);
        if (argumentChanged || !_isDefined(this._visible)) {
            this._visible = true
        }
        this.minValue = this.initialMinValue = this.originalMinValue = _isDefined(data.minValue) ? data.minValue : 0
    },
    animate: function(complete, duration, delay) {
        this.graphic.animate({
            x: this.centerX,
            y: this.centerY,
            outerRadius: this.radiusOuter,
            innerRadius: this.radiusInner,
            startAngle: this.toAngle,
            endAngle: this.fromAngle
        }, {
            delay: delay,
            partitionDuration: duration
        }, complete)
    },
    correctPosition: function(correction) {
        this.correctRadius(correction);
        this.correctLabelRadius(correction.radiusOuter + RADIAL_LABEL_INDENT);
        this.centerX = correction.centerX;
        this.centerY = correction.centerY
    },
    correctRadius: function(correction) {
        this.radiusInner = correction.radiusInner;
        this.radiusOuter = correction.radiusOuter
    },
    correctLabelRadius: function(radiusLabels) {
        this.radiusLabels = radiusLabels
    },
    correctValue: function(correction, percent, base) {
        this.value = (base || this.normalInitialValue) + correction;
        this.minValue = correction;
        this.percent = percent;
        this._label.setDataField("percent", percent)
    },
    _updateLabelData: function() {
        this._label.setData(this._getLabelFormatObject())
    },
    _getShiftLabelCoords: function() {
        var bBox = this._label.getBoundingRect();
        var coord = this._getLabelCoords(this._label);
        var visibleArea = this._getVisibleArea();
        if (this._isLabelDrawingWithoutPoints) {
            return this._checkLabelPosition(coord, bBox, visibleArea)
        } else {
            return this._getLabelExtraCoord(coord, this._checkVerticalLabelPosition(coord, bBox, visibleArea), bBox)
        }
    },
    _getLabelPosition: function(options) {
        return options.position
    },
    getAnnotationCoords: function(location) {
        return this._getElementCoords("edge" !== location ? "inside" : "outside", this.radiusOuter, 0)
    },
    _getElementCoords: function(position, elementRadius, radialOffset) {
        var bBox = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : {
            x: 0,
            y: 0,
            width: 0,
            height: 0
        };
        var that = this;
        var angleFunctions = _getCosAndSin(that.middleAngle);
        var radiusInner = that.radiusInner;
        var radiusOuter = that.radiusOuter;
        var columnsPosition = "columns" === position;
        var rad;
        var x;
        if ("inside" === position) {
            rad = radiusInner + (radiusOuter - radiusInner) / 2 + radialOffset;
            x = that.centerX + rad * angleFunctions.cos - bBox.width / 2
        } else {
            rad = elementRadius + radialOffset;
            if (angleFunctions.cos > .1 || columnsPosition && angleFunctions.cos >= 0) {
                x = that.centerX + rad * angleFunctions.cos
            } else if (angleFunctions.cos < -.1 || columnsPosition && angleFunctions.cos < 0) {
                x = that.centerX + rad * angleFunctions.cos - bBox.width
            } else {
                x = that.centerX + rad * angleFunctions.cos - bBox.width / 2
            }
        }
        return {
            x: x,
            y: _round(that.centerY - rad * angleFunctions.sin - bBox.height / 2)
        }
    },
    _getLabelCoords: function(label) {
        var bBox = label.getBoundingRect();
        var options = label.getLayoutOptions();
        var position = this._getLabelPosition(options);
        return this._getElementCoords(position, this.radiusLabels, options.radialOffset, bBox)
    },
    _correctLabelCoord: function(coord, moveLabelsFromCenter) {
        var label = this._label;
        var bBox = label.getBoundingRect();
        var labelWidth = bBox.width;
        var options = label.getLayoutOptions();
        var visibleArea = this._getVisibleArea();
        var rightBorderX = visibleArea.maxX - labelWidth;
        var leftBorderX = visibleArea.minX;
        var angleOfPoint = _normalizeAngle(this.middleAngle);
        var centerX = this.centerX;
        var connectorOffset = options.connectorOffset;
        var x = coord.x;
        if ("columns" === options.position) {
            if (angleOfPoint <= 90 || angleOfPoint >= 270) {
                x = rightBorderX
            } else {
                x = leftBorderX
            }
            coord.x = x
        } else if ("inside" !== options.position && moveLabelsFromCenter) {
            if (angleOfPoint <= 90 || angleOfPoint >= 270) {
                if (x - connectorOffset < centerX) {
                    x = centerX + connectorOffset
                }
            } else if (x + labelWidth + connectorOffset > centerX) {
                x = centerX - labelWidth - connectorOffset
            }
            coord.x = x
        }
        return coord
    },
    drawLabel: function() {
        this.translate();
        this._isLabelDrawingWithoutPoints = true;
        this._drawLabel();
        this._isLabelDrawingWithoutPoints = false
    },
    updateLabelCoord: function(moveLabelsFromCenter) {
        var bBox = this._label.getBoundingRect();
        var coord = this._correctLabelCoord(bBox, moveLabelsFromCenter);
        coord = this._checkHorizontalLabelPosition(coord, bBox, this._getVisibleArea());
        this._label.shift(_round(coord.x), _round(bBox.y))
    },
    _checkVerticalLabelPosition: function(coord, box, visibleArea) {
        var x = coord.x;
        var y = coord.y;
        if (coord.y + box.height > visibleArea.maxY) {
            y = visibleArea.maxY - box.height
        } else if (coord.y < visibleArea.minY) {
            y = visibleArea.minY
        }
        return {
            x: x,
            y: y
        }
    },
    _getLabelExtraCoord: function(coord, shiftCoord, box) {
        return coord.y !== shiftCoord.y ? getVerticallyShiftedAngularCoords({
            x: coord.x,
            y: coord.y,
            width: box.width,
            height: box.height
        }, shiftCoord.y - coord.y, {
            x: this.centerX,
            y: this.centerY
        }) : coord
    },
    _checkHorizontalLabelPosition: function(coord, box, visibleArea) {
        var x = coord.x;
        var y = coord.y;
        if (coord.x + box.width > visibleArea.maxX) {
            x = visibleArea.maxX - box.width
        } else if (coord.x < visibleArea.minX) {
            x = visibleArea.minX
        }
        return {
            x: x,
            y: y
        }
    },
    applyWordWrap: function(moveLabelsFromCenter) {
        var label = this._label;
        var box = label.getBoundingRect();
        var visibleArea = this._getVisibleArea();
        var position = label.getLayoutOptions().position;
        var width = box.width;
        var rowCountChanged = false;
        if ("columns" === position && this.series.index > 0) {
            width = visibleArea.maxX - this.centerX - this.radiusLabels
        } else if ("inside" === position) {
            if (width > visibleArea.maxX - visibleArea.minX) {
                width = visibleArea.maxX - visibleArea.minX
            }
        } else if (moveLabelsFromCenter && box.x < this.centerX && box.width + box.x > this.centerX) {
            width = Math.floor((visibleArea.maxX - visibleArea.minX) / 2)
        } else if (box.x + width > visibleArea.maxX) {
            width = visibleArea.maxX - box.x
        } else if (box.x < visibleArea.minX) {
            width = box.x + width - visibleArea.minX
        }
        if (width < box.width) {
            rowCountChanged = label.fit(width)
        }
        return rowCountChanged
    },
    setLabelTrackerData: function() {
        this._label.setTrackerData(this)
    },
    _checkLabelPosition: function(coord, bBox, visibleArea) {
        coord = this._checkHorizontalLabelPosition(coord, bBox, visibleArea);
        return this._checkVerticalLabelPosition(coord, bBox, visibleArea)
    },
    _getLabelConnector: function() {
        var rad = this.radiusOuter;
        var seriesStyle = this._options.styles.normal;
        var strokeWidthBy2 = seriesStyle["stroke-width"] / 2;
        var borderWidth = this.series.getOptions().containerBackgroundColor === seriesStyle.stroke ? _round(strokeWidthBy2) : _round(-strokeWidthBy2);
        var angleFunctions = _getCosAndSin(_round(this.middleAngle));
        return {
            x: _round(this.centerX + (rad - borderWidth) * angleFunctions.cos),
            y: _round(this.centerY - (rad - borderWidth) * angleFunctions.sin),
            angle: this.middleAngle
        }
    },
    _drawMarker: function(renderer, group, animationEnabled, firstDrawing) {
        var radiusOuter = this.radiusOuter;
        var radiusInner = this.radiusInner;
        var fromAngle = this.fromAngle;
        var toAngle = this.toAngle;
        if (animationEnabled) {
            radiusInner = radiusOuter = 0;
            if (!firstDrawing) {
                fromAngle = toAngle = this.shiftedAngle
            }
        }
        this.graphic = renderer.arc(this.centerX, this.centerY, radiusInner, radiusOuter, toAngle, fromAngle).attr({
            "stroke-linejoin": "round"
        }).smartAttr(this._getStyle()).data({
            "chart-data-point": this
        }).sharp().append(group)
    },
    getTooltipParams: function() {
        var angleFunctions = _getCosAndSin(this.middleAngle);
        var radiusInner = this.radiusInner;
        var radiusOuter = this.radiusOuter;
        return {
            x: this.centerX + (radiusInner + (radiusOuter - radiusInner) / 2) * angleFunctions.cos,
            y: this.centerY - (radiusInner + (radiusOuter - radiusInner) / 2) * angleFunctions.sin,
            offset: 0
        }
    },
    _translate: function() {
        var angle = this.shiftedAngle || 0;
        var value = this.value;
        var minValue = this.minValue;
        var translator = this._getValTranslator();
        this.fromAngle = translator.translate(minValue) + angle;
        this.toAngle = translator.translate(value) + angle;
        this.middleAngle = translator.translate((value - minValue) / 2 + minValue) + angle;
        if (!this.isVisible()) {
            this.middleAngle = this.toAngle = this.fromAngle = this.fromAngle || angle
        }
    },
    getMarkerVisibility: function() {
        return true
    },
    _updateMarker: function(animationEnabled, style, _, callback) {
        if (!animationEnabled) {
            style = _extend({
                x: this.centerX,
                y: this.centerY,
                outerRadius: this.radiusOuter,
                innerRadius: this.radiusInner,
                startAngle: this.toAngle,
                endAngle: this.fromAngle
            }, style)
        }
        this.graphic.smartAttr(style).sharp();
        callback && callback()
    },
    getLegendStyles: function() {
        return this._styles.legendStyles
    },
    isInVisibleArea: function() {
        return true
    },
    hide: function() {
        if (this._visible) {
            this._visible = false;
            this.hideTooltip();
            this._options.visibilityChanged()
        }
    },
    show: function() {
        if (!this._visible) {
            this._visible = true;
            this._options.visibilityChanged()
        }
    },
    setInvisibility: function() {
        this._label.draw(false)
    },
    isVisible: function() {
        return this._visible
    },
    _getFormatObject: function(tooltip) {
        var formatObject = symbolPoint._getFormatObject.call(this, tooltip);
        var percent = this.percent;
        formatObject.percent = percent;
        formatObject.percentText = tooltip.formatValue(percent, "percent");
        return formatObject
    },
    getColor: function() {
        return this._styles.normal.fill
    },
    coordsIn: function(x, y) {
        var lx = x - this.centerX;
        var ly = y - this.centerY;
        var r = _sqrt(lx * lx + ly * ly);
        var fromAngle = this.fromAngle % 360;
        var toAngle = this.toAngle % 360;
        var angle;
        if (r < this.radiusInner || r > this.radiusOuter || 0 === r) {
            return false
        }
        angle = _acos(lx / r) * DEG * (ly > 0 ? -1 : 1);
        if (angle < 0) {
            angle += 360
        }
        if (fromAngle === toAngle && _abs(this.toAngle - this.fromAngle) > 1e-4) {
            return true
        } else {
            return fromAngle >= toAngle ? angle <= fromAngle && angle >= toAngle : !(angle >= fromAngle && angle <= toAngle)
        }
    }
});
