/**
 * DevExtreme (esm/ui/widget/selectors.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
var focusableFn = function(element, tabIndex) {
    if (!visible(element)) {
        return false
    }
    var nodeName = element.nodeName.toLowerCase();
    var isTabIndexNotNaN = !isNaN(tabIndex);
    var isDisabled = element.disabled;
    var isDefaultFocus = /^(input|select|textarea|button|object|iframe)$/.test(nodeName);
    var isHyperlink = "a" === nodeName;
    var isFocusable = true;
    var isContentEditable = element.isContentEditable;
    if (isDefaultFocus || isContentEditable) {
        isFocusable = !isDisabled
    } else if (isHyperlink) {
        isFocusable = element.href || isTabIndexNotNaN
    } else {
        isFocusable = isTabIndexNotNaN
    }
    return isFocusable
};

function visible(element) {
    var $element = $(element);
    return $element.is(":visible") && "hidden" !== $element.css("visibility") && "hidden" !== $element.parents().css("visibility")
}
export var focusable = function(index, element) {
    return focusableFn(element, $(element).attr("tabIndex"))
};
export var tabbable = function(index, element) {
    var tabIndex = $(element).attr("tabIndex");
    return (isNaN(tabIndex) || tabIndex >= 0) && focusableFn(element, tabIndex)
};
export var focused = function($element) {
    var element = $($element).get(0);
    return domAdapter.getActiveElement() === element
};
