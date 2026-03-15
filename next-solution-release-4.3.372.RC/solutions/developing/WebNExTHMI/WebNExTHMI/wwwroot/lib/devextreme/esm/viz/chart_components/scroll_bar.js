/**
 * DevExtreme (esm/viz/chart_components/scroll_bar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../../events/core/events_engine";
import {
    fireEvent
} from "../../events/utils/index";
import {
    extend
} from "../../core/utils/extend";
import {
    Translator2D
} from "../translators/translator2d";
import {
    isDefined
} from "../../core/utils/type";
import {
    noop
} from "../../core/utils/common";
import {
    start as dragEventStart,
    move as dragEventMove,
    end as dragEventEnd
} from "../../events/drag";
var _min = Math.min;
var _max = Math.max;
var MIN_SCROLL_BAR_SIZE = 2;
export var ScrollBar = function(renderer, group) {
    this._translator = new Translator2D({}, {}, {});
    this._scroll = renderer.rect().append(group);
    this._addEvents()
};

function _getXCoord(canvas, pos, offset, width) {
    var x = 0;
    if ("right" === pos) {
        x = canvas.width - canvas.right + offset
    } else if ("left" === pos) {
        x = canvas.left - offset - width
    }
    return x
}

function _getYCoord(canvas, pos, offset, width) {
    var y = 0;
    if ("top" === pos) {
        y = canvas.top - offset
    } else if ("bottom" === pos) {
        y = canvas.height - canvas.bottom + width + offset
    }
    return y
}
ScrollBar.prototype = {
    _addEvents: function() {
        var scrollElement = this._scroll.element;
        eventsEngine.on(scrollElement, dragEventStart, e => {
            fireEvent({
                type: "dxc-scroll-start",
                originalEvent: e,
                target: scrollElement
            })
        });
        eventsEngine.on(scrollElement, dragEventMove, e => {
            var dX = -e.offset.x * this._scale;
            var dY = -e.offset.y * this._scale;
            var lx = this._offset - (this._layoutOptions.vertical ? dY : dX) / this._scale;
            this._applyPosition(lx, lx + this._translator.canvasLength / this._scale);
            fireEvent({
                type: "dxc-scroll-move",
                originalEvent: e,
                target: scrollElement,
                offset: {
                    x: dX,
                    y: dY
                }
            })
        });
        eventsEngine.on(scrollElement, dragEventEnd, e => {
            fireEvent({
                type: "dxc-scroll-end",
                originalEvent: e,
                target: scrollElement,
                offset: {
                    x: -e.offset.x * this._scale,
                    y: -e.offset.y * this._scale
                }
            })
        })
    },
    update: function(options) {
        var position = options.position;
        var isVertical = options.rotated;
        var defaultPosition = isVertical ? "right" : "top";
        var secondaryPosition = isVertical ? "left" : "bottom";
        if (position !== defaultPosition && position !== secondaryPosition) {
            position = defaultPosition
        }
        this._scroll.attr({
            rotate: !options.rotated ? -90 : 0,
            rotateX: 0,
            rotateY: 0,
            fill: options.color,
            width: options.width,
            opacity: options.opacity
        });
        this._layoutOptions = {
            width: options.width,
            offset: options.offset,
            vertical: isVertical,
            position: position
        };
        return this
    },
    init: function(range, stick) {
        var isDiscrete = "discrete" === range.axisType;
        this._translateWithOffset = isDiscrete && !stick ? 1 : 0;
        this._translator.update(extend({}, range, {
            minVisible: null,
            maxVisible: null,
            visibleCategories: null
        }, isDiscrete && {
            min: null,
            max: null
        } || {}), this._canvas, {
            isHorizontal: !this._layoutOptions.vertical,
            stick: stick
        });
        return this
    },
    getOptions: function() {
        return this._layoutOptions
    },
    setPane: function(panes) {
        var position = this._layoutOptions.position;
        var pane;
        if ("left" === position || "top" === position) {
            pane = panes[0]
        } else {
            pane = panes[panes.length - 1]
        }
        this.pane = pane.name;
        return this
    },
    updateSize: function(canvas) {
        this._canvas = extend({}, canvas);
        var options = this._layoutOptions;
        var pos = options.position;
        var offset = options.offset;
        var width = options.width;
        this._scroll.attr({
            translateX: _getXCoord(canvas, pos, offset, width),
            translateY: _getYCoord(canvas, pos, offset, width)
        })
    },
    getMultipleAxesSpacing: function() {
        return 0
    },
    estimateMargins: function() {
        return this.getMargins()
    },
    getMargins: function() {
        var options = this._layoutOptions;
        var margins = {
            left: 0,
            top: 0,
            right: 0,
            bottom: 0
        };
        margins[options.position] = options.width + options.offset;
        return margins
    },
    shift: function(margins) {
        var _that$_scroll$attr, _that$_scroll$attr2;
        var options = this._layoutOptions;
        var side = options.position;
        var isVertical = options.vertical;
        var attr = {
            translateX: null !== (_that$_scroll$attr = this._scroll.attr("translateX")) && void 0 !== _that$_scroll$attr ? _that$_scroll$attr : 0,
            translateY: null !== (_that$_scroll$attr2 = this._scroll.attr("translateY")) && void 0 !== _that$_scroll$attr2 ? _that$_scroll$attr2 : 0
        };
        var shift = margins[side];
        attr[isVertical ? "translateX" : "translateY"] += ("left" === side || "top" === side ? -1 : 1) * shift;
        this._scroll.attr(attr)
    },
    hideTitle: noop,
    hideOuterElements: noop,
    setPosition: function(min, max) {
        var translator = this._translator;
        var minPoint = isDefined(min) ? translator.translate(min, -this._translateWithOffset) : translator.translate("canvas_position_start");
        var maxPoint = isDefined(max) ? translator.translate(max, this._translateWithOffset) : translator.translate("canvas_position_end");
        this._offset = _min(minPoint, maxPoint);
        this._scale = translator.getScale(min, max);
        this._applyPosition(_min(minPoint, maxPoint), _max(minPoint, maxPoint))
    },
    customPositionIsAvailable: () => false,
    dispose: function() {
        this._scroll.dispose();
        this._scroll = this._translator = null
    },
    _applyPosition: function(x1, x2) {
        var visibleArea = this._translator.getCanvasVisibleArea();
        x1 = _max(x1, visibleArea.min);
        x1 = _min(x1, visibleArea.max);
        x2 = _min(x2, visibleArea.max);
        x2 = _max(x2, visibleArea.min);
        var height = Math.abs(x2 - x1);
        this._scroll.attr({
            y: x1,
            height: height < MIN_SCROLL_BAR_SIZE ? MIN_SCROLL_BAR_SIZE : height
        })
    }
};
