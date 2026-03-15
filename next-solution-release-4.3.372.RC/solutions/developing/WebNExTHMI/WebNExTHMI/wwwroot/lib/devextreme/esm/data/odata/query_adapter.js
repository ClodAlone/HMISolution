/**
 * DevExtreme (esm/data/odata/query_adapter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isFunction
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import config from "../../core/config";
import {
    extend
} from "../../core/utils/extend";
import queryAdapters from "../query_adapters";
import {
    sendRequest,
    generateSelect,
    generateExpand,
    serializeValue,
    convertPrimitiveValue,
    serializePropName
} from "./utils";
import {
    errors
} from "../errors";
import {
    isConjunctiveOperator,
    normalizeBinaryCriterion,
    isUnaryOperation
} from "../utils";
var DEFAULT_PROTOCOL_VERSION = 2;
var STRING_FUNCTIONS = ["contains", "notcontains", "startswith", "endswith"];
var compileCriteria = (() => {
    var protocolVersion;
    var forceLowerCase;
    var fieldTypes;
    var createBinaryOperationFormatter = op => (prop, val) => "".concat(prop, " ").concat(op, " ").concat(val);
    var createStringFuncFormatter = (op, reverse) => (prop, val) => {
        var bag = [op, "("];
        if (forceLowerCase) {
            prop = -1 === prop.indexOf("tolower(") ? "tolower(".concat(prop, ")") : prop;
            val = val.toLowerCase()
        }
        if (reverse) {
            bag.push(val, ",", prop)
        } else {
            bag.push(prop, ",", val)
        }
        bag.push(")");
        return bag.join("")
    };
    var formatters = {
        "=": createBinaryOperationFormatter("eq"),
        "<>": createBinaryOperationFormatter("ne"),
        ">": createBinaryOperationFormatter("gt"),
        ">=": createBinaryOperationFormatter("ge"),
        "<": createBinaryOperationFormatter("lt"),
        "<=": createBinaryOperationFormatter("le"),
        startswith: createStringFuncFormatter("startswith"),
        endswith: createStringFuncFormatter("endswith")
    };
    var formattersV2 = extend({}, formatters, {
        contains: createStringFuncFormatter("substringof", true),
        notcontains: createStringFuncFormatter("not substringof", true)
    });
    var formattersV4 = extend({}, formatters, {
        contains: createStringFuncFormatter("contains"),
        notcontains: createStringFuncFormatter("not contains")
    });
    var compileBinary = criteria => {
        var _fieldTypes;
        criteria = normalizeBinaryCriterion(criteria);
        var op = criteria[1];
        var fieldName = criteria[0];
        var fieldType = fieldTypes && fieldTypes[fieldName];
        if (fieldType && (name = op, STRING_FUNCTIONS.some(funcName => funcName === name)) && "String" !== fieldType) {
            throw new errors.Error("E4024", op, fieldName, fieldType)
        }
        var name;
        var formatters = 4 === protocolVersion ? formattersV4 : formattersV2;
        var formatter = formatters[op.toLowerCase()];
        if (!formatter) {
            throw errors.Error("E4003", op)
        }
        var value = criteria[2];
        if (null !== (_fieldTypes = fieldTypes) && void 0 !== _fieldTypes && _fieldTypes[fieldName]) {
            value = convertPrimitiveValue(fieldTypes[fieldName], value)
        }
        return formatter(serializePropName(fieldName), serializeValue(value, protocolVersion))
    };
    var compileGroup = criteria => {
        var bag = [];
        var groupOperator;
        var nextGroupOperator;
        each(criteria, (function(index, criterion) {
            if (Array.isArray(criterion)) {
                if (bag.length > 1 && groupOperator !== nextGroupOperator) {
                    throw new errors.Error("E4019")
                }
                bag.push("(".concat(compileCore(criterion), ")"));
                groupOperator = nextGroupOperator;
                nextGroupOperator = "and"
            } else {
                nextGroupOperator = isConjunctiveOperator(this) ? "and" : "or"
            }
        }));
        return bag.join(" ".concat(groupOperator, " "))
    };
    var compileCore = criteria => {
        if (Array.isArray(criteria[0])) {
            return compileGroup(criteria)
        }
        if (isUnaryOperation(criteria)) {
            return (criteria => {
                var op = criteria[0];
                var crit = compileCore(criteria[1]);
                if ("!" === op) {
                    return "not (".concat(crit, ")")
                }
                throw errors.Error("E4003", op)
            })(criteria)
        }
        return compileBinary(criteria)
    };
    return (criteria, version, types, filterToLower) => {
        fieldTypes = types;
        forceLowerCase = null !== filterToLower && void 0 !== filterToLower ? filterToLower : config().oDataFilterToLower;
        protocolVersion = version;
        return compileCore(criteria)
    }
})();
var createODataQueryAdapter = queryOptions => {
    var _sorting = [];
    var _criteria = [];
    var _expand = queryOptions.expand;
    var _select;
    var _skip;
    var _take;
    var _countQuery;
    var _oDataVersion = queryOptions.version || DEFAULT_PROTOCOL_VERSION;
    var hasSlice = () => _skip || void 0 !== _take;
    var hasFunction = criterion => {
        for (var i = 0; i < criterion.length; i++) {
            if (isFunction(criterion[i])) {
                return true
            }
            if (Array.isArray(criterion[i]) && hasFunction(criterion[i])) {
                return true
            }
        }
        return false
    };
    var requestData = () => {
        var result = {};
        if (!_countQuery) {
            if (_sorting.length) {
                result.$orderby = _sorting.join(",")
            }
            if (_skip) {
                result.$skip = _skip
            }
            if (void 0 !== _take) {
                result.$top = _take
            }
            result.$select = generateSelect(_oDataVersion, _select) || void 0;
            result.$expand = generateExpand(_oDataVersion, _expand, _select) || void 0
        }
        if (_criteria.length) {
            var criteria = _criteria.length < 2 ? _criteria[0] : _criteria;
            var fieldTypes = null === queryOptions || void 0 === queryOptions ? void 0 : queryOptions.fieldTypes;
            var filterToLower = null === queryOptions || void 0 === queryOptions ? void 0 : queryOptions.filterToLower;
            result.$filter = compileCriteria(criteria, _oDataVersion, fieldTypes, filterToLower)
        }
        if (_countQuery) {
            result.$top = 0
        }
        if (queryOptions.requireTotalCount || _countQuery) {
            if (4 !== _oDataVersion) {
                result.$inlinecount = "allpages"
            } else {
                result.$count = "true"
            }
        }
        return result
    };
    return {
        optimize: tasks => {
            var selectIndex = -1;
            for (var i = 0; i < tasks.length; i++) {
                if ("select" === tasks[i].name) {
                    selectIndex = i;
                    break
                }
            }
            if (selectIndex < 0 || !isFunction(tasks[selectIndex].args[0])) {
                return
            }
            var nextTask = tasks[1 + selectIndex];
            if (!nextTask || "slice" !== nextTask.name) {
                return
            }
            tasks[1 + selectIndex] = tasks[selectIndex];
            tasks[selectIndex] = nextTask
        },
        exec: url => sendRequest(_oDataVersion, {
            url: url,
            params: extend(requestData(), null === queryOptions || void 0 === queryOptions ? void 0 : queryOptions.params)
        }, {
            beforeSend: queryOptions.beforeSend,
            jsonp: queryOptions.jsonp,
            withCredentials: queryOptions.withCredentials,
            countOnly: _countQuery,
            deserializeDates: queryOptions.deserializeDates,
            fieldTypes: queryOptions.fieldTypes,
            isPaged: isFinite(_take)
        }),
        multiSort(args) {
            var rules;
            if (hasSlice()) {
                return false
            }
            for (var i = 0; i < args.length; i++) {
                var getter = args[i][0];
                var desc = !!args[i][1];
                var rule = void 0;
                if ("string" !== typeof getter) {
                    return false
                }
                rule = serializePropName(getter);
                if (desc) {
                    rule += " desc"
                }
                rules = rules || [];
                rules.push(rule)
            }
            _sorting = rules
        },
        slice(skipCount, takeCount) {
            if (hasSlice()) {
                return false
            }
            _skip = skipCount;
            _take = takeCount
        },
        filter(criterion) {
            if (hasSlice()) {
                return false
            }
            if (!Array.isArray(criterion)) {
                criterion = [].slice.call(arguments)
            }
            if (hasFunction(criterion)) {
                return false
            }
            if (_criteria.length) {
                _criteria.push("and")
            }
            _criteria.push(criterion)
        },
        select(expr) {
            if (_select || isFunction(expr)) {
                return false
            }
            if (!Array.isArray(expr)) {
                expr = [].slice.call(arguments)
            }
            _select = expr
        },
        count: () => _countQuery = true
    }
};
queryAdapters.odata = createODataQueryAdapter;
export var odata = createODataQueryAdapter;
