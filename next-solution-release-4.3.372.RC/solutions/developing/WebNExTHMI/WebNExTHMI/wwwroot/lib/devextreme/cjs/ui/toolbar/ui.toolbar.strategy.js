/**
 * DevExtreme (cjs/ui/toolbar/ui.toolbar.strategy.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _common = require("../../core/utils/common");
var _iterator = require("../../core/utils/iterator");
var _data = require("../../core/utils/data");
var _class = _interopRequireDefault(require("../../core/class"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var abstract = _class.default.abstract;
var TOOLBAR_MENU_CONTAINER_CLASS = "dx-toolbar-menu-container";
var ToolbarStrategy = _class.default.inherit({
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
        var $menu = (0, _renderer.default)("<div>").appendTo(this._menuContainer());
        this._menu = this._toolbar._createComponent($menu, this._menuWidget(), this._widgetOptions());
        this.renderMenuItems()
    },
    _menuContainer: abstract,
    _menuWidget: abstract,
    _hasVisibleMenuItems: function(items) {
        var menuItems = items || this._toolbar.option("items");
        var result = false;
        var optionGetter = (0, _data.compileGetter)("visible");
        var overflowGetter = (0, _data.compileGetter)("locateInMenu");
        (0, _iterator.each)(menuItems, (function(index, item) {
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
    _updateMenuVisibility: _common.noop,
    _renderMenuButtonContainer: function() {
        var $afterSection = this._toolbar._$afterSection;
        this._$menuButtonContainer = (0, _renderer.default)("<div>").appendTo($afterSection).addClass(this._toolbar._buttonClass()).addClass(TOOLBAR_MENU_CONTAINER_CLASS)
    },
    renderMenuItems: function() {
        this._menu && this._menu.option("items", this._getMenuItems())
    },
    widgetOption: function(name, value) {
        this._menu && this._menu.option(name, value)
    }
});
var _default = ToolbarStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
