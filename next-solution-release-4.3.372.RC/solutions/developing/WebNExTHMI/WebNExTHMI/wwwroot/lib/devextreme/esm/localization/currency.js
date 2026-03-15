/**
 * DevExtreme (esm/localization/currency.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../core/utils/extend";
export default {
    _formatNumberCore: function(value, format, formatConfig) {
        if ("currency" === format) {
            formatConfig.precision = formatConfig.precision || 0;
            var result = this.format(value, extend({}, formatConfig, {
                type: "fixedpoint"
            }));
            var currencyPart = this.getCurrencySymbol().symbol.replace(/\$/g, "$$$$");
            result = result.replace(/^(\D*)(\d.*)/, "$1" + currencyPart + "$2");
            return result
        }
        return this.callBase.apply(this, arguments)
    },
    getCurrencySymbol: function() {
        return {
            symbol: "$"
        }
    },
    getOpenXmlCurrencyFormat: function() {
        return "$#,##0{0}_);\\($#,##0{0}\\)"
    }
};
