/**
 * DevExtreme (cjs/renovation/ui/scheduler/workspaces/timeline/header_panel/layout.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../../../../core/component_registrator"));
var _header_panel = require("../../../../../component_wrapper/scheduler/header_panel");
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
var TimelineHeaderPanelLayout = function(_HeaderPanel) {
    _inheritsLoose(TimelineHeaderPanelLayout, _HeaderPanel);

    function TimelineHeaderPanelLayout() {
        return _HeaderPanel.apply(this, arguments) || this
    }
    _createClass(TimelineHeaderPanelLayout, [{
        key: "_propsInfo",
        get: function() {
            return {
                twoWay: [],
                allowNull: [],
                elements: [],
                templates: ["dateCellTemplate", "timeCellTemplate", "dateHeaderTemplate", "resourceCellTemplate"],
                props: ["dateHeaderData", "isRenderDateHeader", "dateCellTemplate", "timeCellTemplate", "dateHeaderTemplate", "groups", "groupOrientation", "groupPanelData", "groupByDate", "height", "className", "resourceCellTemplate"]
            }
        }
    }, {
        key: "_viewComponent",
        get: function() {
            return _layout.TimelineHeaderPanelLayout
        }
    }]);
    return TimelineHeaderPanelLayout
}(_header_panel.HeaderPanel);
exports.default = TimelineHeaderPanelLayout;
(0, _component_registrator.default)("dxTimelineHeaderPanelLayout", TimelineHeaderPanelLayout);
module.exports = exports.default;
module.exports.default = exports.default;
