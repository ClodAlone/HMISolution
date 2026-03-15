/**
 * DevExtreme (esm/ui/scroll_view/ui.scrollable.simulated.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import eventsEngine from "../../events/core/events_engine";
import {
    titleize
} from "../../core/utils/inflector";
import {
    extend
} from "../../core/utils/extend";
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
import {
    each,
    map
} from "../../core/utils/iterator";
import {
    isDefined
} from "../../core/utils/type";
import {
    getBoundingRect
} from "../../core/utils/position";
import {
    resetPosition,
    move,
    locate
} from "../../animation/translator";
import Class from "../../core/class";
import Animator from "./animator";
import {
    isDxMouseWheelEvent,
    addNamespace as addEventNamespace,
    normalizeKeyName,
    isCommandKeyPressed
} from "../../events/utils/index";
import {
    deferUpdate,
    deferUpdater,
    deferRender,
    deferRenderer,
    noop
} from "../../core/utils/common";
import Scrollbar from "./ui.scrollbar";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
var SCROLLABLE_SIMULATED = "dxSimulatedScrollable";
var SCROLLABLE_STRATEGY = "dxScrollableStrategy";
var SCROLLABLE_SIMULATED_CURSOR = SCROLLABLE_SIMULATED + "Cursor";
var SCROLLABLE_SIMULATED_KEYBOARD = SCROLLABLE_SIMULATED + "Keyboard";
var SCROLLABLE_SIMULATED_CLASS = "dx-scrollable-simulated";
var SCROLLABLE_SCROLLBARS_ALWAYSVISIBLE = "dx-scrollable-scrollbars-alwaysvisible";
var SCROLLABLE_SCROLLBAR_CLASS = "dx-scrollable-scrollbar";
var VERTICAL = "vertical";
var HORIZONTAL = "horizontal";
var ACCELERATION = .92;
var OUT_BOUNDS_ACCELERATION = .5;
var MIN_VELOCITY_LIMIT = 1;
var FRAME_DURATION = Math.round(1e3 / 60);
var SCROLL_LINE_HEIGHT = 40;
var VALIDATE_WHEEL_TIMEOUT = 500;
var BOUNCE_MIN_VELOCITY_LIMIT = MIN_VELOCITY_LIMIT / 5;
var BOUNCE_DURATION = 400;
var BOUNCE_FRAMES = BOUNCE_DURATION / FRAME_DURATION;
var BOUNCE_ACCELERATION_SUM = (1 - Math.pow(ACCELERATION, BOUNCE_FRAMES)) / (1 - ACCELERATION);
var KEY_CODES = {
    PAGE_UP: "pageUp",
    PAGE_DOWN: "pageDown",
    END: "end",
    HOME: "home",
    LEFT: "leftArrow",
    UP: "upArrow",
    RIGHT: "rightArrow",
    DOWN: "downArrow",
    TAB: "tab"
};
var InertiaAnimator = Animator.inherit({
    ctor: function(scroller) {
        this.callBase();
        this.scroller = scroller
    },
    VELOCITY_LIMIT: MIN_VELOCITY_LIMIT,
    _isFinished: function() {
        return Math.abs(this.scroller._velocity) <= this.VELOCITY_LIMIT
    },
    _step: function() {
        this.scroller._scrollStep(this.scroller._velocity);
        this.scroller._velocity *= this._acceleration()
    },
    _acceleration: function() {
        return this.scroller._inBounds() ? ACCELERATION : OUT_BOUNDS_ACCELERATION
    },
    _complete: function() {
        this.scroller._scrollComplete()
    }
});
var BounceAnimator = InertiaAnimator.inherit({
    VELOCITY_LIMIT: BOUNCE_MIN_VELOCITY_LIMIT,
    _isFinished: function() {
        return this.scroller._crossBoundOnNextStep() || this.callBase()
    },
    _acceleration: function() {
        return ACCELERATION
    },
    _complete: function() {
        this.scroller._move(this.scroller._bounceLocation);
        this.callBase()
    }
});
export var Scroller = Class.inherit({
    ctor: function(options) {
        this._initOptions(options);
        this._initAnimators();
        this._initScrollbar()
    },
    _initOptions: function(options) {
        this._location = 0;
        this._topReached = false;
        this._bottomReached = false;
        this._axis = options.direction === HORIZONTAL ? "x" : "y";
        this._prop = options.direction === HORIZONTAL ? "left" : "top";
        this._dimension = options.direction === HORIZONTAL ? "width" : "height";
        this._scrollProp = options.direction === HORIZONTAL ? "scrollLeft" : "scrollTop";
        each(options, (optionName, optionValue) => {
            this["_" + optionName] = optionValue
        })
    },
    _initAnimators: function() {
        this._inertiaAnimator = new InertiaAnimator(this);
        this._bounceAnimator = new BounceAnimator(this)
    },
    _initScrollbar: function() {
        this._scrollbar = new Scrollbar($("<div>").appendTo(this._$container), {
            direction: this._direction,
            visible: this._scrollByThumb,
            visibilityMode: this._visibilityModeNormalize(this._scrollbarVisible),
            expandable: this._scrollByThumb
        });
        this._$scrollbar = this._scrollbar.$element()
    },
    _visibilityModeNormalize: function(mode) {
        return true === mode ? "onScroll" : false === mode ? "never" : mode
    },
    _scrollStep: function(delta) {
        var prevLocation = this._location;
        this._location += delta;
        this._suppressBounce();
        this._move();
        if (Math.abs(prevLocation - this._location) < 1) {
            return
        }
        eventsEngine.triggerHandler(this._$container, {
            type: "scroll"
        })
    },
    _suppressBounce: function() {
        if (this._bounceEnabled || this._inBounds(this._location)) {
            return
        }
        this._velocity = 0;
        this._location = this._boundLocation()
    },
    _boundLocation: function(location) {
        location = void 0 !== location ? location : this._location;
        return Math.max(Math.min(location, this._maxOffset), this._minOffset)
    },
    _move: function(location) {
        this._location = void 0 !== location ? location * this._getScaleRatio() : this._location;
        this._moveContent();
        this._moveScrollbar()
    },
    _moveContent: function() {
        var location = this._location;
        this._$container[this._scrollProp](-location / this._getScaleRatio());
        this._moveContentByTranslator(location)
    },
    _getScaleRatio: function() {
        if (hasWindow() && !this._scaleRatio) {
            var element = this._$element.get(0);
            var realDimension = this._getRealDimension(element, this._dimension);
            var baseDimension = this._getBaseDimension(element, this._dimension);
            this._scaleRatio = Math.round(realDimension / baseDimension * 100) / 100
        }
        return this._scaleRatio || 1
    },
    _getRealDimension: function(element, dimension) {
        return Math.round(getBoundingRect(element)[dimension])
    },
    _getBaseDimension: function(element, dimension) {
        var dimensionName = "offset" + titleize(dimension);
        return element[dimensionName]
    },
    _moveContentByTranslator: function(location) {
        var translateOffset;
        var minOffset = -this._maxScrollPropValue;
        if (location > 0) {
            translateOffset = location
        } else if (location <= minOffset) {
            translateOffset = location - minOffset
        } else {
            translateOffset = location % 1
        }
        if (this._translateOffset === translateOffset) {
            return
        }
        var targetLocation = {};
        targetLocation[this._prop] = translateOffset;
        this._translateOffset = translateOffset;
        if (0 === translateOffset) {
            resetPosition(this._$content);
            return
        }
        move(this._$content, targetLocation)
    },
    _moveScrollbar: function() {
        this._scrollbar.moveTo(this._location)
    },
    _scrollComplete: function() {
        if (this._inBounds()) {
            this._hideScrollbar();
            if (this._completeDeferred) {
                this._completeDeferred.resolve()
            }
        }
        this._scrollToBounds()
    },
    _scrollToBounds: function() {
        if (this._inBounds()) {
            return
        }
        this._bounceAction();
        this._setupBounce();
        this._bounceAnimator.start()
    },
    _setupBounce: function() {
        var boundLocation = this._bounceLocation = this._boundLocation();
        var bounceDistance = boundLocation - this._location;
        this._velocity = bounceDistance / BOUNCE_ACCELERATION_SUM
    },
    _inBounds: function(location) {
        location = void 0 !== location ? location : this._location;
        return this._boundLocation(location) === location
    },
    _crossBoundOnNextStep: function() {
        var location = this._location;
        var nextLocation = location + this._velocity;
        return location < this._minOffset && nextLocation >= this._minOffset || location > this._maxOffset && nextLocation <= this._maxOffset
    },
    _initHandler: function(e) {
        this._stopScrolling();
        this._prepareThumbScrolling(e)
    },
    _stopScrolling: deferRenderer((function() {
        this._hideScrollbar();
        this._inertiaAnimator.stop();
        this._bounceAnimator.stop()
    })),
    _prepareThumbScrolling: function(e) {
        if (isDxMouseWheelEvent(e.originalEvent)) {
            return
        }
        var $target = $(e.originalEvent.target);
        var scrollbarClicked = this._isScrollbar($target);
        if (scrollbarClicked) {
            this._moveToMouseLocation(e)
        }
        this._thumbScrolling = scrollbarClicked || this._isThumb($target);
        this._crossThumbScrolling = !this._thumbScrolling && this._isAnyThumbScrolling($target);
        if (this._thumbScrolling) {
            this._scrollbar.feedbackOn()
        }
    },
    _isThumbScrollingHandler: function($target) {
        return this._isThumb($target)
    },
    _moveToMouseLocation: function(e) {
        var mouseLocation = e["page" + this._axis.toUpperCase()] - this._$element.offset()[this._prop];
        var location = this._location + mouseLocation / this._containerToContentRatio() - getHeight(this._$container) / 2;
        this._scrollStep(-Math.round(location))
    },
    _startHandler: function() {
        this._showScrollbar()
    },
    _moveHandler: function(delta) {
        if (this._crossThumbScrolling) {
            return
        }
        if (this._thumbScrolling) {
            delta[this._axis] = -Math.round(delta[this._axis] / this._containerToContentRatio())
        }
        this._scrollBy(delta)
    },
    _scrollBy: function(delta) {
        delta = delta[this._axis];
        if (!this._inBounds()) {
            delta *= OUT_BOUNDS_ACCELERATION
        }
        this._scrollStep(delta)
    },
    _scrollByHandler: function(delta) {
        this._scrollBy(delta);
        this._scrollComplete()
    },
    _containerToContentRatio: function() {
        return this._scrollbar.containerToContentRatio()
    },
    _endHandler: function(velocity) {
        this._completeDeferred = new Deferred;
        this._velocity = velocity[this._axis];
        this._inertiaHandler();
        this._resetThumbScrolling();
        return this._completeDeferred.promise()
    },
    _inertiaHandler: function() {
        this._suppressInertia();
        this._inertiaAnimator.start()
    },
    _suppressInertia: function() {
        if (!this._inertiaEnabled || this._thumbScrolling) {
            this._velocity = 0
        }
    },
    _resetThumbScrolling: function() {
        this._thumbScrolling = false;
        this._crossThumbScrolling = false
    },
    _stopHandler: function() {
        if (this._thumbScrolling) {
            this._scrollComplete()
        }
        this._resetThumbScrolling();
        this._scrollToBounds()
    },
    _disposeHandler: function() {
        this._stopScrolling();
        this._$scrollbar.remove()
    },
    _updateHandler: function() {
        this._update();
        this._moveToBounds()
    },
    _update: function() {
        this._stopScrolling();
        return deferUpdate(() => {
            this._resetScaleRatio();
            this._updateLocation();
            this._updateBounds();
            this._updateScrollbar();
            deferRender(() => {
                this._moveScrollbar();
                this._scrollbar.update()
            })
        })
    },
    _resetScaleRatio: function() {
        this._scaleRatio = null
    },
    _updateLocation: function() {
        this._location = (locate(this._$content)[this._prop] - this._$container[this._scrollProp]()) * this._getScaleRatio()
    },
    _updateBounds: function() {
        this._maxOffset = this._getMaxOffset();
        this._minOffset = this._getMinOffset()
    },
    _getMaxOffset: function() {
        return 0
    },
    _getMinOffset: function() {
        this._maxScrollPropValue = Math.max(this._contentSize() - this._containerSize(), 0);
        return -this._maxScrollPropValue
    },
    _updateScrollbar: deferUpdater((function() {
        var containerSize = this._containerSize();
        var contentSize = this._contentSize();
        var baseContainerSize = this._getBaseDimension(this._$container.get(0), this._dimension);
        var baseContentSize = this._getBaseDimension(this._$content.get(0), this._dimension);
        deferRender(() => {
            this._scrollbar.option({
                containerSize: containerSize,
                contentSize: contentSize,
                baseContainerSize: baseContainerSize,
                baseContentSize: baseContentSize,
                scaleRatio: this._getScaleRatio()
            })
        })
    })),
    _moveToBounds: deferRenderer(deferUpdater(deferRenderer((function() {
        var location = this._boundLocation();
        var locationChanged = location !== this._location;
        this._location = location;
        this._move();
        if (locationChanged) {
            this._scrollAction()
        }
    })))),
    _createActionsHandler: function(actions) {
        this._scrollAction = actions.scroll;
        this._bounceAction = actions.bounce
    },
    _showScrollbar: function() {
        this._scrollbar.option("visible", true)
    },
    _hideScrollbar: function() {
        this._scrollbar.option("visible", false)
    },
    _containerSize: function() {
        return this._getRealDimension(this._$container.get(0), this._dimension)
    },
    _contentSize: function() {
        var isOverflowHidden = "hidden" === this._$content.css("overflow" + this._axis.toUpperCase());
        var contentSize = this._getRealDimension(this._$content.get(0), this._dimension);
        if (!isOverflowHidden) {
            var containerScrollSize = this._$content[0]["scroll" + titleize(this._dimension)] * this._getScaleRatio();
            contentSize = Math.max(containerScrollSize, contentSize)
        }
        return contentSize
    },
    _validateEvent: function(e) {
        var $target = $(e.originalEvent.target);
        return this._isThumb($target) || this._isScrollbar($target)
    },
    _isThumb: function($element) {
        return this._scrollByThumb && this._scrollbar.isThumb($element)
    },
    _isScrollbar: function($element) {
        return this._scrollByThumb && $element && $element.is(this._$scrollbar)
    },
    _reachedMin: function() {
        return Math.round(this._location - this._minOffset) <= 0
    },
    _reachedMax: function() {
        return Math.round(this._location - this._maxOffset) >= 0
    },
    _cursorEnterHandler: function() {
        this._resetScaleRatio();
        this._updateScrollbar();
        this._scrollbar.cursorEnter()
    },
    _cursorLeaveHandler: function() {
        this._scrollbar.cursorLeave()
    },
    dispose: noop
});
var hoveredScrollable;
var activeScrollable;
export var SimulatedStrategy = Class.inherit({
    ctor: function(scrollable) {
        this._init(scrollable)
    },
    _init: function(scrollable) {
        this._component = scrollable;
        this._$element = scrollable.$element();
        this._$container = $(scrollable.container());
        this._$wrapper = scrollable._$wrapper;
        this._$content = scrollable.$content();
        this.option = scrollable.option.bind(scrollable);
        this._createActionByOption = scrollable._createActionByOption.bind(scrollable);
        this._isLocked = scrollable._isLocked.bind(scrollable);
        this._isDirection = scrollable._isDirection.bind(scrollable);
        this._allowedDirection = scrollable._allowedDirection.bind(scrollable);
        this._getMaxOffset = scrollable._getMaxOffset.bind(scrollable)
    },
    render: function() {
        this._$element.addClass(SCROLLABLE_SIMULATED_CLASS);
        this._createScrollers();
        if (this.option("useKeyboard")) {
            this._$container.prop("tabIndex", 0)
        }
        this._attachKeyboardHandler();
        this._attachCursorHandlers()
    },
    _createScrollers: function() {
        this._scrollers = {};
        if (this._isDirection(HORIZONTAL)) {
            this._createScroller(HORIZONTAL)
        }
        if (this._isDirection(VERTICAL)) {
            this._createScroller(VERTICAL)
        }
        this._$element.toggleClass(SCROLLABLE_SCROLLBARS_ALWAYSVISIBLE, "always" === this.option("showScrollbar"))
    },
    _createScroller: function(direction) {
        this._scrollers[direction] = new Scroller(this._scrollerOptions(direction))
    },
    _scrollerOptions: function(direction) {
        return {
            direction: direction,
            $content: this._$content,
            $container: this._$container,
            $wrapper: this._$wrapper,
            $element: this._$element,
            scrollByThumb: this.option("scrollByThumb"),
            scrollbarVisible: this.option("showScrollbar"),
            bounceEnabled: this.option("bounceEnabled"),
            inertiaEnabled: this.option("inertiaEnabled"),
            isAnyThumbScrolling: this._isAnyThumbScrolling.bind(this)
        }
    },
    _applyScaleRatio: function(targetLocation) {
        for (var direction in this._scrollers) {
            var prop = this._getPropByDirection(direction);
            if (isDefined(targetLocation[prop])) {
                var scroller = this._scrollers[direction];
                targetLocation[prop] *= scroller._getScaleRatio()
            }
        }
        return targetLocation
    },
    _isAnyThumbScrolling: function($target) {
        var result = false;
        this._eventHandler("isThumbScrolling", $target).done((function(isThumbScrollingVertical, isThumbScrollingHorizontal) {
            result = isThumbScrollingVertical || isThumbScrollingHorizontal
        }));
        return result
    },
    handleInit: function(e) {
        this._suppressDirections(e);
        this._eventForUserAction = e;
        this._eventHandler("init", e)
    },
    _suppressDirections: function(e) {
        if (isDxMouseWheelEvent(e.originalEvent)) {
            this._prepareDirections(true);
            return
        }
        this._prepareDirections();
        this._eachScroller((function(scroller, direction) {
            var $target = $(e.originalEvent.target);
            var isValid = scroller._validateEvent(e) || this.option("scrollByContent") && this._isContent($target);
            this._validDirections[direction] = isValid
        }))
    },
    _isContent: function($element) {
        return !!$element.closest(this._$element).length
    },
    _prepareDirections: function(value) {
        value = value || false;
        this._validDirections = {};
        this._validDirections[HORIZONTAL] = value;
        this._validDirections[VERTICAL] = value
    },
    _eachScroller: function(callback) {
        callback = callback.bind(this);
        each(this._scrollers, (function(direction, scroller) {
            callback(scroller, direction)
        }))
    },
    handleStart: function(e) {
        this._eventForUserAction = e;
        this._eventHandler("start").done(this._startAction)
    },
    _saveActive: function() {
        activeScrollable = this
    },
    _resetActive: function() {
        if (activeScrollable === this) {
            activeScrollable = null
        }
    },
    handleMove: function(e) {
        if (this._isLocked()) {
            e.cancel = true;
            this._resetActive();
            return
        }
        this._saveActive();
        e.preventDefault && e.preventDefault();
        this._adjustDistance(e, e.delta);
        this._eventForUserAction = e;
        this._eventHandler("move", e.delta)
    },
    _adjustDistance: function(e, distance) {
        distance.x *= this._validDirections[HORIZONTAL];
        distance.y *= this._validDirections[VERTICAL];
        var devicePixelRatio = this._tryGetDevicePixelRatio();
        if (devicePixelRatio && isDxMouseWheelEvent(e.originalEvent)) {
            distance.x = Math.round(distance.x / devicePixelRatio * 100) / 100;
            distance.y = Math.round(distance.y / devicePixelRatio * 100) / 100
        }
    },
    _tryGetDevicePixelRatio: function() {
        if (hasWindow()) {
            return getWindow().devicePixelRatio
        }
    },
    handleEnd: function(e) {
        this._resetActive();
        this._refreshCursorState(e.originalEvent && e.originalEvent.target);
        this._adjustDistance(e, e.velocity);
        this._eventForUserAction = e;
        return this._eventHandler("end", e.velocity).done(this._endAction)
    },
    handleCancel: function(e) {
        this._resetActive();
        this._eventForUserAction = e;
        return this._eventHandler("end", {
            x: 0,
            y: 0
        })
    },
    handleStop: function() {
        this._resetActive();
        this._eventHandler("stop")
    },
    handleScroll: function() {
        this._updateRtlConfig();
        this._scrollAction()
    },
    _attachKeyboardHandler: function() {
        eventsEngine.off(this._$element, ".".concat(SCROLLABLE_SIMULATED_KEYBOARD));
        if (!this.option("disabled") && this.option("useKeyboard")) {
            eventsEngine.on(this._$element, addEventNamespace("keydown", SCROLLABLE_SIMULATED_KEYBOARD), this._keyDownHandler.bind(this))
        }
    },
    _keyDownHandler: function(e) {
        clearTimeout(this._updateHandlerTimeout);
        this._updateHandlerTimeout = setTimeout(() => {
            if (normalizeKeyName(e) === KEY_CODES.TAB) {
                this._eachScroller(scroller => {
                    scroller._updateHandler()
                })
            }
        });
        if (!this._$container.is(domAdapter.getActiveElement())) {
            return
        }
        var handled = true;
        switch (normalizeKeyName(e)) {
            case KEY_CODES.DOWN:
                this._scrollByLine({
                    y: 1
                });
                break;
            case KEY_CODES.UP:
                this._scrollByLine({
                    y: -1
                });
                break;
            case KEY_CODES.RIGHT:
                this._scrollByLine({
                    x: 1
                });
                break;
            case KEY_CODES.LEFT:
                this._scrollByLine({
                    x: -1
                });
                break;
            case KEY_CODES.PAGE_DOWN:
                this._scrollByPage(1);
                break;
            case KEY_CODES.PAGE_UP:
                this._scrollByPage(-1);
                break;
            case KEY_CODES.HOME:
                this._scrollToHome();
                break;
            case KEY_CODES.END:
                this._scrollToEnd();
                break;
            default:
                handled = false
        }
        if (handled) {
            e.stopPropagation();
            e.preventDefault()
        }
    },
    _scrollByLine: function(lines) {
        var devicePixelRatio = this._tryGetDevicePixelRatio();
        var scrollOffset = SCROLL_LINE_HEIGHT;
        if (devicePixelRatio) {
            scrollOffset = Math.abs(scrollOffset / devicePixelRatio * 100) / 100
        }
        this.scrollBy({
            top: (lines.y || 0) * -scrollOffset,
            left: (lines.x || 0) * -scrollOffset
        })
    },
    _scrollByPage: function(page) {
        var prop = this._wheelProp();
        var dimension = this._dimensionByProp(prop);
        var distance = {};
        var getter = "width" === dimension ? getWidth : getHeight;
        distance[prop] = page * -getter(this._$container);
        this.scrollBy(distance)
    },
    _dimensionByProp: function(prop) {
        return "left" === prop ? "width" : "height"
    },
    _getPropByDirection: function(direction) {
        return direction === HORIZONTAL ? "left" : "top"
    },
    _scrollToHome: function() {
        var prop = this._wheelProp();
        var distance = {};
        distance[prop] = 0;
        this._component.scrollTo(distance)
    },
    _scrollToEnd: function() {
        var prop = this._wheelProp();
        var dimension = this._dimensionByProp(prop);
        var distance = {};
        var getter = "width" === dimension ? getWidth : getHeight;
        distance[prop] = getter(this._$content) - getter(this._$container);
        this._component.scrollTo(distance)
    },
    createActions: function() {
        this._startAction = this._createActionHandler("onStart");
        this._endAction = this._createActionHandler("onEnd");
        this._updateAction = this._createActionHandler("onUpdated");
        this._createScrollerActions()
    },
    _createScrollerActions: function() {
        this._scrollAction = this._createActionHandler("onScroll");
        this._bounceAction = this._createActionHandler("onBounce");
        this._eventHandler("createActions", {
            scroll: this._scrollAction,
            bounce: this._bounceAction
        })
    },
    _createActionHandler: function(optionName) {
        var actionHandler = this._createActionByOption(optionName);
        return () => {
            actionHandler(extend(this._createActionArgs(), arguments))
        }
    },
    _createActionArgs: function() {
        var {
            horizontal: scrollerX,
            vertical: scrollerY
        } = this._scrollers;
        var offset = this._getScrollOffset();
        this._scrollOffset = {
            top: scrollerY && offset.top,
            left: scrollerX && offset.left
        };
        return {
            event: this._eventForUserAction,
            scrollOffset: this._scrollOffset,
            reachedLeft: scrollerX && scrollerX._reachedMax(),
            reachedRight: scrollerX && scrollerX._reachedMin(),
            reachedTop: scrollerY && scrollerY._reachedMax(),
            reachedBottom: scrollerY && scrollerY._reachedMin()
        }
    },
    _getScrollOffset() {
        return {
            top: -this.location().top,
            left: -this.location().left
        }
    },
    _eventHandler: function(eventName) {
        var args = [].slice.call(arguments).slice(1);
        var deferreds = map(this._scrollers, scroller => scroller["_" + eventName + "Handler"].apply(scroller, args));
        return when.apply($, deferreds).promise()
    },
    location: function() {
        var location = locate(this._$content);
        location.top -= this._$container.scrollTop();
        location.left -= this._$container.scrollLeft();
        return location
    },
    disabledChanged: function() {
        this._attachCursorHandlers()
    },
    _attachCursorHandlers: function() {
        eventsEngine.off(this._$element, ".".concat(SCROLLABLE_SIMULATED_CURSOR));
        if (!this.option("disabled") && this._isHoverMode()) {
            eventsEngine.on(this._$element, addEventNamespace("mouseenter", SCROLLABLE_SIMULATED_CURSOR), this._cursorEnterHandler.bind(this));
            eventsEngine.on(this._$element, addEventNamespace("mouseleave", SCROLLABLE_SIMULATED_CURSOR), this._cursorLeaveHandler.bind(this))
        }
    },
    _isHoverMode: function() {
        return "onHover" === this.option("showScrollbar")
    },
    _cursorEnterHandler: function(e) {
        e = e || {};
        e.originalEvent = e.originalEvent || {};
        if (activeScrollable || e.originalEvent._hoverHandled) {
            return
        }
        if (hoveredScrollable) {
            hoveredScrollable._cursorLeaveHandler()
        }
        hoveredScrollable = this;
        this._eventHandler("cursorEnter");
        e.originalEvent._hoverHandled = true
    },
    _cursorLeaveHandler: function(e) {
        if (hoveredScrollable !== this || activeScrollable === hoveredScrollable) {
            return
        }
        this._eventHandler("cursorLeave");
        hoveredScrollable = null;
        this._refreshCursorState(e && e.relatedTarget)
    },
    _refreshCursorState: function(target) {
        if (!this._isHoverMode() && (!target || activeScrollable)) {
            return
        }
        var $target = $(target);
        var $scrollable = $target.closest(".".concat(SCROLLABLE_SIMULATED_CLASS, ":not(.dx-state-disabled)"));
        var targetScrollable = $scrollable.length && $scrollable.data(SCROLLABLE_STRATEGY);
        if (hoveredScrollable && hoveredScrollable !== targetScrollable) {
            hoveredScrollable._cursorLeaveHandler()
        }
        if (targetScrollable) {
            targetScrollable._cursorEnterHandler()
        }
    },
    update: function() {
        var result = this._eventHandler("update").done(this._updateAction);
        return when(result, deferUpdate(() => {
            var allowedDirections = this._allowedDirections();
            deferRender(() => {
                var touchDirection = allowedDirections.vertical ? "pan-x" : "";
                touchDirection = allowedDirections.horizontal ? "pan-y" : touchDirection;
                touchDirection = allowedDirections.vertical && allowedDirections.horizontal ? "none" : touchDirection;
                this._$container.css("touchAction", touchDirection)
            });
            return when().promise()
        }))
    },
    _allowedDirections: function() {
        var bounceEnabled = this.option("bounceEnabled");
        var verticalScroller = this._scrollers[VERTICAL];
        var horizontalScroller = this._scrollers[HORIZONTAL];
        return {
            vertical: verticalScroller && (verticalScroller._minOffset < 0 || bounceEnabled),
            horizontal: horizontalScroller && (horizontalScroller._minOffset < 0 || bounceEnabled)
        }
    },
    _updateBounds: function() {
        this._scrollers[HORIZONTAL] && this._scrollers[HORIZONTAL]._updateBounds()
    },
    _isHorizontalAndRtlEnabled: function() {
        return this.option("rtlEnabled") && this.option("direction") !== VERTICAL
    },
    updateRtlPosition: function(needInitializeRtlConfig) {
        if (needInitializeRtlConfig) {
            this._rtlConfig = {
                scrollRight: 0,
                clientWidth: this._$container.get(0).clientWidth,
                windowPixelRatio: this._getWindowDevicePixelRatio()
            }
        }
        this._updateBounds();
        if (this._isHorizontalAndRtlEnabled()) {
            deferUpdate(() => {
                var scrollLeft = this._getMaxOffset().left - this._rtlConfig.scrollRight;
                if (scrollLeft <= 0) {
                    scrollLeft = 0;
                    this._rtlConfig.scrollRight = this._getMaxOffset().left
                }
                deferRender(() => {
                    if (this._getScrollOffset().left !== scrollLeft) {
                        this._rtlConfig.skipUpdating = true;
                        this._component.scrollTo({
                            left: scrollLeft
                        });
                        this._rtlConfig.skipUpdating = false
                    }
                })
            })
        }
    },
    _updateRtlConfig: function() {
        if (this._isHorizontalAndRtlEnabled() && !this._rtlConfig.skipUpdating) {
            var {
                clientWidth: clientWidth,
                scrollLeft: scrollLeft
            } = this._$container.get(0);
            var windowPixelRatio = this._getWindowDevicePixelRatio();
            if (this._rtlConfig.windowPixelRatio === windowPixelRatio && this._rtlConfig.clientWidth === clientWidth) {
                this._rtlConfig.scrollRight = this._getMaxOffset().left - scrollLeft
            }
            this._rtlConfig.clientWidth = clientWidth;
            this._rtlConfig.windowPixelRatio = windowPixelRatio
        }
    },
    _getWindowDevicePixelRatio: function() {
        return hasWindow() ? getWindow().devicePixelRatio : 1
    },
    scrollBy: function(distance) {
        var verticalScroller = this._scrollers[VERTICAL];
        var horizontalScroller = this._scrollers[HORIZONTAL];
        if (verticalScroller) {
            distance.top = verticalScroller._boundLocation(distance.top + verticalScroller._location) - verticalScroller._location
        }
        if (horizontalScroller) {
            distance.left = horizontalScroller._boundLocation(distance.left + horizontalScroller._location) - horizontalScroller._location
        }
        this._prepareDirections(true);
        this._startAction();
        this._eventHandler("scrollBy", {
            x: distance.left,
            y: distance.top
        });
        this._endAction();
        this._updateRtlConfig()
    },
    validate: function(e) {
        if (isDxMouseWheelEvent(e) && isCommandKeyPressed(e)) {
            return false
        }
        if (this.option("disabled")) {
            return false
        }
        if (this.option("bounceEnabled")) {
            return true
        }
        return isDxMouseWheelEvent(e) ? this._validateWheel(e) : this._validateMove(e)
    },
    _validateWheel: function(e) {
        var scroller = this._scrollers[this._wheelDirection(e)];
        var reachedMin = scroller._reachedMin();
        var reachedMax = scroller._reachedMax();
        var contentGreaterThanContainer = !reachedMin || !reachedMax;
        var locatedNotAtBound = !reachedMin && !reachedMax;
        var scrollFromMin = reachedMin && e.delta > 0;
        var scrollFromMax = reachedMax && e.delta < 0;
        var validated = contentGreaterThanContainer && (locatedNotAtBound || scrollFromMin || scrollFromMax);
        validated = validated || void 0 !== this._validateWheelTimer;
        if (validated) {
            clearTimeout(this._validateWheelTimer);
            this._validateWheelTimer = setTimeout(() => {
                this._validateWheelTimer = void 0
            }, VALIDATE_WHEEL_TIMEOUT)
        }
        return validated
    },
    _validateMove: function(e) {
        if (!this.option("scrollByContent") && !$(e.target).closest(".".concat(SCROLLABLE_SCROLLBAR_CLASS)).length) {
            return false
        }
        return this._allowedDirection()
    },
    getDirection: function(e) {
        return isDxMouseWheelEvent(e) ? this._wheelDirection(e) : this._allowedDirection()
    },
    _wheelProp: function() {
        return this._wheelDirection() === HORIZONTAL ? "left" : "top"
    },
    _wheelDirection: function(e) {
        switch (this.option("direction")) {
            case HORIZONTAL:
                return HORIZONTAL;
            case VERTICAL:
                return VERTICAL;
            default:
                return e && e.shiftKey ? HORIZONTAL : VERTICAL
        }
    },
    dispose: function() {
        this._resetActive();
        if (hoveredScrollable === this) {
            hoveredScrollable = null
        }
        this._eventHandler("dispose");
        this._detachEventHandlers();
        this._$element.removeClass(SCROLLABLE_SIMULATED_CLASS);
        this._eventForUserAction = null;
        clearTimeout(this._validateWheelTimer);
        clearTimeout(this._updateHandlerTimeout)
    },
    _detachEventHandlers: function() {
        eventsEngine.off(this._$element, ".".concat(SCROLLABLE_SIMULATED_CURSOR));
        eventsEngine.off(this._$container, ".".concat(SCROLLABLE_SIMULATED_KEYBOARD))
    }
});
