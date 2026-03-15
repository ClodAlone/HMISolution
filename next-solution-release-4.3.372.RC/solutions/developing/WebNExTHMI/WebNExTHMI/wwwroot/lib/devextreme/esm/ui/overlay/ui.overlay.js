/**
 * DevExtreme (esm/ui/overlay/ui.overlay.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getOuterHeight,
    getWidth,
    getHeight
} from "../../core/utils/size";
import fx from "../../animation/fx";
import registerComponent from "../../core/component_registrator";
import devices from "../../core/devices";
import domAdapter from "../../core/dom_adapter";
import {
    getPublicElement
} from "../../core/element";
import $ from "../../core/renderer";
import {
    EmptyTemplate
} from "../../core/templates/empty_template";
import {
    inArray
} from "../../core/utils/array";
import {
    noop
} from "../../core/utils/common";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    contains,
    resetActiveElement
} from "../../core/utils/dom";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import readyCallbacks from "../../core/utils/ready_callbacks";
import {
    isDefined,
    isFunction,
    isPlainObject,
    isObject
} from "../../core/utils/type";
import {
    changeCallback
} from "../../core/utils/view_port";
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
import errors from "../../core/errors";
import eventsEngine from "../../events/core/events_engine";
import {
    move as dragEventMove
} from "../../events/drag";
import pointerEvents from "../../events/pointer";
import {
    keyboard
} from "../../events/short";
import {
    addNamespace,
    isCommandKeyPressed,
    normalizeKeyName
} from "../../events/utils/index";
import {
    triggerHidingEvent,
    triggerResizeEvent,
    triggerShownEvent
} from "../../events/visibility_change";
import {
    hideCallback as hideTopOverlayCallback
} from "../../mobile/hide_callback";
import Resizable from "../resizable";
import OverlayDrag from "./overlay_drag";
import {
    tabbable
} from "../widget/selectors";
import swatch from "../widget/swatch_container";
import Widget from "../widget/ui.widget";
import browser from "../../core/utils/browser";
import * as zIndexPool from "./z_index";
import resizeObserverSingleton from "../../core/resize_observer";
import {
    OverlayPositionController,
    OVERLAY_POSITION_ALIASES
} from "./overlay_position_controller";
var ready = readyCallbacks.add;
var window = getWindow();
var viewPortChanged = changeCallback;
var OVERLAY_CLASS = "dx-overlay";
var OVERLAY_WRAPPER_CLASS = "dx-overlay-wrapper";
var OVERLAY_CONTENT_CLASS = "dx-overlay-content";
var OVERLAY_SHADER_CLASS = "dx-overlay-shader";
var OVERLAY_MODAL_CLASS = "dx-overlay-modal";
var INNER_OVERLAY_CLASS = "dx-inner-overlay";
var INVISIBLE_STATE_CLASS = "dx-state-invisible";
var ANONYMOUS_TEMPLATE_NAME = "content";
var RTL_DIRECTION_CLASS = "dx-rtl";
var ACTIONS = ["onShowing", "onShown", "onHiding", "onHidden", "onPositioned", "onResizeStart", "onResize", "onResizeEnd", "onVisualPositionChanged"];
var OVERLAY_STACK = [];
var PREVENT_SAFARI_SCROLLING_CLASS = "dx-prevent-safari-scrolling";
var TAB_KEY = "tab";
ready(() => {
    eventsEngine.subscribeGlobal(domAdapter.getDocument(), pointerEvents.down, e => {
        for (var i = OVERLAY_STACK.length - 1; i >= 0; i--) {
            if (!OVERLAY_STACK[i]._proxiedDocumentDownHandler(e)) {
                return
            }
        }
    })
});
var Overlay = Widget.inherit({
    _supportedKeys: function() {
        return extend(this.callBase(), {
            escape: function() {
                this.hide()
            },
            upArrow: e => {
                this._drag.moveUp(e)
            },
            downArrow: e => {
                this._drag.moveDown(e)
            },
            leftArrow: e => {
                this._drag.moveLeft(e)
            },
            rightArrow: e => {
                this._drag.moveRight(e)
            }
        })
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            activeStateEnabled: false,
            visible: false,
            deferRendering: true,
            shading: true,
            shadingColor: "",
            wrapperAttr: {},
            position: extend({}, OVERLAY_POSITION_ALIASES.center),
            width: "80vw",
            minWidth: null,
            maxWidth: null,
            height: "80vh",
            minHeight: null,
            maxHeight: null,
            animation: {
                show: {
                    type: "pop",
                    duration: 300,
                    from: {
                        scale: .55
                    }
                },
                hide: {
                    type: "pop",
                    duration: 300,
                    to: {
                        opacity: 0,
                        scale: .55
                    },
                    from: {
                        opacity: 1,
                        scale: 1
                    }
                }
            },
            dragOutsideBoundary: false,
            closeOnOutsideClick: false,
            copyRootClassesToWrapper: false,
            _ignoreCopyRootClassesToWrapperDeprecation: false,
            onShowing: null,
            onShown: null,
            onHiding: null,
            onHidden: null,
            contentTemplate: "content",
            dragEnabled: false,
            dragAndResizeArea: void 0,
            outsideDragFactor: 0,
            resizeEnabled: false,
            onResizeStart: null,
            onResize: null,
            onResizeEnd: null,
            innerOverlay: false,
            restorePosition: true,
            target: void 0,
            container: void 0,
            hideTopOverlayHandler: () => {
                this.hide()
            },
            hideOnParentScroll: false,
            onPositioned: null,
            propagateOutsideClick: false,
            ignoreChildEvents: true,
            _checkParentVisibility: true,
            _fixWrapperPosition: false
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return !hasWindow()
            },
            options: {
                width: null,
                height: null,
                animation: null,
                _checkParentVisibility: false
            }
        }])
    },
    _setOptionsByReference: function() {
        this.callBase();
        extend(this._optionsByReference, {
            animation: true
        })
    },
    $wrapper: function() {
        return this._$wrapper
    },
    _eventBindingTarget: function() {
        return this._$content
    },
    _setDeprecatedOptions() {
        this.callBase();
        extend(this._deprecatedOptions, {
            elementAttr: {
                since: "21.2",
                message: 'Use the "wrapperAttr" option instead'
            }
        })
    },
    ctor: function(element, options) {
        this.callBase(element, options);
        if (options && options.copyRootClassesToWrapper && !options._ignoreCopyRootClassesToWrapperDeprecation) {
            errors.log("W0001", this.NAME, "copyRootClassesToWrapper", "21.2", 'Use the "wrapperAttr" option instead')
        }
    },
    _init: function() {
        this.callBase();
        this._initActions();
        this._initCloseOnOutsideClickHandler();
        this._initTabTerminatorHandler();
        this._customWrapperClass = null;
        this._$wrapper = $("<div>").addClass(OVERLAY_WRAPPER_CLASS);
        this._$content = $("<div>").addClass(OVERLAY_CONTENT_CLASS);
        this._initInnerOverlayClass();
        var $element = this.$element();
        if (this.option("copyRootClassesToWrapper")) {
            this._$wrapper.addClass($element.attr("class"))
        }
        $element.addClass(OVERLAY_CLASS);
        this._$wrapper.attr("data-bind", "dxControlsDescendantBindings: true");
        this._toggleViewPortSubscription(true);
        this._initHideTopOverlayHandler(this.option("hideTopOverlayHandler"));
        this._parentsScrollSubscriptionInfo = {
            handler: e => {
                this._targetParentsScrollHandler(e)
            }
        };
        this._updateResizeCallbackSkipCondition();
        this.warnPositionAsFunction()
    },
    warnPositionAsFunction() {
        if (isFunction(this.option("position"))) {
            errors.log("W0018")
        }
    },
    _initOptions: function(options) {
        this._setAnimationTarget(options.target);
        this.callBase(options)
    },
    _initInnerOverlayClass: function() {
        this._$content.toggleClass(INNER_OVERLAY_CLASS, this.option("innerOverlay"))
    },
    _setAnimationTarget: function(target) {
        if (!isDefined(target)) {
            return
        }
        var options = this.option();
        ["animation.show.from.position.of", "animation.show.to.position.of", "animation.hide.from.position.of", "animation.hide.to.position.of"].forEach(path => {
            var pathParts = path.split(".");
            var option = options;
            while (option) {
                if (1 === pathParts.length) {
                    if (isPlainObject(option)) {
                        option[pathParts.shift()] = target
                    }
                    break
                } else {
                    option = option[pathParts.shift()]
                }
            }
        })
    },
    _initHideTopOverlayHandler: function(handler) {
        this._hideTopOverlayHandler = handler
    },
    _initActions: function() {
        this._actions = {};
        each(ACTIONS, (_, action) => {
            this._actions[action] = this._createActionByOption(action, {
                excludeValidators: ["disabled", "readOnly"]
            }) || noop
        })
    },
    _initCloseOnOutsideClickHandler: function() {
        var _this = this;
        this._proxiedDocumentDownHandler = function() {
            return _this._documentDownHandler(...arguments)
        }
    },
    _areContentDimensionsRendered: function(entry) {
        var _entry$contentBoxSize, _this$_renderedDimens3, _this$_renderedDimens4;
        var contentBox = null === (_entry$contentBoxSize = entry.contentBoxSize) || void 0 === _entry$contentBoxSize ? void 0 : _entry$contentBoxSize[0];
        if (contentBox) {
            var _this$_renderedDimens, _this$_renderedDimens2;
            return parseInt(contentBox.inlineSize, 10) === (null === (_this$_renderedDimens = this._renderedDimensions) || void 0 === _this$_renderedDimens ? void 0 : _this$_renderedDimens.width) && parseInt(contentBox.blockSize, 10) === (null === (_this$_renderedDimens2 = this._renderedDimensions) || void 0 === _this$_renderedDimens2 ? void 0 : _this$_renderedDimens2.height)
        }
        var contentRect = entry.contentRect;
        return parseInt(contentRect.width, 10) === (null === (_this$_renderedDimens3 = this._renderedDimensions) || void 0 === _this$_renderedDimens3 ? void 0 : _this$_renderedDimens3.width) && parseInt(contentRect.height, 10) === (null === (_this$_renderedDimens4 = this._renderedDimensions) || void 0 === _this$_renderedDimens4 ? void 0 : _this$_renderedDimens4.height)
    },
    _contentResizeHandler: function(entry) {
        if (!this._shouldSkipContentResize(entry)) {
            this._renderGeometry({
                shouldOnlyReposition: true
            })
        }
    },
    _updateResizeCallbackSkipCondition() {
        var doesShowAnimationChangeDimensions = this._doesShowAnimationChangeDimensions();
        this._shouldSkipContentResize = entry => doesShowAnimationChangeDimensions && this._showAnimationProcessing || this._areContentDimensionsRendered(entry)
    },
    _doesShowAnimationChangeDimensions: function() {
        var animation = this.option("animation");
        return ["to", "from"].some(prop => {
            var _animation$show;
            var config = null === animation || void 0 === animation ? void 0 : null === (_animation$show = animation.show) || void 0 === _animation$show ? void 0 : _animation$show[prop];
            return isObject(config) && ("width" in config || "height" in config)
        })
    },
    _observeContentResize: function(shouldObserve) {
        if (!this.option("useResizeObserver")) {
            return
        }
        var contentElement = this._$content.get(0);
        if (shouldObserve) {
            resizeObserverSingleton.observe(contentElement, entry => {
                this._contentResizeHandler(entry)
            })
        } else {
            resizeObserverSingleton.unobserve(contentElement)
        }
    },
    _initMarkup() {
        this.callBase();
        this._renderWrapperAttributes();
        this._initPositionController()
    },
    _documentDownHandler: function(e) {
        if (this._showAnimationProcessing) {
            this._stopAnimation()
        }
        var closeOnOutsideClick = this.option("closeOnOutsideClick");
        if (isFunction(closeOnOutsideClick)) {
            closeOnOutsideClick = closeOnOutsideClick(e)
        }
        var isAttachedTarget = $(window.document).is(e.target) || contains(window.document, e.target);
        var isInnerOverlay = $(e.target).closest(".".concat(INNER_OVERLAY_CLASS)).length;
        var outsideClick = isAttachedTarget && !isInnerOverlay && !(this._$content.is(e.target) || contains(this._$content.get(0), e.target));
        if (outsideClick && closeOnOutsideClick) {
            this._outsideClickHandler(e)
        }
        return this.option("propagateOutsideClick")
    },
    _outsideClickHandler(e) {
        if (this.option("shading")) {
            e.preventDefault()
        }
        this.hide()
    },
    _getAnonymousTemplateName: function() {
        return ANONYMOUS_TEMPLATE_NAME
    },
    _initTemplates: function() {
        this._templateManager.addDefaultTemplates({
            content: new EmptyTemplate
        });
        this.callBase()
    },
    _isTopOverlay: function() {
        var overlayStack = this._overlayStack();
        for (var i = overlayStack.length - 1; i >= 0; i--) {
            var tabbableElements = overlayStack[i]._findTabbableBounds();
            if (tabbableElements.first || tabbableElements.last) {
                return overlayStack[i] === this
            }
        }
        return false
    },
    _overlayStack: function() {
        return OVERLAY_STACK
    },
    _zIndexInitValue: function() {
        return Overlay.baseZIndex()
    },
    _toggleViewPortSubscription: function(toggle) {
        var _this2 = this;
        viewPortChanged.remove(this._viewPortChangeHandle);
        if (toggle) {
            this._viewPortChangeHandle = function() {
                _this2._viewPortChangeHandler(...arguments)
            };
            viewPortChanged.add(this._viewPortChangeHandle)
        }
    },
    _viewPortChangeHandler: function() {
        this._positionController.updateContainer(this.option("container"));
        this._refresh()
    },
    _renderWrapperAttributes() {
        var {
            wrapperAttr: wrapperAttr
        } = this.option();
        var attributes = extend({}, wrapperAttr);
        var classNames = attributes.class;
        delete attributes.class;
        this.$wrapper().attr(attributes).removeClass(this._customWrapperClass).addClass(classNames);
        this._customWrapperClass = classNames
    },
    _renderVisibilityAnimate: function(visible) {
        this._observeContentResize(visible);
        this._stopAnimation();
        return visible ? this._show() : this._hide()
    },
    _getAnimationConfig: function() {
        return this._getOptionValue("animation", this)
    },
    _animateShowing: function() {
        var _this$_getAnimationCo, _showAnimation$start, _showAnimation$comple, _this3 = this;
        var animation = null !== (_this$_getAnimationCo = this._getAnimationConfig()) && void 0 !== _this$_getAnimationCo ? _this$_getAnimationCo : {};
        var showAnimation = this._normalizeAnimation(animation.show, "to");
        var startShowAnimation = null !== (_showAnimation$start = null === showAnimation || void 0 === showAnimation ? void 0 : showAnimation.start) && void 0 !== _showAnimation$start ? _showAnimation$start : noop;
        var completeShowAnimation = null !== (_showAnimation$comple = null === showAnimation || void 0 === showAnimation ? void 0 : showAnimation.complete) && void 0 !== _showAnimation$comple ? _showAnimation$comple : noop;
        this._animate(showAnimation, (function() {
            if (_this3._isAnimationPaused) {
                return
            }
            if (_this3.option("focusStateEnabled")) {
                eventsEngine.trigger(_this3._focusTarget(), "focus")
            }
            for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
                args[_key] = arguments[_key]
            }
            completeShowAnimation.call(_this3, ...args);
            _this3._showAnimationProcessing = false;
            _this3._isHidden = false;
            _this3._actions.onShown();
            _this3._toggleSafariScrolling();
            _this3._showingDeferred.resolve()
        }), (function() {
            if (_this3._isAnimationPaused) {
                return
            }
            for (var _len2 = arguments.length, args = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
                args[_key2] = arguments[_key2]
            }
            startShowAnimation.call(_this3, ...args);
            _this3._showAnimationProcessing = true
        }))
    },
    _show: function() {
        this._showingDeferred = new Deferred;
        this._parentHidden = this._isParentHidden();
        this._showingDeferred.done(() => {
            delete this._parentHidden
        });
        if (this._parentHidden) {
            this._isHidden = true;
            return this._showingDeferred.resolve()
        }
        if (this._currentVisible) {
            return (new Deferred).resolve().promise()
        }
        this._currentVisible = true;
        if (this._isHidingActionCanceled) {
            delete this._isHidingActionCanceled;
            this._showingDeferred.resolve()
        } else {
            var show = () => {
                this._renderVisibility(true);
                if (this._isShowingActionCanceled) {
                    delete this._isShowingActionCanceled;
                    this._showingDeferred.resolve();
                    return
                }
                this._animateShowing()
            };
            if (this.option("templatesRenderAsynchronously")) {
                this._stopShowTimer();
                this._asyncShowTimeout = setTimeout(show)
            } else {
                show()
            }
        }
        return this._showingDeferred.promise()
    },
    _normalizeAnimation: function(animation, prop) {
        if (animation) {
            animation = extend({
                type: "slide",
                skipElementInitialStyles: true
            }, animation);
            if (animation[prop] && "object" === typeof animation[prop]) {
                extend(animation[prop], {
                    position: this._positionController._position
                })
            }
        }
        return animation
    },
    _animateHiding: function() {
        var _this$_getAnimationCo2, _hideAnimation$start, _hideAnimation$comple, _this4 = this;
        var animation = null !== (_this$_getAnimationCo2 = this._getAnimationConfig()) && void 0 !== _this$_getAnimationCo2 ? _this$_getAnimationCo2 : {};
        var hideAnimation = this._normalizeAnimation(animation.hide, "from");
        var startHideAnimation = null !== (_hideAnimation$start = null === hideAnimation || void 0 === hideAnimation ? void 0 : hideAnimation.start) && void 0 !== _hideAnimation$start ? _hideAnimation$start : noop;
        var completeHideAnimation = null !== (_hideAnimation$comple = null === hideAnimation || void 0 === hideAnimation ? void 0 : hideAnimation.complete) && void 0 !== _hideAnimation$comple ? _hideAnimation$comple : noop;
        this._animate(hideAnimation, (function() {
            var _this4$_actions;
            _this4._$content.css("pointerEvents", "");
            _this4._renderVisibility(false);
            for (var _len3 = arguments.length, args = new Array(_len3), _key3 = 0; _key3 < _len3; _key3++) {
                args[_key3] = arguments[_key3]
            }
            completeHideAnimation.call(_this4, ...args);
            _this4._hideAnimationProcessing = false;
            null === (_this4$_actions = _this4._actions) || void 0 === _this4$_actions ? void 0 : _this4$_actions.onHidden();
            _this4._hidingDeferred.resolve()
        }), (function() {
            _this4._$content.css("pointerEvents", "none");
            for (var _len4 = arguments.length, args = new Array(_len4), _key4 = 0; _key4 < _len4; _key4++) {
                args[_key4] = arguments[_key4]
            }
            startHideAnimation.call(_this4, ...args);
            _this4._hideAnimationProcessing = true
        }))
    },
    _hide: function() {
        if (!this._currentVisible) {
            return (new Deferred).resolve().promise()
        }
        this._currentVisible = false;
        this._hidingDeferred = new Deferred;
        var hidingArgs = {
            cancel: false
        };
        if (this._isShowingActionCanceled) {
            this._hidingDeferred.resolve()
        } else {
            this._actions.onHiding(hidingArgs);
            this._toggleSafariScrolling();
            if (hidingArgs.cancel) {
                this._isHidingActionCanceled = true;
                this.option("visible", true);
                this._hidingDeferred.resolve()
            } else {
                this._forceFocusLost();
                this._toggleShading(false);
                this._toggleSubscriptions(false);
                this._stopShowTimer();
                this._animateHiding()
            }
        }
        return this._hidingDeferred.promise()
    },
    _forceFocusLost: function() {
        var activeElement = domAdapter.getActiveElement();
        var shouldResetActiveElement = !!this._$content.find(activeElement).length;
        if (shouldResetActiveElement) {
            resetActiveElement()
        }
    },
    _animate: function(animation, completeCallback, startCallback) {
        if (animation) {
            startCallback = startCallback || animation.start || noop;
            fx.animate(this._$content, extend({}, animation, {
                start: startCallback,
                complete: completeCallback
            }))
        } else {
            completeCallback()
        }
    },
    _stopAnimation: function() {
        fx.stop(this._$content, true)
    },
    _renderVisibility: function(visible) {
        if (visible && this._isParentHidden()) {
            return
        }
        this._currentVisible = visible;
        this._stopAnimation();
        if (!visible) {
            triggerHidingEvent(this._$content)
        }
        this._toggleVisibility(visible);
        this._$content.toggleClass(INVISIBLE_STATE_CLASS, !visible);
        this._updateZIndexStackPosition(visible);
        if (visible) {
            this._positionController.openingHandled();
            this._renderContent();
            var showingArgs = {
                cancel: false
            };
            this._actions.onShowing(showingArgs);
            if (showingArgs.cancel) {
                this._toggleVisibility(false);
                this._$content.toggleClass(INVISIBLE_STATE_CLASS, true);
                this._updateZIndexStackPosition(false);
                this._moveFromContainer();
                this._isShowingActionCanceled = true;
                this.option("visible", false);
                return
            }
            this._moveToContainer();
            this._renderGeometry();
            triggerShownEvent(this._$content);
            triggerResizeEvent(this._$content)
        } else {
            this._moveFromContainer()
        }
        this._toggleShading(visible);
        this._toggleSubscriptions(visible)
    },
    _updateZIndexStackPosition: function(pushToStack) {
        var overlayStack = this._overlayStack();
        var index = inArray(this, overlayStack);
        if (pushToStack) {
            if (-1 === index) {
                this._zIndex = zIndexPool.create(this._zIndexInitValue());
                overlayStack.push(this)
            }
            this._$wrapper.css("zIndex", this._zIndex);
            this._$content.css("zIndex", this._zIndex)
        } else if (-1 !== index) {
            overlayStack.splice(index, 1);
            zIndexPool.remove(this._zIndex)
        }
    },
    _toggleShading: function(visible) {
        this._$wrapper.toggleClass(OVERLAY_MODAL_CLASS, this.option("shading") && !this.option("container"));
        this._$wrapper.toggleClass(OVERLAY_SHADER_CLASS, visible && this.option("shading"));
        this._$wrapper.css("backgroundColor", this.option("shading") ? this.option("shadingColor") : "");
        this._toggleTabTerminator(visible && this.option("shading"))
    },
    _initTabTerminatorHandler: function() {
        var _this5 = this;
        this._proxiedTabTerminatorHandler = function() {
            _this5._tabKeyHandler(...arguments)
        }
    },
    _toggleTabTerminator: function(enabled) {
        var eventName = addNamespace("keydown", this.NAME);
        if (enabled) {
            eventsEngine.on(domAdapter.getDocument(), eventName, this._proxiedTabTerminatorHandler)
        } else {
            eventsEngine.off(domAdapter.getDocument(), eventName, this._proxiedTabTerminatorHandler)
        }
    },
    _findTabbableBounds: function() {
        var $elements = this._$wrapper.find("*");
        var elementsCount = $elements.length - 1;
        var result = {
            first: null,
            last: null
        };
        for (var i = 0; i <= elementsCount; i++) {
            if (!result.first && $elements.eq(i).is(tabbable)) {
                result.first = $elements.eq(i)
            }
            if (!result.last && $elements.eq(elementsCount - i).is(tabbable)) {
                result.last = $elements.eq(elementsCount - i)
            }
            if (result.first && result.last) {
                break
            }
        }
        return result
    },
    _tabKeyHandler: function(e) {
        if (normalizeKeyName(e) !== TAB_KEY || !this._isTopOverlay()) {
            return
        }
        var tabbableElements = this._findTabbableBounds();
        var $firstTabbable = tabbableElements.first;
        var $lastTabbable = tabbableElements.last;
        var isTabOnLast = !e.shiftKey && e.target === $lastTabbable.get(0);
        var isShiftTabOnFirst = e.shiftKey && e.target === $firstTabbable.get(0);
        var isEmptyTabList = 0 === tabbableElements.length;
        var isOutsideTarget = !contains(this._$wrapper.get(0), e.target);
        if (isTabOnLast || isShiftTabOnFirst || isEmptyTabList || isOutsideTarget) {
            e.preventDefault();
            var $focusElement = e.shiftKey ? $lastTabbable : $firstTabbable;
            eventsEngine.trigger($focusElement, "focusin");
            eventsEngine.trigger($focusElement, "focus")
        }
    },
    _toggleSubscriptions: function(enabled) {
        if (hasWindow()) {
            this._toggleHideTopOverlayCallback(enabled);
            this._toggleParentsScrollSubscription(enabled)
        }
    },
    _toggleHideTopOverlayCallback: function(subscribe) {
        if (!this._hideTopOverlayHandler) {
            return
        }
        if (subscribe) {
            hideTopOverlayCallback.add(this._hideTopOverlayHandler)
        } else {
            hideTopOverlayCallback.remove(this._hideTopOverlayHandler)
        }
    },
    _toggleParentsScrollSubscription: function(needSubscribe) {
        var _this$_parentsScrollS;
        var scrollEvent = addNamespace("scroll", this.NAME);
        var {
            prevTargets: prevTargets,
            handler: handler
        } = null !== (_this$_parentsScrollS = this._parentsScrollSubscriptionInfo) && void 0 !== _this$_parentsScrollS ? _this$_parentsScrollS : {};
        eventsEngine.off(prevTargets, scrollEvent, handler);
        var closeOnScroll = this.option("hideOnParentScroll");
        if (needSubscribe && closeOnScroll) {
            var $parents = this._$wrapper.parents();
            if ("desktop" === devices.real().deviceType) {
                $parents = $parents.add(window)
            }
            eventsEngine.on($parents, scrollEvent, handler);
            this._parentsScrollSubscriptionInfo.prevTargets = $parents
        }
    },
    _targetParentsScrollHandler: function(e) {
        var closeHandled = false;
        var closeOnScroll = this.option("hideOnParentScroll");
        if (isFunction(closeOnScroll)) {
            closeHandled = closeOnScroll(e)
        }
        if (!closeHandled && !this._showAnimationProcessing) {
            this.hide()
        }
    },
    _render: function() {
        this.callBase();
        this._appendContentToElement();
        this._renderVisibilityAnimate(this.option("visible"))
    },
    _appendContentToElement: function() {
        if (!this._$content.parent().is(this.$element())) {
            this._$content.appendTo(this.$element())
        }
    },
    _renderContent: function() {
        var shouldDeferRendering = !this._currentVisible && this.option("deferRendering");
        var isParentHidden = this.option("visible") && this._isParentHidden();
        if (isParentHidden) {
            this._isHidden = true;
            return
        }
        if (this._contentAlreadyRendered || shouldDeferRendering) {
            return
        }
        this._contentAlreadyRendered = true;
        this._appendContentToElement();
        this.callBase()
    },
    _isParentHidden: function() {
        if (!this.option("_checkParentVisibility")) {
            return false
        }
        if (void 0 !== this._parentHidden) {
            return this._parentHidden
        }
        var $parent = this.$element().parent();
        if ($parent.is(":visible")) {
            return false
        }
        var isHidden = false;
        $parent.add($parent.parents()).each((function() {
            var $element = $(this);
            if ("none" === $element.css("display")) {
                isHidden = true;
                return false
            }
        }));
        return isHidden || !domAdapter.getBody().contains($parent.get(0))
    },
    _renderContentImpl: function() {
        var whenContentRendered = new Deferred;
        var contentTemplateOption = this.option("contentTemplate");
        var contentTemplate = this._getTemplate(contentTemplateOption);
        var transclude = this._templateManager.anonymousTemplateName === contentTemplateOption;
        contentTemplate && contentTemplate.render({
            container: getPublicElement(this.$content()),
            noModel: true,
            transclude: transclude,
            onRendered: () => {
                whenContentRendered.resolve()
            }
        });
        this._renderDrag();
        this._renderResize();
        this._renderScrollTerminator();
        whenContentRendered.done(() => {
            if (this.option("visible")) {
                this._moveToContainer()
            }
        });
        return whenContentRendered.promise()
    },
    _getPositionControllerConfig() {
        var {
            target: target,
            container: container,
            dragAndResizeArea: dragAndResizeArea,
            dragOutsideBoundary: dragOutsideBoundary,
            outsideDragFactor: outsideDragFactor,
            _fixWrapperPosition: _fixWrapperPosition,
            restorePosition: restorePosition
        } = this.option();
        return {
            target: target,
            container: container,
            $root: this.$element(),
            $content: this._$content,
            $wrapper: this._$wrapper,
            onPositioned: this._actions.onPositioned,
            onVisualPositionChanged: this._actions.onVisualPositionChanged,
            restorePosition: restorePosition,
            dragAndResizeArea: dragAndResizeArea,
            dragOutsideBoundary: dragOutsideBoundary,
            outsideDragFactor: outsideDragFactor,
            _fixWrapperPosition: _fixWrapperPosition
        }
    },
    _initPositionController() {
        this._positionController = new OverlayPositionController(this._getPositionControllerConfig())
    },
    _renderDrag: function() {
        var $dragTarget = this._getDragTarget();
        if (!$dragTarget) {
            return
        }
        var config = {
            dragEnabled: this.option("dragEnabled"),
            handle: $dragTarget.get(0),
            draggableElement: this._$content.get(0),
            positionController: this._positionController
        };
        if (this._drag) {
            this._drag.init(config)
        } else {
            this._drag = new OverlayDrag(config)
        }
    },
    _renderResize: function() {
        this._resizable = this._createComponent(this._$content, Resizable, {
            handles: this.option("resizeEnabled") ? "all" : "none",
            onResizeEnd: e => {
                this._resizeEndHandler(e);
                this._observeContentResize(true)
            },
            onResize: e => {
                this._actions.onResize(e)
            },
            onResizeStart: e => {
                this._observeContentResize(false);
                this._actions.onResizeStart(e)
            },
            minHeight: 100,
            minWidth: 100,
            area: this._positionController.$dragResizeContainer
        })
    },
    _resizeEndHandler: function(e) {
        var width = this._resizable.option("width");
        var height = this._resizable.option("height");
        width && this._setOptionWithoutOptionChange("width", width);
        height && this._setOptionWithoutOptionChange("height", height);
        this._cacheDimensions();
        this._positionController.resizeHandled();
        this._positionController.detectVisualPositionChange(e.event);
        this._actions.onResizeEnd(e)
    },
    _renderScrollTerminator: function() {
        var $scrollTerminator = this._$wrapper;
        var terminatorEventName = addNamespace(dragEventMove, this.NAME);
        eventsEngine.off($scrollTerminator, terminatorEventName);
        eventsEngine.on($scrollTerminator, terminatorEventName, {
            validate: function() {
                return true
            },
            getDirection: function() {
                return "both"
            },
            _toggleGestureCover: function(toggle) {
                if (!toggle) {
                    this._toggleGestureCoverImpl(toggle)
                }
            },
            _clearSelection: noop,
            isNative: true
        }, e => {
            var originalEvent = e.originalEvent.originalEvent;
            var {
                type: type
            } = originalEvent || {};
            var isWheel = "wheel" === type;
            var isMouseMove = "mousemove" === type;
            var isScrollByWheel = isWheel && !isCommandKeyPressed(e);
            e._cancelPreventDefault = true;
            if (originalEvent && false !== e.cancelable && (!isMouseMove && !isWheel || isScrollByWheel)) {
                e.preventDefault()
            }
        })
    },
    _getDragTarget: function() {
        return this.$content()
    },
    _moveFromContainer: function() {
        this._$content.appendTo(this.$element());
        this._detachWrapperToContainer()
    },
    _detachWrapperToContainer: function() {
        this._$wrapper.detach()
    },
    _moveToContainer: function() {
        this._attachWrapperToContainer();
        this._$content.appendTo(this._$wrapper)
    },
    _attachWrapperToContainer: function() {
        var $element = this.$element();
        var containerDefined = void 0 !== this.option("container");
        var renderContainer = containerDefined ? this._positionController.$container : swatch.getSwatchContainer($element);
        if (renderContainer && renderContainer[0] === $element.parent()[0]) {
            renderContainer = $element
        }
        this._$wrapper.appendTo(renderContainer)
    },
    _renderGeometry: function(options) {
        var {
            visible: visible,
            useResizeObserver: useResizeObserver
        } = this.option();
        if (visible && hasWindow()) {
            var isAnimated = this._showAnimationProcessing;
            var shouldRepeatAnimation = isAnimated && !(null !== options && void 0 !== options && options.forceStopAnimation) && useResizeObserver;
            this._isAnimationPaused = shouldRepeatAnimation || void 0;
            this._stopAnimation();
            if (null !== options && void 0 !== options && options.shouldOnlyReposition) {
                this._positionController.positionContent()
            } else {
                this._renderGeometryImpl()
            }
            if (shouldRepeatAnimation) {
                this._animateShowing();
                this._isAnimationPaused = void 0
            }
        }
    },
    _cacheDimensions: function() {
        if (!this.option("useResizeObserver")) {
            return
        }
        this._renderedDimensions = {
            width: parseInt(getWidth(this._$content), 10),
            height: parseInt(getHeight(this._$content), 10)
        }
    },
    _renderGeometryImpl: function() {
        this._positionController.updatePosition(this._getOptionValue("position"));
        this._renderWrapper();
        this._renderDimensions();
        this._renderPosition();
        this._cacheDimensions()
    },
    _renderPosition() {
        this._positionController.positionContent()
    },
    _isAllWindowCovered: function() {
        return this._positionController.isAllWindowCoveredByWrapper() && this.option("shading")
    },
    _toggleSafariScrolling: function() {
        var visible = this.option("visible");
        var $body = $(domAdapter.getBody());
        var isIosSafari = "ios" === devices.real().platform && browser.safari;
        var isAllWindowCovered = this._isAllWindowCovered();
        var isScrollingPrevented = $body.hasClass(PREVENT_SAFARI_SCROLLING_CLASS);
        var shouldPreventScrolling = !isScrollingPrevented && visible && isAllWindowCovered;
        var shouldEnableScrolling = isScrollingPrevented && (!visible || !isAllWindowCovered || this._disposed);
        if (isIosSafari) {
            if (shouldEnableScrolling) {
                $body.removeClass(PREVENT_SAFARI_SCROLLING_CLASS);
                window.scrollTo(0, this._cachedBodyScrollTop);
                this._cachedBodyScrollTop = void 0
            } else if (shouldPreventScrolling) {
                this._cachedBodyScrollTop = window.pageYOffset;
                $body.addClass(PREVENT_SAFARI_SCROLLING_CLASS)
            }
        }
    },
    _renderWrapper: function() {
        this._positionController.styleWrapperPosition();
        this._renderWrapperDimensions();
        this._positionController.positionWrapper()
    },
    _renderWrapperDimensions: function() {
        var wrapperWidth;
        var wrapperHeight;
        var $container = this._positionController._$wrapperCoveredElement;
        if (!$container) {
            return
        }
        var isWindow = this._positionController.isAllWindowCoveredByWrapper();
        var documentElement = domAdapter.getDocumentElement();
        wrapperWidth = isWindow ? documentElement.clientWidth : getOuterWidth($container), wrapperHeight = isWindow ? window.innerHeight : getOuterHeight($container);
        this._$wrapper.css({
            width: wrapperWidth,
            height: wrapperHeight
        })
    },
    _renderDimensions: function() {
        var content = this._$content.get(0);
        this._$content.css({
            minWidth: this._getOptionValue("minWidth", content),
            maxWidth: this._getOptionValue("maxWidth", content),
            minHeight: this._getOptionValue("minHeight", content),
            maxHeight: this._getOptionValue("maxHeight", content),
            width: this._getOptionValue("width", content),
            height: this._getOptionValue("height", content)
        })
    },
    _focusTarget: function() {
        return this._$content
    },
    _attachKeyboardEvents: function() {
        this._keyboardListenerId = keyboard.on(this._$content, null, opts => this._keyboardHandler(opts))
    },
    _keyboardHandler: function(options) {
        var e = options.originalEvent;
        var $target = $(e.target);
        if ($target.is(this._$content) || !this.option("ignoreChildEvents")) {
            this.callBase(...arguments)
        }
    },
    _isVisible: function() {
        return this.option("visible")
    },
    _visibilityChanged: function(visible) {
        if (visible) {
            if (this.option("visible")) {
                this._renderVisibilityAnimate(visible)
            }
        } else {
            this._renderVisibilityAnimate(visible)
        }
    },
    _dimensionChanged: function() {
        this._renderGeometry()
    },
    _clean: function() {
        if (!this._contentAlreadyRendered) {
            this.$content().empty()
        }
        this._renderVisibility(false);
        this._stopShowTimer();
        this._observeContentResize(false);
        this._cleanFocusState()
    },
    _stopShowTimer() {
        if (this._asyncShowTimeout) {
            clearTimeout(this._asyncShowTimeout)
        }
        this._asyncShowTimeout = null
    },
    _dispose: function() {
        fx.stop(this._$content, false);
        clearTimeout(this._deferShowTimer);
        this._toggleViewPortSubscription(false);
        this._toggleSubscriptions(false);
        this._updateZIndexStackPosition(false);
        this._toggleTabTerminator(false);
        this._actions = null;
        this._parentsScrollSubscriptionInfo = null;
        this.callBase();
        this._toggleSafariScrolling();
        zIndexPool.remove(this._zIndex);
        this._$wrapper.remove();
        this._$content.remove()
    },
    _toggleRTLDirection: function(rtl) {
        this._$content.toggleClass(RTL_DIRECTION_CLASS, rtl)
    },
    _optionChanged: function(args) {
        var _this$_resizable;
        var value = args.value;
        if (inArray(args.name, ACTIONS) > -1) {
            this._initActions();
            return
        }
        switch (args.name) {
            case "dragEnabled":
                this._renderDrag();
                this._renderGeometry();
                break;
            case "resizeEnabled":
                this._renderResize();
                this._renderGeometry();
                break;
            case "shading":
                this._toggleShading(this.option("visible"));
                this._toggleSafariScrolling();
                break;
            case "shadingColor":
                this._toggleShading(this.option("visible"));
                break;
            case "width":
            case "height":
                this._renderGeometry();
                null === (_this$_resizable = this._resizable) || void 0 === _this$_resizable ? void 0 : _this$_resizable.option(args.name, args.value);
                break;
            case "minWidth":
            case "maxWidth":
            case "minHeight":
            case "maxHeight":
                this._renderGeometry();
                break;
            case "position":
                this._positionController.updatePosition(this.option("position"));
                this._positionController.restorePositionOnNextRender(true);
                this._renderGeometry();
                this._toggleSafariScrolling();
                break;
            case "visible":
                this._renderVisibilityAnimate(value).done(() => {
                    if (!this._animateDeferred) {
                        return
                    }
                    this._animateDeferred.resolveWith(this)
                });
                break;
            case "target":
                this._positionController.updateTarget(value);
                this._setAnimationTarget(value);
                this._invalidate();
                break;
            case "container":
                this._positionController.updateContainer(value);
                this._invalidate();
                this._toggleSafariScrolling();
                if (this.option("resizeEnabled")) {
                    var _this$_resizable2;
                    null === (_this$_resizable2 = this._resizable) || void 0 === _this$_resizable2 ? void 0 : _this$_resizable2.option("area", this._positionController.$dragResizeContainer)
                }
                break;
            case "innerOverlay":
                this._initInnerOverlayClass();
                break;
            case "deferRendering":
            case "contentTemplate":
                this._contentAlreadyRendered = false;
                this._clean();
                this._invalidate();
                break;
            case "hideTopOverlayHandler":
                this._toggleHideTopOverlayCallback(false);
                this._initHideTopOverlayHandler(args.value);
                this._toggleHideTopOverlayCallback(this.option("visible"));
                break;
            case "hideOnParentScroll":
                this._toggleParentsScrollSubscription(this.option("visible"));
                break;
            case "closeOnOutsideClick":
            case "propagateOutsideClick":
                break;
            case "animation":
                this._updateResizeCallbackSkipCondition();
                break;
            case "rtlEnabled":
                this._contentAlreadyRendered = false;
                this.callBase(args);
                break;
            case "_fixWrapperPosition":
                this._positionController.fixWrapperPosition = value;
                break;
            case "wrapperAttr":
                this._renderWrapperAttributes();
                break;
            case "dragAndResizeArea":
                this._positionController.dragAndResizeArea = value;
                if (this.option("resizeEnabled")) {
                    this._resizable.option("area", this._positionController.$dragResizeContainer)
                }
                this._positionController.positionContent();
                break;
            case "dragOutsideBoundary":
                this._positionController.dragOutsideBoundary = value;
                if (this.option("resizeEnabled")) {
                    this._resizable.option("area", this._positionController.$dragResizeContainer)
                }
                break;
            case "outsideDragFactor":
                this._positionController.outsideDragFactor = value;
                break;
            case "restorePosition":
                this._positionController.restorePosition = args.value;
                break;
            default:
                this.callBase(args)
        }
    },
    toggle: function(showing) {
        showing = void 0 === showing ? !this.option("visible") : showing;
        var result = new Deferred;
        if (showing === this.option("visible")) {
            return result.resolveWith(this, [showing]).promise()
        }
        var animateDeferred = new Deferred;
        this._animateDeferred = animateDeferred;
        this.option("visible", showing);
        animateDeferred.promise().done(() => {
            delete this._animateDeferred;
            result.resolveWith(this, [this.option("visible")])
        });
        return result.promise()
    },
    $content: function() {
        return this._$content
    },
    show: function() {
        return this.toggle(true)
    },
    hide: function() {
        return this.toggle(false)
    },
    content: function() {
        return getPublicElement(this._$content)
    },
    repaint: function() {
        if (this._contentAlreadyRendered) {
            this._positionController.restorePositionOnNextRender(true);
            this._renderGeometry({
                forceStopAnimation: true
            });
            triggerResizeEvent(this._$content)
        } else {
            this.callBase()
        }
    }
});
Overlay.baseZIndex = zIndex => zIndexPool.base(zIndex);
registerComponent("dxOverlay", Overlay);
export default Overlay;
