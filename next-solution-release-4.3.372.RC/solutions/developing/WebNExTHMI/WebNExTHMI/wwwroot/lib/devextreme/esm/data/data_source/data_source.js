/**
 * DevExtreme (esm/data/data_source/data_source.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Class from "../../core/class";
import {
    extend
} from "../../core/utils/extend";
import {
    executeAsync
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import {
    isString,
    isNumeric,
    isBoolean,
    isDefined,
    isPlainObject
} from "../../core/utils/type";
import {
    throttleChanges
} from "../utils";
import {
    applyBatch
} from "../array_utils";
import CustomStore from "../custom_store";
import {
    EventsStrategy
} from "../../core/events_strategy";
import {
    errors
} from "../errors";
import {
    isEmpty
} from "../../core/utils/array";
import {
    create
} from "../../core/utils/queue";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
import OperationManager from "./operation_manager";
import {
    normalizeDataSourceOptions,
    normalizeStoreLoadOptionAccessorArguments,
    CANCELED_TOKEN,
    isPending,
    normalizeLoadResult,
    mapDataRespectingGrouping
} from "./utils";
export var DataSource = Class.inherit({
    ctor(options) {
        var _options$reshapeOnPus;
        options = normalizeDataSourceOptions(options);
        this._eventsStrategy = new EventsStrategy(this, {
            syncStrategy: true
        });
        this._store = options.store;
        this._changedTime = 0;
        var needThrottling = 0 !== options.pushAggregationTimeout;
        if (needThrottling) {
            var throttlingTimeout = void 0 === options.pushAggregationTimeout ? () => 5 * this._changedTime : options.pushAggregationTimeout;
            var pushDeferred;
            var lastPushWaiters;
            var throttlingPushHandler = throttleChanges(changes => {
                pushDeferred.resolve();
                var storePushPending = when(...lastPushWaiters);
                storePushPending.done(() => this._onPush(changes));
                lastPushWaiters = void 0;
                pushDeferred = void 0
            }, throttlingTimeout);
            this._onPushHandler = args => {
                this._aggregationTimeoutId = throttlingPushHandler(args.changes);
                if (!pushDeferred) {
                    pushDeferred = new Deferred
                }
                lastPushWaiters = args.waitFor;
                args.waitFor.push(pushDeferred.promise())
            };
            this._store.on("beforePush", this._onPushHandler)
        } else {
            this._onPushHandler = changes => this._onPush(changes);
            this._store.on("push", this._onPushHandler)
        }
        this._storeLoadOptions = this._extractLoadOptions(options);
        this._mapFunc = options.map;
        this._postProcessFunc = options.postProcess;
        this._pageIndex = void 0 !== options.pageIndex ? options.pageIndex : 0;
        this._pageSize = void 0 !== options.pageSize ? options.pageSize : 20;
        this._loadingCount = 0;
        this._loadQueue = this._createLoadQueue();
        this._searchValue = "searchValue" in options ? options.searchValue : null;
        this._searchOperation = options.searchOperation || "contains";
        this._searchExpr = options.searchExpr;
        this._paginate = options.paginate;
        this._reshapeOnPush = null !== (_options$reshapeOnPus = options.reshapeOnPush) && void 0 !== _options$reshapeOnPus ? _options$reshapeOnPus : false;
        each(["onChanged", "onLoadError", "onLoadingChanged", "onCustomizeLoadResult", "onCustomizeStoreLoadOptions"], (_, optionName) => {
            if (optionName in options) {
                this.on(optionName.substr(2, 1).toLowerCase() + optionName.substr(3), options[optionName])
            }
        });
        this._operationManager = new OperationManager;
        this._init()
    },
    _init() {
        this._items = [];
        this._userData = {};
        this._totalCount = -1;
        this._isLoaded = false;
        if (!isDefined(this._paginate)) {
            this._paginate = !this.group()
        }
        this._isLastPage = !this._paginate
    },
    dispose() {
        var _this$_delayedLoadTas;
        this._store.off("beforePush", this._onPushHandler);
        this._store.off("push", this._onPushHandler);
        this._eventsStrategy.dispose();
        clearTimeout(this._aggregationTimeoutId);
        null === (_this$_delayedLoadTas = this._delayedLoadTask) || void 0 === _this$_delayedLoadTas ? void 0 : _this$_delayedLoadTas.abort();
        this._operationManager.cancelAll();
        delete this._store;
        delete this._items;
        delete this._delayedLoadTask;
        this._disposed = true
    },
    _extractLoadOptions(options) {
        var result = {};
        var names = ["sort", "filter", "select", "group", "requireTotalCount"];
        var customNames = this._store._customLoadOptions();
        if (customNames) {
            names = names.concat(customNames)
        }
        each(names, (function() {
            result[this] = options[this]
        }));
        return result
    },
    loadOptions() {
        return this._storeLoadOptions
    },
    items() {
        return this._items
    },
    pageIndex(newIndex) {
        if (!isNumeric(newIndex)) {
            return this._pageIndex
        }
        this._pageIndex = newIndex;
        this._isLastPage = !this._paginate
    },
    paginate(value) {
        if (!isBoolean(value)) {
            return this._paginate
        }
        if (this._paginate !== value) {
            this._paginate = value;
            this.pageIndex(0)
        }
    },
    pageSize(value) {
        if (!isNumeric(value)) {
            return this._pageSize
        }
        this._pageSize = value
    },
    isLastPage() {
        return this._isLastPage
    },
    generateStoreLoadOptionAccessor(optionName) {
        return args => {
            var normalizedArgs = normalizeStoreLoadOptionAccessorArguments(args);
            if (void 0 === normalizedArgs) {
                return this._storeLoadOptions[optionName]
            }
            this._storeLoadOptions[optionName] = normalizedArgs
        }
    },
    sort() {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key]
        }
        return this.generateStoreLoadOptionAccessor("sort")(args)
    },
    filter() {
        var newFilter = normalizeStoreLoadOptionAccessorArguments(arguments);
        if (void 0 === newFilter) {
            return this._storeLoadOptions.filter
        }
        this._storeLoadOptions.filter = newFilter;
        this.pageIndex(0)
    },
    group() {
        for (var _len2 = arguments.length, args = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
            args[_key2] = arguments[_key2]
        }
        return this.generateStoreLoadOptionAccessor("group")(args)
    },
    select() {
        for (var _len3 = arguments.length, args = new Array(_len3), _key3 = 0; _key3 < _len3; _key3++) {
            args[_key3] = arguments[_key3]
        }
        return this.generateStoreLoadOptionAccessor("select")(args)
    },
    requireTotalCount(value) {
        if (!isBoolean(value)) {
            return this._storeLoadOptions.requireTotalCount
        }
        this._storeLoadOptions.requireTotalCount = value
    },
    searchValue(value) {
        if (arguments.length < 1) {
            return this._searchValue
        }
        this._searchValue = value;
        this.pageIndex(0)
    },
    searchOperation(op) {
        if (!isString(op)) {
            return this._searchOperation
        }
        this._searchOperation = op;
        this.pageIndex(0)
    },
    searchExpr(expr) {
        var argc = arguments.length;
        if (0 === argc) {
            return this._searchExpr
        }
        if (argc > 1) {
            expr = [].slice.call(arguments)
        }
        this._searchExpr = expr;
        this.pageIndex(0)
    },
    store() {
        return this._store
    },
    key() {
        var _this$_store;
        return null === (_this$_store = this._store) || void 0 === _this$_store ? void 0 : _this$_store.key()
    },
    totalCount() {
        return this._totalCount
    },
    isLoaded() {
        return this._isLoaded
    },
    isLoading() {
        return this._loadingCount > 0
    },
    beginLoading() {
        this._changeLoadingCount(1)
    },
    endLoading() {
        this._changeLoadingCount(-1)
    },
    _createLoadQueue: () => create(),
    _changeLoadingCount(increment) {
        var oldLoading = this.isLoading();
        this._loadingCount += increment;
        var newLoading = this.isLoading();
        if (oldLoading ^ newLoading) {
            this._eventsStrategy.fireEvent("loadingChanged", [newLoading])
        }
    },
    _scheduleLoadCallbacks(deferred) {
        this.beginLoading();
        deferred.always(() => {
            this.endLoading()
        })
    },
    _scheduleFailCallbacks(deferred) {
        var _this = this;
        deferred.fail((function() {
            for (var _len4 = arguments.length, args = new Array(_len4), _key4 = 0; _key4 < _len4; _key4++) {
                args[_key4] = arguments[_key4]
            }
            if (args[0] === CANCELED_TOKEN) {
                return
            }
            _this._eventsStrategy.fireEvent("loadError", args)
        }))
    },
    _fireChanged(args) {
        var date = new Date;
        this._eventsStrategy.fireEvent("changed", args);
        this._changedTime = new Date - date
    },
    _scheduleChangedCallbacks(deferred) {
        deferred.done(() => this._fireChanged())
    },
    loadSingle(propName, propValue) {
        var d = new Deferred;
        var key = this.key();
        var store = this._store;
        var options = this._createStoreLoadOptions();
        this._scheduleFailCallbacks(d);
        if (arguments.length < 2) {
            propValue = propName;
            propName = key
        }
        delete options.skip;
        delete options.group;
        delete options.refresh;
        delete options.pageIndex;
        delete options.searchString;
        (() => {
            if (propName === key || store instanceof CustomStore && !store._byKeyViaLoad()) {
                return store.byKey(propValue, options)
            }
            options.take = 1;
            options.filter = options.filter ? [options.filter, [propName, propValue]] : [propName, propValue];
            return store.load(options)
        })().fail(d.reject).done(data => {
            if (!isDefined(data) || isEmpty(data)) {
                d.reject(new errors.Error("E4009"))
            } else {
                if (!Array.isArray(data)) {
                    data = [data]
                }
                d.resolve(this._applyMapFunction(data)[0])
            }
        });
        return d.promise()
    },
    load() {
        var d = new Deferred;
        var loadTask = () => {
            if (this._disposed) {
                return
            }
            if (!isPending(d)) {
                return
            }
            return this._loadFromStore(loadOperation, d)
        };
        this._scheduleLoadCallbacks(d);
        this._scheduleFailCallbacks(d);
        this._scheduleChangedCallbacks(d);
        var loadOperation = this._createLoadOperation(d);
        this._eventsStrategy.fireEvent("customizeStoreLoadOptions", [loadOperation]);
        this._loadQueue.add(() => {
            if ("number" === typeof loadOperation.delay) {
                this._delayedLoadTask = executeAsync(loadTask, loadOperation.delay)
            } else {
                loadTask()
            }
            return d.promise()
        });
        return d.promise({
            operationId: loadOperation.operationId
        })
    },
    _onPush(changes) {
        if (this._reshapeOnPush) {
            this.load()
        } else {
            this._eventsStrategy.fireEvent("changing", [{
                changes: changes
            }]);
            var group = this.group();
            var items = this.items();
            var groupLevel = 0;
            var dataSourceChanges = this.paginate() || group ? changes.filter(item => "update" === item.type) : changes;
            if (group) {
                groupLevel = Array.isArray(group) ? group.length : 1
            }
            if (this._mapFunc) {
                dataSourceChanges.forEach(item => {
                    if ("insert" === item.type) {
                        item.data = this._mapFunc(item.data)
                    }
                })
            }
            applyBatch({
                keyInfo: this.store(),
                data: items,
                changes: dataSourceChanges,
                groupCount: groupLevel,
                useInsertIndex: true
            });
            this._fireChanged([{
                changes: changes
            }])
        }
    },
    _createLoadOperation(deferred) {
        var operationId = this._operationManager.add(deferred);
        var storeLoadOptions = this._createStoreLoadOptions();
        deferred.always(() => this._operationManager.remove(operationId));
        return {
            operationId: operationId,
            storeLoadOptions: storeLoadOptions
        }
    },
    reload() {
        var store = this.store();
        if (store instanceof CustomStore) {
            store.clearRawDataCache()
        }
        this._init();
        return this.load()
    },
    cancel(operationId) {
        return this._operationManager.cancel(operationId)
    },
    cancelAll() {
        return this._operationManager.cancelAll()
    },
    _addSearchOptions(storeLoadOptions) {
        if (this._disposed) {
            return
        }
        if (this.store()._useDefaultSearch) {
            this._addSearchFilter(storeLoadOptions)
        } else {
            storeLoadOptions.searchOperation = this._searchOperation;
            storeLoadOptions.searchValue = this._searchValue;
            storeLoadOptions.searchExpr = this._searchExpr
        }
    },
    _createStoreLoadOptions() {
        var result = extend({}, this._storeLoadOptions);
        this._addSearchOptions(result);
        if (this._paginate) {
            if (this._pageSize) {
                result.skip = this._pageIndex * this._pageSize;
                result.take = this._pageSize
            }
        }
        result.userData = this._userData;
        return result
    },
    _addSearchFilter(storeLoadOptions) {
        var value = this._searchValue;
        var op = this._searchOperation;
        var selector = this._searchExpr;
        var searchFilter = [];
        if (!value) {
            return
        }
        if (!selector) {
            selector = "this"
        }
        if (!Array.isArray(selector)) {
            selector = [selector]
        }
        each(selector, (function(i, item) {
            if (searchFilter.length) {
                searchFilter.push("or")
            }
            searchFilter.push([item, op, value])
        }));
        if (storeLoadOptions.filter) {
            storeLoadOptions.filter = [searchFilter, storeLoadOptions.filter]
        } else {
            storeLoadOptions.filter = searchFilter
        }
    },
    _loadFromStore(loadOptions, pendingDeferred) {
        var handleSuccess = (data, extra) => {
            if (this._disposed) {
                return
            }
            if (!isPending(pendingDeferred)) {
                return
            }
            var loadResult = extend(normalizeLoadResult(data, extra), loadOptions);
            this._eventsStrategy.fireEvent("customizeLoadResult", [loadResult]);
            when(loadResult.data).done(data => {
                loadResult.data = data;
                this._processStoreLoadResult(loadResult, pendingDeferred)
            }).fail(pendingDeferred.reject)
        };
        if (loadOptions.data) {
            return (new Deferred).resolve(loadOptions.data).done(handleSuccess)
        }
        return this.store().load(loadOptions.storeLoadOptions).done(handleSuccess).fail(pendingDeferred.reject)
    },
    _processStoreLoadResult(loadResult, pendingDeferred) {
        var data = loadResult.data;
        var extra = loadResult.extra;
        var storeLoadOptions = loadResult.storeLoadOptions;
        var resolvePendingDeferred = () => {
            this._isLoaded = true;
            this._totalCount = isFinite(extra.totalCount) ? extra.totalCount : -1;
            return pendingDeferred.resolve(data, extra)
        };
        if (this._disposed) {
            return
        }
        data = this._applyPostProcessFunction(this._applyMapFunction(data));
        if (!isPlainObject(extra)) {
            extra = {}
        }
        this._items = data;
        if (!data.length || !this._paginate || this._pageSize && data.length < this._pageSize) {
            this._isLastPage = true
        }
        if (storeLoadOptions.requireTotalCount && !isFinite(extra.totalCount)) {
            (() => {
                this.store().totalCount(storeLoadOptions).done((function(count) {
                    extra.totalCount = count;
                    resolvePendingDeferred()
                })).fail(pendingDeferred.reject)
            })()
        } else {
            resolvePendingDeferred()
        }
    },
    _applyMapFunction(data) {
        if (this._mapFunc) {
            return mapDataRespectingGrouping(data, this._mapFunc, this.group())
        }
        return data
    },
    _applyPostProcessFunction(data) {
        if (this._postProcessFunc) {
            return this._postProcessFunc(data)
        }
        return data
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
