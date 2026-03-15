/**
 * DevExtreme (esm/viz/vector_map/data_exchanger.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Callbacks from "../../core/utils/callbacks";
export function DataExchanger() {
    this._store = {}
}
DataExchanger.prototype = {
    constructor: DataExchanger,
    dispose: function() {
        this._store = null;
        return this
    },
    _get: function(category, name) {
        var store = this._store[category] || (this._store[category] = {});
        return store[name] || (store[name] = {
            callbacks: Callbacks()
        })
    },
    set: function(category, name, data) {
        var item = this._get(category, name);
        item.data = data;
        item.callbacks.fire(data);
        return this
    },
    bind: function(category, name, callback) {
        var item = this._get(category, name);
        item.callbacks.add(callback);
        item.data && callback(item.data);
        return this
    },
    unbind: function(category, name, callback) {
        var item = this._get(category, name);
        item.callbacks.remove(callback);
        return this
    }
};
