/**
 * DevExtreme (esm/viz/chart_components/multi_axes_synchronizer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    debug
} from "../../core/utils/console";
import {
    isDefined,
    isNumeric
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    getLogExt,
    raiseToExt
} from "../core/utils";
import {
    adjust
} from "../../core/utils/math";
var _math = Math;
var _floor = _math.floor;
var _max = _math.max;
var _abs = _math.abs;

function getValueAxesPerPanes(valueAxes) {
    var result = {};
    valueAxes.forEach(axis => {
        var pane = axis.pane;
        if (!result[pane]) {
            result[pane] = []
        }
        result[pane].push(axis)
    });
    return result
}
var linearConverter = br => ({
    transform: function(v, b) {
        return adjust(getLogExt(v, b, br.allowNegatives, br.linearThreshold))
    },
    getTicks: function(interval, tickValues, base) {
        var ticks = [];
        var tick = this.transform(tickValues[0], base);
        while (ticks.length < tickValues.length) {
            ticks.push(tick);
            tick = adjust(tick + interval)
        }
        return ticks
    }
});
var logConverter = br => ({
    transform: function(v, b) {
        return adjust(raiseToExt(v, b, br.allowNegatives, br.linearThreshold))
    },
    getTicks: function(interval, tickValues, base) {
        var ticks = [];
        var tick;
        for (var i = 0; i < tickValues.length; i += 1) {
            tick = this.transform(tickValues[i], base);
            ticks.push(tick)
        }
        return ticks
    }
});

function convertAxisInfo(axisInfo, converter) {
    if (!axisInfo.isLogarithmic) {
        return
    }
    var base = axisInfo.logarithmicBase;
    var tickValues = axisInfo.tickValues;
    axisInfo.minValue = converter.transform(axisInfo.minValue, base);
    axisInfo.oldMinValue = converter.transform(axisInfo.oldMinValue, base);
    axisInfo.maxValue = converter.transform(axisInfo.maxValue, base);
    axisInfo.oldMaxValue = converter.transform(axisInfo.oldMaxValue, base);
    axisInfo.tickInterval = _math.round(axisInfo.tickInterval);
    if (axisInfo.tickInterval < 1) {
        axisInfo.tickInterval = 1
    }
    var ticks = converter.getTicks(axisInfo.tickInterval, tickValues, base);
    ticks.tickInterval = axisInfo.tickInterval;
    axisInfo.tickValues = ticks
}

function populateAxesInfo(axes) {
    return axes.reduce((function(result, axis) {
        var ticksValues = axis.getTicksValues();
        var majorTicks = ticksValues.majorTicksValues;
        var options = axis.getOptions();
        var businessRange = axis.getTranslator().getBusinessRange();
        var visibleArea = axis.getVisibleArea();
        var axisInfo;
        var tickInterval = axis._tickInterval;
        var synchronizedValue = options.synchronizedValue;
        var action = axis.getViewport().action;
        if (majorTicks && majorTicks.length > 0 && isNumeric(majorTicks[0]) && "discrete" !== options.type && !businessRange.isEmpty() && !(businessRange.breaks && businessRange.breaks.length) && "zoom" !== action && "pan" !== action) {
            axis.applyMargins();
            var startValue = axis.getTranslator().from(visibleArea[0]);
            var endValue = axis.getTranslator().from(visibleArea[1]);
            var minValue = startValue < endValue ? startValue : endValue;
            var maxValue = startValue < endValue ? endValue : startValue;
            if (minValue === maxValue && isDefined(synchronizedValue)) {
                tickInterval = _abs(majorTicks[0] - synchronizedValue) || 1;
                minValue = majorTicks[0] - tickInterval;
                maxValue = majorTicks[0] + tickInterval
            }
            axisInfo = {
                axis: axis,
                isLogarithmic: "logarithmic" === options.type,
                logarithmicBase: businessRange.base,
                tickValues: majorTicks,
                minorValues: ticksValues.minorTicksValues,
                minorTickInterval: axis._minorTickInterval,
                minValue: minValue,
                oldMinValue: minValue,
                maxValue: maxValue,
                oldMaxValue: maxValue,
                inverted: businessRange.invert,
                tickInterval: tickInterval,
                synchronizedValue: synchronizedValue
            };
            convertAxisInfo(axisInfo, linearConverter(axis.getTranslator().getBusinessRange()));
            result.push(axisInfo)
        }
        return result
    }), [])
}

function updateTickValues(axesInfo) {
    var maxTicksCount = axesInfo.reduce((max, axisInfo) => _max(max, axisInfo.tickValues.length), 0);
    axesInfo.forEach(axisInfo => {
        var ticksMultiplier;
        var ticksCount;
        var additionalStartTicksCount = 0;
        var synchronizedValue = axisInfo.synchronizedValue;
        var tickValues = axisInfo.tickValues;
        var tickInterval = axisInfo.tickInterval;
        if (isDefined(synchronizedValue)) {
            axisInfo.baseTickValue = axisInfo.invertedBaseTickValue = synchronizedValue;
            axisInfo.tickValues = [axisInfo.baseTickValue]
        } else {
            if (tickValues.length > 1 && tickInterval) {
                ticksMultiplier = _floor((maxTicksCount + 1) / tickValues.length);
                ticksCount = ticksMultiplier > 1 ? _floor((maxTicksCount + 1) / ticksMultiplier) : maxTicksCount;
                additionalStartTicksCount = _floor((ticksCount - tickValues.length) / 2);
                while (additionalStartTicksCount > 0 && 0 !== tickValues[0]) {
                    tickValues.unshift(adjust(tickValues[0] - tickInterval));
                    additionalStartTicksCount--
                }
                while (tickValues.length < ticksCount) {
                    tickValues.push(adjust(tickValues[tickValues.length - 1] + tickInterval))
                }
                axisInfo.tickInterval = tickInterval / ticksMultiplier
            }
            axisInfo.baseTickValue = tickValues[0];
            axisInfo.invertedBaseTickValue = tickValues[tickValues.length - 1]
        }
    })
}

function getAxisRange(axisInfo) {
    return axisInfo.maxValue - axisInfo.minValue || 1
}

function getMainAxisInfo(axesInfo) {
    for (var i = 0; i < axesInfo.length; i++) {
        if (!axesInfo[i].stubData) {
            return axesInfo[i]
        }
    }
    return null
}

function correctMinMaxValues(axesInfo) {
    var mainAxisInfo = getMainAxisInfo(axesInfo);
    var mainAxisInfoTickInterval = mainAxisInfo.tickInterval;
    axesInfo.forEach(axisInfo => {
        var scale;
        var move;
        var mainAxisBaseValueOffset;
        var valueFromAxisInfo;
        if (axisInfo !== mainAxisInfo) {
            if (mainAxisInfoTickInterval && axisInfo.tickInterval) {
                if (axisInfo.stubData && isDefined(axisInfo.synchronizedValue)) {
                    axisInfo.oldMinValue = axisInfo.minValue = axisInfo.baseTickValue - (mainAxisInfo.baseTickValue - mainAxisInfo.minValue) / mainAxisInfoTickInterval * axisInfo.tickInterval;
                    axisInfo.oldMaxValue = axisInfo.maxValue = axisInfo.baseTickValue - (mainAxisInfo.baseTickValue - mainAxisInfo.maxValue) / mainAxisInfoTickInterval * axisInfo.tickInterval
                }
                scale = mainAxisInfoTickInterval / getAxisRange(mainAxisInfo) / axisInfo.tickInterval * getAxisRange(axisInfo);
                axisInfo.maxValue = axisInfo.minValue + getAxisRange(axisInfo) / scale
            }
            if (mainAxisInfo.inverted && !axisInfo.inverted || !mainAxisInfo.inverted && axisInfo.inverted) {
                mainAxisBaseValueOffset = mainAxisInfo.maxValue - mainAxisInfo.invertedBaseTickValue
            } else {
                mainAxisBaseValueOffset = mainAxisInfo.baseTickValue - mainAxisInfo.minValue
            }
            valueFromAxisInfo = getAxisRange(axisInfo);
            move = (mainAxisBaseValueOffset / getAxisRange(mainAxisInfo) - (axisInfo.baseTickValue - axisInfo.minValue) / valueFromAxisInfo) * valueFromAxisInfo;
            axisInfo.minValue -= move;
            axisInfo.maxValue -= move
        }
    })
}

function calculatePaddings(axesInfo) {
    var minPadding;
    var maxPadding;
    var startPadding = 0;
    var endPadding = 0;
    axesInfo.forEach(axisInfo => {
        var inverted = axisInfo.inverted;
        minPadding = axisInfo.minValue > axisInfo.oldMinValue ? (axisInfo.minValue - axisInfo.oldMinValue) / getAxisRange(axisInfo) : 0;
        maxPadding = axisInfo.maxValue < axisInfo.oldMaxValue ? (axisInfo.oldMaxValue - axisInfo.maxValue) / getAxisRange(axisInfo) : 0;
        startPadding = _max(startPadding, inverted ? maxPadding : minPadding);
        endPadding = _max(endPadding, inverted ? minPadding : maxPadding)
    });
    return {
        start: startPadding,
        end: endPadding
    }
}

function correctMinMaxValuesByPaddings(axesInfo, paddings) {
    axesInfo.forEach(info => {
        var range = getAxisRange(info);
        var inverted = info.inverted;
        info.minValue = adjust(info.minValue - paddings[inverted ? "end" : "start"] * range);
        info.maxValue = adjust(info.maxValue + paddings[inverted ? "start" : "end"] * range)
    })
}

function updateTickValuesIfSynchronizedValueUsed(axesInfo) {
    var hasSynchronizedValue = false;
    axesInfo.forEach(info => {
        hasSynchronizedValue = hasSynchronizedValue || isDefined(info.synchronizedValue)
    });
    axesInfo.forEach(info => {
        var tickInterval = info.tickInterval;
        var tickValues = info.tickValues;
        var maxValue = info.maxValue;
        var minValue = info.minValue;
        var tick;
        if (hasSynchronizedValue && tickInterval) {
            while ((tick = adjust(tickValues[0] - tickInterval)) >= minValue) {
                tickValues.unshift(tick)
            }
            tick = tickValues[tickValues.length - 1];
            while ((tick = adjust(tick + tickInterval)) <= maxValue) {
                tickValues.push(tick)
            }
        }
        while (tickValues[0] + tickInterval / 10 < minValue) {
            tickValues.shift()
        }
        while (tickValues[tickValues.length - 1] - tickInterval / 10 > maxValue) {
            tickValues.pop()
        }
    })
}

function applyMinMaxValues(axesInfo) {
    axesInfo.forEach(info => {
        var axis = info.axis;
        var range = axis.getTranslator().getBusinessRange();
        if (range.min === range.minVisible) {
            range.min = info.minValue
        }
        if (range.max === range.maxVisible) {
            range.max = info.maxValue
        }
        range.minVisible = info.minValue;
        range.maxVisible = info.maxValue;
        if (range.min > range.minVisible) {
            range.min = range.minVisible
        }
        if (range.max < range.maxVisible) {
            range.max = range.maxVisible
        }
        axis.getTranslator().updateBusinessRange(range);
        axis.setTicks({
            majorTicks: info.tickValues,
            minorTicks: info.minorValues
        })
    })
}

function correctAfterSynchronize(axesInfo) {
    var invalidAxisInfo = [];
    var correctValue;
    axesInfo.forEach(info => {
        if (info.oldMaxValue - info.oldMinValue === 0) {
            invalidAxisInfo.push(info)
        } else if (!isDefined(correctValue) && !isDefined(info.synchronizedValue)) {
            correctValue = _abs((info.maxValue - info.minValue) / (info.tickValues[_floor(info.tickValues.length / 2)] - info.minValue || info.maxValue))
        }
    });
    if (!isDefined(correctValue)) {
        return
    }
    invalidAxisInfo.forEach(info => {
        var firstTick = info.tickValues[0];
        var correctedTick = firstTick * correctValue;
        if (firstTick > 0) {
            info.maxValue = correctedTick;
            info.minValue = 0
        } else if (firstTick < 0) {
            info.minValue = correctedTick;
            info.maxValue = 0
        }
    })
}

function updateMinorTicks(axesInfo) {
    axesInfo.forEach((function(axisInfo) {
        if (!axisInfo.minorTickInterval) {
            return
        }
        var ticks = [];
        var interval = axisInfo.minorTickInterval;
        var tickCount = axisInfo.tickInterval / interval - 1;
        for (var i = 1; i < axisInfo.tickValues.length; i++) {
            var tick = axisInfo.tickValues[i - 1];
            for (var j = 0; j < tickCount; j++) {
                tick += interval;
                ticks.push(tick)
            }
        }
        axisInfo.minorValues = ticks
    }))
}

function correctPaddings(axesInfo, paddings) {
    return axesInfo.reduce((prev, info) => {
        var inverted = info.inverted;
        var {
            start: start,
            end: end
        } = info.axis.getCorrectedValuesToZero(info.minValue, info.maxValue);
        if (isDefined(start) || isDefined(end)) {
            return inverted ? {
                start: prev.start,
                end: Math.min(prev.end, end)
            } : {
                start: Math.min(prev.start, start),
                end: prev.end
            }
        }
        return prev
    }, paddings)
}
var multiAxesSynchronizer = {
    synchronize: function(valueAxes) {
        each(getValueAxesPerPanes(valueAxes), (function(_, axes) {
            var axesInfo;
            var paddings;
            if (axes.length > 1) {
                axesInfo = populateAxesInfo(axes);
                if (axesInfo.length < 2 || !getMainAxisInfo(axesInfo)) {
                    return
                }
                updateTickValues(axesInfo);
                correctMinMaxValues(axesInfo);
                paddings = calculatePaddings(axesInfo);
                paddings = correctPaddings(axesInfo, paddings);
                correctMinMaxValuesByPaddings(axesInfo, paddings);
                correctAfterSynchronize(axesInfo);
                updateTickValuesIfSynchronizedValueUsed(axesInfo);
                updateMinorTicks(axesInfo);
                axesInfo.forEach(info => {
                    convertAxisInfo(info, logConverter(info.axis.getTranslator().getBusinessRange()))
                });
                applyMinMaxValues(axesInfo)
            }
        }))
    }
};
export default multiAxesSynchronizer;
