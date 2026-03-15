/**
 * DevExtreme (renovation/ui/pager/pager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Pager = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _resizable_container = require("./resizable_container");
var _pager_props = require("./common/pager_props");
var _content = require("./content");
var _combine_classes = require("../../utils/combine_classes");
var _excluded = ["className", "defaultPageIndex", "defaultPageSize", "displayMode", "gridCompatibility", "hasKnownLastPage", "infoText", "lightModeEnabled", "maxPagesCount", "onKeyDown", "pageCount", "pageIndex", "pageIndexChange", "pageSize", "pageSizeChange", "pageSizes", "pagesCountText", "pagesNavigatorVisible", "rtlEnabled", "showInfo", "showNavigationButtons", "showPageSizes", "totalCount", "visible"];

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
    var pagerProps = _ref.pagerProps,
        restAttributes = _ref.restAttributes;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _resizable_container.ResizableContainer, _extends({
        contentTemplate: _content.PagerContent,
        pagerProps: pagerProps
    }, restAttributes)))
};
exports.viewFunction = viewFunction;
var Pager = function(_InfernoWrapperCompon) {
    _inheritsLoose(Pager, _InfernoWrapperCompon);

    function Pager(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.__getterCache = {};
        _this.state = {
            pageSize: void 0 !== _this.props.pageSize ? _this.props.pageSize : _this.props.defaultPageSize,
            pageIndex: void 0 !== _this.props.pageIndex ? _this.props.pageIndex : _this.props.defaultPageIndex
        };
        _this.pageIndexChange = _this.pageIndexChange.bind(_assertThisInitialized(_this));
        _this.pageSizeChange = _this.pageSizeChange.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Pager.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.pageIndexChange = function(newPageIndex) {
        if (this.props.gridCompatibility) {
            var __newValue;
            this.setState((function(__state_argument) {
                __newValue = newPageIndex + 1;
                return {
                    pageIndex: __newValue
                }
            }));
            this.props.pageIndexChange(__newValue)
        } else {
            var _newValue;
            this.setState((function(__state_argument) {
                _newValue = newPageIndex;
                return {
                    pageIndex: _newValue
                }
            }));
            this.props.pageIndexChange(_newValue)
        }
    };
    _proto.pageSizeChange = function(newPageSize) {
        var __newValue;
        this.setState((function(__state_argument) {
            __newValue = newPageSize;
            return {
                pageSize: __newValue
            }
        }));
        this.props.pageSizeChange(__newValue)
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoWrapperCompon.prototype.componentWillUpdate.call(this);
        if (this.props !== nextProps || this.props.gridCompatibility !== nextProps.gridCompatibility || this.props.className !== nextProps.className || this.state.pageIndex !== nextState.pageIndex || this.props.pageIndex !== nextProps.pageIndex || this.props.pageIndexChange !== nextProps.pageIndexChange || this.props.pageSizeChange !== nextProps.pageSizeChange) {
            this.__getterCache.pagerProps = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.state.pageSize,
                pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
            }),
            pageIndexChange: this.pageIndexChange,
            pageIndex: this.pageIndex,
            pageSizeChange: this.pageSizeChange,
            className: this.className,
            pagerProps: this.pagerProps,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Pager, [{
        key: "pageIndex",
        get: function() {
            if (this.props.gridCompatibility) {
                return (void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex) - 1
            }
            return void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
        }
    }, {
        key: "className",
        get: function() {
            if (this.props.gridCompatibility) {
                return (0, _combine_classes.combineClasses)(_defineProperty({
                    "dx-datagrid-pager": true
                }, "".concat(this.props.className), !!this.props.className))
            }
            return this.props.className
        }
    }, {
        key: "pagerProps",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.pagerProps) {
                return this.__getterCache.pagerProps
            }
            return this.__getterCache.pagerProps = _extends({}, _extends({}, _this2.props, {
                pageSize: void 0 !== _this2.props.pageSize ? _this2.props.pageSize : _this2.state.pageSize,
                pageIndex: void 0 !== _this2.props.pageIndex ? _this2.props.pageIndex : _this2.state.pageIndex
            }), {
                className: _this2.className,
                pageIndex: _this2.pageIndex,
                pageIndexChange: function(pageIndex) {
                    return _this2.pageIndexChange(pageIndex)
                },
                pageSizeChange: function(pageSize) {
                    return _this2.pageSizeChange(pageSize)
                }
            })
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$pageSize$ = _extends({}, this.props, {
                    pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.state.pageSize,
                    pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
                }),
                restProps = (_this$props$pageSize$.className, _this$props$pageSize$.defaultPageIndex, _this$props$pageSize$.defaultPageSize, _this$props$pageSize$.displayMode, _this$props$pageSize$.gridCompatibility, _this$props$pageSize$.hasKnownLastPage, _this$props$pageSize$.infoText, _this$props$pageSize$.lightModeEnabled, _this$props$pageSize$.maxPagesCount, _this$props$pageSize$.onKeyDown, _this$props$pageSize$.pageCount, _this$props$pageSize$.pageIndex, _this$props$pageSize$.pageIndexChange, _this$props$pageSize$.pageSize, _this$props$pageSize$.pageSizeChange, _this$props$pageSize$.pageSizes, _this$props$pageSize$.pagesCountText, _this$props$pageSize$.pagesNavigatorVisible, _this$props$pageSize$.rtlEnabled, _this$props$pageSize$.showInfo, _this$props$pageSize$.showNavigationButtons, _this$props$pageSize$.showPageSizes, _this$props$pageSize$.totalCount, _this$props$pageSize$.visible, _objectWithoutProperties(_this$props$pageSize$, _excluded));
            return restProps
        }
    }]);
    return Pager
}(_inferno2.InfernoWrapperComponent);
exports.Pager = Pager;
Pager.defaultProps = _pager_props.PagerProps;
