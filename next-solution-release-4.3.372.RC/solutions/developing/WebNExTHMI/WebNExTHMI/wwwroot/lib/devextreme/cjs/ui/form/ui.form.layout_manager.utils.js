/**
 * DevExtreme (cjs/ui/form/ui.form.layout_manager.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.EDITORS_WITHOUT_LABELS = void 0;
exports.convertToLabelMarkOptions = convertToLabelMarkOptions;
exports.convertToRenderFieldItemOptions = convertToRenderFieldItemOptions;
exports.getLabelMarkText = getLabelMarkText;
var _extend = require("../../core/utils/extend");
var _type = require("../../core/utils/type");
var _iterator = require("../../core/utils/iterator");
var _inflector = require("../../core/utils/inflector");
var _array = require("../../core/utils/array");
var _guid = _interopRequireDefault(require("../../core/guid"));
var _constants = require("./constants");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var EDITORS_WITH_ARRAY_VALUE = ["dxTagBox", "dxRangeSlider"];
var EDITORS_WITHOUT_LABELS = ["dxCalendar", "dxCheckBox", "dxHtmlEditor", "dxRadioGroup", "dxRangeSlider", "dxSlider", "dxSwitch"];
exports.EDITORS_WITHOUT_LABELS = EDITORS_WITHOUT_LABELS;

function convertToRenderFieldItemOptions(_ref) {
    var $parent = _ref.$parent,
        rootElementCssClassList = _ref.rootElementCssClassList,
        formOrLayoutManager = _ref.formOrLayoutManager,
        createComponentCallback = _ref.createComponentCallback,
        useFlexLayout = _ref.useFlexLayout,
        item = _ref.item,
        template = _ref.template,
        name = _ref.name,
        formLabelLocation = _ref.formLabelLocation,
        requiredMessageTemplate = _ref.requiredMessageTemplate,
        validationGroup = _ref.validationGroup,
        editorValue = _ref.editorValue,
        canAssignUndefinedValueToEditor = _ref.canAssignUndefinedValueToEditor,
        editorValidationBoundary = _ref.editorValidationBoundary,
        editorStylingMode = _ref.editorStylingMode,
        showColonAfterLabel = _ref.showColonAfterLabel,
        managerLabelLocation = _ref.managerLabelLocation,
        itemId = _ref.itemId,
        managerMarkOptions = _ref.managerMarkOptions,
        labelMode = _ref.labelMode;
    var isRequired = (0, _type.isDefined)(item.isRequired) ? item.isRequired : !!_hasRequiredRuleInSet(item.validationRules);
    var isSimpleItem = item.itemType === _constants.SIMPLE_ITEM_TYPE;
    var helpID = item.helpText ? "dx-" + new _guid.default : null;
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
    var labelLocation = labelOptions.location,
        labelID = labelOptions.labelID;
    var labelNeedBaselineAlign = "top" !== labelLocation && (!!item.helpText && !useFlexLayout || -1 !== (0, _array.inArray)(item.editorType, ["dxTextArea", "dxRadioGroup", "dxCalendar", "dxHtmlEditor"]));
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
    var needRenderOptionalMarkAsHelpText = labelOptions.markOptions.showOptionalMark && !labelOptions.visible && "hidden" !== editorOptions.labelMode && !(0, _type.isDefined)(item.helpText);
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

function getLabelMarkText(_ref2) {
    var showRequiredMark = _ref2.showRequiredMark,
        requiredMark = _ref2.requiredMark,
        showOptionalMark = _ref2.showOptionalMark,
        optionalMark = _ref2.optionalMark;
    if (!showRequiredMark && !showOptionalMark) {
        return ""
    }
    return String.fromCharCode(160) + (showRequiredMark ? requiredMark : optionalMark)
}

function convertToLabelMarkOptions(_ref3, isRequired) {
    var showRequiredMark = _ref3.showRequiredMark,
        requiredMark = _ref3.requiredMark,
        showOptionalMark = _ref3.showOptionalMark,
        optionalMark = _ref3.optionalMark;
    return {
        showRequiredMark: showRequiredMark && isRequired,
        requiredMark: requiredMark,
        showOptionalMark: showOptionalMark && !isRequired,
        optionalMark: optionalMark
    }
}

function _convertToEditorOptions(_ref4) {
    var editorType = _ref4.editorType,
        defaultEditorName = _ref4.defaultEditorName,
        editorValue = _ref4.editorValue,
        canAssignUndefinedValueToEditor = _ref4.canAssignUndefinedValueToEditor,
        externalEditorOptions = _ref4.externalEditorOptions,
        editorInputId = _ref4.editorInputId,
        editorValidationBoundary = _ref4.editorValidationBoundary,
        editorStylingMode = _ref4.editorStylingMode,
        formLabelMode = _ref4.formLabelMode,
        labelText = _ref4.labelText,
        labelMark = _ref4.labelMark;
    var editorOptionsWithValue = {};
    if (void 0 !== editorValue || canAssignUndefinedValueToEditor) {
        editorOptionsWithValue.value = editorValue
    }
    if (-1 !== EDITORS_WITH_ARRAY_VALUE.indexOf(editorType)) {
        editorOptionsWithValue.value = editorOptionsWithValue.value || []
    }
    var labelMode = null === externalEditorOptions || void 0 === externalEditorOptions ? void 0 : externalEditorOptions.labelMode;
    if (!(0, _type.isDefined)(labelMode)) {
        labelMode = "outside" === formLabelMode ? "hidden" : formLabelMode
    }
    var stylingMode = (null === externalEditorOptions || void 0 === externalEditorOptions ? void 0 : externalEditorOptions.stylingMode) || editorStylingMode;
    var result = (0, _extend.extend)(true, editorOptionsWithValue, externalEditorOptions, {
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
        (0, _iterator.each)(rules, (function(index, rule) {
            if ("required" === rule.type) {
                hasRequiredRule = true;
                return false
            }
        }))
    }
    return hasRequiredRule
}

function _convertToLabelOptions(_ref5) {
    var item = _ref5.item,
        id = _ref5.id,
        isRequired = _ref5.isRequired,
        managerMarkOptions = _ref5.managerMarkOptions,
        showColonAfterLabel = _ref5.showColonAfterLabel,
        labelLocation = _ref5.labelLocation,
        formLabelMode = _ref5.formLabelMode;
    var isEditorWithoutLabels = -1 !== (0, _array.inArray)(item.editorType, EDITORS_WITHOUT_LABELS);
    var labelOptions = (0, _extend.extend)({
        showColon: showColonAfterLabel,
        location: labelLocation,
        id: id,
        visible: "outside" === formLabelMode || isEditorWithoutLabels && "hidden" !== formLabelMode,
        isRequired: isRequired
    }, item ? item.label : {}, {
        markOptions: convertToLabelMarkOptions(managerMarkOptions, isRequired)
    });
    if (-1 !== (0, _array.inArray)(item.editorType, ["dxRadioGroup", "dxCheckBox", "dxLookup", "dxSlider", "dxRangeSlider", "dxSwitch", "dxHtmlEditor"])) {
        labelOptions.labelID = "dx-label-".concat(new _guid.default)
    }
    if (!labelOptions.text && item.dataField) {
        labelOptions.text = (0, _inflector.captionize)(item.dataField)
    }
    if (labelOptions.text) {
        labelOptions.textWithoutColon = labelOptions.text;
        labelOptions.text += labelOptions.showColon ? ":" : ""
    }
    return labelOptions
}
