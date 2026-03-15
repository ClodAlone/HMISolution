/**
 * DevExtreme (esm/ui/form/components/field_item.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import eventsEngine from "../../../events/core/events_engine";
import {
    name as clickEventName
} from "../../../events/click";
import {
    getPublicElement
} from "../../../core/element";
import {
    captionize
} from "../../../core/utils/inflector";
import {
    format
} from "../../../core/utils/string";
import {
    isMaterial
} from "../../themes";
import errors from "../../widget/ui.errors";
import Validator from "../../validator";
import {
    FIELD_ITEM_CONTENT_CLASS
} from "../constants";
export var FLEX_LAYOUT_CLASS = "dx-flex-layout";
export var FIELD_ITEM_OPTIONAL_CLASS = "dx-field-item-optional";
export var FIELD_ITEM_REQUIRED_CLASS = "dx-field-item-required";
export var FIELD_ITEM_CONTENT_WRAPPER_CLASS = "dx-field-item-content-wrapper";
export var FIELD_ITEM_CONTENT_LOCATION_CLASS = "dx-field-item-content-location-";
export var FIELD_ITEM_LABEL_ALIGN_CLASS = "dx-field-item-label-align";
export var FIELD_ITEM_HELP_TEXT_CLASS = "dx-field-item-help-text";
export var LABEL_VERTICAL_ALIGNMENT_CLASS = "dx-label-v-align";
export var LABEL_HORIZONTAL_ALIGNMENT_CLASS = "dx-label-h-align";
import {
    renderLabel
} from "./label";
var TEMPLATE_WRAPPER_CLASS = "dx-template-wrapper";
var INVALID_CLASS = "dx-invalid";
export function renderFieldItem(_ref) {
    var {
        $parent: $parent,
        rootElementCssClassList: rootElementCssClassList,
        formOrLayoutManager: formOrLayoutManager,
        createComponentCallback: createComponentCallback,
        useFlexLayout: useFlexLayout,
        labelOptions: labelOptions,
        labelNeedBaselineAlign: labelNeedBaselineAlign,
        labelLocation: labelLocation,
        needRenderLabel: needRenderLabel,
        formLabelLocation: formLabelLocation,
        item: item,
        editorOptions: editorOptions,
        isSimpleItem: isSimpleItem,
        isRequired: isRequired,
        template: template,
        helpID: helpID,
        labelID: labelID,
        name: name,
        helpText: helpText,
        requiredMessageTemplate: requiredMessageTemplate,
        validationGroup: validationGroup
    } = _ref;
    var $rootElement = $("<div>").addClass(rootElementCssClassList.join(" ")).appendTo($parent);
    $rootElement.addClass(isRequired ? FIELD_ITEM_REQUIRED_CLASS : FIELD_ITEM_OPTIONAL_CLASS);
    if (isSimpleItem && useFlexLayout) {
        $rootElement.addClass(FLEX_LAYOUT_CLASS)
    }
    if (isSimpleItem && labelNeedBaselineAlign) {
        $rootElement.addClass(FIELD_ITEM_LABEL_ALIGN_CLASS)
    }
    var $fieldEditorContainer = $("<div>");
    $fieldEditorContainer.data("dx-form-item", item);
    $fieldEditorContainer.addClass(FIELD_ITEM_CONTENT_CLASS).addClass(FIELD_ITEM_CONTENT_LOCATION_CLASS + {
        right: "left",
        left: "right",
        top: "bottom"
    } [formLabelLocation]);
    var $label = needRenderLabel ? renderLabel(labelOptions) : null;
    if ($label) {
        $rootElement.append($label);
        if ("top" === labelLocation || "left" === labelLocation) {
            $rootElement.append($fieldEditorContainer)
        }
        if ("right" === labelLocation) {
            $rootElement.prepend($fieldEditorContainer)
        }
        if ("top" === labelLocation) {
            $rootElement.addClass(LABEL_VERTICAL_ALIGNMENT_CLASS)
        } else {
            $rootElement.addClass(LABEL_HORIZONTAL_ALIGNMENT_CLASS)
        }
        if ("dxCheckBox" === item.editorType || "dxSwitch" === item.editorType) {
            eventsEngine.on($label, clickEventName, (function() {
                eventsEngine.trigger($fieldEditorContainer.children(), clickEventName)
            }))
        }
    } else {
        $rootElement.append($fieldEditorContainer)
    }
    var widgetInstance;
    if (template) {
        template.render({
            container: getPublicElement($fieldEditorContainer),
            model: {
                dataField: item.dataField,
                editorType: item.editorType,
                editorOptions: editorOptions,
                component: formOrLayoutManager,
                name: item.name
            }
        })
    } else {
        var $div = $("<div>").appendTo($fieldEditorContainer);
        try {
            widgetInstance = createComponentCallback($div, item.editorType, editorOptions);
            widgetInstance.setAria("describedby", helpID);
            if (labelID) {
                widgetInstance.setAria("labelledby", labelID)
            }
            widgetInstance.setAria("required", isRequired)
        } catch (e) {
            errors.log("E1035", e.message)
        }
    }
    var editorElem = $fieldEditorContainer.children().first();
    var $validationTarget = editorElem.hasClass(TEMPLATE_WRAPPER_CLASS) ? editorElem.children().first() : editorElem;
    var validationTargetInstance = $validationTarget && $validationTarget.data("dx-validation-target");
    if (validationTargetInstance) {
        var isItemHaveCustomLabel = item.label && item.label.text;
        var itemName = isItemHaveCustomLabel ? null : name;
        var fieldName = isItemHaveCustomLabel ? item.label.text : itemName && captionize(itemName);
        var validationRules;
        if (isSimpleItem) {
            if (item.validationRules) {
                validationRules = item.validationRules
            } else {
                var requiredMessage = format(requiredMessageTemplate, fieldName || "");
                validationRules = item.isRequired ? [{
                    type: "required",
                    message: requiredMessage
                }] : null
            }
        }
        if (Array.isArray(validationRules) && validationRules.length) {
            createComponentCallback($validationTarget, Validator, {
                validationRules: validationRules,
                validationGroup: validationGroup,
                dataGetter: function() {
                    return {
                        formItem: item
                    }
                }
            })
        }
        if (isMaterial()) {
            var wrapperClass = "." + FIELD_ITEM_CONTENT_WRAPPER_CLASS;
            var toggleInvalidClass = function(e) {
                $(e.element).parents(wrapperClass).toggleClass(INVALID_CLASS, false === e.component.option("isValid") && (e.component._isFocused() || "always" === e.component.option("validationMessageMode")))
            };
            validationTargetInstance.on("optionChanged", e => {
                if ("isValid" !== e.name) {
                    return
                }
                toggleInvalidClass(e)
            });
            validationTargetInstance.on("focusIn", toggleInvalidClass).on("focusOut", toggleInvalidClass).on("enterKey", toggleInvalidClass)
        }
    }
    if (helpText && isSimpleItem) {
        var $editorParent = $fieldEditorContainer.parent();
        $editorParent.append($("<div>").addClass(FIELD_ITEM_CONTENT_WRAPPER_CLASS).append($fieldEditorContainer).append($("<div>").addClass(FIELD_ITEM_HELP_TEXT_CLASS).attr("id", helpID).text(helpText)))
    }
    return {
        $fieldEditorContainer: $fieldEditorContainer,
        $rootElement: $rootElement,
        widgetInstance: widgetInstance
    }
}
