/**
 * DevExtreme (cjs/renovation/ui/pager/info.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PAGER_INFO_CLASS = exports.InfoTextProps = exports.InfoText = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _string = require("../../../core/utils/string");
var _message = _interopRequireDefault(require("../../../localization/message"));
var _pager_props = require("./common/pager_props");
var _excluded = ["infoText", "pageCount", "pageIndex", "rootElementRef", "totalCount"];

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
var PAGER_INFO_CLASS = "dx-info";
exports.PAGER_INFO_CLASS = PAGER_INFO_CLASS;
var viewFunction = function(_ref) {
    var rootElementRef = _ref.props.rootElementRef,
        text = _ref.text;
    return (0, _inferno.createVNode)(1, "div", PAGER_INFO_CLASS, text, 0, null, null, rootElementRef)
};
exports.viewFunction = viewFunction;
var InfoTextProps = {};
exports.InfoTextProps = InfoTextProps;
var InfoTextPropsType = Object.defineProperties({}, {
    pageIndex: {
        get: function() {
            return _pager_props.InternalPagerProps.pageIndex
        },
        configurable: true,
        enumerable: true
    },
    pageCount: {
        get: function() {
            return _pager_props.InternalPagerProps.pageCount
        },
        configurable: true,
        enumerable: true
    },
    totalCount: {
        get: function() {
            return _pager_props.InternalPagerProps.totalCount
        },
        configurable: true,
        enumerable: true
    }
});
var InfoText = function(_BaseInfernoComponent) {
    _inheritsLoose(InfoText, _BaseInfernoComponent);

    function InfoText(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = InfoText.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            infoText: this.infoText,
            text: this.text,
            restAttributes: this.restAttributes
        })
    };
    _createClass(InfoText, [{
        key: "infoText",
        get: function() {
            var _this$props$infoText;
            return (null !== (_this$props$infoText = this.props.infoText) && void 0 !== _this$props$infoText ? _this$props$infoText : "") || _message.default.getFormatter("dxPager-infoText")()
        }
    }, {
        key: "text",
        get: function() {
            var _this$props = this.props,
                pageCount = _this$props.pageCount,
                pageIndex = _this$props.pageIndex,
                totalCount = _this$props.totalCount;
            return (0, _string.format)(this.infoText, (pageIndex + 1).toString(), pageCount.toString(), totalCount.toString())
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.infoText, _this$props2.pageCount, _this$props2.pageIndex, _this$props2.rootElementRef, _this$props2.totalCount, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return InfoText
}(_inferno2.BaseInfernoComponent);
exports.InfoText = InfoText;
InfoText.defaultProps = InfoTextPropsType;
