/**
 * DevExtreme (esm/viz/tree_map/hover.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import TreeMapBase from "./tree_map.base";
import Node from "./node";
import {
    expand
} from "../core/helpers";
import {
    buildRectAppearance
} from "./common";
import {
    parseScalar as _parseScalar
} from "../core/utils";
var proto = TreeMapBase.prototype;
var nodeProto = Node.prototype;
var STATE_CODE = 1;
import "./api";
import "./states";
proto._eventsMap.onHoverChanged = {
    name: "hoverChanged"
};
expand(proto._handlers, "calculateAdditionalStates", (function(states, options) {
    states[1] = options.hoverStyle ? buildRectAppearance(options.hoverStyle) : {}
}));
TreeMapBase.addChange({
    code: "HOVER_ENABLED",
    handler: function() {
        var hoverEnabled = _parseScalar(this._getOption("hoverEnabled", true), true);
        if (!hoverEnabled) {
            this.clearHover()
        }
        this._hoverEnabled = hoverEnabled
    },
    isThemeDependent: true,
    isOptionChange: true,
    option: "hoverEnabled"
});
nodeProto.statesMap[1] = 1;
nodeProto.additionalStates.push(1);
expand(proto, "_extendProxyType", (function(proto) {
    var that = this;
    proto.setHover = function() {
        that._hoverNode(this._id)
    };
    proto.isHovered = function() {
        return that._hoverIndex === this._id
    }
}));
expand(proto, "_onNodesCreated", (function() {
    this._hoverIndex = -1
}));
expand(proto, "_changeGroupSettings", (function() {
    this._groupHoverEnabled = _parseScalar(this._getOption("group").hoverEnabled, true);
    if (!this._groupHoverEnabled) {
        this.clearHover()
    }
}));
proto._applyHoverState = function(index, state) {
    setNodeStateRecursive(this._nodes[index], STATE_CODE, state);
    this._eventTrigger("hoverChanged", {
        node: this._nodes[index].proxy
    })
};

function setNodeStateRecursive(node, code, state) {
    var nodes = node.isNode() && node.nodes;
    var i;
    var ii = nodes && nodes.length;
    node.setState(code, state);
    for (i = 0; i < ii; ++i) {
        setNodeStateRecursive(nodes[i], code, state)
    }
}
proto._hoverNode = function(index) {
    var currentIndex = this._hoverIndex;
    if (this._hoverEnabled && currentIndex !== index) {
        if (!this._groupHoverEnabled && index >= 0 && this._nodes[index].isNode()) {
            this.clearHover();
            return
        }
        this._context.suspend();
        this._hoverIndex = -1;
        if (currentIndex >= 0) {
            this._applyHoverState(currentIndex, false)
        }
        this._hoverIndex = index;
        if (index >= 0) {
            this._applyHoverState(index, true)
        }
        this._context.resume()
    }
};
proto.clearHover = function() {
    this._hoverNode(-1)
};
