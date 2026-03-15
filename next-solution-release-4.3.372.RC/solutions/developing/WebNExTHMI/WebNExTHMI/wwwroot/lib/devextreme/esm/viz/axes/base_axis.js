/**
 * DevExtreme (esm/viz/axes/base_axis.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    smartFormatter as _format,
    formatRange
} from "./smart_formatter";
import {
    patchFontOptions,
    getVizRangeObject,
    getLogExt as getLog,
    raiseToExt as raiseTo,
    valueOf,
    rotateBBox,
    getCategoriesInfo,
    adjustVisualRange,
    getAddFunction,
    convertVisualRangeObject
} from "../core/utils";
import {
    isDefined,
    isFunction,
    isPlainObject,
    type
} from "../../core/utils/type";
import constants from "./axes_constants";
import {
    extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import formatHelper from "../../format_helper";
import {
    getParser
} from "../components/parse_utils";
import {
    tickGenerator
} from "./tick_generator";
import {
    Translator2D
} from "../translators/translator2d";
import {
    Range
} from "../translators/range";
import {
    tick
} from "./tick";
import {
    adjust
} from "../../core/utils/math";
import dateUtils from "../../core/utils/date";
import {
    noop as _noop
} from "../../core/utils/common";
import xyMethods from "./xy_axes";
import * as polarMethods from "./polar_axes";
import createConstantLine from "./constant_line";
import createStrip from "./strip";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
import {
    calculateCanvasMargins,
    measureLabels
} from "./axes_utils";
var convertTicksToValues = constants.convertTicksToValues;
var _math = Math;
var _abs = _math.abs;
var _max = _math.max;
var _min = _math.min;
var _isArray = Array.isArray;
var DEFAULT_AXIS_LABEL_SPACING = 5;
var MAX_GRID_BORDER_ADHENSION = 4;
var TOP = constants.top;
var BOTTOM = constants.bottom;
var LEFT = constants.left;
var RIGHT = constants.right;
var CENTER = constants.center;
var KEEP = "keep";
var SHIFT = "shift";
var RESET = "reset";
var ROTATE = "rotate";
var DEFAULT_AXIS_DIVISION_FACTOR = 50;
var DEFAULT_MINOR_AXIS_DIVISION_FACTOR = 15;
var SCROLL_THRESHOLD = 5;
var MIN_BAR_MARGIN = 5;
var MAX_MARGIN_VALUE = .8;
var dateIntervals = {
    day: 864e5,
    week: 6048e5
};

function getTickGenerator(options, incidentOccurred, skipTickGeneration, rangeIsEmpty, adjustDivisionFactor, _ref) {
    var _options$workWeek;
    var {
        allowNegatives: allowNegatives,
        linearThreshold: linearThreshold
    } = _ref;
    return tickGenerator({
        axisType: options.type,
        dataType: options.dataType,
        logBase: options.logarithmBase,
        allowNegatives: allowNegatives,
        linearThreshold: linearThreshold,
        axisDivisionFactor: adjustDivisionFactor(options.axisDivisionFactor || DEFAULT_AXIS_DIVISION_FACTOR),
        minorAxisDivisionFactor: adjustDivisionFactor(options.minorAxisDivisionFactor || DEFAULT_MINOR_AXIS_DIVISION_FACTOR),
        numberMultipliers: options.numberMultipliers,
        calculateMinors: options.minorTick.visible || options.minorGrid.visible || options.calculateMinors,
        allowDecimals: options.allowDecimals,
        endOnTick: options.endOnTick,
        incidentOccurred: incidentOccurred,
        firstDayOfWeek: null === (_options$workWeek = options.workWeek) || void 0 === _options$workWeek ? void 0 : _options$workWeek[0],
        skipTickGeneration: skipTickGeneration,
        skipCalculationLimits: options.skipCalculationLimits,
        generateExtraTick: options.generateExtraTick,
        minTickInterval: options.minTickInterval,
        rangeIsEmpty: rangeIsEmpty
    })
}

function createMajorTick(axis, renderer, skippedCategory) {
    var options = axis.getOptions();
    return tick(axis, renderer, options.tick, options.grid, skippedCategory, false)
}

function createMinorTick(axis, renderer) {
    var options = axis.getOptions();
    return tick(axis, renderer, options.minorTick, options.minorGrid)
}

function createBoundaryTick(axis, renderer, isFirst) {
    var options = axis.getOptions();
    return tick(axis, renderer, extend({}, options.tick, {
        visible: options.showCustomBoundaryTicks
    }), options.grid, void 0, false, isFirst ? -1 : 1)
}

function callAction(elements, action, actionArgument1, actionArgument2) {
    (elements || []).forEach(e => e[action](actionArgument1, actionArgument2))
}

function initTickCoords(ticks) {
    callAction(ticks, "initCoords")
}

function drawTickMarks(ticks, options) {
    callAction(ticks, "drawMark", options)
}

function drawGrids(ticks, drawLine) {
    callAction(ticks, "drawGrid", drawLine)
}

function updateTicksPosition(ticks, options, animate) {
    callAction(ticks, "updateTickPosition", options, animate)
}

function updateGridsPosition(ticks, animate) {
    callAction(ticks, "updateGridPosition", animate)
}

function cleanUpInvalidTicks(ticks) {
    var i = ticks.length - 1;
    for (i; i >= 0; i--) {
        if (!removeInvalidTick(ticks, i)) {
            break
        }
    }
    for (i = 0; i < ticks.length; i++) {
        if (removeInvalidTick(ticks, i)) {
            i--
        } else {
            break
        }
    }
}

function removeInvalidTick(ticks, i) {
    if (null === ticks[i].coords.x || null === ticks[i].coords.y) {
        ticks.splice(i, 1);
        return true
    }
    return false
}

function validateAxisOptions(options) {
    var _labelOptions$minSpac;
    var labelOptions = options.label;
    var position = options.position;
    var defaultPosition = options.isHorizontal ? BOTTOM : LEFT;
    var secondaryPosition = options.isHorizontal ? TOP : RIGHT;
    var labelPosition = labelOptions.position;
    if (position !== defaultPosition && position !== secondaryPosition) {
        position = defaultPosition
    }
    if (!labelPosition || "outside" === labelPosition) {
        labelPosition = position
    } else if ("inside" === labelPosition) {
        labelPosition = {
            [TOP]: BOTTOM,
            [BOTTOM]: TOP,
            [LEFT]: RIGHT,
            [RIGHT]: LEFT
        } [position]
    }
    if (labelPosition !== defaultPosition && labelPosition !== secondaryPosition) {
        labelPosition = position
    }
    if (labelOptions.alignment !== CENTER && !labelOptions.userAlignment) {
        labelOptions.alignment = {
            [TOP]: CENTER,
            [BOTTOM]: CENTER,
            [LEFT]: RIGHT,
            [RIGHT]: LEFT
        } [labelPosition]
    }
    options.position = position;
    labelOptions.position = labelPosition;
    options.hoverMode = options.hoverMode ? options.hoverMode.toLowerCase() : "none";
    labelOptions.minSpacing = null !== (_labelOptions$minSpac = labelOptions.minSpacing) && void 0 !== _labelOptions$minSpac ? _labelOptions$minSpac : DEFAULT_AXIS_LABEL_SPACING;
    options.type && (options.type = options.type.toLowerCase());
    options.argumentType && (options.argumentType = options.argumentType.toLowerCase());
    options.valueType && (options.valueType = options.valueType.toLowerCase())
}

function getOptimalAngle(boxes, labelOpt) {
    var angle = 180 * _math.asin((boxes[0].height + labelOpt.minSpacing) / (boxes[1].x - boxes[0].x)) / _math.PI;
    return angle < 45 ? -45 : -90
}

function updateLabels(ticks, step, func) {
    ticks.forEach((function(tick, index) {
        if (tick.getContentContainer()) {
            if (index % step !== 0) {
                tick.removeLabel()
            } else if (func) {
                func(tick, index)
            }
        }
    }))
}

function getZoomBoundValue(optionValue, dataValue) {
    if (void 0 === optionValue) {
        return dataValue
    } else if (null === optionValue) {
        return
    } else {
        return optionValue
    }
}

function configureGenerator(options, axisDivisionFactor, viewPort, screenDelta, minTickInterval) {
    var tickGeneratorOptions = extend({}, options, {
        endOnTick: true,
        axisDivisionFactor: axisDivisionFactor,
        skipCalculationLimits: true,
        generateExtraTick: true,
        minTickInterval: minTickInterval
    });
    return function(tickInterval, skipTickGeneration, min, max, breaks) {
        return getTickGenerator(tickGeneratorOptions, _noop, skipTickGeneration, viewPort.isEmpty(), v => v, viewPort)({
            min: min,
            max: max,
            categories: viewPort.categories,
            isSpacedMargin: viewPort.isSpacedMargin
        }, screenDelta, tickInterval, isDefined(tickInterval), void 0, void 0, void 0, breaks)
    }
}

function getConstantLineSharpDirection(coord, axisCanvas) {
    return Math.max(axisCanvas.start, axisCanvas.end) !== coord ? 1 : -1
}
export var Axis = function(renderSettings) {
    this._renderer = renderSettings.renderer;
    this._incidentOccurred = renderSettings.incidentOccurred;
    this._eventTrigger = renderSettings.eventTrigger;
    this._stripsGroup = renderSettings.stripsGroup;
    this._stripLabelAxesGroup = renderSettings.stripLabelAxesGroup;
    this._labelsAxesGroup = renderSettings.labelsAxesGroup;
    this._constantLinesGroup = renderSettings.constantLinesGroup;
    this._scaleBreaksGroup = renderSettings.scaleBreaksGroup;
    this._axesContainerGroup = renderSettings.axesContainerGroup;
    this._gridContainerGroup = renderSettings.gridGroup;
    this._axisCssPrefix = renderSettings.widgetClass + "-" + (renderSettings.axisClass ? renderSettings.axisClass + "-" : "");
    this._setType(renderSettings.axisType, renderSettings.drawingType);
    this._createAxisGroups();
    this._translator = this._createTranslator();
    this.isArgumentAxis = renderSettings.isArgumentAxis;
    this._viewport = {};
    this._prevDataInfo = {};
    this._firstDrawing = true;
    this._initRange = {};
    this._getTemplate = renderSettings.getTemplate
};
Axis.prototype = {
    constructor: Axis,
    _drawAxis() {
        var options = this._options;
        if (!options.visible) {
            return
        }
        this._axisElement = this._createAxisElement();
        this._updateAxisElementPosition();
        this._axisElement.attr({
            "stroke-width": options.width,
            stroke: options.color,
            "stroke-opacity": options.opacity
        }).sharp(this._getSharpParam(true), this.getAxisSharpDirection()).append(this._axisLineGroup)
    },
    _createPathElement(points, attr, sharpDirection) {
        return this.sharp(this._renderer.path(points, "line").attr(attr), sharpDirection)
    },
    sharp(svgElement) {
        var sharpDirection = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 1;
        return svgElement.sharp(this._getSharpParam(), sharpDirection)
    },
    customPositionIsAvailable: () => false,
    getOrthogonalAxis: _noop,
    getCustomPosition: _noop,
    getCustomBoundaryPosition: _noop,
    resolveOverlappingForCustomPositioning: _noop,
    hasNonBoundaryPosition: () => false,
    customPositionIsBoundaryOrthogonalAxis: () => false,
    getResolvedBoundaryPosition() {
        return this.getOptions().position
    },
    getAxisSharpDirection() {
        var position = this.getResolvedBoundaryPosition();
        return this.hasNonBoundaryPosition() || position !== BOTTOM && position !== RIGHT ? 1 : -1
    },
    getSharpDirectionByCoords(coords) {
        var canvas = this._getCanvasStartEnd();
        var maxCoord = Math.max(canvas.start, canvas.end);
        return this.getRadius ? 0 : maxCoord !== coords[this._isHorizontal ? "x" : "y"] ? 1 : -1
    },
    _getGridLineDrawer: function() {
        var that = this;
        return function(tick, gridStyle) {
            var grid = that._getGridPoints(tick.coords);
            if (grid.points) {
                return that._createPathElement(grid.points, gridStyle, that.getSharpDirectionByCoords(tick.coords))
            }
            return null
        }
    },
    _getGridPoints: function(coords) {
        var isHorizontal = this._isHorizontal;
        var tickPositionField = isHorizontal ? "x" : "y";
        var orthogonalPositions = this._orthogonalPositions;
        var positionFrom = orthogonalPositions.start;
        var positionTo = orthogonalPositions.end;
        var borderOptions = this.borderOptions;
        var canvasStart = isHorizontal ? LEFT : TOP;
        var canvasEnd = isHorizontal ? RIGHT : BOTTOM;
        var axisCanvas = this.getCanvas();
        var canvas = {
            left: axisCanvas.left,
            right: axisCanvas.width - axisCanvas.right,
            top: axisCanvas.top,
            bottom: axisCanvas.height - axisCanvas.bottom
        };
        var firstBorderLinePosition = borderOptions.visible && borderOptions[canvasStart] ? canvas[canvasStart] : void 0;
        var lastBorderLinePosition = borderOptions.visible && borderOptions[canvasEnd] ? canvas[canvasEnd] : void 0;
        var minDelta = MAX_GRID_BORDER_ADHENSION + firstBorderLinePosition;
        var maxDelta = lastBorderLinePosition - MAX_GRID_BORDER_ADHENSION;
        if (this.areCoordsOutsideAxis(coords) || void 0 === coords[tickPositionField] || coords[tickPositionField] < minDelta || coords[tickPositionField] > maxDelta) {
            return {
                points: null
            }
        }
        return {
            points: isHorizontal ? null !== coords[tickPositionField] ? [coords[tickPositionField], positionFrom, coords[tickPositionField], positionTo] : null : null !== coords[tickPositionField] ? [positionFrom, coords[tickPositionField], positionTo, coords[tickPositionField]] : null
        }
    },
    _getConstantLinePos: function(parsedValue, canvasStart, canvasEnd) {
        var value = this._getTranslatedCoord(parsedValue);
        if (!isDefined(value) || value < _min(canvasStart, canvasEnd) || value > _max(canvasStart, canvasEnd)) {
            return
        }
        return value
    },
    _getConstantLineGraphicAttributes: function(value) {
        var positionFrom = this._orthogonalPositions.start;
        var positionTo = this._orthogonalPositions.end;
        return {
            points: this._isHorizontal ? [value, positionFrom, value, positionTo] : [positionFrom, value, positionTo, value]
        }
    },
    _createConstantLine: function(value, attr) {
        return this._createPathElement(this._getConstantLineGraphicAttributes(value).points, attr, getConstantLineSharpDirection(value, this._getCanvasStartEnd()))
    },
    _drawConstantLineLabelText: function(text, x, y, _ref2, group) {
        var {
            font: font,
            cssClass: cssClass
        } = _ref2;
        return this._renderer.text(text, x, y).css(patchFontOptions(extend({}, this._options.label.font, font))).attr({
            align: "center",
            class: cssClass
        }).append(group)
    },
    _drawConstantLineLabels: function(parsedValue, lineLabelOptions, value, group) {
        var _text;
        var text = lineLabelOptions.text;
        var options = this._options;
        var labelOptions = options.label;
        this._checkAlignmentConstantLineLabels(lineLabelOptions);
        text = null !== (_text = text) && void 0 !== _text ? _text : this.formatLabel(parsedValue, labelOptions);
        var coords = this._getConstantLineLabelsCoords(value, lineLabelOptions);
        return this._drawConstantLineLabelText(text, coords.x, coords.y, lineLabelOptions, group)
    },
    _getStripPos: function(startValue, endValue, canvasStart, canvasEnd, range) {
        var isContinuous = !!(range.minVisible || range.maxVisible);
        var categories = (range.categories || []).reduce((function(result, cat) {
            result.push(cat.valueOf());
            return result
        }), []);
        var start;
        var end;
        var swap;
        var startCategoryIndex;
        var endCategoryIndex;
        if (!isContinuous) {
            if (isDefined(startValue) && isDefined(endValue)) {
                var parsedStartValue = this.parser(startValue);
                var parsedEndValue = this.parser(endValue);
                startCategoryIndex = inArray(isDefined(parsedStartValue) ? parsedStartValue.valueOf() : void 0, categories);
                endCategoryIndex = inArray(isDefined(parsedEndValue) ? parsedEndValue.valueOf() : void 0, categories);
                if (-1 === startCategoryIndex || -1 === endCategoryIndex) {
                    return {
                        from: 0,
                        to: 0,
                        outOfCanvas: true
                    }
                }
                if (startCategoryIndex > endCategoryIndex) {
                    swap = endValue;
                    endValue = startValue;
                    startValue = swap
                }
            }
        }
        if (isDefined(startValue)) {
            startValue = this.validateUnit(startValue, "E2105", "strip");
            start = this._getTranslatedCoord(startValue, -1)
        } else {
            start = canvasStart
        }
        if (isDefined(endValue)) {
            endValue = this.validateUnit(endValue, "E2105", "strip");
            end = this._getTranslatedCoord(endValue, 1)
        } else {
            end = canvasEnd
        }
        var stripPosition = start < end ? {
            from: start,
            to: end
        } : {
            from: end,
            to: start
        };
        var visibleArea = this.getVisibleArea();
        if (stripPosition.from <= visibleArea[0] && stripPosition.to <= visibleArea[0] || stripPosition.from >= visibleArea[1] && stripPosition.to >= visibleArea[1]) {
            stripPosition.outOfCanvas = true
        }
        return stripPosition
    },
    _getStripGraphicAttributes: function(fromPoint, toPoint) {
        var x;
        var y;
        var width;
        var height;
        var orthogonalPositions = this._orthogonalPositions;
        var positionFrom = orthogonalPositions.start;
        var positionTo = orthogonalPositions.end;
        if (this._isHorizontal) {
            x = fromPoint;
            y = _min(positionFrom, positionTo);
            width = toPoint - fromPoint;
            height = _abs(positionFrom - positionTo)
        } else {
            x = _min(positionFrom, positionTo);
            y = fromPoint;
            width = _abs(positionFrom - positionTo);
            height = _abs(fromPoint - toPoint)
        }
        return {
            x: x,
            y: y,
            width: width,
            height: height
        }
    },
    _createStrip: function(attrs) {
        return this._renderer.rect(attrs.x, attrs.y, attrs.width, attrs.height)
    },
    _adjustStripLabels: function() {
        var that = this;
        this._strips.forEach((function(strip) {
            if (strip.label) {
                strip.label.attr(that._getAdjustedStripLabelCoords(strip))
            }
        }))
    },
    _adjustLabelsCoord(offset, maxWidth, checkCanvas) {
        var getContainerAttrs = tick => this._getLabelAdjustedCoord(tick, offset + (tick.labelOffset || 0), maxWidth, checkCanvas);
        this._majorTicks.forEach((function(tick) {
            if (tick.label) {
                tick.updateMultilineTextAlignment();
                tick.label.attr(getContainerAttrs(tick))
            } else {
                tick.templateContainer && tick.templateContainer.attr(getContainerAttrs(tick))
            }
        }))
    },
    _adjustLabels: function(offset) {
        var options = this.getOptions();
        var positionsAreConsistent = options.position === options.label.position;
        var maxSize = this._majorTicks.reduce((function(size, tick) {
            if (!tick.getContentContainer()) {
                return size
            }
            var bBox = tick.labelRotationAngle ? rotateBBox(tick.labelBBox, [tick.labelCoords.x, tick.labelCoords.y], -tick.labelRotationAngle) : tick.labelBBox;
            return {
                width: _max(size.width || 0, bBox.width),
                height: _max(size.height || 0, bBox.height),
                offset: _max(size.offset || 0, tick.labelOffset || 0)
            }
        }), {});
        var additionalOffset = positionsAreConsistent ? this._isHorizontal ? maxSize.height : maxSize.width : 0;
        this._adjustLabelsCoord(offset, maxSize.width);
        return offset + additionalOffset + (additionalOffset && this._options.label.indentFromAxis) + (positionsAreConsistent ? maxSize.offset : 0)
    },
    _getLabelAdjustedCoord: function(tick, offset, maxWidth) {
        offset = offset || 0;
        var options = this._options;
        var templateBox = tick.templateContainer && tick.templateContainer.getBBox();
        var box = templateBox || rotateBBox(tick.labelBBox, [tick.labelCoords.x, tick.labelCoords.y], -tick.labelRotationAngle || 0);
        var textAlign = tick.labelAlignment || options.label.alignment;
        var isDiscrete = "discrete" === this._options.type;
        var isFlatLabel = tick.labelRotationAngle % 90 === 0;
        var indentFromAxis = options.label.indentFromAxis;
        var labelPosition = options.label.position;
        var axisPosition = this._axisPosition;
        var labelCoords = tick.labelCoords;
        var labelX = labelCoords.x;
        var translateX;
        var translateY;
        if (this._isHorizontal) {
            if (labelPosition === BOTTOM) {
                translateY = axisPosition + indentFromAxis - box.y + offset
            } else {
                translateY = axisPosition - indentFromAxis - (box.y + box.height) - offset
            }
            if (textAlign === RIGHT) {
                translateX = isDiscrete && isFlatLabel ? tick.coords.x - (box.x + box.width) : labelX - box.x - box.width
            } else if (textAlign === LEFT) {
                translateX = isDiscrete && isFlatLabel ? labelX - box.x - (tick.coords.x - labelX) : labelX - box.x
            } else {
                translateX = labelX - box.x - box.width / 2
            }
        } else {
            translateY = labelCoords.y - box.y - box.height / 2;
            if (labelPosition === LEFT) {
                if (textAlign === LEFT) {
                    translateX = axisPosition - indentFromAxis - maxWidth - box.x
                } else if (textAlign === CENTER) {
                    translateX = axisPosition - indentFromAxis - maxWidth / 2 - box.x - box.width / 2
                } else {
                    translateX = axisPosition - indentFromAxis - box.x - box.width
                }
                translateX -= offset
            } else {
                if (textAlign === RIGHT) {
                    translateX = axisPosition + indentFromAxis + maxWidth - box.x - box.width
                } else if (textAlign === CENTER) {
                    translateX = axisPosition + indentFromAxis + maxWidth / 2 - box.x - box.width / 2
                } else {
                    translateX = axisPosition + indentFromAxis - box.x
                }
                translateX += offset
            }
        }
        return {
            translateX: translateX,
            translateY: translateY
        }
    },
    _createAxisConstantLineGroups: function() {
        var renderer = this._renderer;
        var classSelector = this._axisCssPrefix;
        var constantLinesClass = classSelector + "constant-lines";
        var insideGroup = renderer.g().attr({
            class: constantLinesClass
        });
        var outsideGroup1 = renderer.g().attr({
            class: constantLinesClass
        });
        var outsideGroup2 = renderer.g().attr({
            class: constantLinesClass
        });
        return {
            inside: insideGroup,
            outside1: outsideGroup1,
            left: outsideGroup1,
            top: outsideGroup1,
            outside2: outsideGroup2,
            right: outsideGroup2,
            bottom: outsideGroup2,
            remove: function() {
                this.inside.remove();
                this.outside1.remove();
                this.outside2.remove()
            },
            clear: function() {
                this.inside.clear();
                this.outside1.clear();
                this.outside2.clear()
            }
        }
    },
    _createAxisGroups: function() {
        var renderer = this._renderer;
        var classSelector = this._axisCssPrefix;
        this._axisGroup = renderer.g().attr({
            class: classSelector + "axis"
        }).enableLinks();
        this._axisStripGroup = renderer.g().attr({
            class: classSelector + "strips"
        });
        this._axisGridGroup = renderer.g().attr({
            class: classSelector + "grid"
        });
        this._axisElementsGroup = renderer.g().attr({
            class: classSelector + "elements"
        });
        this._axisLineGroup = renderer.g().attr({
            class: classSelector + "line"
        }).linkOn(this._axisGroup, "axisLine").linkAppend();
        this._axisTitleGroup = renderer.g().attr({
            class: classSelector + "title"
        }).append(this._axisGroup);
        this._axisConstantLineGroups = {
            above: this._createAxisConstantLineGroups(),
            under: this._createAxisConstantLineGroups()
        };
        this._axisStripLabelGroup = renderer.g().attr({
            class: classSelector + "axis-labels"
        })
    },
    _clearAxisGroups: function() {
        this._axisGroup.remove();
        this._axisStripGroup.remove();
        this._axisStripLabelGroup.remove();
        this._axisConstantLineGroups.above.remove();
        this._axisConstantLineGroups.under.remove();
        this._axisGridGroup.remove();
        this._axisTitleGroup.clear();
        if (!this._options.label.template || !this.isRendered()) {
            this._axisElementsGroup.remove();
            this._axisElementsGroup.clear()
        }
        this._axisLineGroup && this._axisLineGroup.clear();
        this._axisStripGroup && this._axisStripGroup.clear();
        this._axisGridGroup && this._axisGridGroup.clear();
        this._axisConstantLineGroups.above.clear();
        this._axisConstantLineGroups.under.clear();
        this._axisStripLabelGroup && this._axisStripLabelGroup.clear()
    },
    _getLabelFormatObject: function(value, labelOptions, range, point, tickInterval, ticks) {
        range = range || this._getViewportRange();
        var formatObject = {
            value: value,
            valueText: _format(value, {
                labelOptions: labelOptions,
                ticks: ticks || convertTicksToValues(this._majorTicks),
                tickInterval: null !== tickInterval && void 0 !== tickInterval ? tickInterval : this._tickInterval,
                dataType: this._options.dataType,
                logarithmBase: this._options.logarithmBase,
                type: this._options.type,
                showTransition: !this._options.marker.visible,
                point: point
            }) || "",
            min: range.minVisible,
            max: range.maxVisible
        };
        if (point) {
            formatObject.point = point
        }
        return formatObject
    },
    formatLabel: function(value, labelOptions, range, point, tickInterval, ticks) {
        var formatObject = this._getLabelFormatObject(value, labelOptions, range, point, tickInterval, ticks);
        return isFunction(labelOptions.customizeText) ? labelOptions.customizeText.call(formatObject, formatObject) : formatObject.valueText
    },
    formatHint: function(value, labelOptions, range) {
        var formatObject = this._getLabelFormatObject(value, labelOptions, range);
        return isFunction(labelOptions.customizeHint) ? labelOptions.customizeHint.call(formatObject, formatObject) : void 0
    },
    formatRange(startValue, endValue, interval, argumentFormat) {
        return formatRange({
            startValue: startValue,
            endValue: endValue,
            tickInterval: interval,
            argumentFormat: argumentFormat,
            axisOptions: this.getOptions()
        })
    },
    _setTickOffset: function() {
        var options = this._options;
        var discreteAxisDivisionMode = options.discreteAxisDivisionMode;
        this._tickOffset = +("crossLabels" !== discreteAxisDivisionMode || !discreteAxisDivisionMode)
    },
    resetApplyingAnimation: function(isFirstDrawing) {
        this._resetApplyingAnimation = true;
        if (isFirstDrawing) {
            this._firstDrawing = true
        }
    },
    isFirstDrawing() {
        return this._firstDrawing
    },
    getMargins: function() {
        var that = this;
        var {
            position: position,
            offset: offset,
            customPosition: customPosition,
            placeholderSize: placeholderSize,
            grid: grid,
            tick: tick,
            crosshairMargin: crosshairMargin
        } = that._options;
        var isDefinedCustomPositionOption = isDefined(customPosition);
        var boundaryPosition = that.getResolvedBoundaryPosition();
        var canvas = that.getCanvas();
        var cLeft = canvas.left;
        var cTop = canvas.top;
        var cRight = canvas.width - canvas.right;
        var cBottom = canvas.height - canvas.bottom;
        var edgeMarginCorrection = _max(grid.visible && grid.width || 0, tick.visible && tick.width || 0);
        var constantLineAboveSeries = that._axisConstantLineGroups.above;
        var constantLineUnderSeries = that._axisConstantLineGroups.under;
        var boxes = [that._axisElementsGroup, constantLineAboveSeries.outside1, constantLineAboveSeries.outside2, constantLineUnderSeries.outside1, constantLineUnderSeries.outside2, that._axisLineGroup].map(group => group && group.getBBox()).concat(function(group) {
            var box = group && group.getBBox();
            if (!box || box.isEmpty) {
                return box
            }
            if (that._isHorizontal) {
                box.x = cLeft;
                box.width = cRight - cLeft
            } else {
                box.y = cTop;
                box.height = cBottom - cTop
            }
            return box
        }(that._axisTitleGroup));
        var margins = calculateCanvasMargins(boxes, canvas);
        margins[position] += crosshairMargin;
        if (that.hasNonBoundaryPosition() && isDefinedCustomPositionOption) {
            margins[boundaryPosition] = 0
        }
        if (placeholderSize) {
            margins[position] = placeholderSize
        }
        if (edgeMarginCorrection) {
            if (that._isHorizontal && canvas.right < edgeMarginCorrection && margins.right < edgeMarginCorrection) {
                margins.right = edgeMarginCorrection
            }
            if (!that._isHorizontal && canvas.bottom < edgeMarginCorrection && margins.bottom < edgeMarginCorrection) {
                margins.bottom = edgeMarginCorrection
            }
        }
        if (!isDefinedCustomPositionOption && isDefined(offset)) {
            var moveByOffset = that.customPositionIsBoundary() && (offset > 0 && (boundaryPosition === LEFT || boundaryPosition === TOP) || offset < 0 && (boundaryPosition === RIGHT || boundaryPosition === BOTTOM));
            margins[boundaryPosition] -= moveByOffset ? offset : 0
        }
        return margins
    },
    validateUnit: function(unit, idError, parameters) {
        unit = this.parser(unit);
        if (void 0 === unit && idError) {
            this._incidentOccurred(idError, [parameters])
        }
        return unit
    },
    _setType: function(axisType, drawingType) {
        var axisTypeMethods;
        switch (axisType) {
            case "xyAxes":
                axisTypeMethods = xyMethods;
                break;
            case "polarAxes":
                axisTypeMethods = polarMethods
        }
        extend(this, axisTypeMethods[drawingType])
    },
    _getSharpParam: function() {
        return true
    },
    _disposeBreaksGroup: _noop,
    dispose: function() {
        [this._axisElementsGroup, this._axisStripGroup, this._axisGroup].forEach((function(g) {
            g.dispose()
        }));
        this._strips = this._title = null;
        this._axisStripGroup = this._axisConstantLineGroups = this._axisStripLabelGroup = this._axisBreaksGroup = null;
        this._axisLineGroup = this._axisElementsGroup = this._axisGridGroup = null;
        this._axisGroup = this._axisTitleGroup = null;
        this._axesContainerGroup = this._stripsGroup = this._constantLinesGroup = this._labelsAxesGroup = null;
        this._renderer = this._options = this._textOptions = this._textFontStyles = null;
        this._translator = null;
        this._majorTicks = this._minorTicks = null;
        this._disposeBreaksGroup();
        this._templatesRendered && this._templatesRendered.reject()
    },
    getOptions: function() {
        return this._options
    },
    setPane: function(pane) {
        this.pane = pane;
        this._options.pane = pane
    },
    setTypes: function(type, axisType, typeSelector) {
        this._options.type = type || this._options.type;
        this._options[typeSelector] = axisType || this._options[typeSelector];
        this._updateTranslator()
    },
    resetTypes: function(typeSelector) {
        this._options.type = this._initTypes.type;
        this._options[typeSelector] = this._initTypes[typeSelector]
    },
    getTranslator: function() {
        return this._translator
    },
    updateOptions: function(options) {
        var that = this;
        var labelOpt = options.label;
        validateAxisOptions(options);
        that._options = options;
        options.tick = options.tick || {};
        options.minorTick = options.minorTick || {};
        options.grid = options.grid || {};
        options.minorGrid = options.minorGrid || {};
        options.title = options.title || {};
        options.marker = options.marker || {};
        that._initTypes = {
            type: options.type,
            argumentType: options.argumentType,
            valueType: options.valueType
        };
        that._setTickOffset();
        that._isHorizontal = options.isHorizontal;
        that.pane = options.pane;
        that.name = options.name;
        that.priority = options.priority;
        that._hasLabelFormat = "" !== labelOpt.format && isDefined(labelOpt.format);
        that._textOptions = {
            opacity: labelOpt.opacity,
            align: "center",
            class: labelOpt.cssClass
        };
        that._textFontStyles = patchFontOptions(labelOpt.font);
        if (options.type === constants.logarithmic) {
            if (options.logarithmBaseError) {
                that._incidentOccurred("E2104");
                delete options.logarithmBaseError
            }
        }
        that._updateTranslator();
        that._createConstantLines();
        that._strips = (options.strips || []).map(o => createStrip(that, o));
        that._majorTicks = that._minorTicks = null;
        that._firstDrawing = true
    },
    calculateInterval: function(value, prevValue) {
        var options = this._options;
        if (!options || options.type !== constants.logarithmic) {
            return _abs(value - prevValue)
        }
        var {
            allowNegatives: allowNegatives,
            linearThreshold: linearThreshold
        } = new Range(this.getTranslator().getBusinessRange());
        return _abs(getLog(value, options.logarithmBase, allowNegatives, linearThreshold) - getLog(prevValue, options.logarithmBase, allowNegatives, linearThreshold))
    },
    getCanvasRange() {
        var translator = this._translator;
        return {
            startValue: translator.from(translator.translate("canvas_position_start")),
            endValue: translator.from(translator.translate("canvas_position_end"))
        }
    },
    _processCanvas: function(canvas) {
        return canvas
    },
    updateCanvas: function(canvas, canvasRedesign) {
        if (!canvasRedesign) {
            var positions = this._orthogonalPositions = {
                start: !this._isHorizontal ? canvas.left : canvas.top,
                end: !this._isHorizontal ? canvas.width - canvas.right : canvas.height - canvas.bottom
            };
            positions.center = positions.start + (positions.end - positions.start) / 2
        } else {
            this._orthogonalPositions = null
        }
        this._canvas = canvas;
        this._translator.updateCanvas(this._processCanvas(canvas));
        this._initAxisPositions()
    },
    getCanvas: function() {
        return this._canvas
    },
    getAxisShift() {
        return this._axisShift || 0
    },
    hideTitle: function() {
        if (this._options.title.text) {
            this._incidentOccurred("W2105", [this._isHorizontal ? "horizontal" : "vertical"]);
            this._axisTitleGroup.clear()
        }
    },
    getTitle: function() {
        return this._title
    },
    hideOuterElements: function() {
        var options = this._options;
        if ((options.label.visible || this._outsideConstantLines.length) && !this._translator.getBusinessRange().isEmpty()) {
            this._incidentOccurred("W2106", [this._isHorizontal ? "horizontal" : "vertical"]);
            this._axisElementsGroup.clear();
            callAction(this._outsideConstantLines, "removeLabel")
        }
    },
    _resolveLogarithmicOptionsForRange(range) {
        var options = this._options;
        if (options.type === constants.logarithmic) {
            range.addRange({
                allowNegatives: void 0 !== options.allowNegatives ? options.allowNegatives : range.min <= 0
            });
            if (!isNaN(options.linearThreshold)) {
                range.linearThreshold = options.linearThreshold
            }
        }
    },
    adjustViewport(businessRange) {
        var options = this._options;
        var isDiscrete = options.type === constants.discrete;
        var categories = this._seriesData && this._seriesData.categories || [];
        var wholeRange = this.adjustRange(getVizRangeObject(options.wholeRange));
        var visualRange = this.getViewport() || {};
        var result = new Range(businessRange);
        this._addConstantLinesToRange(result, "minVisible", "maxVisible");
        var minDefined = isDefined(visualRange.startValue);
        var maxDefined = isDefined(visualRange.endValue);
        if (!isDiscrete) {
            minDefined = minDefined && (!isDefined(wholeRange.endValue) || visualRange.startValue < wholeRange.endValue);
            maxDefined = maxDefined && (!isDefined(wholeRange.startValue) || visualRange.endValue > wholeRange.startValue)
        }
        var minVisible = minDefined ? visualRange.startValue : result.minVisible;
        var maxVisible = maxDefined ? visualRange.endValue : result.maxVisible;
        if (!isDiscrete) {
            var _wholeRange$startValu, _wholeRange$endValue;
            result.min = null !== (_wholeRange$startValu = wholeRange.startValue) && void 0 !== _wholeRange$startValu ? _wholeRange$startValu : result.min;
            result.max = null !== (_wholeRange$endValue = wholeRange.endValue) && void 0 !== _wholeRange$endValue ? _wholeRange$endValue : result.max
        } else {
            var categoriesInfo = getCategoriesInfo(categories, wholeRange.startValue, wholeRange.endValue);
            categories = categoriesInfo.categories;
            result.categories = categories
        }
        var adjustedVisualRange = adjustVisualRange({
            axisType: options.type,
            dataType: options.dataType,
            base: options.logarithmBase
        }, {
            startValue: minDefined ? visualRange.startValue : void 0,
            endValue: maxDefined ? visualRange.endValue : void 0,
            length: visualRange.length
        }, {
            categories: categories,
            min: wholeRange.startValue,
            max: wholeRange.endValue
        }, {
            categories: categories,
            min: minVisible,
            max: maxVisible
        });
        result.minVisible = adjustedVisualRange.startValue;
        result.maxVisible = adjustedVisualRange.endValue;
        !isDefined(result.min) && (result.min = result.minVisible);
        !isDefined(result.max) && (result.max = result.maxVisible);
        result.addRange({});
        this._resolveLogarithmicOptionsForRange(result);
        return result
    },
    adjustRange(range) {
        range = range || {};
        var isDiscrete = this._options.type === constants.discrete;
        var isLogarithmic = this._options.type === constants.logarithmic;
        var disabledNegatives = false === this._options.allowNegatives;
        if (isLogarithmic) {
            range.startValue = disabledNegatives && range.startValue <= 0 ? null : range.startValue;
            range.endValue = disabledNegatives && range.endValue <= 0 ? null : range.endValue
        }
        if (!isDiscrete && isDefined(range.startValue) && isDefined(range.endValue) && range.startValue > range.endValue) {
            var tmp = range.endValue;
            range.endValue = range.startValue;
            range.startValue = tmp
        }
        return range
    },
    _getVisualRangeUpdateMode(viewport, newRange, oppositeValue) {
        var value = this._options.visualRangeUpdateMode;
        var translator = this._translator;
        var range = this._seriesData;
        var prevDataInfo = this._prevDataInfo;
        if (prevDataInfo.isEmpty && !prevDataInfo.containsConstantLine) {
            return KEEP
        }
        if (!this.isArgumentAxis) {
            var _viewport = this.getViewport();
            if (!isDefined(_viewport.startValue) && !isDefined(_viewport.endValue) && !isDefined(_viewport.length)) {
                return RESET
            }
        }
        if (this.isArgumentAxis) {
            if (-1 === [SHIFT, KEEP, RESET].indexOf(value)) {
                if (range.axisType === constants.discrete) {
                    var categories = range.categories;
                    var newCategories = newRange.categories;
                    var visualRange = this.visualRange();
                    if (categories && newCategories && categories.length && -1 !== newCategories.map(c => c.valueOf()).join(",").indexOf(categories.map(c => c.valueOf()).join(",")) && (visualRange.startValue.valueOf() !== categories[0].valueOf() || visualRange.endValue.valueOf() !== categories[categories.length - 1].valueOf())) {
                        value = KEEP
                    } else {
                        value = RESET
                    }
                } else {
                    var minPoint = translator.translate(range.min);
                    var minVisiblePoint = translator.translate(viewport.startValue);
                    var maxPoint = translator.translate(range.max);
                    var maxVisiblePoint = translator.translate(viewport.endValue);
                    if (minPoint === minVisiblePoint && maxPoint === maxVisiblePoint) {
                        value = RESET
                    } else if (minPoint !== minVisiblePoint && maxPoint === maxVisiblePoint) {
                        value = SHIFT
                    } else {
                        value = KEEP
                    }
                }
                if (value === KEEP && prevDataInfo.isEmpty && prevDataInfo.containsConstantLine) {
                    value = RESET
                }
            }
        } else if (-1 === [KEEP, RESET].indexOf(value)) {
            if (oppositeValue === KEEP) {
                value = KEEP
            } else {
                value = RESET
            }
        }
        return value
    },
    _handleBusinessRangeChanged(oppositeVisualRangeUpdateMode, axisReinitialized, newRange) {
        var visualRange = this.visualRange();
        if (axisReinitialized || this._translator.getBusinessRange().isEmpty()) {
            return
        }
        var visualRangeUpdateMode = this._lastVisualRangeUpdateMode = this._getVisualRangeUpdateMode(visualRange, newRange, oppositeVisualRangeUpdateMode);
        if (visualRangeUpdateMode === KEEP) {
            this._setVisualRange([visualRange.startValue, visualRange.endValue])
        } else if (visualRangeUpdateMode === RESET) {
            this._setVisualRange([null, null])
        } else if (visualRangeUpdateMode === SHIFT) {
            this._setVisualRange({
                length: this.getVisualRangeLength()
            })
        }
    },
    getVisualRangeLength(range) {
        var currentBusinessRange = range || this._translator.getBusinessRange();
        var {
            type: type
        } = this._options;
        var length;
        if (type === constants.logarithmic) {
            length = adjust(this.calculateInterval(currentBusinessRange.maxVisible, currentBusinessRange.minVisible))
        } else if (type === constants.discrete) {
            var categoriesInfo = getCategoriesInfo(currentBusinessRange.categories, currentBusinessRange.minVisible, currentBusinessRange.maxVisible);
            length = categoriesInfo.categories.length
        } else {
            length = currentBusinessRange.maxVisible - currentBusinessRange.minVisible
        }
        return length
    },
    getVisualRangeCenter(range, useMerge) {
        var translator = this.getTranslator();
        var businessRange = translator.getBusinessRange();
        var currentBusinessRange = useMerge ? extend(true, {}, businessRange, range || {}) : range || businessRange;
        var {
            type: type,
            logarithmBase: logarithmBase
        } = this._options;
        var center;
        if (!isDefined(currentBusinessRange.minVisible) || !isDefined(currentBusinessRange.maxVisible)) {
            return
        }
        if (type === constants.logarithmic) {
            var {
                allowNegatives: allowNegatives,
                linearThreshold: linearThreshold,
                minVisible: minVisible,
                maxVisible: maxVisible
            } = currentBusinessRange;
            center = raiseTo(adjust(getLog(maxVisible, logarithmBase, allowNegatives, linearThreshold) + getLog(minVisible, logarithmBase, allowNegatives, linearThreshold)) / 2, logarithmBase, allowNegatives, linearThreshold)
        } else if (type === constants.discrete) {
            var categoriesInfo = getCategoriesInfo(currentBusinessRange.categories, currentBusinessRange.minVisible, currentBusinessRange.maxVisible);
            var index = Math.ceil(categoriesInfo.categories.length / 2) - 1;
            center = businessRange.categories.indexOf(categoriesInfo.categories[index])
        } else {
            center = translator.toValue((currentBusinessRange.maxVisible.valueOf() + currentBusinessRange.minVisible.valueOf()) / 2)
        }
        return center
    },
    setBusinessRange(range, axisReinitialized, oppositeVisualRangeUpdateMode, argCategories) {
        var _that$_seriesData$min, _that$_seriesData$max;
        var options = this._options;
        var isDiscrete = options.type === constants.discrete;
        this._handleBusinessRangeChanged(oppositeVisualRangeUpdateMode, axisReinitialized, range);
        this._seriesData = new Range(range);
        var dataIsEmpty = this._seriesData.isEmpty();
        var rangeWithConstantLines = new Range(this._seriesData);
        this._addConstantLinesToRange(rangeWithConstantLines, "minVisible", "maxVisible");
        this._prevDataInfo = {
            isEmpty: dataIsEmpty,
            containsConstantLine: rangeWithConstantLines.containsConstantLine
        };
        this._seriesData.addRange({
            categories: options.categories,
            dataType: options.dataType,
            axisType: options.type,
            base: options.logarithmBase,
            invert: options.inverted
        });
        this._resolveLogarithmicOptionsForRange(this._seriesData);
        if (!isDiscrete) {
            if (!isDefined(this._seriesData.min) && !isDefined(this._seriesData.max)) {
                var visualRange = this.getViewport();
                visualRange && this._seriesData.addRange({
                    min: visualRange.startValue,
                    max: visualRange.endValue
                })
            }
            var synchronizedValue = options.synchronizedValue;
            if (isDefined(synchronizedValue)) {
                this._seriesData.addRange({
                    min: synchronizedValue,
                    max: synchronizedValue
                })
            }
        }
        this._seriesData.minVisible = null !== (_that$_seriesData$min = this._seriesData.minVisible) && void 0 !== _that$_seriesData$min ? _that$_seriesData$min : this._seriesData.min;
        this._seriesData.maxVisible = null !== (_that$_seriesData$max = this._seriesData.maxVisible) && void 0 !== _that$_seriesData$max ? _that$_seriesData$max : this._seriesData.max;
        if (!this.isArgumentAxis && options.showZero) {
            this._seriesData.correctValueZeroLevel()
        }
        this._seriesData.sortCategories(this.getCategoriesSorter(argCategories));
        this._seriesData.userBreaks = this._seriesData.isEmpty() ? [] : this._getScaleBreaks(options, this._seriesData, this._series, this.isArgumentAxis);
        this._translator.updateBusinessRange(this._getViewportRange())
    },
    _addConstantLinesToRange(dataRange, minValueField, maxValueField) {
        this._outsideConstantLines.concat(this._insideConstantLines || []).forEach(cl => {
            if (cl.options.extendAxis) {
                var value = cl.getParsedValue();
                dataRange.addRange({
                    containsConstantLine: true,
                    [minValueField]: value,
                    [maxValueField]: value
                })
            }
        })
    },
    setGroupSeries: function(series) {
        this._series = series
    },
    getLabelsPosition: function() {
        var options = this._options;
        var position = options.position;
        var labelShift = options.label.indentFromAxis + (this._axisShift || 0) + this._constantLabelOffset;
        var axisPosition = this._axisPosition;
        return position === TOP || position === LEFT ? axisPosition - labelShift : axisPosition + labelShift
    },
    getFormattedValue: function(value, options, point) {
        var labelOptions = this._options.label;
        return isDefined(value) ? this.formatLabel(value, extend(true, {}, labelOptions, options), void 0, point) : null
    },
    _getBoundaryTicks: function(majors, viewPort) {
        var length = majors.length;
        var options = this._options;
        var customBounds = options.customBoundTicks;
        var min = viewPort.minVisible;
        var max = viewPort.maxVisible;
        var addMinMax = options.showCustomBoundaryTicks ? this._boundaryTicksVisibility : {};
        var boundaryTicks = [];
        if (options.type === constants.discrete) {
            if (this._tickOffset && 0 !== majors.length) {
                boundaryTicks = [majors[0], majors[majors.length - 1]]
            }
        } else if (customBounds) {
            if (addMinMax.min && isDefined(customBounds[0])) {
                boundaryTicks.push(customBounds[0])
            }
            if (addMinMax.max && isDefined(customBounds[1])) {
                boundaryTicks.push(customBounds[1])
            }
        } else {
            if (addMinMax.min && (0 === length || majors[0] > min)) {
                boundaryTicks.push(min)
            }
            if (addMinMax.max && (0 === length || majors[length - 1] < max)) {
                boundaryTicks.push(max)
            }
        }
        return boundaryTicks
    },
    setPercentLabelFormat: function() {
        if (!this._hasLabelFormat) {
            this._options.label.format = "percent"
        }
    },
    resetAutoLabelFormat: function() {
        if (!this._hasLabelFormat) {
            delete this._options.label.format
        }
    },
    getMultipleAxesSpacing: function() {
        return this._options.multipleAxesSpacing || 0
    },
    getTicksValues: function() {
        return {
            majorTicksValues: convertTicksToValues(this._majorTicks),
            minorTicksValues: convertTicksToValues(this._minorTicks)
        }
    },
    estimateTickInterval: function(canvas) {
        this.updateCanvas(canvas);
        return this._tickInterval !== this._getTicks(this._getViewportRange(), _noop, true).tickInterval
    },
    setTicks: function(ticks) {
        var majors = ticks.majorTicks || [];
        this._majorTicks = majors.map(createMajorTick(this, this._renderer, this._getSkippedCategory(majors)));
        this._minorTicks = (ticks.minorTicks || []).map(createMinorTick(this, this._renderer));
        this._isSynchronized = true
    },
    _adjustDivisionFactor: function(val) {
        return val
    },
    _getTicks: function(viewPort, incidentOccurred, skipTickGeneration) {
        var options = this._options;
        var customTicks = options.customTicks;
        var customMinorTicks = options.customMinorTicks;
        return getTickGenerator(options, incidentOccurred || this._incidentOccurred, skipTickGeneration, this._translator.getBusinessRange().isEmpty(), this._adjustDivisionFactor.bind(this), viewPort)({
            min: viewPort.minVisible,
            max: viewPort.maxVisible,
            categories: viewPort.categories,
            isSpacedMargin: viewPort.isSpacedMargin
        }, this._getScreenDelta(), options.tickInterval, "ignore" === options.label.overlappingBehavior || options.forceUserTickInterval, {
            majors: customTicks,
            minors: customMinorTicks
        }, options.minorTickInterval, options.minorTickCount, this._initialBreaks)
    },
    _createTicksAndLabelFormat: function(range, incidentOccurred) {
        var options = this._options;
        var ticks = this._getTicks(range, incidentOccurred, false);
        if (!range.isEmpty() && options.type === constants.discrete && "datetime" === options.dataType && !this._hasLabelFormat && ticks.ticks.length) {
            options.label.format = formatHelper.getDateFormatByTicks(ticks.ticks)
        }
        return ticks
    },
    getAggregationInfo(useAllAggregatedPoints, range) {
        var _visualRange$startVal, _visualRange$endValue, _that$_seriesData;
        var options = this._options;
        var marginOptions = this._marginOptions;
        var businessRange = new Range(this.getTranslator().getBusinessRange()).addRange(range);
        var visualRange = this.getViewport();
        var minVisible = null !== (_visualRange$startVal = null === visualRange || void 0 === visualRange ? void 0 : visualRange.startValue) && void 0 !== _visualRange$startVal ? _visualRange$startVal : businessRange.minVisible;
        var maxVisible = null !== (_visualRange$endValue = null === visualRange || void 0 === visualRange ? void 0 : visualRange.endValue) && void 0 !== _visualRange$endValue ? _visualRange$endValue : businessRange.maxVisible;
        var ticks = [];
        if (options.type === constants.discrete && options.aggregateByCategory) {
            return {
                aggregateByCategory: true
            }
        }
        var aggregationInterval = options.aggregationInterval;
        var aggregationGroupWidth = options.aggregationGroupWidth;
        if (!aggregationGroupWidth && marginOptions) {
            if (marginOptions.checkInterval) {
                aggregationGroupWidth = options.axisDivisionFactor
            }
            if (marginOptions.sizePointNormalState) {
                aggregationGroupWidth = Math.min(marginOptions.sizePointNormalState, options.axisDivisionFactor)
            }
        }
        var minInterval = !options.aggregationGroupWidth && !aggregationInterval && range.interval;
        var generateTicks = configureGenerator(options, aggregationGroupWidth, businessRange, this._getScreenDelta(), minInterval);
        var tickInterval = generateTicks(aggregationInterval, true, minVisible, maxVisible, null === (_that$_seriesData = this._seriesData) || void 0 === _that$_seriesData ? void 0 : _that$_seriesData.breaks).tickInterval;
        if (options.type !== constants.discrete) {
            var min = useAllAggregatedPoints ? businessRange.min : minVisible;
            var max = useAllAggregatedPoints ? businessRange.max : maxVisible;
            if (isDefined(min) && isDefined(max)) {
                var add = getAddFunction({
                    base: options.logarithmBase,
                    axisType: options.type,
                    dataType: options.dataType
                }, false);
                var start = min;
                var end = max;
                if (!useAllAggregatedPoints) {
                    var maxMinDistance = Math.max(this.calculateInterval(max, min), "datetime" === options.dataType ? dateUtils.dateToMilliseconds(tickInterval) : tickInterval);
                    start = add(min, maxMinDistance, -1);
                    end = add(max, maxMinDistance)
                }
                start = start < businessRange.min ? businessRange.min : start;
                end = end > businessRange.max ? businessRange.max : end;
                var breaks = this._getScaleBreaks(options, {
                    minVisible: start,
                    maxVisible: end
                }, this._series, this.isArgumentAxis);
                var filteredBreaks = this._filterBreaks(breaks, {
                    minVisible: start,
                    maxVisible: end
                }, options.breakStyle);
                ticks = generateTicks(tickInterval, false, start, end, filteredBreaks).ticks
            }
        }
        this._aggregationInterval = tickInterval;
        return {
            interval: tickInterval,
            ticks: ticks
        }
    },
    getTickInterval() {
        return this._tickInterval
    },
    getAggregationInterval() {
        return this._aggregationInterval
    },
    createTicks: function(canvas) {
        var that = this;
        var renderer = that._renderer;
        var options = that._options;
        if (!canvas) {
            return
        }
        that._isSynchronized = false;
        that.updateCanvas(canvas);
        var range = that._getViewportRange();
        that._initialBreaks = range.breaks = this._seriesData.breaks = that._filterBreaks(this._seriesData.userBreaks, range, options.breakStyle);
        that._estimatedTickInterval = that._getTicks(that.adjustViewport(this._seriesData), _noop, true).tickInterval;
        var margins = this._calculateValueMargins();
        range.addRange({
            minVisible: margins.minValue,
            maxVisible: margins.maxValue,
            isSpacedMargin: margins.isSpacedMargin
        });
        var ticks = that._createTicksAndLabelFormat(range);
        var boundaryTicks = that._getBoundaryTicks(ticks.ticks, that._getViewportRange());
        if (options.showCustomBoundaryTicks && boundaryTicks.length) {
            that._boundaryTicks = [boundaryTicks[0]].map(createBoundaryTick(that, renderer, true));
            if (boundaryTicks.length > 1) {
                that._boundaryTicks = that._boundaryTicks.concat([boundaryTicks[1]].map(createBoundaryTick(that, renderer, false)))
            }
        } else {
            that._boundaryTicks = []
        }
        var minors = (ticks.minorTicks || []).filter((function(minor) {
            return !boundaryTicks.some((function(boundary) {
                return valueOf(boundary) === valueOf(minor)
            }))
        }));
        that._tickInterval = ticks.tickInterval;
        that._minorTickInterval = ticks.minorTickInterval;
        var oldMajorTicks = that._majorTicks || [];
        var majorTicksByValues = oldMajorTicks.reduce((r, t) => {
            r[t.value.valueOf()] = t;
            return r
        }, {});
        var sameType = type(ticks.ticks[0]) === type(oldMajorTicks[0] && oldMajorTicks[0].value);
        var skippedCategory = that._getSkippedCategory(ticks.ticks);
        var majorTicks = ticks.ticks.map(v => {
            var tick = majorTicksByValues[v.valueOf()];
            if (tick && sameType) {
                delete majorTicksByValues[v.valueOf()];
                tick.setSkippedCategory(skippedCategory);
                return tick
            } else {
                return createMajorTick(that, renderer, skippedCategory)(v)
            }
        });
        that._majorTicks = majorTicks;
        var oldMinorTicks = that._minorTicks || [];
        that._minorTicks = minors.map((v, i) => {
            var minorTick = oldMinorTicks[i];
            if (minorTick) {
                minorTick.updateValue(v);
                return minorTick
            }
            return createMinorTick(that, renderer)(v)
        });
        that._ticksToRemove = Object.keys(majorTicksByValues).map(k => majorTicksByValues[k]).concat(oldMinorTicks.slice(that._minorTicks.length, oldMinorTicks.length));
        that._ticksToRemove.forEach(t => {
            var _t$label;
            return null === (_t$label = t.label) || void 0 === _t$label ? void 0 : _t$label.removeTitle()
        });
        if (ticks.breaks) {
            that._seriesData.breaks = ticks.breaks
        }
        that._reinitTranslator(that._getViewportRange())
    },
    _reinitTranslator: function(range) {
        var translator = this._translator;
        if (this._isSynchronized) {
            return
        }
        translator.updateBusinessRange(range)
    },
    _getViewportRange() {
        return this.adjustViewport(this._seriesData)
    },
    setMarginOptions: function(options) {
        this._marginOptions = options
    },
    getMarginOptions() {
        var _this$_marginOptions;
        return null !== (_this$_marginOptions = this._marginOptions) && void 0 !== _this$_marginOptions ? _this$_marginOptions : {}
    },
    _calculateRangeInterval: function(interval) {
        var isDateTime = "datetime" === this._options.dataType;
        var minArgs = [];
        var addToArgs = function(value) {
            isDefined(value) && minArgs.push(isDateTime ? dateUtils.dateToMilliseconds(value) : value)
        };
        addToArgs(this._tickInterval);
        addToArgs(this._estimatedTickInterval);
        isDefined(interval) && minArgs.push(interval);
        addToArgs(this._aggregationInterval);
        return this._calculateWorkWeekInterval(_min.apply(this, minArgs))
    },
    _calculateWorkWeekInterval(businessInterval) {
        var options = this._options;
        if ("datetime" === options.dataType && options.workdaysOnly && businessInterval) {
            var workWeek = options.workWeek.length * dateIntervals.day;
            var weekend = dateIntervals.week - workWeek;
            if (workWeek !== businessInterval && weekend < businessInterval) {
                var weekendsCount = Math.ceil(businessInterval / dateIntervals.week);
                businessInterval -= weekend * weekendsCount
            } else if (weekend >= businessInterval && businessInterval > dateIntervals.day) {
                businessInterval = dateIntervals.day
            }
        }
        return businessInterval
    },
    _getConvertIntervalCoefficient(intervalInPx, screenDelta) {
        var ratioOfCanvasRange = this._translator.ratioOfCanvasRange();
        return ratioOfCanvasRange / (ratioOfCanvasRange * screenDelta / (intervalInPx + screenDelta))
    },
    _calculateValueMargins(ticks) {
        this._resetMargins();
        var margins = this.getMarginOptions();
        var marginSize = (margins.size || 0) / 2;
        var options = this._options;
        var dataRange = this._getViewportRange();
        var viewPort = this.getViewport();
        var screenDelta = this._getScreenDelta();
        var isDiscrete = -1 !== (options.type || "").indexOf(constants.discrete);
        var valueMarginsEnabled = options.valueMarginsEnabled && !isDiscrete && !this.customPositionIsBoundaryOrthogonalAxis();
        var translator = this._translator;
        var minValueMargin = options.minValueMargin;
        var maxValueMargin = options.maxValueMargin;
        var minPadding = 0;
        var maxPadding = 0;
        var interval = 0;
        var rangeInterval;
        if (dataRange.stubData || !screenDelta) {
            return {
                startPadding: 0,
                endPadding: 0
            }
        }
        if (this.isArgumentAxis && margins.checkInterval) {
            rangeInterval = this._calculateRangeInterval(dataRange.interval);
            var pxInterval = translator.getInterval(rangeInterval);
            if (isFinite(pxInterval)) {
                interval = Math.ceil(pxInterval / (2 * this._getConvertIntervalCoefficient(pxInterval, screenDelta)))
            } else {
                rangeInterval = 0
            }
        }
        var minPercentPadding;
        var maxPercentPadding;
        var maxPaddingValue = screenDelta * MAX_MARGIN_VALUE / 2;
        if (valueMarginsEnabled) {
            if (isDefined(minValueMargin)) {
                minPercentPadding = isFinite(minValueMargin) ? minValueMargin : 0
            } else if (!this.isArgumentAxis && margins.checkInterval && valueOf(dataRange.minVisible) > 0 && valueOf(dataRange.minVisible) === valueOf(dataRange.min)) {
                minPadding = MIN_BAR_MARGIN
            } else {
                minPadding = Math.max(marginSize, interval);
                minPadding = Math.min(maxPaddingValue, minPadding)
            }
            if (isDefined(maxValueMargin)) {
                maxPercentPadding = isFinite(maxValueMargin) ? maxValueMargin : 0
            } else if (!this.isArgumentAxis && margins.checkInterval && valueOf(dataRange.maxVisible) < 0 && valueOf(dataRange.maxVisible) === valueOf(dataRange.max)) {
                maxPadding = MIN_BAR_MARGIN
            } else {
                maxPadding = Math.max(marginSize, interval);
                maxPadding = Math.min(maxPaddingValue, maxPadding)
            }
        }
        var percentStick = margins.percentStick && !this.isArgumentAxis;
        if (percentStick) {
            if (1 === _abs(dataRange.max)) {
                maxPadding = 0
            }
            if (1 === _abs(dataRange.min)) {
                minPadding = 0
            }
        }
        var canvasStartEnd = this._getCanvasStartEnd();
        var commonMargin = 1 + (minPercentPadding || 0) + (maxPercentPadding || 0);
        var screenDeltaWithMargins = (screenDelta - minPadding - maxPadding) / commonMargin || screenDelta;
        if (void 0 !== minPercentPadding || void 0 !== maxPercentPadding) {
            if (void 0 !== minPercentPadding) {
                minPadding = screenDeltaWithMargins * minPercentPadding
            }
            if (void 0 !== maxPercentPadding) {
                maxPadding = screenDeltaWithMargins * maxPercentPadding
            }
        }
        var minValue;
        var maxValue;
        if (options.type !== constants.discrete && ticks && ticks.length > 1 && !options.skipViewportExtending && !viewPort.action && false !== options.endOnTick) {
            var length = ticks.length;
            var firstTickPosition = translator.translate(ticks[0].value);
            var lastTickPosition = translator.translate(ticks[length - 1].value);
            var invertMultiplier = firstTickPosition > lastTickPosition ? -1 : 1;
            var minTickPadding = _max(invertMultiplier * (canvasStartEnd.start - firstTickPosition), 0);
            var maxTickPadding = _max(invertMultiplier * (lastTickPosition - canvasStartEnd.end), 0);
            if (minTickPadding > minPadding || maxTickPadding > maxPadding) {
                var commonPadding = maxTickPadding + minTickPadding;
                var coeff = this._getConvertIntervalCoefficient(commonPadding, screenDelta);
                if (minTickPadding >= minPadding) {
                    minValue = ticks[0].value
                }
                if (maxTickPadding >= maxPadding) {
                    maxValue = ticks[length - 1].value
                }
                minPadding = _max(minTickPadding, minPadding) / coeff;
                maxPadding = _max(maxTickPadding, maxPadding) / coeff
            }
        }
        minPercentPadding = void 0 === minPercentPadding ? minPadding / screenDeltaWithMargins : minPercentPadding;
        maxPercentPadding = void 0 === maxPercentPadding ? maxPadding / screenDeltaWithMargins : maxPercentPadding;
        if (!isDiscrete) {
            if (this._translator.isInverted()) {
                var _minValue, _maxValue;
                minValue = null !== (_minValue = minValue) && void 0 !== _minValue ? _minValue : translator.from(canvasStartEnd.start + screenDelta * minPercentPadding, -1);
                maxValue = null !== (_maxValue = maxValue) && void 0 !== _maxValue ? _maxValue : translator.from(canvasStartEnd.end - screenDelta * maxPercentPadding, 1)
            } else {
                var _minValue2, _maxValue2;
                minValue = null !== (_minValue2 = minValue) && void 0 !== _minValue2 ? _minValue2 : translator.from(canvasStartEnd.start - screenDelta * minPercentPadding, -1);
                maxValue = null !== (_maxValue2 = maxValue) && void 0 !== _maxValue2 ? _maxValue2 : translator.from(canvasStartEnd.end + screenDelta * maxPercentPadding, 1)
            }
        }
        var {
            correctedMin: correctedMin,
            correctedMax: correctedMax,
            start: start,
            end: end
        } = this.getCorrectedValuesToZero(minValue, maxValue);
        minPadding = null !== start && void 0 !== start ? start : minPadding;
        maxPadding = null !== end && void 0 !== end ? end : maxPadding;
        return {
            startPadding: translator.isInverted() ? maxPadding : minPadding,
            endPadding: translator.isInverted() ? minPadding : maxPadding,
            minValue: null !== correctedMin && void 0 !== correctedMin ? correctedMin : minValue,
            maxValue: null !== correctedMax && void 0 !== correctedMax ? correctedMax : maxValue,
            interval: rangeInterval,
            isSpacedMargin: minPadding === maxPadding && 0 !== minPadding
        }
    },
    getCorrectedValuesToZero(minValue, maxValue) {
        var that = this;
        var translator = that._translator;
        var canvasStartEnd = that._getCanvasStartEnd();
        var dataRange = that._getViewportRange();
        var screenDelta = that._getScreenDelta();
        var options = that._options;
        var start;
        var end;
        var correctedMin;
        var correctedMax;
        var correctZeroLevel = (minPoint, maxPoint) => {
            var minExpectedPadding = _abs(canvasStartEnd.start - minPoint);
            var maxExpectedPadding = _abs(canvasStartEnd.end - maxPoint);
            var coeff = that._getConvertIntervalCoefficient(minExpectedPadding + maxExpectedPadding, screenDelta);
            start = minExpectedPadding / coeff;
            end = maxExpectedPadding / coeff
        };
        if (!that.isArgumentAxis && "datetime" !== options.dataType) {
            if (minValue * dataRange.min <= 0 && minValue * dataRange.minVisible <= 0) {
                correctZeroLevel(translator.translate(0), translator.translate(maxValue));
                correctedMin = 0
            }
            if (maxValue * dataRange.max <= 0 && maxValue * dataRange.maxVisible <= 0) {
                correctZeroLevel(translator.translate(minValue), translator.translate(0));
                correctedMax = 0
            }
        }
        return {
            start: isFinite(start) ? start : null,
            end: isFinite(end) ? end : null,
            correctedMin: correctedMin,
            correctedMax: correctedMax
        }
    },
    applyMargins() {
        if (this._isSynchronized) {
            return
        }
        var margins = this._calculateValueMargins(this._majorTicks);
        var canvas = extend({}, this._canvas, {
            startPadding: margins.startPadding,
            endPadding: margins.endPadding
        });
        this._translator.updateCanvas(this._processCanvas(canvas));
        if (isFinite(margins.interval)) {
            var br = this._translator.getBusinessRange();
            br.addRange({
                interval: margins.interval
            });
            this._translator.updateBusinessRange(br)
        }
    },
    _resetMargins: function() {
        this._reinitTranslator(this._getViewportRange());
        if (this._canvas) {
            this._translator.updateCanvas(this._processCanvas(this._canvas))
        }
    },
    _createConstantLines() {
        var constantLines = (this._options.constantLines || []).map(o => createConstantLine(this, o));
        this._outsideConstantLines = constantLines.filter(l => "outside" === l.labelPosition);
        this._insideConstantLines = constantLines.filter(l => "inside" === l.labelPosition)
    },
    draw: function(canvas, borderOptions) {
        var that = this;
        var options = this._options;
        that.borderOptions = borderOptions || {
            visible: false
        };
        that._resetMargins();
        that.createTicks(canvas);
        that.applyMargins();
        that._clearAxisGroups();
        initTickCoords(that._majorTicks);
        initTickCoords(that._minorTicks);
        initTickCoords(that._boundaryTicks);
        that._axisGroup.append(that._axesContainerGroup);
        that._drawAxis();
        that._drawTitle();
        drawTickMarks(that._majorTicks, options.tick);
        drawTickMarks(that._minorTicks, options.minorTick);
        drawTickMarks(that._boundaryTicks, options.tick);
        var drawGridLine = that._getGridLineDrawer();
        drawGrids(that._majorTicks, drawGridLine);
        drawGrids(that._minorTicks, drawGridLine);
        callAction(that._majorTicks, "drawLabel", that._getViewportRange(), that._getTemplate(options.label.template));
        that._templatesRendered && that._templatesRendered.reject();
        that._templatesRendered = new Deferred;
        that._majorTicks.forEach((function(tick) {
            tick.labelRotationAngle = 0;
            tick.labelAlignment = void 0;
            tick.labelOffset = 0
        }));
        callAction(that._outsideConstantLines.concat(that._insideConstantLines), "draw");
        callAction(that._strips, "draw");
        that._dateMarkers = that._drawDateMarkers() || [];
        that._stripLabelAxesGroup && that._axisStripLabelGroup.append(that._stripLabelAxesGroup);
        that._gridContainerGroup && that._axisGridGroup.append(that._gridContainerGroup);
        that._stripsGroup && that._axisStripGroup.append(that._stripsGroup);
        that._labelsAxesGroup && that._axisElementsGroup.append(that._labelsAxesGroup);
        if (that._constantLinesGroup) {
            that._axisConstantLineGroups.above.inside.append(that._constantLinesGroup.above);
            that._axisConstantLineGroups.above.outside1.append(that._constantLinesGroup.above);
            that._axisConstantLineGroups.above.outside2.append(that._constantLinesGroup.above);
            that._axisConstantLineGroups.under.inside.append(that._constantLinesGroup.under);
            that._axisConstantLineGroups.under.outside1.append(that._constantLinesGroup.under);
            that._axisConstantLineGroups.under.outside2.append(that._constantLinesGroup.under)
        }
        that._measureTitle();
        measureLabels(that._majorTicks);
        !options.label.template && that._applyWordWrap();
        measureLabels(that._outsideConstantLines);
        measureLabels(that._insideConstantLines);
        measureLabels(that._strips);
        measureLabels(that._dateMarkers);
        that._adjustConstantLineLabels(that._insideConstantLines);
        that._adjustStripLabels();
        var offset = that._constantLabelOffset = that._adjustConstantLineLabels(that._outsideConstantLines);
        if (!that._translator.getBusinessRange().isEmpty()) {
            that._setLabelsPlacement();
            offset = that._adjustLabels(offset)
        }
        when.apply(this, that._majorTicks.map(tick => tick.getTemplateDeferred())).done(() => {
            that._templatesRendered.resolve()
        });
        offset = that._adjustDateMarkers(offset);
        that._adjustTitle(offset)
    },
    getTemplatesDef() {
        return this._templatesRendered
    },
    setRenderedState(state) {
        this._drawn = state
    },
    isRendered() {
        return this._drawn
    },
    _applyWordWrap() {
        var convertedTickInterval;
        var textWidth;
        var textHeight;
        var options = this._options;
        var tickInterval = this._tickInterval;
        if (isDefined(tickInterval)) {
            convertedTickInterval = this.getTranslator().getInterval("datetime" === options.dataType ? dateUtils.dateToMilliseconds(tickInterval) : tickInterval)
        }
        var displayMode = this._validateDisplayMode(options.label.displayMode);
        var overlappingMode = this._validateOverlappingMode(options.label.overlappingBehavior, displayMode);
        var wordWrapMode = options.label.wordWrap || "none";
        var overflowMode = options.label.textOverflow || "none";
        if (("none" !== wordWrapMode || "none" !== overflowMode) && displayMode !== ROTATE && overlappingMode !== ROTATE && "auto" !== overlappingMode) {
            var usefulSpace = isDefined(options.placeholderSize) ? options.placeholderSize - options.label.indentFromAxis : void 0;
            if (this._isHorizontal) {
                textWidth = convertedTickInterval;
                textHeight = usefulSpace
            } else {
                textWidth = usefulSpace;
                textHeight = convertedTickInterval
            }
            var correctByWidth = false;
            var correctByHeight = false;
            if (textWidth) {
                if (this._majorTicks.some(tick => tick.labelBBox.width > textWidth)) {
                    correctByWidth = true
                }
            }
            if (textHeight) {
                if (this._majorTicks.some(tick => tick.labelBBox.height > textHeight)) {
                    correctByHeight = true
                }
            }
            if (correctByWidth || correctByHeight) {
                this._majorTicks.forEach(tick => {
                    tick.label && tick.label.setMaxSize(textWidth, textHeight, options.label)
                });
                measureLabels(this._majorTicks)
            }
        }
    },
    _measureTitle: _noop,
    animate() {
        callAction(this._majorTicks, "animateLabels")
    },
    updateSize(canvas, animate) {
        var updateTitle = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : true;
        this.updateCanvas(canvas);
        if (updateTitle) {
            this._checkTitleOverflow();
            this._measureTitle();
            this._updateTitleCoords()
        }
        this._reinitTranslator(this._getViewportRange());
        this.applyMargins();
        var animationEnabled = !this._firstDrawing && animate;
        var options = this._options;
        initTickCoords(this._majorTicks);
        initTickCoords(this._minorTicks);
        initTickCoords(this._boundaryTicks);
        if (this._resetApplyingAnimation && !this._firstDrawing) {
            this._resetStartCoordinates()
        }
        cleanUpInvalidTicks(this._majorTicks);
        cleanUpInvalidTicks(this._minorTicks);
        cleanUpInvalidTicks(this._boundaryTicks);
        if (this._axisElement) {
            this._updateAxisElementPosition()
        }
        updateTicksPosition(this._majorTicks, options.tick, animationEnabled);
        updateTicksPosition(this._minorTicks, options.minorTick, animationEnabled);
        updateTicksPosition(this._boundaryTicks, options.tick);
        callAction(this._majorTicks, "updateLabelPosition", animationEnabled);
        this._outsideConstantLines.concat(this._insideConstantLines || []).forEach(l => l.updatePosition(animationEnabled));
        callAction(this._strips, "updatePosition", animationEnabled);
        updateGridsPosition(this._majorTicks, animationEnabled);
        updateGridsPosition(this._minorTicks, animationEnabled);
        if (animationEnabled) {
            callAction(this._ticksToRemove || [], "fadeOutElements")
        }
        this.prepareAnimation();
        this._ticksToRemove = null;
        if (!this._translator.getBusinessRange().isEmpty()) {
            this._firstDrawing = false
        }
        this._resetApplyingAnimation = false;
        this._updateLabelsPosition()
    },
    _updateLabelsPosition: _noop,
    prepareAnimation() {
        var action = "saveCoords";
        callAction(this._majorTicks, action);
        callAction(this._minorTicks, action);
        callAction(this._insideConstantLines, action);
        callAction(this._outsideConstantLines, action);
        callAction(this._strips, action)
    },
    _resetStartCoordinates() {
        var action = "resetCoordinates";
        callAction(this._majorTicks, action);
        callAction(this._minorTicks, action);
        callAction(this._insideConstantLines, action);
        callAction(this._outsideConstantLines, action);
        callAction(this._strips, action)
    },
    applyClipRects: function(elementsClipID, canvasClipID) {
        this._axisGroup.attr({
            "clip-path": canvasClipID
        });
        this._axisStripGroup.attr({
            "clip-path": elementsClipID
        });
        this._axisElementsGroup.attr({
            "clip-path": canvasClipID
        })
    },
    _validateVisualRange(optionValue) {
        var range = getVizRangeObject(optionValue);
        if (void 0 !== range.startValue) {
            range.startValue = this.validateUnit(range.startValue)
        }
        if (void 0 !== range.endValue) {
            range.endValue = this.validateUnit(range.endValue)
        }
        return convertVisualRangeObject(range, !_isArray(optionValue))
    },
    _validateOptions(options) {
        options.wholeRange = this._validateVisualRange(options.wholeRange);
        options.visualRange = options._customVisualRange = this._validateVisualRange(options._customVisualRange);
        this._setVisualRange(options._customVisualRange)
    },
    validate() {
        var options = this._options;
        var dataType = this.isArgumentAxis ? options.argumentType : options.valueType;
        var parser = dataType ? getParser(dataType) : function(unit) {
            return unit
        };
        this.parser = parser;
        options.dataType = dataType;
        this._validateOptions(options)
    },
    resetVisualRange(isSilent) {
        this._seriesData.minVisible = this._seriesData.min;
        this._seriesData.maxVisible = this._seriesData.max;
        this.handleZooming([null, null], {
            start: !!isSilent,
            end: !!isSilent
        })
    },
    _setVisualRange(visualRange, allowPartialUpdate) {
        var range = this.adjustRange(getVizRangeObject(visualRange));
        if (allowPartialUpdate) {
            isDefined(range.startValue) && (this._viewport.startValue = range.startValue);
            isDefined(range.endValue) && (this._viewport.endValue = range.endValue)
        } else {
            this._viewport = range
        }
    },
    _applyZooming(visualRange, allowPartialUpdate) {
        this._resetVisualRangeOption();
        this._setVisualRange(visualRange, allowPartialUpdate);
        var viewPort = this.getViewport();
        this._seriesData.userBreaks = this._getScaleBreaks(this._options, {
            minVisible: viewPort.startValue,
            maxVisible: viewPort.endValue
        }, this._series, this.isArgumentAxis);
        this._translator.updateBusinessRange(this._getViewportRange())
    },
    getZoomStartEventArg(event, actionType) {
        return {
            axis: this,
            range: this.visualRange(),
            cancel: false,
            event: event,
            actionType: actionType
        }
    },
    _getZoomEndEventArg(previousRange, event, actionType, zoomFactor, shift) {
        var newRange = this.visualRange();
        return {
            axis: this,
            previousRange: previousRange,
            range: newRange,
            cancel: false,
            event: event,
            actionType: actionType,
            zoomFactor: zoomFactor,
            shift: shift,
            rangeStart: newRange.startValue,
            rangeEnd: newRange.endValue
        }
    },
    getZoomBounds() {
        var wholeRange = getVizRangeObject(this._options.wholeRange);
        var range = this.getTranslator().getBusinessRange();
        var secondPriorityRange = {
            startValue: getZoomBoundValue(this._initRange.startValue, range.min),
            endValue: getZoomBoundValue(this._initRange.endValue, range.max)
        };
        return {
            startValue: getZoomBoundValue(wholeRange.startValue, secondPriorityRange.startValue),
            endValue: getZoomBoundValue(wholeRange.endValue, secondPriorityRange.endValue)
        }
    },
    setInitRange() {
        this._initRange = {};
        if (0 === Object.keys(this._options.wholeRange || {}).length) {
            this._initRange = this.getZoomBounds()
        }
    },
    _resetVisualRangeOption() {
        this._options._customVisualRange = {}
    },
    getTemplatesGroups() {
        var ticks = this._majorTicks;
        if (ticks) {
            return this._majorTicks.map(tick => tick.templateContainer).filter(item => isDefined(item))
        } else {
            return []
        }
    },
    setCustomVisualRange(range) {
        this._options._customVisualRange = range
    },
    visualRange() {
        var args = arguments;
        var visualRange;
        if (0 === args.length) {
            var adjustedRange = this._getAdjustedBusinessRange();
            var startValue = adjustedRange.minVisible;
            var endValue = adjustedRange.maxVisible;
            if (this._options.type === constants.discrete) {
                var _startValue, _endValue;
                startValue = null !== (_startValue = startValue) && void 0 !== _startValue ? _startValue : adjustedRange.categories[0];
                endValue = null !== (_endValue = endValue) && void 0 !== _endValue ? _endValue : adjustedRange.categories[adjustedRange.categories.length - 1];
                return {
                    startValue: startValue,
                    endValue: endValue,
                    categories: getCategoriesInfo(adjustedRange.categories, startValue, endValue).categories
                }
            }
            return {
                startValue: startValue,
                endValue: endValue
            }
        } else if (_isArray(args[0])) {
            visualRange = args[0]
        } else if (isPlainObject(args[0])) {
            visualRange = extend({}, args[0])
        } else {
            visualRange = [args[0], args[1]]
        }
        var zoomResults = this.handleZooming(visualRange, args[1]);
        if (!zoomResults.isPrevented) {
            this._visualRange(this, zoomResults)
        }
    },
    handleZooming(visualRange, preventEvents, domEvent, action) {
        preventEvents = preventEvents || {};
        if (isDefined(visualRange)) {
            visualRange = this._validateVisualRange(visualRange);
            visualRange.action = action
        }
        var zoomStartEvent = this.getZoomStartEventArg(domEvent, action);
        var previousRange = zoomStartEvent.range;
        !preventEvents.start && this._eventTrigger("zoomStart", zoomStartEvent);
        var zoomResults = {
            isPrevented: zoomStartEvent.cancel,
            skipEventRising: preventEvents.skipEventRising,
            range: visualRange || zoomStartEvent.range
        };
        if (!zoomStartEvent.cancel) {
            isDefined(visualRange) && this._applyZooming(visualRange, preventEvents.allowPartialUpdate);
            if (!isDefined(this._storedZoomEndParams)) {
                this._storedZoomEndParams = {
                    startRange: previousRange,
                    type: this.getOptions().type
                }
            }
            this._storedZoomEndParams.event = domEvent;
            this._storedZoomEndParams.action = action;
            this._storedZoomEndParams.prevent = !!preventEvents.end
        }
        return zoomResults
    },
    handleZoomEnd() {
        if (isDefined(this._storedZoomEndParams) && !this._storedZoomEndParams.prevent) {
            var previousRange = this._storedZoomEndParams.startRange;
            var domEvent = this._storedZoomEndParams.event;
            var action = this._storedZoomEndParams.action;
            var previousBusinessRange = {
                minVisible: previousRange.startValue,
                maxVisible: previousRange.endValue,
                categories: previousRange.categories
            };
            var typeIsNotChanged = this.getOptions().type === this._storedZoomEndParams.type;
            var shift = typeIsNotChanged ? adjust(this.getVisualRangeCenter() - this.getVisualRangeCenter(previousBusinessRange, false)) : NaN;
            var zoomFactor = typeIsNotChanged ? +(Math.round(this.getVisualRangeLength(previousBusinessRange) / (this.getVisualRangeLength() || 1) + "e+2") + "e-2") : NaN;
            var zoomEndEvent = this._getZoomEndEventArg(previousRange, domEvent, action, zoomFactor, shift);
            zoomEndEvent.cancel = this.checkZoomingLowerLimitOvercome(1 === zoomFactor ? "pan" : "zoom", zoomFactor).stopInteraction;
            this._eventTrigger("zoomEnd", zoomEndEvent);
            if (zoomEndEvent.cancel) {
                this._restorePreviousVisualRange(previousRange)
            }
            this._storedZoomEndParams = null
        }
    },
    _restorePreviousVisualRange(previousRange) {
        this._storedZoomEndParams = null;
        this._applyZooming(previousRange);
        this._visualRange(this, previousRange)
    },
    checkZoomingLowerLimitOvercome(actionType, zoomFactor, range) {
        var options = this._options;
        var translator = this._translator;
        var minZoom = options.minVisualRangeLength;
        var correctedRange = range;
        var visualRange;
        var isOvercoming = "zoom" === actionType && zoomFactor >= 1;
        var businessRange = translator.getBusinessRange();
        if (range) {
            visualRange = this.adjustRange(getVizRangeObject(range));
            visualRange = {
                minVisible: visualRange.startValue,
                maxVisible: visualRange.endValue,
                categories: businessRange.categories
            }
        }
        var beforeVisualRangeLength = this.getVisualRangeLength(businessRange);
        var afterVisualRangeLength = this.getVisualRangeLength(visualRange);
        if (isDefined(minZoom) || "discrete" === options.type) {
            minZoom = translator.convert(minZoom);
            if (visualRange && minZoom < beforeVisualRangeLength && minZoom >= afterVisualRangeLength) {
                correctedRange = getVizRangeObject(translator.getRangeByMinZoomValue(minZoom, visualRange));
                isOvercoming = false
            } else {
                isOvercoming &= minZoom > afterVisualRangeLength
            }
        } else {
            var canvasLength = this._translator.canvasLength;
            var fullRange = {
                minVisible: businessRange.min,
                maxVisible: businessRange.max,
                categories: businessRange.categories
            };
            isOvercoming &= this.getVisualRangeLength(fullRange) / canvasLength >= afterVisualRangeLength
        }
        return {
            stopInteraction: !!isOvercoming,
            correctedRange: correctedRange
        }
    },
    isExtremePosition(isMax) {
        var extremeDataValue;
        var seriesData;
        if ("discrete" === this._options.type) {
            seriesData = this._translator.getBusinessRange();
            extremeDataValue = isMax ? seriesData.categories[seriesData.categories.length - 1] : seriesData.categories[0]
        } else {
            seriesData = this.getZoomBounds();
            extremeDataValue = isMax ? seriesData.endValue : seriesData.startValue
        }
        var translator = this.getTranslator();
        var extremePoint = translator.translate(extremeDataValue);
        var visualRange = this.visualRange();
        var visualRangePoint = isMax ? translator.translate(visualRange.endValue) : translator.translate(visualRange.startValue);
        return _abs(visualRangePoint - extremePoint) < SCROLL_THRESHOLD
    },
    getViewport() {
        return this._viewport
    },
    getFullTicks: function() {
        var majors = this._majorTicks || [];
        if (this._options.type === constants.discrete) {
            return convertTicksToValues(majors)
        } else {
            return convertTicksToValues(majors.concat(this._minorTicks, this._boundaryTicks)).sort((function(a, b) {
                return valueOf(a) - valueOf(b)
            }))
        }
    },
    measureLabels: function(canvas, withIndents) {
        var that = this;
        var options = that._options;
        var widthAxis = options.visible ? options.width : 0;
        var ticks;
        var indent = withIndents ? options.label.indentFromAxis + .5 * options.tick.length : 0;
        var tickInterval;
        var viewportRange = that._getViewportRange();
        if (viewportRange.isEmpty() || !options.label.visible || !that._axisElementsGroup) {
            return {
                height: widthAxis,
                width: widthAxis,
                x: 0,
                y: 0
            }
        }
        if (that._majorTicks) {
            ticks = convertTicksToValues(that._majorTicks)
        } else {
            that.updateCanvas(canvas);
            ticks = that._createTicksAndLabelFormat(viewportRange, _noop);
            tickInterval = ticks.tickInterval;
            ticks = ticks.ticks
        }
        var maxText = ticks.reduce((function(prevLabel, tick, index) {
            var label = that.formatLabel(tick, options.label, viewportRange, void 0, tickInterval, ticks);
            if (prevLabel.length < label.length) {
                return label
            } else {
                return prevLabel
            }
        }), that.formatLabel(ticks[0], options.label, viewportRange, void 0, tickInterval, ticks));
        var text = that._renderer.text(maxText, 0, 0).css(that._textFontStyles).attr(that._textOptions).append(that._renderer.root);
        var box = text.getBBox();
        text.remove();
        return {
            x: box.x,
            y: box.y,
            width: box.width + indent,
            height: box.height + indent
        }
    },
    _setLabelsPlacement: function() {
        if (!this._options.label.visible) {
            return
        }
        var labelOpt = this._options.label;
        var displayMode = this._validateDisplayMode(labelOpt.displayMode);
        var overlappingMode = this._validateOverlappingMode(labelOpt.overlappingBehavior, displayMode);
        var ignoreOverlapping = "none" === overlappingMode || "ignore" === overlappingMode;
        var behavior = {
            rotationAngle: labelOpt.rotationAngle,
            staggeringSpacing: labelOpt.staggeringSpacing
        };
        var notRecastStep;
        var boxes = this._majorTicks.map((function(tick) {
            return tick.labelBBox
        }));
        var step = this._getStep(boxes);
        switch (displayMode) {
            case ROTATE:
                if (ignoreOverlapping) {
                    notRecastStep = true;
                    step = 1
                }
                this._applyLabelMode(displayMode, step, boxes, labelOpt, notRecastStep);
                break;
            case "stagger":
                if (ignoreOverlapping) {
                    step = 2
                }
                this._applyLabelMode(displayMode, _max(step, 2), boxes, labelOpt);
                break;
            default:
                this._applyLabelOverlapping(boxes, overlappingMode, step, behavior)
        }
    },
    _applyLabelOverlapping: function(boxes, mode, step, behavior) {
        var labelOpt = this._options.label;
        var majorTicks = this._majorTicks;
        if ("none" === mode || "ignore" === mode) {
            return
        }
        if (step > 1 && boxes.some((function(box, index, array) {
                if (0 === index) {
                    return false
                }
                return constants.areLabelsOverlap(box, array[index - 1], labelOpt.minSpacing, labelOpt.alignment)
            }))) {
            this._applyLabelMode(mode, step, boxes, behavior)
        }
        this._checkBoundedLabelsOverlapping(majorTicks, boxes, mode);
        this._checkShiftedLabels(majorTicks, boxes, labelOpt.minSpacing, labelOpt.alignment)
    },
    _applyLabelMode: function(mode, step, boxes, behavior, notRecastStep) {
        var majorTicks = this._majorTicks;
        var labelOpt = this._options.label;
        var angle = behavior.rotationAngle;
        var labelHeight;
        var alignment;
        var func;
        switch (mode) {
            case ROTATE:
                if (!labelOpt.userAlignment) {
                    alignment = angle < 0 ? RIGHT : LEFT;
                    if (angle % 90 === 0) {
                        alignment = CENTER
                    }
                }
                step = notRecastStep ? step : this._getStep(boxes, angle);
                func = function(tick) {
                    var contentContainer = tick.getContentContainer();
                    if (!contentContainer) {
                        return
                    }
                    contentContainer.rotate(angle);
                    tick.labelRotationAngle = angle;
                    alignment && (tick.labelAlignment = alignment)
                };
                updateLabels(majorTicks, step, func);
                break;
            case "stagger":
                labelHeight = this._getMaxLabelHeight(boxes, behavior.staggeringSpacing);
                func = function(tick, index) {
                    if (index / (step - 1) % 2 !== 0) {
                        tick.labelOffset = labelHeight
                    }
                };
                updateLabels(majorTicks, step - 1, func);
                break;
            case "auto":
            case "_auto":
                if (2 === step) {
                    this._applyLabelMode("stagger", step, boxes, behavior)
                } else {
                    this._applyLabelMode(ROTATE, step, boxes, {
                        rotationAngle: getOptimalAngle(boxes, labelOpt)
                    })
                }
                break;
            default:
                updateLabels(majorTicks, step)
        }
    },
    getMarkerTrackers: _noop,
    _drawDateMarkers: _noop,
    _adjustDateMarkers: _noop,
    coordsIn: _noop,
    areCoordsOutsideAxis: _noop,
    _getSkippedCategory: _noop,
    _initAxisPositions: _noop,
    _drawTitle: _noop,
    _updateTitleCoords: _noop,
    _adjustConstantLineLabels: _noop,
    _createTranslator: function() {
        return new Translator2D({}, {}, {})
    },
    _updateTranslator: function() {
        var translator = this._translator;
        translator.update(translator.getBusinessRange(), this._canvas || {}, this._getTranslatorOptions())
    },
    _getTranslatorOptions: function() {
        var _options$workWeek2, _options$breakStyle$w, _options$breakStyle;
        var options = this._options;
        return {
            isHorizontal: this._isHorizontal,
            shiftZeroValue: !this.isArgumentAxis,
            interval: options.semiDiscreteInterval,
            firstDayOfWeek: null === (_options$workWeek2 = options.workWeek) || void 0 === _options$workWeek2 ? void 0 : _options$workWeek2[0],
            stick: this._getStick(),
            breaksSize: null !== (_options$breakStyle$w = null === (_options$breakStyle = options.breakStyle) || void 0 === _options$breakStyle ? void 0 : _options$breakStyle.width) && void 0 !== _options$breakStyle$w ? _options$breakStyle$w : 0
        }
    },
    getVisibleArea() {
        var canvas = this._getCanvasStartEnd();
        return [canvas.start, canvas.end].sort((a, b) => a - b)
    },
    _getCanvasStartEnd: function() {
        var isHorizontal = this._isHorizontal;
        var canvas = this._canvas || {};
        var invert = this._translator.getBusinessRange().invert;
        var coords = isHorizontal ? [canvas.left, canvas.width - canvas.right] : [canvas.height - canvas.bottom, canvas.top];
        invert && coords.reverse();
        return {
            start: coords[0],
            end: coords[1]
        }
    },
    _getScreenDelta: function() {
        var canvas = this._getCanvasStartEnd();
        var breaks = this._seriesData ? this._seriesData.breaks || [] : [];
        var breaksLength = breaks.length;
        var screenDelta = _abs(canvas.start - canvas.end);
        return screenDelta - (breaksLength ? breaks[breaksLength - 1].cumulativeWidth : 0)
    },
    _getScaleBreaks: function() {
        return []
    },
    _filterBreaks: function() {
        return []
    },
    _adjustTitle: _noop,
    _checkTitleOverflow: _noop,
    getSpiderTicks: _noop,
    setSpiderTicks: _noop,
    _checkBoundedLabelsOverlapping: _noop,
    _checkShiftedLabels: _noop,
    drawScaleBreaks: _noop,
    _visualRange: _noop,
    _rotateConstantLine: _noop,
    applyVisualRangeSetter(visualRangeSetter) {
        this._visualRange = visualRangeSetter
    },
    getCategoriesSorter(argCategories) {
        var sort;
        if (this.isArgumentAxis) {
            sort = argCategories
        } else {
            var categoriesSortingMethod = this._options.categoriesSortingMethod;
            sort = null !== categoriesSortingMethod && void 0 !== categoriesSortingMethod ? categoriesSortingMethod : this._options.categories
        }
        return sort
    },
    _getAdjustedBusinessRange() {
        return this.adjustViewport(this._translator.getBusinessRange())
    }
};
