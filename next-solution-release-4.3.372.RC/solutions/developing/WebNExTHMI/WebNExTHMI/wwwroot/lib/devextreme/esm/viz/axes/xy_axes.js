/**
 * DevExtreme (esm/viz/axes/xy_axes.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    Range
} from "../translators/range";
import formatHelper from "../../format_helper";
import dateUtils from "../../core/utils/date";
import {
    extend
} from "../../core/utils/extend";
import {
    generateDateBreaks
} from "./datetime_breaks";
import {
    noop
} from "../../core/utils/common";
import {
    getLog,
    patchFontOptions,
    getCosAndSin
} from "../core/utils";
import {
    isDefined
} from "../../core/utils/type";
import constants from "./axes_constants";
var getNextDateUnit = dateUtils.getNextDateUnit;
var correctDateWithUnitBeginning = dateUtils.correctDateWithUnitBeginning;
var _math = Math;
var _max = _math.max;
var TOP = constants.top;
var BOTTOM = constants.bottom;
var LEFT = constants.left;
var RIGHT = constants.right;
var CENTER = constants.center;
var SCALE_BREAK_OFFSET = 3;
var RANGE_RATIO = .3;
var WAVED_LINE_CENTER = 2;
var WAVED_LINE_TOP = 0;
var WAVED_LINE_BOTTOM = 4;
var WAVED_LINE_LENGTH = 24;
var TICKS_CORRECTIONS = {
    left: -1,
    top: -1,
    right: 0,
    bottom: 0,
    center: -.5
};

function prepareDatesDifferences(datesDifferences, tickInterval) {
    var dateUnitInterval;
    var i;
    if ("week" === tickInterval) {
        tickInterval = "day"
    }
    if ("quarter" === tickInterval) {
        tickInterval = "month"
    }
    if (datesDifferences[tickInterval]) {
        for (i = 0; i < dateUtils.dateUnitIntervals.length; i++) {
            dateUnitInterval = dateUtils.dateUnitIntervals[i];
            if (datesDifferences[dateUnitInterval]) {
                datesDifferences[dateUnitInterval] = false;
                datesDifferences.count--
            }
            if (dateUnitInterval === tickInterval) {
                break
            }
        }
    }
}

function sortingBreaks(breaks) {
    return breaks.sort((function(a, b) {
        return a.from - b.from
    }))
}

function getMarkerDates(min, max, markerInterval) {
    var origMin = min;
    var dates;
    min = correctDateWithUnitBeginning(min, markerInterval);
    max = correctDateWithUnitBeginning(max, markerInterval);
    dates = dateUtils.getSequenceByInterval(min, max, markerInterval);
    if (dates.length && origMin > dates[0]) {
        dates = dates.slice(1)
    }
    return dates
}

function getStripHorizontalAlignmentPosition(alignment) {
    var position = "start";
    if ("center" === alignment) {
        position = "center"
    }
    if ("right" === alignment) {
        position = "end"
    }
    return position
}

function getStripVerticalAlignmentPosition(alignment) {
    var position = "start";
    if ("center" === alignment) {
        position = "center"
    }
    if ("bottom" === alignment) {
        position = "end"
    }
    return position
}

function getMarkerInterval(tickInterval) {
    var markerInterval = getNextDateUnit(tickInterval);
    if ("quarter" === markerInterval) {
        markerInterval = getNextDateUnit(markerInterval)
    }
    return markerInterval
}

function getMarkerFormat(curDate, prevDate, tickInterval, markerInterval) {
    var format = markerInterval;
    var datesDifferences = prevDate && dateUtils.getDatesDifferences(prevDate, curDate);
    if (prevDate && "year" !== tickInterval) {
        prepareDatesDifferences(datesDifferences, tickInterval);
        format = formatHelper.getDateFormatByDifferences(datesDifferences)
    }
    return format
}

function getMaxSide(act, boxes) {
    return boxes.reduce((function(prevValue, box) {
        return _max(prevValue, act(box))
    }), 0)
}

function getDistanceByAngle(bBox, rotationAngle) {
    rotationAngle = _math.abs(rotationAngle);
    rotationAngle = rotationAngle % 180 >= 90 ? 90 - rotationAngle % 90 : rotationAngle % 90;
    var a = rotationAngle * (_math.PI / 180);
    if (a >= _math.atan(bBox.height / bBox.width)) {
        return bBox.height / _math.abs(_math.sin(a))
    } else {
        return bBox.width
    }
}

function getMaxConstantLinePadding(constantLines) {
    return constantLines.reduce((function(padding, options) {
        return _max(padding, options.paddingTopBottom)
    }), 0)
}

function getConstantLineLabelMarginForVerticalAlignment(constantLines, alignment, labelHeight) {
    return constantLines.some((function(options) {
        return options.label.verticalAlignment === alignment
    })) && labelHeight || 0
}

function getLeftMargin(bBox) {
    return _math.abs(bBox.x) || 0
}

function getRightMargin(bBox) {
    return _math.abs(bBox.width - _math.abs(bBox.x)) || 0
}

function generateRangesOnPoints(points, edgePoints, getRange) {
    var i;
    var length;
    var maxRange = null;
    var ranges = [];
    var curValue;
    var prevValue;
    var curRange;
    for (i = 1, length = points.length; i < length; i++) {
        curValue = points[i];
        prevValue = points[i - 1];
        curRange = getRange(curValue, prevValue);
        if (edgePoints.indexOf(curValue) >= 0) {
            if (!maxRange || curRange > maxRange.length) {
                maxRange = {
                    start: curValue,
                    end: prevValue,
                    length: curRange
                }
            }
        } else {
            if (maxRange && curRange < maxRange.length) {
                ranges.push(maxRange)
            } else {
                ranges.push({
                    start: curValue,
                    end: prevValue,
                    length: curRange
                })
            }
            maxRange = null
        }
    }
    if (maxRange) {
        ranges.push(maxRange)
    }
    return ranges
}

function generateAutoBreaks(_ref, series, _ref2) {
    var {
        logarithmBase: logarithmBase,
        type: type,
        maxAutoBreakCount: maxAutoBreakCount
    } = _ref;
    var {
        minVisible: minVisible,
        maxVisible: maxVisible
    } = _ref2;
    var breaks = [];
    var getRange = "logarithmic" === type ? (min, max) => getLog(max / min, logarithmBase) : (min, max) => max - min;
    var visibleRange = getRange(minVisible, maxVisible);
    var points = series.reduce((result, s) => {
        var points = s.getPointsInViewPort();
        result[0] = result[0].concat(points[0]);
        result[1] = result[1].concat(points[1]);
        return result
    }, [
        [],
        []
    ]);
    var sortedAllPoints = points[0].concat(points[1]).sort((a, b) => b - a);
    var edgePoints = points[1].filter(p => points[0].indexOf(p) < 0);
    var minDiff = RANGE_RATIO * visibleRange;
    var ranges = generateRangesOnPoints(sortedAllPoints, edgePoints, getRange).sort((a, b) => b.length - a.length);
    var epsilon = _math.min.apply(null, ranges.map(r => r.length)) / 1e3;
    var _maxAutoBreakCount = isDefined(maxAutoBreakCount) ? _math.min(maxAutoBreakCount, ranges.length) : ranges.length;
    for (var i = 0; i < _maxAutoBreakCount; i++) {
        if (ranges[i].length >= minDiff) {
            if (visibleRange <= ranges[i].length) {
                break
            }
            visibleRange -= ranges[i].length;
            if (visibleRange > epsilon || visibleRange < -epsilon) {
                breaks.push({
                    from: ranges[i].start,
                    to: ranges[i].end
                });
                minDiff = RANGE_RATIO * visibleRange
            }
        } else {
            break
        }
    }
    sortingBreaks(breaks);
    return breaks
}
export default {
    linear: {
        _getStep: function(boxes, rotationAngle) {
            var spacing = this._options.label.minSpacing;
            var func = this._isHorizontal ? function(box) {
                return box.width + spacing
            } : function(box) {
                return box.height
            };
            var maxLabelLength = getMaxSide(func, boxes);
            if (rotationAngle) {
                maxLabelLength = getDistanceByAngle({
                    width: maxLabelLength,
                    height: this._getMaxLabelHeight(boxes, 0)
                }, rotationAngle)
            }
            return constants.getTicksCountInRange(this._majorTicks, this._isHorizontal ? "x" : "y", maxLabelLength)
        },
        _getMaxLabelHeight: function(boxes, spacing) {
            return getMaxSide((function(box) {
                return box.height
            }), boxes) + spacing
        },
        _validateOverlappingMode: function(mode, displayMode) {
            if (this._isHorizontal && ("rotate" === displayMode || "stagger" === displayMode) || !this._isHorizontal) {
                return constants.validateOverlappingMode(mode)
            }
            return mode
        },
        _validateDisplayMode: function(mode) {
            return this._isHorizontal ? mode : "standard"
        },
        getMarkerTrackers: function() {
            return this._markerTrackers
        },
        _getSharpParam: function(opposite) {
            return this._isHorizontal ^ opposite ? "h" : "v"
        },
        _createAxisElement: function() {
            return this._renderer.path([], "line")
        },
        _updateAxisElementPosition: function() {
            var axisCoord = this._axisPosition;
            var canvas = this._getCanvasStartEnd();
            this._axisElement.attr({
                points: this._isHorizontal ? [canvas.start, axisCoord, canvas.end, axisCoord] : [axisCoord, canvas.start, axisCoord, canvas.end]
            })
        },
        _getTranslatedCoord: function(value, offset) {
            return this._translator.translate(value, offset)
        },
        _initAxisPositions() {
            if (this.customPositionIsAvailable()) {
                this._customBoundaryPosition = this.getCustomBoundaryPosition()
            }
            if (!this.customPositionIsAvailable() || this.customPositionIsBoundary()) {
                this._axisPosition = this.getPredefinedPosition(this.getResolvedBoundaryPosition())
            } else {
                this._axisPosition = this.getCustomPosition()
            }
        },
        _getTickMarkPoints(coords, length, tickOptions) {
            var isHorizontal = this._isHorizontal;
            var tickOrientation = this._options.tickOrientation;
            var labelPosition = this._options.label.position;
            var tickStartCoord;
            if (isDefined(tickOrientation)) {
                tickStartCoord = TICKS_CORRECTIONS[tickOrientation] * length
            } else {
                var shift = tickOptions.shift || 0;
                if (!isHorizontal && labelPosition === LEFT || isHorizontal && labelPosition !== BOTTOM) {
                    shift = -shift
                }
                tickStartCoord = shift + this.getTickStartPositionShift(length)
            }
            return [coords.x + (isHorizontal ? 0 : tickStartCoord), coords.y + (isHorizontal ? tickStartCoord : 0), coords.x + (isHorizontal ? 0 : tickStartCoord + length), coords.y + (isHorizontal ? tickStartCoord + length : 0)]
        },
        getTickStartPositionShift(length) {
            var width = this._options.width;
            var position = this.getResolvedBoundaryPosition();
            return length % 2 === 1 ? width % 2 === 0 && (position === LEFT || position === TOP) || width % 2 === 1 && (position === RIGHT || position === BOTTOM) && !this.hasNonBoundaryPosition() ? Math.floor(-length / 2) : -Math.floor(length / 2) : -length / 2 + (width % 2 === 0 ? 0 : position === BOTTOM || position === RIGHT ? -1 : 1)
        },
        _getTitleCoords: function() {
            var horizontal = this._isHorizontal;
            var x = this._axisPosition;
            var y = this._axisPosition;
            var align = this._options.title.alignment;
            var canvas = this._getCanvasStartEnd();
            var fromStartToEnd = horizontal || this._options.position === LEFT;
            var canvasStart = fromStartToEnd ? canvas.start : canvas.end;
            var canvasEnd = fromStartToEnd ? canvas.end : canvas.start;
            var coord = align === LEFT ? canvasStart : align === RIGHT ? canvasEnd : canvas.start + (canvas.end - canvas.start) / 2;
            if (horizontal) {
                x = coord
            } else {
                y = coord
            }
            return {
                x: x,
                y: y
            }
        },
        _drawTitleText: function(group, coords) {
            var options = this._options;
            var titleOptions = options.title;
            var attrs = {
                opacity: titleOptions.opacity,
                align: titleOptions.alignment,
                class: titleOptions.cssClass
            };
            if (!titleOptions.text || !group) {
                return
            }
            coords = coords || this._getTitleCoords();
            if (!this._isHorizontal) {
                attrs.rotate = options.position === LEFT ? 270 : 90
            }
            var text = this._renderer.text(titleOptions.text, coords.x, coords.y).css(patchFontOptions(titleOptions.font)).attr(attrs).append(group);
            this._checkTitleOverflow(text);
            return text
        },
        _updateTitleCoords: function() {
            this._title && this._title.element.attr(this._getTitleCoords())
        },
        _drawTitle: function() {
            var title = this._drawTitleText(this._axisTitleGroup);
            if (title) {
                this._title = {
                    element: title
                }
            }
        },
        _measureTitle: function() {
            if (this._title) {
                if (this._title.bBox && !this._title.originalSize) {
                    this._title.originalSize = this._title.bBox
                }
                this._title.bBox = this._title.element.getBBox()
            }
        },
        _drawDateMarker: function(date, options, range) {
            var markerOptions = this._options.marker;
            var invert = this._translator.getBusinessRange().invert;
            var textIndent = markerOptions.width + markerOptions.textLeftIndent;
            var pathElement;
            if (null === options.x) {
                return
            }
            if (!options.withoutStick) {
                pathElement = this._renderer.path([options.x, options.y, options.x, options.y + markerOptions.separatorHeight], "line").attr({
                    "stroke-width": markerOptions.width,
                    stroke: markerOptions.color,
                    "stroke-opacity": markerOptions.opacity,
                    sharp: "h"
                }).append(this._axisElementsGroup)
            }
            var text = String(this.formatLabel(date, options.labelOptions, range));
            return {
                date: date,
                x: options.x,
                y: options.y,
                cropped: options.withoutStick,
                label: this._renderer.text(text, options.x, options.y).css(patchFontOptions(markerOptions.label.font)).append(this._axisElementsGroup),
                line: pathElement,
                getContentContainer() {
                    return this.label
                },
                getEnd: function() {
                    return this.x + (invert ? -1 : 1) * (textIndent + this.labelBBox.width)
                },
                setTitle: function() {
                    this.title = text
                },
                hideLabel: function() {
                    this.label.dispose();
                    this.label = null;
                    this.title = text
                },
                hide: function() {
                    if (pathElement) {
                        pathElement.dispose();
                        pathElement = null
                    }
                    this.label.dispose();
                    this.label = null;
                    this.hidden = true
                }
            }
        },
        _drawDateMarkers: function() {
            var that = this;
            var options = that._options;
            var translator = that._translator;
            var viewport = that._getViewportRange();
            var minBound = viewport.minVisible;
            var dateMarkers = [];
            var dateMarker;

            function draw(markerDate, format, withoutStick) {
                return that._drawDateMarker(markerDate, {
                    x: translator.translate(markerDate),
                    y: markersAreaTop,
                    labelOptions: that._getLabelFormatOptions(format),
                    withoutStick: withoutStick
                }, viewport)
            }
            if (viewport.isEmpty() || !options.marker.visible || "datetime" !== options.argumentType || "discrete" === options.type || that._majorTicks.length <= 1) {
                return []
            }
            var markersAreaTop = that._axisPosition + options.marker.topIndent;
            var tickInterval = dateUtils.getDateUnitInterval(this._tickInterval);
            var markerInterval = getMarkerInterval(tickInterval);
            var markerDates = getMarkerDates(minBound, viewport.maxVisible, markerInterval);
            if (markerDates.length > 1 || 1 === markerDates.length && minBound < markerDates[0]) {
                dateMarkers = markerDates.reduce((function(markers, curDate, i, dates) {
                    var marker = draw(curDate, getMarkerFormat(curDate, dates[i - 1] || minBound < curDate && minBound, tickInterval, markerInterval));
                    marker && markers.push(marker);
                    return markers
                }), []);
                if (minBound < markerDates[0]) {
                    dateMarker = draw(minBound, getMarkerFormat(minBound, markerDates[0], tickInterval, markerInterval), true);
                    dateMarker && dateMarkers.unshift(dateMarker)
                }
            }
            return dateMarkers
        },
        _adjustDateMarkers: function(offset) {
            offset = offset || 0;
            var that = this;
            var markerOptions = this._options.marker;
            var textIndent = markerOptions.width + markerOptions.textLeftIndent;
            var invert = this._translator.getBusinessRange().invert;
            var canvas = that._getCanvasStartEnd();
            var dateMarkers = this._dateMarkers;
            if (!dateMarkers.length) {
                return offset
            }
            if (dateMarkers[0].cropped) {
                if (!this._checkMarkersPosition(invert, dateMarkers[1], dateMarkers[0])) {
                    dateMarkers[0].hideLabel()
                }
            }
            var prevDateMarker;
            dateMarkers.forEach((function(marker, i, markers) {
                if (marker.cropped) {
                    return
                }
                if (invert ? marker.getEnd() < canvas.end : marker.getEnd() > canvas.end) {
                    marker.hideLabel()
                } else if (that._checkMarkersPosition(invert, marker, prevDateMarker)) {
                    prevDateMarker = marker
                } else {
                    marker.hide()
                }
            }));
            this._dateMarkers.forEach((function(marker) {
                if (marker.label) {
                    var labelBBox = marker.labelBBox;
                    var dy = marker.y + markerOptions.textTopIndent - labelBBox.y;
                    marker.label.attr({
                        translateX: invert ? marker.x - textIndent - labelBBox.x - labelBBox.width : marker.x + textIndent - labelBBox.x,
                        translateY: dy + offset
                    })
                }
                if (marker.line) {
                    marker.line.attr({
                        translateY: offset
                    })
                }
            }));
            that._initializeMarkersTrackers(offset);
            return offset + markerOptions.topIndent + markerOptions.separatorHeight
        },
        _checkMarkersPosition: function(invert, dateMarker, prevDateMarker) {
            if (void 0 === prevDateMarker) {
                return true
            }
            return invert ? dateMarker.x < prevDateMarker.getEnd() : dateMarker.x > prevDateMarker.getEnd()
        },
        _initializeMarkersTrackers: function(offset) {
            var separatorHeight = this._options.marker.separatorHeight;
            var renderer = this._renderer;
            var businessRange = this._translator.getBusinessRange();
            var canvas = this._getCanvasStartEnd();
            var group = this._axisElementsGroup;
            this._markerTrackers = this._dateMarkers.filter((function(marker) {
                return !marker.hidden
            })).map((function(marker, i, markers) {
                var nextMarker = markers[i + 1] || {
                    x: canvas.end,
                    date: businessRange.max
                };
                var x = marker.x;
                var y = marker.y + offset;
                var markerTracker = renderer.path([x, y, x, y + separatorHeight, nextMarker.x, y + separatorHeight, nextMarker.x, y, x, y], "area").attr({
                    "stroke-width": 1,
                    stroke: "grey",
                    fill: "grey",
                    opacity: 1e-4
                }).append(group);
                markerTracker.data("range", {
                    startValue: marker.date,
                    endValue: nextMarker.date
                });
                if (marker.title) {
                    markerTracker.setTitle(marker.title)
                }
                return markerTracker
            }))
        },
        _getLabelFormatOptions: function(formatString) {
            var markerLabelOptions = this._markerLabelOptions;
            if (!markerLabelOptions) {
                this._markerLabelOptions = markerLabelOptions = extend(true, {}, this._options.marker.label)
            }
            if (!isDefined(this._options.marker.label.format)) {
                markerLabelOptions.format = formatString
            }
            return markerLabelOptions
        },
        _adjustConstantLineLabels: function(constantLines) {
            var that = this;
            var axisPosition = that._options.position;
            var canvas = that.getCanvas();
            var canvasLeft = canvas.left;
            var canvasRight = canvas.width - canvas.right;
            var canvasTop = canvas.top;
            var canvasBottom = canvas.height - canvas.bottom;
            var verticalCenter = canvasTop + (canvasBottom - canvasTop) / 2;
            var horizontalCenter = canvasLeft + (canvasRight - canvasLeft) / 2;
            var maxLabel = 0;
            constantLines.forEach((function(item) {
                var isHorizontal = that._isHorizontal;
                var linesOptions = item.options;
                var paddingTopBottom = linesOptions.paddingTopBottom;
                var paddingLeftRight = linesOptions.paddingLeftRight;
                var labelOptions = linesOptions.label;
                var labelVerticalAlignment = labelOptions.verticalAlignment;
                var labelHorizontalAlignment = labelOptions.horizontalAlignment;
                var labelIsInside = "inside" === labelOptions.position;
                var label = item.label;
                var box = item.labelBBox;
                var translateX;
                var translateY;
                if (null === label || box.isEmpty) {
                    return
                }
                if (isHorizontal) {
                    if (labelIsInside) {
                        if (labelHorizontalAlignment === LEFT) {
                            translateX = item.coord - paddingLeftRight - box.x - box.width
                        } else {
                            translateX = item.coord + paddingLeftRight - box.x
                        }
                        switch (labelVerticalAlignment) {
                            case CENTER:
                                translateY = verticalCenter - box.y - box.height / 2;
                                break;
                            case BOTTOM:
                                translateY = canvasBottom - paddingTopBottom - box.y - box.height;
                                break;
                            default:
                                translateY = canvasTop + paddingTopBottom - box.y
                        }
                    } else {
                        if (axisPosition === labelVerticalAlignment) {
                            maxLabel = _max(maxLabel, box.height + paddingTopBottom)
                        }
                        translateX = item.coord - box.x - box.width / 2;
                        if (labelVerticalAlignment === BOTTOM) {
                            translateY = canvasBottom + paddingTopBottom - box.y
                        } else {
                            translateY = canvasTop - paddingTopBottom - box.y - box.height
                        }
                    }
                } else if (labelIsInside) {
                    if (labelVerticalAlignment === BOTTOM) {
                        translateY = item.coord + paddingTopBottom - box.y
                    } else {
                        translateY = item.coord - paddingTopBottom - box.y - box.height
                    }
                    switch (labelHorizontalAlignment) {
                        case CENTER:
                            translateX = horizontalCenter - box.x - box.width / 2;
                            break;
                        case RIGHT:
                            translateX = canvasRight - paddingLeftRight - box.x - box.width;
                            break;
                        default:
                            translateX = canvasLeft + paddingLeftRight - box.x
                    }
                } else {
                    if (axisPosition === labelHorizontalAlignment) {
                        maxLabel = _max(maxLabel, box.width + paddingLeftRight)
                    }
                    translateY = item.coord - box.y - box.height / 2;
                    if (labelHorizontalAlignment === RIGHT) {
                        translateX = canvasRight + paddingLeftRight - box.x
                    } else {
                        translateX = canvasLeft - paddingLeftRight - box.x - box.width
                    }
                }
                label.attr({
                    translateX: translateX,
                    translateY: translateY
                })
            }));
            return maxLabel
        },
        _drawConstantLinesForEstimating: function(constantLines) {
            var that = this;
            var renderer = this._renderer;
            var group = renderer.g();
            constantLines.forEach((function(options) {
                that._drawConstantLineLabelText(options.label.text, 0, 0, options.label, group).attr({
                    align: "center"
                })
            }));
            return group.append(renderer.root)
        },
        _estimateLabelHeight: function(bBox, labelOptions) {
            var height = bBox.height;
            var drawingType = labelOptions.drawingType;
            if ("stagger" === this._validateDisplayMode(drawingType) || "stagger" === this._validateOverlappingMode(labelOptions.overlappingBehavior, drawingType)) {
                height = 2 * height + labelOptions.staggeringSpacing
            }
            if ("rotate" === this._validateDisplayMode(drawingType) || "rotate" === this._validateOverlappingMode(labelOptions.overlappingBehavior, drawingType)) {
                var sinCos = getCosAndSin(labelOptions.rotationAngle);
                height = height * sinCos.cos + bBox.width * sinCos.sin
            }
            return height && (height + labelOptions.indentFromAxis || 0) || 0
        },
        estimateMargins: function(canvas) {
            this.updateCanvas(canvas);
            var range = this._getViewportRange();
            var ticksData = this._createTicksAndLabelFormat(range);
            var ticks = ticksData.ticks;
            var tickInterval = ticksData.tickInterval;
            var options = this._options;
            var constantLineOptions = this._outsideConstantLines.filter(l => l.labelOptions.visible).map(l => l.options);
            var rootElement = this._renderer.root;
            var labelIsVisible = options.label.visible && !range.isEmpty() && ticks.length;
            var labelValue = labelIsVisible && this.formatLabel(ticks[ticks.length - 1], options.label, void 0, void 0, tickInterval, ticks);
            var labelElement = labelIsVisible && this._renderer.text(labelValue, 0, 0).css(this._textFontStyles).attr(this._textOptions).append(rootElement);
            var titleElement = this._drawTitleText(rootElement, {
                x: 0,
                y: 0
            });
            var constantLinesLabelsElement = this._drawConstantLinesForEstimating(constantLineOptions);
            var labelBox = !options.label.template && labelElement && labelElement.getBBox() || {
                x: 0,
                y: 0,
                width: 0,
                height: 0
            };
            var titleBox = titleElement && titleElement.getBBox() || {
                x: 0,
                y: 0,
                width: 0,
                height: 0
            };
            var constantLinesBox = constantLinesLabelsElement.getBBox();
            var titleHeight = titleBox.height ? titleBox.height + options.title.margin : 0;
            var labelHeight = this._estimateLabelHeight(labelBox, options.label);
            var constantLinesHeight = constantLinesBox.height ? constantLinesBox.height + getMaxConstantLinePadding(constantLineOptions) : 0;
            var height = labelHeight + titleHeight;
            var margins = {
                left: _max(getLeftMargin(labelBox), getLeftMargin(constantLinesBox)),
                right: _max(getRightMargin(labelBox), getRightMargin(constantLinesBox)),
                top: ("top" === options.position ? height : 0) + getConstantLineLabelMarginForVerticalAlignment(constantLineOptions, "top", constantLinesHeight),
                bottom: ("top" !== options.position ? height : 0) + getConstantLineLabelMarginForVerticalAlignment(constantLineOptions, "bottom", constantLinesHeight)
            };
            labelElement && labelElement.remove();
            titleElement && titleElement.remove();
            constantLinesLabelsElement && constantLinesLabelsElement.remove();
            return margins
        },
        _checkAlignmentConstantLineLabels: function(labelOptions) {
            var position = labelOptions.position;
            var verticalAlignment = (labelOptions.verticalAlignment || "").toLowerCase();
            var horizontalAlignment = (labelOptions.horizontalAlignment || "").toLowerCase();
            if (this._isHorizontal) {
                if ("outside" === position) {
                    verticalAlignment = verticalAlignment === BOTTOM ? BOTTOM : TOP;
                    horizontalAlignment = CENTER
                } else {
                    verticalAlignment = verticalAlignment === CENTER ? CENTER : verticalAlignment === BOTTOM ? BOTTOM : TOP;
                    horizontalAlignment = horizontalAlignment === LEFT ? LEFT : RIGHT
                }
            } else if ("outside" === position) {
                verticalAlignment = CENTER;
                horizontalAlignment = horizontalAlignment === LEFT ? LEFT : RIGHT
            } else {
                verticalAlignment = verticalAlignment === BOTTOM ? BOTTOM : TOP;
                horizontalAlignment = horizontalAlignment === RIGHT ? RIGHT : horizontalAlignment === CENTER ? CENTER : LEFT
            }
            labelOptions.verticalAlignment = verticalAlignment;
            labelOptions.horizontalAlignment = horizontalAlignment
        },
        _getConstantLineLabelsCoords: function(value, lineLabelOptions) {
            var x = value;
            var y = value;
            if (this._isHorizontal) {
                y = this._orthogonalPositions["top" === lineLabelOptions.verticalAlignment ? "start" : "end"]
            } else {
                x = this._orthogonalPositions["right" === lineLabelOptions.horizontalAlignment ? "end" : "start"]
            }
            return {
                x: x,
                y: y
            }
        },
        _getAdjustedStripLabelCoords: function(strip) {
            var stripOptions = strip.options;
            var paddingTopBottom = stripOptions.paddingTopBottom;
            var paddingLeftRight = stripOptions.paddingLeftRight;
            var horizontalAlignment = stripOptions.label.horizontalAlignment;
            var verticalAlignment = stripOptions.label.verticalAlignment;
            var box = strip.labelBBox;
            var labelHeight = box.height;
            var labelWidth = box.width;
            var labelCoords = strip.labelCoords;
            var y = labelCoords.y - box.y;
            var x = labelCoords.x - box.x;
            if (verticalAlignment === TOP) {
                y += paddingTopBottom
            } else if (verticalAlignment === CENTER) {
                y -= labelHeight / 2
            } else if (verticalAlignment === BOTTOM) {
                y -= paddingTopBottom + labelHeight
            }
            if (horizontalAlignment === LEFT) {
                x += paddingLeftRight
            } else if (horizontalAlignment === CENTER) {
                x -= labelWidth / 2
            } else if (horizontalAlignment === RIGHT) {
                x -= paddingLeftRight + labelWidth
            }
            return {
                translateX: x,
                translateY: y
            }
        },
        _adjustTitle: function(offset) {
            offset = offset || 0;
            if (!this._title) {
                return
            }
            var options = this._options;
            var position = options.position;
            var margin = options.title.margin;
            var title = this._title;
            var boxTitle = title.bBox;
            var x = boxTitle.x;
            var y = boxTitle.y;
            var width = boxTitle.width;
            var height = boxTitle.height;
            var axisPosition = this._axisPosition;
            var loCoord = axisPosition - margin - offset;
            var hiCoord = axisPosition + margin + offset;
            var params = {};
            if (this._isHorizontal) {
                if (position === TOP) {
                    params.translateY = loCoord - (y + height)
                } else {
                    params.translateY = hiCoord - y
                }
            } else if (position === LEFT) {
                params.translateX = loCoord - (x + width)
            } else {
                params.translateX = hiCoord - x
            }
            title.element.attr(params)
        },
        _checkTitleOverflow: function(titleElement) {
            if (!this._title && !titleElement) {
                return
            }
            var canvasLength = this._getScreenDelta();
            var title = titleElement ? {
                bBox: titleElement.getBBox(),
                element: titleElement
            } : this._title;
            var titleOptions = this._options.title;
            var boxTitle = title.bBox;
            if ((this._isHorizontal ? boxTitle.width : boxTitle.height) > canvasLength) {
                title.element.setMaxSize(canvasLength, void 0, {
                    wordWrap: titleOptions.wordWrap || "none",
                    textOverflow: titleOptions.textOverflow || "ellipsis"
                });
                this._wrapped = titleOptions.wordWrap && "none" !== titleOptions.wordWrap
            } else {
                var moreThanOriginalSize = title.originalSize && canvasLength > (this._isHorizontal ? title.originalSize.width : title.originalSize.height);
                !this._wrapped && moreThanOriginalSize && title.element.restoreText()
            }
        },
        coordsIn: function(x, y) {
            var canvas = this.getCanvas();
            var isHorizontal = this._options.isHorizontal;
            var position = this._options.position;
            var coord = isHorizontal ? y : x;
            if (isHorizontal && (x < canvas.left || x > canvas.width - canvas.right) || !isHorizontal && (y < canvas.top || y > canvas.height - canvas.bottom)) {
                return false
            }
            if (isHorizontal && position === constants.top || !isHorizontal && position === constants.left) {
                return coord < canvas[position]
            }
            return coord > canvas[isHorizontal ? "height" : "width"] - canvas[position]
        },
        _boundaryTicksVisibility: {
            min: true,
            max: true
        },
        adjust() {
            var seriesData = this._seriesData;
            var viewport = this._series.filter(s => s.isVisible()).reduce((range, s) => {
                var seriesRange = s.getViewport();
                range.min = isDefined(seriesRange.min) ? range.min < seriesRange.min ? range.min : seriesRange.min : range.min;
                range.max = isDefined(seriesRange.max) ? range.max > seriesRange.max ? range.max : seriesRange.max : range.max;
                if (s.showZero) {
                    range = new Range(range);
                    range.correctValueZeroLevel()
                }
                return range
            }, {});
            if (isDefined(viewport.min) && isDefined(viewport.max)) {
                seriesData.minVisible = viewport.min;
                seriesData.maxVisible = viewport.max
            }
            seriesData.userBreaks = this._getScaleBreaks(this._options, {
                minVisible: seriesData.minVisible,
                maxVisible: seriesData.maxVisible
            }, this._series, this.isArgumentAxis);
            this._translator.updateBusinessRange(this._getViewportRange())
        },
        hasWrap() {
            return this._wrapped
        },
        getAxisPosition() {
            return this._axisPosition
        },
        _getStick: function() {
            return !this._options.valueMarginsEnabled
        },
        _getStripLabelCoords: function(from, to, stripLabelOptions) {
            var orthogonalPositions = this._orthogonalPositions;
            var isHorizontal = this._isHorizontal;
            var horizontalAlignment = stripLabelOptions.horizontalAlignment;
            var verticalAlignment = stripLabelOptions.verticalAlignment;
            var x;
            var y;
            if (isHorizontal) {
                if (horizontalAlignment === CENTER) {
                    x = from + (to - from) / 2
                } else if (horizontalAlignment === LEFT) {
                    x = from
                } else if (horizontalAlignment === RIGHT) {
                    x = to
                }
                y = orthogonalPositions[getStripVerticalAlignmentPosition(verticalAlignment)]
            } else {
                x = orthogonalPositions[getStripHorizontalAlignmentPosition(horizontalAlignment)];
                if (verticalAlignment === TOP) {
                    y = from
                } else if (verticalAlignment === CENTER) {
                    y = to + (from - to) / 2
                } else if (verticalAlignment === BOTTOM) {
                    y = to
                }
            }
            return {
                x: x,
                y: y
            }
        },
        _getTranslatedValue: function(value, offset) {
            var pos1 = this._translator.translate(value, offset, "semidiscrete" === this._options.type && this._options.tickInterval);
            var pos2 = this._axisPosition;
            var isHorizontal = this._isHorizontal;
            return {
                x: isHorizontal ? pos1 : pos2,
                y: isHorizontal ? pos2 : pos1
            }
        },
        areCoordsOutsideAxis: function(coords) {
            var coord = this._isHorizontal ? coords.x : coords.y;
            var visibleArea = this.getVisibleArea();
            if (coord < visibleArea[0] || coord > visibleArea[1]) {
                return true
            }
            return false
        },
        _getSkippedCategory: function(ticks) {
            var skippedCategory;
            if (this._options.type === constants.discrete && this._tickOffset && 0 !== ticks.length) {
                skippedCategory = ticks[ticks.length - 1]
            }
            return skippedCategory
        },
        _filterBreaks: function(breaks, viewport, breakStyle) {
            var minVisible = viewport.minVisible;
            var maxVisible = viewport.maxVisible;
            var breakSize = breakStyle ? breakStyle.width : 0;
            return breaks.reduce((function(result, currentBreak) {
                var from = currentBreak.from;
                var to = currentBreak.to;
                var lastResult = result[result.length - 1];
                var newBreak;
                if (!isDefined(from) || !isDefined(to)) {
                    return result
                }
                if (from > to) {
                    to = [from, from = to][0]
                }
                if (result.length && from < lastResult.to) {
                    if (to > lastResult.to) {
                        lastResult.to = to > maxVisible ? maxVisible : to;
                        if (lastResult.gapSize) {
                            lastResult.gapSize = void 0;
                            lastResult.cumulativeWidth += breakSize
                        }
                    }
                } else if (from >= minVisible && from < maxVisible || to <= maxVisible && to > minVisible) {
                    from = from >= minVisible ? from : minVisible;
                    to = to <= maxVisible ? to : maxVisible;
                    if (to - from < maxVisible - minVisible) {
                        var _lastResult$cumulativ;
                        newBreak = {
                            from: from,
                            to: to,
                            cumulativeWidth: (null !== (_lastResult$cumulativ = null === lastResult || void 0 === lastResult ? void 0 : lastResult.cumulativeWidth) && void 0 !== _lastResult$cumulativ ? _lastResult$cumulativ : 0) + breakSize
                        };
                        if (currentBreak.gapSize) {
                            var _lastResult$cumulativ2;
                            newBreak.gapSize = dateUtils.convertMillisecondsToDateUnits(to - from);
                            newBreak.cumulativeWidth = null !== (_lastResult$cumulativ2 = null === lastResult || void 0 === lastResult ? void 0 : lastResult.cumulativeWidth) && void 0 !== _lastResult$cumulativ2 ? _lastResult$cumulativ2 : 0
                        }
                        result.push(newBreak)
                    }
                }
                return result
            }), [])
        },
        _getScaleBreaks: function(axisOptions, viewport, series, isArgumentAxis) {
            var that = this;
            var breaks = (axisOptions.breaks || []).map((function(b) {
                return {
                    from: that.parser(b.startValue),
                    to: that.parser(b.endValue)
                }
            }));
            if ("discrete" !== axisOptions.type && "datetime" === axisOptions.dataType && axisOptions.workdaysOnly) {
                breaks = breaks.concat(generateDateBreaks(viewport.minVisible, viewport.maxVisible, axisOptions.workWeek, axisOptions.singleWorkdays, axisOptions.holidays))
            }
            if (!isArgumentAxis && "discrete" !== axisOptions.type && "datetime" !== axisOptions.dataType && axisOptions.autoBreaksEnabled && 0 !== axisOptions.maxAutoBreakCount) {
                breaks = breaks.concat(generateAutoBreaks(axisOptions, series, viewport))
            }
            return sortingBreaks(breaks)
        },
        _drawBreak: function(translatedEnd, positionFrom, positionTo, width, options, group) {
            var breakStart = translatedEnd - (!this._translator.isInverted() ? width + 1 : 0);
            var attr = {
                "stroke-width": 1,
                stroke: options.borderColor,
                sharp: !options.isWaved ? options.isHorizontal ? "h" : "v" : void 0
            };
            var spaceAttr = {
                stroke: options.color,
                "stroke-width": width
            };
            var getPoints = this._isHorizontal ? rotateLine : function(p) {
                return p
            };
            var drawer = getLineDrawer(this._renderer, group, getPoints, positionFrom, breakStart, positionTo, options.isWaved);
            drawer(width / 2, spaceAttr);
            drawer(0, attr);
            drawer(width, attr)
        },
        _createBreakClipRect: function(from, to) {
            var canvas = this._canvas;
            var clipWidth = to - from;
            var clipRect;
            if (this._isHorizontal) {
                clipRect = this._renderer.clipRect(canvas.left, from, canvas.width, clipWidth)
            } else {
                clipRect = this._renderer.clipRect(from, canvas.top, clipWidth, canvas.height)
            }
            this._breaksElements = this._breaksElements || [];
            this._breaksElements.push(clipRect);
            return clipRect.id
        },
        _createBreaksGroup: function(clipFrom, clipTo) {
            var group = this._renderer.g().attr({
                class: this._axisCssPrefix + "breaks",
                "clip-path": this._createBreakClipRect(clipFrom, clipTo)
            }).append(this._scaleBreaksGroup);
            this._breaksElements = this._breaksElements || [];
            this._breaksElements.push(group);
            return group
        },
        _disposeBreaksGroup: function() {
            (this._breaksElements || []).forEach((function(clipRect) {
                clipRect.dispose()
            }));
            this._breaksElements = null
        },
        drawScaleBreaks: function(customCanvas) {
            var that = this;
            var options = that._options;
            var breakStyle = options.breakStyle;
            var position = options.position;
            var positionFrom;
            var positionTo;
            var breaks = that._translator.getBusinessRange().breaks || [];
            var additionGroup;
            var additionBreakFrom;
            var additionBreakTo;
            that._disposeBreaksGroup();
            if (!(breaks && breaks.length)) {
                return
            }
            var breakOptions = {
                color: that._options.containerColor,
                borderColor: breakStyle.color,
                isHorizontal: that._isHorizontal,
                isWaved: "straight" !== breakStyle.line.toLowerCase()
            };
            if (customCanvas) {
                positionFrom = customCanvas.start;
                positionTo = customCanvas.end
            } else {
                positionFrom = that._orthogonalPositions.start - (options.visible && !that._axisShift && (position === LEFT || position === TOP) ? SCALE_BREAK_OFFSET : 0);
                positionTo = that._orthogonalPositions.end + (options.visible && (position === RIGHT || position === BOTTOM) ? SCALE_BREAK_OFFSET : 0)
            }
            var mainGroup = that._createBreaksGroup(positionFrom, positionTo);
            if (that._axisShift && options.visible) {
                additionBreakFrom = that._axisPosition - that._axisShift - SCALE_BREAK_OFFSET;
                additionBreakTo = additionBreakFrom + 2 * SCALE_BREAK_OFFSET;
                additionGroup = that._createBreaksGroup(additionBreakFrom, additionBreakTo)
            }
            breaks.forEach((function(br) {
                if (!br.gapSize) {
                    var breakCoord = that._getTranslatedCoord(br.to);
                    that._drawBreak(breakCoord, positionFrom, positionTo, breakStyle.width, breakOptions, mainGroup);
                    if (that._axisShift && options.visible) {
                        that._drawBreak(breakCoord, additionBreakFrom, additionBreakTo, breakStyle.width, breakOptions, additionGroup)
                    }
                }
            }))
        },
        _getSpiderCategoryOption: noop,
        shift: function(margins) {
            var options = this._options;
            var isHorizontal = options.isHorizontal;
            var axesSpacing = this.getMultipleAxesSpacing();
            var constantLinesGroups = this._axisConstantLineGroups;

            function shiftGroup(side, group) {
                var attr = {
                    translateX: 0,
                    translateY: 0
                };
                var shift = margins[side] ? margins[side] + axesSpacing : 0;
                attr[isHorizontal ? "translateY" : "translateX"] = (side === LEFT || side === TOP ? -1 : 1) * shift;
                (group[side] || group).attr(attr);
                return shift
            }
            this._axisShift = shiftGroup(options.position, this._axisGroup);
            shiftGroup(options.position, this._axisElementsGroup);
            (isHorizontal ? [TOP, BOTTOM] : [LEFT, RIGHT]).forEach(side => {
                shiftGroup(side, constantLinesGroups.above);
                shiftGroup(side, constantLinesGroups.under)
            })
        },
        getCustomPosition(position) {
            var orthogonalAxis = this.getOrthogonalAxis();
            var resolvedPosition = null !== position && void 0 !== position ? position : this.getResolvedPositionOption();
            var offset = this.getOptions().offset;
            var orthogonalTranslator = orthogonalAxis.getTranslator();
            var orthogonalAxisType = orthogonalAxis.getOptions().type;
            var validPosition = orthogonalAxis.validateUnit(resolvedPosition);
            var currentPosition;
            if ("discrete" === orthogonalAxisType && (!orthogonalTranslator._categories || orthogonalTranslator._categories.indexOf(validPosition) < 0)) {
                validPosition = void 0
            }
            if (this.positionIsBoundary(resolvedPosition)) {
                currentPosition = this.getPredefinedPosition(resolvedPosition)
            } else if (!isDefined(validPosition)) {
                currentPosition = this.getPredefinedPosition(this.getOptions().position)
            } else {
                currentPosition = orthogonalTranslator.to(validPosition, -1)
            }
            if (isFinite(currentPosition) && isFinite(offset)) {
                currentPosition += offset
            }
            return currentPosition
        },
        getCustomBoundaryPosition(position) {
            var {
                customPosition: customPosition,
                offset: offset
            } = this.getOptions();
            var resolvedPosition = null !== position && void 0 !== position ? position : this.getResolvedPositionOption();
            var orthogonalAxis = this.getOrthogonalAxis();
            var orthogonalTranslator = orthogonalAxis.getTranslator();
            var visibleArea = orthogonalTranslator.getCanvasVisibleArea();
            if (!isDefined(orthogonalAxis._orthogonalPositions) || 0 === orthogonalTranslator.canvasLength) {
                return
            }
            var currentPosition = this.getCustomPosition(resolvedPosition);
            if (!isDefined(currentPosition)) {
                return this.getResolvedBoundaryPosition()
            } else if (isDefined(customPosition)) {
                if (currentPosition <= visibleArea.min) {
                    return this._isHorizontal ? TOP : LEFT
                } else if (currentPosition >= visibleArea.max) {
                    return this._isHorizontal ? BOTTOM : RIGHT
                }
            } else if (isDefined(offset)) {
                if (currentPosition <= this._orthogonalPositions.start) {
                    return this._isHorizontal ? TOP : LEFT
                } else if (currentPosition >= this._orthogonalPositions.end) {
                    return this._isHorizontal ? BOTTOM : RIGHT
                }
            }
            return currentPosition
        },
        getResolvedPositionOption() {
            var _options$customPositi;
            var options = this.getOptions();
            return null !== (_options$customPositi = options.customPosition) && void 0 !== _options$customPositi ? _options$customPositi : options.position
        },
        customPositionIsAvailable() {
            var options = this.getOptions();
            return isDefined(this.getOrthogonalAxis()) && (isDefined(options.customPosition) || isFinite(options.offset))
        },
        hasNonBoundaryPosition() {
            return this.customPositionIsAvailable() && !this.customPositionIsBoundary()
        },
        getResolvedBoundaryPosition() {
            return this.customPositionIsBoundary() ? this._customBoundaryPosition : this.getOptions().position
        },
        customPositionEqualsToPredefined() {
            return this.customPositionIsBoundary() && this._customBoundaryPosition === this.getOptions().position
        },
        customPositionIsBoundary() {
            return this.positionIsBoundary(this._customBoundaryPosition)
        },
        positionIsBoundary: position => [TOP, LEFT, BOTTOM, RIGHT].indexOf(position) >= 0,
        getPredefinedPosition(position) {
            var _this$_orthogonalPosi;
            return null === (_this$_orthogonalPosi = this._orthogonalPositions) || void 0 === _this$_orthogonalPosi ? void 0 : _this$_orthogonalPosi[position === TOP || position === LEFT ? "start" : "end"]
        },
        resolveOverlappingForCustomPositioning(oppositeAxes) {
            var that = this;
            if (!that.hasNonBoundaryPosition() && !that.customPositionIsBoundary() && !oppositeAxes.some(a => a.hasNonBoundaryPosition())) {
                return
            }
            var overlappingObj = {
                axes: [],
                ticks: []
            };
            oppositeAxes.filter(orthogonalAxis => orthogonalAxis.pane === that.pane).forEach(orthogonalAxis => {
                for (var i = 0; i < that._majorTicks.length; i++) {
                    var tick = that._majorTicks[i];
                    var label = tick.label;
                    if (label) {
                        if (overlappingObj.axes.indexOf(orthogonalAxis) < 0 && that._detectElementsOverlapping(label, orthogonalAxis._axisElement)) {
                            overlappingObj.axes.push(orthogonalAxis);
                            that._shiftThroughOrthogonalAxisOverlappedTick(label, orthogonalAxis)
                        }
                        for (var j = 0; j < orthogonalAxis._majorTicks.length; j++) {
                            var oppositeTick = orthogonalAxis._majorTicks[j];
                            var oppositeLabel = oppositeTick.label;
                            if (oppositeLabel && that._detectElementsOverlapping(label, oppositeLabel)) {
                                overlappingObj.ticks.push(tick);
                                that._shiftThroughAxisOverlappedTick(tick);
                                i = that._majorTicks.length;
                                break
                            }
                        }
                    }
                    if (tick.mark && overlappingObj.ticks.indexOf(tick) < 0) {
                        if (that._isHorizontal && tick.mark.attr("translateY")) {
                            tick.mark.attr({
                                translateY: 0
                            })
                        } else if (!that._isHorizontal && tick.mark.attr("translateX")) {
                            tick.mark.attr({
                                translateX: 0
                            })
                        }
                    }
                }
            })
        },
        _shiftThroughOrthogonalAxisOverlappedTick(label, orthogonalAxis) {
            var labelBBox = label.getBBox();
            var orthogonalAxisPosition = orthogonalAxis.getAxisPosition();
            var orthogonalAxisLabelOptions = orthogonalAxis.getOptions().label;
            var orthogonalAxisLabelPosition = orthogonalAxisLabelOptions.position;
            var orthogonalAxisLabelIndent = orthogonalAxisLabelOptions.indentFromAxis / 2;
            var translateCoordName = this._isHorizontal ? "translateX" : "translateY";
            var defaultOrthogonalAxisLabelPosition = this._isHorizontal ? LEFT : TOP;
            var translate = label.attr(translateCoordName);
            var labelCoord = (this._isHorizontal ? labelBBox.x : labelBBox.y) + translate;
            var labelSize = this._isHorizontal ? labelBBox.width : labelBBox.height;
            var outsidePart = orthogonalAxisPosition - labelCoord;
            var insidePart = labelCoord + labelSize - orthogonalAxisPosition;
            var attr = {};
            attr[translateCoordName] = translate;
            if (outsidePart > 0 && insidePart > 0) {
                if (insidePart - outsidePart > 1) {
                    attr[translateCoordName] += outsidePart + orthogonalAxisLabelIndent
                } else if (outsidePart - insidePart > 1) {
                    attr[translateCoordName] -= insidePart + orthogonalAxisLabelIndent
                } else {
                    attr[translateCoordName] += orthogonalAxisLabelPosition === defaultOrthogonalAxisLabelPosition ? outsidePart + orthogonalAxisLabelIndent : -(insidePart + orthogonalAxisLabelIndent)
                }
                label.attr(attr)
            }
        },
        _shiftThroughAxisOverlappedTick(tick) {
            var _tick$mark;
            var label = tick.label;
            if (!label) {
                return
            }
            var labelBBox = label.getBBox();
            var tickMarkBBox = null === (_tick$mark = tick.mark) || void 0 === _tick$mark ? void 0 : _tick$mark.getBBox();
            var axisPosition = this.getAxisPosition();
            var labelOptions = this.getOptions().label;
            var labelIndent = labelOptions.indentFromAxis;
            var labelPosition = labelOptions.position;
            var defaultLabelPosition = this._isHorizontal ? TOP : LEFT;
            var translateCoordName = this._isHorizontal ? "translateY" : "translateX";
            var translate = label.attr(translateCoordName);
            var labelCoord = (this._isHorizontal ? labelBBox.y : labelBBox.x) + translate;
            var labelSize = this._isHorizontal ? labelBBox.height : labelBBox.width;
            var attr = {};
            attr[translateCoordName] = translate + (labelPosition === defaultLabelPosition ? axisPosition - labelCoord + labelIndent : -(labelCoord - axisPosition + labelSize + labelIndent));
            label.attr(attr);
            if (tick.mark) {
                var markerSize = this._isHorizontal ? tickMarkBBox.height : tickMarkBBox.width;
                var dir = labelPosition === defaultLabelPosition ? 1 : -1;
                attr[translateCoordName] = dir * (markerSize - 1);
                tick.mark.attr(attr)
            }
        },
        _detectElementsOverlapping(element1, element2) {
            if (!element1 || !element2) {
                return false
            }
            var bBox1 = element1.getBBox();
            var x1 = bBox1.x + element1.attr("translateX");
            var y1 = bBox1.y + element1.attr("translateY");
            var bBox2 = element2.getBBox();
            var x2 = bBox2.x + element2.attr("translateX");
            var y2 = bBox2.y + element2.attr("translateY");
            return (x2 >= x1 && x2 <= x1 + bBox1.width || x1 >= x2 && x1 <= x2 + bBox2.width) && (y2 >= y1 && y2 <= y1 + bBox1.height || y1 >= y2 && y1 <= y2 + bBox2.height)
        }
    }
};

function getLineDrawer(renderer, root, rotatePoints, positionFrom, breakStart, positionTo, isWaved) {
    var elementType = isWaved ? "bezier" : "line";
    var group = renderer.g().append(root);
    return function(offset, attr) {
        renderer.path(rotatePoints(getPoints(positionFrom, breakStart, positionTo, offset, isWaved)), elementType).attr(attr).append(group)
    }
}

function getPoints(positionFrom, breakStart, positionTo, offset, isWaved) {
    if (!isWaved) {
        return [positionFrom, breakStart + offset, positionTo, breakStart + offset]
    }
    breakStart += offset;
    var currentPosition;
    var topPoint = breakStart + WAVED_LINE_TOP;
    var centerPoint = breakStart + WAVED_LINE_CENTER;
    var bottomPoint = breakStart + WAVED_LINE_BOTTOM;
    var points = [
        [positionFrom, centerPoint]
    ];
    for (currentPosition = positionFrom; currentPosition < positionTo + WAVED_LINE_LENGTH; currentPosition += WAVED_LINE_LENGTH) {
        points.push([currentPosition + 6, topPoint, currentPosition + 6, topPoint, currentPosition + 12, centerPoint, currentPosition + 18, bottomPoint, currentPosition + 18, bottomPoint, currentPosition + 24, centerPoint])
    }
    return [].concat.apply([], points)
}

function rotateLine(lineCoords) {
    var points = [];
    var i;
    for (i = 0; i < lineCoords.length; i += 2) {
        points.push(lineCoords[i + 1]);
        points.push(lineCoords[i])
    }
    return points
}
