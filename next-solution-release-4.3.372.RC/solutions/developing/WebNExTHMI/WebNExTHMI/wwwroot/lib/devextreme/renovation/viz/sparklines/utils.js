/**
 * DevExtreme (renovation/viz/sparklines/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.generateCustomizeTooltipCallback = exports.createAxis = void 0;
var _translator2d = require("../../../viz/translators/translator2d");
var _common = require("../../../core/utils/common");
var _type = require("../../../core/utils/type");

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var DEFAULT_LINE_SPACING = 2;
var createAxis = function(isHorizontal) {
    var translator = new _translator2d.Translator2D({}, {}, {
        shiftZeroValue: !isHorizontal,
        isHorizontal: !!isHorizontal
    });
    return {
        getTranslator: function() {
            return translator
        },
        update: function(range, canvas, options) {
            return translator.update(range, canvas, options)
        },
        getVisibleArea: function() {
            var visibleArea = translator.getCanvasVisibleArea();
            return [visibleArea.min, visibleArea.max]
        },
        visualRange: _common.noop,
        calculateInterval: _common.noop,
        getMarginOptions: function() {
            return {}
        }
    }
};
exports.createAxis = createAxis;
var generateDefaultCustomizeTooltipCallback = function(fontOptions, rtlEnabled) {
    var _ref = null !== fontOptions && void 0 !== fontOptions ? fontOptions : {},
        lineSpacing = _ref.lineSpacing,
        size = _ref.size;
    var lineHeight = (null !== lineSpacing && void 0 !== lineSpacing ? lineSpacing : DEFAULT_LINE_SPACING) + (null !== size && void 0 !== size ? size : 0);
    return function(customizeObject) {
        var _customizeObject$valu;
        var html = "";
        var vt = null !== (_customizeObject$valu = customizeObject.valueTexts) && void 0 !== _customizeObject$valu ? _customizeObject$valu : [];
        for (var i = 0; i < vt.length; i += 2) {
            html += "<tr><td>".concat(vt[i], "</td><td style='width: 15px'></td><td style='text-align: ").concat(rtlEnabled ? "left" : "right", "'>").concat(vt[i + 1], "</td></tr>")
        }
        return {
            html: "<table style='border-spacing:0px; line-height: ".concat(lineHeight, "px'>").concat(html, "</table>")
        }
    }
};
var generateCustomizeTooltipCallback = function(customizeTooltip, fontOptions, rtlEnabled) {
    var defaultCustomizeTooltip = generateDefaultCustomizeTooltipCallback(fontOptions, rtlEnabled);
    if ((0, _type.isFunction)(customizeTooltip)) {
        return function(customizeObject) {
            var _customizeTooltip$cal;
            var res = null !== (_customizeTooltip$cal = customizeTooltip.call(customizeObject, customizeObject)) && void 0 !== _customizeTooltip$cal ? _customizeTooltip$cal : {};
            if (!("html" in res) && !("text" in res)) {
                res = _extends({}, res, defaultCustomizeTooltip.call(customizeObject, customizeObject))
            }
            return res
        }
    }
    return defaultCustomizeTooltip
};
exports.generateCustomizeTooltipCallback = generateCustomizeTooltipCallback;
