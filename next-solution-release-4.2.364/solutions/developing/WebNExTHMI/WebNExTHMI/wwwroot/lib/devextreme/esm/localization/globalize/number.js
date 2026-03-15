/**
 * DevExtreme (esm/localization/globalize/number.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import "./core";
import Globalize from "globalize";
import numberLocalization from "../number";
import errors from "../../core/errors";
import "globalize/number";
if (Globalize && Globalize.formatNumber) {
    var enNumbers = {
        main: {
            en: {
                identity: {
                    version: {
                        _cldrVersion: "28",
                        _number: "$Revision: 11972 $"
                    },
                    language: "en"
                },
                numbers: {
                    defaultNumberingSystem: "latn",
                    otherNumberingSystems: {
                        native: "latn"
                    },
                    minimumGroupingDigits: "1",
                    "symbols-numberSystem-latn": {
                        decimal: ".",
                        group: ",",
                        list: ";",
                        percentSign: "%",
                        plusSign: "+",
                        minusSign: "-",
                        exponential: "E",
                        superscriptingExponent: "\xd7",
                        perMille: "\u2030",
                        infinity: "\u221e",
                        nan: "NaN",
                        timeSeparator: ":"
                    },
                    "decimalFormats-numberSystem-latn": {
                        standard: "#,##0.###",
                        long: {
                            decimalFormat: {
                                "1000-count-one": "0 thousand",
                                "1000-count-other": "0 thousand",
                                "10000-count-one": "00 thousand",
                                "10000-count-other": "00 thousand",
                                "100000-count-one": "000 thousand",
                                "100000-count-other": "000 thousand",
                                "1000000-count-one": "0 million",
                                "1000000-count-other": "0 million",
                                "10000000-count-one": "00 million",
                                "10000000-count-other": "00 million",
                                "100000000-count-one": "000 million",
                                "100000000-count-other": "000 million",
                                "1000000000-count-one": "0 billion",
                                "1000000000-count-other": "0 billion",
                                "10000000000-count-one": "00 billion",
                                "10000000000-count-other": "00 billion",
                                "100000000000-count-one": "000 billion",
                                "100000000000-count-other": "000 billion",
                                "1000000000000-count-one": "0 trillion",
                                "1000000000000-count-other": "0 trillion",
                                "10000000000000-count-one": "00 trillion",
                                "10000000000000-count-other": "00 trillion",
                                "100000000000000-count-one": "000 trillion",
                                "100000000000000-count-other": "000 trillion"
                            }
                        },
                        short: {
                            decimalFormat: {
                                "1000-count-one": "0K",
                                "1000-count-other": "0K",
                                "10000-count-one": "00K",
                                "10000-count-other": "00K",
                                "100000-count-one": "000K",
                                "100000-count-other": "000K",
                                "1000000-count-one": "0M",
                                "1000000-count-other": "0M",
                                "10000000-count-one": "00M",
                                "10000000-count-other": "00M",
                                "100000000-count-one": "000M",
                                "100000000-count-other": "000M",
                                "1000000000-count-one": "0B",
                                "1000000000-count-other": "0B",
                                "10000000000-count-one": "00B",
                                "10000000000-count-other": "00B",
                                "100000000000-count-one": "000B",
                                "100000000000-count-other": "000B",
                                "1000000000000-count-one": "0T",
                                "1000000000000-count-other": "0T",
                                "10000000000000-count-one": "00T",
                                "10000000000000-count-other": "00T",
                                "100000000000000-count-one": "000T",
                                "100000000000000-count-other": "000T"
                            }
                        }
                    },
                    "scientificFormats-numberSystem-latn": {
                        standard: "#E0"
                    },
                    "percentFormats-numberSystem-latn": {
                        standard: "#,##0%"
                    },
                    "currencyFormats-numberSystem-latn": {
                        currencySpacing: {
                            beforeCurrency: {
                                currencyMatch: "[:^S:]",
                                surroundingMatch: "[:digit:]",
                                insertBetween: "\xa0"
                            },
                            afterCurrency: {
                                currencyMatch: "[:^S:]",
                                surroundingMatch: "[:digit:]",
                                insertBetween: "\xa0"
                            }
                        },
                        standard: "\xa4#,##0.00",
                        accounting: "\xa4#,##0.00;(\xa4#,##0.00)",
                        short: {
                            standard: {
                                "1000-count-one": "\xa40K",
                                "1000-count-other": "\xa40K",
                                "10000-count-one": "\xa400K",
                                "10000-count-other": "\xa400K",
                                "100000-count-one": "\xa4000K",
                                "100000-count-other": "\xa4000K",
                                "1000000-count-one": "\xa40M",
                                "1000000-count-other": "\xa40M",
                                "10000000-count-one": "\xa400M",
                                "10000000-count-other": "\xa400M",
                                "100000000-count-one": "\xa4000M",
                                "100000000-count-other": "\xa4000M",
                                "1000000000-count-one": "\xa40B",
                                "1000000000-count-other": "\xa40B",
                                "10000000000-count-one": "\xa400B",
                                "10000000000-count-other": "\xa400B",
                                "100000000000-count-one": "\xa4000B",
                                "100000000000-count-other": "\xa4000B",
                                "1000000000000-count-one": "\xa40T",
                                "1000000000000-count-other": "\xa40T",
                                "10000000000000-count-one": "\xa400T",
                                "10000000000000-count-other": "\xa400T",
                                "100000000000000-count-one": "\xa4000T",
                                "100000000000000-count-other": "\xa4000T"
                            }
                        },
                        "unitPattern-count-one": "{0} {1}",
                        "unitPattern-count-other": "{0} {1}"
                    },
                    "miscPatterns-numberSystem-latn": {
                        atLeast: "{0}+",
                        range: "{0}\u2013{1}"
                    }
                }
            }
        }
    };
    if ("en" === Globalize.locale().locale) {
        Globalize.load(enNumbers);
        Globalize.locale("en")
    }
    var formattersCache = {};
    var getFormatter = format => {
        var formatter;
        var formatCacheKey;
        if ("object" === typeof format) {
            formatCacheKey = Globalize.locale().locale + ":" + JSON.stringify(format)
        } else {
            formatCacheKey = Globalize.locale().locale + ":" + format
        }
        formatter = formattersCache[formatCacheKey];
        if (!formatter) {
            formatter = formattersCache[formatCacheKey] = Globalize.numberFormatter(format)
        }
        return formatter
    };
    var globalizeNumberLocalization = {
        engine: function() {
            return "globalize"
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
                    minimumIntegerDigits: formatConfig.precision || 1,
                    useGrouping: false,
                    minimumFractionDigits: 0,
                    maximumFractionDigits: 20,
                    round: value < 0 ? "ceil" : "floor"
                }
            } else {
                config = this._getPrecisionConfig(formatConfig.precision)
            }
            if ("percent" === format) {
                config.style = "percent"
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
            if (!_format || "function" !== typeof _format && !_format.type && !_format.formatter) {
                return getFormatter(_format)(value)
            }
            return this.callBase.apply(this, arguments)
        },
        parse: function(text, format) {
            if (!text) {
                return
            }
            if (format && (format.parser || "string" === typeof format)) {
                return this.callBase.apply(this, arguments)
            }
            if (format) {
                errors.log("W0011")
            }
            var result = Globalize.parseNumber(text);
            if (isNaN(result)) {
                result = this.callBase.apply(this, arguments)
            }
            return result
        }
    };
    numberLocalization.resetInjection();
    numberLocalization.inject(globalizeNumberLocalization)
}
