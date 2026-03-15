/**
 * DevExtreme (esm/data/odata/request_dispatcher.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    sendRequest
} from "./utils";
import "./query_adapter";
var DEFAULT_PROTOCOL_VERSION = 2;
export default class RequestDispatcher {
    constructor(options) {
        options = options || {};
        this._url = String(options.url).replace(/\/+$/, "");
        this._beforeSend = options.beforeSend;
        this._jsonp = options.jsonp;
        this._version = options.version || DEFAULT_PROTOCOL_VERSION;
        this._withCredentials = options.withCredentials;
        this._deserializeDates = options.deserializeDates;
        this._filterToLower = options.filterToLower
    }
    sendRequest(url, method, params, payload) {
        return sendRequest(this.version, {
            url: url,
            method: method,
            params: params || {},
            payload: payload
        }, {
            beforeSend: this._beforeSend,
            jsonp: this._jsonp,
            withCredentials: this._withCredentials,
            deserializeDates: this._deserializeDates
        })
    }
    get version() {
        return this._version
    }
    get beforeSend() {
        return this._beforeSend
    }
    get url() {
        return this._url
    }
    get jsonp() {
        return this._jsonp
    }
    get filterToLower() {
        return this._filterToLower
    }
}
