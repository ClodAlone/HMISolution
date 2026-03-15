/**
 * DevExtreme (esm/ui/toolbar/ui.toolbar.strategy.drop_down_menu.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
import domAdapter from "../../core/dom_adapter";
import ToolbarStrategy from "./ui.toolbar.strategy";
import ToolbarMenu from "./ui.toolbar.menu";
import DropDownMenu from "../drop_down_menu";
import devices from "../../core/devices";
var MENU_INVISIBLE_CLASS = "dx-state-invisible";
var POPOVER_BOUNDARY_OFFSET = 10;
var DropDownMenuStrategy = ToolbarStrategy.inherit({
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
        return DropDownMenu
    },
    _widgetOptions: function() {
        var topAndBottomOffset = 2 * POPOVER_BOUNDARY_OFFSET;
        return extend(this.callBase(), {
            deferRendering: true,
            container: this._toolbar.option("menuContainer"),
            popupMaxHeight: "android" === devices.current().platform ? domAdapter.getDocumentElement().clientHeight - topAndBottomOffset : void 0,
            menuWidget: ToolbarMenu,
            onOptionChanged: _ref => {
                var {
                    name: name,
                    value: value
                } = _ref;
                if ("opened" === name) {
                    this._toolbar.option("overflowMenuVisible", value)
                }
                if ("items" === name) {
                    this._updateMenuVisibility(value)
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
export default DropDownMenuStrategy;
