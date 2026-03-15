/**
 * DevExtreme (esm/viz/translators/interval_translator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isNumeric as isNumber,
    isDefined
} from "../../core/utils/type";
import dateUtils from "../../core/utils/date";
var floor = Math.floor;
import {
    adjust
} from "../../core/utils/math";
export default {
    _intervalize: function(value, interval) {
        if (!isDefined(value)) {
            return
        }
        if ("datetime" === this._businessRange.dataType) {
            if (isNumber(value)) {
                value = new Date(value)
            } else {
                value = new Date(value.getTime())
            }
            value = dateUtils.correctDateWithUnitBeginning(value, interval, null, this._options.firstDayOfWeek)
        } else {
            value = adjust(floor(adjust(value / interval)) * interval, interval)
        }
        return value
    },
    translate: function(bp, direction, interval) {
        var specialValue = this.translateSpecialCase(bp);
        if (isDefined(specialValue)) {
            return Math.round(specialValue)
        }
        interval = interval || this._options.interval;
        if (!this.isValid(bp, interval)) {
            return null
        }
        return this.to(bp, direction, interval)
    },
    getInterval: function() {
        return Math.round(this._canvasOptions.ratioOfCanvasRange * (this._businessRange.interval || Math.abs(this._canvasOptions.rangeMax - this._canvasOptions.rangeMin)))
    },
    zoom: function() {},
    getMinScale: function() {},
    getScale: function() {},
    _parse: function(value) {
        return "datetime" === this._businessRange.dataType ? new Date(value) : Number(value)
    },
    fromValue: function(value) {
        return this._parse(value)
    },
    toValue: function(value) {
        return this._parse(value)
    },
    isValid: function(value, interval) {
        var co = this._canvasOptions;
        var rangeMin = co.rangeMin;
        var rangeMax = co.rangeMax;
        interval = interval || this._options.interval;
        if (null === value || isNaN(value)) {
            return false
        }
        value = "datetime" === this._businessRange.dataType && isNumber(value) ? new Date(value) : value;
        if (interval !== this._options.interval) {
            rangeMin = this._intervalize(rangeMin, interval);
            rangeMax = this._intervalize(rangeMax, interval)
        }
        if (value.valueOf() < rangeMin || value.valueOf() >= dateUtils.addInterval(rangeMax, interval)) {
            return false
        }
        return true
    },
    to: function(bp, direction, interval) {
        interval = interval || this._options.interval;
        var v1 = this._intervalize(bp, interval);
        var v2 = dateUtils.addInterval(v1, interval);
        var res = this._to(v1);
        var p2 = this._to(v2);
        if (!direction) {
            res = floor((res + p2) / 2)
        } else if (direction > 0) {
            res = p2
        }
        return res
    },
    _to: function(value) {
        var co = this._canvasOptions;
        var rMin = co.rangeMinVisible;
        var rMax = co.rangeMaxVisible;
        var offset = value - rMin;
        if (value < rMin) {
            offset = 0
        } else if (value > rMax) {
            offset = dateUtils.addInterval(rMax, this._options.interval) - rMin
        }
        return this._conversionValue(this._calculateProjection(offset * this._canvasOptions.ratioOfCanvasRange))
    },
    from: function(position, direction) {
        var origInterval = this._options.interval;
        var interval = origInterval;
        var co = this._canvasOptions;
        var rMin = co.rangeMinVisible;
        var rMax = co.rangeMaxVisible;
        var value;
        if ("datetime" === this._businessRange.dataType) {
            interval = dateUtils.dateToMilliseconds(origInterval)
        }
        value = this._calculateUnProjection((position - this._canvasOptions.startPoint) / this._canvasOptions.ratioOfCanvasRange);
        value = this._intervalize(dateUtils.addInterval(value, interval / 2, direction > 0), origInterval);
        if (value < rMin) {
            value = rMin
        } else if (value > rMax) {
            value = rMax
        }
        return value
    },
    _add: function() {
        return NaN
    },
    isValueProlonged: true
};
