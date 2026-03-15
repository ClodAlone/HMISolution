/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/header_panel/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.HeaderPanelLayoutProps = exports.HeaderPanelLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _utils = require("../../utils");
var _group_panel = require("../group_panel/group_panel");
var _layout = require("./date_header/layout");
var _excluded = ["className", "dateCellTemplate", "dateHeaderData", "dateHeaderTemplate", "elementRef", "groupByDate", "groupOrientation", "groupPanelData", "groups", "height", "isRenderDateHeader", "resourceCellTemplate", "timeCellTemplate"];

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
    var isHorizontalGrouping = _ref.isHorizontalGrouping,
        _ref$props = _ref.props,
        dateCellTemplate = _ref$props.dateCellTemplate,
        dateHeaderData = _ref$props.dateHeaderData,
        DateHeader = _ref$props.dateHeaderTemplate,
        groupByDate = _ref$props.groupByDate,
        groupOrientation = _ref$props.groupOrientation,
        groupPanelData = _ref$props.groupPanelData,
        groups = _ref$props.groups,
        isRenderDateHeader = _ref$props.isRenderDateHeader,
        resourceCellTemplate = _ref$props.resourceCellTemplate,
        timeCellTemplate = _ref$props.timeCellTemplate;
    return (0, _inferno.createVNode)(1, "thead", null, [isHorizontalGrouping && !groupByDate && (0, _inferno.createComponentVNode)(2, _group_panel.GroupPanel, {
        groupPanelData: groupPanelData,
        groups: groups,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        resourceCellTemplate: resourceCellTemplate
    }), isRenderDateHeader && DateHeader({
        groupByDate: groupByDate,
        dateHeaderData: dateHeaderData,
        groupOrientation: groupOrientation,
        groups: groups,
        dateCellTemplate: dateCellTemplate,
        timeCellTemplate: timeCellTemplate
    }), groupByDate && (0, _inferno.createComponentVNode)(2, _group_panel.GroupPanel, {
        groupPanelData: groupPanelData,
        groups: groups,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        resourceCellTemplate: resourceCellTemplate
    })], 0)
};
exports.viewFunction = viewFunction;
var HeaderPanelLayoutProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_group_panel.GroupPanelProps), Object.getOwnPropertyDescriptors({
    isRenderDateHeader: true,
    dateHeaderTemplate: _layout.DateHeaderLayout
})));
exports.HeaderPanelLayoutProps = HeaderPanelLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var HeaderPanelLayout = function(_InfernoWrapperCompon) {
    _inheritsLoose(HeaderPanelLayout, _InfernoWrapperCompon);

    function HeaderPanelLayout(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = HeaderPanelLayout.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                dateHeaderTemplate: getTemplate(props.dateHeaderTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            isHorizontalGrouping: this.isHorizontalGrouping,
            restAttributes: this.restAttributes
        })
    };
    _createClass(HeaderPanelLayout, [{
        key: "isHorizontalGrouping",
        get: function() {
            var _this$props = this.props,
                groupOrientation = _this$props.groupOrientation,
                groups = _this$props.groups;
            return (0, _utils.isHorizontalGroupingApplied)(groups, groupOrientation)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.className, _this$props2.dateCellTemplate, _this$props2.dateHeaderData, _this$props2.dateHeaderTemplate, _this$props2.elementRef, _this$props2.groupByDate, _this$props2.groupOrientation, _this$props2.groupPanelData, _this$props2.groups, _this$props2.height, _this$props2.isRenderDateHeader, _this$props2.resourceCellTemplate, _this$props2.timeCellTemplate, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return HeaderPanelLayout
}(_inferno2.InfernoWrapperComponent);
exports.HeaderPanelLayout = HeaderPanelLayout;
HeaderPanelLayout.defaultProps = HeaderPanelLayoutProps;
