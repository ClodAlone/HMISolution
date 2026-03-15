/**
 * DevExtreme (esm/core/events_strategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Callbacks from "./utils/callbacks";
import {
    each
} from "./utils/iterator";
import {
    isFunction,
    isPlainObject
} from "./utils/type";
export class EventsStrategy {
    constructor(owner) {
        var options = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
        this._events = {};
        this._owner = owner;
        this._options = options
    }
    static create(owner, strategy) {
        if (strategy) {
            return isFunction(strategy) ? strategy(owner) : strategy
        } else {
            return new EventsStrategy(owner)
        }
    }
    hasEvent(eventName) {
        var callbacks = this._events[eventName];
        return callbacks ? callbacks.has() : false
    }
    fireEvent(eventName, eventArgs) {
        var callbacks = this._events[eventName];
        if (callbacks) {
            callbacks.fireWith(this._owner, eventArgs)
        }
        return this._owner
    }
    on(eventName, eventHandler) {
        if (isPlainObject(eventName)) {
            each(eventName, (e, h) => {
                this.on(e, h)
            })
        } else {
            var callbacks = this._events[eventName];
            if (!callbacks) {
                callbacks = Callbacks({
                    syncStrategy: this._options.syncStrategy
                });
                this._events[eventName] = callbacks
            }
            var addFn = callbacks.originalAdd || callbacks.add;
            addFn.call(callbacks, eventHandler)
        }
    }
    off(eventName, eventHandler) {
        var callbacks = this._events[eventName];
        if (callbacks) {
            if (isFunction(eventHandler)) {
                callbacks.remove(eventHandler)
            } else {
                callbacks.empty()
            }
        }
    }
    dispose() {
        each(this._events, (eventName, event) => {
            event.empty()
        })
    }
}
