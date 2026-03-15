/**
 * DevExtreme (esm/core/options/index.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isFunction,
    isObject,
    type
} from "../utils/type";
import {
    equalByValue,
    noop
} from "../utils/common";
import {
    OptionManager
} from "./option_manager";
import {
    getPathParts
} from "../utils/data";
import {
    getFieldName,
    getParentName,
    convertRulesToOptions,
    getNestedOptionValue
} from "./utils";
import {
    extend
} from "../utils/extend";
export class Options {
    constructor(options, defaultOptions, optionsByReference, deprecatedOptions) {
        this._deprecatedCallback;
        this._startChangeCallback;
        this._endChangeCallback;
        this._default = defaultOptions;
        this._deprecated = deprecatedOptions;
        this._deprecatedNames = [];
        this._initDeprecatedNames();
        this._optionManager = new OptionManager(options, optionsByReference);
        this._optionManager.onRelevantNamesPrepared((options, name, value, silent) => this._setRelevantNames(options, name, value, silent));
        this._cachedOptions = {};
        this._rules = []
    }
    set _initial(value) {
        this._initialOptions = value
    }
    get _initial() {
        if (!this._initialOptions) {
            var rulesOptions = this._getByRules(this.silent("defaultOptionsRules"));
            this._initialOptions = this._default;
            this._optionManager._setByReference(this._initialOptions, rulesOptions)
        }
        return this._initialOptions
    }
    _initDeprecatedNames() {
        for (var optionName in this._deprecated) {
            this._deprecatedNames.push(optionName)
        }
    }
    _getByRules(rules) {
        rules = Array.isArray(rules) ? this._rules.concat(rules) : this._rules;
        return convertRulesToOptions(rules)
    }
    _notifyDeprecated(option) {
        var info = this._deprecated[option];
        if (info) {
            this._deprecatedCallback(option, info)
        }
    }
    _setRelevantNames(options, name, value, silent) {
        if (name) {
            var normalizedName = this._normalizeName(name, silent);
            if (normalizedName && normalizedName !== name) {
                this._setField(options, normalizedName, value);
                this._clearField(options, name)
            }
        }
    }
    _setField(options, fullName, value) {
        var fieldName = "";
        var fieldObject = null;
        do {
            fieldName = fieldName ? ".".concat(fieldName) : "";
            fieldName = getFieldName(fullName) + fieldName;
            fullName = getParentName(fullName);
            fieldObject = fullName ? this._optionManager.get(options, fullName, false) : options
        } while (!fieldObject);
        fieldObject[fieldName] = value
    }
    _clearField(options, name) {
        delete options[name];
        var previousFieldName = getParentName(name);
        var fieldObject = previousFieldName ? this._optionManager.get(options, previousFieldName, false) : options;
        if (fieldObject) {
            delete fieldObject[getFieldName(name)]
        }
    }
    _normalizeName(name, silent) {
        if (this._deprecatedNames.length && name) {
            for (var i = 0; i < this._deprecatedNames.length; i++) {
                if (this._deprecatedNames[i] === name) {
                    var deprecate = this._deprecated[name];
                    if (deprecate) {
                        !silent && this._notifyDeprecated(name);
                        return deprecate.alias || name
                    }
                }
            }
        }
        return name
    }
    addRules(rules) {
        this._rules = rules.concat(this._rules)
    }
    applyRules(rules) {
        var options = this._getByRules(rules);
        this.silent(options)
    }
    dispose() {
        this._deprecatedCallback = noop;
        this._startChangeCallback = noop;
        this._endChangeCallback = noop;
        this._optionManager.dispose()
    }
    onChanging(callBack) {
        this._optionManager.onChanging(callBack)
    }
    onChanged(callBack) {
        this._optionManager.onChanged(callBack)
    }
    onDeprecated(callBack) {
        this._deprecatedCallback = callBack
    }
    onStartChange(callBack) {
        this._startChangeCallback = callBack
    }
    onEndChange(callBack) {
        this._endChangeCallback = callBack
    }
    isInitial(name) {
        var value = this.silent(name);
        var initialValue = this.initial(name);
        var areFunctions = isFunction(value) && isFunction(initialValue);
        return areFunctions ? value.toString() === initialValue.toString() : equalByValue(value, initialValue)
    }
    initial(name) {
        return getNestedOptionValue(this._initial, name)
    }
    option(options, value) {
        var isGetter = arguments.length < 2 && "object" !== type(options);
        if (isGetter) {
            return this._optionManager.get(void 0, this._normalizeName(options))
        } else {
            this._startChangeCallback();
            try {
                this._optionManager.set(options, value)
            } finally {
                this._endChangeCallback()
            }
        }
    }
    silent(options, value) {
        var isGetter = arguments.length < 2 && "object" !== type(options);
        if (isGetter) {
            return this._optionManager.get(void 0, options, void 0, true)
        } else {
            this._optionManager.set(options, value, void 0, true)
        }
    }
    reset(name) {
        if (name) {
            var fullPath = getPathParts(name);
            var value = fullPath.reduce((value, field) => value ? value[field] : this.initial(field), null);
            var defaultValue = isObject(value) ? _extends({}, value) : value;
            this._optionManager.set(name, defaultValue, false)
        }
    }
    getAliasesByName(name) {
        return Object.keys(this._deprecated).filter(aliasName => name === this._deprecated[aliasName].alias)
    }
    isDeprecated(name) {
        return Object.prototype.hasOwnProperty.call(this._deprecated, name)
    }
    cache(name, options) {
        var isGetter = arguments.length < 2;
        if (isGetter) {
            return this._cachedOptions[name]
        } else {
            this._cachedOptions[name] = extend(this._cachedOptions[name], options)
        }
    }
}
