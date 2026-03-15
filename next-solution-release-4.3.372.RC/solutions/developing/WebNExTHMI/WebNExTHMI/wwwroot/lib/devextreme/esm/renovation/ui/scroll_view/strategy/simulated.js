/**
 * DevExtreme (esm/renovation/ui/scroll_view/strategy/simulated.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["addWidgetClass", "aria", "bounceEnabled", "children", "classes", "direction", "disabled", "forceGeneratePockets", "height", "inertiaEnabled", "needRenderScrollbars", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "onBounce", "onEnd", "onPullDown", "onReachBottom", "onScroll", "onStart", "onUpdated", "onVisibilityChange", "pullDownEnabled", "pulledDownText", "pullingDownText", "reachBottomEnabled", "reachBottomText", "refreshStrategy", "refreshingText", "rtlEnabled", "scrollByContent", "scrollByThumb", "scrollLocationChange", "showScrollbar", "useKeyboard", "visible", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import "../../../../events/gesture/emitter.gesture.scroll";
import {
    subscribeToScrollEvent,
    subscribeToScrollInitEvent,
    subscribeToDXScrollStartEvent,
    subscribeToDXScrollMoveEvent,
    subscribeToDXScrollEndEvent,
    subscribeToDXScrollStopEvent,
    subscribeToDXScrollCancelEvent,
    subscribeToMouseEnterEvent,
    subscribeToMouseLeaveEvent,
    subscribeToDXPointerDownEvent,
    subscribeToDXPointerUpEvent
} from "../../../utils/subscribe_to_event";
import {
    ScrollViewLoadPanel
} from "../internal/load_panel";
import {
    AnimatedScrollbar
} from "../scrollbar/animated_scrollbar";
import {
    Widget
} from "../../common/widget";
import {
    combineClasses
} from "../../../utils/combine_classes";
import {
    getOffsetDistance
} from "../utils/get_offset_distance";
import {
    getBoundaryProps
} from "../utils/get_boundary_props";
import {
    permissibleWheelDirection
} from "../utils/get_permissible_wheel_direction";
import {
    isDxMouseWheelEvent,
    normalizeKeyName,
    isCommandKeyPressed
} from "../../../../events/utils/index";
import {
    isDefined
} from "../../../../core/utils/type";
import {
    ScrollableSimulatedProps
} from "../common/simulated_strategy_props";
import eventsEngine from "../../../../events/core/events_engine";
import {
    inRange
} from "../../../../core/utils/math";
import {
    ScrollDirection
} from "../utils/scroll_direction";
import {
    DIRECTION_VERTICAL,
    DIRECTION_HORIZONTAL,
    SCROLLABLE_SIMULATED_CLASS,
    SCROLLABLE_CONTAINER_CLASS,
    SCROLLABLE_CONTENT_CLASS,
    SCROLLABLE_WRAPPER_CLASS,
    SCROLLVIEW_CONTENT_CLASS,
    SCROLLABLE_DISABLED_CLASS,
    SCROLLABLE_SCROLLBARS_ALWAYSVISIBLE,
    SCROLL_LINE_HEIGHT,
    SCROLLABLE_SCROLLBAR_CLASS,
    KEY_CODES,
    VALIDATE_WHEEL_TIMEOUT,
    TopPocketState,
    DIRECTION_BOTH
} from "../common/consts";
import {
    getElementOffset
} from "../../../utils/get_element_offset";
import {
    getElementPadding,
    getElementOverflowX,
    getElementOverflowY
} from "../utils/get_element_style";
import {
    TopPocket
} from "../internal/pocket/top";
import {
    BottomPocket
} from "../internal/pocket/bottom";
import {
    getDevicePixelRatio
} from "../utils/get_device_pixel_ratio";
import {
    isElementVisible
} from "../utils/is_element_visible";
import {
    clampIntoRange
} from "../utils/clamp_into_range";
import {
    allowedDirection
} from "../utils/get_allowed_direction";
import {
    subscribeToResize
} from "../utils/subscribe_to_resize";
import domAdapter from "../../../../core/dom_adapter";
export var viewFunction = viewModel => {
    var {
        active: active,
        bottomPocketHeight: bottomPocketHeight,
        bottomPocketRef: bottomPocketRef,
        containerClientHeight: containerClientHeight,
        containerClientWidth: containerClientWidth,
        containerHasSizes: containerHasSizes,
        containerRef: containerRef,
        containerStyles: containerStyles,
        contentHeight: contentHeight,
        contentPaddingBottom: contentPaddingBottom,
        contentRef: contentRef,
        contentStyles: contentStyles,
        contentWidth: contentWidth,
        cssClasses: cssClasses,
        direction: direction,
        hScrollLocation: hScrollLocation,
        hScrollOffsetMax: hScrollOffsetMax,
        hScrollbarRef: hScrollbarRef,
        handleKeyDown: handleKeyDown,
        hovered: hovered,
        isLoadPanelVisible: isLoadPanelVisible,
        lock: lock,
        onBounce: onBounce,
        onEnd: onEnd,
        onPullDown: onPullDown,
        onReachBottom: onReachBottom,
        onVisibilityChangeHandler: onVisibilityChangeHandler,
        props: {
            aria: aria,
            bounceEnabled: bounceEnabled,
            children: children,
            forceGeneratePockets: forceGeneratePockets,
            height: height,
            inertiaEnabled: inertiaEnabled,
            needRenderScrollbars: needRenderScrollbars,
            needScrollViewContentWrapper: needScrollViewContentWrapper,
            needScrollViewLoadPanel: needScrollViewLoadPanel,
            pullDownEnabled: pullDownEnabled,
            pulledDownText: pulledDownText,
            pullingDownText: pullingDownText,
            reachBottomEnabled: reachBottomEnabled,
            reachBottomText: reachBottomText,
            refreshStrategy: refreshStrategy,
            refreshingText: refreshingText,
            rtlEnabled: rtlEnabled,
            scrollByThumb: scrollByThumb,
            showScrollbar: showScrollbar,
            useKeyboard: useKeyboard,
            visible: visible,
            width: width
        },
        pulledDown: pulledDown,
        restAttributes: restAttributes,
        scrollLocationChange: scrollLocationChange,
        scrollViewContentRef: scrollViewContentRef,
        scrollableRef: scrollableRef,
        scrolling: scrolling,
        topPocketRef: topPocketRef,
        topPocketState: topPocketState,
        unlock: unlock,
        vScrollLocation: vScrollLocation,
        vScrollOffsetMax: vScrollOffsetMax,
        vScrollOffsetMin: vScrollOffsetMin,
        vScrollbarRef: vScrollbarRef,
        wrapperRef: wrapperRef
    } = viewModel;
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        rootElementRef: scrollableRef,
        focusStateEnabled: useKeyboard,
        aria: aria,
        addWidgetClass: false,
        classes: cssClasses,
        rtlEnabled: rtlEnabled,
        height: height,
        width: width,
        visible: visible,
        onVisibilityChange: onVisibilityChangeHandler
    }, restAttributes, {
        onKeyDown: useKeyboard ? handleKeyDown : void 0,
        children: [createVNode(1, "div", SCROLLABLE_WRAPPER_CLASS, createVNode(1, "div", SCROLLABLE_CONTAINER_CLASS, [createVNode(1, "div", SCROLLABLE_CONTENT_CLASS, [forceGeneratePockets && createComponentVNode(2, TopPocket, {
            topPocketRef: topPocketRef,
            pullingDownText: pullingDownText,
            pulledDownText: pulledDownText,
            refreshingText: refreshingText,
            refreshStrategy: refreshStrategy,
            pocketState: topPocketState,
            visible: !!pullDownEnabled
        }), needScrollViewContentWrapper ? createVNode(1, "div", SCROLLVIEW_CONTENT_CLASS, children, 0, null, null, scrollViewContentRef) : children, forceGeneratePockets && createComponentVNode(2, BottomPocket, {
            bottomPocketRef: bottomPocketRef,
            reachBottomText: reachBottomText,
            visible: !!reachBottomEnabled
        })], 0, {
            style: normalizeStyles(contentStyles)
        }, null, contentRef), needRenderScrollbars && direction.isHorizontal && createComponentVNode(2, AnimatedScrollbar, {
            direction: "horizontal",
            contentSize: contentWidth,
            containerSize: containerClientWidth,
            visible: hovered || scrolling || active,
            minOffset: 0,
            maxOffset: hScrollOffsetMax,
            scrollLocation: hScrollLocation,
            scrollLocationChange: scrollLocationChange,
            scrollByThumb: scrollByThumb,
            bounceEnabled: bounceEnabled,
            showScrollbar: showScrollbar,
            inertiaEnabled: inertiaEnabled,
            onBounce: onBounce,
            onEnd: onEnd,
            containerHasSizes: containerHasSizes,
            rtlEnabled: rtlEnabled
        }, null, hScrollbarRef), needRenderScrollbars && direction.isVertical && createComponentVNode(2, AnimatedScrollbar, {
            direction: "vertical",
            contentSize: contentHeight,
            containerSize: containerClientHeight,
            visible: hovered || scrolling || active,
            minOffset: vScrollOffsetMin,
            maxOffset: vScrollOffsetMax,
            scrollLocation: vScrollLocation,
            scrollLocationChange: scrollLocationChange,
            scrollByThumb: scrollByThumb,
            bounceEnabled: bounceEnabled,
            showScrollbar: showScrollbar,
            inertiaEnabled: inertiaEnabled,
            onBounce: onBounce,
            onEnd: onEnd,
            containerHasSizes: containerHasSizes,
            forceGeneratePockets: forceGeneratePockets,
            bottomPocketSize: bottomPocketHeight,
            contentPaddingBottom: contentPaddingBottom,
            pulledDown: pulledDown,
            onPullDown: onPullDown,
            onReachBottom: onReachBottom,
            pullDownEnabled: pullDownEnabled,
            reachBottomEnabled: reachBottomEnabled,
            onLock: lock,
            onUnlock: unlock
        }, null, vScrollbarRef)], 0, {
            style: normalizeStyles(containerStyles)
        }, null, containerRef), 2, null, null, wrapperRef), needScrollViewLoadPanel && createComponentVNode(2, ScrollViewLoadPanel, {
            targetElement: scrollableRef,
            refreshingText: refreshingText,
            visible: isLoadPanelVisible
        })]
    })))
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class ScrollableSimulated extends InfernoComponent {
    constructor(props) {
        super(props);
        this.scrollableRef = infernoCreateRef();
        this.topPocketRef = infernoCreateRef();
        this.bottomPocketRef = infernoCreateRef();
        this.wrapperRef = infernoCreateRef();
        this.contentRef = infernoCreateRef();
        this.scrollViewContentRef = infernoCreateRef();
        this.containerRef = infernoCreateRef();
        this.vScrollbarRef = infernoCreateRef();
        this.hScrollbarRef = infernoCreateRef();
        this.locked = false;
        this.loadingIndicatorEnabled = true;
        this.validDirections = {};
        this.endActionDirections = {
            horizontal: false,
            vertical: false
        };
        this.savedScrollOffset = {
            scrollTop: 0,
            scrollLeft: 0
        };
        this.__getterCache = {};
        this.state = {
            active: false,
            hovered: false,
            scrolling: false,
            containerClientWidth: 0,
            containerClientHeight: 0,
            contentScrollWidth: 0,
            contentScrollHeight: 0,
            contentClientWidth: 0,
            contentClientHeight: 0,
            contentPaddingBottom: 0,
            topPocketHeight: 0,
            bottomPocketHeight: 0,
            topPocketState: TopPocketState.STATE_RELEASED,
            isLoadPanelVisible: false,
            vScrollLocation: 0,
            hScrollLocation: 0
        };
        this.content = this.content.bind(this);
        this.container = this.container.bind(this);
        this.refresh = this.refresh.bind(this);
        this.release = this.release.bind(this);
        this.updateHandler = this.updateHandler.bind(this);
        this.scrollHeight = this.scrollHeight.bind(this);
        this.scrollWidth = this.scrollWidth.bind(this);
        this.scrollOffset = this.scrollOffset.bind(this);
        this.scrollTop = this.scrollTop.bind(this);
        this.scrollLeft = this.scrollLeft.bind(this);
        this.clientHeight = this.clientHeight.bind(this);
        this.clientWidth = this.clientWidth.bind(this);
        this.disposeWheelTimer = this.disposeWheelTimer.bind(this);
        this.scrollEffect = this.scrollEffect.bind(this);
        this.startEffect = this.startEffect.bind(this);
        this.initEffect = this.initEffect.bind(this);
        this.moveEffect = this.moveEffect.bind(this);
        this.endEffect = this.endEffect.bind(this);
        this.stopEffect = this.stopEffect.bind(this);
        this.cancelEffect = this.cancelEffect.bind(this);
        this.pointerDownEffect = this.pointerDownEffect.bind(this);
        this.pointerUpEffect = this.pointerUpEffect.bind(this);
        this.mouseEnterEffect = this.mouseEnterEffect.bind(this);
        this.mouseLeaveEffect = this.mouseLeaveEffect.bind(this);
        this.validate = this.validate.bind(this);
        this.moveIsAllowed = this.moveIsAllowed.bind(this);
        this.effectDisabledState = this.effectDisabledState.bind(this);
        this.effectResetInactiveState = this.effectResetInactiveState.bind(this);
        this.updatePocketState = this.updatePocketState.bind(this);
        this.subscribeTopPocketToResize = this.subscribeTopPocketToResize.bind(this);
        this.subscribeBottomPocketToResize = this.subscribeBottomPocketToResize.bind(this);
        this.subscribeContainerToResize = this.subscribeContainerToResize.bind(this);
        this.subscribeContentToResize = this.subscribeContentToResize.bind(this);
        this.updateDimensions = this.updateDimensions.bind(this);
        this.scrollByLocation = this.scrollByLocation.bind(this);
        this.handleScroll = this.handleScroll.bind(this);
        this.syncScrollbarsWithContent = this.syncScrollbarsWithContent.bind(this);
        this.startLoading = this.startLoading.bind(this);
        this.finishLoading = this.finishLoading.bind(this);
        this.getEventArgs = this.getEventArgs.bind(this);
        this.getInitEventData = this.getInitEventData.bind(this);
        this.onStart = this.onStart.bind(this);
        this.onEnd = this.onEnd.bind(this);
        this.restoreEndActionDirections = this.restoreEndActionDirections.bind(this);
        this.onUpdated = this.onUpdated.bind(this);
        this.onBounce = this.onBounce.bind(this);
        this.onPullDown = this.onPullDown.bind(this);
        this.onRelease = this.onRelease.bind(this);
        this.onReachBottom = this.onReachBottom.bind(this);
        this.scrollLocationChange = this.scrollLocationChange.bind(this);
        this.onScroll = this.onScroll.bind(this);
        this.handleInit = this.handleInit.bind(this);
        this.handleStart = this.handleStart.bind(this);
        this.handleMove = this.handleMove.bind(this);
        this.handleEnd = this.handleEnd.bind(this);
        this.handleStop = this.handleStop.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
        this.isCrossThumbScrolling = this.isCrossThumbScrolling.bind(this);
        this.adjustDistance = this.adjustDistance.bind(this);
        this.suppressDirections = this.suppressDirections.bind(this);
        this.validateEvent = this.validateEvent.bind(this);
        this.prepareDirections = this.prepareDirections.bind(this);
        this.isContent = this.isContent.bind(this);
        this.tryGetAllowedDirection = this.tryGetAllowedDirection.bind(this);
        this.isLocked = this.isLocked.bind(this);
        this.validateWheel = this.validateWheel.bind(this);
        this.clearWheelValidationTimer = this.clearWheelValidationTimer.bind(this);
        this.validateMove = this.validateMove.bind(this);
        this.handleKeyDown = this.handleKeyDown.bind(this);
        this.scrollByLine = this.scrollByLine.bind(this);
        this.scrollByPage = this.scrollByPage.bind(this);
        this.scrollByKey = this.scrollByKey.bind(this);
        this.lock = this.lock.bind(this);
        this.unlock = this.unlock.bind(this);
        this.onVisibilityChangeHandler = this.onVisibilityChangeHandler.bind(this);
        this.updateElementDimensions = this.updateElementDimensions.bind(this);
        this.setTopPocketDimensions = this.setTopPocketDimensions.bind(this);
        this.setBottomPocketDimensions = this.setBottomPocketDimensions.bind(this);
        this.setContentDimensions = this.setContentDimensions.bind(this);
        this.setContainerDimensions = this.setContainerDimensions.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.disposeWheelTimer, []), new InfernoEffect(this.scrollEffect, [this.state.scrolling, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.props.rtlEnabled, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.onScroll, this.props.direction, this.state.topPocketHeight]), new InfernoEffect(this.startEffect, [this.props.onStart, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.direction, this.state.topPocketHeight]), new InfernoEffect(this.initEffect, [this.props.direction, this.props.scrollByThumb, this.props.scrollByContent, this.props.bounceEnabled, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.forceGeneratePockets, this.props.pullDownEnabled, this.props.reachBottomEnabled, this.props.needScrollViewContentWrapper, this.props.onUpdated, this.state.topPocketHeight, this.props.disabled]), new InfernoEffect(this.moveEffect, []), new InfernoEffect(this.endEffect, []), new InfernoEffect(this.stopEffect, []), new InfernoEffect(this.cancelEffect, []), new InfernoEffect(this.pointerDownEffect, []), new InfernoEffect(this.pointerUpEffect, []), new InfernoEffect(this.mouseEnterEffect, [this.props.disabled, this.props.showScrollbar]), new InfernoEffect(this.mouseLeaveEffect, [this.props.disabled, this.props.showScrollbar]), new InfernoEffect(this.effectDisabledState, [this.props.disabled]), new InfernoEffect(this.effectResetInactiveState, [this.props.direction]), new InfernoEffect(this.updatePocketState, [this.props.forceGeneratePockets, this.props.pullDownEnabled, this.props.bounceEnabled, this.state.topPocketHeight, this.state.vScrollLocation]), new InfernoEffect(this.subscribeTopPocketToResize, []), new InfernoEffect(this.subscribeBottomPocketToResize, []), new InfernoEffect(this.subscribeContainerToResize, []), new InfernoEffect(this.subscribeContentToResize, []), new InfernoEffect(this.updateDimensions, [])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5, _this$_effects$6, _this$_effects$7, _this$_effects$8, _this$_effects$9, _this$_effects$10, _this$_effects$11, _this$_effects$12;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.scrolling, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.props.rtlEnabled, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.onScroll, this.props.direction, this.state.topPocketHeight]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.onStart, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.direction, this.state.topPocketHeight]);
        null === (_this$_effects$3 = this._effects[3]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.direction, this.props.scrollByThumb, this.props.scrollByContent, this.props.bounceEnabled, this.state.contentClientHeight, this.state.contentScrollHeight, this.state.containerClientHeight, this.state.contentClientWidth, this.state.contentScrollWidth, this.state.containerClientWidth, this.props.forceGeneratePockets, this.props.pullDownEnabled, this.props.reachBottomEnabled, this.props.needScrollViewContentWrapper, this.props.onUpdated, this.state.topPocketHeight, this.props.disabled]);
        null === (_this$_effects$4 = this._effects[4]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([]);
        null === (_this$_effects$5 = this._effects[5]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([]);
        null === (_this$_effects$6 = this._effects[6]) || void 0 === _this$_effects$6 ? void 0 : _this$_effects$6.update([]);
        null === (_this$_effects$7 = this._effects[7]) || void 0 === _this$_effects$7 ? void 0 : _this$_effects$7.update([]);
        null === (_this$_effects$8 = this._effects[10]) || void 0 === _this$_effects$8 ? void 0 : _this$_effects$8.update([this.props.disabled, this.props.showScrollbar]);
        null === (_this$_effects$9 = this._effects[11]) || void 0 === _this$_effects$9 ? void 0 : _this$_effects$9.update([this.props.disabled, this.props.showScrollbar]);
        null === (_this$_effects$10 = this._effects[12]) || void 0 === _this$_effects$10 ? void 0 : _this$_effects$10.update([this.props.disabled]);
        null === (_this$_effects$11 = this._effects[13]) || void 0 === _this$_effects$11 ? void 0 : _this$_effects$11.update([this.props.direction]);
        null === (_this$_effects$12 = this._effects[14]) || void 0 === _this$_effects$12 ? void 0 : _this$_effects$12.update([this.props.forceGeneratePockets, this.props.pullDownEnabled, this.props.bounceEnabled, this.state.topPocketHeight, this.state.vScrollLocation])
    }
    disposeWheelTimer() {
        return () => this.clearWheelValidationTimer()
    }
    scrollEffect() {
        return subscribeToScrollEvent(this.containerRef.current, () => {
            this.handleScroll()
        })
    }
    startEffect() {
        return subscribeToDXScrollStartEvent(this.wrapperRef.current, event => {
            this.handleStart(event)
        })
    }
    initEffect() {
        return subscribeToScrollInitEvent(this.wrapperRef.current, event => {
            this.handleInit(event)
        }, this.getInitEventData())
    }
    moveEffect() {
        return subscribeToDXScrollMoveEvent(this.wrapperRef.current, event => {
            this.handleMove(event)
        })
    }
    endEffect() {
        return subscribeToDXScrollEndEvent(this.wrapperRef.current, event => {
            this.handleEnd(event)
        })
    }
    stopEffect() {
        return subscribeToDXScrollStopEvent(this.wrapperRef.current, () => {
            this.handleStop()
        })
    }
    cancelEffect() {
        return subscribeToDXScrollCancelEvent(this.wrapperRef.current, event => {
            this.handleCancel(event)
        })
    }
    pointerDownEffect() {
        return subscribeToDXPointerDownEvent(this.wrapperRef.current, () => {
            this.setState(__state_argument => ({
                active: true
            }))
        })
    }
    pointerUpEffect() {
        return subscribeToDXPointerUpEvent(domAdapter.getDocument(), () => {
            this.setState(__state_argument => ({
                active: false
            }))
        })
    }
    mouseEnterEffect() {
        if (this.isHoverable) {
            return subscribeToMouseEnterEvent(this.scrollableRef.current, () => {
                this.setState(__state_argument => ({
                    hovered: true
                }))
            })
        }
        return
    }
    mouseLeaveEffect() {
        if (this.isHoverable) {
            return subscribeToMouseLeaveEvent(this.scrollableRef.current, () => {
                this.setState(__state_argument => ({
                    hovered: false
                }))
            })
        }
        return
    }
    effectDisabledState() {
        if (this.props.disabled) {
            this.lock()
        } else {
            this.unlock()
        }
    }
    effectResetInactiveState() {
        if (this.direction.isBoth) {
            return
        }
        var inactiveScrollProp = !this.direction.isVertical ? "scrollTop" : "scrollLeft";
        this.scrollLocationChange({
            fullScrollProp: inactiveScrollProp,
            location: 0,
            needFireScroll: false
        })
    }
    updatePocketState() {
        if (this.props.forceGeneratePockets) {
            this.setState(__state_argument => ({
                topPocketState: this.pulledDown ? TopPocketState.STATE_READY : TopPocketState.STATE_RELEASED
            }))
        }
    }
    subscribeTopPocketToResize() {
        var _this$topPocketRef;
        return subscribeToResize(null === (_this$topPocketRef = this.topPocketRef) || void 0 === _this$topPocketRef ? void 0 : _this$topPocketRef.current, element => {
            this.setTopPocketDimensions(element)
        })
    }
    subscribeBottomPocketToResize() {
        var _this$bottomPocketRef;
        return subscribeToResize(null === (_this$bottomPocketRef = this.bottomPocketRef) || void 0 === _this$bottomPocketRef ? void 0 : _this$bottomPocketRef.current, element => {
            this.setBottomPocketDimensions(element)
        })
    }
    subscribeContainerToResize() {
        return subscribeToResize(this.containerRef.current, element => {
            this.setContainerDimensions(element)
        })
    }
    subscribeContentToResize() {
        return subscribeToResize(this.content(), element => {
            this.setContentDimensions(element)
        })
    }
    updateDimensions() {
        this.updateElementDimensions()
    }
    get pulledDown() {
        return this.props.pullDownEnabled && this.props.bounceEnabled && this.state.topPocketHeight > 0 && this.state.vScrollLocation - this.state.topPocketHeight >= 0
    }
    handleScroll() {
        var _this$props$onScroll, _this$props;
        if (!this.state.scrolling) {
            this.syncScrollbarsWithContent()
        }
        null === (_this$props$onScroll = (_this$props = this.props).onScroll) || void 0 === _this$props$onScroll ? void 0 : _this$props$onScroll.call(_this$props, this.getEventArgs())
    }
    syncScrollbarsWithContent() {
        var {
            scrollLeft: scrollLeft,
            scrollTop: scrollTop
        } = this.containerRef.current;
        this.scrollLocationChange({
            fullScrollProp: "scrollTop",
            location: -clampIntoRange(-scrollTop, 0, this.vScrollOffsetMax),
            needFireScroll: false
        });
        if (!this.props.rtlEnabled) {
            this.scrollLocationChange({
                fullScrollProp: "scrollLeft",
                location: -clampIntoRange(-scrollLeft, 0, this.hScrollOffsetMax),
                needFireScroll: false
            })
        }
    }
    startLoading() {
        if (this.loadingIndicatorEnabled && isElementVisible(this.scrollableRef.current)) {
            this.setState(__state_argument => ({
                isLoadPanelVisible: true
            }))
        }
        this.lock()
    }
    finishLoading() {
        this.setState(__state_argument => ({
            isLoadPanelVisible: false
        }));
        this.unlock()
    }
    getEventArgs() {
        var scrollOffset = this.scrollOffset();
        return _extends({
            event: this.eventForUserAction,
            scrollOffset: scrollOffset
        }, getBoundaryProps(this.props.direction, scrollOffset, this.containerRef.current, this.state.topPocketHeight))
    }
    getInitEventData() {
        return {
            getDirection: event => this.tryGetAllowedDirection(event),
            validate: event => this.validate(event),
            isNative: false,
            scrollTarget: this.containerRef.current
        }
    }
    onStart() {
        var _this$props$onStart, _this$props2;
        null === (_this$props$onStart = (_this$props2 = this.props).onStart) || void 0 === _this$props$onStart ? void 0 : _this$props$onStart.call(_this$props2, this.getEventArgs())
    }
    onEnd(direction) {
        if (this.direction.isBoth) {
            this.endActionDirections[direction] = true;
            var {
                horizontal: horizontal,
                vertical: vertical
            } = this.endActionDirections;
            if (horizontal && vertical) {
                var _this$props$onEnd, _this$props3;
                this.restoreEndActionDirections();
                this.setState(__state_argument => ({
                    scrolling: false
                }));
                null === (_this$props$onEnd = (_this$props3 = this.props).onEnd) || void 0 === _this$props$onEnd ? void 0 : _this$props$onEnd.call(_this$props3, this.getEventArgs())
            }
        } else {
            var _this$props$onEnd2, _this$props4;
            this.setState(__state_argument => ({
                scrolling: false
            }));
            null === (_this$props$onEnd2 = (_this$props4 = this.props).onEnd) || void 0 === _this$props$onEnd2 ? void 0 : _this$props$onEnd2.call(_this$props4, this.getEventArgs())
        }
    }
    restoreEndActionDirections() {
        this.endActionDirections[DIRECTION_HORIZONTAL] = false;
        this.endActionDirections[DIRECTION_VERTICAL] = false
    }
    onUpdated() {
        var _this$props$onUpdated, _this$props5;
        null === (_this$props$onUpdated = (_this$props5 = this.props).onUpdated) || void 0 === _this$props$onUpdated ? void 0 : _this$props$onUpdated.call(_this$props5, this.getEventArgs())
    }
    onBounce() {
        var _this$props$onBounce, _this$props6;
        null === (_this$props$onBounce = (_this$props6 = this.props).onBounce) || void 0 === _this$props$onBounce ? void 0 : _this$props$onBounce.call(_this$props6, this.getEventArgs())
    }
    onPullDown() {
        var _this$props$onPullDow, _this$props7;
        this.setState(__state_argument => ({
            topPocketState: TopPocketState.STATE_REFRESHING
        }));
        this.loadingIndicatorEnabled = false;
        this.startLoading();
        null === (_this$props$onPullDow = (_this$props7 = this.props).onPullDown) || void 0 === _this$props$onPullDow ? void 0 : _this$props$onPullDow.call(_this$props7, {})
    }
    onRelease() {
        this.setState(__state_argument => ({
            topPocketState: TopPocketState.STATE_RELEASED
        }));
        this.loadingIndicatorEnabled = true;
        this.finishLoading();
        this.updateHandler()
    }
    onReachBottom() {
        var _this$props$onReachBo, _this$props8;
        this.loadingIndicatorEnabled = false;
        this.startLoading();
        null === (_this$props$onReachBo = (_this$props8 = this.props).onReachBottom) || void 0 === _this$props$onReachBo ? void 0 : _this$props$onReachBo.call(_this$props8, {})
    }
    scrollLocationChange(eventData) {
        if (!isElementVisible(this.scrollableRef.current)) {
            return
        }
        var {
            fullScrollProp: fullScrollProp,
            location: location,
            needFireScroll: needFireScroll
        } = eventData;
        this.containerRef.current[fullScrollProp] = location;
        if ("scrollLeft" === fullScrollProp) {
            this.setState(__state_argument => ({
                hScrollLocation: -location
            }))
        } else {
            this.setState(__state_argument => ({
                vScrollLocation: -location
            }))
        }
        var scrollDelta = Math.abs(this.savedScrollOffset[fullScrollProp] - location);
        this.savedScrollOffset[fullScrollProp] = location;
        if (needFireScroll && scrollDelta >= 1) {
            this.onScroll()
        }
    }
    get hScrollOffsetMax() {
        return -Math.max(this.contentWidth - this.state.containerClientWidth, 0)
    }
    get vScrollOffsetMax() {
        return -Math.max(this.contentHeight - this.state.containerClientHeight, 0)
    }
    get vScrollOffsetMin() {
        return this.pulledDown && this.state.topPocketState !== TopPocketState.STATE_RELEASED ? this.state.topPocketHeight : 0
    }
    onScroll() {
        eventsEngine.triggerHandler(this.containerRef.current, {
            type: "scroll"
        })
    }
    handleInit(event) {
        var _this$hScrollbarRef$c, _this$vScrollbarRef$c;
        this.suppressDirections(event);
        this.restoreEndActionDirections();
        this.eventForUserAction = event;
        var crossThumbScrolling = this.isCrossThumbScrolling(event);
        var {
            left: left,
            top: top
        } = getElementOffset(this.scrollableRef.current);
        null === (_this$hScrollbarRef$c = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c ? void 0 : _this$hScrollbarRef$c.initHandler(event, crossThumbScrolling, left);
        null === (_this$vScrollbarRef$c = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c ? void 0 : _this$vScrollbarRef$c.initHandler(event, crossThumbScrolling, top)
    }
    handleStart(event) {
        this.setState(__state_argument => ({
            scrolling: true
        }));
        this.eventForUserAction = event;
        this.onStart()
    }
    handleMove(e) {
        var _e$preventDefault, _this$hScrollbarRef$c2, _this$vScrollbarRef$c2;
        if (this.isLocked()) {
            e.cancel = true;
            return
        }
        null === (_e$preventDefault = e.preventDefault) || void 0 === _e$preventDefault ? void 0 : _e$preventDefault.call(e);
        this.adjustDistance(e, "delta");
        this.eventForUserAction = e;
        var isDxMouseWheel = isDxMouseWheelEvent(e.originalEvent);
        null === (_this$hScrollbarRef$c2 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c2 ? void 0 : _this$hScrollbarRef$c2.moveHandler(e.delta.x, isDxMouseWheel);
        null === (_this$vScrollbarRef$c2 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c2 ? void 0 : _this$vScrollbarRef$c2.moveHandler(e.delta.y, isDxMouseWheel)
    }
    handleEnd(event) {
        var _this$hScrollbarRef$c3, _this$vScrollbarRef$c3;
        this.adjustDistance(event, "velocity");
        this.eventForUserAction = event;
        null === (_this$hScrollbarRef$c3 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c3 ? void 0 : _this$hScrollbarRef$c3.endHandler(event.velocity.x, true);
        null === (_this$vScrollbarRef$c3 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c3 ? void 0 : _this$vScrollbarRef$c3.endHandler(event.velocity.y, true)
    }
    handleStop() {
        var _this$hScrollbarRef$c4, _this$vScrollbarRef$c4;
        null === (_this$hScrollbarRef$c4 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c4 ? void 0 : _this$hScrollbarRef$c4.stopHandler();
        null === (_this$vScrollbarRef$c4 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c4 ? void 0 : _this$vScrollbarRef$c4.stopHandler()
    }
    handleCancel(event) {
        var _this$hScrollbarRef$c5, _this$vScrollbarRef$c5;
        this.eventForUserAction = event;
        null === (_this$hScrollbarRef$c5 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c5 ? void 0 : _this$hScrollbarRef$c5.endHandler(0, false);
        null === (_this$vScrollbarRef$c5 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c5 ? void 0 : _this$vScrollbarRef$c5.endHandler(0, false)
    }
    isCrossThumbScrolling(event) {
        var {
            target: target
        } = event.originalEvent;
        var verticalScrolling = false;
        var horizontalScrolling = false;
        if (this.direction.isVertical) {
            verticalScrolling = this.props.scrollByThumb && this.vScrollbarRef.current.isThumb(target)
        }
        if (this.direction.isHorizontal) {
            horizontalScrolling = this.props.scrollByThumb && this.hScrollbarRef.current.isThumb(target)
        }
        return verticalScrolling || horizontalScrolling
    }
    adjustDistance(event, property) {
        var distance = event[property];
        distance.x *= this.validDirections[DIRECTION_HORIZONTAL] ? 1 : 0;
        distance.y *= this.validDirections[DIRECTION_VERTICAL] ? 1 : 0;
        if (isDxMouseWheelEvent(event.originalEvent)) {
            var devicePixelRatio = getDevicePixelRatio();
            distance.x = Math.round(distance.x / devicePixelRatio * 100) / 100;
            distance.y = Math.round(distance.y / devicePixelRatio * 100) / 100
        }
    }
    suppressDirections(event) {
        if (isDxMouseWheelEvent(event.originalEvent)) {
            this.prepareDirections(true);
            return
        }
        this.prepareDirections(false);
        var {
            target: target
        } = event.originalEvent;
        if (this.direction.isVertical) {
            var scrollbar = this.vScrollbarRef.current;
            this.validDirections[DIRECTION_VERTICAL] = this.validateEvent(this.isContent(target), scrollbar.isScrollbar(target), scrollbar.isThumb(target))
        }
        if (this.direction.isHorizontal) {
            var _scrollbar = this.hScrollbarRef.current;
            this.validDirections[DIRECTION_HORIZONTAL] = this.validateEvent(this.isContent(target), _scrollbar.isScrollbar(target), _scrollbar.isThumb(target))
        }
    }
    validateEvent(isContent, isScrollbar, isThumb) {
        return this.props.scrollByThumb && (isScrollbar || isThumb) || this.props.scrollByContent && isContent
    }
    prepareDirections(value) {
        this.validDirections[DIRECTION_HORIZONTAL] = value;
        this.validDirections[DIRECTION_VERTICAL] = value
    }
    isContent(element) {
        var closest = element.closest(".".concat(SCROLLABLE_SIMULATED_CLASS));
        if (isDefined(closest)) {
            return closest === this.scrollableRef.current
        }
        return false
    }
    tryGetAllowedDirection(event) {
        return isDxMouseWheelEvent(event) ? permissibleWheelDirection(this.props.direction, event.shiftKey) : this.permissibleDirection
    }
    isLocked() {
        return this.locked
    }
    validateWheel(event) {
        var scrollbar = permissibleWheelDirection(this.props.direction, event.shiftKey) === DIRECTION_HORIZONTAL ? this.hScrollbarRef.current : this.vScrollbarRef.current;
        var reachedMin = scrollbar.reachedMin();
        var reachedMax = scrollbar.reachedMax();
        var contentGreaterThanContainer = !reachedMin || !reachedMax;
        var locatedNotAtBound = !reachedMin && !reachedMax;
        var scrollFromMin = reachedMin && event.delta > 0;
        var scrollFromMax = reachedMax && event.delta < 0;
        var validated = contentGreaterThanContainer && (locatedNotAtBound || scrollFromMin || scrollFromMax);
        validated = validated || void 0 !== this.validateWheelTimer;
        if (validated) {
            this.clearWheelValidationTimer();
            this.validateWheelTimer = setTimeout(this.clearWheelValidationTimer, VALIDATE_WHEEL_TIMEOUT)
        }
        return validated
    }
    clearWheelValidationTimer() {
        clearTimeout(this.validateWheelTimer);
        this.validateWheelTimer = void 0
    }
    validateMove(event) {
        if (!this.props.scrollByContent && !isDefined(event.target.closest(".".concat(SCROLLABLE_SCROLLBAR_CLASS)))) {
            return false
        }
        return isDefined(this.permissibleDirection)
    }
    handleKeyDown(event) {
        if (this.state.scrolling) {
            event.originalEvent.stopPropagation();
            event.originalEvent.preventDefault();
            return
        }
        var isKeySupported = Object.values(KEY_CODES).includes(normalizeKeyName(event));
        if (isKeySupported) {
            event.originalEvent.stopPropagation();
            event.originalEvent.preventDefault()
        }
        switch (normalizeKeyName(event)) {
            case KEY_CODES.DOWN:
                this.scrollByLine({
                    top: 1,
                    left: 0
                });
                break;
            case KEY_CODES.UP:
                this.scrollByLine({
                    top: -1,
                    left: 0
                });
                break;
            case KEY_CODES.RIGHT:
                this.scrollByLine({
                    top: 0,
                    left: 1
                });
                break;
            case KEY_CODES.LEFT:
                this.scrollByLine({
                    top: 0,
                    left: -1
                });
                break;
            case KEY_CODES.PAGE_DOWN:
                this.scrollByPage(1);
                break;
            case KEY_CODES.PAGE_UP:
                this.scrollByPage(-1);
                break;
            case KEY_CODES.HOME:
                this.scrollByKey(KEY_CODES.HOME);
                break;
            case KEY_CODES.END:
                this.scrollByKey(KEY_CODES.END)
        }
    }
    scrollByLine(lines) {
        var scrollOffset = Math.abs(SCROLL_LINE_HEIGHT / getDevicePixelRatio() * 100) / 100;
        this.scrollByLocation({
            top: lines.top * scrollOffset,
            left: lines.left * scrollOffset
        })
    }
    scrollByPage(page) {
        var distance = {
            left: 0,
            top: 0
        };
        var {
            clientHeight: clientHeight,
            clientWidth: clientWidth
        } = this.containerRef.current;
        if (permissibleWheelDirection(this.props.direction, false) === DIRECTION_VERTICAL) {
            distance.top = page * clientHeight
        } else {
            distance.left = page * clientWidth
        }
        this.scrollByLocation(distance)
    }
    scrollByKey(key) {
        var {
            scrollLeft: scrollLeft,
            scrollTop: scrollTop
        } = this.containerRef.current;
        var vOffsetMax = -this.vScrollOffsetMax + this.state.bottomPocketHeight + this.state.contentPaddingBottom;
        var hOffsetMax = -this.hScrollOffsetMax;
        var offset = getOffsetDistance(key === KEY_CODES.HOME ? {
            top: 0,
            left: this.props.rtlEnabled ? hOffsetMax : 0
        } : {
            top: vOffsetMax,
            left: this.props.rtlEnabled ? 0 : hOffsetMax
        }, {
            top: scrollTop,
            left: scrollLeft
        });
        var direction = permissibleWheelDirection(this.props.direction, false);
        this.scrollByLocation(direction === DIRECTION_VERTICAL ? {
            top: offset.top,
            left: 0
        } : {
            top: 0,
            left: offset.left
        })
    }
    lock() {
        this.locked = true
    }
    unlock() {
        if (!this.props.disabled) {
            this.locked = false
        }
    }
    onVisibilityChangeHandler(visible) {
        var _this$props$onVisibil, _this$props9;
        if (visible) {
            this.updateHandler();
            var {
                scrollLeft: scrollLeft,
                scrollTop: scrollTop
            } = this.savedScrollOffset;
            this.scrollLocationChange({
                fullScrollProp: "scrollTop",
                location: scrollTop,
                needFireScroll: false
            });
            this.scrollLocationChange({
                fullScrollProp: "scrollLeft",
                location: scrollLeft,
                needFireScroll: false
            })
        }
        null === (_this$props$onVisibil = (_this$props9 = this.props).onVisibilityChange) || void 0 === _this$props$onVisibil ? void 0 : _this$props$onVisibil.call(_this$props9, visible)
    }
    updateElementDimensions() {
        if (this.props.forceGeneratePockets) {
            this.setTopPocketDimensions(this.topPocketRef.current);
            this.setBottomPocketDimensions(this.bottomPocketRef.current)
        }
        this.setContentDimensions(this.content());
        this.setContainerDimensions(this.containerRef.current)
    }
    setTopPocketDimensions(topPocketEl) {
        this.setState(__state_argument => ({
            topPocketHeight: this.props.forceGeneratePockets && this.props.pullDownEnabled ? topPocketEl.clientHeight : 0
        }))
    }
    setBottomPocketDimensions(bottomPocketEl) {
        this.setState(__state_argument => ({
            bottomPocketHeight: this.props.forceGeneratePockets && this.props.reachBottomEnabled ? bottomPocketEl.clientHeight : 0
        }))
    }
    setContentDimensions(contentEl) {
        this.setState(__state_argument => ({
            contentClientHeight: contentEl.clientHeight
        }));
        this.setState(__state_argument => ({
            contentScrollHeight: contentEl.scrollHeight
        }));
        this.setState(__state_argument => ({
            contentClientWidth: contentEl.clientWidth
        }));
        this.setState(__state_argument => ({
            contentScrollWidth: contentEl.scrollWidth
        }));
        this.setState(__state_argument => ({
            contentPaddingBottom: getElementPadding(this.contentRef.current, "bottom")
        }))
    }
    setContainerDimensions(containerEl) {
        this.setState(__state_argument => ({
            containerClientHeight: containerEl.clientHeight
        }));
        this.setState(__state_argument => ({
            containerClientWidth: containerEl.clientWidth
        }))
    }
    get contentHeight() {
        var _this$contentRef;
        return "hidden" === getElementOverflowY(null === (_this$contentRef = this.contentRef) || void 0 === _this$contentRef ? void 0 : _this$contentRef.current) ? this.state.contentClientHeight : Math.max(this.state.contentScrollHeight, this.state.contentClientHeight)
    }
    get contentWidth() {
        var _this$contentRef2;
        return "hidden" === getElementOverflowX(null === (_this$contentRef2 = this.contentRef) || void 0 === _this$contentRef2 ? void 0 : _this$contentRef2.current) ? this.state.contentClientWidth : Math.max(this.state.contentScrollWidth, this.state.contentClientWidth)
    }
    get containerHasSizes() {
        return this.state.containerClientHeight > 0 && this.state.containerClientWidth > 0
    }
    get contentStyles() {
        if (void 0 !== this.__getterCache.contentStyles) {
            return this.__getterCache.contentStyles
        }
        return this.__getterCache.contentStyles = (() => ({
            transform: "translate(".concat(this.contentTranslateX, "px, ").concat(this.contentTranslateY, "px)")
        }))()
    }
    get contentTranslateY() {
        var location = this.state.vScrollLocation;
        var transformValue = location % 1;
        var maxOffset = this.vScrollOffsetMax - this.state.bottomPocketHeight - this.state.contentPaddingBottom;
        if (maxOffset >= 0) {
            return 0
        }
        if (!this.props.bounceEnabled || inRange(this.state.vScrollLocation, maxOffset, 0)) {
            return -this.state.topPocketHeight
        }
        if (location > 0) {
            transformValue = location
        }
        if (location < maxOffset) {
            transformValue = location - maxOffset
        }
        return transformValue - this.state.topPocketHeight
    }
    get contentTranslateX() {
        var location = this.state.hScrollLocation;
        var transformValue = location % 1;
        if (!this.props.bounceEnabled || 0 === this.hScrollOffsetMax || inRange(this.state.hScrollLocation, this.hScrollOffsetMax, 0)) {
            return 0
        }
        if (location > 0) {
            transformValue = location
        }
        if (location < this.hScrollOffsetMax) {
            transformValue = location - this.hScrollOffsetMax
        }
        return transformValue
    }
    get containerStyles() {
        if (void 0 !== this.__getterCache.containerStyles) {
            return this.__getterCache.containerStyles
        }
        return this.__getterCache.containerStyles = (() => {
            var direction = this.permissibleDirection;
            var vDirectionAllowed = direction === DIRECTION_VERTICAL || direction === DIRECTION_BOTH;
            var hDirectionAllowed = direction === DIRECTION_HORIZONTAL || direction === DIRECTION_BOTH;
            var touchDirection = vDirectionAllowed ? "pan-x" : "";
            touchDirection = hDirectionAllowed ? "pan-y" : touchDirection;
            touchDirection = vDirectionAllowed && hDirectionAllowed ? "none" : touchDirection;
            return {
                touchAction: touchDirection
            }
        })()
    }
    get cssClasses() {
        var {
            classes: classes,
            direction: direction,
            disabled: disabled,
            showScrollbar: showScrollbar
        } = this.props;
        var classesMap = {
            "dx-scrollable": true,
            [SCROLLABLE_SIMULATED_CLASS]: true,
            ["dx-scrollable-".concat(direction)]: true,
            [SCROLLABLE_DISABLED_CLASS]: !!disabled,
            [SCROLLABLE_SCROLLBARS_ALWAYSVISIBLE]: "always" === showScrollbar,
            [String(classes)]: !!classes
        };
        return combineClasses(classesMap)
    }
    get direction() {
        if (void 0 !== this.__getterCache.direction) {
            return this.__getterCache.direction
        }
        return this.__getterCache.direction = (() => new ScrollDirection(this.props.direction))()
    }
    get permissibleDirection() {
        var {
            bounceEnabled: bounceEnabled
        } = this.props;
        return allowedDirection(this.props.direction, -this.vScrollOffsetMax, -this.hScrollOffsetMax, bounceEnabled)
    }
    get isHoverable() {
        return !this.props.disabled && "onHover" === this.props.showScrollbar
    }
    get restAttributes() {
        var _this$props10 = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props10, _excluded);
        return restProps
    }
    content() {
        if (this.props.needScrollViewContentWrapper) {
            return this.scrollViewContentRef.current
        }
        return this.contentRef.current
    }
    container() {
        return this.containerRef.current
    }
    refresh() {
        var _this$props$onPullDow2, _this$props11;
        this.setState(__state_argument => ({
            topPocketState: TopPocketState.STATE_READY
        }));
        this.startLoading();
        null === (_this$props$onPullDow2 = (_this$props11 = this.props).onPullDown) || void 0 === _this$props$onPullDow2 ? void 0 : _this$props$onPullDow2.call(_this$props11, {})
    }
    release() {
        var _this$hScrollbarRef$c6, _this$vScrollbarRef$c6;
        this.onRelease();
        null === (_this$hScrollbarRef$c6 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c6 ? void 0 : _this$hScrollbarRef$c6.releaseHandler();
        null === (_this$vScrollbarRef$c6 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c6 ? void 0 : _this$vScrollbarRef$c6.releaseHandler()
    }
    updateHandler() {
        this.updateElementDimensions();
        this.onUpdated()
    }
    scrollHeight() {
        return this.content().offsetHeight
    }
    scrollWidth() {
        return this.content().offsetWidth
    }
    scrollOffset() {
        var {
            scrollLeft: scrollLeft,
            scrollTop: scrollTop
        } = this.savedScrollOffset;
        return {
            top: 0 === this.vScrollOffsetMax ? 0 : scrollTop,
            left: 0 === this.hScrollOffsetMax ? 0 : scrollLeft
        }
    }
    scrollTop() {
        return this.scrollOffset().top
    }
    scrollLeft() {
        return this.scrollOffset().left
    }
    clientHeight() {
        return this.containerRef.current.clientHeight
    }
    clientWidth() {
        return this.containerRef.current.clientWidth
    }
    validate(event) {
        if (this.isLocked()) {
            return false
        }
        this.updateHandler();
        return this.moveIsAllowed(event)
    }
    moveIsAllowed(event) {
        if (this.props.disabled || isDxMouseWheelEvent(event) && isCommandKeyPressed({
                ctrlKey: event.ctrlKey,
                metaKey: event.metaKey
            })) {
            return false
        }
        if (this.props.bounceEnabled) {
            return true
        }
        return isDxMouseWheelEvent(event) ? this.validateWheel(event) : this.validateMove(event)
    }
    scrollByLocation(location) {
        var _this$hScrollbarRef$c7, _this$vScrollbarRef$c7;
        this.updateHandler();
        this.setState(__state_argument => ({
            scrolling: true
        }));
        this.prepareDirections(true);
        this.onStart();
        var {
            scrollLeft: scrollLeft,
            scrollTop: scrollTop
        } = this.containerRef.current;
        var {
            left: left,
            top: top
        } = location;
        null === (_this$hScrollbarRef$c7 = this.hScrollbarRef.current) || void 0 === _this$hScrollbarRef$c7 ? void 0 : _this$hScrollbarRef$c7.scrollTo(scrollLeft + left);
        null === (_this$vScrollbarRef$c7 = this.vScrollbarRef.current) || void 0 === _this$vScrollbarRef$c7 ? void 0 : _this$vScrollbarRef$c7.scrollTo(scrollTop + top);
        this.setState(__state_argument => ({
            scrolling: false
        }))
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.state.hScrollLocation !== nextState.hScrollLocation || this.props.bounceEnabled !== nextProps.bounceEnabled || this.state.contentClientWidth !== nextState.contentClientWidth || this.state.contentScrollWidth !== nextState.contentScrollWidth || this.state.containerClientWidth !== nextState.containerClientWidth || this.state.vScrollLocation !== nextState.vScrollLocation || this.state.contentClientHeight !== nextState.contentClientHeight || this.state.contentScrollHeight !== nextState.contentScrollHeight || this.state.containerClientHeight !== nextState.containerClientHeight || this.state.bottomPocketHeight !== nextState.bottomPocketHeight || this.state.contentPaddingBottom !== nextState.contentPaddingBottom || this.state.topPocketHeight !== nextState.topPocketHeight) {
            this.__getterCache.contentStyles = void 0
        }
        if (this.props.bounceEnabled !== nextProps.bounceEnabled || this.props.direction !== nextProps.direction || this.state.contentClientHeight !== nextState.contentClientHeight || this.state.contentScrollHeight !== nextState.contentScrollHeight || this.state.containerClientHeight !== nextState.containerClientHeight || this.state.contentClientWidth !== nextState.contentClientWidth || this.state.contentScrollWidth !== nextState.contentScrollWidth || this.state.containerClientWidth !== nextState.containerClientWidth) {
            this.__getterCache.containerStyles = void 0
        }
        if (this.props.direction !== nextProps.direction) {
            this.__getterCache.direction = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            active: this.state.active,
            hovered: this.state.hovered,
            scrolling: this.state.scrolling,
            containerClientWidth: this.state.containerClientWidth,
            containerClientHeight: this.state.containerClientHeight,
            contentScrollWidth: this.state.contentScrollWidth,
            contentScrollHeight: this.state.contentScrollHeight,
            contentClientWidth: this.state.contentClientWidth,
            contentClientHeight: this.state.contentClientHeight,
            contentPaddingBottom: this.state.contentPaddingBottom,
            topPocketHeight: this.state.topPocketHeight,
            bottomPocketHeight: this.state.bottomPocketHeight,
            topPocketState: this.state.topPocketState,
            isLoadPanelVisible: this.state.isLoadPanelVisible,
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
            pulledDown: this.pulledDown,
            handleScroll: this.handleScroll,
            syncScrollbarsWithContent: this.syncScrollbarsWithContent,
            startLoading: this.startLoading,
            finishLoading: this.finishLoading,
            getEventArgs: this.getEventArgs,
            getInitEventData: this.getInitEventData,
            onStart: this.onStart,
            onEnd: this.onEnd,
            restoreEndActionDirections: this.restoreEndActionDirections,
            onUpdated: this.onUpdated,
            onBounce: this.onBounce,
            onPullDown: this.onPullDown,
            onRelease: this.onRelease,
            onReachBottom: this.onReachBottom,
            scrollLocationChange: this.scrollLocationChange,
            hScrollOffsetMax: this.hScrollOffsetMax,
            vScrollOffsetMax: this.vScrollOffsetMax,
            vScrollOffsetMin: this.vScrollOffsetMin,
            onScroll: this.onScroll,
            handleInit: this.handleInit,
            handleStart: this.handleStart,
            handleMove: this.handleMove,
            handleEnd: this.handleEnd,
            handleStop: this.handleStop,
            handleCancel: this.handleCancel,
            isCrossThumbScrolling: this.isCrossThumbScrolling,
            adjustDistance: this.adjustDistance,
            suppressDirections: this.suppressDirections,
            validateEvent: this.validateEvent,
            prepareDirections: this.prepareDirections,
            isContent: this.isContent,
            tryGetAllowedDirection: this.tryGetAllowedDirection,
            isLocked: this.isLocked,
            validateWheel: this.validateWheel,
            clearWheelValidationTimer: this.clearWheelValidationTimer,
            validateMove: this.validateMove,
            handleKeyDown: this.handleKeyDown,
            scrollByLine: this.scrollByLine,
            scrollByPage: this.scrollByPage,
            scrollByKey: this.scrollByKey,
            lock: this.lock,
            unlock: this.unlock,
            onVisibilityChangeHandler: this.onVisibilityChangeHandler,
            updateElementDimensions: this.updateElementDimensions,
            setTopPocketDimensions: this.setTopPocketDimensions,
            setBottomPocketDimensions: this.setBottomPocketDimensions,
            setContentDimensions: this.setContentDimensions,
            setContainerDimensions: this.setContainerDimensions,
            contentHeight: this.contentHeight,
            contentWidth: this.contentWidth,
            containerHasSizes: this.containerHasSizes,
            contentStyles: this.contentStyles,
            contentTranslateY: this.contentTranslateY,
            contentTranslateX: this.contentTranslateX,
            containerStyles: this.containerStyles,
            cssClasses: this.cssClasses,
            direction: this.direction,
            permissibleDirection: this.permissibleDirection,
            isHoverable: this.isHoverable,
            restAttributes: this.restAttributes
        })
    }
}
ScrollableSimulated.defaultProps = ScrollableSimulatedProps;
