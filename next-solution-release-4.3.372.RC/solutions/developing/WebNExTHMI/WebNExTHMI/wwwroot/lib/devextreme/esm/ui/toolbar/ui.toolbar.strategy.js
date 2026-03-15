/**
 * DevExtreme (esm/ui/toolbar/ui.toolbar.strategy.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    noop
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import {
    compileGetter
} from "../../core/utils/data";
import Class from "../../core/class";
var abstract = Class.abstract;
var TOOLBAR_MENU_CONTAINER_CLASS = "dx-toolbar-menu-container";
var ToolbarStrategy = Class.inherit({
    ctor: function(toolbar) {
        this._toolbar = toolbar
    },
    _widgetOptions: function() {
        var itemClickAction = this._toolbar._createActionByOption("onItemClick");
        return {
            itemTemplate: this._getMenuItemTemplate.bind(this),
            onItemClick: function(e) {
                this._toolbar.option("overflowMenuVisible", false);
                itemClickAction(e)
            }.bind(this)
        }
    },
    _getMenuItemTemplate: function() {
        return this._toolbar._getTemplateByOption("menuItemTemplate")
    },
    _renderWidget: function() {
        var $menu = $("<div>").appendTo(this._menuContainer());
        this._menu = this._toolbar._createComponent($menu, this._menuWidget(), this._widgetOptions());
        this.renderMenuItems()
    },
    _menuContainer: abstract,
    _menuWidget: abstract,
    _hasVisibleMenuItems: function(items) {
        var menuItems = items || this._toolbar.option("items");
        var result = false;
        var optionGetter = compileGetter("visible");
        var overflowGetter = compileGetter("locateInMenu");
        each(menuItems, (function(index, item) {
            var itemVisible = optionGetter(item, {
                functionsAsIs: true
            });
            var itemOverflow = overflowGetter(item, {
                functionsAsIs: true
            });
            if (false !== itemVisible && ("auto" === itemOverflow || "always" === itemOverflow) || "menu" === item.location) {
                result = true
            }
        }));
        return result
    },
    _getMenuItems: function() {
        return this._toolbar._getMenuItems()
    },
    _updateMenuVisibility: noop,
    _renderMenuButtonContainer: function() {
        var $afterSection = this._toolbar._$afterSection;
        this._$menuButtonContainer = $("<div>").appendTo($afterSection).addClass(this._toolbar._buttonClass()).addClass(TOOLBAR_MENU_CONTAINER_CLASS)
    },
    renderMenuItems: function() {
        this._menu && this._menu.option("items", this._getMenuItems())
    },
    widgetOption: function(name, value) {
        this._menu && this._menu.option(name, value)
    }
});
export default ToolbarStrategy;
