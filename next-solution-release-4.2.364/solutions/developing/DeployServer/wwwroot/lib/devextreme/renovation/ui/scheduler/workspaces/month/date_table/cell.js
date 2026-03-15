/**
 * DevExtreme (renovation/ui/scheduler/workspaces/month/date_table/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.MonthDateTableCell = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _combine_classes = require("../../../../../utils/combine_classes");
var _cell = require("../../base/date_table/cell");
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
    var classes = _ref.classes,
        contentTemplateProps = _ref.contentTemplateProps,
        _ref$props = _ref.props,
        dataCellTemplate = _ref$props.dataCellTemplate,
        endDate = _ref$props.endDate,
        groupIndex = _ref$props.groupIndex,
        groups = _ref$props.groups,
        index = _ref$props.index,
        isFirstGroupCell = _ref$props.isFirstGroupCell,
        isFocused = _ref$props.isFocused,
        isLastGroupCell = _ref$props.isLastGroupCell,
        isSelected = _ref$props.isSelected,
        startDate = _ref$props.startDate,
        text = _ref$props.text;
    return (0, _inferno.createComponentVNode)(2, _cell.DateTableCellBase, {
        className: classes,
        dataCellTemplate: dataCellTemplate,
        startDate: startDate,
        endDate: endDate,
        text: text,
        groups: groups,
        groupIndex: groupIndex,
        index: index,
        isFirstGroupCell: isFirstGroupCell,
        isLastGroupCell: isLastGroupCell,
        isSelected: isSelected,
        isFocused: isFocused,
        contentTemplateProps: contentTemplateProps,
        children: (0, _inferno.createVNode)(1, "div", "dx-scheduler-date-table-cell-text", text, 0)
    })
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var MonthDateTableCell = function(_BaseInfernoComponent) {
    _inheritsLoose(MonthDateTableCell, _BaseInfernoComponent);

    function MonthDateTableCell(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        return _this
    }
    var _proto = MonthDateTableCell.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.index !== nextProps.index || this.props.text !== nextProps.text) {
            this.__getterCache.contentTemplateProps = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            classes: this.classes,
            contentTemplateProps: this.contentTemplateProps,
            restAttributes: this.restAttributes
        })
    };
    _createClass(MonthDateTableCell, [{
        key: "classes",
        get: function() {
            var _this$props = this.props,
                className = _this$props.className,
                firstDayOfMonth = _this$props.firstDayOfMonth,
                otherMonth = _this$props.otherMonth,
                today = _this$props.today;
            return (0, _combine_classes.combineClasses)(_defineProperty({
                "dx-scheduler-date-table-other-month": !!otherMonth,
                "dx-scheduler-date-table-current-date": !!today,
                "dx-scheduler-date-table-first-of-month": !!firstDayOfMonth
            }, className, !!className))
        }
    }, {
        key: "contentTemplateProps",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.contentTemplateProps) {
                return this.__getterCache.contentTemplateProps
            }
            return this.__getterCache.contentTemplateProps = (_this2$props = _this2.props, index = _this2$props.index, text = _this2$props.text, {
                data: {
                    text: text
                },
                index: index
            });
            var _this2$props, index, text
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props2 = this.props,
                restProps = (_this$props2.allDay, _this$props2.ariaLabel, _this$props2.children, _this$props2.className, _this$props2.contentTemplate, _this$props2.contentTemplateProps, _this$props2.dataCellTemplate, _this$props2.endDate, _this$props2.firstDayOfMonth, _this$props2.groupIndex, _this$props2.groups, _this$props2.index, _this$props2.isFirstGroupCell, _this$props2.isFocused, _this$props2.isLastGroupCell, _this$props2.isSelected, _this$props2.otherMonth, _this$props2.startDate, _this$props2.text, _this$props2.today, _objectWithoutProperties(_this$props2, _excluded));
            return restProps
        }
    }]);
    return MonthDateTableCell
}(_inferno2.BaseInfernoComponent);
exports.MonthDateTableCell = MonthDateTableCell;
MonthDateTableCell.defaultProps = _cell.DateTableCellBaseProps;
