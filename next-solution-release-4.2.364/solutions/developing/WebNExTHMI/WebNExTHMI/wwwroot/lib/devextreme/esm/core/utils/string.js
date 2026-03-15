/**
 * DevExtreme (esm/core/utils/string.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isFunction,
    isString
} from "./type";
export var encodeHtml = function() {
    var encodeRegExp = [new RegExp("&", "g"), new RegExp('"', "g"), new RegExp("'", "g"), new RegExp("<", "g"), new RegExp(">", "g")];
    return function(str) {
        return String(str).replace(encodeRegExp[0], "&amp;").replace(encodeRegExp[1], "&quot;").replace(encodeRegExp[2], "&#39;").replace(encodeRegExp[3], "&lt;").replace(encodeRegExp[4], "&gt;")
    }
}();
var splitQuad = function(raw) {
    switch (typeof raw) {
        case "string":
            return raw.split(/\s+/, 4);
        case "object":
            return [raw.x || raw.h || raw.left, raw.y || raw.v || raw.top, raw.x || raw.h || raw.right, raw.y || raw.v || raw.bottom];
        case "number":
            return [raw];
        default:
            return raw
    }
};
export var quadToObject = function(raw) {
    var quad = splitQuad(raw);
    var left = parseInt(quad && quad[0], 10);
    var top = parseInt(quad && quad[1], 10);
    var right = parseInt(quad && quad[2], 10);
    var bottom = parseInt(quad && quad[3], 10);
    if (!isFinite(left)) {
        left = 0
    }
    if (!isFinite(top)) {
        top = left
    }
    if (!isFinite(right)) {
        right = left
    }
    if (!isFinite(bottom)) {
        bottom = top
    }
    return {
        top: top,
        right: right,
        bottom: bottom,
        left: left
    }
};
export function format(template) {
    for (var _len = arguments.length, values = new Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
        values[_key - 1] = arguments[_key]
    }
    if (isFunction(template)) {
        return template(...values)
    }
    values.forEach((value, index) => {
        if (isString(value)) {
            value = value.replace(/\$/g, "$$$$")
        }
        var placeholderReg = new RegExp("\\{" + index + "\\}", "gm");
        template = template.replace(placeholderReg, value)
    });
    return template
}
export var replaceAll = function(text, searchToken, replacementToken) {
    return text.replace(new RegExp("(" + (str = searchToken, (str + "").replace(/([+*?.[^\]$(){}><|=!:])/g, "\\$1")) + ")", "gi"), replacementToken);
    var str
};
export var isEmpty = function() {
    var SPACE_REGEXP = /\s/g;
    return function(text) {
        return !text || !text.replace(SPACE_REGEXP, "")
    }
}();
