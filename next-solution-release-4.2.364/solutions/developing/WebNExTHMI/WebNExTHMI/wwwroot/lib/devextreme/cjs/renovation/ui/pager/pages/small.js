/**
 * DevExtreme (cjs/renovation/ui/pager/pages/small.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PagesSmall = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _page = require("./page");
var _info = require("../info");
var _number_box = require("../../editors/number_box");
var _message = _interopRequireDefault(require("../../../../localization/message"));
var _calculate_values_fitted_width = require("../utils/calculate_values_fitted_width");
var _get_element_width = require("../utils/get_element_width");
var _pager_props = require("../common/pager_props");
var _excluded = ["pageCount", "pageIndex", "pageIndexChange", "pagesCountText"];

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
var PAGER_INFO_TEXT_CLASS = "".concat(_info.PAGER_INFO_CLASS, "  dx-info-text");
var PAGER_PAGE_INDEX_CLASS = "dx-page-index";
var LIGHT_PAGES_CLASS = "dx-light-pages";
var PAGER_PAGES_COUNT_CLASS = "dx-pages-count";
var viewFunction = function(_ref) {
    var pageIndexRef = _ref.pageIndexRef,
        pagesCountText = _ref.pagesCountText,
        pageCount = _ref.props.pageCount,
        selectLastPageIndex = _ref.selectLastPageIndex,
        value = _ref.value,
        valueChange = _ref.valueChange,
        width = _ref.width;
    return (0, _inferno.createVNode)(1, "div", LIGHT_PAGES_CLASS, [(0, _inferno.createComponentVNode)(2, _number_box.NumberBox, {
        className: PAGER_PAGE_INDEX_CLASS,
        min: 1,
        max: pageCount,
        width: width,
        value: value,
        valueChange: valueChange
    }), (0, _inferno.createVNode)(1, "span", PAGER_INFO_TEXT_CLASS, pagesCountText, 0), (0, _inferno.createComponentVNode)(2, _page.Page, {
        className: PAGER_PAGES_COUNT_CLASS,
        selected: false,
        index: pageCount - 1,
        onClick: selectLastPageIndex
    })], 4, null, null, pageIndexRef)
};
exports.viewFunction = viewFunction;
var PagerSmallProps = Object.defineProperties({}, {
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
    }
});
var PagesSmall = function(_InfernoComponent) {
    _inheritsLoose(PagesSmall, _InfernoComponent);

    function PagesSmall(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.pageIndexRef = (0, _inferno.createRef)();
        _this.state = {
            minWidth: 10
        };
        _this.updateWidth = _this.updateWidth.bind(_assertThisInitialized(_this));
        _this.selectLastPageIndex = _this.selectLastPageIndex.bind(_assertThisInitialized(_this));
        _this.valueChange = _this.valueChange.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = PagesSmall.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.updateWidth, [this.state.minWidth])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.minWidth])
    };
    _proto.updateWidth = function() {
        var _this$pageIndexRef$cu;
        var el = null === (_this$pageIndexRef$cu = this.pageIndexRef.current) || void 0 === _this$pageIndexRef$cu ? void 0 : _this$pageIndexRef$cu.querySelector(".".concat(PAGER_PAGE_INDEX_CLASS));
        this.setState((function(__state_argument) {
            return {
                minWidth: el && (0, _get_element_width.getElementMinWidth)(el) || __state_argument.minWidth
            }
        }))
    };
    _proto.selectLastPageIndex = function() {
        this.props.pageIndexChange(this.props.pageCount - 1)
    };
    _proto.valueChange = function(value) {
        this.props.pageIndexChange(value - 1)
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pageIndexRef: this.pageIndexRef,
            value: this.value,
            width: this.width,
            pagesCountText: this.pagesCountText,
            selectLastPageIndex: this.selectLastPageIndex,
            valueChange: this.valueChange,
            restAttributes: this.restAttributes
        })
    };
    _createClass(PagesSmall, [{
        key: "value",
        get: function() {
            return this.props.pageIndex + 1
        }
    }, {
        key: "width",
        get: function() {
            var pageCount = this.props.pageCount;
            return (0, _calculate_values_fitted_width.calculateValuesFittedWidth)(this.state.minWidth, [pageCount])
        }
    }, {
        key: "pagesCountText",
        get: function() {
            var _this$props$pagesCoun;
            return (null !== (_this$props$pagesCoun = this.props.pagesCountText) && void 0 !== _this$props$pagesCoun ? _this$props$pagesCoun : "") || _message.default.getFormatter("dxPager-pagesCountText")()
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.pageCount, _this$props.pageIndex, _this$props.pageIndexChange, _this$props.pagesCountText, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return PagesSmall
}(_inferno2.InfernoComponent);
exports.PagesSmall = PagesSmall;
PagesSmall.defaultProps = PagerSmallProps;
