/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/main_layout_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.MainLayoutProps = void 0;
var _layout_props = require("./layout_props");

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
var MainLayoutProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_layout_props.LayoutProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    intervalCount: 1,
    className: "",
    isRenderDateHeader: true,
    groupByDate: false,
    groupPanelClassName: "dx-scheduler-work-space-vertical-group-table",
    isAllDayPanelCollapsed: true,
    isAllDayPanelVisible: false,
    isRenderHeaderEmptyCell: true,
    isRenderGroupPanel: false,
    isStandaloneAllDayPanel: false
}, {
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
    },
    groupPanelData: {
        get: function() {
            return {
                groupPanelItems: [],
                baseColSpan: 1
            }
        },
        configurable: true,
        enumerable: true
    },
    groups: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.MainLayoutProps = MainLayoutProps;
