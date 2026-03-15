/**
 * DevExtreme (esm/ui/form/ui.form.layout_manager.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
import {
    isDefined
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    captionize
} from "../../core/utils/inflector";
import {
    inArray
} from "../../core/utils/array";
import Guid from "../../core/guid";
import {
    SIMPLE_ITEM_TYPE
} from "./constants";
var EDITORS_WITH_ARRAY_VALUE = ["dxTagBox", "dxRangeSlider"];
export var EDITORS_WITHOUT_LABELS = ["dxCalendar", "dxCheckBox", "dxHtmlEditor", "dxRadioGroup", "dxRangeSlider", "dxSlider", "dxSwitch"];
export function convertToRenderFieldItemOptions(_ref) {
    var {
        $parent: $parent,
        rootElementCssClassList: rootElementCssClassList,
        formOrLayoutManager: formOrLayoutManager,
        createComponentCallback: createComponentCallback,
        useFlexLayout: useFlexLayout,
        item: item,
        template: template,
        name: name,
        formLabelLocation: formLabelLocation,
        requiredMessageTemplate: requiredMessageTemplate,
        validationGroup: validationGroup,
        editorValue: editorValue,
        canAssignUndefinedValueToEditor: canAssignUndefinedValueToEditor,
        editorValidationBoundary: editorValidationBoundary,
        editorStylingMode: editorStylingMode,
        showColonAfterLabel: showColonAfterLabel,
        managerLabelLocation: managerLabelLocation,
        itemId: itemId,
        managerMarkOptions: managerMarkOptions,
        labelMode: labelMode
    } = _ref;
    var isRequired = isDefined(item.isRequired) ? item.isRequired : !!_hasRequiredRuleInSet(item.validationRules);
    var isSimpleItem = item.itemType === SIMPLE_ITEM_TYPE;
    var helpID = item.helpText ? "dx-" + new Guid : null;
    var labelOptions = _convertToLabelOptions({
        item: item,
        id: itemId,
        isRequired: isRequired,
        managerMarkOptions: managerMarkOptions,
        showColonAfterLabel: showColonAfterLabel,
        labelLocation: managerLabelLocation,
        formLabelMode: labelMode
    });
    var needRenderLabel = labelOptions.visible && labelOptions.text;
    var {
        location: labelLocation,
        labelID: labelID
    } = labelOptions;
    var labelNeedBaselineAlign = "top" !== labelLocation && (!!item.helpText && !useFlexLayout || -1 !== inArray(item.editorType, ["dxTextArea", "dxRadioGroup", "dxCalendar", "dxHtmlEditor"]));
    var editorOptions = _convertToEditorOptions({
        editorType: item.editorType,
        editorValue: editorValue,
        defaultEditorName: item.dataField,
        canAssignUndefinedValueToEditor: canAssignUndefinedValueToEditor,
        externalEditorOptions: item.editorOptions,
        editorInputId: itemId,
        editorValidationBoundary: editorValidationBoundary,
        editorStylingMode: editorStylingMode,
        formLabelMode: labelMode,
        labelText: labelOptions.textWithoutColon,
        labelMark: labelOptions.markOptions.showRequiredMark ? String.fromCharCode(160) + labelOptions.markOptions.requiredMark : ""
    });
    var needRenderOptionalMarkAsHelpText = labelOptions.markOptions.showOptionalMark && !labelOptions.visible && "hidden" !== editorOptions.labelMode && !isDefined(item.helpText);
    var helpText = needRenderOptionalMarkAsHelpText ? labelOptions.markOptions.optionalMark : item.helpText;
    return {
        $parent: $parent,
        rootElementCssClassList: rootElementCssClassList,
        formOrLayoutManager: formOrLayoutManager,
        createComponentCallback: createComponentCallback,
        useFlexLayout: useFlexLayout,
        labelOptions: labelOptions,
        labelNeedBaselineAlign: labelNeedBaselineAlign,
        labelLocation: labelLocation,
        needRenderLabel: needRenderLabel,
        item: item,
        isSimpleItem: isSimpleItem,
        isRequired: isRequired,
        template: template,
        helpID: helpID,
        labelID: labelID,
        name: name,
        helpText: helpText,
        formLabelLocation: formLabelLocation,
        requiredMessageTemplate: requiredMessageTemplate,
        validationGroup: validationGroup,
        editorOptions: editorOptions
    }
}
export function getLabelMarkText(_ref2) {
    var {
        showRequiredMark: showRequiredMark,
        requiredMark: requiredMark,
        showOptionalMark: showOptionalMark,
        optionalMark: optionalMark
    } = _ref2;
    if (!showRequiredMark && !showOptionalMark) {
        return ""
    }
    return String.fromCharCode(160) + (showRequiredMark ? requiredMark : optionalMark)
}
export function convertToLabelMarkOptions(_ref3, isRequired) {
    var {
        showRequiredMark: showRequiredMark,
        requiredMark: requiredMark,
        showOptionalMark: showOptionalMark,
        optionalMark: optionalMark
    } = _ref3;
    return {
        showRequiredMark: showRequiredMark && isRequired,
        requiredMark: requiredMark,
        showOptionalMark: showOptionalMark && !isRequired,
        optionalMark: optionalMark
    }
}

function _convertToEditorOptions(_ref4) {
    var {
        editorType: editorType,
        defaultEditorName: defaultEditorName,
        editorValue: editorValue,
        canAssignUndefinedValueToEditor: canAssignUndefinedValueToEditor,
        externalEditorOptions: externalEditorOptions,
        editorInputId: editorInputId,
        editorValidationBoundary: editorValidationBoundary,
        editorStylingMode: editorStylingMode,
        formLabelMode: formLabelMode,
        labelText: labelText,
        labelMark: labelMark
    } = _ref4;
    var editorOptionsWithValue = {};
    if (void 0 !== editorValue || canAssignUndefinedValueToEditor) {
        editorOptionsWithValue.value = editorValue
    }
    if (-1 !== EDITORS_WITH_ARRAY_VALUE.indexOf(editorType)) {
        editorOptionsWithValue.value = editorOptionsWithValue.value || []
    }
    var labelMode = null === externalEditorOptions || void 0 === externalEditorOptions ? void 0 : externalEditorOptions.labelMode;
    if (!isDefined(labelMode)) {
        labelMode = "outside" === formLabelMode ? "hidden" : formLabelMode
    }
    var stylingMode = (null === externalEditorOptions || void 0 === externalEditorOptions ? void 0 : externalEditorOptions.stylingMode) || editorStylingMode;
    var result = extend(true, editorOptionsWithValue, externalEditorOptions, {
        inputAttr: {
            id: editorInputId
        },
        validationBoundary: editorValidationBoundary,
        stylingMode: stylingMode,
        label: labelText,
        labelMode: labelMode,
        labelMark: labelMark
    });
    if (externalEditorOptions) {
        if (result.dataSource) {
            result.dataSource = externalEditorOptions.dataSource
        }
        if (result.items) {
            result.items = externalEditorOptions.items
        }
    }
    if (defaultEditorName && !result.name) {
        result.name = defaultEditorName
    }
    return result
}

function _hasRequiredRuleInSet(rules) {
    var hasRequiredRule;
    if (rules && rules.length) {
        each(rules, (function(index, rule) {
            if ("required" === rule.type) {
                hasRequiredRule = true;
                return false
            }
        }))
    }
    return hasRequiredRule
}

function _convertToLabelOptions(_ref5) {
    var {
        item: item,
        id: id,
        isRequired: isRequired,
        managerMarkOptions: managerMarkOptions,
        showColonAfterLabel: showColonAfterLabel,
        labelLocation: labelLocation,
        formLabelMode: formLabelMode
    } = _ref5;
    var isEditorWithoutLabels = -1 !== inArray(item.editorType, EDITORS_WITHOUT_LABELS);
    var labelOptions = extend({
        showColon: showColonAfterLabel,
        location: labelLocation,
        id: id,
        visible: "outside" === formLabelMode || isEditorWithoutLabels && "hidden" !== formLabelMode,
        isRequired: isRequired
    }, item ? item.label : {}, {
        markOptions: convertToLabelMarkOptions(managerMarkOptions, isRequired)
    });
    if (-1 !== inArray(item.editorType, ["dxRadioGroup", "dxCheckBox", "dxLookup", "dxSlider", "dxRangeSlider", "dxSwitch", "dxHtmlEditor"])) {
        labelOptions.labelID = "dx-label-".concat(new Guid)
    }
    if (!labelOptions.text && item.dataField) {
        labelOptions.text = captionize(item.dataField)
    }
    if (labelOptions.text) {
        labelOptions.textWithoutColon = labelOptions.text;
        labelOptions.text += labelOptions.showColon ? ":" : ""
    }
    return labelOptions
}
