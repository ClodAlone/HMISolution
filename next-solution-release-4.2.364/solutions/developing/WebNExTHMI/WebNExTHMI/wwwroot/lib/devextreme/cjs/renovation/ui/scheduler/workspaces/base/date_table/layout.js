/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/date_table/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.DateTableLayoutProps = exports.DateTableLayoutBase = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _table = require("../table");
var _table_body = require("./table_body");
var _layout_props = require("../layout_props");
var _cell = require("./cell");
var _excluded = ["addDateTableClass", "addVerticalSizesClassToRows", "bottomVirtualRowHeight", "cellTemplate", "dataCellTemplate", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "tableRef", "topVirtualRowHeight", "viewData", "width"];

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
    var bottomVirtualRowHeight = _ref.bottomVirtualRowHeight,
        classes = _ref.classes,
        leftVirtualCellWidth = _ref.leftVirtualCellWidth,
        _ref$props = _ref.props,
        addVerticalSizesClassToRows = _ref$props.addVerticalSizesClassToRows,
        cellTemplate = _ref$props.cellTemplate,
        dataCellTemplate = _ref$props.dataCellTemplate,
        groupOrientation = _ref$props.groupOrientation,
        tableRef = _ref$props.tableRef,
        viewData = _ref$props.viewData,
        width = _ref$props.width,
        restAttributes = _ref.restAttributes,
        rightVirtualCellWidth = _ref.rightVirtualCellWidth,
        topVirtualRowHeight = _ref.topVirtualRowHeight,
        virtualCellsCount = _ref.virtualCellsCount;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _table.Table, _extends({}, restAttributes, {
        tableRef: tableRef,
        topVirtualRowHeight: topVirtualRowHeight,
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        leftVirtualCellWidth: leftVirtualCellWidth,
        rightVirtualCellWidth: rightVirtualCellWidth,
        leftVirtualCellCount: viewData.leftVirtualCellCount,
        rightVirtualCellCount: viewData.rightVirtualCellCount,
        virtualCellsCount: virtualCellsCount,
        className: classes,
        width: width,
        children: (0, _inferno.createComponentVNode)(2, _table_body.DateTableBody, {
            cellTemplate: cellTemplate,
            viewData: viewData,
            dataCellTemplate: dataCellTemplate,
            leftVirtualCellWidth: leftVirtualCellWidth,
            rightVirtualCellWidth: rightVirtualCellWidth,
            groupOrientation: groupOrientation,
            addVerticalSizesClassToRows: addVerticalSizesClassToRows
        })
    })))
};
exports.viewFunction = viewFunction;
var DateTableLayoutProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_layout_props.LayoutProps), Object.getOwnPropertyDescriptors({
    cellTemplate: _cell.DateTableCellBase
})));
exports.DateTableLayoutProps = DateTableLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var DateTableLayoutBase = function(_InfernoWrapperCompon) {
    _inheritsLoose(DateTableLayoutBase, _InfernoWrapperCompon);

    function DateTableLayoutBase(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = DateTableLayoutBase.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                cellTemplate: getTemplate(props.cellTemplate),
                dataCellTemplate: getTemplate(props.dataCellTemplate)
            }),
            classes: this.classes,
            topVirtualRowHeight: this.topVirtualRowHeight,
            bottomVirtualRowHeight: this.bottomVirtualRowHeight,
            leftVirtualCellWidth: this.leftVirtualCellWidth,
            rightVirtualCellWidth: this.rightVirtualCellWidth,
            virtualCellsCount: this.virtualCellsCount,
            restAttributes: this.restAttributes
        })
    };
    _createClass(DateTableLayoutBase, [{
        key: "classes",
        get: function() {
            var addDateTableClass = this.props.addDateTableClass;
            return addDateTableClass ? "dx-scheduler-date-table" : void 0
        }
    }, {
        key: "topVirtualRowHeight",
        get: function() {
            var _this$props$viewData$;
            return null !== (_this$props$viewData$ = this.props.viewData.topVirtualRowHeight) && void 0 !== _this$props$viewData$ ? _this$props$viewData$ : 0
        }
    }, {
        key: "bottomVirtualRowHeight",
        get: function() {
            var _this$props$viewData$2;
            return null !== (_this$props$viewData$2 = this.props.viewData.bottomVirtualRowHeight) && void 0 !== _this$props$viewData$2 ? _this$props$viewData$2 : 0
        }
    }, {
        key: "leftVirtualCellWidth",
        get: function() {
            var _this$props$viewData$3;
            return null !== (_this$props$viewData$3 = this.props.viewData.leftVirtualCellWidth) && void 0 !== _this$props$viewData$3 ? _this$props$viewData$3 : 0
        }
    }, {
        key: "rightVirtualCellWidth",
        get: function() {
            var _this$props$viewData$4;
            return null !== (_this$props$viewData$4 = this.props.viewData.rightVirtualCellWidth) && void 0 !== _this$props$viewData$4 ? _this$props$viewData$4 : 0
        }
    }, {
        key: "virtualCellsCount",
        get: function() {
            return this.props.viewData.groupedData[0].dateTable[0].length
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.addDateTableClass, _this$props.addVerticalSizesClassToRows, _this$props.bottomVirtualRowHeight, _this$props.cellTemplate, _this$props.dataCellTemplate, _this$props.groupOrientation, _this$props.leftVirtualCellWidth, _this$props.rightVirtualCellWidth, _this$props.tableRef, _this$props.topVirtualRowHeight, _this$props.viewData, _this$props.width, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return DateTableLayoutBase
}(_inferno2.InfernoWrapperComponent);
exports.DateTableLayoutBase = DateTableLayoutBase;
DateTableLayoutBase.defaultProps = DateTableLayoutProps;
