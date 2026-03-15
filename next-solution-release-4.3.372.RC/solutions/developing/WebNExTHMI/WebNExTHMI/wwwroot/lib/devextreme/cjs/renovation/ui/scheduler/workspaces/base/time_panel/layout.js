/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/base/time_panel/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.TimePanelTableLayout = exports.TimePanelLayoutProps = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _row = require("../row");
var _cell = require("./cell");
var _cell2 = require("../cell");
var _utils = require("../../utils");
var _table = require("../table");
var _title = require("../date_table/all_day_panel/title");
var _excluded = ["groupOrientation", "tableRef", "timeCellTemplate", "timePanelData"];

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
        _ref$props = _ref.props,
        groupOrientation = _ref$props.groupOrientation,
        tableRef = _ref$props.tableRef,
        timeCellTemplate = _ref$props.timeCellTemplate,
        timePanelData = _ref$props.timePanelData,
        restAttributes = _ref.restAttributes,
        topVirtualRowHeight = _ref.topVirtualRowHeight;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _table.Table, _extends({}, restAttributes, {
        topVirtualRowHeight: topVirtualRowHeight,
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        virtualCellsCount: 1,
        className: "dx-scheduler-time-panel",
        tableRef: tableRef,
        children: timePanelData.groupedData.map((function(_ref2, index) {
            var dateTable = _ref2.dateTable,
                groupIndex = _ref2.groupIndex;
            return (0, _inferno.createFragment)([(0, _utils.getIsGroupedAllDayPanel)(timePanelData, index) && (0, _inferno.createComponentVNode)(2, _row.Row, {
                children: (0, _inferno.createComponentVNode)(2, _cell2.CellBase, {
                    className: "dx-scheduler-time-panel-title-cell",
                    children: (0, _inferno.createComponentVNode)(2, _title.AllDayPanelTitle)
                })
            }), dateTable.map((function(cell) {
                var groups = cell.groups,
                    cellIndex = cell.index,
                    isFirstGroupCell = cell.isFirstGroupCell,
                    isLastGroupCell = cell.isLastGroupCell,
                    key = cell.key,
                    startDate = cell.startDate,
                    text = cell.text;
                return (0, _inferno.createComponentVNode)(2, _row.Row, {
                    className: "dx-scheduler-time-panel-row",
                    children: (0, _inferno.createComponentVNode)(2, _cell.TimePanelCell, {
                        startDate: startDate,
                        text: text,
                        groups: groups,
                        groupIndex: groupIndex,
                        isFirstGroupCell: isFirstGroupCell,
                        isLastGroupCell: isLastGroupCell,
                        index: cellIndex,
                        timeCellTemplate: timeCellTemplate
                    })
                }, key)
            }))], 0, (0, _utils.getKeyByGroup)(groupIndex, groupOrientation))
        }))
    })))
};
exports.viewFunction = viewFunction;
var TimePanelLayoutProps = Object.defineProperties({}, {
    timePanelData: {
        get: function() {
            return {
                groupedData: [],
                leftVirtualCellCount: 0,
                rightVirtualCellCount: 0,
                topVirtualRowCount: 0,
                bottomVirtualRowCount: 0
            }
        },
        configurable: true,
        enumerable: true
    }
});
exports.TimePanelLayoutProps = TimePanelLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var TimePanelTableLayout = function(_InfernoWrapperCompon) {
    _inheritsLoose(TimePanelTableLayout, _InfernoWrapperCompon);

    function TimePanelTableLayout(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = TimePanelTableLayout.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                timeCellTemplate: getTemplate(props.timeCellTemplate)
            }),
            topVirtualRowHeight: this.topVirtualRowHeight,
            bottomVirtualRowHeight: this.bottomVirtualRowHeight,
            restAttributes: this.restAttributes
        })
    };
    _createClass(TimePanelTableLayout, [{
        key: "topVirtualRowHeight",
        get: function() {
            var _this$props$timePanel;
            return null !== (_this$props$timePanel = this.props.timePanelData.topVirtualRowHeight) && void 0 !== _this$props$timePanel ? _this$props$timePanel : 0
        }
    }, {
        key: "bottomVirtualRowHeight",
        get: function() {
            var _this$props$timePanel2;
            return null !== (_this$props$timePanel2 = this.props.timePanelData.bottomVirtualRowHeight) && void 0 !== _this$props$timePanel2 ? _this$props$timePanel2 : 0
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.groupOrientation, _this$props.tableRef, _this$props.timeCellTemplate, _this$props.timePanelData, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return TimePanelTableLayout
}(_inferno2.InfernoWrapperComponent);
exports.TimePanelTableLayout = TimePanelTableLayout;
TimePanelTableLayout.defaultProps = TimePanelLayoutProps;
