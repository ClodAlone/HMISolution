/**
 * DevExtreme (renovation/ui/scheduler/semaphore.js)
 * Version: 21.2.5
 * Build date: Mon Jan 17 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.Semaphore = void 0;
var Semaphore = function() {
    function Semaphore() {
        this.counter = 0
    }
    var _proto = Semaphore.prototype;
    _proto.isFree = function() {
        return 0 === this.counter
    };
    _proto.take = function() {
        this.counter += 1
    };
    _proto.release = function() {
        this.counter -= 1;
        if (this.counter < 0) {
            this.counter = 0
        }
    };
    return Semaphore
}();
exports.Semaphore = Semaphore;
