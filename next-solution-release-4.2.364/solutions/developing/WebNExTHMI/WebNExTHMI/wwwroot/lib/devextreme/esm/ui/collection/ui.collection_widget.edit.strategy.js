/**
 * DevExtreme (esm/ui/collection/ui.collection_widget.edit.strategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Class from "../../core/class";
import {
    equalByValue
} from "../../core/utils/common";
import domAdapter from "../../core/dom_adapter";
import {
    isRenderer
} from "../../core/utils/type";
var abstract = Class.abstract;
var EditStrategy = Class.inherit({
    ctor: function(collectionWidget) {
        this._collectionWidget = collectionWidget
    },
    getIndexByItemData: abstract,
    getItemDataByIndex: abstract,
    getKeysByItems: abstract,
    getItemsByKeys: abstract,
    itemsGetter: abstract,
    getKeyByIndex: function(index) {
        var resultIndex = this._denormalizeItemIndex(index);
        return this.getKeysByItems([this.getItemDataByIndex(resultIndex)])[0]
    },
    _equalKeys: function(key1, key2) {
        if (this._collectionWidget._isKeySpecified()) {
            return equalByValue(key1, key2)
        } else {
            return key1 === key2
        }
    },
    beginCache: function() {
        this._cache = {}
    },
    endCache: function() {
        this._cache = null
    },
    getIndexByKey: abstract,
    getNormalizedIndex: function(value) {
        if (this._isNormalizedItemIndex(value)) {
            return value
        }
        if (this._isItemIndex(value)) {
            return this._normalizeItemIndex(value)
        }
        if (this._isNode(value)) {
            return this._getNormalizedItemIndex(value)
        }
        return this._normalizeItemIndex(this.getIndexByItemData(value))
    },
    getIndex: function(value) {
        if (this._isNormalizedItemIndex(value)) {
            return this._denormalizeItemIndex(value)
        }
        if (this._isItemIndex(value)) {
            return value
        }
        if (this._isNode(value)) {
            return this._denormalizeItemIndex(this._getNormalizedItemIndex(value))
        }
        return this.getIndexByItemData(value)
    },
    getItemElement: function(value) {
        if (this._isNormalizedItemIndex(value)) {
            return this._getItemByNormalizedIndex(value)
        }
        if (this._isItemIndex(value)) {
            return this._getItemByNormalizedIndex(this._normalizeItemIndex(value))
        }
        if (this._isNode(value)) {
            return $(value)
        }
        var normalizedItemIndex = this._normalizeItemIndex(this.getIndexByItemData(value));
        return this._getItemByNormalizedIndex(normalizedItemIndex)
    },
    _isNode: el => domAdapter.isNode(el && isRenderer(el) ? el.get(0) : el),
    deleteItemAtIndex: abstract,
    itemPlacementFunc: function(movingIndex, destinationIndex) {
        return this._itemsFromSameParent(movingIndex, destinationIndex) && movingIndex < destinationIndex ? "after" : "before"
    },
    moveItemAtIndexToIndex: abstract,
    _isNormalizedItemIndex: function(index) {
        return "number" === typeof index && Math.round(index) === index
    },
    _isItemIndex: abstract,
    _getNormalizedItemIndex: abstract,
    _normalizeItemIndex: abstract,
    _denormalizeItemIndex: abstract,
    _getItemByNormalizedIndex: abstract,
    _itemsFromSameParent: abstract
});
export default EditStrategy;
