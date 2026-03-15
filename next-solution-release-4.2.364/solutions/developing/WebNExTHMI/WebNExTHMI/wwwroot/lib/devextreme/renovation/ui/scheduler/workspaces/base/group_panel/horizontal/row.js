/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/group_panel/horizontal/row.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Row = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _cell = require("./cell");
var _row_props = require("../row_props");
var _excluded = ["cellTemplate", "className", "groupItems"];

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
var viewFunction = function(_ref) {
    var _ref$props = _ref.props,
        cellTemplate = _ref$props.cellTemplate,
        className = _ref$props.className,
        groupItems = _ref$props.groupItems;
    return (0, _inferno.createVNode)(1, "tr", "dx-scheduler-group-row ".concat(className), groupItems.map((function(_ref2, index) {
        var colSpan = _ref2.colSpan,
            color = _ref2.color,
            data = _ref2.data,
            id = _ref2.id,
            isFirstGroupCell = _ref2.isFirstGroupCell,
            isLastGroupCell = _ref2.isLastGroupCell,
            key = _ref2.key,
            text = _ref2.text;
        return (0, _inferno.createComponentVNode)(2, _cell.GroupPanelHorizontalCell, {
            text: text,
            id: id,
            data: data,
            index: index,
            color: color,
            colSpan: colSpan,
            isFirstGroupCell: !!isFirstGroupCell,
            isLastGroupCell: !!isLastGroupCell,
            cellTemplate: cellTemplate
        }, key)
    })), 0)
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var Row = function(_BaseInfernoComponent) {
    _inheritsLoose(Row, _BaseInfernoComponent);

    function Row(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Row.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                cellTemplate: getTemplate(props.cellTemplate)
            }),
            restAttributes: this.restAttributes
        })
    };
    _createClass(Row, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.cellTemplate, _this$props.className, _this$props.groupItems, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Row
}(_inferno2.BaseInfernoComponent);
exports.Row = Row;
Row.defaultProps = _row_props.GroupPanelRowProps;
