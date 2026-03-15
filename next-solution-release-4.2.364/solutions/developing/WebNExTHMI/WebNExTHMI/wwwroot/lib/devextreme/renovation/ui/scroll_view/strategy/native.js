/**
 * DevExtreme (renovation/ui/scroll_view/strategy/native.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.ScrollableNative = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
require("../../../../events/gesture/emitter.gesture.scroll");
var _subscribe_to_event = require("../../../utils/subscribe_to_event");
var _widget = require("../../common/widget");
var _load_panel = require("../internal/load_panel");
var _combine_classes = require("../../../utils/combine_classes");
var _get_scroll_left_max = require("../utils/get_scroll_left_max");
var _get_boundary_props = require("../utils/get_boundary_props");
var _normalize_offset_left = require("../utils/normalize_offset_left");
var _get_element_style = require("../utils/get_element_style");
var _devices = _interopRequireDefault(require("../../../../core/devices"));
var _type = require("../../../../core/utils/type");
var _top = require("../internal/pocket/top");
var _bottom = require("../internal/pocket/bottom");
var _index = require("../../../../events/utils/index");
var _scroll_direction = require("../utils/scroll_direction");
var _consts = require("../common/consts");
var _scrollbar = require("../scrollbar/scrollbar");
var _is_element_visible = require("../utils/is_element_visible");
var _native_strategy_props = require("../common/native_strategy_props");
var _get_allowed_direction = require("../utils/get_allowed_direction");
var _get_scroll_top_max = require("../utils/get_scroll_top_max");
var _subscribe_to_resize = require("../utils/subscribe_to_resize");
var _excluded = ["addWidgetClass", "aria", "bounceEnabled", "children", "classes", "direction", "disabled", "forceGeneratePockets", "height", "needRenderScrollbars", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "onPullDown", "onReachBottom", "onScroll", "onUpdated", "pullDownEnabled", "pulledDownText", "pullingDownText", "reachBottomEnabled", "reachBottomText", "refreshStrategy", "refreshingText", "rtlEnabled", "scrollByContent", "showScrollbar", "useSimulatedScrollbar", "visible", "width"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
}

function _defineProperty(obj, key, value) {
    if (key in obj) {
        Object.defineProperty(obj, key, {
            value: value,
            enumerable: true,
            configurable: true,
            writable: true
        })
    } else {
        obj[key] = value
    }
    return obj
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var viewFunction = function(viewModel) {
    var bottomPocketRef = viewModel.bottomPocketRef,
        containerClientHeight = viewModel.containerClientHeight,
        containerClientWidth = viewModel.containerClientWidth,
        containerRef = viewModel.containerRef,
        contentHeight = viewModel.contentHeight,
        contentRef = viewModel.contentRef,
        contentStyles = viewModel.contentStyles,
        contentTranslateTop = viewModel.contentTranslateTop,
        contentWidth = viewModel.contentWidth,
        cssClasses = viewModel.cssClasses,
        direction = viewModel.direction,
        hScrollLocation = viewModel.hScrollLocation,
        hScrollOffsetMax = viewModel.hScrollOffsetMax,
        hScrollbarRef = viewModel.hScrollbarRef,
        isLoadPanelVisible = viewModel.isLoadPanelVisible,
        _viewModel$props = viewModel.props,
        aria = _viewModel$props.aria,
        children = _viewModel$props.children,
        disabled = _viewModel$props.disabled,
        forceGeneratePockets = _viewModel$props.forceGeneratePockets,
        height = _viewModel$props.height,
        needRenderScrollbars = _viewModel$props.needRenderScrollbars,
        needScrollViewContentWrapper = _viewModel$props.needScrollViewContentWrapper,
        needScrollViewLoadPanel = _viewModel$props.needScrollViewLoadPanel,
        pullDownEnabled = _viewModel$props.pullDownEnabled,
        pulledDownText = _viewModel$props.pulledDownText,
        pullingDownText = _viewModel$props.pullingDownText,
        reachBottomEnabled = _viewModel$props.reachBottomEnabled,
        reachBottomText = _viewModel$props.reachBottomText,
        refreshStrategy = _viewModel$props.refreshStrategy,
        refreshingText = _viewModel$props.refreshingText,
        rtlEnabled = _viewModel$props.rtlEnabled,
        showScrollbar = _viewModel$props.showScrollbar,
        useSimulatedScrollbar = _viewModel$props.useSimulatedScrollbar,
        visible = _viewModel$props.visible,
        width = _viewModel$props.width,
        pullDownIconAngle = viewModel.pullDownIconAngle,
        pullDownOpacity = viewModel.pullDownOpacity,
        pullDownTranslateTop = viewModel.pullDownTranslateTop,
        restAttributes = viewModel.restAttributes,
        scrollViewContentRef = viewModel.scrollViewContentRef,
        scrollableRef = viewModel.scrollableRef,
        scrolling = viewModel.scrolling,
        topPocketHeight = viewModel.topPocketHeight,
        topPocketRef = viewModel.topPocketRef,
        topPocketState = viewModel.topPocketState,
        vScrollLocation = viewModel.vScrollLocation,
        vScrollOffsetMax = viewModel.vScrollOffsetMax,
        vScrollbarRef = viewModel.vScrollbarRef,
        wrapperRef = viewModel.wrapperRef;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _widget.Widget, _extends({
        rootElementRef: scrollableRef,
        aria: aria,
        addWidgetClass: false,
        classes: cssClasses,
        disabled: disabled,
        rtlEnabled: rtlEnabled,
        height: height,
        width: width,
        visible: visible
    }, restAttributes, {
        children: [(0, _inferno.createVNode)(1, "div", _consts.SCROLLABLE_WRAPPER_CLASS, (0, _inferno.createVNode)(1, "div", _consts.SCROLLABLE_CONTAINER_CLASS, (0, _inferno.createVNode)(1, "div", _consts.SCROLLABLE_CONTENT_CLASS, [forceGeneratePockets && (0, _inferno.createComponentVNode)(2, _top.TopPocket, {
            topPocketRef: topPocketRef,
            pullingDownText: pullingDownText,
            pulledDownText: pulledDownText,
            refreshingText: refreshingText,
            pocketState: topPocketState,
            refreshStrategy: refreshStrategy,
            pullDownTranslateTop: pullDownTranslateTop,
            pullDownIconAngle: pullDownIconAngle,
            topPocketTranslateTop: contentTranslateTop,
            pullDownOpacity: pullDownOpacity,
            pocketTop: topPocketHeight,
            visible: !!pullDownEnabled
        }), needScrollViewContentWrapper ? (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_CONTENT_CLASS, children, 0, {
            style: (0, _inferno2.normalizeStyles)(contentStyles)
        }, null, scrollViewContentRef) : children, forceGeneratePockets && (0, _inferno.createComponentVNode)(2, _bottom.BottomPocket, {
            bottomPocketRef: bottomPocketRef,
            reachBottomText: reachBottomText,
            visible: !!reachBottomEnabled
        })], 0, null, null, contentRef), 2, null, null, containerRef), 2, null, null, wrapperRef), needScrollViewLoadPanel && (0, _inferno.createComponentVNode)(2, _load_panel.ScrollViewLoadPanel, {
            targetElement: scrollableRef,
            refreshingText: refreshingText,
            visible: isLoadPanelVisible
        }), needRenderScrollbars && "never" !== showScrollbar && useSimulatedScrollbar && direction.isHorizontal && (0, _inferno.createComponentVNode)(2, _scrollbar.Scrollbar, {
            direction: "horizontal",
            showScrollbar: "onScroll",
            contentSize: contentWidth,
            containerSize: containerClientWidth,
            maxOffset: hScrollOffsetMax,
            scrollLocation: hScrollLocation,
            visible: scrolling
        }, null, hScrollbarRef), needRenderScrollbars && "never" !== showScrollbar && useSimulatedScrollbar && direction.isVertical && (0, _inferno.createComponentVNode)(2, _scrollbar.Scrollbar, {
            direction: "vertical",
            showScrollbar: "onScroll",
            contentSize: contentHeight,
            containerSize: containerClientHeight,
            maxOffset: vScrollOffsetMax,
            scrollLocation: vScrollLocation,
            visible: scrolling
        }, null, vScrollbarRef)]
    })))
};
exports.viewFunction = viewFunction;
var ScrollableNative = function(_InfernoComponent) {
    _inheritsLoose(ScrollableNative, _InfernoComponent);

    function ScrollableNative(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.scrollableRef = (0, _inferno.createRef)();
        _this.topPocketRef = (0, _inferno.createRef)();
        _this.bottomPocketRef = (0, _inferno.createRef)();
        _this.wrapperRef = (0, _inferno.createRef)();
        _this.contentRef = (0, _inferno.createRef)();
        _this.scrollViewContentRef = (0, _inferno.createRef)();
        _this.containerRef = (0, _inferno.createRef)();
        _this.vScrollbarRef = (0, _inferno.createRef)();
        _this.hScrollbarRef = (0, _inferno.createRef)();
        _this.locked = false;
        _this.loadingIndicatorEnabled = true;
        _this.initPageY = 0;
        _this.deltaY = 0;
        _this.locationTop = 0;
        _this.__getterCache = {};
        _this.state = {
            containerClientWidth: 0,
            containerClientHeight: 0,
            contentClientWidth: 0,
            contentClientHeight: 0,
            contentScrollWidth: 0,
            contentScrollHeight: 0,
            topPocketHeight: 0,
            bottomPocketHeight: 0,
            scrolling: false,
            topPocketState: _consts.TopPocketState.STATE_RELEASED,
            isLoadPanelVisible: false,
            pullDownTranslateTop: 0,
            pullDownIconAngle: 0,
            pullDownOpacity: 0,
            contentTranslateTop: 0,
            vScrollLocation: 0,
            hScrollLocation: 0
        };
        _this.content = _this.content.bind(_assertThisInitialized(_this));
        _this.container = _this.container.bind(_assertThisInitialized(_this));
        _this.refresh = _this.refresh.bind(_assertThisInitialized(_this));
        _this.release = _this.release.bind(_assertThisInitialized(_this));
        _this.disposeReleaseTimer = _this.disposeReleaseTimer.bind(_assertThisInitialized(_this));
        _this.scrollHeight = _this.scrollHeight.bind(_assertThisInitialized(_this));
        _this.scrollWidth = _this.scrollWidth.bind(_assertThisInitialized(_this));
        _this.scrollOffset = _this.scrollOffset.bind(_assertThisInitialized(_this));
        _this.scrollTop = _this.scrollTop.bind(_assertThisInitialized(_this));
        _this.scrollLeft = _this.scrollLeft.bind(_assertThisInitialized(_this));
        _this.clientHeight = _this.clientHeight.bind(_assertThisInitialized(_this));
        _this.clientWidth = _this.clientWidth.bind(_assertThisInitialized(_this));
        _this.scrollEffect = _this.scrollEffect.bind(_assertThisInitialized(_this));
        _this.effectDisabledState = _this.effectDisabledState.bind(_assertThisInitialized(_this));
        _this.effectResetInactiveState = _this.effectResetInactiveState.bind(_assertThisInitialized(_this));
        _this.initEffect = _this.initEffect.bind(_assertThisInitialized(_this));
        _this.moveEffect = _this.moveEffect.bind(_assertThisInitialized(_this));
        _this.endEffect = _this.endEffect.bind(_assertThisInitialized(_this));
        _this.stopEffect = _this.stopEffect.bind(_assertThisInitialized(_this));
        _this.disposeRefreshTimer = _this.disposeRefreshTimer.bind(_assertThisInitialized(_this));
        _this.validate = _this.validate.bind(_assertThisInitialized(_this));
        _this.moveIsAllowed = _this.moveIsAllowed.bind(_assertThisInitialized(_this));
        _this.updateHandler = _this.updateHandler.bind(_assertThisInitialized(_this));
        _this.updateDimensions = _this.updateDimensions.bind(_assertThisInitialized(_this));
        _this.subscribeContainerToResize = _this.subscribeContainerToResize.bind(_assertThisInitialized(_this));
        _this.subscribeContentToResize = _this.subscribeContentToResize.bind(_assertThisInitialized(_this));
        _this.scrollByLocation = _this.scrollByLocation.bind(_assertThisInitialized(_this));
        _this.clearReleaseTimer = _this.clearReleaseTimer.bind(_assertThisInitialized(_this));
        _this.onRelease = _this.onRelease.bind(_assertThisInitialized(_this));
        _this.onUpdated = _this.onUpdated.bind(_assertThisInitialized(_this));
        _this.startLoading = _this.startLoading.bind(_assertThisInitialized(_this));
        _this.finishLoading = _this.finishLoading.bind(_assertThisInitialized(_this));
        _this.setPocketState = _this.setPocketState.bind(_assertThisInitialized(_this));
        _this.handleScroll = _this.handleScroll.bind(_assertThisInitialized(_this));
        _this.handlePocketState = _this.handlePocketState.bind(_assertThisInitialized(_this));
        _this.pullDownReady = _this.pullDownReady.bind(_assertThisInitialized(_this));
        _this.onReachBottom = _this.onReachBottom.bind(_assertThisInitialized(_this));
        _this.onPullDown = _this.onPullDown.bind(_assertThisInitialized(_this));
        _this.stateReleased = _this.stateReleased.bind(_assertThisInitialized(_this));
        _this.getEventArgs = _this.getEventArgs.bind(_assertThisInitialized(_this));
        _this.lock = _this.lock.bind(_assertThisInitialized(_this));
        _this.unlock = _this.unlock.bind(_assertThisInitialized(_this));
        _this.updateElementDimensions = _this.updateElementDimensions.bind(_assertThisInitialized(_this));
        _this.setContainerDimensions = _this.setContainerDimensions.bind(_assertThisInitialized(_this));
        _this.setContentDimensions = _this.setContentDimensions.bind(_assertThisInitialized(_this));
        _this.syncScrollbarsWithContent = _this.syncScrollbarsWithContent.bind(_assertThisInitialized(_this));
        _this.getInitEventData = _this.getInitEventData.bind(_assertThisInitialized(_this));
        _this.handleInit = _this.handleInit.bind(_assertThisInitialized(_this));
        _this.handleMove = _this.handleMove.bind(_assertThisInitialized(_this));
        _this.handleEnd = _this.handleEnd.bind(_assertThisInitialized(_this));
        _this.handleStop = _this.handleStop.bind(_assertThisInitialized(_this));
        _this.pullDownComplete = _this.pullDownComplete.bind(_assertThisInitialized(_this));
        _this.clearRefreshTimer = _this.clearRefreshTimer.bind(_assertThisInitialized(_this));
        _this.pullDownRefreshing = _this.pullDownRefreshing.bind(_assertThisInitialized(_this));
        _this.movePullDown = _this.movePullDown.bind(_assertThisInitialized(_this));
        _this.getPullDownHeight = _this.getPullDownHeight.bind(_assertThisInitialized(_this));
        _this.getPullDownStartPosition = _this.getPullDownStartPosition.bind(_assertThisInitialized(_this));
        _this.complete = _this.complete.bind(_assertThisInitialized(_this));
        _this.releaseState = _this.releaseState.bind(_assertThisInitialized(_this));
        _this.isSwipeDown = _this.isSwipeDown.bind(_assertThisInitialized(_this));
        _this.pulledDown = _this.pulledDown.bind(_assertThisInitialized(_this));
        _this.isReachBottom = _this.isReachBottom.bind(_assertThisInitialized(_this));
        _this.tryGetAllowedDirection = _this.tryGetAllowedDirection.bind(_assertThisInitialized(_this));
        _this.isLocked = _this.isLocked.bind(_assertThisInitialized(_this));
        _this.isScrollingOutOfBound = _this.isScrollingOutOfBound.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = ScrollableNative.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.disposeReleaseTimer, []), new _inferno2.InfernoEffect(this.scrollEffect, [this.props.useSimulatedScrollbar, this.props.onScroll, this.props.rtlEnabled, this.props.direction, this.props.forceGeneratePockets, this.state.topPocketState, this.props.refreshStrategy, this.props.reachBottomEnabled, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.props.onReachBottom, this.props.pullDownEnabled, this.state.topPocketHeight]), new _inferno2.InfernoEffect(this.effectDisabledState, [this.props.disabled]), new _inferno2.InfernoEffect(this.effectResetInactiveState, [this.props.direction]), new _inferno2.InfernoEffect(this.initEffect, [this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.props.direction, this.props.needScrollViewContentWrapper, this.props.onUpdated, this.props.rtlEnabled, this.props.disabled]), new _inferno2.InfernoEffect(this.moveEffect, [this.props.direction, this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.props.pullDownEnabled, this.state.topPocketHeight]), new _inferno2.InfernoEffect(this.endEffect, [this.props.forceGeneratePockets, this.props.refreshStrategy, this.props.pullDownEnabled, this.state.topPocketState, this.state.topPocketHeight, this.props.onPullDown]), new _inferno2.InfernoEffect(this.stopEffect, [this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.state.topPocketHeight, this.props.onPullDown]), new _inferno2.InfernoEffect(this.disposeRefreshTimer, []), new _inferno2.InfernoEffect(this.updateDimensions, []), new _inferno2.InfernoEffect(this.subscribeContainerToResize, []), new _inferno2.InfernoEffect(this.subscribeContentToResize, [])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5, _this$_effects$6, _this$_effects$7;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.useSimulatedScrollbar, this.props.onScroll, this.props.rtlEnabled, this.props.direction, this.props.forceGeneratePockets, this.state.topPocketState, this.props.refreshStrategy, this.props.reachBottomEnabled, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.props.onReachBottom, this.props.pullDownEnabled, this.state.topPocketHeight]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.disabled]);
        null === (_this$_effects$3 = this._effects[3]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.direction]);
        null === (_this$_effects$4 = this._effects[4]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.props.direction, this.props.needScrollViewContentWrapper, this.props.onUpdated, this.props.rtlEnabled, this.props.disabled]);
        null === (_this$_effects$5 = this._effects[5]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.props.direction, this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.props.pullDownEnabled, this.state.topPocketHeight]);
        null === (_this$_effects$6 = this._effects[6]) || void 0 === _this$_effects$6 ? void 0 : _this$_effects$6.update([this.props.forceGeneratePockets, this.props.refreshStrategy, this.props.pullDownEnabled, this.state.topPocketState, this.state.topPocketHeight, this.props.onPullDown]);
        null === (_this$_effects$7 = this._effects[7]) || void 0 === _this$_effects$7 ? void 0 : _this$_effects$7.update([this.props.forceGeneratePockets, this.props.refreshStrategy, this.state.topPocketState, this.state.topPocketHeight, this.props.onPullDown])
    };
    _proto.disposeReleaseTimer = function() {
        var _this2 = this;
        return function() {
            return _this2.clearReleaseTimer()
        }
    };
    _proto.scrollEffect = function() {
        var _this3 = this;
        return (0, _subscribe_to_event.subscribeToScrollEvent)(this.containerRef.current, (function(event) {
            _this3.handleScroll(event)
        }))
    };
    _proto.effectDisabledState = function() {
        if (this.props.disabled) {
            this.lock()
        } else {
            this.unlock()
        }
    };
    _proto.effectResetInactiveState = function() {
        if (this.props.direction === _consts.DIRECTION_BOTH) {
            return
        }
        this.containerRef.current[this.fullScrollInactiveProp] = 0
    };
    _proto.initEffect = function() {
        var _this4 = this;
        return (0, _subscribe_to_event.subscribeToScrollInitEvent)(this.wrapperRef.current, (function(event) {
            _this4.handleInit(event)
        }), this.getInitEventData())
    };
    _proto.moveEffect = function() {
        var _this5 = this;
        return (0, _subscribe_to_event.subscribeToDXScrollMoveEvent)(this.wrapperRef.current, (function(event) {
            _this5.handleMove(event)
        }))
    };
    _proto.endEffect = function() {
        var _this6 = this;
        return (0, _subscribe_to_event.subscribeToDXScrollEndEvent)(this.wrapperRef.current, (function() {
            _this6.handleEnd()
        }))
    };
    _proto.stopEffect = function() {
        var _this7 = this;
        return (0, _subscribe_to_event.subscribeToDXScrollStopEvent)(this.wrapperRef.current, (function() {
            _this7.handleStop()
        }))
    };
    _proto.disposeRefreshTimer = function() {
        var _this8 = this;
        return function() {
            return _this8.clearRefreshTimer()
        }
    };
    _proto.updateDimensions = function() {
        this.updateElementDimensions()
    };
    _proto.subscribeContainerToResize = function() {
        var _this9 = this;
        return (0, _subscribe_to_resize.subscribeToResize)(this.containerRef.current, (function(element) {
            _this9.setContainerDimensions(element)
        }))
    };
    _proto.subscribeContentToResize = function() {
        var _this10 = this;
        return (0, _subscribe_to_resize.subscribeToResize)(this.content(), (function(element) {
            _this10.setContentDimensions(element)
        }))
    };
    _proto.clearReleaseTimer = function() {
        clearTimeout(this.releaseTimer);
        this.releaseTimer = void 0
    };
    _proto.onRelease = function() {
        this.loadingIndicatorEnabled = true;
        this.finishLoading();
        this.updateHandler()
    };
    _proto.onUpdated = function() {
        var _this$props$onUpdated, _this$props;
        null === (_this$props$onUpdated = (_this$props = this.props).onUpdated) || void 0 === _this$props$onUpdated ? void 0 : _this$props$onUpdated.call(_this$props, this.getEventArgs())
    };
    _proto.startLoading = function() {
        if (this.loadingIndicatorEnabled && (0, _is_element_visible.isElementVisible)(this.scrollableRef.current)) {
            this.setState((function(__state_argument) {
                return {
                    isLoadPanelVisible: true
                }
            }))
        }
        this.lock()
    };
    _proto.finishLoading = function() {
        this.setState((function(__state_argument) {
            return {
                isLoadPanelVisible: false
            }
        }));
        this.unlock()
    };
    _proto.setPocketState = function(newState) {
        this.setState((function(__state_argument) {
            return {
                topPocketState: newState
            }
        }))
    };
    _proto.handleScroll = function(event) {
        var _this$props$onScroll, _this$props2;
        this.eventForUserAction = event;
        if (this.props.useSimulatedScrollbar) {
            this.setState((function(__state_argument) {
                return {
                    scrolling: true
                }
            }));
            this.syncScrollbarsWithContent();
            this.setState((function(__state_argument) {
                return {
                    scrolling: false
                }
            }))
        }
        null === (_this$props$onScroll = (_this$props2 = this.props).onScroll) || void 0 === _this$props$onScroll ? void 0 : _this$props$onScroll.call(_this$props2, this.getEventArgs());
        this.handlePocketState()
    };
    _proto.handlePocketState = function() {
        if (this.props.forceGeneratePockets) {
            if (this.state.topPocketState === _consts.TopPocketState.STATE_REFRESHING) {
                return
            }
            var scrollTop = this.containerRef.current.scrollTop;
            var scrollDelta = this.locationTop + scrollTop;
            this.locationTop = -scrollTop;
            if (this.isSwipeDownStrategy && scrollDelta > 0 && this.isReachBottom()) {
                this.onReachBottom();
                return
            }
            if (this.isPullDownStrategy) {
                if (this.pulledDown()) {
                    this.pullDownReady();
                    return
                }
                if (scrollDelta > 0 && this.isReachBottom()) {
                    if (this.state.topPocketState !== _consts.TopPocketState.STATE_LOADING) {
                        this.setPocketState(_consts.TopPocketState.STATE_LOADING);
                        this.onReachBottom()
                    }
                    return
                }
            }
            this.stateReleased()
        }
    };
    _proto.pullDownReady = function() {
        if (this.state.topPocketState === _consts.TopPocketState.STATE_READY) {
            return
        }
        this.setPocketState(_consts.TopPocketState.STATE_READY)
    };
    _proto.onReachBottom = function() {
        var _this$props$onReachBo, _this$props3;
        null === (_this$props$onReachBo = (_this$props3 = this.props).onReachBottom) || void 0 === _this$props$onReachBo ? void 0 : _this$props$onReachBo.call(_this$props3, {})
    };
    _proto.onPullDown = function() {
        var _this$props$onPullDow, _this$props4;
        null === (_this$props$onPullDow = (_this$props4 = this.props).onPullDown) || void 0 === _this$props$onPullDow ? void 0 : _this$props$onPullDow.call(_this$props4, {})
    };
    _proto.stateReleased = function() {
        if (this.state.topPocketState === _consts.TopPocketState.STATE_RELEASED) {
            return
        }
        this.releaseState()
    };
    _proto.getEventArgs = function() {
        var scrollOffset = this.scrollOffset();
        return _extends({
            event: this.eventForUserAction,
            scrollOffset: scrollOffset
        }, (0, _get_boundary_props.getBoundaryProps)(this.props.direction, scrollOffset, this.containerRef.current))
    };
    _proto.lock = function() {
        this.locked = true
    };
    _proto.unlock = function() {
        if (!this.props.disabled) {
            this.locked = false
        }
    };
    _proto.updateElementDimensions = function() {
        this.setContentDimensions(this.content());
        this.setContainerDimensions(this.containerRef.current)
    };
    _proto.setContainerDimensions = function(containerEl) {
        this.setState((function(__state_argument) {
            return {
                containerClientWidth: containerEl.clientWidth
            }
        }));
        this.setState((function(__state_argument) {
            return {
                containerClientHeight: containerEl.clientHeight
            }
        }))
    };
    _proto.setContentDimensions = function(contentEl) {
        var _this11 = this;
        this.setState((function(__state_argument) {
            return {
                contentClientWidth: contentEl.clientWidth
            }
        }));
        this.setState((function(__state_argument) {
            return {
                contentClientHeight: contentEl.clientHeight
            }
        }));
        this.setState((function(__state_argument) {
            return {
                contentScrollWidth: contentEl.scrollWidth
            }
        }));
        this.setState((function(__state_argument) {
            return {
                contentScrollHeight: contentEl.scrollHeight
            }
        }));
        if (this.props.forceGeneratePockets) {
            this.setState((function(__state_argument) {
                var _this11$topPocketRef;
                return {
                    topPocketHeight: (null === (_this11$topPocketRef = _this11.topPocketRef) || void 0 === _this11$topPocketRef ? void 0 : _this11$topPocketRef.current.clientHeight) || 0
                }
            }));
            this.setState((function(__state_argument) {
                var _this11$bottomPocketR;
                return {
                    bottomPocketHeight: (null === (_this11$bottomPocketR = _this11.bottomPocketRef) || void 0 === _this11$bottomPocketR ? void 0 : _this11$bottomPocketR.current.clientHeight) || 0
                }
            }))
        }
    };
    _proto.syncScrollbarsWithContent = function() {
        var _this$scrollOffset = this.scrollOffset(),
            left = _this$scrollOffset.left,
            top = _this$scrollOffset.top;
        this.setState((function(__state_argument) {
            return {
                hScrollLocation: -left
            }
        }));
        this.setState((function(__state_argument) {
            return {
                vScrollLocation: -top
            }
        }))
    };
    _proto.getInitEventData = function() {
        var _this12 = this;
        return {
            getDirection: function() {
                return _this12.tryGetAllowedDirection()
            },
            validate: function(event) {
                return _this12.validate(event)
            },
            isNative: true,
            scrollTarget: this.containerRef.current
        }
    };
    _proto.handleInit = function(event) {
        if (this.props.forceGeneratePockets && this.isSwipeDownStrategy) {
            var scrollTop = this.containerRef.current.scrollTop;
            if (this.state.topPocketState === _consts.TopPocketState.STATE_RELEASED && 0 === scrollTop) {
                this.initPageY = event.originalEvent.pageY;
                this.setPocketState(_consts.TopPocketState.STATE_TOUCHED)
            }
        }
    };
    _proto.handleMove = function(e) {
        if (this.locked) {
            e.cancel = true;
            return
        }
        if ((0, _type.isDefined)(this.tryGetAllowedDirection())) {
            e.originalEvent.isScrollingEvent = true
        }
        if (this.props.forceGeneratePockets && this.isSwipeDownStrategy) {
            this.deltaY = e.originalEvent.pageY - this.initPageY;
            if (this.state.topPocketState === _consts.TopPocketState.STATE_TOUCHED) {
                if (this.pullDownEnabled && this.deltaY > 0) {
                    this.setPocketState(_consts.TopPocketState.STATE_PULLED)
                } else {
                    this.complete()
                }
            }
            if (this.state.topPocketState === _consts.TopPocketState.STATE_PULLED) {
                e.preventDefault();
                this.movePullDown()
            }
        }
    };
    _proto.handleEnd = function() {
        if (this.props.forceGeneratePockets) {
            if (this.isSwipeDownStrategy) {
                if (this.isSwipeDown()) {
                    this.pullDownRefreshing()
                }
                this.complete()
            }
            if (this.isPullDownStrategy) {
                this.pullDownComplete()
            }
        }
    };
    _proto.handleStop = function() {
        if (this.props.forceGeneratePockets) {
            if (this.isSwipeDownStrategy) {
                this.complete()
            }
            if (this.isPullDownStrategy) {
                this.pullDownComplete()
            }
        }
    };
    _proto.pullDownComplete = function() {
        var _this13 = this;
        if (this.state.topPocketState === _consts.TopPocketState.STATE_READY) {
            this.setState((function(__state_argument) {
                return {
                    contentTranslateTop: _this13.state.topPocketHeight
                }
            }));
            this.clearRefreshTimer();
            this.refreshTimer = setTimeout((function() {
                _this13.pullDownRefreshing()
            }), 400)
        }
    };
    _proto.clearRefreshTimer = function() {
        clearTimeout(this.refreshTimer);
        this.refreshTimer = void 0
    };
    _proto.pullDownRefreshing = function() {
        var _this14 = this;
        if (this.state.topPocketState === _consts.TopPocketState.STATE_REFRESHING) {
            return
        }
        this.setPocketState(_consts.TopPocketState.STATE_REFRESHING);
        if (this.isSwipeDownStrategy) {
            this.setState((function(__state_argument) {
                return {
                    pullDownTranslateTop: _this14.getPullDownHeight()
                }
            }))
        }
        this.onPullDown()
    };
    _proto.movePullDown = function() {
        var pullDownHeight = this.getPullDownHeight();
        var top = Math.min(3 * pullDownHeight, this.deltaY + this.getPullDownStartPosition());
        var angle = 180 * top / pullDownHeight / 3;
        this.setState((function(__state_argument) {
            return {
                pullDownOpacity: 1
            }
        }));
        this.setState((function(__state_argument) {
            return {
                pullDownTranslateTop: top
            }
        }));
        this.setState((function(__state_argument) {
            return {
                pullDownIconAngle: angle
            }
        }))
    };
    _proto.getPullDownHeight = function() {
        return Math.round(.05 * this.scrollableRef.current.offsetHeight)
    };
    _proto.getPullDownStartPosition = function() {
        return -Math.round(1.5 * this.state.topPocketHeight)
    };
    _proto.complete = function() {
        if (this.state.topPocketState === _consts.TopPocketState.STATE_TOUCHED || this.state.topPocketState === _consts.TopPocketState.STATE_PULLED) {
            this.releaseState()
        }
    };
    _proto.releaseState = function() {
        this.setPocketState(_consts.TopPocketState.STATE_RELEASED);
        this.setState((function(__state_argument) {
            return {
                pullDownOpacity: 0
            }
        }))
    };
    _proto.isSwipeDown = function() {
        return this.pullDownEnabled && this.state.topPocketState === _consts.TopPocketState.STATE_PULLED && this.deltaY >= this.getPullDownHeight() - this.getPullDownStartPosition()
    };
    _proto.pulledDown = function() {
        var scrollTop = this.containerRef.current.scrollTop;
        return this.pullDownEnabled && scrollTop <= -this.state.topPocketHeight
    };
    _proto.isReachBottom = function() {
        var scrollTop = this.containerRef.current.scrollTop;
        return this.props.reachBottomEnabled && Math.round(-scrollTop - this.vScrollOffsetMax) <= 1
    };
    _proto.tryGetAllowedDirection = function() {
        var containerEl = this.containerRef.current;
        return (0, _get_allowed_direction.allowedDirection)(this.props.direction, (0, _get_scroll_top_max.getScrollTopMax)(containerEl), (0, _get_scroll_left_max.getScrollLeftMax)(containerEl), false)
    };
    _proto.isLocked = function() {
        return this.locked
    };
    _proto.isScrollingOutOfBound = function(event) {
        var delta = event.delta,
            shiftKey = event.shiftKey;
        var _this$containerRef$cu = this.containerRef.current,
            clientHeight = _this$containerRef$cu.clientHeight,
            clientWidth = _this$containerRef$cu.clientWidth,
            scrollHeight = _this$containerRef$cu.scrollHeight,
            scrollLeft = _this$containerRef$cu.scrollLeft,
            scrollTop = _this$containerRef$cu.scrollTop,
            scrollWidth = _this$containerRef$cu.scrollWidth;
        if (delta > 0) {
            return shiftKey ? !scrollLeft : !scrollTop
        }
        return shiftKey ? clientWidth >= scrollWidth - scrollLeft : clientHeight >= scrollHeight - scrollTop
    };
    _proto.content = function() {
        if (this.props.needScrollViewContentWrapper) {
            return this.scrollViewContentRef.current
        }
        return this.contentRef.current
    };
    _proto.container = function() {
        return this.containerRef.current
    };
    _proto.refresh = function() {
        this.setPocketState(_consts.TopPocketState.STATE_READY);
        this.startLoading();
        this.onPullDown()
    };
    _proto.release = function() {
        var _this15 = this;
        this.clearReleaseTimer();
        if (this.isPullDownStrategy) {
            if (this.state.topPocketState === _consts.TopPocketState.STATE_LOADING) {
                this.setPocketState(_consts.TopPocketState.STATE_RELEASED)
            }
        }
        this.releaseTimer = setTimeout((function() {
            if (_this15.isPullDownStrategy) {
                _this15.setState((function(__state_argument) {
                    return {
                        contentTranslateTop: 0
                    }
                }))
            }
            _this15.stateReleased();
            _this15.onRelease()
        }), this.isSwipeDownStrategy ? 800 : 400)
    };
    _proto.scrollHeight = function() {
        return this.content().offsetHeight
    };
    _proto.scrollWidth = function() {
        return this.content().offsetWidth
    };
    _proto.scrollOffset = function() {
        return {
            top: this.scrollTop(),
            left: this.scrollLeft()
        }
    };
    _proto.scrollTop = function() {
        return this.containerRef.current.scrollTop
    };
    _proto.scrollLeft = function() {
        var containerEl = this.containerRef.current;
        var scrollLeftMax = (0, _get_scroll_left_max.getScrollLeftMax)(containerEl);
        return (0, _normalize_offset_left.normalizeOffsetLeft)(containerEl.scrollLeft, scrollLeftMax, !!this.props.rtlEnabled)
    };
    _proto.clientHeight = function() {
        return this.containerRef.current.clientHeight
    };
    _proto.clientWidth = function() {
        return this.containerRef.current.clientWidth
    };
    _proto.validate = function(event) {
        if (this.isLocked()) {
            return false
        }
        this.updateHandler();
        return this.moveIsAllowed(event)
    };
    _proto.moveIsAllowed = function(event) {
        if (this.props.disabled || (0, _index.isDxMouseWheelEvent)(event) && this.isScrollingOutOfBound(event)) {
            return false
        }
        return (0, _type.isDefined)(this.tryGetAllowedDirection())
    };
    _proto.updateHandler = function() {
        this.updateElementDimensions();
        this.onUpdated()
    };
    _proto.scrollByLocation = function(location) {
        var containerEl = this.containerRef.current;
        if (this.direction.isVertical) {
            containerEl.scrollTop += location.top
        }
        if (this.direction.isHorizontal) {
            containerEl.scrollLeft += location.left
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoComponent.prototype.componentWillUpdate.call(this);
        if (this.props.direction !== nextProps.direction) {
            this.__getterCache.direction = void 0
        }
        if (this.props.forceGeneratePockets !== nextProps.forceGeneratePockets || this.props.refreshStrategy !== nextProps.refreshStrategy || this.state.contentTranslateTop !== nextState.contentTranslateTop) {
            this.__getterCache.contentStyles = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            containerClientWidth: this.state.containerClientWidth,
            containerClientHeight: this.state.containerClientHeight,
            contentClientWidth: this.state.contentClientWidth,
            contentClientHeight: this.state.contentClientHeight,
            contentScrollWidth: this.state.contentScrollWidth,
            contentScrollHeight: this.state.contentScrollHeight,
            topPocketHeight: this.state.topPocketHeight,
            bottomPocketHeight: this.state.bottomPocketHeight,
            scrolling: this.state.scrolling,
            topPocketState: this.state.topPocketState,
            isLoadPanelVisible: this.state.isLoadPanelVisible,
            pullDownTranslateTop: this.state.pullDownTranslateTop,
            pullDownIconAngle: this.state.pullDownIconAngle,
            pullDownOpacity: this.state.pullDownOpacity,
            contentTranslateTop: this.state.contentTranslateTop,
            vScrollLocation: this.state.vScrollLocation,
            hScrollLocation: this.state.hScrollLocation,
            wrapperRef: this.wrapperRef,
            contentRef: this.contentRef,
            scrollViewContentRef: this.scrollViewContentRef,
            containerRef: this.containerRef,
            scrollableRef: this.scrollableRef,
            topPocketRef: this.topPocketRef,
            bottomPocketRef: this.bottomPocketRef,
            vScrollbarRef: this.vScrollbarRef,
            hScrollbarRef: this.hScrollbarRef,
            clearReleaseTimer: this.clearReleaseTimer,
            onRelease: this.onRelease,
            onUpdated: this.onUpdated,
            startLoading: this.startLoading,
            finishLoading: this.finishLoading,
            setPocketState: this.setPocketState,
            handleScroll: this.handleScroll,
            handlePocketState: this.handlePocketState,
            pullDownReady: this.pullDownReady,
            onReachBottom: this.onReachBottom,
            onPullDown: this.onPullDown,
            stateReleased: this.stateReleased,
            getEventArgs: this.getEventArgs,
            lock: this.lock,
            unlock: this.unlock,
            fullScrollInactiveProp: this.fullScrollInactiveProp,
            updateElementDimensions: this.updateElementDimensions,
            setContainerDimensions: this.setContainerDimensions,
            setContentDimensions: this.setContentDimensions,
            syncScrollbarsWithContent: this.syncScrollbarsWithContent,
            getInitEventData: this.getInitEventData,
            handleInit: this.handleInit,
            handleMove: this.handleMove,
            handleEnd: this.handleEnd,
            handleStop: this.handleStop,
            pullDownComplete: this.pullDownComplete,
            clearRefreshTimer: this.clearRefreshTimer,
            pullDownRefreshing: this.pullDownRefreshing,
            movePullDown: this.movePullDown,
            getPullDownHeight: this.getPullDownHeight,
            getPullDownStartPosition: this.getPullDownStartPosition,
            complete: this.complete,
            releaseState: this.releaseState,
            isSwipeDownStrategy: this.isSwipeDownStrategy,
            isPullDownStrategy: this.isPullDownStrategy,
            isSwipeDown: this.isSwipeDown,
            pulledDown: this.pulledDown,
            isReachBottom: this.isReachBottom,
            tryGetAllowedDirection: this.tryGetAllowedDirection,
            isLocked: this.isLocked,
            isScrollingOutOfBound: this.isScrollingOutOfBound,
            cssClasses: this.cssClasses,
            direction: this.direction,
            pullDownEnabled: this.pullDownEnabled,
            contentStyles: this.contentStyles,
            contentHeight: this.contentHeight,
            contentWidth: this.contentWidth,
            hScrollOffsetMax: this.hScrollOffsetMax,
            vScrollOffsetMax: this.vScrollOffsetMax,
            restAttributes: this.restAttributes
        })
    };
    _createClass(ScrollableNative, [{
        key: "fullScrollInactiveProp",
        get: function() {
            return this.props.direction === _consts.DIRECTION_HORIZONTAL ? "scrollTop" : "scrollLeft"
        }
    }, {
        key: "isSwipeDownStrategy",
        get: function() {
            return "swipeDown" === this.props.refreshStrategy
        }
    }, {
        key: "isPullDownStrategy",
        get: function() {
            return "pullDown" === this.props.refreshStrategy
        }
    }, {
        key: "cssClasses",
        get: function() {
            var _classesMap;
            var _this$props5 = this.props,
                classes = _this$props5.classes,
                direction = _this$props5.direction,
                disabled = _this$props5.disabled,
                showScrollbar = _this$props5.showScrollbar;
            var classesMap = (_classesMap = {}, _defineProperty(_classesMap, "dx-scrollable dx-scrollable-native dx-scrollable-native-".concat(_devices.default.real().platform), true), _defineProperty(_classesMap, "dx-scrollable-".concat(direction), true), _defineProperty(_classesMap, _consts.SCROLLABLE_DISABLED_CLASS, !!disabled), _defineProperty(_classesMap, _consts.SCROLLABLE_SCROLLBAR_SIMULATED, "never" !== showScrollbar && this.props.useSimulatedScrollbar), _defineProperty(_classesMap, _consts.SCROLLABLE_SCROLLBARS_HIDDEN, "never" === showScrollbar), _defineProperty(_classesMap, String(classes), !!classes), _classesMap);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "direction",
        get: function() {
            var _this16 = this;
            if (void 0 !== this.__getterCache.direction) {
                return this.__getterCache.direction
            }
            return this.__getterCache.direction = new _scroll_direction.ScrollDirection(_this16.props.direction)
        }
    }, {
        key: "pullDownEnabled",
        get: function() {
            return this.props.pullDownEnabled && "generic" !== _devices.default.real().platform
        }
    }, {
        key: "contentStyles",
        get: function() {
            var _this17 = this;
            if (void 0 !== this.__getterCache.contentStyles) {
                return this.__getterCache.contentStyles
            }
            return this.__getterCache.contentStyles = function() {
                if (_this17.props.forceGeneratePockets && _this17.isPullDownStrategy) {
                    return {
                        transform: "translate(0px, ".concat(_this17.state.contentTranslateTop, "px)")
                    }
                }
                return
            }()
        }
    }, {
        key: "contentHeight",
        get: function() {
            var _this$contentRef;
            return "hidden" === (0, _get_element_style.getElementOverflowY)(null === (_this$contentRef = this.contentRef) || void 0 === _this$contentRef ? void 0 : _this$contentRef.current) ? this.state.contentClientHeight : Math.max(this.state.contentScrollHeight, this.state.contentClientHeight)
        }
    }, {
        key: "contentWidth",
        get: function() {
            var _this$contentRef2;
            return "hidden" === (0, _get_element_style.getElementOverflowX)(null === (_this$contentRef2 = this.contentRef) || void 0 === _this$contentRef2 ? void 0 : _this$contentRef2.current) ? this.state.contentClientWidth : Math.max(this.state.contentScrollWidth, this.state.contentClientWidth)
        }
    }, {
        key: "hScrollOffsetMax",
        get: function() {
            return -Math.max(this.contentWidth - this.state.containerClientWidth, 0)
        }
    }, {
        key: "vScrollOffsetMax",
        get: function() {
            return -Math.max(this.contentHeight - this.state.containerClientHeight, 0)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props6 = this.props,
                restProps = (_this$props6.addWidgetClass, _this$props6.aria, _this$props6.bounceEnabled, _this$props6.children, _this$props6.classes, _this$props6.direction, _this$props6.disabled, _this$props6.forceGeneratePockets, _this$props6.height, _this$props6.needRenderScrollbars, _this$props6.needScrollViewContentWrapper, _this$props6.needScrollViewLoadPanel, _this$props6.onPullDown, _this$props6.onReachBottom, _this$props6.onScroll, _this$props6.onUpdated, _this$props6.pullDownEnabled, _this$props6.pulledDownText, _this$props6.pullingDownText, _this$props6.reachBottomEnabled, _this$props6.reachBottomText, _this$props6.refreshStrategy, _this$props6.refreshingText, _this$props6.rtlEnabled, _this$props6.scrollByContent, _this$props6.showScrollbar, _this$props6.useSimulatedScrollbar, _this$props6.visible, _this$props6.width, _objectWithoutProperties(_this$props6, _excluded));
            return restProps
        }
    }]);
    return ScrollableNative
}(_inferno2.InfernoComponent);
exports.ScrollableNative = ScrollableNative;
ScrollableNative.defaultProps = _native_strategy_props.ScrollableNativeProps;
