/**
 * DevExtreme (esm/localization/message.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import dependencyInjector from "../core/utils/dependency_injector";
import {
    extend
} from "../core/utils/extend";
import {
    each
} from "../core/utils/iterator";
import {
    format as stringFormat
} from "../core/utils/string";
import {
    humanize
} from "../core/utils/inflector";
import coreLocalization from "./core";
import {
    defaultMessages
} from "./default_messages";
var baseDictionary = extend(true, {}, defaultMessages);
var getDataByLocale = (localeData, locale) => localeData[locale] || {};
var newMessages = {};
var messageLocalization = dependencyInjector({
    engine: function() {
        return "base"
    },
    _dictionary: baseDictionary,
    load: function(messages) {
        extend(true, this._dictionary, messages)
    },
    _localizablePrefix: "@",
    setup: function(localizablePrefix) {
        this._localizablePrefix = localizablePrefix
    },
    localizeString: function(text) {
        var that = this;
        var regex = new RegExp("(^|[^a-zA-Z_0-9" + that._localizablePrefix + "-]+)(" + that._localizablePrefix + "{1,2})([a-zA-Z_0-9-]+)", "g");
        var escapeString = that._localizablePrefix + that._localizablePrefix;
        return text.replace(regex, (str, prefix, escape, localizationKey) => {
            var defaultResult = that._localizablePrefix + localizationKey;
            var result;
            if (escape !== escapeString) {
                result = that.format(localizationKey)
            }
            if (!result) {
                newMessages[localizationKey] = humanize(localizationKey)
            }
            return prefix + (result || defaultResult)
        })
    },
    localizeNode: function(node) {
        var that = this;
        $(node).each((index, nodeItem) => {
            if (!nodeItem.nodeType) {
                return
            }
            if (3 === nodeItem.nodeType) {
                nodeItem.nodeValue = that.localizeString(nodeItem.nodeValue)
            } else if (!$(nodeItem).is("iframe")) {
                each(nodeItem.attributes || [], (index, attr) => {
                    if ("string" === typeof attr.value) {
                        var localizedValue = that.localizeString(attr.value);
                        if (attr.value !== localizedValue) {
                            attr.value = localizedValue
                        }
                    }
                });
                $(nodeItem).contents().each((index, node) => {
                    that.localizeNode(node)
                })
            }
        })
    },
    getMessagesByLocales: function() {
        return this._dictionary
    },
    getDictionary: function(onlyNew) {
        if (onlyNew) {
            return newMessages
        }
        return extend({}, newMessages, this.getMessagesByLocales()[coreLocalization.locale()])
    },
    getFormatter: function(key) {
        return this._getFormatterBase(key) || this._getFormatterBase(key, "en")
    },
    _getFormatterBase: function(key, locale) {
        var message = coreLocalization.getValueByClosestLocale(locale => getDataByLocale(this._dictionary, locale)[key]);
        if (message) {
            return function() {
                var args = 1 === arguments.length && Array.isArray(arguments[0]) ? arguments[0].slice(0) : Array.prototype.slice.call(arguments, 0);
                args.unshift(message);
                return stringFormat.apply(this, args)
            }
        }
    },
    format: function(key) {
        var formatter = this.getFormatter(key);
        var values = Array.prototype.slice.call(arguments, 1);
        return formatter && formatter.apply(this, values) || ""
    }
});
export default messageLocalization;
