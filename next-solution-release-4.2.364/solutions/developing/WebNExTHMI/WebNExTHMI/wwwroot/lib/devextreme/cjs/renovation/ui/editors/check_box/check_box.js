/**
 * DevExtreme (cjs/renovation/ui/editors/check_box/check_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.defaultOptionRules = exports.CheckBoxPropsType = exports.CheckBoxProps = exports.CheckBox = void 0;
exports.defaultOptions = defaultOptions;
exports.viewFunction = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _utils = require("../../../../core/options/utils");
var _devices = _interopRequireDefault(require("../../../../core/devices"));
var _editor = require("../internal/editor");
var _combine_classes = require("../../../utils/combine_classes");
var _check_box_icon = require("./check_box_icon");
var _widget = require("../../common/widget");
var _excluded = ["accessKey", "activeStateEnabled", "aria", "className", "defaultValue", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "iconSize", "isValid", "name", "onClick", "onFocusIn", "onKeyDown", "readOnly", "rtlEnabled", "saveValueChangeEvent", "tabIndex", "text", "validationError", "validationErrors", "validationMessageMode", "validationStatus", "value", "valueChange", "visible", "width"];

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
var getCssClasses = function(model) {
    var text = model.text,
        value = model.value;
    var checked = value;
    var indeterminate = null === checked;
    var classesMap = {
        "dx-checkbox": true,
        "dx-checkbox-checked": true === checked,
        "dx-checkbox-has-text": !!text,
        "dx-checkbox-indeterminate": indeterminate
    };
    return (0, _combine_classes.combineClasses)(classesMap)
};
var viewFunction = function(viewModel) {
    var aria = viewModel.aria,
        classes = viewModel.cssClasses,
        editorRef = viewModel.editorRef,
        onKeyDown = viewModel.keyDown,
        onClick = viewModel.onWidgetClick,
        _viewModel$props = viewModel.props,
        accessKey = _viewModel$props.accessKey,
        activeStateEnabled = _viewModel$props.activeStateEnabled,
        className = _viewModel$props.className,
        disabled = _viewModel$props.disabled,
        focusStateEnabled = _viewModel$props.focusStateEnabled,
        height = _viewModel$props.height,
        hint = _viewModel$props.hint,
        hoverStateEnabled = _viewModel$props.hoverStateEnabled,
        iconSize = _viewModel$props.iconSize,
        isValid = _viewModel$props.isValid,
        name = _viewModel$props.name,
        onFocusIn = _viewModel$props.onFocusIn,
        readOnly = _viewModel$props.readOnly,
        rtlEnabled = _viewModel$props.rtlEnabled,
        tabIndex = _viewModel$props.tabIndex,
        text = _viewModel$props.text,
        validationError = _viewModel$props.validationError,
        validationErrors = _viewModel$props.validationErrors,
        validationMessageMode = _viewModel$props.validationMessageMode,
        validationStatus = _viewModel$props.validationStatus,
        value = _viewModel$props.value,
        visible = _viewModel$props.visible,
        width = _viewModel$props.width,
        restAttributes = viewModel.restAttributes;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _editor.Editor, _extends({
        aria: aria,
        classes: classes,
        onClick: onClick,
        onKeyDown: onKeyDown,
        accessKey: accessKey,
        activeStateEnabled: activeStateEnabled,
        focusStateEnabled: focusStateEnabled,
        hoverStateEnabled: hoverStateEnabled,
        className: className,
        disabled: disabled,
        readOnly: readOnly,
        hint: hint,
        height: height,
        width: width,
        rtlEnabled: rtlEnabled,
        tabIndex: tabIndex,
        visible: visible,
        validationError: validationError,
        validationErrors: validationErrors,
        validationMessageMode: validationMessageMode,
        validationStatus: validationStatus,
        isValid: isValid,
        onFocusIn: onFocusIn
    }, restAttributes, {
        children: (0, _inferno.createFragment)([(0, _inferno.normalizeProps)((0, _inferno.createVNode)(64, "input", null, null, 1, _extends({
            type: "hidden",
            value: "".concat(value)
        }, name && {
            name: name
        }))), (0, _inferno.createVNode)(1, "div", "dx-checkbox-container", [(0, _inferno.createComponentVNode)(2, _check_box_icon.CheckBoxIcon, {
            size: iconSize,
            isChecked: true === value
        }), text && (0, _inferno.createVNode)(1, "span", "dx-checkbox-text", text, 0)], 0)], 4)
    }), null, editorRef))
};
exports.viewFunction = viewFunction;
var CheckBoxProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_editor.EditorProps), Object.getOwnPropertyDescriptors({
    text: "",
    activeStateEnabled: true,
    hoverStateEnabled: true,
    defaultValue: false,
    valueChange: function() {}
})));
exports.CheckBoxProps = CheckBoxProps;
var CheckBoxPropsType = Object.defineProperties({}, {
    text: {
        get: function() {
            return CheckBoxProps.text
        },
        configurable: true,
        enumerable: true
    },
    activeStateEnabled: {
        get: function() {
            return CheckBoxProps.activeStateEnabled
        },
        configurable: true,
        enumerable: true
    },
    hoverStateEnabled: {
        get: function() {
            return CheckBoxProps.hoverStateEnabled
        },
        configurable: true,
        enumerable: true
    },
    defaultValue: {
        get: function() {
            return CheckBoxProps.defaultValue
        },
        configurable: true,
        enumerable: true
    },
    valueChange: {
        get: function() {
            return CheckBoxProps.valueChange
        },
        configurable: true,
        enumerable: true
    },
    readOnly: {
        get: function() {
            return CheckBoxProps.readOnly
        },
        configurable: true,
        enumerable: true
    },
    name: {
        get: function() {
            return CheckBoxProps.name
        },
        configurable: true,
        enumerable: true
    },
    validationError: {
        get: function() {
            return CheckBoxProps.validationError
        },
        configurable: true,
        enumerable: true
    },
    validationErrors: {
        get: function() {
            return CheckBoxProps.validationErrors
        },
        configurable: true,
        enumerable: true
    },
    validationMessageMode: {
        get: function() {
            return CheckBoxProps.validationMessageMode
        },
        configurable: true,
        enumerable: true
    },
    validationStatus: {
        get: function() {
            return CheckBoxProps.validationStatus
        },
        configurable: true,
        enumerable: true
    },
    isValid: {
        get: function() {
            return CheckBoxProps.isValid
        },
        configurable: true,
        enumerable: true
    },
    className: {
        get: function() {
            return CheckBoxProps.className
        },
        configurable: true,
        enumerable: true
    },
    disabled: {
        get: function() {
            return CheckBoxProps.disabled
        },
        configurable: true,
        enumerable: true
    },
    focusStateEnabled: {
        get: function() {
            return CheckBoxProps.focusStateEnabled
        },
        configurable: true,
        enumerable: true
    },
    rtlEnabled: {
        get: function() {
            return CheckBoxProps.rtlEnabled
        },
        configurable: true,
        enumerable: true
    },
    tabIndex: {
        get: function() {
            return CheckBoxProps.tabIndex
        },
        configurable: true,
        enumerable: true
    },
    visible: {
        get: function() {
            return CheckBoxProps.visible
        },
        configurable: true,
        enumerable: true
    },
    aria: {
        get: function() {
            return _widget.WidgetProps.aria
        },
        configurable: true,
        enumerable: true
    }
});
exports.CheckBoxPropsType = CheckBoxPropsType;
var defaultOptionRules = (0, _utils.createDefaultOptionRules)([{
    device: function() {
        return "desktop" === _devices.default.real().deviceType && !_devices.default.isSimulator()
    },
    options: {
        focusStateEnabled: true
    }
}]);
exports.defaultOptionRules = defaultOptionRules;
var CheckBox = function(_InfernoWrapperCompon) {
    _inheritsLoose(CheckBox, _InfernoWrapperCompon);

    function CheckBox(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.editorRef = (0, _inferno.createRef)();
        _this.state = {
            value: void 0 !== _this.props.value ? _this.props.value : _this.props.defaultValue
        };
        _this.focus = _this.focus.bind(_assertThisInitialized(_this));
        _this.blur = _this.blur.bind(_assertThisInitialized(_this));
        _this.onWidgetClick = _this.onWidgetClick.bind(_assertThisInitialized(_this));
        _this.keyDown = _this.keyDown.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = CheckBox.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.onWidgetClick = function(event) {
        var _ref;
        var _this$props = this.props,
            readOnly = _this$props.readOnly,
            saveValueChangeEvent = _this$props.saveValueChangeEvent;
        var value = null !== (_ref = void 0 !== this.props.value ? this.props.value : this.state.value) && void 0 !== _ref ? _ref : false;
        if (!readOnly) {
            null === saveValueChangeEvent || void 0 === saveValueChangeEvent ? void 0 : saveValueChangeEvent(event);
            var __newValue;
            this.setState((function(__state_argument) {
                __newValue = !value;
                return {
                    value: __newValue
                }
            }));
            this.props.valueChange(__newValue)
        }
    };
    _proto.keyDown = function(e) {
        var onKeyDown = this.props.onKeyDown;
        var keyName = e.keyName,
            originalEvent = e.originalEvent,
            which = e.which;
        var result = null === onKeyDown || void 0 === onKeyDown ? void 0 : onKeyDown(e);
        if (null !== result && void 0 !== result && result.cancel) {
            return result
        }
        if ("space" === keyName || "space" === which) {
            originalEvent.preventDefault();
            this.onWidgetClick(originalEvent)
        }
        return
    };
    _proto.focus = function() {
        this.editorRef.current.focus()
    };
    _proto.blur = function() {
        this.editorRef.current.blur()
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            editorRef: this.editorRef,
            onWidgetClick: this.onWidgetClick,
            keyDown: this.keyDown,
            cssClasses: this.cssClasses,
            aria: this.aria,
            restAttributes: this.restAttributes
        })
    };
    _createClass(CheckBox, [{
        key: "cssClasses",
        get: function() {
            return getCssClasses(_extends({}, this.props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }))
        }
    }, {
        key: "aria",
        get: function() {
            var checked = true === (void 0 !== this.props.value ? this.props.value : this.state.value);
            var indeterminate = null === (void 0 !== this.props.value ? this.props.value : this.state.value);
            var result = {
                role: "checkbox",
                checked: indeterminate ? "mixed" : "".concat(checked)
            };
            return _extends({}, result, this.props.aria)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$value = _extends({}, this.props, {
                    value: void 0 !== this.props.value ? this.props.value : this.state.value
                }),
                restProps = (_this$props$value.accessKey, _this$props$value.activeStateEnabled, _this$props$value.aria, _this$props$value.className, _this$props$value.defaultValue, _this$props$value.disabled, _this$props$value.focusStateEnabled, _this$props$value.height, _this$props$value.hint, _this$props$value.hoverStateEnabled, _this$props$value.iconSize, _this$props$value.isValid, _this$props$value.name, _this$props$value.onClick, _this$props$value.onFocusIn, _this$props$value.onKeyDown, _this$props$value.readOnly, _this$props$value.rtlEnabled, _this$props$value.saveValueChangeEvent, _this$props$value.tabIndex, _this$props$value.text, _this$props$value.validationError, _this$props$value.validationErrors, _this$props$value.validationMessageMode, _this$props$value.validationStatus, _this$props$value.value, _this$props$value.valueChange, _this$props$value.visible, _this$props$value.width, _objectWithoutProperties(_this$props$value, _excluded));
            return restProps
        }
    }]);
    return CheckBox
}(_inferno2.InfernoWrapperComponent);
exports.CheckBox = CheckBox;

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["value"];
    return Object.keys(defaultProps).reduce((function(props, propName) {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some((function(p) {
            return p === propName
        })) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }), {})
}
CheckBox.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(CheckBoxPropsType), Object.getOwnPropertyDescriptors(_extends({}, __processTwoWayProps((0, _utils.convertRulesToOptions)(defaultOptionRules))))));
var __defaultOptionRules = [];

function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    CheckBox.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(CheckBox.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps((0, _utils.convertRulesToOptions)(defaultOptionRules))), Object.getOwnPropertyDescriptors(__processTwoWayProps((0, _utils.convertRulesToOptions)(__defaultOptionRules)))))
}
