/**
 * DevExtreme (cjs/localization/parentLocale.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var PARENT_LOCALE_SEPARATOR = "-";
var _default = function(parentLocales, locale) {
    var parentLocale = parentLocales[locale];
    if (parentLocale) {
        return "root" !== parentLocale && parentLocale
    }
    return locale.substr(0, locale.lastIndexOf(PARENT_LOCALE_SEPARATOR))
};
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
