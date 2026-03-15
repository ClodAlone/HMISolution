/**
 * DevExtreme (cjs/events/core/events_engine.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _event_registrator_callbacks = _interopRequireDefault(require("./event_registrator_callbacks"));
var _extend = require("../../core/utils/extend");
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _window = require("../../core/utils/window");
var _dependency_injector = _interopRequireDefault(require("../../core/utils/dependency_injector"));
var _type = require("../../core/utils/type");
var _callbacks = _interopRequireDefault(require("../../core/utils/callbacks"));
var _errors = _interopRequireDefault(require("../../core/errors"));
var _weak_map = _interopRequireDefault(require("../../core/polyfills/weak_map"));
var _hook_touch_props = _interopRequireDefault(require("../../events/core/hook_touch_props"));
var _call_once = _interopRequireDefault(require("../../core/utils/call_once"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var window = (0, _window.getWindow)();
var EMPTY_EVENT_NAME = "dxEmptyEventType";
var NATIVE_EVENTS_TO_SUBSCRIBE = {
    mouseenter: "mouseover",
    mouseleave: "mouseout",
    pointerenter: "pointerover",
    pointerleave: "pointerout"
};
var NATIVE_EVENTS_TO_TRIGGER = {
    focusin: "focus",
    focusout: "blur"
};
var NO_BUBBLE_EVENTS = ["blur", "focus", "load"];
var forcePassiveFalseEventNames = ["touchmove", "wheel", "mousewheel", "touchstart"];
var EVENT_PROPERTIES = ["target", "relatedTarget", "delegateTarget", "altKey", "bubbles", "cancelable", "changedTouches", "ctrlKey", "detail", "eventPhase", "metaKey", "shiftKey", "view", "char", "code", "charCode", "key", "keyCode", "button", "buttons", "offsetX", "offsetY", "pointerId", "pointerType", "targetTouches", "toElement", "touches"];

function matchesSafe(target, selector) {
    return !(0, _type.isWindow)(target) && "#document" !== target.nodeName && _dom_adapter.default.elementMatches(target, selector)
}
var elementDataMap = new _weak_map.default;
var guid = 0;
var skipEvent;
var special = function() {
    var specialData = {};
    _event_registrator_callbacks.default.add((function(eventName, eventObject) {
        specialData[eventName] = eventObject
    }));
    return {
        getField: function(eventName, field) {
            return specialData[eventName] && specialData[eventName][field]
        },
        callMethod: function(eventName, methodName, context, args) {
            return specialData[eventName] && specialData[eventName][methodName] && specialData[eventName][methodName].apply(context, args)
        }
    }
}();
var eventsEngine = (0, _dependency_injector.default)({
    on: getHandler(normalizeOnArguments(iterate((function(element, eventName, selector, data, handler) {
        var handlersController = getHandlersController(element, eventName);
        handlersController.addHandler(handler, selector, data)
    })))),
    one: getHandler(normalizeOnArguments((function(element, eventName, selector, data, handler) {
        eventsEngine.on(element, eventName, selector, data, (function oneTimeHandler() {
            eventsEngine.off(element, eventName, selector, oneTimeHandler);
            handler.apply(this, arguments)
        }))
    }))),
    off: getHandler(normalizeOffArguments(iterate((function(element, eventName, selector, handler) {
        var handlersController = getHandlersController(element, eventName);
        handlersController.removeHandler(handler, selector)
    })))),
    trigger: getHandler(normalizeTriggerArguments((function(element, event, extraParameters) {
        var eventName = event.type;
        var handlersController = getHandlersController(element, event.type);
        special.callMethod(eventName, "trigger", element, [event, extraParameters]);
        handlersController.callHandlers(event, extraParameters);
        var noBubble = special.getField(eventName, "noBubble") || event.isPropagationStopped() || -1 !== NO_BUBBLE_EVENTS.indexOf(eventName);
        if (!noBubble) {
            var parents = [];
            ! function getParents(element) {
                var parent = element.parentNode;
                if (parent) {
                    parents.push(parent);
                    getParents(parent)
                }
            }(element);
            parents.push(window);
            var i = 0;
            while (parents[i] && !event.isPropagationStopped()) {
                var parentDataByEvent = getHandlersController(parents[i], event.type);
                parentDataByEvent.callHandlers((0, _extend.extend)(event, {
                    currentTarget: parents[i]
                }), extraParameters);
                i++
            }
        }
        if (element.nodeType || (0, _type.isWindow)(element)) {
            special.callMethod(eventName, "_default", element, [event, extraParameters]);
            callNativeMethod(eventName, element)
        }
    }))),
    triggerHandler: getHandler(normalizeTriggerArguments((function(element, event, extraParameters) {
        var handlersController = getHandlersController(element, event.type);
        handlersController.callHandlers(event, extraParameters)
    })))
});

function applyForEach(args, method) {
    var element = args[0];
    if (!element) {
        return
    }
    if (_dom_adapter.default.isNode(element) || (0, _type.isWindow)(element)) {
        method.apply(eventsEngine, args)
    } else if (!(0, _type.isString)(element) && "length" in element) {
        var itemArgs = Array.prototype.slice.call(args, 0);
        Array.prototype.forEach.call(element, (function(itemElement) {
            itemArgs[0] = itemElement;
            applyForEach(itemArgs, method)
        }))
    } else {
        throw _errors.default.Error("E0025")
    }
}

function getHandler(method) {
    return function() {
        applyForEach(arguments, method)
    }
}

function detectPassiveEventHandlersSupport() {
    var isSupported = false;
    try {
        var options = Object.defineProperty({}, "passive", {
            get: function() {
                isSupported = true;
                return true
            }
        });
        window.addEventListener("test", null, options)
    } catch (e) {}
    return isSupported
}
var passiveEventHandlersSupported = (0, _call_once.default)(detectPassiveEventHandlersSupport);
var contains = function contains(container, element) {
    if ((0, _type.isWindow)(container)) {
        return contains(container.document, element)
    }
    return container.contains ? container.contains(element) : !!(element.compareDocumentPosition(container) & element.DOCUMENT_POSITION_CONTAINS)
};

function getHandlersController(element, eventName) {
    var elementData = elementDataMap.get(element);
    eventName = eventName || "";
    var eventNameParts = eventName.split(".");
    var namespaces = eventNameParts.slice(1);
    var eventNameIsDefined = !!eventNameParts[0];
    eventName = eventNameParts[0] || EMPTY_EVENT_NAME;
    if (!elementData) {
        elementData = {};
        elementDataMap.set(element, elementData)
    }
    if (!elementData[eventName]) {
        elementData[eventName] = {
            handleObjects: [],
            nativeHandler: null
        }
    }
    var eventData = elementData[eventName];
    return {
        addHandler: function(handler, selector, data) {
            var callHandler = function(e, extraParameters) {
                var handlerArgs = [e];
                var target = e.currentTarget;
                var relatedTarget = e.relatedTarget;
                var secondaryTargetIsInside;
                var result;
                if (eventName in NATIVE_EVENTS_TO_SUBSCRIBE) {
                    secondaryTargetIsInside = relatedTarget && target && (relatedTarget === target || contains(target, relatedTarget))
                }
                if (void 0 !== extraParameters) {
                    handlerArgs.push(extraParameters)
                }
                special.callMethod(eventName, "handle", element, [e, data]);
                if (!secondaryTargetIsInside) {
                    result = handler.apply(target, handlerArgs)
                }
                if (false === result) {
                    e.preventDefault();
                    e.stopPropagation()
                }
            };
            var handleObject = {
                handler: handler,
                wrappedHandler: function(e, extraParameters) {
                    if (skipEvent && e.type === skipEvent) {
                        return
                    }
                    e.data = data;
                    e.delegateTarget = element;
                    if (selector) {
                        var currentTarget = e.target;
                        while (currentTarget && currentTarget !== element) {
                            if (matchesSafe(currentTarget, selector)) {
                                e.currentTarget = currentTarget;
                                callHandler(e, extraParameters)
                            }
                            currentTarget = currentTarget.parentNode
                        }
                    } else {
                        e.currentTarget = e.delegateTarget || e.target;
                        callHandler(e, extraParameters)
                    }
                },
                selector: selector,
                type: eventName,
                data: data,
                namespace: namespaces.join("."),
                namespaces: namespaces,
                guid: ++guid
            };
            eventData.handleObjects.push(handleObject);
            var firstHandlerForTheType = 1 === eventData.handleObjects.length;
            var shouldAddNativeListener = firstHandlerForTheType && eventNameIsDefined;
            var nativeListenerOptions;
            if (shouldAddNativeListener) {
                shouldAddNativeListener = !special.callMethod(eventName, "setup", element, [data, namespaces, handler])
            }
            if (shouldAddNativeListener) {
                eventData.nativeHandler = getNativeHandler(eventName);
                if (passiveEventHandlersSupported() && forcePassiveFalseEventNames.indexOf(eventName) > -1) {
                    nativeListenerOptions = {
                        passive: false
                    }
                }
                eventData.removeListener = _dom_adapter.default.listen(element, NATIVE_EVENTS_TO_SUBSCRIBE[eventName] || eventName, eventData.nativeHandler, nativeListenerOptions)
            }
            special.callMethod(eventName, "add", element, [handleObject])
        },
        removeHandler: function(handler, selector) {
            var removeByEventName = function(eventName) {
                var eventData = elementData[eventName];
                if (!eventData.handleObjects.length) {
                    delete elementData[eventName];
                    return
                }
                var removedHandler;
                eventData.handleObjects = eventData.handleObjects.filter((function(handleObject) {
                    var skip = namespaces.length && !isSubset(handleObject.namespaces, namespaces) || handler && handleObject.handler !== handler || selector && handleObject.selector !== selector;
                    if (!skip) {
                        removedHandler = handleObject.handler;
                        special.callMethod(eventName, "remove", element, [handleObject])
                    }
                    return skip
                }));
                var lastHandlerForTheType = !eventData.handleObjects.length;
                var shouldRemoveNativeListener = lastHandlerForTheType && eventName !== EMPTY_EVENT_NAME;
                if (shouldRemoveNativeListener) {
                    special.callMethod(eventName, "teardown", element, [namespaces, removedHandler]);
                    if (eventData.nativeHandler) {
                        eventData.removeListener()
                    }
                    delete elementData[eventName]
                }
            };
            if (eventNameIsDefined) {
                removeByEventName(eventName)
            } else {
                for (var name in elementData) {
                    removeByEventName(name)
                }
            }
            var elementDataIsEmpty = 0 === Object.keys(elementData).length;
            if (elementDataIsEmpty) {
                elementDataMap.delete(element)
            }
        },
        callHandlers: function(event, extraParameters) {
            var forceStop = false;
            var handleCallback = function(handleObject) {
                if (forceStop) {
                    return
                }
                if (!namespaces.length || isSubset(handleObject.namespaces, namespaces)) {
                    handleObject.wrappedHandler(event, extraParameters);
                    forceStop = event.isImmediatePropagationStopped()
                }
            };
            eventData.handleObjects.forEach(handleCallback);
            if (namespaces.length && elementData[EMPTY_EVENT_NAME]) {
                elementData[EMPTY_EVENT_NAME].handleObjects.forEach(handleCallback)
            }
        }
    }
}

function getNativeHandler(subscribeName) {
    return function(event, extraParameters) {
        var handlersController = getHandlersController(this, subscribeName);
        event = eventsEngine.Event(event);
        handlersController.callHandlers(event, extraParameters)
    }
}

function isSubset(original, checked) {
    for (var i = 0; i < checked.length; i++) {
        if (original.indexOf(checked[i]) < 0) {
            return false
        }
    }
    return true
}

function normalizeOnArguments(callback) {
    return function(element, eventName, selector, data, handler) {
        if (!handler) {
            handler = data;
            data = void 0
        }
        if ("string" !== typeof selector) {
            data = selector;
            selector = void 0
        }
        if (!handler && "string" === typeof eventName) {
            handler = data || selector;
            selector = void 0;
            data = void 0
        }
        callback(element, eventName, selector, data, handler)
    }
}

function normalizeOffArguments(callback) {
    return function(element, eventName, selector, handler) {
        if ("function" === typeof selector) {
            handler = selector;
            selector = void 0
        }
        callback(element, eventName, selector, handler)
    }
}

function normalizeTriggerArguments(callback) {
    return function(element, src, extraParameters) {
        if ("string" === typeof src) {
            src = {
                type: src
            }
        }
        if (!src.target) {
            src.target = element
        }
        src.currentTarget = element;
        if (!src.delegateTarget) {
            src.delegateTarget = element
        }
        if (!src.type && src.originalEvent) {
            src.type = src.originalEvent.type
        }
        callback(element, src instanceof eventsEngine.Event ? src : eventsEngine.Event(src), extraParameters)
    }
}

function normalizeEventArguments(callback) {
    eventsEngine.Event = function(src, config) {
        if (!(this instanceof eventsEngine.Event)) {
            return new eventsEngine.Event(src, config)
        }
        if (!src) {
            src = {}
        }
        if ("string" === typeof src) {
            src = {
                type: src
            }
        }
        if (!config) {
            config = {}
        }
        callback.call(this, src, config)
    };
    _extends(eventsEngine.Event.prototype, {
        _propagationStopped: false,
        _immediatePropagationStopped: false,
        _defaultPrevented: false,
        isPropagationStopped: function() {
            return !!(this._propagationStopped || this.originalEvent && this.originalEvent.propagationStopped)
        },
        stopPropagation: function() {
            this._propagationStopped = true;
            this.originalEvent && this.originalEvent.stopPropagation()
        },
        isImmediatePropagationStopped: function() {
            return this._immediatePropagationStopped
        },
        stopImmediatePropagation: function() {
            this.stopPropagation();
            this._immediatePropagationStopped = true;
            this.originalEvent && this.originalEvent.stopImmediatePropagation()
        },
        isDefaultPrevented: function() {
            return !!(this._defaultPrevented || this.originalEvent && this.originalEvent.defaultPrevented)
        },
        preventDefault: function() {
            this._defaultPrevented = true;
            this.originalEvent && this.originalEvent.preventDefault()
        }
    });
    return eventsEngine.Event
}

function iterate(callback) {
    var iterateEventNames = function(element, eventName) {
        if (eventName && eventName.indexOf(" ") > -1) {
            var args = Array.prototype.slice.call(arguments, 0);
            eventName.split(" ").forEach((function(eventName) {
                args[1] = eventName;
                callback.apply(this, args)
            }))
        } else {
            callback.apply(this, arguments)
        }
    };
    return function(element, eventName) {
        if ("object" === _typeof(eventName)) {
            var args = Array.prototype.slice.call(arguments, 0);
            for (var name in eventName) {
                args[1] = name;
                args[args.length - 1] = eventName[name];
                iterateEventNames.apply(this, args)
            }
        } else {
            iterateEventNames.apply(this, arguments)
        }
    }
}

function callNativeMethod(eventName, element) {
    var nativeMethodName = NATIVE_EVENTS_TO_TRIGGER[eventName] || eventName;
    if (function(eventName, element) {
            return "click" === eventName && "a" === element.localName
        }(eventName, element)) {
        return
    }
    if ((0, _type.isFunction)(element[nativeMethodName])) {
        skipEvent = eventName;
        element[nativeMethodName]();
        skipEvent = void 0
    }
}

function calculateWhich(event) {
    if (function(event) {
            return null == event.which && 0 === event.type.indexOf("key")
        }(event)) {
        return null != event.charCode ? event.charCode : event.keyCode
    }
    if (function(event) {
            return !event.which && void 0 !== event.button && /^(?:mouse|pointer|contextmenu|drag|drop)|click/.test(event.type)
        }(event)) {
        return {
            1: 1,
            2: 3,
            3: 1,
            4: 2
        } [event.button]
    }
    return event.which
}

function initEvent(EventClass) {
    if (EventClass) {
        eventsEngine.Event = EventClass;
        eventsEngine.Event.prototype = EventClass.prototype
    }
}
initEvent(normalizeEventArguments((function(src, config) {
    var _src$view;
    var srcIsEvent = src instanceof eventsEngine.Event || (0, _window.hasWindow)() && src instanceof window.Event || (null === (_src$view = src.view) || void 0 === _src$view ? void 0 : _src$view.Event) && src instanceof src.view.Event;
    if (srcIsEvent) {
        this.originalEvent = src;
        this.type = src.type;
        this.currentTarget = void 0;
        if (Object.prototype.hasOwnProperty.call(src, "isTrusted")) {
            this.isTrusted = src.isTrusted
        }
        this.timeStamp = src.timeStamp || Date.now()
    } else {
        _extends(this, src)
    }
    addProperty("which", calculateWhich, this);
    if (0 === src.type.indexOf("touch")) {
        delete config.pageX;
        delete config.pageY
    }
    _extends(this, config);
    this.guid = ++guid
})));

function addProperty(propName, hook, eventInstance) {
    Object.defineProperty(eventInstance || eventsEngine.Event.prototype, propName, {
        enumerable: true,
        configurable: true,
        get: function() {
            return this.originalEvent && hook(this.originalEvent)
        },
        set: function(value) {
            Object.defineProperty(this, propName, {
                enumerable: true,
                configurable: true,
                writable: true,
                value: value
            })
        }
    })
}
EVENT_PROPERTIES.forEach((function(prop) {
    return addProperty(prop, (function(event) {
        return event[prop]
    }))
}));
(0, _hook_touch_props.default)(addProperty);
var beforeSetStrategy = (0, _callbacks.default)();
var afterSetStrategy = (0, _callbacks.default)();
eventsEngine.set = function(engine) {
    beforeSetStrategy.fire();
    eventsEngine.inject(engine);
    initEvent(engine.Event);
    afterSetStrategy.fire()
};
eventsEngine.subscribeGlobal = function() {
    applyForEach(arguments, normalizeOnArguments((function() {
        var args = arguments;
        eventsEngine.on.apply(this, args);
        beforeSetStrategy.add((function() {
            var offArgs = Array.prototype.slice.call(args, 0);
            offArgs.splice(3, 1);
            eventsEngine.off.apply(this, offArgs)
        }));
        afterSetStrategy.add((function() {
            eventsEngine.on.apply(this, args)
        }))
    })))
};
eventsEngine.forcePassiveFalseEventNames = forcePassiveFalseEventNames;
eventsEngine.passiveEventHandlersSupported = passiveEventHandlersSupported;
var _default = eventsEngine;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
