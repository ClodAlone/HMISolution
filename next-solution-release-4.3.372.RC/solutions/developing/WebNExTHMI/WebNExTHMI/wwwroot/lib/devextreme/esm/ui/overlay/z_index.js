/**
 * DevExtreme (esm/ui/overlay/z_index.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    ensureDefined
} from "../../core/utils/common";
var baseZIndex = 1500;
var zIndexStack = [];
export var base = ZIndex => {
    baseZIndex = ensureDefined(ZIndex, baseZIndex);
    return baseZIndex
};
export var create = function() {
    var baseIndex = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : baseZIndex;
    var length = zIndexStack.length;
    var index = (length ? zIndexStack[length - 1] : baseIndex) + 1;
    zIndexStack.push(index);
    return index
};
export var remove = zIndex => {
    var position = zIndexStack.indexOf(zIndex);
    if (position >= 0) {
        zIndexStack.splice(position, 1)
    }
};
export var clearStack = () => {
    zIndexStack = []
};
