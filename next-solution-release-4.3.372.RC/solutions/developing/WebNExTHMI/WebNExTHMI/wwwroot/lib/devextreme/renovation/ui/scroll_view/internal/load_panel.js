/**
 * DevExtreme (renovation/ui/scroll_view/internal/load_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.ScrollViewLoadPanelPropsType = exports.ScrollViewLoadPanelProps = exports.ScrollViewLoadPanel = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _type = require("../../../../core/utils/type");
var _message = _interopRequireDefault(require("../../../../localization/message"));
var _load_panel = require("../../overlays/load_panel");
var _excluded = ["refreshingText", "targetElement", "visible"];

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
var SCROLLVIEW_LOADPANEL = "dx-scrollview-loadpanel";
var viewFunction = function(viewModel) {
    var position = viewModel.position,
        visible = viewModel.props.visible,
        refreshingText = viewModel.refreshingText;
    return (0, _inferno.createComponentVNode)(2, _load_panel.LoadPanel, {
        className: SCROLLVIEW_LOADPANEL,
        shading: false,
        delay: 400,
        message: refreshingText,
        position: position,
        visible: visible
    })
};
exports.viewFunction = viewFunction;
var ScrollViewLoadPanelProps = {};
exports.ScrollViewLoadPanelProps = ScrollViewLoadPanelProps;
var ScrollViewLoadPanelPropsType = Object.defineProperties({}, {
    visible: {
        get: function() {
            return _load_panel.LoadPanelProps.visible
        },
        configurable: true,
        enumerable: true
    }
});
exports.ScrollViewLoadPanelPropsType = ScrollViewLoadPanelPropsType;
var ScrollViewLoadPanel = function(_BaseInfernoComponent) {
    _inheritsLoose(ScrollViewLoadPanel, _BaseInfernoComponent);

    function ScrollViewLoadPanel(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        return _this
    }
    var _proto = ScrollViewLoadPanel.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props["targetElement?.current"] !== nextProps["targetElement?.current"]) {
            this.__getterCache.position = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            refreshingText: this.refreshingText,
            position: this.position,
            restAttributes: this.restAttributes
        })
    };
    _createClass(ScrollViewLoadPanel, [{
        key: "refreshingText",
        get: function() {
            var refreshingText = this.props.refreshingText;
            if ((0, _type.isDefined)(refreshingText)) {
                return refreshingText
            }
            return _message.default.format("dxScrollView-refreshingText")
        }
    }, {
        key: "position",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.position) {
                return this.__getterCache.position
            }
            return this.__getterCache.position = function() {
                if (_this2.props.targetElement) {
                    return {
                        of: _this2.props.targetElement.current
                    }
                }
                return
            }()
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.refreshingText, _this$props.targetElement, _this$props.visible, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return ScrollViewLoadPanel
}(_inferno2.BaseInfernoComponent);
exports.ScrollViewLoadPanel = ScrollViewLoadPanel;
ScrollViewLoadPanel.defaultProps = ScrollViewLoadPanelPropsType;
