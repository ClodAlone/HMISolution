/**
 * DevExtreme (renovation/ui/scheduler/workspaces/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.isVerticalGroupingApplied = exports.isHorizontalGroupingApplied = exports.isGroupingByDate = exports.getKeyByGroup = exports.getKeyByDateAndGroup = exports.getIsGroupedAllDayPanel = exports.getGroupCellClasses = exports.addWidthToStyle = exports.addToStyles = exports.addHeightToStyle = void 0;
var _combine_classes = require("../../../utils/combine_classes");
var _consts = require("../consts");

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
var getKeyByDateAndGroup = function(date, groupIndex) {
    var key = date.getTime();
    if (!groupIndex) {
        return key.toString()
    }
    return (key + groupIndex).toString()
};
exports.getKeyByDateAndGroup = getKeyByDateAndGroup;
var getKeyByGroup = function(groupIndex, groupOrientation) {
    if (groupOrientation === _consts.VERTICAL_GROUP_ORIENTATION && !!groupIndex) {
        return groupIndex.toString()
    }
    return "0"
};
exports.getKeyByGroup = getKeyByGroup;
var addToStyles = function(options, style) {
    var nextStyle = null !== style && void 0 !== style ? style : {};
    var result = _extends({}, nextStyle);
    options.forEach((function(_ref) {
        var attr = _ref.attr,
            value = _ref.value;
        result[attr] = value || nextStyle[attr]
    }));
    return result
};
exports.addToStyles = addToStyles;
var addHeightToStyle = function(value, style) {
    var height = value ? "".concat(value, "px") : "";
    return addToStyles([{
        attr: "height",
        value: height
    }], style)
};
exports.addHeightToStyle = addHeightToStyle;
var addWidthToStyle = function(value, style) {
    var width = value ? "".concat(value, "px") : "";
    return addToStyles([{
        attr: "width",
        value: width
    }], style)
};
exports.addWidthToStyle = addWidthToStyle;
var getGroupCellClasses = function() {
    var isFirstGroupCell = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : false;
    var isLastGroupCell = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
    var className = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : "";
    return (0, _combine_classes.combineClasses)(_defineProperty({
        "dx-scheduler-first-group-cell": isFirstGroupCell,
        "dx-scheduler-last-group-cell": isLastGroupCell
    }, className, true))
};
exports.getGroupCellClasses = getGroupCellClasses;
var getIsGroupedAllDayPanel = function(viewData, index) {
    var groupedData = viewData.groupedData;
    var groupData = groupedData[index];
    var isAllDayPanel = !!(null !== groupData && void 0 !== groupData && groupData.allDayPanel);
    var isGroupedAllDayPanel = !!(null !== groupData && void 0 !== groupData && groupData.isGroupedAllDayPanel);
    return isAllDayPanel && isGroupedAllDayPanel
};
exports.getIsGroupedAllDayPanel = getIsGroupedAllDayPanel;
var isVerticalGroupingApplied = function(groups, groupOrientation) {
    return groupOrientation === _consts.VERTICAL_GROUP_ORIENTATION && !!groups.length
};
exports.isVerticalGroupingApplied = isVerticalGroupingApplied;
var isHorizontalGroupingApplied = function(groups, groupOrientation) {
    return groupOrientation === _consts.HORIZONTAL_GROUP_ORIENTATION && !!groups.length
};
exports.isHorizontalGroupingApplied = isHorizontalGroupingApplied;
var isGroupingByDate = function(groups, groupOrientation, groupByDate) {
    var isHorizontalGrouping = isHorizontalGroupingApplied(groups, groupOrientation);
    return groupByDate && isHorizontalGrouping
};
exports.isGroupingByDate = isGroupingByDate;
