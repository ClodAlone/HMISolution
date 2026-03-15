/**
 * DevExtreme (esm/core/utils/dom.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import domAdapter from "../../core/dom_adapter";
import $ from "../../core/renderer";
import {
    each
} from "./iterator";
import {
    isDefined,
    isRenderer,
    isWindow
} from "./type";
import {
    getWindow
} from "./window";
var window = getWindow();
export var resetActiveElement = () => {
    var activeElement = domAdapter.getActiveElement();
    if (activeElement && activeElement !== domAdapter.getBody()) {
        var _activeElement$blur;
        null === (_activeElement$blur = activeElement.blur) || void 0 === _activeElement$blur ? void 0 : _activeElement$blur.call(activeElement)
    }
};
export var clearSelection = () => {
    var selection = window.getSelection();
    if (!selection) {
        return
    }
    if ("Caret" === selection.type) {
        return
    }
    if (selection.empty) {
        selection.empty()
    } else if (selection.removeAllRanges) {
        try {
            selection.removeAllRanges()
        } catch (e) {}
    }
};
export var closestCommonParent = (startTarget, endTarget) => {
    var $startTarget = $(startTarget);
    var $endTarget = $(endTarget);
    if ($startTarget[0] === $endTarget[0]) {
        return $startTarget[0]
    }
    var $startParents = $startTarget.parents();
    var $endParents = $endTarget.parents();
    var startingParent = Math.min($startParents.length, $endParents.length);
    for (var i = -startingParent; i < 0; i++) {
        if ($startParents.get(i) === $endParents.get(i)) {
            return $startParents.get(i)
        }
    }
};
export var extractTemplateMarkup = element => {
    element = $(element);
    var templateTag = element.length && element.filter((function() {
        var $node = $(this);
        return $node.is("script[type]") && $node.attr("type").indexOf("script") < 0
    }));
    if (templateTag.length) {
        return templateTag.eq(0).html()
    } else {
        element = $("<div>").append(element);
        return element.html()
    }
};
export var normalizeTemplateElement = element => {
    var $element = isDefined(element) && (element.nodeType || isRenderer(element)) ? $(element) : $("<div>").html(element).contents();
    if (1 === $element.length) {
        if ($element.is("script")) {
            $element = normalizeTemplateElement($element.html().trim())
        } else if ($element.is("table")) {
            $element = $element.children("tbody").contents()
        }
    }
    return $element
};
export var clipboardText = (event, text) => {
    var clipboard = event.originalEvent && event.originalEvent.clipboardData || window.clipboardData;
    if (!text) {
        return clipboard && clipboard.getData("Text")
    }
    clipboard && clipboard.setData("Text", text)
};
export var contains = (container, element) => {
    if (!element) {
        return false
    }
    if (domAdapter.isTextNode(element)) {
        element = element.parentNode
    }
    if (domAdapter.isDocument(container)) {
        return container.documentElement.contains(element)
    }
    if (isWindow(container)) {
        return contains(container.document, element)
    }
    return container.contains ? container.contains(element) : !!(element.compareDocumentPosition(container) & element.DOCUMENT_POSITION_CONTAINS)
};
export var createTextElementHiddenCopy = (element, text, options) => {
    var elementStyles = window.getComputedStyle($(element).get(0));
    var includePaddings = options && options.includePaddings;
    return $("<div>").text(text).css({
        fontStyle: elementStyles.fontStyle,
        fontVariant: elementStyles.fontVariant,
        fontWeight: elementStyles.fontWeight,
        fontSize: elementStyles.fontSize,
        fontFamily: elementStyles.fontFamily,
        letterSpacing: elementStyles.letterSpacing,
        border: elementStyles.border,
        paddingTop: includePaddings ? elementStyles.paddingTop : "",
        paddingRight: includePaddings ? elementStyles.paddingRight : "",
        paddingBottom: includePaddings ? elementStyles.paddingBottom : "",
        paddingLeft: includePaddings ? elementStyles.paddingLeft : "",
        visibility: "hidden",
        whiteSpace: "pre",
        position: "absolute",
        float: "left"
    })
};
export var insertBefore = (element, newElement) => {
    if (newElement) {
        domAdapter.insertElement(element.parentNode, newElement, element)
    }
    return element
};
export var replaceWith = (element, newElement) => {
    if (!(newElement && newElement[0])) {
        return
    }
    if (newElement.is(element)) {
        return element
    }
    each(newElement, (_, currentElement) => {
        insertBefore(element[0], currentElement)
    });
    element.remove();
    return newElement
};
