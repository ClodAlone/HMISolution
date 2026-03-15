/**
 * DevExtreme (esm/ui/html_editor/modules/widget_collector.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    each
} from "../../../core/utils/iterator";
export default class WidgetCollector {
    constructor() {
        this._collection = []
    }
    clear() {
        this._collection = []
    }
    add(name, instance) {
        this._collection.push({
            name: name,
            instance: instance
        })
    }
    getByName(widgetName) {
        var widget = null;
        each(this._collection, (index, _ref) => {
            var {
                name: name,
                instance: instance
            } = _ref;
            if (name === widgetName) {
                widget = instance;
                return false
            }
        });
        return widget
    }
    each(handler) {
        this._collection.forEach(_ref2 => {
            var {
                name: name,
                instance: instance
            } = _ref2;
            return instance && handler(name, instance)
        })
    }
}
