/**
 * DevExtreme (cjs/ui/form/ui.form.layout_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _uiForm = _interopRequireDefault(require("./ui.form.items_runtime_info"));
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _type = require("../../core/utils/type");
var _variable_wrapper = _interopRequireDefault(require("../../core/utils/variable_wrapper"));
var _window = require("../../core/utils/window");
var _iterator = require("../../core/utils/iterator");
var _extend = require("../../core/utils/extend");
var _array = require("../../core/utils/array");
var _data = require("../../core/utils/data");
var _remove = require("../../events/remove");
var _message = _interopRequireDefault(require("../../localization/message"));
var _style = require("../../core/utils/style");
var _ui = _interopRequireDefault(require("../widget/ui.widget"));
var _responsive_box = _interopRequireDefault(require("../responsive_box"));
var _constants = require("./constants");
require("../text_box");
require("../number_box");
require("../check_box");
require("../date_box");
require("../button");
var _field_item = require("./components/field_item");
var _button_item = require("./components/button_item");
var _empty_item = require("./components/empty_item");
var _uiFormLayout_manager = require("./ui.form.layout_manager.utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}
var FORM_EDITOR_BY_DEFAULT = "dxTextBox";
var LAYOUT_MANAGER_FIRST_ROW_CLASS = "dx-first-row";
var LAYOUT_MANAGER_LAST_ROW_CLASS = "dx-last-row";
var LAYOUT_MANAGER_FIRST_COL_CLASS = "dx-first-col";
var LAYOUT_MANAGER_LAST_COL_CLASS = "dx-last-col";
var LAYOUT_STRATEGY_FLEX = "flex";
var LAYOUT_STRATEGY_FALLBACK = "fallback";
var LayoutManager = _ui.default.inherit({
    _getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
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
            optionalMark: _message.default.format("dxForm-optionalMark"),
            requiredMessage: _message.default.getFormatter("dxForm-requiredMessage")
        })
    },
    _setOptionsByReference: function() {
        this.callBase();
        (0, _extend.extend)(this._optionsByReference, {
            layoutData: true,
            validationGroup: true
        })
    },
    _init: function() {
        var layoutData = this.option("layoutData");
        this.callBase();
        this._itemWatchers = [];
        this._itemsRunTimeInfo = new _uiForm.default;
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
        var _this = this;
        var layoutData = this.option("layoutData");
        var userItems = this.option("items");
        if ((0, _type.isDefined)(userItems)) {
            userItems.forEach((function(item) {
                if (item.dataField && void 0 === _this._getDataByField(item.dataField)) {
                    var value;
                    if (item.editorOptions) {
                        value = item.editorOptions.value
                    }
                    if ((0, _type.isDefined)(value) || item.dataField in layoutData) {
                        _this._updateFieldValue(item.dataField, value)
                    }
                }
            }))
        }
    },
    _getDataByField: function(dataField) {
        return dataField ? this.option("layoutData." + dataField) : null
    },
    _isCheckboxUndefinedStateEnabled: function(_ref) {
        var allowIndeterminateState = _ref.allowIndeterminateState,
            editorType = _ref.editorType,
            dataField = _ref.dataField;
        if (true === allowIndeterminateState && "dxCheckBox" === editorType) {
            var nameParts = ["layoutData"].concat(_toConsumableArray(dataField.split(".")));
            var propertyName = nameParts.pop();
            var layoutData = this.option(nameParts.join("."));
            return layoutData && propertyName in layoutData
        }
        return false
    },
    _updateFieldValue: function(dataField, value) {
        var layoutData = this.option("layoutData");
        var newValue = value;
        if (!_variable_wrapper.default.isWrapped(layoutData[dataField]) && (0, _type.isDefined)(dataField)) {
            this.option("layoutData." + dataField, newValue)
        } else if (_variable_wrapper.default.isWritableWrapped(layoutData[dataField])) {
            newValue = (0, _type.isFunction)(newValue) ? newValue() : newValue;
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
        var isUserItemsExist = (0, _type.isDefined)(userItems);
        var customizeItem = that.option("customizeItem");
        var items = isUserItemsExist ? userItems : this._generateItemsByData(layoutData);
        if ((0, _type.isDefined)(items)) {
            var processedItems = [];
            (0, _iterator.each)(items, (function(index, item) {
                if (that._isAcceptableItem(item)) {
                    item = that._processItem(item);
                    customizeItem && customizeItem(item);
                    if ((0, _type.isObject)(item) && false !== _variable_wrapper.default.unwrap(item.visible)) {
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
            if ((0, _type.isObject)(item) && (0, _type.isDefined)(item.visible) && (0, _type.isFunction)(watch)) {
                that._itemWatchers.push(watch((function() {
                    return _variable_wrapper.default.unwrap(item.visible)
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
        if ((0, _type.isDefined)(layoutData)) {
            (0, _iterator.each)(layoutData, (function(dataField) {
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
        return !((0, _type.isFunction)(itemData) && !_variable_wrapper.default.isWrapped(itemData))
    },
    _processItem: function(item) {
        if ("string" === typeof item) {
            item = {
                dataField: item
            }
        }
        if ("object" === _typeof(item) && !item.itemType) {
            item.itemType = _constants.SIMPLE_ITEM_TYPE
        }
        if (!(0, _type.isDefined)(item.editorType) && (0, _type.isDefined)(item.dataField)) {
            var value = this._getDataByField(item.dataField);
            item.editorType = (0, _type.isDefined)(value) ? this._getEditorTypeByDataType((0, _type.type)(value)) : FORM_EDITOR_BY_DEFAULT
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
        (0, _array.normalizeIndexes)(this._items, "visibleIndex");
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
        this.$element().addClass(_constants.FORM_LAYOUT_MANAGER_CLASS);
        this.callBase();
        this._renderResponsiveBox()
    },
    _hasBrowserFlex: function() {
        return (0, _style.styleProp)(LAYOUT_STRATEGY_FLEX) === LAYOUT_STRATEGY_FLEX
    },
    _renderResponsiveBox: function() {
        var templatesInfo = [];
        if (this._items && this._items.length) {
            var colCount = this._getColCount();
            var $container = (0, _renderer.default)("<div>").appendTo(this.$element());
            this._prepareItemsWithMerging(colCount);
            var layoutItems = this._generateLayoutItems();
            this._responsiveBox = this._createComponent($container, _responsive_box.default, this._getResponsiveBoxConfig(layoutItems, colCount, templatesInfo));
            if (!(0, _window.hasWindow)()) {
                this._renderTemplates(templatesInfo)
            }
        }
    },
    _itemStateChangedHandler: function(e) {
        this._refresh()
    },
    _renderTemplates: function(templatesInfo) {
        var that = this;
        (0, _iterator.each)(templatesInfo, (function(_, info) {
            switch (info.itemType) {
                case "empty":
                    (0, _empty_item.renderEmptyItem)(info);
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
                    that.$element().toggleClass(_constants.LAYOUT_MANAGER_ONE_COLUMN, isSingleColumnMode);
                    onLayoutChanged(isSingleColumnMode)
                }
            },
            onContentReady: function(e) {
                if ((0, _window.hasWindow)()) {
                    that._renderTemplates(templatesInfo)
                }
                if (that.option("onLayoutChanged")) {
                    that.$element().toggleClass(_constants.LAYOUT_MANAGER_ONE_COLUMN, that.isSingleColumnMode(e.component))
                }
            },
            itemTemplate: function(e, itemData, itemElement) {
                if (!e.location) {
                    return
                }
                var $itemElement = (0, _renderer.default)(itemElement);
                var itemRenderedCountInPreviousRows = e.location.row * colCount;
                var item = that._items[e.location.col + itemRenderedCountInPreviousRows];
                if (!item) {
                    return
                }
                var itemCssClassList = [item.cssClass];
                $itemElement.toggleClass(_constants.SINGLE_COLUMN_ITEM_CONTENT, that.isSingleColumnMode(this));
                if (0 === e.location.row) {
                    itemCssClassList.push(LAYOUT_MANAGER_FIRST_ROW_CLASS)
                }
                if (0 === e.location.col) {
                    itemCssClassList.push(LAYOUT_MANAGER_FIRST_COL_CLASS)
                }
                if (item.itemType === _constants.SIMPLE_ITEM_TYPE && that.option("isRoot")) {
                    $itemElement.addClass(_constants.ROOT_SIMPLE_ITEM_CLASS)
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
                    itemCssClassList.push(_constants.FIELD_ITEM_CLASS);
                    itemCssClassList.push(that.option("cssItemClass"));
                    if ((0, _type.isDefined)(item.col)) {
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
                screenFactor = (0, _window.hasWindow)() ? (0, _window.getCurrentScreenFactor)(this.option("screenByWidth")) : "lg"
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
        if (!(0, _window.hasWindow)()) {
            return 1
        }
        var minColWidth = this.option("minColWidth");
        var width = (0, _size.getWidth)(this.$element());
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
                if ((0, _type.isDefined)(item.disabled)) {
                    generatedItem.disabled = item.disabled
                }
                if ((0, _type.isDefined)(item.visible)) {
                    generatedItem.visible = item.visible
                }
                if ((0, _type.isDefined)(item.colSpan)) {
                    generatedItem.location.colspan = item.colSpan
                }
                if ((0, _type.isDefined)(item.rowSpan)) {
                    generatedItem.location.rowspan = item.rowSpan
                }
                result.push(generatedItem)
            }
        }
        return result
    },
    _renderEmptyItem: function($container) {
        (0, _empty_item.renderEmptyItem)({
            $container: $container
        })
    },
    _renderButtonItem: function(_ref2) {
        var item = _ref2.item,
            $parent = _ref2.$parent,
            rootElementCssClassList = _ref2.rootElementCssClassList;
        var _renderButtonItem2 = (0, _button_item.renderButtonItem)({
                item: item,
                $parent: $parent,
                rootElementCssClassList: rootElementCssClassList,
                validationGroup: this.option("validationGroup"),
                createComponentCallback: this._createComponent.bind(this)
            }),
            $rootElement = _renderButtonItem2.$rootElement,
            buttonInstance = _renderButtonItem2.buttonInstance;
        this._itemsRunTimeInfo.add({
            item: item,
            widgetInstance: buttonInstance,
            guid: item.guid,
            $itemContainer: $rootElement
        })
    },
    _renderFieldItem: function(_ref3) {
        var _this$option;
        var item = _ref3.item,
            $parent = _ref3.$parent,
            rootElementCssClassList = _ref3.rootElementCssClassList;
        var editorValue = this._getDataByField(item.dataField);
        var canAssignUndefinedValueToEditor = false;
        if (void 0 === editorValue) {
            var allowIndeterminateState = item.allowIndeterminateState,
                editorType = item.editorType,
                dataField = item.dataField;
            canAssignUndefinedValueToEditor = this._isCheckboxUndefinedStateEnabled({
                allowIndeterminateState: allowIndeterminateState,
                editorType: editorType,
                dataField: dataField
            })
        }
        var name = item.dataField || item.name;
        var _renderFieldItem2 = (0, _field_item.renderFieldItem)((0, _uiFormLayout_manager.convertToRenderFieldItemOptions)({
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
            })),
            $fieldEditorContainer = _renderFieldItem2.$fieldEditorContainer,
            widgetInstance = _renderFieldItem2.widgetInstance,
            $rootElement = _renderFieldItem2.$rootElement;
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
            formOrThis._createActionByOption("onEditorEnterKey")((0, _extend.extend)(args, {
                dataField: dataField
            }))
        }));
        this._createWatcher(editorInstance, $container, dataField);
        this.linkEditorToDataField(editorInstance, dataField, editorType)
    },
    _createWatcher: function(editorInstance, $container, dataField) {
        var that = this;
        var watch = that._getWatch();
        if (!(0, _type.isFunction)(watch)) {
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
        _events_engine.default.on($container, _remove.removeEvent, dispose)
    },
    _getWatch: function() {
        if (!(0, _type.isDefined)(this._watch)) {
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
            if ("readOnly" === args.name && !(0, _type.isDefined)(editorOptions.readOnly)) {
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
        var _this2 = this;
        var layoutData = this.option("layoutData");
        if ((0, _type.isObject)(layoutData)) {
            Object.getOwnPropertyNames(layoutData).forEach((function(property) {
                return delete _this2._optionsByReference["layoutData." + property]
            }))
        }
        if ((0, _type.isObject)(newLayoutData)) {
            Object.getOwnPropertyNames(newLayoutData).forEach((function(property) {
                return _this2._optionsByReference["layoutData." + property] = true
            }))
        }
    },
    _resetWidget: function(instance) {
        this._disableEditorValueChangedHandler = true;
        instance.reset();
        this._disableEditorValueChangedHandler = false;
        instance.option("isValid", true)
    },
    _optionChanged: function(args) {
        var _this3 = this;
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
                    if (!(0, _type.isEmptyObject)(args.value)) {
                        this._itemsRunTimeInfo.each((function(_, itemRunTimeInfo) {
                            if ((0, _type.isDefined)(itemRunTimeInfo.item)) {
                                var dataField = itemRunTimeInfo.item.dataField;
                                if (dataField && (0, _type.isDefined)(itemRunTimeInfo.widgetInstance)) {
                                    var valueGetter = (0, _data.compileGetter)(dataField);
                                    var dataValue = valueGetter(args.value);
                                    var _itemRunTimeInfo$item = itemRunTimeInfo.item,
                                        allowIndeterminateState = _itemRunTimeInfo$item.allowIndeterminateState,
                                        editorType = _itemRunTimeInfo$item.editorType;
                                    if (void 0 !== dataValue || _this3._isCheckboxUndefinedStateEnabled({
                                            allowIndeterminateState: allowIndeterminateState,
                                            editorType: editorType,
                                            dataField: dataField
                                        })) {
                                        itemRunTimeInfo.widgetInstance.option("value", dataValue)
                                    } else {
                                        _this3._resetWidget(itemRunTimeInfo.widgetInstance)
                                    }
                                }
                            }
                        }))
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
    linkEditorToDataField: function(editorInstance, dataField) {
        var _this4 = this;
        this.on("optionChanged", (function(args) {
            if (args.fullName === "layoutData.".concat(dataField)) {
                editorInstance._setOptionWithoutOptionChange("value", args.value)
            }
        }));
        editorInstance.on("valueChanged", (function(args) {
            var isValueReferenceType = (0, _type.isObject)(args.value) || Array.isArray(args.value);
            if (!_this4._disableEditorValueChangedHandler && !(isValueReferenceType && args.value === args.previousValue)) {
                _this4._updateFieldValue(dataField, args.value)
            }
        }))
    },
    _dimensionChanged: function() {
        if ("auto" === this.option("colCount") && this.isCachedColCountObsolete()) {
            this._eventsStrategy.fireEvent("autoColCountChanged")
        }
    },
    updateData: function(data, value) {
        var that = this;
        if ((0, _type.isObject)(data)) {
            (0, _iterator.each)(data, (function(dataField, fieldValue) {
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
(0, _component_registrator.default)("dxLayoutManager", LayoutManager);
var _default = LayoutManager;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
