/**
 * DevExtreme (esm/core/component.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Config from "./config";
import {
    extend
} from "./utils/extend";
import {
    Options
} from "./options/index";
import {
    convertRulesToOptions
} from "./options/utils";
import Class from "./class";
import Action from "./action";
import errors from "./errors";
import Callbacks from "./utils/callbacks";
import {
    EventsStrategy
} from "./events_strategy";
import {
    name as publicComponentName
} from "./utils/public_component";
import {
    PostponedOperations
} from "./postponed_operations";
import {
    isFunction,
    isPlainObject,
    isDefined
} from "./utils/type";
import {
    noop
} from "./utils/common";
import {
    getPathParts
} from "./utils/data";
var getEventName = actionName => actionName.charAt(2).toLowerCase() + actionName.substr(3);
var isInnerOption = optionName => 0 === optionName.indexOf("_", 0);
export var Component = Class.inherit({
    _setDeprecatedOptions() {
        this._deprecatedOptions = {}
    },
    _getDeprecatedOptions() {
        return this._deprecatedOptions
    },
    _getDefaultOptions: () => ({
        onInitialized: null,
        onOptionChanged: null,
        onDisposing: null,
        defaultOptionsRules: null
    }),
    _defaultOptionsRules: () => [],
    _setOptionsByDevice(rules) {
        this._options.applyRules(rules)
    },
    _convertRulesToOptions: rules => convertRulesToOptions(rules),
    _isInitialOptionValue(name) {
        return this._options.isInitial(name)
    },
    _setOptionsByReference() {
        this._optionsByReference = {}
    },
    _getOptionsByReference() {
        return this._optionsByReference
    },
    ctor() {
        var options = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : {};
        var {
            _optionChangedCallbacks: _optionChangedCallbacks,
            _disposingCallbacks: _disposingCallbacks
        } = options;
        this.NAME = publicComponentName(this.constructor);
        this._eventsStrategy = EventsStrategy.create(this, options.eventsStrategy);
        this._updateLockCount = 0;
        this._optionChangedCallbacks = _optionChangedCallbacks || Callbacks();
        this._disposingCallbacks = _disposingCallbacks || Callbacks();
        this.postponedOperations = new PostponedOperations;
        this._createOptions(options)
    },
    _createOptions(options) {
        this.beginUpdate();
        try {
            this._setOptionsByReference();
            this._setDeprecatedOptions();
            this._options = new Options(this._getDefaultOptions(), this._getDefaultOptions(), this._getOptionsByReference(), this._getDeprecatedOptions());
            this._options.onChanging((name, previousValue, value) => this._initialized && this._optionChanging(name, previousValue, value));
            this._options.onDeprecated((option, info) => this._logDeprecatedOptionWarning(option, info));
            this._options.onChanged((name, value, previousValue) => this._notifyOptionChanged(name, value, previousValue));
            this._options.onStartChange(() => this.beginUpdate());
            this._options.onEndChange(() => this.endUpdate());
            this._options.addRules(this._defaultOptionsRules());
            if (options && options.onInitializing) {
                options.onInitializing.apply(this, [options])
            }
            this._setOptionsByDevice(options.defaultOptionsRules);
            this._initOptions(options)
        } finally {
            this.endUpdate()
        }
    },
    _initOptions(options) {
        this.option(options)
    },
    _init() {
        this._createOptionChangedAction();
        this.on("disposing", args => {
            this._disposingCallbacks.fireWith(this, [args])
        })
    },
    _logDeprecatedOptionWarning(option, info) {
        var message = info.message || "Use the '".concat(info.alias, "' option instead");
        errors.log("W0001", this.NAME, option, info.since, message)
    },
    _logDeprecatedComponentWarning(since, alias) {
        errors.log("W0000", this.NAME, since, "Use the '".concat(alias, "' widget instead"))
    },
    _createOptionChangedAction() {
        this._optionChangedAction = this._createActionByOption("onOptionChanged", {
            excludeValidators: ["disabled", "readOnly"]
        })
    },
    _createDisposingAction() {
        this._disposingAction = this._createActionByOption("onDisposing", {
            excludeValidators: ["disabled", "readOnly"]
        })
    },
    _optionChanged(args) {
        switch (args.name) {
            case "onDisposing":
            case "onInitialized":
                break;
            case "onOptionChanged":
                this._createOptionChangedAction()
        }
    },
    _dispose() {
        this._optionChangedCallbacks.empty();
        this._createDisposingAction();
        this._disposingAction();
        this._eventsStrategy.dispose();
        this._options.dispose();
        this._disposed = true
    },
    _lockUpdate() {
        this._updateLockCount++
    },
    _unlockUpdate() {
        this._updateLockCount = Math.max(this._updateLockCount - 1, 0)
    },
    _isUpdateAllowed() {
        return 0 === this._updateLockCount
    },
    _isInitializingRequired() {
        return !this._initializing && !this._initialized
    },
    isInitialized() {
        return this._initialized
    },
    _commitUpdate() {
        this.postponedOperations.callPostponedOperations();
        this._isInitializingRequired() && this._initializeComponent()
    },
    _initializeComponent() {
        this._initializing = true;
        try {
            this._init()
        } finally {
            this._initializing = false;
            this._lockUpdate();
            this._createActionByOption("onInitialized", {
                excludeValidators: ["disabled", "readOnly"]
            })();
            this._unlockUpdate();
            this._initialized = true
        }
    },
    instance() {
        return this
    },
    beginUpdate: function() {
        this._lockUpdate()
    },
    endUpdate: function() {
        this._unlockUpdate();
        this._isUpdateAllowed() && this._commitUpdate()
    },
    _optionChanging: noop,
    _notifyOptionChanged(option, value, previousValue) {
        if (this._initialized) {
            var optionNames = [option].concat(this._options.getAliasesByName(option));
            for (var i = 0; i < optionNames.length; i++) {
                var name = optionNames[i];
                var args = {
                    name: getPathParts(name)[0],
                    fullName: name,
                    value: value,
                    previousValue: previousValue
                };
                if (!isInnerOption(name)) {
                    this._optionChangedCallbacks.fireWith(this, [extend(this._defaultActionArgs(), args)]);
                    this._optionChangedAction(extend({}, args))
                }
                if (!this._disposed && this._cancelOptionChange !== name) {
                    this._optionChanged(args)
                }
            }
        }
    },
    initialOption(name) {
        return this._options.initial(name)
    },
    _defaultActionConfig() {
        return {
            context: this,
            component: this
        }
    },
    _defaultActionArgs() {
        return {
            component: this
        }
    },
    _createAction(actionSource, config) {
        var action;
        return e => {
            if (!isDefined(e)) {
                e = {}
            }
            if (!isPlainObject(e)) {
                e = {
                    actionValue: e
                }
            }
            action = action || new Action(actionSource, extend(config, this._defaultActionConfig()));
            return action.execute.call(action, extend(e, this._defaultActionArgs()))
        }
    },
    _createActionByOption(optionName, config) {
        var _this = this;
        var action;
        var eventName;
        var actionFunc;
        var result = function() {
            if (!eventName) {
                config = config || {};
                if ("string" !== typeof optionName) {
                    throw errors.Error("E0008")
                }
                if (0 === optionName.indexOf("on")) {
                    eventName = getEventName(optionName)
                }
                actionFunc = _this.option()[optionName]
            }
            if (!action && !actionFunc && !config.beforeExecute && !config.afterExecute && !_this._eventsStrategy.hasEvent(eventName)) {
                return
            }
            if (!action) {
                var beforeExecute = config.beforeExecute;
                config.beforeExecute = function() {
                    for (var _len2 = arguments.length, props = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
                        props[_key2] = arguments[_key2]
                    }
                    beforeExecute && beforeExecute.apply(_this, props);
                    _this._eventsStrategy.fireEvent(eventName, props[0].args)
                };
                action = _this._createAction(actionFunc, config)
            }
            for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
                args[_key] = arguments[_key]
            }
            if (Config().wrapActionsBeforeExecute) {
                var beforeActionExecute = _this.option("beforeActionExecute") || noop;
                var wrappedAction = beforeActionExecute(_this, action, config) || action;
                return wrappedAction.apply(_this, args)
            }
            return action.apply(_this, args)
        };
        if (Config().wrapActionsBeforeExecute) {
            return result
        }
        var onActionCreated = this.option("onActionCreated") || noop;
        return onActionCreated(this, result, config) || result
    },
    on(eventName, eventHandler) {
        this._eventsStrategy.on(eventName, eventHandler);
        return this
    },
    off(eventName, eventHandler) {
        this._eventsStrategy.off(eventName, eventHandler);
        return this
    },
    hasActionSubscription: function(actionName) {
        return !!this._options.silent(actionName) || this._eventsStrategy.hasEvent(getEventName(actionName))
    },
    isOptionDeprecated(name) {
        return this._options.isDeprecated(name)
    },
    _setOptionWithoutOptionChange(name, value) {
        this._cancelOptionChange = name;
        this.option(name, value);
        this._cancelOptionChange = false
    },
    _getOptionValue(name, context) {
        var value = this.option(name);
        if (isFunction(value)) {
            return value.bind(context)()
        }
        return value
    },
    option() {
        return this._options.option(...arguments)
    },
    resetOption(name) {
        this.beginUpdate();
        this._options.reset(name);
        this.endUpdate()
    }
});
