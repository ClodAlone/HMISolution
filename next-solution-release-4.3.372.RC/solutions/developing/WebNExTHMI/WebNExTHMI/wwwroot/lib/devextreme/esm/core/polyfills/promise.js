/**
 * DevExtreme (esm/core/polyfills/promise.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    Deferred,
    when
} from "../../core/utils/deferred";
import {
    hasWindow,
    getWindow
} from "../../core/utils/window";
var promise = hasWindow() ? getWindow().Promise : Promise;
if (!promise) {
    promise = function(resolver) {
        var d = new Deferred;
        resolver(d.resolve.bind(this), d.reject.bind(this));
        return d.promise()
    };
    promise.resolve = function(val) {
        return (new Deferred).resolve(val).promise()
    };
    promise.reject = function(val) {
        return (new Deferred).reject(val).promise()
    };
    promise.all = function(promises) {
        return when.apply(this, promises).then((function() {
            return [].slice.call(arguments)
        }))
    }
}
export default promise;
