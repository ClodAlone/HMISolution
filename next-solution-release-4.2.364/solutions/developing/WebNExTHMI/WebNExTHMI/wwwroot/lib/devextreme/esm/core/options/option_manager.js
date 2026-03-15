/**
 * DevExtreme (esm/core/options/option_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    compileGetter,
    compileSetter,
    getPathParts
} from "../utils/data";
import {
    noop
} from "../utils/common";
import {
    equals
} from "../utils/comparator";
import {
    extend
} from "../utils/extend";
import {
    isDefined,
    isPlainObject
} from "../utils/type";
import {
    normalizeOptions
} from "./utils";
var cachedGetters = {};
var cachedSetters = {};
export class OptionManager {
    constructor(options, optionsByReference) {
        this._options = options;
        this._optionsByReference = optionsByReference;
        this._changingCallback;
        this._changedCallback;
        this._namePreparedCallbacks
    }
    _setByReference(options, rulesOptions) {
        extend(true, options, rulesOptions);
        for (var fieldName in this._optionsByReference) {
            if (Object.prototype.hasOwnProperty.call(rulesOptions, fieldName)) {
                options[fieldName] = rulesOptions[fieldName]
            }
        }
    }
    _setPreparedValue(name, value, merge, silent) {
        var previousValue = this.get(this._options, name, false);
        if (!equals(previousValue, value)) {
            var path = getPathParts(name);
            !silent && this._changingCallback(name, previousValue, value);
            cachedSetters[name] = cachedSetters[name] || compileSetter(name);
            cachedSetters[name](this._options, value, {
                functionsAsIs: true,
                merge: isDefined(merge) ? merge : !this._optionsByReference[name],
                unwrapObservables: path.length > 1 && !!this._optionsByReference[path[0]]
            });
            !silent && this._changedCallback(name, value, previousValue)
        }
    }
    _prepareRelevantNames(options, name, value, silent) {
        if (isPlainObject(value)) {
            for (var valueName in value) {
                this._prepareRelevantNames(options, "".concat(name, ".").concat(valueName), value[valueName])
            }
        }
        this._namePreparedCallbacks(options, name, value, silent)
    }
    get() {
        var options = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this._options;
        var name = arguments.length > 1 ? arguments[1] : void 0;
        var unwrapObservables = arguments.length > 2 ? arguments[2] : void 0;
        cachedGetters[name] = cachedGetters[name] || compileGetter(name);
        return cachedGetters[name](options, {
            functionsAsIs: true,
            unwrapObservables: unwrapObservables
        })
    }
    set(options, value, merge, silent) {
        options = normalizeOptions(options, value);
        for (var name in options) {
            this._prepareRelevantNames(options, name, options[name], silent)
        }
        for (var _name in options) {
            this._setPreparedValue(_name, options[_name], merge, silent)
        }
    }
    onRelevantNamesPrepared(callBack) {
        this._namePreparedCallbacks = callBack
    }
    onChanging(callBack) {
        this._changingCallback = callBack
    }
    onChanged(callBack) {
        this._changedCallback = callBack
    }
    dispose() {
        this._changingCallback = noop;
        this._changedCallback = noop
    }
}
