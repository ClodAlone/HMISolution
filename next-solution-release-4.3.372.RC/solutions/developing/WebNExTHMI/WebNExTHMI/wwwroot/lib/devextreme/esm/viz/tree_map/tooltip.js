/**
 * DevExtreme (esm/viz/tree_map/tooltip.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    expand
} from "../core/helpers";
import TreeMapBase from "./tree_map.base";
var proto = TreeMapBase.prototype;
import "./api";
expand(proto, "_extendProxyType", (function(proto) {
    var that = this;
    proto.showTooltip = function(coords) {
        that._showTooltip(this._id, coords)
    }
}));
expand(proto, "_onNodesCreated", (function() {
    if (this._tooltipIndex >= 0) {
        this._tooltip.hide()
    }
    this._tooltipIndex = -1
}));
expand(proto, "_onTilingPerformed", (function() {
    if (this._tooltipIndex >= 0) {
        this._moveTooltip(this._nodes[this._tooltipIndex])
    }
}));

function getCoords(coords, rect, renderer) {
    var offset = renderer.getRootOffset();
    return coords || rect && [(rect[0] + rect[2]) / 2 + offset.left, (rect[1] + rect[3]) / 2 + offset.top] || [-1e3, -1e3]
}
proto._showTooltip = function(index, coords) {
    var that = this;
    var tooltip = that._tooltip;
    var node = that._nodes[index];
    if (that._tooltipIndex === index) {
        that._moveTooltip(node, coords);
        return
    }
    var callback = result => {
        if (void 0 === result) {
            return
        }
        if (!result) {
            tooltip.hide()
        }
        that._tooltipIndex = result ? index : -1
    };
    var xy = getCoords(coords, node.rect, this._renderer);
    callback(tooltip.show({
        value: node.value,
        valueText: tooltip.formatValue(node.value),
        node: node.proxy
    }, {
        x: xy[0],
        y: xy[1],
        offset: 0
    }, {
        node: node.proxy
    }, void 0, callback))
};
proto._moveTooltip = function(node, coords) {
    var xy = getCoords(coords, node.rect, this._renderer);
    this._tooltip.move(xy[0], xy[1], 0)
};
proto.hideTooltip = function() {
    if (this._tooltipIndex >= 0) {
        this._tooltipIndex = -1;
        this._tooltip.hide()
    }
};
import {
    plugin as tooltipPlugin
} from "../core/tooltip";
TreeMapBase.addPlugin(tooltipPlugin);
