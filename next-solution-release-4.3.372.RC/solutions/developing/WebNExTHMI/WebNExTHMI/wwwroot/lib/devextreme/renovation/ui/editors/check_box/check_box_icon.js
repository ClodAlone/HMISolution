/**
 * DevExtreme (renovation/ui/editors/check_box/check_box_icon.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.CheckBoxIconProps = exports.CheckBoxIcon = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _get_computed_style = _interopRequireDefault(require("../../../utils/get_computed_style"));
var _window = require("../../../../core/utils/window");
var _style = require("../../../../core/utils/style");
var _type = require("../../../../core/utils/type");
var _utils = require("./utils");
var _excluded = ["isChecked", "size"];

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
var viewFunction = function(viewModel) {
    var cssStyles = viewModel.cssStyles,
        elementRef = viewModel.elementRef;
    return (0, _inferno.createVNode)(1, "span", "dx-checkbox-icon", null, 1, {
        style: (0, _inferno2.normalizeStyles)(cssStyles)
    }, null, elementRef)
};
exports.viewFunction = viewFunction;
var CheckBoxIconProps = {
    isChecked: false
};
exports.CheckBoxIconProps = CheckBoxIconProps;
var CheckBoxIcon = function(_InfernoComponent) {
    _inheritsLoose(CheckBoxIcon, _InfernoComponent);

    function CheckBoxIcon(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.elementRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        _this.updateFontSize = _this.updateFontSize.bind(_assertThisInitialized(_this));
        _this.setIconFontSize = _this.setIconFontSize.bind(_assertThisInitialized(_this));
        _this.getIconSize = _this.getIconSize.bind(_assertThisInitialized(_this));
        _this.getComputedIconSize = _this.getComputedIconSize.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = CheckBoxIcon.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.updateFontSize, [this.props.isChecked, this.props.size])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.isChecked, this.props.size])
    };
    _proto.updateFontSize = function() {
        var _this$props = this.props,
            isChecked = _this$props.isChecked,
            size = _this$props.size;
        if ((0, _window.hasWindow)() && size) {
            var newIconSize = this.getIconSize(size);
            var newFontSize = (0, _utils.getFontSizeByIconSize)(newIconSize, isChecked);
            this.setIconFontSize(newFontSize)
        }
    };
    _proto.setIconFontSize = function(fontSize) {
        var element = this.elementRef.current;
        element.style.fontSize = "".concat(fontSize, "px")
    };
    _proto.getIconSize = function(size) {
        if ((0, _type.isNumeric)(size)) {
            return size
        }
        if (size.endsWith("px")) {
            return parseInt(size, 10)
        }
        return this.getComputedIconSize()
    };
    _proto.getComputedIconSize = function() {
        var element = this.elementRef.current;
        var iconComputedStyle = (0, _get_computed_style.default)(element);
        var computedIconSize = parseInt(null === iconComputedStyle || void 0 === iconComputedStyle ? void 0 : iconComputedStyle.width, 10);
        return computedIconSize
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoComponent.prototype.componentWillUpdate.call(this);
        if (this.props.size !== nextProps.size) {
            this.__getterCache.cssStyles = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            elementRef: this.elementRef,
            setIconFontSize: this.setIconFontSize,
            getIconSize: this.getIconSize,
            getComputedIconSize: this.getComputedIconSize,
            cssStyles: this.cssStyles,
            restAttributes: this.restAttributes
        })
    };
    _createClass(CheckBoxIcon, [{
        key: "cssStyles",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.cssStyles) {
                return this.__getterCache.cssStyles
            }
            return this.__getterCache.cssStyles = (size = _this2.props.size, width = (0, _style.normalizeStyleProp)("width", size), height = (0, _style.normalizeStyleProp)("height", size), {
                height: height,
                width: width
            });
            var size, width, height
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.isChecked, _this$props2.size, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return CheckBoxIcon
}(_inferno2.InfernoComponent);
exports.CheckBoxIcon = CheckBoxIcon;
CheckBoxIcon.defaultProps = CheckBoxIconProps;
