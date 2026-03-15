/**
 * DevExtreme (esm/ui/scroll_view/ui.scrollbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import eventsEngine from "../../events/core/events_engine";
import readyCallback from "../../core/utils/ready_callbacks";
import {
    move
} from "../../animation/translator";
import Widget from "../widget/ui.widget";
import {
    addNamespace
} from "../../events/utils/index";
import {
    deferRenderer
} from "../../core/utils/common";
import {
    isPlainObject
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import pointerEvents from "../../events/pointer";
var SCROLLBAR = "dxScrollbar";
var SCROLLABLE_SCROLLBAR_CLASS = "dx-scrollable-scrollbar";
var SCROLLABLE_SCROLLBAR_ACTIVE_CLASS = "".concat(SCROLLABLE_SCROLLBAR_CLASS, "-active");
var SCROLLABLE_SCROLL_CLASS = "dx-scrollable-scroll";
var SCROLLABLE_SCROLL_CONTENT_CLASS = "dx-scrollable-scroll-content";
var HOVER_ENABLED_STATE = "dx-scrollbar-hoverable";
var HORIZONTAL = "horizontal";
var THUMB_MIN_SIZE = 15;
var SCROLLBAR_VISIBLE = {
    onScroll: "onScroll",
    onHover: "onHover",
    always: "always",
    never: "never"
};
var activeScrollbar = null;
var Scrollbar = Widget.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            direction: null,
            visible: false,
            activeStateEnabled: false,
            visibilityMode: SCROLLBAR_VISIBLE.onScroll,
            containerSize: 0,
            contentSize: 0,
            expandable: true,
            scaleRatio: 1
        })
    },
    _init: function() {
        this.callBase();
        this._isHovered = false
    },
    _initMarkup: function() {
        this._renderThumb();
        this.callBase()
    },
    _render: function() {
        this.callBase();
        this._renderDirection();
        this._update();
        this._attachPointerDownHandler();
        this.option("hoverStateEnabled", this._isHoverMode());
        this.$element().toggleClass(HOVER_ENABLED_STATE, this.option("hoverStateEnabled"))
    },
    _renderThumb: function() {
        this._$thumb = $("<div>").addClass(SCROLLABLE_SCROLL_CLASS);
        $("<div>").addClass(SCROLLABLE_SCROLL_CONTENT_CLASS).appendTo(this._$thumb);
        this.$element().addClass(SCROLLABLE_SCROLLBAR_CLASS).append(this._$thumb)
    },
    isThumb: function($element) {
        return !!this.$element().find($element).length
    },
    _isHoverMode: function() {
        var visibilityMode = this.option("visibilityMode");
        return (visibilityMode === SCROLLBAR_VISIBLE.onHover || visibilityMode === SCROLLBAR_VISIBLE.always) && this.option("expandable")
    },
    _renderDirection: function() {
        var direction = this.option("direction");
        this.$element().addClass("dx-scrollbar-" + direction);
        this._dimension = direction === HORIZONTAL ? "width" : "height";
        this._prop = direction === HORIZONTAL ? "left" : "top"
    },
    _attachPointerDownHandler: function() {
        eventsEngine.on(this._$thumb, addNamespace(pointerEvents.down, SCROLLBAR), this.feedbackOn.bind(this))
    },
    feedbackOn: function() {
        this.$element().addClass(SCROLLABLE_SCROLLBAR_ACTIVE_CLASS);
        activeScrollbar = this
    },
    feedbackOff: function() {
        this.$element().removeClass(SCROLLABLE_SCROLLBAR_ACTIVE_CLASS);
        activeScrollbar = null
    },
    cursorEnter: function() {
        this._isHovered = true;
        if (this._needScrollbar()) {
            this.option("visible", true)
        }
    },
    cursorLeave: function() {
        this._isHovered = false;
        this.option("visible", false)
    },
    _renderDimensions: function() {
        this._$thumb.css({
            width: this.option("width"),
            height: this.option("height")
        })
    },
    _toggleVisibility: function(visible) {
        if (this.option("visibilityMode") === SCROLLBAR_VISIBLE.onScroll) {
            this._$thumb.css("opacity")
        }
        visible = this._adjustVisibility(visible);
        this.option().visible = visible;
        this._$thumb.toggleClass("dx-state-invisible", !visible)
    },
    _adjustVisibility: function(visible) {
        if (this._baseContainerToContentRatio && !this._needScrollbar()) {
            return false
        }
        switch (this.option("visibilityMode")) {
            case SCROLLBAR_VISIBLE.onScroll:
                break;
            case SCROLLBAR_VISIBLE.onHover:
                visible = visible || !!this._isHovered;
                break;
            case SCROLLBAR_VISIBLE.never:
                visible = false;
                break;
            case SCROLLBAR_VISIBLE.always:
                visible = true
        }
        return visible
    },
    moveTo: function(location) {
        if (this._isHidden()) {
            return
        }
        if (isPlainObject(location)) {
            location = location[this._prop] || 0
        }
        var scrollBarLocation = {};
        scrollBarLocation[this._prop] = this._calculateScrollBarPosition(location);
        move(this._$thumb, scrollBarLocation)
    },
    _calculateScrollBarPosition: function(location) {
        return -location * this._thumbRatio
    },
    _update: function() {
        var containerSize = Math.round(this.option("containerSize"));
        var contentSize = Math.round(this.option("contentSize"));
        var baseContainerSize = Math.round(this.option("baseContainerSize"));
        var baseContentSize = Math.round(this.option("baseContentSize"));
        if (isNaN(baseContainerSize)) {
            baseContainerSize = containerSize;
            baseContentSize = contentSize
        }
        this._baseContainerToContentRatio = baseContentSize ? baseContainerSize / baseContentSize : baseContainerSize;
        this._realContainerToContentRatio = contentSize ? containerSize / contentSize : containerSize;
        var thumbSize = Math.round(Math.max(Math.round(containerSize * this._realContainerToContentRatio), THUMB_MIN_SIZE));
        this._thumbRatio = (containerSize - thumbSize) / (this.option("scaleRatio") * (contentSize - containerSize));
        this.option(this._dimension, thumbSize / this.option("scaleRatio"));
        this.$element().css("display", this._needScrollbar() ? "" : "none")
    },
    _isHidden: function() {
        return this.option("visibilityMode") === SCROLLBAR_VISIBLE.never
    },
    _needScrollbar: function() {
        return !this._isHidden() && this._baseContainerToContentRatio < 1
    },
    containerToContentRatio: function() {
        return this._realContainerToContentRatio
    },
    _normalizeSize: function(size) {
        return isPlainObject(size) ? size[this._dimension] || 0 : size
    },
    _clean: function() {
        this.callBase();
        if (this === activeScrollbar) {
            activeScrollbar = null
        }
        eventsEngine.off(this._$thumb, "." + SCROLLBAR)
    },
    _optionChanged: function(args) {
        if (this._isHidden()) {
            return
        }
        switch (args.name) {
            case "containerSize":
            case "contentSize":
                this.option()[args.name] = this._normalizeSize(args.value);
                this._update();
                break;
            case "baseContentSize":
            case "baseContainerSize":
                this._update();
                break;
            case "visibilityMode":
            case "direction":
                this._invalidate();
                break;
            case "scaleRatio":
                this._update();
                break;
            default:
                this.callBase.apply(this, arguments)
        }
    },
    update: deferRenderer((function() {
        this._adjustVisibility() && this.option("visible", true)
    }))
});
readyCallback.add((function() {
    eventsEngine.subscribeGlobal(domAdapter.getDocument(), addNamespace(pointerEvents.up, SCROLLBAR), (function() {
        if (activeScrollbar) {
            activeScrollbar.feedbackOff()
        }
    }))
}));
export default Scrollbar;
