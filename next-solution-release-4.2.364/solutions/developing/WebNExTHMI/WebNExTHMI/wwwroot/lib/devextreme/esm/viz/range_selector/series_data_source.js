/**
 * DevExtreme (esm/viz/range_selector/series_data_source.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    Series
} from "../series/base_series";
import {
    SeriesFamily
} from "../core/series_family";
import {
    isNumeric,
    isDate,
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import {
    each
} from "../../core/utils/iterator";
import {
    mergeMarginOptions,
    processSeriesTemplate
} from "../core/utils";
import {
    Range
} from "../translators/range";
import {
    validateData
} from "../components/data_validator";
import {
    ThemeManager as ChartThemeManager
} from "../components/chart_theme_manager";
var createThemeManager = function(chartOptions) {
    return new ChartThemeManager({
        options: chartOptions,
        themeSection: "rangeSelector.chart",
        fontFields: ["commonSeriesSettings.label.font"]
    })
};
var processSeriesFamilies = function(series, minBubbleSize, maxBubbleSize, barOptions, negativesAsZeroes) {
    var families = [];
    var types = [];
    each(series, (function(i, item) {
        if (-1 === inArray(item.type, types)) {
            types.push(item.type)
        }
    }));
    each(types, (function(_, type) {
        var family = new SeriesFamily({
            type: type,
            minBubbleSize: minBubbleSize,
            maxBubbleSize: maxBubbleSize,
            barGroupPadding: barOptions.barGroupPadding,
            barGroupWidth: barOptions.barGroupWidth,
            negativesAsZeroes: negativesAsZeroes
        });
        family.add(series);
        family.adjustSeriesValues();
        families.push(family)
    }));
    return families
};
export var SeriesDataSource = function(options) {
    var themeManager = this._themeManager = createThemeManager(options.chart);
    themeManager.setTheme(options.chart.theme);
    var topIndent = themeManager.getOptions("topIndent");
    var bottomIndent = themeManager.getOptions("bottomIndent");
    this._indent = {
        top: topIndent >= 0 && topIndent < 1 ? topIndent : 0,
        bottom: bottomIndent >= 0 && bottomIndent < 1 ? bottomIndent : 0
    };
    this._valueAxis = themeManager.getOptions("valueAxisRangeSelector") || {};
    this._hideChart = false;
    this._series = this._calculateSeries(options);
    this._seriesFamilies = []
};
SeriesDataSource.prototype = {
    constructor: SeriesDataSource,
    _calculateSeries: function(options) {
        var series = [];
        var particularSeriesOptions;
        var seriesTheme;
        var data = options.dataSource || [];
        var parsedData;
        var chartThemeManager = this._themeManager;
        var seriesTemplate = chartThemeManager.getOptions("seriesTemplate");
        var allSeriesOptions = seriesTemplate ? processSeriesTemplate(seriesTemplate, data) : options.chart.series;
        var dataSourceField;
        var valueAxis = this._valueAxis;
        var i;
        var newSeries;
        var groupsData;
        if (options.dataSource && !allSeriesOptions) {
            dataSourceField = options.dataSourceField || "arg";
            allSeriesOptions = {
                argumentField: dataSourceField,
                valueField: dataSourceField
            };
            this._hideChart = true
        }
        allSeriesOptions = Array.isArray(allSeriesOptions) ? allSeriesOptions : allSeriesOptions ? [allSeriesOptions] : [];
        for (i = 0; i < allSeriesOptions.length; i++) {
            particularSeriesOptions = extend(true, {}, allSeriesOptions[i]);
            particularSeriesOptions.rotated = false;
            seriesTheme = chartThemeManager.getOptions("series", particularSeriesOptions, allSeriesOptions.length);
            seriesTheme.argumentField = seriesTheme.argumentField || options.dataSourceField;
            if (!seriesTheme.name) {
                seriesTheme.name = "Series " + (i + 1).toString()
            }
            if (data && data.length > 0) {
                newSeries = new Series({
                    renderer: options.renderer,
                    argumentAxis: options.argumentAxis,
                    valueAxis: options.valueAxis,
                    incidentOccurred: options.incidentOccurred
                }, seriesTheme);
                series.push(newSeries)
            }
        }
        if (series.length) {
            groupsData = {
                groups: [{
                    series: series,
                    valueAxis: options.valueAxis,
                    valueOptions: {
                        type: valueAxis.type,
                        valueType: dataSourceField ? options.valueType : valueAxis.valueType
                    }
                }],
                argumentOptions: {
                    categories: options.categories,
                    argumentType: options.valueType,
                    type: options.axisType
                }
            };
            parsedData = validateData(data, groupsData, options.incidentOccurred, chartThemeManager.getOptions("dataPrepareSettings"));
            this.argCategories = groupsData.categories;
            for (i = 0; i < series.length; i++) {
                series[i].updateData(parsedData[series[i].getArgumentField()])
            }
        }
        return series
    },
    createPoints() {
        if (0 === this._series.length) {
            return
        }
        var series = this._series;
        var viewport = new Range;
        var axis = series[0].getArgumentAxis();
        var themeManager = this._themeManager;
        var negativesAsZeroes = themeManager.getOptions("negativesAsZeroes");
        var negativesAsZeros = themeManager.getOptions("negativesAsZeros");
        series.forEach((function(s) {
            viewport.addRange(s.getArgumentRange())
        }));
        axis.getTranslator().updateBusinessRange(viewport);
        series.forEach((function(s) {
            s.createPoints()
        }));
        this._seriesFamilies = processSeriesFamilies(series, themeManager.getOptions("minBubbleSize"), themeManager.getOptions("maxBubbleSize"), {
            barGroupPadding: themeManager.getOptions("barGroupPadding"),
            barGroupWidth: themeManager.getOptions("barGroupWidth")
        }, isDefined(negativesAsZeroes) ? negativesAsZeroes : negativesAsZeros)
    },
    adjustSeriesDimensions: function() {
        each(this._seriesFamilies, (function(_, family) {
            family.adjustSeriesDimensions()
        }))
    },
    getBoundRange: function() {
        var rangeData;
        var valueAxis = this._valueAxis;
        var valRange = new Range({
            min: valueAxis.min,
            minVisible: valueAxis.min,
            max: valueAxis.max,
            maxVisible: valueAxis.max,
            axisType: valueAxis.type,
            base: valueAxis.logarithmBase
        });
        var argRange = new Range({});
        var rangeYSize;
        var rangeVisibleSizeY;
        var minIndent;
        var maxIndent;
        each(this._series, (function(_, series) {
            rangeData = series.getRangeData();
            valRange.addRange(rangeData.val);
            argRange.addRange(rangeData.arg)
        }));
        if (!valRange.isEmpty() && !argRange.isEmpty()) {
            minIndent = valueAxis.inverted ? this._indent.top : this._indent.bottom;
            maxIndent = valueAxis.inverted ? this._indent.bottom : this._indent.top;
            rangeYSize = valRange.max - valRange.min;
            rangeVisibleSizeY = (isNumeric(valRange.maxVisible) ? valRange.maxVisible : valRange.max) - (isNumeric(valRange.minVisible) ? valRange.minVisible : valRange.min);
            if (isDate(valRange.min)) {
                valRange.min = new Date(valRange.min.valueOf() - rangeYSize * minIndent)
            } else {
                valRange.min -= rangeYSize * minIndent
            }
            if (isDate(valRange.max)) {
                valRange.max = new Date(valRange.max.valueOf() + rangeYSize * maxIndent)
            } else {
                valRange.max += rangeYSize * maxIndent
            }
            if (isNumeric(rangeVisibleSizeY)) {
                valRange.maxVisible = valRange.maxVisible ? valRange.maxVisible + rangeVisibleSizeY * maxIndent : void 0;
                valRange.minVisible = valRange.minVisible ? valRange.minVisible - rangeVisibleSizeY * minIndent : void 0
            }
            valRange.invert = valueAxis.inverted
        }
        return {
            arg: argRange,
            val: valRange
        }
    },
    getMarginOptions: function(canvas) {
        var bubbleSize = Math.min(canvas.width, canvas.height) * this._themeManager.getOptions("maxBubbleSize");
        return this._series.reduce((function(marginOptions, series) {
            var seriesOptions = series.getMarginOptions();
            if (true === seriesOptions.processBubbleSize) {
                seriesOptions.size = bubbleSize
            }
            return mergeMarginOptions(marginOptions, seriesOptions)
        }), {})
    },
    getSeries: function() {
        return this._series
    },
    isEmpty: function() {
        return 0 === this.getSeries().length
    },
    isShowChart: function() {
        return !this._hideChart
    },
    getCalculatedValueType: function() {
        var series = this._series[0];
        return null === series || void 0 === series ? void 0 : series.argumentType
    },
    getThemeManager: function() {
        return this._themeManager
    }
};
