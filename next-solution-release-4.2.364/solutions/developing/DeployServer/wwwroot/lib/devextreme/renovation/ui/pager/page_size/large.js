/**
 * DevExtreme (renovation/ui/pager/page_size/large.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PageSizeLargeProps = exports.PageSizeLarge = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _light_button = require("../common/light_button");
var _pager_props = require("../common/pager_props");
var _consts = require("../common/consts");
var _excluded = ["pageSize", "pageSizeChange", "pageSizes"];

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
var viewFunction = function(_ref) {
    var pageSizesText = _ref.pageSizesText;
    return (0, _inferno.createFragment)(pageSizesText.map((function(_ref2) {
        var className = _ref2.className,
            click = _ref2.click,
            label = _ref2.label,
            text = _ref2.text;
        return (0, _inferno.createComponentVNode)(2, _light_button.LightButton, {
            className: className,
            label: label,
            onClick: click,
            children: text
        }, text)
    })), 0)
};
exports.viewFunction = viewFunction;
var PageSizeLargeProps = {};
exports.PageSizeLargeProps = PageSizeLargeProps;
var PageSizeLargePropsType = Object.defineProperties({}, {
    pageSize: {
        get: function() {
            return _pager_props.InternalPagerProps.pageSize
        },
        configurable: true,
        enumerable: true
    }
});
var PageSizeLarge = function(_BaseInfernoComponent) {
    _inheritsLoose(PageSizeLarge, _BaseInfernoComponent);

    function PageSizeLarge(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        _this.onPageSizeChange = _this.onPageSizeChange.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = PageSizeLarge.prototype;
    _proto.onPageSizeChange = function(processedPageSize) {
        var _this2 = this;
        return function() {
            _this2.props.pageSizeChange(processedPageSize);
            return _this2.props.pageSize
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.pageSize !== nextProps.pageSize || this.props.pageSizes !== nextProps.pageSizes || this.props.pageSizeChange !== nextProps.pageSizeChange) {
            this.__getterCache.pageSizesText = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pageSizesText: this.pageSizesText,
            restAttributes: this.restAttributes
        })
    };
    _createClass(PageSizeLarge, [{
        key: "pageSizesText",
        get: function() {
            var _this3 = this;
            if (void 0 !== this.__getterCache.pageSizesText) {
                return this.__getterCache.pageSizesText
            }
            return this.__getterCache.pageSizesText = (_this3$props = _this3.props, pageSize = _this3$props.pageSize, pageSizes = _this3$props.pageSizes, pageSizes.map((function(_ref3) {
                var text = _ref3.text,
                    processedPageSize = _ref3.value;
                var selected = processedPageSize === pageSize;
                var className = selected ? _consts.PAGER_SELECTED_PAGE_SIZE_CLASS : _consts.PAGER_PAGE_SIZE_CLASS;
                return {
                    className: className,
                    click: _this3.onPageSizeChange(processedPageSize),
                    label: "Display ".concat(processedPageSize, " items on page"),
                    text: text
                }
            })));
            var _this3$props, pageSize, pageSizes
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.pageSize, _this$props.pageSizeChange, _this$props.pageSizes, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return PageSizeLarge
}(_inferno2.BaseInfernoComponent);
exports.PageSizeLarge = PageSizeLarge;
PageSizeLarge.defaultProps = PageSizeLargePropsType;
