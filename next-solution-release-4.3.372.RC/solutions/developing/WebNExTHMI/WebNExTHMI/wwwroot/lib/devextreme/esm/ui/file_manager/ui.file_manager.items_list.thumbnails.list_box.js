/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.items_list.thumbnails.list_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getOuterHeight,
    getInnerWidth,
    getInnerHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    find
} from "../../core/utils/array";
import {
    isDefined
} from "../../core/utils/type";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
import holdEvent from "../../events/hold";
import {
    addNamespace,
    isCommandKeyPressed
} from "../../events/utils/index";
import eventsEngine from "../../events/core/events_engine";
import {
    BindableTemplate
} from "../../core/templates/bindable_template";
import ScrollView from "../scroll_view";
import CollectionWidget from "../collection/ui.collection_widget.edit";
import Selection from "../selection/selection";
var FILE_MANAGER_THUMBNAILS_VIEW_PORT_CLASS = "dx-filemanager-thumbnails-view-port";
var FILE_MANAGER_THUMBNAILS_ITEM_LIST_CONTAINER_CLASS = "dx-filemanager-thumbnails-container";
var FILE_MANAGER_THUMBNAILS_ITEM_CLASS = "dx-filemanager-thumbnails-item";
var FILE_MANAGER_THUMBNAILS_ITEM_NAME_CLASS = "dx-filemanager-thumbnails-item-name";
var FILE_MANAGER_THUMBNAILS_ITEM_SPACER_CLASS = "dx-filemanager-thumbnails-item-spacer";
var FILE_MANAGER_THUMBNAILS_ITEM_DATA_KEY = "dxFileManagerItemData";
var FILE_MANAGER_THUMBNAILS_LIST_BOX_NAMESPACE = "dxFileManagerThumbnailsListBox";
var FILE_MANAGER_THUMBNAILS_LIST_BOX_HOLD_EVENT_NAME = addNamespace(holdEvent.name, FILE_MANAGER_THUMBNAILS_LIST_BOX_NAMESPACE);
class FileManagerThumbnailListBox extends CollectionWidget {
    _initMarkup() {
        this._initActions();
        this._lockFocusedItemProcessing = false;
        this.$element().addClass(FILE_MANAGER_THUMBNAILS_VIEW_PORT_CLASS);
        this._renderScrollView();
        this._renderItemsContainer();
        this._createScrollViewControl();
        super._initMarkup();
        this.onFocusedItemChanged = this._onFocusedItemChanged.bind(this);
        this._layoutUtils = new ListBoxLayoutUtils(this._scrollView, this.$element(), this._$itemContainer, this.itemElements().first());
        this._syncFocusedItemKey()
    }
    _initActions() {
        this._actions = {
            onItemEnterKeyPressed: this._createActionByOption("onItemEnterKeyPressed"),
            onFocusedItemChanged: this._createActionByOption("onFocusedItemChanged")
        }
    }
    _initTemplates() {
        super._initTemplates();
        this._itemThumbnailTemplate = this.option("itemThumbnailTemplate");
        this._getTooltipText = this.option("getTooltipText");
        this._templateManager.addDefaultTemplates({
            item: new BindableTemplate(function($container, data, itemModel) {
                var $itemElement = this._getDefaultItemTemplate(itemModel, $container);
                $container.append($itemElement)
            }.bind(this), ["fileItem"], this.option("integrationOptions.watchMethod"))
        })
    }
    _createScrollViewControl() {
        if (!this._scrollView) {
            this._scrollView = this._createComponent(this._$scrollView, ScrollView, {
                scrollByContent: true,
                scrollByThumb: true,
                useKeyboard: false,
                showScrollbar: "onHover"
            })
        }
    }
    _renderScrollView() {
        if (!this._$scrollView) {
            this._$scrollView = $("<div>").appendTo(this.$element())
        }
    }
    _renderItemsContainer() {
        if (!this._$itemContainer) {
            this._$itemContainer = $("<div>").addClass(FILE_MANAGER_THUMBNAILS_ITEM_LIST_CONTAINER_CLASS).appendTo(this._$scrollView)
        }
    }
    _render() {
        super._render();
        this._detachEventHandlers();
        this._attachEventHandlers()
    }
    _clean() {
        this._detachEventHandlers();
        super._clean()
    }
    _supportedKeys() {
        return extend(super._supportedKeys(), {
            upArrow(e) {
                this._beforeKeyProcessing(e);
                this._processArrowKeys(-1, false, e)
            },
            downArrow(e) {
                this._beforeKeyProcessing(e);
                this._processArrowKeys(1, false, e)
            },
            home(e) {
                this._beforeKeyProcessing(e);
                this._processHomeEndKeys(0, true, e)
            },
            end(e) {
                this._beforeKeyProcessing(e);
                this._processHomeEndKeys(this._getItemsLength() - 1, true, e)
            },
            pageUp(e) {
                this._beforeKeyProcessing(e);
                this._processPageChange(true, e)
            },
            pageDown(e) {
                this._beforeKeyProcessing(e);
                this._processPageChange(false, e)
            },
            enter(e) {
                this._beforeKeyProcessing(e);
                this._actions.onItemEnterKeyPressed(this._getFocusedItem())
            },
            A(e) {
                this._beforeKeyProcessing(e);
                if (isCommandKeyPressed(e)) {
                    this.selectAll()
                }
            }
        })
    }
    _beforeKeyProcessing(e) {
        e.preventDefault();
        this._layoutUtils.reset()
    }
    _processArrowKeys(offset, horizontal, eventArgs) {
        var item = this._getFocusedItem();
        if (item) {
            if (!horizontal) {
                var layout = this._layoutUtils.getLayoutModel();
                if (!layout) {
                    return
                }
                offset *= layout.itemPerRowCount
            }
            var newItemIndex = this._getIndexByItem(item) + offset;
            this._focusItemByIndex(newItemIndex, true, eventArgs)
        }
    }
    _processHomeEndKeys(index, scrollToItem, eventArgs) {
        this._focusItemByIndex(index, scrollToItem, eventArgs)
    }
    _processPageChange(pageUp, eventArgs) {
        var item = this._getFocusedItem();
        if (!item) {
            return
        }
        var layout = this._layoutUtils.getLayoutModel();
        if (!layout) {
            return
        }
        var itemLayout = this._layoutUtils.createItemLayoutModel(this._getIndexByItem(item));
        var rowOffset = pageUp ? layout.rowPerPageRate : -layout.rowPerPageRate;
        var newRowRate = itemLayout.itemRowIndex - rowOffset;
        var roundFunc = pageUp ? Math.ceil : Math.floor;
        var newRowIndex = roundFunc(newRowRate);
        var newItemIndex = newRowIndex * layout.itemPerRowCount + itemLayout.itemColumnIndex;
        if (newItemIndex < 0) {
            newItemIndex = 0
        } else if (newItemIndex >= this._getItemsLength()) {
            newItemIndex = this._getItemsLength() - 1
        }
        this._focusItemByIndex(newItemIndex, true, eventArgs)
    }
    _processLongTap(e) {
        var $targetItem = this._closestItemElement($(e.target));
        var itemIndex = this._getIndexByItemElement($targetItem);
        this._selection.changeItemSelection(itemIndex, {
            control: true
        })
    }
    _attachEventHandlers() {
        if ("multiple" === this.option("selectionMode")) {
            eventsEngine.on(this._itemContainer(), FILE_MANAGER_THUMBNAILS_LIST_BOX_HOLD_EVENT_NAME, ".".concat(this._itemContentClass()), e => {
                this._processLongTap(e);
                e.stopPropagation()
            })
        }
        eventsEngine.on(this._itemContainer(), "mousedown selectstart", e => {
            if (e.shiftKey) {
                e.preventDefault()
            }
        })
    }
    _detachEventHandlers() {
        eventsEngine.off(this._itemContainer(), FILE_MANAGER_THUMBNAILS_LIST_BOX_HOLD_EVENT_NAME);
        eventsEngine.off(this._itemContainer(), "mousedown selectstart")
    }
    _itemContainer() {
        return this._$itemContainer
    }
    _itemClass() {
        return FILE_MANAGER_THUMBNAILS_ITEM_CLASS
    }
    _itemDataKey() {
        return FILE_MANAGER_THUMBNAILS_ITEM_DATA_KEY
    }
    _getDefaultItemTemplate(fileItemInfo, $itemElement) {
        $itemElement.attr("title", this._getTooltipText(fileItemInfo));
        var $itemThumbnail = this._itemThumbnailTemplate(fileItemInfo);
        var $itemSpacer = $("<div>").addClass(FILE_MANAGER_THUMBNAILS_ITEM_SPACER_CLASS);
        var $itemName = $("<div>").addClass(FILE_MANAGER_THUMBNAILS_ITEM_NAME_CLASS).text(fileItemInfo.fileItem.name);
        $itemElement.append($itemThumbnail, $itemSpacer, $itemName)
    }
    _itemSelectHandler(e) {
        var options = {};
        if ("multiple" === this.option("selectionMode")) {
            if (!this._isPreserveSelectionMode) {
                this._isPreserveSelectionMode = isCommandKeyPressed(e) || e.shiftKey
            }
            options = {
                control: this._isPreserveSelectionMode,
                shift: e.shiftKey
            }
        }
        var index = this._getIndexByItemElement(e.currentTarget);
        this._selection.changeItemSelection(index, options)
    }
    _initSelectionModule() {
        super._initSelectionModule();
        var options = extend(this._selection.options, {
            selectedKeys: this.option("selectedItemKeys"),
            onSelectionChanged: args => {
                this.option("selectedItems", this._getItemsByKeys(args.selectedItemKeys, args.selectedItems));
                this._updateSelectedItems(args)
            }
        });
        this._selection = new Selection(options)
    }
    _updateSelectedItems(args) {
        var addedItemKeys = args.addedItemKeys;
        var removedItemKeys = args.removedItemKeys;
        if (this._rendered && (addedItemKeys.length || removedItemKeys.length)) {
            var selectionChangePromise = this._selectionChangePromise;
            if (!this._rendering) {
                var addedSelection = [];
                var normalizedIndex;
                var removedSelection = [];
                this._editStrategy.beginCache();
                for (var i = 0; i < removedItemKeys.length; i++) {
                    normalizedIndex = this._getIndexByKey(removedItemKeys[i]);
                    removedSelection.push(normalizedIndex);
                    this._removeSelection(normalizedIndex)
                }
                for (var _i = 0; _i < addedItemKeys.length; _i++) {
                    normalizedIndex = this._getIndexByKey(addedItemKeys[_i]);
                    addedSelection.push(normalizedIndex);
                    this._addSelection(normalizedIndex)
                }
                this._editStrategy.endCache();
                this._updateSelection(addedSelection, removedSelection)
            }
            when(selectionChangePromise).done(() => this._fireSelectionChangeEvent(args))
        }
    }
    _fireSelectionChangeEvent(args) {
        this._createActionByOption("onSelectionChanged", {
            excludeValidators: ["disabled", "readOnly"]
        })(args)
    }
    _updateSelection(addedSelection, removedSelection) {
        var selectedItemsCount = this.getSelectedItems().length;
        if (0 === selectedItemsCount) {
            this._isPreserveSelectionMode = false
        }
    }
    _normalizeSelectedItems() {
        var newKeys = this._getKeysByItems(this.option("selectedItems"));
        var oldKeys = this._selection.getSelectedItemKeys();
        if (!this._compareKeys(oldKeys, newKeys)) {
            this._selection.setSelection(newKeys)
        }
        return (new Deferred).resolve().promise()
    }
    _focusOutHandler() {}
    _getItems() {
        return this.option("items") || []
    }
    _getItemsLength() {
        return this._getItems().length
    }
    _getIndexByItemElement(itemElement) {
        return this._editStrategy.getNormalizedIndex(itemElement)
    }
    _getItemByIndex(index) {
        return this._getItems()[index]
    }
    _getFocusedItem() {
        return this.getItemByItemElement(this.option("focusedElement"))
    }
    _focusItem(item, scrollToItem) {
        this.option("focusedElement", this.getItemElementByItem(item));
        if (scrollToItem) {
            this._layoutUtils.scrollToItem(this._getIndexByItem(item))
        }
    }
    _focusItemByIndex(index, scrollToItem, eventArgs) {
        if (index >= 0 && index < this._getItemsLength()) {
            var item = this._getItemByIndex(index);
            this._focusItem(item, scrollToItem, eventArgs)
        }
    }
    _syncFocusedItemKey() {
        if (!this._syncFocusedItemKeyDeferred) {
            this._syncFocusedItemKeyDeferred = new Deferred
        }
        var deferred = this._syncFocusedItemKeyDeferred;
        if (this._dataSource && this._dataSource.isLoading()) {
            return deferred.promise()
        }
        var focusedItemKey = this.option("focusedItemKey");
        if (isDefined(focusedItemKey)) {
            var items = this.option("items");
            var focusedItem = find(items, item => this.keyOf(item) === focusedItemKey);
            if (focusedItem) {
                this._focusItem(focusedItem, true);
                deferred.resolve()
            } else {
                this.option("focusedItemKey", void 0);
                deferred.reject()
            }
        } else {
            deferred.resolve()
        }
        this._syncFocusedItemKeyDeferred = null;
        return deferred.promise()
    }
    _onFocusedItemChanged() {
        var focusedItem = this._getFocusedItem();
        var newFocusedItemKey = this.keyOf(focusedItem);
        var oldFocusedItemKey = this.option("focusedItemKey");
        if (newFocusedItemKey !== oldFocusedItemKey) {
            this._lockFocusedItemProcessing = true;
            this.option("focusedItemKey", newFocusedItemKey);
            this._lockFocusedItemProcessing = false;
            this._raiseFocusedItemChanged(focusedItem)
        }
    }
    _raiseFocusedItemChanged(focusedItem) {
        var args = {
            item: focusedItem,
            itemElement: this.option("focusedElement")
        };
        this._actions.onFocusedItemChanged(args)
    }
    _changeItemSelection(item, select) {
        if (this.isItemSelected(item) === select) {
            return
        }
        var itemElement = this.getItemElementByItem(item);
        var index = this._getIndexByItemElement(itemElement);
        this._selection.changeItemSelection(index, {
            control: this._isPreserveSelectionMode
        })
    }
    _chooseSelectOption() {
        return "selectedItemKeys"
    }
    getSelectedItems() {
        return this._selection.getSelectedItems()
    }
    getItemElementByItem(item) {
        return this._editStrategy.getItemElement(item)
    }
    getItemByItemElement(itemElement) {
        return this._getItemByIndex(this._getIndexByItemElement(itemElement))
    }
    selectAll() {
        if ("multiple" !== this.option("selectionMode")) {
            return
        }
        this._selection.selectAll();
        this._isPreserveSelectionMode = true
    }
    selectItem(item) {
        this._changeItemSelection(item, true)
    }
    deselectItem(item) {
        this._changeItemSelection(item, false)
    }
    clearSelection() {
        this._selection.deselectAll()
    }
    _optionChanged(args) {
        switch (args.name) {
            case "items":
                if (this._layoutUtils) {
                    this._layoutUtils.updateItems(this.itemElements().first())
                }
                super._optionChanged(args);
                break;
            case "focusedItemKey":
                if (this._lockFocusedItemProcessing) {
                    break
                }
                if (isDefined(args.value)) {
                    this._syncFocusedItemKey().done(() => {
                        var focusedItem = this._getFocusedItem();
                        this._raiseFocusedItemChanged(focusedItem)
                    })
                } else {
                    this.option("focusedElement", null);
                    this._raiseFocusedItemChanged(null)
                }
                break;
            case "onItemEnterKeyPressed":
            case "onFocusedItemChanged":
                this._actions[args.name] = this._createActionByOption(args.name);
                break;
            default:
                super._optionChanged(args)
        }
    }
}
class ListBoxLayoutUtils {
    constructor(scrollView, $viewPort, $itemContainer, $item) {
        this._layoutModel = null;
        this._scrollView = scrollView;
        this._$viewPort = $viewPort;
        this._$itemContainer = $itemContainer;
        this._$item = $item
    }
    updateItems($item) {
        this._$item = $item
    }
    reset() {
        this._layoutModel = null
    }
    getLayoutModel() {
        if (!this._layoutModel) {
            this._layoutModel = this._createLayoutModel()
        }
        return this._layoutModel
    }
    _createLayoutModel() {
        if (!this._$item) {
            return null
        }
        var itemWidth = getOuterWidth(this._$item, true);
        if (0 === itemWidth) {
            return null
        }
        var itemHeight = getOuterHeight(this._$item, true);
        var viewPortWidth = getInnerWidth(this._$itemContainer);
        var viewPortHeight = getInnerHeight(this._$viewPort);
        var viewPortScrollTop = this._scrollView.scrollTop();
        var viewPortScrollBottom = viewPortScrollTop + viewPortHeight;
        var itemPerRowCount = Math.floor(viewPortWidth / itemWidth);
        var rowPerPageRate = viewPortHeight / itemHeight;
        return {
            itemWidth: itemWidth,
            itemHeight: itemHeight,
            viewPortWidth: viewPortWidth,
            viewPortHeight: viewPortHeight,
            viewPortScrollTop: viewPortScrollTop,
            viewPortScrollBottom: viewPortScrollBottom,
            itemPerRowCount: itemPerRowCount,
            rowPerPageRate: rowPerPageRate
        }
    }
    createItemLayoutModel(index) {
        var layout = this.getLayoutModel();
        if (!layout) {
            return null
        }
        var itemRowIndex = Math.floor(index / layout.itemPerRowCount);
        var itemColumnIndex = index % layout.itemPerRowCount;
        var itemTop = itemRowIndex * layout.itemHeight;
        var itemBottom = itemTop + layout.itemHeight;
        return {
            itemRowIndex: itemRowIndex,
            itemColumnIndex: itemColumnIndex,
            itemTop: itemTop,
            itemBottom: itemBottom
        }
    }
    scrollToItem(index) {
        var layout = this.getLayoutModel();
        if (!layout) {
            return
        }
        var itemRowIndex = Math.floor(index / layout.itemPerRowCount);
        var itemTop = itemRowIndex * layout.itemHeight;
        var itemBottom = itemTop + layout.itemHeight;
        var newScrollTop = layout.viewPortScrollTop;
        if (itemTop < layout.viewPortScrollTop) {
            newScrollTop = itemTop
        } else if (itemBottom > layout.viewPortScrollBottom) {
            newScrollTop = itemBottom - layout.viewPortHeight
        }
        this._scrollView.scrollTo(newScrollTop)
    }
}
export default FileManagerThumbnailListBox;
