/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/row.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.RowProps = exports.Row = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _virtual_cell = require("./virtual_cell");
var _excluded = ["children", "className", "isHeaderRow", "leftVirtualCellCount", "leftVirtualCellWidth", "rightVirtualCellCount", "rightVirtualCellWidth", "styles"];

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
    var hasLeftVirtualCell = _ref.hasLeftVirtualCell,
        hasRightVirtualCell = _ref.hasRightVirtualCell,
        _ref$props = _ref.props,
        children = _ref$props.children,
        className = _ref$props.className,
        isHeaderRow = _ref$props.isHeaderRow,
        leftVirtualCellCount = _ref$props.leftVirtualCellCount,
        leftVirtualCellWidth = _ref$props.leftVirtualCellWidth,
        rightVirtualCellCount = _ref$props.rightVirtualCellCount,
        rightVirtualCellWidth = _ref$props.rightVirtualCellWidth,
        styles = _ref$props.styles;
    return (0, _inferno.createVNode)(1, "tr", className, [hasLeftVirtualCell && (0, _inferno.createComponentVNode)(2, _virtual_cell.VirtualCell, {
        width: leftVirtualCellWidth,
        colSpan: leftVirtualCellCount,
        isHeaderCell: isHeaderRow
    }), children, hasRightVirtualCell && (0, _inferno.createComponentVNode)(2, _virtual_cell.VirtualCell, {
        width: rightVirtualCellWidth,
        colSpan: rightVirtualCellCount,
        isHeaderCell: isHeaderRow
    })], 0, {
        style: (0, _inferno2.normalizeStyles)(styles)
    })
};
exports.viewFunction = viewFunction;
var RowProps = {
    className: "",
    leftVirtualCellWidth: 0,
    rightVirtualCellWidth: 0,
    isHeaderRow: false
};
exports.RowProps = RowProps;
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
            props: _extends({}, props),
            hasLeftVirtualCell: this.hasLeftVirtualCell,
            hasRightVirtualCell: this.hasRightVirtualCell,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Row, [{
        key: "hasLeftVirtualCell",
        get: function() {
            var leftVirtualCellCount = this.props.leftVirtualCellCount;
            return !!leftVirtualCellCount
        }
    }, {
        key: "hasRightVirtualCell",
        get: function() {
            var rightVirtualCellCount = this.props.rightVirtualCellCount;
            return !!rightVirtualCellCount
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.children, _this$props.className, _this$props.isHeaderRow, _this$props.leftVirtualCellCount, _this$props.leftVirtualCellWidth, _this$props.rightVirtualCellCount, _this$props.rightVirtualCellWidth, _this$props.styles, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Row
}(_inferno2.BaseInfernoComponent);
exports.Row = Row;
Row.defaultProps = RowProps;
