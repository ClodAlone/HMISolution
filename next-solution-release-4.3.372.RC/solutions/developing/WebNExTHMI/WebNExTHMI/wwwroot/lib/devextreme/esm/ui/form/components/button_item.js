/**
 * DevExtreme (esm/ui/form/components/button_item.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import {
    isDefined
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
var FIELD_BUTTON_ITEM_CLASS = "dx-field-button-item";
export function renderButtonItem(_ref) {
    var {
        item: item,
        $parent: $parent,
        rootElementCssClassList: rootElementCssClassList,
        validationGroup: validationGroup,
        createComponentCallback: createComponentCallback
    } = _ref;
    var $rootElement = $("<div>").appendTo($parent).addClass(rootElementCssClassList.join(" ")).addClass(FIELD_BUTTON_ITEM_CLASS).css("textAlign", convertAlignmentToTextAlign(item.horizontalAlignment));
    $parent.css("justifyContent", convertAlignmentToJustifyContent(item.verticalAlignment));
    var $button = $("<div>").appendTo($rootElement);
    return {
        $rootElement: $rootElement,
        buttonInstance: createComponentCallback($button, "dxButton", extend({
            validationGroup: validationGroup
        }, item.buttonOptions))
    }
}

function convertAlignmentToTextAlign(horizontalAlignment) {
    return isDefined(horizontalAlignment) ? horizontalAlignment : "right"
}

function convertAlignmentToJustifyContent(verticalAlignment) {
    switch (verticalAlignment) {
        case "center":
            return "center";
        case "bottom":
            return "flex-end";
        default:
            return "flex-start"
    }
}
