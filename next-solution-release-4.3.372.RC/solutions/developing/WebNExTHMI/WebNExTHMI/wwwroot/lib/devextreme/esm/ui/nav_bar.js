/**
 * DevExtreme (esm/ui/nav_bar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../core/component_registrator";
import {
    extend
} from "../core/utils/extend";
import NavBarItem from "./nav_bar/item";
import Tabs from "./tabs";
var NAVBAR_CLASS = "dx-navbar";
var ITEM_CLASS = "dx-item-content";
var NAVBAR_ITEM_CLASS = "dx-nav-item";
var NAVBAR_ITEM_CONTENT_CLASS = "dx-nav-item-content";
var NavBar = Tabs.inherit({
    ctor: function(element, options) {
        this.callBase(element, options);
        this._logDeprecatedComponentWarning("20.1", "dxTabs")
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            scrollingEnabled: false
        })
    },
    _render: function() {
        this.callBase();
        this.$element().addClass(NAVBAR_CLASS)
    },
    _postprocessRenderItem: function(args) {
        this.callBase(args);
        var $itemElement = args.itemElement;
        var itemData = args.itemData;
        $itemElement.addClass(NAVBAR_ITEM_CLASS);
        $itemElement.find("." + ITEM_CLASS).addClass(NAVBAR_ITEM_CONTENT_CLASS);
        if (!itemData.icon) {
            $itemElement.addClass("dx-navbar-text-item")
        }
    }
});
NavBar.ItemClass = NavBarItem;
registerComponent("dxNavBar", NavBar);
export default NavBar;
