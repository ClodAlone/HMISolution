/**
 * DevExtreme (cjs/renovation/ui/pager/pages/page_index_selector.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PageIndexSelectorProps = exports.PageIndexSelector = exports.PAGER_BUTTON_DISABLE_CLASS = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _light_button = require("../common/light_button");
var _large = require("./large");
var _small = require("./small");
var _pager_props = require("../common/pager_props");
var _config_context = require("../../../common/config_context");
var _excluded = ["hasKnownLastPage", "isLargeDisplayMode", "maxPagesCount", "pageCount", "pageIndex", "pageIndexChange", "pagesCountText", "showNavigationButtons", "totalCount"];

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
var PAGER_NAVIGATE_BUTTON = "dx-navigate-button";
var PAGER_PREV_BUTTON_CLASS = "dx-prev-button";
var PAGER_NEXT_BUTTON_CLASS = "dx-next-button";
var PAGER_BUTTON_DISABLE_CLASS = "dx-button-disable";
exports.PAGER_BUTTON_DISABLE_CLASS = PAGER_BUTTON_DISABLE_CLASS;
var nextButtonClassName = "".concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_NEXT_BUTTON_CLASS);
var prevButtonClassName = "".concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_PREV_BUTTON_CLASS);
var nextButtonDisabledClassName = "".concat(PAGER_BUTTON_DISABLE_CLASS, " ").concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_NEXT_BUTTON_CLASS);
var prevButtonDisabledClassName = "".concat(PAGER_BUTTON_DISABLE_CLASS, " ").concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_PREV_BUTTON_CLASS);
var viewFunction = function(_ref) {
    var navigateToNextPage = _ref.navigateToNextPage,
        navigateToPrevPage = _ref.navigateToPrevPage,
        nextClassName = _ref.nextClassName,
        pageIndexChange = _ref.pageIndexChange,
        prevClassName = _ref.prevClassName,
        _ref$props = _ref.props,
        isLargeDisplayMode = _ref$props.isLargeDisplayMode,
        maxPagesCount = _ref$props.maxPagesCount,
        pageCount = _ref$props.pageCount,
        pageIndex = _ref$props.pageIndex,
        pagesCountText = _ref$props.pagesCountText,
        renderNextButton = _ref.renderNextButton,
        renderPrevButton = _ref.renderPrevButton;
    return (0, _inferno.createFragment)([renderPrevButton && (0, _inferno.createComponentVNode)(2, _light_button.LightButton, {
        className: prevClassName,
        label: "Previous page",
        onClick: navigateToPrevPage
    }), isLargeDisplayMode && (0, _inferno.createComponentVNode)(2, _large.PagesLarge, {
        maxPagesCount: maxPagesCount,
        pageCount: pageCount,
        pageIndex: pageIndex,
        pageIndexChange: pageIndexChange
    }), !isLargeDisplayMode && (0, _inferno.createComponentVNode)(2, _small.PagesSmall, {
        pageCount: pageCount,
        pageIndex: pageIndex,
        pageIndexChange: pageIndexChange,
        pagesCountText: pagesCountText
    }), renderNextButton && (0, _inferno.createComponentVNode)(2, _light_button.LightButton, {
        className: nextClassName,
        label: "Next page",
        onClick: navigateToNextPage
    })], 0)
};
exports.viewFunction = viewFunction;

function getIncrement(direction) {
    return "next" === direction ? 1 : -1
}
var PageIndexSelectorProps = {
    isLargeDisplayMode: true
};
exports.PageIndexSelectorProps = PageIndexSelectorProps;
var PageIndexSelectorPropsType = Object.defineProperties({}, {
    pageIndex: {
        get: function() {
            return _pager_props.InternalPagerProps.pageIndex
        },
        configurable: true,
        enumerable: true
    },
    maxPagesCount: {
        get: function() {
            return _pager_props.InternalPagerProps.maxPagesCount
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
    hasKnownLastPage: {
        get: function() {
            return _pager_props.InternalPagerProps.hasKnownLastPage
        },
        configurable: true,
        enumerable: true
    },
    showNavigationButtons: {
        get: function() {
            return _pager_props.InternalPagerProps.showNavigationButtons
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
    },
    isLargeDisplayMode: {
        get: function() {
            return PageIndexSelectorProps.isLargeDisplayMode
        },
        configurable: true,
        enumerable: true
    }
});
var PageIndexSelector = function(_BaseInfernoComponent) {
    _inheritsLoose(PageIndexSelector, _BaseInfernoComponent);

    function PageIndexSelector(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.pageIndexChange = _this.pageIndexChange.bind(_assertThisInitialized(_this));
        _this.navigateToNextPage = _this.navigateToNextPage.bind(_assertThisInitialized(_this));
        _this.navigateToPrevPage = _this.navigateToPrevPage.bind(_assertThisInitialized(_this));
        _this.getNextDirection = _this.getNextDirection.bind(_assertThisInitialized(_this));
        _this.getPrevDirection = _this.getPrevDirection.bind(_assertThisInitialized(_this));
        _this.canNavigateToPage = _this.canNavigateToPage.bind(_assertThisInitialized(_this));
        _this.getNextPageIndex = _this.getNextPageIndex.bind(_assertThisInitialized(_this));
        _this.canNavigateTo = _this.canNavigateTo.bind(_assertThisInitialized(_this));
        _this.navigateToPage = _this.navigateToPage.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = PageIndexSelector.prototype;
    _proto.pageIndexChange = function(pageIndex) {
        if (this.canNavigateToPage(pageIndex)) {
            this.props.pageIndexChange(pageIndex)
        }
    };
    _proto.navigateToNextPage = function() {
        this.navigateToPage(this.getNextDirection())
    };
    _proto.navigateToPrevPage = function() {
        this.navigateToPage(this.getPrevDirection())
    };
    _proto.getNextDirection = function() {
        var _this$config;
        return !(null !== (_this$config = this.config) && void 0 !== _this$config && _this$config.rtlEnabled) ? "next" : "prev"
    };
    _proto.getPrevDirection = function() {
        var _this$config2;
        return !(null !== (_this$config2 = this.config) && void 0 !== _this$config2 && _this$config2.rtlEnabled) ? "prev" : "next"
    };
    _proto.canNavigateToPage = function(pageIndex) {
        if (!this.props.hasKnownLastPage) {
            return pageIndex >= 0
        }
        return pageIndex >= 0 && pageIndex <= this.props.pageCount - 1
    };
    _proto.getNextPageIndex = function(direction) {
        return this.props.pageIndex + getIncrement(direction)
    };
    _proto.canNavigateTo = function(direction) {
        return this.canNavigateToPage(this.getNextPageIndex(direction))
    };
    _proto.navigateToPage = function(direction) {
        this.pageIndexChange(this.getNextPageIndex(direction))
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            config: this.config,
            pageIndexChange: this.pageIndexChange,
            navigateToNextPage: this.navigateToNextPage,
            navigateToPrevPage: this.navigateToPrevPage,
            renderPrevButton: this.renderPrevButton,
            renderNextButton: this.renderNextButton,
            nextClassName: this.nextClassName,
            prevClassName: this.prevClassName,
            restAttributes: this.restAttributes
        })
    };
    _createClass(PageIndexSelector, [{
        key: "config",
        get: function() {
            if ("ConfigContext" in this.context) {
                return this.context.ConfigContext
            }
            return _config_context.ConfigContext
        }
    }, {
        key: "renderPrevButton",
        get: function() {
            var _this$props = this.props,
                isLargeDisplayMode = _this$props.isLargeDisplayMode,
                showNavigationButtons = _this$props.showNavigationButtons;
            return !isLargeDisplayMode || showNavigationButtons
        }
    }, {
        key: "renderNextButton",
        get: function() {
            return this.renderPrevButton || !this.props.hasKnownLastPage
        }
    }, {
        key: "nextClassName",
        get: function() {
            var direction = this.getNextDirection();
            var canNavigate = this.canNavigateTo(direction);
            return canNavigate ? nextButtonClassName : nextButtonDisabledClassName
        }
    }, {
        key: "prevClassName",
        get: function() {
            var direction = this.getPrevDirection();
            var canNavigate = this.canNavigateTo(direction);
            return canNavigate ? prevButtonClassName : prevButtonDisabledClassName
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.hasKnownLastPage, _this$props2.isLargeDisplayMode, _this$props2.maxPagesCount, _this$props2.pageCount, _this$props2.pageIndex, _this$props2.pageIndexChange, _this$props2.pagesCountText, _this$props2.showNavigationButtons, _this$props2.totalCount, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return PageIndexSelector
}(_inferno2.BaseInfernoComponent);
exports.PageIndexSelector = PageIndexSelector;
PageIndexSelector.defaultProps = PageIndexSelectorPropsType;
