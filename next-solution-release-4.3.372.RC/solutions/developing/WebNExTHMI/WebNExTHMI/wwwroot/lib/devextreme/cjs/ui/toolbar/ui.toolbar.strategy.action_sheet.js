/**
 * DevExtreme (cjs/ui/toolbar/ui.toolbar.strategy.action_sheet.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _uiToolbar = _interopRequireDefault(require("./ui.toolbar.strategy"));
var _extend = require("../../core/utils/extend");
var _action_sheet = _interopRequireDefault(require("../action_sheet"));
var _button = _interopRequireDefault(require("../button"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var TOOLBAR_MENU_BUTTON_CLASS = "dx-toolbar-menu-button";
var ActionSheetStrategy = _uiToolbar.default.inherit({
    NAME: "actionSheet",
    _getMenuItemTemplate: function() {
        return this._toolbar._getTemplate("actionSheetItem")
    },
    render: function() {
        if (!this._hasVisibleMenuItems()) {
            return
        }
        this._renderMenuButton();
        this._renderWidget()
    },
    _renderMenuButton: function() {
        var _this = this;
        this._renderMenuButtonContainer();
        this._$button = (0, _renderer.default)("<div>").appendTo(this._$menuButtonContainer).addClass(TOOLBAR_MENU_BUTTON_CLASS);
        this._toolbar._createComponent(this._$button, _button.default, {
            icon: "overflow",
            onClick: function() {
                _this._toolbar.option("overflowMenuVisible", !_this._toolbar.option("overflowMenuVisible"))
            }
        })
    },
    _menuWidget: function() {
        return _action_sheet.default
    },
    _menuContainer: function() {
        return this._toolbar.$element()
    },
    _widgetOptions: function() {
        var _this2 = this;
        return (0, _extend.extend)(this.callBase(), {
            target: this._$button,
            showTitle: false,
            onOptionChanged: function(_ref) {
                var name = _ref.name,
                    value = _ref.value;
                if ("visible" === name) {
                    _this2._toolbar.option("overflowMenuVisible", value)
                }
            }
        })
    }
});
var _default = ActionSheetStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
