/**
 * DevExtreme (bundles/modules/data.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var DevExpress = require("./core");
var errors = require("../../core/errors");
module.exports = DevExpress.data = DevExpress.data || {};
Object.defineProperty(DevExpress.data, "errorHandler", {
    get: function() {
        return require("../../data/errors").errorHandler
    },
    set: function(value) {
        errors.log("W0003", "DevExpress.data", "errorHandler", "21.1", "Use the 'setErrorHandler' method instead");
        require("../../data/errors").setErrorHandler(value)
    }
});
Object.defineProperty(DevExpress.data, "_errorHandler", {
    get: function() {
        return require("../../data/errors").handleError
    },
    set: function(value) {
        errors.log("W0003", "DevExpress.data", "_errorHandler", "21.1", "Use the 'setErrorHandler' method instead");
        require("../../data/errors").setErrorHandler(value)
    }
});
DevExpress.data.setErrorHandler = require("../../data/errors").setErrorHandler;
DevExpress.data.DataSource = require("../../data/data_source");
DevExpress.data.query = require("../../data/query");
DevExpress.data.Store = require("../../data/abstract_store");
DevExpress.data.ArrayStore = require("../../data/array_store");
DevExpress.data.CustomStore = require("../../data/custom_store");
DevExpress.data.LocalStore = require("../../data/local_store");
DevExpress.data.base64_encode = require("../../data/utils").base64_encode;
DevExpress.data.applyChanges = require("../../data/apply_changes");
DevExpress.data.Guid = require("../../core/guid");
DevExpress.data.utils = {};
DevExpress.data.utils.compileGetter = require("../../core/utils/data").compileGetter;
DevExpress.data.utils.compileSetter = require("../../core/utils/data").compileSetter;
DevExpress.EndpointSelector = require("../../data/endpoint_selector");
DevExpress.data.queryImpl = require("../../data/query_implementation").queryImpl;
DevExpress.data.queryAdapters = require("../../data/query_adapters");
var dataUtils = require("../../data/utils");
DevExpress.data.utils.normalizeBinaryCriterion = dataUtils.normalizeBinaryCriterion;
DevExpress.data.utils.normalizeSortingInfo = dataUtils.normalizeSortingInfo;
DevExpress.data.utils.errorMessageFromXhr = dataUtils.errorMessageFromXhr;
DevExpress.data.utils.aggregators = dataUtils.aggregators;
DevExpress.data.utils.keysEqual = dataUtils.keysEqual;
DevExpress.data.utils.isDisjunctiveOperator = dataUtils.isDisjunctiveOperator;
DevExpress.data.utils.isConjunctiveOperator = dataUtils.isConjunctiveOperator;
DevExpress.data.utils.processRequestResultLock = dataUtils.processRequestResultLock;
DevExpress.data.utils.toComparable = require("../../core/utils/data").toComparable;
DevExpress.data.utils.multiLevelGroup = require("../../data/store_helper").multiLevelGroup;
DevExpress.data.utils.arrangeSortingInfo = require("../../data/store_helper").arrangeSortingInfo;
DevExpress.data.utils.normalizeDataSourceOptions = require("../../data/data_source/utils").normalizeDataSourceOptions;
