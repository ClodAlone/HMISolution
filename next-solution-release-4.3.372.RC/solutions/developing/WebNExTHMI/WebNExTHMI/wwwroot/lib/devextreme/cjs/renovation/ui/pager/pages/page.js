/**
 * DevExtreme (cjs/renovation/ui/pager/pages/page.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PageProps = exports.Page = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _light_button = require("../common/light_button");
var _consts = require("../common/consts");
var _combine_classes = require("../../../utils/combine_classes");
var _excluded = ["className", "index", "onClick", "selected"];

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
var viewFunction = function(_ref) {
    var className = _ref.className,
        label = _ref.label,
        onClick = _ref.props.onClick,
        value = _ref.value;
    return (0, _inferno.createComponentVNode)(2, _light_button.LightButton, {
        className: className,
        label: label,
        onClick: onClick,
        children: value
    })
};
exports.viewFunction = viewFunction;
var PageProps = {
    index: 0,
    selected: false,
    className: _consts.PAGER_PAGE_CLASS
};
exports.PageProps = PageProps;
var Page = function(_BaseInfernoComponent) {
    _inheritsLoose(Page, _BaseInfernoComponent);

    function Page(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Page.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            label: this.label,
            value: this.value,
            className: this.className,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Page, [{
        key: "label",
        get: function() {
            return "Page ".concat(this.value)
        }
    }, {
        key: "value",
        get: function() {
            return this.props.index + 1
        }
    }, {
        key: "className",
        get: function() {
            var _combineClasses;
            var selected = this.props.selected;
            return (0, _combine_classes.combineClasses)((_combineClasses = {}, _defineProperty(_combineClasses, "".concat(this.props.className), !!this.props.className), _defineProperty(_combineClasses, _consts.PAGER_SELECTION_CLASS, !!selected), _combineClasses))
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.className, _this$props.index, _this$props.onClick, _this$props.selected, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Page
}(_inferno2.BaseInfernoComponent);
exports.Page = Page;
Page.defaultProps = PageProps;
