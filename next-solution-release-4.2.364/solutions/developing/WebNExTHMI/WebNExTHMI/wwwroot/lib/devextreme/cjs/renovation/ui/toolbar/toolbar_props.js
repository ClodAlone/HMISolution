/**
 * DevExtreme (cjs/renovation/ui/toolbar/toolbar_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ToolbarTextBoxProps = exports.ToolbarProps = exports.ToolbarItemType = exports.ToolbarItem = exports.ToolbarDropDownButtonProps = exports.ToolbarDropDownButtonItemPropsType = exports.ToolbarDropDownButtonItemProps = exports.ToolbarCheckBoxProps = exports.ToolbarButtonProps = exports.ToolbarButtonGroupProps = exports.CollectionWidgetItem = exports.BaseToolbarItemProps = void 0;
var _base_props = require("../common/base_props");

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
var CollectionWidgetItem = {};
exports.CollectionWidgetItem = CollectionWidgetItem;
var BaseToolbarItemProps = {};
exports.BaseToolbarItemProps = BaseToolbarItemProps;
var ToolbarTextBoxProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseToolbarItemProps), Object.getOwnPropertyDescriptors({
    value: ""
})));
exports.ToolbarTextBoxProps = ToolbarTextBoxProps;
var ToolbarCheckBoxProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseToolbarItemProps), Object.getOwnPropertyDescriptors({
    value: false
})));
exports.ToolbarCheckBoxProps = ToolbarCheckBoxProps;
var ToolbarButtonGroupProps = BaseToolbarItemProps;
exports.ToolbarButtonGroupProps = ToolbarButtonGroupProps;
var ToolbarButtonProps = BaseToolbarItemProps;
exports.ToolbarButtonProps = ToolbarButtonProps;
var ToolbarDropDownButtonItemProps = CollectionWidgetItem;
exports.ToolbarDropDownButtonItemProps = ToolbarDropDownButtonItemProps;
var ToolbarDropDownButtonItemPropsType = {};
exports.ToolbarDropDownButtonItemPropsType = ToolbarDropDownButtonItemPropsType;
var ToolbarDropDownButtonProps = BaseToolbarItemProps;
exports.ToolbarDropDownButtonProps = ToolbarDropDownButtonProps;
var ToolbarItem = CollectionWidgetItem;
exports.ToolbarItem = ToolbarItem;
var ToolbarItemType = {};
exports.ToolbarItemType = ToolbarItemType;
var ToolbarProps = _base_props.BaseWidgetProps;
exports.ToolbarProps = ToolbarProps;
