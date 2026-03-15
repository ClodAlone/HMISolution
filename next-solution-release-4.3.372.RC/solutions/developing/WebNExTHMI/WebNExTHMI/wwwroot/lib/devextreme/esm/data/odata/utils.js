/**
 * DevExtreme (esm/data/odata/utils.js)
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
    isDefined,
    isPlainObject,
    type,
    isObject
} from "../../core/utils/type";
import {
    each,
    map
} from "../../core/utils/iterator";
import ajax from "../../core/utils/ajax";
import _Guid from "../../core/guid";
import {
    grep
} from "../../core/utils/common";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    errors
} from "../errors";
import {
    errorMessageFromXhr,
    XHR_ERROR_UNLOAD
} from "../utils";
import {
    format as stringFormat
} from "../../core/utils/string";
var GUID_REGEX = /^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$/;
var VERBOSE_DATE_REGEX = /^\/Date\((-?\d+)((\+|-)?(\d+)?)\)\/$/;
var ISO8601_DATE_REGEX = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(Z|[-+]{1}\d{2}(:?)(\d{2})?)?$/;
var JSON_VERBOSE_MIME_TYPE = "application/json;odata=verbose";
var makeArray = value => "string" === type(value) ? value.split() : value;
var hasDot = x => /\./.test(x);
var pad = (text, length, right) => {
    text = String(text);
    while (text.length < length) {
        text = right ? "".concat(text, "0") : "0".concat(text)
    }
    return text
};
var formatISO8601 = (date, skipZeroTime, skipTimezone) => {
    var bag = [];
    var padLeft2 = text => pad(text, 2);
    bag.push(date.getFullYear());
    bag.push("-");
    bag.push(padLeft2(date.getMonth() + 1));
    bag.push("-");
    bag.push(padLeft2(date.getDate()));
    if (!(skipZeroTime && date.getHours() + date.getMinutes() + date.getSeconds() + date.getMilliseconds() < 1)) {
        bag.push("T");
        bag.push(padLeft2(date.getHours()));
        bag.push(":");
        bag.push(padLeft2(date.getMinutes()));
        bag.push(":");
        bag.push(padLeft2(date.getSeconds()));
        if (date.getMilliseconds()) {
            bag.push(".");
            bag.push(pad(date.getMilliseconds(), 3))
        }
        if (!skipTimezone) {
            bag.push("Z")
        }
    }
    return bag.join("")
};
var parseISO8601 = isoString => {
    var result = new Date(60 * new Date(0).getTimezoneOffset() * 1e3);
    var chunks = isoString.replace("Z", "").split("T");
    var date = /(\d{4})-(\d{2})-(\d{2})/.exec(chunks[0]);
    var time = /(\d{2}):(\d{2}):(\d{2})\.?(\d{0,7})?/.exec(chunks[1]);
    result.setFullYear(Number(date[1]));
    result.setMonth(Number(date[2]) - 1);
    result.setDate(Number(date[3]));
    if (Array.isArray(time) && time.length) {
        result.setHours(Number(time[1]));
        result.setMinutes(Number(time[2]));
        result.setSeconds(Number(time[3]));
        var fractional = (time[4] || "").slice(0, 3);
        fractional = pad(fractional, 3, true);
        result.setMilliseconds(Number(fractional))
    }
    return result
};
var isAbsoluteUrl = url => /^(?:[a-z]+:)?\/\//i.test(url);
var stripParams = url => {
    var index = url.indexOf("?");
    if (index > -1) {
        return url.substr(0, index)
    }
    return url
};
var toAbsoluteUrl = (basePath, relativePath) => {
    var part;
    var baseParts = stripParams(basePath).split("/");
    var relativeParts = relativePath.split("/");
    baseParts.pop();
    while (relativeParts.length) {
        part = relativeParts.shift();
        if (".." === part) {
            baseParts.pop()
        } else {
            baseParts.push(part)
        }
    }
    return baseParts.join("/")
};
var param = params => {
    var result = [];
    for (var name in params) {
        result.push(name + "=" + params[name])
    }
    return result.join("&")
};
var ajaxOptionsForRequest = function(protocolVersion, request) {
    var _options$beforeSend;
    var options = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
    var formatPayload = payload => JSON.stringify(payload, (function(key, value) {
        if (!(this[key] instanceof Date)) {
            return value
        }
        value = formatISO8601(this[key]);
        switch (protocolVersion) {
            case 2:
                return value.substr(0, value.length - 1);
            case 3:
            case 4:
                return value;
            default:
                throw errors.Error("E4002")
        }
    }));
    request = extend({
        async: true,
        method: "get",
        url: "",
        params: {},
        payload: null,
        headers: {},
        timeout: 3e4
    }, request);
    null === (_options$beforeSend = options.beforeSend) || void 0 === _options$beforeSend ? void 0 : _options$beforeSend.call(options, request);
    var {
        async: async,
        timeout: timeout,
        headers: headers
    } = request;
    var {
        url: url,
        method: method
    } = request;
    var {
        jsonp: jsonp,
        withCredentials: withCredentials
    } = options;
    method = (method || "get").toLowerCase();
    var isGet = "get" === method;
    var useJsonp = isGet && jsonp;
    var params = extend({}, request.params);
    var ajaxData = isGet ? params : formatPayload(request.payload);
    var qs = !isGet && param(params);
    var contentType = !isGet && JSON_VERBOSE_MIME_TYPE;
    if (qs) {
        url += (url.indexOf("?") > -1 ? "&" : "?") + qs
    }
    if (useJsonp) {
        ajaxData.$format = "json"
    }
    return {
        url: url,
        data: ajaxData,
        dataType: useJsonp ? "jsonp" : "json",
        jsonp: useJsonp && "$callback",
        method: method,
        async: async,
        timeout: timeout,
        headers: headers,
        contentType: contentType,
        accepts: {
            json: [JSON_VERBOSE_MIME_TYPE, "text/plain"].join()
        },
        xhrFields: {
            withCredentials: withCredentials
        }
    }
};
export var sendRequest = (protocolVersion, request, options) => {
    var {
        deserializeDates: deserializeDates,
        fieldTypes: fieldTypes,
        countOnly: countOnly,
        isPaged: isPaged
    } = options;
    var d = new Deferred;
    var ajaxOptions = ajaxOptionsForRequest(protocolVersion, request, options);
    ajax.sendRequest(ajaxOptions).always((obj, textStatus) => {
        var transformOptions = {
            deserializeDates: deserializeDates,
            fieldTypes: fieldTypes
        };
        var tuple = interpretJsonFormat(obj, textStatus, transformOptions, ajaxOptions);
        var {
            error: error,
            data: data,
            count: count
        } = tuple;
        var {
            nextUrl: nextUrl
        } = tuple;
        if (error) {
            if (error.message !== XHR_ERROR_UNLOAD) {
                d.reject(error)
            }
        } else if (countOnly) {
            if (isFinite(count)) {
                d.resolve(count)
            } else {
                d.reject(new errors.Error("E4018"))
            }
        } else if (nextUrl && !isPaged) {
            if (!isAbsoluteUrl(nextUrl)) {
                nextUrl = toAbsoluteUrl(ajaxOptions.url, nextUrl)
            }
            sendRequest(protocolVersion, {
                url: nextUrl
            }, options).fail(d.reject).done(nextData => d.resolve(data.concat(nextData)))
        } else {
            var extra = isFinite(count) ? {
                totalCount: count
            } : void 0;
            d.resolve(data, extra)
        }
    });
    return d.promise()
};
var formatDotNetError = errorObj => {
    var message;
    var currentError = errorObj;
    if ("message" in errorObj) {
        var _errorObj$message;
        message = (null === (_errorObj$message = errorObj.message) || void 0 === _errorObj$message ? void 0 : _errorObj$message.value) || errorObj.message
    }
    while (currentError = currentError.innererror || currentError.internalexception) {
        message = currentError.message;
        if (currentError.internalexception && -1 === message.indexOf("inner exception")) {
            break
        }
    }
    return message
};
var errorFromResponse = (obj, textStatus, ajaxOptions) => {
    var _response, _response2, _response3, _response4;
    if ("nocontent" === textStatus) {
        return null
    }
    var message = "Unknown error";
    var response = obj;
    var httpStatus = 200;
    var errorData = {
        requestOptions: ajaxOptions
    };
    if ("success" !== textStatus) {
        var {
            status: status,
            responseText: responseText
        } = obj;
        httpStatus = status;
        message = errorMessageFromXhr(obj, textStatus);
        try {
            response = JSON.parse(responseText)
        } catch (x) {}
    }
    var errorObj = (null === (_response = response) || void 0 === _response ? void 0 : _response.then) || (null === (_response2 = response) || void 0 === _response2 ? void 0 : _response2.error) || (null === (_response3 = response) || void 0 === _response3 ? void 0 : _response3["odata.error"]) || (null === (_response4 = response) || void 0 === _response4 ? void 0 : _response4["@odata.error"]);
    if (errorObj) {
        message = formatDotNetError(errorObj) || message;
        errorData.errorDetails = errorObj;
        if (200 === httpStatus) {
            httpStatus = 500
        }
        var customCode = Number(errorObj.code);
        if (isFinite(customCode) && customCode >= 400) {
            httpStatus = customCode
        }
    }
    if (httpStatus >= 400 || 0 === httpStatus) {
        errorData.httpStatus = httpStatus;
        return extend(Error(message), errorData)
    }
    return null
};
var interpretJsonFormat = (obj, textStatus, transformOptions, ajaxOptions) => {
    var error = errorFromResponse(obj, textStatus, ajaxOptions);
    if (error) {
        return {
            error: error
        }
    }
    if (!isPlainObject(obj)) {
        return {
            data: obj
        }
    }
    var value = "d" in obj && (Array.isArray(obj.d) || isObject(obj.d)) ? interpretVerboseJsonFormat(obj, textStatus) : interpretLightJsonFormat(obj, textStatus);
    transformTypes(value, transformOptions);
    return value
};
var interpretVerboseJsonFormat = _ref => {
    var _data$results;
    var {
        d: data
    } = _ref;
    if (!isDefined(data)) {
        return {
            error: Error("Malformed or unsupported JSON response received")
        }
    }
    return {
        data: null !== (_data$results = data.results) && void 0 !== _data$results ? _data$results : data,
        nextUrl: data.__next,
        count: parseInt(data.__count, 10)
    }
};
var interpretLightJsonFormat = obj => {
    var _obj$value;
    return {
        data: null !== (_obj$value = obj.value) && void 0 !== _obj$value ? _obj$value : obj,
        nextUrl: obj["@odata.nextLink"],
        count: parseInt(obj["@odata.count"], 10)
    }
};
export var EdmLiteral = Class.inherit({
    ctor(value) {
        this._value = value
    },
    valueOf() {
        return this._value
    }
});
var transformTypes = function transformTypes(obj) {
    var options = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
    each(obj, (key, value) => {
        if (null !== value && "object" === typeof value) {
            if ("results" in value) {
                obj[key] = value.results
            }
            transformTypes(obj[key], options)
        } else if ("string" === typeof value) {
            var {
                fieldTypes: fieldTypes,
                deserializeDates: deserializeDates
            } = options;
            var canBeGuid = !fieldTypes || "String" !== fieldTypes[key];
            if (canBeGuid && GUID_REGEX.test(value)) {
                obj[key] = new _Guid(value)
            }
            if (false !== deserializeDates) {
                if (value.match(VERBOSE_DATE_REGEX)) {
                    var date = new Date(Number(RegExp.$1) + 60 * RegExp.$2 * 1e3);
                    obj[key] = new Date(date.valueOf() + 60 * date.getTimezoneOffset() * 1e3)
                } else if (ISO8601_DATE_REGEX.test(value)) {
                    obj[key] = new Date(parseISO8601(obj[key]).valueOf())
                }
            }
        }
    })
};
var serializeDate = date => "datetime'".concat(formatISO8601(date, true, true), "'");
var serializeString = value => "'".concat(value.replace(/'/g, "''"), "'");
export var serializePropName = propName => propName instanceof EdmLiteral ? propName.valueOf() : propName.replace(/\./g, "/");
var serializeValueV4 = value => {
    if (value instanceof Date) {
        return formatISO8601(value, false, false)
    }
    if (value instanceof _Guid) {
        return value.valueOf()
    }
    if (Array.isArray(value)) {
        return "[".concat(value.map(item => serializeValueV4(item)).join(","), "]")
    }
    return serializeValueV2(value)
};
var serializeValueV2 = value => {
    if (value instanceof Date) {
        return serializeDate(value)
    }
    if (value instanceof _Guid) {
        return "guid'".concat(value, "'")
    }
    if (value instanceof EdmLiteral) {
        return value.valueOf()
    }
    if ("string" === typeof value) {
        return serializeString(value)
    }
    return String(value)
};
export var serializeValue = (value, protocolVersion) => {
    switch (protocolVersion) {
        case 2:
        case 3:
            return serializeValueV2(value);
        case 4:
            return serializeValueV4(value);
        default:
            throw errors.Error("E4002")
    }
};
export var serializeKey = (key, protocolVersion) => {
    if (isPlainObject(key)) {
        var parts = [];
        each(key, (k, v) => parts.push("".concat(serializePropName(k), "=").concat(serializeValue(v, protocolVersion))));
        return parts.join()
    }
    return serializeValue(key, protocolVersion)
};
export var keyConverters = {
    String: value => "".concat(value),
    Int32: value => Math.floor(value),
    Int64: value => value instanceof EdmLiteral ? value : new EdmLiteral("".concat(value, "L")),
    Guid: value => value instanceof _Guid ? value : new _Guid(value),
    Boolean: value => !!value,
    Single: value => value instanceof EdmLiteral ? value : new EdmLiteral(value + "f"),
    Decimal: value => value instanceof EdmLiteral ? value : new EdmLiteral(value + "m")
};
export var convertPrimitiveValue = (type, value) => {
    if (null === value) {
        return null
    }
    var converter = keyConverters[type];
    if (!converter) {
        throw errors.Error("E4014", type)
    }
    return converter(value)
};
export var generateSelect = (oDataVersion, select) => {
    if (!select) {
        return
    }
    return oDataVersion < 4 ? serializePropName(select.join()) : grep(select, hasDot, true).join()
};
var formatCore = hash => {
    var result = "";
    var selectValue = [];
    var expandValue = [];
    each(hash, (key, value) => {
        if (Array.isArray(value)) {
            [].push.apply(selectValue, value)
        }
        if (isPlainObject(value)) {
            expandValue.push("".concat(key).concat(formatCore(value)))
        }
    });
    if (selectValue.length || expandValue.length) {
        result += "(";
        if (selectValue.length) {
            result += "$select=".concat(map(selectValue, serializePropName).join())
        }
        if (expandValue.length) {
            if (selectValue.length) {
                result += ";"
            }
            result += "$expand=".concat(map(expandValue, serializePropName).join())
        }
        result += ")"
    }
    return result
};
var format = hash => {
    var result = [];
    each(hash, (key, value) => result.push("".concat(key).concat(formatCore(value))));
    return result.join()
};
var parseCore = (exprParts, root, stepper) => {
    var result = stepper(root, exprParts.shift(), exprParts);
    if (false === result) {
        return
    }
    parseCore(exprParts, result, stepper)
};
var parseTree = (exprs, root, stepper) => each(exprs, (_, x) => parseCore(x.split("."), root, stepper));
var generatorV2 = (expand, select) => {
    var hash = {};
    if (expand) {
        each(makeArray(expand), (function() {
            hash[serializePropName(this)] = 1
        }))
    }
    if (select) {
        each(makeArray(select), (function() {
            var path = this.split(".");
            if (path.length < 2) {
                return
            }
            path.pop();
            hash[serializePropName(path.join("."))] = 1
        }))
    }
    return map(hash, (_, v) => v).join()
};
var generatorV4 = (expand, select) => {
    var hash = {};
    if (expand || select) {
        if (expand) {
            parseTree(makeArray(expand), hash, (node, key, path) => {
                node[key] = node[key] || {};
                return !path.length ? false : node[key]
            })
        }
        if (select) {
            parseTree(grep(makeArray(select), hasDot), hash, (node, key, path) => {
                if (!path.length) {
                    node[key] = node[key] || [];
                    node[key].push(key);
                    return false
                }
                return node[key] = node[key] || {}
            })
        }
        return format(hash)
    }
};
export var generateExpand = (oDataVersion, expand, select) => oDataVersion < 4 ? generatorV2(expand, select) : generatorV4(expand, select);
export var formatFunctionInvocationUrl = (baseUrl, args) => stringFormat("{0}({1})", baseUrl, map(args || {}, (value, key) => stringFormat("{0}={1}", key, value)).join(","));
export var escapeServiceOperationParams = (params, version) => {
    if (!params) {
        return params
    }
    var result = {};
    each(params, (k, v) => {
        result[k] = serializeValue(v, version)
    });
    return result
};
