/**
 * DevExtreme (esm/integration/angular/component_registrator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import angular from "angular";
import eventsEngine from "../../events/core/events_engine";
import Config from "../../core/config";
import registerComponentCallbacks from "../../core/component_registrator_callbacks";
import Class from "../../core/class";
import Callbacks from "../../core/utils/callbacks";
import {
    type,
    isDefined,
    isNumeric
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    inArray
} from "../../core/utils/array";
import Locker from "../../core/utils/locker";
import Editor from "../../ui/editor/editor";
import {
    NgTemplate
} from "./template";
import ngModule from "./module";
import CollectionWidget from "../../ui/collection/ui.collection_widget.edit";
import {
    compileGetter,
    compileSetter
} from "../../core/utils/data";
import {
    extendFromObject
} from "../../core/utils/extend";
import {
    equals
} from "../../core/utils/comparator";
import {
    dasherize
} from "../../core/utils/inflector";
var ITEM_ALIAS_ATTRIBUTE_NAME = "dxItemAlias";
var SKIP_APPLY_ACTION_CATEGORIES = ["rendering"];
var NG_MODEL_OPTION = "value";
if (angular) {
    var safeApply = (func, scope) => {
        if (scope.$root.$$phase) {
            return func(scope)
        } else {
            return scope.$apply(() => func(scope))
        }
    };
    var getClassMethod = (initClass, methodName) => {
        var hasParentProperty = Object.prototype.hasOwnProperty.bind(initClass)("parent");
        var isES6Class = !hasParentProperty && initClass.parent;
        if (isES6Class) {
            var baseClass = Object.getPrototypeOf(initClass);
            return baseClass.prototype[methodName] ? () => baseClass.prototype[methodName]() : getClassMethod(baseClass, methodName)
        } else {
            var method = initClass.parent.prototype[methodName];
            if (method) {
                return () => method()
            }
            if (!method || !initClass.parent.subclassOf) {
                return () => {}
            }
            return getClassMethod(initClass.parent, methodName)
        }
    };
    var ComponentBuilder = Class.inherit({
        ctor(options) {
            this._componentDisposing = Callbacks();
            this._optionChangedCallbacks = Callbacks();
            this._ngLocker = new Locker;
            this._scope = options.scope;
            this._$element = options.$element;
            this._$templates = options.$templates;
            this._componentClass = options.componentClass;
            this._parse = options.parse;
            this._compile = options.compile;
            this._itemAlias = options.itemAlias;
            this._transcludeFn = options.transcludeFn;
            this._digestCallbacks = options.dxDigestCallbacks;
            this._normalizeOptions(options.ngOptions);
            this._initComponentBindings();
            this._initComponent(this._scope);
            if (!options.ngOptions) {
                this._addOptionsStringWatcher(options.ngOptionsString)
            }
        },
        _addOptionsStringWatcher(optionsString) {
            var clearOptionsStringWatcher = this._scope.$watch(optionsString, newOptions => {
                if (!newOptions) {
                    return
                }
                clearOptionsStringWatcher();
                this._normalizeOptions(newOptions);
                this._initComponentBindings();
                this._component.option(this._evalOptions(this._scope))
            });
            this._componentDisposing.add(clearOptionsStringWatcher)
        },
        _normalizeOptions(options) {
            this._ngOptions = extendFromObject({}, options);
            if (!options) {
                return
            }
            if (!Object.prototype.hasOwnProperty.call(options, "bindingOptions") && options.bindingOptions) {
                this._ngOptions.bindingOptions = options.bindingOptions
            }
            if (options.bindingOptions) {
                each(options.bindingOptions, (key, value) => {
                    if ("string" === type(value)) {
                        this._ngOptions.bindingOptions[key] = {
                            dataPath: value
                        }
                    }
                })
            }
        },
        _initComponent(scope) {
            this._component = new this._componentClass(this._$element, this._evalOptions(scope));
            this._component._isHidden = true;
            this._handleDigestPhase()
        },
        _handleDigestPhase() {
            var beginUpdate = () => {
                this._component.beginUpdate()
            };
            var endUpdate = () => {
                this._component.endUpdate()
            };
            this._digestCallbacks.begin.add(beginUpdate);
            this._digestCallbacks.end.add(endUpdate);
            this._componentDisposing.add(() => {
                this._digestCallbacks.begin.remove(beginUpdate);
                this._digestCallbacks.end.remove(endUpdate)
            })
        },
        _initComponentBindings() {
            var optionDependencies = {};
            if (!this._ngOptions.bindingOptions) {
                return
            }
            each(this._ngOptions.bindingOptions, (optionPath, value) => {
                var separatorIndex = optionPath.search(/\[|\./);
                var optionForSubscribe = separatorIndex > -1 ? optionPath.substring(0, separatorIndex) : optionPath;
                var prevWatchMethod;
                var clearWatcher;
                var valuePath = value.dataPath;
                var deepWatch = true;
                var forcePlainWatchMethod = false;
                if (void 0 !== value.deep) {
                    forcePlainWatchMethod = deepWatch = !!value.deep
                }
                if (!optionDependencies[optionForSubscribe]) {
                    optionDependencies[optionForSubscribe] = {}
                }
                optionDependencies[optionForSubscribe][optionPath] = valuePath;
                var updateWatcher = () => {
                    var watchMethod = Array.isArray(this._scope.$eval(valuePath)) && !forcePlainWatchMethod ? "$watchCollection" : "$watch";
                    if (prevWatchMethod !== watchMethod) {
                        if (clearWatcher) {
                            clearWatcher()
                        }
                        clearWatcher = this._scope[watchMethod](valuePath, (newValue, oldValue) => {
                            if (this._ngLocker.locked(optionPath)) {
                                return
                            }
                            this._ngLocker.obtain(optionPath);
                            this._component.option(optionPath, newValue);
                            updateWatcher();
                            if (equals(oldValue, newValue) && this._ngLocker.locked(optionPath)) {
                                this._ngLocker.release(optionPath)
                            }
                        }, deepWatch);
                        prevWatchMethod = watchMethod
                    }
                };
                updateWatcher();
                this._componentDisposing.add(clearWatcher)
            });
            this._optionChangedCallbacks.add(args => {
                var optionName = args.name;
                var fullName = args.fullName;
                var component = args.component;
                if (this._ngLocker.locked(fullName)) {
                    this._ngLocker.release(fullName);
                    return
                }
                if (!optionDependencies || !optionDependencies[optionName]) {
                    return
                }
                var isActivePhase = this._scope.$root.$$phase;
                var obtainOption = () => {
                    this._ngLocker.obtain(fullName)
                };
                if (isActivePhase) {
                    this._digestCallbacks.begin.add(obtainOption)
                } else {
                    obtainOption()
                }
                safeApply(() => {
                    each(optionDependencies[optionName], (optionPath, valuePath) => {
                        if (!this._optionsAreLinked(fullName, optionPath)) {
                            return
                        }
                        var value = component.option(optionPath);
                        this._parse(valuePath).assign(this._scope, value);
                        var scopeValue = this._parse(valuePath)(this._scope);
                        if (scopeValue !== value) {
                            args.component.option(optionPath, scopeValue)
                        }
                    })
                }, this._scope);
                var releaseOption = () => {
                    if (this._ngLocker.locked(fullName)) {
                        this._ngLocker.release(fullName)
                    }
                    this._digestCallbacks.begin.remove(obtainOption);
                    this._digestCallbacks.end.remove(releaseOption)
                };
                if (isActivePhase) {
                    this._digestCallbacks.end.addPrioritized(releaseOption)
                } else {
                    releaseOption()
                }
            })
        },
        _optionsAreNested(optionPath1, optionPath2) {
            var parentSeparator = optionPath1[optionPath2.length];
            return 0 === optionPath1.indexOf(optionPath2) && ("." === parentSeparator || "[" === parentSeparator)
        },
        _optionsAreLinked(optionPath1, optionPath2) {
            if (optionPath1 === optionPath2) {
                return true
            }
            return optionPath1.length > optionPath2.length ? this._optionsAreNested(optionPath1, optionPath2) : this._optionsAreNested(optionPath2, optionPath1)
        },
        _compilerByTemplate(template) {
            var scopeItemsPath = this._getScopeItemsPath();
            return options => {
                var $resultMarkup = $(template).clone();
                var dataIsScope = options.model && options.model.constructor === this._scope.$root.constructor;
                var templateScope = dataIsScope ? options.model : options.noModel ? this._scope : this._createScopeWithData(options);
                if (scopeItemsPath) {
                    this._synchronizeScopes(templateScope, scopeItemsPath, options.index)
                }
                $resultMarkup.appendTo(options.container);
                if (!options.noModel) {
                    eventsEngine.on($resultMarkup, "$destroy", () => {
                        var destroyAlreadyCalled = !templateScope.$parent;
                        if (destroyAlreadyCalled) {
                            return
                        }
                        templateScope.$destroy()
                    })
                }
                var ngTemplate = this._compile($resultMarkup, this._transcludeFn);
                this._applyAsync(scope => {
                    ngTemplate(scope, null, {
                        parentBoundTranscludeFn: this._transcludeFn
                    })
                }, templateScope);
                return $resultMarkup
            }
        },
        _applyAsync(func, scope) {
            func(scope);
            if (!scope.$root.$$phase) {
                if (!this._renderingTimer) {
                    var clearRenderingTimer = () => {
                        clearTimeout(this._renderingTimer)
                    };
                    this._renderingTimer = setTimeout(() => {
                        scope.$apply();
                        this._renderingTimer = null;
                        this._componentDisposing.remove(clearRenderingTimer)
                    });
                    this._componentDisposing.add(clearRenderingTimer)
                }
            }
        },
        _getScopeItemsPath() {
            if (this._componentClass.subclassOf(CollectionWidget) && this._ngOptions.bindingOptions && this._ngOptions.bindingOptions.items) {
                return this._ngOptions.bindingOptions.items.dataPath
            }
        },
        _createScopeWithData(options) {
            var newScope = this._scope.$new();
            if (this._itemAlias) {
                newScope[this._itemAlias] = options.model
            }
            if (isDefined(options.index)) {
                newScope.$index = options.index
            }
            return newScope
        },
        _synchronizeScopes(itemScope, parentPrefix, itemIndex) {
            if (this._itemAlias && "object" !== typeof itemScope[this._itemAlias]) {
                this._synchronizeScopeField({
                    parentScope: this._scope,
                    childScope: itemScope,
                    fieldPath: this._itemAlias,
                    parentPrefix: parentPrefix,
                    itemIndex: itemIndex
                })
            }
        },
        _synchronizeScopeField(args) {
            var parentScope = args.parentScope;
            var childScope = args.childScope;
            var fieldPath = args.fieldPath;
            var parentPrefix = args.parentPrefix;
            var itemIndex = args.itemIndex;
            var innerPathSuffix = fieldPath === this._itemAlias ? "" : "." + fieldPath;
            var collectionField = void 0 !== itemIndex;
            var optionOuterBag = [parentPrefix];
            if (collectionField) {
                if (!isNumeric(itemIndex)) {
                    return
                }
                optionOuterBag.push("[", itemIndex, "]")
            }
            optionOuterBag.push(innerPathSuffix);
            var optionOuterPath = optionOuterBag.join("");
            var clearParentWatcher = parentScope.$watch(optionOuterPath, (newValue, oldValue) => {
                if (newValue !== oldValue) {
                    compileSetter(fieldPath)(childScope, newValue)
                }
            });
            var clearItemWatcher = childScope.$watch(fieldPath, (newValue, oldValue) => {
                if (newValue !== oldValue) {
                    if (collectionField && !compileGetter(parentPrefix)(parentScope)[itemIndex]) {
                        clearItemWatcher();
                        return
                    }
                    compileSetter(optionOuterPath)(parentScope, newValue)
                }
            });
            this._componentDisposing.add([clearParentWatcher, clearItemWatcher])
        },
        _evalOptions(scope) {
            var result = extendFromObject({}, this._ngOptions);
            delete result.bindingOptions;
            if (this._ngOptions.bindingOptions) {
                each(this._ngOptions.bindingOptions, (key, value) => {
                    result[key] = scope.$eval(value.dataPath)
                })
            }
            result._optionChangedCallbacks = this._optionChangedCallbacks;
            result._disposingCallbacks = this._componentDisposing;
            result.onActionCreated = (component, action, config) => {
                if (config && inArray(config.category, SKIP_APPLY_ACTION_CATEGORIES) > -1) {
                    return action
                }
                return function() {
                    var args = arguments;
                    if (!scope || !scope.$root || scope.$root.$$phase) {
                        return action.apply(this, args)
                    }
                    return safeApply(() => action.apply(this, args), scope)
                }
            };
            result.beforeActionExecute = result.onActionCreated;
            result.nestedComponentOptions = component => ({
                templatesRenderAsynchronously: component.option("templatesRenderAsynchronously"),
                forceApplyBindings: component.option("forceApplyBindings"),
                modelByElement: component.option("modelByElement"),
                onActionCreated: component.option("onActionCreated"),
                beforeActionExecute: component.option("beforeActionExecute"),
                nestedComponentOptions: component.option("nestedComponentOptions")
            });
            result.templatesRenderAsynchronously = true;
            if (Config().wrapActionsBeforeExecute) {
                result.forceApplyBindings = () => {
                    safeApply(() => {}, scope)
                }
            }
            result.integrationOptions = {
                createTemplate: element => new NgTemplate(element, this._compilerByTemplate.bind(this)),
                watchMethod: (fn, callback, options) => {
                    options = options || {};
                    var immediateValue;
                    var skipCallback = options.skipImmediate;
                    var disposeWatcher = scope.$watch(() => {
                        var value = fn();
                        if (value instanceof Date) {
                            value = value.valueOf()
                        }
                        return value
                    }, newValue => {
                        var isSameValue = immediateValue === newValue;
                        if (!skipCallback && (!isSameValue || isSameValue && options.deep)) {
                            callback(newValue)
                        }
                        skipCallback = false
                    }, options.deep);
                    if (!skipCallback) {
                        immediateValue = fn();
                        callback(immediateValue)
                    }
                    if (Config().wrapActionsBeforeExecute) {
                        this._applyAsync(() => {}, scope)
                    }
                    return disposeWatcher
                },
                templates: {
                    "dx-polymorph-widget": {
                        render: options => {
                            var widgetName = options.model.widget;
                            if (!widgetName) {
                                return
                            }
                            var markup = $("<div>").attr(dasherize(widgetName), "options").get(0);
                            var newScope = this._scope.$new();
                            newScope.options = options.model.options;
                            options.container.append(markup);
                            this._compile(markup)(newScope)
                        }
                    }
                }
            };
            result.modelByElement = () => scope;
            return result
        }
    });
    ComponentBuilder = ComponentBuilder.inherit({
        ctor(options) {
            this._componentName = options.componentName;
            this._ngModel = options.ngModel;
            this._ngModelController = options.ngModelController;
            this.callBase(...arguments)
        },
        _isNgModelRequired() {
            return Editor.isEditor(this._componentClass.prototype) && this._ngModel
        },
        _initComponentBindings() {
            this.callBase(...arguments);
            this._initNgModelBinding()
        },
        _initNgModelBinding() {
            if (!this._isNgModelRequired()) {
                return
            }
            var clearNgModelWatcher = this._scope.$watch(this._ngModel, (newValue, oldValue) => {
                if (this._ngLocker.locked(NG_MODEL_OPTION)) {
                    return
                }
                if (newValue === oldValue) {
                    return
                }
                this._component.option(NG_MODEL_OPTION, newValue)
            });
            this._optionChangedCallbacks.add(args => {
                this._ngLocker.obtain(NG_MODEL_OPTION);
                try {
                    if (args.name !== NG_MODEL_OPTION) {
                        return
                    }
                    this._ngModelController.$setViewValue(args.value)
                } finally {
                    if (this._ngLocker.locked(NG_MODEL_OPTION)) {
                        this._ngLocker.release(NG_MODEL_OPTION)
                    }
                }
            });
            this._componentDisposing.add(clearNgModelWatcher)
        },
        _evalOptions() {
            if (!this._isNgModelRequired()) {
                return this.callBase(...arguments)
            }
            var result = this.callBase(...arguments);
            result[NG_MODEL_OPTION] = this._parse(this._ngModel)(this._scope);
            return result
        }
    });
    var registeredComponents = {};
    var registerComponentDirective = name => {
        var priority = "dxValidator" !== name ? 1 : 10;
        ngModule.directive(name, ["$compile", "$parse", "dxDigestCallbacks", ($compile, $parse, dxDigestCallbacks) => ({
            restrict: "A",
            require: "^?ngModel",
            priority: priority,
            compile($element) {
                var componentClass = registeredComponents[name];
                var useTemplates = componentClass.prototype._useTemplates ? componentClass.prototype._useTemplates() : getClassMethod(componentClass, "_useTemplates")();
                var $content = useTemplates ? $element.contents().detach() : null;
                return (scope, $element, attrs, ngModelController, transcludeFn) => {
                    $element.append($content);
                    safeApply(() => {
                        new ComponentBuilder({
                            componentClass: componentClass,
                            componentName: name,
                            compile: $compile,
                            parse: $parse,
                            $element: $element,
                            scope: scope,
                            ngOptionsString: attrs[name],
                            ngOptions: attrs[name] ? scope.$eval(attrs[name]) : {},
                            ngModel: attrs.ngModel,
                            ngModelController: ngModelController,
                            transcludeFn: transcludeFn,
                            itemAlias: attrs[ITEM_ALIAS_ATTRIBUTE_NAME],
                            dxDigestCallbacks: dxDigestCallbacks
                        })
                    }, scope)
                }
            }
        })])
    };
    registerComponentCallbacks.add((name, componentClass) => {
        if (!registeredComponents[name]) {
            registerComponentDirective(name)
        }
        registeredComponents[name] = componentClass
    })
}
