/**
 * DevExtreme (esm/ui/form/components/label.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["$FIELD_ITEM_LABEL_CONTENT_CLASS"];
import $ from "../../../core/renderer";
import {
    isDefined
} from "../../../core/utils/type";
import {
    isEmpty
} from "../../../core/utils/string";
import {
    getLabelMarkText
} from "../ui.form.layout_manager.utils";
import {
    WIDGET_CLASS,
    FIELD_ITEM_LABEL_CONTENT_CLASS,
    FIELD_ITEM_LABEL_CLASS
} from "../constants";
export var GET_LABEL_WIDTH_BY_TEXT_CLASS = "dx-layout-manager-hidden-label";
export var FIELD_ITEM_REQUIRED_MARK_CLASS = "dx-field-item-required-mark";
export var FIELD_ITEM_LABEL_LOCATION_CLASS = "dx-field-item-label-location-";
export var FIELD_ITEM_OPTIONAL_MARK_CLASS = "dx-field-item-optional-mark";
export var FIELD_ITEM_LABEL_TEXT_CLASS = "dx-field-item-label-text";
export function renderLabel(_ref) {
    var {
        text: text,
        id: id,
        location: location,
        alignment: alignment,
        labelID: labelID = null,
        markOptions: markOptions = {}
    } = _ref;
    if (!isDefined(text) || text.length <= 0) {
        return null
    }
    return $("<label>").addClass(FIELD_ITEM_LABEL_CLASS + " " + FIELD_ITEM_LABEL_LOCATION_CLASS + location).attr("for", id).attr("id", labelID).css("textAlign", alignment).append($("<span>").addClass(FIELD_ITEM_LABEL_CONTENT_CLASS).append($("<span>").addClass(FIELD_ITEM_LABEL_TEXT_CLASS).text(text), _renderLabelMark(markOptions)))
}

function _renderLabelMark(markOptions) {
    var markText = getLabelMarkText(markOptions);
    if ("" === markText) {
        return null
    }
    return $("<span>").addClass(markOptions.showRequiredMark ? FIELD_ITEM_REQUIRED_MARK_CLASS : FIELD_ITEM_OPTIONAL_MARK_CLASS).text(markText)
}
export function setLabelWidthByMaxLabelWidth($targetContainer, labelsSelector, labelMarkOptions) {
    var FIELD_ITEM_LABEL_CONTENT_CLASS_Selector = "".concat(labelsSelector, " > .").concat(FIELD_ITEM_LABEL_CLASS, ":not(.").concat(FIELD_ITEM_LABEL_LOCATION_CLASS, "top) > .").concat(FIELD_ITEM_LABEL_CONTENT_CLASS);
    var $FIELD_ITEM_LABEL_CONTENT_CLASS_Items = $targetContainer.find(FIELD_ITEM_LABEL_CONTENT_CLASS_Selector);
    var FIELD_ITEM_LABEL_CONTENT_CLASS_Length = $FIELD_ITEM_LABEL_CONTENT_CLASS_Items.length;
    var labelWidth;
    var i;
    var maxWidth = 0;
    for (i = 0; i < FIELD_ITEM_LABEL_CONTENT_CLASS_Length; i++) {
        labelWidth = getLabelWidthByInnerHTML({
            $FIELD_ITEM_LABEL_CONTENT_CLASS: $FIELD_ITEM_LABEL_CONTENT_CLASS_Items[i],
            location: "left",
            markOptions: labelMarkOptions
        });
        if (labelWidth > maxWidth) {
            maxWidth = labelWidth
        }
    }
    for (i = 0; i < FIELD_ITEM_LABEL_CONTENT_CLASS_Length; i++) {
        $FIELD_ITEM_LABEL_CONTENT_CLASS_Items[i].style.width = maxWidth + "px"
    }
}

function getLabelWidthByInnerHTML(options) {
    var {
        $FIELD_ITEM_LABEL_CONTENT_CLASS: $FIELD_ITEM_LABEL_CONTENT_CLASS
    } = options, renderLabelOptions = _objectWithoutPropertiesLoose(options, _excluded);
    var $hiddenContainer = $("<div>").addClass(WIDGET_CLASS).addClass(GET_LABEL_WIDTH_BY_TEXT_CLASS).appendTo("body");
    renderLabelOptions.text = " ";
    var $label = renderLabel(renderLabelOptions).appendTo($hiddenContainer);
    var labelTextElement = $label.find("." + FIELD_ITEM_LABEL_TEXT_CLASS)[0];
    labelTextElement.innerHTML = getLabelInnerHTML($FIELD_ITEM_LABEL_CONTENT_CLASS);
    var result = labelTextElement.offsetWidth;
    $hiddenContainer.remove();
    return result
}

function getLabelInnerHTML($FIELD_ITEM_LABEL_CONTENT_CLASS) {
    var length = $FIELD_ITEM_LABEL_CONTENT_CLASS.children.length;
    var child;
    var result = "";
    var i;
    for (i = 0; i < length; i++) {
        child = $FIELD_ITEM_LABEL_CONTENT_CLASS.children[i];
        result += !isEmpty(child.innerText) ? child.innerText : child.innerHTML
    }
    return result
}
