/**
 * DevExtreme (esm/renovation/ui/pager/utils/calculate_values_fitted_width.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export var oneDigitWidth = 10;
export function calculateValuesFittedWidth(minWidth, values) {
    return minWidth + oneDigitWidth * Math.max(...values).toString().length
}
