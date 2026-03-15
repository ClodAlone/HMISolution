/**
 * DevExtreme (esm/core/options/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import devices from "../devices";
import {
    isEmptyObject,
    isFunction
} from "../utils/type";
import {
    findBestMatches
} from "../utils/common";
import {
    extend
} from "../utils/extend";
import {
    compileGetter
} from "../utils/data";
var cachedGetters = {};
export var convertRulesToOptions = rules => {
    var currentDevice = devices.current();
    return rules.reduce((options, _ref) => {
        var {
            device: device,
            options: ruleOptions
        } = _ref;
        var deviceFilter = device || {};
        var match = isFunction(deviceFilter) ? deviceFilter(currentDevice) : deviceMatch(currentDevice, deviceFilter);
        if (match) {
            extend(true, options, ruleOptions)
        }
        return options
    }, {})
};
export var normalizeOptions = (options, value) => "string" !== typeof options ? options : {
    [options]: value
};
export var deviceMatch = (device, filter) => isEmptyObject(filter) || findBestMatches(device, [filter]).length > 0;
export var getFieldName = fullName => fullName.substr(fullName.lastIndexOf(".") + 1);
export var getParentName = fullName => fullName.substr(0, fullName.lastIndexOf("."));
export var getNestedOptionValue = function(optionsObject, name) {
    cachedGetters[name] = cachedGetters[name] || compileGetter(name);
    return cachedGetters[name](optionsObject, {
        functionsAsIs: true
    })
};
export var createDefaultOptionRules = function() {
    var options = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : [];
    return options
};
