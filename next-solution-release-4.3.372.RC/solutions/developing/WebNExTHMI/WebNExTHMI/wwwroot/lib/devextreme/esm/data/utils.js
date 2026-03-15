/**
 * DevExtreme (esm/data/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isFunction
} from "../core/utils/type";
import domAdapter from "../core/dom_adapter";
import readyCallbacks from "../core/utils/ready_callbacks";
import {
    getWindow
} from "../core/utils/window";
import {
    map
} from "../core/utils/iterator";
import {
    Deferred
} from "../core/utils/deferred";
import {
    equalByValue
} from "../core/utils/common";
var ready = readyCallbacks.add;
export var XHR_ERROR_UNLOAD = "DEVEXTREME_XHR_ERROR_UNLOAD";
export var normalizeBinaryCriterion = function(crit) {
    return [crit[0], crit.length < 3 ? "=" : String(crit[1]).toLowerCase(), crit.length < 2 ? true : crit[crit.length - 1]]
};
export var normalizeSortingInfo = function(info) {
    if (!Array.isArray(info)) {
        info = [info]
    }
    return map(info, (function(i) {
        var result = {
            selector: isFunction(i) || "string" === typeof i ? i : i.getter || i.field || i.selector,
            desc: !!(i.desc || "d" === String(i.dir).charAt(0).toLowerCase())
        };
        if (i.compare) {
            result.compare = i.compare
        }
        return result
    }))
};
export var errorMessageFromXhr = function() {
    var textStatusMessages = {
        timeout: "Network connection timeout",
        error: "Unspecified network error",
        parsererror: "Unexpected server response"
    };
    var unloading;
    ready((function() {
        var window = getWindow();
        domAdapter.listen(window, "beforeunload", (function() {
            unloading = true
        }))
    }));
    return function(xhr, textStatus) {
        if (unloading) {
            return XHR_ERROR_UNLOAD
        }
        if (xhr.status < 400) {
            return function(textStatus) {
                var result = textStatusMessages[textStatus];
                if (!result) {
                    return textStatus
                }
                return result
            }(textStatus)
        }
        return xhr.statusText
    }
}();
export var aggregators = {
    count: {
        seed: 0,
        step: function(count) {
            return 1 + count
        }
    },
    sum: {
        seed: 0,
        step: function(sum, item) {
            return sum + item
        }
    },
    min: {
        step: function(min, item) {
            return item < min ? item : min
        }
    },
    max: {
        step: function(max, item) {
            return item > max ? item : max
        }
    },
    avg: {
        seed: [0, 0],
        step: function(pair, value) {
            return [pair[0] + value, pair[1] + 1]
        },
        finalize: function(pair) {
            return pair[1] ? pair[0] / pair[1] : NaN
        }
    }
};
export var processRequestResultLock = function() {
    var lockCount = 0;
    var lockDeferred;
    return {
        obtain: function() {
            if (0 === lockCount) {
                lockDeferred = new Deferred
            }
            lockCount++
        },
        release: function() {
            lockCount--;
            if (lockCount < 1) {
                lockDeferred.resolve()
            }
        },
        promise: function() {
            var deferred = 0 === lockCount ? (new Deferred).resolve() : lockDeferred;
            return deferred.promise()
        },
        reset: function() {
            lockCount = 0;
            if (lockDeferred) {
                lockDeferred.resolve()
            }
        }
    }
}();
export function isDisjunctiveOperator(condition) {
    return /^(or|\|\||\|)$/i.test(condition)
}
export function isConjunctiveOperator(condition) {
    return /^(and|&&|&)$/i.test(condition)
}
export var keysEqual = function(keyExpr, key1, key2) {
    if (Array.isArray(keyExpr)) {
        var names = map(key1, (function(v, k) {
            return k
        }));
        var name;
        for (var i = 0; i < names.length; i++) {
            name = names[i];
            if (!equalByValue(key1[name], key2[name], 0, false)) {
                return false
            }
        }
        return true
    }
    return equalByValue(key1, key2, 0, false)
};
var BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
export var base64_encode = function(input) {
    if (!Array.isArray(input)) {
        input = stringToByteArray(String(input))
    }
    var result = "";

    function getBase64Char(index) {
        return BASE64_CHARS.charAt(index)
    }
    for (var i = 0; i < input.length; i += 3) {
        var octet1 = input[i];
        var octet2 = input[i + 1];
        var octet3 = input[i + 2];
        result += map([octet1 >> 2, (3 & octet1) << 4 | octet2 >> 4, isNaN(octet2) ? 64 : (15 & octet2) << 2 | octet3 >> 6, isNaN(octet3) ? 64 : 63 & octet3], getBase64Char).join("")
    }
    return result
};

function stringToByteArray(str) {
    var bytes = [];
    var code;
    var i;
    for (i = 0; i < str.length; i++) {
        code = str.charCodeAt(i);
        if (code < 128) {
            bytes.push(code)
        } else if (code < 2048) {
            bytes.push(192 + (code >> 6), 128 + (63 & code))
        } else if (code < 65536) {
            bytes.push(224 + (code >> 12), 128 + (code >> 6 & 63), 128 + (63 & code))
        } else if (code < 2097152) {
            bytes.push(240 + (code >> 18), 128 + (code >> 12 & 63), 128 + (code >> 6 & 63), 128 + (63 & code))
        }
    }
    return bytes
}
export var isUnaryOperation = function(crit) {
    return "!" === crit[0] && Array.isArray(crit[1])
};
var isGroupOperator = function(value) {
    return "and" === value || "or" === value
};
export var isGroupCriterion = function(crit) {
    var first = crit[0];
    var second = crit[1];
    if (Array.isArray(first)) {
        return true
    }
    if (isFunction(first)) {
        if (Array.isArray(second) || isFunction(second) || isGroupOperator(second)) {
            return true
        }
    }
    return false
};
export var trivialPromise = function() {
    var d = new Deferred;
    return d.resolve.apply(d, arguments).promise()
};
export var rejectedPromise = function() {
    var d = new Deferred;
    return d.reject.apply(d, arguments).promise()
};

function throttle(func, timeout) {
    var timeoutId;
    var lastArgs;
    return function() {
        lastArgs = arguments;
        if (!timeoutId) {
            timeoutId = setTimeout(() => {
                timeoutId = void 0;
                if (lastArgs) {
                    func.call(this, lastArgs)
                }
            }, isFunction(timeout) ? timeout() : timeout)
        }
        return timeoutId
    }
}
export function throttleChanges(func, timeout) {
    var cache = [];
    var throttled = throttle((function() {
        func.call(this, cache);
        cache = []
    }), timeout);
    return function(changes) {
        if (Array.isArray(changes)) {
            cache.push(...changes)
        }
        return throttled.call(this, cache)
    }
}
