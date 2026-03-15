/**
 * DevExtreme (esm/renovation/component_wrapper/utils/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    each
} from "../../../core/utils/iterator";
export var removeDifferentElements = ($children, $newChildren) => {
    each($newChildren, (__, element) => {
        var hasComponent = false;
        each($children, (_, oldElement) => {
            if (element === oldElement) {
                hasComponent = true
            }
        });
        if (!hasComponent && element.parentNode) {
            element.parentNode.removeChild(element)
        }
    })
};
