/**
 * DevExtreme (cjs/viz/tree_map/drilldown.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _tree_map = _interopRequireDefault(require("./tree_map.base"));
var _helpers = require("../core/helpers");
require("./api");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var proto = _tree_map.default.prototype;
proto._eventsMap.onDrill = {
    name: "drill"
};
(0, _helpers.expand)(proto, "_extendProxyType", (function(proto) {
    var that = this;
    proto.drillDown = function() {
        that._drillToNode(this._id)
    }
}));
(0, _helpers.expand)(proto, "_onNodesCreated", (function() {
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
