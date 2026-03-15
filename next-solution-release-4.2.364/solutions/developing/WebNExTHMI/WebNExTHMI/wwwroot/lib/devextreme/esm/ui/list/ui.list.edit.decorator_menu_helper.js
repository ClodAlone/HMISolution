/**
 * DevExtreme (esm/ui/list/ui.list.edit.decorator_menu_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var EditDecoratorMenuHelperMixin = {
    _menuEnabled: function() {
        return !!this._menuItems().length
    },
    _menuItems: function() {
        return this._list.option("menuItems")
    },
    _deleteEnabled: function() {
        return this._list.option("allowItemDeleting")
    },
    _fireMenuAction: function($itemElement, action) {
        this._list._itemEventHandlerByHandler($itemElement, action, {}, {
            excludeValidators: ["disabled", "readOnly"]
        })
    }
};
export default EditDecoratorMenuHelperMixin;
