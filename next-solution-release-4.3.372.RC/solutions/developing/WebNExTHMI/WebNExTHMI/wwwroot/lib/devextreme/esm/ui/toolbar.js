/**
 * DevExtreme (esm/ui/toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth,
    getOuterWidth
} from "../core/utils/size";
import $ from "../core/renderer";
import registerComponent from "../core/component_registrator";
import {
    grep,
    deferRender
} from "../core/utils/common";
import {
    extend
} from "../core/utils/extend";
import {
    merge
} from "../core/utils/array";
import {
    each
} from "../core/utils/iterator";
import ToolbarDropDownMenu from "./toolbar/ui.toolbar.drop_down_menu";
import ToolbarBase from "./toolbar/ui.toolbar.base";
import {
    ChildDefaultTemplate
} from "../core/templates/child_default_template";
import {
    toggleItemFocusableElementTabIndex
} from "./toolbar/ui.toolbar.utils";
var TOOLBAR_AUTO_HIDE_ITEM_CLASS = "dx-toolbar-item-auto-hide";
var TOOLBAR_AUTO_HIDE_TEXT_CLASS = "dx-toolbar-text-auto-hide";
var TOOLBAR_HIDDEN_ITEM = "dx-toolbar-item-invisible";
var Toolbar = ToolbarBase.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
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
            actionSheetItem: new ChildDefaultTemplate("item")
        })
    },
    _initMarkup: function() {
        this.callBase();
        this._updateFocusableItemsTabIndex();
        this._renderMenu()
    },
    _postProcessRenderItems: function() {
        this._hideOverflowItems();
        this._menu._updateMenuVisibility();
        this.callBase();
        deferRender(() => {
            this._menu.renderMenuItems()
        })
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
        elementWidth = elementWidth || getWidth(this.$element());
        $(overflowItems).removeClass(TOOLBAR_HIDDEN_ITEM);
        var itemsWidth = this._getItemsWidth();
        while (overflowItems.length && elementWidth < itemsWidth) {
            var $item = overflowItems.eq(-1);
            itemsWidth -= getOuterWidth($item);
            $item.addClass(TOOLBAR_HIDDEN_ITEM);
            overflowItems.splice(-1, 1)
        }
    },
    _getMenuItems: function() {
        var that = this;
        var menuItems = grep(this.option("items") || [], (function(item) {
            return that._isMenuItem(item)
        }));
        var $hiddenItems = this._itemContainer().children("." + TOOLBAR_AUTO_HIDE_ITEM_CLASS + "." + TOOLBAR_HIDDEN_ITEM).not(".dx-state-invisible");
        this._restoreItems = this._restoreItems || [];
        var overflowItems = [].slice.call($hiddenItems).map(item => {
            var itemData = that._getItemData(item);
            var $itemContainer = $(item).children();
            var $itemMarkup = $itemContainer.children();
            return extend({
                menuItemTemplate: function() {
                    that._restoreItems.push({
                        container: $itemContainer,
                        item: $itemMarkup
                    });
                    var $container = $("<div>").addClass(TOOLBAR_AUTO_HIDE_ITEM_CLASS);
                    return $container.append($itemMarkup)
                }
            }, itemData)
        });
        return merge(overflowItems, menuItems)
    },
    _getToolbarItems: function() {
        var that = this;
        return grep(this.option("items") || [], (function(item) {
            return !that._isMenuItem(item)
        }))
    },
    _renderMenu: function() {
        this._renderMenuStrategy();
        deferRender(() => {
            this._menu.render()
        })
    },
    _renderMenuStrategy: function() {
        if (!this._menu) {
            this._menu = new ToolbarDropDownMenu(this)
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
        each(this._restoreItems || [], (function(_, obj) {
            $(obj.container).append(obj.item)
        }));
        this._restoreItems = [];
        var elementWidth = getWidth(this.$element());
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
            toggleItemFocusableElementTabIndex(this, item)
        }
        if ("location" === property) {
            this.repaint()
        }
    },
    _updateFocusableItemsTabIndex() {
        this._getToolbarItems().forEach(item => toggleItemFocusableElementTabIndex(this, item))
    },
    _isMenuItem: function(itemData) {
        return "menu" === itemData.location || "always" === itemData.locateInMenu
    },
    _isToolbarItem: function(itemData) {
        return void 0 === itemData.location || "never" === itemData.locateInMenu
    },
    _optionChanged: function(args) {
        var {
            name: name,
            value: value
        } = args;
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
registerComponent("dxToolbar", Toolbar);
export default Toolbar;
