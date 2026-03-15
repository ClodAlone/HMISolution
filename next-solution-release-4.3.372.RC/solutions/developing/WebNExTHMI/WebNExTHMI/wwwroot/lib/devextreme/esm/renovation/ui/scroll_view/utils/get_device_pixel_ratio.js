/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_device_pixel_ratio.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWindow,
    hasWindow
} from "../../../../core/utils/window";
export function getDevicePixelRatio() {
    return hasWindow() ? getWindow().devicePixelRatio : 1
}
