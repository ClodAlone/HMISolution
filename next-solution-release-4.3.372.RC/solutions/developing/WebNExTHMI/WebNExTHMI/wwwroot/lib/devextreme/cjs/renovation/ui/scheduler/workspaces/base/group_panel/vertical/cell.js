/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/group_panel/vertical/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.GroupPanelVerticalCell = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _cell_props = require("../cell_props");
var _excluded = ["cellTemplate", "className", "color", "data", "id", "index", "text"];

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
var viewFunction = function(viewModel) {
    var CellTemplate = viewModel.props.cellTemplate;
    return (0, _inferno.createVNode)(1, "div", "dx-scheduler-group-header ".concat(viewModel.props.className), [!!viewModel.props.cellTemplate && CellTemplate({
        data: {
            data: viewModel.props.data,
            id: viewModel.props.id,
            color: viewModel.props.color,
            text: viewModel.props.text
        },
        index: viewModel.props.index
    }), !viewModel.props.cellTemplate && (0, _inferno.createVNode)(1, "div", "dx-scheduler-group-header-content", viewModel.props.text, 0)], 0)
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var GroupPanelVerticalCell = function(_BaseInfernoComponent) {
    _inheritsLoose(GroupPanelVerticalCell, _BaseInfernoComponent);

    function GroupPanelVerticalCell(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = GroupPanelVerticalCell.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                cellTemplate: getTemplate(props.cellTemplate)
            }),
            restAttributes: this.restAttributes
        })
    };
    _createClass(GroupPanelVerticalCell, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.cellTemplate, _this$props.className, _this$props.color, _this$props.data, _this$props.id, _this$props.index, _this$props.text, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return GroupPanelVerticalCell
}(_inferno2.BaseInfernoComponent);
exports.GroupPanelVerticalCell = GroupPanelVerticalCell;
GroupPanelVerticalCell.defaultProps = _cell_props.GroupPanelCellProps;
