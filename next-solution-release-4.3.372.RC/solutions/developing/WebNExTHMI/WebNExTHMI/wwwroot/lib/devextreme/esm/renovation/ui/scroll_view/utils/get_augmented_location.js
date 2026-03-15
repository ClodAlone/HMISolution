/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_augmented_location.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isNumeric
} from "../../../../core/utils/type";
export function getAugmentedLocation(location) {
    if (isNumeric(location)) {
        return {
            left: location,
            top: location
        }
    }
    return _extends({
        top: 0,
        left: 0
    }, location)
}
