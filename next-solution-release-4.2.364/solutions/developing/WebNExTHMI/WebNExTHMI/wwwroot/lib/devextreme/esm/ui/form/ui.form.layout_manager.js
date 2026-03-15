/**
 * DevExtreme (esm/ui/form/ui.form.layout_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
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
import variableWrapper from "../../core/utils/variable_wrapper";
import {
    getCurrentScreenFactor,
    hasWindow
} from "../../core/utils/window";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    normalizeIndexes
} from "../../core/utils/array";
import {
    compileGetter
} from "../../core/utils/data";
import {
    removeEvent
} from "../../events/remove";
import messageLocalization from "../../localization/message";
import {
    styleProp
} from "../../core/utils/style";
import Widget from "../widget/ui.widget";
import ResponsiveBox from "../responsive_box";
import {
    LAYOUT_MANAGER_ONE_COLUMN,
    FORM_LAYOUT_MANAGER_CLASS,
    SINGLE_COLUMN_ITEM_CONTENT,
    ROOT_SIMPLE_ITEM_CLASS,
    SIMPLE_ITEM_TYPE,
    FIELD_ITEM_CLASS
} from "./constants";
import "../text_box";
import "../number_box";
import "../check_box";
import "../date_box";
import "../button";
import {
    renderFieldItem
} from "./components/field_item";
import {
    renderButtonItem
} from "./components/button_item";
import {
    renderEmptyItem
} from "./components/empty_item";
import {
    convertToRenderFieldItemOptions
} from "./ui.form.layout_manager.utils";
var FORM_EDITOR_BY_DEFAULT = "dxTextBox";
var LAYOUT_MANAGER_FIRST_ROW_CLASS = "dx-first-row";
var LAYOUT_MANAGER_LAST_ROW_CLASS = "dx-last-row";
var LAYOUT_MANAGER_FIRST_COL_CLASS = "dx-first-col";
var LAYOUT_MANAGER_LAST_COL_CLASS = "dx-last-col";
var LAYOUT_STRATEGY_FLEX = "flex";
var LAYOUT_STRATEGY_FALLBACK = "fallback";
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
            screenByWidth: null,
            showOptionalMark: false,
            requiredMark: "*",
            labelMode: "outside",
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
    _isCheckboxUndefinedStateEnabled: function(_ref) {
        var {
            allowIndeterminateState: allowIndeterminateState,
            editorType: editorType,
            dataField: dataField
        } = _ref;
        if (true === allowIndeterminateState && "dxCheckBox" === editorType) {
            var nameParts = ["layoutData", ...dataField.split(".")];
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
        this.$element().addClass(FORM_LAYOUT_MANAGER_CLASS);
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
            var $container = $("<div>").appendTo(this.$element());
            this._prepareItemsWithMerging(colCount);
            var layoutItems = this._generateLayoutItems();
            this._responsiveBox = this._createComponent($container, ResponsiveBox, this._getResponsiveBoxConfig(layoutItems, colCount, templatesInfo));
            if (!hasWindow()) {
                this._renderTemplates(templatesInfo)
            }
        }
    },
    _itemStateChangedHandler: function(e) {
        this._refresh()
    },
    _renderTemplates: function(templatesInfo) {
        var that = this;
        each(templatesInfo, (function(_, info) {
            switch (info.itemType) {
                case "empty":
                    renderEmptyItem(info);
                    break;
                case "button":
                    that._renderButtonItem(info);
                    break;
                default:
                    that._renderFieldItem(info)
            }
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
                if (!item) {
                    return
                }
                var itemCssClassList = [item.cssClass];
                $itemElement.toggleClass(SINGLE_COLUMN_ITEM_CONTENT, that.isSingleColumnMode(this));
                if (0 === e.location.row) {
                    itemCssClassList.push(LAYOUT_MANAGER_FIRST_ROW_CLASS)
                }
                if (0 === e.location.col) {
                    itemCssClassList.push(LAYOUT_MANAGER_FIRST_COL_CLASS)
                }
                if (item.itemType === SIMPLE_ITEM_TYPE && that.option("isRoot")) {
                    $itemElement.addClass(ROOT_SIMPLE_ITEM_CLASS)
                }
                var isLastColumn = e.location.col === colCount - 1 || e.location.col + e.location.colspan === colCount;
                var rowsCount = that._getRowsCount();
                var isLastRow = e.location.row === rowsCount - 1;
                if (isLastColumn) {
                    itemCssClassList.push(LAYOUT_MANAGER_LAST_COL_CLASS)
                }
                if (isLastRow) {
                    itemCssClassList.push(LAYOUT_MANAGER_LAST_ROW_CLASS)
                }
                if ("empty" !== item.itemType) {
                    itemCssClassList.push(FIELD_ITEM_CLASS);
                    itemCssClassList.push(that.option("cssItemClass"));
                    if (isDefined(item.col)) {
                        itemCssClassList.push("dx-col-" + item.col)
                    }
                }
                templatesInfo.push({
                    itemType: item.itemType,
                    item: item,
                    $parent: $itemElement,
                    rootElementCssClassList: itemCssClassList
                })
            },
            cols: that._generateRatio(colCount),
            rows: that._generateRatio(that._getRowsCount(), true),
            dataSource: layoutItems,
            screenByWidth: that.option("screenByWidth"),
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
                if (isDefined(item.disabled)) {
                    generatedItem.disabled = item.disabled
                }
                if (isDefined(item.visible)) {
                    generatedItem.visible = item.visible
                }
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
        renderEmptyItem({
            $container: $container
        })
    },
    _renderButtonItem: function(_ref2) {
        var {
            item: item,
            $parent: $parent,
            rootElementCssClassList: rootElementCssClassList
        } = _ref2;
        var {
            $rootElement: $rootElement,
            buttonInstance: buttonInstance
        } = renderButtonItem({
            item: item,
            $parent: $parent,
            rootElementCssClassList: rootElementCssClassList,
            validationGroup: this.option("validationGroup"),
            createComponentCallback: this._createComponent.bind(this)
        });
        this._itemsRunTimeInfo.add({
            item: item,
            widgetInstance: buttonInstance,
            guid: item.guid,
            $itemContainer: $rootElement
        })
    },
    _renderFieldItem: function(_ref3) {
        var _this$option;
        var {
            item: item,
            $parent: $parent,
            rootElementCssClassList: rootElementCssClassList
        } = _ref3;
        var editorValue = this._getDataByField(item.dataField);
        var canAssignUndefinedValueToEditor = false;
        if (void 0 === editorValue) {
            var {
                allowIndeterminateState: allowIndeterminateState,
                editorType: editorType,
                dataField: dataField
            } = item;
            canAssignUndefinedValueToEditor = this._isCheckboxUndefinedStateEnabled({
                allowIndeterminateState: allowIndeterminateState,
                editorType: editorType,
                dataField: dataField
            })
        }
        var name = item.dataField || item.name;
        var {
            $fieldEditorContainer: $fieldEditorContainer,
            widgetInstance: widgetInstance,
            $rootElement: $rootElement
        } = renderFieldItem(convertToRenderFieldItemOptions({
            $parent: $parent,
            rootElementCssClassList: rootElementCssClassList,
            item: item,
            name: name,
            editorValue: editorValue,
            canAssignUndefinedValueToEditor: canAssignUndefinedValueToEditor,
            formOrLayoutManager: this._getFormOrThis(),
            createComponentCallback: this._createComponent.bind(this),
            useFlexLayout: this._hasBrowserFlex(),
            formLabelLocation: this.option("labelLocation"),
            requiredMessageTemplate: this.option("requiredMessage"),
            validationGroup: this.option("validationGroup"),
            editorValidationBoundary: this.option("validationBoundary"),
            editorStylingMode: this.option("form") && this.option("form").option("stylingMode"),
            showColonAfterLabel: this.option("showColonAfterLabel"),
            managerLabelLocation: this.option("labelLocation"),
            template: item.template ? this._getTemplate(item.template) : null,
            itemId: this.option("form") && this.option("form").getItemID(name),
            managerMarkOptions: this._getMarkOptions(),
            labelMode: this.option("labelMode")
        }));
        null === (_this$option = this.option("onFieldItemRendered")) || void 0 === _this$option ? void 0 : _this$option();
        if (widgetInstance && item.dataField) {
            this._bindDataField(widgetInstance, item.dataField, item.editorType, $fieldEditorContainer)
        }
        this._itemsRunTimeInfo.add({
            item: item,
            widgetInstance: widgetInstance,
            guid: item.guid,
            $itemContainer: $rootElement
        })
    },
    _getMarkOptions: function() {
        return {
            showRequiredMark: this.option("showRequiredMark"),
            requiredMark: this.option("requiredMark"),
            showOptionalMark: this.option("showOptionalMark"),
            optionalMark: this.option("optionalMark")
        }
    },
    _getFormOrThis: function() {
        return this.option("form") || this
    },
    _bindDataField: function(editorInstance, dataField, editorType, $container) {
        var formOrThis = this._getFormOrThis();
        editorInstance.on("enterKey", (function(args) {
            formOrThis._createActionByOption("onEditorEnterKey")(extend(args, {
                dataField: dataField
            }))
        }));
        this._createWatcher(editorInstance, $container, dataField);
        this.linkEditorToDataField(editorInstance, dataField, editorType)
    },
    _createWatcher: function(editorInstance, $container, dataField) {
        var that = this;
        var watch = that._getWatch();
        if (!isFunction(watch)) {
            return
        }
        var dispose = watch((function() {
            return that._getDataByField(dataField)
        }), (function() {
            var fieldValue = that._getDataByField(dataField);
            if ("dxTagBox" === editorInstance.NAME) {
                var editorValue = editorInstance.option("value");
                if (fieldValue !== editorValue && function(array1, array2) {
                        if (!Array.isArray(array1) || !Array.isArray(array2) || array1.length !== array2.length) {
                            return false
                        }
                        for (var i = 0; i < array1.length; i++) {
                            if (array1[i] !== array2[i]) {
                                return false
                            }
                        }
                        return true
                    }(fieldValue, editorValue)) {
                    return
                }
            }
            editorInstance.option("value", fieldValue)
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
                                    var {
                                        allowIndeterminateState: allowIndeterminateState,
                                        editorType: editorType
                                    } = itemRunTimeInfo.item;
                                    if (void 0 !== dataValue || this._isCheckboxUndefinedStateEnabled({
                                            allowIndeterminateState: allowIndeterminateState,
                                            editorType: editorType,
                                            dataField: dataField
                                        })) {
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
            case "labelMode":
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
