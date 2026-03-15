/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.item_list.thumbnails.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    Deferred
} from "../../core/utils/deferred";
import eventsEngine from "../../events/core/events_engine";
import {
    addNamespace
} from "../../events/utils/index";
import {
    name as contextMenuEventName
} from "../../events/contextmenu";
import {
    getDisplayFileSize
} from "./ui.file_manager.common";
import messageLocalization from "../../localization/message";
import FileManagerThumbnailListBox from "./ui.file_manager.items_list.thumbnails.list_box";
import FileManagerItemListBase from "./ui.file_manager.item_list";
var FILE_MANAGER_THUMBNAILS_ITEM_LIST_CLASS = "dx-filemanager-thumbnails";
var FILE_MANAGER_THUMBNAILS_ITEM_CLASS = "dx-filemanager-thumbnails-item";
var FILE_MANAGER_THUMBNAILS_ITEM_THUMBNAIL_CLASS = "dx-filemanager-thumbnails-item-thumbnail";
var FILE_MANAGER_THUMBNAILS_EVENT_NAMESPACE = "dxFileManager_thumbnails";
class FileManagerThumbnailsItemList extends FileManagerItemListBase {
    _initMarkup() {
        super._initMarkup();
        this.$element().addClass(FILE_MANAGER_THUMBNAILS_ITEM_LIST_CLASS);
        var contextMenuEvent = addNamespace(contextMenuEventName, FILE_MANAGER_THUMBNAILS_EVENT_NAMESPACE);
        eventsEngine.on(this.$element(), contextMenuEvent, this._onContextMenu.bind(this));
        this._createItemList()
    }
    _createItemList() {
        var selectionMode = this._isMultipleSelectionMode() ? "multiple" : "single";
        var $itemListContainer = $("<div>").appendTo(this.$element());
        this._itemList = this._createComponent($itemListContainer, FileManagerThumbnailListBox, {
            dataSource: this._createDataSource(),
            selectionMode: selectionMode,
            selectedItemKeys: this.option("selectedItemKeys"),
            focusedItemKey: this.option("focusedItemKey"),
            activeStateEnabled: true,
            hoverStateEnabled: true,
            loopItemFocus: false,
            focusStateEnabled: true,
            onItemEnterKeyPressed: this._tryOpen.bind(this),
            itemThumbnailTemplate: this._getItemThumbnailContainer.bind(this),
            getTooltipText: this._getTooltipText.bind(this),
            onSelectionChanged: this._onItemListSelectionChanged.bind(this),
            onFocusedItemChanged: this._onItemListFocusedItemChanged.bind(this),
            onContentReady: () => {
                var _this$_refreshDeferre;
                return null === (_this$_refreshDeferre = this._refreshDeferred) || void 0 === _this$_refreshDeferre ? void 0 : _this$_refreshDeferre.resolve()
            }
        })
    }
    _onContextMenu(e) {
        e.preventDefault();
        e.stopPropagation();
        if (!this._isDesktop()) {
            return
        }
        var items = null;
        var targetItemElement = $(e.target).closest(this._getItemSelector());
        var targetItem = null;
        if (targetItemElement.length > 0) {
            targetItem = this._itemList.getItemByItemElement(targetItemElement);
            this._itemList.selectItem(targetItem);
            items = this._getFileItemsForContextMenu(targetItem)
        }
        var target = {
            itemData: targetItem,
            itemElement: targetItemElement.length ? targetItemElement : void 0
        };
        this._showContextMenu(items, e.target, e, target)
    }
    _getItemThumbnailCssClass() {
        return FILE_MANAGER_THUMBNAILS_ITEM_THUMBNAIL_CLASS
    }
    _getItemSelector() {
        return ".".concat(FILE_MANAGER_THUMBNAILS_ITEM_CLASS)
    }
    _getTooltipText(fileItemInfo) {
        var item = fileItemInfo.fileItem;
        if (item.tooltipText) {
            return item.tooltipText
        }
        var text = "".concat(item.name, "\r\n");
        if (!item.isDirectory) {
            text += "".concat(messageLocalization.format("dxFileManager-listThumbnailsTooltipTextSize"), ": ").concat(getDisplayFileSize(item.size), "\r\n")
        }
        text += "".concat(messageLocalization.format("dxFileManager-listThumbnailsTooltipTextDateModified"), ": ").concat(item.dateModified);
        return text
    }
    _onItemDblClick(e) {
        var $item = $(e.currentTarget);
        var item = this._itemList.getItemByItemElement($item);
        this._tryOpen(item)
    }
    _tryOpen(item) {
        if (item) {
            this._raiseSelectedItemOpened(item)
        }
    }
    _getItemsInternal() {
        return super._getItemsInternal().then(items => {
            var deferred = new Deferred;
            setTimeout(() => deferred.resolve(items));
            return deferred.promise()
        })
    }
    _disableDragging() {
        return false
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            focusStateEnabled: true
        })
    }
    _onItemListSelectionChanged(_ref) {
        var {
            addedItemKeys: addedItemKeys,
            removedItemKeys: removedItemKeys
        } = _ref;
        var selectedItemInfos = this.getSelectedItems();
        var selectedItems = selectedItemInfos.map(itemInfo => itemInfo.fileItem);
        var selectedItemKeys = selectedItems.map(item => item.key);
        this._tryRaiseSelectionChanged({
            selectedItemInfos: selectedItemInfos,
            selectedItems: selectedItems,
            selectedItemKeys: selectedItemKeys,
            currentSelectedItemKeys: addedItemKeys,
            currentDeselectedItemKeys: removedItemKeys
        })
    }
    _onItemListFocusedItemChanged(_ref2) {
        var {
            item: item,
            itemElement: itemElement
        } = _ref2;
        if (!this._isMultipleSelectionMode()) {
            this._selectItemSingleSelection(item)
        }
        var fileSystemItem = (null === item || void 0 === item ? void 0 : item.fileItem) || null;
        this._onFocusedItemChanged({
            item: fileSystemItem,
            itemKey: null === fileSystemItem || void 0 === fileSystemItem ? void 0 : fileSystemItem.key,
            itemElement: itemElement || void 0
        })
    }
    _setSelectedItemKeys(itemKeys) {
        this._itemList.option("selectedItemKeys", itemKeys)
    }
    _setFocusedItemKey(itemKey) {
        this._itemList.option("focusedItemKey", itemKey)
    }
    refresh(options) {
        var actualOptions = {
            dataSource: this._createDataSource()
        };
        if (options && Object.prototype.hasOwnProperty.call(options, "focusedItemKey")) {
            actualOptions.focusedItemKey = options.focusedItemKey
        }
        if (options && Object.prototype.hasOwnProperty.call(options, "selectedItemKeys")) {
            actualOptions.selectedItemKeys = options.selectedItemKeys
        }
        this._itemList.option(actualOptions);
        this._refreshDeferred = new Deferred;
        return this._refreshDeferred.promise()
    }
    _deselectItem(item) {
        var itemElement = this._itemList.getItemElementByItem(item);
        this._itemList.unselectItem(itemElement)
    }
    _selectItemSingleSelection(item) {
        if (item) {
            this._itemList.selectItem(item)
        } else {
            this._itemList.clearSelection()
        }
    }
    clearSelection() {
        this._itemList.clearSelection()
    }
    getSelectedItems() {
        return this._itemList.getSelectedItems()
    }
}
export default FileManagerThumbnailsItemList;
