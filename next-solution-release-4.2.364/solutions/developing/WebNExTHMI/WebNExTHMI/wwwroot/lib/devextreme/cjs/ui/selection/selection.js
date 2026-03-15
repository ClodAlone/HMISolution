/**
 * DevExtreme (cjs/ui/selection/selection.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _class = _interopRequireDefault(require("../../core/class"));
var _selectionStrategy = _interopRequireDefault(require("./selection.strategy.deferred"));
var _selectionStrategy2 = _interopRequireDefault(require("./selection.strategy.standard"));
var _extend = require("../../core/utils/extend");
var _common = require("../../core/utils/common");
var _type = require("../../core/utils/type");
var _deferred = require("../../core/utils/deferred");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _default = _class.default.inherit({
    ctor: function(options) {
        this.options = (0, _extend.extend)(this._getDefaultOptions(), options, {
            selectedItemKeys: options.selectedKeys || []
        });
        this._selectionStrategy = this.options.deferred ? new _selectionStrategy.default(this.options) : new _selectionStrategy2.default(this.options);
        this._focusedItemIndex = -1;
        if (!this.options.equalByReference) {
            this._selectionStrategy.updateSelectedItemKeyHash(this.options.selectedItemKeys)
        }
    },
    _getDefaultOptions: function() {
        return {
            allowNullValue: false,
            deferred: false,
            equalByReference: false,
            mode: "multiple",
            selectedItems: [],
            selectionFilter: [],
            maxFilterLengthInRequest: 0,
            onSelectionChanged: _common.noop,
            key: _common.noop,
            keyOf: function(item) {
                return item
            },
            load: function() {
                return (new _deferred.Deferred).resolve([])
            },
            totalCount: function() {
                return -1
            },
            isSelectableItem: function() {
                return true
            },
            isItemSelected: function() {
                return false
            },
            getItemData: function(item) {
                return item
            },
            dataFields: _common.noop,
            filter: _common.noop
        }
    },
    validate: function() {
        this._selectionStrategy.validate()
    },
    getSelectedItemKeys: function() {
        return this._selectionStrategy.getSelectedItemKeys()
    },
    getSelectedItems: function() {
        return this._selectionStrategy.getSelectedItems()
    },
    selectionFilter: function(value) {
        if (void 0 === value) {
            return this.options.selectionFilter
        }
        var filterIsChanged = this.options.selectionFilter !== value && JSON.stringify(this.options.selectionFilter) !== JSON.stringify(value);
        this.options.selectionFilter = value;
        filterIsChanged && this.onSelectionChanged()
    },
    setSelection: function(keys, updatedKeys) {
        return this.selectedItemKeys(keys, false, false, false, updatedKeys)
    },
    select: function(keys) {
        return this.selectedItemKeys(keys, true)
    },
    deselect: function(keys) {
        return this.selectedItemKeys(keys, true, true)
    },
    selectedItemKeys: function(keys, preserve, isDeselect, isSelectAll, updatedKeys) {
        var _keys;
        keys = null !== (_keys = keys) && void 0 !== _keys ? _keys : [];
        keys = Array.isArray(keys) ? keys : [keys];
        this.validate();
        return this._selectionStrategy.selectedItemKeys(keys, preserve, isDeselect, isSelectAll, updatedKeys)
    },
    clearSelection: function() {
        return this.selectedItemKeys([])
    },
    _addSelectedItem: function(itemData, key) {
        this._selectionStrategy.addSelectedItem(key, itemData)
    },
    _removeSelectedItem: function(key) {
        this._selectionStrategy.removeSelectedItem(key)
    },
    _setSelectedItems: function(keys, items) {
        this._selectionStrategy.setSelectedItems(keys, items)
    },
    onSelectionChanged: function() {
        this._selectionStrategy.onSelectionChanged()
    },
    changeItemSelection: function(itemIndex, keys) {
        var _this$options$allowLo, _this$options, _this = this;
        var isSelectedItemsChanged;
        var items = this.options.plainItems();
        var item = items[itemIndex];
        var deferred;
        var allowLoadByRange = null === (_this$options$allowLo = (_this$options = this.options).allowLoadByRange) || void 0 === _this$options$allowLo ? void 0 : _this$options$allowLo.call(_this$options);
        var indexOffset;
        var focusedItemNotInLoadedRange = false;
        if (allowLoadByRange) {
            indexOffset = item.loadIndex - itemIndex;
            itemIndex = item.loadIndex;
            focusedItemNotInLoadedRange = this._focusedItemIndex >= 0 && !items.filter((function(it) {
                return it.loadIndex === _this._focusedItemIndex
            })).length
        }
        if (!this.isSelectable() || !this.isDataItem(item)) {
            return false
        }
        var itemData = this.options.getItemData(item);
        var itemKey = this.options.keyOf(itemData);
        keys = keys || {};
        if (keys.shift && "multiple" === this.options.mode && this._focusedItemIndex >= 0) {
            if (focusedItemNotInLoadedRange) {
                isSelectedItemsChanged = itemIndex !== this._shiftFocusedItemIndex || this._focusedItemIndex !== this._shiftFocusedItemIndex;
                if (isSelectedItemsChanged) {
                    deferred = this.changeItemSelectionWhenShiftKeyInVirtualPaging(itemIndex)
                }
            } else {
                isSelectedItemsChanged = this.changeItemSelectionWhenShiftKeyPressed(itemIndex, items, indexOffset)
            }
        } else if (keys.control) {
            this._resetItemSelectionWhenShiftKeyPressed();
            var isSelected = this._selectionStrategy.isItemDataSelected(itemData);
            if ("single" === this.options.mode) {
                this.clearSelectedItems()
            }
            if (isSelected) {
                this._removeSelectedItem(itemKey)
            } else {
                this._addSelectedItem(itemData, itemKey)
            }
            isSelectedItemsChanged = true
        } else {
            this._resetItemSelectionWhenShiftKeyPressed();
            var isKeysEqual = this._selectionStrategy.equalKeys(this.options.selectedItemKeys[0], itemKey);
            if (1 !== this.options.selectedItemKeys.length || !isKeysEqual) {
                this._setSelectedItems([itemKey], [itemData]);
                isSelectedItemsChanged = true
            }
        }
        if (isSelectedItemsChanged) {
            (0, _deferred.when)(deferred).done((function() {
                _this._focusedItemIndex = itemIndex;
                _this.onSelectionChanged()
            }));
            return true
        }
    },
    isDataItem: function(item) {
        return this.options.isSelectableItem(item)
    },
    isSelectable: function() {
        return "single" === this.options.mode || "multiple" === this.options.mode
    },
    isItemDataSelected: function(data) {
        return this._selectionStrategy.isItemDataSelected(data, {
            checkPending: true
        })
    },
    isItemSelected: function(arg, options) {
        return this._selectionStrategy.isItemKeySelected(arg, options)
    },
    _resetItemSelectionWhenShiftKeyPressed: function() {
        delete this._shiftFocusedItemIndex
    },
    _resetFocusedItemIndex: function() {
        this._focusedItemIndex = -1
    },
    changeItemSelectionWhenShiftKeyInVirtualPaging: function(loadIndex) {
        var _this2 = this;
        var loadOptions = this.options.getLoadOptions(loadIndex, this._focusedItemIndex, this._shiftFocusedItemIndex);
        var deferred = new _deferred.Deferred;
        var indexOffset = loadOptions.skip;
        this.options.load(loadOptions).done((function(items) {
            _this2.changeItemSelectionWhenShiftKeyPressed(loadIndex, items, indexOffset);
            deferred.resolve()
        }));
        return deferred.promise()
    },
    changeItemSelectionWhenShiftKeyPressed: function(itemIndex, items, indexOffset) {
        var isSelectedItemsChanged = false;
        var itemIndexStep;
        var indexOffsetDefined = (0, _type.isDefined)(indexOffset);
        var index = indexOffsetDefined ? this._focusedItemIndex - indexOffset : this._focusedItemIndex;
        var keyOf = this.options.keyOf;
        var focusedItem = items[index];
        var focusedData = this.options.getItemData(focusedItem);
        var focusedKey = keyOf(focusedData);
        var isFocusedItemSelected = focusedItem && this.isItemDataSelected(focusedData);
        if (!(0, _type.isDefined)(this._shiftFocusedItemIndex)) {
            this._shiftFocusedItemIndex = this._focusedItemIndex
        }
        var data;
        var itemKey;
        var startIndex;
        var endIndex;
        if (this._shiftFocusedItemIndex !== this._focusedItemIndex) {
            itemIndexStep = this._focusedItemIndex < this._shiftFocusedItemIndex ? 1 : -1;
            startIndex = indexOffsetDefined ? this._focusedItemIndex - indexOffset : this._focusedItemIndex;
            endIndex = indexOffsetDefined ? this._shiftFocusedItemIndex - indexOffset : this._shiftFocusedItemIndex;
            for (index = startIndex; index !== endIndex; index += itemIndexStep) {
                if (indexOffsetDefined || this.isDataItem(items[index])) {
                    itemKey = keyOf(this.options.getItemData(items[index]));
                    this._removeSelectedItem(itemKey);
                    isSelectedItemsChanged = true
                }
            }
        }
        if (itemIndex !== this._shiftFocusedItemIndex) {
            itemIndexStep = itemIndex < this._shiftFocusedItemIndex ? 1 : -1;
            startIndex = indexOffsetDefined ? itemIndex - indexOffset : itemIndex;
            endIndex = indexOffsetDefined ? this._shiftFocusedItemIndex - indexOffset : this._shiftFocusedItemIndex;
            for (index = startIndex; index !== endIndex; index += itemIndexStep) {
                if (indexOffsetDefined || this.isDataItem(items[index])) {
                    data = this.options.getItemData(items[index]);
                    itemKey = keyOf(data);
                    this._addSelectedItem(data, itemKey);
                    isSelectedItemsChanged = true
                }
            }
        }
        if ((indexOffsetDefined || this.isDataItem(focusedItem)) && !isFocusedItemSelected) {
            this._addSelectedItem(focusedData, focusedKey);
            isSelectedItemsChanged = true
        }
        return isSelectedItemsChanged
    },
    clearSelectedItems: function() {
        this._setSelectedItems([], [])
    },
    selectAll: function(isOnePage) {
        this._resetFocusedItemIndex();
        if (isOnePage) {
            return this._onePageSelectAll(false)
        } else {
            return this.selectedItemKeys([], true, false, true)
        }
    },
    deselectAll: function(isOnePage) {
        this._resetFocusedItemIndex();
        if (isOnePage) {
            return this._onePageSelectAll(true)
        } else {
            return this.selectedItemKeys([], true, true, true)
        }
    },
    _onePageSelectAll: function(isDeselect) {
        var items = this._selectionStrategy.getSelectableItems(this.options.plainItems());
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (this.isDataItem(item)) {
                var itemData = this.options.getItemData(item);
                var itemKey = this.options.keyOf(itemData);
                var isSelected = this.isItemSelected(itemKey);
                if (!isSelected && !isDeselect) {
                    this._addSelectedItem(itemData, itemKey)
                }
                if (isSelected && isDeselect) {
                    this._removeSelectedItem(itemKey)
                }
            }
        }
        this.onSelectionChanged();
        return (new _deferred.Deferred).resolve()
    },
    getSelectAllState: function(visibleOnly) {
        return this._selectionStrategy.getSelectAllState(visibleOnly)
    }
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
