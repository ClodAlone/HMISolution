/**
 * DevExtreme (esm/renovation/ui/form/_toDelete/ui.form.layout_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../../../../core/utils/size";
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import Guid from "../../core/guid";
import {
    default as FormItemsRunTimeInfo
} from "./ui.form.items_runtime_info";
import registerComponent from "../../core/component_registrator";
import {
    isDefined,
    isEmptyObject,
    isFunction,
    isObject,
    type
} from "../../core/utils/type";
import {
    getPublicElement
} from "../../core/element";
import variableWrapper from "../../core/utils/variable_wrapper";
import {
    getCurrentScreenFactor,
    hasWindow
} from "../../core/utils/window";
import {
    format
} from "../../core/utils/string";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    inArray,
    normalizeIndexes
} from "../../core/utils/array";
import {
    compileGetter
} from "../../core/utils/data";
import {
    removeEvent
} from "../../events/remove";
import {
    name as clickEventName
} from "../../events/click";
import errors from "../widget/ui.errors";
import messageLocalization from "../../localization/message";
import {
    styleProp
} from "../../core/utils/style";
import {
    captionize
} from "../../core/utils/inflector";
import Widget from "../widget/ui.widget";
import Validator from "../validator";
import ResponsiveBox from "../responsive_box";
import {
    isMaterial
} from "../themes";
import {
    FIELD_ITEM_CLASS,
    FLEX_LAYOUT_CLASS,
    LAYOUT_MANAGER_ONE_COLUMN,
    FIELD_ITEM_OPTIONAL_MARK_CLASS,
    FIELD_ITEM_REQUIRED_MARK_CLASS,
    FIELD_ITEM_OPTIONAL_CLASS,
    FIELD_ITEM_REQUIRED_CLASS,
    FIELD_ITEM_LABEL_TEXT_CLASS,
    FIELD_ITEM_LABEL_CONTENT_CLASS,
    FIELD_ITEM_HELP_TEXT_CLASS,
    FIELD_ITEM_CONTENT_WRAPPER_CLASS,
    LABEL_VERTICAL_ALIGNMENT_CLASS,
    LABEL_HORIZONTAL_ALIGNMENT_CLASS,
    FIELD_ITEM_LABEL_LOCATION_CLASS,
    FIELD_ITEM_LABEL_ALIGN_CLASS,
    FIELD_ITEM_LABEL_CLASS,
    FIELD_ITEM_CONTENT_LOCATION_CLASS,
    FIELD_ITEM_CONTENT_CLASS,
    FIELD_EMPTY_ITEM_CLASS,
    FIELD_BUTTON_ITEM_CLASS,
    SINGLE_COLUMN_ITEM_CONTENT,
    ROOT_SIMPLE_ITEM_CLASS
} from "./constants";
import "../text_box";
import "../number_box";
import "../check_box";
import "../date_box";
import "../button";
var FORM_EDITOR_BY_DEFAULT = "dxTextBox";
var LAYOUT_MANAGER_FIRST_ROW_CLASS = "dx-first-row";
var LAYOUT_MANAGER_LAST_ROW_CLASS = "dx-last-row";
var LAYOUT_MANAGER_FIRST_COL_CLASS = "dx-first-col";
var LAYOUT_MANAGER_LAST_COL_CLASS = "dx-last-col";
var INVALID_CLASS = "dx-invalid";
var LAYOUT_STRATEGY_FLEX = "flex";
var LAYOUT_STRATEGY_FALLBACK = "fallback";
var SIMPLE_ITEM_TYPE = "simple";
var TEMPLATE_WRAPPER_CLASS = "dx-template-wrapper";
var DATA_OPTIONS = ["dataSource", "items"];
var EDITORS_WITH_ARRAY_VALUE = ["dxTagBox", "dxRangeSlider"];
var LayoutManager = Widget.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            layoutData: {},
            readOnly: false,
            colCount: 1,
            colCountByScreen: void 0,
            labelLocation: "left",
            onFieldDataChanged: null,
            onEditorEnterKey: null,
            customizeItem: null,
            alignItemLabels: true,
            minColWidth: 200,
            showRequiredMark: true,
            showOptionalMark: false,
            requiredMark: "*",
            optionalMark: messageLocalization.format("dxForm-optionalMark"),
            requiredMessage: messageLocalization.getFormatter("dxForm-requiredMessage")
        })
    },
    _setOptionsByReference: function() {
        this.callBase();
        extend(this._optionsByReference, {
            layoutData: true,
            validationGroup: true
        })
    },
    _init: function() {
        var layoutData = this.option("layoutData");
        this.callBase();
        this._itemWatchers = [];
        this._itemsRunTimeInfo = new FormItemsRunTimeInfo;
        this._updateReferencedOptions(layoutData);
        this._initDataAndItems(layoutData)
    },
    _dispose: function() {
        this.callBase();
        this._cleanItemWatchers()
    },
    _initDataAndItems: function(initialData) {
        this._syncDataWithItems();
        this._updateItems(initialData)
    },
    _syncDataWithItems: function() {
        var layoutData = this.option("layoutData");
        var userItems = this.option("items");
        if (isDefined(userItems)) {
            userItems.forEach(item => {
                if (item.dataField && void 0 === this._getDataByField(item.dataField)) {
                    var value;
                    if (item.editorOptions) {
                        value = item.editorOptions.value
                    }
                    if (isDefined(value) || item.dataField in layoutData) {
                        this._updateFieldValue(item.dataField, value)
                    }
                }
            })
        }
    },
    _getDataByField: function(dataField) {
        return dataField ? this.option("layoutData." + dataField) : null
    },
    _isCheckboxUndefinedStateEnabled: function(editorOption) {
        if (true === editorOption.allowIndeterminateState && "dxCheckBox" === editorOption.editorType) {
            var nameParts = ["layoutData", ...editorOption.dataField.split(".")];
            var propertyName = nameParts.pop();
            var layoutData = this.option(nameParts.join("."));
            return layoutData && propertyName in layoutData
        }
        return false
    },
    _updateFieldValue: function(dataField, value) {
        var layoutData = this.option("layoutData");
        var newValue = value;
        if (!variableWrapper.isWrapped(layoutData[dataField]) && isDefined(dataField)) {
            this.option("layoutData." + dataField, newValue)
        } else if (variableWrapper.isWritableWrapped(layoutData[dataField])) {
            newValue = isFunction(newValue) ? newValue() : newValue;
            layoutData[dataField](newValue)
        }
        this._triggerOnFieldDataChanged({
            dataField: dataField,
            value: newValue
        })
    },
    _triggerOnFieldDataChanged: function(args) {
        this._createActionByOption("onFieldDataChanged")(args)
    },
    _updateItems: function(layoutData) {
        var that = this;
        var userItems = this.option("items");
        var isUserItemsExist = isDefined(userItems);
        var customizeItem = that.option("customizeItem");
        var items = isUserItemsExist ? userItems : this._generateItemsByData(layoutData);
        if (isDefined(items)) {
            var processedItems = [];
            each(items, (function(index, item) {
                if (that._isAcceptableItem(item)) {
                    item = that._processItem(item);
                    customizeItem && customizeItem(item);
                    if (isObject(item) && false !== variableWrapper.unwrap(item.visible)) {
                        processedItems.push(item)
                    }
                }
            }));
            if (!that._itemWatchers.length || !isUserItemsExist) {
                that._updateItemWatchers(items)
            }
            this._setItems(processedItems);
            this._sortItems()
        }
    },
    _cleanItemWatchers: function() {
        this._itemWatchers.forEach((function(dispose) {
            dispose()
        }));
        this._itemWatchers = []
    },
    _updateItemWatchers: function(items) {
        var that = this;
        var watch = that._getWatch();
        items.forEach((function(item) {
            if (isObject(item) && isDefined(item.visible) && isFunction(watch)) {
                that._itemWatchers.push(watch((function() {
                    return variableWrapper.unwrap(item.visible)
                }), (function() {
                    that._updateItems(that.option("layoutData"));
                    that.repaint()
                }), {
                    skipImmediate: true
                }))
            }
        }))
    },
    _generateItemsByData: function(layoutData) {
        var result = [];
        if (isDefined(layoutData)) {
            each(layoutData, (function(dataField) {
                result.push({
                    dataField: dataField
                })
            }))
        }
        return result
    },
    _isAcceptableItem: function(item) {
        var itemField = item.dataField || item;
        var itemData = this._getDataByField(itemField);
        return !(isFunction(itemData) && !variableWrapper.isWrapped(itemData))
    },
    _processItem: function(item) {
        if ("string" === typeof item) {
            item = {
                dataField: item
            }
        }
        if ("object" === typeof item && !item.itemType) {
            item.itemType = SIMPLE_ITEM_TYPE
        }
        if (!isDefined(item.editorType) && isDefined(item.dataField)) {
            var value = this._getDataByField(item.dataField);
            item.editorType = isDefined(value) ? this._getEditorTypeByDataType(type(value)) : FORM_EDITOR_BY_DEFAULT
        }
        if ("dxCheckBox" === item.editorType) {
            var _item$allowIndetermin;
            item.allowIndeterminateState = null !== (_item$allowIndetermin = item.allowIndeterminateState) && void 0 !== _item$allowIndetermin ? _item$allowIndetermin : true
        }
        return item
    },
    _getEditorTypeByDataType: function(dataType) {
        switch (dataType) {
            case "number":
                return "dxNumberBox";
            case "date":
                return "dxDateBox";
            case "boolean":
                return "dxCheckBox";
            default:
                return "dxTextBox"
        }
    },
    _sortItems: function() {
        normalizeIndexes(this._items, "visibleIndex");
        this._sortIndexes()
    },
    _sortIndexes: function() {
        this._items.sort((function(itemA, itemB) {
            var indexA = itemA.visibleIndex;
            var indexB = itemB.visibleIndex;
            var result;
            if (indexA > indexB) {
                result = 1
            } else if (indexA < indexB) {
                result = -1
            } else {
                result = 0
            }
            return result
        }))
    },
    _initMarkup: function() {
        this._itemsRunTimeInfo.clear();
        this.callBase();
        this._renderResponsiveBox()
    },
    _hasBrowserFlex: function() {
        return styleProp(LAYOUT_STRATEGY_FLEX) === LAYOUT_STRATEGY_FLEX
    },
    _renderResponsiveBox: function() {
        var templatesInfo = [];
        if (this._items && this._items.length) {
            var colCount = this._getColCount();
            this._prepareItemsWithMerging(colCount);
            var layoutItems = this._generateLayoutItems();
            this._extendItemsWithDefaultTemplateOptions(layoutItems, this._items);
            this._responsiveBox = this._createComponent(ResponsiveBox, this._getResponsiveBoxConfig(layoutItems, colCount, templatesInfo));
            if (!hasWindow()) {
                this._renderTemplates(templatesInfo)
            }
        }
    },
    _extendItemsWithDefaultTemplateOptions: function(targetItems, sourceItems) {
        sourceItems.forEach((function(item) {
            if (!item.merged) {
                if (isDefined(item.disabled)) {
                    targetItems[item.visibleIndex].disabled = item.disabled
                }
                if (isDefined(item.visible)) {
                    targetItems[item.visibleIndex].visible = item.visible
                }
            }
        }))
    },
    _itemStateChangedHandler: function(e) {
        this._refresh()
    },
    _renderTemplate: function($container, item) {
        switch (item.itemType) {
            case "empty":
                this._renderEmptyItem($container);
                break;
            case "button":
                this._renderButtonItem(item, $container);
                break;
            default:
                this._renderFieldItem(item, $container)
        }
    },
    _renderTemplates: function(templatesInfo) {
        var that = this;
        each(templatesInfo, (function(index, info) {
            that._renderTemplate(info.container, info.formItem)
        }))
    },
    _getResponsiveBoxConfig: function(layoutItems, colCount, templatesInfo) {
        var that = this;
        var colCountByScreen = that.option("colCountByScreen");
        var xsColCount = colCountByScreen && colCountByScreen.xs;
        return {
            onItemStateChanged: this._itemStateChangedHandler.bind(this),
            _layoutStrategy: that._hasBrowserFlex() ? LAYOUT_STRATEGY_FLEX : LAYOUT_STRATEGY_FALLBACK,
            onLayoutChanged: function() {
                var onLayoutChanged = that.option("onLayoutChanged");
                var isSingleColumnMode = that.isSingleColumnMode();
                if (onLayoutChanged) {
                    that.$element().toggleClass(LAYOUT_MANAGER_ONE_COLUMN, isSingleColumnMode);
                    onLayoutChanged(isSingleColumnMode)
                }
            },
            onContentReady: function(e) {
                if (hasWindow()) {
                    that._renderTemplates(templatesInfo)
                }
                if (that.option("onLayoutChanged")) {
                    that.$element().toggleClass(LAYOUT_MANAGER_ONE_COLUMN, that.isSingleColumnMode(e.component))
                }
            },
            itemTemplate: function(e, itemData, itemElement) {
                if (!e.location) {
                    return
                }
                var $itemElement = $(itemElement);
                var itemRenderedCountInPreviousRows = e.location.row * colCount;
                var item = that._items[e.location.col + itemRenderedCountInPreviousRows];
                var $fieldItem = $("<div>").addClass(item.cssClass).appendTo($itemElement);
                templatesInfo.push({
                    container: $fieldItem,
                    formItem: item
                });
                $itemElement.toggleClass(SINGLE_COLUMN_ITEM_CONTENT, that.isSingleColumnMode(this));
                if (0 === e.location.row) {
                    $fieldItem.addClass(LAYOUT_MANAGER_FIRST_ROW_CLASS)
                }
                if (0 === e.location.col) {
                    $fieldItem.addClass(LAYOUT_MANAGER_FIRST_COL_CLASS)
                }
                if (item.itemType === SIMPLE_ITEM_TYPE && that.option("isRoot")) {
                    $itemElement.addClass(ROOT_SIMPLE_ITEM_CLASS)
                }
                var isLastColumn = e.location.col === colCount - 1 || e.location.col + e.location.colspan === colCount;
                var rowsCount = that._getRowsCount();
                var isLastRow = e.location.row === rowsCount - 1;
                if (isLastColumn) {
                    $fieldItem.addClass(LAYOUT_MANAGER_LAST_COL_CLASS)
                }
                if (isLastRow) {
                    $fieldItem.addClass(LAYOUT_MANAGER_LAST_ROW_CLASS)
                }
            },
            cols: that._generateRatio(colCount),
            rows: that._generateRatio(that._getRowsCount(), true),
            dataSource: layoutItems,
            singleColumnScreen: xsColCount ? false : "xs"
        }
    },
    _getColCount: function() {
        var colCount = this.option("colCount");
        var colCountByScreen = this.option("colCountByScreen");
        if (colCountByScreen) {
            var screenFactor = this.option("form").getTargetScreenFactor();
            if (!screenFactor) {
                screenFactor = hasWindow() ? getCurrentScreenFactor(this.option("screenByWidth")) : "lg"
            }
            colCount = colCountByScreen[screenFactor] || colCount
        }
        if ("auto" === colCount) {
            if (this._cashedColCount) {
                return this._cashedColCount
            }
            this._cashedColCount = colCount = this._getMaxColCount()
        }
        return colCount < 1 ? 1 : colCount
    },
    _getMaxColCount: function() {
        if (!hasWindow()) {
            return 1
        }
        var minColWidth = this.option("minColWidth");
        var width = getWidth(this.$element());
        var itemsCount = this._items.length;
        var maxColCount = Math.floor(width / minColWidth) || 1;
        return itemsCount < maxColCount ? itemsCount : maxColCount
    },
    isCachedColCountObsolete: function() {
        return this._cashedColCount && this._getMaxColCount() !== this._cashedColCount
    },
    _prepareItemsWithMerging: function(colCount) {
        var items = this._items.slice(0);
        var item;
        var itemsMergedByCol;
        var result = [];
        var j;
        var i;
        for (i = 0; i < items.length; i++) {
            item = items[i];
            result.push(item);
            if (this.option("alignItemLabels") || item.alignItemLabels || item.colSpan) {
                item.col = this._getColByIndex(result.length - 1, colCount)
            }
            if (item.colSpan > 1 && item.col + item.colSpan <= colCount) {
                itemsMergedByCol = [];
                for (j = 0; j < item.colSpan - 1; j++) {
                    itemsMergedByCol.push({
                        merged: true
                    })
                }
                result = result.concat(itemsMergedByCol)
            } else {
                delete item.colSpan
            }
        }
        this._setItems(result)
    },
    _getColByIndex: function(index, colCount) {
        return index % colCount
    },
    _setItems: function(items) {
        this._items = items;
        this._cashedColCount = null
    },
    _generateLayoutItems: function() {
        var items = this._items;
        var colCount = this._getColCount();
        var result = [];
        var item;
        var i;
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (!item.merged) {
                var generatedItem = {
                    location: {
                        row: parseInt(i / colCount),
                        col: this._getColByIndex(i, colCount)
                    }
                };
                if (isDefined(item.colSpan)) {
                    generatedItem.location.colspan = item.colSpan
                }
                if (isDefined(item.rowSpan)) {
                    generatedItem.location.rowspan = item.rowSpan
                }
                result.push(generatedItem)
            }
        }
        return result
    },
    _renderEmptyItem: function($container) {
        return $container.addClass(FIELD_EMPTY_ITEM_CLASS).html("&nbsp;")
    },
    _getButtonHorizontalAlignment: function(item) {
        if (isDefined(item.horizontalAlignment)) {
            return item.horizontalAlignment
        }
        return "right"
    },
    _getButtonVerticalAlignment: function(item) {
        switch (item.verticalAlignment) {
            case "center":
                return "center";
            case "bottom":
                return "flex-end";
            default:
                return "flex-start"
        }
    },
    _renderButtonItem: function(item, $container) {
        var $button = $("<div>").appendTo($container);
        var defaultOptions = {
            validationGroup: this.option("validationGroup")
        };
        $container.addClass(FIELD_BUTTON_ITEM_CLASS).css("textAlign", this._getButtonHorizontalAlignment(item));
        $container.parent().css("justifyContent", this._getButtonVerticalAlignment(item));
        var instance = this._createComponent($button, "dxButton", extend(defaultOptions, item.buttonOptions));
        this._itemsRunTimeInfo.add({
            item: item,
            widgetInstance: instance,
            guid: item.guid,
            $itemContainer: $container
        });
        this._addItemClasses($container, item.col);
        return $button
    },
    _addItemClasses: function($item, column) {
        $item.addClass(FIELD_ITEM_CLASS).addClass(this.option("cssItemClass")).addClass(isDefined(column) ? "dx-col-" + column : "")
    },
    _renderFieldItem: function(item, $container) {
        var name = this._getName(item);
        var id = this.getItemID(name);
        var isRequired = isDefined(item.isRequired) ? item.isRequired : !!this._hasRequiredRuleInSet(item.validationRules);
        var labelOptions = this._getLabelOptions(item, id, isRequired);
        var $editor = $("<div>");
        var helpID = item.helpText ? "dx-" + new Guid : null;
        var $label;
        this._addItemClasses($container, item.col);
        $container.addClass(isRequired ? FIELD_ITEM_REQUIRED_CLASS : FIELD_ITEM_OPTIONAL_CLASS);
        if (labelOptions.visible && labelOptions.text) {
            $label = this._renderLabel(labelOptions).appendTo($container)
        }
        if (item.itemType === SIMPLE_ITEM_TYPE) {
            if (this._isLabelNeedBaselineAlign(item) && "top" !== labelOptions.location) {
                $container.addClass(FIELD_ITEM_LABEL_ALIGN_CLASS)
            }
            this._hasBrowserFlex() && $container.addClass(FLEX_LAYOUT_CLASS)
        }
        $editor.data("dx-form-item", item);
        this._appendEditorToField({
            $fieldItem: $container,
            $label: $label,
            $editor: $editor,
            labelOptions: labelOptions
        });
        var instance = this._renderEditor({
            $container: $editor,
            dataField: item.dataField,
            name: item.name,
            editorType: item.editorType,
            editorOptions: item.editorOptions,
            template: this._getTemplateByFieldItem(item),
            isRequired: isRequired,
            helpID: helpID,
            labelID: labelOptions.labelID,
            id: id,
            validationBoundary: this.option("validationBoundary"),
            allowIndeterminateState: item.allowIndeterminateState
        });
        this._itemsRunTimeInfo.add({
            item: item,
            widgetInstance: instance,
            guid: item.guid,
            $itemContainer: $container
        });
        var editorElem = $editor.children().first();
        var $validationTarget = editorElem.hasClass(TEMPLATE_WRAPPER_CLASS) ? editorElem.children().first() : editorElem;
        var validationTargetInstance = $validationTarget && $validationTarget.data("dx-validation-target");
        if (validationTargetInstance) {
            this._renderValidator($validationTarget, item);
            if (isMaterial()) {
                this._addWrapperInvalidClass(validationTargetInstance)
            }
        }
        this._renderHelpText(item, $editor, helpID);
        this._attachClickHandler($label, $editor, item.editorType)
    },
    _hasRequiredRuleInSet: function(rules) {
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
    },
    _getName: function(item) {
        return item.dataField || item.name
    },
    _isLabelNeedBaselineAlign: function(item) {
        return !!item.helpText && !this._hasBrowserFlex() || -1 !== inArray(item.editorType, ["dxTextArea", "dxRadioGroup", "dxCalendar", "dxHtmlEditor"])
    },
    _isLabelNeedId: function(item) {
        return -1 !== inArray(item.editorType, ["dxRadioGroup", "dxCheckBox", "dxLookup", "dxSlider", "dxRangeSlider", "dxSwitch", "dxHtmlEditor"])
    },
    _getLabelOptions: function(item, id, isRequired) {
        var labelOptions = extend({
            showColon: this.option("showColonAfterLabel"),
            location: this.option("labelLocation"),
            id: id,
            visible: true,
            isRequired: isRequired
        }, item ? item.label : {});
        if (this._isLabelNeedId(item)) {
            labelOptions.labelID = "dx-label-".concat(new Guid)
        }
        if (!labelOptions.text && item.dataField) {
            labelOptions.text = captionize(item.dataField)
        }
        if (labelOptions.text) {
            labelOptions.text += labelOptions.showColon ? ":" : ""
        }
        return labelOptions
    },
    _renderLabel: function(options) {
        var {
            text: text,
            id: id,
            location: location,
            alignment: alignment,
            isRequired: isRequired,
            labelID: labelID = null
        } = options;
        if (isDefined(text) && text.length > 0) {
            var labelClasses = FIELD_ITEM_LABEL_CLASS + " " + FIELD_ITEM_LABEL_LOCATION_CLASS + location;
            var $label = $("<label>").addClass(labelClasses).attr("for", id).attr("id", labelID);
            var $labelContent = $("<span>").addClass(FIELD_ITEM_LABEL_CONTENT_CLASS).appendTo($label);
            $("<span>").addClass(FIELD_ITEM_LABEL_TEXT_CLASS).text(text).appendTo($labelContent);
            if (alignment) {
                $label.css("textAlign", alignment)
            }
            $labelContent.append(this._renderLabelMark(isRequired));
            return $label
        }
    },
    _renderLabelMark: function(isRequired) {
        var $mark;
        var requiredMarksConfig = this._getRequiredMarksConfig();
        var isRequiredMark = requiredMarksConfig.showRequiredMark && isRequired;
        var isOptionalMark = requiredMarksConfig.showOptionalMark && !isRequired;
        if (isRequiredMark || isOptionalMark) {
            var markClass = isRequiredMark ? FIELD_ITEM_REQUIRED_MARK_CLASS : FIELD_ITEM_OPTIONAL_MARK_CLASS;
            var markText = isRequiredMark ? requiredMarksConfig.requiredMark : requiredMarksConfig.optionalMark;
            $mark = $("<span>").addClass(markClass).html("&nbsp" + markText)
        }
        return $mark
    },
    _getRequiredMarksConfig: function() {
        if (!this._cashedRequiredConfig) {
            this._cashedRequiredConfig = {
                showRequiredMark: this.option("showRequiredMark"),
                showOptionalMark: this.option("showOptionalMark"),
                requiredMark: this.option("requiredMark"),
                optionalMark: this.option("optionalMark")
            }
        }
        return this._cashedRequiredConfig
    },
    _renderEditor: function(options) {
        var dataValue = this._getDataByField(options.dataField);
        var defaultEditorOptions = void 0 !== dataValue || this._isCheckboxUndefinedStateEnabled(options) ? {
            value: dataValue
        } : {};
        if (-1 !== EDITORS_WITH_ARRAY_VALUE.indexOf(options.editorType)) {
            defaultEditorOptions.value = defaultEditorOptions.value || []
        }
        var formInstance = this.option("form");
        var editorOptions = extend(true, defaultEditorOptions, options.editorOptions, {
            inputAttr: {
                id: options.id
            },
            validationBoundary: options.validationBoundary,
            stylingMode: formInstance && formInstance.option("stylingMode")
        });
        this._replaceDataOptions(options.editorOptions, editorOptions);
        var renderOptions = {
            editorType: options.editorType,
            dataField: options.dataField,
            template: options.template,
            name: options.name,
            helpID: options.helpID,
            labelID: options.labelID,
            isRequired: options.isRequired
        };
        return this._createEditor(options.$container, renderOptions, editorOptions)
    },
    _replaceDataOptions: function(originalOptions, resultOptions) {
        if (originalOptions) {
            DATA_OPTIONS.forEach((function(item) {
                if (resultOptions[item]) {
                    resultOptions[item] = originalOptions[item]
                }
            }))
        }
    },
    _renderValidator: function($editor, item) {
        var fieldName = this._getFieldLabelName(item);
        var validationRules = this._prepareValidationRules(item.validationRules, item.isRequired, item.itemType, fieldName);
        if (Array.isArray(validationRules) && validationRules.length) {
            this._createComponent($editor, Validator, {
                validationRules: validationRules,
                validationGroup: this.option("validationGroup"),
                dataGetter: function() {
                    return {
                        formItem: item
                    }
                }
            })
        }
    },
    _getFieldLabelName: function(item) {
        var isItemHaveCustomLabel = item.label && item.label.text;
        var itemName = isItemHaveCustomLabel ? null : this._getName(item);
        return isItemHaveCustomLabel ? item.label.text : itemName && captionize(itemName)
    },
    _prepareValidationRules: function(userValidationRules, isItemRequired, itemType, itemName) {
        var isSimpleItem = itemType === SIMPLE_ITEM_TYPE;
        var validationRules;
        if (isSimpleItem) {
            if (userValidationRules) {
                validationRules = userValidationRules
            } else {
                var requiredMessage = format(this.option("requiredMessage"), itemName || "");
                validationRules = isItemRequired ? [{
                    type: "required",
                    message: requiredMessage
                }] : null
            }
        }
        return validationRules
    },
    _addWrapperInvalidClass: function(editorInstance) {
        var wrapperClass = "." + FIELD_ITEM_CONTENT_WRAPPER_CLASS;
        var toggleInvalidClass = function(e) {
            $(e.element).parents(wrapperClass).toggleClass(INVALID_CLASS, e.component._isFocused() && false === e.component.option("isValid"))
        };
        editorInstance.on("focusIn", toggleInvalidClass).on("focusOut", toggleInvalidClass).on("enterKey", toggleInvalidClass)
    },
    _createEditor: function($container, renderOptions, editorOptions) {
        var template = renderOptions.template;
        var editorInstance;
        if (renderOptions.dataField && !editorOptions.name) {
            editorOptions.name = renderOptions.dataField
        }
        this._addItemContentClasses($container);
        if (template) {
            var data = {
                dataField: renderOptions.dataField,
                editorType: renderOptions.editorType,
                editorOptions: editorOptions,
                component: this._getComponentOwner(),
                name: renderOptions.name
            };
            template.render({
                model: data,
                container: getPublicElement($container)
            })
        } else {
            var $editor = $("<div>").appendTo($container);
            try {
                editorInstance = this._createComponent($editor, renderOptions.editorType, editorOptions);
                editorInstance.setAria("describedby", renderOptions.helpID);
                editorInstance.setAria("labelledby", renderOptions.labelID);
                editorInstance.setAria("required", renderOptions.isRequired);
                if (renderOptions.dataField) {
                    this._bindDataField(editorInstance, renderOptions, $container)
                }
            } catch (e) {
                errors.log("E1035", e.message)
            }
        }
        return editorInstance
    },
    _getComponentOwner: function() {
        return this.option("form") || this
    },
    _bindDataField: function(editorInstance, renderOptions, $container) {
        var componentOwner = this._getComponentOwner();
        editorInstance.on("enterKey", (function(args) {
            componentOwner._createActionByOption("onEditorEnterKey")(extend(args, {
                dataField: renderOptions.dataField
            }))
        }));
        this._createWatcher(editorInstance, $container, renderOptions);
        this.linkEditorToDataField(editorInstance, renderOptions.dataField, renderOptions.editorType)
    },
    _createWatcher: function(editorInstance, $container, renderOptions) {
        var that = this;
        var watch = that._getWatch();
        if (!isFunction(watch)) {
            return
        }
        var dispose = watch((function() {
            return that._getDataByField(renderOptions.dataField)
        }), (function() {
            editorInstance.option("value", that._getDataByField(renderOptions.dataField))
        }), {
            deep: true,
            skipImmediate: true
        });
        eventsEngine.on($container, removeEvent, dispose)
    },
    _getWatch: function() {
        if (!isDefined(this._watch)) {
            var formInstance = this.option("form");
            this._watch = formInstance && formInstance.option("integrationOptions.watchMethod")
        }
        return this._watch
    },
    _addItemContentClasses: function($itemContent) {
        var locationSpecificClass = this._getItemContentLocationSpecificClass();
        $itemContent.addClass([FIELD_ITEM_CONTENT_CLASS, locationSpecificClass].join(" "))
    },
    _getItemContentLocationSpecificClass: function() {
        var labelLocation = this.option("labelLocation");
        return FIELD_ITEM_CONTENT_LOCATION_CLASS + {
            right: "left",
            left: "right",
            top: "bottom"
        } [labelLocation]
    },
    _createComponent: function($editor, type, editorOptions) {
        var readOnlyState = this.option("readOnly");
        var instance = this.callBase($editor, type, editorOptions);
        readOnlyState && instance.option("readOnly", readOnlyState);
        this.on("optionChanged", (function(args) {
            if ("readOnly" === args.name && !isDefined(editorOptions.readOnly)) {
                instance.option(args.name, args.value)
            }
        }));
        return instance
    },
    _getTemplateByFieldItem: function(fieldItem) {
        return fieldItem.template ? this._getTemplate(fieldItem.template) : null
    },
    _appendEditorToField: function(params) {
        if (params.$label) {
            var location = params.labelOptions.location;
            if ("top" === location || "left" === location) {
                params.$fieldItem.append(params.$editor)
            }
            if ("right" === location) {
                params.$fieldItem.prepend(params.$editor)
            }
            this._addInnerItemAlignmentClass(params.$fieldItem, location)
        } else {
            params.$fieldItem.append(params.$editor)
        }
    },
    _addInnerItemAlignmentClass: function($fieldItem, location) {
        if ("top" === location) {
            $fieldItem.addClass(LABEL_VERTICAL_ALIGNMENT_CLASS)
        } else {
            $fieldItem.addClass(LABEL_HORIZONTAL_ALIGNMENT_CLASS)
        }
    },
    _renderHelpText: function(fieldItem, $editor, helpID) {
        var helpText = fieldItem.helpText;
        var isSimpleItem = fieldItem.itemType === SIMPLE_ITEM_TYPE;
        if (helpText && isSimpleItem) {
            var $editorWrapper = $("<div>").addClass(FIELD_ITEM_CONTENT_WRAPPER_CLASS);
            $editor.wrap($editorWrapper);
            $("<div>").addClass(FIELD_ITEM_HELP_TEXT_CLASS).attr("id", helpID).text(helpText).appendTo($editor.parent())
        }
    },
    _attachClickHandler: function($label, $editor, editorType) {
        var isBooleanEditors = "dxCheckBox" === editorType || "dxSwitch" === editorType;
        if ($label && isBooleanEditors) {
            eventsEngine.on($label, clickEventName, (function() {
                eventsEngine.trigger($editor.children(), clickEventName)
            }))
        }
    },
    _generateRatio: function(count, isAutoSize) {
        var result = [];
        var ratio;
        var i;
        for (i = 0; i < count; i++) {
            ratio = {
                ratio: 1
            };
            if (isAutoSize) {
                ratio.baseSize = "auto"
            }
            result.push(ratio)
        }
        return result
    },
    _getRowsCount: function() {
        return Math.ceil(this._items.length / this._getColCount())
    },
    _updateReferencedOptions: function(newLayoutData) {
        var layoutData = this.option("layoutData");
        if (isObject(layoutData)) {
            Object.getOwnPropertyNames(layoutData).forEach(property => delete this._optionsByReference["layoutData." + property])
        }
        if (isObject(newLayoutData)) {
            Object.getOwnPropertyNames(newLayoutData).forEach(property => this._optionsByReference["layoutData." + property] = true)
        }
    },
    _resetWidget(instance) {
        this._disableEditorValueChangedHandler = true;
        instance.reset();
        this._disableEditorValueChangedHandler = false;
        instance.option("isValid", true)
    },
    _optionChanged(args) {
        if (0 === args.fullName.search("layoutData.")) {
            return
        }
        switch (args.name) {
            case "showRequiredMark":
            case "showOptionalMark":
            case "requiredMark":
            case "optionalMark":
                this._cashedRequiredConfig = null;
                this._invalidate();
                break;
            case "layoutData":
                this._updateReferencedOptions(args.value);
                if (this.option("items")) {
                    if (!isEmptyObject(args.value)) {
                        this._itemsRunTimeInfo.each((_, itemRunTimeInfo) => {
                            if (isDefined(itemRunTimeInfo.item)) {
                                var dataField = itemRunTimeInfo.item.dataField;
                                if (dataField && isDefined(itemRunTimeInfo.widgetInstance)) {
                                    var valueGetter = compileGetter(dataField);
                                    var dataValue = valueGetter(args.value);
                                    if (void 0 !== dataValue || this._isCheckboxUndefinedStateEnabled(itemRunTimeInfo.item)) {
                                        itemRunTimeInfo.widgetInstance.option("value", dataValue)
                                    } else {
                                        this._resetWidget(itemRunTimeInfo.widgetInstance)
                                    }
                                }
                            }
                        })
                    }
                } else {
                    this._initDataAndItems(args.value);
                    this._invalidate()
                }
                break;
            case "items":
                this._cleanItemWatchers();
                this._initDataAndItems(args.value);
                this._invalidate();
                break;
            case "alignItemLabels":
            case "labelLocation":
            case "requiredMessage":
                this._invalidate();
                break;
            case "customizeItem":
                this._updateItems(this.option("layoutData"));
                this._invalidate();
                break;
            case "colCount":
                this._resetColCount();
                break;
            case "minColWidth":
                if ("auto" === this.option("colCount")) {
                    this._resetColCount()
                }
                break;
            case "readOnly":
                break;
            case "width":
                this.callBase(args);
                if ("auto" === this.option("colCount")) {
                    this._resetColCount()
                }
                break;
            case "onFieldDataChanged":
                break;
            default:
                this.callBase(args)
        }
    },
    _resetColCount: function() {
        this._cashedColCount = null;
        this._invalidate()
    },
    linkEditorToDataField(editorInstance, dataField) {
        this.on("optionChanged", args => {
            if (args.fullName === "layoutData.".concat(dataField)) {
                editorInstance._setOptionWithoutOptionChange("value", args.value)
            }
        });
        editorInstance.on("valueChanged", args => {
            var isValueReferenceType = isObject(args.value) || Array.isArray(args.value);
            if (!this._disableEditorValueChangedHandler && !(isValueReferenceType && args.value === args.previousValue)) {
                this._updateFieldValue(dataField, args.value)
            }
        })
    },
    _dimensionChanged: function() {
        if ("auto" === this.option("colCount") && this.isCachedColCountObsolete()) {
            this._eventsStrategy.fireEvent("autoColCountChanged")
        }
    },
    getItemID: function(name) {
        var formInstance = this.option("form");
        return formInstance && formInstance.getItemID(name)
    },
    updateData: function(data, value) {
        var that = this;
        if (isObject(data)) {
            each(data, (function(dataField, fieldValue) {
                that._updateFieldValue(dataField, fieldValue)
            }))
        } else if ("string" === typeof data) {
            that._updateFieldValue(data, value)
        }
    },
    getEditor: function(field) {
        return this._itemsRunTimeInfo.findWidgetInstanceByDataField(field) || this._itemsRunTimeInfo.findWidgetInstanceByName(field)
    },
    isSingleColumnMode: function(component) {
        var responsiveBox = this._responsiveBox || component;
        if (responsiveBox) {
            return responsiveBox.option("currentScreenFactor") === responsiveBox.option("singleColumnScreen")
        }
    },
    getItemsRunTimeInfo: function() {
        return this._itemsRunTimeInfo
    }
});
registerComponent("dxLayoutManager", LayoutManager);
export default LayoutManager;
