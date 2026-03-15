/**
 * DevExtreme (esm/renovation/ui/form/_toDelete/ui.form.item_option_action.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Class from "../../core/class";
export default class ItemOptionAction {
    constructor(options) {
        this._options = options;
        this._itemsRunTimeInfo = this._options.itemsRunTimeInfo
    }
    findInstance() {
        return this._itemsRunTimeInfo.findWidgetInstanceByItem(this._options.item)
    }
    findItemContainer() {
        return this._itemsRunTimeInfo.findItemContainerByItem(this._options.item)
    }
    tryExecute() {
        Class.abstract()
    }
}
