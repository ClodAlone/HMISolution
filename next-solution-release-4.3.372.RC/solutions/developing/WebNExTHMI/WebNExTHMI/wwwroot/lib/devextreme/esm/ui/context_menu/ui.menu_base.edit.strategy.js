/**
 * DevExtreme (esm/ui/context_menu/ui.menu_base.edit.strategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    map
} from "../../core/utils/iterator";
import PlainEditStrategy from "../collection/ui.collection_widget.edit.strategy.plain";
class MenuBaseEditStrategy extends PlainEditStrategy {
    _getPlainItems() {
        return map(this._collectionWidget.option("items"), (function getMenuItems(item) {
            return item.items ? [item].concat(map(item.items, getMenuItems)) : item
        }))
    }
    _stringifyItem(item) {
        return JSON.stringify(item, (key, value) => {
            if ("template" === key) {
                return this._getTemplateString(value)
            }
            return value
        })
    }
    _getTemplateString(template) {
        var result;
        if ("object" === typeof template) {
            result = $(template).text()
        } else {
            result = template.toString()
        }
        return result
    }
}
export default MenuBaseEditStrategy;
