/**
 * DevExtreme (esm/ui/collection/ui.collection_widget.live_update.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import CollectionWidget from "./ui.collection_widget.edit";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import {
    update,
    insert,
    indexByKey
} from "../../data/array_utils";
import {
    keysEqual
} from "../../data/utils";
import {
    when
} from "../../core/utils/deferred";
import {
    findChanges
} from "../../core/utils/array_compare";
import domAdapter from "../../core/dom_adapter";
import {
    noop
} from "../../core/utils/common";
var PRIVATE_KEY_FIELD = "__dx_key__";
export default CollectionWidget.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            repaintChangesOnly: false
        })
    },
    ctor: function() {
        this.callBase.apply(this, arguments);
        this._customizeStoreLoadOptions = e => {
            var dataSource = this._dataSource;
            if (dataSource && !dataSource.isLoaded()) {
                this._correctionIndex = 0
            }
            if (this._correctionIndex && e.storeLoadOptions) {
                e.storeLoadOptions.skip += this._correctionIndex
            }
        }, this._dataSource && this._dataSource.on("customizeStoreLoadOptions", this._customizeStoreLoadOptions)
    },
    reload: function() {
        this._correctionIndex = 0
    },
    _init: function() {
        this.callBase();
        this._refreshItemsCache();
        this._correctionIndex = 0
    },
    _findItemElementByKey: function(key) {
        var result = $();
        var keyExpr = this.key();
        this.itemElements().each((_, item) => {
            var $item = $(item);
            var itemData = this._getItemData($item);
            if (keyExpr ? keysEqual(keyExpr, this.keyOf(itemData), key) : this._isItemEquals(itemData, key)) {
                result = $item;
                return false
            }
        });
        return result
    },
    _dataSourceChangedHandler: function(newItems, e) {
        if (null !== e && void 0 !== e && e.changes) {
            this._modifyByChanges(e.changes)
        } else {
            this.callBase(newItems, e);
            this._refreshItemsCache()
        }
    },
    _isItemEquals: function(item1, item2) {
        if (item1 && item1[PRIVATE_KEY_FIELD]) {
            item1 = item1.data
        }
        try {
            return JSON.stringify(item1) === JSON.stringify(item2)
        } catch (e) {
            return item1 === item2
        }
    },
    _isItemStrictEquals: function(item1, item2) {
        return this._isItemEquals(item1, item2)
    },
    _shouldAddNewGroup: function(changes, items) {
        var result = false;
        if (this.option("grouped")) {
            if (!changes.length) {
                result = true
            }
            each(changes, (i, change) => {
                if ("insert" === change.type) {
                    result = true;
                    each(items, (_, item) => {
                        if (void 0 !== change.data.key && change.data.key === item.key) {
                            result = false;
                            return false
                        }
                    })
                }
            })
        }
        return result
    },
    _partialRefresh: function() {
        if (this.option("repaintChangesOnly")) {
            var result = findChanges(this._itemsCache, this._editStrategy.itemsGetter(), data => {
                if (data && void 0 !== data[PRIVATE_KEY_FIELD]) {
                    return data[PRIVATE_KEY_FIELD]
                }
                return this.keyOf(data)
            }, this._isItemStrictEquals.bind(this));
            if (result && this._itemsCache.length && !this._shouldAddNewGroup(result, this._itemsCache)) {
                this._modifyByChanges(result, true);
                this._renderEmptyMessage();
                return true
            } else {
                this._refreshItemsCache()
            }
        }
        return false
    },
    _refreshItemsCache: function() {
        if (this.option("repaintChangesOnly")) {
            var items = this._editStrategy.itemsGetter();
            try {
                this._itemsCache = extend(true, [], items);
                if (!this.key()) {
                    this._itemsCache = this._itemsCache.map((itemCache, index) => ({
                        [PRIVATE_KEY_FIELD]: items[index],
                        data: itemCache
                    }))
                }
            } catch (e) {
                this._itemsCache = extend([], items)
            }
        }
    },
    _dispose: function() {
        this._dataSource && this._dataSource.off("customizeStoreLoadOptions", this._customizeStoreLoadOptions);
        this.callBase()
    },
    _updateByChange: function(keyInfo, items, change, isPartialRefresh) {
        if (isPartialRefresh) {
            this._renderItem(change.index, change.data, null, this._findItemElementByKey(change.key))
        } else {
            var changedItem = items[indexByKey(keyInfo, items, change.key)];
            if (changedItem) {
                update(keyInfo, items, change.key, change.data).done(() => {
                    this._renderItem(items.indexOf(changedItem), changedItem, null, this._findItemElementByKey(change.key))
                })
            }
        }
    },
    _insertByChange: function(keyInfo, items, change, isPartialRefresh) {
        when(isPartialRefresh || insert(keyInfo, items, change.data, change.index)).done(() => {
            var _change$index;
            this._beforeItemElementInserted(change);
            var $itemContainer = this._getItemContainer(change.data);
            this._renderItem(null !== (_change$index = change.index) && void 0 !== _change$index ? _change$index : items.length, change.data, $itemContainer);
            this._afterItemElementInserted();
            this._correctionIndex++
        })
    },
    _getItemContainer: function(changeData) {
        return this._itemContainer()
    },
    _updateSelectionAfterRemoveByChange: function(removeIndex) {
        var selectedIndex = this.option("selectedIndex");
        if (selectedIndex > removeIndex) {
            this.option("selectedIndex", selectedIndex - 1)
        } else if (selectedIndex === removeIndex && 1 === this.option("selectedItems").length) {
            this.option("selectedItems", [])
        } else {
            this._normalizeSelectedItems()
        }
    },
    _beforeItemElementInserted: function(change) {
        var selectedIndex = this.option("selectedIndex");
        if (change.index <= selectedIndex) {
            this.option("selectedIndex", selectedIndex + 1)
        }
    },
    _afterItemElementInserted: noop,
    _removeByChange: function(keyInfo, items, change, isPartialRefresh) {
        var index = isPartialRefresh ? change.index : indexByKey(keyInfo, items, change.key);
        var removedItem = isPartialRefresh ? change.oldItem : items[index];
        if (removedItem) {
            var $removedItemElement = this._findItemElementByKey(change.key);
            var deletedActionArgs = this._extendActionArgs($removedItemElement);
            this._waitDeletingPrepare($removedItemElement).done(() => {
                if (isPartialRefresh) {
                    this._updateIndicesAfterIndex(index - 1);
                    this._afterItemElementDeleted($removedItemElement, deletedActionArgs);
                    this._updateSelectionAfterRemoveByChange(index)
                } else {
                    this._deleteItemElementByIndex(index);
                    this._afterItemElementDeleted($removedItemElement, deletedActionArgs)
                }
            });
            this._correctionIndex--
        }
    },
    _modifyByChanges: function(changes, isPartialRefresh) {
        var items = this._editStrategy.itemsGetter();
        var keyInfo = {
            key: this.key.bind(this),
            keyOf: this.keyOf.bind(this)
        };
        var dataSource = this._dataSource;
        var paginate = dataSource && dataSource.paginate();
        var group = dataSource && dataSource.group();
        if (paginate || group) {
            changes = changes.filter(item => "insert" !== item.type || void 0 !== item.index)
        }
        changes.forEach(change => this["_".concat(change.type, "ByChange")](keyInfo, items, change, isPartialRefresh));
        this._renderedItemsCount = items.length;
        this._refreshItemsCache();
        this._fireContentReadyAction()
    },
    _appendItemToContainer: function($container, $itemFrame, index) {
        var nextSiblingElement = $container.children(this._itemSelector()).get(index);
        domAdapter.insertElement($container.get(0), $itemFrame.get(0), nextSiblingElement)
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "items":
                var isItemsUpdated = this._partialRefresh(args.value);
                if (!isItemsUpdated) {
                    this.callBase(args)
                }
                break;
            case "dataSource":
                if (!this.option("repaintChangesOnly") || !args.value) {
                    this.option("items", [])
                }
                this.callBase(args);
                break;
            case "repaintChangesOnly":
                break;
            default:
                this.callBase(args)
        }
    }
});
