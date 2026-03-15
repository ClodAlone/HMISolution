/**
 * DevExtreme (esm/renovation/ui/form/_toDelete/ui.form.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import registerComponent from "../../core/component_registrator";
import Guid from "../../core/guid";
import {
    ensureDefined
} from "../../core/utils/common";
import {
    isDefined,
    isEmptyObject,
    isObject,
    isString
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    inArray
} from "../../core/utils/array";
import {
    extend
} from "../../core/utils/extend";
import {
    isEmpty
} from "../../core/utils/string";
import {
    triggerResizeEvent,
    triggerShownEvent
} from "../../events/visibility_change";
import {
    getPublicElement
} from "../../core/element";
import messageLocalization from "../../localization/message";
import Widget from "../widget/ui.widget";
import Editor from "../editor/editor";
import {
    getCurrentScreenFactor,
    hasWindow
} from "../../core/utils/window";
import ValidationEngine from "../validation_engine";
import {
    default as FormItemsRunTimeInfo
} from "./ui.form.items_runtime_info";
import TabPanel from "../tab_panel";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    isMaterial
} from "../themes";
import tryCreateItemOptionAction from "./ui.form.item_options_actions";
import "./ui.form.layout_manager";
import {
    concatPaths,
    createItemPathByIndex,
    getFullOptionName,
    getOptionNameFromFullName,
    tryGetTabPath,
    getTextWithoutSpaces,
    isExpectedItem,
    isFullPathContainsTabs,
    getItemPath
} from "./ui.form.utils";
import "../validation_summary";
import "../validation_group";
import {
    FIELD_ITEM_CLASS,
    FIELD_ITEM_LABEL_TEXT_CLASS,
    FORM_GROUP_CLASS,
    FORM_GROUP_CONTENT_CLASS,
    FIELD_ITEM_CONTENT_HAS_GROUP_CLASS,
    FIELD_ITEM_CONTENT_HAS_TABS_CLASS,
    FORM_GROUP_WITH_CAPTION_CLASS,
    FORM_GROUP_CAPTION_CLASS,
    HIDDEN_LABEL_CLASS,
    FIELD_ITEM_LABEL_CLASS,
    FIELD_ITEM_LABEL_CONTENT_CLASS,
    FIELD_ITEM_TAB_CLASS,
    FORM_FIELD_ITEM_COL_CLASS,
    GROUP_COL_COUNT_CLASS,
    GROUP_COL_COUNT_ATTR,
    FIELD_ITEM_CONTENT_CLASS,
    FORM_VALIDATION_SUMMARY,
    ROOT_SIMPLE_ITEM_CLASS
} from "./constants";
import {
    TOOLBAR_CLASS
} from "../toolbar/constants";
var WIDGET_CLASS = "dx-widget";
var FOCUSED_STATE_CLASS = "dx-state-focused";
var ITEM_OPTIONS_FOR_VALIDATION_UPDATING = ["items", "isRequired", "validationRules", "visible"];
var Form = Widget.inherit({
    _init: function() {
        this.callBase();
        this._cachedColCountOptions = [];
        this._itemsRunTimeInfo = new FormItemsRunTimeInfo;
        this._groupsColCount = [];
        this._attachSyncSubscriptions()
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            formID: "dx-" + new Guid,
            formData: {},
            colCount: 1,
            colCountByScreen: void 0,
            labelLocation: "left",
            readOnly: false,
            onFieldDataChanged: null,
            customizeItem: null,
            onEditorEnterKey: null,
            minColWidth: 200,
            alignItemLabels: true,
            alignItemLabelsInAllGroups: true,
            alignRootItemLabels: true,
            showColonAfterLabel: true,
            showRequiredMark: true,
            showOptionalMark: false,
            requiredMark: "*",
            optionalMark: messageLocalization.format("dxForm-optionalMark"),
            requiredMessage: messageLocalization.getFormatter("dxForm-requiredMessage"),
            showValidationSummary: false,
            items: void 0,
            scrollingEnabled: false,
            validationGroup: void 0,
            stylingMode: void 0
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return isMaterial()
            },
            options: {
                showColonAfterLabel: false,
                labelLocation: "top"
            }
        }])
    },
    _setOptionsByReference: function() {
        this.callBase();
        extend(this._optionsByReference, {
            formData: true,
            validationGroup: true
        })
    },
    _getGroupColCount: function($element) {
        return parseInt($element.attr(GROUP_COL_COUNT_ATTR))
    },
    _createHiddenElement: function(rootLayoutManager) {
        this._$hiddenElement = $("<div>").addClass(WIDGET_CLASS).addClass(HIDDEN_LABEL_CLASS).appendTo("body");
        var $hiddenLabel = rootLayoutManager._renderLabel({
            text: " ",
            location: this._labelLocation()
        }).appendTo(this._$hiddenElement);
        this._hiddenLabelText = $hiddenLabel.find("." + FIELD_ITEM_LABEL_TEXT_CLASS)[0]
    },
    _removeHiddenElement: function() {
        this._$hiddenElement.remove();
        this._hiddenLabelText = null
    },
    _getLabelWidthByText: function(text) {
        this._hiddenLabelText.innerHTML = text;
        return this._hiddenLabelText.offsetWidth
    },
    _getLabelsSelectorByCol: function(index, options) {
        options = options || {};
        var fieldItemClass = options.inOneColumn ? FIELD_ITEM_CLASS : FORM_FIELD_ITEM_COL_CLASS + index;
        var cssExcludeTabbedSelector = options.excludeTabbed ? ":not(." + FIELD_ITEM_TAB_CLASS + ")" : "";
        var childLabelContentSelector = "> ." + FIELD_ITEM_LABEL_CLASS + " > ." + FIELD_ITEM_LABEL_CONTENT_CLASS;
        return "." + fieldItemClass + cssExcludeTabbedSelector + childLabelContentSelector
    },
    _getLabelText: function(labelText) {
        var length = labelText.children.length;
        var child;
        var result = "";
        var i;
        for (i = 0; i < length; i++) {
            child = labelText.children[i];
            result += !isEmpty(child.innerText) ? child.innerText : child.innerHTML
        }
        return result
    },
    _applyLabelsWidthByCol: function($container, index, options) {
        var $labelTexts = $container.find(this._getLabelsSelectorByCol(index, options));
        var $labelTextsLength = $labelTexts.length;
        var labelWidth;
        var i;
        var maxWidth = 0;
        for (i = 0; i < $labelTextsLength; i++) {
            labelWidth = this._getLabelWidthByText(this._getLabelText($labelTexts[i]));
            if (labelWidth > maxWidth) {
                maxWidth = labelWidth
            }
        }
        for (i = 0; i < $labelTextsLength; i++) {
            $labelTexts[i].style.width = maxWidth + "px"
        }
    },
    _applyLabelsWidth: function($container, excludeTabbed, inOneColumn, colCount) {
        colCount = inOneColumn ? 1 : colCount || this._getGroupColCount($container);
        var applyLabelsOptions = {
            excludeTabbed: excludeTabbed,
            inOneColumn: inOneColumn
        };
        var i;
        for (i = 0; i < colCount; i++) {
            this._applyLabelsWidthByCol($container, i, applyLabelsOptions)
        }
    },
    _getGroupElementsInColumn: function($container, columnIndex, colCount) {
        var cssColCountSelector = isDefined(colCount) ? "." + GROUP_COL_COUNT_CLASS + colCount : "";
        var groupSelector = "." + FORM_FIELD_ITEM_COL_CLASS + columnIndex + " > ." + FIELD_ITEM_CONTENT_CLASS + " > ." + FORM_GROUP_CLASS + cssColCountSelector;
        return $container.find(groupSelector)
    },
    _applyLabelsWidthWithGroups: function($container, colCount, excludeTabbed) {
        if (true === this.option("alignRootItemLabels")) {
            this._alignRootSimpleItems($container, colCount, excludeTabbed)
        }
        var alignItemLabelsInAllGroups = this.option("alignItemLabelsInAllGroups");
        if (alignItemLabelsInAllGroups) {
            this._applyLabelsWidthWithNestedGroups($container, colCount, excludeTabbed)
        } else {
            var $groups = this.$element().find("." + FORM_GROUP_CLASS);
            var i;
            for (i = 0; i < $groups.length; i++) {
                this._applyLabelsWidth($groups.eq(i), excludeTabbed)
            }
        }
    },
    _alignRootSimpleItems: function($container, colCount, excludeTabbed) {
        var $rootSimpleItems = $container.find(".".concat(ROOT_SIMPLE_ITEM_CLASS));
        for (var colIndex = 0; colIndex < colCount; colIndex++) {
            this._applyLabelsWidthByCol($rootSimpleItems, colIndex, excludeTabbed)
        }
    },
    _applyLabelsWidthWithNestedGroups: function($container, colCount, excludeTabbed) {
        var applyLabelsOptions = {
            excludeTabbed: excludeTabbed
        };
        var colIndex;
        var groupsColIndex;
        var groupColIndex;
        var $groupsByCol;
        for (colIndex = 0; colIndex < colCount; colIndex++) {
            $groupsByCol = this._getGroupElementsInColumn($container, colIndex);
            this._applyLabelsWidthByCol($groupsByCol, 0, applyLabelsOptions);
            for (groupsColIndex = 0; groupsColIndex < this._groupsColCount.length; groupsColIndex++) {
                $groupsByCol = this._getGroupElementsInColumn($container, colIndex, this._groupsColCount[groupsColIndex]);
                var groupColCount = this._getGroupColCount($groupsByCol);
                for (groupColIndex = 1; groupColIndex < groupColCount; groupColIndex++) {
                    this._applyLabelsWidthByCol($groupsByCol, groupColIndex, applyLabelsOptions)
                }
            }
        }
    },
    _labelLocation: function() {
        return this.option("labelLocation")
    },
    _alignLabelsInColumn: function(_ref) {
        var {
            layoutManager: layoutManager,
            inOneColumn: inOneColumn,
            $container: $container,
            excludeTabbed: excludeTabbed,
            items: items
        } = _ref;
        if (!hasWindow() || "top" === this._labelLocation()) {
            return
        }
        this._createHiddenElement(layoutManager);
        if (inOneColumn) {
            this._applyLabelsWidth($container, excludeTabbed, true)
        } else if (this._checkGrouping(items)) {
            this._applyLabelsWidthWithGroups($container, layoutManager._getColCount(), excludeTabbed)
        } else {
            this._applyLabelsWidth($container, excludeTabbed, false, layoutManager._getColCount())
        }
        this._removeHiddenElement()
    },
    _prepareFormData: function() {
        if (!isDefined(this.option("formData"))) {
            this.option("formData", {})
        }
    },
    _initMarkup: function() {
        ValidationEngine.addGroup(this._getValidationGroup());
        this._clearCachedInstances();
        this._prepareFormData();
        this.callBase();
        this._renderLayout();
        this._renderValidationSummary();
        this._lastMarkupScreenFactor = this._targetScreenFactor || this._getCurrentScreenFactor()
    },
    _getCurrentScreenFactor: function() {
        return hasWindow() ? getCurrentScreenFactor(this.option("screenByWidth")) : "lg"
    },
    _clearCachedInstances: function() {
        this._itemsRunTimeInfo.clear();
        this._cachedLayoutManagers = []
    },
    _alignLabels: function(layoutManager, inOneColumn) {
        this._alignLabelsInColumn({
            $container: this.$element(),
            layoutManager: layoutManager,
            excludeTabbed: true,
            items: this.option("items"),
            inOneColumn: inOneColumn
        });
        triggerResizeEvent(this.$element().find(".".concat(TOOLBAR_CLASS)))
    },
    _clean: function() {
        this.callBase();
        this._groupsColCount = [];
        this._cachedColCountOptions = [];
        this._lastMarkupScreenFactor = void 0
    },
    _getContent: function() {
        return this.option("scrollingEnabled") ? this._scrollable.$content() : this.$element()
    },
    _renderValidationSummary: function() {
        var $validationSummary = this.$element().find("." + FORM_VALIDATION_SUMMARY);
        if ($validationSummary.length > 0) {
            $validationSummary.remove()
        }
        if (this.option("showValidationSummary")) {
            var _$validationSummary = $("<div>").addClass(FORM_VALIDATION_SUMMARY).appendTo(this._getContent());
            this._validationSummary = _$validationSummary.dxValidationSummary({
                validationGroup: this._getValidationGroup()
            }).dxValidationSummary("instance")
        }
    },
    _prepareItems(items, parentIsTabbedItem, currentPath, isTabs) {
        if (items) {
            var result = [];
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var path = concatPaths(currentPath, createItemPathByIndex(i, isTabs));
                var guid = this._itemsRunTimeInfo.add({
                    item: item,
                    itemIndex: i,
                    path: path
                });
                if (isString(item)) {
                    item = {
                        dataField: item
                    }
                }
                if (isObject(item)) {
                    var itemCopy = extend({}, item);
                    itemCopy.guid = guid;
                    this._tryPrepareGroupItem(itemCopy);
                    this._tryPrepareTabbedItem(itemCopy, path);
                    this._tryPrepareItemTemplate(itemCopy);
                    if (parentIsTabbedItem) {
                        itemCopy.cssItemClass = FIELD_ITEM_TAB_CLASS
                    }
                    if (itemCopy.items) {
                        itemCopy.items = this._prepareItems(itemCopy.items, parentIsTabbedItem, path)
                    }
                    result.push(itemCopy)
                } else {
                    result.push(item)
                }
            }
            return result
        }
    },
    _tryPrepareGroupItem: function(item) {
        if ("group" === item.itemType) {
            item.alignItemLabels = ensureDefined(item.alignItemLabels, true);
            if (item.template) {
                item.groupContentTemplate = this._getTemplate(item.template)
            }
            item.template = this._itemGroupTemplate.bind(this, item)
        }
    },
    _tryPrepareTabbedItem: function(item, path) {
        if ("tabbed" === item.itemType) {
            item.template = this._itemTabbedTemplate.bind(this, item);
            item.tabs = this._prepareItems(item.tabs, true, path, true)
        }
    },
    _tryPrepareItemTemplate: function(item) {
        if (item.template) {
            item.template = this._getTemplate(item.template)
        }
    },
    _checkGrouping: function(items) {
        if (items) {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if ("group" === item.itemType) {
                    return true
                }
            }
        }
    },
    _renderLayout: function() {
        var that = this;
        var items = that.option("items");
        items = that._prepareItems(items);
        that._rootLayoutManager = that._renderLayoutManager(items, {
            isRoot: true,
            colCount: that.option("colCount"),
            alignItemLabels: that.option("alignItemLabels"),
            colCountByScreen: this.option("colCountByScreen"),
            onLayoutChanged: function(inOneColumn) {
                that._alignLabels.bind(that)(that._rootLayoutManager, inOneColumn)
            },
            onContentReady: function(e) {
                that._alignLabels(e.component, e.component.isSingleColumnMode())
            }
        })
    },
    _tryGetItemsForTemplate: function(item) {
        return item.items || []
    },
    _itemTabbedTemplate: function(item, e, $container) {
        var $tabPanel = $("<div>").appendTo($container);
        var tabPanelOptions = extend({}, item.tabPanelOptions, {
            dataSource: item.tabs,
            onItemRendered: args => triggerShownEvent(args.itemElement),
            itemTemplate: (itemData, e, container) => {
                var $container = $(container);
                var alignItemLabels = ensureDefined(itemData.alignItemLabels, true);
                var layoutManager = this._renderLayoutManager(this._tryGetItemsForTemplate(itemData), $container, {
                    colCount: itemData.colCount,
                    alignItemLabels: alignItemLabels,
                    screenByWidth: this.option("screenByWidth"),
                    colCountByScreen: itemData.colCountByScreen,
                    cssItemClass: itemData.cssItemClass,
                    onLayoutChanged: inOneColumn => {
                        this._alignLabelsInColumn({
                            $container: $container,
                            layoutManager: layoutManager,
                            items: itemData.items,
                            inOneColumn: inOneColumn
                        })
                    }
                });
                if (this._itemsRunTimeInfo) {
                    this._itemsRunTimeInfo.extendRunTimeItemInfoByKey(itemData.guid, {
                        layoutManager: layoutManager
                    })
                }
                if (alignItemLabels) {
                    this._alignLabelsInColumn({
                        $container: $container,
                        layoutManager: layoutManager,
                        items: itemData.items,
                        inOneColumn: layoutManager.isSingleColumnMode()
                    })
                }
            }
        });
        var tryUpdateTabPanelInstance = (items, instance) => {
            if (Array.isArray(items)) {
                items.forEach(item => this._itemsRunTimeInfo.extendRunTimeItemInfoByKey(item.guid, {
                    widgetInstance: instance
                }))
            }
        };
        var tabPanel = this._createComponent($tabPanel, TabPanel, tabPanelOptions);
        $($container).parent().addClass(FIELD_ITEM_CONTENT_HAS_TABS_CLASS);
        tabPanel.on("optionChanged", e => {
            if ("dataSource" === e.fullName) {
                tryUpdateTabPanelInstance(e.value, e.component)
            }
        });
        tryUpdateTabPanelInstance([{
            guid: item.guid
        }, ...item.tabs], tabPanel)
    },
    _itemGroupTemplate: function(item, e, $container) {
        var $group = $("<div>").toggleClass(FORM_GROUP_WITH_CAPTION_CLASS, isDefined(item.caption) && item.caption.length).addClass(FORM_GROUP_CLASS).appendTo($container);
        $($container).parent().addClass(FIELD_ITEM_CONTENT_HAS_GROUP_CLASS);
        var colCount;
        var layoutManager;
        if (item.caption) {
            $("<span>").addClass(FORM_GROUP_CAPTION_CLASS).text(item.caption).appendTo($group)
        }
        var $groupContent = $("<div>").addClass(FORM_GROUP_CONTENT_CLASS).appendTo($group);
        if (item.groupContentTemplate) {
            var data = {
                formData: this.option("formData"),
                component: this
            };
            item.groupContentTemplate.render({
                model: data,
                container: getPublicElement($groupContent)
            })
        } else {
            layoutManager = this._renderLayoutManager(this._tryGetItemsForTemplate(item), $groupContent, {
                colCount: item.colCount,
                colCountByScreen: item.colCountByScreen,
                alignItemLabels: item.alignItemLabels,
                cssItemClass: item.cssItemClass
            });
            this._itemsRunTimeInfo && this._itemsRunTimeInfo.extendRunTimeItemInfoByKey(item.guid, {
                layoutManager: layoutManager
            });
            colCount = layoutManager._getColCount();
            if (-1 === inArray(colCount, this._groupsColCount)) {
                this._groupsColCount.push(colCount)
            }
            $group.addClass(GROUP_COL_COUNT_CLASS + colCount);
            $group.attr(GROUP_COL_COUNT_ATTR, colCount)
        }
    },
    _renderLayoutManager: function(items, $rootElement, options) {
        var that = this;
        var config = that._getLayoutManagerConfig(items, options);
        var baseColCountByScreen = {
            lg: options.colCount,
            md: options.colCount,
            sm: options.colCount,
            xs: 1
        };
        that._cachedColCountOptions.push({
            colCountByScreen: extend(baseColCountByScreen, options.colCountByScreen)
        });
        var instance = that._createComponent("dxLayoutManager", config);
        instance.on("autoColCountChanged", (function() {
            that._refresh()
        }));
        that._cachedLayoutManagers.push(instance);
        return instance
    },
    _getValidationGroup: function() {
        return this.option("validationGroup") || this
    },
    _getLayoutManagerConfig: function(items, options) {
        var baseConfig = {
            form: this,
            isRoot: options.isRoot,
            validationGroup: this._getValidationGroup(),
            showRequiredMark: this.option("showRequiredMark"),
            showOptionalMark: this.option("showOptionalMark"),
            requiredMark: this.option("requiredMark"),
            optionalMark: this.option("optionalMark"),
            requiredMessage: this.option("requiredMessage"),
            screenByWidth: this.option("screenByWidth"),
            layoutData: this.option("formData"),
            labelLocation: this.option("labelLocation"),
            customizeItem: this.option("customizeItem"),
            minColWidth: this.option("minColWidth"),
            showColonAfterLabel: this.option("showColonAfterLabel"),
            onEditorEnterKey: this.option("onEditorEnterKey"),
            onFieldDataChanged: args => {
                if (!this._isDataUpdating) {
                    this._triggerOnFieldDataChanged(args)
                }
            },
            validationBoundary: this.option("scrollingEnabled") ? this.$element() : void 0
        };
        return extend(baseConfig, {
            items: items,
            onContentReady: args => {
                this._itemsRunTimeInfo.addItemsOrExtendFrom(args.component._itemsRunTimeInfo);
                options.onContentReady && options.onContentReady(args)
            },
            onDisposing: _ref2 => {
                var {
                    component: component
                } = _ref2;
                var nestedItemsRunTimeInfo = component.getItemsRunTimeInfo();
                this._itemsRunTimeInfo.removeItemsByItems(nestedItemsRunTimeInfo)
            },
            colCount: options.colCount,
            alignItemLabels: options.alignItemLabels,
            cssItemClass: options.cssItemClass,
            colCountByScreen: options.colCountByScreen,
            onLayoutChanged: options.onLayoutChanged,
            width: options.width
        })
    },
    _createComponent: function($element, type, config) {
        config = config || {};
        this._extendConfig(config, {
            readOnly: this.option("readOnly")
        });
        return this.callBase($element, type, config)
    },
    _attachSyncSubscriptions: function() {
        var that = this;
        that.on("optionChanged", (function(args) {
            var optionFullName = args.fullName;
            if ("formData" === optionFullName) {
                if (!isDefined(args.value)) {
                    that._options.silent("formData", args.value = {})
                }
                that._triggerOnFieldDataChangedByDataSet(args.value)
            }
            if (that._cachedLayoutManagers.length) {
                each(that._cachedLayoutManagers, (function(index, layoutManager) {
                    if ("formData" === optionFullName) {
                        that._isDataUpdating = true;
                        layoutManager.option("layoutData", args.value);
                        that._isDataUpdating = false
                    }
                    if ("readOnly" === args.name || "disabled" === args.name) {
                        layoutManager.option(optionFullName, args.value)
                    }
                }))
            }
        }))
    },
    _optionChanged: function(args) {
        var rootNameOfComplexOption = this._getRootLevelOfExpectedComplexOption(args.fullName, ["formData", "items"]);
        if (rootNameOfComplexOption) {
            this._customHandlerOfComplexOption(args, rootNameOfComplexOption);
            return
        }
        switch (args.name) {
            case "formData":
                if (!this.option("items")) {
                    this._invalidate()
                } else if (isEmptyObject(args.value)) {
                    this._resetValues()
                }
                break;
            case "items":
            case "colCount":
            case "onFieldDataChanged":
            case "onEditorEnterKey":
            case "labelLocation":
            case "alignItemLabels":
            case "showColonAfterLabel":
            case "customizeItem":
            case "alignItemLabelsInAllGroups":
            case "showRequiredMark":
            case "showOptionalMark":
            case "requiredMark":
            case "optionalMark":
            case "requiredMessage":
            case "scrollingEnabled":
            case "formID":
            case "colCountByScreen":
            case "screenByWidth":
            case "stylingMode":
                this._invalidate();
                break;
            case "showValidationSummary":
                this._renderValidationSummary();
                break;
            case "minColWidth":
                if ("auto" === this.option("colCount")) {
                    this._invalidate()
                }
                break;
            case "alignRootItemLabels":
            case "readOnly":
                break;
            case "width":
                this.callBase(args);
                this._rootLayoutManager.option(args.name, args.value);
                this._alignLabels(this._rootLayoutManager, this._rootLayoutManager.isSingleColumnMode());
                break;
            case "visible":
                this.callBase(args);
                if (args.value) {
                    triggerShownEvent(this.$element())
                }
                break;
            case "validationGroup":
                ValidationEngine.removeGroup(args.previousValue || this);
                this._invalidate();
                break;
            default:
                this.callBase(args)
        }
    },
    _getRootLevelOfExpectedComplexOption: function(fullOptionName, expectedRootNames) {
        var splitFullName = fullOptionName.split(".");
        var result;
        if (splitFullName.length > 1) {
            var i;
            var rootOptionName = splitFullName[0];
            for (i = 0; i < expectedRootNames.length; i++) {
                if (-1 !== rootOptionName.search(expectedRootNames[i])) {
                    result = expectedRootNames[i]
                }
            }
        }
        return result
    },
    _tryCreateItemOptionAction: function(optionName, item, value, previousValue, itemPath) {
        if ("tabs" === optionName) {
            this._itemsRunTimeInfo.removeItemsByPathStartWith("".concat(itemPath, ".tabs"));
            value = this._prepareItems(value, true, itemPath, true)
        }
        return tryCreateItemOptionAction(optionName, {
            item: item,
            value: value,
            previousValue: previousValue,
            itemsRunTimeInfo: this._itemsRunTimeInfo
        })
    },
    _tryExecuteItemOptionAction: function(action) {
        return action && action.tryExecute()
    },
    _updateValidationGroupAndSummaryIfNeeded: function(fullName) {
        var optionName = getOptionNameFromFullName(fullName);
        if (ITEM_OPTIONS_FOR_VALIDATION_UPDATING.indexOf(optionName) > -1) {
            ValidationEngine.addGroup(this._getValidationGroup());
            if (this.option("showValidationSummary")) {
                this._validationSummary && this._validationSummary._initGroupRegistration()
            }
        }
    },
    _setLayoutManagerItemOption(layoutManager, optionName, value, path) {
        if (this._updateLockCount > 0) {
            !layoutManager._updateLockCount && layoutManager.beginUpdate();
            var key = this._itemsRunTimeInfo.getKeyByPath(path);
            this.postponedOperations.add(key, () => {
                !layoutManager._disposed && layoutManager.endUpdate();
                return (new Deferred).resolve()
            })
        }
        var contentReadyHandler = e => {
            e.component.off("contentReady", contentReadyHandler);
            if (isFullPathContainsTabs(path)) {
                var tabPath = tryGetTabPath(path);
                var tabLayoutManager = this._itemsRunTimeInfo.getGroupOrTabLayoutManagerByPath(tabPath);
                this._alignLabelsInColumn({
                    items: tabLayoutManager.option("items"),
                    layoutManager: tabLayoutManager,
                    $container: tabLayoutManager.$element(),
                    inOneColumn: tabLayoutManager.isSingleColumnMode()
                })
            } else {
                this._alignLabels(this._rootLayoutManager, this._rootLayoutManager.isSingleColumnMode())
            }
        };
        layoutManager.on("contentReady", contentReadyHandler);
        layoutManager.option(optionName, value);
        this._updateValidationGroupAndSummaryIfNeeded(optionName)
    },
    _tryChangeLayoutManagerItemOption(fullName, value) {
        var nameParts = fullName.split(".");
        var optionName = getOptionNameFromFullName(fullName);
        if ("items" === optionName && nameParts.length > 1) {
            var itemPath = this._getItemPath(nameParts);
            var layoutManager = this._itemsRunTimeInfo.getGroupOrTabLayoutManagerByPath(itemPath);
            if (layoutManager) {
                this._itemsRunTimeInfo.removeItemsByItems(layoutManager.getItemsRunTimeInfo());
                var items = this._prepareItems(value, false, itemPath);
                this._setLayoutManagerItemOption(layoutManager, optionName, items, itemPath);
                return true
            }
        } else if (nameParts.length > 2) {
            var endPartIndex = nameParts.length - 2;
            var _itemPath = this._getItemPath(nameParts.slice(0, endPartIndex));
            var _layoutManager = this._itemsRunTimeInfo.getGroupOrTabLayoutManagerByPath(_itemPath);
            if (_layoutManager) {
                var fullOptionName = getFullOptionName(nameParts[endPartIndex], optionName);
                if ("editorType" === optionName) {
                    if (_layoutManager.option(fullOptionName) !== value) {
                        return false
                    }
                }
                if ("visible" === optionName) {
                    var formItems = this.option(getFullOptionName(_itemPath, "items"));
                    if (formItems && formItems.length) {
                        var layoutManagerItems = _layoutManager.option("items");
                        formItems.forEach((item, index) => {
                            var layoutItem = layoutManagerItems[index];
                            layoutItem.visibleIndex = item.visibleIndex
                        })
                    }
                }
                this._setLayoutManagerItemOption(_layoutManager, fullOptionName, value, _itemPath);
                return true
            }
        }
        return false
    },
    _tryChangeLayoutManagerItemOptions(itemPath, options) {
        var result;
        this.beginUpdate();
        each(options, (optionName, optionValue) => {
            result = this._tryChangeLayoutManagerItemOption(getFullOptionName(itemPath, optionName), optionValue);
            if (!result) {
                return false
            }
        });
        this.endUpdate();
        return result
    },
    _customHandlerOfComplexOption: function(args, rootOptionName) {
        var nameParts = args.fullName.split(".");
        var value = args.value;
        if ("items" === rootOptionName) {
            var itemPath = this._getItemPath(nameParts);
            var item = this.option(itemPath);
            var optionNameWithoutPath = args.fullName.replace(itemPath + ".", "");
            var simpleOptionName = optionNameWithoutPath.split(".")[0].replace(/\[\d+]/, "");
            var itemAction = this._tryCreateItemOptionAction(simpleOptionName, item, item[simpleOptionName], args.previousValue, itemPath);
            if (!this._tryExecuteItemOptionAction(itemAction) && !this._tryChangeLayoutManagerItemOption(args.fullName, value)) {
                if (item) {
                    this._changeItemOption(item, optionNameWithoutPath, value);
                    var items = this._generateItemsFromData(this.option("items"));
                    this.option("items", items)
                }
            }
        }
        if ("formData" === rootOptionName) {
            var dataField = nameParts.slice(1).join(".");
            var editor = this.getEditor(dataField);
            if (editor) {
                editor.option("value", value)
            } else {
                this._triggerOnFieldDataChanged({
                    dataField: dataField,
                    value: value
                })
            }
        }
    },
    _getItemPath: function(nameParts) {
        var itemPath = nameParts[0];
        var i;
        for (i = 1; i < nameParts.length; i++) {
            if (-1 !== nameParts[i].search(/items\[\d+]|tabs\[\d+]/)) {
                itemPath += "." + nameParts[i]
            } else {
                break
            }
        }
        return itemPath
    },
    _triggerOnFieldDataChanged: function(args) {
        this._createActionByOption("onFieldDataChanged")(args)
    },
    _triggerOnFieldDataChangedByDataSet: function(data) {
        var that = this;
        if (data && isObject(data)) {
            each(data, (function(dataField, value) {
                that._triggerOnFieldDataChanged({
                    dataField: dataField,
                    value: value
                })
            }))
        }
    },
    _updateFieldValue: function(dataField, value) {
        if (isDefined(this.option("formData"))) {
            var editor = this.getEditor(dataField);
            this.option("formData." + dataField, value);
            if (editor) {
                var editorValue = editor.option("value");
                if (editorValue !== value) {
                    editor.option("value", value)
                }
            }
        }
    },
    _generateItemsFromData: function(items) {
        var formData = this.option("formData");
        var result = [];
        if (!items && isDefined(formData)) {
            each(formData, (function(dataField) {
                result.push({
                    dataField: dataField
                })
            }))
        }
        if (items) {
            each(items, (function(index, item) {
                if (isObject(item)) {
                    result.push(item)
                } else {
                    result.push({
                        dataField: item
                    })
                }
            }))
        }
        return result
    },
    _getItemByField: function(field, items) {
        var that = this;
        var fieldParts = isObject(field) ? field : that._getFieldParts(field);
        var fieldName = fieldParts.fieldName;
        var fieldPath = fieldParts.fieldPath;
        var resultItem;
        if (items.length) {
            each(items, (function(index, item) {
                var itemType = item.itemType;
                if (fieldPath.length) {
                    var path = fieldPath.slice();
                    item = that._getItemByFieldPath(path, fieldName, item)
                } else if ("group" === itemType && !(item.caption || item.name) || "tabbed" === itemType && !item.name) {
                    var subItemsField = that._getSubItemField(itemType);
                    item.items = that._generateItemsFromData(item.items);
                    item = that._getItemByField({
                        fieldName: fieldName,
                        fieldPath: fieldPath
                    }, item[subItemsField])
                }
                if (isExpectedItem(item, fieldName)) {
                    resultItem = item;
                    return false
                }
            }))
        }
        return resultItem
    },
    _getFieldParts: function(field) {
        var fieldName = field;
        var separatorIndex = fieldName.indexOf(".");
        var resultPath = [];
        while (-1 !== separatorIndex) {
            resultPath.push(fieldName.substr(0, separatorIndex));
            fieldName = fieldName.substr(separatorIndex + 1);
            separatorIndex = fieldName.indexOf(".")
        }
        return {
            fieldName: fieldName,
            fieldPath: resultPath.reverse()
        }
    },
    _getItemByFieldPath: function(path, fieldName, item) {
        var itemType = item.itemType;
        var subItemsField = this._getSubItemField(itemType);
        var isItemWithSubItems = "group" === itemType || "tabbed" === itemType || item.title;
        var result;
        do {
            if (isItemWithSubItems) {
                var name = item.name || item.caption || item.title;
                var isGroupWithName = isDefined(name);
                var nameWithoutSpaces = getTextWithoutSpaces(name);
                var pathNode = void 0;
                item[subItemsField] = this._generateItemsFromData(item[subItemsField]);
                if (isGroupWithName) {
                    pathNode = path.pop()
                }
                if (!path.length) {
                    result = this._getItemByField(fieldName, item[subItemsField]);
                    if (result) {
                        break
                    }
                }
                if (!isGroupWithName || isGroupWithName && nameWithoutSpaces === pathNode) {
                    if (path.length) {
                        result = this._searchItemInEverySubItem(path, fieldName, item[subItemsField])
                    }
                }
            } else {
                break
            }
        } while (path.length && !isDefined(result));
        return result
    },
    _getSubItemField: function(itemType) {
        return "tabbed" === itemType ? "tabs" : "items"
    },
    _searchItemInEverySubItem: function(path, fieldName, items) {
        var that = this;
        var result;
        each(items, (function(index, groupItem) {
            result = that._getItemByFieldPath(path.slice(), fieldName, groupItem);
            if (result) {
                return false
            }
        }));
        if (!result) {
            result = false
        }
        return result
    },
    _changeItemOption: function(item, option, value) {
        if (isObject(item)) {
            item[option] = value
        }
    },
    _dimensionChanged: function() {
        var currentScreenFactor = this._getCurrentScreenFactor();
        if (this._lastMarkupScreenFactor !== currentScreenFactor) {
            if (this._isColCountChanged(this._lastMarkupScreenFactor, currentScreenFactor)) {
                this._targetScreenFactor = currentScreenFactor;
                this._refresh();
                this._targetScreenFactor = void 0
            }
            this._lastMarkupScreenFactor = currentScreenFactor
        }
    },
    _isColCountChanged: function(oldScreenSize, newScreenSize) {
        var isChanged = false;
        each(this._cachedColCountOptions, (function(index, item) {
            if (item.colCountByScreen[oldScreenSize] !== item.colCountByScreen[newScreenSize]) {
                isChanged = true;
                return false
            }
        }));
        return isChanged
    },
    _refresh: function() {
        var editorSelector = "." + FOCUSED_STATE_CLASS + " input, ." + FOCUSED_STATE_CLASS + " textarea";
        eventsEngine.trigger(this.$element().find(editorSelector), "change");
        this.callBase()
    },
    _resetValues: function() {
        this._itemsRunTimeInfo.each((function(_, itemRunTimeInfo) {
            if (isDefined(itemRunTimeInfo.widgetInstance) && Editor.isEditor(itemRunTimeInfo.widgetInstance)) {
                itemRunTimeInfo.widgetInstance.reset();
                itemRunTimeInfo.widgetInstance.option("isValid", true)
            }
        }));
        ValidationEngine.resetGroup(this._getValidationGroup())
    },
    _updateData: function(data, value, isComplexData) {
        var that = this;
        var _data = isComplexData ? value : data;
        if (isObject(_data)) {
            each(_data, (function(dataField, fieldValue) {
                that._updateData(isComplexData ? data + "." + dataField : dataField, fieldValue, isObject(fieldValue))
            }))
        } else if (isString(data)) {
            that._updateFieldValue(data, value)
        }
    },
    registerKeyHandler: function(key, handler) {
        this.callBase(key, handler);
        this._itemsRunTimeInfo.each((function(_, itemRunTimeInfo) {
            if (isDefined(itemRunTimeInfo.widgetInstance)) {
                itemRunTimeInfo.widgetInstance.registerKeyHandler(key, handler)
            }
        }))
    },
    _focusTarget: function() {
        return this.$element().find("." + FIELD_ITEM_CONTENT_CLASS + " [tabindex]").first()
    },
    _visibilityChanged: function() {},
    _dispose: function() {
        ValidationEngine.removeGroup(this._getValidationGroup());
        this.callBase()
    },
    resetValues: function() {
        this._resetValues()
    },
    updateData: function(data, value) {
        this._updateData(data, value)
    },
    getEditor: function(dataField) {
        return this._itemsRunTimeInfo.findWidgetInstanceByDataField(dataField) || this._itemsRunTimeInfo.findWidgetInstanceByName(dataField)
    },
    getButton: function(name) {
        return this._itemsRunTimeInfo.findWidgetInstanceByName(name)
    },
    updateDimensions: function() {
        var that = this;
        var deferred = new Deferred;
        if (that._scrollable) {
            that._scrollable.update().done((function() {
                deferred.resolveWith(that)
            }))
        } else {
            deferred.resolveWith(that)
        }
        return deferred.promise()
    },
    itemOption: function(id, option, value) {
        var items = this._generateItemsFromData(this.option("items"));
        var item = this._getItemByField(id, items);
        var path = getItemPath(items, item);
        if (!item) {
            return
        }
        switch (arguments.length) {
            case 1:
                return item;
            case 3:
                var itemAction = this._tryCreateItemOptionAction(option, item, value, item[option], path);
                this._changeItemOption(item, option, value);
                var fullName = getFullOptionName(path, option);
                if (!this._tryExecuteItemOptionAction(itemAction) && !this._tryChangeLayoutManagerItemOption(fullName, value)) {
                    this.option("items", items)
                }
                break;
            default:
                if (isObject(option)) {
                    if (!this._tryChangeLayoutManagerItemOptions(path, option)) {
                        var allowUpdateItems;
                        each(option, (optionName, optionValue) => {
                            var itemAction = this._tryCreateItemOptionAction(optionName, item, optionValue, item[optionName], path);
                            this._changeItemOption(item, optionName, optionValue);
                            if (!allowUpdateItems && !this._tryExecuteItemOptionAction(itemAction)) {
                                allowUpdateItems = true
                            }
                        });
                        allowUpdateItems && this.option("items", items)
                    }
                }
        }
    },
    validate: function() {
        return ValidationEngine.validateGroup(this._getValidationGroup())
    },
    getItemID: function(name) {
        return "dx_" + this.option("formID") + "_" + (name || new Guid)
    },
    getTargetScreenFactor: function() {
        return this._targetScreenFactor
    }
});
registerComponent("dxForm", Form);
export default Form;
