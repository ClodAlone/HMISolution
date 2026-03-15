/**
 * DevExtreme (esm/renovation/utils/type_conversion.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export function toNumber(attribute) {
    return attribute ? Number(attribute.replace("px", "")) : 0
}
