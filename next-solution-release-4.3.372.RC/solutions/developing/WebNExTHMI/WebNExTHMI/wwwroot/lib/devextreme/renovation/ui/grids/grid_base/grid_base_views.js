/**
 * DevExtreme (renovation/ui/grids/grid_base/grid_base_views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.GridBaseViews = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _combine_classes = require("../../../utils/combine_classes");
var _grid_base_view_wrapper = require("./grid_base_view_wrapper");
var _data_grid_props = require("../data_grid/common/data_grid_props");
var _excluded = ["className", "onRendered", "role", "showBorders", "views"];

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
var GRIDBASE_CONTAINER_CLASS = "dx-gridbase-container";
var BORDERS_CLASS = "borders";
var viewFunction = function(_ref) {
    var className = _ref.className,
        views = _ref.props.views,
        viewRendered = _ref.viewRendered;
    return (0, _inferno.createVNode)(1, "div", className, views.map((function(_ref2) {
        var name = _ref2.name,
            view = _ref2.view;
        return (0, _inferno.createComponentVNode)(2, _grid_base_view_wrapper.GridBaseViewWrapper, {
            view: view,
            onRendered: viewRendered
        }, name)
    })), 0, {
        role: "grid"
    })
};
exports.viewFunction = viewFunction;
var GridBaseViewPropsType = Object.defineProperties({}, {
    showBorders: {
        get: function() {
            return _data_grid_props.DataGridProps.showBorders
        },
        configurable: true,
        enumerable: true
    }
});
var GridBaseViews = function(_BaseInfernoComponent) {
    _inheritsLoose(GridBaseViews, _BaseInfernoComponent);

    function GridBaseViews(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.viewRenderedCount = 0;
        _this.viewRendered = _this.viewRendered.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = GridBaseViews.prototype;
    _proto.viewRendered = function() {
        this.viewRenderedCount += 1;
        if (this.viewRenderedCount === this.props.views.length) {
            var _this$props$onRendere, _this$props;
            null === (_this$props$onRendere = (_this$props = this.props).onRendered) || void 0 === _this$props$onRendere ? void 0 : _this$props$onRendere.call(_this$props)
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            className: this.className,
            viewRendered: this.viewRendered,
            restAttributes: this.restAttributes
        })
    };
    _createClass(GridBaseViews, [{
        key: "className",
        get: function() {
            var _combineClasses;
            var showBorders = this.props.showBorders;
            return (0, _combine_classes.combineClasses)((_combineClasses = {}, _defineProperty(_combineClasses, GRIDBASE_CONTAINER_CLASS, true), _defineProperty(_combineClasses, "".concat(this.props.className), !!this.props.className), _defineProperty(_combineClasses, "".concat(this.props.className, "-").concat(BORDERS_CLASS), !!showBorders), _combineClasses))
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.className, _this$props2.onRendered, _this$props2.role, _this$props2.showBorders, _this$props2.views, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return GridBaseViews
}(_inferno2.BaseInfernoComponent);
exports.GridBaseViews = GridBaseViews;
GridBaseViews.defaultProps = GridBaseViewPropsType;
