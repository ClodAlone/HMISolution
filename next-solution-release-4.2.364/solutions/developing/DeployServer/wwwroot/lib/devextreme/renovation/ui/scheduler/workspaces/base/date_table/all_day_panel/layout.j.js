/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/layout.j.js)
 * Version: 21.2.4
 * Build date: Mon Dec 06 2021
 *
 * Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../../../../../core/component_registrator"));
var _date_table = require("../../../../../../component_wrapper/scheduler/date_table");
var _layout = require("./layout");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var AllDayPanelLayout = function(_DateTable) {
    _inheritsLoose(AllDayPanelLayout, _DateTable);

    function AllDayPanelLayout() {
        return _DateTable.apply(this, arguments) || this
    }
    _createClass(AllDayPanelLayout, [{
        key: "_propsInfo",
        get: function() {
            return {
                twoWay: [],
                allowNull: [],
                elements: [],
                templates: ["dataCellTemplate"],
                props: ["className", "viewData", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "topVirtualRowHeight", "bottomVirtualRowHeight", "addDateTableClass", "addVerticalSizesClassToRows", "width", "dataCellTemplate"]
            }
        }
    }, {
        key: "_viewComponent",
        get: function() {
            return _layout.AllDayPanelLayout
        }
    }]);
    return AllDayPanelLayout
}(_date_table.DateTable);
exports.default = AllDayPanelLayout;
(0, _component_registrator.default)("dxAllDayPanelLayout", AllDayPanelLayout);
module.exports = exports.default;
module.exports.default = exports.default;
