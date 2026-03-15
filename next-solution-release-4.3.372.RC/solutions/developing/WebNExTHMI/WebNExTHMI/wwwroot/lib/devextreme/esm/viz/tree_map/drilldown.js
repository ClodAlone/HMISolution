/**
 * DevExtreme (esm/viz/tree_map/drilldown.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import TreeMapBase from "./tree_map.base";
import {
    expand as _expand
} from "../core/helpers";
import "./api";
var proto = TreeMapBase.prototype;
proto._eventsMap.onDrill = {
    name: "drill"
};
_expand(proto, "_extendProxyType", (function(proto) {
    var that = this;
    proto.drillDown = function() {
        that._drillToNode(this._id)
    }
}));
_expand(proto, "_onNodesCreated", (function() {
    this._drilldownIndex = -1
}));
proto._drillToNode = function(index) {
    var node;
    if (this._drilldownIndex !== index) {
        node = this._nodes[index] || this._root;
        if (node.nodes) {
            this._drilldownIndex = index;
            this._topNode = node;
            this._context.suspend();
            this._context.change(["MAX_DEPTH", "NODES_RESET"]);
            this._context.resume();
            this._eventTrigger("drill", {
                node: node.proxy
            })
        }
    }
};
proto.resetDrillDown = function() {
    this._drillToNode(-1);
    return this
};
proto.drillUp = function() {
    this._drillToNode(this._topNode.parent._id || -1);
    return this
};
proto.getCurrentNode = function() {
    return this._topNode.proxy
};
