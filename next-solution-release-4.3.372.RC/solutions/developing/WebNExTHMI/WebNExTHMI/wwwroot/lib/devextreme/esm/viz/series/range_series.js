/**
 * DevExtreme (esm/viz/series/range_series.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
var _extend = extend;
import {
    isDefined as _isDefined
} from "../../core/utils/type";
import {
    map as _map
} from "../core/utils";
import {
    noop as _noop
} from "../../core/utils/common";
import {
    chart as scatterSeries
} from "./scatter_series";
import {
    chart as barChart
} from "./bar_series";
import {
    chart as areaChart
} from "./area_series";
var barSeries = barChart.bar;
var areaSeries = areaChart.area;
var chart = {};
var baseRangeSeries = {
    areErrorBarsVisible: _noop,
    _createErrorBarGroup: _noop,
    _checkData: function(data, skippedFields) {
        var valueFields = this.getValueFields();
        return scatterSeries._checkData.call(this, data, skippedFields, {
            minValue: valueFields[0],
            value: valueFields[1]
        }) && data.minValue === data.minValue
    },
    getValueRangeInitialValue: scatterSeries.getValueRangeInitialValue,
    _getPointDataSelector: function(data) {
        var valueFields = this.getValueFields();
        var val1Field = valueFields[0];
        var val2Field = valueFields[1];
        var tagField = this.getTagField();
        var argumentField = this.getArgumentField();
        return data => ({
            tag: data[tagField],
            minValue: this._processEmptyValue(data[val1Field]),
            value: this._processEmptyValue(data[val2Field]),
            argument: data[argumentField],
            data: data
        })
    },
    _defaultAggregator: "range",
    _aggregators: {
        range(_ref, series) {
            var {
                intervalStart: intervalStart,
                intervalEnd: intervalEnd,
                data: data
            } = _ref;
            if (!data.length) {
                return
            }
            var valueFields = series.getValueFields();
            var val1Field = valueFields[0];
            var val2Field = valueFields[1];
            var result = data.reduce((result, item) => {
                var val1 = item[val1Field];
                var val2 = item[val2Field];
                if (!_isDefined(val1) || !_isDefined(val2)) {
                    return result
                }
                result[val1Field] = Math.min(result[val1Field], Math.min(val1, val2));
                result[val2Field] = Math.max(result[val2Field], Math.max(val1, val2));
                return result
            }, {
                [val1Field]: 1 / 0,
                [val2Field]: -1 / 0,
                [series.getArgumentField()]: series._getIntervalCenter(intervalStart, intervalEnd)
            });
            if (!isFinite(result[val1Field]) || !isFinite(result[val2Field])) {
                if (data.filter(i => null === i[val1Field] && null === i[val2Field]).length === data.length) {
                    result[val1Field] = result[val2Field] = null
                } else {
                    return
                }
            }
            return result
        }
    },
    getValueFields: function() {
        return [this._options.rangeValue1Field || "val1", this._options.rangeValue2Field || "val2"]
    },
    getSeriesPairCoord(coord, isArgument) {
        var oppositeCoord = null;
        var {
            rotated: rotated
        } = this._options;
        var isOpposite = !isArgument && !rotated || isArgument && rotated;
        var coordName = isOpposite ? "vy" : "vx";
        var minCoordName = rotated ? "minX" : "minY";
        var oppositeCoordName = isOpposite ? "vx" : "vy";
        var points = this.getPoints();
        for (var i = 0; i < points.length; i++) {
            var p = points[i];
            var tmpCoord = void 0;
            if (isArgument) {
                tmpCoord = p.getCenterCoord()[coordName[1]] === coord ? p[oppositeCoordName] : void 0
            } else {
                var coords = [Math.min(p[coordName], p[minCoordName]), Math.max(p[coordName], p[minCoordName])];
                tmpCoord = coord >= coords[0] && coord <= coords[1] ? p[oppositeCoordName] : void 0
            }
            if (this._checkAxisVisibleAreaCoord(!isArgument, tmpCoord)) {
                oppositeCoord = tmpCoord;
                break
            }
        }
        return oppositeCoord
    }
};
chart.rangebar = _extend({}, barSeries, baseRangeSeries);
chart.rangearea = _extend({}, areaSeries, {
    _drawPoint: function(options) {
        var point = options.point;
        if (point.isInVisibleArea()) {
            point.clearVisibility();
            point.draw(this._renderer, options.groups);
            this._drawnPoints.push(point);
            if (!point.visibleTopMarker) {
                point.hideMarker("top")
            }
            if (!point.visibleBottomMarker) {
                point.hideMarker("bottom")
            }
        } else {
            point.setInvisibility()
        }
    },
    _prepareSegment: function(points, rotated) {
        var processedPoints = this._processSinglePointsAreaSegment(points, rotated);
        var processedMinPointsCoords = _map(processedPoints, (function(pt) {
            return pt.getCoords(true)
        }));
        return {
            line: processedPoints,
            bottomLine: processedMinPointsCoords,
            area: _map(processedPoints, (function(pt) {
                return pt.getCoords()
            })).concat(processedMinPointsCoords.slice().reverse()),
            singlePointSegment: processedPoints !== points
        }
    },
    _getDefaultSegment: function(segment) {
        var defaultSegment = areaSeries._getDefaultSegment.call(this, segment);
        defaultSegment.bottomLine = defaultSegment.line;
        return defaultSegment
    },
    _removeElement: function(element) {
        areaSeries._removeElement.call(this, element);
        element.bottomLine && element.bottomLine.remove()
    },
    _drawElement: function(segment, group) {
        var drawnElement = areaSeries._drawElement.call(this, segment, group);
        drawnElement.bottomLine = this._bordersGroup && this._createBorderElement(segment.bottomLine, {
            "stroke-width": this._styles.normal.border["stroke-width"]
        }).append(this._bordersGroup);
        return drawnElement
    },
    _applyStyle: function(style) {
        var elementsGroup = this._elementsGroup;
        var bordersGroup = this._bordersGroup;
        elementsGroup && elementsGroup.smartAttr(style.elements);
        bordersGroup && bordersGroup.attr(style.border);
        (this._graphics || []).forEach((function(graphic) {
            graphic.line && graphic.line.attr({
                "stroke-width": style.border["stroke-width"]
            });
            graphic.bottomLine && graphic.bottomLine.attr({
                "stroke-width": style.border["stroke-width"]
            })
        }))
    },
    _updateElement: function(element, segment, animate, complete) {
        var bottomLineParams = {
            points: segment.bottomLine
        };
        var bottomBorderElement = element.bottomLine;
        areaSeries._updateElement.apply(this, arguments);
        if (bottomBorderElement) {
            animate ? bottomBorderElement.animate(bottomLineParams) : bottomBorderElement.attr(bottomLineParams)
        }
    }
}, baseRangeSeries);
export {
    chart
};
