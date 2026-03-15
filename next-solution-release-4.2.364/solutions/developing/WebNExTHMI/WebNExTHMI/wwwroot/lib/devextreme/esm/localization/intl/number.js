/**
 * DevExtreme (esm/localization/intl/number.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dxConfig from "../../core/config";
import localizationCoreUtils from "../core";
import openXmlCurrencyFormat from "../open_xml_currency_format";
import accountingFormats from "../cldr-data/accounting_formats";
var detectCurrencySymbolRegex = /([^\s0]+)?(\s*)0*[.,]*0*(\s*)([^\s0]+)?/;
var formattersCache = {};
var getFormatter = format => {
    var key = localizationCoreUtils.locale() + "/" + JSON.stringify(format);
    if (!formattersCache[key]) {
        formattersCache[key] = new Intl.NumberFormat(localizationCoreUtils.locale(), format).format
    }
    return formattersCache[key]
};
var getCurrencyFormatter = currency => new Intl.NumberFormat(localizationCoreUtils.locale(), {
    style: "currency",
    currency: currency
});
export default {
    engine: function() {
        return "intl"
    },
    _formatNumberCore: function(value, format, formatConfig) {
        if ("exponential" === format) {
            return this.callBase.apply(this, arguments)
        }
        return getFormatter(this._normalizeFormatConfig(format, formatConfig, value))(value)
    },
    _normalizeFormatConfig: function(format, formatConfig, value) {
        var config;
        if ("decimal" === format) {
            config = {
                minimumIntegerDigits: formatConfig.precision || void 0,
                useGrouping: false,
                maximumFractionDigits: String(value).length,
                round: value < 0 ? "ceil" : "floor"
            }
        } else {
            config = this._getPrecisionConfig(formatConfig.precision)
        }
        if ("percent" === format) {
            config.style = "percent"
        } else if ("currency" === format) {
            config.style = "currency";
            config.currency = formatConfig.currency || dxConfig().defaultCurrency
        }
        return config
    },
    _getPrecisionConfig: function(precision) {
        var config;
        if (null === precision) {
            config = {
                minimumFractionDigits: 0,
                maximumFractionDigits: 20
            }
        } else {
            config = {
                minimumFractionDigits: precision || 0,
                maximumFractionDigits: precision || 0
            }
        }
        return config
    },
    format: function(value, _format) {
        if ("number" !== typeof value) {
            return value
        }
        _format = this._normalizeFormat(_format);
        if ("default" === _format.currency) {
            _format.currency = dxConfig().defaultCurrency
        }
        if (!_format || "function" !== typeof _format && !_format.type && !_format.formatter) {
            return getFormatter(_format)(value)
        }
        return this.callBase.apply(this, arguments)
    },
    _getCurrencySymbolInfo: function(currency) {
        var formatter = getCurrencyFormatter(currency);
        return this._extractCurrencySymbolInfo(formatter.format(0))
    },
    _extractCurrencySymbolInfo: function(currencyValueString) {
        var match = detectCurrencySymbolRegex.exec(currencyValueString) || [];
        var position = match[1] ? "before" : "after";
        var symbol = match[1] || match[4] || "";
        var delimiter = match[2] || match[3] || "";
        return {
            position: position,
            symbol: symbol,
            delimiter: delimiter
        }
    },
    getCurrencySymbol: function(currency) {
        if (!currency) {
            currency = dxConfig().defaultCurrency
        }
        var symbolInfo = this._getCurrencySymbolInfo(currency);
        return {
            symbol: symbolInfo.symbol
        }
    },
    getOpenXmlCurrencyFormat: function(currency) {
        var targetCurrency = currency || dxConfig().defaultCurrency;
        var currencySymbol = this._getCurrencySymbolInfo(targetCurrency).symbol;
        var closestAccountingFormat = localizationCoreUtils.getValueByClosestLocale(locale => accountingFormats[locale]);
        return openXmlCurrencyFormat(currencySymbol, closestAccountingFormat)
    }
};
