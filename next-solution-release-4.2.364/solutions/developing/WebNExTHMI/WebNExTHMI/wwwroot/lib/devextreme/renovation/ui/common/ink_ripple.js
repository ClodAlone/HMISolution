/**
 * DevExtreme (renovation/ui/common/ink_ripple.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.InkRippleProps = exports.InkRipple = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _utils = require("../../../ui/widget/utils.ink_ripple");
var _excluded = ["config"];

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
var viewFunction = function(model) {
    return (0, _inferno.normalizeProps)((0, _inferno.createVNode)(1, "div", "dx-inkripple", null, 1, _extends({}, model.restAttributes)))
};
exports.viewFunction = viewFunction;
var InkRippleProps = Object.defineProperties({}, {
    config: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    }
});
exports.InkRippleProps = InkRippleProps;
var InkRipple = function(_BaseInfernoComponent) {
    _inheritsLoose(InkRipple, _BaseInfernoComponent);

    function InkRipple(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        _this.hideWave = _this.hideWave.bind(_assertThisInitialized(_this));
        _this.showWave = _this.showWave.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = InkRipple.prototype;
    _proto.hideWave = function(opts) {
        (0, _utils.hideWave)(this.getConfig, opts)
    };
    _proto.showWave = function(opts) {
        (0, _utils.showWave)(this.getConfig, opts)
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.config !== nextProps.config) {
            this.__getterCache.getConfig = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            getConfig: this.getConfig,
            restAttributes: this.restAttributes
        })
    };
    _createClass(InkRipple, [{
        key: "getConfig",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.getConfig) {
                return this.__getterCache.getConfig
            }
            return this.__getterCache.getConfig = (config = _this2.props.config, (0, _utils.initConfig)(config));
            var config
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.config, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return InkRipple
}(_inferno2.BaseInfernoComponent);
exports.InkRipple = InkRipple;
InkRipple.defaultProps = InkRippleProps;
