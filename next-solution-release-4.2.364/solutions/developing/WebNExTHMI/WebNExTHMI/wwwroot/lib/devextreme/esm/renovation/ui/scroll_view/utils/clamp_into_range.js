/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/clamp_into_range.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export function clampIntoRange(value, max, min) {
    return Math.max(Math.min(value, max), min)
}
