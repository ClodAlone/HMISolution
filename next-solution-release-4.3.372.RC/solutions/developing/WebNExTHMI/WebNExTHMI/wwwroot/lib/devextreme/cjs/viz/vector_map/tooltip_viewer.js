/**
 * DevExtreme (cjs/viz/vector_map/tooltip_viewer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.TooltipViewer = TooltipViewer;
var TOOLTIP_OFFSET = 12;

function TooltipViewer(params) {
    this._subscribeToTracker(params.tracker, params.tooltip, params.layerCollection)
}
TooltipViewer.prototype = {
    constructor: TooltipViewer,
    dispose: function() {
        this._offTracker();
        this._offTracker = null
    },
    _subscribeToTracker: function(tracker, tooltip, layerCollection) {
        this._offTracker = tracker.on({
            "focus-on": function(arg) {
                var layer;
                var proxy;
                if (tooltip.isEnabled()) {
                    layer = layerCollection.byName(arg.data.name);
                    proxy = layer && layer.getProxy(arg.data.index);
                    var callback = function(result) {
                        result && arg.done(result)
                    };
                    proxy && callback(tooltip.show(proxy, {
                        x: arg.x,
                        y: arg.y,
                        offset: TOOLTIP_OFFSET
                    }, {
                        target: proxy
                    }, void 0, callback))
                }
            },
            "focus-move": function(arg) {
                tooltip.move(arg.x, arg.y, TOOLTIP_OFFSET)
            },
            "focus-off": function() {
                tooltip.hide()
            }
        })
    }
};
