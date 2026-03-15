/**
 * DevExtreme (cjs/ui/toolbar/ui.toolbar.strategy.drop_down_menu.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _extend = require("../../core/utils/extend");
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _uiToolbar = _interopRequireDefault(require("./ui.toolbar.strategy"));
var _uiToolbar2 = _interopRequireDefault(require("./ui.toolbar.menu"));
var _drop_down_menu = _interopRequireDefault(require("../drop_down_menu"));
var _devices = _interopRequireDefault(require("../../core/devices"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var MENU_INVISIBLE_CLASS = "dx-state-invisible";
var POPOVER_BOUNDARY_OFFSET = 10;
var DropDownMenuStrategy = _uiToolbar.default.inherit({
    NAME: "dropDownMenu",
    render: function() {
        if (!this._hasVisibleMenuItems()) {
            return
        }
        this._renderMenuButtonContainer();
        this._renderWidget()
    },
    renderMenuItems: function() {
        if (!this._menu) {
            this.render()
        }
        this.callBase();
        if (this._menu && !this._menu.option("items").length) {
            this._menu.close()
        }
    },
    _menuWidget: function() {
        return _drop_down_menu.default
    },
    _widgetOptions: function() {
        var _this = this;
        var topAndBottomOffset = 2 * POPOVER_BOUNDARY_OFFSET;
        return (0, _extend.extend)(this.callBase(), {
            deferRendering: true,
            container: this._toolbar.option("menuContainer"),
            popupMaxHeight: "android" === _devices.default.current().platform ? _dom_adapter.default.getDocumentElement().clientHeight - topAndBottomOffset : void 0,
            menuWidget: _uiToolbar2.default,
            onOptionChanged: function(_ref) {
                var name = _ref.name,
                    value = _ref.value;
                if ("opened" === name) {
                    _this._toolbar.option("overflowMenuVisible", value)
                }
                if ("items" === name) {
                    _this._updateMenuVisibility(value)
                }
            },
            popupPosition: {
                at: "bottom right",
                my: "top right"
            }
        })
    },
    _updateMenuVisibility: function(menuItems) {
        var items = menuItems || this._getMenuItems();
        var isMenuVisible = items.length && this._hasVisibleMenuItems(items);
        this._toggleMenuVisibility(isMenuVisible)
    },
    _toggleMenuVisibility: function(value) {
        if (!this._menuContainer()) {
            return
        }
        this._menuContainer().toggleClass(MENU_INVISIBLE_CLASS, !value)
    },
    _menuContainer: function() {
        return this._$menuButtonContainer
    }
});
var _default = DropDownMenuStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
