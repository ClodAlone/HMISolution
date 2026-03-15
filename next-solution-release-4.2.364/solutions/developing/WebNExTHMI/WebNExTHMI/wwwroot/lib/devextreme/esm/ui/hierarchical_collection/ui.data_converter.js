/**
 * DevExtreme (esm/ui/hierarchical_collection/ui.data_converter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Class from "../../core/class";
import {
    extend
} from "../../core/utils/extend";
import errors from "../../ui/widget/ui.errors";
import {
    each
} from "../../core/utils/iterator";
import {
    isDefined,
    isPrimitive
} from "../../core/utils/type";
var DataConverter = Class.inherit({
    ctor: function() {
        this._dataStructure = [];
        this._itemsCount = 0;
        this._visibleItemsCount = 0
    },
    _indexByKey: {},
    _convertItemsToNodes: function(items, parentKey) {
        var that = this;
        each(items, (function(_, item) {
            var parentId = isDefined(parentKey) ? parentKey : that._getParentId(item);
            var node = that._convertItemToNode(item, parentId);
            that._dataStructure.push(node);
            that._checkForDuplicateId(node.internalFields.key);
            that._indexByKey[node.internalFields.key] = that._dataStructure.length - 1;
            if (that._itemHasChildren(item)) {
                that._convertItemsToNodes(that._dataAccessors.getters.items(item), node.internalFields.key)
            }
        }))
    },
    _checkForDuplicateId: function(key) {
        if (isDefined(this._indexByKey[key])) {
            throw errors.Error("E1040", key)
        }
    },
    _getParentId: function(item) {
        return "plain" === this._dataType ? this._dataAccessors.getters.parentKey(item) : void 0
    },
    _itemHasChildren: function(item) {
        if ("plain" === this._dataType) {
            return
        }
        var items = this._dataAccessors.getters.items(item);
        return items && items.length
    },
    _getUniqueKey: function(item) {
        var keyGetter = this._dataAccessors.getters.key;
        var itemKey = keyGetter(item);
        var isCorrectKey = keyGetter && (itemKey || 0 === itemKey) && isPrimitive(itemKey);
        return isCorrectKey ? itemKey : this.getItemsCount()
    },
    _convertItemToNode: function(item, parentKey) {
        this._itemsCount++;
        false !== item.visible && this._visibleItemsCount++;
        var node = {
            internalFields: {
                disabled: this._dataAccessors.getters.disabled(item, {
                    defaultValue: false
                }),
                expanded: this._dataAccessors.getters.expanded(item, {
                    defaultValue: false
                }),
                selected: this._dataAccessors.getters.selected(item, {
                    defaultValue: false
                }),
                key: this._getUniqueKey(item),
                parentKey: isDefined(parentKey) ? parentKey : this._rootValue,
                item: this._makeObjectFromPrimitive(item),
                childrenKeys: []
            }
        };
        extend(node, item);
        delete node.items;
        return node
    },
    setChildrenKeys: function() {
        var that = this;
        each(this._dataStructure, (function(_, node) {
            if (node.internalFields.parentKey === that._rootValue) {
                return
            }
            var parent = that.getParentNode(node);
            parent && parent.internalFields.childrenKeys.push(node.internalFields.key)
        }))
    },
    _makeObjectFromPrimitive: function(item) {
        if (isPrimitive(item)) {
            var key = item;
            item = {};
            this._dataAccessors.setters.key(item, key)
        }
        return item
    },
    _convertToPublicNode: function(node, parent) {
        if (!node) {
            return null
        }
        var publicNode = {
            text: this._dataAccessors.getters.display(node),
            key: node.internalFields.key,
            selected: node.internalFields.selected,
            expanded: node.internalFields.expanded,
            disabled: node.internalFields.disabled,
            parent: parent || null,
            itemData: node.internalFields.item,
            children: [],
            items: []
        };
        if (publicNode.parent) {
            publicNode.parent.children.push(publicNode);
            publicNode.parent.items.push(publicNode)
        }
        return publicNode
    },
    convertToPublicNodes: function(data, parent) {
        if (!data.length) {
            return []
        }
        var that = this;
        var publicNodes = [];
        each(data, (function(_, node) {
            node = isPrimitive(node) ? that._getByKey(node) : node;
            var publicNode = that._convertToPublicNode(node, parent);
            publicNode.children = that.convertToPublicNodes(node.internalFields.childrenKeys, publicNode);
            publicNodes.push(publicNode);
            node.internalFields.publicNode = publicNode
        }));
        return publicNodes
    },
    setDataAccessors: function(accessors) {
        this._dataAccessors = accessors
    },
    _getByKey: function(key) {
        return this._dataStructure[this.getIndexByKey(key)] || null
    },
    getParentNode: function(node) {
        return this._getByKey(node.internalFields.parentKey)
    },
    getByKey: function(data, key) {
        if (null === key || void 0 === key) {
            return null
        }
        var result = null;
        var that = this;
        var getByKey = function(data, key) {
            each(data, (function(_, element) {
                var currentElementKey = element.internalFields && element.internalFields.key || that._dataAccessors.getters.key(element);
                if (currentElementKey.toString() === key.toString()) {
                    result = element;
                    return false
                }
            }));
            return result
        };
        return getByKey(data, key)
    },
    getItemsCount: function() {
        return this._itemsCount
    },
    getVisibleItemsCount: function() {
        return this._visibleItemsCount
    },
    updateIndexByKey: function() {
        var that = this;
        this._indexByKey = {};
        each(this._dataStructure, (function(index, node) {
            that._checkForDuplicateId(node.internalFields.key);
            that._indexByKey[node.internalFields.key] = index
        }))
    },
    updateChildrenKeys: function() {
        this._indexByKey = {};
        this.removeChildrenKeys();
        this.updateIndexByKey();
        this.setChildrenKeys()
    },
    removeChildrenKeys: function() {
        this._indexByKey = {};
        each(this._dataStructure, (function(index, node) {
            node.internalFields.childrenKeys = []
        }))
    },
    getIndexByKey: function(key) {
        return this._indexByKey[key]
    },
    createPlainStructure: function(items, rootValue, dataType) {
        this._itemsCount = 0;
        this._visibleItemsCount = 0;
        this._rootValue = rootValue;
        this._dataType = dataType;
        this._indexByKey = {};
        this._convertItemsToNodes(items);
        this.setChildrenKeys();
        return this._dataStructure
    }
});
export default DataConverter;
