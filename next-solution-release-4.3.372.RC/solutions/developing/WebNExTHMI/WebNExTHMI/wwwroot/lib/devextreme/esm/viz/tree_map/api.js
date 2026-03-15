/**
 * DevExtreme (esm/viz/tree_map/api.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import TreeMapBase from "./tree_map.base";
import Node from "./node";
import {
    extend as _extend
} from "../../core/utils/extend";
import {
    noop
} from "../../core/utils/common";
var proto = TreeMapBase.prototype;
var nodeProto = Node.prototype;
proto._eventsMap.onNodesInitialized = {
    name: "nodesInitialized"
};
proto._eventsMap.onNodesRendering = {
    name: "nodesRendering"
};
proto._createProxyType = function() {
    var that = this;
    var nodes;
    Proxy.prototype = {
        constructor: Proxy,
        getParent: function() {
            return nodes[this._id].parent.proxy || null
        },
        getChild: function(index) {
            var _nodes = nodes[this._id].nodes;
            return _nodes ? _nodes[index].proxy : null
        },
        getChildrenCount: function() {
            var _nodes = nodes[this._id].nodes;
            return _nodes ? _nodes.length : 0
        },
        getAllChildren: function() {
            var _nodes = nodes[this._id].nodes;
            var i;
            var ii = _nodes && _nodes.length;
            var list = [];
            for (i = 0; i < ii; ++i) {
                list.push(_nodes[i].proxy)
            }
            return list
        },
        getAllNodes: function() {
            var list = [];
            collectNodes(nodes[this._id], list);
            return list
        },
        isLeaf: function() {
            return !nodes[this._id].isNode()
        },
        isActive: function() {
            return nodes[this._id].isActive()
        },
        value: function(arg) {
            var node = nodes[this._id];
            var result;
            if (void 0 !== arg) {
                updateValue(node, arg > 0 ? Number(arg) : 0);
                change(node, ["TILING"]);
                result = this
            } else {
                result = node.value
            }
            return result
        },
        label: function(arg) {
            var node = nodes[this._id];
            var result;
            if (void 0 !== arg) {
                node.customLabel = arg ? String(arg) : null;
                change(node, ["LABELS"]);
                result = this
            } else {
                result = node.customLabel || node.label
            }
            return result
        },
        customize: function(settings) {
            var node = nodes[this._id];
            if (settings) {
                node._custom = node._custom || {};
                _extend(true, node._custom, settings);
                node._partialState = node._partialLabelState = null
            }
            change(node, ["TILES", "LABELS"]);
            return this
        },
        resetCustomization: function() {
            var node = nodes[this._id];
            node._custom = node._partialState = node._partialLabelState = null;
            change(node, ["TILES", "LABELS"]);
            return this
        }
    };
    that._extendProxyType(Proxy.prototype);

    function Proxy(node) {
        node.proxy = this;
        this._id = node._id;
        this.level = node.level;
        this.index = node.index;
        this.data = node.data
    }
    that._handlers.beginBuildNodes = function() {
        nodes = that._nodes;
        new Proxy(that._root)
    };
    that._handlers.buildNode = function(node) {
        new Proxy(node)
    };
    that._handlers.endBuildNodes = function() {
        that._eventTrigger("nodesInitialized", {
            root: that._root.proxy
        })
    }
};

function change(node, codes) {
    var ctx = node.ctx;
    ctx.suspend();
    ctx.change(codes);
    ctx.resume()
}

function collectNodes(node, list) {
    var nodes = node.nodes;
    var i;
    var ii = nodes && nodes.length;
    for (i = 0; i < ii; ++i) {
        list.push(nodes[i].proxy);
        collectNodes(nodes[i], list)
    }
}

function updateValue(node, value) {
    var delta = value - node.value;
    while (node) {
        node.value += delta;
        node = node.parent
    }
}
proto._extendProxyType = noop;
var _resetNodes = proto._resetNodes;
proto._resetNodes = function() {
    _resetNodes.call(this);
    this._eventTrigger("nodesRendering", {
        node: this._topNode.proxy
    })
};
var _updateStyles = nodeProto.updateStyles;
nodeProto.updateStyles = function() {
    _updateStyles.call(this);
    if (this._custom) {
        this._partialState = !this.ctx.forceReset && this._partialState || this.ctx.calculateState(this._custom);
        _extend(true, this.state, this._partialState)
    }
};
var _updateLabelStyle = nodeProto.updateLabelStyle;
nodeProto.updateLabelStyle = function() {
    var custom = this._custom;
    _updateLabelStyle.call(this);
    if (custom && custom.label) {
        this._partialLabelState = !this.ctx.forceReset && this._partialLabelState || calculatePartialLabelState(this, custom.label);
        this.labelState = _extend(true, {}, this.labelState, this._partialLabelState)
    }
};

function calculatePartialLabelState(node, settings) {
    var state = node.ctx.calculateLabelState(settings);
    if ("visible" in settings) {
        state.visible = !!settings.visible
    }
    return state
}
proto.getRootNode = function() {
    return this._root.proxy
};
proto.resetNodes = function() {
    var context = this._context;
    context.suspend();
    context.change(["NODES_CREATE"]);
    context.resume();
    return this
};
