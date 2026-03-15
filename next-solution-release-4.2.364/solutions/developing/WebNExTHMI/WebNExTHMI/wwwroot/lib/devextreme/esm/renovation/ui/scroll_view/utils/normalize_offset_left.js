/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/normalize_offset_left.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export function normalizeOffsetLeft(scrollLeft, maxLeftOffset, rtlEnabled) {
    if (rtlEnabled) {
        return maxLeftOffset + scrollLeft
    }
    return scrollLeft
}
