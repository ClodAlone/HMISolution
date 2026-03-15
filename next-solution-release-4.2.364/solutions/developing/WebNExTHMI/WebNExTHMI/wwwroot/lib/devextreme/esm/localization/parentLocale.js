/**
 * DevExtreme (esm/localization/parentLocale.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var PARENT_LOCALE_SEPARATOR = "-";
export default (parentLocales, locale) => {
    var parentLocale = parentLocales[locale];
    if (parentLocale) {
        return "root" !== parentLocale && parentLocale
    }
    return locale.substr(0, locale.lastIndexOf(PARENT_LOCALE_SEPARATOR))
};
