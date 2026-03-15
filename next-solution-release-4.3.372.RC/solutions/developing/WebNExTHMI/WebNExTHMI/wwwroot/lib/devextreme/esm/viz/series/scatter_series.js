/**
 * DevExtreme (esm/viz/series/scatter_series.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend as _extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import {
    each as _each
} from "../../core/utils/iterator";
import rangeCalculator from "./helpers/range_data_calculator";
import {
    isDefined as _isDefined,
    isString as _isString
} from "../../core/utils/type";
import {
    map as _map,
    normalizeEnum as _normalizeEnum,
    convertXYToPolar
} from "../core/utils";
import {
    noop as _noop
} from "../../core/utils/common";
var math = Math;
var _abs = math.abs;
var _sqrt = math.sqrt;
var _max = math.max;
var DEFAULT_TRACKER_WIDTH = 12;
var DEFAULT_DURATION = 400;
var HIGH_ERROR = "highError";
var LOW_ERROR = "lowError";
var VARIANCE = "variance";
var STANDARD_DEVIATION = "stddeviation";
var STANDARD_ERROR = "stderror";
var PERCENT = "percent";
var FIXED = "fixed";
var UNDEFINED = "undefined";
var DISCRETE = "discrete";
var LOGARITHMIC = "logarithmic";
var DATETIME = "datetime";
var chart = {};
var polar = {};

function sum(array) {
    var result = 0;
    _each(array, (function(_, value) {
        result += value
    }));
    return result
}

function isErrorBarTypeCorrect(type) {
    return -1 !== inArray(type, [FIXED, PERCENT, VARIANCE, STANDARD_DEVIATION, STANDARD_ERROR])
}

function variance(array, expectedValue) {
    return sum(_map(array, (function(value) {
        return (value - expectedValue) * (value - expectedValue)
    }))) / array.length
}

function calculateAvgErrorBars(result, data, series) {
    var errorBarsOptions = series.getOptions().valueErrorBar;
    var valueField = series.getValueFields()[0];
    var lowValueField = errorBarsOptions.lowValueField || LOW_ERROR;
    var highValueField = errorBarsOptions.highValueField || HIGH_ERROR;
    if (series.areErrorBarsVisible() && void 0 === errorBarsOptions.type) {
        var fusionData = data.reduce((function(result, item) {
            if (_isDefined(item[lowValueField])) {
                result[0] += item[valueField] - item[lowValueField];
                result[1]++
            }
            if (_isDefined(item[highValueField])) {
                result[2] += item[highValueField] - item[valueField];
                result[3]++
            }
            return result
        }), [0, 0, 0, 0]);
        if (fusionData[1]) {
            result[lowValueField] = result[valueField] - fusionData[0] / fusionData[1]
        }
        if (fusionData[2]) {
            result[highValueField] = result[valueField] + fusionData[2] / fusionData[3]
        }
    }
    return result
}

function calculateSumErrorBars(result, data, series) {
    var errorBarsOptions = series.getOptions().valueErrorBar;
    var lowValueField = errorBarsOptions.lowValueField || LOW_ERROR;
    var highValueField = errorBarsOptions.highValueField || HIGH_ERROR;
    if (series.areErrorBarsVisible() && void 0 === errorBarsOptions.type) {
        result[lowValueField] = 0;
        result[highValueField] = 0;
        result = data.reduce((function(result, item) {
            result[lowValueField] += item[lowValueField];
            result[highValueField] += item[highValueField];
            return result
        }), result)
    }
    return result
}

function getMinMaxAggregator(compare) {
    return (_ref, series) => {
        var {
            intervalStart: intervalStart,
            intervalEnd: intervalEnd,
            data: data
        } = _ref;
        var valueField = series.getValueFields()[0];
        var targetData = data[0];
        targetData = data.reduce((result, item) => {
            var value = item[valueField];
            if (null === result[valueField]) {
                result = item
            }
            if (null !== value && compare(value, result[valueField])) {
                return item
            }
            return result
        }, targetData);
        return _extend({}, targetData, {
            [series.getArgumentField()]: series._getIntervalCenter(intervalStart, intervalEnd)
        })
    }
}

function checkFields(data, fieldsToCheck, skippedFields) {
    var allFieldsIsValid = true;
    for (var field in fieldsToCheck) {
        var isArgument = "argument" === field;
        if (isArgument || "size" === field ? !_isDefined(data[field]) : void 0 === data[field]) {
            var selector = fieldsToCheck[field];
            if (!isArgument) {
                skippedFields[selector] = (skippedFields[selector] || 0) + 1
            }
            allFieldsIsValid = false
        }
    }
    return allFieldsIsValid
}
var baseScatterMethods = {
    _defaultDuration: DEFAULT_DURATION,
    _defaultTrackerWidth: DEFAULT_TRACKER_WIDTH,
    _applyStyle: _noop,
    _updateOptions: _noop,
    _parseStyle: _noop,
    _prepareSegment: _noop,
    _drawSegment: _noop,
    _appendInGroup: function() {
        this._group.append(this._extGroups.seriesGroup)
    },
    _createLegendState: function(styleOptions, defaultColor) {
        return {
            fill: styleOptions.color || defaultColor,
            hatching: styleOptions.hatching ? _extend({}, styleOptions.hatching, {
                direction: "right"
            }) : void 0
        }
    },
    _applyElementsClipRect: function(settings) {
        settings["clip-path"] = this._paneClipRectID
    },
    _applyMarkerClipRect: function(settings) {
        settings["clip-path"] = this._forceClipping ? this._paneClipRectID : null
    },
    _createGroup: function(groupName, parent, target, settings) {
        var group = parent[groupName] = parent[groupName] || this._renderer.g();
        target && group.append(target);
        settings && group.attr(settings)
    },
    _applyClearingSettings: function(settings) {
        settings.opacity = null;
        settings.scale = null;
        if (this._options.rotated) {
            settings.translateX = null
        } else {
            settings.translateY = null
        }
    },
    _createGroups: function() {
        this._createGroup("_markersGroup", this, this._group);
        this._createGroup("_labelsGroup", this)
    },
    _setMarkerGroupSettings: function() {
        var settings = this._createPointStyles(this._getMarkerGroupOptions()).normal;
        settings.class = "dxc-markers";
        settings.opacity = 1;
        this._applyMarkerClipRect(settings);
        this._markersGroup.attr(settings)
    },
    getVisibleArea: function() {
        return this._visibleArea
    },
    areErrorBarsVisible: function() {
        var errorBarOptions = this._options.valueErrorBar;
        return errorBarOptions && this._errorBarsEnabled() && "none" !== errorBarOptions.displayMode && (isErrorBarTypeCorrect(_normalizeEnum(errorBarOptions.type)) || _isDefined(errorBarOptions.lowValueField) || _isDefined(errorBarOptions.highValueField))
    },
    groupPointsByCoords(rotated) {
        var cat = [];
        _each(this.getVisiblePoints(), (function(_, p) {
            var pointCoord = parseInt(rotated ? p.vy : p.vx);
            if (!cat[pointCoord]) {
                cat[pointCoord] = p
            } else {
                Array.isArray(cat[pointCoord]) ? cat[pointCoord].push(p) : cat[pointCoord] = [cat[pointCoord], p]
            }
        }));
        return cat
    },
    _createErrorBarGroup: function(animationEnabled) {
        var errorBarOptions = this._options.valueErrorBar;
        var settings;
        if (this.areErrorBarsVisible()) {
            settings = {
                class: "dxc-error-bars",
                stroke: errorBarOptions.color,
                "stroke-width": errorBarOptions.lineWidth,
                opacity: animationEnabled ? .001 : errorBarOptions.opacity || 1,
                "stroke-linecap": "square",
                sharp: true,
                "clip-path": this._forceClipping ? this._paneClipRectID : this._widePaneClipRectID
            };
            this._createGroup("_errorBarGroup", this, this._group, settings)
        }
    },
    _setGroupsSettings: function(animationEnabled) {
        this._setMarkerGroupSettings();
        this._setLabelGroupSettings(animationEnabled);
        this._createErrorBarGroup(animationEnabled)
    },
    _getCreatingPointOptions: function() {
        var defaultPointOptions;
        var creatingPointOptions = this._predefinedPointOptions;
        var normalStyle;
        if (!creatingPointOptions) {
            defaultPointOptions = this._getPointOptions();
            this._predefinedPointOptions = creatingPointOptions = _extend(true, {
                styles: {}
            }, defaultPointOptions);
            normalStyle = defaultPointOptions.styles && defaultPointOptions.styles.normal || {};
            creatingPointOptions.styles = creatingPointOptions.styles || {};
            creatingPointOptions.styles.normal = {
                "stroke-width": normalStyle["stroke-width"],
                r: normalStyle.r,
                opacity: normalStyle.opacity
            }
        }
        return creatingPointOptions
    },
    _getPointOptions: function() {
        return this._parsePointOptions(this._preparePointOptions(), this._options.label)
    },
    _getOptionsForPoint: function() {
        return this._options.point
    },
    _parsePointStyle: function(style, defaultColor, defaultBorderColor, defaultSize) {
        var border = style.border || {};
        var sizeValue = void 0 !== style.size ? style.size : defaultSize;
        return {
            fill: style.color || defaultColor,
            stroke: border.color || defaultBorderColor,
            "stroke-width": border.visible ? border.width : 0,
            r: sizeValue / 2 + (border.visible && 0 !== sizeValue ? ~~(border.width / 2) || 0 : 0)
        }
    },
    _createPointStyles: function(pointOptions) {
        var mainPointColor = pointOptions.color || this._options.mainSeriesColor;
        var containerColor = this._options.containerBackgroundColor;
        var normalStyle = this._parsePointStyle(pointOptions, mainPointColor, mainPointColor);
        normalStyle.visibility = pointOptions.visible ? "visible" : "hidden";
        return {
            normal: normalStyle,
            hover: this._parsePointStyle(pointOptions.hoverStyle, containerColor, mainPointColor, pointOptions.size),
            selection: this._parsePointStyle(pointOptions.selectionStyle, containerColor, mainPointColor, pointOptions.size)
        }
    },
    _checkData: function(data, skippedFields, fieldsToCheck) {
        fieldsToCheck = fieldsToCheck || {
            value: this.getValueFields()[0]
        };
        fieldsToCheck.argument = this.getArgumentField();
        return checkFields(data, fieldsToCheck, skippedFields || {}) && data.value === data.value
    },
    getArgumentRangeInitialValue() {
        var points = this.getPoints();
        if (this.useAggregation() && points.length) {
            var _points$0$aggregation, _points$aggregationIn;
            return {
                min: null === (_points$0$aggregation = points[0].aggregationInfo) || void 0 === _points$0$aggregation ? void 0 : _points$0$aggregation.intervalStart,
                max: null === (_points$aggregationIn = points[points.length - 1].aggregationInfo) || void 0 === _points$aggregationIn ? void 0 : _points$aggregationIn.intervalEnd
            }
        }
        return
    },
    getValueRangeInitialValue: function() {
        return
    },
    _getRangeData: function() {
        return rangeCalculator.getRangeData(this)
    },
    _getPointDataSelector: function() {
        var valueField = this.getValueFields()[0];
        var argumentField = this.getArgumentField();
        var tagField = this.getTagField();
        var areErrorBarsVisible = this.areErrorBarsVisible();
        var lowValueField;
        var highValueField;
        if (areErrorBarsVisible) {
            var errorBarOptions = this._options.valueErrorBar;
            lowValueField = errorBarOptions.lowValueField || LOW_ERROR;
            highValueField = errorBarOptions.highValueField || HIGH_ERROR
        }
        return data => {
            var pointData = {
                value: this._processEmptyValue(data[valueField]),
                argument: data[argumentField],
                tag: data[tagField],
                data: data
            };
            if (areErrorBarsVisible) {
                pointData.lowError = data[lowValueField];
                pointData.highError = data[highValueField]
            }
            return pointData
        }
    },
    _errorBarsEnabled: function() {
        return this.valueAxisType !== DISCRETE && this.valueAxisType !== LOGARITHMIC && this.valueType !== DATETIME
    },
    _drawPoint: function(options) {
        var point = options.point;
        if (point.isInVisibleArea()) {
            point.clearVisibility();
            point.draw(this._renderer, options.groups, options.hasAnimation, options.firstDrawing);
            this._drawnPoints.push(point)
        } else {
            point.setInvisibility()
        }
    },
    _animateComplete: function() {
        var animationSettings = {
            duration: this._defaultDuration
        };
        this._labelsGroup && this._labelsGroup.animate({
            opacity: 1
        }, animationSettings);
        this._errorBarGroup && this._errorBarGroup.animate({
            opacity: this._options.valueErrorBar.opacity || 1
        }, animationSettings)
    },
    _animate: function() {
        var that = this;
        var lastPointIndex = that._drawnPoints.length - 1;
        _each(that._drawnPoints || [], (function(i, p) {
            p.animate(i === lastPointIndex ? function() {
                that._animateComplete()
            } : void 0, {
                translateX: p.x,
                translateY: p.y
            })
        }))
    },
    _getIntervalCenter(intervalStart, intervalEnd) {
        var argAxis = this.getArgumentAxis();
        return "discrete" !== argAxis.getOptions().type ? argAxis.getVisualRangeCenter({
            minVisible: intervalStart,
            maxVisible: intervalEnd
        }, true) : intervalStart
    },
    _defaultAggregator: "avg",
    _aggregators: {
        avg(_ref2, series) {
            var {
                data: data,
                intervalStart: intervalStart,
                intervalEnd: intervalEnd
            } = _ref2;
            if (!data.length) {
                return
            }
            var valueField = series.getValueFields()[0];
            var aggregationResult = data.reduce((result, item) => {
                var value = item[valueField];
                if (_isDefined(value)) {
                    result[0] += value;
                    result[1]++
                } else if (null === value) {
                    result[2]++
                }
                return result
            }, [0, 0, 0]);
            return calculateAvgErrorBars({
                [valueField]: aggregationResult[2] === data.length ? null : aggregationResult[0] / aggregationResult[1],
                [series.getArgumentField()]: series._getIntervalCenter(intervalStart, intervalEnd)
            }, data, series)
        },
        sum(_ref3, series) {
            var {
                intervalStart: intervalStart,
                intervalEnd: intervalEnd,
                data: data
            } = _ref3;
            if (!data.length) {
                return
            }
            var valueField = series.getValueFields()[0];
            var aggregationResult = data.reduce((result, item) => {
                var value = item[valueField];
                if (void 0 !== value) {
                    result[0] += value
                }
                if (null === value) {
                    result[1]++
                } else if (void 0 === value) {
                    result[2]++
                }
                return result
            }, [0, 0, 0]);
            var value = aggregationResult[0];
            if (aggregationResult[1] === data.length) {
                value = null
            }
            if (aggregationResult[2] === data.length) {
                return
            }
            return calculateSumErrorBars({
                [valueField]: value,
                [series.getArgumentField()]: series._getIntervalCenter(intervalStart, intervalEnd)
            }, data, series)
        },
        count(_ref4, series) {
            var {
                data: data,
                intervalStart: intervalStart,
                intervalEnd: intervalEnd
            } = _ref4;
            var valueField = series.getValueFields()[0];
            return {
                [series.getArgumentField()]: series._getIntervalCenter(intervalStart, intervalEnd),
                [valueField]: data.filter(i => void 0 !== i[valueField]).length
            }
        },
        min: getMinMaxAggregator((a, b) => a < b),
        max: getMinMaxAggregator((a, b) => a > b)
    },
    _endUpdateData: function() {
        delete this._predefinedPointOptions
    },
    getArgumentField: function() {
        return this._options.argumentField || "arg"
    },
    getValueFields: function() {
        var options = this._options;
        var errorBarsOptions = options.valueErrorBar;
        var valueFields = [options.valueField || "val"];
        var lowValueField;
        var highValueField;
        if (errorBarsOptions) {
            lowValueField = errorBarsOptions.lowValueField;
            highValueField = errorBarsOptions.highValueField;
            _isString(lowValueField) && valueFields.push(lowValueField);
            _isString(highValueField) && valueFields.push(highValueField)
        }
        return valueFields
    },
    _calculateErrorBars: function(data) {
        if (!this.areErrorBarsVisible()) {
            return
        }
        var options = this._options;
        var errorBarsOptions = options.valueErrorBar;
        var errorBarType = _normalizeEnum(errorBarsOptions.type);
        var floatErrorValue = parseFloat(errorBarsOptions.value);
        var valueField = this.getValueFields()[0];
        var value;
        var lowValueField = errorBarsOptions.lowValueField || LOW_ERROR;
        var highValueField = errorBarsOptions.highValueField || HIGH_ERROR;
        var valueArray;
        var valueArrayLength;
        var meanValue;
        var processDataItem;
        var addSubError = function(_i, item) {
            value = item.value;
            item.lowError = value - floatErrorValue;
            item.highError = value + floatErrorValue
        };
        switch (errorBarType) {
            case FIXED:
                processDataItem = addSubError;
                break;
            case PERCENT:
                processDataItem = function(_, item) {
                    value = item.value;
                    var error = value * floatErrorValue / 100;
                    item.lowError = value - error;
                    item.highError = value + error
                };
                break;
            case UNDEFINED:
                processDataItem = function(_, item) {
                    item.lowError = item.data[lowValueField];
                    item.highError = item.data[highValueField]
                };
                break;
            default:
                valueArray = _map(data, (function(item) {
                    return _isDefined(item.data[valueField]) ? item.data[valueField] : null
                }));
                valueArrayLength = valueArray.length;
                floatErrorValue = floatErrorValue || 1;
                switch (errorBarType) {
                    case VARIANCE:
                        floatErrorValue = variance(valueArray, sum(valueArray) / valueArrayLength) * floatErrorValue;
                        processDataItem = addSubError;
                        break;
                    case STANDARD_DEVIATION:
                        meanValue = sum(valueArray) / valueArrayLength;
                        floatErrorValue = _sqrt(variance(valueArray, meanValue)) * floatErrorValue;
                        processDataItem = function(_, item) {
                            item.lowError = meanValue - floatErrorValue;
                            item.highError = meanValue + floatErrorValue
                        };
                        break;
                    case STANDARD_ERROR:
                        floatErrorValue = _sqrt(variance(valueArray, sum(valueArray) / valueArrayLength) / valueArrayLength) * floatErrorValue;
                        processDataItem = addSubError
                }
        }
        processDataItem && _each(data, processDataItem)
    },
    _patchMarginOptions: function(options) {
        var pointOptions = this._getCreatingPointOptions();
        var styles = pointOptions.styles;
        var maxSize = [styles.normal, styles.hover, styles.selection].reduce((function(max, style) {
            return _max(max, 2 * style.r + style["stroke-width"])
        }), 0);
        options.size = pointOptions.visible ? maxSize : 0;
        options.sizePointNormalState = pointOptions.visible ? 2 * styles.normal.r + styles.normal["stroke-width"] : 2;
        return options
    },
    usePointsToDefineAutoHiding: () => true
};
chart = _extend({}, baseScatterMethods, {
    drawTrackers: function() {
        var that = this;
        var trackers;
        var trackersGroup;
        var segments = that._segments || [];
        var rotated = that._options.rotated;
        if (!that.isVisible()) {
            return
        }
        if (segments.length) {
            trackers = that._trackers = that._trackers || [];
            trackersGroup = that._trackersGroup = (that._trackersGroup || that._renderer.g().attr({
                fill: "gray",
                opacity: .001,
                stroke: "gray",
                class: "dxc-trackers"
            })).attr({
                "clip-path": this._paneClipRectID || null
            }).append(that._group);
            _each(segments, (function(i, segment) {
                if (!trackers[i]) {
                    trackers[i] = that._drawTrackerElement(segment).data({
                        "chart-data-series": that
                    }).append(trackersGroup)
                } else {
                    that._updateTrackerElement(segment, trackers[i])
                }
            }))
        }
        that._trackersTranslator = that.groupPointsByCoords(rotated)
    },
    _checkAxisVisibleAreaCoord(isArgument, coord) {
        var axis = isArgument ? this.getArgumentAxis() : this.getValueAxis();
        var visibleArea = axis.getVisibleArea();
        return _isDefined(coord) && visibleArea[0] <= coord && visibleArea[1] >= coord
    },
    checkSeriesViewportCoord(axis, coord) {
        return this.getPoints().length && this.isVisible()
    },
    getSeriesPairCoord(coord, isArgument) {
        var oppositeCoord = null;
        var isOpposite = !isArgument && !this._options.rotated || isArgument && this._options.rotated;
        var coordName = !isOpposite ? "vx" : "vy";
        var oppositeCoordName = !isOpposite ? "vy" : "vx";
        var points = this.getVisiblePoints();
        for (var i = 0; i < points.length; i++) {
            var p = points[i];
            var tmpCoord = p[coordName] === coord ? p[oppositeCoordName] : void 0;
            if (this._checkAxisVisibleAreaCoord(!isArgument, tmpCoord)) {
                oppositeCoord = tmpCoord;
                break
            }
        }
        return oppositeCoord
    },
    _getNearestPoints: (point, nextPoint) => [point, nextPoint],
    _getBezierPoints: () => [],
    _getNearestPointsByCoord(coord, isArgument) {
        var that = this;
        var rotated = that.getOptions().rotated;
        var isOpposite = !isArgument && !rotated || isArgument && rotated;
        var coordName = isOpposite ? "vy" : "vx";
        var allPoints = that.getPoints();
        var bezierPoints = that._getBezierPoints();
        var nearestPoints = [];
        if (allPoints.length > 1) {
            allPoints.forEach((point, i) => {
                var nextPoint = allPoints[i + 1];
                if (nextPoint && (point[coordName] <= coord && nextPoint[coordName] >= coord || point[coordName] >= coord && nextPoint[coordName] <= coord)) {
                    nearestPoints.push(that._getNearestPoints(point, nextPoint, bezierPoints))
                }
            })
        } else {
            nearestPoints.push([allPoints[0], allPoints[0]])
        }
        return nearestPoints
    },
    getNeighborPoint: function(x, y) {
        var pCoord = this._options.rotated ? y : x;
        var nCoord = pCoord;
        var cat = this._trackersTranslator;
        var point = null;
        var minDistance;
        var oppositeCoord = this._options.rotated ? x : y;
        var oppositeCoordName = this._options.rotated ? "vx" : "vy";
        if (this.isVisible() && cat) {
            point = cat[pCoord];
            do {
                point = cat[nCoord] || cat[pCoord];
                pCoord--;
                nCoord++
            } while ((pCoord >= 0 || nCoord < cat.length) && !point);
            if (Array.isArray(point)) {
                minDistance = _abs(point[0][oppositeCoordName] - oppositeCoord);
                _each(point, (function(i, p) {
                    var distance = _abs(p[oppositeCoordName] - oppositeCoord);
                    if (minDistance >= distance) {
                        minDistance = distance;
                        point = p
                    }
                }))
            }
        }
        return point
    },
    _applyVisibleArea: function() {
        var rotated = this._options.rotated;
        var visibleX = (rotated ? this.getValueAxis() : this.getArgumentAxis()).getVisibleArea();
        var visibleY = (rotated ? this.getArgumentAxis() : this.getValueAxis()).getVisibleArea();
        this._visibleArea = {
            minX: visibleX[0],
            maxX: visibleX[1],
            minY: visibleY[0],
            maxY: visibleY[1]
        }
    },
    getPointCenterByArg(arg) {
        var point = this.getPointsByArg(arg)[0];
        return point ? point.getCenterCoord() : void 0
    }
});
polar = _extend({}, baseScatterMethods, {
    drawTrackers: function() {
        chart.drawTrackers.call(this);
        var cat = this._trackersTranslator;
        var index;
        if (!this.isVisible()) {
            return
        }
        _each(cat, (function(i, category) {
            if (category) {
                index = i;
                return false
            }
        }));
        cat[index + 360] = cat[index]
    },
    getNeighborPoint: function(x, y) {
        var pos = convertXYToPolar(this.getValueAxis().getCenter(), x, y);
        return chart.getNeighborPoint.call(this, pos.phi, pos.r)
    },
    _applyVisibleArea: function() {
        var canvas = this.getValueAxis().getCanvas();
        this._visibleArea = {
            minX: canvas.left,
            maxX: canvas.width - canvas.right,
            minY: canvas.top,
            maxY: canvas.height - canvas.bottom
        }
    },
    getSeriesPairCoord(params, isArgument) {
        var coords = null;
        var paramName = isArgument ? "argument" : "radius";
        var points = this.getVisiblePoints();
        for (var i = 0; i < points.length; i++) {
            var p = points[i];
            var tmpPoint = _isDefined(p[paramName]) && _isDefined(params[paramName]) && p[paramName].valueOf() === params[paramName].valueOf() ? {
                x: p.x,
                y: p.y
            } : void 0;
            if (_isDefined(tmpPoint)) {
                coords = tmpPoint;
                break
            }
        }
        return coords
    }
});
export {
    chart,
    polar
};
