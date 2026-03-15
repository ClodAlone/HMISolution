/**
 * DevExtreme (esm/viz/translators/range.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined,
    isDate,
    isFunction
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
var _isDefined = isDefined;
var _isDate = isDate;
var _isFunction = isFunction;
import {
    unique
} from "../core/utils";
var minSelector = "min";
var maxSelector = "max";
var minVisibleSelector = "minVisible";
var maxVisibleSelector = "maxVisible";
var baseSelector = "base";
var axisTypeSelector = "axisType";

function otherLessThan(thisValue, otherValue) {
    return otherValue < thisValue
}

function otherGreaterThan(thisValue, otherValue) {
    return otherValue > thisValue
}

function compareAndReplace(thisValue, otherValue, setValue, compare) {
    var otherValueDefined = _isDefined(otherValue);
    if (_isDefined(thisValue)) {
        if (otherValueDefined && compare(thisValue, otherValue)) {
            setValue(otherValue)
        }
    } else if (otherValueDefined) {
        setValue(otherValue)
    }
}
export var Range = function(range) {
    range && extend(this, range)
};
var _Range = Range;
_Range.prototype = {
    constructor: _Range,
    addRange: function(otherRange) {
        var that = this;
        var categories = that.categories;
        var otherCategories = otherRange.categories;
        var isDiscrete = "discrete" === that[axisTypeSelector];
        var compareAndReplaceByField = function(field, compare) {
            compareAndReplace(that[field], otherRange[field], (function(value) {
                that[field] = value
            }), compare)
        };
        var controlValuesByVisibleBounds = function(valueField, visibleValueField, compare) {
            compareAndReplace(that[valueField], that[visibleValueField], (function(value) {
                _isDefined(that[valueField]) && (that[valueField] = value)
            }), compare)
        };
        var checkField = function(field) {
            that[field] = that[field] || otherRange[field]
        };
        checkField("invert");
        checkField("containsConstantLine");
        checkField(axisTypeSelector);
        checkField("dataType");
        checkField("isSpacedMargin");
        if ("logarithmic" === that[axisTypeSelector]) {
            checkField(baseSelector)
        } else {
            that[baseSelector] = void 0
        }
        compareAndReplaceByField(minSelector, otherLessThan);
        compareAndReplaceByField(maxSelector, otherGreaterThan);
        if (isDiscrete) {
            checkField(minVisibleSelector);
            checkField(maxVisibleSelector)
        } else {
            compareAndReplaceByField(minVisibleSelector, otherLessThan);
            compareAndReplaceByField(maxVisibleSelector, otherGreaterThan)
        }
        compareAndReplaceByField("interval", otherLessThan);
        if (!isDiscrete) {
            controlValuesByVisibleBounds(minSelector, minVisibleSelector, otherLessThan);
            controlValuesByVisibleBounds(minSelector, maxVisibleSelector, otherLessThan);
            controlValuesByVisibleBounds(maxSelector, maxVisibleSelector, otherGreaterThan);
            controlValuesByVisibleBounds(maxSelector, minVisibleSelector, otherGreaterThan)
        }
        if (void 0 === categories) {
            that.categories = otherCategories
        } else {
            that.categories = otherCategories ? unique(categories.concat(otherCategories)) : categories
        }
        if ("logarithmic" === that[axisTypeSelector]) {
            checkField("allowNegatives");
            compareAndReplaceByField("linearThreshold", otherLessThan)
        }
        return that
    },
    isEmpty: function() {
        return (!_isDefined(this[minSelector]) || !_isDefined(this[maxSelector])) && (!this.categories || 0 === this.categories.length)
    },
    correctValueZeroLevel: function() {
        var that = this;
        if (_isDate(that[maxSelector]) || _isDate(that[minSelector])) {
            return that
        }

        function setZeroLevel(min, max) {
            that[min] < 0 && that[max] < 0 && (that[max] = 0);
            that[min] > 0 && that[max] > 0 && (that[min] = 0)
        }
        setZeroLevel(minSelector, maxSelector);
        setZeroLevel(minVisibleSelector, maxVisibleSelector);
        return that
    },
    sortCategories(sort) {
        if (false === sort || !this.categories) {
            return
        }
        if (Array.isArray(sort)) {
            var sortValues = sort.map(item => item.valueOf());
            var filteredSeriesCategories = this.categories.filter(item => -1 === sortValues.indexOf(item.valueOf()));
            this.categories = sort.concat(filteredSeriesCategories)
        } else {
            var notAFunction = !_isFunction(sort);
            if (notAFunction && "string" !== this.dataType) {
                sort = (a, b) => a.valueOf() - b.valueOf()
            } else if (notAFunction) {
                sort = false
            }
            sort && this.categories.sort(sort)
        }
    }
};
