/**
 * DevExtreme (cjs/renovation/ui/editors/calendar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.CalendarProps = exports.Calendar = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _calendar = _interopRequireDefault(require("../../../ui/calendar"));
var _dom_component_wrapper = require("../common/dom_component_wrapper");
var _base_props = require("../common/base_props");
var _excluded = ["_todayDate", "accessKey", "activeStateEnabled", "className", "defaultValue", "disabled", "firstDayOfWeek", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "max", "min", "onClick", "onKeyDown", "rtlEnabled", "skipFocusCheck", "tabIndex", "value", "valueChange", "visible", "width"];

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

function today() {
    return new Date
}
var viewFunction = function(_ref) {
    var domComponentWrapperRef = _ref.domComponentWrapperRef,
        props = _ref.props,
        restAttributes = _ref.restAttributes;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _dom_component_wrapper.DomComponentWrapper, _extends({
        componentType: _calendar.default,
        componentProps: props,
        templateNames: ["cellTemplate"]
    }, restAttributes), null, domComponentWrapperRef))
};
exports.viewFunction = viewFunction;
var CalendarProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors({
    _todayDate: today,
    skipFocusCheck: false,
    defaultValue: null,
    isReactComponentWrapper: true
})));
exports.CalendarProps = CalendarProps;
var Calendar = function(_InfernoComponent) {
    _inheritsLoose(Calendar, _InfernoComponent);

    function Calendar(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.domComponentWrapperRef = (0, _inferno.createRef)();
        _this.state = {
            value: void 0 !== _this.props.value ? _this.props.value : _this.props.defaultValue
        };
        _this.saveInstance = _this.saveInstance.bind(_assertThisInitialized(_this));
        _this.focus = _this.focus.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Calendar.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.saveInstance, [])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([])
    };
    _proto.saveInstance = function() {
        var _this$domComponentWra;
        this.instance = null === (_this$domComponentWra = this.domComponentWrapperRef.current) || void 0 === _this$domComponentWra ? void 0 : _this$domComponentWra.getInstance()
    };
    _proto.focus = function() {
        var _this$instance;
        null === (_this$instance = this.instance) || void 0 === _this$instance ? void 0 : _this$instance.focus()
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            domComponentWrapperRef: this.domComponentWrapperRef,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Calendar, [{
        key: "restAttributes",
        get: function() {
            var _this$props$value = _extends({}, this.props, {
                    value: void 0 !== this.props.value ? this.props.value : this.state.value
                }),
                restProps = (_this$props$value._todayDate, _this$props$value.accessKey, _this$props$value.activeStateEnabled, _this$props$value.className, _this$props$value.defaultValue, _this$props$value.disabled, _this$props$value.firstDayOfWeek, _this$props$value.focusStateEnabled, _this$props$value.height, _this$props$value.hint, _this$props$value.hoverStateEnabled, _this$props$value.max, _this$props$value.min, _this$props$value.onClick, _this$props$value.onKeyDown, _this$props$value.rtlEnabled, _this$props$value.skipFocusCheck, _this$props$value.tabIndex, _this$props$value.value, _this$props$value.valueChange, _this$props$value.visible, _this$props$value.width, _objectWithoutProperties(_this$props$value, _excluded));
            return restProps
        }
    }]);
    return Calendar
}(_inferno2.InfernoComponent);
exports.Calendar = Calendar;
Calendar.defaultProps = CalendarProps;
