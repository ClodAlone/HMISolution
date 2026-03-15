/**
 * DevExtreme (esm/core/utils/icon.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
var ICON_CLASS = "dx-icon";
var SVG_ICON_CLASS = "dx-svg-icon";
export var getImageSourceType = source => {
    if (!source || "string" !== typeof source) {
        return false
    }
    if (/^\s*<svg[^>]*>(.|\r?\n)*?<\/svg>\s*$/i.test(source)) {
        return "svg"
    }
    if (/data:.*base64|\.|[^<\s]\//.test(source)) {
        return "image"
    }
    if (/^[\w-_]+$/.test(source)) {
        return "dxIcon"
    }
    if (/^\s?([\w-_]\s?)+$/.test(source)) {
        return "fontIcon"
    }
    return false
};
export var getImageContainer = source => {
    switch (getImageSourceType(source)) {
        case "image":
            return $("<img>").attr("src", source).addClass(ICON_CLASS);
        case "fontIcon":
            return $("<i>").addClass("".concat(ICON_CLASS, " ").concat(source));
        case "dxIcon":
            return $("<i>").addClass("".concat(ICON_CLASS, " ").concat(ICON_CLASS, "-").concat(source));
        case "svg":
            return $("<i>").addClass("".concat(ICON_CLASS, " ").concat(SVG_ICON_CLASS)).append(source);
        default:
            return null
    }
};
