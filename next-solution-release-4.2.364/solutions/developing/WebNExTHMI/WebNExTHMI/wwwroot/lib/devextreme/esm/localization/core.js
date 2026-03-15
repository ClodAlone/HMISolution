/**
 * DevExtreme (esm/localization/core.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dependencyInjector from "../core/utils/dependency_injector";
import parentLocales from "./cldr-data/parent_locales";
import getParentLocale from "./parentLocale";
var DEFAULT_LOCALE = "en";
export default dependencyInjector({
    locale: (() => {
        var currentLocale = DEFAULT_LOCALE;
        return locale => {
            if (!locale) {
                return currentLocale
            }
            currentLocale = locale
        }
    })(),
    getValueByClosestLocale: function(getter) {
        var locale = this.locale();
        var value = getter(locale);
        var isRootLocale;
        while (!value && !isRootLocale) {
            locale = getParentLocale(parentLocales, locale);
            if (locale) {
                value = getter(locale)
            } else {
                isRootLocale = true
            }
        }
        if (void 0 === value && locale !== DEFAULT_LOCALE) {
            return getter(DEFAULT_LOCALE)
        }
        return value
    }
});
