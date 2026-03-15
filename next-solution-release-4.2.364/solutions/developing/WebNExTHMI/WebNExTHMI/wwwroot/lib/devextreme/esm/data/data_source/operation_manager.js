/**
 * DevExtreme (esm/data/data_source/operation_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    CANCELED_TOKEN
} from "./utils";
export default class OperationManager {
    constructor() {
        this._counter = -1;
        this._deferreds = {}
    }
    add(deferred) {
        this._counter++;
        this._deferreds[this._counter] = deferred;
        return this._counter
    }
    remove(operationId) {
        return delete this._deferreds[operationId]
    }
    cancel(operationId) {
        if (operationId in this._deferreds) {
            this._deferreds[operationId].reject(CANCELED_TOKEN);
            return true
        }
        return false
    }
    cancelAll() {
        while (this._counter > -1) {
            this.cancel(this._counter);
            this._counter--
        }
    }
}
