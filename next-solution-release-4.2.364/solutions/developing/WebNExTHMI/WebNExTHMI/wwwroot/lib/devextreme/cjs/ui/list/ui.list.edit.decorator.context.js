/**
 * DevExtreme (cjs/ui/list/ui.list.edit.decorator.context.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _uiListEdit = _interopRequireDefault(require("./ui.list.edit.decorator_menu_helper"));
var _message = _interopRequireDefault(require("../../localization/message"));
var _uiListEdit2 = require("./ui.list.edit.decorator_registry");
var _uiListEdit3 = _interopRequireDefault(require("./ui.list.edit.decorator"));
var _ui = _interopRequireDefault(require("../overlay/ui.overlay"));
var _uiList = require("./ui.list.base");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var CONTEXTMENU_CLASS = "dx-list-context-menu";
var CONTEXTMENU_MENUCONTENT_CLASS = "dx-list-context-menucontent";
(0, _uiListEdit2.register)("menu", "context", _uiListEdit3.default.inherit({
    _init: function() {
        var $menu = (0, _renderer.default)("<div>").addClass(CONTEXTMENU_CLASS);
        this._list.$element().append($menu);
        this._menu = this._renderOverlay($menu)
    },
    _renderOverlay: function($element) {
        return this._list._createComponent($element, _ui.default, {
            shading: false,
            deferRendering: true,
            hideOnParentScroll: true,
            closeOnOutsideClick: function(e) {
                return !(0, _renderer.default)(e.target).closest("." + CONTEXTMENU_CLASS).length
            },
            animation: {
                show: {
                    type: "slide",
                    duration: 300,
                    from: {
                        height: 0,
                        opacity: 1
                    },
                    to: {
                        height: function() {
                            return (0, _size.getOuterHeight)(this._$menuList)
                        }.bind(this),
                        opacity: 1
                    }
                },
                hide: {
                    type: "slide",
                    duration: 0,
                    from: {
                        opacity: 1
                    },
                    to: {
                        opacity: 0
                    }
                }
            },
            _ignoreFunctionValueDeprecation: true,
            height: function() {
                return this._$menuList ? (0, _size.getOuterHeight)(this._$menuList) : 0
            }.bind(this),
            width: function() {
                return (0, _size.getOuterWidth)(this._list.$element())
            }.bind(this),
            onContentReady: this._renderMenuContent.bind(this)
        })
    },
    _renderMenuContent: function(e) {
        var $overlayContent = e.component.$content();
        var items = this._menuItems().slice();
        if (this._deleteEnabled()) {
            items.push({
                text: _message.default.format("dxListEditDecorator-delete"),
                action: this._deleteItem.bind(this)
            })
        }
        this._$menuList = (0, _renderer.default)("<div>");
        this._list._createComponent(this._$menuList, _uiList.ListBase, {
            items: items,
            onItemClick: this._menuItemClickHandler.bind(this),
            height: "auto",
            integrationOptions: {}
        });
        $overlayContent.addClass(CONTEXTMENU_MENUCONTENT_CLASS);
        $overlayContent.append(this._$menuList)
    },
    _menuItemClickHandler: function(args) {
        this._menu.hide();
        this._fireMenuAction(this._$itemWithMenu, args.itemData.action)
    },
    _deleteItem: function() {
        this._list.deleteItem(this._$itemWithMenu)
    },
    handleContextMenu: function($itemElement) {
        this._$itemWithMenu = $itemElement;
        this._menu.option({
            position: {
                my: "top",
                at: "bottom",
                of: $itemElement,
                collision: "flip"
            }
        });
        this._menu.show();
        return true
    },
    dispose: function() {
        if (this._menu) {
            this._menu.$element().remove()
        }
        this.callBase.apply(this, arguments)
    }
}).include(_uiListEdit.default));
