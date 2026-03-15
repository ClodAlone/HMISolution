/**
 * DevExtreme (esm/ui/form/components/empty_item.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
export var FIELD_EMPTY_ITEM_CLASS = "dx-field-empty-item";
export function renderEmptyItem(_ref) {
    var {
        $parent: $parent,
        rootElementCssClassList: rootElementCssClassList
    } = _ref;
    return $("<div>").addClass(FIELD_EMPTY_ITEM_CLASS).html("&nbsp;").addClass(rootElementCssClassList.join(" ")).appendTo($parent)
}
