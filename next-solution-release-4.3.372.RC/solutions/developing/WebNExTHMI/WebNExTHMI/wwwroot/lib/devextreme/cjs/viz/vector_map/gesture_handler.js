/**
 * DevExtreme (cjs/viz/vector_map/gesture_handler.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GestureHandler = GestureHandler;
var _ln = Math.log;
var _LN2 = Math.LN2;

function GestureHandler(params) {
    this._projection = params.projection;
    this._renderer = params.renderer;
    this._x = this._y = 0;
    this._subscribeToTracker(params.tracker)
}
GestureHandler.prototype = {
    constructor: GestureHandler,
    dispose: function() {
        this._offTracker();
        this._offTracker = null
    },
    _subscribeToTracker: function(tracker) {
        var that = this;
        var isActive = false;
        that._offTracker = tracker.on({
            start: function(arg) {
                isActive = "control-bar" !== arg.data.name;
                if (isActive) {
                    that._processStart(arg)
                }
            },
            move: function(arg) {
                if (isActive) {
                    that._processMove(arg)
                }
            },
            end: function() {
                if (isActive) {
                    that._processEnd()
                }
            },
            zoom: function(arg) {
                that._processZoom(arg)
            }
        })
    },
    setInteraction: function(options) {
        this._processEnd();
        this._centeringEnabled = options.centeringEnabled;
        this._zoomingEnabled = options.zoomingEnabled
    },
    _processStart: function(arg) {
        if (this._centeringEnabled) {
            this._x = arg.x;
            this._y = arg.y;
            this._projection.beginMoveCenter()
        }
    },
    _processMove: function(arg) {
        if (this._centeringEnabled) {
            this._renderer.root.attr({
                cursor: "move"
            });
            this._projection.moveCenter([this._x - arg.x, this._y - arg.y]);
            this._x = arg.x;
            this._y = arg.y
        }
    },
    _processEnd: function() {
        if (this._centeringEnabled) {
            this._renderer.root.attr({
                cursor: "default"
            });
            this._projection.endMoveCenter()
        }
    },
    _processZoom: function(arg) {
        var delta;
        var screenPosition;
        var coords;
        if (this._zoomingEnabled) {
            if (arg.delta) {
                delta = arg.delta
            } else if (arg.ratio) {
                delta = _ln(arg.ratio) / _LN2
            }
            if (this._centeringEnabled) {
                screenPosition = this._renderer.getRootOffset();
                screenPosition = [arg.x - screenPosition.left, arg.y - screenPosition.top];
                coords = this._projection.fromScreenPoint(screenPosition)
            }
            this._projection.changeScaledZoom(delta);
            if (this._centeringEnabled) {
                this._projection.setCenterByPoint(coords, screenPosition)
            }
        }
    }
};
