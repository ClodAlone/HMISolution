/**
 * DevExtreme (esm/viz/chart_components/advanced_chart.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    extend as _extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import {
    each as _each,
    reverseEach as _reverseEach
} from "../../core/utils/iterator";
import {
    Range
} from "../translators/range";
import {
    Axis
} from "../axes/base_axis";
import {
    SeriesFamily
} from "../core/series_family";
import {
    BaseChart
} from "./base_chart";
import rangeDataCalculator from "../series/helpers/range_data_calculator";
import {
    isDefined as _isDefined,
    type
} from "../../core/utils/type";
import {
    noop as _noop
} from "../../core/utils/common";
import {
    convertVisualRangeObject,
    rangesAreEqual,
    map as _map,
    mergeMarginOptions,
    setCanvasValues,
    unique
} from "../core/utils";
var _isArray = Array.isArray;
var DEFAULT_AXIS_NAME = "defaultAxisName";
var FONT = "font";
var COMMON_AXIS_SETTINGS = "commonAxisSettings";
var DEFAULT_PANE_NAME = "default";
var VISUAL_RANGE = "VISUAL_RANGE";

function prepareAxis(axisOptions) {
    return _isArray(axisOptions) ? 0 === axisOptions.length ? [{}] : axisOptions : [axisOptions]
}

function processBubbleMargin(opt, bubbleSize) {
    if (opt.processBubbleSize) {
        opt.size = bubbleSize
    }
    return opt
}

function estimateBubbleSize(size, panesCount, maxSize, rotated) {
    var width = rotated ? size.width / panesCount : size.width;
    var height = rotated ? size.height : size.height / panesCount;
    return Math.min(width, height) * maxSize
}

function setAxisVisualRangeByOption(arg, axis, isDirectOption, index) {
    var options;
    var visualRange;
    if (isDirectOption) {
        visualRange = arg.value;
        options = {
            skipEventRising: true
        };
        var wrappedVisualRange = wrapVisualRange(arg.fullName, visualRange);
        if (wrappedVisualRange) {
            options = {
                allowPartialUpdate: true
            };
            visualRange = wrappedVisualRange
        }
    } else {
        visualRange = (_isDefined(index) ? arg.value[index] : arg.value).visualRange
    }
    axis.visualRange(visualRange, options)
}

function getAxisTypes(groupsData, axis, isArgumentAxes) {
    if (isArgumentAxes) {
        return {
            argumentAxisType: groupsData.argumentAxisType,
            argumentType: groupsData.argumentType
        }
    }
    var {
        valueAxisType: valueAxisType,
        valueType: valueType
    } = groupsData.groups.filter(g => g.valueAxis === axis)[0];
    return {
        valueAxisType: valueAxisType,
        valueType: valueType
    }
}

function wrapVisualRange(fullName, value) {
    var pathElements = fullName.split(".");
    var destElem = pathElements[pathElements.length - 1];
    if ("endValue" === destElem || "startValue" === destElem) {
        return {
            [destElem]: value
        }
    }
}
export var AdvancedChart = BaseChart.inherit({
    _fontFields: [COMMON_AXIS_SETTINGS + ".label." + FONT, COMMON_AXIS_SETTINGS + ".title." + FONT],
    _partialOptionChangesMap: {
        visualRange: VISUAL_RANGE,
        _customVisualRange: VISUAL_RANGE,
        strips: "REFRESH_AXES",
        constantLines: "REFRESH_AXES"
    },
    _partialOptionChangesPath: {
        argumentAxis: ["strips", "constantLines", "visualRange", "_customVisualRange"],
        valueAxis: ["strips", "constantLines", "visualRange", "_customVisualRange"]
    },
    _initCore() {
        this._panesClipRects = {};
        this.callBase()
    },
    _disposeCore() {
        var disposeObjectsInArray = this._disposeObjectsInArray;
        var panesClipRects = this._panesClipRects;
        this.callBase();
        disposeObjectsInArray.call(panesClipRects, "fixed");
        disposeObjectsInArray.call(panesClipRects, "base");
        disposeObjectsInArray.call(panesClipRects, "wide");
        this._panesClipRects = null;
        this._labelsAxesGroup.linkOff();
        this._labelsAxesGroup.dispose();
        this._labelsAxesGroup = null
    },
    _dispose: function() {
        var disposeObjectsInArray = this._disposeObjectsInArray;
        this.callBase();
        this.panes = null;
        if (this._legend) {
            this._legend.dispose();
            this._legend = null
        }
        disposeObjectsInArray.call(this, "panesBackground");
        disposeObjectsInArray.call(this, "seriesFamilies");
        this._disposeAxes()
    },
    _createPanes: function() {
        this._cleanPanesClipRects("fixed");
        this._cleanPanesClipRects("base");
        this._cleanPanesClipRects("wide")
    },
    _cleanPanesClipRects(clipArrayName) {
        var clipArray = this._panesClipRects[clipArrayName];
        (clipArray || []).forEach(clipRect => clipRect && clipRect.dispose());
        this._panesClipRects[clipArrayName] = []
    },
    _getElementsClipRectID(paneName) {
        var clipShape = this._panesClipRects.fixed[this._getPaneIndex(paneName)];
        return clipShape && clipShape.id
    },
    _getPaneIndex(paneName) {
        var paneIndex;
        var name = paneName || DEFAULT_PANE_NAME;
        _each(this.panes, (index, pane) => {
            if (pane.name === name) {
                paneIndex = index;
                return false
            }
        });
        return paneIndex
    },
    _updateSize() {
        this.callBase();
        setCanvasValues(this._canvas)
    },
    _reinitAxes: function() {
        this.panes = this._createPanes();
        this._populateAxes();
        this._axesReinitialized = true
    },
    _populateAxes() {
        var that = this;
        var panes = that.panes;
        var rotated = that._isRotated();
        var argumentAxesOptions = prepareAxis(that.option("argumentAxis") || {})[0];
        var valueAxisOption = that.option("valueAxis");
        var valueAxesOptions = prepareAxis(valueAxisOption || {});
        var argumentAxesPopulatedOptions;
        var valueAxesPopulatedOptions = [];
        var axisNames = [];
        var valueAxesCounter = 0;
        var paneWithNonVirtualAxis;
        var crosshairMargins = that._getCrosshairMargins();
        if (rotated) {
            paneWithNonVirtualAxis = "right" === argumentAxesOptions.position ? panes[panes.length - 1].name : panes[0].name
        } else {
            paneWithNonVirtualAxis = "top" === argumentAxesOptions.position ? panes[0].name : panes[panes.length - 1].name
        }
        argumentAxesPopulatedOptions = _map(panes, pane => {
            var virtual = pane.name !== paneWithNonVirtualAxis;
            return that._populateAxesOptions("argumentAxis", argumentAxesOptions, {
                pane: pane.name,
                name: null,
                optionPath: "argumentAxis",
                crosshairMargin: rotated ? crosshairMargins.x : crosshairMargins.y
            }, rotated, virtual)
        });
        _each(valueAxesOptions, (priority, axisOptions) => {
            var _axisOptions$panes;
            var axisPanes = [];
            var name = axisOptions.name;
            if (name && -1 !== inArray(name, axisNames)) {
                that._incidentOccurred("E2102");
                return
            }
            name && axisNames.push(name);
            if (axisOptions.pane) {
                axisPanes.push(axisOptions.pane)
            }
            if (null !== (_axisOptions$panes = axisOptions.panes) && void 0 !== _axisOptions$panes && _axisOptions$panes.length) {
                axisPanes = axisPanes.concat(axisOptions.panes.slice(0))
            }
            axisPanes = unique(axisPanes);
            if (!axisPanes.length) {
                axisPanes.push(void 0)
            }
            _each(axisPanes, (_, pane) => {
                var optionPath = _isArray(valueAxisOption) ? "valueAxis[".concat(priority, "]") : "valueAxis";
                valueAxesPopulatedOptions.push(that._populateAxesOptions("valueAxis", axisOptions, {
                    name: name || DEFAULT_AXIS_NAME + valueAxesCounter++,
                    pane: pane,
                    priority: priority,
                    optionPath: optionPath,
                    crosshairMargin: rotated ? crosshairMargins.y : crosshairMargins.x
                }, rotated))
            })
        });
        that._redesignAxes(argumentAxesPopulatedOptions, true, paneWithNonVirtualAxis);
        that._redesignAxes(valueAxesPopulatedOptions, false)
    },
    _redesignAxes(options, isArgumentAxes, paneWithNonVirtualAxis) {
        var that = this;
        var axesBasis = [];
        var axes = isArgumentAxes ? that._argumentAxes : that._valueAxes;
        _each(options, (_, opt) => {
            var curAxes = axes && axes.filter(a => a.name === opt.name && (!_isDefined(opt.pane) && that.panes.some(p => p.name === a.pane) || a.pane === opt.pane));
            if (curAxes && curAxes.length > 0) {
                _each(curAxes, (_, axis) => {
                    var axisTypes = getAxisTypes(that._groupsData, axis, isArgumentAxes);
                    axis.updateOptions(opt);
                    if (isArgumentAxes) {
                        axis.setTypes(axisTypes.argumentAxisType, axisTypes.argumentType, "argumentType")
                    } else {
                        axis.setTypes(axisTypes.valueAxisType, axisTypes.valueType, "valueType")
                    }
                    axis.validate();
                    axesBasis.push({
                        axis: axis
                    })
                })
            } else {
                axesBasis.push({
                    options: opt
                })
            }
        });
        if (axes) {
            _reverseEach(axes, (index, axis) => {
                if (!axesBasis.some(basis => basis.axis && basis.axis === axis)) {
                    that._disposeAxis(index, isArgumentAxes)
                }
            })
        } else if (isArgumentAxes) {
            axes = that._argumentAxes = []
        } else {
            axes = that._valueAxes = []
        }
        _each(axesBasis, (_, basis) => {
            var axis = basis.axis;
            if (basis.axis && isArgumentAxes) {
                basis.axis.isVirtual = basis.axis.pane !== paneWithNonVirtualAxis
            } else if (basis.options) {
                axis = that._createAxis(isArgumentAxes, basis.options, isArgumentAxes ? basis.options.pane !== paneWithNonVirtualAxis : void 0);
                axes.push(axis)
            }
            axis.applyVisualRangeSetter(that._getVisualRangeSetter())
        })
    },
    _disposeAxis(index, isArgumentAxis) {
        var axes = isArgumentAxis ? this._argumentAxes : this._valueAxes;
        var axis = axes[index];
        if (!axis) {
            return
        }
        axis.dispose();
        axes.splice(index, 1)
    },
    _disposeAxes: function() {
        var disposeObjectsInArray = this._disposeObjectsInArray;
        disposeObjectsInArray.call(this, "_argumentAxes");
        disposeObjectsInArray.call(this, "_valueAxes")
    },
    _appendAdditionalSeriesGroups: function() {
        this._crosshairCursorGroup.linkAppend();
        this._scrollBar && this._scrollBarGroup.linkAppend()
    },
    _getLegendTargets: function() {
        return (this.series || []).map(s => {
            var item = this._getLegendOptions(s);
            item.legendData.series = s;
            if (!s.getOptions().showInLegend) {
                item.legendData.visible = false
            }
            return item
        })
    },
    _legendItemTextField: "name",
    _seriesPopulatedHandlerCore: function() {
        this._processSeriesFamilies();
        this._processValueAxisFormat()
    },
    _renderTrackers: function() {
        var i;
        for (i = 0; i < this.series.length; ++i) {
            this.series[i].drawTrackers()
        }
    },
    _specialProcessSeries: function() {
        this._processSeriesFamilies()
    },
    _processSeriesFamilies: function() {
        var _that$seriesFamilies;
        var that = this;
        var types = [];
        var families = [];
        var paneSeries;
        var themeManager = that._themeManager;
        var negativesAsZeroes = themeManager.getOptions("negativesAsZeroes");
        var negativesAsZeros = themeManager.getOptions("negativesAsZeros");
        var familyOptions = {
            minBubbleSize: themeManager.getOptions("minBubbleSize"),
            maxBubbleSize: themeManager.getOptions("maxBubbleSize"),
            barGroupPadding: themeManager.getOptions("barGroupPadding"),
            barGroupWidth: themeManager.getOptions("barGroupWidth"),
            negativesAsZeroes: _isDefined(negativesAsZeroes) ? negativesAsZeroes : negativesAsZeros
        };
        if (null !== (_that$seriesFamilies = that.seriesFamilies) && void 0 !== _that$seriesFamilies && _that$seriesFamilies.length) {
            _each(that.seriesFamilies, (function(_, family) {
                family.updateOptions(familyOptions);
                family.adjustSeriesValues()
            }));
            return
        }
        _each(that.series, (function(_, item) {
            if (-1 === inArray(item.type, types)) {
                types.push(item.type)
            }
        }));
        _each(that._getLayoutTargets(), (function(_, pane) {
            paneSeries = that._getSeriesForPane(pane.name);
            _each(types, (function(_, type) {
                var family = new SeriesFamily({
                    type: type,
                    pane: pane.name,
                    minBubbleSize: familyOptions.minBubbleSize,
                    maxBubbleSize: familyOptions.maxBubbleSize,
                    barGroupPadding: familyOptions.barGroupPadding,
                    barGroupWidth: familyOptions.barGroupWidth,
                    negativesAsZeroes: familyOptions.negativesAsZeroes,
                    rotated: that._isRotated()
                });
                family.add(paneSeries);
                family.adjustSeriesValues();
                families.push(family)
            }))
        }));
        that.seriesFamilies = families
    },
    _updateSeriesDimensions: function() {
        var i;
        var seriesFamilies = this.seriesFamilies || [];
        for (i = 0; i < seriesFamilies.length; i++) {
            var family = seriesFamilies[i];
            family.updateSeriesValues();
            family.adjustSeriesDimensions()
        }
    },
    _getLegendCallBack: function(series) {
        return this._legend && this._legend.getActionCallback(series)
    },
    _appendAxesGroups: function() {
        this._stripsGroup.linkAppend();
        this._gridGroup.linkAppend();
        this._axesGroup.linkAppend();
        this._labelsAxesGroup.linkAppend();
        this._constantLinesGroup.linkAppend();
        this._stripLabelAxesGroup.linkAppend();
        this._scaleBreaksGroup.linkAppend()
    },
    _populateMarginOptions() {
        var that = this;
        var bubbleSize = estimateBubbleSize(that.getSize(), that.panes.length, that._themeManager.getOptions("maxBubbleSize"), that._isRotated());
        var argumentMarginOptions = {};
        that._valueAxes.forEach(valueAxis => {
            var groupSeries = that.series.filter((function(series) {
                return series.getValueAxis() === valueAxis
            }));
            var marginOptions = {};
            groupSeries.forEach(series => {
                if (series.isVisible()) {
                    var seriesMarginOptions = processBubbleMargin(series.getMarginOptions(), bubbleSize);
                    marginOptions = mergeMarginOptions(marginOptions, seriesMarginOptions);
                    argumentMarginOptions = mergeMarginOptions(argumentMarginOptions, seriesMarginOptions)
                }
            });
            valueAxis.setMarginOptions(marginOptions)
        });
        that._argumentAxes.forEach(a => a.setMarginOptions(argumentMarginOptions))
    },
    _populateBusinessRange(updatedAxis, keepRange) {
        var that = this;
        var rotated = that._isRotated();
        var series = that._getVisibleSeries();
        var argRanges = {};
        var commonArgRange = new Range({
            rotated: !!rotated
        });
        var getPaneName = axis => axis.pane || DEFAULT_PANE_NAME;
        that.panes.forEach(p => argRanges[p.name] = new Range({
            rotated: !!rotated
        }));
        that._valueAxes.forEach(valueAxis => {
            var groupRange = new Range({
                rotated: !!rotated,
                pane: valueAxis.pane,
                axis: valueAxis.name
            });
            var groupSeries = series.filter(series => series.getValueAxis() === valueAxis);
            groupSeries.forEach(series => {
                var seriesRange = series.getRangeData();
                groupRange.addRange(seriesRange.val);
                argRanges[getPaneName(valueAxis)].addRange(seriesRange.arg)
            });
            if (!updatedAxis || updatedAxis && groupSeries.length && valueAxis === updatedAxis) {
                valueAxis.setGroupSeries(groupSeries);
                valueAxis.setBusinessRange(groupRange, that._axesReinitialized || keepRange, that._argumentAxes[0]._lastVisualRangeUpdateMode)
            }
        });
        if (!updatedAxis || updatedAxis && series.length) {
            Object.keys(argRanges).forEach(p => commonArgRange.addRange(argRanges[p]));
            var commonInterval = commonArgRange.interval;
            that._argumentAxes.forEach(a => {
                var _argRanges$getPaneNam;
                var currentInterval = null !== (_argRanges$getPaneNam = argRanges[getPaneName(a)].interval) && void 0 !== _argRanges$getPaneNam ? _argRanges$getPaneNam : commonInterval;
                a.setBusinessRange(new Range(_extends({}, commonArgRange, {
                    interval: currentInterval
                })), that._axesReinitialized, void 0, that._groupsData.categories)
            })
        }
        that._populateMarginOptions()
    },
    getArgumentAxis: function() {
        return (this._argumentAxes || []).filter(a => !a.isVirtual)[0]
    },
    getValueAxis: function(name) {
        return (this._valueAxes || []).filter(_isDefined(name) ? a => a.name === name : a => a.pane === this.defaultPane)[0]
    },
    _getGroupsData: function() {
        var that = this;
        var groups = [];
        that._valueAxes.forEach((function(axis) {
            groups.push({
                series: that.series.filter((function(series) {
                    return series.getValueAxis() === axis
                })),
                valueAxis: axis,
                valueOptions: axis.getOptions()
            })
        }));
        return {
            groups: groups,
            argumentAxes: that._argumentAxes,
            argumentOptions: that._argumentAxes[0].getOptions()
        }
    },
    _groupSeries: function() {
        this._correctValueAxes(false);
        this._groupsData = this._getGroupsData()
    },
    _processValueAxisFormat: function() {
        var axesWithFullStackedFormat = [];
        this.series.forEach((function(series) {
            var axis = series.getValueAxis();
            if (series.isFullStackedSeries()) {
                axis.setPercentLabelFormat();
                axesWithFullStackedFormat.push(axis)
            }
        }));
        this._valueAxes.forEach((function(axis) {
            if (-1 === axesWithFullStackedFormat.indexOf(axis)) {
                axis.resetAutoLabelFormat()
            }
        }))
    },
    _populateAxesOptions(typeSelector, userOptions, axisOptions, rotated, virtual) {
        var preparedUserOptions = this._prepareStripsAndConstantLines(typeSelector, userOptions, rotated);
        var options = _extend(true, {}, preparedUserOptions, axisOptions, this._prepareAxisOptions(typeSelector, preparedUserOptions, rotated));
        if (virtual) {
            options.visible = options.tick.visible = options.minorTick.visible = options.label.visible = false;
            options.title = {}
        }
        return options
    },
    _getValFilter: series => rangeDataCalculator.getViewPortFilter(series.getValueAxis().visualRange() || {}),
    _createAxis(isArgumentAxes, options, virtual) {
        var that = this;
        var typeSelector = isArgumentAxes ? "argumentAxis" : "valueAxis";
        var renderingSettings = _extend({
            renderer: that._renderer,
            incidentOccurred: that._incidentOccurred,
            eventTrigger: that._eventTrigger,
            axisClass: isArgumentAxes ? "arg" : "val",
            widgetClass: "dxc",
            stripsGroup: that._stripsGroup,
            stripLabelAxesGroup: that._stripLabelAxesGroup,
            constantLinesGroup: that._constantLinesGroup,
            scaleBreaksGroup: that._scaleBreaksGroup,
            axesContainerGroup: that._axesGroup,
            labelsAxesGroup: that._labelsAxesGroup,
            gridGroup: that._gridGroup,
            isArgumentAxis: isArgumentAxes,
            getTemplate: template => that._getTemplate(template)
        }, that._getAxisRenderingOptions(typeSelector));
        var axis = new Axis(renderingSettings);
        axis.updateOptions(options);
        axis.isVirtual = virtual;
        return axis
    },
    _applyVisualRangeByVirtualAxes: (axis, range) => false,
    _applyCustomVisualRangeOption(axis, range) {
        if (axis.getOptions().optionPath) {
            this._parseVisualRangeOption("".concat(axis.getOptions().optionPath, ".visualRange"), range)
        }
    },
    _getVisualRangeSetter() {
        var chart = this;
        return function(axis, _ref) {
            var {
                skipEventRising: skipEventRising,
                range: range
            } = _ref;
            chart._applyCustomVisualRangeOption(axis, range);
            axis.setCustomVisualRange(range);
            axis.skipEventRising = skipEventRising;
            if (!chart._applyVisualRangeByVirtualAxes(axis, range)) {
                if (chart._applyingChanges) {
                    chart._change_VISUAL_RANGE()
                } else {
                    chart._requestChange([VISUAL_RANGE])
                }
            }
        }
    },
    _getTrackerSettings: function() {
        return _extend(this.callBase(), {
            argumentAxis: this.getArgumentAxis()
        })
    },
    _prepareStripsAndConstantLines: function(typeSelector, userOptions, rotated) {
        userOptions = this._themeManager.getOptions(typeSelector, userOptions, rotated);
        if (userOptions.strips) {
            _each(userOptions.strips, (function(i) {
                userOptions.strips[i] = _extend(true, {}, userOptions.stripStyle, userOptions.strips[i])
            }))
        }
        if (userOptions.constantLines) {
            _each(userOptions.constantLines, (function(i, line) {
                userOptions.constantLines[i] = _extend(true, {}, userOptions.constantLineStyle, line)
            }))
        }
        return userOptions
    },
    refresh: function() {
        this._disposeAxes();
        this.callBase()
    },
    _layoutAxes(drawAxes) {
        drawAxes();
        var needSpace = this.checkForMoreSpaceForPanesCanvas();
        if (needSpace) {
            var rect = this._rect.slice();
            var size = this._layout.backward(rect, rect, [needSpace.width, needSpace.height]);
            needSpace.width = Math.max(0, size[0]);
            needSpace.height = Math.max(0, size[1]);
            this._canvas = this._createCanvasFromRect(rect);
            drawAxes(needSpace)
        }
    },
    checkForMoreSpaceForPanesCanvas() {
        return this.layoutManager.needMoreSpaceForPanesCanvas(this._getLayoutTargets(), this._isRotated())
    },
    _parseVisualRangeOption(fullName, value) {
        var that = this;
        var name = fullName.split(/[.[]/)[0];
        var index = fullName.match(/\d+/g);
        index = _isDefined(index) ? parseInt(index[0]) : index;
        if (fullName.indexOf("visualRange") > 0) {
            if ("object" !== type(value)) {
                value = wrapVisualRange(fullName, value) || value
            }
            that._setCustomVisualRange(name, index, value)
        } else if (("object" === type(value) || _isArray(value)) && name.indexOf("Axis") > 0 && JSON.stringify(value).indexOf("visualRange") > 0) {
            if (_isDefined(value.visualRange)) {
                that._setCustomVisualRange(name, index, value.visualRange)
            } else if (_isArray(value)) {
                value.forEach((a, i) => _isDefined(a.visualRange) && that._setCustomVisualRange(name, i, a.visualRange))
            }
        }
    },
    _setCustomVisualRange(axesName, index, value) {
        var options = this._options.silent(axesName);
        if (!options) {
            return
        }
        if (!_isDefined(index)) {
            options._customVisualRange = value
        } else {
            options[index]._customVisualRange = value
        }
        this._axesReinitialized = true
    },
    _raiseZoomEndHandlers() {
        this._valueAxes.forEach(axis => axis.handleZoomEnd())
    },
    _setOptionsByReference() {
        this.callBase();
        _extend(this._optionsByReference, {
            "valueAxis.visualRange": true
        })
    },
    _notifyOptionChanged(option, value, previousValue) {
        this.callBase.apply(this, arguments);
        if (!this._optionChangedLocker) {
            this._parseVisualRangeOption(option, value)
        }
    },
    _notifyVisualRange() {
        var that = this;
        that._valueAxes.forEach(axis => {
            var axisPath = axis.getOptions().optionPath;
            if (axisPath) {
                var path = "".concat(axisPath, ".visualRange");
                var visualRange = convertVisualRangeObject(axis.visualRange(), !_isArray(that.option(path)));
                if (!axis.skipEventRising || !rangesAreEqual(visualRange, that.option(path))) {
                    if (!that.option(axisPath) && "valueAxis" !== axisPath) {
                        that.option(axisPath, {
                            name: axis.name,
                            visualRange: visualRange
                        })
                    } else {
                        that.option(path, visualRange)
                    }
                } else {
                    axis.skipEventRising = null
                }
            }
        })
    },
    _notify() {
        this.callBase();
        this._axesReinitialized = false;
        if (true !== this.option("disableTwoWayBinding")) {
            this.skipOptionsRollBack = true;
            this._notifyVisualRange();
            this.skipOptionsRollBack = false
        }
    },
    _getAxesForScaling() {
        return this._valueAxes
    },
    _getAxesByOptionPath(arg, isDirectOption, optionName) {
        var sourceAxes = this._getAxesForScaling();
        var axes = [];
        if (isDirectOption) {
            var axisPath;
            if (arg.fullName) {
                axisPath = arg.fullName.slice(0, arg.fullName.indexOf("."))
            }
            axes = sourceAxes.filter(a => a.getOptions().optionPath === axisPath)
        } else if ("object" === type(arg.value)) {
            axes = sourceAxes.filter(a => a.getOptions().optionPath === arg.name)
        } else if (_isArray(arg.value)) {
            arg.value.forEach((v, index) => {
                var axis = sourceAxes.filter(a => a.getOptions().optionPath === "".concat(arg.name, "[").concat(index, "]"))[0];
                _isDefined(v[optionName]) && _isDefined(axis) && (axes[index] = axis)
            })
        }
        return axes
    },
    _optionChanged(arg) {
        if (!this._optionChangedLocker) {
            var axes;
            var isDirectOption = arg.fullName.indexOf("visualRange") > 0 ? true : this.getPartialChangeOptionsName(arg).indexOf("visualRange") > -1 ? false : void 0;
            if (_isDefined(isDirectOption)) {
                axes = this._getAxesByOptionPath(arg, isDirectOption, "visualRange");
                if (axes) {
                    if (axes.length > 1 || _isArray(arg.value)) {
                        axes.forEach((a, index) => setAxisVisualRangeByOption(arg, a, isDirectOption, index))
                    } else if (1 === axes.length) {
                        setAxisVisualRangeByOption(arg, axes[0], isDirectOption)
                    }
                }
            }
        }
        this.callBase(arg)
    },
    _change_VISUAL_RANGE: function() {
        this._recreateSizeDependentObjects(false);
        if (!this._changes.has("FULL_RENDER")) {
            var resizePanesOnZoom = this.option("resizePanesOnZoom");
            this._doRender({
                force: true,
                drawTitle: false,
                drawLegend: false,
                adjustAxes: null !== resizePanesOnZoom && void 0 !== resizePanesOnZoom ? resizePanesOnZoom : this.option("adjustAxesOnZoom") || false,
                animate: false
            });
            this._raiseZoomEndHandlers()
        }
    },
    resetVisualRange() {
        var that = this;
        that._valueAxes.forEach(axis => {
            axis.resetVisualRange(false);
            that._applyCustomVisualRangeOption(axis)
        });
        that._requestChange([VISUAL_RANGE])
    },
    _getCrosshairMargins: () => ({
        x: 0,
        y: 0
    }),
    _legendDataField: "series",
    _adjustSeriesLabels: _noop,
    _correctValueAxes: _noop
});
