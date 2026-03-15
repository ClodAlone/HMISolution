/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/group_panel/vertical/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.VerticalGroupPanelLayoutProps = exports.GroupPanelVerticalLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _row = require("./row");
var _utils = require("../../../utils");
var _group_panel_layout_props = require("../group_panel_layout_props");
var _excluded = ["className", "elementRef", "groupByDate", "groupPanelData", "height", "resourceCellTemplate", "styles"];

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
        className = _ref$props.className,
        elementRef = _ref$props.elementRef,
        groupPanelData = _ref$props.groupPanelData,
        resourceCellTemplate = _ref$props.resourceCellTemplate,
        style = _ref.style;
    return (0, _inferno.createVNode)(1, "div", className, (0, _inferno.createVNode)(1, "div", "dx-scheduler-group-flex-container", groupPanelData.groupPanelItems.map((function(group) {
        return (0, _inferno.createComponentVNode)(2, _row.Row, {
            groupItems: group,
            cellTemplate: resourceCellTemplate
        }, group[0].key)
    })), 0), 2, {
        style: (0, _inferno2.normalizeStyles)(style)
    }, null, elementRef)
};
exports.viewFunction = viewFunction;
var VerticalGroupPanelLayoutProps = _group_panel_layout_props.GroupPanelLayoutProps;
exports.VerticalGroupPanelLayoutProps = VerticalGroupPanelLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var GroupPanelVerticalLayout = function(_BaseInfernoComponent) {
    _inheritsLoose(GroupPanelVerticalLayout, _BaseInfernoComponent);

    function GroupPanelVerticalLayout(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = GroupPanelVerticalLayout.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            style: this.style,
            restAttributes: this.restAttributes
        })
    };
    _createClass(GroupPanelVerticalLayout, [{
        key: "style",
        get: function() {
            var _this$props = this.props,
                height = _this$props.height,
                styles = _this$props.styles;
            return (0, _utils.addHeightToStyle)(height, styles)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.className, _this$props2.elementRef, _this$props2.groupByDate, _this$props2.groupPanelData, _this$props2.height, _this$props2.resourceCellTemplate, _this$props2.styles, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return GroupPanelVerticalLayout
}(_inferno2.BaseInfernoComponent);
exports.GroupPanelVerticalLayout = GroupPanelVerticalLayout;
GroupPanelVerticalLayout.defaultProps = VerticalGroupPanelLayoutProps;
