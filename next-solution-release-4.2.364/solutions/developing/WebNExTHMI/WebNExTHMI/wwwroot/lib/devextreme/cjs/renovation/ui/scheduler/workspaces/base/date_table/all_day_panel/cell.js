/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.AllDayPanelCell = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _cell = require("../cell");
var _excluded = ["allDay", "ariaLabel", "children", "className", "contentTemplate", "contentTemplateProps", "dataCellTemplate", "endDate", "firstDayOfMonth", "groupIndex", "groups", "index", "isFirstGroupCell", "isFocused", "isLastGroupCell", "isSelected", "otherMonth", "startDate", "text", "today"];

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
        dataCellTemplate = _ref$props.dataCellTemplate,
        endDate = _ref$props.endDate,
        groupIndex = _ref$props.groupIndex,
        groups = _ref$props.groups,
        index = _ref$props.index,
        isFirstGroupCell = _ref$props.isFirstGroupCell,
        isFocused = _ref$props.isFocused,
        isLastGroupCell = _ref$props.isLastGroupCell,
        isSelected = _ref$props.isSelected,
        startDate = _ref$props.startDate;
    return (0, _inferno.createComponentVNode)(2, _cell.DateTableCellBase, {
        className: "dx-scheduler-all-day-table-cell ".concat(className),
        startDate: startDate,
        endDate: endDate,
        groups: groups,
        groupIndex: groupIndex,
        allDay: true,
        isFirstGroupCell: isFirstGroupCell,
        isLastGroupCell: isLastGroupCell,
        index: index,
        dataCellTemplate: dataCellTemplate,
        isSelected: isSelected,
        isFocused: isFocused
    })
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var AllDayPanelCell = function(_BaseInfernoComponent) {
    _inheritsLoose(AllDayPanelCell, _BaseInfernoComponent);

    function AllDayPanelCell(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = AllDayPanelCell.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            restAttributes: this.restAttributes
        })
    };
    _createClass(AllDayPanelCell, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.allDay, _this$props.ariaLabel, _this$props.children, _this$props.className, _this$props.contentTemplate, _this$props.contentTemplateProps, _this$props.dataCellTemplate, _this$props.endDate, _this$props.firstDayOfMonth, _this$props.groupIndex, _this$props.groups, _this$props.index, _this$props.isFirstGroupCell, _this$props.isFocused, _this$props.isLastGroupCell, _this$props.isSelected, _this$props.otherMonth, _this$props.startDate, _this$props.text, _this$props.today, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return AllDayPanelCell
}(_inferno2.BaseInfernoComponent);
exports.AllDayPanelCell = AllDayPanelCell;
AllDayPanelCell.defaultProps = _cell.DateTableCellBaseProps;
