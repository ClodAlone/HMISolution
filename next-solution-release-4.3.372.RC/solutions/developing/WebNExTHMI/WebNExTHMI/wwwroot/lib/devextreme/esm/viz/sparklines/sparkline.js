/**
 * DevExtreme (esm/viz/sparklines/sparkline.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseSparkline from "./base_sparkline";
import {
    validateData
} from "../components/data_validator";
import {
    Series
} from "../series/base_series";
var MIN_BAR_WIDTH = 1;
var MAX_BAR_WIDTH = 50;
var DEFAULT_BAR_INTERVAL = 4;
var DEFAULT_CANVAS_WIDTH = 250;
var DEFAULT_CANVAS_HEIGHT = 30;
var DEFAULT_POINT_BORDER = 2;
var ALLOWED_TYPES = {
    line: true,
    spline: true,
    stepline: true,
    area: true,
    steparea: true,
    splinearea: true,
    bar: true,
    winloss: true
};
var _math = Math;
var _abs = _math.abs;
var _round = _math.round;
var _max = _math.max;
var _min = _math.min;
var _isFinite = isFinite;
import {
    map as _map,
    normalizeEnum as _normalizeEnum
} from "../core/utils";
import {
    isDefined as _isDefined
} from "../../core/utils/type";
var _Number = Number;
var _String = String;

function findMinMax(data, valField) {
    var firstItem = data[0] || {};
    var firstValue = firstItem[valField] || 0;
    var min = firstValue;
    var max = firstValue;
    var minIndexes = [0];
    var maxIndexes = [0];
    var dataLength = data.length;
    var value;
    var i;
    for (i = 1; i < dataLength; i++) {
        value = data[i][valField];
        if (value < min) {
            min = value;
            minIndexes = [i]
        } else if (value === min) {
            minIndexes.push(i)
        }
        if (value > max) {
            max = value;
            maxIndexes = [i]
        } else if (value === max) {
            maxIndexes.push(i)
        }
    }
    if (max === min) {
        minIndexes = maxIndexes = []
    }
    return {
        minIndexes: minIndexes,
        maxIndexes: maxIndexes
    }
}

function parseNumericDataSource(data, argField, valField, ignoreEmptyPoints) {
    return _map(data, (function(dataItem, index) {
        var item = null;
        var isDataNumber;
        var value;
        if (void 0 !== dataItem) {
            item = {};
            isDataNumber = _isFinite(dataItem);
            item[argField] = isDataNumber ? _String(index) : dataItem[argField];
            value = isDataNumber ? dataItem : dataItem[valField];
            item[valField] = null === value ? ignoreEmptyPoints ? void 0 : value : _Number(value);
            item = void 0 !== item[argField] && void 0 !== item[valField] ? item : null
        }
        return item
    }))
}

function parseWinlossDataSource(data, argField, valField, target) {
    return _map(data, (function(dataItem) {
        var item = {};
        item[argField] = dataItem[argField];
        if (_abs(dataItem[valField] - target) < 1e-4) {
            item[valField] = 0
        } else if (dataItem[valField] > target) {
            item[valField] = 1
        } else {
            item[valField] = -1
        }
        return item
    }))
}

function selectPointColor(color, options, index, pointIndexes) {
    if (index === pointIndexes.first || index === pointIndexes.last) {
        color = options.firstLastColor
    }
    if ((pointIndexes.min || []).indexOf(index) >= 0) {
        color = options.minColor
    }
    if ((pointIndexes.max || []).indexOf(index) >= 0) {
        color = options.maxColor
    }
    return color
}

function createLineCustomizeFunction(pointIndexes, options) {
    return function() {
        var color = selectPointColor(void 0, options, this.index, pointIndexes);
        return color ? {
            visible: true,
            border: {
                color: color
            }
        } : {}
    }
}

function createBarCustomizeFunction(pointIndexes, options, winlossData) {
    return function() {
        var index = this.index;
        var isWinloss = "winloss" === options.type;
        var target = isWinloss ? options.winlossThreshold : 0;
        var value = isWinloss ? winlossData[index][options.valueField] : this.value;
        var positiveColor = isWinloss ? options.winColor : options.barPositiveColor;
        var negativeColor = isWinloss ? options.lossColor : options.barNegativeColor;
        return {
            color: selectPointColor(value >= target ? positiveColor : negativeColor, options, index, pointIndexes)
        }
    }
}
var dxSparkline = BaseSparkline.inherit({
    _rootClassPrefix: "dxsl",
    _rootClass: "dxsl-sparkline",
    _themeSection: "sparkline",
    _defaultSize: {
        width: DEFAULT_CANVAS_WIDTH,
        height: DEFAULT_CANVAS_HEIGHT
    },
    _initCore: function() {
        this.callBase();
        this._createSeries()
    },
    _initialChanges: ["DATA_SOURCE"],
    _dataSourceChangedHandler: function() {
        this._requestChange(["UPDATE"])
    },
    _updateWidgetElements: function() {
        this._updateSeries();
        this.callBase()
    },
    _disposeWidgetElements: function() {
        this._series && this._series.dispose();
        this._series = this._seriesGroup = this._seriesLabelGroup = null
    },
    _cleanWidgetElements: function() {
        this._seriesGroup.remove();
        this._seriesLabelGroup.remove();
        this._seriesGroup.clear();
        this._seriesLabelGroup.clear();
        this._series.removeGraphicElements();
        this._series.removePointElements();
        this._series.removeBordersGroup()
    },
    _drawWidgetElements: function() {
        if (this._dataIsLoaded()) {
            this._drawSeries();
            this._drawn()
        }
    },
    _getCorrectCanvas: function() {
        var options = this._allOptions;
        var canvas = this._canvas;
        var halfPointSize = options.pointSize && Math.ceil(options.pointSize / 2) + DEFAULT_POINT_BORDER;
        var type = options.type;
        if ("bar" !== type && "winloss" !== type && (options.showFirstLast || options.showMinMax)) {
            return {
                width: canvas.width,
                height: canvas.height,
                left: canvas.left + halfPointSize,
                right: canvas.right + halfPointSize,
                top: canvas.top + halfPointSize,
                bottom: canvas.bottom + halfPointSize
            }
        }
        return canvas
    },
    _prepareOptions: function() {
        this._allOptions = this.callBase();
        this._allOptions.type = _normalizeEnum(this._allOptions.type);
        if (!ALLOWED_TYPES[this._allOptions.type]) {
            this._allOptions.type = "line"
        }
    },
    _createHtmlElements: function() {
        this._seriesGroup = this._renderer.g().attr({
            class: "dxsl-series"
        });
        this._seriesLabelGroup = this._renderer.g().attr({
            class: "dxsl-series-labels"
        })
    },
    _createSeries: function() {
        this._series = new Series({
            renderer: this._renderer,
            seriesGroup: this._seriesGroup,
            labelsGroup: this._seriesLabelGroup,
            argumentAxis: this._argumentAxis,
            valueAxis: this._valueAxis
        }, {
            widgetType: "chart",
            type: "line"
        })
    },
    _updateSeries: function() {
        var singleSeries = this._series;
        this._prepareDataSource();
        var seriesOptions = this._prepareSeriesOptions();
        singleSeries.updateOptions(seriesOptions);
        var groupsData = {
            groups: [{
                series: [singleSeries]
            }]
        };
        groupsData.argumentOptions = {
            type: "bar" === seriesOptions.type ? "discrete" : void 0
        };
        this._simpleDataSource = validateData(this._simpleDataSource, groupsData, this._incidentOccurred, {
            checkTypeForAllData: false,
            convertToAxisDataType: true,
            sortingMethod: true
        })[singleSeries.getArgumentField()];
        seriesOptions.customizePoint = this._getCustomizeFunction();
        singleSeries.updateData(this._simpleDataSource);
        singleSeries.createPoints();
        this._groupsDataCategories = groupsData.categories
    },
    _optionChangesMap: {
        dataSource: "DATA_SOURCE"
    },
    _optionChangesOrder: ["DATA_SOURCE"],
    _change_DATA_SOURCE: function() {
        this._updateDataSource()
    },
    _prepareDataSource: function() {
        var options = this._allOptions;
        var argField = options.argumentField;
        var valField = options.valueField;
        var dataSource = this._dataSourceItems() || [];
        var data = parseNumericDataSource(dataSource, argField, valField, this.option("ignoreEmptyPoints"));
        if ("winloss" === options.type) {
            this._winlossDataSource = data;
            this._simpleDataSource = parseWinlossDataSource(data, argField, valField, options.winlossThreshold)
        } else {
            this._simpleDataSource = data
        }
    },
    _prepareSeriesOptions: function() {
        var options = this._allOptions;
        var type = "winloss" === options.type ? "bar" : options.type;
        return {
            visible: true,
            argumentField: options.argumentField,
            valueField: options.valueField,
            color: options.lineColor,
            width: options.lineWidth,
            widgetType: "chart",
            type: type,
            opacity: -1 !== type.indexOf("area") ? this._allOptions.areaOpacity : void 0,
            point: {
                size: options.pointSize,
                symbol: options.pointSymbol,
                border: {
                    visible: true,
                    width: DEFAULT_POINT_BORDER
                },
                color: options.pointColor,
                visible: false,
                hoverStyle: {
                    border: {}
                },
                selectionStyle: {
                    border: {}
                }
            },
            border: {
                color: options.lineColor,
                width: options.lineWidth,
                visible: "bar" !== type
            }
        }
    },
    _getCustomizeFunction: function() {
        var options = this._allOptions;
        var dataSource = this._winlossDataSource || this._simpleDataSource;
        var drawnPointIndexes = this._getExtremumPointsIndexes(dataSource);
        var customizeFunction;
        if ("winloss" === options.type || "bar" === options.type) {
            customizeFunction = createBarCustomizeFunction(drawnPointIndexes, options, this._winlossDataSource)
        } else {
            customizeFunction = createLineCustomizeFunction(drawnPointIndexes, options)
        }
        return customizeFunction
    },
    _getExtremumPointsIndexes: function(data) {
        var options = this._allOptions;
        var lastIndex = data.length - 1;
        var indexes = {};
        this._minMaxIndexes = findMinMax(data, options.valueField);
        if (options.showFirstLast) {
            indexes.first = 0;
            indexes.last = lastIndex
        }
        if (options.showMinMax) {
            indexes.min = this._minMaxIndexes.minIndexes;
            indexes.max = this._minMaxIndexes.maxIndexes
        }
        return indexes
    },
    _getStick: function() {
        return {
            stick: "bar" !== this._series.type
        }
    },
    _updateRange: function() {
        var series = this._series;
        var type = series.type;
        var isBarType = "bar" === type;
        var isWinlossType = "winloss" === type;
        var rangeData = series.getRangeData();
        var minValue = this._allOptions.minValue;
        var hasMinY = _isDefined(minValue) && _isFinite(minValue);
        var maxValue = this._allOptions.maxValue;
        var hasMaxY = _isDefined(maxValue) && _isFinite(maxValue);
        var argCoef;
        var valCoef = .15 * (rangeData.val.max - rangeData.val.min);
        if (isBarType || isWinlossType || "area" === type) {
            if (0 !== rangeData.val.min) {
                rangeData.val.min -= valCoef
            }
            if (0 !== rangeData.val.max) {
                rangeData.val.max += valCoef
            }
        } else {
            rangeData.val.min -= valCoef;
            rangeData.val.max += valCoef
        }
        if (hasMinY || hasMaxY) {
            if (hasMinY && hasMaxY) {
                rangeData.val.minVisible = _min(minValue, maxValue);
                rangeData.val.maxVisible = _max(minValue, maxValue)
            } else {
                rangeData.val.minVisible = hasMinY ? _Number(minValue) : void 0;
                rangeData.val.maxVisible = hasMaxY ? _Number(maxValue) : void 0
            }
            if (isWinlossType) {
                rangeData.val.minVisible = hasMinY ? _max(rangeData.val.minVisible, -1) : void 0;
                rangeData.val.maxVisible = hasMaxY ? _min(rangeData.val.maxVisible, 1) : void 0
            }
        }
        if (series.getPoints().length > 1) {
            if (isBarType) {
                argCoef = .1 * (rangeData.arg.max - rangeData.arg.min);
                rangeData.arg.min = rangeData.arg.min - argCoef;
                rangeData.arg.max = rangeData.arg.max + argCoef
            }
        }
        rangeData.arg.categories = this._groupsDataCategories;
        this._ranges = rangeData
    },
    _getBarWidth: function(pointsCount) {
        var canvas = this._canvas;
        var intervalWidth = pointsCount * DEFAULT_BAR_INTERVAL;
        var rangeWidth = canvas.width - canvas.left - canvas.right - intervalWidth;
        var width = _round(rangeWidth / pointsCount);
        if (width < MIN_BAR_WIDTH) {
            width = MIN_BAR_WIDTH
        }
        if (width > MAX_BAR_WIDTH) {
            width = MAX_BAR_WIDTH
        }
        return width
    },
    _correctPoints: function() {
        var seriesType = this._allOptions.type;
        var seriesPoints = this._series.getPoints();
        var pointsLength = seriesPoints.length;
        var barWidth;
        var i;
        if ("bar" === seriesType || "winloss" === seriesType) {
            barWidth = this._getBarWidth(pointsLength);
            for (i = 0; i < pointsLength; i++) {
                seriesPoints[i].correctCoordinates({
                    width: barWidth,
                    offset: 0
                })
            }
        }
    },
    _drawSeries: function() {
        if (this._simpleDataSource.length > 0) {
            this._correctPoints();
            this._series.draw();
            this._seriesGroup.append(this._renderer.root)
        }
    },
    _isTooltipEnabled: function() {
        return !!this._simpleDataSource.length
    },
    _getTooltipData: function() {
        var options = this._allOptions;
        var dataSource = this._winlossDataSource || this._simpleDataSource;
        var tooltip = this._tooltip;
        if (0 === dataSource.length) {
            return {}
        }
        var minMax = this._minMaxIndexes;
        var valueField = options.valueField;
        var first = dataSource[0][valueField];
        var last = dataSource[dataSource.length - 1][valueField];
        var min = _isDefined(minMax.minIndexes[0]) ? dataSource[minMax.minIndexes[0]][valueField] : first;
        var max = _isDefined(minMax.maxIndexes[0]) ? dataSource[minMax.maxIndexes[0]][valueField] : first;
        var formattedFirst = tooltip.formatValue(first);
        var formattedLast = tooltip.formatValue(last);
        var formattedMin = tooltip.formatValue(min);
        var formattedMax = tooltip.formatValue(max);
        var customizeObject = {
            firstValue: formattedFirst,
            lastValue: formattedLast,
            minValue: formattedMin,
            maxValue: formattedMax,
            originalFirstValue: first,
            originalLastValue: last,
            originalMinValue: min,
            originalMaxValue: max,
            valueText: ["Start:", formattedFirst, "End:", formattedLast, "Min:", formattedMin, "Max:", formattedMax]
        };
        if ("winloss" === options.type) {
            customizeObject.originalThresholdValue = options.winlossThreshold;
            customizeObject.thresholdValue = tooltip.formatValue(options.winlossThreshold)
        }
        return customizeObject
    }
});
_map(["lineColor", "lineWidth", "areaOpacity", "minColor", "maxColor", "barPositiveColor", "barNegativeColor", "winColor", "lessColor", "firstLastColor", "pointSymbol", "pointColor", "pointSize", "type", "argumentField", "valueField", "winlossThreshold", "showFirstLast", "showMinMax", "ignoreEmptyPoints", "minValue", "maxValue"], (function(name) {
    dxSparkline.prototype._optionChangesMap[name] = "OPTIONS"
}));
import componentRegistrator from "../../core/component_registrator";
componentRegistrator("dxSparkline", dxSparkline);
export default dxSparkline;
import {
    plugin
} from "../core/data_source";
dxSparkline.addPlugin(plugin);
