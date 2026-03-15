/**
 * DevExtreme (cjs/renovation/ui/scroll_view/internal/pocket/top.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.TopPocketProps = exports.TopPocket = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _load_indicator = require("../../../load_indicator");
var _combine_classes = require("../../../../utils/combine_classes");
var _message = _interopRequireDefault(require("../../../../../localization/message"));
var _consts = require("../../common/consts");
var _themes = require("../../../../../ui/themes");
var _excluded = ["pocketState", "pocketTop", "pullDownIconAngle", "pullDownOpacity", "pullDownTranslateTop", "pulledDownText", "pullingDownText", "refreshStrategy", "refreshingText", "topPocketRef", "topPocketTranslateTop", "visible"];

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
var viewFunction = function(viewModel) {
    var _viewModel$props = viewModel.props,
        pulledDownText = _viewModel$props.pulledDownText,
        pullingDownText = _viewModel$props.pullingDownText,
        refreshStrategy = _viewModel$props.refreshStrategy,
        refreshingText = _viewModel$props.refreshingText,
        topPocketRef = _viewModel$props.topPocketRef,
        pullDownClasses = viewModel.pullDownClasses,
        pullDownIconStyles = viewModel.pullDownIconStyles,
        pullDownRef = viewModel.pullDownRef,
        pullDownStyles = viewModel.pullDownStyles,
        readyVisibleClass = viewModel.readyVisibleClass,
        refreshVisibleClass = viewModel.refreshVisibleClass,
        releaseVisibleClass = viewModel.releaseVisibleClass,
        topPocketClasses = viewModel.topPocketClasses,
        topPocketStyles = viewModel.topPocketStyles;
    return (0, _inferno.createVNode)(1, "div", topPocketClasses, (0, _inferno.createVNode)(1, "div", pullDownClasses, ["swipeDown" !== refreshStrategy && (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_PULLDOWN_IMAGE_CLASS), "swipeDown" === refreshStrategy && (0, _inferno.createVNode)(1, "div", _consts.PULLDOWN_ICON_CLASS, null, 1, {
        style: (0, _inferno2.normalizeStyles)(pullDownIconStyles)
    }), (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_PULLDOWN_INDICATOR_CLASS, (0, _inferno.createComponentVNode)(2, _load_indicator.LoadIndicator), 2), "swipeDown" !== refreshStrategy && (0, _inferno.createVNode)(1, "div", _consts.SCROLLVIEW_PULLDOWN_TEXT_CLASS, [(0, _inferno.createVNode)(1, "div", releaseVisibleClass, pullingDownText, 0), (0, _inferno.createVNode)(1, "div", readyVisibleClass, pulledDownText, 0), (0, _inferno.createVNode)(1, "div", refreshVisibleClass, refreshingText, 0)], 4)], 0, {
        style: (0, _inferno2.normalizeStyles)(pullDownStyles)
    }, null, pullDownRef), 2, {
        style: (0, _inferno2.normalizeStyles)(topPocketStyles)
    }, null, topPocketRef)
};
exports.viewFunction = viewFunction;
var TopPocketProps = Object.defineProperties({
    pullDownTranslateTop: 0,
    pullDownIconAngle: 0,
    pullDownOpacity: 0,
    pocketTop: 0,
    topPocketTranslateTop: 0,
    visible: true
}, {
    pullingDownText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-pullingDownText")
        },
        configurable: true,
        enumerable: true
    },
    pulledDownText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-pulledDownText")
        },
        configurable: true,
        enumerable: true
    },
    refreshingText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-refreshingText")
        },
        configurable: true,
        enumerable: true
    },
    pocketState: {
        get: function() {
            return _consts.TopPocketState.STATE_RELEASED
        },
        configurable: true,
        enumerable: true
    }
});
exports.TopPocketProps = TopPocketProps;
var TopPocket = function(_BaseInfernoComponent) {
    _inheritsLoose(TopPocket, _BaseInfernoComponent);

    function TopPocket(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.pullDownRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        return _this
    }
    var _proto = TopPocket.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.refreshStrategy !== nextProps.refreshStrategy || this.props.pullDownOpacity !== nextProps.pullDownOpacity || this.props.pullDownTranslateTop !== nextProps.pullDownTranslateTop) {
            this.__getterCache.pullDownStyles = void 0
        }
        if (this.props.refreshStrategy !== nextProps.refreshStrategy || this.props.pocketTop !== nextProps.pocketTop || this.props.topPocketTranslateTop !== nextProps.topPocketTranslateTop) {
            this.__getterCache.topPocketStyles = void 0
        }
        if (this.props.pullDownIconAngle !== nextProps.pullDownIconAngle) {
            this.__getterCache.pullDownIconStyles = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pullDownRef: this.pullDownRef,
            releaseVisibleClass: this.releaseVisibleClass,
            readyVisibleClass: this.readyVisibleClass,
            refreshVisibleClass: this.refreshVisibleClass,
            pullDownClasses: this.pullDownClasses,
            topPocketClasses: this.topPocketClasses,
            pullDownStyles: this.pullDownStyles,
            topPocketStyles: this.topPocketStyles,
            pullDownIconStyles: this.pullDownIconStyles,
            restAttributes: this.restAttributes
        })
    };
    _createClass(TopPocket, [{
        key: "releaseVisibleClass",
        get: function() {
            return this.props.pocketState === _consts.TopPocketState.STATE_RELEASED ? _consts.SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
        }
    }, {
        key: "readyVisibleClass",
        get: function() {
            return this.props.pocketState === _consts.TopPocketState.STATE_READY ? _consts.SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
        }
    }, {
        key: "refreshVisibleClass",
        get: function() {
            return this.props.pocketState === _consts.TopPocketState.STATE_REFRESHING ? _consts.SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
        }
    }, {
        key: "pullDownClasses",
        get: function() {
            var _classesMap;
            var _this$props = this.props,
                pocketState = _this$props.pocketState,
                visible = _this$props.visible;
            var classesMap = (_classesMap = {}, _defineProperty(_classesMap, _consts.SCROLLVIEW_PULLDOWN, true), _defineProperty(_classesMap, _consts.SCROLLVIEW_PULLDOWN_READY_CLASS, pocketState === _consts.TopPocketState.STATE_READY), _defineProperty(_classesMap, _consts.SCROLLVIEW_PULLDOWN_LOADING_CLASS, pocketState === _consts.TopPocketState.STATE_REFRESHING), _defineProperty(_classesMap, "dx-state-invisible", !visible), _classesMap);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "topPocketClasses",
        get: function() {
            var _classesMap2;
            var classesMap = (_classesMap2 = {}, _defineProperty(_classesMap2, _consts.SCROLLVIEW_TOP_POCKET_CLASS, true), _defineProperty(_classesMap2, "dx-state-invisible", !this.props.visible), _classesMap2);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "pullDownStyles",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.pullDownStyles) {
                return this.__getterCache.pullDownStyles
            }
            return this.__getterCache.pullDownStyles = function() {
                if ("swipeDown" === _this2.props.refreshStrategy) {
                    return {
                        opacity: _this2.props.pullDownOpacity,
                        transform: "translate(0px, ".concat(_this2.props.pullDownTranslateTop, "px)")
                    }
                }
                return
            }()
        }
    }, {
        key: "topPocketStyles",
        get: function() {
            var _this3 = this;
            if (void 0 !== this.__getterCache.topPocketStyles) {
                return this.__getterCache.topPocketStyles
            }
            return this.__getterCache.topPocketStyles = function() {
                if ("pullDown" === _this3.props.refreshStrategy) {
                    return {
                        top: "".concat(-_this3.props.pocketTop, "px"),
                        transform: "translate(0px, ".concat(_this3.props.topPocketTranslateTop, "px)")
                    }
                }
                return
            }()
        }
    }, {
        key: "pullDownIconStyles",
        get: function() {
            var _this4 = this;
            if (void 0 !== this.__getterCache.pullDownIconStyles) {
                return this.__getterCache.pullDownIconStyles
            }
            return this.__getterCache.pullDownIconStyles = {
                transform: "rotate(".concat(_this4.props.pullDownIconAngle, "deg)")
            }
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.pocketState, _this$props2.pocketTop, _this$props2.pullDownIconAngle, _this$props2.pullDownOpacity, _this$props2.pullDownTranslateTop, _this$props2.pulledDownText, _this$props2.pullingDownText, _this$props2.refreshStrategy, _this$props2.refreshingText, _this$props2.topPocketRef, _this$props2.topPocketTranslateTop, _this$props2.visible, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return TopPocket
}(_inferno2.BaseInfernoComponent);
exports.TopPocket = TopPocket;
TopPocket.defaultProps = TopPocketProps;
