/**
 * DevExtreme (cjs/ui/toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../core/utils/size");
var _renderer = _interopRequireDefault(require("../core/renderer"));
var _component_registrator = _interopRequireDefault(require("../core/component_registrator"));
var _common = require("../core/utils/common");
var _extend = require("../core/utils/extend");
var _array = require("../core/utils/array");
var _iterator = require("../core/utils/iterator");
var _uiToolbar = _interopRequireDefault(require("./toolbar/ui.toolbar.drop_down_menu"));
var _uiToolbar2 = _interopRequireDefault(require("./toolbar/ui.toolbar.base"));
var _child_default_template = require("../core/templates/child_default_template");
var _uiToolbar3 = require("./toolbar/ui.toolbar.utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var TOOLBAR_AUTO_HIDE_ITEM_CLASS = "dx-toolbar-item-auto-hide";
var TOOLBAR_AUTO_HIDE_TEXT_CLASS = "dx-toolbar-text-auto-hide";
var TOOLBAR_HIDDEN_ITEM = "dx-toolbar-item-invisible";
var Toolbar = _uiToolbar2.default.inherit({
    _getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
            menuItemTemplate: "menuItem",
            menuContainer: void 0,
            overflowMenuVisible: false
        })
    },
    updateDimensions: function() {
        this._dimensionChanged()
    },
    _dimensionChanged: function(dimension) {
        if ("height" === dimension) {
            return
        }
        this.callBase();
        this._menu.renderMenuItems()
    },
    _initTemplates: function() {
        this.callBase();
        this._templateManager.addDefaultTemplates({
            actionSheetItem: new _child_default_template.ChildDefaultTemplate("item")
        })
    },
    _initMarkup: function() {
        this.callBase();
        this._updateFocusableItemsTabIndex();
        this._renderMenu()
    },
    _postProcessRenderItems: function() {
        var _this = this;
        this._hideOverflowItems();
        this._menu._updateMenuVisibility();
        this.callBase();
        (0, _common.deferRender)((function() {
            _this._menu.renderMenuItems()
        }))
    },
    _renderItem: function(index, item, itemContainer, $after) {
        var itemElement = this.callBase(index, item, itemContainer, $after);
        if ("auto" === item.locateInMenu) {
            itemElement.addClass(TOOLBAR_AUTO_HIDE_ITEM_CLASS)
        }
        if ("dxButton" === item.widget && "inMenu" === item.showText) {
            itemElement.toggleClass(TOOLBAR_AUTO_HIDE_TEXT_CLASS)
        }
        return itemElement
    },
    _getItemsWidth: function() {
        return this._getSummaryItemsWidth([this._$beforeSection, this._$centerSection, this._$afterSection])
    },
    _hideOverflowItems: function(elementWidth) {
        var overflowItems = this.$element().find("." + TOOLBAR_AUTO_HIDE_ITEM_CLASS);
        if (!overflowItems.length) {
            return
        }
        elementWidth = elementWidth || (0, _size.getWidth)(this.$element());
        (0, _renderer.default)(overflowItems).removeClass(TOOLBAR_HIDDEN_ITEM);
        var itemsWidth = this._getItemsWidth();
        while (overflowItems.length && elementWidth < itemsWidth) {
            var $item = overflowItems.eq(-1);
            itemsWidth -= (0, _size.getOuterWidth)($item);
            $item.addClass(TOOLBAR_HIDDEN_ITEM);
            overflowItems.splice(-1, 1)
        }
    },
    _getMenuItems: function() {
        var that = this;
        var menuItems = (0, _common.grep)(this.option("items") || [], (function(item) {
            return that._isMenuItem(item)
        }));
        var $hiddenItems = this._itemContainer().children("." + TOOLBAR_AUTO_HIDE_ITEM_CLASS + "." + TOOLBAR_HIDDEN_ITEM).not(".dx-state-invisible");
        this._restoreItems = this._restoreItems || [];
        var overflowItems = [].slice.call($hiddenItems).map((function(item) {
            var itemData = that._getItemData(item);
            var $itemContainer = (0, _renderer.default)(item).children();
            var $itemMarkup = $itemContainer.children();
            return (0, _extend.extend)({
                menuItemTemplate: function() {
                    that._restoreItems.push({
                        container: $itemContainer,
                        item: $itemMarkup
                    });
                    var $container = (0, _renderer.default)("<div>").addClass(TOOLBAR_AUTO_HIDE_ITEM_CLASS);
                    return $container.append($itemMarkup)
                }
            }, itemData)
        }));
        return (0, _array.merge)(overflowItems, menuItems)
    },
    _getToolbarItems: function() {
        var that = this;
        return (0, _common.grep)(this.option("items") || [], (function(item) {
            return !that._isMenuItem(item)
        }))
    },
    _renderMenu: function() {
        var _this2 = this;
        this._renderMenuStrategy();
        (0, _common.deferRender)((function() {
            _this2._menu.render()
        }))
    },
    _renderMenuStrategy: function() {
        if (!this._menu) {
            this._menu = new _uiToolbar.default(this)
        }
    },
    _arrangeItems: function() {
        if (this.$element().is(":hidden")) {
            return
        }
        this._$centerSection.css({
            margin: "0 auto",
            float: "none"
        });
        (0, _iterator.each)(this._restoreItems || [], (function(_, obj) {
            (0, _renderer.default)(obj.container).append(obj.item)
        }));
        this._restoreItems = [];
        var elementWidth = (0, _size.getWidth)(this.$element());
        this._hideOverflowItems(elementWidth);
        this.callBase(elementWidth)
    },
    _itemOptionChanged: function(item, property, value) {
        if (this._isMenuItem(item)) {
            this._menu.itemOption(item, property, value)
        } else if (this._isToolbarItem(item)) {
            this.callBase(item, property, value)
        } else {
            this.callBase(item, property, value);
            this._menu.itemOption(item, property, value)
        }
        if ("disabled" === property || "options.disabled" === property) {
            (0, _uiToolbar3.toggleItemFocusableElementTabIndex)(this, item)
        }
        if ("location" === property) {
            this.repaint()
        }
    },
    _updateFocusableItemsTabIndex: function() {
        var _this3 = this;
        this._getToolbarItems().forEach((function(item) {
            return (0, _uiToolbar3.toggleItemFocusableElementTabIndex)(_this3, item)
        }))
    },
    _isMenuItem: function(itemData) {
        return "menu" === itemData.location || "always" === itemData.locateInMenu
    },
    _isToolbarItem: function(itemData) {
        return void 0 === itemData.location || "never" === itemData.locateInMenu
    },
    _optionChanged: function(args) {
        var name = args.name,
            value = args.value;
        switch (name) {
            case "menuItemTemplate":
                this._changeMenuOption("itemTemplate", this._getTemplate(value));
                break;
            case "onItemClick":
                this._changeMenuOption(name, value);
                this.callBase.apply(this, arguments);
                break;
            case "menuContainer":
                this._changeMenuOption("container", value);
                break;
            case "overflowMenuVisible":
                this._changeMenuOption("opened", value);
                break;
            case "disabled":
                this._changeMenuOption("disabled", value);
                this.callBase.apply(this, arguments);
                this._updateFocusableItemsTabIndex();
                break;
            default:
                this.callBase.apply(this, arguments)
        }
    },
    _changeMenuOption: function(name, value) {
        this._menu.widgetOption(name, value)
    }
});
(0, _component_registrator.default)("dxToolbar", Toolbar);
var _default = Toolbar;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
