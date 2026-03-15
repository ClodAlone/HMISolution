/**
 * DevExtreme (cjs/renovation/ui/grids/data_grid/data_grid_views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.DataGridViews = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _grid_base_views = require("../grid_base/grid_base_views");
var _uiGrid_core = require("../../../../ui/grid_core/ui.grid_core.grid_view");
var _data_grid_props = require("./common/data_grid_props");
var _common = require("../../../../core/utils/common");
var _window = require("../../../../core/utils/window");
var _excluded = ["instance", "showBorders"];

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
var VIEW_NAMES = _uiGrid_core.gridViewModule.VIEW_NAMES;
var DATA_GRID_CLASS = "dx-datagrid";
var DATA_GRID_ROLE_NAME = "grid";
var viewFunction = function(_ref) {
    var showBorders = _ref.props.showBorders,
        update = _ref.update,
        views = _ref.views;
    return (0, _inferno.createComponentVNode)(2, _grid_base_views.GridBaseViews, {
        views: views,
        className: DATA_GRID_CLASS,
        showBorders: showBorders,
        role: DATA_GRID_ROLE_NAME,
        onRendered: update
    })
};
exports.viewFunction = viewFunction;
var DataGridPropsType = Object.defineProperties({}, {
    showBorders: {
        get: function() {
            return _data_grid_props.DataGridProps.showBorders
        },
        configurable: true,
        enumerable: true
    }
});
var DataGridViews = function(_BaseInfernoComponent) {
    _inheritsLoose(DataGridViews, _BaseInfernoComponent);

    function DataGridViews(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        _this.update = _this.update.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = DataGridViews.prototype;
    _proto.update = function() {
        var gridInstance = this.props.instance;
        var dataController = gridInstance.getController("data");
        var resizingController = gridInstance.getController("resizing");
        if ((0, _window.hasWindow)()) {
            (0, _common.deferRender)((function() {
                resizingController.resize();
                if (dataController.isLoaded()) {
                    resizingController.fireContentReadyAction()
                }
            }))
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.instance !== nextProps.instance) {
            this.__getterCache.views = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            views: this.views,
            update: this.update,
            restAttributes: this.restAttributes
        })
    };
    _createClass(DataGridViews, [{
        key: "views",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.views) {
                return this.__getterCache.views
            }
            return this.__getterCache.views = function() {
                if (!_this2.props.instance) {
                    return []
                }
                var views = VIEW_NAMES.map((function(viewName) {
                    return _this2.props.instance.getView(viewName)
                })).filter((function(view) {
                    return view
                }));
                return views.map((function(view) {
                    return {
                        name: view.name,
                        view: view
                    }
                }))
            }()
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.instance, _this$props.showBorders, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return DataGridViews
}(_inferno2.BaseInfernoComponent);
exports.DataGridViews = DataGridViews;
DataGridViews.defaultProps = DataGridPropsType;
