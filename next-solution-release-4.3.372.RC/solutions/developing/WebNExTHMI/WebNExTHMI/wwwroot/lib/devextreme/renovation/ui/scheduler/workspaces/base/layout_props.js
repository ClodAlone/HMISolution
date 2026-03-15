/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/layout_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.LayoutProps = void 0;
var LayoutProps = Object.defineProperties({
    leftVirtualCellWidth: 0,
    rightVirtualCellWidth: 0,
    topVirtualRowHeight: 0,
    bottomVirtualRowHeight: 0,
    addDateTableClass: true,
    addVerticalSizesClassToRows: true
}, {
    viewData: {
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
exports.LayoutProps = LayoutProps;
