/**
 * DevExtreme (esm/viz/translators/category_translator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
import {
    adjust
} from "../../core/utils/math";
var round = Math.round;

function getValue(value) {
    return value
}
export default {
    translate: function(category, directionOffset) {
        var canvasOptions = this._canvasOptions;
        var categoryIndex = this._categoriesToPoints[null === category || void 0 === category ? void 0 : category.valueOf()];
        var specialValue = this.translateSpecialCase(category);
        var startPointIndex = canvasOptions.startPointIndex || 0;
        var stickInterval = this._options.stick ? 0 : .5;
        if (isDefined(specialValue)) {
            return round(specialValue)
        }
        if (!categoryIndex && 0 !== categoryIndex) {
            return null
        }
        directionOffset = directionOffset || 0;
        var stickDelta = categoryIndex + stickInterval - startPointIndex + .5 * directionOffset;
        return round(this._calculateProjection(canvasOptions.interval * stickDelta))
    },
    getInterval: function() {
        return this._canvasOptions.interval
    },
    getEventScale: function(zoomEvent) {
        var scale = zoomEvent.deltaScale || 1;
        return 1 - (1 - scale) / (.75 + this.visibleCategories.length / this._categories.length)
    },
    zoom: function(translate, scale) {
        var categories = this._categories;
        var canvasOptions = this._canvasOptions;
        var stick = this._options.stick;
        var invert = canvasOptions.invert;
        var interval = canvasOptions.interval * scale;
        var translateCategories = translate / interval;
        var visibleCount = (this.visibleCategories || []).length;
        var startCategoryIndex = parseInt((canvasOptions.startPointIndex || 0) + translateCategories + .5);
        var categoriesLength = parseInt(adjust(canvasOptions.canvasLength / interval) + (stick ? 1 : 0)) || 1;
        var endCategoryIndex;
        if (invert) {
            startCategoryIndex = parseInt((canvasOptions.startPointIndex || 0) + visibleCount - translateCategories + .5) - categoriesLength
        }
        if (startCategoryIndex < 0) {
            startCategoryIndex = 0
        }
        endCategoryIndex = startCategoryIndex + categoriesLength;
        if (endCategoryIndex > categories.length) {
            endCategoryIndex = categories.length;
            startCategoryIndex = endCategoryIndex - categoriesLength;
            if (startCategoryIndex < 0) {
                startCategoryIndex = 0
            }
        }
        var newVisibleCategories = categories.slice(parseInt(startCategoryIndex), parseInt(endCategoryIndex));
        var newInterval = this._getDiscreteInterval(newVisibleCategories.length, canvasOptions);
        scale = newInterval / canvasOptions.interval;
        translate = this.translate(!invert ? newVisibleCategories[0] : newVisibleCategories[newVisibleCategories.length - 1]) * scale - (canvasOptions.startPoint + (stick ? 0 : newInterval / 2));
        return {
            min: newVisibleCategories[0],
            max: newVisibleCategories[newVisibleCategories.length - 1],
            translate: translate,
            scale: scale
        }
    },
    getMinScale: function(zoom) {
        var canvasOptions = this._canvasOptions;
        var categoriesLength = (this.visibleCategories || this._categories).length;
        categoriesLength += (parseInt(.1 * categoriesLength) || 1) * (zoom ? -2 : 2);
        return canvasOptions.canvasLength / (Math.max(categoriesLength, 1) * canvasOptions.interval)
    },
    getScale: function(min, max) {
        var canvasOptions = this._canvasOptions;
        var visibleArea = this.getCanvasVisibleArea();
        var stickOffset = !this._options.stick && 1;
        var minPoint = isDefined(min) ? this.translate(min, -stickOffset) : null;
        var maxPoint = isDefined(max) ? this.translate(max, +stickOffset) : null;
        if (null === minPoint) {
            minPoint = canvasOptions.invert ? visibleArea.max : visibleArea.min
        }
        if (null === maxPoint) {
            maxPoint = canvasOptions.invert ? visibleArea.min : visibleArea.max
        }
        return this.canvasLength / Math.abs(maxPoint - minPoint)
    },
    isValid: function(value) {
        return isDefined(value) ? this._categoriesToPoints[value.valueOf()] >= 0 : false
    },
    getCorrectValue: getValue,
    to: function(value, direction) {
        var canvasOptions = this._canvasOptions;
        var categoryIndex = this._categoriesToPoints[null === value || void 0 === value ? void 0 : value.valueOf()];
        var startPointIndex = canvasOptions.startPointIndex || 0;
        var stickDelta = categoryIndex + (this._options.stick ? 0 : .5) - startPointIndex + (this._businessRange.invert ? -1 : 1) * direction * .5;
        return round(this._calculateProjection(canvasOptions.interval * stickDelta))
    },
    from: function(position) {
        var direction = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 0;
        var canvasOptions = this._canvasOptions;
        var startPoint = canvasOptions.startPoint;
        var categories = this.visibleCategories || this._categories;
        var categoriesLength = categories.length;
        var stickInterval = this._options.stick ? .5 : 0;
        var result = round((position - startPoint) / canvasOptions.interval + stickInterval - .5 - .5 * direction);
        if (result >= categoriesLength) {
            result = categoriesLength - 1
        }
        if (result < 0) {
            result = 0
        }
        if (canvasOptions.invert) {
            result = categoriesLength - result - 1
        }
        return categories[result]
    },
    _add: function() {
        return NaN
    },
    toValue: getValue,
    isValueProlonged: true,
    getRangeByMinZoomValue(minZoom, visualRange) {
        var categories = this._categories;
        var minVisibleIndex = categories.indexOf(visualRange.minVisible);
        var maxVisibleIndex = categories.indexOf(visualRange.maxVisible);
        var startIndex = minVisibleIndex + minZoom - 1;
        var endIndex = maxVisibleIndex - minZoom + 1;
        if (categories[startIndex]) {
            return [visualRange.minVisible, categories[startIndex]]
        } else {
            return [categories[endIndex], visualRange.maxVisible]
        }
    }
};
