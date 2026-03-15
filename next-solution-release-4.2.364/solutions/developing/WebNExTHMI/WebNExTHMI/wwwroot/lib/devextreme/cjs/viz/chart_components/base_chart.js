/**
 * DevExtreme (cjs/viz/chart_components/base_chart.js)
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
exports.overlapping = exports.BaseChart = void 0;
var _common = require("../../core/utils/common");
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _type = require("../../core/utils/type");
var _iterator = require("../../core/utils/iterator");
var _extend = require("../../core/utils/extend");
var _array = require("../../core/utils/array");
var _index = require("../../events/utils/index");
var _base_widget = _interopRequireDefault(require("../core/base_widget"));
var _legend = require("../components/legend");
var _data_validator = require("../components/data_validator");
var _base_series = require("../series/base_series");
var _chart_theme_manager = require("../components/chart_theme_manager");
var _layout_manager = require("./layout_manager");
var trackerModule = _interopRequireWildcard(require("./tracker"));
var _utils = require("../core/utils");
var _export = require("../core/export");
var _title = require("../core/title");
var _data_source = require("../core/data_source");
var _tooltip = require("../core/tooltip");
var _loading_indicator = require("../core/loading_indicator");

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
var _isArray = Array.isArray;
var REINIT_REFRESH_ACTION = "_reinit";
var REINIT_DATA_SOURCE_REFRESH_ACTION = "_updateDataSource";
var DATA_INIT_REFRESH_ACTION = "_dataInit";
var FORCE_RENDER_REFRESH_ACTION = "_forceRender";
var RESIZE_REFRESH_ACTION = "_resize";
var ACTIONS_BY_PRIORITY = [REINIT_REFRESH_ACTION, REINIT_DATA_SOURCE_REFRESH_ACTION, DATA_INIT_REFRESH_ACTION, FORCE_RENDER_REFRESH_ACTION, RESIZE_REFRESH_ACTION];
var DEFAULT_OPACITY = .3;
var REFRESH_SERIES_DATA_INIT_ACTION_OPTIONS = ["series", "commonSeriesSettings", "dataPrepareSettings", "seriesSelectionMode", "pointSelectionMode", "synchronizeMultiAxes", "resolveLabelsOverlapping"];
var REFRESH_SERIES_FAMILIES_ACTION_OPTIONS = ["minBubbleSize", "maxBubbleSize", "barGroupPadding", "barGroupWidth", "negativesAsZeroes", "negativesAsZeros"];
var FORCE_RENDER_REFRESH_ACTION_OPTIONS = ["adaptiveLayout", "crosshair", "resolveLabelOverlapping", "adjustOnZoom", "stickyHovering"];
var FONT = "font";

function checkHeightRollingStock(rollingStocks, stubCanvas) {
    var canvasSize = stubCanvas.end - stubCanvas.start;
    var size = 0;
    rollingStocks.forEach((function(rollingStock) {
        size += rollingStock.getBoundingRect().width
    }));
    while (canvasSize < size) {
        size -= findAndKillSmallValue(rollingStocks)
    }
}

function findAndKillSmallValue(rollingStocks) {
    var smallestObject = rollingStocks.reduce((function(prev, rollingStock, index) {
        if (!rollingStock) {
            return prev
        }
        var value = rollingStock.value();
        return value < prev.value ? {
            value: value,
            rollingStock: rollingStock,
            index: index
        } : prev
    }), {
        rollingStock: void 0,
        value: 1 / 0,
        index: void 0
    });
    smallestObject.rollingStock.getLabels()[0].draw(false);
    var width = smallestObject.rollingStock.getBoundingRect().width;
    rollingStocks[smallestObject.index] = null;
    return width
}

function checkStackOverlap(rollingStocks) {
    var i;
    var j;
    var iLength;
    var jLength;
    var overlap = false;
    for (i = 0, iLength = rollingStocks.length - 1; i < iLength; i++) {
        for (j = i + 1, jLength = rollingStocks.length; j < jLength; j++) {
            if (i !== j && checkStacksOverlapping(rollingStocks[i], rollingStocks[j], true)) {
                overlap = true;
                break
            }
        }
        if (overlap) {
            break
        }
    }
    return overlap
}

function resolveLabelOverlappingInOneDirection(points, canvas, isRotated, isInverted, shiftFunction) {
    var customSorting = arguments.length > 5 && void 0 !== arguments[5] ? arguments[5] : function() {
        return 0
    };
    var rollingStocks = [];
    var stubCanvas = {
        start: isRotated ? canvas.left : canvas.top,
        end: isRotated ? canvas.width - canvas.right : canvas.height - canvas.bottom
    };
    var hasStackedSeries = false;
    points.forEach((function(p) {
        if (!p) {
            return
        }
        hasStackedSeries = hasStackedSeries || p.series.isStackedSeries() || p.series.isFullStackedSeries();
        p.getLabels().forEach((function(l) {
            l.isVisible() && rollingStocks.push(new RollingStock(l, isRotated, shiftFunction))
        }))
    }));
    if (hasStackedSeries) {
        !isRotated ^ isInverted && rollingStocks.reverse()
    } else {
        var rollingStocksTmp = rollingStocks.slice();
        rollingStocks.sort((function(a, b) {
            return customSorting(a, b) || a.getInitialPosition() - b.getInitialPosition() || rollingStocksTmp.indexOf(a) - rollingStocksTmp.indexOf(b)
        }))
    }
    if (!checkStackOverlap(rollingStocks)) {
        return false
    }
    checkHeightRollingStock(rollingStocks, stubCanvas);
    prepareOverlapStacks(rollingStocks);
    rollingStocks.reverse();
    moveRollingStock(rollingStocks, stubCanvas);
    return true
}

function checkStacksOverlapping(firstRolling, secondRolling, inTwoSides) {
    if (!firstRolling || !secondRolling) {
        return
    }
    var firstRect = firstRolling.getBoundingRect();
    var secondRect = secondRolling.getBoundingRect();
    var oppositeOverlapping = inTwoSides ? firstRect.oppositeStart <= secondRect.oppositeStart && firstRect.oppositeEnd > secondRect.oppositeStart || secondRect.oppositeStart <= firstRect.oppositeStart && secondRect.oppositeEnd > firstRect.oppositeStart : true;
    return firstRect.end > secondRect.start && oppositeOverlapping
}

function prepareOverlapStacks(rollingStocks) {
    var i;
    var currentRollingStock;
    var root;
    for (i = 0; i < rollingStocks.length - 1; i++) {
        currentRollingStock = root || rollingStocks[i];
        if (checkStacksOverlapping(currentRollingStock, rollingStocks[i + 1])) {
            currentRollingStock.toChain(rollingStocks[i + 1]);
            rollingStocks[i + 1] = null;
            root = currentRollingStock
        } else {
            root = rollingStocks[i + 1] || currentRollingStock
        }
    }
}

function moveRollingStock(rollingStocks, canvas) {
    var i;
    var j;
    var currentRollingStock;
    var nextRollingStock;
    var currentBBox;
    var nextBBox;
    for (i = 0; i < rollingStocks.length; i++) {
        currentRollingStock = rollingStocks[i];
        if (rollingStocksIsOut(currentRollingStock, canvas)) {
            currentBBox = currentRollingStock.getBoundingRect();
            for (j = i + 1; j < rollingStocks.length; j++) {
                nextRollingStock = rollingStocks[j];
                if (!nextRollingStock) {
                    continue
                }
                nextBBox = nextRollingStock.getBoundingRect();
                if (nextBBox.end > currentBBox.start - (currentBBox.end - canvas.end)) {
                    nextRollingStock.toChain(currentRollingStock);
                    rollingStocks[i] = currentRollingStock = null;
                    break
                }
            }
        }
        currentRollingStock && currentRollingStock.setRollingStockInCanvas(canvas)
    }
}

function rollingStocksIsOut(rollingStock, canvas) {
    return rollingStock && rollingStock.getBoundingRect().end > canvas.end
}

function RollingStock(label, isRotated, shiftFunction) {
    var bBox = label.getBoundingRect();
    var x = bBox.x;
    var y = bBox.y;
    var endX = bBox.x + bBox.width;
    var endY = bBox.y + bBox.height;
    this.labels = [label];
    this.shiftFunction = shiftFunction;
    this._bBox = {
        start: isRotated ? x : y,
        width: isRotated ? bBox.width : bBox.height,
        end: isRotated ? endX : endY,
        oppositeStart: isRotated ? y : x,
        oppositeEnd: isRotated ? endY : endX
    };
    this._initialPosition = isRotated ? bBox.x : bBox.y;
    return this
}
RollingStock.prototype = {
    toChain: function(nextRollingStock) {
        var nextRollingStockBBox = nextRollingStock.getBoundingRect();
        nextRollingStock.shift(nextRollingStockBBox.start - this._bBox.end);
        this._changeBoxWidth(nextRollingStockBBox.width);
        this.labels = this.labels.concat(nextRollingStock.labels)
    },
    getBoundingRect: function() {
        return this._bBox
    },
    shift: function(shiftLength) {
        var shiftFunction = this.shiftFunction;
        (0, _iterator.each)(this.labels, (function(index, label) {
            var bBox = label.getBoundingRect();
            var coords = shiftFunction(bBox, shiftLength);
            if (!label.hideInsideLabel(coords)) {
                label.shift(coords.x, coords.y)
            }
        }));
        this._bBox.end -= shiftLength;
        this._bBox.start -= shiftLength
    },
    setRollingStockInCanvas: function(canvas) {
        if (this._bBox.end > canvas.end) {
            this.shift(this._bBox.end - canvas.end)
        }
    },
    getLabels: function() {
        return this.labels
    },
    value: function() {
        return this.labels[0].getData().value
    },
    getInitialPosition: function() {
        return this._initialPosition
    },
    _changeBoxWidth: function(width) {
        this._bBox.end += width;
        this._bBox.width += width
    }
};

function getLegendFields(name) {
    return {
        nameField: name + "Name",
        colorField: name + "Color",
        indexField: name + "Index"
    }
}

function getLegendSettings(legendDataField) {
    var formatObjectFields = getLegendFields(legendDataField);
    return {
        getFormatObject: function(data) {
            var res = {};
            res[formatObjectFields.indexField] = data.id;
            res[formatObjectFields.colorField] = data.states.normal.fill;
            res[formatObjectFields.nameField] = data.text;
            return res
        },
        textField: formatObjectFields.nameField
    }
}

function checkOverlapping(firstRect, secondRect) {
    return (firstRect.x <= secondRect.x && secondRect.x <= firstRect.x + firstRect.width || firstRect.x >= secondRect.x && firstRect.x <= secondRect.x + secondRect.width) && (firstRect.y <= secondRect.y && secondRect.y <= firstRect.y + firstRect.height || firstRect.y >= secondRect.y && firstRect.y <= secondRect.y + secondRect.height)
}
var overlapping = {
    resolveLabelOverlappingInOneDirection: resolveLabelOverlappingInOneDirection
};
exports.overlapping = overlapping;
var BaseChart = _base_widget.default.inherit({
    _eventsMap: {
        onSeriesClick: {
            name: "seriesClick"
        },
        onPointClick: {
            name: "pointClick"
        },
        onArgumentAxisClick: {
            name: "argumentAxisClick"
        },
        onLegendClick: {
            name: "legendClick"
        },
        onSeriesSelectionChanged: {
            name: "seriesSelectionChanged"
        },
        onPointSelectionChanged: {
            name: "pointSelectionChanged"
        },
        onSeriesHoverChanged: {
            name: "seriesHoverChanged"
        },
        onPointHoverChanged: {
            name: "pointHoverChanged"
        },
        onDone: {
            name: "done"
        },
        onZoomStart: {
            name: "zoomStart"
        },
        onZoomEnd: {
            name: "zoomEnd"
        }
    },
    _fontFields: ["legend." + FONT, "legend.title." + FONT, "legend.title.subtitle." + FONT, "commonSeriesSettings.label." + FONT],
    _rootClassPrefix: "dxc",
    _rootClass: "dxc-chart",
    _initialChanges: ["INIT"],
    _themeDependentChanges: ["REFRESH_SERIES_REINIT"],
    _getThemeManagerOptions: function() {
        var themeOptions = this.callBase.apply(this, arguments);
        themeOptions.options = this.option();
        return themeOptions
    },
    _createThemeManager: function() {
        var chartOption = this.option();
        var themeManager = new _chart_theme_manager.ThemeManager(this._getThemeManagerOptions());
        themeManager.setTheme(chartOption.theme, chartOption.rtlEnabled);
        return themeManager
    },
    _initCore: function() {
        this._canvasClipRect = this._renderer.clipRect();
        this._createHtmlStructure();
        this._createLegend();
        this._createTracker();
        this._needHandleRenderComplete = true;
        this.layoutManager = new _layout_manager.LayoutManager;
        this._createScrollBar();
        _events_engine.default.on(this._$element, "contextmenu", (function(event) {
            if ((0, _index.isTouchEvent)(event) || (0, _index.isPointerEvent)(event)) {
                event.preventDefault()
            }
        }));
        _events_engine.default.on(this._$element, "MSHoldVisual", (function(event) {
            event.preventDefault()
        }))
    },
    _getLayoutItems: _common.noop,
    _layoutManagerOptions: function() {
        return this._themeManager.getOptions("adaptiveLayout")
    },
    _reinit: function() {
        (0, _utils.setCanvasValues)(this._canvas);
        this._reinitAxes();
        this._requestChange(["DATA_SOURCE", "DATA_INIT", "CORRECT_AXIS", "FULL_RENDER"])
    },
    _correctAxes: _common.noop,
    _createHtmlStructure: function() {
        var that = this;
        var renderer = that._renderer;
        var root = renderer.root;
        var createConstantLinesGroup = function() {
            return renderer.g().attr({
                class: "dxc-constant-lines-group"
            }).linkOn(root, "constant-lines")
        };
        that._constantLinesGroup = {
            dispose: function() {
                this.under.dispose();
                this.above.dispose()
            },
            linkOff: function() {
                this.under.linkOff();
                this.above.linkOff()
            },
            clear: function() {
                this.under.linkRemove().clear();
                this.above.linkRemove().clear()
            },
            linkAppend: function() {
                this.under.linkAppend();
                this.above.linkAppend()
            }
        };
        that._labelsAxesGroup = renderer.g().attr({
            class: "dxc-elements-axes-group"
        });
        var appendLabelsAxesGroup = function() {
            that._labelsAxesGroup.linkOn(root, "elements")
        };
        that._backgroundRect = renderer.rect().attr({
            fill: "gray",
            opacity: 1e-4
        }).append(root);
        that._panesBackgroundGroup = renderer.g().attr({
            class: "dxc-background"
        }).append(root);
        that._stripsGroup = renderer.g().attr({
            class: "dxc-strips-group"
        }).linkOn(root, "strips");
        that._gridGroup = renderer.g().attr({
            class: "dxc-grids-group"
        }).linkOn(root, "grids");
        that._panesBorderGroup = renderer.g().attr({
            class: "dxc-border"
        }).linkOn(root, "border");
        that._axesGroup = renderer.g().attr({
            class: "dxc-axes-group"
        }).linkOn(root, "axes");
        that._executeAppendBeforeSeries(appendLabelsAxesGroup);
        that._stripLabelAxesGroup = renderer.g().attr({
            class: "dxc-strips-labels-group"
        }).linkOn(root, "strips-labels");
        that._constantLinesGroup.under = createConstantLinesGroup();
        that._seriesGroup = renderer.g().attr({
            class: "dxc-series-group"
        }).linkOn(root, "series");
        that._executeAppendAfterSeries(appendLabelsAxesGroup);
        that._constantLinesGroup.above = createConstantLinesGroup();
        that._scaleBreaksGroup = renderer.g().attr({
            class: "dxc-scale-breaks"
        }).linkOn(root, "scale-breaks");
        that._labelsGroup = renderer.g().attr({
            class: "dxc-labels-group"
        }).linkOn(root, "labels");
        that._crosshairCursorGroup = renderer.g().attr({
            class: "dxc-crosshair-cursor"
        }).linkOn(root, "crosshair");
        that._legendGroup = renderer.g().attr({
            class: "dxc-legend",
            "clip-path": that._getCanvasClipRectID()
        }).linkOn(root, "legend").linkAppend(root).enableLinks();
        that._scrollBarGroup = renderer.g().attr({
            class: "dxc-scroll-bar"
        }).linkOn(root, "scroll-bar")
    },
    _executeAppendBeforeSeries: function() {},
    _executeAppendAfterSeries: function() {},
    _disposeObjectsInArray: function(propName, fieldNames) {
        (0, _iterator.each)(this[propName] || [], (function(_, item) {
            if (fieldNames && item) {
                (0, _iterator.each)(fieldNames, (function(_, field) {
                    item[field] && item[field].dispose()
                }))
            } else {
                item && item.dispose()
            }
        }));
        this[propName] = null
    },
    _disposeCore: function() {
        var that = this;
        var disposeObject = function(propName) {
            if (that[propName]) {
                that[propName].dispose();
                that[propName] = null
            }
        };
        var unlinkGroup = function(name) {
            that[name].linkOff()
        };
        var disposeObjectsInArray = this._disposeObjectsInArray;
        that._renderer.stopAllAnimations();
        disposeObjectsInArray.call(that, "series");
        disposeObject("_tracker");
        disposeObject("_crosshair");
        that.layoutManager = that._userOptions = that._canvas = that._groupsData = null;
        unlinkGroup("_stripsGroup");
        unlinkGroup("_gridGroup");
        unlinkGroup("_axesGroup");
        unlinkGroup("_constantLinesGroup");
        unlinkGroup("_stripLabelAxesGroup");
        unlinkGroup("_panesBorderGroup");
        unlinkGroup("_seriesGroup");
        unlinkGroup("_labelsGroup");
        unlinkGroup("_crosshairCursorGroup");
        unlinkGroup("_legendGroup");
        unlinkGroup("_scrollBarGroup");
        unlinkGroup("_scaleBreaksGroup");
        disposeObject("_canvasClipRect");
        disposeObject("_panesBackgroundGroup");
        disposeObject("_backgroundRect");
        disposeObject("_stripsGroup");
        disposeObject("_gridGroup");
        disposeObject("_axesGroup");
        disposeObject("_constantLinesGroup");
        disposeObject("_stripLabelAxesGroup");
        disposeObject("_panesBorderGroup");
        disposeObject("_seriesGroup");
        disposeObject("_labelsGroup");
        disposeObject("_crosshairCursorGroup");
        disposeObject("_legendGroup");
        disposeObject("_scrollBarGroup");
        disposeObject("_scaleBreaksGroup")
    },
    _getAnimationOptions: function() {
        return this._themeManager.getOptions("animation")
    },
    _getDefaultSize: function() {
        return {
            width: 400,
            height: 400
        }
    },
    _getOption: function(name) {
        return this._themeManager.getOptions(name)
    },
    _applySize: function(rect) {
        this._rect = rect.slice();
        if (!this._changes.has("FULL_RENDER")) {
            this._processRefreshData(RESIZE_REFRESH_ACTION)
        }
    },
    _resize: function() {
        this._doRender(this.__renderOptions || {
            animate: false,
            isResize: true
        })
    },
    _trackerType: "ChartTracker",
    _createTracker: function() {
        this._tracker = new trackerModule[this._trackerType]({
            seriesGroup: this._seriesGroup,
            renderer: this._renderer,
            tooltip: this._tooltip,
            legend: this._legend,
            eventTrigger: this._eventTrigger
        })
    },
    _getTrackerSettings: function() {
        return (0, _extend.extend)({
            chart: this
        }, this._getSelectionModes())
    },
    _getSelectionModes: function() {
        var themeManager = this._themeManager;
        return {
            seriesSelectionMode: themeManager.getOptions("seriesSelectionMode"),
            pointSelectionMode: themeManager.getOptions("pointSelectionMode")
        }
    },
    _updateTracker: function(trackerCanvases) {
        this._tracker.update(this._getTrackerSettings());
        this._tracker.setCanvases({
            left: 0,
            right: this._canvas.width,
            top: 0,
            bottom: this._canvas.height
        }, trackerCanvases)
    },
    _createCanvasFromRect: function(rect) {
        var currentCanvas = this._canvas;
        return (0, _utils.setCanvasValues)({
            left: rect[0],
            top: rect[1],
            right: currentCanvas.width - rect[2],
            bottom: currentCanvas.height - rect[3],
            width: currentCanvas.width,
            height: currentCanvas.height
        })
    },
    _doRender: function(_options) {
        if (0 === this._canvas.width && 0 === this._canvas.height) {
            return
        }
        this._resetIsReady();
        var drawOptions = this._prepareDrawOptions(_options);
        var recreateCanvas = drawOptions.recreateCanvas;
        this._preserveOriginalCanvas();
        if (recreateCanvas) {
            this.__currentCanvas = this._canvas
        } else {
            this._canvas = this.__currentCanvas
        }
        recreateCanvas && this._updateCanvasClipRect(this._canvas);
        this._canvas = this._createCanvasFromRect(this._rect);
        this._renderer.stopAllAnimations(true);
        this._cleanGroups();
        var startTime = new Date;
        this._renderElements(drawOptions);
        this._lastRenderingTime = new Date - startTime
    },
    _preserveOriginalCanvas: function() {
        this.__originalCanvas = this._canvas;
        this._canvas = (0, _extend.extend)({}, this._canvas)
    },
    _layoutAxes: _common.noop,
    _renderElements: function(drawOptions) {
        var that = this;
        var preparedOptions = that._prepareToRender(drawOptions);
        var isRotated = that._isRotated();
        var isLegendInside = that._isLegendInside();
        var trackerCanvases = [];
        (0, _extend.extend)({}, that._canvas);
        var argBusinessRange;
        var zoomMinArg;
        var zoomMaxArg;
        that._renderer.lock();
        if (drawOptions.drawLegend && that._legend) {
            that._legendGroup.linkAppend()
        }
        that.layoutManager.setOptions(that._layoutManagerOptions());
        var layoutTargets = that._getLayoutTargets();
        this._layoutAxes((function(needSpace) {
            var axisDrawOptions = needSpace ? (0, _extend.extend)({}, drawOptions, {
                animate: false,
                recreateCanvas: true
            }) : drawOptions;
            var canvas = that._renderAxes(axisDrawOptions, preparedOptions);
            that._shrinkAxes(needSpace, canvas)
        }));
        that._applyClipRects(preparedOptions);
        that._appendSeriesGroups();
        that._createCrosshairCursor();
        layoutTargets.forEach((function(_ref) {
            var canvas = _ref.canvas;
            trackerCanvases.push({
                left: canvas.left,
                right: canvas.width - canvas.right,
                top: canvas.top,
                bottom: canvas.height - canvas.bottom
            })
        }));
        if (that._scrollBar) {
            argBusinessRange = that._argumentAxes[0].getTranslator().getBusinessRange();
            if ("discrete" === argBusinessRange.axisType && argBusinessRange.categories && argBusinessRange.categories.length <= 1 || "discrete" !== argBusinessRange.axisType && argBusinessRange.min === argBusinessRange.max) {
                zoomMinArg = zoomMaxArg = void 0
            } else {
                zoomMinArg = argBusinessRange.minVisible;
                zoomMaxArg = argBusinessRange.maxVisible
            }
            that._scrollBar.init(argBusinessRange, !that._argumentAxes[0].getOptions().valueMarginsEnabled).setPosition(zoomMinArg, zoomMaxArg)
        }
        that._updateTracker(trackerCanvases);
        that._updateLegendPosition(drawOptions, isLegendInside);
        that._applyPointMarkersAutoHiding();
        that._renderSeries(drawOptions, isRotated, isLegendInside);
        that._renderer.unlock()
    },
    _updateLegendPosition: _common.noop,
    _createCrosshairCursor: _common.noop,
    _appendSeriesGroups: function() {
        this._seriesGroup.linkAppend();
        this._labelsGroup.linkAppend();
        this._appendAdditionalSeriesGroups()
    },
    _renderSeries: function(drawOptions, isRotated, isLegendInside) {
        this._calculateSeriesLayout(drawOptions, isRotated);
        this._renderSeriesElements(drawOptions, isLegendInside)
    },
    _calculateSeriesLayout: function(drawOptions, isRotated) {
        drawOptions.hideLayoutLabels = this.layoutManager.needMoreSpaceForPanesCanvas(this._getLayoutTargets(), isRotated) && !this._themeManager.getOptions("adaptiveLayout").keepLabels;
        this._updateSeriesDimensions(drawOptions)
    },
    _getArgFilter: function() {
        return function() {
            return true
        }
    },
    _getValFilter: function(series) {
        return function() {
            return true
        }
    },
    _getPointsToAnimation: function(series) {
        var _this = this;
        var argViewPortFilter = this._getArgFilter();
        return series.map((function(s) {
            var valViewPortFilter = _this._getValFilter(s);
            return s.getPoints().filter((function(p) {
                return p.getOptions().visible && argViewPortFilter(p.argument) && (valViewPortFilter(p.getMinValue(true)) || valViewPortFilter(p.getMaxValue(true)))
            })).length
        }))
    },
    _renderSeriesElements: function(drawOptions, isLegendInside) {
        var i;
        var series = this.series;
        var singleSeries;
        var seriesLength = series.length;
        var resolveLabelOverlapping = this._themeManager.getOptions("resolveLabelOverlapping");
        var pointsToAnimation = this._getPointsToAnimation(series);
        for (i = 0; i < seriesLength; i++) {
            singleSeries = series[i];
            this._applyExtraSettings(singleSeries, drawOptions);
            singleSeries.draw(drawOptions.animate && pointsToAnimation[i] <= drawOptions.animationPointsLimit && this._renderer.animationEnabled(), drawOptions.hideLayoutLabels, this._getLegendCallBack(singleSeries))
        }
        if ("none" === resolveLabelOverlapping) {
            this._adjustSeriesLabels(false)
        } else {
            this._locateLabels(resolveLabelOverlapping)
        }
        this._renderTrackers(isLegendInside);
        this._tracker.repairTooltip();
        this._renderExtraElements();
        this._clearCanvas();
        this._seriesElementsDrawn = true
    },
    _changesApplied: function() {
        if (this._seriesElementsDrawn) {
            this._seriesElementsDrawn = false;
            this._drawn();
            this._renderCompleteHandler()
        }
    },
    _locateLabels: function(resolveLabelOverlapping) {
        this._resolveLabelOverlapping(resolveLabelOverlapping)
    },
    _renderExtraElements: function() {},
    _clearCanvas: function() {
        this._canvas = this.__originalCanvas
    },
    _resolveLabelOverlapping: function(resolveLabelOverlapping) {
        var func;
        switch (resolveLabelOverlapping) {
            case "stack":
                func = this._resolveLabelOverlappingStack;
                break;
            case "hide":
                func = this._resolveLabelOverlappingHide;
                break;
            case "shift":
                func = this._resolveLabelOverlappingShift
        }
        return (0, _type.isFunction)(func) && func.call(this)
    },
    _getVisibleSeries: function() {
        return (0, _common.grep)(this.getAllSeries(), (function(series) {
            return series.isVisible()
        }))
    },
    _resolveLabelOverlappingHide: function() {
        var labels = [];
        var currentLabel;
        var nextLabel;
        var currentLabelRect;
        var nextLabelRect;
        var i;
        var j;
        var points;
        var series = this._getVisibleSeries();
        for (i = 0; i < series.length; i++) {
            points = series[i].getVisiblePoints();
            for (j = 0; j < points.length; j++) {
                labels.push.apply(labels, points[j].getLabels())
            }
        }
        for (i = 0; i < labels.length; i++) {
            currentLabel = labels[i];
            if (!currentLabel.isVisible()) {
                continue
            }
            currentLabelRect = currentLabel.getBoundingRect();
            for (j = i + 1; j < labels.length; j++) {
                nextLabel = labels[j];
                nextLabelRect = nextLabel.getBoundingRect();
                if (checkOverlapping(currentLabelRect, nextLabelRect)) {
                    nextLabel.draw(false)
                }
            }
        }
    },
    _cleanGroups: function() {
        this._stripsGroup.linkRemove().clear();
        this._gridGroup.linkRemove().clear();
        this._axesGroup.linkRemove().clear();
        this._constantLinesGroup.above.clear();
        this._stripLabelAxesGroup.linkRemove().clear();
        this._labelsGroup.linkRemove().clear();
        this._crosshairCursorGroup.linkRemove().clear();
        this._scaleBreaksGroup.linkRemove().clear()
    },
    _allowLegendInsidePosition: function() {
        return false
    },
    _createLegend: function() {
        var legendSettings = getLegendSettings(this._legendDataField);
        this._legend = new _legend.Legend({
            renderer: this._renderer,
            widget: this,
            group: this._legendGroup,
            backgroundClass: "dxc-border",
            itemGroupClass: "dxc-item",
            titleGroupClass: "dxc-title",
            textField: legendSettings.textField,
            getFormatObject: legendSettings.getFormatObject,
            allowInsidePosition: this._allowLegendInsidePosition()
        });
        this._updateLegend();
        this._layout.add(this._legend)
    },
    _updateLegend: function() {
        var themeManager = this._themeManager;
        var legendOptions = themeManager.getOptions("legend");
        var legendData = this._getLegendData();
        legendOptions.containerBackgroundColor = themeManager.getOptions("containerBackgroundColor");
        legendOptions._incidentOccurred = this._incidentOccurred;
        this._legend.update(legendData, legendOptions, themeManager.theme("legend").title);
        this._change(["LAYOUT"])
    },
    _prepareDrawOptions: function(drawOptions) {
        var animationOptions = this._getAnimationOptions();
        var options = (0, _extend.extend)({}, {
            force: false,
            adjustAxes: true,
            drawLegend: true,
            drawTitle: true,
            animate: animationOptions.enabled,
            animationPointsLimit: animationOptions.maxPointCountSupported
        }, drawOptions, this.__renderOptions);
        if (!(0, _type.isDefined)(options.recreateCanvas)) {
            options.recreateCanvas = options.adjustAxes && options.drawLegend && options.drawTitle
        }
        return options
    },
    _processRefreshData: function(newRefreshAction) {
        var currentRefreshActionPosition = (0, _array.inArray)(this._currentRefreshData, ACTIONS_BY_PRIORITY);
        var newRefreshActionPosition = (0, _array.inArray)(newRefreshAction, ACTIONS_BY_PRIORITY);
        if (!this._currentRefreshData || currentRefreshActionPosition >= 0 && newRefreshActionPosition < currentRefreshActionPosition) {
            this._currentRefreshData = newRefreshAction
        }
        this._requestChange(["REFRESH"])
    },
    _getLegendData: function() {
        return (0, _utils.map)(this._getLegendTargets(), (function(item) {
            var legendData = item.legendData;
            var style = item.getLegendStyles;
            var opacity = style.normal.opacity;
            if (!item.visible) {
                if (!(0, _type.isDefined)(opacity) || opacity > DEFAULT_OPACITY) {
                    opacity = DEFAULT_OPACITY
                }
                legendData.textOpacity = DEFAULT_OPACITY
            }
            var opacityStyle = {
                opacity: opacity
            };
            legendData.states = {
                hover: (0, _extend.extend)({}, style.hover, opacityStyle),
                selection: (0, _extend.extend)({}, style.selection, opacityStyle),
                normal: (0, _extend.extend)({}, style.normal, opacityStyle)
            };
            return legendData
        }))
    },
    _getLegendOptions: function(item) {
        return {
            legendData: {
                text: item[this._legendItemTextField],
                id: item.index,
                visible: true
            },
            getLegendStyles: item.getLegendStyles(),
            visible: item.isVisible()
        }
    },
    _disposeSeries: function(seriesIndex) {
        var _that$series;
        if (this.series) {
            if ((0, _type.isDefined)(seriesIndex)) {
                this.series[seriesIndex].dispose();
                this.series.splice(seriesIndex, 1)
            } else {
                (0, _iterator.each)(this.series, (function(_, s) {
                    return s.dispose()
                }));
                this.series.length = 0
            }
        }
        if (!(null !== (_that$series = this.series) && void 0 !== _that$series && _that$series.length)) {
            this.series = []
        }
    },
    _disposeSeriesFamilies: function() {
        (0, _iterator.each)(this.seriesFamilies || [], (function(_, family) {
            family.dispose()
        }));
        this.seriesFamilies = null;
        this._needHandleRenderComplete = true
    },
    _optionChanged: function(arg) {
        this._themeManager.resetOptions(arg.name);
        this.callBase.apply(this, arguments)
    },
    _applyChanges: function() {
        var that = this;
        that._themeManager.update(that._options.silent());
        that.callBase.apply(that, arguments)
    },
    _optionChangesMap: {
        animation: "ANIMATION",
        dataSource: "DATA_SOURCE",
        palette: "PALETTE",
        paletteExtensionMode: "PALETTE",
        legend: "FORCE_DATA_INIT",
        seriesTemplate: "FORCE_DATA_INIT",
        export: "FORCE_RENDER",
        valueAxis: "AXES_AND_PANES",
        argumentAxis: "AXES_AND_PANES",
        commonAxisSettings: "AXES_AND_PANES",
        panes: "AXES_AND_PANES",
        commonPaneSettings: "AXES_AND_PANES",
        defaultPane: "AXES_AND_PANES",
        containerBackgroundColor: "AXES_AND_PANES",
        rotated: "ROTATED",
        autoHidePointMarkers: "REFRESH_SERIES_REINIT",
        customizePoint: "REFRESH_SERIES_REINIT",
        customizeLabel: "REFRESH_SERIES_REINIT",
        scrollBar: "SCROLL_BAR"
    },
    _optionChangesOrder: ["ROTATED", "PALETTE", "REFRESH_SERIES_REINIT", "AXES_AND_PANES", "INIT", "REINIT", "DATA_SOURCE", "REFRESH_SERIES_DATA_INIT", "DATA_INIT", "FORCE_DATA_INIT", "REFRESH_AXES", "CORRECT_AXIS"],
    _customChangesOrder: ["ANIMATION", "REFRESH_SERIES_FAMILIES", "FORCE_FIRST_DRAWING", "FORCE_DRAWING", "FORCE_RENDER", "VISUAL_RANGE", "SCROLL_BAR", "REINIT", "REFRESH", "FULL_RENDER"],
    _change_ANIMATION: function() {
        this._renderer.updateAnimationOptions(this._getAnimationOptions())
    },
    _change_DATA_SOURCE: function() {
        this._needHandleRenderComplete = true;
        this._updateDataSource()
    },
    _change_PALETTE: function() {
        this._themeManager.updatePalette();
        this._refreshSeries("DATA_INIT")
    },
    _change_REFRESH_SERIES_DATA_INIT: function() {
        this._refreshSeries("DATA_INIT")
    },
    _change_DATA_INIT: function() {
        if ((!this.series || this.needToPopulateSeries) && !this._changes.has("FORCE_DATA_INIT")) {
            this._dataInit()
        }
    },
    _change_FORCE_DATA_INIT: function() {
        this._dataInit()
    },
    _change_REFRESH_SERIES_FAMILIES: function() {
        this._processSeriesFamilies();
        this._populateBusinessRange();
        this._processRefreshData(FORCE_RENDER_REFRESH_ACTION)
    },
    _change_FORCE_RENDER: function() {
        this._processRefreshData(FORCE_RENDER_REFRESH_ACTION)
    },
    _change_AXES_AND_PANES: function() {
        this._refreshSeries("INIT")
    },
    _change_ROTATED: function() {
        this._createScrollBar();
        this._refreshSeries("INIT")
    },
    _change_REFRESH_SERIES_REINIT: function() {
        this._refreshSeries("INIT")
    },
    _change_REFRESH_AXES: function() {
        (0, _utils.setCanvasValues)(this._canvas);
        this._reinitAxes();
        this._requestChange(["CORRECT_AXIS", "FULL_RENDER"])
    },
    _change_SCROLL_BAR: function() {
        this._createScrollBar();
        this._processRefreshData(FORCE_RENDER_REFRESH_ACTION)
    },
    _change_REINIT: function() {
        this._processRefreshData(REINIT_REFRESH_ACTION)
    },
    _change_FORCE_DRAWING: function() {
        this._resetComponentsAnimation()
    },
    _change_FORCE_FIRST_DRAWING: function() {
        this._resetComponentsAnimation(true)
    },
    _resetComponentsAnimation: function(isFirstDrawing) {
        this.series.forEach((function(s) {
            s.resetApplyingAnimation(isFirstDrawing)
        }));
        this._resetAxesAnimation(isFirstDrawing)
    },
    _resetAxesAnimation: _common.noop,
    _refreshSeries: function(actionName) {
        this.needToPopulateSeries = true;
        this._requestChange([actionName])
    },
    _change_CORRECT_AXIS: function() {
        this._correctAxes()
    },
    _doRefresh: function() {
        var methodName = this._currentRefreshData;
        if (methodName) {
            this._currentRefreshData = null;
            this._renderer.stopAllAnimations(true);
            this[methodName]()
        }
    },
    _updateCanvasClipRect: function(canvas) {
        var width = Math.max(canvas.width - canvas.left - canvas.right, 0);
        var height = Math.max(canvas.height - canvas.top - canvas.bottom, 0);
        this._canvasClipRect.attr({
            x: canvas.left,
            y: canvas.top,
            width: width,
            height: height
        });
        this._backgroundRect.attr({
            x: canvas.left,
            y: canvas.top,
            width: width,
            height: height
        })
    },
    _getCanvasClipRectID: function() {
        return this._canvasClipRect.id
    },
    _dataSourceChangedHandler: function() {
        if (this._changes.has("INIT")) {
            this._requestChange(["DATA_INIT"])
        } else {
            this._requestChange(["FORCE_DATA_INIT"])
        }
    },
    _dataInit: function() {
        this._dataSpecificInit(true)
    },
    _processSingleSeries: function(singleSeries) {
        singleSeries.createPoints(false)
    },
    _handleSeriesDataUpdated: function() {
        var _this2 = this;
        if (this._getVisibleSeries().some((function(s) {
                return s.useAggregation()
            }))) {
            this._populateMarginOptions()
        }
        this.series.forEach((function(s) {
            return _this2._processSingleSeries(s)
        }), this)
    },
    _dataSpecificInit: function(needRedraw) {
        if (!this.series || this.needToPopulateSeries) {
            this.series = this._populateSeries()
        }
        this._repopulateSeries();
        this._seriesPopulatedHandlerCore();
        this._populateBusinessRange();
        this._tracker.updateSeries(this.series, this._changes.has("INIT"));
        this._updateLegend();
        if (needRedraw) {
            this._requestChange(["FULL_RENDER"])
        }
    },
    _forceRender: function() {
        this._doRender({
            force: true
        })
    },
    _repopulateSeries: function() {
        var themeManager = this._themeManager;
        var data = this._dataSourceItems();
        var dataValidatorOptions = themeManager.getOptions("dataPrepareSettings");
        var seriesTemplate = themeManager.getOptions("seriesTemplate");
        if (seriesTemplate) {
            this._populateSeries(data)
        }
        this._groupSeries();
        var parsedData = (0, _data_validator.validateData)(data, this._groupsData, this._incidentOccurred, dataValidatorOptions);
        themeManager.resetPalette();
        this.series.forEach((function(singleSeries) {
            singleSeries.updateData(parsedData[singleSeries.getArgumentField()])
        }));
        this._handleSeriesDataUpdated()
    },
    _renderCompleteHandler: function() {
        var allSeriesInited = true;
        if (this._needHandleRenderComplete) {
            (0, _iterator.each)(this.series, (function(_, s) {
                allSeriesInited = allSeriesInited && s.canRenderCompleteHandle()
            }));
            if (allSeriesInited) {
                this._needHandleRenderComplete = false;
                this._eventTrigger("done", {
                    target: this
                })
            }
        }
    },
    _dataIsReady: function() {
        return (0, _type.isDefined)(this.option("dataSource")) && this._dataIsLoaded()
    },
    _populateSeriesOptions: function(data) {
        var that = this;
        var themeManager = that._themeManager;
        var seriesTemplate = themeManager.getOptions("seriesTemplate");
        var seriesOptions = seriesTemplate ? (0, _utils.processSeriesTemplate)(seriesTemplate, data || []) : that.option("series");
        var allSeriesOptions = _isArray(seriesOptions) ? seriesOptions : seriesOptions ? [seriesOptions] : [];
        var extraOptions = that._getExtraOptions();
        var particularSeriesOptions;
        var seriesTheme;
        var seriesThemes = [];
        var seriesVisibilityChanged = function(target) {
            that._specialProcessSeries();
            that._populateBusinessRange(target && target.getValueAxis(), true);
            that._renderer.stopAllAnimations(true);
            that._updateLegend();
            that._requestChange(["FULL_RENDER"])
        };
        for (var i = 0; i < allSeriesOptions.length; i++) {
            particularSeriesOptions = (0, _extend.extend)(true, {}, allSeriesOptions[i], extraOptions);
            if (!(0, _type.isDefined)(particularSeriesOptions.name) || "" === particularSeriesOptions.name) {
                particularSeriesOptions.name = "Series " + (i + 1).toString()
            }
            particularSeriesOptions.rotated = that._isRotated();
            particularSeriesOptions.customizePoint = themeManager.getOptions("customizePoint");
            particularSeriesOptions.customizeLabel = themeManager.getOptions("customizeLabel");
            particularSeriesOptions.visibilityChanged = seriesVisibilityChanged;
            particularSeriesOptions.incidentOccurred = that._incidentOccurred;
            seriesTheme = themeManager.getOptions("series", particularSeriesOptions, allSeriesOptions.length);
            if (that._checkPaneName(seriesTheme)) {
                seriesThemes.push(seriesTheme)
            }
        }
        return seriesThemes
    },
    _populateSeries: function(data) {
        var _that$series2;
        var that = this;
        var seriesBasis = [];
        var incidentOccurred = that._incidentOccurred;
        var seriesThemes = that._populateSeriesOptions(data);
        var particularSeries;
        var disposeSeriesFamilies = false;
        that.needToPopulateSeries = false;
        (0, _iterator.each)(seriesThemes, (function(_, theme) {
            var curSeries = that.series && that.series.filter((function(s) {
                return s.name === theme.name && -1 === seriesBasis.map((function(sb) {
                    return sb.series
                })).indexOf(s)
            }))[0];
            if (curSeries && curSeries.type === theme.type) {
                seriesBasis.push({
                    series: curSeries,
                    options: theme
                })
            } else {
                seriesBasis.push({
                    options: theme
                });
                disposeSeriesFamilies = true
            }
        }));
        0 !== (null === (_that$series2 = that.series) || void 0 === _that$series2 ? void 0 : _that$series2.length) && that._tracker.clearHover();
        (0, _iterator.reverseEach)(that.series, (function(index, series) {
            if (!seriesBasis.some((function(s) {
                    return series === s.series
                }))) {
                that._disposeSeries(index);
                disposeSeriesFamilies = true
            }
        }));
        !disposeSeriesFamilies && (disposeSeriesFamilies = seriesBasis.some((function(sb) {
            return sb.series.name !== seriesThemes[sb.series.index].name
        })));
        that.series = [];
        disposeSeriesFamilies && that._disposeSeriesFamilies();
        that._themeManager.resetPalette();
        var eventPipe = function(data) {
            that.series.forEach((function(currentSeries) {
                currentSeries.notify(data)
            }))
        };
        (0, _iterator.each)(seriesBasis, (function(_, basis) {
            var _that$_argumentAxes$f, _that$_argumentAxes;
            var seriesTheme = basis.options;
            var argumentAxis = null !== (_that$_argumentAxes$f = null === (_that$_argumentAxes = that._argumentAxes) || void 0 === _that$_argumentAxes ? void 0 : _that$_argumentAxes.filter((function(a) {
                return a.pane === seriesTheme.pane
            }))[0]) && void 0 !== _that$_argumentAxes$f ? _that$_argumentAxes$f : that.getArgumentAxis();
            var renderSettings = {
                commonSeriesModes: that._getSelectionModes(),
                argumentAxis: argumentAxis,
                valueAxis: that._getValueAxis(seriesTheme.pane, seriesTheme.axis)
            };
            if (basis.series) {
                particularSeries = basis.series;
                particularSeries.updateOptions(seriesTheme, renderSettings)
            } else {
                particularSeries = new _base_series.Series((0, _extend.extend)({
                    renderer: that._renderer,
                    seriesGroup: that._seriesGroup,
                    labelsGroup: that._labelsGroup,
                    eventTrigger: that._eventTrigger,
                    eventPipe: eventPipe,
                    incidentOccurred: incidentOccurred
                }, renderSettings), seriesTheme)
            }
            if (!particularSeries.isUpdated) {
                incidentOccurred("E2101", [seriesTheme.type])
            } else {
                particularSeries.index = that.series.length;
                that.series.push(particularSeries)
            }
        }));
        return that.series
    },
    getStackedPoints: function(point) {
        var stackName = point.series.getStackName();
        return this._getVisibleSeries().reduce((function(stackPoints, series) {
            if (!(0, _type.isDefined)(series.getStackName()) || !(0, _type.isDefined)(stackName) || stackName === series.getStackName()) {
                stackPoints = stackPoints.concat(series.getPointsByArg(point.argument))
            }
            return stackPoints
        }), [])
    },
    getAllSeries: function() {
        return (this.series || []).slice()
    },
    getSeriesByName: function(name) {
        var found = null;
        (0, _iterator.each)(this.series, (function(i, singleSeries) {
            if (singleSeries.name === name) {
                found = singleSeries;
                return false
            }
        }));
        return found
    },
    getSeriesByPos: function(pos) {
        return (this.series || [])[pos]
    },
    clearSelection: function() {
        this._tracker.clearSelection()
    },
    hideTooltip: function() {
        this._tracker._hideTooltip()
    },
    clearHover: function() {
        this._tracker.clearHover()
    },
    render: function(renderOptions) {
        var that = this;
        that.__renderOptions = renderOptions;
        that.__forceRender = renderOptions && renderOptions.force;
        that.callBase.apply(that, arguments);
        that.__renderOptions = that.__forceRender = null;
        return that
    },
    refresh: function() {
        this._disposeSeries();
        this._disposeSeriesFamilies();
        this._requestChange(["CONTAINER_SIZE", "REFRESH_SERIES_REINIT"])
    },
    _getMinSize: function() {
        var adaptiveLayout = this._layoutManagerOptions();
        return [adaptiveLayout.width, adaptiveLayout.height]
    },
    _change_REFRESH: function() {
        if (!this._changes.has("INIT")) {
            this._doRefresh()
        } else {
            this._currentRefreshData = null
        }
    },
    _change_FULL_RENDER: function() {
        this._forceRender()
    },
    _change_INIT: function() {
        this._reinit()
    },
    _stopCurrentHandling: function() {
        this._tracker.stopCurrentHandling()
    }
});
exports.BaseChart = BaseChart;
REFRESH_SERIES_DATA_INIT_ACTION_OPTIONS.forEach((function(name) {
    BaseChart.prototype._optionChangesMap[name] = "REFRESH_SERIES_DATA_INIT"
}));
FORCE_RENDER_REFRESH_ACTION_OPTIONS.forEach((function(name) {
    BaseChart.prototype._optionChangesMap[name] = "FORCE_RENDER"
}));
REFRESH_SERIES_FAMILIES_ACTION_OPTIONS.forEach((function(name) {
    BaseChart.prototype._optionChangesMap[name] = "REFRESH_SERIES_FAMILIES"
}));
BaseChart.addPlugin(_export.plugin);
BaseChart.addPlugin(_title.plugin);
BaseChart.addPlugin(_data_source.plugin);
BaseChart.addPlugin(_tooltip.plugin);
BaseChart.addPlugin(_loading_indicator.plugin);
var _change_TITLE = BaseChart.prototype._change_TITLE;
BaseChart.prototype._change_TITLE = function() {
    _change_TITLE.apply(this, arguments);
    this._change(["FORCE_RENDER"])
};
