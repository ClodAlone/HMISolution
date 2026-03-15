/**
 * DevExtreme (esm/core/utils/locker.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../errors";
var Locker = function() {
    var info = {};
    var currentCount = function(lockName) {
        return info[lockName] || 0
    };
    return {
        obtain: function(lockName) {
            info[lockName] = currentCount(lockName) + 1
        },
        release: function(lockName) {
            var count = currentCount(lockName);
            if (count < 1) {
                throw errors.Error("E0014")
            }
            if (1 === count) {
                delete info[lockName]
            } else {
                info[lockName] = count - 1
            }
        },
        locked: function(lockName) {
            return currentCount(lockName) > 0
        }
    }
};
export default Locker;
