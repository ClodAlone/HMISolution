/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.AllDayPanelLayoutProps = exports.AllDayPanelLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _layout_props = require("../../layout_props");
var _table = require("./table");
var _excluded = ["addDateTableClass", "addVerticalSizesClassToRows", "allDayAppointments", "bottomVirtualRowHeight", "dataCellTemplate", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "tableRef", "topVirtualRowHeight", "viewData", "width"];

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
        allDayAppointments = _ref$props.allDayAppointments,
        dataCellTemplate = _ref$props.dataCellTemplate,
        tableRef = _ref$props.tableRef,
        viewData = _ref$props.viewData,
        width = _ref$props.width;
    return (0, _inferno.createVNode)(1, "div", "dx-scheduler-all-day-panel", [allDayAppointments, (0, _inferno.createComponentVNode)(2, _table.AllDayTable, {
        tableRef: tableRef,
        viewData: viewData,
        dataCellTemplate: dataCellTemplate,
        width: width
    })], 0)
};
exports.viewFunction = viewFunction;
var AllDayPanelLayoutProps = _layout_props.LayoutProps;
exports.AllDayPanelLayoutProps = AllDayPanelLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var AllDayPanelLayout = function(_BaseInfernoComponent) {
    _inheritsLoose(AllDayPanelLayout, _BaseInfernoComponent);

    function AllDayPanelLayout(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = AllDayPanelLayout.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate)
            }),
            restAttributes: this.restAttributes
        })
    };
    _createClass(AllDayPanelLayout, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.addDateTableClass, _this$props.addVerticalSizesClassToRows, _this$props.allDayAppointments, _this$props.bottomVirtualRowHeight, _this$props.dataCellTemplate, _this$props.groupOrientation, _this$props.leftVirtualCellWidth, _this$props.rightVirtualCellWidth, _this$props.tableRef, _this$props.topVirtualRowHeight, _this$props.viewData, _this$props.width, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return AllDayPanelLayout
}(_inferno2.BaseInfernoComponent);
exports.AllDayPanelLayout = AllDayPanelLayout;
AllDayPanelLayout.defaultProps = AllDayPanelLayoutProps;
