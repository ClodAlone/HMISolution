/**
 * DevExtreme (esm/viz/pie_chart.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import consts from "./components/consts";
import {
    normalizeAngle,
    getVerticallyShiftedAngularCoords as _getVerticallyShiftedAngularCoords,
    patchFontOptions
} from "./core/utils";
import {
    extend as _extend
} from "../core/utils/extend";
import {
    isNumeric
} from "../core/utils/type";
import {
    each as _each
} from "../core/utils/iterator";
import {
    Range
} from "./translators/range";
import registerComponent from "../core/component_registrator";
import {
    BaseChart,
    overlapping
} from "./chart_components/base_chart";
import {
    noop as _noop
} from "../core/utils/common";
import {
    Translator1D
} from "./translators/translator1d";
var {
    states: states
} = consts;
var seriesSpacing = consts.pieSeriesSpacing;
var OPTIONS_FOR_REFRESH_SERIES = ["startAngle", "innerRadius", "segmentsDirection", "type"];
var NORMAL_STATE = states.normalMark;
var HOVER_STATE = states.hoverMark;
var SELECTED_STATE = states.selectedMark;
var MAX_RESOLVE_ITERATION_COUNT = 5;
var LEGEND_ACTIONS = [states.resetItem, states.applyHover, states.applySelected, states.applySelected];

function getLegendItemAction(points) {
    var state = NORMAL_STATE;
    points.forEach(point => {
        var _point$series;
        var seriesOptions = null === (_point$series = point.series) || void 0 === _point$series ? void 0 : _point$series.getOptions();
        var pointState = point.fullState;
        if ("none" === (null === seriesOptions || void 0 === seriesOptions ? void 0 : seriesOptions.hoverMode)) {
            pointState &= ~HOVER_STATE
        }
        if ("none" === (null === seriesOptions || void 0 === seriesOptions ? void 0 : seriesOptions.selectionMode)) {
            pointState &= ~SELECTED_STATE
        }
        state |= pointState
    });
    return LEGEND_ACTIONS[state]
}

function correctPercentValue(value) {
    if (isNumeric(value)) {
        if (value > 1) {
            value = 1
        } else if (value < 0) {
            value = 0
        }
    } else {
        value = void 0
    }
    return value
}
var pieSizeEqualizer = function() {
    function removeFromList(list, item) {
        return list.filter((function(li) {
            return li !== item
        }))
    }
    var pies = [];
    var timers = {};
    return {
        queue: function(pie) {
            var group = pie.getSizeGroup();
            pies = (list = pies, item = pie, removeFromList(list, item).concat(item));
            var list, item;
            clearTimeout(timers[group]);
            timers[group] = setTimeout((function() {
                ! function(group, allPies) {
                    var pies = allPies.filter(p => p._isVisible() && p.getSizeGroup() === group);
                    var minRadius = Math.min.apply(null, pies.map(p => p.getSizeGroupLayout().radius));
                    var minPie = pies.filter(p => p.getSizeGroupLayout().radius === minRadius);
                    pies.forEach(p => p.render({
                        force: true,
                        sizeGroupLayout: minPie.length ? minPie[0].getSizeGroupLayout() : {}
                    }))
                }(group, pies)
            }))
        },
        remove: function(pie) {
            pies = removeFromList(pies, pie);
            if (!pies.length) {
                timers = {}
            }
        }
    }
}();
var dxPieChart = BaseChart.inherit({
    _themeSection: "pie",
    _layoutManagerOptions: function() {
        return _extend(true, {}, this.callBase(), {
            piePercentage: correctPercentValue(this._themeManager.getOptions("diameter")),
            minPiePercentage: correctPercentValue(this._themeManager.getOptions("minDiameter"))
        })
    },
    _customChangesOrder: ["CENTER_TEMPLATE"],
    _optionChangesMap: {
        diameter: "REINIT",
        minDiameter: "REINIT",
        sizeGroup: "REINIT",
        centerTemplate: "CENTER_TEMPLATE"
    },
    _change_CENTER_TEMPLATE() {
        this._renderCenterTemplate()
    },
    _disposeCore: function() {
        pieSizeEqualizer.remove(this);
        this.callBase();
        this._centerTemplateGroup.linkOff().dispose()
    },
    _groupSeries: function() {
        var series = this.series;
        this._groupsData = {
            groups: [{
                series: series,
                valueOptions: {
                    valueType: "numeric"
                }
            }],
            argumentOptions: series[0] && series[0].getOptions()
        }
    },
    getArgumentAxis: function() {
        return null
    },
    _getValueAxis: function() {
        var translator = (new Translator1D).setCodomain(360, 0);
        return {
            getTranslator: function() {
                return translator
            },
            setBusinessRange: function(range) {
                translator.setDomain(range.min, range.max)
            }
        }
    },
    _populateBusinessRange: function() {
        this.series.map((function(series) {
            var range = new Range;
            range.addRange(series.getRangeData().val);
            series.getValueAxis().setBusinessRange(range);
            return range
        }))
    },
    _specialProcessSeries: function() {
        _each(this.series, (function(_, singleSeries) {
            singleSeries.arrangePoints()
        }))
    },
    _checkPaneName: function() {
        return true
    },
    _processSingleSeries: function(singleSeries) {
        this.callBase(singleSeries);
        singleSeries.arrangePoints()
    },
    _handleSeriesDataUpdated: function() {
        var maxPointCount = 0;
        this.series.forEach((function(s) {
            maxPointCount = Math.max(s.getPointsCount(), maxPointCount)
        }));
        this.series.forEach((function(s) {
            s.setMaxPointsCount(maxPointCount)
        }));
        this.callBase()
    },
    _getLegendOptions: function(item) {
        var legendItem = this.callBase(item);
        var legendData = legendItem.legendData;
        legendData.argument = item.argument;
        legendData.argumentIndex = item.argumentIndex;
        legendData.points = [item];
        return legendItem
    },
    _getLegendTargets: function() {
        var that = this;
        var itemsByArgument = {};
        (that.series || []).forEach((function(series) {
            series.getPoints().forEach((function(point) {
                var argument = point.argument.valueOf();
                var index = series.getPointsByArg(argument).indexOf(point);
                var key = argument.valueOf().toString() + index;
                itemsByArgument[key] = itemsByArgument[key] || [];
                var argumentCount = itemsByArgument[key].push(point);
                point.index = itemsByArgument[key][argumentCount - 2] ? itemsByArgument[key][argumentCount - 2].index : Object.keys(itemsByArgument).length - 1;
                point.argumentIndex = index
            }))
        }));
        var items = [];
        _each(itemsByArgument, (function(_, points) {
            points.forEach((function(point, index) {
                if (0 === index) {
                    items.push(that._getLegendOptions(point));
                    return
                }
                var item = items[items.length - 1];
                item.legendData.points.push(point);
                if (!item.visible) {
                    item.visible = point.isVisible()
                }
            }))
        }));
        return items
    },
    _getLayoutTargets: function() {
        return [{
            canvas: this._canvas
        }]
    },
    _getLayoutSeries: function(series, drawOptions) {
        var layout;
        var canvas = this._canvas;
        var drawnLabels = false;
        layout = this.layoutManager.applyPieChartSeriesLayout(canvas, series, true);
        series.forEach((function(singleSeries) {
            singleSeries.correctPosition(layout, canvas);
            drawnLabels = singleSeries.drawLabelsWOPoints() || drawnLabels
        }));
        if (drawnLabels) {
            layout = this.layoutManager.applyPieChartSeriesLayout(canvas, series, drawOptions.hideLayoutLabels)
        }
        series.forEach((function(singleSeries) {
            singleSeries.hideLabels()
        }));
        this._sizeGroupLayout = {
            x: layout.centerX,
            y: layout.centerY,
            radius: layout.radiusOuter,
            drawOptions: drawOptions
        };
        return layout
    },
    _getLayoutSeriesForEqualPies: function(series, sizeGroupLayout) {
        var canvas = this._canvas;
        var layout = this.layoutManager.applyEqualPieChartLayout(series, sizeGroupLayout);
        series.forEach((function(s) {
            s.correctPosition(layout, canvas);
            s.drawLabelsWOPoints()
        }));
        this.layoutManager.correctPieLabelRadius(series, layout, canvas);
        return layout
    },
    _updateSeriesDimensions: function(drawOptions) {
        var visibleSeries = this._getVisibleSeries();
        var lengthVisibleSeries = visibleSeries.length;
        var innerRad;
        var delta;
        var layout;
        var sizeGroupLayout = drawOptions.sizeGroupLayout;
        if (lengthVisibleSeries) {
            layout = sizeGroupLayout ? this._getLayoutSeriesForEqualPies(visibleSeries, sizeGroupLayout) : this._getLayoutSeries(visibleSeries, drawOptions);
            delta = (layout.radiusOuter - layout.radiusInner - seriesSpacing * (lengthVisibleSeries - 1)) / lengthVisibleSeries;
            innerRad = layout.radiusInner;
            this._setGeometry(layout);
            visibleSeries.forEach((function(singleSeries) {
                singleSeries.correctRadius({
                    radiusInner: innerRad,
                    radiusOuter: innerRad + delta
                });
                innerRad += delta + seriesSpacing
            }))
        }
    },
    _renderSeries: function(drawOptions, isRotated, isLegendInside) {
        this._calculateSeriesLayout(drawOptions, isRotated);
        if (!drawOptions.sizeGroupLayout && this.getSizeGroup()) {
            pieSizeEqualizer.queue(this);
            this._clearCanvas();
            return
        }
        this._renderSeriesElements(drawOptions, isLegendInside)
    },
    _createHtmlStructure() {
        this.callBase();
        this._centerTemplateGroup = this._renderer.g().attr({
            class: "dxc-hole-template"
        }).linkOn(this._renderer.root, "center-template").css(patchFontOptions(this._themeManager._font)).linkAppend()
    },
    _renderExtraElements() {
        this._requestChange(["CENTER_TEMPLATE"])
    },
    _renderCenterTemplate() {
        var template = this.option("centerTemplate");
        var centerTemplateGroup = this._centerTemplateGroup.clear();
        if (!template) {
            return
        }
        centerTemplateGroup.attr({
            visibility: "hidden"
        });
        template = this._getTemplate(template);
        template.render({
            model: this,
            container: centerTemplateGroup.element,
            onRendered: () => {
                var group = centerTemplateGroup;
                var bBox = group.getBBox();
                group.move(this._center.x - (bBox.x + bBox.width / 2), this._center.y - (bBox.y + bBox.height / 2));
                group.attr({
                    visibility: "visible"
                })
            }
        })
    },
    getInnerRadius() {
        return this._innerRadius
    },
    _getLegendCallBack: function() {
        var that = this;
        var legend = this._legend;
        var items = this._getLegendTargets().map((function(i) {
            return i.legendData
        }));
        return function(target) {
            items.forEach((function(data) {
                var points = [];
                var callback = legend.getActionCallback({
                    index: data.id
                });
                that.series.forEach((function(series) {
                    var seriesPoints = series.getPointsByKeys(data.argument, data.argumentIndex);
                    points.push.apply(points, seriesPoints)
                }));
                if (target && target.argument === data.argument && target.argumentIndex === data.argumentIndex) {
                    points.push(target)
                }
                callback(getLegendItemAction(points))
            }))
        }
    },
    _locateLabels(resolveLabelOverlapping) {
        var iterationCount = 0;
        var labelsWereOverlapped;
        var wordWrapApplied;
        do {
            wordWrapApplied = this._adjustSeriesLabels("shift" === resolveLabelOverlapping);
            labelsWereOverlapped = this._resolveLabelOverlapping(resolveLabelOverlapping)
        } while ((labelsWereOverlapped || wordWrapApplied) && ++iterationCount < MAX_RESOLVE_ITERATION_COUNT)
    },
    _adjustSeriesLabels: function(moveLabelsFromCenter) {
        return this.series.reduce((r, s) => s.adjustLabels(moveLabelsFromCenter) || r, false)
    },
    _applyExtraSettings: _noop,
    _resolveLabelOverlappingShift: function() {
        var that = this;
        var inverseDirection = "anticlockwise" === that.option("segmentsDirection");
        var seriesByPosition = that.series.reduce((function(r, s) {
            (r[s.getOptions().label.position] || r.outside).push(s);
            return r
        }), {
            inside: [],
            columns: [],
            outside: []
        });
        var labelsOverlapped = false;
        if (seriesByPosition.inside.length > 0) {
            labelsOverlapped = resolve(seriesByPosition.inside.reduce((function(r, singleSeries) {
                return singleSeries.getVisiblePoints().reduce((function(r, point) {
                    r.left.push(point);
                    return r
                }), r)
            }), {
                left: [],
                right: []
            }), shiftInColumnFunction) || labelsOverlapped
        }
        labelsOverlapped = seriesByPosition.columns.reduce((r, singleSeries) => resolve(dividePoints(singleSeries), shiftInColumnFunction) || r, labelsOverlapped);
        if (seriesByPosition.outside.length > 0) {
            labelsOverlapped = resolve(seriesByPosition.outside.reduce((function(r, singleSeries) {
                return dividePoints(singleSeries, r)
            }), null), (function(box, length) {
                return _getVerticallyShiftedAngularCoords(box, -length, that._center)
            })) || labelsOverlapped
        }
        return labelsOverlapped;

        function dividePoints(series, points) {
            return series.getVisiblePoints().reduce((function(r, point) {
                var angle = normalizeAngle(point.middleAngle);
                (angle <= 90 || angle >= 270 ? r.right : r.left).push(point);
                return r
            }), points || {
                left: [],
                right: []
            })
        }

        function resolve(points, shiftCallback) {
            var overlapped;
            if (inverseDirection) {
                points.left.reverse();
                points.right.reverse()
            }
            overlapped = overlapping.resolveLabelOverlappingInOneDirection(points.left, that._canvas, false, false, shiftCallback);
            return overlapping.resolveLabelOverlappingInOneDirection(points.right, that._canvas, false, false, shiftCallback) || overlapped
        }

        function shiftInColumnFunction(box, length) {
            return {
                x: box.x,
                y: box.y - length
            }
        }
    },
    _setGeometry: function(_ref) {
        var {
            centerX: x,
            centerY: y,
            radiusInner: radiusInner
        } = _ref;
        this._center = {
            x: x,
            y: y
        };
        this._innerRadius = radiusInner
    },
    _disposeSeries(seriesIndex) {
        this.callBase.apply(this, arguments);
        this._abstractSeries = null
    },
    _legendDataField: "point",
    _legendItemTextField: "argument",
    _applyPointMarkersAutoHiding: _noop,
    _renderTrackers: _noop,
    _trackerType: "PieTracker",
    _createScrollBar: _noop,
    _updateAxesLayout: _noop,
    _applyClipRects: _noop,
    _appendAdditionalSeriesGroups: _noop,
    _prepareToRender: _noop,
    _isLegendInside: _noop,
    _renderAxes: _noop,
    _shrinkAxes: _noop,
    _isRotated: _noop,
    _seriesPopulatedHandlerCore: _noop,
    _reinitAxes: _noop,
    _correctAxes: _noop,
    _getExtraOptions: function() {
        return {
            startAngle: this.option("startAngle"),
            innerRadius: this.option("innerRadius"),
            segmentsDirection: this.option("segmentsDirection"),
            type: this.option("type")
        }
    },
    getSizeGroup: function() {
        return this._themeManager.getOptions("sizeGroup")
    },
    getSizeGroupLayout: function() {
        return this._sizeGroupLayout || {}
    }
});
_each(OPTIONS_FOR_REFRESH_SERIES, (function(_, name) {
    dxPieChart.prototype._optionChangesMap[name] = "REFRESH_SERIES_DATA_INIT"
}));
import {
    plugins
} from "./core/annotations";
dxPieChart.addPlugin(plugins.core);
dxPieChart.addPlugin(plugins.pieChart);
registerComponent("dxPieChart", dxPieChart);
export default dxPieChart;
