/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.state_storing_core.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../../events/core/events_engine";
import {
    getWindow
} from "../../core/utils/window";
import modules from "./ui.grid_core.modules";
import errors from "../widget/ui.errors";
import {
    sessionStorage
} from "../../core/utils/storage";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import {
    isDefined,
    isPlainObject,
    isEmptyObject
} from "../../core/utils/type";
import {
    fromPromise
} from "../../core/utils/deferred";
var DATE_REGEX = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/;
var parseDates = function parseDates(state) {
    if (!state) {
        return
    }
    each(state, (function(key, value) {
        if (isPlainObject(value) || Array.isArray(value)) {
            parseDates(value)
        } else if ("string" === typeof value) {
            var date = DATE_REGEX.exec(value);
            if (date) {
                state[key] = new Date(Date.UTC(+date[1], +date[2] - 1, +date[3], +date[4], +date[5], +date[6]))
            }
        }
    }))
};
export var StateStoringController = modules.ViewController.inherit(function() {
    var getStorage = function(options) {
        var storage = "sessionStorage" === options.type ? sessionStorage() : getWindow().localStorage;
        if (!storage) {
            throw new Error("E1007")
        }
        return storage
    };
    var getUniqueStorageKey = function(options) {
        return isDefined(options.storageKey) ? options.storageKey : "storage"
    };
    return {
        _loadState: function() {
            var options = this.option("stateStoring");
            if ("custom" === options.type) {
                return options.customLoad && options.customLoad()
            }
            try {
                return JSON.parse(getStorage(options).getItem(getUniqueStorageKey(options)))
            } catch (e) {
                errors.log(e.message)
            }
        },
        _saveState: function(state) {
            var options = this.option("stateStoring");
            if ("custom" === options.type) {
                options.customSave && options.customSave(state);
                return
            }
            try {
                getStorage(options).setItem(getUniqueStorageKey(options), JSON.stringify(state))
            } catch (e) {
                errors.log(e.message)
            }
        },
        publicMethods: function() {
            return ["state"]
        },
        isEnabled: function() {
            return this.option("stateStoring.enabled")
        },
        init: function() {
            var that = this;
            that._state = {};
            that._isLoaded = false;
            that._isLoading = false;
            that._windowUnloadHandler = function() {
                if (void 0 !== that._savingTimeoutID) {
                    that._saveState(that.state())
                }
            };
            eventsEngine.on(getWindow(), "unload", that._windowUnloadHandler);
            return that
        },
        isLoaded: function() {
            return this._isLoaded
        },
        isLoading: function() {
            return this._isLoading
        },
        load: function() {
            this._isLoading = true;
            var loadResult = fromPromise(this._loadState());
            loadResult.always(() => {
                this._isLoaded = true;
                this._isLoading = false
            }).done(state => {
                if (null !== state && !isEmptyObject(state)) {
                    this.state(state)
                }
            });
            return loadResult
        },
        state: function(_state) {
            var that = this;
            if (!arguments.length) {
                return extend(true, {}, that._state)
            } else {
                that._state = extend({}, _state);
                parseDates(that._state)
            }
        },
        save: function() {
            var that = this;
            clearTimeout(that._savingTimeoutID);
            that._savingTimeoutID = setTimeout((function() {
                that._saveState(that.state());
                that._savingTimeoutID = void 0
            }), that.option("stateStoring.savingTimeout"))
        },
        optionChanged: function(args) {
            switch (args.name) {
                case "stateStoring":
                    if (this.isEnabled() && !this.isLoading()) {
                        this.load()
                    }
                    args.handled = true;
                    break;
                default:
                    this.callBase(args)
            }
        },
        dispose: function() {
            clearTimeout(this._savingTimeoutID);
            eventsEngine.off(getWindow(), "unload", this._windowUnloadHandler)
        }
    }
}());
