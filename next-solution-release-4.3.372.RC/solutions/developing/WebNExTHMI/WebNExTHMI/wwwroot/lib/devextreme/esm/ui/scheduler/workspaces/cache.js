/**
 * DevExtreme (esm/ui/scheduler/workspaces/cache.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../../core/utils/type";
export class Cache {
    constructor() {
        this._cache = new Map
    }
    get size() {
        return this._cache.size
    }
    clear() {
        this._cache.clear()
    }
    get(name, callback) {
        if (!this._cache.has(name) && callback) {
            this.set(name, callback())
        }
        return this._cache.get(name)
    }
    set(name, value) {
        isDefined(value) && this._cache.set(name, value)
    }
}
