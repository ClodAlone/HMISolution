/**
 * DevExtreme (esm/viz/tree_map/node.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend as _extend
} from "../../core/utils/extend";

function Node() {}
var updateTile = [updateLeaf, updateGroup];
_extend(Node.prototype, {
    value: 0,
    isNode: function() {
        return !!(this.nodes && this.level < this.ctx.maxLevel)
    },
    isActive: function() {
        var ctx = this.ctx;
        return this.level >= ctx.minLevel && this.level <= ctx.maxLevel
    },
    updateStyles: function() {
        var isNode = Number(this.isNode());
        this.state = this._buildState(this.ctx.settings[isNode].state, !isNode && this.color && {
            fill: this.color
        })
    },
    _buildState: function(state, extra) {
        var base = _extend({}, state);
        return extra ? _extend(base, extra) : base
    },
    updateLabelStyle: function() {
        var settings = this.ctx.settings[Number(this.isNode())];
        this.labelState = settings.labelState;
        this.labelParams = settings.labelParams
    },
    _getState: function() {
        return this.state
    },
    applyState: function() {
        updateTile[Number(this.isNode())](this.tile, this._getState())
    }
});

function updateLeaf(content, attrs) {
    content.smartAttr(attrs)
}

function updateGroup(content, attrs) {
    content.outer.attr({
        stroke: attrs.stroke,
        "stroke-width": attrs["stroke-width"],
        "stroke-opacity": attrs["stroke-opacity"]
    });
    content.inner.smartAttr({
        fill: attrs.fill,
        opacity: attrs.opacity,
        hatching: attrs.hatching
    })
}
export default Node;
