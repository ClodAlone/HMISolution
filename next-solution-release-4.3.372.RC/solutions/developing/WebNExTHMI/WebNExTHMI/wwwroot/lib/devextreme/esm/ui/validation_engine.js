/**
 * DevExtreme (esm/ui/validation_engine.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Class from "../core/class";
import {
    extend
} from "../core/utils/extend";
import {
    inArray
} from "../core/utils/array";
import {
    each
} from "../core/utils/iterator";
import {
    EventsStrategy
} from "../core/events_strategy";
import errors from "../core/errors";
import {
    grep
} from "../core/utils/common";
import {
    isDefined,
    isString,
    isDate,
    isBoolean,
    isObject,
    isFunction,
    isPromise,
    isNumeric
} from "../core/utils/type";
import numberLocalization from "../localization/number";
import messageLocalization from "../localization/message";
import Promise from "../core/polyfills/promise";
import {
    fromPromise,
    Deferred
} from "../core/utils/deferred";
var STATUS = {
    valid: "valid",
    invalid: "invalid",
    pending: "pending"
};
class BaseRuleValidator {
    constructor() {
        this.NAME = "base"
    }
    defaultMessage(value) {
        return messageLocalization.getFormatter("validation-".concat(this.NAME))(value)
    }
    defaultFormattedMessage(value) {
        return messageLocalization.getFormatter("validation-".concat(this.NAME, "-formatted"))(value)
    }
    _isValueEmpty(value) {
        return !rulesValidators.required.validate(value, {})
    }
    validate(value, rule) {
        var valueArray = Array.isArray(value) ? value : [value];
        var result = true;
        if (valueArray.length) {
            valueArray.every(itemValue => {
                result = this._validate(itemValue, rule);
                return result
            })
        } else {
            result = this._validate(null, rule)
        }
        return result
    }
}
class RequiredRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "required"
    }
    _validate(value, rule) {
        if (!isDefined(value)) {
            return false
        }
        if (false === value) {
            return false
        }
        value = String(value);
        if (rule.trim || !isDefined(rule.trim)) {
            value = value.trim()
        }
        return "" !== value
    }
}
class NumericRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "numeric"
    }
    _validate(value, rule) {
        if (false !== rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        if (rule.useCultureSettings && isString(value)) {
            return !isNaN(numberLocalization.parse(value))
        } else {
            return isNumeric(value)
        }
    }
}
class RangeRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "range"
    }
    _validate(value, rule) {
        if (false !== rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        var validNumber = rulesValidators.numeric.validate(value, rule);
        var validValue = isDefined(value) && "" !== value;
        var number = validNumber ? parseFloat(value) : validValue && value.valueOf();
        var min = rule.min;
        var max = rule.max;
        if (!(validNumber || isDate(value)) && !validValue) {
            return false
        }
        if (isDefined(min)) {
            if (isDefined(max)) {
                return number >= min && number <= max
            }
            return number >= min
        } else if (isDefined(max)) {
            return number <= max
        } else {
            throw errors.Error("E0101")
        }
    }
}
class StringLengthRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "stringLength"
    }
    _validate(value, rule) {
        var _value;
        value = String(null !== (_value = value) && void 0 !== _value ? _value : "");
        if (rule.trim || !isDefined(rule.trim)) {
            value = value.trim()
        }
        if (rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        return rulesValidators.range.validate(value.length, extend({}, rule))
    }
}
class CustomRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "custom"
    }
    validate(value, rule) {
        if (rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        var validator = rule.validator;
        var dataGetter = validator && isFunction(validator.option) && validator.option("dataGetter");
        var extraParams = isFunction(dataGetter) && dataGetter();
        var params = {
            value: value,
            validator: validator,
            rule: rule
        };
        if (extraParams) {
            extend(params, extraParams)
        }
        return rule.validationCallback(params)
    }
}
class AsyncRuleValidator extends CustomRuleValidator {
    constructor() {
        super();
        this.NAME = "async"
    }
    validate(value, rule) {
        if (!isDefined(rule.reevaluate)) {
            extend(rule, {
                reevaluate: true
            })
        }
        if (rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        var validator = rule.validator;
        var dataGetter = validator && isFunction(validator.option) && validator.option("dataGetter");
        var extraParams = isFunction(dataGetter) && dataGetter();
        var params = {
            value: value,
            validator: validator,
            rule: rule
        };
        if (extraParams) {
            extend(params, extraParams)
        }
        var callbackResult = rule.validationCallback(params);
        if (!isPromise(callbackResult)) {
            throw errors.Error("E0103")
        }
        return this._getWrappedPromise(fromPromise(callbackResult).promise())
    }
    _getWrappedPromise(promise) {
        var deferred = new Deferred;
        promise.then((function(res) {
            deferred.resolve(res)
        }), (function(err) {
            var res = {
                isValid: false
            };
            if (isDefined(err)) {
                if (isString(err)) {
                    res.message = err
                } else if (isObject(err) && isDefined(err.message) && isString(err.message)) {
                    res.message = err.message
                }
            }
            deferred.resolve(res)
        }));
        return deferred.promise()
    }
}
class CompareRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "compare"
    }
    _validate(value, rule) {
        if (!rule.comparisonTarget) {
            throw errors.Error("E0102")
        }
        if (rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        extend(rule, {
            reevaluate: true
        });
        var otherValue = rule.comparisonTarget();
        var type = rule.comparisonType || "==";
        switch (type) {
            case "==":
                return value == otherValue;
            case "!=":
                return value != otherValue;
            case "===":
                return value === otherValue;
            case "!==":
                return value !== otherValue;
            case ">":
                return value > otherValue;
            case ">=":
                return value >= otherValue;
            case "<":
                return value < otherValue;
            case "<=":
                return value <= otherValue
        }
    }
}
class PatternRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "pattern"
    }
    _validate(value, rule) {
        if (false !== rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        var pattern = rule.pattern;
        if (isString(pattern)) {
            pattern = new RegExp(pattern)
        }
        return pattern.test(value)
    }
}
class EmailRuleValidator extends BaseRuleValidator {
    constructor() {
        super();
        this.NAME = "email"
    }
    _validate(value, rule) {
        if (false !== rule.ignoreEmptyValue && this._isValueEmpty(value)) {
            return true
        }
        return rulesValidators.pattern.validate(value, extend({}, rule, {
            pattern: /^[\d\w._-]+@[\d\w._-]+\.[\w]+$/i
        }))
    }
}
var rulesValidators = {
    required: new RequiredRuleValidator,
    numeric: new NumericRuleValidator,
    range: new RangeRuleValidator,
    stringLength: new StringLengthRuleValidator,
    custom: new CustomRuleValidator,
    async: new AsyncRuleValidator,
    compare: new CompareRuleValidator,
    pattern: new PatternRuleValidator,
    email: new EmailRuleValidator
};
var GroupConfig = Class.inherit({
    ctor(group) {
        this.group = group;
        this.validators = [];
        this._pendingValidators = [];
        this._onValidatorStatusChanged = this._onValidatorStatusChanged.bind(this);
        this._resetValidationInfo();
        this._eventsStrategy = new EventsStrategy(this)
    },
    validate() {
        var result = {
            isValid: true,
            brokenRules: [],
            validators: [],
            status: STATUS.valid,
            complete: null
        };
        this._unsubscribeFromAllChangeEvents();
        this._pendingValidators = [];
        this._resetValidationInfo();
        each(this.validators, (_, validator) => {
            var validatorResult = validator.validate();
            result.isValid = result.isValid && validatorResult.isValid;
            if (validatorResult.brokenRules) {
                result.brokenRules = result.brokenRules.concat(validatorResult.brokenRules)
            }
            result.validators.push(validator);
            if (validatorResult.status === STATUS.pending) {
                this._addPendingValidator(validator)
            }
            this._subscribeToChangeEvents(validator)
        });
        if (this._pendingValidators.length) {
            result.status = STATUS.pending
        } else {
            result.status = result.isValid ? STATUS.valid : STATUS.invalid;
            this._unsubscribeFromAllChangeEvents();
            this._raiseValidatedEvent(result)
        }
        this._updateValidationInfo(result);
        return extend({}, this._validationInfo.result)
    },
    _subscribeToChangeEvents(validator) {
        validator.on("validating", this._onValidatorStatusChanged);
        validator.on("validated", this._onValidatorStatusChanged)
    },
    _unsubscribeFromChangeEvents(validator) {
        validator.off("validating", this._onValidatorStatusChanged);
        validator.off("validated", this._onValidatorStatusChanged)
    },
    _unsubscribeFromAllChangeEvents() {
        each(this.validators, (_, validator) => {
            this._unsubscribeFromChangeEvents(validator)
        })
    },
    _updateValidationInfo(result) {
        this._validationInfo.result = result;
        if (result.status !== STATUS.pending) {
            return
        }
        if (!this._validationInfo.deferred) {
            this._validationInfo.deferred = new Deferred;
            this._validationInfo.result.complete = this._validationInfo.deferred.promise()
        }
    },
    _addPendingValidator(validator) {
        var foundValidator = grep(this._pendingValidators, (function(val) {
            return val === validator
        }))[0];
        if (!foundValidator) {
            this._pendingValidators.push(validator)
        }
    },
    _removePendingValidator(validator) {
        var index = inArray(validator, this._pendingValidators);
        if (index >= 0) {
            this._pendingValidators.splice(index, 1)
        }
    },
    _orderBrokenRules(brokenRules) {
        var orderedRules = [];
        each(this.validators, (function(_, validator) {
            var foundRules = grep(brokenRules, (function(rule) {
                return rule.validator === validator
            }));
            if (foundRules.length) {
                orderedRules = orderedRules.concat(foundRules)
            }
        }));
        return orderedRules
    },
    _updateBrokenRules(result) {
        if (!this._validationInfo.result) {
            return
        }
        var brokenRules = this._validationInfo.result.brokenRules;
        var rules = grep(brokenRules, (function(rule) {
            return rule.validator !== result.validator
        }));
        if (result.brokenRules) {
            brokenRules = rules.concat(result.brokenRules)
        }
        this._validationInfo.result.brokenRules = this._orderBrokenRules(brokenRules)
    },
    _onValidatorStatusChanged(result) {
        if (result.status === STATUS.pending) {
            this._addPendingValidator(result.validator);
            return
        }
        this._resolveIfComplete(result)
    },
    _resolveIfComplete(result) {
        this._removePendingValidator(result.validator);
        this._updateBrokenRules(result);
        if (!this._pendingValidators.length) {
            this._unsubscribeFromAllChangeEvents();
            if (!this._validationInfo.result) {
                return
            }
            this._validationInfo.result.status = 0 === this._validationInfo.result.brokenRules.length ? STATUS.valid : STATUS.invalid;
            this._validationInfo.result.isValid = this._validationInfo.result.status === STATUS.valid;
            var res = extend({}, this._validationInfo.result, {
                complete: null
            });
            var deferred = this._validationInfo.deferred;
            this._validationInfo.deferred = null;
            this._raiseValidatedEvent(res);
            deferred && setTimeout(() => {
                deferred.resolve(res)
            })
        }
    },
    _raiseValidatedEvent(result) {
        this._eventsStrategy.fireEvent("validated", [result])
    },
    _resetValidationInfo() {
        this._validationInfo = {
            result: null,
            deferred: null
        }
    },
    _synchronizeValidationInfo() {
        if (this._validationInfo.result) {
            this._validationInfo.result.validators = this.validators
        }
    },
    removeRegisteredValidator(validator) {
        var index = inArray(validator, this.validators);
        if (index > -1) {
            this.validators.splice(index, 1);
            this._synchronizeValidationInfo();
            this._resolveIfComplete({
                validator: validator
            })
        }
    },
    registerValidator(validator) {
        if (inArray(validator, this.validators) < 0) {
            this.validators.push(validator);
            this._synchronizeValidationInfo()
        }
    },
    reset() {
        each(this.validators, (function(_, validator) {
            validator.reset()
        }));
        this._pendingValidators = [];
        this._resetValidationInfo()
    },
    on(eventName, eventHandler) {
        this._eventsStrategy.on(eventName, eventHandler);
        return this
    },
    off(eventName, eventHandler) {
        this._eventsStrategy.off(eventName, eventHandler);
        return this
    }
});
var ValidationEngine = {
    groups: [],
    getGroupConfig(group) {
        var result = grep(this.groups, (function(config) {
            return config.group === group
        }));
        if (result.length) {
            return result[0]
        }
    },
    findGroup($element, model) {
        var $dxGroup = $element.parents(".dx-validationgroup").first();
        if ($dxGroup.length) {
            return $dxGroup.dxValidationGroup("instance")
        }
        return model
    },
    initGroups() {
        this.groups = [];
        this.addGroup()
    },
    addGroup(group) {
        var config = this.getGroupConfig(group);
        if (!config) {
            config = new GroupConfig(group);
            this.groups.push(config)
        }
        return config
    },
    removeGroup(group) {
        var config = this.getGroupConfig(group);
        var index = inArray(config, this.groups);
        if (index > -1) {
            this.groups.splice(index, 1)
        }
        return config
    },
    _setDefaultMessage(info) {
        var {
            rule: rule,
            validator: validator,
            name: name
        } = info;
        if (!isDefined(rule.message)) {
            if (validator.defaultFormattedMessage && isDefined(name)) {
                rule.message = validator.defaultFormattedMessage(name)
            } else {
                rule.message = validator.defaultMessage()
            }
        }
    },
    _addBrokenRule(info) {
        var {
            result: result,
            rule: rule
        } = info;
        if (!result.brokenRule) {
            result.brokenRule = rule
        }
        if (!result.brokenRules) {
            result.brokenRules = []
        }
        result.brokenRules.push(rule)
    },
    validate(value, rules, name) {
        var _rules$;
        var result = {
            name: name,
            value: value,
            brokenRule: null,
            brokenRules: null,
            isValid: true,
            validationRules: rules,
            pendingRules: null,
            status: STATUS.valid,
            complete: null
        };
        var validator = null === rules || void 0 === rules ? void 0 : null === (_rules$ = rules[0]) || void 0 === _rules$ ? void 0 : _rules$.validator;
        var asyncRuleItems = [];
        each(rules || [], (_, rule) => {
            var ruleValidator = rulesValidators[rule.type];
            var ruleValidationResult;
            if (ruleValidator) {
                if (isDefined(rule.isValid) && rule.value === value && !rule.reevaluate) {
                    if (!rule.isValid) {
                        result.isValid = false;
                        this._addBrokenRule({
                            result: result,
                            rule: rule
                        });
                        return false
                    }
                    return true
                }
                rule.value = value;
                if ("async" === rule.type) {
                    asyncRuleItems.push({
                        rule: rule,
                        ruleValidator: ruleValidator
                    });
                    return true
                }
                ruleValidationResult = ruleValidator.validate(value, rule);
                rule.isValid = ruleValidationResult;
                if (!ruleValidationResult) {
                    result.isValid = false;
                    this._setDefaultMessage({
                        rule: rule,
                        validator: ruleValidator,
                        name: name
                    });
                    this._addBrokenRule({
                        result: result,
                        rule: rule
                    })
                }
                if (!rule.isValid) {
                    return false
                }
            } else {
                throw errors.Error("E0100")
            }
        });
        if (result.isValid && !result.brokenRules && asyncRuleItems.length) {
            result = this._validateAsyncRules({
                value: value,
                items: asyncRuleItems,
                result: result,
                name: name
            })
        }
        this._synchronizeGroupValidationInfo(validator, result);
        result.status = result.pendingRules ? STATUS.pending : result.isValid ? STATUS.valid : STATUS.invalid;
        return result
    },
    _synchronizeGroupValidationInfo(validator, result) {
        var _result$brokenRules;
        if (!validator) {
            return
        }
        var groupConfig = ValidationEngine.getGroupConfig(validator._validationGroup);
        groupConfig._updateBrokenRules.call(groupConfig, {
            validator: validator,
            brokenRules: null !== (_result$brokenRules = result.brokenRules) && void 0 !== _result$brokenRules ? _result$brokenRules : []
        })
    },
    _validateAsyncRules(_ref) {
        var {
            result: result,
            value: value,
            items: items,
            name: name
        } = _ref;
        var asyncResults = [];
        each(items, (_, item) => {
            var validateResult = item.ruleValidator.validate(value, item.rule);
            if (!isPromise(validateResult)) {
                this._updateRuleConfig({
                    rule: item.rule,
                    ruleResult: this._getPatchedRuleResult(validateResult),
                    validator: item.ruleValidator,
                    name: name
                })
            } else {
                if (!result.pendingRules) {
                    result.pendingRules = []
                }
                result.pendingRules.push(item.rule);
                var asyncResult = validateResult.then(res => {
                    var ruleResult = this._getPatchedRuleResult(res);
                    this._updateRuleConfig({
                        rule: item.rule,
                        ruleResult: ruleResult,
                        validator: item.ruleValidator,
                        name: name
                    });
                    return ruleResult
                });
                asyncResults.push(asyncResult)
            }
        });
        if (asyncResults.length) {
            result.complete = Promise.all(asyncResults).then(values => this._getAsyncRulesResult({
                result: result,
                values: values
            }))
        }
        return result
    },
    _updateRuleConfig(_ref2) {
        var {
            rule: rule,
            ruleResult: ruleResult,
            validator: validator,
            name: name
        } = _ref2;
        rule.isValid = ruleResult.isValid;
        if (!ruleResult.isValid) {
            if (isDefined(ruleResult.message) && isString(ruleResult.message) && ruleResult.message.length) {
                rule.message = ruleResult.message
            } else {
                this._setDefaultMessage({
                    rule: rule,
                    validator: validator,
                    name: name
                })
            }
        }
    },
    _getPatchedRuleResult(ruleResult) {
        var result;
        if (isObject(ruleResult)) {
            result = extend({}, ruleResult);
            if (!isDefined(result.isValid)) {
                result.isValid = true
            }
        } else {
            result = {
                isValid: isBoolean(ruleResult) ? ruleResult : true
            }
        }
        return result
    },
    _getAsyncRulesResult(_ref3) {
        var {
            values: values,
            result: result
        } = _ref3;
        each(values, (index, val) => {
            if (false === val.isValid) {
                result.isValid = val.isValid;
                var rule = result.pendingRules[index];
                this._addBrokenRule({
                    result: result,
                    rule: rule
                })
            }
        });
        result.pendingRules = null;
        result.complete = null;
        result.status = result.isValid ? STATUS.valid : STATUS.invalid;
        return result
    },
    registerValidatorInGroup(group, validator) {
        var groupConfig = ValidationEngine.addGroup(group);
        groupConfig.registerValidator.call(groupConfig, validator)
    },
    _shouldRemoveGroup(group, validatorsInGroup) {
        var isDefaultGroup = void 0 === group;
        var isValidationGroupInstance = group && "dxValidationGroup" === group.NAME;
        return !isDefaultGroup && !isValidationGroupInstance && !validatorsInGroup.length
    },
    removeRegisteredValidator(group, validator) {
        var config = ValidationEngine.getGroupConfig(group);
        if (config) {
            config.removeRegisteredValidator.call(config, validator);
            var validatorsInGroup = config.validators;
            if (this._shouldRemoveGroup(group, validatorsInGroup)) {
                this.removeGroup(group)
            }
        }
    },
    initValidationOptions(options) {
        var initedOptions = {};
        if (options) {
            ["isValid", "validationStatus", "validationError", "validationErrors"].forEach(prop => {
                if (prop in options) {
                    extend(initedOptions, this.synchronizeValidationOptions({
                        name: prop,
                        value: options[prop]
                    }, options))
                }
            })
        }
        return initedOptions
    },
    synchronizeValidationOptions(_ref4, options) {
        var {
            name: name,
            value: value
        } = _ref4;
        switch (name) {
            case "validationStatus":
                var isValid = value === STATUS.valid || value === STATUS.pending;
                return options.isValid !== isValid ? {
                    isValid: isValid
                } : {};
            case "isValid":
                var {
                    validationStatus: validationStatus
                } = options;
                var newStatus = validationStatus;
                if (value && validationStatus === STATUS.invalid) {
                    newStatus = STATUS.valid
                } else if (!value && validationStatus !== STATUS.invalid) {
                    newStatus = STATUS.invalid
                }
                return newStatus !== validationStatus ? {
                    validationStatus: newStatus
                } : {};
            case "validationErrors":
                var validationError = !value || !value.length ? null : value[0];
                return options.validationError !== validationError ? {
                    validationError: validationError
                } : {};
            case "validationError":
                var {
                    validationErrors: validationErrors
                } = options;
                if (!value && validationErrors) {
                    return {
                        validationErrors: null
                    }
                } else if (value && !validationErrors) {
                    return {
                        validationErrors: [value]
                    }
                } else if (value && validationErrors && value !== validationErrors[0]) {
                    validationErrors[0] = value;
                    return {
                        validationErrors: validationErrors.slice()
                    }
                }
        }
        return {}
    },
    validateGroup(group) {
        var groupConfig = ValidationEngine.getGroupConfig(group);
        if (!groupConfig) {
            throw errors.Error("E0110")
        }
        return groupConfig.validate()
    },
    resetGroup(group) {
        var groupConfig = ValidationEngine.getGroupConfig(group);
        if (!groupConfig) {
            throw errors.Error("E0110")
        }
        return groupConfig.reset()
    }
};
ValidationEngine.initGroups();
export default ValidationEngine;
