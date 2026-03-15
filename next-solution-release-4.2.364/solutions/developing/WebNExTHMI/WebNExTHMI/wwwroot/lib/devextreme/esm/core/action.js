/**
 * DevExtreme (esm/core/action.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "./renderer";
import {
    getWindow
} from "./utils/window";
import {
    isPlainObject,
    isFunction
} from "./utils/type";
import {
    each
} from "./utils/iterator";
export default class Action {
    constructor(action, config) {
        config = config || {};
        this._action = action;
        this._context = config.context || getWindow();
        this._beforeExecute = config.beforeExecute;
        this._afterExecute = config.afterExecute;
        this._component = config.component;
        this._validatingTargetName = config.validatingTargetName;
        var excludeValidators = this._excludeValidators = {};
        if (config.excludeValidators) {
            for (var i = 0; i < config.excludeValidators.length; i++) {
                excludeValidators[config.excludeValidators[i]] = true
            }
        }
    }
    execute() {
        var e = {
            action: this._action,
            args: Array.prototype.slice.call(arguments),
            context: this._context,
            component: this._component,
            validatingTargetName: this._validatingTargetName,
            cancel: false,
            handled: false
        };
        var beforeExecute = this._beforeExecute;
        var afterExecute = this._afterExecute;
        var argsBag = e.args[0] || {};
        if (!this._validateAction(e)) {
            return
        }
        null === beforeExecute || void 0 === beforeExecute ? void 0 : beforeExecute.call(this._context, e);
        if (e.cancel) {
            return
        }
        var result = this._executeAction(e);
        if (argsBag.cancel) {
            return
        }
        null === afterExecute || void 0 === afterExecute ? void 0 : afterExecute.call(this._context, e);
        return result
    }
    _validateAction(e) {
        var excludeValidators = this._excludeValidators;
        var {
            executors: executors
        } = Action;
        for (var name in executors) {
            if (!excludeValidators[name]) {
                var _executor$validate;
                var executor = executors[name];
                null === (_executor$validate = executor.validate) || void 0 === _executor$validate ? void 0 : _executor$validate.call(executor, e);
                if (e.cancel) {
                    return false
                }
            }
        }
        return true
    }
    _executeAction(e) {
        var result;
        var {
            executors: executors
        } = Action;
        for (var name in executors) {
            var _executor$execute;
            var executor = executors[name];
            null === (_executor$execute = executor.execute) || void 0 === _executor$execute ? void 0 : _executor$execute.call(executor, e);
            if (e.handled) {
                result = e.result;
                break
            }
        }
        return result
    }
    static registerExecutor(name, executor) {
        if (isPlainObject(name)) {
            each(name, Action.registerExecutor);
            return
        }
        Action.executors[name] = executor
    }
    static unregisterExecutor() {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key]
        }
        each(args, (function() {
            delete Action.executors[this]
        }))
    }
}
Action.executors = {};
var createValidatorByTargetElement = condition => e => {
    if (!e.args.length) {
        return
    }
    var args = e.args[0];
    var element = args[e.validatingTargetName] || args.element;
    if (element && condition($(element))) {
        e.cancel = true
    }
};
Action.registerExecutor({
    disabled: {
        validate: createValidatorByTargetElement($target => $target.is(".dx-state-disabled, .dx-state-disabled *"))
    },
    readOnly: {
        validate: createValidatorByTargetElement($target => $target.is(".dx-state-readonly, .dx-state-readonly *:not(.dx-state-independent)"))
    },
    undefined: {
        execute: e => {
            if (!e.action) {
                e.result = void 0;
                e.handled = true
            }
        }
    },
    func: {
        execute: e => {
            if (isFunction(e.action)) {
                e.result = e.action.call(e.context, e.args[0]);
                e.handled = true
            }
        }
    }
});
