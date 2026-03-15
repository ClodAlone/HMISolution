/**
 * DevExtreme (cjs/renovation/ui/scroll_view/scrollbar/animated_scrollbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.OUT_BOUNDS_ACCELERATION = exports.MIN_VELOCITY_LIMIT = exports.BOUNCE_MIN_VELOCITY_LIMIT = exports.BOUNCE_ACCELERATION_SUM = exports.AnimatedScrollbar = exports.ACCELERATION = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _base_props = require("../../common/base_props");
var _scrollbar = require("./scrollbar");
var _frame = require("../../../../animation/frame");
var _simulated_strategy_props = require("../common/simulated_strategy_props");
var _math = require("../../../../core/utils/math");
var _clamp_into_range = require("../utils/clamp_into_range");
var _animated_scrollbar_props = require("../common/animated_scrollbar_props");
var _index = require("../../../../events/utils/index");
var _excluded = ["bottomPocketSize", "bounceEnabled", "containerHasSizes", "containerSize", "contentPaddingBottom", "contentSize", "direction", "forceGeneratePockets", "inertiaEnabled", "maxOffset", "minOffset", "onBounce", "onEnd", "onLock", "onPullDown", "onReachBottom", "onUnlock", "pullDownEnabled", "pulledDown", "reachBottomEnabled", "rtlEnabled", "scrollByThumb", "scrollLocation", "scrollLocationChange", "showScrollbar", "visible"];

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
var OUT_BOUNDS_ACCELERATION = .5;
exports.OUT_BOUNDS_ACCELERATION = OUT_BOUNDS_ACCELERATION;
var ACCELERATION = .92;
exports.ACCELERATION = ACCELERATION;
var MIN_VELOCITY_LIMIT = 1;
exports.MIN_VELOCITY_LIMIT = MIN_VELOCITY_LIMIT;
var BOUNCE_MIN_VELOCITY_LIMIT = MIN_VELOCITY_LIMIT / 5;
exports.BOUNCE_MIN_VELOCITY_LIMIT = BOUNCE_MIN_VELOCITY_LIMIT;
var FRAME_DURATION = 17;
var BOUNCE_DURATION = 400;
var BOUNCE_FRAMES = BOUNCE_DURATION / FRAME_DURATION;
var BOUNCE_ACCELERATION_SUM = (1 - Math.pow(ACCELERATION, BOUNCE_FRAMES)) / (1 - ACCELERATION);
exports.BOUNCE_ACCELERATION_SUM = BOUNCE_ACCELERATION_SUM;
var viewFunction = function(viewModel) {
    var newScrollLocation = viewModel.newScrollLocation,
        _viewModel$props = viewModel.props,
        bounceEnabled = _viewModel$props.bounceEnabled,
        containerHasSizes = _viewModel$props.containerHasSizes,
        containerSize = _viewModel$props.containerSize,
        contentSize = _viewModel$props.contentSize,
        direction = _viewModel$props.direction,
        maxOffset = _viewModel$props.maxOffset,
        minOffset = _viewModel$props.minOffset,
        rtlEnabled = _viewModel$props.rtlEnabled,
        scrollByThumb = _viewModel$props.scrollByThumb,
        scrollLocationChange = _viewModel$props.scrollLocationChange,
        showScrollbar = _viewModel$props.showScrollbar,
        visible = _viewModel$props.visible,
        scrollbarRef = viewModel.scrollbarRef;
    return (0, _inferno.createComponentVNode)(2, _scrollbar.Scrollbar, {
        direction: direction,
        contentSize: contentSize,
        containerSize: containerSize,
        visible: visible,
        minOffset: minOffset,
        maxOffset: maxOffset,
        scrollLocation: newScrollLocation,
        scrollLocationChange: scrollLocationChange,
        scrollByThumb: scrollByThumb,
        bounceEnabled: bounceEnabled,
        showScrollbar: showScrollbar,
        containerHasSizes: containerHasSizes,
        rtlEnabled: rtlEnabled
    }, null, scrollbarRef)
};
exports.viewFunction = viewFunction;
var AnimatedScrollbarPropsType = Object.defineProperties({}, {
    pulledDown: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.pulledDown
        },
        configurable: true,
        enumerable: true
    },
    bottomPocketSize: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.bottomPocketSize
        },
        configurable: true,
        enumerable: true
    },
    contentPaddingBottom: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.contentPaddingBottom
        },
        configurable: true,
        enumerable: true
    },
    direction: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.direction
        },
        configurable: true,
        enumerable: true
    },
    containerSize: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.containerSize
        },
        configurable: true,
        enumerable: true
    },
    contentSize: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.contentSize
        },
        configurable: true,
        enumerable: true
    },
    visible: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.visible
        },
        configurable: true,
        enumerable: true
    },
    containerHasSizes: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.containerHasSizes
        },
        configurable: true,
        enumerable: true
    },
    scrollLocation: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.scrollLocation
        },
        configurable: true,
        enumerable: true
    },
    minOffset: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.minOffset
        },
        configurable: true,
        enumerable: true
    },
    maxOffset: {
        get: function() {
            return _animated_scrollbar_props.AnimatedScrollbarProps.maxOffset
        },
        configurable: true,
        enumerable: true
    },
    rtlEnabled: {
        get: function() {
            return _base_props.BaseWidgetProps.rtlEnabled
        },
        configurable: true,
        enumerable: true
    },
    inertiaEnabled: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.inertiaEnabled
        },
        configurable: true,
        enumerable: true
    },
    showScrollbar: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.showScrollbar
        },
        configurable: true,
        enumerable: true
    },
    scrollByThumb: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.scrollByThumb
        },
        configurable: true,
        enumerable: true
    },
    bounceEnabled: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.bounceEnabled
        },
        configurable: true,
        enumerable: true
    },
    pullDownEnabled: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.pullDownEnabled
        },
        configurable: true,
        enumerable: true
    },
    reachBottomEnabled: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.reachBottomEnabled
        },
        configurable: true,
        enumerable: true
    },
    forceGeneratePockets: {
        get: function() {
            return _simulated_strategy_props.ScrollableSimulatedProps.forceGeneratePockets
        },
        configurable: true,
        enumerable: true
    }
});
var AnimatedScrollbar = function(_InfernoComponent) {
    _inheritsLoose(AnimatedScrollbar, _InfernoComponent);

    function AnimatedScrollbar(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.scrollbarRef = (0, _inferno.createRef)();
        _this.thumbScrolling = false;
        _this.crossThumbScrolling = false;
        _this.stepAnimationFrame = 0;
        _this.velocity = 0;
        _this.refreshing = false;
        _this.loading = false;
        _this.state = {
            canceled: false,
            newScrollLocation: 0,
            forceAnimationToBottomBound: false,
            pendingRefreshing: false,
            pendingLoading: false,
            pendingBounceAnimator: false,
            pendingInertiaAnimator: false,
            needRiseEnd: false,
            wasRelease: false
        };
        _this.isThumb = _this.isThumb.bind(_assertThisInitialized(_this));
        _this.isScrollbar = _this.isScrollbar.bind(_assertThisInitialized(_this));
        _this.reachedMin = _this.reachedMin.bind(_assertThisInitialized(_this));
        _this.reachedMax = _this.reachedMax.bind(_assertThisInitialized(_this));
        _this.initHandler = _this.initHandler.bind(_assertThisInitialized(_this));
        _this.moveHandler = _this.moveHandler.bind(_assertThisInitialized(_this));
        _this.endHandler = _this.endHandler.bind(_assertThisInitialized(_this));
        _this.stopHandler = _this.stopHandler.bind(_assertThisInitialized(_this));
        _this.scrollTo = _this.scrollTo.bind(_assertThisInitialized(_this));
        _this.releaseHandler = _this.releaseHandler.bind(_assertThisInitialized(_this));
        _this.disposeAnimationFrame = _this.disposeAnimationFrame.bind(_assertThisInitialized(_this));
        _this.risePullDown = _this.risePullDown.bind(_assertThisInitialized(_this));
        _this.riseEnd = _this.riseEnd.bind(_assertThisInitialized(_this));
        _this.riseReachBottom = _this.riseReachBottom.bind(_assertThisInitialized(_this));
        _this.startAnimator = _this.startAnimator.bind(_assertThisInitialized(_this));
        _this.syncScrollLocation = _this.syncScrollLocation.bind(_assertThisInitialized(_this));
        _this.performAnimation = _this.performAnimation.bind(_assertThisInitialized(_this));
        _this.updateLockedState = _this.updateLockedState.bind(_assertThisInitialized(_this));
        _this.suppressVelocityBeforeBoundary = _this.suppressVelocityBeforeBoundary.bind(_assertThisInitialized(_this));
        _this.scrollToNextStep = _this.scrollToNextStep.bind(_assertThisInitialized(_this));
        _this.setActiveState = _this.setActiveState.bind(_assertThisInitialized(_this));
        _this.moveTo = _this.moveTo.bind(_assertThisInitialized(_this));
        _this.moveToMouseLocation = _this.moveToMouseLocation.bind(_assertThisInitialized(_this));
        _this.resetThumbScrolling = _this.resetThumbScrolling.bind(_assertThisInitialized(_this));
        _this.stop = _this.stop.bind(_assertThisInitialized(_this));
        _this.cancel = _this.cancel.bind(_assertThisInitialized(_this));
        _this.calcThumbScrolling = _this.calcThumbScrolling.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = AnimatedScrollbar.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.disposeAnimationFrame, []), new _inferno2.InfernoEffect(this.risePullDown, [this.props.forceGeneratePockets, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.pulledDown, this.props.onPullDown]), new _inferno2.InfernoEffect(this.riseEnd, [this.props.scrollLocation, this.props.maxOffset, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.pulledDown, this.props.pullDownEnabled, this.props.reachBottomEnabled, this.state.wasRelease, this.props.onEnd, this.props.direction]), new _inferno2.InfernoEffect(this.riseReachBottom, [this.props.forceGeneratePockets, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.onReachBottom]), new _inferno2.InfernoEffect(this.startAnimator, [this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.forceGeneratePockets, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.bounceEnabled, this.props.onBounce, this.props.inertiaEnabled]), new _inferno2.InfernoEffect(this.syncScrollLocation, [this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.props.scrollLocation]), new _inferno2.InfernoEffect(this.performAnimation, [this.state.pendingInertiaAnimator, this.state.canceled, this.state.pendingBounceAnimator, this.props.bounceEnabled, this.props.minOffset, this.props.scrollLocation, this.props.forceGeneratePockets, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom]), new _inferno2.InfernoEffect(this.updateLockedState, [this.state.pendingBounceAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.onLock, this.props.onUnlock])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5, _this$_effects$6, _this$_effects$7;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.forceGeneratePockets, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.pulledDown, this.props.onPullDown]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.scrollLocation, this.props.maxOffset, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.pulledDown, this.props.pullDownEnabled, this.props.reachBottomEnabled, this.state.wasRelease, this.props.onEnd, this.props.direction]);
        null === (_this$_effects$3 = this._effects[3]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.forceGeneratePockets, this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.onReachBottom]);
        null === (_this$_effects$4 = this._effects[4]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.state.needRiseEnd, this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.scrollLocation, this.props.forceGeneratePockets, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom, this.props.minOffset, this.props.bounceEnabled, this.props.onBounce, this.props.inertiaEnabled]);
        null === (_this$_effects$5 = this._effects[5]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.state.pendingBounceAnimator, this.state.pendingInertiaAnimator, this.props.scrollLocation]);
        null === (_this$_effects$6 = this._effects[6]) || void 0 === _this$_effects$6 ? void 0 : _this$_effects$6.update([this.state.pendingInertiaAnimator, this.state.canceled, this.state.pendingBounceAnimator, this.props.bounceEnabled, this.props.minOffset, this.props.scrollLocation, this.props.forceGeneratePockets, this.props.reachBottomEnabled, this.state.forceAnimationToBottomBound, this.props.maxOffset, this.props.bottomPocketSize, this.props.contentPaddingBottom]);
        null === (_this$_effects$7 = this._effects[7]) || void 0 === _this$_effects$7 ? void 0 : _this$_effects$7.update([this.state.pendingBounceAnimator, this.state.pendingRefreshing, this.state.pendingLoading, this.props.onLock, this.props.onUnlock])
    };
    _proto.disposeAnimationFrame = function() {
        var _this2 = this;
        return function() {
            _this2.cancel()
        }
    };
    _proto.risePullDown = function() {
        if (this.props.forceGeneratePockets && this.isReadyToStart && this.inRange && this.props.pulledDown && !this.refreshing) {
            var _this$props$onPullDow, _this$props;
            this.refreshing = true;
            this.setState((function(__state_argument) {
                return {
                    pendingRefreshing: true
                }
            }));
            null === (_this$props$onPullDow = (_this$props = this.props).onPullDown) || void 0 === _this$props$onPullDow ? void 0 : _this$props$onPullDow.call(_this$props)
        }
    };
    _proto.riseEnd = function() {
        var isInsideBounds = (0, _math.inRange)(this.props.scrollLocation, this.props.maxOffset, 0);
        if (isInsideBounds && this.isReadyToStart && this.finished && !this.pendingRelease) {
            var _this$props$onEnd, _this$props2;
            this.setState((function(__state_argument) {
                return {
                    needRiseEnd: false
                }
            }));
            this.setState((function(__state_argument) {
                return {
                    wasRelease: false
                }
            }));
            this.setState((function(__state_argument) {
                return {
                    forceAnimationToBottomBound: false
                }
            }));
            null === (_this$props$onEnd = (_this$props2 = this.props).onEnd) || void 0 === _this$props$onEnd ? void 0 : _this$props$onEnd.call(_this$props2, this.props.direction)
        }
    };
    _proto.riseReachBottom = function() {
        if (this.props.forceGeneratePockets && this.isReadyToStart && this.inRange && this.isReachBottom && !this.loading && this.finished) {
            var _this$props$onReachBo, _this$props3;
            this.loading = true;
            this.setState((function(__state_argument) {
                return {
                    pendingLoading: true
                }
            }));
            null === (_this$props$onReachBo = (_this$props3 = this.props).onReachBottom) || void 0 === _this$props$onReachBo ? void 0 : _this$props$onReachBo.call(_this$props3)
        }
    };
    _proto.startAnimator = function() {
        if (this.isReadyToStart) {
            this.setState((function(__state_argument) {
                return {
                    canceled: false
                }
            }));
            if (!this.inRange && this.props.bounceEnabled) {
                var _this$props$onBounce, _this$props4;
                var distanceToBound = (0, _clamp_into_range.clampIntoRange)(this.props.scrollLocation, this.props.minOffset, this.maxOffset) - this.props.scrollLocation;
                this.velocity = distanceToBound / BOUNCE_ACCELERATION_SUM;
                null === (_this$props$onBounce = (_this$props4 = this.props).onBounce) || void 0 === _this$props$onBounce ? void 0 : _this$props$onBounce.call(_this$props4);
                this.setState((function(__state_argument) {
                    return {
                        pendingBounceAnimator: true
                    }
                }))
            }
            if (this.inRange && this.props.inertiaEnabled) {
                if (this.thumbScrolling || !this.thumbScrolling && this.crossThumbScrolling) {
                    this.velocity = 0
                }
                this.setState((function(__state_argument) {
                    return {
                        pendingInertiaAnimator: true
                    }
                }))
            }
        }
    };
    _proto.syncScrollLocation = function() {
        var _this3 = this;
        if (!this.inProgress) {
            this.setState((function(__state_argument) {
                return {
                    newScrollLocation: _this3.props.scrollLocation
                }
            }))
        }
    };
    _proto.performAnimation = function() {
        if (this.state.pendingInertiaAnimator) {
            if (this.state.canceled) {
                this.setState((function(__state_argument) {
                    return {
                        needRiseEnd: false
                    }
                }));
                this.stop();
                return
            }
            if (this.finished || !this.props.bounceEnabled && 0 === this.distanceToNearestBoundary) {
                this.stop();
                return
            }
            if (!this.props.bounceEnabled) {
                this.suppressVelocityBeforeBoundary()
            }
            this.scrollToNextStep()
        }
        if (this.state.pendingBounceAnimator) {
            if (0 === this.distanceToNearestBoundary) {
                this.stop();
                return
            }
            this.suppressVelocityBeforeBoundary();
            this.scrollToNextStep()
        }
    };
    _proto.updateLockedState = function() {
        if (this.state.pendingBounceAnimator || this.state.pendingRefreshing || this.state.pendingLoading) {
            var _this$props$onLock, _this$props5;
            null === (_this$props$onLock = (_this$props5 = this.props).onLock) || void 0 === _this$props$onLock ? void 0 : _this$props$onLock.call(_this$props5)
        } else {
            var _this$props$onUnlock, _this$props6;
            null === (_this$props$onUnlock = (_this$props6 = this.props).onUnlock) || void 0 === _this$props$onUnlock ? void 0 : _this$props$onUnlock.call(_this$props6)
        }
    };
    _proto.suppressVelocityBeforeBoundary = function() {
        if (Math.abs(this.distanceToMin) - Math.abs(this.velocity) <= 0) {
            this.velocity = this.distanceToMin
        }
        if (Math.abs(this.distanceToMax) - Math.abs(this.velocity) <= 0) {
            this.velocity = this.distanceToMax
        }
    };
    _proto.scrollToNextStep = function() {
        var _this4 = this;
        (0, _frame.cancelAnimationFrame)(this.stepAnimationFrame);
        this.stepAnimationFrame = (0, _frame.requestAnimationFrame)((function() {
            _this4.setState((function(__state_argument) {
                return {
                    newScrollLocation: _this4.props.scrollLocation + _this4.velocity
                }
            }));
            _this4.velocity *= _this4.acceleration
        }))
    };
    _proto.setActiveState = function() {
        this.scrollbarRef.current.setActiveState()
    };
    _proto.moveTo = function(value) {
        this.scrollbarRef.current.moveTo(value)
    };
    _proto.moveToMouseLocation = function(event, offset) {
        this.scrollbarRef.current.moveToMouseLocation(event, offset)
    };
    _proto.resetThumbScrolling = function() {
        this.thumbScrolling = false;
        this.crossThumbScrolling = false
    };
    _proto.stop = function() {
        this.velocity = 0;
        this.setState((function(__state_argument) {
            return {
                pendingBounceAnimator: false
            }
        }));
        this.setState((function(__state_argument) {
            return {
                pendingInertiaAnimator: false
            }
        }))
    };
    _proto.cancel = function() {
        this.setState((function(__state_argument) {
            return {
                canceled: true
            }
        }));
        this.stop();
        (0, _frame.cancelAnimationFrame)(this.stepAnimationFrame)
    };
    _proto.calcThumbScrolling = function(event, currentCrossThumbScrolling, isScrollbarClicked) {
        var target = event.originalEvent.target;
        this.thumbScrolling = isScrollbarClicked || this.props.scrollByThumb && this.isThumb(target);
        this.crossThumbScrolling = !this.thumbScrolling && currentCrossThumbScrolling
    };
    _proto.isThumb = function(element) {
        return this.scrollbarRef.current.isThumb(element)
    };
    _proto.isScrollbar = function(element) {
        return this.scrollbarRef.current.isScrollbar(element)
    };
    _proto.reachedMin = function() {
        return this.props.scrollLocation <= this.maxOffset
    };
    _proto.reachedMax = function() {
        return this.props.scrollLocation >= this.props.minOffset
    };
    _proto.initHandler = function(event, crossThumbScrolling, offset) {
        this.cancel();
        this.refreshing = false;
        this.loading = false;
        if (!(0, _index.isDxMouseWheelEvent)(event.originalEvent)) {
            var target = event.originalEvent.target;
            var scrollbarClicked = this.props.scrollByThumb && this.isScrollbar(target);
            this.calcThumbScrolling(event, crossThumbScrolling, scrollbarClicked);
            if (scrollbarClicked) {
                this.moveToMouseLocation(event, offset)
            }
            if (this.thumbScrolling) {
                this.setActiveState()
            }
        }
    };
    _proto.moveHandler = function(delta, isDxMouseWheel) {
        if (this.crossThumbScrolling) {
            return
        }
        var resultDelta = delta;
        if (this.thumbScrolling) {
            resultDelta = -Math.round(delta / (this.props.containerSize / this.props.contentSize))
        }
        var isOutBounds = !(0, _math.inRange)(this.props.scrollLocation, this.maxOffset, this.props.minOffset);
        if (isOutBounds) {
            resultDelta *= OUT_BOUNDS_ACCELERATION
        }
        var scrollValue = this.props.scrollLocation + resultDelta;
        this.moveTo(this.props.bounceEnabled && !isDxMouseWheel ? scrollValue : (0, _clamp_into_range.clampIntoRange)(scrollValue, this.props.minOffset, this.maxOffset))
    };
    _proto.endHandler = function(receivedVelocity, needRiseEnd) {
        this.velocity = this.props.inertiaEnabled && !this.thumbScrolling ? receivedVelocity : 0;
        this.setState((function(__state_argument) {
            return {
                needRiseEnd: needRiseEnd
            }
        }));
        this.resetThumbScrolling()
    };
    _proto.stopHandler = function() {
        if (this.thumbScrolling) {
            this.setState((function(__state_argument) {
                return {
                    needRiseEnd: true
                }
            }))
        }
        this.resetThumbScrolling()
    };
    _proto.scrollTo = function(value) {
        this.loading = false;
        this.refreshing = false;
        this.moveTo(-(0, _clamp_into_range.clampIntoRange)(value, -this.maxOffset, 0));
        this.setState((function(__state_argument) {
            return {
                needRiseEnd: true
            }
        }))
    };
    _proto.releaseHandler = function() {
        if (this.props.forceGeneratePockets && this.props.reachBottomEnabled && (0, _math.inRange)(this.props.scrollLocation, this.maxOffset, this.props.maxOffset)) {
            this.setState((function(__state_argument) {
                return {
                    forceAnimationToBottomBound: true
                }
            }))
        }
        this.setState((function(__state_argument) {
            return {
                wasRelease: true
            }
        }));
        this.setState((function(__state_argument) {
            return {
                needRiseEnd: true
            }
        }));
        this.resetThumbScrolling();
        this.setState((function(__state_argument) {
            return {
                pendingRefreshing: false
            }
        }));
        this.setState((function(__state_argument) {
            return {
                pendingLoading: false
            }
        }))
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            canceled: this.state.canceled,
            newScrollLocation: this.state.newScrollLocation,
            forceAnimationToBottomBound: this.state.forceAnimationToBottomBound,
            pendingRefreshing: this.state.pendingRefreshing,
            pendingLoading: this.state.pendingLoading,
            pendingBounceAnimator: this.state.pendingBounceAnimator,
            pendingInertiaAnimator: this.state.pendingInertiaAnimator,
            needRiseEnd: this.state.needRiseEnd,
            wasRelease: this.state.wasRelease,
            scrollbarRef: this.scrollbarRef,
            isReadyToStart: this.isReadyToStart,
            distanceToNearestBoundary: this.distanceToNearestBoundary,
            suppressVelocityBeforeBoundary: this.suppressVelocityBeforeBoundary,
            scrollToNextStep: this.scrollToNextStep,
            setActiveState: this.setActiveState,
            moveTo: this.moveTo,
            moveToMouseLocation: this.moveToMouseLocation,
            resetThumbScrolling: this.resetThumbScrolling,
            stop: this.stop,
            cancel: this.cancel,
            calcThumbScrolling: this.calcThumbScrolling,
            distanceToMin: this.distanceToMin,
            distanceToMax: this.distanceToMax,
            pendingRelease: this.pendingRelease,
            inProgress: this.inProgress,
            inRange: this.inRange,
            isReachBottom: this.isReachBottom,
            finished: this.finished,
            acceleration: this.acceleration,
            maxOffset: this.maxOffset,
            restAttributes: this.restAttributes
        })
    };
    _createClass(AnimatedScrollbar, [{
        key: "isReadyToStart",
        get: function() {
            return this.state.needRiseEnd && !this.inProgress && !(this.state.pendingRefreshing || this.state.pendingLoading)
        }
    }, {
        key: "distanceToNearestBoundary",
        get: function() {
            return Math.min(Math.abs(this.distanceToMin), Math.abs(this.distanceToMax))
        }
    }, {
        key: "distanceToMin",
        get: function() {
            return this.props.minOffset - this.props.scrollLocation
        }
    }, {
        key: "distanceToMax",
        get: function() {
            return this.maxOffset - this.props.scrollLocation
        }
    }, {
        key: "pendingRelease",
        get: function() {
            return (this.props.pulledDown && this.props.pullDownEnabled || this.isReachBottom && this.props.reachBottomEnabled) && !this.state.wasRelease
        }
    }, {
        key: "inProgress",
        get: function() {
            return this.state.pendingBounceAnimator || this.state.pendingInertiaAnimator
        }
    }, {
        key: "inRange",
        get: function() {
            return (0, _math.inRange)(this.props.scrollLocation, this.maxOffset, this.props.minOffset)
        }
    }, {
        key: "isReachBottom",
        get: function() {
            return this.props.reachBottomEnabled && Math.round(this.props.scrollLocation - this.props.maxOffset) <= 1
        }
    }, {
        key: "finished",
        get: function() {
            if (this.state.pendingBounceAnimator) {
                return Math.abs(this.velocity) <= BOUNCE_MIN_VELOCITY_LIMIT
            }
            return Math.abs(this.velocity) <= MIN_VELOCITY_LIMIT
        }
    }, {
        key: "acceleration",
        get: function() {
            return this.state.pendingBounceAnimator || this.inRange ? ACCELERATION : OUT_BOUNDS_ACCELERATION
        }
    }, {
        key: "maxOffset",
        get: function() {
            if (this.props.forceGeneratePockets && this.props.reachBottomEnabled && !this.state.forceAnimationToBottomBound) {
                return this.props.maxOffset - this.props.bottomPocketSize - this.props.contentPaddingBottom
            }
            return this.props.maxOffset
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props7 = this.props,
                restProps = (_this$props7.bottomPocketSize, _this$props7.bounceEnabled, _this$props7.containerHasSizes, _this$props7.containerSize, _this$props7.contentPaddingBottom, _this$props7.contentSize, _this$props7.direction, _this$props7.forceGeneratePockets, _this$props7.inertiaEnabled, _this$props7.maxOffset, _this$props7.minOffset, _this$props7.onBounce, _this$props7.onEnd, _this$props7.onLock, _this$props7.onPullDown, _this$props7.onReachBottom, _this$props7.onUnlock, _this$props7.pullDownEnabled, _this$props7.pulledDown, _this$props7.reachBottomEnabled, _this$props7.rtlEnabled, _this$props7.scrollByThumb, _this$props7.scrollLocation, _this$props7.scrollLocationChange, _this$props7.showScrollbar, _this$props7.visible, _objectWithoutProperties(_this$props7, _excluded));
            return restProps
        }
    }]);
    return AnimatedScrollbar
}(_inferno2.InfernoComponent);
exports.AnimatedScrollbar = AnimatedScrollbar;
AnimatedScrollbar.defaultProps = AnimatedScrollbarPropsType;
