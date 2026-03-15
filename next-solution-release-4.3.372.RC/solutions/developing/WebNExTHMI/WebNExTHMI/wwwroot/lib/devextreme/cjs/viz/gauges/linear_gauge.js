/**
 * DevExtreme (cjs/viz/gauges/linear_gauge.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}
exports.default = void 0;
var _iterator = require("../../core/utils/iterator");
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _extend = require("../../core/utils/extend");
var _object = require("../../core/utils/object");
var _base_gauge = require("./base_gauge");
var _common = require("./common");
var _utils = require("../core/utils");
var linearIndicators = _interopRequireWildcard(require("./linear_indicators"));
var _linear_range_container = _interopRequireDefault(require("./linear_range_container"));

function _getRequireWildcardCache(nodeInterop) {
    if ("function" !== typeof WeakMap) {
        return null
    }
    var cacheBabelInterop = new WeakMap;
    var cacheNodeInterop = new WeakMap;
    return (_getRequireWildcardCache = function(nodeInterop) {
        return nodeInterop ? cacheNodeInterop : cacheBabelInterop
    })(nodeInterop)
}

function _interopRequireWildcard(obj, nodeInterop) {
    if (!nodeInterop && obj && obj.__esModule) {
        return obj
    }
    if (null === obj || "object" !== _typeof(obj) && "function" !== typeof obj) {
        return {
            default: obj
        }
    }
    var cache = _getRequireWildcardCache(nodeInterop);
    if (cache && cache.has(obj)) {
        return cache.get(obj)
    }
    var newObj = {};
    var hasPropertyDescriptor = Object.defineProperty && Object.getOwnPropertyDescriptor;
    for (var key in obj) {
        if ("default" !== key && Object.prototype.hasOwnProperty.call(obj, key)) {
            var desc = hasPropertyDescriptor ? Object.getOwnPropertyDescriptor(obj, key) : null;
            if (desc && (desc.get || desc.set)) {
                Object.defineProperty(newObj, key, desc)
            } else {
                newObj[key] = obj[key]
            }
        }
    }
    newObj.default = obj;
    if (cache) {
        cache.set(obj, newObj)
    }
    return newObj
}

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _max = Math.max;
var _min = Math.min;
var _round = Math.round;
var dxLinearGauge = _common.dxGauge.inherit({
    _rootClass: "dxg-linear-gauge",
    _factoryMethods: {
        rangeContainer: "createLinearRangeContainer",
        indicator: "createLinearIndicator"
    },
    _gridSpacingFactor: 25,
    _scaleTypes: {
        type: "xyAxes",
        drawingType: "linear"
    },
    _getTicksOrientation: function(scaleOptions) {
        return scaleOptions.isHorizontal ? scaleOptions.verticalOrientation : scaleOptions.horizontalOrientation
    },
    _getThemeManagerOptions: function() {
        var options = this.callBase.apply(this, arguments);
        options.subTheme = "_linear";
        return options
    },
    _getInvertedState: function() {
        return !this._area.vertical && this.option("rtlEnabled")
    },
    _prepareScaleSettings: function() {
        var scaleOptions = this.callBase();
        scaleOptions.inverted = this._getInvertedState();
        return scaleOptions
    },
    _updateScaleTickIndent: function(scaleOptions) {
        var indentFromTick = scaleOptions.label.indentFromTick;
        var length = scaleOptions.tick.length;
        var textParams = this._scale.measureLabels((0, _extend.extend)({}, this._canvas));
        var verticalTextCorrection = scaleOptions.isHorizontal ? textParams.height + textParams.y : 0;
        var isIndentPositive = indentFromTick > 0;
        var orientation;
        var textCorrection;
        var tickCorrection;
        if (scaleOptions.isHorizontal) {
            orientation = isIndentPositive ? {
                center: .5,
                top: 0,
                bottom: 1
            } : {
                center: .5,
                top: 1,
                bottom: 0
            };
            tickCorrection = length * orientation[scaleOptions.verticalOrientation];
            textCorrection = textParams.y
        } else {
            orientation = isIndentPositive ? {
                center: .5,
                left: 0,
                right: 1
            } : {
                center: .5,
                left: 1,
                right: 0
            };
            tickCorrection = length * orientation[scaleOptions.horizontalOrientation];
            textCorrection = -textParams.width
        }
        scaleOptions.label.indentFromAxis = -indentFromTick + (isIndentPositive ? -tickCorrection + textCorrection : tickCorrection - verticalTextCorrection);
        this._scale.updateOptions(scaleOptions)
    },
    _shiftScale: function(layout, scaleOptions) {
        var canvas = (0, _extend.extend)({}, this._canvas);
        var isHorizontal = scaleOptions.isHorizontal;
        var scale = this._scale;
        canvas[isHorizontal ? "left" : "top"] = this._area[isHorizontal ? "startCoord" : "endCoord"];
        canvas[isHorizontal ? "right" : "bottom"] = canvas[isHorizontal ? "width" : "height"] - this._area[isHorizontal ? "endCoord" : "startCoord"];
        scale.draw(canvas);
        scale.shift({
            left: -layout.x,
            top: -layout.y
        })
    },
    _setupCodomain: function() {
        var geometry = this.option("geometry") || {};
        var vertical = "vertical" === (0, _utils.normalizeEnum)(geometry.orientation);
        this._area = {
            vertical: vertical,
            x: 0,
            y: 0,
            startCoord: -100,
            endCoord: 100
        };
        this._rangeContainer.vertical = vertical;
        this._translator.setInverted(this._getInvertedState());
        this._translator.setCodomain(-100, 100)
    },
    _getScaleLayoutValue: function() {
        return this._area[this._area.vertical ? "x" : "y"]
    },
    _getTicksCoefficients: function(options) {
        var coefs = {
            inner: 0,
            outer: 1
        };
        if (this._area.vertical) {
            if ("left" === options.horizontalOrientation) {
                coefs.inner = 1;
                coefs.outer = 0
            } else if ("center" === options.horizontalOrientation) {
                coefs.inner = coefs.outer = .5
            }
        } else if ("top" === options.verticalOrientation) {
            coefs.inner = 1;
            coefs.outer = 0
        } else if ("center" === options.verticalOrientation) {
            coefs.inner = coefs.outer = .5
        }
        return coefs
    },
    _correctScaleIndents: function(result, indentFromTick, textParams) {
        var vertical = this._area.vertical;
        if (indentFromTick >= 0) {
            result.max += indentFromTick + textParams[vertical ? "width" : "height"]
        } else {
            result.min -= -indentFromTick + textParams[vertical ? "width" : "height"]
        }
        result.indent = textParams[vertical ? "height" : "width"] / 2
    },
    _measureMainElements: function(elements, scaleMeasurement) {
        var x = this._area.x;
        var y = this._area.y;
        var minBound = 1e3;
        var maxBound = 0;
        var indent = 0;
        var scale = this._scale;
        (0, _iterator.each)(elements.concat(scale), (function(_, element) {
            var bounds = element.measure ? element.measure({
                x: x + element.getOffset(),
                y: y + element.getOffset()
            }) : scaleMeasurement;
            void 0 !== bounds.max && (maxBound = _max(maxBound, bounds.max));
            void 0 !== bounds.min && (minBound = _min(minBound, bounds.min));
            bounds.indent > 0 && (indent = _max(indent, bounds.indent))
        }));
        return {
            minBound: minBound,
            maxBound: maxBound,
            indent: indent
        }
    },
    _applyMainLayout: function(elements, scaleMeasurement) {
        var measurements = this._measureMainElements(elements, scaleMeasurement);
        var area = this._area;
        var rect;
        var offset;
        if (area.vertical) {
            rect = selectRectBySizes(this._innerRect, {
                width: measurements.maxBound - measurements.minBound
            });
            offset = (rect.left + rect.right) / 2 - (measurements.minBound + measurements.maxBound) / 2;
            area.startCoord = rect.bottom - measurements.indent;
            area.endCoord = rect.top + measurements.indent;
            area.x = _round(area.x + offset)
        } else {
            rect = selectRectBySizes(this._innerRect, {
                height: measurements.maxBound - measurements.minBound
            });
            offset = (rect.top + rect.bottom) / 2 - (measurements.minBound + measurements.maxBound) / 2;
            area.startCoord = rect.left + measurements.indent;
            area.endCoord = rect.right - measurements.indent;
            area.y = _round(area.y + offset)
        }
        this._translator.setCodomain(area.startCoord, area.endCoord);
        this._innerRect = rect
    },
    _getElementLayout: function(offset) {
        return {
            x: _round(this._area.x + offset),
            y: _round(this._area.y + offset)
        }
    },
    _getApproximateScreenRange: function() {
        var area = this._area;
        var s = area.vertical ? this._canvas.height : this._canvas.width;
        s > area.totalSize && (s = area.totalSize);
        s *= .8;
        return s
    },
    _getDefaultSize: function() {
        var geometry = this.option("geometry") || {};
        if ("vertical" === geometry.orientation) {
            return {
                width: 100,
                height: 300
            }
        } else {
            return {
                width: 300,
                height: 100
            }
        }
    },
    _factory: (0, _object.clone)(_base_gauge.BaseGauge.prototype._factory)
});

function selectRectBySizes(srcRect, sizes, margins) {
    var rect = (0, _extend.extend)({}, srcRect);
    var step;
    margins = margins || {};
    if (sizes) {
        rect.left += margins.left || 0;
        rect.right -= margins.right || 0;
        rect.top += margins.top || 0;
        rect.bottom -= margins.bottom || 0;
        if (sizes.width > 0) {
            step = (rect.right - rect.left - sizes.width) / 2;
            if (step > 0) {
                rect.left += step;
                rect.right -= step
            }
        }
        if (sizes.height > 0) {
            step = (rect.bottom - rect.top - sizes.height) / 2;
            if (step > 0) {
                rect.top += step;
                rect.bottom -= step
            }
        }
    }
    return rect
}
var indicators = dxLinearGauge.prototype._factory.indicators = {};
dxLinearGauge.prototype._factory.createIndicator = (0, _common.createIndicatorCreator)(indicators);
indicators._default = linearIndicators._default;
indicators.rectangle = linearIndicators.rectangle;
indicators.rhombus = linearIndicators.rhombus;
indicators.circle = linearIndicators.circle;
indicators.trianglemarker = linearIndicators.trianglemarker;
indicators.textcloud = linearIndicators.textcloud;
indicators.rangebar = linearIndicators.rangebar;
dxLinearGauge.prototype._factory.RangeContainer = _linear_range_container.default;
(0, _component_registrator.default)("dxLinearGauge", dxLinearGauge);
var _default = dxLinearGauge;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
