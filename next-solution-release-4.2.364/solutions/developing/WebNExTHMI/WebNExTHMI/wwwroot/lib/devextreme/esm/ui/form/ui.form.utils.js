/**
 * DevExtreme (esm/ui/form/ui.form.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
export var createItemPathByIndex = (index, isTabs) => "".concat(isTabs ? "tabs" : "items", "[").concat(index, "]");
export var concatPaths = (path1, path2) => {
    if (isDefined(path1) && isDefined(path2)) {
        return "".concat(path1, ".").concat(path2)
    }
    return path1 || path2
};
export var getTextWithoutSpaces = text => text ? text.replace(/\s/g, "") : void 0;
export var isEqualToDataFieldOrNameOrTitleOrCaption = (item, fieldName) => {
    if (item) {
        return item.dataField === fieldName || item.name === fieldName || getTextWithoutSpaces(item.title) === fieldName || "group" === item.itemType && getTextWithoutSpaces(item.caption) === fieldName
    }
    return false
};
export var getFullOptionName = (path, optionName) => "".concat(path, ".").concat(optionName);
export var getOptionNameFromFullName = fullName => {
    var parts = fullName.split(".");
    return parts[parts.length - 1].replace(/\[\d+]/, "")
};
export var tryGetTabPath = fullPath => {
    var pathParts = fullPath.split(".");
    var resultPathParts = [...pathParts];
    for (var i = pathParts.length - 1; i >= 0; i--) {
        if (isFullPathContainsTabs(pathParts[i])) {
            return resultPathParts.join(".")
        }
        resultPathParts.splice(i, 1)
    }
    return ""
};
export var isFullPathContainsTabs = fullPath => fullPath.indexOf("tabs") > -1;
export var getItemPath = (items, item, isTabs) => {
    var index = items.indexOf(item);
    if (index > -1) {
        return createItemPathByIndex(index, isTabs)
    }
    for (var i = 0; i < items.length; i++) {
        var targetItem = items[i];
        var tabOrGroupItems = targetItem.tabs || targetItem.items;
        if (tabOrGroupItems) {
            var itemPath = getItemPath(tabOrGroupItems, item, targetItem.tabs);
            if (itemPath) {
                return concatPaths(createItemPathByIndex(i, isTabs), itemPath)
            }
        }
    }
};
export function convertToLayoutManagerOptions(_ref) {
    var {
        form: form,
        $formElement: $formElement,
        formOptions: formOptions,
        items: items,
        validationGroup: validationGroup,
        extendedLayoutManagerOptions: extendedLayoutManagerOptions,
        onFieldDataChanged: onFieldDataChanged,
        onContentReady: onContentReady,
        onDisposing: onDisposing,
        onFieldItemRendered: onFieldItemRendered
    } = _ref;
    var baseOptions = {
        form: form,
        items: items,
        $formElement: $formElement,
        validationGroup: validationGroup,
        onFieldDataChanged: onFieldDataChanged,
        onContentReady: onContentReady,
        onDisposing: onDisposing,
        onFieldItemRendered: onFieldItemRendered,
        validationBoundary: formOptions.scrollingEnabled ? $formElement : void 0,
        scrollingEnabled: formOptions.scrollingEnabled,
        showRequiredMark: formOptions.showRequiredMark,
        showOptionalMark: formOptions.showOptionalMark,
        requiredMark: formOptions.requiredMark,
        optionalMark: formOptions.optionalMark,
        requiredMessage: formOptions.requiredMessage,
        screenByWidth: formOptions.screenByWidth,
        layoutData: formOptions.formData,
        labelLocation: formOptions.labelLocation,
        customizeItem: formOptions.customizeItem,
        minColWidth: formOptions.minColWidth,
        showColonAfterLabel: formOptions.showColonAfterLabel,
        onEditorEnterKey: formOptions.onEditorEnterKey,
        labelMode: formOptions.labelMode
    };
    var result = extend(baseOptions, {
        isRoot: extendedLayoutManagerOptions.isRoot,
        colCount: extendedLayoutManagerOptions.colCount,
        alignItemLabels: extendedLayoutManagerOptions.alignItemLabels,
        cssItemClass: extendedLayoutManagerOptions.cssItemClass,
        colCountByScreen: extendedLayoutManagerOptions.colCountByScreen,
        onLayoutChanged: extendedLayoutManagerOptions.onLayoutChanged,
        width: extendedLayoutManagerOptions.width
    });
    return result
}
