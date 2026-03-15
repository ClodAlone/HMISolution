/**
 * DevExtreme (renovation/ui/pager/content.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.PagerContentProps = exports.PagerContent = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _info = require("./info");
var _page_index_selector = require("./pages/page_index_selector");
var _selector = require("./page_size/selector");
var _consts = require("./common/consts");
var _pager_props = require("./common/pager_props");
var _combine_classes = require("../../utils/combine_classes");
var _widget = require("../common/widget");
var _accessibility = require("../../../ui/shared/accessibility");
var _excluded = ["className", "displayMode", "gridCompatibility", "hasKnownLastPage", "infoText", "infoTextRef", "infoTextVisible", "isLargeDisplayMode", "lightModeEnabled", "maxPagesCount", "onKeyDown", "pageCount", "pageIndex", "pageIndexChange", "pageSize", "pageSizeChange", "pageSizes", "pageSizesRef", "pagesCountText", "pagesNavigatorVisible", "pagesRef", "rootElementRef", "rtlEnabled", "showInfo", "showNavigationButtons", "showPageSizes", "totalCount", "visible"];

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
var viewFunction = function(_ref) {
    var classes = _ref.classes,
        infoVisible = _ref.infoVisible,
        isLargeDisplayMode = _ref.isLargeDisplayMode,
        pageIndexSelectorVisible = _ref.pageIndexSelectorVisible,
        pagesContainerVisibility = _ref.pagesContainerVisibility,
        pagesContainerVisible = _ref.pagesContainerVisible,
        _ref$props = _ref.props,
        hasKnownLastPage = _ref$props.hasKnownLastPage,
        infoText = _ref$props.infoText,
        infoTextRef = _ref$props.infoTextRef,
        maxPagesCount = _ref$props.maxPagesCount,
        pageCount = _ref$props.pageCount,
        pageIndex = _ref$props.pageIndex,
        pageIndexChange = _ref$props.pageIndexChange,
        pageSize = _ref$props.pageSize,
        pageSizeChange = _ref$props.pageSizeChange,
        pageSizes = _ref$props.pageSizes,
        pageSizesRef = _ref$props.pageSizesRef,
        pagesCountText = _ref$props.pagesCountText,
        pagesRef = _ref$props.pagesRef,
        rtlEnabled = _ref$props.rtlEnabled,
        showNavigationButtons = _ref$props.showNavigationButtons,
        showPageSizes = _ref$props.showPageSizes,
        totalCount = _ref$props.totalCount,
        visible = _ref$props.visible,
        restAttributes = _ref.restAttributes,
        widgetRootElementRef = _ref.widgetRootElementRef;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _widget.Widget, _extends({
        rootElementRef: widgetRootElementRef,
        rtlEnabled: rtlEnabled,
        classes: classes,
        visible: visible
    }, restAttributes, {
        children: [showPageSizes && (0, _inferno.createComponentVNode)(2, _selector.PageSizeSelector, {
            rootElementRef: pageSizesRef,
            isLargeDisplayMode: isLargeDisplayMode,
            pageSize: pageSize,
            pageSizeChange: pageSizeChange,
            pageSizes: pageSizes
        }), pagesContainerVisible && (0, _inferno.createVNode)(1, "div", _consts.PAGER_PAGES_CLASS, [infoVisible && (0, _inferno.createComponentVNode)(2, _info.InfoText, {
            rootElementRef: infoTextRef,
            infoText: infoText,
            pageCount: pageCount,
            pageIndex: pageIndex,
            totalCount: totalCount
        }), pageIndexSelectorVisible && (0, _inferno.createVNode)(1, "div", _consts.PAGER_PAGE_INDEXES_CLASS, (0, _inferno.createComponentVNode)(2, _page_index_selector.PageIndexSelector, {
            hasKnownLastPage: hasKnownLastPage,
            isLargeDisplayMode: isLargeDisplayMode,
            maxPagesCount: maxPagesCount,
            pageCount: pageCount,
            pageIndex: pageIndex,
            pageIndexChange: pageIndexChange,
            pagesCountText: pagesCountText,
            showNavigationButtons: showNavigationButtons,
            totalCount: totalCount
        }), 2, null, null, pagesRef)], 0, {
            style: (0, _inferno2.normalizeStyles)({
                visibility: pagesContainerVisibility
            })
        })]
    })))
};
exports.viewFunction = viewFunction;
var PagerContentProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_pager_props.InternalPagerProps), Object.getOwnPropertyDescriptors({
    infoTextVisible: true,
    isLargeDisplayMode: true
})));
exports.PagerContentProps = PagerContentProps;
var PagerContent = function(_InfernoComponent) {
    _inheritsLoose(PagerContent, _InfernoComponent);

    function PagerContent(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.widgetRootElementRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        _this.setRootElementRef = _this.setRootElementRef.bind(_assertThisInitialized(_this));
        _this.createFakeInstance = _this.createFakeInstance.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = PagerContent.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.setRootElementRef, [])]
    };
    _proto.getChildContext = function() {
        return _extends({}, this.context, {
            KeyboardActionContext: this.keyboardAction
        })
    };
    _proto.setRootElementRef = function() {
        var rootElementRef = this.props.rootElementRef;
        if (rootElementRef) {
            rootElementRef.current = this.widgetRootElementRef.current
        }
    };
    _proto.createFakeInstance = function() {
        var _this2 = this;
        return {
            option: function() {
                return false
            },
            element: function() {
                return _this2.widgetRootElementRef.current
            },
            _createActionByOption: function() {
                return function(e) {
                    var _this2$props$onKeyDow, _this2$props;
                    null === (_this2$props$onKeyDow = (_this2$props = _this2.props).onKeyDown) || void 0 === _this2$props$onKeyDow ? void 0 : _this2$props$onKeyDow.call(_this2$props, e)
                }
            }
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoComponent.prototype.componentWillUpdate.call(this);
        if (this.props.onKeyDown !== nextProps.onKeyDown) {
            this.__getterCache.keyboardAction = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            widgetRootElementRef: this.widgetRootElementRef,
            keyboardAction: this.keyboardAction,
            infoVisible: this.infoVisible,
            pageIndexSelectorVisible: this.pageIndexSelectorVisible,
            pagesContainerVisible: this.pagesContainerVisible,
            pagesContainerVisibility: this.pagesContainerVisibility,
            isLargeDisplayMode: this.isLargeDisplayMode,
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    };
    _createClass(PagerContent, [{
        key: "keyboardAction",
        get: function() {
            var _this3 = this;
            if (void 0 !== this.__getterCache.keyboardAction) {
                return this.__getterCache.keyboardAction
            }
            return this.__getterCache.keyboardAction = {
                registerKeyboardAction: function(element, action) {
                    var fakePagerInstance = _this3.createFakeInstance();
                    return (0, _accessibility.registerKeyboardAction)("pager", fakePagerInstance, element, void 0, action)
                }
            }
        }
    }, {
        key: "infoVisible",
        get: function() {
            var _this$props = this.props,
                infoTextVisible = _this$props.infoTextVisible,
                showInfo = _this$props.showInfo;
            return showInfo && infoTextVisible && this.isLargeDisplayMode
        }
    }, {
        key: "pageIndexSelectorVisible",
        get: function() {
            return 0 !== this.props.pageSize
        }
    }, {
        key: "normalizedDisplayMode",
        get: function() {
            var _this$props2 = this.props,
                displayMode = _this$props2.displayMode,
                lightModeEnabled = _this$props2.lightModeEnabled;
            if ("adaptive" === displayMode && void 0 !== lightModeEnabled) {
                return lightModeEnabled ? "compact" : "full"
            }
            return displayMode
        }
    }, {
        key: "pagesContainerVisible",
        get: function() {
            return !!this.props.pagesNavigatorVisible && this.props.pageCount > 0
        }
    }, {
        key: "pagesContainerVisibility",
        get: function() {
            if ("auto" === this.props.pagesNavigatorVisible && 1 === this.props.pageCount && this.props.hasKnownLastPage) {
                return "hidden"
            }
            return
        }
    }, {
        key: "isLargeDisplayMode",
        get: function() {
            var displayMode = this.normalizedDisplayMode;
            var result = false;
            if ("adaptive" === displayMode) {
                result = this.props.isLargeDisplayMode
            } else {
                result = "full" === displayMode
            }
            return result
        }
    }, {
        key: "classes",
        get: function() {
            var _classesMap;
            var classesMap = (_classesMap = {}, _defineProperty(_classesMap, "".concat(this.props.className), !!this.props.className), _defineProperty(_classesMap, _consts.PAGER_CLASS, true), _defineProperty(_classesMap, _consts.LIGHT_MODE_CLASS, !this.isLargeDisplayMode), _classesMap);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props3 = this.props,
                restProps = (_this$props3.className, _this$props3.displayMode, _this$props3.gridCompatibility, _this$props3.hasKnownLastPage, _this$props3.infoText, _this$props3.infoTextRef, _this$props3.infoTextVisible, _this$props3.isLargeDisplayMode, _this$props3.lightModeEnabled, _this$props3.maxPagesCount, _this$props3.onKeyDown, _this$props3.pageCount, _this$props3.pageIndex, _this$props3.pageIndexChange, _this$props3.pageSize, _this$props3.pageSizeChange, _this$props3.pageSizes, _this$props3.pageSizesRef, _this$props3.pagesCountText, _this$props3.pagesNavigatorVisible, _this$props3.pagesRef, _this$props3.rootElementRef, _this$props3.rtlEnabled, _this$props3.showInfo, _this$props3.showNavigationButtons, _this$props3.showPageSizes, _this$props3.totalCount, _this$props3.visible, _objectWithoutProperties(_this$props3, _excluded));
            return restProps
        }
    }]);
    return PagerContent
}(_inferno2.InfernoComponent);
exports.PagerContent = PagerContent;
PagerContent.defaultProps = PagerContentProps;
