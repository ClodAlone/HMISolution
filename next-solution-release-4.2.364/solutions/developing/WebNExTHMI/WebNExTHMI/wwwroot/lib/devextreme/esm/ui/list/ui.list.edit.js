/**
 * DevExtreme (esm/ui/list/ui.list.edit.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    isTouchEvent
} from "../../events/utils/index";
import {
    extend
} from "../../core/utils/extend";
import GroupedEditStrategy from "./ui.list.edit.strategy.grouped";
import localizationMessage from "../../localization/message";
import EditProvider from "./ui.list.edit.provider";
import {
    ListBase
} from "./ui.list.base";
var LIST_ITEM_SELECTED_CLASS = "dx-list-item-selected";
var LIST_ITEM_RESPONSE_WAIT_CLASS = "dx-list-item-response-wait";
var ListEdit = ListBase.inherit({
    _supportedKeys() {
        var that = this;
        var parent = this.callBase();
        var moveFocusedItem = (e, moveUp) => {
            var editStrategy = this._editStrategy;
            var focusedElement = this.option("focusedElement");
            var focusedItemIndex = editStrategy.getNormalizedIndex(focusedElement);
            var isLastIndexFocused = focusedItemIndex === this._getLastItemIndex();
            if (isLastIndexFocused && this._isDataSourceLoading()) {
                return
            }
            if (e.shiftKey && that.option("itemDragging.allowReordering")) {
                var nextItemIndex = focusedItemIndex + (moveUp ? -1 : 1);
                var $nextItem = editStrategy.getItemElement(nextItemIndex);
                this.reorderItem(focusedElement, $nextItem);
                this.scrollToItem(focusedElement);
                e.preventDefault()
            } else {
                var editProvider = this._editProvider;
                var isInternalMoving = editProvider.handleKeyboardEvents(focusedItemIndex, moveUp);
                if (!isInternalMoving) {
                    moveUp ? parent.upArrow(e) : parent.downArrow(e)
                }
            }
        };
        return extend({}, parent, {
            del: e => {
                if (that.option("allowItemDeleting")) {
                    e.preventDefault();
                    that.deleteItem(that.option("focusedElement"))
                }
            },
            upArrow: e => moveFocusedItem(e, true),
            downArrow: e => moveFocusedItem(e),
            enter: function(e) {
                if (!this._editProvider.handleEnterPressing(e)) {
                    parent.enter.apply(this, arguments)
                }
            },
            space: function(e) {
                if (!this._editProvider.handleEnterPressing(e)) {
                    parent.space.apply(this, arguments)
                }
            }
        })
    },
    _updateSelection() {
        this._editProvider.afterItemsRendered();
        this.callBase()
    },
    _getLastItemIndex() {
        return this._itemElements().length - 1
    },
    _refreshItemElements() {
        this.callBase();
        var excludedSelectors = this._editProvider.getExcludedItemSelectors();
        if (excludedSelectors.length) {
            this._itemElementsCache = this._itemElementsCache.not(excludedSelectors)
        }
    },
    _isItemStrictEquals: function(item1, item2) {
        var privateKey = item1 && item1.__dx_key__;
        if (privateKey && !this.key() && this._selection.isItemSelected(privateKey)) {
            return false
        }
        return this.callBase(item1, item2)
    },
    _getDefaultOptions() {
        return extend(this.callBase(), {
            showSelectionControls: false,
            selectionMode: "none",
            selectAllMode: "page",
            onSelectAllValueChanged: null,
            selectAllText: localizationMessage.format("dxList-selectAll"),
            menuItems: [],
            menuMode: "context",
            allowItemDeleting: false,
            itemDeleteMode: "static",
            itemDragging: {}
        })
    },
    _defaultOptionsRules() {
        return this.callBase().concat([{
            device: _device => "ios" === _device.platform,
            options: {
                menuMode: "slide",
                itemDeleteMode: "slideItem"
            }
        }, {
            device: {
                platform: "android"
            },
            options: {
                itemDeleteMode: "swipe"
            }
        }])
    },
    _init() {
        this.callBase();
        this._initEditProvider()
    },
    _initDataSource() {
        this.callBase();
        if (!this._isPageSelectAll()) {
            this._dataSource && this._dataSource.requireTotalCount(true)
        }
    },
    _isPageSelectAll() {
        return "page" === this.option("selectAllMode")
    },
    _initEditProvider() {
        this._editProvider = new EditProvider(this)
    },
    _disposeEditProvider() {
        if (this._editProvider) {
            this._editProvider.dispose()
        }
    },
    _refreshEditProvider() {
        this._disposeEditProvider();
        this._initEditProvider()
    },
    _initEditStrategy() {
        if (this.option("grouped")) {
            this._editStrategy = new GroupedEditStrategy(this)
        } else {
            this.callBase()
        }
    },
    _initMarkup() {
        this._refreshEditProvider();
        this.callBase()
    },
    _renderItems() {
        this.callBase(...arguments);
        this._editProvider.afterItemsRendered()
    },
    _selectedItemClass: () => LIST_ITEM_SELECTED_CLASS,
    _itemResponseWaitClass: () => LIST_ITEM_RESPONSE_WAIT_CLASS,
    _itemClickHandler(e) {
        var $itemElement = $(e.currentTarget);
        if ($itemElement.is(".dx-state-disabled, .dx-state-disabled *")) {
            return
        }
        var handledByEditProvider = this._editProvider.handleClick($itemElement, e);
        if (handledByEditProvider) {
            return
        }
        this._saveSelectionChangeEvent(e);
        this.callBase(...arguments)
    },
    _shouldFireContextMenuEvent() {
        return this.callBase(...arguments) || this._editProvider.contextMenuHandlerExists()
    },
    _itemHoldHandler(e) {
        var $itemElement = $(e.currentTarget);
        if ($itemElement.is(".dx-state-disabled, .dx-state-disabled *")) {
            return
        }
        var handledByEditProvider = isTouchEvent(e) && this._editProvider.handleContextMenu($itemElement, e);
        if (handledByEditProvider) {
            e.handledByEditProvider = true;
            return
        }
        this.callBase(...arguments)
    },
    _getItemContainer: function(changeData) {
        if (this.option("grouped")) {
            var _this$_editStrategy$g;
            var groupIndex = null === (_this$_editStrategy$g = this._editStrategy.getIndexByItemData(changeData)) || void 0 === _this$_editStrategy$g ? void 0 : _this$_editStrategy$g.group;
            return this._getGroupContainerByIndex(groupIndex)
        } else {
            return this.callBase(changeData)
        }
    },
    _itemContextMenuHandler(e) {
        var $itemElement = $(e.currentTarget);
        if ($itemElement.is(".dx-state-disabled, .dx-state-disabled *")) {
            return
        }
        var handledByEditProvider = !e.handledByEditProvider && this._editProvider.handleContextMenu($itemElement, e);
        if (handledByEditProvider) {
            e.preventDefault();
            return
        }
        this.callBase(...arguments)
    },
    _postprocessRenderItem(args) {
        this.callBase(...arguments);
        this._editProvider.modifyItemElement(args)
    },
    _clean() {
        this._disposeEditProvider();
        this.callBase()
    },
    focusListItem(index) {
        var $item = this._editStrategy.getItemElement(index);
        this.option("focusedElement", $item);
        this.focus();
        this.scrollToItem(this.option("focusedElement"))
    },
    _optionChanged(args) {
        switch (args.name) {
            case "selectAllMode":
                this._initDataSource();
                this._dataSource.pageIndex(0);
                this._dataSource.load();
                break;
            case "grouped":
                this._clearSelectedItems();
                delete this._renderingGroupIndex;
                this._initEditStrategy();
                this.callBase(args);
                break;
            case "showSelectionControls":
            case "menuItems":
            case "menuMode":
            case "allowItemDeleting":
            case "itemDeleteMode":
            case "itemDragging":
            case "selectAllText":
                this._invalidate();
                break;
            case "onSelectAllValueChanged":
                break;
            default:
                this.callBase(args)
        }
    },
    selectAll() {
        return this._selection.selectAll(this._isPageSelectAll())
    },
    unselectAll() {
        return this._selection.deselectAll(this._isPageSelectAll())
    },
    isSelectAll() {
        return this._selection.getSelectAllState(this._isPageSelectAll())
    },
    getFlatIndexByItemElement(itemElement) {
        return this._itemElements().index(itemElement)
    },
    getItemElementByFlatIndex(flatIndex) {
        var $itemElements = this._itemElements();
        if (flatIndex < 0 || flatIndex >= $itemElements.length) {
            return $()
        }
        return $itemElements.eq(flatIndex)
    },
    getItemByIndex(index) {
        return this._editStrategy.getItemDataByIndex(index)
    }
});
export default ListEdit;
