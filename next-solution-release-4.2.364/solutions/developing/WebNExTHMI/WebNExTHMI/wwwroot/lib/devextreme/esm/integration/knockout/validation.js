/**
 * DevExtreme (esm/integration/knockout/validation.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    each,
    map
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import Class from "../../core/class";
import {
    EventsStrategy
} from "../../core/events_strategy";
import ValidationEngine from "../../ui/validation_engine";
import {
    Deferred
} from "../../core/utils/deferred";
import Guid from "../../core/guid";
import ko from "knockout";
if (ko) {
    var VALIDATION_STATUS_VALID = "valid";
    var VALIDATION_STATUS_PENDING = "pending";
    var koDxValidator = Class.inherit({
        ctor(target, _ref) {
            var {
                name: name,
                validationRules: validationRules
            } = _ref;
            this.target = target;
            this.name = name;
            this.isValid = ko.observable(true);
            this.validationError = ko.observable();
            this.validationErrors = ko.observable();
            this.validationStatus = ko.observable(VALIDATION_STATUS_VALID);
            this._eventsStrategy = new EventsStrategy(this);
            this.validationRules = map(validationRules, (rule, index) => extend({}, rule, {
                validator: this,
                index: index
            }));
            this._validationInfo = {
                result: null,
                deferred: null
            }
        },
        _updateValidationResult(result) {
            if (!this._validationInfo.result || this._validationInfo.result.id !== result.id) {
                var complete = this._validationInfo.deferred && this._validationInfo.result.complete;
                this._validationInfo.result = extend({}, result, {
                    complete: complete
                })
            } else {
                for (var prop in result) {
                    if ("id" !== prop && "complete" !== prop) {
                        this._validationInfo.result[prop] = result[prop]
                    }
                }
            }
        },
        validate() {
            var currentResult = this._validationInfo && this._validationInfo.result;
            var value = this.target();
            if (currentResult && currentResult.status === VALIDATION_STATUS_PENDING && currentResult.value === value) {
                return extend({}, currentResult)
            }
            var result = ValidationEngine.validate(value, this.validationRules, this.name);
            result.id = (new Guid).toString();
            this._applyValidationResult(result);
            result.complete && result.complete.then(res => {
                if (res.id === this._validationInfo.result.id) {
                    this._applyValidationResult(res)
                }
            });
            return extend({}, this._validationInfo.result)
        },
        reset() {
            this.target(null);
            var result = {
                id: null,
                isValid: true,
                brokenRule: null,
                pendingRules: null,
                status: VALIDATION_STATUS_VALID,
                complete: null
            };
            this._applyValidationResult(result);
            return result
        },
        _applyValidationResult(result) {
            result.validator = this;
            this._updateValidationResult(result);
            this.target.dxValidator.isValid(this._validationInfo.result.isValid);
            this.target.dxValidator.validationError(this._validationInfo.result.brokenRule);
            this.target.dxValidator.validationErrors(this._validationInfo.result.brokenRules);
            this.target.dxValidator.validationStatus(this._validationInfo.result.status);
            if (result.status === VALIDATION_STATUS_PENDING) {
                if (!this._validationInfo.deferred) {
                    this._validationInfo.deferred = new Deferred;
                    this._validationInfo.result.complete = this._validationInfo.deferred.promise()
                }
                this._eventsStrategy.fireEvent("validating", [this._validationInfo.result]);
                return
            }
            if (result.status !== VALIDATION_STATUS_PENDING) {
                this._eventsStrategy.fireEvent("validated", [result]);
                if (this._validationInfo.deferred) {
                    this._validationInfo.deferred.resolve(result);
                    this._validationInfo.deferred = null
                }
            }
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
    ko.extenders.dxValidator = function(target, option) {
        target.dxValidator = new koDxValidator(target, option);
        target.subscribe(target.dxValidator.validate.bind(target.dxValidator));
        return target
    };
    ValidationEngine.registerModelForValidation = function(model) {
        each(model, (function(name, member) {
            if (ko.isObservable(member) && member.dxValidator) {
                ValidationEngine.registerValidatorInGroup(model, member.dxValidator)
            }
        }))
    };
    ValidationEngine.unregisterModelForValidation = function(model) {
        each(model, (function(name, member) {
            if (ko.isObservable(member) && member.dxValidator) {
                ValidationEngine.removeRegisteredValidator(model, member.dxValidator)
            }
        }))
    };
    ValidationEngine.validateModel = ValidationEngine.validateGroup
}
