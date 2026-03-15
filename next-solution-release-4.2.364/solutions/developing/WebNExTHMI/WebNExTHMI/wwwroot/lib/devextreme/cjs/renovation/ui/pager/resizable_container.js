/**
 * DevExtreme (cjs/renovation/ui/pager/resizable_container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ResizableContainerProps = exports.ResizableContainer = void 0;
exports.calculateAdaptivityProps = calculateAdaptivityProps;
exports.viewFunction = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _resize_callbacks = _interopRequireDefault(require("../../../core/utils/resize_callbacks"));
var _get_element_width = require("./utils/get_element_width");
var _type = require("../../../core/utils/type");
var _excluded = ["contentTemplate", "pagerProps"];

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
var viewFunction = function(_ref) {
    var contentAttributes = _ref.contentAttributes,
        infoTextRef = _ref.infoTextRef,
        infoTextVisible = _ref.infoTextVisible,
        isLargeDisplayMode = _ref.isLargeDisplayMode,
        pageSizesRef = _ref.pageSizesRef,
        pagesRef = _ref.pagesRef,
        parentRef = _ref.parentRef,
        Content = _ref.props.contentTemplate;
    return Content(_extends({
        rootElementRef: parentRef,
        pageSizesRef: pageSizesRef,
        infoTextRef: infoTextRef,
        pagesRef: pagesRef,
        infoTextVisible: infoTextVisible,
        isLargeDisplayMode: isLargeDisplayMode
    }, contentAttributes))
};
exports.viewFunction = viewFunction;

function calculateAdaptivityProps(_ref2) {
    var infoWidth = _ref2.info,
        pageSizesWidth = _ref2.pageSizes,
        pagesWidth = _ref2.pages,
        parentWidth = _ref2.parent;
    var minimalWidth = pageSizesWidth + pagesWidth + infoWidth;
    var infoTextVisible = parentWidth - minimalWidth > 0;
    var isLargeDisplayMode = parentWidth - (pageSizesWidth + pagesWidth) > 0;
    return {
        infoTextVisible: infoTextVisible,
        isLargeDisplayMode: isLargeDisplayMode
    }
}

function getElementsWidth(_ref3) {
    var info = _ref3.info,
        pageSizes = _ref3.pageSizes,
        pages = _ref3.pages,
        parent = _ref3.parent;
    var parentWidth = (0, _get_element_width.getElementWidth)(parent);
    var pageSizesWidth = (0, _get_element_width.getElementWidth)(pageSizes);
    var infoWidth = (0, _get_element_width.getElementWidth)(info);
    var pagesHtmlWidth = (0, _get_element_width.getElementWidth)(pages);
    return {
        parent: parentWidth,
        pageSizes: pageSizesWidth,
        info: infoWidth + (0, _get_element_width.getElementStyle)("marginLeft", info) + (0, _get_element_width.getElementStyle)("marginRight", info),
        pages: pagesHtmlWidth
    }
}
var ResizableContainerProps = {};
exports.ResizableContainerProps = ResizableContainerProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var ResizableContainer = function(_InfernoComponent) {
    _inheritsLoose(ResizableContainer, _InfernoComponent);

    function ResizableContainer(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.parentRef = (0, _inferno.createRef)();
        _this.pageSizesRef = (0, _inferno.createRef)();
        _this.infoTextRef = (0, _inferno.createRef)();
        _this.pagesRef = (0, _inferno.createRef)();
        _this.state = {
            infoTextVisible: true,
            isLargeDisplayMode: true
        };
        _this.subscribeToResize = _this.subscribeToResize.bind(_assertThisInitialized(_this));
        _this.effectUpdateChildProps = _this.effectUpdateChildProps.bind(_assertThisInitialized(_this));
        _this.updateAdaptivityProps = _this.updateAdaptivityProps.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = ResizableContainer.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.subscribeToResize, [this.state.infoTextVisible, this.state.isLargeDisplayMode]), new _inferno2.InfernoEffect(this.effectUpdateChildProps, [this.props, this.state.infoTextVisible, this.state.isLargeDisplayMode, this.props.pagerProps, this.props.contentTemplate])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.infoTextVisible, this.state.isLargeDisplayMode]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props, this.state.infoTextVisible, this.state.isLargeDisplayMode, this.props.pagerProps, this.props.contentTemplate])
    };
    _proto.subscribeToResize = function() {
        var _this2 = this;
        var callback = function() {
            _this2.parentWidth > 0 && _this2.updateAdaptivityProps()
        };
        _resize_callbacks.default.add(callback);
        return function() {
            _resize_callbacks.default.remove(callback)
        }
    };
    _proto.effectUpdateChildProps = function() {
        if (this.parentWidth > 0) {
            this.updateAdaptivityProps()
        }
    };
    _proto.updateAdaptivityProps = function() {
        var _this3 = this;
        var currentElementsWidth = getElementsWidth({
            parent: this.parentRef.current,
            pageSizes: this.pageSizesRef.current,
            info: this.infoTextRef.current,
            pages: this.pagesRef.current
        });
        if ((0, _type.isDefined)(this.actualAdaptivityProps) && (this.actualAdaptivityProps.infoTextVisible !== this.state.infoTextVisible || this.actualAdaptivityProps.isLargeDisplayMode !== this.state.isLargeDisplayMode)) {
            return
        }
        var isEmpty = !(0, _type.isDefined)(this.elementsWidth);
        if (isEmpty) {
            this.elementsWidth = {}
        }
        if (isEmpty || this.state.isLargeDisplayMode) {
            this.elementsWidth.pageSizes = currentElementsWidth.pageSizes;
            this.elementsWidth.pages = currentElementsWidth.pages
        }
        if (isEmpty || this.state.infoTextVisible) {
            this.elementsWidth.info = currentElementsWidth.info
        }
        this.actualAdaptivityProps = calculateAdaptivityProps(_extends({
            parent: currentElementsWidth.parent
        }, this.elementsWidth));
        this.setState((function(__state_argument) {
            return {
                infoTextVisible: _this3.actualAdaptivityProps.infoTextVisible
            }
        }));
        this.setState((function(__state_argument) {
            return {
                isLargeDisplayMode: _this3.actualAdaptivityProps.isLargeDisplayMode
            }
        }))
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            infoTextVisible: this.state.infoTextVisible,
            isLargeDisplayMode: this.state.isLargeDisplayMode,
            parentRef: this.parentRef,
            pageSizesRef: this.pageSizesRef,
            infoTextRef: this.infoTextRef,
            pagesRef: this.pagesRef,
            contentAttributes: this.contentAttributes,
            parentWidth: this.parentWidth,
            updateAdaptivityProps: this.updateAdaptivityProps,
            restAttributes: this.restAttributes
        })
    };
    _createClass(ResizableContainer, [{
        key: "contentAttributes",
        get: function() {
            var _this$props$pagerProp = this.props.pagerProps,
                className = _this$props$pagerProp.className,
                displayMode = _this$props$pagerProp.displayMode,
                gridCompatibility = _this$props$pagerProp.gridCompatibility,
                hasKnownLastPage = _this$props$pagerProp.hasKnownLastPage,
                infoText = _this$props$pagerProp.infoText,
                lightModeEnabled = _this$props$pagerProp.lightModeEnabled,
                maxPagesCount = _this$props$pagerProp.maxPagesCount,
                onKeyDown = _this$props$pagerProp.onKeyDown,
                pageCount = _this$props$pagerProp.pageCount,
                pageIndex = _this$props$pagerProp.pageIndex,
                pageIndexChange = _this$props$pagerProp.pageIndexChange,
                pageSize = _this$props$pagerProp.pageSize,
                pageSizeChange = _this$props$pagerProp.pageSizeChange,
                pageSizes = _this$props$pagerProp.pageSizes,
                pagesCountText = _this$props$pagerProp.pagesCountText,
                pagesNavigatorVisible = _this$props$pagerProp.pagesNavigatorVisible,
                rtlEnabled = _this$props$pagerProp.rtlEnabled,
                showInfo = _this$props$pagerProp.showInfo,
                showNavigationButtons = _this$props$pagerProp.showNavigationButtons,
                showPageSizes = _this$props$pagerProp.showPageSizes,
                totalCount = _this$props$pagerProp.totalCount,
                visible = _this$props$pagerProp.visible;
            return _extends({}, this.restAttributes, {
                pageSize: pageSize,
                pageIndex: pageIndex,
                pageIndexChange: pageIndexChange,
                pageSizeChange: pageSizeChange,
                gridCompatibility: gridCompatibility,
                className: className,
                showInfo: showInfo,
                infoText: infoText,
                lightModeEnabled: lightModeEnabled,
                displayMode: displayMode,
                maxPagesCount: maxPagesCount,
                pageCount: pageCount,
                pagesCountText: pagesCountText,
                visible: visible,
                hasKnownLastPage: hasKnownLastPage,
                pagesNavigatorVisible: pagesNavigatorVisible,
                showPageSizes: showPageSizes,
                pageSizes: pageSizes,
                rtlEnabled: rtlEnabled,
                showNavigationButtons: showNavigationButtons,
                totalCount: totalCount,
                onKeyDown: onKeyDown
            })
        }
    }, {
        key: "parentWidth",
        get: function() {
            return this.parentRef.current ? (0, _get_element_width.getElementWidth)(this.parentRef.current) : 0
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.contentTemplate, _this$props.pagerProps, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return ResizableContainer
}(_inferno2.InfernoComponent);
exports.ResizableContainer = ResizableContainer;
ResizableContainer.defaultProps = ResizableContainerProps;
