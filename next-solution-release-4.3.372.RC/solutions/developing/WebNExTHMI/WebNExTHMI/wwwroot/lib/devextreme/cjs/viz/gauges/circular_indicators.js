/**
 * DevExtreme (cjs/viz/gauges/circular_indicators.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.twocolorneedle = exports.triangleneedle = exports.trianglemarker = exports.textcloud = exports.rectangleneedle = exports.rangebar = exports._default = void 0;
var _base_indicators = require("./base_indicators");
var _utils = require("../core/utils");
var _Number = Number;
var _getCosAndSin = _utils.getCosAndSin;
var _convertAngleToRendererSpace = _utils.convertAngleToRendererSpace;

function correctRadius(layout, size) {
    if (layout && layout.radius - size <= 0) {
        layout.radius = size + 1
    }
    return layout
}
var SimpleIndicator = _base_indicators.BaseIndicator.inherit({
    _move: function() {
        var options = this._options;
        var angle = _convertAngleToRendererSpace(this._actualPosition);
        this._rootElement.rotate(angle, options.x, options.y);
        this._trackerElement && this._trackerElement.rotate(angle, options.x, options.y)
    },
    _isEnabled: function() {
        return this._options.width > 0
    },
    _isVisible: function(layout) {
        return layout.radius - _Number(this._options.indentFromCenter) > 0
    },
    _getTrackerSettings: function() {
        var options = this._options;
        var radius = this._getRadius();
        var indentFromCenter = this._getIndentFromCenter();
        var x = options.x;
        var y = options.y - (radius + indentFromCenter) / 2;
        var width = options.width / 2;
        var length = (radius - indentFromCenter) / 2;
        width > 10 || (width = 10);
        length > 10 || (length = 10);
        return {
            points: [x - width, y - length, x - width, y + length, x + width, y + length, x + width, y - length]
        }
    },
    _render: function() {
        this._renderPointer()
    },
    _clearPointer: function() {
        delete this._element
    },
    _clear: function() {
        this._clearPointer()
    },
    _getIndentFromCenter: function(radius) {
        return Number(this._options.indentFromCenter) || 0
    },
    _getRadius: function() {
        return 0
    },
    measure: function(layout) {
        var result = {
            max: layout.radius
        };
        if (this._options.indentFromCenter < 0) {
            result.inverseHorizontalOffset = result.inverseVerticalOffset = -_Number(this._options.indentFromCenter)
        }
        return result
    },
    getTooltipParameters: function() {
        var options = this._options;
        var cosSin = _getCosAndSin(this._actualPosition);
        var r = (this._getRadius() + this._getIndentFromCenter()) / 2;
        return {
            x: options.x + cosSin.cos * r,
            y: options.y - cosSin.sin * r,
            value: this._currentValue,
            color: options.color,
            offset: options.width / 2
        }
    }
});
var NeedleIndicator = SimpleIndicator.inherit({
    _isVisible: function(layout) {
        var indentFromCenter = this._adjustOffset(Number(this._options.indentFromCenter), layout.radius);
        var offset = this._adjustOffset(Number(this._options.offset), layout.radius);
        return layout.radius - indentFromCenter - offset > 0
    },
    getOffset: function() {
        return 0
    },
    _adjustOffset: function(value, radius) {
        var minRadius = Number(this._options.beginAdaptingAtRadius);
        var diff = radius / minRadius;
        if (diff < 1) {
            value = Math.floor(value * diff)
        }
        return value || 0
    },
    _getIndentFromCenter: function(radius) {
        return this._adjustOffset(Number(this._options.indentFromCenter), this._options.radius)
    },
    _getRadius: function() {
        var options = this._options;
        return options.radius - this._adjustOffset(Number(options.offset), options.radius)
    },
    _renderSpindle: function() {
        var options = this._options;
        var radius = options.radius;
        var spindleSize = 2 * this._adjustOffset(_Number(options.spindleSize) / 2, radius);
        var gapSize = 2 * this._adjustOffset(_Number(options.spindleGapSize) / 2, radius) || 0;
        if (gapSize > 0) {
            gapSize = gapSize <= spindleSize ? gapSize : spindleSize
        }
        if (spindleSize > 0) {
            this._spindleOuter = this._spindleOuter || this._renderer.circle().append(this._rootElement);
            this._spindleInner = this._spindleInner || this._renderer.circle().append(this._rootElement);
            this._spindleOuter.attr({
                class: "dxg-spindle-border",
                cx: options.x,
                cy: options.y,
                r: spindleSize / 2
            });
            this._spindleInner.attr({
                class: "dxg-spindle-hole",
                cx: options.x,
                cy: options.y,
                r: gapSize / 2,
                fill: options.containerBackgroundColor
            })
        }
    },
    _render: function() {
        this.callBase();
        this._renderSpindle()
    },
    _clear: function() {
        this.callBase();
        delete this._spindleOuter;
        delete this._spindleInner
    }
});
var rectangleNeedle = NeedleIndicator.inherit({
    _renderPointer: function() {
        var options = this._options;
        var y2 = options.y - this._getRadius();
        var y1 = options.y - this._getIndentFromCenter();
        var x1 = options.x - options.width / 2;
        var x2 = x1 + _Number(options.width);
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        this._element.attr({
            points: [x1, y1, x1, y2, x2, y2, x2, y1]
        })
    }
});
exports.rectangleneedle = exports._default = rectangleNeedle;
var triangleNeedle = NeedleIndicator.inherit({
    _renderPointer: function() {
        var options = this._options;
        var y2 = options.y - this._getRadius();
        var y1 = options.y - this._getIndentFromCenter();
        var x1 = options.x - options.width / 2;
        var x2 = options.x + options.width / 2;
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        this._element.attr({
            points: [x1, y1, options.x, y2, x2, y1]
        })
    }
});
exports.triangleneedle = triangleNeedle;
var twoColorNeedle = NeedleIndicator.inherit({
    _renderPointer: function() {
        var options = this._options;
        var x1 = options.x - options.width / 2;
        var x2 = options.x + options.width / 2;
        var y4 = options.y - this._getRadius();
        var y1 = options.y - this._getIndentFromCenter();
        var fraction = _Number(options.secondFraction) || 0;
        var y2;
        var y3;
        if (fraction >= 1) {
            y2 = y3 = y1
        } else if (fraction <= 0) {
            y2 = y3 = y4
        } else {
            y3 = y4 + (y1 - y4) * fraction;
            y2 = y3 + _Number(options.space)
        }
        this._firstElement = this._firstElement || this._renderer.path([], "area").append(this._rootElement);
        this._spaceElement = this._spaceElement || this._renderer.path([], "area").append(this._rootElement);
        this._secondElement = this._secondElement || this._renderer.path([], "area").append(this._rootElement);
        this._firstElement.attr({
            points: [x1, y1, x1, y2, x2, y2, x2, y1]
        });
        this._spaceElement.attr({
            points: [x1, y2, x1, y3, x2, y3, x2, y2],
            class: "dxg-hole",
            fill: options.containerBackgroundColor
        });
        this._secondElement.attr({
            points: [x1, y3, x1, y4, x2, y4, x2, y3],
            class: "dxg-part",
            fill: options.secondColor
        })
    },
    _clearPointer: function() {
        delete this._firstElement;
        delete this._secondElement;
        delete this._spaceElement
    }
});
exports.twocolorneedle = twoColorNeedle;
var triangleMarker = SimpleIndicator.inherit({
    _isEnabled: function() {
        return this._options.length > 0 && this._options.width > 0
    },
    _isVisible: function(layout) {
        return true
    },
    resize: function(layout) {
        return this.callBase(correctRadius(layout, 0))
    },
    _render: function() {
        var options = this._options;
        var x = options.x;
        var y1 = options.y - options.radius;
        var dx = options.width / 2 || 0;
        var y2 = y1 - _Number(options.length);
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        var settings = {
            points: [x, y1, x - dx, y2, x + dx, y2],
            stroke: "none",
            "stroke-width": 0,
            "stroke-linecap": "square"
        };
        if (options.space > 0) {
            settings["stroke-width"] = Math.min(options.space, options.width / 4) || 0;
            settings.stroke = settings["stroke-width"] > 0 ? options.containerBackgroundColor || "none" : "none"
        }
        this._element.attr(settings).sharp()
    },
    _clear: function() {
        delete this._element
    },
    _getTrackerSettings: function() {
        var options = this._options;
        var x = options.x;
        var y = options.y - options.radius - options.length / 2;
        var width = options.width / 2;
        var length = options.length / 2;
        width > 10 || (width = 10);
        length > 10 || (length = 10);
        return {
            points: [x - width, y - length, x - width, y + length, x + width, y + length, x + width, y - length]
        }
    },
    measure: function(layout) {
        return {
            min: layout.radius,
            max: layout.radius + _Number(this._options.length)
        }
    },
    getTooltipParameters: function() {
        var options = this._options;
        var cosSin = _getCosAndSin(this._actualPosition);
        var r = options.radius + options.length / 2;
        var parameters = this.callBase();
        parameters.x = options.x + cosSin.cos * r;
        parameters.y = options.y - cosSin.sin * r;
        parameters.offset = options.length / 2;
        return parameters
    }
});
exports.trianglemarker = triangleMarker;
var textCloud = _base_indicators.BaseTextCloudMarker.inherit({
    _isEnabled: function() {
        return true
    },
    _isVisible: function(layout) {
        return true
    },
    resize: function(layout) {
        return this.callBase(correctRadius(layout, 0))
    },
    _getTextCloudOptions: function() {
        var cosSin = _getCosAndSin(this._actualPosition);
        var nAngle = (0, _utils.normalizeAngle)(this._actualPosition);
        return {
            x: this._options.x + cosSin.cos * this._options.radius,
            y: this._options.y - cosSin.sin * this._options.radius,
            type: nAngle > 270 ? "left-top" : nAngle > 180 ? "top-right" : nAngle > 90 ? "right-bottom" : "bottom-left"
        }
    },
    measure: function(layout) {
        var arrowLength = _Number(this._options.arrowLength) || 0;
        this._measureText();
        var verticalOffset = this._textFullHeight + arrowLength;
        var horizontalOffset = this._textFullWidth + arrowLength;
        return {
            min: layout.radius,
            max: layout.radius,
            horizontalOffset: horizontalOffset,
            verticalOffset: verticalOffset,
            inverseHorizontalOffset: horizontalOffset,
            inverseVerticalOffset: verticalOffset
        }
    }
});
exports.textcloud = textCloud;
var rangeBar = _base_indicators.BaseRangeBar.inherit({
    _isEnabled: function() {
        return this._options.size > 0
    },
    _isVisible: function(layout) {
        return true
    },
    resize: function(layout) {
        return this.callBase(correctRadius(layout, _Number(this._options.size)))
    },
    _createBarItem: function() {
        return this._renderer.arc().attr({
            "stroke-linejoin": "round"
        }).append(this._rootElement)
    },
    _createTracker: function() {
        return this._renderer.arc().attr({
            "stroke-linejoin": "round"
        })
    },
    _setBarSides: function() {
        this._maxSide = this._options.radius;
        this._minSide = this._maxSide - _Number(this._options.size)
    },
    _getSpace: function() {
        var options = this._options;
        return options.space > 0 ? 180 * options.space / options.radius / Math.PI : 0
    },
    _isTextVisible: function() {
        var options = this._options.text || {};
        return options.indent > 0
    },
    _setTextItemsSides: function() {
        var options = this._options;
        var indent = _Number(options.text.indent);
        this._lineFrom = options.y - options.radius;
        this._lineTo = this._lineFrom - indent;
        this._textRadius = options.radius + indent
    },
    _getPositions: function() {
        var basePosition = this._basePosition;
        var actualPosition = this._actualPosition;
        var mainPosition1;
        var mainPosition2;
        if (basePosition >= actualPosition) {
            mainPosition1 = basePosition;
            mainPosition2 = actualPosition
        } else {
            mainPosition1 = actualPosition;
            mainPosition2 = basePosition
        }
        return {
            start: this._startPosition,
            end: this._endPosition,
            main1: mainPosition1,
            main2: mainPosition2,
            back1: Math.min(mainPosition1 + this._space, this._startPosition),
            back2: Math.max(mainPosition2 - this._space, this._endPosition)
        }
    },
    _buildItemSettings: function(from, to) {
        return {
            x: this._options.x,
            y: this._options.y,
            innerRadius: this._minSide,
            outerRadius: this._maxSide,
            startAngle: to,
            endAngle: from
        }
    },
    _updateTextPosition: function() {
        var cosSin = _getCosAndSin(this._actualPosition);
        var x = this._options.x + this._textRadius * cosSin.cos;
        var y = this._options.y - this._textRadius * cosSin.sin;
        x += cosSin.cos * this._textWidth * .6;
        y -= cosSin.sin * this._textHeight * .6;
        this._text.attr({
            x: x,
            y: y + this._textVerticalOffset
        })
    },
    _updateLinePosition: function() {
        var x = this._options.x;
        var x1;
        var x2;
        if (this._basePosition > this._actualPosition) {
            x1 = x - 2;
            x2 = x
        } else if (this._basePosition < this._actualPosition) {
            x1 = x;
            x2 = x + 2
        } else {
            x1 = x - 1;
            x2 = x + 1
        }
        this._line.attr({
            points: [x1, this._lineFrom, x1, this._lineTo, x2, this._lineTo, x2, this._lineFrom]
        }).rotate(_convertAngleToRendererSpace(this._actualPosition), x, this._options.y).sharp()
    },
    _getTooltipPosition: function() {
        var cosSin = _getCosAndSin((this._basePosition + this._actualPosition) / 2);
        var r = (this._minSide + this._maxSide) / 2;
        return {
            x: this._options.x + cosSin.cos * r,
            y: this._options.y - cosSin.sin * r
        }
    },
    measure: function(layout) {
        var result = {
            min: layout.radius - _Number(this._options.size),
            max: layout.radius
        };
        this._measureText();
        if (this._hasText) {
            result.max += _Number(this._options.text.indent);
            result.horizontalOffset = this._textWidth;
            result.verticalOffset = this._textHeight
        }
        return result
    }
});
exports.rangebar = rangeBar;
